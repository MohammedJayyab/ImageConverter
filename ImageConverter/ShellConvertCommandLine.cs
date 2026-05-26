namespace ImageConverter;

internal sealed class ShellConvertCommandLine
{
    private const string Flag = "--shell-convert";

    public int OutputFormatIndex { get; init; }

    public IReadOnlyList<string> SourcePaths { get; init; } = [];

    public static bool TryParse(string[] args, out ShellConvertCommandLine? parsed)
    {
        parsed = null;
        if (args is null || args.Length < 3)
        {
            return false;
        }

        var flagIndex = -1;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(Flag, StringComparison.OrdinalIgnoreCase))
            {
                flagIndex = i;
                break;
            }
        }

        if (flagIndex < 0 || flagIndex + 1 >= args.Length)
        {
            return false;
        }

        var formatToken = args[flagIndex + 1];
        if (formatToken.Equals("--", StringComparison.Ordinal) || formatToken.StartsWith('-'))
        {
            return false;
        }

        if (!SupportedFormats.TryGetFormatIndexFromShellName(formatToken, out var formatIndex))
        {
            return false;
        }

        var paths = ShellConvertPaths.CollectExistingImagePathsFromArgs(args);
        if (paths.Count == 0)
        {
            paths = ShellConvertPaths.CollectFromArgs(args, flagIndex + 2);
        }

        if (paths.Count == 0)
        {
            return false;
        }

        parsed = new ShellConvertCommandLine
        {
            OutputFormatIndex = formatIndex,
            SourcePaths = paths
        };
        return true;
    }
}
