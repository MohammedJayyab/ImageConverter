using ImageMagick;

namespace ImageConverter;

internal static class ImageResize
{
    private static readonly string[] ResizableExtensions =
    [
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".ico"
    ];

    internal static bool IsResizablePath(string filePath)
    {
        var ext = Path.GetExtension(filePath);
        return ResizableExtensions.Any(e => ext.Equals(e, StringComparison.OrdinalIgnoreCase));
    }

    internal static string BuildScaledOutputPath(string sourcePath) =>
        Path.Combine(
            Path.GetDirectoryName(sourcePath) ?? string.Empty,
            Path.GetFileNameWithoutExtension(sourcePath) + "_scaled" + Path.GetExtension(sourcePath));

    internal static int Scale(
        string sourcePath,
        string destinationPath,
        int scaleFactor,
        CancellationToken cancellationToken,
        out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();

        if (scaleFactor is not (2 or 4))
        {
            errorMessage = "Scale factor must be 2 or 4.";
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
            var newWidth = img.Width * (uint)scaleFactor;
            var newHeight = img.Height * (uint)scaleFactor;
            if (newWidth == 0 || newHeight == 0)
            {
                errorMessage = "Image dimensions are too small to scale.";
                return 1;
            }

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
