# Image Converter — Implementation Plan

Companion to [requirements.md](./requirements.md). Requirements win on conflict.

---

## 1. UI (`frmMain`)

### 1.1 Initialization

- **`OnLoad`:** `Load` settings, **`WireEvents`**, **`PopulatePreviewSizeCombo`**, **`ApplySettingsToUi`**, placeholder, **`SetStatusMessage("Ready")`**, **`InitialPreviewLoadAsync`**.
- **`OnShown`:** **`BeginInvoke(ApplySavedSplitterDistanceOnce)`** only (no separate ICO caption pass).

### 1.2 Designer surface

- **No** “Convert from / to” combo boxes. **`grpFormats`** title **Icon output (.ico)**; contains hint label + **`cmbIcoOutputSize`**.
- **`panelActions`:** **`btnUndo`**, **`btnCancel`** only.

### 1.3 Selection

- **`listViewPreview.ItemSelectionChanged`** → **`RefreshSelectionStatusText`** only.

### 1.4 Context menu (`PreviewContextMenuStrip_Opening`)

On hit **selected** item:

- Build **`Convert to`** cascade: **`BuildAllowedConvertToFormatIndices(paths)`** → for each index **`i`**, add **`ToolStripMenuItem(SupportedFormatLabels[i])`** wired to **`ConvertSelectionToFormatAsync(i, false)`**.
- **`toolStripMenuItemPreviewConvertTo`:** **`DropDownItems.Clear()`** each open; enabled if **`CanConvertSelection()`**, not busy, and **`DropDownItems.Count > 0`**.
- **`toolStripMenuItemConvertToQuickIcon`:** **`ConvertSelectionToFormatAsync(SupportedFormats.Count - 1, true)`**.

Placeholder / empty: paste-only path when clipboard has image.

### 1.5 `ConvertSelectionToFormatAsync`

- Validates destination via **`EnsureDestinationFolderExists`** (existence + **`IsDriveRootFolderPath`**).
- **`iconQuickAccess && ICO`:** **`pathsToConvert =`** full selection.
- Else: **`SelectPathsNeedingTargetFormat(paths, outputFormatIndex)`**; if empty → MessageBox *Every selected file is already …*.
- **`RunBatchConversionAndRefreshAsync`**.

Helpers: **`BuildAllowedConvertToFormatIndices`** (include format **i** iff some selected path fails **`FormatIndexMatchesExtension(..., i)`**).

### 1.6 Preview reload

- **`ReloadSourcePreviewsAsync`**, **`ReplacePreviewLoadCancellationAsync`** (**`CancelAsync`**), **`EnumeratePreviewFilesAsync`**, **`LoadPreviewThumbnailBatches`** (batch **30**), **`FinalizePreviewListReload`**.

### 1.7 Conversion batch

- **`ReplaceConversionCancellationAsync`**, **`Task.Run`** **`BatchConversionRunner.Run`**, **`ApplyConversionResultAndReloadAsync`** (undo registration, **`ReloadSourcePreviewsAsync`**).

### 1.8 Busy state

- **`SetConversionBusy`:** disables browse, paths, ICO combos, solid color, preview, list; **`UpdateUndoButtonEnabled`**; **`btnCancel`** when busy. **No** format combos.

### 1.9 Destination actions

- **`OpenDestinationForSelection`:** **`Process.Start`** destination folder path only.

### 1.10 Persistence

- **`PersistSettings`** / **`SchedulePersistUiSettings`** (**400 ms**): mirrors **`AppSettings`**—folders, **`PreviewThumbnailSizeIndex`**, ICO size, solid color, window bounds/maximized, splitter distance.
- **`AppSettings`:** no format-index field; conversion target is never persisted.
- **`AppSettingsStore`:** **`Load`/`Save`** keys listed in [requirements.md](./requirements.md) Section 4.

---

## 2. Libraries

| File | Role |
|------|------|
| `ImageConversion` | Magick paths; reject identical src/dst full path |
| `BatchConversionRunner` | Loop + **`SuccessfulDestinationPaths`** |
| `SupportedFormats` | Indices, **`BuildDestinationPath`**, **`FormatIndexMatchesExtension`**, **`TryGetFormatIndexForPath`** (defined; not used by current `frmMain`—menu logic uses **`FormatIndexMatchesExtension`**) |
| `ConversionRequest`, `IconBackgroundKind` | DTO / enum |
| `FolderThumbnailLoader` | Enumerate + thumbnails |
| `AppSettings` / `AppSettingsStore` | Settings; **`internal sealed`** |

### Dependencies

**Magick.NET-Q16-AnyCPU** **14.12.0**; nullable + implicit usings.

### ICO write path

**`ApplySquareIconCanvas`** → **`WriteIcoWithPngNormalization`** (PNG32 round-trip).

---

## 3. Snapshot

| Area | Status |
|------|--------|
| Menu-driven convert + quick ICO + redundant-format suppression | Implemented |
| Identical-path rejection in **`ImageConversion`** | Implemented |
| Drive-root guard (dest + paste) | Implemented |
| Legacy combo-based UI | Removed |
