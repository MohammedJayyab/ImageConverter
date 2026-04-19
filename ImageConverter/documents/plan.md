# Image Converter — Implementation Plan

Companion to [requirements.md](./requirements.md). On conflict, **requirements take precedence**.

---

## 1. UI / UX (`frmMain`)

### 1.1 Initialization

- **`InitializeComponent()`** in **`frmMain.Designer.cs`** owns all controls.
- **`OnLoad`:** load settings, wire events, populate combos, **`ApplySettingsToUi`** (includes **`UpdateIcoOutputUi`**), placeholder and convert availability, initial preview load.
- **`OnShown`:** **`ApplySavedSplitterDistanceOnce`** then **`UpdateIcoOutputUi`** so splitter and convert caption match persisted **Convert to**.

### 1.2 Layout

- **`SplitContainer`** horizontal: settings stack (Panel1), review (Panel2). **`PreviewSplitterDistance`** persists Panel1 height; clamp when applying saved distance.
- Actions row: **Undo**, **Convert**, **Cancel** (see Designer tab order).

### 1.3 Folders

- **`FolderBrowserDialog`**; **`GetInitialDirectoryHint`** uses current path or **My Pictures**.
- **`EnsureDestinationFolderExists`:** directory must exist; **`IsDriveRootFolderPath`** rejects volume roots (compare normalized folder to **`Path.GetPathRoot`**, skip UNC heuristic).
- Paste uses **`IsDriveRootFolderPath`** on source folder before saving PNG.

### 1.4 Format combos

- **`PopulateFormatCombos`:** labels from **`SupportedFormatLabels`**; default **PNG** (index 1) for both before **`ApplySettingsToUi`** overrides **Convert to** from **`AppSettings.DefaultConvertToIndex`**.
- **`SyncConvertFromToSelectedFiles`:** if every selected path resolves to one index via **`TryGetFormatIndexForPath`**, set **Convert from**; mixed → leave unchanged.

### 1.5 Primary action text

- **`ConvertToButtonTargetPhrases`** maps index → short phrase (**icon** for ICO).
- **`GetConvertToMenuActionText()`:** if **Convert to** and **Convert from** are both ICO index → **`&Rebuild icon`**; else **`&Convert to {phrase}`**.
- **`UpdateIcoOutputUi`** updates button, context item text, ICO hint visibility, ICO size panel visibility.

### 1.6 Preview pipeline

- **`ReloadSourcePreviewsAsync`:** **`ReplacePreviewLoadCancellationAsync`** (new **`CancellationTokenSource`**, **`CancelAsync`** + dispose prior), enumerate on **`Task.Run`**, **`BeginPreviewListReload`**, **`LoadPreviewThumbnailBatches`** (size **30**), **`FinalizePreviewListReload`** with optional selection set and status override.
- **`AppendThumbnailBatchToListView`:** ICO files get label **`"{name}\r\n( icon)"`** via **`FormatIndexMatchesExtension(..., 5)`**.
- **`ApplyPreviewTileLayoutFromPixelSize`:** **`textBand = lineH * 3 + 12`**.

### 1.7 Conversion orchestration

- **`ConvertSelectedAsync`:** **`SyncConvertFromToSelectedFiles`**, **`TryValidateConversionFormats`**, **`EnsureDestinationFolderExists`**, **`TryGetNonEmptyPreviewSelection`**, **`TryResolvePathsMatchingConvertFrom`** (mismatch dialog + extension list), **`RunBatchConversionAndRefreshAsync`**.
- **`ReplaceConversionCancellationAsync`** mirrors preview cancellation pattern.
- **`RunBatchConversionAndRefreshAsync`:** **`Task.Run`** **`BatchConversionRunner.Run`** with **`Progress`**; **`ApplyConversionResultAndReloadAsync`** registers undo from **`GetSuccessfulOutputsExistingOnDisk`**, **`FilterOutputsUnderSourceFolder`**, **`UpdatePendingBoldForOutputsNotInReview`**, **`ReloadSourcePreviewsAsync`**.

### 1.8 Undo

- **`RegisterUndo`**, **`UndoLastOperationAsync`**; staged delete via **`TryUndoDeleteMoves`**; **`UpdateUndoButtonEnabled`** from **`SetConversionBusy`** end state.

### 1.9 Status strip

- **`statusLabelMessage`**, **`statusLabelSelection`**, **`toolStripProgressBatch`**, **`statusLabelSpring`**.
- **`SetStatusMessage`** refreshes **`RefreshSelectionStatusText`**.

### 1.10 Keyboard

- **`ProcessCmdKey`:** skip **`TextBoxBase`**; ComboBox **Ctrl+C** → base; preview **Ctrl+C/V/P**, **Delete**, **Ctrl+Z** as in requirements.

### 1.11 Persistence

- **`SchedulePersistUiSettings`:** **400 ms** WinForms timer debounce.
- **`PersistSettings`** on timer tick, resize end, splitter move, combos (when not loading), **`OnFormClosing`**.

---

## 2. Business logic

### 2.1 Module map

| Unit | Responsibility |
|------|----------------|
| `Program.cs` | **`ApplicationConfiguration.Initialize`**, **`Application.Run`** |
| `frmMain` | Presentation, orchestration, undo registration |
| `ImageConversion` | Magick pipelines; **reject identical src/dst full paths** (code 1); ICO PNG32 normalization |
| `BatchConversionRunner` | Loop, **`RunResult`**, **`SuccessfulDestinationPaths`** |
| `SupportedFormats` | Indices, **`BuildDestinationPath`**, extension matching |
| `ConversionRequest` | DTO |
| `FolderThumbnailLoader` | Enumerate + thumbnails |
| `AppSettings` / `AppSettingsStore` | **`config.ini`** load/save (internal types) |

### 2.2 Dependencies

**Magick.NET-Q16-AnyCPU** **14.12.0**; nullable + implicit usings enabled.

### 2.3 Path rules (`ImageConversion`)

Early exit when **`Path.GetFullPath`** of source equals destination (**case-insensitive**): return **1**, *Source and destination are the same file.* No in-place write path in current code.

### 2.4 ICO pipeline

- **To ICO:** **`ApplySquareIconCanvas`** → **`WriteIcoWithPngNormalization`**.
- **From ICO:** collection read → **`SelectLargestFrame`** → raster path with flatten when needed.

### 2.5 Snapshot

| Topic | State |
|-------|--------|
| WinForms UI, preview batches, cancellation, drive-root checks, **Rebuild icon** label | Implemented |
| Magick conversion, batch + undo, **`config.ini`** | Implemented |
| Transparent ICO letterbox UI | Out of scope |
