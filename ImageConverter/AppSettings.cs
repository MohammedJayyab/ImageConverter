namespace ImageConverter;

internal sealed class AppSettings
{
    public string? LastFolder { get; set; }

    public int PreviewThumbnailSizeIndex { get; set; } = 1;

    public bool MainWindowPlacementSaved { get; set; }

    public int MainWindowLeft { get; set; }

    public int MainWindowTop { get; set; }

    public int MainWindowWidth { get; set; } = 1100;

    public int MainWindowHeight { get; set; } = 745;

    public bool MainWindowMaximized { get; set; }

    public int PreviewSplitterDistance { get; set; }

    public int IcoOutputSizeIndex { get; set; } = 5;

    public int SolidColorIndex { get; set; }
}
