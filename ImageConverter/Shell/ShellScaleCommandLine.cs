using System.Globalization;
using ImageConverter;

namespace ImageConverter.Shell;

internal sealed class ShellScaleCommandLine
{
    private const string Flag = "--shell-scale";

    public double ScaleFactor { get; init; }

    public IReadOnlyList<string> SourcePaths { get; init; } = [];

    public static bool TryParse(string[] args, out ShellScaleCommandLine? parsed)
    {
        parsed = null;
        if (args is null || args.Length < 3)
        {
            return false;
        }

        if (!ShellArguments.TryGetTokenAfterFlag(args, Flag, out var flagIndex, out var scaleToken))
        {
            return false;
        }

        if (!TryParseScaleFactor(scaleToken, out var scaleFactor))
        {
            return false;
        }

        var paths = ShellArguments.ResolvePaths(args, flagIndex);
        if (paths.Count == 0)
        {
            return false;
        }

        parsed = new ShellScaleCommandLine
        {
            ScaleFactor = scaleFactor,
            SourcePaths = paths
        };
        return true;
    }

    internal static bool TryParseScaleFactor(string token, out double scaleFactor)
    {
        scaleFactor = 0;
        if (string.Equals(token, "0.5", StringComparison.Ordinal))
        {
            scaleFactor = 0.5;
            return ImageResize.IsSupportedScaleFactor(scaleFactor);
        }

        if (string.Equals(token, "2", StringComparison.Ordinal))
        {
            scaleFactor = 2;
            return ImageResize.IsSupportedScaleFactor(scaleFactor);
        }

        if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out scaleFactor))
        {
            return false;
        }

        return ImageResize.IsSupportedScaleFactor(scaleFactor);
    }
}
