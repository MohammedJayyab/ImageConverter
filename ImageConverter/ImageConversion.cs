using System.Text;
using ImageMagick;

namespace ImageConverter;

internal static class ImageConversion
{
    internal static int Convert(ConversionRequest request, out string? errorMessage, CancellationToken cancellationToken = default)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();

        if (request.OutputFormatIndex < 0 || request.OutputFormatIndex >= SupportedFormats.Count)
        {
            errorMessage = "Invalid output format index.";
            return 1;
        }

        if (string.IsNullOrWhiteSpace(request.SourcePath) || !File.Exists(request.SourcePath))
        {
            errorMessage = "Source file not found.";
            return 1;
        }

        if (string.IsNullOrWhiteSpace(request.DestinationPath))
        {
            errorMessage = "Destination path is empty.";
            return 1;
        }

        var srcFull = Path.GetFullPath(request.SourcePath);
        var dstFull = Path.GetFullPath(request.DestinationPath);
        if (string.Equals(srcFull, dstFull, StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "Source and destination are the same file.";
            return 1;
        }

        var destDir = Path.GetDirectoryName(dstFull);
        if (!string.IsNullOrEmpty(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var dstIsIco = request.OutputFormatIndex == SupportedFormats.IcoFormatIndex;
            var dstIsSvg = request.OutputFormatIndex == SupportedFormats.SvgFormatIndex;
            var dstIsPdf = request.OutputFormatIndex == SupportedFormats.PdfFormatIndex;
            var srcIsIco = Path.GetExtension(request.SourcePath).Equals(".ico", StringComparison.OrdinalIgnoreCase);

            if (srcIsIco && dstIsIco)
            {
                return ConvertIcoToIco(request, cancellationToken, out errorMessage);
            }

            if (srcIsIco && dstIsSvg)
            {
                return ConvertIcoToEmbeddedSvg(request, cancellationToken, out errorMessage);
            }

            if (srcIsIco && dstIsPdf)
            {
                return ConvertIcoToPdf(request, cancellationToken, out errorMessage);
            }

            if (srcIsIco && !dstIsIco)
            {
                var outFmt = MapToMagickFormat(request.OutputFormatIndex);
                return ConvertIcoToRaster(request, outFmt, cancellationToken, out errorMessage);
            }

            if (!srcIsIco && dstIsIco)
            {
                return ConvertRasterToIco(request, cancellationToken, out errorMessage);
            }

            if (!srcIsIco && dstIsSvg)
            {
                return ConvertRasterToEmbeddedSvg(request, cancellationToken, out errorMessage);
            }

            if (!srcIsIco && dstIsPdf)
            {
                return ConvertRasterToPdf(request, cancellationToken, out errorMessage);
            }

            var rasterFmt = MapToMagickFormat(request.OutputFormatIndex);
            return ConvertRasterToRaster(request, rasterFmt, cancellationToken, out errorMessage);
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

    private static MagickFormat MapToMagickFormat(int index) => index switch
    {
        0 => MagickFormat.Jpeg,
        1 => MagickFormat.Png,
        2 => MagickFormat.Bmp,
        3 => MagickFormat.Gif,
        4 => MagickFormat.WebP,
        5 => MagickFormat.Ico,
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    private static int ConvertRasterToRaster(ConversionRequest request, MagickFormat outFmt, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        using var img = new MagickImage(request.SourcePath);
        img.Format = outFmt;
        ApplyQualityHints(img, outFmt);
        ApplyOutputBackground(img, outFmt, request.IconBackground);
        img.Write(request.DestinationPath);
        return 0;
    }

    private static int ConvertRasterToEmbeddedSvg(ConversionRequest request, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        using var img = new MagickImage(request.SourcePath);
        ApplyOutputBackground(img, MagickFormat.Png, request.IconBackground);
        WriteEmbeddedSvgFromMagickImage(img, request.DestinationPath);
        return 0;
    }

    private static int ConvertIcoToEmbeddedSvg(ConversionRequest request, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        using var collection = new MagickImageCollection();
        collection.Read(request.SourcePath);
        if (collection.Count == 0)
        {
            errorMessage = "ICO file contains no images.";
            return 2;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var best = SelectLargestFrame(collection);
        using var output = (MagickImage)best.Clone();
        ApplyOutputBackground(output, MagickFormat.Png, request.IconBackground);
        WriteEmbeddedSvgFromMagickImage(output, request.DestinationPath);
        return 0;
    }

    private static int ConvertRasterToPdf(ConversionRequest request, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        using var img = new MagickImage(request.SourcePath);
        WriteMagickImageAsPdf(img, request.DestinationPath);
        return 0;
    }

    private static int ConvertIcoToPdf(ConversionRequest request, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        using var collection = new MagickImageCollection();
        collection.Read(request.SourcePath);
        if (collection.Count == 0)
        {
            errorMessage = "ICO file contains no images.";
            return 2;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var best = SelectLargestFrame(collection);
        using var output = (MagickImage)best.Clone();
        WriteMagickImageAsPdf(output, request.DestinationPath);
        return 0;
    }

    private static void WriteMagickImageAsPdf(MagickImage img, string destinationPath)
    {
        using var pdf = (MagickImage)img.Clone();
        pdf.Format = MagickFormat.Pdf;
        pdf.Write(destinationPath);
    }

    private static void WriteEmbeddedSvgFromMagickImage(MagickImage img, string destinationPath)
    {
        using var pngMs = new MemoryStream();
        img.Format = MagickFormat.Png;
        img.Write(pngMs);
        WriteEmbeddedSvg(pngMs.ToArray(), img.Width, img.Height, destinationPath);
    }

    private static void WriteEmbeddedSvg(byte[] pngBytes, uint width, uint height, string destinationPath)
    {
        var base64 = System.Convert.ToBase64String(pngBytes);
        var svg = $"""
<?xml version="1.0" encoding="UTF-8"?>
<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="{width}" height="{height}" viewBox="0 0 {width} {height}">
  <image width="{width}" height="{height}" xlink:href="data:image/png;base64,{base64}"/>
</svg>
""";
        File.WriteAllText(destinationPath, svg, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static void ApplyQualityHints(MagickImage img, MagickFormat fmt)
    {
        if (fmt == MagickFormat.Jpeg)
        {
            img.Quality = 92;
        }
        else if (fmt == MagickFormat.WebP)
        {
            img.Quality = 90;
        }
    }

    private static void ApplyOutputBackground(MagickImage img, MagickFormat fmt, IconBackgroundKind background)
    {
        if (background == IconBackgroundKind.Transparent)
        {
            if (fmt == MagickFormat.Jpeg || fmt == MagickFormat.Bmp)
            {
                return;
            }

            PreserveTransparentBackground(img);
            return;
        }

        FlattenIfNeededForOpaque(img, fmt);
    }

    private static void PreserveTransparentBackground(MagickImage img)
    {
        if (img.HasAlpha)
        {
            return;
        }

        img.ColorFuzz = new Percentage(12);
        img.Transparent(MagickColors.White);
    }

    private static void FlattenIfNeededForOpaque(MagickImage img, MagickFormat fmt)
    {
        if (fmt != MagickFormat.Jpeg && fmt != MagickFormat.Bmp)
        {
            return;
        }

        if (!img.HasAlpha)
        {
            return;
        }

        img.BackgroundColor = MagickColors.White;
        img.Alpha(AlphaOption.Remove);
    }

    private static int ConvertRasterToIco(ConversionRequest request, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        var size = request.IcoSquareSizePixels;
        if (size is < 16 or > 256)
        {
            errorMessage = "ICO size must be between 16 and 256.";
            return 1;
        }

        using var img = new MagickImage(request.SourcePath);
        cancellationToken.ThrowIfCancellationRequested();
        ApplySquareIconCanvas(img, size, request.IconBackground);
        WriteIcoWithPngNormalization(img, request.DestinationPath);
        return 0;
    }

    private static int ConvertIcoToRaster(ConversionRequest request, MagickFormat outFmt, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        using var collection = new MagickImageCollection();
        collection.Read(request.SourcePath);
        if (collection.Count == 0)
        {
            errorMessage = "ICO file contains no images.";
            return 2;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var best = SelectLargestFrame(collection);
        using var output = (MagickImage)best.Clone();
        output.Format = outFmt;
        ApplyQualityHints(output, outFmt);
        ApplyOutputBackground(output, outFmt, request.IconBackground);
        output.Write(request.DestinationPath);
        return 0;
    }

    private static MagickImage SelectLargestFrame(MagickImageCollection collection)
    {
        return (MagickImage)collection
            .OrderByDescending(m => (long)m.Width * m.Height)
            .ThenByDescending(m => m.Width)
            .First();
    }

    private static int ConvertIcoToIco(ConversionRequest request, CancellationToken cancellationToken, out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();
        var size = request.IcoSquareSizePixels;
        if (size is < 16 or > 256)
        {
            errorMessage = "ICO size must be between 16 and 256.";
            return 1;
        }

        using var collection = new MagickImageCollection();
        collection.Read(request.SourcePath);
        if (collection.Count == 0)
        {
            errorMessage = "ICO file contains no images.";
            return 2;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var best = SelectLargestFrame(collection);
        using var img = (MagickImage)best.Clone();
        ApplySquareIconCanvas(img, size, request.IconBackground);
        WriteIcoWithPngNormalization(img, request.DestinationPath);
        return 0;
    }

    private static void ApplySquareIconCanvas(MagickImage image, int size, IconBackgroundKind bg)
    {
        var magickBg = ToMagickBackground(bg);
        image.BackgroundColor = magickBg;
        image.Resize(new MagickGeometry($"{size}x{size}"));
        image.Extent((uint)size, (uint)size, Gravity.Center, magickBg);
    }

    private static void ApplyIcoColorTypeForWrite(MagickImage image)
    {
        image.ColorType = ColorType.TrueColorAlpha;
    }

    private static void WriteIcoWithPngNormalization(MagickImage image, string destinationPath)
    {
        ApplyIcoColorTypeForWrite(image);
        using var pngMs = new MemoryStream();
        image.Settings.SetDefine(MagickFormat.Png, "format", "png32");
        image.Format = MagickFormat.Png;
        image.Write(pngMs);
        pngMs.Position = 0;
        using var normalized = new MagickImage(pngMs);
        normalized.Settings.SetDefine(MagickFormat.Png, "format", "png32");
        normalized.Format = MagickFormat.Ico;
        normalized.ColorType = ColorType.TrueColorAlpha;
        normalized.Write(destinationPath);
    }

    private static MagickColor ToMagickBackground(IconBackgroundKind bg)
    {
        return bg switch
        {
            IconBackgroundKind.SolidWhite => MagickColors.White,
            IconBackgroundKind.SolidBlack => MagickColors.Black,
            IconBackgroundKind.Transparent => MagickColors.Transparent,
            _ => MagickColors.White
        };
    }
}
