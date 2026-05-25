using System.Diagnostics;

namespace ImageConverter;

internal sealed class StartupApplicationContext : ApplicationContext
{
    private static readonly TimeSpan MinimumSplashVisible = TimeSpan.FromSeconds(1.5);

    public StartupApplicationContext()
    {
        var splash = new SplashForm();
        MainForm = splash;
        splash.Shown += OnSplashShown;
    }

    private async void OnSplashShown(object? sender, EventArgs e)
    {
        var splash = (SplashForm)sender!;
        splash.Shown -= OnSplashShown;
        splash.Activate();
        splash.BringToFront();

        var splashClock = Stopwatch.StartNew();
        var remaining = MinimumSplashVisible - splashClock.Elapsed;
        if (remaining > TimeSpan.Zero)
        {
            await Task.Delay(remaining).ConfigureAwait(true);
        }

        var main = new frmMain();
        MainForm = main;
        splash.Close();
        main.Show();
    }
}
