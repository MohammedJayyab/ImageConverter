using System.Diagnostics;
using ImageConverter.Shell;

namespace ImageConverter;

internal sealed class StartupApplicationContext : ApplicationContext
{
    private static readonly TimeSpan MinimumSplashVisible = TimeSpan.FromSeconds(1.5);

    private readonly ShellOpenRequest? _openRequest;

    public StartupApplicationContext(ShellOpenRequest? openRequest = null)
    {
        _openRequest = openRequest;
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

        var main = new frmMain(_openRequest?.FolderPath, _openRequest?.SelectedImagePaths);
        MainForm = main;
        splash.Close();
        main.Show();
    }
}
