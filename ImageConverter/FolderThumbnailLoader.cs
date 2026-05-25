using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ImageMagick;

namespace ImageConverter;

internal static class FolderThumbnailLoader
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".ico", ".svg", ".pdf"
    };

    private static readonly HashSet<string> GdiPreferredExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".ico"
    };

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

    internal static Bitmap CreateThumbnailOrPlaceholder(string filePath, int size)
    {
        return TryCreateThumbnail(filePath, size) ?? CreatePlaceholderThumbnail(size, filePath);
    }

    private static Bitmap? TryCreateThumbnail(string filePath, int size)
    {
        var ext = Path.GetExtension(filePath);
        if (GdiPreferredExtensions.Contains(ext))
        {
            var gdi = TryCreateThumbnailViaGdi(filePath, size);
            if (gdi is not null)
            {
                return gdi;
            }
        }

        return TryCreateThumbnailViaMagick(filePath, size);
    }

    private static Bitmap? TryCreateThumbnailViaGdi(string filePath, int size)
    {
        try
        {
            using var src = Image.FromFile(filePath);
            return LetterboxImage(src, size);
        }
        catch
        {
            return null;
        }
    }

    private static Bitmap? TryCreateThumbnailViaMagick(string filePath, int size)
    {
        try
        {
            using var img = new MagickImage(filePath);
            if (img.Width == 0 || img.Height == 0)
            {
                return null;
            }

            var ratio = Math.Min((float)size / img.Width, (float)size / img.Height);
            var w = Math.Max(1, (int)Math.Round(img.Width * ratio));
            var h = Math.Max(1, (int)Math.Round(img.Height * ratio));

            using var resized = (MagickImage)img.Clone();
            resized.Resize((uint)w, (uint)h);
            resized.Format = MagickFormat.Png;

            using var ms = new MemoryStream();
            resized.Write(ms);
            ms.Position = 0;
            using var decoded = Image.FromStream(ms);
            return LetterboxImage(decoded, size);
        }
        catch
        {
            return null;
        }
    }

    private static Bitmap LetterboxImage(Image src, int size)
    {
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

    private static Bitmap CreatePlaceholderThumbnail(int size, string filePath)
    {
        var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.FromArgb(235, 235, 235));
            using var borderPen = new Pen(Color.FromArgb(190, 190, 190));
            g.DrawRectangle(borderPen, 0, 0, size - 1, size - 1);

            var ext = Path.GetExtension(filePath).TrimStart('.');
            if (ext.Length > 5)
            {
                ext = ext[..5];
            }

            if (string.IsNullOrEmpty(ext))
            {
                ext = "?";
            }

            using var font = new Font(FontFamily.GenericSansSerif, Math.Max(8f, size / 6f), FontStyle.Bold, GraphicsUnit.Pixel);
            var rect = new RectangleF(0, 0, size, size);
            using var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            g.DrawString(ext.ToUpperInvariant(), font, Brushes.DimGray, rect, format);
        }

        return bmp;
    }
}
