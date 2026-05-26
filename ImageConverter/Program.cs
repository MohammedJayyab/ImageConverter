namespace ImageConverter;

internal static class Program
{
    private const string ShellConvertFlag = "--shell-convert";

    [STAThread]
    static void Main(string[] args)
    {
        args ??= [];

        if (ContainsFlag(args, ShellConvertFlag))
        {
            if (ShellConvertCommandLine.TryParse(args, out var shellCommand) && shellCommand is not null)
            {
                Environment.Exit(ShellConvertRunner.Run(shellCommand));
                return;
            }

            MessageBox.Show(
                "Converter To could not read the selected file from Explorer.\r\n\r\n" +
                "Use Refresh Explorer menu in Image Converter, then close all Explorer windows and open a new one.",
                "Image Converter — Converter To",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Environment.Exit(1);
            return;
        }

        ApplicationConfiguration.Initialize();

        using var instanceMutex = TryAcquireSingleInstanceMutex();
        if (instanceMutex is null)
        {
            MessageBox.Show(
                "Image Converter is already running.",
                "Image Converter",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        Application.Run(new StartupApplicationContext());
    }

    private static bool ContainsFlag(string[] args, string flag) =>
        args.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));

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
