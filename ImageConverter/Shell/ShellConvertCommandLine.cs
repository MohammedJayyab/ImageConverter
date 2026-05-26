using ImageConverter;

namespace ImageConverter.Shell;

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

        if (!ShellArguments.TryGetTokenAfterFlag(args, Flag, out var flagIndex, out var formatToken))
        {
            return false;
        }

        if (!SupportedFormats.TryGetFormatIndexFromShellName(formatToken, out var formatIndex))
        {
            return false;
        }

        var paths = ShellArguments.ResolvePaths(args, flagIndex);
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
