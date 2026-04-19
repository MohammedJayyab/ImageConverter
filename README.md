# Image Converter

**Windows batch image conversion with format preview.**

**.NET 8** Windows Forms app: batch-convert images (**JPEG, PNG, BMP, GIF, WEBP, ICO**) with **Magick.NET**, folder thumbnails (**System.Drawing**), ICO size and letterbox settings, batch **Cancel**, and **single-step undo**.

---

## How it works

1. Pick **source** and **destination** folders.
2. Tune **ICO output size** and **letterbox color** when you plan to export icons.
3. Select files in the **review** list.
4. Right-click → **Convert to** → choose a format (the menu omits targets that would change nothing for the current selection), or use **Convert to Icon** for a fast ICO pass (includes rebuilding existing `.ico` files).

There is **no** toolbar **Convert** button—conversion runs from the **context menu** only. The bottom bar has **Undo** and **Cancel** (during work).

---

## Highlights

| Topic | Detail |
|-------|--------|
| Paths | **`ImageConversion`** rejects outputs whose path equals the source file path—pick a destination folder so each output gets a distinct path (usually a new extension). |
| Safety | Destination cannot be a **drive root** (e.g. `C:\`); paste into a drive root is blocked. |
| Config | **`config.ini`** beside the exe (`AppContext.BaseDirectory`): folders, window geometry, splitter, thumbnail size, ICO size index, letterbox index. No stored last-convert-format. |

---

## Requirements

- **Windows**
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** to build / `dotnet run`

---

## Build

```bash
dotnet build ImageConverter\ImageConverter.csproj -c Release
dotnet run --project ImageConverter\ImageConverter.csproj
```

Output: `ImageConverter\bin\<Configuration>\net8.0-windows\`.

---

## Docs

| File | Content |
|------|---------|
| [ImageConverter/documents/requirements.md](ImageConverter/documents/requirements.md) | Full product behavior |
| [ImageConverter/documents/plan.md](ImageConverter/documents/plan.md) | Implementation map |

---

## Dependency

[NuGet: Magick.NET-Q16-AnyCPU](https://www.nuget.org/packages/Magick.NET-Q16-AnyCPU) — version pinned in `ImageConverter.csproj`.
