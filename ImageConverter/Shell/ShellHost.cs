using System.Diagnostics;

namespace ImageConverter.Shell;

internal static class ShellHost
{
    internal const string RegisterMenuFlag = "--shell-register-menu";

    private const string ConvertFlag = "--shell-convert";
    private const string ScaleFlag = "--shell-scale";

    internal static bool TryHandleCommandLine(string[] args, out int exitCode)
    {
        exitCode = 0;

        if (ShellArguments.ContainsFlag(args, RegisterMenuFlag))
        {
            exitCode = RunRegisterMenu();
            return true;
        }

        if (ShellArguments.ContainsFlag(args, ScaleFlag))
        {
            exitCode = RunScale(args);
            return true;
        }

        if (ShellArguments.ContainsFlag(args, ConvertFlag))
        {
            exitCode = RunConvert(args);
            return true;
        }

        return false;
    }

    internal static bool TryLaunchElevatedRegisterMenu()
    {
        var exePath = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
        {
            return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = RegisterMenuFlag,
                UseShellExecute = true,
                Verb = "runas"
            });
            return true;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }

    private static int RunRegisterMenu()
    {
        var exePath = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
        {
            return 1;
        }

        try
        {
            var result = ExplorerShellRegistry.Sync(enabled: true, exePath);
            var scaleOk = ExplorerShellRegistry.VerifyScaleVerbsPresent(requireHklm: true);
            return result.HklmCommandStoreWritten && scaleOk ? 0 : 1;
        }
        catch
        {
            return 1;
        }
    }

    private static int RunScale(string[] args)
    {
        if (ShellScaleCommandLine.TryParse(args, out var scaleCommand) && scaleCommand is not null)
        {
            return ShellScaleRunner.Run(scaleCommand);
        }

        MessageBox.Show(
            "Scale could not read the selected file from Explorer.\r\n\r\n" +
            "Use Refresh Explorer menu in Image Converter, then close all Explorer windows and open a new one.",
            "Image Converter — Convert to",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return 1;
    }

    private static int RunConvert(string[] args)
    {
        if (ShellConvertCommandLine.TryParse(args, out var shellCommand) && shellCommand is not null)
        {
            return ShellConvertRunner.Run(shellCommand);
        }

        MessageBox.Show(
            "Converter To could not read the selected file from Explorer.\r\n\r\n" +
            "Use Refresh Explorer menu in Image Converter, then close all Explorer windows and open a new one.",
            "Image Converter — Converter To",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return 1;
    }
}
