# Image Converter — Project Specification

**Windows batch image conversion with format preview.**

Windows Forms desktop application using **.NET 8** (`net8.0-windows`) and **Magick.NET** for batch conversion among **JPEG, PNG, BMP, GIF, WEBP, and ICO**, with a folder review pane, ICO-specific controls, cancellable work, and **single-level undo** for recent file-changing actions.

**Artifacts:** `ImageConverter.sln`, project `ImageConverter`, application icon `ImageConvert.ico`. Scope is limited to the formats below—not every format ImageMagick can decode.

**Companion:** [plan.md](./plan.md) — implementation layout, threading, and module responsibilities.

---

## 1. Image processing

### 1.1 Stacks

| Concern | Technology |
|--------|------------|
| Conversion | **Magick.NET** (`MagickImage`, `MagickImageCollection`) |
| Review thumbnails | **System.Drawing** (GDI+), separate from Magick |

### 1.2 Supported formats

| Role | Formats |
|------|---------|
| Input / output | JPEG, PNG, BMP, GIF, WEBP, ICO |

Cross-convert within this set (for example WEBP→BMP, PNG→ICO). ICO-specific behavior applies whenever source or target is ICO.

- **JPEG output** uses `.jpg`; **Convert from** treats `.jpg` and `.jpeg` as JPEG.

### 1.3 ICO

**To ICO**

- One embedded square size chosen from the UI list (**16×16 … 256×256** at fixed steps).
- Image is scaled to **fit inside** that square (aspect preserved, no stretch); **Extent** fills a square with **solid white or black** letterboxing (no transparent ICO canvas option).

**From ICO**

- Read with `MagickImageCollection`; pick **largest** frame (area, then width).
- Output format follows **Convert to**.

### 1.4 Raster quality and alpha

- JPEG quality **92**, WebP **90** (fixed in code).
- JPEG and BMP outputs: if source has alpha, flatten to opaque using **white** background before write.

---

## 2. Conversion API behavior

### 2.1 `ImageConversion.Convert`

- Accepts **`ConversionRequest`**: paths, output format index (0–5), ICO pixel size when relevant, **`IconBackgroundKind`** for ICO canvases.
- Returns **0** success, **1** invalid arguments, **2** processing error.

**Identical source and destination path:** After **`Path.GetFullPath`**, if source and destination compare equal (case-insensitive), conversion **does not run**—returns **1** with message *Source and destination are the same file.* Callers must ensure output paths differ from inputs when source and destination folders coincide (for example rely on **different extensions** from **Convert to**).

Domain types (**`ConversionRequest`**, **`SupportedFormats`**, settings types, **`IconBackgroundKind`**) are **`internal`**; **`frmMain`** is **`public partial`** for the WinForms surface.

### 2.2 Batch orchestration

- **`BatchConversionRunner`** builds each destination with **`SupportedFormats.BuildDestinationPath`**, invokes **`ImageConversion.Convert`**, returns counts and **`SuccessfulDestinationPaths`** for undo and UI refresh.
- **`SupportedFormats`**: **`FormatIndexMatchesExtension`**, **`TryGetFormatIndexForPath`** for UI logic.

---

## 3. User interface

### 3.1 Principles

- Designer-owned layout in **`frmMain.Designer.cs`**; code-behind coordinates only.
- Conversion, **`config.ini`**, thumbnails, and batch logic live outside the form class.

### 3.2 Folders and guardrails

