using ImageMagick;

namespace ImageConverter;

internal readonly record struct ImageFileMetadata(long SizeBytes, uint Width, uint Height, DateTime LastModifiedLocal);

internal static class ImageFileMetadataReader
{
    internal static ImageFileMetadata? TryRead(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var info = new FileInfo(filePath);
            TryReadDimensions(filePath, out var width, out var height);
            return new ImageFileMetadata(info.Length, width, height, info.LastWriteTime);
        }
        catch
        {
            return null;
        }
    }

    private static bool TryReadDimensions(string filePath, out uint width, out uint height)
    {
        width = 0;
        height = 0;

        try
        {
            using var img = new MagickImage(filePath);
            width = img.Width;
            height = img.Height;
            return true;
        }
        catch (MagickException)
        {
            return false;
        }
    }
}
