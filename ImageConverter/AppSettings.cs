namespace ImageConverter;

/// <summary>Serializable UI preferences for <c>config.ini</c>.</summary>
internal sealed class AppSettings
{
    public string? LastSourceFolder { get; set; }

    public string? LastDestinationFolder { get; set; }

    /// <summary>0 = small, 1 = medium, 2 = large preview thumbnails.</summary>
    public int PreviewThumbnailSizeIndex { get; set; } = 1;

    /// <summary>Whether main window placement (position/size) has been saved at least once.</summary>
    public bool MainWindowPlacementSaved { get; set; }

    public int MainWindowLeft { get; set; }

    public int MainWindowTop { get; set; }

    public int MainWindowWidth { get; set; } = 1100;

    public int MainWindowHeight { get; set; } = 745;

    public bool MainWindowMaximized { get; set; }

    /// <summary>Horizontal split: height of the top panel (settings stack) in pixels.</summary>
    public int PreviewSplitterDistance { get; set; }

    /// <summary>Index into the ICO output size list (six entries: 16 … 256).</summary>
    public int IcoOutputSizeIndex { get; set; } = 5;

    /// <summary>Letterbox solid color: 0 = White, 1 = Black.</summary>
    public int SolidColorIndex { get; set; }
}
