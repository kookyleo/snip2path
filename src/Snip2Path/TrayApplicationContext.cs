using System.Drawing;

namespace Snip2Path;

sealed class TrayApplicationContext : ApplicationContext
{
    private const int HotkeyId = 1;

    private readonly NotifyIcon _trayIcon;
    private readonly HotkeyWindow _hotkeyWindow;
    private readonly HotkeySettings _settings;
    private bool _capturing;

    public TrayApplicationContext()
    {
        _settings = HotkeySettings.Load();

        _hotkeyWindow = new HotkeyWindow(OnHotkeyPressed);
        _hotkeyWindow.CreateHandle(new CreateParams
        {
            Parent = NativeMethods.HWND_MESSAGE
        });

        bool registered = NativeMethods.RegisterHotKey(
            _hotkeyWindow.Handle,
            HotkeyId,
            _settings.Modifiers | NativeMethods.MOD_NOREPEAT,
            _settings.VirtualKey);

        _trayIcon = new NotifyIcon
        {
            Icon = CreateDefaultIcon(),
            Text = $"Snip2Path ({_settings.ToDisplayString()})",
            Visible = true,
            ContextMenuStrip = CreateMenu()
        };

        _trayIcon.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
                OnHotkeyPressed();
        };

        if (!registered)
        {
            _trayIcon.ShowBalloonTip(3000, "Snip2Path",
                $"Failed to register hotkey {_settings.ToDisplayString()}. Another program may be using it.",
                ToolTipIcon.Warning);
        }
    }

    private ContextMenuStrip CreateMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add($"Capture ({_settings.ToDisplayString()})", null, (_, _) => OnHotkeyPressed());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Change Hotkey...", null, (_, _) => OnChangeHotkey());
        menu.Items.Add("About", null, (_, _) => OnAbout());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitApplication());
        return menu;
    }

    private void OnHotkeyPressed()
    {
        if (_capturing) return;
        _capturing = true;

        try
        {
            // 1. Capture full virtual screen
            var (screenshot, offset) = ScreenCapture.CaptureVirtualScreen();

            // 2. Pre-cache visible window rects
            var windowRects = WindowDetector.CaptureVisibleWindowRects();

            // 3. Show overlay for user interaction
            using var overlay = new OverlayForm(screenshot, offset, windowRects);
            var result = overlay.ShowDialog();

            if (result == DialogResult.OK && overlay.SelectedRegion is Rectangle region && !region.IsEmpty)
            {
                // 4. Save the selected region
                string? filePath = SnipResult.SaveRegion(screenshot, offset, region);

                if (filePath != null)
                {
                    // 5. Copy path to clipboard
                    Clipboard.SetText(filePath);
                    _trayIcon.ShowBalloonTip(2000, "Snip2Path",
                        $"Saved & copied:\n{filePath}",
                        ToolTipIcon.Info);
                }
            }

            screenshot.Dispose();
        }
        catch (Exception ex)
        {
            _trayIcon.ShowBalloonTip(3000, "Snip2Path Error",
                ex.Message, ToolTipIcon.Error);
        }
        finally
        {
            _capturing = false;
        }
    }

    private void OnChangeHotkey()
    {
        using var form = new ChangeHotkeyForm(_settings.Modifiers, _settings.VirtualKey);
        if (form.ShowDialog() != DialogResult.OK)
            return;

        int oldModifiers = _settings.Modifiers;
        int oldVirtualKey = _settings.VirtualKey;

        // Unregister old hotkey
        NativeMethods.UnregisterHotKey(_hotkeyWindow.Handle, HotkeyId);

        // Try to register new hotkey
        bool registered = NativeMethods.RegisterHotKey(
            _hotkeyWindow.Handle,
            HotkeyId,
            form.NewModifiers | NativeMethods.MOD_NOREPEAT,
            form.NewVirtualKey);

        if (registered)
        {
            _settings.Modifiers = form.NewModifiers;
            _settings.VirtualKey = form.NewVirtualKey;
            _settings.Save();
            UpdateHotkeyUI();
        }
        else
        {
            // Restore the old hotkey
            NativeMethods.RegisterHotKey(
                _hotkeyWindow.Handle,
                HotkeyId,
                oldModifiers | NativeMethods.MOD_NOREPEAT,
                oldVirtualKey);

            var attempted = new HotkeySettings { Modifiers = form.NewModifiers, VirtualKey = form.NewVirtualKey };
            MessageBox.Show(
                $"Failed to register hotkey {attempted.ToDisplayString()}.\nAnother program may be using it.",
                "Snip2Path",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void UpdateHotkeyUI()
    {
        string display = _settings.ToDisplayString();
        _trayIcon.Text = $"Snip2Path ({display})";

        // Update the Capture menu item text
        var menu = _trayIcon.ContextMenuStrip;
        if (menu != null && menu.Items.Count > 0)
        {
            menu.Items[0].Text = $"Capture ({display})";
        }
    }

    private static void OnAbout()
    {
        using var form = new AboutForm();
        form.ShowDialog();
    }

    private void ExitApplication()
    {
        NativeMethods.UnregisterHotKey(_hotkeyWindow.Handle, HotkeyId);
        _hotkeyWindow.DestroyHandle();
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NativeMethods.UnregisterHotKey(_hotkeyWindow.Handle, HotkeyId);
            _hotkeyWindow.DestroyHandle();
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        }
        base.Dispose(disposing);
    }

    private static Icon CreateDefaultIcon()
    {
        // Use system-reported small icon size (DPI-aware, e.g. 32x32 at 200%)
        var size = SystemInformation.SmallIconSize;

        string icoPath = Path.Combine(AppContext.BaseDirectory, "snip2path.ico");
        if (File.Exists(icoPath))
            return new Icon(icoPath, size);

        // Fallback
        var bmp = new Bitmap(size.Width, size.Height);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);
            using var fillBrush = new SolidBrush(Color.FromArgb(24, 100, 210));
            g.FillRectangle(fillBrush, 1, 1, size.Width - 2, size.Height - 2);
        }
        return Icon.FromHandle(bmp.GetHicon());
    }

    private sealed class HotkeyWindow : NativeWindow
    {
        private readonly Action _onHotkey;

        public HotkeyWindow(Action onHotkey) => _onHotkey = onHotkey;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_HOTKEY)
            {
                _onHotkey();
            }
            base.WndProc(ref m);
        }
    }
}
