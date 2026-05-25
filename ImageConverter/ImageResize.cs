using ImageMagick;

namespace ImageConverter;

internal static class ImageResize
{
    internal static readonly double[] SupportedScaleFactors = [0.5, 0.75, 2, 4];

    internal static bool IsSupportedScaleFactor(double scaleFactor) =>
        SupportedScaleFactors.Any(s => Math.Abs(s - scaleFactor) < 0.001);

    internal static bool IsResizablePath(string filePath) => SupportedFormats.IsResizableFile(filePath);

    internal static string BuildScaledOutputPath(string sourcePath) =>
        Path.Combine(
            Path.GetDirectoryName(sourcePath) ?? string.Empty,
            Path.GetFileNameWithoutExtension(sourcePath) + "_scaled" + Path.GetExtension(sourcePath));

    internal static int Scale(
        string sourcePath,
        string destinationPath,
        double scaleFactor,
        CancellationToken cancellationToken,
        out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsSupportedScaleFactor(scaleFactor))
        {
            errorMessage = "Scale factor must be 0.5, 0.75, 2, or 4.";
            return 1;
        }

        if (!IsResizablePath(sourcePath))
        {
            errorMessage = "This file type cannot be resized here.";
            return 1;
        }

        if (!File.Exists(sourcePath))
        {
            errorMessage = "Source file not found.";
            return 1;
        }

        var srcFull = Path.GetFullPath(sourcePath);
        var dstFull = Path.GetFullPath(destinationPath);
        if (string.Equals(srcFull, dstFull, StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "Source and destination are the same file.";
            return 1;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var img = new MagickImage(sourcePath);
            var newWidth = (uint)Math.Max(1, (int)Math.Round(img.Width * scaleFactor));
            var newHeight = (uint)Math.Max(1, (int)Math.Round(img.Height * scaleFactor));

            img.FilterType = FilterType.Lanczos;
            img.Resize(newWidth, newHeight);

            var destDir = Path.GetDirectoryName(dstFull);
            if (!string.IsNullOrEmpty(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            img.Write(dstFull);
            return 0;
        }
        catch (MagickException ex)
        {
            errorMessage = ex.Message;
            return 2;
        }
        catch (IOException ex)
        {
            errorMessage = ex.Message;
            return 2;
        }
        catch (UnauthorizedAccessException ex)
        {
            errorMessage = ex.Message;
            return 2;
        }
    }
}
