namespace ImageConverter;

internal static class BatchImageResizeRunner
{
    internal sealed record RunResult(int SuccessCount, int FailCount, IReadOnlyList<string> SuccessfulOutputPaths);

    internal static RunResult Run(
        IReadOnlyList<string> sourcePaths,
        double scaleFactor,
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

            var outputPath = ImageResize.BuildScaledOutputPath(src);
            var code = ImageResize.Scale(src, outputPath, scaleFactor, cancellationToken, out _);
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
