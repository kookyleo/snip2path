using System.Drawing;
using System.Drawing.Drawing2D;

namespace Snip2Path;

sealed class OverlayForm : Form
{
    private readonly Bitmap _screenshot;
    private readonly Point _offset;
    private readonly List<Rectangle> _windowRects;

    // Dragging state
    private bool _isDragging;
    private Point _dragStart;
    private Rectangle _currentSelection;

    // Window hover highlight
    private Rectangle _hoveredWindow;

    /// <summary>
    /// The selected region in screen coordinates after the user finishes selection.
    /// </summary>
    public Rectangle? SelectedRegion { get; private set; }

    public OverlayForm(Bitmap screenshot, Point offset, List<Rectangle> windowRects)
    {
        _screenshot = screenshot;
        _offset = offset;
        _windowRects = windowRects;

        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        Cursor = Cursors.Cross;

        // Cover the entire virtual screen
        var virtualScreen = SystemInformation.VirtualScreen;
        Location = new Point(virtualScreen.Left, virtualScreen.Top);
        Size = new Size(virtualScreen.Width, virtualScreen.Height);

        // Double buffering for smooth painting
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

        KeyPreview = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;

        // Draw frozen screenshot as background
        g.DrawImage(_screenshot, 0, 0);

        // Semi-transparent dark overlay
        using (var dimBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
        {
            g.FillRectangle(dimBrush, ClientRectangle);
        }

        Rectangle highlightRect;

        if (_isDragging && _currentSelection.Width > 0 && _currentSelection.Height > 0)
        {
            highlightRect = ScreenToClient(_currentSelection);
        }
        else if (!_hoveredWindow.IsEmpty)
        {
            highlightRect = ScreenToClient(_hoveredWindow);
        }
        else
        {
            return;
        }

        // "Cut out" the selected/hovered region to show original brightness
        g.SetClip(highlightRect);
        g.DrawImage(_screenshot, 0, 0);
        g.ResetClip();

        // Draw border around selection
        using var borderPen = new Pen(Color.FromArgb(0, 120, 212), 2);
        g.DrawRectangle(borderPen, highlightRect);

        // Draw size label
        var sizeRect = _isDragging ? _currentSelection : _hoveredWindow;
        string sizeText = $"{sizeRect.Width} x {sizeRect.Height}";
        using var font = new Font("Segoe UI", 10f);
        var textSize = g.MeasureString(sizeText, font);

        int labelX = highlightRect.Left;
        int labelY = highlightRect.Bottom + 4;

        // Keep label on screen
        if (labelY + (int)textSize.Height > ClientRectangle.Bottom)
            labelY = highlightRect.Top - (int)textSize.Height - 4;

        using var bgBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
        g.FillRectangle(bgBrush, labelX, labelY, textSize.Width + 8, textSize.Height + 4);
        using var textBrush = new SolidBrush(Color.White);
        g.DrawString(sizeText, font, textBrush, labelX + 4, labelY + 2);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        if (e.Button == MouseButtons.Left)
        {
            _isDragging = true;
            _dragStart = ClientToScreen(e.Location);
            _currentSelection = Rectangle.Empty;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_isDragging)
        {
            var current = ClientToScreen(e.Location);
            _currentSelection = MakeRect(_dragStart, current);
            Invalidate();
        }
        else
        {
            // Window hover detection
            var screenPt = ClientToScreen(e.Location);
            var hit = WindowDetector.HitTest(_windowRects, screenPt);
            var newHover = hit ?? Rectangle.Empty;

            if (newHover != _hoveredWindow)
            {
                _hoveredWindow = newHover;
                Invalidate();
            }
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && _isDragging)
        {
            _isDragging = false;
            var endPoint = ClientToScreen(e.Location);
            var selection = MakeRect(_dragStart, endPoint);

            if (selection.Width > 3 && selection.Height > 3)
            {
                // User dragged a rectangle
                SelectedRegion = selection;
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (!_hoveredWindow.IsEmpty)
            {
                // User clicked on a window (tiny drag = click)
                SelectedRegion = _hoveredWindow;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                // Too small, reset
                _currentSelection = Rectangle.Empty;
                Invalidate();
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    private Point ClientToScreen(Point clientPoint)
    {
        return new Point(
            clientPoint.X + Location.X,
            clientPoint.Y + Location.Y);
    }

    private Rectangle ScreenToClient(Rectangle screenRect)
    {
        return new Rectangle(
            screenRect.X - Location.X,
            screenRect.Y - Location.Y,
            screenRect.Width,
            screenRect.Height);
    }

    private static Rectangle MakeRect(Point a, Point b)
    {
        int x = Math.Min(a.X, b.X);
        int y = Math.Min(a.Y, b.Y);
        int w = Math.Abs(a.X - b.X);
        int h = Math.Abs(a.Y - b.Y);
        return new Rectangle(x, y, w, h);
    }
}
