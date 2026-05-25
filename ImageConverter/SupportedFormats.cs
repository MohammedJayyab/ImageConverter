namespace ImageConverter;

internal static class SupportedFormats
{
    internal const int Count = 8;

    internal const int IcoFormatIndex = 5;

    internal const int SvgFormatIndex = 6;

    internal const int PdfFormatIndex = 7;

    private static readonly string[] FileExtensions = [".jpg", ".png", ".bmp", ".gif", ".webp", ".ico", ".svg", ".pdf"];

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
