using ImageConverter;

namespace ImageConverter.Shell;

internal static class ShellConversionDefaults
{
    private static readonly int[] IcoOutputSizeValues = [16, 32, 48, 64, 128, 256];

    internal static int GetIcoSquareSizePixels(AppSettings settings)
    {
        var idx = Math.Clamp(settings.IcoOutputSizeIndex, 0, IcoOutputSizeValues.Length - 1);
        return IcoOutputSizeValues[idx];
    }

    internal static IconBackgroundKind GetIconBackground(AppSettings settings) =>
        settings.SolidColorIndex switch
        {
            1 => IconBackgroundKind.SolidBlack,
            2 => IconBackgroundKind.Transparent,
            _ => IconBackgroundKind.SolidWhite
        };
}
