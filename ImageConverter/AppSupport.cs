using System.Diagnostics;

namespace ImageConverter;

internal static class AppSupport
{
    internal const string BuyMeACoffeeUrl = "https://buymeacoffee.com/mjayyab";

    internal static void OpenBuyMeACoffee(IWin32Window? owner)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = BuyMeACoffeeUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                owner,
                "Could not open your browser: " + ex.Message,
                "Support",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
