using System.ComponentModel;
using System.Diagnostics;

namespace ImageConverter
{
    /// <summary>
    /// Main form: presentation and orchestration; delegates conversion and settings to dedicated types.
    /// </summary>
    public partial class frmMain : Form
    {
        private System.Windows.Forms.Timer? _layoutPersistTimer;
        private bool _loadingUiSettings;
        private bool _splitterDistanceAppliedOnce;
        private bool _applyingSavedSplitterDistance;

        private static readonly string[] SupportedFormatLabels =
        [
            "JPEG (.jpg / .jpeg)",
            "PNG (.png)",
            "BMP (.bmp)",
            "GIF (.gif)",
            "WEBP (.webp)",
            "ICO (.ico)"
        ];

        /// <summary>Pixel dimensions matching <see cref="cmbIcoOutputSize"/> items (16 … 256).</summary>
        private static readonly int[] IcoOutputSizeValues = [16, 32, 48, 64, 128, 256];

        private readonly AppSettingsStore _settingsStore = new();
        private AppSettings _settings = new();

        private CancellationTokenSource? _previewLoadCts;
        private CancellationTokenSource? _conversionCts;

        private Func<bool>? _undoLastOperation;
        private bool _conversionBusy;
        private string? _pendingBoldFileNameFromConvert;

        public frmMain()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _settings = _settingsStore.Load();
            WireEvents();
            PopulatePreviewSizeCombo();
            ApplySettingsToUi();
            UpdatePlaceholderVisibility();
            SetStatusMessage("Ready");
            _ = InitialPreviewLoadAsync();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            SchedulePersistUiSettings();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BeginInvoke(ApplySavedSplitterDistanceOnce);
        }

        private void WireEvents()
        {
            btnBrowseSource.Click += async (_, _) => await BrowseForFolderAsync(isSource: true);
            btnBrowseDest.Click += async (_, _) => await BrowseForFolderAsync(isSource: false);
            listViewPreview.ItemSelectionChanged += (_, _) =>
            {
                RefreshSelectionStatusText();
            };
            btnRefreshPreview.Click += async (_, _) => await ReloadSourcePreviewsAsync();
            btnOpenDestination.Click += (_, _) => OpenDestinationForSelection();
            contextMenuPreview.Opening += PreviewContextMenuStrip_Opening;
            toolStripMenuItemPreviewCopy.Click += async (_, _) => await CopySelectedPreviewFilesToClipboardAsync();
            toolStripMenuItemPreviewPaste.Click += async (_, _) => await PasteClipboardImageIntoSourceFolderAsync();
            toolStripMenuItemPreviewDelete.Click += async (_, _) => await DeleteSelectedPreviewFilesAsync();
            toolStripMenuItemOpenSourceLocation.Click += (_, _) => OpenSelectedSourceFileLocation();
            toolStripMenuItemConvertToQuickIcon.Click += async (_, _) =>
            {
                try
                {
                    await ConvertSelectionToFormatAsync(SupportedFormats.Count - 1, iconQuickAccess: true);
                }
                catch (Exception ex)
                {
                    SetStatusMessage("Conversion error: " + ex.Message);
                }
            };
            cmbPreviewSize.SelectedIndexChanged += async (_, _) =>
            {
                if (_loadingUiSettings)
                {
                    return;
                }

                ApplyPreviewTileLayoutFromPixelSize(GetPreviewThumbnailPixelSize());
                SchedulePersistUiSettings();
                await ReloadSourcePreviewsAsync();
            };
            cmbIcoOutputSize.SelectedIndexChanged += (_, _) =>
            {
                if (!_loadingUiSettings)
                {
                    SchedulePersistUiSettings();
                }
            };
            cmbSolidColor.SelectedIndexChanged += (_, _) =>
            {
                if (!_loadingUiSettings)
                {
                    SchedulePersistUiSettings();
                }
            };
            splitReview.SplitterMoved += (_, _) =>
            {
                if (!_applyingSavedSplitterDistance)
                {
                    SchedulePersistUiSettings();
                }
            };
            listViewPreview.DragEnter += ListViewPreview_DragEnter;
            listViewPreview.DragOver += ListViewPreview_DragOver;
            listViewPreview.DragDrop += async (_, e) => await ListViewPreview_DragDropAsync(e);
            lblPreviewPlaceholder.DragEnter += ListViewPreview_DragEnter;
            lblPreviewPlaceholder.DragOver += ListViewPreview_DragOver;
            lblPreviewPlaceholder.DragDrop += async (_, e) => await ListViewPreview_DragDropAsync(e);

            btnCancel.Click += (_, _) => _conversionCts?.Cancel();
            btnUndo.Click += async (_, _) => await UndoLastOperationAsync();
        }

        private void PopulatePreviewSizeCombo()
        {
            cmbPreviewSize.Items.Clear();
            cmbPreviewSize.Items.AddRange(new object[] { "Small", "Medium", "Large" });
            if (cmbPreviewSize.Items.Count > 0)
            {
                cmbPreviewSize.SelectedIndex = Math.Min(1, cmbPreviewSize.Items.Count - 1);
            }
        }

