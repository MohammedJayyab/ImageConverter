using System.Globalization;
using System.Text;

namespace ImageConverter;

/// <summary>Minimal INI-style key=value store for <c>config.ini</c> (no extra NuGet packages).</summary>
internal sealed class AppSettingsStore
{
    private readonly string _filePath;

    public AppSettingsStore()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "config.ini");
    }

    public AppSettings Load()
    {
        var s = new AppSettings();
        if (!File.Exists(_filePath))
        {
            return s;
        }

        try
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
                    case "LastSourceFolder":
                        s.LastSourceFolder = value;
                        break;
                    case "LastDestinationFolder":
                        s.LastDestinationFolder = value;
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
        catch
        {
            // Keep defaults on malformed file.
        }

        return s;
    }

    private static bool ParseBool(string value)
    {
        return value.Equals("true", StringComparison.OrdinalIgnoreCase)
               || value.Equals("1", StringComparison.OrdinalIgnoreCase)
               || value.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }

    public void Save(AppSettings settings)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Image Converter — auto-generated");
        sb.AppendLine($"LastSourceFolder={Escape(settings.LastSourceFolder)}");
        sb.AppendLine($"LastDestinationFolder={Escape(settings.LastDestinationFolder)}");
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

    private static string Escape(string? value)
    {
        return value ?? string.Empty;
    }
}
