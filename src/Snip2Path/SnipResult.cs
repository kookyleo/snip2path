using System.Drawing;
using System.Drawing.Imaging;

namespace Snip2Path;

static class SnipResult
{
    /// <summary>
    /// Crops the given region from the screenshot and saves it as PNG.
    /// The region is in screen coordinates; offset is the virtual screen origin.
    /// Returns the saved file path, or null on failure.
    /// </summary>
    public static string? SaveRegion(Bitmap screenshot, Point offset, Rectangle screenRegion)
    {
        // Convert screen coordinates to bitmap coordinates
        var bitmapRegion = new Rectangle(
            screenRegion.X - offset.X,
            screenRegion.Y - offset.Y,
            screenRegion.Width,
            screenRegion.Height);

        // Clamp to bitmap bounds
        bitmapRegion.Intersect(new Rectangle(0, 0, screenshot.Width, screenshot.Height));

        if (bitmapRegion.Width <= 0 || bitmapRegion.Height <= 0)
            return null;

        using var cropped = screenshot.Clone(bitmapRegion, screenshot.PixelFormat);

        string fileName = $"snip2path_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";
        string filePath = Path.Combine(Path.GetTempPath(), fileName);

        cropped.Save(filePath, ImageFormat.Png);
        return filePath;
    }
}