        private void ApplySettingsToUi()
        {
            _loadingUiSettings = true;
            try
            {
                if (!string.IsNullOrWhiteSpace(_settings.LastSourceFolder) && Directory.Exists(_settings.LastSourceFolder))
                {
                    txtSourceFolder.Text = _settings.LastSourceFolder;
                }

                if (!string.IsNullOrWhiteSpace(_settings.LastDestinationFolder) && Directory.Exists(_settings.LastDestinationFolder))
                {
                    txtDestFolder.Text = _settings.LastDestinationFolder;
                }
                else
                {
                    var src = txtSourceFolder.Text.Trim();
                    if (!string.IsNullOrEmpty(src) && Directory.Exists(src))
                    {
                        txtDestFolder.Text = src;
                    }
                }

                var previewIdx = Math.Clamp(_settings.PreviewThumbnailSizeIndex, 0, cmbPreviewSize.Items.Count > 0 ? cmbPreviewSize.Items.Count - 1 : 0);
                if (cmbPreviewSize.Items.Count > 0)
                {
                    cmbPreviewSize.SelectedIndex = previewIdx;
                }

                ApplyPreviewTileLayoutFromPixelSize(GetPreviewThumbnailPixelSize());

                var icoIdx = Math.Clamp(_settings.IcoOutputSizeIndex, 0, IcoOutputSizeValues.Length - 1);
                if (cmbIcoOutputSize.Items.Count > 0)
                {
                    cmbIcoOutputSize.SelectedIndex = icoIdx;
                }

                var solidIdx = Math.Clamp(_settings.SolidColorIndex, 0, Math.Max(0, cmbSolidColor.Items.Count - 1));
                if (cmbSolidColor.Items.Count > 0)
                {
                    cmbSolidColor.SelectedIndex = solidIdx;
                }

                if (_settings.MainWindowPlacementSaved)
                {
                    var w = Math.Max(MinimumSize.Width, _settings.MainWindowWidth);
                    var h = Math.Max(MinimumSize.Height, _settings.MainWindowHeight);
                    StartPosition = FormStartPosition.Manual;
                    Location = new Point(_settings.MainWindowLeft, _settings.MainWindowTop);
                    Size = new Size(w, h);
                    WindowState = _settings.MainWindowMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
                }

            }
            finally
            {
                _loadingUiSettings = false;
            }
        }

        private void PersistSettings()
        {
            _settings.LastSourceFolder = txtSourceFolder.Text.Trim();
            _settings.LastDestinationFolder = txtDestFolder.Text.Trim();
            _settings.PreviewThumbnailSizeIndex = cmbPreviewSize.SelectedIndex >= 0 ? Math.Clamp(cmbPreviewSize.SelectedIndex, 0, 2) : 1;
            _settings.IcoOutputSizeIndex = cmbIcoOutputSize.SelectedIndex >= 0 ? Math.Clamp(cmbIcoOutputSize.SelectedIndex, 0, IcoOutputSizeValues.Length - 1) : 5;
            _settings.SolidColorIndex = cmbSolidColor.SelectedIndex >= 0 ? Math.Clamp(cmbSolidColor.SelectedIndex, 0, 1) : 0;
            _settings.MainWindowPlacementSaved = true;

            var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
            _settings.MainWindowLeft = bounds.Left;
            _settings.MainWindowTop = bounds.Top;
            _settings.MainWindowWidth = bounds.Width;
            _settings.MainWindowHeight = bounds.Height;
            _settings.MainWindowMaximized = WindowState == FormWindowState.Maximized;

            if (splitReview.IsHandleCreated)
            {
                _settings.PreviewSplitterDistance = splitReview.SplitterDistance;
            }

            _settingsStore.Save(_settings);
        }

        private void SchedulePersistUiSettings()
        {
            if (_layoutPersistTimer == null)
            {
                _layoutPersistTimer = new System.Windows.Forms.Timer { Interval = 400 };
                _layoutPersistTimer.Tick += (_, _) =>
                {
                    _layoutPersistTimer!.Stop();
                    PersistSettings();
                };
            }

            _layoutPersistTimer.Stop();
            _layoutPersistTimer.Start();
        }

        private static int GetPreviewPixelSizeForIndex(int index)
        {
            return index switch
            {
                0 => 80,
                1 => 128,
                2 => 192,
                _ => 128
            };
        }

        private int GetPreviewThumbnailPixelSize()
        {
            var idx = cmbPreviewSize.SelectedIndex;
            if (idx < 0)
            {
                idx = 1;
            }

            idx = Math.Clamp(idx, 0, 2);
            return GetPreviewPixelSizeForIndex(idx);
        }

        private void ApplyPreviewTileLayoutFromPixelSize(int px)
        {
            imageListThumbnails.ImageSize = new Size(px, px);

            // Tile view packs icon + wrapped filename into TileSize; if height is too small (common with "Small"
            // thumbnails + larger fonts / DPI), comctl clips from the top/bottom. Reserve space from font metrics.
            const int horizontalPad = 16;
            var tileW = Math.Max(px + horizontalPad, 96);

            var lineH = TextRenderer.MeasureText("Aygjp", listViewPreview.Font, Size.Empty,
                TextFormatFlags.NoPadding).Height;
            // Up to 3 text lines: long name wrap, or name + “( icon)” on its own line.
            var textBand = lineH * 3 + 12;
            const int iconToTextGap = 10;
            const int verticalChrome = 14;

            var tileH = px + iconToTextGap + textBand + verticalChrome;

            listViewPreview.TileSize = new Size(tileW, tileH);
            listViewPreview.Padding = new Padding(6, 8, 6, 8);
        }

