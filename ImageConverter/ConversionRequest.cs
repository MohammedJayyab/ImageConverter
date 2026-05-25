namespace ImageConverter;

internal sealed class ConversionRequest
{
    public required string SourcePath { get; init; }
    public required string DestinationPath { get; init; }
    public int OutputFormatIndex { get; init; }
    public int IcoSquareSizePixels { get; init; }

    public IconBackgroundKind IconBackground { get; init; }
}
