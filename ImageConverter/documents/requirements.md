# Image Converter — Project Specification

Windows Forms desktop application using **.NET** and **Magick.NET** to convert images across a fixed set of formats (full cross-conversion within that list), including ICO source and destination handling. The tool emphasizes simplicity, responsive batch conversion, a review pane with thumbnails, and **single-level undo** for destructive or file-creating actions.

**Target framework:** `net8.0-windows` (see `ImageConverter.csproj`).

**Solution layout:** `ImageConverter.sln` builds the `ImageConverter` WinForms project. Application icon: `ImageConvert.ico`.

Scope is limited to the formats listed below—not every format ImageMagick can decode.

**Related document:** [plan.md](./plan.md) — WinForms layout, threading, undo behavior, separation of UI from conversion logic, and source file map.

---

## Core functional requirements

### 1. Image processing

- Use **Magick.NET** (`MagickImage` / `MagickImageCollection`) for file conversion and ICO read/write semantics.
- **Preview thumbnails** in the review list use **System.Drawing** (GDI+); conversion uses Magick.NET only.

#### Supported formats

| | Formats |
|---|--------|
| **Input** | JPEG, PNG, BMP, GIF, WEBP, ICO |
| **Output** | JPEG, PNG, BMP, GIF, WEBP, ICO |

**Cross-conversion:** Any pair where both source and target appear above is supported (for example WEBP→BMP, PNG→ICO). ICO-specific rules apply whenever the source or target is ICO.

**JPEG output** uses extension `.jpg`; **Convert from** treats both `.jpg` and `.jpeg` as JPEG.

### 2. ICO handling

**Converting to ICO**

- User selects **exactly one** embedded square size from **16×16** through **256×256** at the fixed steps offered in the UI (single-selection drop-down).
- The written `.ico` embeds that resolution. The source image **fits inside** the square with **aspect ratio preserved** (no stretching); **solid** letterboxing uses **white** or **black** only (no transparent ICO canvas option).

**Converting from ICO**

- Load with `MagickImageCollection`.
- Use the **largest** frame (area, then width as tie-breaker).
- Output format follows the user’s **Convert to** selection.

### 3. Scaling rules

- Preserve aspect ratio; fit within target dimensions; no distortion.
- Solid letterbox (white or black) when composing the square ICO canvas.

### 4. Raster quality and alpha

- JPEG and WebP writes use fixed quality hints in code (JPEG 92, WebP 90).
- When writing **JPEG** or **BMP**, alpha is flattened to an **opaque** result (white background) when the source has transparency.

---

## Application architecture

### Conversion layer

- **`ImageConversion.Convert`** accepts a `ConversionRequest` (paths, output format index, ICO size, letterbox kind) and returns integer status codes (see below).
- **`BatchConversionRunner`** loops selected files, builds destinations with **`SupportedFormats.BuildDestinationPath`**, and returns success/fail counts plus the list of **successful output paths** (for undo and UI selection).
- Settings and batch logic live outside the form class.

### Error handling

Surface missing files, invalid arguments, I/O failures, and read/write errors via return codes and user-visible messages (status bar and dialogs).

#### Return codes

| Code | Meaning |
|------|---------|
| 0 | Success |
| 1 | Invalid arguments |
| 2 | Processing error |

---

## User interface (WinForms)

### UX principles

- **Designer-first:** main form controls are created in `frmMain.Designer.cs`; `frmMain.cs` wires events and coordinates behavior.
- **Separation of concerns:** conversion, `config.ini`, batch execution, and thumbnail enumeration are implemented outside the form.

### Core features

