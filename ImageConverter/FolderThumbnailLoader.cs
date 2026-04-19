using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageConverter;

/// <summary>
/// Enumerates supported image files and builds thumbnails for the preview pane (System.Drawing; conversion uses Magick.NET later).
/// </summary>
internal static class FolderThumbnailLoader
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".ico"
    };

    /// <summary>
    /// Top-level files only, sorted by file name (case-insensitive).
    /// </summary>
    internal static List<string> EnumerateImageFiles(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            return [];
        }

        try
        {
            return Directory.EnumerateFiles(folderPath)
                .Where(f => SupportedExtensions.Contains(Path.GetExtension(f)))
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Creates a square thumbnail with aspect ratio preserved (letterboxed on transparent background).
    /// Caller must dispose the bitmap when adding to <see cref="ImageList"/> (ImageList takes ownership when added).
    /// </summary>
    internal static Bitmap? TryCreateThumbnail(string filePath, int size)
    {
        try
        {
            using var src = Image.FromFile(filePath);
            var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var ratio = Math.Min((float)size / src.Width, (float)size / src.Height);
                var w = Math.Max(1, (int)Math.Round(src.Width * ratio));
                var h = Math.Max(1, (int)Math.Round(src.Height * ratio));
                var x = (size - w) / 2;
                var y = (size - h) / 2;
                g.DrawImage(src, x, y, w, h);
            }

            return bmp;
        }
        catch
        {
            return null;
        }
    }
}
