using System.Globalization;
using System.Text;

namespace ImageConverter;

internal sealed class AppSettingsStore
{
    private string _filePath;

    public AppSettingsStore()
    {
        _filePath = GetWritableSettingsFilePath();
        TryMigrateLegacySettingsFromInstallFolder();
    }

    private static string GetWritableSettingsFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "Image Converter");
        return Path.Combine(folder, "config.ini");
    }

    private void TryMigrateLegacySettingsFromInstallFolder()
    {
        if (File.Exists(_filePath))
        {
            return;
        }

        var legacy = Path.Combine(AppContext.BaseDirectory, "config.ini");
        if (!File.Exists(legacy))
        {
            return;
        }

        try
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.Copy(legacy, _filePath, overwrite: false);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public AppSettings Load()
    {
        var s = new AppSettings();
        if (File.Exists(_filePath))
        {
            try
            {
                LoadFromIniFile(s);
            }
            catch (IOException)
            {
            }
        }

        ApplyDefaultLastFolderIfNeeded(s);
        return s;
    }

    private void LoadFromIniFile(AppSettings s)
    {
        foreach (var line in File.ReadAllLines(_filePath))
        {
            var t = line.Trim();
            if (t.Length == 0 || t.StartsWith('#') || t.StartsWith(';'))
            {
                continue;
            }

            var eq = t.IndexOf('=');
            if (eq <= 0)
            {
                continue;
            }

            var key = t[..eq].Trim();
            var value = t[(eq + 1)..].Trim();

            switch (key)
            {
                case "LastFolder":
                case "LastSourceFolder":
                    s.LastFolder = value;
                    break;
                case "LastDestinationFolder":
                    if (string.IsNullOrWhiteSpace(s.LastFolder))
                    {
                        s.LastFolder = value;
                    }

                    break;
                case "PreviewThumbnailSizeIndex":
                    if (int.TryParse(value, out var psi))
                    {
                        s.PreviewThumbnailSizeIndex = psi;
                    }

                    break;
                case "MainWindowPlacementSaved":
                    s.MainWindowPlacementSaved = ParseBool(value);
                    break;
                case "MainWindowLeft":
                    if (int.TryParse(value, out var l))
                    {
                        s.MainWindowLeft = l;
                    }

                    break;
                case "MainWindowTop":
                    if (int.TryParse(value, out var tp))
                    {
                        s.MainWindowTop = tp;
                    }

                    break;
                case "MainWindowWidth":
                    if (int.TryParse(value, out var w))
                    {
                        s.MainWindowWidth = w;
                    }

                    break;
                case "MainWindowHeight":
                    if (int.TryParse(value, out var h))
                    {
                        s.MainWindowHeight = h;
                    }

                    break;
                case "MainWindowMaximized":
                    s.MainWindowMaximized = ParseBool(value);
                    break;
                case "PreviewSplitterDistance":
                    if (int.TryParse(value, out var sd))
                    {
                        s.PreviewSplitterDistance = sd;
                    }

                    break;
                case "IcoOutputSizeIndex":
                    if (int.TryParse(value, out var icoIdx))
                    {
                        s.IcoOutputSizeIndex = icoIdx;
                    }

                    break;
                case "SolidColorIndex":
                    if (int.TryParse(value, out var scIdx))
                    {
                        s.SolidColorIndex = scIdx;
                    }

                    break;
            }
        }
    }

    private static void ApplyDefaultLastFolderIfNeeded(AppSettings settings)
    {
        if (IsUsableImageFolder(settings.LastFolder))
        {
            return;
        }

        settings.LastFolder = GetDefaultPicturesFolder();
    }

    private static bool IsUsableImageFolder(string? path) =>
        !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);

    private static string GetDefaultPicturesFolder()
    {
        var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        if (IsUsableImageFolder(myPictures))
        {
            return myPictures;
        }

        var userPictures = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Pictures");
        if (IsUsableImageFolder(userPictures))
        {
            return userPictures;
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    private static bool ParseBool(string value)
    {
        return value.Equals("true", StringComparison.OrdinalIgnoreCase)
               || value.Equals("1", StringComparison.OrdinalIgnoreCase)
               || value.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }

    public void Save(AppSettings settings)
    {
        if (IsPathUnderInstallDirectory(_filePath))
        {
            _filePath = GetWritableSettingsFilePath();
        }

        var sb = new StringBuilder();
        sb.AppendLine($"LastFolder={Escape(settings.LastFolder)}");
        sb.AppendLine($"PreviewThumbnailSizeIndex={settings.PreviewThumbnailSizeIndex}");
        sb.AppendLine($"MainWindowPlacementSaved={(settings.MainWindowPlacementSaved ? "true" : "false")}");
        sb.AppendLine($"MainWindowLeft={settings.MainWindowLeft.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"MainWindowTop={settings.MainWindowTop.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"MainWindowWidth={settings.MainWindowWidth.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"MainWindowHeight={settings.MainWindowHeight.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"MainWindowMaximized={(settings.MainWindowMaximized ? "true" : "false")}");
        sb.AppendLine($"PreviewSplitterDistance={settings.PreviewSplitterDistance.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"IcoOutputSizeIndex={settings.IcoOutputSizeIndex.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"SolidColorIndex={settings.SolidColorIndex.ToString(CultureInfo.InvariantCulture)}");
        var content = sb.ToString();

        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        try
        {
            var temp = _filePath + ".tmp";
            File.WriteAllText(temp, content);
            if (File.Exists(_filePath))
            {
                File.Replace(temp, _filePath, null);
            }
            else
            {
                File.Move(temp, _filePath);
            }
        }
        catch (IOException)
        {
            TryDeleteFile(_filePath + ".tmp");
        }
        catch (UnauthorizedAccessException)
        {
            TryDeleteFile(_filePath + ".tmp");
        }
    }

    private static bool IsPathUnderInstallDirectory(string filePath)
    {
        try
        {
            var installDir = Path.GetFullPath(
                AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar);
            var settingsDir = Path.GetFullPath(Path.GetDirectoryName(filePath) ?? installDir);
            return settingsDir.StartsWith(installDir, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
        }
    }

    private static string Escape(string? value) => value ?? string.Empty;
}
