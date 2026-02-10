using System.Drawing;

namespace Snip2Path;

static class ScreenCapture
{
    /// <summary>
    /// Captures the entire virtual screen (all monitors).
    /// Returns the bitmap and the top-left offset of the virtual screen.
    /// </summary>
    public static (Bitmap Bitmap, Point Offset) CaptureVirtualScreen()
    {
        var virtualScreen = SystemInformation.VirtualScreen;
        var bitmap = new Bitmap(virtualScreen.Width, virtualScreen.Height);

        using (var g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(
                virtualScreen.Left,
                virtualScreen.Top,
                0, 0,
                virtualScreen.Size,
                CopyPixelOperation.SourceCopy);
        }

        return (bitmap, new Point(virtualScreen.Left, virtualScreen.Top));
    }
}
