using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;

namespace ImageConverter.Shell;

internal static class ExplorerShellRegistry
{
    internal const string ParentShellKeyName = "ImageConverter.ConvertTo";

    private const string ParentMenuLabel = "Convert to";

    private const string AssociationsRoot = @"Software\Classes\SystemFileAssociations";

    private const string CommandStoreRoot =
        @"Software\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell";

    private const int SeparatorBefore = 0x20;

    private const string ScaleHalfVerbId = "ImageConverter.ConvertTo.Scale.Half";
    private const string Scale2xVerbId = "ImageConverter.ConvertTo.Scale.2x";

    private static readonly string[] ObsoleteMenuTreeKeys =
    [
        @"Software\Classes\ImageConverter.ConvertToMenu",
        @"Software\Classes\ImageConverter.ConvertToScaleMenu"
    ];

    private static readonly string[] ObsoleteCommandStoreVerbIds =
    [
        "ImageConverter.ConvertTo.Scale"
    ];

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

    private static readonly (string VerbId, string MenuLabel, string ScaleArg, bool SeparatorBefore)[] ScaleVerbs =
    [
        (ScaleHalfVerbId, "Scale 0.5x", "0.5", SeparatorBefore: true),
        (Scale2xVerbId, "Scale 2x", "2", SeparatorBefore: false)
    ];

    private static readonly string[] LegacyShellKeyNames =
    [
        "ConverterTo",
        "ImageConverter.ConverterTo",
        "ImageConverter.ConverterTo.Menu",
        "ImageConverter.ConvertTo"
    ];

    internal sealed record SyncResult(bool HklmCommandStoreWritten, string? Warning);

    internal static SyncResult Sync(bool enabled, string exePath)
    {
        if (!enabled)
        {
            UnregisterAll();
            NotifyAssociationChanged();
            return new SyncResult(false, null);
        }

        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
        {
            throw new InvalidOperationException("Application executable path is not available.");
        }

        var fullExe = Path.GetFullPath(exePath);
        var iconPath = ResolveIconPath(fullExe);

        UnregisterAll();

        var hklmOk = RegisterCommandStoreVerbs(fullExe);
        var subCommands = BuildSubCommandsList();

        foreach (var extension in FileExtensions)
        {
            RegisterAssociationParent($@"{AssociationsRoot}\{extension}\shell", subCommands, iconPath);
        }

        NotifyAssociationChanged();

        string? warning = null;
        if (!hklmOk)
        {
            warning =
                "Format submenu verbs could not be written to HKLM (not elevated). " +
                "Run Image Converter as Administrator once, then click Refresh Explorer menu.";
        }

        return new SyncResult(hklmOk, warning);
    }

    internal static void UnregisterAll()
    {
        DeleteObsoleteMenuTrees();
        UnregisterCommandStoreVerbs();

        foreach (var extension in FileExtensions)
        {
            var shellPath = $@"{AssociationsRoot}\{extension}\shell";
            DeleteKey(Registry.CurrentUser, $@"{shellPath}\{ParentShellKeyName}");

            foreach (var legacy in LegacyShellKeyNames)
            {
                DeleteKey(Registry.CurrentUser, $@"{shellPath}\{legacy}");
            }

            foreach (var (_, verbSuffix, _) in OutputFormats)
            {
                DeleteKey(Registry.CurrentUser, $@"{shellPath}\ImageConverter.To.{verbSuffix}");
            }

            DeleteLegacyKeysUnderClassesRoot(extension);
        }
    }

    private static string BuildSubCommandsList()
    {
        var ids = OutputFormats
            .Select(f => FormatVerbId(f.VerbSuffix))
            .Concat(ScaleVerbs.Select(s => s.VerbId));
        return string.Join(";", ids);
    }

    private static string FormatVerbId(string verbSuffix) =>
        $"ImageConverter.ConvertTo.{verbSuffix}";

    private static bool RegisterCommandStoreVerbs(string exePath)
    {
        var hklmOk = true;

        foreach (var hive in new[] { Registry.LocalMachine, Registry.CurrentUser })
        {
            var requireHklm = hive == Registry.LocalMachine;

            foreach (var (shellArg, verbSuffix, menuLabel) in OutputFormats)
            {
                if (!RegisterCommandStoreActionVerb(
                        hive,
                        FormatVerbId(verbSuffix),
                        menuLabel,
                        $"\"{exePath}\" --shell-convert {shellArg} \"%1\"",
                        commandFlags: null,
                        requireSuccess: requireHklm))
                {
                    hklmOk = false;
                }
            }

            foreach (var (verbId, menuLabel, scaleArg, separatorBefore) in ScaleVerbs)
            {
                if (!RegisterCommandStoreActionVerb(
                        hive,
                        verbId,
                        menuLabel,
                        $"\"{exePath}\" --shell-scale {scaleArg} \"%1\"",
                        separatorBefore ? SeparatorBefore : null,
                        requireSuccess: requireHklm))
                {
                    hklmOk = false;
                }
            }
        }

        return hklmOk;
    }

