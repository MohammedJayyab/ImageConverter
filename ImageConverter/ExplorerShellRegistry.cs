using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;

namespace ImageConverter;

internal static class ExplorerShellRegistry
{
    internal const string ParentShellKeyName = "ImageConverter.ConvertTo";

    private const string ParentMenuLabel = "Convert to";

    private const string AssociationsRoot = @"Software\Classes\SystemFileAssociations";

    private const string CommandStoreRoot =
        @"Software\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell";

    private static readonly string[] FileExtensions =
    [
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".ico", ".svg", ".pdf"
    ];

    private static readonly (string ShellArg, string VerbSuffix, string MenuLabel)[] OutputFormats =
    [
        ("bmp", "Bmp", "BMP (.bmp)"),
        ("gif", "Gif", "GIF (.gif)"),
        ("ico", "Ico", "ICO (.ico)"),
        ("jpg", "Jpg", "JPEG (.jpg)"),
        ("pdf", "Pdf", "PDF (.pdf)"),
        ("png", "Png", "PNG (.png)"),
        ("svg", "Svg", "SVG (.svg)"),
        ("webp", "Webp", "WEBP (.webp)")
    ];

    private static readonly string[] LegacyShellKeyNames =
    [
        "ConverterTo",
        "ImageConverter.ConverterTo",
        "ImageConverter.ConverterTo.Menu",
        "ImageConverter.ConvertTo"
    ];

    internal static void Sync(bool enabled, string exePath)
    {
        if (!enabled)
        {
            UnregisterAll();
            NotifyAssociationChanged();
            return;
        }

        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
        {
            throw new InvalidOperationException("Application executable path is not available.");
        }

        var fullExe = Path.GetFullPath(exePath);
        var iconPath = ResolveIconPath(fullExe);

        UnregisterAll();
        RegisterCommandStoreVerbs(fullExe, iconPath);

        var subCommands = string.Join(";", OutputFormats.Select(f => VerbId(f.VerbSuffix)));

        foreach (var extension in FileExtensions)
        {
            RegisterCascadeParent($@"{AssociationsRoot}\{extension}\shell", subCommands, iconPath);
        }

        NotifyAssociationChanged();
    }

    internal static void UnregisterAll()
    {
        UnregisterCommandStoreVerbs();

        foreach (var extension in FileExtensions)
        {
            var shellPath = $@"{AssociationsRoot}\{extension}\shell";
            DeleteKey($@"{shellPath}\{ParentShellKeyName}");

            foreach (var legacy in LegacyShellKeyNames)
            {
                DeleteKey($@"{shellPath}\{legacy}");
            }

            foreach (var (_, verbSuffix, _) in OutputFormats)
            {
                DeleteKey($@"{shellPath}\ImageConverter.To.{verbSuffix}");
            }

            DeleteLegacyKeysUnderClassesRoot(extension);
        }
    }

    private static string VerbId(string verbSuffix) =>
        $"ImageConverter.ConvertTo.{verbSuffix}";

    private static void RegisterCommandStoreVerbs(string exePath, string? iconPath)
    {
        foreach (var (shellArg, verbSuffix, menuLabel) in OutputFormats)
        {
            var verbId = VerbId(verbSuffix);
            RegisterCommandStoreVerb(Registry.LocalMachine, verbId, menuLabel, shellArg, exePath, iconPath);
            RegisterCommandStoreVerb(Registry.CurrentUser, verbId, menuLabel, shellArg, exePath, iconPath);
        }
    }

