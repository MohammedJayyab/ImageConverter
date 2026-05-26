namespace ImageConverter.Shell;

internal static class ShellArguments
{
    internal static int FindFlagIndex(string[] args, string flag)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(flag, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    internal static bool TryGetTokenAfterFlag(string[] args, string flag, out int flagIndex, out string token)
    {
        flagIndex = FindFlagIndex(args, flag);
        token = string.Empty;
        if (flagIndex < 0 || flagIndex + 1 >= args.Length)
        {
            return false;
        }

        token = args[flagIndex + 1];
        if (token.Equals("--", StringComparison.Ordinal) || token.StartsWith('-'))
        {
            return false;
        }

        return true;
    }

    internal static List<string> ResolvePaths(string[] args, int flagIndex)
    {
        var paths = ShellConvertPaths.CollectExistingImagePathsFromArgs(args);
        if (paths.Count == 0)
        {
            paths = ShellConvertPaths.CollectFromArgs(args, flagIndex + 2);
        }

        return paths;
    }

    internal static bool ContainsFlag(string[] args, string flag) =>
        FindFlagIndex(args, flag) >= 0;
}
