namespace ImageConverter;

internal static class Program
{
    [STAThread]
    static void Main()
    {
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
