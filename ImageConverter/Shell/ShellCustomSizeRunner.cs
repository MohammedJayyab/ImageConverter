using ImageConverter;

namespace ImageConverter.Shell;

internal static class ShellCustomSizeRunner
{
    internal static int Run(string[] args)
    {
        try
        {
            ApplicationConfiguration.Initialize();

            var sourcePaths = ShellConvertPaths.CollectExistingImagePathsFromArgs(args)
                .Where(ImageResize.IsResizablePath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (sourcePaths.Count == 0)
            {
                ShowWarning(
                    "No resizable image files were found. Custom size supports JPEG, PNG, BMP, GIF, WEBP, ICO, and SVG only.");
                return 1;
            }

            var initialOutputFolder = Path.GetDirectoryName(sourcePaths[0]) ?? string.Empty;
            using var dialog = new CustomSizeForm(initialOutputFolder)
            {
                StartPosition = FormStartPosition.CenterScreen
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return 0;
            }

            var outputPaths = sourcePaths
                .Select(path => ImageResize.BuildScaledOutputPath(path, dialog.OutputFolder))
                .ToList();
            if (!ConfirmOverwriteExistingFiles(outputPaths))
            {
                return 0;
            }

            var result = BatchImageResizeRunner.RunToSize(
                sourcePaths,
                dialog.OutputWidth,
                dialog.OutputHeight,
                dialog.OutputFolder,
                CancellationToken.None);

            if (result.FailCount == 0)
            {
                return 0;
            }

            var message = result.SuccessCount > 0
                ? $"Custom size: {result.SuccessCount} succeeded, {result.FailCount} failed."
                : $"Custom size failed for {result.FailCount} file(s).";
            ShowWarning(message);
            return 1;
        }
        catch (Exception ex)
        {
            ShowWarning("Custom size: " + ex.Message);
            return 2;
        }
    }

    private static bool ConfirmOverwriteExistingFiles(IReadOnlyList<string> outputPaths)
    {
        var existing = outputPaths
            .Where(File.Exists)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (existing.Count == 0)
        {
            return true;
        }

        var message = existing.Count == 1
            ? $"\"{Path.GetFileName(existing[0])}\" already exists.\r\n\r\nOverwrite it?"
            : $"{existing.Count} output files already exist.\r\n\r\nOverwrite them?";

        return MessageBox.Show(
                message,
                "Confirm overwrite",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2)
            == DialogResult.Yes;
    }

    private static void ShowWarning(string message)
    {
        MessageBox.Show(
            message,
            "Image Converter — Custom size",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
