namespace ImageConverter;

internal static class BatchConversionRunner
{
    internal sealed record RunResult(int SuccessCount, int FailCount, IReadOnlyList<string> SuccessfulOutputPaths);

    internal static RunResult Run(
        IReadOnlyList<string> sourcePaths,
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

            var outputPath = SupportedFormats.BuildOutputPath(src, outputFormatIndex);
            var request = new ConversionRequest
            {
                SourcePath = src,
                DestinationPath = outputPath,
                OutputFormatIndex = outputFormatIndex,
                IcoSquareSizePixels = icoSquareSizePixels,
                IconBackground = iconBackground
            };

            var code = ImageConversion.Convert(request, out _, cancellationToken);
            if (code == 0)
            {
                success++;
                createdOutputs.Add(outputPath);
            }
            else
            {
                fail++;
            }
        }

        return new RunResult(success, fail, createdOutputs);
    }
}
