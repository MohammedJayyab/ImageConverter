using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using ImageMagick;

namespace ImageConverter;

internal static class ImageResize
{
    internal static readonly double[] SupportedScaleFactors = [0.5, 0.75, 2, 4];

    internal static bool IsSupportedScaleFactor(double scaleFactor) =>
        SupportedScaleFactors.Any(s => Math.Abs(s - scaleFactor) < 0.001);

    internal static bool IsResizablePath(string filePath) => SupportedFormats.IsResizableFile(filePath);

    internal static string BuildScaledOutputPath(string sourcePath) =>
        BuildScaledOutputPath(sourcePath, Path.GetDirectoryName(sourcePath) ?? string.Empty);

    internal static string BuildScaledOutputPath(string sourcePath, string outputFolder) =>
        Path.Combine(
            outputFolder,
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

        if (IsSvgPath(sourcePath))
        {
            return ResizeSvg(sourcePath, dstFull, scaleFactor, null, null, cancellationToken, out errorMessage);
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

    internal static int ResizeTo(
        string sourcePath,
        string destinationPath,
        int width,
        int height,
        CancellationToken cancellationToken,
        out string? errorMessage)
    {
        errorMessage = null;
        cancellationToken.ThrowIfCancellationRequested();

        if (width < 1 || height < 1)
        {
            errorMessage = "Width and height must be greater than zero.";
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

        if (IsSvgPath(sourcePath))
        {
            return ResizeSvg(sourcePath, dstFull, null, width, height, cancellationToken, out errorMessage);
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var img = new MagickImage(sourcePath);
            img.FilterType = FilterType.Lanczos;
            img.Resize(new MagickGeometry($"{width}x{height}!"));

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

    private static bool IsSvgPath(string path) =>
        Path.GetExtension(path).Equals(".svg", StringComparison.OrdinalIgnoreCase);

    private static int ResizeSvg(
        string sourcePath,
        string destinationPath,
        double? scaleFactor,
        int? requestedWidth,
        int? requestedHeight,
        CancellationToken cancellationToken,
        out string? errorMessage)
    {
        errorMessage = null;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var readerSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };

            XDocument document;
            using (var reader = XmlReader.Create(sourcePath, readerSettings))
            {
                document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
            }

            var root = document.Root;
            if (root is null || !root.Name.LocalName.Equals("svg", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "The file does not contain a valid SVG root element.";
                return 2;
            }

            GetSvgViewportSize(root, out var currentWidth, out var currentHeight, out var hasViewBox);
            var width = requestedWidth ?? Math.Max(1, (int)Math.Round(currentWidth * scaleFactor!.Value));
            var height = requestedHeight ?? Math.Max(1, (int)Math.Round(currentHeight * scaleFactor!.Value));

            if (!hasViewBox)
            {
                root.SetAttributeValue(
                    "viewBox",
                    $"0 0 {FormatSvgNumber(currentWidth)} {FormatSvgNumber(currentHeight)}");
            }

            root.SetAttributeValue("width", width.ToString(CultureInfo.InvariantCulture));
            root.SetAttributeValue("height", height.ToString(CultureInfo.InvariantCulture));
            UpdateInlineSvgDimensions(root, width, height);

            cancellationToken.ThrowIfCancellationRequested();
            var destinationFolder = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            document.Save(destinationPath, SaveOptions.DisableFormatting);
            return 0;
        }
        catch (XmlException ex)
        {
            errorMessage = ex.Message;
            return 2;
        }
        catch (InvalidOperationException ex)
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

    private static void GetSvgViewportSize(
        XElement root,
        out double width,
        out double height,
        out bool hasViewBox)
    {
        var hasWidth = TryGetSvgLength(root, "width", out width);
        var hasHeight = TryGetSvgLength(root, "height", out height);
        hasViewBox = TryGetSvgViewBoxSize(root, out var viewBoxWidth, out var viewBoxHeight);

        if (!hasWidth && hasHeight && hasViewBox)
        {
            width = height * viewBoxWidth / viewBoxHeight;
        }
        else if (hasWidth && !hasHeight && hasViewBox)
        {
            height = width * viewBoxHeight / viewBoxWidth;
        }
        else
        {
            if (!hasWidth)
            {
                width = hasViewBox ? viewBoxWidth : 300;
            }

            if (!hasHeight)
            {
                height = hasViewBox ? viewBoxHeight : 150;
            }
        }
    }

    private static bool TryGetSvgLength(XElement root, string propertyName, out double pixels)
    {
        var style = root.Attribute("style")?.Value;
        if (!string.IsNullOrWhiteSpace(style))
        {
            var declarations = style.Split(';', StringSplitOptions.RemoveEmptyEntries);
            for (var i = declarations.Length - 1; i >= 0; i--)
            {
                var separatorIndex = declarations[i].IndexOf(':');
                if (separatorIndex < 0
                    || !declarations[i][..separatorIndex].Trim().Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var value = declarations[i][(separatorIndex + 1)..].Trim();
                var importantIndex = value.IndexOf("!important", StringComparison.OrdinalIgnoreCase);
                if (importantIndex >= 0)
                {
                    value = value[..importantIndex].TrimEnd();
                }

                if (TryParseSvgLength(value, out pixels))
                {
                    return true;
                }

                break;
            }
        }

        return TryParseSvgLength(root.Attribute(propertyName)?.Value, out pixels);
    }

    private static void UpdateInlineSvgDimensions(XElement root, int width, int height)
    {
        var styleAttribute = root.Attribute("style");
        if (styleAttribute is null
            || (!HasInlineStyleProperty(styleAttribute.Value, "width")
                && !HasInlineStyleProperty(styleAttribute.Value, "height")))
        {
            return;
        }

        var style = styleAttribute.Value.TrimEnd();
        var separator = style.EndsWith(';') ? " " : "; ";
        styleAttribute.Value =
            $"{style}{separator}width: {width}px !important; height: {height}px !important";
    }

    private static bool HasInlineStyleProperty(string style, string propertyName)
    {
        foreach (var declaration in style.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var separatorIndex = declaration.IndexOf(':');
            if (separatorIndex >= 0
                && declaration[..separatorIndex].Trim().Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetSvgViewBoxSize(XElement root, out double width, out double height)
    {
        width = 0;
        height = 0;
        var value = root.Attribute("viewBox")?.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Replace(',', ' ')
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == 4
            && double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out width)
            && double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out height)
            && width > 0
            && height > 0;
    }

    private static bool TryParseSvgLength(string? text, out double pixels)
    {
        pixels = 0;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var value = text.Trim();
        var multiplier = 1d;
        var units = new (string Suffix, double Multiplier)[]
        {
            ("px", 1),
            ("in", 96),
            ("cm", 96 / 2.54),
            ("mm", 96 / 25.4),
            ("q", 96 / 101.6),
            ("pt", 96 / 72),
            ("pc", 16)
        };

        foreach (var unit in units)
        {
            if (!value.EndsWith(unit.Suffix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            value = value[..^unit.Suffix.Length].TrimEnd();
            multiplier = unit.Multiplier;
            break;
        }

        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
            && number > 0
            && double.IsFinite(number)
            && (pixels = number * multiplier) > 0
            && double.IsFinite(pixels);
    }

    private static string FormatSvgNumber(double value) =>
        value.ToString("0.###", CultureInfo.InvariantCulture);
}