- **Browse** for source and destination; paths shown read-only.
- Empty destination may track source when the user picks a source or drops files.
- Folder browser starts from the current path when valid, else **My Pictures**.
- **Destination for conversion** must exist and must **not** be a **drive root** (for example `C:\`): normal (non-elevated) processes typically cannot create files there; the UI blocks it with an explanatory dialog.
- **Paste** into the source folder is **blocked** when the source path is a drive root (same rationale).

### 3.3 Review / preview

- **Top-level** files only; thumbnails in batches of **30**; reload is **cancellable** (new load cancels the previous token cooperatively).
- **Tile** view; multi-select; ICO rows show a second line **`( icon)`** under the name.
- **Thumbnail size:** Small / Medium / Large → **80 / 128 / 192** px; tile height sized for up to **three** text lines (font metrics).
- **Toolbar:** thumbnail size, **Refresh review**, **Open destination**.
- **Context menu** on selected tile: **Copy**, primary convert action (same caption as main button), **Delete**, **Open file location**. Empty / unselected / placeholder: **Paste** only if clipboard holds an image.
- **Open destination:** single selection + expected output exists → Explorer **`/select`**; else open folder.

### 3.4 Formats and primary action caption

- **Convert from** / **Convert to** share one ordered list aligned with **`SupportedFormats`** indices.
- **Convert to** persisted in **`config.ini`**; **Convert from** defaults with combos (typically PNG) and **auto-aligns** when every selected file maps to the **same** format index; mixed or unknown extensions leave **Convert from** unchanged.
- Main button and context menu use **`GetConvertToMenuActionText()`**: normally **Convert to** followed by the target shorthand (JPEG, PNG, …). When **both** **Convert from** and **Convert to** are **ICO**, the label is **Rebuild icon** (reapply canvas size and letterbox).

### 3.5 ICO panel

- When **Convert to** is ICO: show ICO size drop-down and **ICO canvas (letterbox)** (**White** / **Black**).

### 3.6 Conversion flow (user-visible)

- Before running, **SyncConvertFromToSelectedFiles** may adjust **Convert from**.
- Filter selected paths by **Convert from**; if none match, show dialog listing **distinct extensions** and offer **convert all selected anyway** or cancel.
- Progress on status bar; **Cancel** aborts the batch token.
- After success, refresh preview; if outputs landed **in** the source folder, reselect those paths; if outputs are only elsewhere, the bold status segment can show output filename hints.

### 3.7 Undo (single slot)

Reversible operations: delete outputs from last successful batch (all deletes must succeed), delete pasted PNG, or restore files from staged delete folder under **`%TEMP%\ImageConverterUndo\`**. **Ctrl+Z** / **Undo** button; disabled while converting. New operation replaces prior undo.

### 3.8 Keyboard (`KeyPreview`)

| Input | Action |
|-------|--------|
| Ctrl+C | Copy selection (not when focus in multiline text; ComboBox Ctrl+C passes through) |
| Ctrl+V / Ctrl+P | Paste image if clipboard has image |
| Delete | Delete selected (confirm) |
| Ctrl+Z | Undo |

### 3.9 Status strip

Primary label; **bold** segment for selection summary or post-convert filename hint; progress bar during batch; spring.

---

## 4. Configuration (`config.ini`)

**Path:** `Path.Combine(AppContext.BaseDirectory, "config.ini")`.

**Persisted keys:** last source/destination folders, **DefaultConvertToIndex**, preview thumbnail size index, window placement (after first save), splitter distance, ICO size index, letterbox index. Write path: temp file then **`File.Replace`** / move.

**Not persisted:** **Convert from** (may still change at runtime via selection sync).

---

## 5. Constraints

- Single NuGet dependency for imaging: **`Magick.NET-Q16-AnyCPU`** (see project file). WinForms + BCL + System.Drawing are framework-supplied.

---

## 6. Non-functional expectations

- Heavy work off the UI thread; progress via **`IProgress`** / **`Progress<T>`** on the UI synchronization context.
- Predictable feedback for success, partial failure, cancellation, and undo failure.

---

## 7. Expected outcome

A focused Windows utility: reliable cross-format conversion within the supported set, correct ICO handling, folder preview with guardrails (paths, drive roots), dynamic convert labeling including **Rebuild icon** for ICO→ICO, and **single-step undo** where applicable.
