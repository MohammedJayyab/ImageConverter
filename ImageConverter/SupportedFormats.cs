namespace ImageConverter;

internal static class SupportedFormats
{
    internal const int Count = 8;

    internal const int IcoFormatIndex = 5;

    internal const int SvgFormatIndex = 6;

    internal const int PdfFormatIndex = 7;

    private static readonly string[] FileExtensions = [".jpg", ".png", ".bmp", ".gif", ".webp", ".ico", ".svg", ".pdf"];

    private static readonly string[] FormatLabels =
    [
        "JPEG (.jpg / .jpeg)",
        "PNG (.png)",
        "BMP (.bmp)",
        "GIF (.gif)",
        "WEBP (.webp)",
        "ICO (.ico)",
        "SVG (.svg)",
        "PDF (.pdf)"
    ];

    private static readonly HashSet<string> PreviewExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".ico", ".svg", ".pdf"
    };

    private static readonly HashSet<string> ResizableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".ico", ".svg"
    };

    internal static string GetFormatLabel(int formatIndex)
    {
        if (formatIndex < 0 || formatIndex >= FormatLabels.Length)
        {
            return "Unknown";
        }

        return FormatLabels[formatIndex];
    }

    internal static bool SupportsTransparency(int formatIndex) =>
        formatIndex is 1 or 3 or 4 or 5 or 6;

    internal static bool IsPreviewFile(string filePath)
    {
        var ext = Path.GetExtension(filePath);
        return !string.IsNullOrEmpty(ext) && PreviewExtensions.Contains(ext);
    }

    internal static bool IsResizableFile(string filePath)
    {
        var ext = Path.GetExtension(filePath);
        return !string.IsNullOrEmpty(ext) && ResizableExtensions.Contains(ext);
    }

    internal static string GetFileExtension(int formatIndex)
    {
        if (formatIndex < 0 || formatIndex >= Count)
        {
            return ".png";
        }

        return FileExtensions[formatIndex];
    }

    internal static string BuildOutputPath(string sourceFilePath, int outputFormatIndex)
    {
        var directory = Path.GetDirectoryName(sourceFilePath) ?? string.Empty;
        var ext = GetFileExtension(outputFormatIndex);
        var baseName = Path.GetFileNameWithoutExtension(sourceFilePath);
        return Path.Combine(directory, baseName + ext);
    }

    internal static bool TryGetFormatIndexFromShellName(string shellName, out int formatIndex)
    {
        formatIndex = shellName.Trim().ToLowerInvariant() switch
        {
            "jpg" or "jpeg" => 0,
            "png" => 1,
            "bmp" => 2,
            "gif" => 3,
            "webp" => 4,
            "ico" => 5,
            "svg" => 6,
            "pdf" => 7,
            _ => -1
        };

        return formatIndex >= 0;
    }

    internal static bool FormatIndexMatchesExtension(string filePath, int formatIndex)
    {
        var ext = Path.GetExtension(filePath).Trim();
        if (string.IsNullOrEmpty(ext))
        {
            return false;
        }

        return formatIndex switch
        {
            0 => ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase),
            1 => ext.Equals(".png", StringComparison.OrdinalIgnoreCase),
            2 => ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase),
            3 => ext.Equals(".gif", StringComparison.OrdinalIgnoreCase),
            4 => ext.Equals(".webp", StringComparison.OrdinalIgnoreCase),
            5 => ext.Equals(".ico", StringComparison.OrdinalIgnoreCase),
            6 => ext.Equals(".svg", StringComparison.OrdinalIgnoreCase),
            7 => ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}
