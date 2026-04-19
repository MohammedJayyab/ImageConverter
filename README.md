# Image Converter

**Windows batch image conversion with format preview.**

A minimal **Windows Forms** utility for batch image conversion among **JPEG, PNG, BMP, GIF, WEBP, and ICO**, with folder-based review thumbnails, ICO-specific sizing and letterboxing, cancellable batches, and single-step undo for recent file operations.

---

## Overview

Image Converter targets **.NET 8** on Windows. Conversion uses **Magick.NET** (ImageMagick bindings); on-screen thumbnails use **System.Drawing**. The UI separates presentation (`frmMain`) from conversion (`ImageConversion`, `BatchConversionRunner`), settings (`AppSettings` / `AppSettingsStore`), and preview enumeration (`FolderThumbnailLoader`). User preferences and window layout persist to **`config.ini`** next to the executable.

---

## Features (summary)

| Area | Behavior |
|------|----------|
| Formats | Cross-convert within the supported set; ICO read uses the largest embedded frame; ICO write uses one user-selected square size with solid white/black letterboxing. |
| Workflow | Choose source and destination folders, pick **Convert from** / **Convert to**, select thumbnails, **Convert**; optional **Cancel** during work. |
| Review | Tile thumbnails, configurable size, refresh, clipboard paste/copy, delete (with undo), Explorer shortcuts, drag-and-drop to set source folder. |
| Undo | One-slot undo for the last paste, staged delete, or successful batch outputs. |
| Config | Remembers folders, **Convert to** choice, thumbnail size, window geometry, splitter, ICO options. |

---

## Requirements

- **Windows** (desktop)
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** or compatible runtime for self-contained deployment

---

## Build and run

From the repository root:

```bash
dotnet build ImageConverter\ImageConverter.csproj -c Release
dotnet run --project ImageConverter\ImageConverter.csproj
```

The executable is produced under `ImageConverter\bin\<Configuration>\net8.0-windows\`. A `config.ini` appears in that output folder after the first save.

---

## Repository layout

| Path | Purpose |
|------|---------|
| `ImageConverter.sln` | Visual Studio / `dotnet` solution |
| `ImageConverter/` | WinForms project (source, icon, embedded resources) |
| `ImageConverter/documents/requirements.md` | Product specification |
| `ImageConverter/documents/plan.md` | Implementation companion and module map |

---

## Documentation

- **[Specification](ImageConverter/documents/requirements.md)** — functional scope, UX, ICO rules, configuration keys.  
- **[Implementation plan](ImageConverter/documents/plan.md)** — UI structure, threading, undo, file responsibilities.

---

## Third-party dependency

- **[Magick.NET-Q16-AnyCPU](https://www.nuget.org/packages/Magick.NET-Q16-AnyCPU)** (see `ImageConverter.csproj` for the pinned version).
