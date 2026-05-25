namespace ImageConverter;

internal static class FolderPicker
{
    internal static bool TryPick(IWin32Window? owner, string title, string? initialDirectory, out string? folderPath)
    {
        folderPath = null;

        try
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = title,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
            };

            if (TryGetFilesystemFolder(initialDirectory, out var initial))
            {
                dialog.SelectedPath = initial;
            }

            if (dialog.ShowDialog(owner) != DialogResult.OK)
            {
                return false;
            }

            if (!TryGetFilesystemFolder(dialog.SelectedPath, out var resolved))
            {
                return false;
            }

            folderPath = resolved;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryGetFilesystemFolder(string? path, out string fullPath)
    {
        fullPath = string.Empty;
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        try
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            fullPath = Path.GetFullPath(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static string GetDefaultBrowseFolder()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        if (Directory.Exists(desktop))
        {
            return desktop;
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    }
}
