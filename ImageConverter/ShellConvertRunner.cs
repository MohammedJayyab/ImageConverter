namespace ImageConverter;

internal static class ShellConvertRunner
{
    internal static int Run(ShellConvertCommandLine commandLine)
    {
        try
        {
            ApplicationConfiguration.Initialize();

            var settings = new AppSettingsStore().Load();
            var icoSize = ShellConversionDefaults.GetIcoSquareSizePixels(settings);
            var iconBackground = ShellConversionDefaults.GetIconBackground(settings);

            var existing = commandLine.SourcePaths
                .Where(p => File.Exists(p))
                .Where(SupportedFormats.IsPreviewFile)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var missing = commandLine.SourcePaths.Count - existing.Count;
            if (existing.Count == 0)
            {
                ShowError(
                    missing > 0
                        ? "No supported image files were found in the selection."
                        : "No image files were specified.");
                return 1;
            }

            var toConvert = existing
                .Where(p => !SupportedFormats.FormatIndexMatchesExtension(p, commandLine.OutputFormatIndex))
                .ToList();

            if (toConvert.Count == 0)
            {
                return 0;
            }

            var result = BatchConversionRunner.Run(
                toConvert,
                commandLine.OutputFormatIndex,
                icoSize,
                iconBackground,
                CancellationToken.None);

            if (result.FailCount == 0)
            {
                return 0;
            }

            var label = SupportedFormats.GetFormatLabel(commandLine.OutputFormatIndex);
            var message = result.SuccessCount > 0
                ? $"Converter To {label}: {result.SuccessCount} succeeded, {result.FailCount} failed."
                : $"Converter To {label}: conversion failed for {result.FailCount} file(s).";

            ShowError(message);
            return 1;
        }
        catch (Exception ex)
        {
            ShowError("Converter To: " + ex.Message);
            return 2;
        }
    }

    private static void ShowError(string message)
    {
        MessageBox.Show(
            message,
            "Image Converter — Converter To",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
