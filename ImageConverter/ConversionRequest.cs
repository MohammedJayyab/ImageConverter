namespace ImageConverter;

/// <summary>Parameters for a single file conversion (no Magick types — keeps UI layer free of ImageMagick references).</summary>
public sealed class ConversionRequest
{
    public required string SourcePath { get; init; }
    public required string DestinationPath { get; init; }

    /// <summary>Index 0..5: JPEG, PNG, BMP, GIF, WEBP, ICO (same order as the main window combos).</summary>
    public int OutputFormatIndex { get; init; }

    /// <summary>Square ICO canvas in pixels (16–256) when output is ICO.</summary>
    public int IcoSquareSizePixels { get; init; }

    public IconBackgroundKind IconBackground { get; init; }
}
