using System.Diagnostics;
using System.Reflection;

namespace Snip2Path;

sealed class AboutForm : Form
{
    public AboutForm()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString(3) ?? "1.0.0";

        Text = "About Snip2Path";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var nameLabel = new Label
        {
            Text = "Snip2Path",
            Font = new System.Drawing.Font("Segoe UI", 16f, System.Drawing.FontStyle.Bold),
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 4)
        };

        var versionLabel = new Label
        {
            Text = $"Version {version}",
            AutoSize = true,
            Padding = new Padding(2, 0, 0, 8)
        };

        var descLabel = new Label
        {
            Text = "Capture a screen region and copy the saved image path to clipboard.",
            AutoSize = true,
            Padding = new Padding(2, 0, 0, 8),
            MaximumSize = new System.Drawing.Size(400, 0)
        };

        var githubLink = new LinkLabel
        {
            Text = "github.com/kookyleo/snip2path",
            AutoSize = true,
            Padding = new Padding(2, 0, 0, 16)
        };
        githubLink.LinkClicked += (_, _) =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/kookyleo/snip2path",
                UseShellExecute = true
            });
        };

        var okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            AutoSize = true
        };

        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        buttonPanel.Controls.Add(okButton);

        var layout = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(24),
            Dock = DockStyle.Fill
        };
        layout.Controls.Add(nameLabel);
        layout.Controls.Add(versionLabel);
        layout.Controls.Add(descLabel);
        layout.Controls.Add(githubLink);
        layout.Controls.Add(buttonPanel);

        Controls.Add(layout);
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        AcceptButton = okButton;
        CancelButton = okButton;
    }
}