- **Status bar** (`StatusStrip`): primary message; **bold** supplementary label for the current selection (file name or “first (+N more)”) or, after a conversion that wrote only **outside** the source folder, a short hint naming the output file(s); **progress bar** during batch conversion; spring filler.
- **Source / destination** folders (browse). If destination text is empty when picking a source, destination may default to the source folder. Folder browser initial directory prefers the current path or **My Pictures**.
- **Review / preview**
  - **Top-level** files only; thumbnails built in batches (implementation chunk size: 30 files per slice) with cancellable reload.
  - **Tile** view; **multi-select**.
  - **Vertical split:** settings stack above, review below; **splitter distance** persisted.
  - **Thumbnail size:** Small / Medium / Large → pixel sizes **80 / 128 / 192**; tile height derived from font metrics to avoid clipped file names.
  - **Toolbar:** thumbnail size, **Refresh review**, **Open destination**.
  - **Context menu:** selected item → **Copy**, **Delete**, **Open file location**; empty area / unselected row / placeholder → **Paste** only when the clipboard holds an image (otherwise menu does not open).
  - **Open destination:** one selected row and existing expected output → Explorer `/select` that file; else open the destination folder.
- **Formats:** **Convert from** and **Convert to** share the same ordered list (see `SupportedFormats`). **Convert to** is persisted in `config.ini`; **Convert from** follows the default populated in the UI (typically PNG for both on first run).
- **ICO output:** when **Convert to** is ICO, show ICO size and **ICO canvas (letterbox)** (white/black).
- **Drag and drop** on preview or placeholder: first dropped path resolves to a folder (directory or parent of a file) and sets the source folder.

### Undo (single operation)

- **Undo** button and **Ctrl+Z** reverse the **last** eligible operation when not busy converting.
- Operations that register undo:
  - **Batch conversion:** deletes created output files that still exist (only outputs recorded as successful).
  - **Paste:** deletes the pasted PNG if it still exists.
  - **Delete:** restores files moved to a temporary session folder (delete is implemented as move-to-staging, then restore on undo).
- A new paste, delete, or successful conversion **replaces** the previous undo slot (single-level undo, not a stack history).

### Format filter and conversion

- Conversion uses files whose extension matches **Convert from**, unless **none** of the selected rows match—in that case the user may **convert all selected files anyway** or cancel.

### Keyboard shortcuts (when `KeyPreview` is active)

- **Ctrl+C:** copy selected preview items (skipped when focus is in a multiline text control; **Ctrl+C** in a **ComboBox** is passed through for standard copy).
- **Ctrl+V** or **Ctrl+P:** paste clipboard image into the source folder when the clipboard contains image data.
- **Delete:** delete selected preview files (with confirmation).
- **Ctrl+Z:** undo last operation when available.

### Review refresh policy

Reload thumbnails after **Refresh review**, **paste**, **copy** (with updated status text), **delete**, **undo**, and **completed batch conversion**. After conversion, if new outputs land **in** the source folder, the list reload may **select** those paths; otherwise the status strip can highlight output name(s) when outputs exist only elsewhere.

---

## Configuration (`config.ini`)

Settings file path: **`AppContext.BaseDirectory` + `config.ini`** (typically beside the executable when deployed as a portable folder layout).

**Persisted** (atomic-style write via temp file plus replace/move):

- Last source and destination folders  
- **Convert to** format index (`DefaultConvertToIndex`)  
- Preview thumbnail size index (Small / Medium / Large)  
- Main window bounds and maximized flag (after placement has been saved)  
- Review splitter distance (height of the settings panel)  
- ICO output size combo index  
- Letterbox white/black combo index  

**Not persisted:** **Convert from** index (defaults with the format combo initialization).

---

## Command-line support (optional)

Optional; not required for the current WinForms build.

---

## Constraints

- **Magick.NET** (NuGet: `Magick.NET-Q16-AnyCPU` in this repo) is the only **NuGet** dependency for image conversion. WinForms and BCL types (including **System.Drawing** for thumbnails) are framework-supplied.

---

## Non-functional requirements

- Efficient batch processing; preview loading off the UI thread with marshaled UI updates.
- Responsive UI during conversion (**Task.Run** + **`Progress<T>`** for progress text and bar).
- Clear feedback for success, partial failure, cancellation, and undo failure.

---

## Expected outcome

A compact Windows desktop tool that converts between supported formats reliably, handles ICO per the rules above, provides review plus paste/copy/delete/open-location/open-destination, **single-step undo** for conversion outputs and local file operations, and stays straightforward to operate.
