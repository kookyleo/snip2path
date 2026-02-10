# Snip2Path

轻量级 Windows 截图工具，常驻系统托盘。截取屏幕区域（或点击窗口），保存为 PNG，文件路径自动复制到剪贴板。

## 功能

- **全局快捷键**（默认 `Alt+Shift+S`）或**左键点击**托盘图标触发截图
- **拖拽**选区截图，或**点击**窗口整窗截图
- PNG 保存到 `%TEMP%`，完整路径自动复制到剪贴板
- **自定义快捷键**，设置持久化
- 多显示器 & 高 DPI 支持

## 安装

### 一条命令（PowerShell）

```powershell
$s = if (Test-Path "$env:ProgramFiles\dotnet\shared\Microsoft.WindowsDesktop.App\8.*") { '-compact' } else { '' }; irm "https://github.com/kookyleo/snip2path/releases/latest/download/Snip2Path-win-x64$s.zip" -OutFile $env:TEMP\s2p.zip; Expand-Archive $env:TEMP\s2p.zip "$env:LOCALAPPDATA\Snip2Path" -Force; $ws=New-Object -ComObject WScript.Shell; $sc=$ws.CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Snip2Path.lnk"); $sc.TargetPath="$env:LOCALAPPDATA\Snip2Path\Snip2Path.exe"; $sc.WorkingDirectory="$env:LOCALAPPDATA\Snip2Path"; $sc.Save(); & "$env:LOCALAPPDATA\Snip2Path\Snip2Path.exe"
```

该命令会：
- 自动检测 .NET 8 Desktop Runtime：有则下载精简版（~96 KB），无则下载完整版（~63 MB）
- 安装到 `%LOCALAPPDATA%\Snip2Path`
- 创建开始菜单快捷方式
- 启动应用

### 手动安装

1. 前往 [Releases](https://github.com/kookyleo/snip2path/releases/latest)
2. 下载：
   - **Snip2Path-win-x64.zip**（~69 MB）—— 自包含，无需安装运行时
   - **Snip2Path-win-x64-compact.zip**（~180 KB）—— 需要 [.NET 8 桌面运行时](https://dotnet.microsoft.com/download/dotnet/8.0)
3. 解压后运行 `Snip2Path.exe`

## 使用

| 操作 | 效果 |
|---|---|
| `Alt+Shift+S`（或左键点击托盘图标） | 开始截图 |
| 拖拽矩形 | 截取选区 |
| 点击窗口 | 截取整个窗口 |
| 右键 / `Esc` | 取消截图 |
| 托盘菜单 > Change Hotkey... | 自定义快捷键 |
| 托盘菜单 > Start with Windows | 切换开机启动 |

截图保存为 PNG 到 `%TEMP%`，路径自动复制到剪贴板。

## 从源码构建

需要 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)。

```
dotnet build src/Snip2Path/Snip2Path.csproj
```

推送版本标签后 GitHub Actions 会自动构建发布（`git tag v1.0.0 && git push --tags`）。

## 许可证

[Apache License 2.0](LICENSE)
