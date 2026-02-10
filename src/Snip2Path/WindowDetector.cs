using System.Drawing;
using System.Runtime.InteropServices;

namespace Snip2Path;

static class WindowDetector
{
    /// <summary>
    /// Enumerates all visible, non-minimized, non-cloaked top-level windows
    /// and returns their visible rectangles, sorted by area ascending (smallest first).
    /// This allows hit-testing to prefer smaller (more specific) windows.
    /// </summary>
    public static List<Rectangle> CaptureVisibleWindowRects()
    {
        var rects = new List<Rectangle>();

        NativeMethods.EnumWindows((hWnd, _) =>
        {
            if (!NativeMethods.IsWindowVisible(hWnd))
                return true;

            if (NativeMethods.IsIconic(hWnd))
                return true;

            // Skip tool windows
            int exStyle = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE);
            if ((exStyle & NativeMethods.WS_EX_TOOLWINDOW) != 0)
                return true;

            // Skip cloaked windows (e.g. hidden UWP apps)
            int cloaked = 0;
            NativeMethods.DwmGetWindowAttribute(hWnd, NativeMethods.DWMWA_CLOAKED,
                out cloaked, Marshal.SizeOf<int>());
            if (cloaked != 0)
                return true;

            // Prefer DWM extended frame bounds for accurate visible area
            Rectangle rect;
            NativeMethods.RECT dwmRect;
            int hr = NativeMethods.DwmGetWindowAttribute(hWnd, NativeMethods.DWMWA_EXTENDED_FRAME_BOUNDS,
                out dwmRect, Marshal.SizeOf<NativeMethods.RECT>());

            if (hr == 0)
            {
                rect = dwmRect.ToRectangle();
            }
            else
            {
                NativeMethods.GetWindowRect(hWnd, out var winRect);
                rect = winRect.ToRectangle();
            }

            // Skip zero-size windows
            if (rect.Width > 0 && rect.Height > 0)
            {
                rects.Add(rect);
            }

            return true;
        }, IntPtr.Zero);

        // Sort by area ascending so smallest windows are checked first during hit-test
        rects.Sort((a, b) => (a.Width * a.Height).CompareTo(b.Width * b.Height));

        return rects;
    }

    /// <summary>
    /// Finds the smallest window rectangle that contains the given screen point.
    /// </summary>
    public static Rectangle? HitTest(List<Rectangle> windowRects, Point screenPoint)
    {
        foreach (var rect in windowRects)
        {
            if (rect.Contains(screenPoint))
                return rect;
        }
        return null;
    }
}
