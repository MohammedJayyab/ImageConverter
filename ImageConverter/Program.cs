using ImageConverter.Shell;

namespace ImageConverter;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        args ??= [];

        if (ShellHost.TryHandleCommandLine(args, out var exitCode))
        {
            Environment.Exit(exitCode);
            return;
        }

        var isShellOpen = ShellArguments.ContainsFlag(args, ShellHost.OpenFlag);
        ShellOpenRequest? openRequest = null;
        if (isShellOpen && !ShellOpenRequest.TryParse(args, ShellHost.OpenFlag, out openRequest))
        {
            ApplicationConfiguration.Initialize();
            MessageBox.Show(
                "Image Converter could not open the selected image or folder.",
                "Image Converter",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        ApplicationConfiguration.Initialize();

        using var instanceMutex = TryAcquireSingleInstanceMutex();
        if (openRequest is null && instanceMutex is null)
        {
            MessageBox.Show(
                "Image Converter is already running.",
                "Image Converter",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        Application.Run(new StartupApplicationContext(openRequest));
    }

    private static Mutex? TryAcquireSingleInstanceMutex()
    {
        try
        {
            var mutex = new Mutex(true, AppInfo.SingleInstanceMutexName, out var createdNew);
            if (!createdNew)
            {
                mutex.Dispose();
                return null;
            }

            return mutex;
        }
        catch
        {
            return null;
        }
    }
}
