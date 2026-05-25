using System.Diagnostics;

namespace ImageConverter;

public sealed partial class SplashForm : Form
{
    private static readonly TimeSpan ProgressFillDuration = TimeSpan.FromSeconds(1.5);

    private const int SplashWidthPx = 560;
    private const int ProgressHeightPx = 11;
    private const int GapAfterProgressPx = 12;
    private const int TextInsetPx = 24;
    private const int LoadingHeightPx = 26;
    private const int FooterLineHeightPx = 22;
    private const int BottomPaddingPx = 12;
    private const int DotsTickStride = 8;

    private readonly Stopwatch _fillWatch = new();
    private int _dotPhase;
    private int _tickCount;

    public SplashForm()
    {
        SuspendLayout();
        try
        {
            InitializeComponent();
            lblDevelopedBy.Text = $"Developed by Mohammed Jayyab  ·  V{AppInfo.VersionDisplay}";
            TryLoadSplashImage();
            ApplySplashLayout();
        }
        finally
        {
            ResumeLayout(false);
        }

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

        timerSplash.Tick += OnTimerTick;
        Load += OnSplashLoad;
        FormClosed += (_, _) => timerSplash.Enabled = false;
    }

    private void OnSplashLoad(object? sender, EventArgs e)
    {
        _fillWatch.Start();
        OnTimerTick(null, EventArgs.Empty);
        timerSplash.Enabled = true;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            const int wsExComposited = 0x02000000;
            var cp = base.CreateParams;
            cp.ExStyle |= wsExComposited;
            return cp;
        }
    }

    private void TryLoadSplashImage()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "images", "splash_screen.png");
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            pictureSplash.Image?.Dispose();
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            pictureSplash.Image = Image.FromStream(fs);
        }
        catch
        {
        }
    }

    private void ApplySplashLayout()
    {
        var w = SplashWidthPx;
        var imgH = GetScaledImageHeight(w);
        pictureSplash.SetBounds(0, 0, w, imgH, BoundsSpecified.All);

        var y = pictureSplash.Bottom;
        lblProgressFill.SetBounds(0, y, 0, ProgressHeightPx, BoundsSpecified.All);
        y += ProgressHeightPx + GapAfterProgressPx;

        var textW = w - 2 * TextInsetPx;
        lblLoading.SetBounds(TextInsetPx, y, textW, LoadingHeightPx, BoundsSpecified.All);
        y += LoadingHeightPx + GapAfterProgressPx;

        lblDevelopedBy.SetBounds(TextInsetPx, y, textW, FooterLineHeightPx, BoundsSpecified.All);
        y += FooterLineHeightPx + BottomPaddingPx;

        ClientSize = new Size(w, y);
        MinimumSize = ClientSize;
        MaximumSize = ClientSize;
    }

    private int GetScaledImageHeight(int width)
    {
        if (pictureSplash.Image is not { } img)
        {
            return 120;
        }

        var scale = width / (float)img.Width;
        return Math.Max(1, (int)Math.Floor(img.Height * scale));
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        _tickCount++;
        if (_tickCount % DotsTickStride == 0)
        {
            _dotPhase = (_dotPhase + 1) % 6;
            lblLoading.Text = "Loading" + new string('.', _dotPhase);
        }

        var t = Math.Min(1.0, _fillWatch.Elapsed.TotalMilliseconds / ProgressFillDuration.TotalMilliseconds);
        var fillW = (int)Math.Round(SplashWidthPx * t);
        lblProgressFill.Width = Math.Max(0, Math.Min(fillW, SplashWidthPx));
    }
}
