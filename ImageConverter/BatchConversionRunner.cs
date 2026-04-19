namespace ImageConverter;

/// <summary>Runs <see cref="ImageConversion.Convert"/> over a list of files (used by the main window).</summary>
internal static class BatchConversionRunner
{
    public sealed record RunResult(int SuccessCount, int FailCount, IReadOnlyList<string> SuccessfulDestinationPaths);

    public static RunResult Run(
        IReadOnlyList<string> sourcePaths,
        string destinationFolder,
        int outputFormatIndex,
        int icoSquareSizePixels,
        IconBackgroundKind iconBackground,
        CancellationToken cancellationToken,
        IProgress<(int Current, int Total, string FileName)>? progress = null)
    {
        var success = 0;
        var fail = 0;
        var total = sourcePaths.Count;
        var createdOutputs = new List<string>();

        for (var i = 0; i < sourcePaths.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var src = sourcePaths[i];
            var name = Path.GetFileName(src);
            progress?.Report((i + 1, total, name));

            var dest = SupportedFormats.BuildDestinationPath(src, destinationFolder, outputFormatIndex);
            var request = new ConversionRequest
            {
                SourcePath = src,
                DestinationPath = dest,
                OutputFormatIndex = outputFormatIndex,
                IcoSquareSizePixels = icoSquareSizePixels,
                IconBackground = iconBackground
            };

            var code = ImageConversion.Convert(request, out _, cancellationToken);
            switch (code)
            {
                case 0:
                    success++;
                    createdOutputs.Add(dest);
                    break;
                case 1:
                case 2:
                    fail++;
                    break;
                default:
                    fail++;
                    break;
            }
        }

        return new RunResult(success, fail, createdOutputs);
    }
}
