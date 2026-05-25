using System.Diagnostics;
using Microsoft.Win32;

namespace ImageConverter;

internal static class ExternalImageEditorLauncher
{
    internal static bool IsPaintDotNetAvailable() => ResolvePaintDotNetExecutable() is not null;

    internal static bool TryOpenWithPaint(string imagePath, out string? errorMessage)
    {
        var exe = ResolvePaintExecutable();
        if (exe is not null)
        {
            return TryStartEditor(exe, imagePath, out errorMessage);
        }

        return TryStartProcess("mspaint.exe", imagePath, out errorMessage, "Microsoft Paint was not found on this PC.");
    }

    internal static bool TryOpenWithPaintDotNet(string imagePath, out string? errorMessage)
    {
        var exe = ResolvePaintDotNetExecutable();
        if (exe is null)
        {
            errorMessage = "Paint.NET was not found. Install it or use the default Program Files location.";
            return false;
        }

        return TryStartEditor(exe, imagePath, out errorMessage);
    }

    private static bool TryStartEditor(string editorExecutable, string imagePath, out string? errorMessage) =>
        TryStartProcess(editorExecutable, imagePath, out errorMessage);

    private static bool TryStartProcess(
        string executable,
        string imagePath,
        out string? errorMessage,
        string? failurePrefix = null)
    {
        errorMessage = null;
        if (!File.Exists(imagePath))
        {
            errorMessage = "The selected file was not found on disk.";
            return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = executable,
                Arguments = $"\"{imagePath}\"",
                UseShellExecute = true
            });
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = failurePrefix is null ? ex.Message : $"{failurePrefix} {ex.Message}";
            return false;
        }
    }

    private static string? ResolvePaintExecutable()
    {
        var fromAppPaths = TryGetRegisteredAppPath("mspaint.exe");
        if (fromAppPaths is not null)
        {
            return fromAppPaths;
        }

        var localAppsAlias = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft",
            "WindowsApps",
            "mspaint.exe");
        if (File.Exists(localAppsAlias))
        {
            return localAppsAlias;
        }

        var windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var system = Environment.GetFolderPath(Environment.SpecialFolder.System);
        foreach (var candidate in new[]
                 {
                     Path.Combine(system, "mspaint.exe"),
                     Path.Combine(windows, "System32", "mspaint.exe"),
                     Path.Combine(windows, "Sysnative", "mspaint.exe")
                 })
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        var storePaint = TryFindWindowsStorePaint();
        if (storePaint is not null)
        {
            return storePaint;
        }

        return TryFindOnPath("mspaint.exe");
    }

    private static string? TryGetRegisteredAppPath(string executableName)
    {
        foreach (var hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
        {
            foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                    using var key = baseKey.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{executableName}");
                    var path = key?.GetValue(null) as string;
                    if (!string.IsNullOrWhiteSpace(path) && File.Exists(path.Trim('"').Trim()))
                    {
                        return path.Trim('"').Trim();
                    }
                }
                catch (IOException)
                {
                    continue;
                }
            }
        }

        return null;
    }

    private static string? TryFindWindowsStorePaint()
    {
        var roots = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WindowsApps"),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft",
                "WindowsApps")
        };

        foreach (var root in roots.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!Directory.Exists(root))
            {
                continue;
            }

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(root, "Microsoft.Paint_*"))
                {
                    foreach (var exeName in new[] { "mspaint.exe", "PaintApp.exe" })
                    {
                        var direct = Path.Combine(dir, exeName);
                        if (File.Exists(direct))
                        {
                            return direct;
                        }

                        var inPaintApp = Path.Combine(dir, "PaintApp", exeName);
                        if (File.Exists(inPaintApp))
                        {
                            return inPaintApp;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
        }

        return null;
    }

    private static string? TryFindOnPath(string executableName)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathEnv))
        {
            return null;
        }

        foreach (var folder in pathEnv.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            try
            {
                var candidate = Path.Combine(folder, executableName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
            catch (IOException)
            {
                continue;
            }
        }

        return null;
    }

    private static string? ResolvePaintDotNetExecutable()
    {
        foreach (var root in GetProgramFilesRoots())
        {
            foreach (var folder in new[] { "paint.net", "Paint.NET" })
            {
                var candidate = Path.Combine(root, folder, "PaintDotNet.exe");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return TryGetPaintDotNetFromRegistry();
    }

    private static IEnumerable<string> GetProgramFilesRoots()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var special in new[]
                 {
                     Environment.SpecialFolder.ProgramFiles,
                     Environment.SpecialFolder.ProgramFilesX86
                 })
        {
            var path = Environment.GetFolderPath(special);
            if (!string.IsNullOrWhiteSpace(path) && seen.Add(path))
            {
                yield return path;
            }
        }

        var envProgramFiles = Environment.GetEnvironmentVariable("ProgramFiles");
        if (!string.IsNullOrWhiteSpace(envProgramFiles) && seen.Add(envProgramFiles))
        {
            yield return envProgramFiles;
        }

        var envProgramFilesX86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
        if (!string.IsNullOrWhiteSpace(envProgramFilesX86) && seen.Add(envProgramFilesX86))
        {
            yield return envProgramFilesX86;
        }
    }

    private static string? TryGetPaintDotNetFromRegistry()
    {
        foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                using var paintNetKey = baseKey.OpenSubKey(@"SOFTWARE\paint.net");
                var path = paintNetKey?.GetValue("PaintDotNetExe") as string;
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    return path;
                }

                using var uninstallKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                if (uninstallKey is null)
                {
                    continue;
                }

                foreach (var subKeyName in uninstallKey.GetSubKeyNames())
                {
                    using var subKey = uninstallKey.OpenSubKey(subKeyName);
                    if (subKey is null)
                    {
                        continue;
                    }

                    var displayName = subKey.GetValue("DisplayName") as string;
                    if (displayName is null || !displayName.Contains("paint.net", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var installLocation = subKey.GetValue("InstallLocation") as string;
                    if (string.IsNullOrWhiteSpace(installLocation))
                    {
                        continue;
                    }

                    var exe = Path.Combine(installLocation.Trim('"'), "PaintDotNet.exe");
                    if (File.Exists(exe))
                    {
                        return exe;
                    }
                }
            }
            catch (IOException)
            {
                continue;
            }
        }

        return null;
    }
}
