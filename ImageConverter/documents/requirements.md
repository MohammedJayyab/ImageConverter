# Image Converter — Project Specification

**Windows batch image conversion with format preview.**

.NET **8** WinForms desktop tool using **Magick.NET** for conversions among **JPEG, PNG, BMP, GIF, WEBP, and ICO**. It provides a folder **review** with thumbnails (**System.Drawing**), ICO-specific settings, cancellable batches, and **single-level undo**.

**Artifacts:** `ImageConverter.sln`, project folder `ImageConverter`, icon `ImageConvert.ico`. Supported formats are fixed—not every codec ImageMagick supports.

**Companion:** [plan.md](./plan.md).

---

## 1. Imaging

### Stacks

| Layer | Implementation |
|-------|----------------|
| Encode / decode | **Magick.NET** (`MagickImage`, `MagickImageCollection`) |
| Review thumbnails | **System.Drawing** (separate from conversion) |

### Formats

Input and output (cross-convert within this set): **JPEG, PNG, BMP, GIF, WEBP, ICO**.

- JPEG output uses **`.jpg`**; matching extensions **`.jpg`** / **`.jpeg`** as JPEG.

### ICO

- **To ICO:** one UI-selected square size (fixed steps **16×16 … 256×256**); fit inside square, aspect preserved; **Extent** with **solid white or black** letterbox (no transparent canvas option).
- **From ICO:** `MagickImageCollection`; **largest** frame (area, then width); output format is whatever the user chose in the convert action (see Section 3).

### Raster quality

- JPEG **92**, WebP **90**.
- JPEG/BMP output: flatten alpha to opaque **white** when needed.

---

## 2. `ImageConversion.Convert`

- **`ConversionRequest`**: source/destination paths, output format index **0–5**, ICO size and **`IconBackgroundKind`** when writing ICO.
- **Return codes:** **0** success, **1** invalid arguments, **2** processing error.

**Identical paths:** If **`Path.GetFullPath`** of source equals destination (case-insensitive), return **1** (*Source and destination are the same file.*). Batch paths from **`SupportedFormats.BuildDestinationPath`** must therefore differ from each source path (typically a **different extension** for same basename).

Domain types are **`internal`**; **`frmMain`** is **`public partial`**.

---

## 3. User interface (current design)

### 3.1 Layout

- **Source / destination** folders (read-only text + browse). Empty destination may follow source when appropriate. Folder dialog initial directory: current path if valid, else **My Pictures**.
- **Icon output (.ico):** embedded **size** list and hint text (settings apply when converting **to** ICO from the menu).
- **ICO canvas (letterbox):** **White** / **Black**.
- **Review / preview:** toolbar (**Thumbnail size**, **Refresh review**, **Open destination**); tile **`ListView`**; placeholder when empty; drag-and-drop sets source folder from first path (folder or parent of file).
- **Actions bar:** **Undo** and **Cancel** only—there is **no** primary **Convert** button on the form.

### 3.2 Conversion entry point

All batch conversions start from the **thumbnail context menu** (right-click a **selected** item):

| Command | Behavior |
|---------|------------|
| **Convert to** | **Cascading submenu.** For each format index **F** in **`BuildAllowedConvertToFormatIndices`**, one item labeled with the human-readable format name (e.g. `JPEG (.jpg / .jpeg)`). **F** is included only if **at least one** selected file’s extension does **not** already match **F**—so menu entries that would be “convert everything to the type they already are” are omitted. Each item runs **`ConvertSelectionToFormatAsync(F, iconQuickAccess: false)`**. Parent **Convert to** is enabled only when conversion is allowed and at least one submenu item exists. |
| **Convert to Icon** | **`ConvertSelectionToFormatAsync(ICO, iconQuickAccess: true)`**: converts **all** selected files to ICO, including existing `.ico` files (rebuild / re-encode with current ICO size and letterbox). |

If **`SelectPathsNeedingTargetFormat`** yields **no** paths for a chosen format (non–quick-icon path), show *Every selected file is already … There is nothing to convert.*

### 3.3 Other context actions

- **Copy**, **Delete**, **Open file location** on selected thumbnail.
- **Paste** on placeholder or empty hit area when clipboard contains an image.

### 3.4 Guardrails

- **Destination** must exist and must **not** be a **local drive root** (e.g. `C:\`); explain in dialog (non-elevated apps typically cannot create files there).
- **Paste** blocked when source folder is a drive root.
- **`Open destination`** opens the destination **folder** in Explorer (does not **`/select`** a derived output file).

### 3.5 Preview

- Top-level supported files only; thumbnails in batches of **30**; reload cancellable (replace **`CancellationTokenSource`** with **`CancelAsync`** on prior load).
- ICO files show **`( icon)`** second line in the tile label.
- Thumbnail sizes **80 / 128 / 192** px (Small / Medium / Large).

### 3.6 Batch run

- **`RunBatchConversionAndRefreshAsync`** with progress on status bar; **Cancel** stops the batch token.
- After completion: refresh preview; if outputs exist under the **source** folder path, reselect those files; otherwise optional bold hint for output names when outputs are only outside the review folder.

### 3.7 Undo

Single slot: delete created outputs (all deletes must succeed), delete pasted PNG, or restore staged deletes under **`%TEMP%\ImageConverterUndo\`**. **Ctrl+Z** / **Undo**.

### 3.8 Keyboard (`KeyPreview`)

Ctrl+C (copy), Ctrl+V / Ctrl+P (paste image), Delete, Ctrl+Z—subject to focus rules (text box / combo exceptions as implemented).

---

## 4. Configuration (`config.ini`)

**Path:** `Path.Combine(AppContext.BaseDirectory, "config.ini")`. Atomic write: temp file then **`File.Replace`** (or move when new).

**Keys written and read** (see **`AppSettings`** / **`AppSettingsStore`**):  
`LastSourceFolder`, `LastDestinationFolder`, `PreviewThumbnailSizeIndex`, `MainWindowPlacementSaved`, `MainWindowLeft`, `MainWindowTop`, `MainWindowWidth`, `MainWindowHeight`, `MainWindowMaximized`, `PreviewSplitterDistance`, `IcoOutputSizeIndex`, `SolidColorIndex`.

There is **no** persisted “last output format”—format choice is always made from the context menu when converting.

---

## 5. Constraints

Single imaging NuGet: **`Magick.NET-Q16-AnyCPU`**. Framework: WinForms + BCL + System.Drawing.

---

## 6. Non-functional

Off-UI-thread batch work; **`Progress<T>`** for status and progress bar; clear success, partial failure, cancel, and undo failure messaging.

---

## 7. Outcome

A small Windows tool: menu-driven conversion with redundant targets hidden, quick ICO path, ICO tuning in the settings stack, path safety (drive root, identical src/dst), and predictable undo.
