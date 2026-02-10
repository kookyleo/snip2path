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
irm https://github.com/kookyleo/snip2path/releases/latest/download/Snip2Path-win-x64.zip -OutFile $env:TEMP\s2p.zip; Expand-Archive $env:TEMP\s2p.zip -DestinationPath "$env:LOCALAPPDATA\Snip2Path" -Force; & "$env:LOCALAPPDATA\Snip2Path\Snip2Path.exe"
```

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

The captured image is saved as PNG in `%TEMP%` and the path is copied to clipboard.

## Build from Source

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```
dotnet build src/Snip2Path/Snip2Path.csproj
```

To publish both release variants:

```
publish.bat
```

## License

[Apache License 2.0](LICENSE)