    private static void RegisterCommandStoreVerb(
        RegistryKey hive,
        string verbId,
        string menuLabel,
        string shellArg,
        string exePath,
        string? iconPath)
    {
        try
        {
            using var verbKey = hive.CreateSubKey($@"{CommandStoreRoot}\{verbId}", writable: true);
            if (verbKey is null)
            {
                return;
            }

            verbKey.SetValue(null, menuLabel);
            if (!string.IsNullOrEmpty(iconPath))
            {
                verbKey.SetValue("Icon", iconPath);
            }

            using var commandKey = verbKey.CreateSubKey("command", writable: true);
            commandKey?.SetValue(null, $"\"{exePath}\" --shell-convert {shellArg} \"%1\"");
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
    }

    private static void UnregisterCommandStoreVerbs()
    {
        foreach (var (_, verbSuffix, _) in OutputFormats)
        {
            var path = $@"{CommandStoreRoot}\{VerbId(verbSuffix)}";
            TryDelete(Registry.CurrentUser, path);
            TryDelete(Registry.LocalMachine, path);

            TryDelete(Registry.CurrentUser, $@"{CommandStoreRoot}\ImageConverter.ConverterTo.{verbSuffix}");
            TryDelete(Registry.LocalMachine, $@"{CommandStoreRoot}\ImageConverter.ConverterTo.{verbSuffix}");
        }
    }

    /// <summary>
    /// Cascade parent: MUIVerb + SubCommands (no command on parent). Formats live in CommandStore.
    /// </summary>
    private static void RegisterCascadeParent(string shellParentPath, string subCommands, string? iconPath)
    {
        using var parent = Registry.CurrentUser.CreateSubKey(
            $@"{shellParentPath}\{ParentShellKeyName}",
            writable: true);
        if (parent is null)
        {
            return;
        }

        ClearDefaultValue(parent);
        RemoveValueIfPresent(parent, "SubCommands");
        parent.SetValue("MUIVerb", ParentMenuLabel);
        parent.SetValue("SubCommands", subCommands);

        if (!string.IsNullOrEmpty(iconPath))
        {
            parent.SetValue("Icon", iconPath);
        }

        try
        {
            parent.DeleteSubKeyTree("command", throwOnMissingSubKey: false);
            parent.DeleteSubKeyTree("ExtendedSubCommandsKey", throwOnMissingSubKey: false);
            parent.DeleteSubKeyTree("shell", throwOnMissingSubKey: false);
        }
        catch (ArgumentException)
        {
        }
    }

    private static void DeleteLegacyKeysUnderClassesRoot(string extension)
    {
        const string classesRoot = @"Software\Classes";
        DeleteKey($@"{classesRoot}\{extension}\shell\{ParentShellKeyName}");
        foreach (var legacy in LegacyShellKeyNames)
        {
            DeleteKey($@"{classesRoot}\{extension}\shell\{legacy}");
        }

        foreach (var (_, verbSuffix, _) in OutputFormats)
        {
            DeleteKey($@"{classesRoot}\{extension}\shell\ImageConverter.To.{verbSuffix}");
        }
    }

    private static void DeleteKey(string keyPath)
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(keyPath, throwOnMissingSubKey: false);
        }
        catch (ArgumentException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static void TryDelete(RegistryKey hive, string keyPath)
    {
        try
        {
            hive.DeleteSubKeyTree(keyPath, throwOnMissingSubKey: false);
        }
        catch (ArgumentException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
    }

    private static void ClearDefaultValue(RegistryKey key)
    {
        try
        {
            key.DeleteValue(string.Empty, throwOnMissingValue: false);
        }
        catch (ArgumentException)
        {
        }
    }

    private static void RemoveValueIfPresent(RegistryKey key, string name)
    {
        try
        {
            key.DeleteValue(name, throwOnMissingValue: false);
        }
        catch (ArgumentException)
        {
        }
    }

    /// <summary>Registry Icon value: full path to .ico or .exe plus resource index, e.g. C:\app\ImageConverter.ico,0</summary>
    private static string? ResolveIconPath(string exePath)
    {
        var dir = Path.GetDirectoryName(exePath);
        if (string.IsNullOrEmpty(dir))
        {
            return null;
        }

        var ico = Path.Combine(dir, "ImageConverter.ico");
        if (File.Exists(ico))
        {
            return $"{Path.GetFullPath(ico)},0";
        }

        return File.Exists(exePath) ? $"{Path.GetFullPath(exePath)},0" : null;
    }

    internal static void NotifyAssociationChanged()
    {
        const int shcneAssocChanged = 0x0800_0000;
        const uint shcnfIdList = 0x0000;
        SHChangeNotify(shcneAssocChanged, shcnfIdList, IntPtr.Zero, IntPtr.Zero);
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern void SHChangeNotify(int eventId, uint flags, IntPtr item1, IntPtr item2);
}
