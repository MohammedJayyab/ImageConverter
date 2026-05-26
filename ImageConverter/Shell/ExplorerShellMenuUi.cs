namespace ImageConverter.Shell;

internal static class ExplorerShellMenuUi
{
    internal static void SyncFromSettings(
        bool enabled,
        string exePath,
        Action<string> setStatusMessage,
        ref bool attemptedHklmElevation)
    {
        try
        {
            if (!enabled)
            {
                ExplorerShellRegistry.Sync(false, exePath);
                return;
            }

            var result = ExplorerShellRegistry.Sync(true, exePath);

            if (ExplorerShellRegistry.NeedsHklmScaleRegistration())
            {
                RequestHklmElevation(exePath, setStatusMessage, ref attemptedHklmElevation);
                return;
            }

            if (!string.IsNullOrEmpty(result.Warning))
            {
                setStatusMessage(result.Warning);
            }
        }
        catch (Exception ex)
        {
            setStatusMessage("Converter To menu: " + ex.Message);
        }
    }

    internal static void Refresh(
        IWin32Window? owner,
        bool enabled,
        string exePath,
        Action<string> setStatusMessage,
        ref bool attemptedHklmElevation)
    {
        attemptedHklmElevation = false;
        SyncFromSettings(enabled, exePath, setStatusMessage, ref attemptedHklmElevation);

        if (ExplorerShellRegistry.NeedsHklmScaleRegistration())
        {
            MessageBox.Show(
                owner,
                "Scale 0.5x and Scale 2x must be written to the system registry (HKLM).\r\n\r\n" +
                "A Windows permission prompt should appear — choose Yes.\r\n\r\n" +
                "Then close all File Explorer windows and open a new one.",
                "Convert to — permission required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            RequestHklmElevation(exePath, setStatusMessage, ref attemptedHklmElevation);
            return;
        }

        setStatusMessage("Explorer menu refreshed. Close all File Explorer windows, then open a new one.");
        MessageBox.Show(
            owner,
            "Explorer menu updated.\r\n\r\n" +
            "Right-click an image → Convert to ▶ → formats, then a line, then Scale 0.5x and Scale 2x.\r\n\r\n" +
            "Close all File Explorer windows, then open a new one.",
            "Convert to",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private static void RequestHklmElevation(
        string exePath,
        Action<string> setStatusMessage,
        ref bool attemptedHklmElevation)
    {
        if (attemptedHklmElevation)
        {
            setStatusMessage(
                "Explorer menu: Scale 0.5x / 2x need administrator. Click Refresh Explorer menu and approve UAC.");
            return;
        }

        attemptedHklmElevation = true;

        if (ExplorerShellRegistry.IsProcessElevated())
        {
            ExplorerShellRegistry.Sync(true, exePath);
            ExplorerShellRegistry.NotifyAssociationChanged();
            setStatusMessage("Explorer menu updated (Scale 0.5x / 2x). Close File Explorer windows and open a new one.");
            return;
        }

        if (ShellHost.TryLaunchElevatedRegisterMenu())
        {
            setStatusMessage(
                "Approve the Windows permission prompt to add Scale 0.5x and Scale 2x to the Explorer menu.");
            return;
        }

        setStatusMessage(
            "Scale 0.5x / 2x are missing from the menu. Click Refresh Explorer menu and allow administrator access.");
    }
}
