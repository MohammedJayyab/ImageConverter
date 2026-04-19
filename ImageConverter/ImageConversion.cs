using ImageMagick;

namespace ImageConverter;

/// <summary>Magick.NET conversion pipeline. Return codes: 0 success, 1 invalid arguments, 2 processing error.</summary>
internal static class ImageConversion
{
    public static int Convert(ConversionRequest request, out string? errorMessage, CancellationToken cancellationToken = default)
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
            var outFmt = MapToMagickFormat(request.OutputFormatIndex);
            var srcIsIco = Path.GetExtension(request.SourcePath).Equals(".ico", StringComparison.OrdinalIgnoreCase);
            var dstIsIco = outFmt == MagickFormat.Ico;

            if (srcIsIco && dstIsIco)
            {
                return ConvertIcoToIco(request, cancellationToken, out errorMessage);
            }

            if (srcIsIco && !dstIsIco)
            {
                return ConvertIcoToRaster(request, outFmt, cancellationToken, out errorMessage);
            }

            if (!srcIsIco && dstIsIco)
            {
                return ConvertRasterToIco(request, cancellationToken, out errorMessage);
            }

            return ConvertRasterToRaster(request, outFmt, cancellationToken, out errorMessage);
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
        FlattenIfNeededForOpaque(img, outFmt);
        img.Write(request.DestinationPath);
        return 0;
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
        FlattenIfNeededForOpaque(output, outFmt);
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

    /// <summary>
    /// Fits the image inside a square, then centers on a size×size canvas using a solid letterbox color.
    /// </summary>
    private static void ApplySquareIconCanvas(MagickImage image, int size, IconBackgroundKind bg)
    {
        var magickBg = ToMagickBackground(bg);
        // Match Extent so Resize/composite steps do not default to an opaque white background.
        image.BackgroundColor = magickBg;
        // Unadorned WxH: fit inside the box, preserve aspect ratio (letterbox via Extent).
        image.Resize(new MagickGeometry($"{size}x{size}"));
        image.Extent((uint)size, (uint)size, Gravity.Center, magickBg);
    }

    /// <summary>
    /// ImageMagick may write a palette ICO and discard alpha (IM #6361). Force TrueColorAlpha, then normalize via PNG32 so the ICO coder sees full RGBA.
    /// </summary>
    private static void ApplyIcoColorTypeForWrite(MagickImage image)
    {
        image.ColorType = ColorType.TrueColorAlpha;
    }

    /// <summary>
    /// ICO encoder can still quantize poorly; round-trip PNG32 preserves RGBA before writing ICO.
    /// </summary>
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
            _ => MagickColors.White
        };
    }
}