    private static bool RegisterCommandStoreActionVerb(
        RegistryKey hive,
        string verbId,
        string menuLabel,
        string commandLine,
        int? commandFlags,
        bool requireSuccess)
    {
        try
        {
            using var verbKey = hive.CreateSubKey($@"{CommandStoreRoot}\{verbId}", writable: true);
            if (verbKey is null)
            {
                return !requireSuccess;
            }

            verbKey.SetValue(null, menuLabel);
            RemoveValueIfPresent(verbKey, "Icon");
            RemoveValueIfPresent(verbKey, "SubCommands");
            RemoveValueIfPresent(verbKey, "ExtendedSubCommandsKey");

            if (commandFlags.HasValue)
            {
                verbKey.SetValue("CommandFlags", commandFlags.Value, RegistryValueKind.DWord);
            }
            else
            {
                RemoveValueIfPresent(verbKey, "CommandFlags");
            }

            using var commandKey = verbKey.CreateSubKey("command", writable: true);
            commandKey?.SetValue(null, commandLine);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return !requireSuccess;
        }
        catch (SecurityException)
        {
            return !requireSuccess;
        }
    }

    private static void RegisterAssociationParent(string shellParentPath, string subCommands, string? iconPath)
    {
        using var parent = Registry.CurrentUser.CreateSubKey(
            $@"{shellParentPath}\{ParentShellKeyName}",
            writable: true);
        if (parent is null)
        {
            return;
        }

        ClearDefaultValue(parent);
        RemoveValueIfPresent(parent, "ExtendedSubCommandsKey");
        parent.SetValue("MUIVerb", ParentMenuLabel);
        parent.SetValue("SubCommands", subCommands);

        if (!string.IsNullOrEmpty(iconPath))
        {
            parent.SetValue("Icon", iconPath);
        }

        try
        {
            parent.DeleteSubKeyTree("command", throwOnMissingSubKey: false);
            parent.DeleteSubKeyTree("shell", throwOnMissingSubKey: false);
        }
        catch (ArgumentException)
        {
        }
    }

    private static void UnregisterCommandStoreVerbs()
    {
        foreach (var (_, verbSuffix, _) in OutputFormats)
        {
            var path = $@"{CommandStoreRoot}\{FormatVerbId(verbSuffix)}";
            TryDelete(Registry.CurrentUser, path);
            TryDelete(Registry.LocalMachine, path);
            TryDelete(Registry.CurrentUser, $@"{CommandStoreRoot}\ImageConverter.ConverterTo.{verbSuffix}");
            TryDelete(Registry.LocalMachine, $@"{CommandStoreRoot}\ImageConverter.ConverterTo.{verbSuffix}");
        }

        foreach (var (verbId, _, _, _) in ScaleVerbs)
        {
            TryDelete(Registry.CurrentUser, $@"{CommandStoreRoot}\{verbId}");
            TryDelete(Registry.LocalMachine, $@"{CommandStoreRoot}\{verbId}");
        }

        foreach (var obsolete in ObsoleteCommandStoreVerbIds)
        {
            TryDelete(Registry.CurrentUser, $@"{CommandStoreRoot}\{obsolete}");
            TryDelete(Registry.LocalMachine, $@"{CommandStoreRoot}\{obsolete}");
        }
    }

    private static void DeleteObsoleteMenuTrees()
    {
        foreach (var keyPath in ObsoleteMenuTreeKeys)
        {
            DeleteKey(Registry.CurrentUser, keyPath);
            TryDelete(Registry.LocalMachine, keyPath);
        }
    }

    private static void DeleteLegacyKeysUnderClassesRoot(string extension)
    {
        const string classesRoot = @"Software\Classes";
        DeleteKey(Registry.CurrentUser, $@"{classesRoot}\{extension}\shell\{ParentShellKeyName}");
        foreach (var legacy in LegacyShellKeyNames)
        {
            DeleteKey(Registry.CurrentUser, $@"{classesRoot}\{extension}\shell\{legacy}");
        }

        foreach (var (_, verbSuffix, _) in OutputFormats)
        {
            DeleteKey(Registry.CurrentUser, $@"{classesRoot}\{extension}\shell\ImageConverter.To.{verbSuffix}");
        }
    }

    private static void DeleteKey(RegistryKey hive, string keyPath)
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

    internal static bool VerifyScaleVerbsPresent(bool requireHklm = false)
    {
        foreach (var (verbId, _, _, _) in ScaleVerbs)
        {
            if (requireHklm)
            {
                if (!CommandStoreVerbExists(Registry.LocalMachine, verbId))
                {
                    return false;
                }

                continue;
            }

            if (!CommandStoreVerbExists(Registry.LocalMachine, verbId) &&
                !CommandStoreVerbExists(Registry.CurrentUser, verbId))
            {
                return false;
            }
        }

        return true;
    }

    internal static bool HklmFormatVerbsPresent() =>
        CommandStoreVerbExists(Registry.LocalMachine, FormatVerbId(OutputFormats[0].VerbSuffix));

    internal static bool NeedsHklmScaleRegistration() =>
        HklmFormatVerbsPresent() && !VerifyScaleVerbsPresent(requireHklm: true);

    internal static bool IsProcessElevated()
    {
        try
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    private static bool CommandStoreVerbExists(RegistryKey hive, string verbId)
    {
        try
        {
            using var key = hive.OpenSubKey($@"{CommandStoreRoot}\{verbId}");
            if (key is null)
            {
                return false;
            }

            using var command = key.OpenSubKey("command");
            return command?.GetValue(null) is string cmd && cmd.Length > 0;
        }
        catch (SecurityException)
        {
            return false;
        }
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
