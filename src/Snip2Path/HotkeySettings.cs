using System.Text.Json;

namespace Snip2Path;

sealed class HotkeySettings
{
    private static readonly string SettingsPath =
        Path.Combine(AppContext.BaseDirectory, "hotkey.json");

    public int Modifiers { get; set; } = NativeMethods.MOD_ALT | NativeMethods.MOD_SHIFT;
    public int VirtualKey { get; set; } = 0x53; // VK_S

    public static HotkeySettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<HotkeySettings>(json);
                if (settings != null)
                    return settings;
            }
        }
        catch
        {
            // Corrupted file — fall back to defaults
        }

        return new HotkeySettings();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
    }

    public string ToDisplayString()
    {
        var parts = new List<string>();

        if ((Modifiers & NativeMethods.MOD_CTRL) != 0) parts.Add("Ctrl");
        if ((Modifiers & NativeMethods.MOD_ALT) != 0) parts.Add("Alt");
        if ((Modifiers & NativeMethods.MOD_SHIFT) != 0) parts.Add("Shift");
        if ((Modifiers & NativeMethods.MOD_WIN) != 0) parts.Add("Win");

        parts.Add(KeyNameFromVirtualKey(VirtualKey));

        return string.Join("+", parts);
    }

    private static string KeyNameFromVirtualKey(int vk)
    {
        // Letters A-Z
        if (vk >= 0x41 && vk <= 0x5A)
            return ((char)vk).ToString();

        // Digits 0-9
        if (vk >= 0x30 && vk <= 0x39)
            return ((char)vk).ToString();

        // F1-F12
        if (vk >= 0x70 && vk <= 0x7B)
            return $"F{vk - 0x70 + 1}";

        return $"0x{vk:X2}";
    }
}
