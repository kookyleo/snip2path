# Snip2Path

Lightweight Windows screen-capture tool that lives in the system tray. Snip a region (or click a window), save it as PNG, and get the file path copied to your clipboard instantly.

## Features

- **Global hotkey** (default `Alt+Shift+S`) or **left-click** the tray icon to capture
- **Drag** to select a region, or **click** a window to capture it whole
- Saves PNG to `%TEMP%` and copies the full path to clipboard
- **Customizable hotkey** with persistent settings
- Multi-monitor & per-monitor DPI aware

## Install

### One-liner (PowerShell)

```powershell
$s = if (Test-Path "$env:ProgramFiles\dotnet\shared\Microsoft.WindowsDesktop.App\8.*") { '-compact' } else { '' }; irm "https://github.com/kookyleo/snip2path/releases/latest/download/Snip2Path-win-x64$s.zip" -OutFile $env:TEMP\s2p.zip; Expand-Archive $env:TEMP\s2p.zip "$env:LOCALAPPDATA\Snip2Path" -Force; $ws=New-Object -ComObject WScript.Shell; $sc=$ws.CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Snip2Path.lnk"); $sc.TargetPath="$env:LOCALAPPDATA\Snip2Path\Snip2Path.exe"; $sc.WorkingDirectory="$env:LOCALAPPDATA\Snip2Path"; $sc.Save(); & "$env:LOCALAPPDATA\Snip2Path\Snip2Path.exe"
```

This will:
- Auto-detect .NET 8 Desktop Runtime: download compact (~96 KB) if present, full (~63 MB) otherwise
- Install to `%LOCALAPPDATA%\Snip2Path`
- Create a Start Menu shortcut
- Launch the app

### Manual

1. Go to [Releases](https://github.com/kookyleo/snip2path/releases/latest)
2. Download one of:
   - **Snip2Path-win-x64.zip** (~69 MB) -- self-contained, runs anywhere
   - **Snip2Path-win-x64-compact.zip** (~180 KB) -- requires [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
3. Extract and run `Snip2Path.exe`

## Usage

| Action | Result |
|---|---|
| `Alt+Shift+S` (or left-click tray icon) | Start capture |
| Drag a rectangle | Capture region |
| Click a window | Capture that window |
| Right-click / `Esc` | Cancel capture |
| Tray menu > Change Hotkey... | Customize the hotkey |
| Tray menu > Start with Windows | Toggle launch on login |

The captured image is saved as PNG in `%TEMP%` and the path is copied to clipboard.

## Build from Source

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```
dotnet build src/Snip2Path/Snip2Path.csproj
```

Releases are built automatically via GitHub Actions when a version tag is pushed (`git tag v1.0.0 && git push --tags`).

## License

[Apache License 2.0](LICENSE)
