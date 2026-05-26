namespace ImageConverter;

internal static class ShellConvertPaths
{
    internal static List<string> CollectFromArgs(string[] args, int startIndex)
    {
        var paths = new List<string>();
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.Equals("--", StringComparison.Ordinal))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(arg))
            {
                continue;
            }

            paths.Add(arg.Trim().Trim('"'));
        }

        return paths;
    }

    internal static List<string> NormalizeExistingImageFiles(IEnumerable<string> paths) =>
        paths
            .Where(p => File.Exists(p))
            .Where(SupportedFormats.IsPreviewFile)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>Explorer may pass the file path before or after flags; %1 / %* expansion varies by Windows version.</summary>
    internal static List<string> CollectExistingImagePathsFromArgs(string[] args)
    {
        var paths = new List<string>();
        foreach (var arg in args)
        {
            if (string.IsNullOrWhiteSpace(arg) || arg.StartsWith('-'))
            {
                continue;
            }

            if (SupportedFormats.TryGetFormatIndexFromShellName(arg, out _))
            {
                continue;
            }

            var path = arg.Trim().Trim('"');
            if (File.Exists(path) && SupportedFormats.IsPreviewFile(path))
            {
                paths.Add(path);
            }
        }

        return paths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}
