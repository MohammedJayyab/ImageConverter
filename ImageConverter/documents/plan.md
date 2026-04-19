# Image Converter — Implementation Plan

This document accompanies [requirements.md](./requirements.md). If anything conflicts, **the requirements document takes precedence**.

---

## 1. UI / UX

### 1.1 Component initialization (`frmMain.Designer.cs`)

- Control creation for the main form lives in **`frmMain.Designer.cs`** (`InitializeComponent()`).
- **`frmMain.cs`** wires events and coordinates only; conversion, settings, thumbnails, and undo helpers stay in dedicated methods or static types (Section 2.1).

### 1.2 Layout and navigation

- Single main window: source → review/select → destination and formats → **Undo** / **Convert** / **Cancel**.
- Bottom **status strip** reserved (Section 1.10).

### 1.3 Source and destination

- **`FolderBrowserDialog`** for browse; path text boxes are read-only.
- Destination defaults to source when destination is empty (see browse/drag-drop logic).
- Drag/drop uses the **first** path: directory becomes source folder; file uses its parent folder.

### 1.4 Format selection

- Combo order matches `SupportedFormats`: JPEG, PNG, BMP, GIF, WEBP, ICO (indices `0`–`5`).
- **`PopulateFormatCombos`** sets both combos to **PNG** (`index 1`) after items are filled; **`ApplySettingsToUi`** then sets **Convert to** from `AppSettings.DefaultConvertToIndex` only—**Convert from** is not loaded from disk.

### 1.5 Review / preview panel

- **`SplitContainer`** (horizontal): settings **Panel1**, review **Panel2**; **`PreviewSplitterDistance`** persists the **Panel1** height (implementation uses `splitReview.SplitterDistance` with clamping on show).
- **`ListView`** tile view; `ImageList` thumbnails; **`FolderThumbnailLoader`** enumerates and thumbnails **top-level** files.
- **`ReloadSourcePreviewsAsync`** accepts optional **`selectFullPathsAfter`** (reselect paths after reload) and **`statusOverride`** (final status text). Loads slices of **30** files per iteration; cooperative cancel via **`CancellationTokenSource`** on the preview load CTS.
- **Context menu** and placeholder behavior match requirements: hit-test on `listViewPreview` determines Copy/Delete/Open location vs Paste-only.
- **`btnOpenDestination`**: `SupportedFormats.BuildDestinationPath` + `explorer /select` when a single expected output exists.

### 1.6 Conversion actions and feedback

- **Convert** requires valid folders, format indices, and at least one selected row.
- **Convert from** filter vs mismatch dialog as in requirements.
- **`BatchConversionRunner.Run`** returns **`RunResult(SuccessCount, FailCount, SuccessfulDestinationPaths)`**; the form registers **undo** to delete **`SuccessfulDestinationPaths`** that still exist after a successful batch.
- If some outputs sit **inside** the source folder (normalized path compare), reload selects those paths; otherwise **`_pendingBoldFileNameFromConvert`** drives the **selection** status label text for a short hint.

### 1.7 Undo

- **`btnUndo`** and **Ctrl+Z** call **`UndoLastOperationAsync`**: invokes the last **`Func<bool>`** registered by **`RegisterUndo`**.
- **Convert:** undo = delete produced destination files (best effort, all must delete for success).
- **Paste:** undo = delete the pasted file.
- **Delete:** files are **`File.Move`**’d into a temp session directory under `%TEMP%\ImageConverterUndo\{guid}\`; undo moves them back via **`TryUndoDeleteMoves`**.
- Only **one** undo slot; each new qualifying operation replaces **`_undoLastOperation`**. Undo disabled while **`SetConversionBusy(true)`**.

### 1.8 ICO canvas (no transparency UI)

- Letterbox combo: **White** / **Black** mapped to **`IconBackgroundKind`**; **`ImageConversion`** uses resize + **`Extent`** with solid fill, then ICO write path with PNG32 normalization (`WriteIcoWithPngNormalization`).

### 1.9 Configuration and lifecycle

- **`AppSettingsStore`**: path **`Path.Combine(AppContext.BaseDirectory, "config.ini")`**; load/save INI-style lines (`#`/`;` comments, `key=value`); save uses temp + **`File.Replace`** / move.
- **`SchedulePersistUiSettings`**: WinForms **`Timer`** **400 ms** debounce for layout-heavy updates.
- Persist on resize end, splitter move, combo changes (when not loading settings), and form closing.

### 1.10 Threading

- Preview enumeration and thumbnail creation on **`Task.Run`**; UI updates via **`Invoke`** / **`RunOnUiThread`**.
- Conversion on **`Task.Run`** with **`Progress<(int Current, int Total, string FileName)>`** tied to status + **`toolStripProgressBatch`**.
- **`CancellationToken`** from **`_conversionCts`** for cancel button.

### 1.11 Status bar

- Items (order): **`statusLabelMessage`**, **`statusLabelSelection`** (bold, bordered; selection summary or convert hint), **`toolStripProgressBatch`**, **`statusLabelSpring`**.
- **`SetStatusMessage`** updates the message and **`RefreshSelectionStatusText`** (selection label visibility and text).

### 1.12 Keyboard routing (`ProcessCmdKey`)

- **`KeyPreview = true`** on the form.
- Skips shortcuts when **`ActiveControl`** is **`TextBoxBase`**.
- **`Ctrl+C`** in a **`ComboBox`** delegated to base (standard copy).
- **`Ctrl+C`** with preview selection → async copy.
- **`Ctrl+V`** / **`Ctrl+P`** with image on clipboard → paste.
- **`Delete`** with selection → delete.
- **`Ctrl+Z`** → undo when **`_undoLastOperation != null`** and not converting.

---

## 2. Business logic

### 2.1 Source file map

| File | Role |
|------|------|
| `Program.cs` | Entry: `ApplicationConfiguration.Initialize`, `Application.Run(new frmMain())` |
| `frmMain.cs` / `frmMain.Designer.cs` | UI, events, undo registration, preview/conversion orchestration |
| `ImageConversion.cs` | Magick.NET: raster↔raster, raster↔ICO, ICO↔ICO; quality/flatten; return codes |
| `BatchConversionRunner.cs` | Batch loop; collects successful destination paths |
| `SupportedFormats.cs` | Extension list, destination path builder, **Convert from** extension match |
| `ConversionRequest.cs` | Immutable conversion parameters |
| `IconBackgroundKind.cs` | Letterbox enum |
| `FolderThumbnailLoader.cs` | Enumerate images + GDI+ thumbnails |
| `AppSettings.cs` / `AppSettingsStore.cs` | Settings model + `config.ini` |

### 2.2 Dependencies

- **`Magick.NET-Q16-AnyCPU`** (project references **14.12.0**).
- **ImplicitUsings** enabled; **nullable** enabled.

### 2.3 ICO pipeline (summary)

- **To ICO:** `ApplySquareIconCanvas` → **`WriteIcoWithPngNormalization`** (PNG32 round-trip for encoder behavior).
- **From ICO:** read collection → **`SelectLargestFrame`** → write raster with optional flatten for JPEG/BMP.

### 2.4 Implementation snapshot

| Area | Status |
|------|--------|
| Main form, split layout, preview, formats, ICO + letterbox, status bar + selection label, Undo / Convert / Cancel, drag-drop | Implemented |
| `config.ini`, Magick conversion, batch + progress/cancel + successful paths, undo for convert/paste/delete | Implemented |
| Transparent ICO canvas UI | Not in scope |

---

*Legacy wording about “transparent padding” for ICO is obsolete; see [requirements.md](./requirements.md).*
