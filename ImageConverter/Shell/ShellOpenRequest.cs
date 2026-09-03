using ImageConverter;

namespace ImageConverter.Shell;

internal sealed class ShellOpenRequest
{
    internal string FolderPath { get; init; } = string.Empty;

    internal IReadOnlyCollection<string> SelectedImagePaths { get; init; } = [];

    internal static bool TryParse(string[] args, string flag, out ShellOpenRequest? request)
    {
        request = null;
        var flagIndex = ShellArguments.FindFlagIndex(args, flag);
        if (flagIndex < 0 || flagIndex + 1 >= args.Length)
        {
            return false;
        }

        var path = args[flagIndex + 1].Trim().Trim('"');
        try
        {
            path = Path.GetFullPath(path);
            if (Directory.Exists(path))
            {
                request = new ShellOpenRequest
                {
                    FolderPath = Path.TrimEndingDirectorySeparator(path)
                };
                return true;
            }

            if (!File.Exists(path) || !SupportedFormats.IsPreviewFile(path))
            {
                return false;
            }

            var folder = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                return false;
            }

            request = new ShellOpenRequest
            {
                FolderPath = Path.TrimEndingDirectorySeparator(folder),
                SelectedImagePaths = [path]
            };
            return true;
        }
        catch
        {
            return false;
        }
    }
}
