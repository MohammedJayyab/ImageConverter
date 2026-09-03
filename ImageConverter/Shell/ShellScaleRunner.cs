using ImageConverter;

namespace ImageConverter.Shell;

internal static class ShellScaleRunner
{
    internal static int Run(ShellScaleCommandLine commandLine)
    {
        try
        {
            var existing = commandLine.SourcePaths
                .Where(File.Exists)
                .Where(ImageResize.IsResizablePath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (existing.Count == 0)
            {
                ShowError(
                    "No resizable image files were found. Scale supports JPEG, PNG, BMP, GIF, WEBP, ICO, and SVG only.");
                return 1;
            }

            var result = BatchImageResizeRunner.Run(
                existing,
                commandLine.ScaleFactor,
                CancellationToken.None);

            if (result.FailCount == 0)
            {
                return 0;
            }

            var scaleLabel = FormatScaleLabel(commandLine.ScaleFactor);
            var message = result.SuccessCount > 0
                ? $"Scale {scaleLabel}: {result.SuccessCount} succeeded, {result.FailCount} failed."
                : $"Scale {scaleLabel}: resize failed for {result.FailCount} file(s).";

            ShowError(message);
            return 1;
        }
        catch (Exception ex)
        {
            ShowError("Scale: " + ex.Message);
            return 2;
        }
    }

    private static string FormatScaleLabel(double scaleFactor) =>
        Math.Abs(scaleFactor - 0.5) < 0.001 ? "0.5×" : $"{scaleFactor:G}×";

    private static void ShowError(string message)
    {
        MessageBox.Show(
            message,
            "Image Converter — Convert to",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
