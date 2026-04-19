namespace ImageConverter;

/// <summary>Maps UI format indices to file extensions and Magick.NET formats.</summary>
internal static class SupportedFormats
{
    public const int Count = 6;

    /// <summary>Output file extensions (JPEG uses .jpg).</summary>
    public static readonly string[] FileExtensions = [".jpg", ".png", ".bmp", ".gif", ".webp", ".ico"];

    public static string GetFileExtension(int formatIndex)
    {
        if (formatIndex < 0 || formatIndex >= Count)
        {
            return ".png";
        }

        return FileExtensions[formatIndex];
    }

    /// <summary>Builds destination path: same base name, extension from selected output format.</summary>
    public static string BuildDestinationPath(string sourceFilePath, string destinationFolder, int outputFormatIndex)
    {
        var ext = GetFileExtension(outputFormatIndex);
        var baseName = Path.GetFileNameWithoutExtension(sourceFilePath);
        return Path.Combine(destinationFolder, baseName + ext);
    }

    /// <summary>Returns the UI format index for this path’s extension, or false if unrecognized.</summary>
    public static bool TryGetFormatIndexForPath(string filePath, out int formatIndex)
    {
        formatIndex = -1;
        for (var i = 0; i < Count; i++)
        {
            if (FormatIndexMatchesExtension(filePath, i))
            {
                formatIndex = i;
                return true;
            }
        }

        return false;
    }

    /// <summary>True when the file extension matches the selected “Convert from” format (JPEG accepts .jpg and .jpeg).</summary>
    public static bool FormatIndexMatchesExtension(string filePath, int formatIndex)
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
            _ => false
        };
    }
}
