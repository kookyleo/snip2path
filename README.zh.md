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
irm "https://github.com/kookyleo/snip2path/releases/latest/download/Snip2Path.zip" -OutFile $env:TEMP\s2p.zip; Expand-Archive $env:TEMP\s2p.zip "$env:LOCALAPPDATA\Snip2Path" -Force; $ws=New-Object -ComObject WScript.Shell; $sc=$ws.CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Snip2Path.lnk"); $sc.TargetPath="$env:LOCALAPPDATA\Snip2Path\Snip2Path.exe"; $sc.WorkingDirectory="$env:LOCALAPPDATA\Snip2Path"; $sc.Save(); & "$env:LOCALAPPDATA\Snip2Path\Snip2Path.exe"
```

该命令会：
- 下载 Snip2Path（~1 MB）
- 安装到 `%LOCALAPPDATA%\Snip2Path`
- 创建开始菜单快捷方式
- 启动应用

无需安装任何运行时，Windows 10/11 开箱即用。

### 手动安装

1. 前往 [Releases](https://github.com/kookyleo/snip2path/releases/latest)
2. 下载 **Snip2Path.zip**（~1 MB）
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

需要 [.NET SDK](https://dotnet.microsoft.com/download)。

```
dotnet build src/Snip2Path/Snip2Path.csproj
```

推送版本标签后 GitHub Actions 会自动构建发布（`git tag v1.0.0 && git push --tags`）。

## 许可证

[Apache License 2.0](LICENSE)
