namespace Snip2Path;

sealed class ChangeHotkeyForm : Form
{
    private readonly Label _hotkeyLabel;
    private readonly Button _okButton;

    private int _currentModifiers;
    private int _currentVirtualKey;

    public int NewModifiers => _currentModifiers;
    public int NewVirtualKey => _currentVirtualKey;

    public ChangeHotkeyForm(int currentModifiers, int currentVirtualKey)
    {
        _currentModifiers = currentModifiers;
        _currentVirtualKey = currentVirtualKey;

        Text = "Change Hotkey";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true;

        var promptLabel = new Label
        {
            Text = "Press a new hotkey combination:",
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 8)
        };

        _hotkeyLabel = new Label
        {
            Text = new HotkeySettings { Modifiers = currentModifiers, VirtualKey = currentVirtualKey }.ToDisplayString(),
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI", 14f, System.Drawing.FontStyle.Bold),
            Padding = new Padding(0, 0, 0, 12)
        };

        _okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            AutoSize = true,
            Margin = new Padding(0, 0, 8, 0)
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            AutoSize = true
        };

        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        buttonPanel.Controls.Add(cancelButton);
        buttonPanel.Controls.Add(_okButton);

        var layout = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(20),
            Dock = DockStyle.Fill
        };
        layout.Controls.Add(promptLabel);
        layout.Controls.Add(_hotkeyLabel);
        layout.Controls.Add(buttonPanel);

        Controls.Add(layout);
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        AcceptButton = _okButton;
        CancelButton = cancelButton;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        e.Handled = true;
        e.SuppressKeyPress = true;

        int modifiers = 0;
        if (e.Control) modifiers |= NativeMethods.MOD_CTRL;
        if (e.Alt) modifiers |= NativeMethods.MOD_ALT;
        if (e.Shift) modifiers |= NativeMethods.MOD_SHIFT;

        var key = e.KeyCode;
        bool isModifierOnly = key is Keys.ControlKey or Keys.ShiftKey or Keys.Menu
            or Keys.LControlKey or Keys.RControlKey
            or Keys.LShiftKey or Keys.RShiftKey
            or Keys.LMenu or Keys.RMenu;

        if (isModifierOnly)
        {
            var parts = new List<string>();
            if ((modifiers & NativeMethods.MOD_CTRL) != 0) parts.Add("Ctrl");
            if ((modifiers & NativeMethods.MOD_ALT) != 0) parts.Add("Alt");
            if ((modifiers & NativeMethods.MOD_SHIFT) != 0) parts.Add("Shift");
            if ((modifiers & NativeMethods.MOD_WIN) != 0) parts.Add("Win");
            _hotkeyLabel.Text = string.Join("+", parts) + "+...";
            _okButton.Enabled = false;
            return;
        }

        if (modifiers == 0)
            return;

        int vk = (int)key;
        _currentModifiers = modifiers;
        _currentVirtualKey = vk;

        _hotkeyLabel.Text = new HotkeySettings { Modifiers = modifiers, VirtualKey = vk }.ToDisplayString();
        _okButton.Enabled = true;

        base.OnKeyDown(e);
    }
}
