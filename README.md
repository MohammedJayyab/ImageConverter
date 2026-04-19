# Image Converter

**Windows batch image conversion with format preview.**

Desktop utility on **.NET 8** / **Windows**: batch-convert images among **JPEG, PNG, BMP, GIF, WEBP, and ICO** using **Magick.NET**, with a folder **review pane** (thumbnails via **System.Drawing**), ICO-specific options, progress/cancel, and **single-step undo** for recent writes and deletes.

---

## Overview

The WinForms shell (`frmMain`) delegates conversion to **`ImageConversion`**, batching to **`BatchConversionRunner`**, thumbnails to **`FolderThumbnailLoader`**, and preferences to **`AppSettingsStore`** (**`config.ini`** beside the executable).

---

## Highlights

| Topic | Behavior |
|-------|----------|
| Formats | Full cross-convert within the six formats; ICO uses largest frame when reading; writing ICO uses one embedded size + white/black letterbox. |
| Paths | **`ImageConversion`** rejects identical source and destination paths (same full path). Use a different extension or folder so output differs. Destination cannot be a **drive root** (e.g. `C:\`); paste is blocked for the same case. |
| UI | Dynamic **Convert** label (**Convert to …**, or **Rebuild icon** when both sides are ICO); optional **Convert from** sync from selection; ICO tiles show **`( icon)`**. |
| Undo | One undo slot: batch outputs, paste, or staged delete under **`%TEMP%\ImageConverterUndo\`**. |

---

## Requirements

- **Windows**
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** to build or `dotnet run`

---

## Build

From the repository root:

```bash
dotnet build ImageConverter\ImageConverter.csproj -c Release
dotnet run --project ImageConverter\ImageConverter.csproj
```

Build output: `ImageConverter\bin\<Configuration>\net8.0-windows\`. Settings file **`config.ini`** appears after first save.

---

## Repository

| Path | Role |
|------|------|
| `ImageConverter.sln` | Solution |
| `ImageConverter/` | Project sources, `ImageConvert.ico` |
| `ImageConverter/documents/requirements.md` | Product specification |
| `ImageConverter/documents/plan.md` | Implementation notes |

---

## Documentation

- [Specification](ImageConverter/documents/requirements.md) — behavior and UX contract.  
- [Implementation plan](ImageConverter/documents/plan.md) — code structure and algorithms.

---

## Dependency

[NuGet: Magick.NET-Q16-AnyCPU](https://www.nuget.org/packages/Magick.NET-Q16-AnyCPU) — version in `ImageConverter.csproj`.