        private void ApplySavedSplitterDistanceOnce()
        {
            if (_splitterDistanceAppliedOnce || !splitReview.IsHandleCreated)
            {
                return;
            }

            _splitterDistanceAppliedOnce = true;
            if (_settings.PreviewSplitterDistance <= 0)
            {
                return;
            }

            try
            {
                var max = splitReview.Height - splitReview.Panel2MinSize - splitReview.SplitterWidth;
                if (max < splitReview.Panel1MinSize)
                {
                    return;
                }

                var d = Math.Clamp(_settings.PreviewSplitterDistance, splitReview.Panel1MinSize, max);
                _applyingSavedSplitterDistance = true;
                try
                {
                    splitReview.SplitterDistance = d;
                }
                finally
                {
                    _applyingSavedSplitterDistance = false;
                }
            }
            catch
            {
                // Ignore invalid splitter during early layout.
            }
        }

        private void OpenSelectedSourceFileLocation()
        {
            var paths = GetSelectedSourcePaths();
            if (paths.Count == 0)
            {
                return;
            }

            try
            {
                var path = paths[0];
                if (!File.Exists(path))
                {
                    SetStatusMessage("Selected file was not found on disk.");
                    return;
                }

                var explorer = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
                Process.Start(new ProcessStartInfo
                {
                    FileName = explorer,
                    Arguments = $"/select,\"{path}\"",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                SetStatusMessage("Could not open file location: " + ex.Message);
            }
        }

        private async Task InitialPreviewLoadAsync()
        {
            var folder = txtSourceFolder.Text.Trim();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                return;
            }

            await ReloadSourcePreviewsAsync();
        }

        private async Task BrowseForFolderAsync(bool isSource)
        {
            folderBrowserDialog.InitialDirectory = GetInitialDirectoryHint(isSource);
            if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var path = folderBrowserDialog.SelectedPath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (isSource)
            {
                txtSourceFolder.Text = path;
                if (string.IsNullOrWhiteSpace(txtDestFolder.Text))
                {
                    txtDestFolder.Text = path;
                }

                await ReloadSourcePreviewsAsync();
            }
            else
            {
                txtDestFolder.Text = path;
                SetStatusMessage("Destination folder selected.");
            }
        }

        private string GetInitialDirectoryHint(bool isSource)
        {
            var current = isSource ? txtSourceFolder.Text : txtDestFolder.Text;
            if (!string.IsNullOrWhiteSpace(current) && Directory.Exists(current))
            {
                try
                {
                    return Path.GetFullPath(current);
                }
                catch
                {
                    // Fall through to a known-good default.
                }
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        private void ListViewPreview_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ListViewPreview_DragOver(object? sender, DragEventArgs e)
        {
            ListViewPreview_DragEnter(sender, e);
        }

        private async Task ListViewPreview_DragDropAsync(DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) != true)
            {
                SetStatusMessage("Drop was not a valid file or folder.");
                return;
            }

            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (paths is null || paths.Length == 0)
            {
                SetStatusMessage("Nothing was dropped.");
                return;
            }

            var first = paths[0];
            try
            {
                var attr = File.GetAttributes(first);
                var folder = attr.HasFlag(FileAttributes.Directory)
                    ? first
                    : Path.GetDirectoryName(first);

                if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                {
                    SetStatusMessage("Could not resolve a folder from the drop.");
                    return;
                }

                txtSourceFolder.Text = folder;
                if (string.IsNullOrWhiteSpace(txtDestFolder.Text))
                {
                    txtDestFolder.Text = folder;
                }

                await ReloadSourcePreviewsAsync();
            }
            catch (Exception ex)
            {
                SetStatusMessage("Drop could not be handled: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads thumbnails for the current source folder (top-level files only). Uses System.Drawing for preview; Magick.NET is for conversion later.
        /// </summary>
        /// <param name="selectFullPathsAfter">Optional full paths to select once the list is rebuilt (e.g. files created in the source folder).</param>
        /// <param name="statusOverride">When set, replaces the default Ready / count status after load.</param>
        private async Task ReloadSourcePreviewsAsync(IReadOnlyCollection<string>? selectFullPathsAfter = null, string? statusOverride = null)
        {
            await ReplacePreviewLoadCancellationAsync();

            var ct = _previewLoadCts!.Token;
            var folder = txtSourceFolder.Text.Trim();
            if (!IsValidPreviewSourceFolder(folder))
            {
                HandleInvalidPreviewSourceFolder();
                return;
            }

            SetStatusMessage("Loading previews…");
            List<string> files;
            try
            {
                files = await EnumeratePreviewFilesAsync(folder, ct).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (ct.IsCancellationRequested)
            {
                return;
            }

            BeginPreviewListReload();

            if (files.Count == 0)
            {
                FinishPreviewListReloadWithNoFiles();
                return;
            }

            var thumbPx = GetPreviewThumbnailPixelSize();
            var selectSet = BuildPreviewSelectionSet(selectFullPathsAfter);

            try
            {
                LoadPreviewThumbnailBatches(files, thumbPx, ct);
            }
            finally
            {
                FinalizePreviewListReload(selectSet, statusOverride);
            }
        }

        private async Task ReplacePreviewLoadCancellationAsync()
        {
            var previous = _previewLoadCts;
            _previewLoadCts = new CancellationTokenSource();
            if (previous is null)
            {
                return;
            }

            await previous.CancelAsync().ConfigureAwait(true);
            previous.Dispose();
        }

        private static bool IsValidPreviewSourceFolder(string folder)
        {
            return !string.IsNullOrEmpty(folder) && Directory.Exists(folder);
        }

        private void HandleInvalidPreviewSourceFolder()
        {
            RunOnUiThread(ClearPreviewList);
            UpdatePlaceholderVisibility();
            SetStatusMessage("Invalid source folder.");
        }

        private static Task<List<string>> EnumeratePreviewFilesAsync(string folder, CancellationToken ct)
        {
            return Task.Run(() => FolderThumbnailLoader.EnumerateImageFiles(folder), ct);
        }

        private void BeginPreviewListReload()
        {
            RunOnUiThread(() =>
            {
                listViewPreview.BeginUpdate();
                listViewPreview.Items.Clear();
                imageListThumbnails.Images.Clear();
            });
        }

        private void FinishPreviewListReloadWithNoFiles()
        {
            RunOnUiThread(() =>
            {
                listViewPreview.EndUpdate();
                UpdatePlaceholderVisibility();
                SetStatusMessage("No supported images in this folder.");
            });
        }

        private static HashSet<string>? BuildPreviewSelectionSet(IReadOnlyCollection<string>? selectFullPathsAfter)
        {
            if (selectFullPathsAfter is null || selectFullPathsAfter.Count == 0)
            {
                return null;
            }

            return new HashSet<string>(selectFullPathsAfter, StringComparer.OrdinalIgnoreCase);
        }

        private void LoadPreviewThumbnailBatches(IReadOnlyList<string> files, int thumbPx, CancellationToken ct)
        {
            const int batchSize = 30;
            for (var i = 0; i < files.Count; i += batchSize)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var slice = files.Skip(i).Take(batchSize).ToList();
                var batch = BuildThumbnailBatch(slice, thumbPx, ct);
                if (ct.IsCancellationRequested)
                {
                    DisposeThumbnailBitmaps(batch);
                    break;
                }

                AppendThumbnailBatchToListView(batch);
            }
        }

        private static List<(Bitmap? Bmp, string Name, string FullPath)> BuildThumbnailBatch(
            IReadOnlyList<string> slice,
            int thumbPx,
            CancellationToken ct)
        {
            var batch = new List<(Bitmap? Bmp, string Name, string FullPath)>();
            foreach (var f in slice)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var bmp = FolderThumbnailLoader.TryCreateThumbnail(f, thumbPx);
                batch.Add((bmp, Path.GetFileName(f), f));
            }

            return batch;
        }

        private static void DisposeThumbnailBitmaps(IEnumerable<(Bitmap? Bmp, string Name, string FullPath)> batch)
        {
            foreach (var (bmp, _, _) in batch)
            {
                bmp?.Dispose();
            }
        }

        private void AppendThumbnailBatchToListView(IReadOnlyList<(Bitmap? Bmp, string Name, string FullPath)> batch)
        {
            RunOnUiThread(() =>
            {
                foreach (var (bmp, name, fullPath) in batch)
                {
                    if (bmp is null)
                    {
                        continue;
                    }

                    imageListThumbnails.Images.Add(bmp);
                    var idx = imageListThumbnails.Images.Count - 1;
                    var displayLabel = SupportedFormats.FormatIndexMatchesExtension(fullPath, 5)
                        ? $"{name}{Environment.NewLine}( icon)"
                        : name;
                    listViewPreview.Items.Add(new ListViewItem(displayLabel, idx) { Tag = fullPath });
                }
            });
        }

        private void FinalizePreviewListReload(HashSet<string>? selectSet, string? statusOverride)
        {
            RunOnUiThread(() =>
            {
                ApplyPreviewSelectionAfterReload(selectSet);
                listViewPreview.EndUpdate();
                UpdatePlaceholderVisibility();
                var defaultReady = listViewPreview.Items.Count == 0
                    ? "No images could be previewed."
                    : $"Ready — {listViewPreview.Items.Count} image(s). Right-click a thumbnail to convert.";
                SetStatusMessage(statusOverride ?? defaultReady);
            });
        }

        private void ApplyPreviewSelectionAfterReload(HashSet<string>? selectSet)
        {
            if (listViewPreview.Items.Count == 0)
            {
                return;
            }

            if (selectSet is not null)
            {
                listViewPreview.SelectedItems.Clear();
                foreach (ListViewItem item in listViewPreview.Items)
                {
                    if (item.Tag is string p && selectSet.Contains(p))
                    {
                        item.Selected = true;
                    }
                }
            }

            if (listViewPreview.SelectedItems.Count == 0)
            {
                listViewPreview.Items[0].Selected = true;
                listViewPreview.Items[0].Focused = true;
                return;
            }

            listViewPreview.FocusedItem = listViewPreview.SelectedItems[0];
            listViewPreview.SelectedItems[0].EnsureVisible();
        }

        private void ClearPreviewList()
        {
            listViewPreview.BeginUpdate();
            listViewPreview.Items.Clear();
            imageListThumbnails.Images.Clear();
            listViewPreview.EndUpdate();
        }

        private void RunOnUiThread(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>Formats that are not redundant for every selected path (omit “convert to same type”).</summary>
        private static List<int> BuildAllowedConvertToFormatIndices(IReadOnlyList<string> paths)
        {
            var list = new List<int>();
            for (var i = 0; i < SupportedFormats.Count; i++)
            {
                if (paths.Any(p => !SupportedFormats.FormatIndexMatchesExtension(p, i)))
                {
                    list.Add(i);
                }
            }

            return list;
        }

        private static List<string> SelectPathsNeedingTargetFormat(IReadOnlyList<string> paths, int targetFormatIndex)
        {
            return paths.Where(p => !SupportedFormats.FormatIndexMatchesExtension(p, targetFormatIndex)).ToList();
        }

        /// <summary>Single selected ICO square dimension in pixels.</summary>
        private int GetSelectedIcoOutputSize()
        {
            var idx = cmbIcoOutputSize.SelectedIndex;
            if (idx < 0 || idx >= IcoOutputSizeValues.Length)
            {
                return 256;
            }

            return IcoOutputSizeValues[idx];
        }

        private void UpdatePlaceholderVisibility()
        {
            lblPreviewPlaceholder.Visible = listViewPreview.Items.Count == 0;
            if (lblPreviewPlaceholder.Visible)
            {
                lblPreviewPlaceholder.BringToFront();
            }
        }

        /// <summary>True when source/destination folders exist and at least one thumbnail is selected.</summary>
        private bool CanConvertSelection()
        {
            var pathsOk = !string.IsNullOrWhiteSpace(txtSourceFolder.Text)
                && Directory.Exists(txtSourceFolder.Text.Trim())
                && !string.IsNullOrWhiteSpace(txtDestFolder.Text)
                && Directory.Exists(txtDestFolder.Text.Trim());

            return pathsOk && listViewPreview.SelectedItems.Count > 0;
        }

        private IconBackgroundKind GetIconBackgroundFromUi()
        {
            return cmbSolidColor.SelectedIndex == 1 ? IconBackgroundKind.SolidBlack : IconBackgroundKind.SolidWhite;
        }

        private void SetConversionBusy(bool busy)
        {
            _conversionBusy = busy;
            btnBrowseSource.Enabled = !busy;
            btnBrowseDest.Enabled = !busy;
            txtSourceFolder.Enabled = !busy;
            txtDestFolder.Enabled = !busy;
            cmbIcoOutputSize.Enabled = !busy;
            cmbSolidColor.Enabled = !busy;
            btnRefreshPreview.Enabled = !busy;
            btnOpenDestination.Enabled = !busy;
            cmbPreviewSize.Enabled = !busy;
            listViewPreview.Enabled = !busy;
            UpdateUndoButtonEnabled();
            btnCancel.Enabled = busy;
        }

        /// <param name="iconQuickAccess">When true and target is ICO, convert every selected file (including existing .ico for rebuild).</param>
        private async Task ConvertSelectionToFormatAsync(int outputFormatIndex, bool iconQuickAccess)
        {
            if (outputFormatIndex < 0 || outputFormatIndex >= SupportedFormats.Count)
            {
                return;
            }

            var destFolder = txtDestFolder.Text.Trim();
            if (!EnsureDestinationFolderExists(destFolder))
            {
                return;
            }

            var paths = GetSelectedSourcePaths();
            if (paths.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Select one or more images in the preview list.",
                    "Convert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<string> pathsToConvert;
            var icoIndex = SupportedFormats.Count - 1;
            if (iconQuickAccess && outputFormatIndex == icoIndex)
            {
                pathsToConvert = paths;
            }
            else
            {
                pathsToConvert = SelectPathsNeedingTargetFormat(paths, outputFormatIndex);
                if (pathsToConvert.Count == 0)
                {
                    MessageBox.Show(
                        this,
                        $"Every selected file is already {SupportedFormatLabels[outputFormatIndex]}. There is nothing to convert.",
                        "Convert",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            await RunBatchConversionAndRefreshAsync(pathsToConvert, destFolder, outputFormatIndex).ConfigureAwait(true);
        }

        private bool EnsureDestinationFolderExists(string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                MessageBox.Show(
                    this,
                    "The destination folder does not exist or is not accessible.",
                    "Convert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                SetStatusMessage("Invalid destination folder.");
                return false;
            }

            if (IsDriveRootFolderPath(destFolder))
            {
                MessageBox.Show(
                    this,
                    "The destination cannot be the root of a drive (for example C:\\).\n\n" +
                    "Windows blocks creating files directly under C:\\ for normal (non-elevated) apps. " +
                    "Use Browse to choose a folder inside your profile (for example Documents or Pictures).",
                    "Destination not allowed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                SetStatusMessage("Pick a subfolder — not C:\\.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// True when <paramref name="folderPath"/> is a local volume root such as <c>C:\</c>.
        /// Writing output files there typically fails with “permission denied” unless the process is elevated.
        /// </summary>
        private static bool IsDriveRootFolderPath(string folderPath)
        {
            try
            {
                var full = Path.GetFullPath(folderPath.Trim());
                // UNC roots are ambiguous; rely on existence checks elsewhere.
                if (full.StartsWith(@"\\", StringComparison.Ordinal))
                {
                    return false;
                }

                var root = Path.GetPathRoot(full);
                if (string.IsNullOrEmpty(root))
                {
                    return false;
                }

                var normalizedFolder = full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var normalizedRoot = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return string.Equals(normalizedFolder, normalizedRoot, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private async Task ReplaceConversionCancellationAsync()
        {
            var previous = _conversionCts;
            _conversionCts = new CancellationTokenSource();
            if (previous is null)
            {
                return;
            }

            await previous.CancelAsync().ConfigureAwait(true);
            previous.Dispose();
        }

        private void ShowConversionCompletedWithErrorsDialog(int failCount)
        {
            MessageBox.Show(
                this,
                $"{failCount} file(s) could not be converted. Check paths, permissions, and formats.\nSee the status bar for the summary.",
                "Conversion completed with errors",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private List<string> GetSuccessfulOutputsExistingOnDisk(BatchConversionRunner.RunResult batchResult)
        {
            return batchResult.SuccessfulDestinationPaths.Where(File.Exists).ToList();
        }

        private List<string> FilterOutputsUnderSourceFolder(List<string> createdOutputs)
        {
            var sourceFolder = txtSourceFolder.Text.Trim();
            var normalizedSource = Path.TrimEndingDirectorySeparator(Path.GetFullPath(sourceFolder));
            return createdOutputs
                .Where(p =>
                {
                    var dir = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.GetDirectoryName(p) ?? ""));
                    return dir.Equals(normalizedSource, StringComparison.OrdinalIgnoreCase);
                })
                .ToList();
        }

        private void UpdatePendingBoldForOutputsNotInReview(List<string> createdOutputs, List<string> inSourceFolder)
        {
            _pendingBoldFileNameFromConvert = null;
            if (inSourceFolder.Count != 0 || createdOutputs.Count == 0)
            {
                return;
            }

            _pendingBoldFileNameFromConvert = createdOutputs.Count == 1
                ? Path.GetFileName(createdOutputs[0])
                : $"{Path.GetFileName(createdOutputs[0])} (+{createdOutputs.Count - 1})";
        }

        private void RegisterUndoForSuccessfulOutputs(BatchConversionRunner.RunResult batchResult, List<string> createdOutputs)
        {
            if (batchResult.SuccessCount <= 0 || createdOutputs.Count == 0)
            {
                return;
            }

            RegisterUndo(() =>
            {
                foreach (var p in createdOutputs)
                {
                    try
                    {
                        if (File.Exists(p))
                        {
                            File.Delete(p);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        private async Task ApplyConversionResultAndReloadAsync(BatchConversionRunner.RunResult batchResult)
        {
            var createdOutputs = GetSuccessfulOutputsExistingOnDisk(batchResult);
            var inSourceFolder = FilterOutputsUnderSourceFolder(createdOutputs);
            UpdatePendingBoldForOutputsNotInReview(createdOutputs, inSourceFolder);

            if (batchResult.FailCount > 0)
            {
                ShowConversionCompletedWithErrorsDialog(batchResult.FailCount);
            }

            RegisterUndoForSuccessfulOutputs(batchResult, createdOutputs);

            var summary = batchResult.FailCount == 0
                ? $"Conversion finished — {batchResult.SuccessCount} file(s) converted."
                : $"Conversion finished — {batchResult.SuccessCount} succeeded, {batchResult.FailCount} failed.";
            await ReloadSourcePreviewsAsync(inSourceFolder.Count > 0 ? inSourceFolder : null, summary).ConfigureAwait(true);
        }

        private async Task RunBatchConversionAndRefreshAsync(
            IReadOnlyList<string> pathsToConvert,
            string destFolder,
            int toIndex)
        {
            var icoSquareSize = GetSelectedIcoOutputSize();
            var iconBackground = GetIconBackgroundFromUi();

            await ReplaceConversionCancellationAsync().ConfigureAwait(true);
            var ct = _conversionCts!.Token;
            IProgress<(int Current, int Total, string FileName)> uiProgress = new Progress<(int Current, int Total, string FileName)>(p =>
            {
                toolStripProgressBatch.Value = p.Current;
                SetStatusMessage($"Converting {p.Current} of {p.Total}: {p.FileName}");
            });

            SetConversionBusy(true);
            toolStripProgressBatch.Visible = true;
            toolStripProgressBatch.Minimum = 0;
            toolStripProgressBatch.Maximum = Math.Max(1, pathsToConvert.Count);
            toolStripProgressBatch.Value = 0;
            UseWaitCursor = true;

            BatchConversionRunner.RunResult? batchResult = null;
            try
            {
                batchResult = await Task.Run(
                        () => BatchConversionRunner.Run(
                            pathsToConvert,
                            destFolder,
                            toIndex,
                            icoSquareSize,
                            iconBackground,
                            ct,
                            uiProgress),
                        ct)
                    .ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                SetStatusMessage("Conversion cancelled.");
                return;
            }
            finally
            {
                UseWaitCursor = false;
                toolStripProgressBatch.Visible = false;
                SetConversionBusy(false);
            }

            if (batchResult is null)
            {
                SetStatusMessage("Conversion did not complete.");
                return;
            }

            await ApplyConversionResultAndReloadAsync(batchResult).ConfigureAwait(true);
        }

        /// <summary>Paste image from clipboard into the source folder as PNG, then refresh review.</summary>
        private async Task PasteClipboardImageIntoSourceFolderAsync()
        {
            if (!Clipboard.ContainsImage())
            {
                MessageBox.Show(
                    this,
                    "The clipboard does not contain an image.",
                    "Paste",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var folder = txtSourceFolder.Text.Trim();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show(
                    this,
                    "Choose a valid source folder before pasting.",
                    "Paste",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (IsDriveRootFolderPath(folder))
            {
                MessageBox.Show(
                    this,
                    "Cannot paste into the root of a drive (for example C:\\).\n\n" +
                    "Choose a subfolder under your profile or another folder, then paste again.",
                    "Paste",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var img = Clipboard.GetImage();
                if (img == null)
                {
                    MessageBox.Show(this, "Could not read the image from the clipboard.", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var name = $"Pasted_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var path = Path.Combine(folder, name);
                img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                RegisterUndo(() =>
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });
                await ReloadSourcePreviewsAsync([path], $"Pasted image saved as {name}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Paste failed: " + ex.Message, "Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Copy selected file(s) as images to the clipboard, then refresh review.</summary>
        private async Task CopySelectedPreviewFilesToClipboardAsync()
        {
            var paths = GetSelectedSourcePaths();
            if (paths.Count == 0)
            {
                MessageBox.Show(this, "Select one or more images in the review list.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (paths.Count == 1)
                {
                    using var bmp = new Bitmap(paths[0]);
                    Clipboard.SetImage(new Bitmap(bmp));
                }
                else
                {
                    var files = new System.Collections.Specialized.StringCollection();
                    files.AddRange(paths.ToArray());
                    Clipboard.SetFileDropList(files);
                }

                var copyMsg = paths.Count == 1 ? "Copied image to clipboard." : $"Copied {paths.Count} file paths to clipboard.";
                await ReloadSourcePreviewsAsync(null, copyMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Copy failed: " + ex.Message, "Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteSelectedPreviewFilesAsync()
        {
            var paths = GetSelectedSourcePaths();
            if (paths.Count == 0)
            {
                MessageBox.Show(this, "Select one or more images to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var preview = paths.Count == 1
                ? paths[0]
                : $"{paths.Count} files";
            if (MessageBox.Show(
                    this,
                    $"Permanently delete from disk?\n\n{preview}",
                    "Confirm delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            var sessionDir = Path.Combine(Path.GetTempPath(), "ImageConverterUndo", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(sessionDir);
            var movedPairs = new List<(string Orig, string Temp)>();
            try
            {
                for (var i = 0; i < paths.Count; i++)
                {
                    var p = paths[i];
                    var temp = Path.Combine(sessionDir, $"{i:000}_{Path.GetFileName(p)}");
                    File.Move(p, temp);
                    movedPairs.Add((p, temp));
                }
            }
            catch (Exception ex)
            {
                foreach (var (orig, temp) in movedPairs)
                {
                    try
                    {
                        if (File.Exists(temp) && !File.Exists(orig))
                        {
                            File.Move(temp, orig);
                        }
                    }
                    catch
                    {
                        // best effort rollback
                    }
                }

                try
                {
                    Directory.Delete(sessionDir, true);
                }
                catch
                {
                    // ignore
                }

                MessageBox.Show(this, "Could not move files for delete: " + ex.Message, "Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RegisterUndo(() => TryUndoDeleteMoves(sessionDir, movedPairs));

            await ReloadSourcePreviewsAsync(null, "Deleted selected file(s).");
        }

        private static bool TryUndoDeleteMoves(string sessionDir, IReadOnlyList<(string Orig, string Temp)> pairs)
        {
            foreach (var (orig, temp) in pairs)
            {
                try
                {
                    if (!File.Exists(temp))
                    {
                        continue;
                    }

                    if (File.Exists(orig))
                    {
                        return false;
                    }

                    File.Move(temp, orig);
                }
                catch
                {
                    return false;
                }
            }

            try
            {
                if (Directory.Exists(sessionDir))
                {
                    Directory.Delete(sessionDir, true);
                }
            }
            catch
            {
                // ignore cleanup failure if files are restored
            }

            return true;
        }

        private void RegisterUndo(Func<bool> undo)
        {
            _undoLastOperation = undo;
            UpdateUndoButtonEnabled();
        }

        private void UpdateUndoButtonEnabled()
        {
            btnUndo.Enabled = !_conversionBusy && _undoLastOperation != null;
        }

        private async Task UndoLastOperationAsync()
        {
            if (_undoLastOperation == null || _conversionBusy)
            {
                return;
            }

            var op = _undoLastOperation;
            bool ok;
            try
            {
                ok = op.Invoke();
            }
            catch
            {
                ok = false;
            }

            if (!ok)
            {
                MessageBox.Show(
                    this,
                    "Undo could not complete. Another application may have the files open, or a file already exists at the original location.",
                    "Undo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _undoLastOperation = null;
            UpdateUndoButtonEnabled();
            await ReloadSourcePreviewsAsync(null, "Undid last operation.");
        }

        private void RefreshSelectionStatusText()
        {
            if (InvokeRequired)
            {
                BeginInvoke(RefreshSelectionStatusText);
                return;
            }

            if (listViewPreview.SelectedItems.Count > 0)
            {
                _pendingBoldFileNameFromConvert = null;
                statusLabelSelection.Visible = true;
                var firstPath = listViewPreview.SelectedItems[0].Tag as string ?? "";
                var name = Path.GetFileName(firstPath);
                statusLabelSelection.Text = listViewPreview.SelectedItems.Count == 1
                    ? name
                    : $"{name} (+{listViewPreview.SelectedItems.Count - 1} more)";
                return;
            }

            if (!string.IsNullOrEmpty(_pendingBoldFileNameFromConvert))
            {
                statusLabelSelection.Text = _pendingBoldFileNameFromConvert;
                statusLabelSelection.Visible = true;
                return;
            }

            statusLabelSelection.Text = "";
            statusLabelSelection.Visible = false;
        }

        private void OpenDestinationForSelection()
        {
            var destFolder = txtDestFolder.Text.Trim();
            if (string.IsNullOrEmpty(destFolder) || !Directory.Exists(destFolder))
            {
                MessageBox.Show(this, "Destination folder is missing or invalid.", "Open destination", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = destFolder,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not open Explorer: " + ex.Message, "Open destination", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetSelectedSourcePaths()
        {
            return listViewPreview.SelectedItems.Cast<ListViewItem>()
                .Select(i => i.Tag as string)
                .Where(static s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .ToList();
        }

        /// <summary>
        /// Selected thumbnail: Copy + Delete. Empty area or unselected thumbnail / placeholder: Paste only (when clipboard has an image).
        /// </summary>
        private void PreviewContextMenuStrip_Opening(object? sender, CancelEventArgs e)
        {
            HideAllPreviewContextMenuItems();

            var src = contextMenuPreview.SourceControl;
            if (TryOfferPasteOnlyPlaceholderMenu(src, e))
            {
                return;
            }

            if (src != listViewPreview)
            {
                e.Cancel = true;
                return;
            }

            var pt = listViewPreview.PointToClient(Cursor.Position);
            var hit = listViewPreview.HitTest(pt);

            if (hit.Item != null && hit.Item.Selected)
            {
                PopulatePreviewMenuForSelectedThumbnails();
                return;
            }

            OfferPasteOnReviewEmptyArea(e);
        }

        private void HideAllPreviewContextMenuItems()
        {
            toolStripMenuItemPreviewCopy.Visible = false;
            toolStripMenuItemPreviewPaste.Visible = false;
            toolStripMenuItemPreviewConvertTo.Visible = false;
            toolStripMenuItemConvertToQuickIcon.Visible = false;
            toolStripMenuItemPreviewDelete.Visible = false;
            toolStripMenuItemOpenSourceLocation.Visible = false;
        }

        /// <returns>True if the placeholder branch handled the menu (caller should return).</returns>
        private bool TryOfferPasteOnlyPlaceholderMenu(Control? src, CancelEventArgs e)
        {
            if (src != lblPreviewPlaceholder)
            {
                return false;
            }

            toolStripMenuItemPreviewPaste.Visible = true;
            toolStripMenuItemPreviewPaste.Enabled = Clipboard.ContainsImage();
            if (!toolStripMenuItemPreviewPaste.Enabled)
            {
                e.Cancel = true;
            }

            return true;
        }

        private void PopulatePreviewMenuForSelectedThumbnails()
        {
            toolStripMenuItemPreviewCopy.Visible = true;
            toolStripMenuItemPreviewConvertTo.Visible = true;
            toolStripMenuItemPreviewDelete.Visible = true;
            toolStripMenuItemOpenSourceLocation.Visible = true;
            toolStripMenuItemPreviewCopy.Enabled = listViewPreview.SelectedItems.Count > 0;
            toolStripMenuItemPreviewDelete.Enabled = listViewPreview.SelectedItems.Count > 0;
            toolStripMenuItemOpenSourceLocation.Enabled = listViewPreview.SelectedItems.Count > 0;

            var paths = GetSelectedSourcePaths();
            var convertOk = CanConvertSelection() && !_conversionBusy;
            var icoFormatIndex = SupportedFormats.Count - 1;
            var showQuickConvertToIcon = paths.Exists(p => !SupportedFormats.FormatIndexMatchesExtension(p, icoFormatIndex));
            toolStripMenuItemConvertToQuickIcon.Visible = showQuickConvertToIcon;
            toolStripMenuItemConvertToQuickIcon.Enabled = convertOk && showQuickConvertToIcon;

            toolStripMenuItemPreviewConvertTo.DropDownItems.Clear();
            foreach (var formatIdx in BuildAllowedConvertToFormatIndices(paths))
            {
                var captured = formatIdx;
                var sub = new ToolStripMenuItem(SupportedFormatLabels[captured])
                {
                    Enabled = convertOk
                };
                sub.Click += async (_, _) =>
                {
                    try
                    {
                        await ConvertSelectionToFormatAsync(captured, iconQuickAccess: false);
                    }
                    catch (Exception ex)
                    {
                        SetStatusMessage("Conversion error: " + ex.Message);
                    }
                };
                toolStripMenuItemPreviewConvertTo.DropDownItems.Add(sub);
            }

            toolStripMenuItemPreviewConvertTo.Enabled = convertOk && toolStripMenuItemPreviewConvertTo.DropDownItems.Count > 0;
        }

        private void OfferPasteOnReviewEmptyArea(CancelEventArgs e)
        {
            toolStripMenuItemPreviewPaste.Visible = true;
            toolStripMenuItemPreviewPaste.Enabled = Clipboard.ContainsImage();
            if (!toolStripMenuItemPreviewPaste.Enabled)
            {
                e.Cancel = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ActiveControl is TextBoxBase)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            if (ActiveControl is ComboBox && keyData == (Keys.Control | Keys.C))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            if (keyData == (Keys.Control | Keys.C) && listViewPreview.SelectedItems.Count > 0)
            {
                _ = CopySelectedPreviewFilesToClipboardAsync();
                return true;
            }

            if ((keyData == (Keys.Control | Keys.V) || keyData == (Keys.Control | Keys.P)) && Clipboard.ContainsImage())
            {
                _ = PasteClipboardImageIntoSourceFolderAsync();
                return true;
            }

            if (keyData == Keys.Delete && listViewPreview.SelectedItems.Count > 0)
            {
                _ = DeleteSelectedPreviewFilesAsync();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Z) && _undoLastOperation != null && !_conversionBusy)
            {
                _ = UndoLastOperationAsync();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetStatusMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => SetStatusMessage(message));
                return;
            }

            statusLabelMessage.Text = string.IsNullOrWhiteSpace(message) ? "Ready" : message;
            RefreshSelectionStatusText();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _layoutPersistTimer?.Stop();
            _layoutPersistTimer?.Dispose();
            _layoutPersistTimer = null;
            _conversionCts?.Cancel();
            _conversionCts?.Dispose();
            _conversionCts = null;
            PersistSettings();
            _previewLoadCts?.Cancel();
            _previewLoadCts?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
