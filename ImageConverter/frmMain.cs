using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using ImageConverter.Shell;

namespace ImageConverter
{
    public partial class frmMain : Form
    {
        private System.Windows.Forms.Timer? _layoutPersistTimer;
        private ToolTip? _previewItemToolTip;
        private bool _loadingUiSettings;
        private bool _splitterDistanceAppliedOnce;
        private bool _applyingSavedSplitterDistance;

        private static readonly int[] IcoOutputSizeValues = [16, 32, 48, 64, 128, 256];
        private static readonly Color PreviewItemSelectedBack = Color.FromArgb(229, 243, 255);
        private static readonly Color PreviewItemSelectedBorder = Color.FromArgb(0, 120, 215);

        private readonly AppSettingsStore _settingsStore = new();
        private AppSettings _settings = new();
        private bool _attemptedHklmScaleElevation;

        private CancellationTokenSource? _previewLoadCts;
        private CancellationTokenSource? _conversionCts;

        private Func<bool>? _undoLastOperation;
        private bool _conversionBusy;
        private int _selectionInfoVersion;

        public frmMain()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _settings = _settingsStore.Load();
            WireEvents();
            EnableControlDoubleBuffering(listViewPreview);
            PopulatePreviewSizeCombo();
            ApplySettingsToUi();
            SyncExplorerConvertMenuFromSettings();
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

        private void WireResizeMenuItem(ToolStripMenuItem item, double scaleFactor)
        {
            item.Click += async (_, _) =>
            {
                try
                {
                    await ResizeSelectionAsync(scaleFactor);
                }
                catch (Exception ex)
                {
                    SetStatusMessage("Resize error: " + ex.Message);
                }
            };
        }

        private void WireEvents()
        {
            menuFile.DropDownOpening += (_, _) => UpdateFileMenuState();
            menuFileOpenFolder.Click += async (_, _) => await BrowseForFolderAsync();
            menuFileRefreshReview.Click += async (_, _) => await ReloadSourcePreviewsAsync();
            menuFileOpenFolderInExplorer.Click += (_, _) => OpenImageFolderInExplorer();
            menuFileOpenAppLocation.Click += (_, _) => OpenAppLocationInExplorer();
            menuFilePasteImage.Click += async (_, _) => await PasteClipboardImageAsync();
            menuFileUndo.Click += async (_, _) => await UndoLastOperationAsync();
            menuFileExit.Click += (_, _) => Close();
            menuHelpHowToUse.Click += (_, _) => ShowHowToUseDialog();
            menuHelpSupport.Click += (_, _) => AppSupport.OpenBuyMeACoffee(this);
            menuHelpAbout.Click += (_, _) => ShowAboutDialog();

            btnBrowseSource.Click += async (_, _) => await BrowseForFolderAsync();
            listViewPreview.ItemSelectionChanged += (_, _) => RefreshSelectionStatusText();
            btnRefreshPreview.Click += async (_, _) => await ReloadSourcePreviewsAsync();
            btnOpenFolder.Click += (_, _) => OpenImageFolderInExplorer();
            contextMenuPreview.Opening += PreviewContextMenuStrip_Opening;
            toolStripMenuItemPreviewCopy.Click += async (_, _) => await CopySelectedPreviewFilesToClipboardAsync();
            toolStripMenuItemPreviewRename.Click += async (_, _) => await RenameSelectedPreviewFileAsync();
            toolStripMenuItemPreviewPaste.Click += async (_, _) => await PasteClipboardImageAsync();
            toolStripMenuItemPreviewDelete.Click += async (_, _) => await DeleteSelectedPreviewFilesAsync();
            toolStripMenuItemOpenSourceLocation.Click += (_, _) => OpenSelectedSourceFileLocation();
            toolStripMenuItemPreviewCopyImagePath.Click += (_, _) => CopySelectedImagePathsToClipboard();
            toolStripMenuItemOpenWithPaint.Click += (_, _) => OpenSelectedImageWithPaint();
            toolStripMenuItemOpenWithPaintDotNet.Click += (_, _) => OpenSelectedImageWithPaintDotNet();
            toolStripMenuItemConvertToQuickIcon.Click += async (_, _) =>
            {
                try
                {
                    await ConvertSelectionToFormatAsync(SupportedFormats.IcoFormatIndex, iconQuickAccess: true);
                }
                catch (Exception ex)
                {
                    SetStatusMessage("Conversion error: " + ex.Message);
                }
            };
            WireResizeMenuItem(toolStripMenuItemPreviewResize05x, 0.5);
            WireResizeMenuItem(toolStripMenuItemPreviewResize075x, 0.75);
            WireResizeMenuItem(toolStripMenuItemPreviewResize2x, 2);
            WireResizeMenuItem(toolStripMenuItemPreviewResize4x, 4);
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
            chkEnableExplorerConvertMenu.CheckedChanged += (_, _) =>
            {
                if (_loadingUiSettings)
                {
                    return;
                }

                _settings.EnableExplorerConvertMenu = chkEnableExplorerConvertMenu.Checked;
                SyncExplorerConvertMenuFromSettings();
                SchedulePersistUiSettings();
            };
            btnRefreshExplorerConverterMenu.Click += (_, _) => RefreshExplorerConverterMenu();
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
            listViewPreview.DrawItem += ListViewPreview_DrawItem;
            listViewPreview.KeyDown += ListViewPreview_KeyDown;
            listViewPreview.MouseDown += ListViewPreview_MouseDown;
            listViewPreview.MouseMove += ListViewPreview_MouseMove;
            _previewItemToolTip = new ToolTip(components) { AutoPopDelay = 8000, InitialDelay = 350, ShowAlways = true };
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
                if (!string.IsNullOrWhiteSpace(_settings.LastFolder))
                {
                    txtSourceFolder.Text = _settings.LastFolder;
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

                var solidIdx = Math.Clamp(_settings.SolidColorIndex, 0, Math.Min(2, Math.Max(0, cmbSolidColor.Items.Count - 1)));
                if (cmbSolidColor.Items.Count > 0)
                {
                    cmbSolidColor.SelectedIndex = solidIdx;
                }

                chkEnableExplorerConvertMenu.Checked = _settings.EnableExplorerConvertMenu;

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

        private void SyncExplorerConvertMenuFromSettings() =>
            ExplorerShellMenuUi.SyncFromSettings(
                _settings.EnableExplorerConvertMenu,
                Application.ExecutablePath,
                SetStatusMessage,
                ref _attemptedHklmScaleElevation);

        private void RefreshExplorerConverterMenu() =>
            ExplorerShellMenuUi.Refresh(
                this,
                _settings.EnableExplorerConvertMenu,
                Application.ExecutablePath,
                SetStatusMessage,
                ref _attemptedHklmScaleElevation);

        private void PersistSettings()
        {
            _settings.LastFolder = txtSourceFolder.Text.Trim();
            _settings.PreviewThumbnailSizeIndex = cmbPreviewSize.SelectedIndex >= 0 ? Math.Clamp(cmbPreviewSize.SelectedIndex, 0, 2) : 1;
            _settings.IcoOutputSizeIndex = cmbIcoOutputSize.SelectedIndex >= 0 ? Math.Clamp(cmbIcoOutputSize.SelectedIndex, 0, IcoOutputSizeValues.Length - 1) : 5;
            _settings.SolidColorIndex = cmbSolidColor.SelectedIndex >= 0 ? Math.Clamp(cmbSolidColor.SelectedIndex, 0, 2) : 0;
            _settings.EnableExplorerConvertMenu = chkEnableExplorerConvertMenu.Checked;
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

            try
            {
                _settingsStore.Save(_settings);
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }
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
                0 => 40,
                1 => 64,
                2 => 96,
                _ => 64
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

        private void ApplyPreviewTileLayoutFromPixelSize(int px, IReadOnlyList<string>? filePathsForLabelSizing = null)
        {
            imageListThumbnails.ImageSize = new Size(px, px);

            const int minTileWidth = 140;
            const int maxTileWidth = 300;
            const int horizontalPad = 28;
            var tileW = Math.Max(minTileWidth, px + horizontalPad);

            var lineH = TextRenderer.MeasureText("Aygjp", listViewPreview.Font, Size.Empty,
                TextFormatFlags.NoPadding).Height;
            var maxTextLines = 2;
            if (filePathsForLabelSizing is { Count: > 0 })
            {
                foreach (var path in filePathsForLabelSizing)
                {
                    var label = BuildPreviewItemLabel(path);
                    var singleLineW = TextRenderer.MeasureText(label, listViewPreview.Font, Size.Empty,
                        TextFormatFlags.NoPadding).Width + 16;
                    tileW = Math.Max(tileW, Math.Min(maxTileWidth, singleLineW));
                    var lines = MeasureWrappedLineCount(label, listViewPreview.Font, tileW - 12, lineH);
                    maxTextLines = Math.Max(maxTextLines, lines);
                }
            }

            tileW = Math.Min(maxTileWidth, tileW);
            maxTextLines = Math.Min(maxTextLines, 5);
            const int iconToTextGap = 10;
            const int verticalChrome = 16;
            var textBand = lineH * maxTextLines + 12;
            var tileH = px + iconToTextGap + textBand + verticalChrome;

            listViewPreview.TileSize = new Size(tileW, tileH);
            listViewPreview.Padding = new Padding(6, 8, 6, 8);
        }

        private static string BuildPreviewItemLabel(string fullPath)
        {
            var name = Path.GetFileName(fullPath);
            return SupportedFormats.FormatIndexMatchesExtension(fullPath, SupportedFormats.IcoFormatIndex)
                ? $"{name}{Environment.NewLine}( icon)"
                : name;
        }

        private static int MeasureWrappedLineCount(string text, Font font, int maxWidth, int lineHeight)
        {
            if (string.IsNullOrEmpty(text) || maxWidth <= 0 || lineHeight <= 0)
            {
                return 1;
            }

            var measured = TextRenderer.MeasureText(
                text,
                font,
                new Size(maxWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
            return Math.Max(1, (int)Math.Ceiling(measured.Height / (double)lineHeight));
        }

        private static Rectangle GetPreviewItemTextBounds(Rectangle tileBounds, Size imgSize)
        {
            var imgY = tileBounds.Y + 4;
            var textTop = imgY + imgSize.Height + 6;
            return new Rectangle(
                tileBounds.X + 4,
                textTop,
                Math.Max(0, tileBounds.Width - 8),
                Math.Max(0, tileBounds.Bottom - textTop - 4));
        }

        private static void EnableControlDoubleBuffering(Control control)
        {
            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(control, true, null);
        }

        private void ListViewPreview_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            SelectPreviewItemAtClientPoint(e.Location, clearOtherSelections: true);
        }

        private void ListViewPreview_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_previewItemToolTip is null)
            {
                return;
            }

            var hit = listViewPreview.HitTest(e.Location);
            if (hit.Item?.Tag is string path && !string.IsNullOrEmpty(path))
            {
                _previewItemToolTip.SetToolTip(listViewPreview, path);
            }
            else
            {
                _previewItemToolTip.SetToolTip(listViewPreview, string.Empty);
            }
        }

        private void SelectPreviewItemAtClientPoint(Point clientPoint, bool clearOtherSelections)
        {
            var hit = listViewPreview.HitTest(clientPoint);
            if (hit.Item is null)
            {
                return;
            }

            if (clearOtherSelections && !hit.Item.Selected)
            {
                listViewPreview.SelectedItems.Clear();
            }

            hit.Item.Selected = true;
            hit.Item.Focused = true;
            listViewPreview.FocusedItem = hit.Item;
        }

        private void ListViewPreview_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_conversionBusy)
            {
                return;
            }

            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                _ = ReloadPreviewKeepingSelectionAsync();
                return;
            }

            if (e.KeyCode == Keys.F2 && listViewPreview.SelectedItems.Count == 1)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                _ = RenameSelectedPreviewFileAsync();
            }
        }

        private void ListViewPreview_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = false;
            if (e.Item is null)
            {
                return;
            }

            var tile = e.Bounds;
            var imgSize = imageListThumbnails.ImageSize;
            var imgX = tile.X + Math.Max(0, (tile.Width - imgSize.Width) / 2);
            var imgY = tile.Y + 4;
            var imgRect = new Rectangle(imgX, imgY, imgSize.Width, imgSize.Height);

            var textRect = GetPreviewItemTextBounds(tile, imgSize);

            var backColor = e.Item.Selected ? PreviewItemSelectedBack : listViewPreview.BackColor;
            var foreColor = listViewPreview.ForeColor;

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, tile);
            }

            if (e.Item.ImageIndex >= 0 && e.Item.ImageIndex < imageListThumbnails.Images.Count)
            {
                imageListThumbnails.Draw(e.Graphics, imgRect.Location, e.Item.ImageIndex);
            }

            TextRenderer.DrawText(
                e.Graphics,
                e.Item.Text,
                e.Item.Font ?? listViewPreview.Font,
                textRect,
                foreColor,
                backColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            if (e.Item.Selected)
            {
                using var borderPen = new Pen(PreviewItemSelectedBorder, 2f);
                var border = tile;
                border.Inflate(-1, -1);
                e.Graphics.DrawRectangle(borderPen, border);
            }
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
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

        private void OpenSelectedImageWithPaint()
        {
            OpenSelectedImageWithEditor(ExternalImageEditorLauncher.TryOpenWithPaint, "Paint");
        }

        private void OpenSelectedImageWithPaintDotNet()
        {
            OpenSelectedImageWithEditor(ExternalImageEditorLauncher.TryOpenWithPaintDotNet, "Paint.NET");
        }

        private void OpenSelectedImageWithEditor(TryOpenImageEditor openEditor, string editorName)
        {
            var paths = GetSelectedSourcePaths();
            if (paths.Count == 0)
            {
                return;
            }

            var path = paths[0];
            if (!openEditor(path, out var error))
            {
                MessageBox.Show(
                    this,
                    error ?? $"Could not open the image with {editorName}.",
                    editorName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                SetStatusMessage($"Could not open with {editorName}.");
                return;
            }

            SetStatusMessage($"Opened {Path.GetFileName(path)} with {editorName}.");
        }

        private void CopySelectedImagePathsToClipboard()
        {
            var paths = GetSelectedSourcePaths();
            if (paths.Count == 0)
            {
                return;
            }

            try
            {
                Clipboard.SetText(paths.Count == 1
                    ? paths[0]
                    : string.Join(Environment.NewLine, paths));
                SetStatusMessage(paths.Count == 1
                    ? "Copied image path to clipboard."
                    : $"Copied {paths.Count} image paths to clipboard.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not copy path: " + ex.Message, "Copy image path", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private delegate bool TryOpenImageEditor(string imagePath, out string? errorMessage);

        private async Task InitialPreviewLoadAsync()
        {
            var folder = txtSourceFolder.Text.Trim();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                return;
            }

            await ReloadSourcePreviewsAsync();
        }

        private async Task BrowseForFolderAsync()
        {
            var hint = GetInitialBrowseFolder();
            if (!FolderPicker.TryPick(this, "Select image folder", hint, out var path) || string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            txtSourceFolder.Text = path;
            await ReloadSourcePreviewsAsync();
        }

        private string GetInitialBrowseFolder()
        {
            if (TryNormalizeDirectoryPath(txtSourceFolder.Text.Trim(), out var current))
            {
                return current;
            }

            return FolderPicker.GetDefaultBrowseFolder();
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
                await ReloadSourcePreviewsAsync();
            }
            catch (Exception ex)
            {
                SetStatusMessage("Drop could not be handled: " + ex.Message);
            }
        }

        private async Task ReloadSourcePreviewsAsync(IReadOnlyCollection<string>? selectFullPathsAfter = null, string? statusOverride = null)
        {
            await ReplacePreviewLoadCancellationAsync();

            var ct = _previewLoadCts!.Token;
            if (!TryNormalizeDirectoryPath(txtSourceFolder.Text.Trim(), out var folder))
            {
                HandleInvalidImageFolder();
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
            ApplyPreviewTileLayoutFromPixelSize(thumbPx, files);
            var selectSet = BuildPreviewSelectionSet(selectFullPathsAfter);

            try
            {
                await LoadPreviewThumbnailBatchesAsync(files, thumbPx, ct).ConfigureAwait(true);
            }
            finally
            {
                if (!ct.IsCancellationRequested)
                {
                    FinalizePreviewListReload(selectSet, statusOverride);
                }
                else
                {
                    RunOnUiThread(listViewPreview.EndUpdate);
                }
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

        private void HandleInvalidImageFolder()
        {
            RunOnUiThread(ClearPreviewList);
            UpdatePlaceholderVisibility();
            SetStatusMessage("Invalid image folder.");
        }

        private static Task<List<string>> EnumeratePreviewFilesAsync(string folder, CancellationToken ct)
        {
            return Task.Run(() => FolderThumbnailLoader.EnumerateImageFiles(folder), ct);
        }

        private static bool TryNormalizeDirectoryPath(string? path, out string normalizedPath)
        {
            normalizedPath = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                var fullPath = Path.GetFullPath(path.Trim());
                if (!Directory.Exists(fullPath))
                {
                    return false;
                }

                normalizedPath = Path.TrimEndingDirectorySeparator(fullPath);
                return true;
            }
            catch
            {
                return false;
            }
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

            return new HashSet<string>(
                selectFullPathsAfter.Select(NormalizePreviewPath),
                StringComparer.OrdinalIgnoreCase);
        }

        private static string NormalizePreviewPath(string path)
        {
            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return path;
            }
        }

        private async Task LoadPreviewThumbnailBatchesAsync(IReadOnlyList<string> files, int thumbPx, CancellationToken ct)
        {
            const int batchSize = 30;
            for (var i = 0; i < files.Count; i += batchSize)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var slice = files.Skip(i).Take(batchSize).ToList();
                var batch = await Task.Run(
                        () => BuildThumbnailBatch(slice, thumbPx, CancellationToken.None),
                        ct)
                    .ConfigureAwait(true);
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

                var bmp = FolderThumbnailLoader.CreateThumbnailOrPlaceholder(f, thumbPx);
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
                foreach (var (bmp, _, fullPath) in batch)
                {
                    if (bmp is null)
                    {
                        continue;
                    }

                    imageListThumbnails.Images.Add(bmp);
                    var idx = imageListThumbnails.Images.Count - 1;
                    var normalizedPath = NormalizePreviewPath(fullPath);
                    listViewPreview.Items.Add(new ListViewItem(BuildPreviewItemLabel(normalizedPath), idx)
                    {
                        Tag = normalizedPath
                    });
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
                ListViewItem? firstSelected = null;
                foreach (ListViewItem item in listViewPreview.Items)
                {
                    if (item.Tag is string p && selectSet.Contains(NormalizePreviewPath(p)))
                    {
                        item.Selected = true;
                        firstSelected ??= item;
                    }
                }

                if (firstSelected is not null)
                {
                    listViewPreview.FocusedItem = firstSelected;
                    firstSelected.EnsureVisible();
                    RefreshSelectionStatusText();
                    return;
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

        private bool CanConvertSelection()
        {
            var pathsOk = TryNormalizeDirectoryPath(txtSourceFolder.Text.Trim(), out _);

            return pathsOk && listViewPreview.SelectedItems.Count > 0;
        }

        private IconBackgroundKind GetIconBackgroundFromUi()
        {
            return cmbSolidColor.SelectedIndex switch
            {
                1 => IconBackgroundKind.SolidBlack,
                2 => IconBackgroundKind.Transparent,
                _ => IconBackgroundKind.SolidWhite
            };
        }

        private void UpdateFileMenuState()
        {
            var busy = _conversionBusy;
            menuFileOpenFolder.Enabled = !busy;
            menuFileRefreshReview.Enabled = !busy;
            menuFileOpenFolderInExplorer.Enabled = !busy;
            menuFileOpenAppLocation.Enabled = true;
            menuFilePasteImage.Enabled = !busy && Clipboard.ContainsImage();
            menuFileUndo.Enabled = !busy && _undoLastOperation is not null;
        }

        private void ShowHowToUseDialog()
        {
            using var form = new frmHowToUse();
            form.ShowDialog(this);
        }

        private void ShowAboutDialog()
        {
            using var form = new frmAbout();
            form.ShowDialog(this);
        }

        private void SetConversionBusy(bool busy)
        {
            _conversionBusy = busy;
            UpdateFileMenuState();
            btnBrowseSource.Enabled = !busy;
            txtSourceFolder.Enabled = !busy;
            cmbIcoOutputSize.Enabled = !busy;
            cmbSolidColor.Enabled = !busy;
            btnRefreshPreview.Enabled = !busy;
            btnOpenFolder.Enabled = !busy;
            cmbPreviewSize.Enabled = !busy;
            listViewPreview.Enabled = !busy;
            UpdateUndoButtonEnabled();
            btnCancel.Enabled = busy;
        }

        private async Task ConvertSelectionToFormatAsync(int outputFormatIndex, bool iconQuickAccess)
        {
            if (outputFormatIndex < 0 || outputFormatIndex >= SupportedFormats.Count)
            {
                return;
            }

            if (!EnsureImageFolderExists())
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
            var icoIndex = SupportedFormats.IcoFormatIndex;
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
                        $"Every selected file is already {SupportedFormats.GetFormatLabel(outputFormatIndex)}. There is nothing to convert.",
                        "Convert",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            var outputPaths = pathsToConvert
                .Select(src => SupportedFormats.BuildOutputPath(src, outputFormatIndex))
                .ToList();
            if (!ConfirmOverwriteExistingFiles(outputPaths))
            {
                SetStatusMessage("Conversion cancelled — existing file(s) were not overwritten.");
                return;
            }

            var iconBackground = GetIconBackgroundFromUi();
            if (iconBackground == IconBackgroundKind.Transparent && !SupportedFormats.SupportsTransparency(outputFormatIndex))
            {
                var label = SupportedFormats.GetFormatLabel(outputFormatIndex);
                MessageBox.Show(
                    this,
                    $"{label} does not support transparency. Choose PNG, GIF, WEBP, ICO, or SVG, or set the background to White or Black.\n\nTransparent mode preserves alpha and can key out a uniform border color — it does not remove complex photo backgrounds.",
                    "Convert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            await RunBatchConversionAndRefreshAsync(pathsToConvert, outputFormatIndex).ConfigureAwait(true);
        }

        private async Task ResizeSelectionAsync(double scaleFactor)
        {
            if (!EnsureImageFolderExists())
            {
                return;
            }

            var paths = GetSelectedSourcePaths();
            if (paths.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Select one or more images in the preview list.",
                    "Resize",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var pathsToResize = paths.Where(ImageResize.IsResizablePath).ToList();
            if (pathsToResize.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Resize supports JPEG, PNG, BMP, GIF, WEBP, and ICO only.",
                    "Resize",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (pathsToResize.Count < paths.Count)
            {
                var skipped = paths.Count - pathsToResize.Count;
                MessageBox.Show(
                    this,
                    $"{skipped} selected file(s) were skipped (not a resizable image type).",
                    "Resize",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            var outputPaths = pathsToResize.Select(ImageResize.BuildScaledOutputPath).ToList();
            if (!ConfirmOverwriteExistingFiles(outputPaths))
            {
                SetStatusMessage("Resize cancelled — existing file(s) were not overwritten.");
                return;
            }

            await RunBatchResizeAndRefreshAsync(pathsToResize, scaleFactor).ConfigureAwait(true);
        }

        private bool ConfirmOverwriteExistingFiles(IReadOnlyList<string> destinationPaths)
        {
            var existingOutputs = destinationPaths
                .Where(File.Exists)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (existingOutputs.Count == 0)
            {
                return true;
            }

            string message;
            if (existingOutputs.Count == 1)
            {
                message = $"\"{Path.GetFileName(existingOutputs[0])}\" already exists.\n\nOverwrite it?";
            }
            else
            {
                var preview = string.Join(
                    Environment.NewLine,
                    existingOutputs.Take(3).Select(Path.GetFileName));
                if (existingOutputs.Count > 3)
                {
                    preview += $"{Environment.NewLine}(+{existingOutputs.Count - 3} more)";
                }

                message =
                    $"{existingOutputs.Count} output file(s) already exist and will be overwritten:\n\n{preview}\n\nContinue?";
            }

            return MessageBox.Show(
                    this,
                    message,
                    "Confirm overwrite",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2)
                == DialogResult.Yes;
        }

        private bool EnsureImageFolderExists()
        {
            if (!TryNormalizeDirectoryPath(txtSourceFolder.Text.Trim(), out var folder))
            {
                MessageBox.Show(
                    this,
                    "Choose a valid image folder before converting.",
                    "Convert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                SetStatusMessage("Invalid image folder.");
                return false;
            }

            if (IsDriveRootFolderPath(folder))
            {
                MessageBox.Show(
                    this,
                    "The image folder cannot be the root of a drive (for example C:\\).\n\n" +
                    "Windows blocks creating files directly under C:\\ for normal (non-elevated) apps. " +
                    "Use Browse to choose a folder inside your profile (for example Documents or Pictures).",
                    "Folder not allowed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                SetStatusMessage("Pick a subfolder — not C:\\.");
                return false;
            }

            return true;
        }

        private static bool IsDriveRootFolderPath(string folderPath)
        {
            try
            {
                var full = Path.GetFullPath(folderPath.Trim());
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

        private static List<string> GetSuccessfulOutputsExistingOnDisk(IReadOnlyList<string> successfulOutputPaths)
        {
            return successfulOutputPaths
                .Select(NormalizePreviewPath)
                .Where(File.Exists)
                .ToList();
        }

        private void RegisterUndoForSuccessfulOutputs(int successCount, List<string> createdOutputs)
        {
            if (successCount <= 0 || createdOutputs.Count == 0)
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
            var createdOutputs = GetSuccessfulOutputsExistingOnDisk(batchResult.SuccessfulOutputPaths);
            await WaitForOutputsOnDiskAsync(createdOutputs).ConfigureAwait(true);

            if (batchResult.FailCount > 0)
            {
                ShowConversionCompletedWithErrorsDialog(batchResult.FailCount);
            }

            RegisterUndoForSuccessfulOutputs(batchResult.SuccessCount, createdOutputs);

            var summary = batchResult.FailCount == 0
                ? $"Conversion finished — {batchResult.SuccessCount} file(s) converted."
                : $"Conversion finished — {batchResult.SuccessCount} succeeded, {batchResult.FailCount} failed.";
            await ReloadSourcePreviewsAsync(
                createdOutputs.Count > 0 ? createdOutputs : null,
                summary).ConfigureAwait(true);
        }

        private static async Task WaitForOutputsOnDiskAsync(IReadOnlyList<string> paths)
        {
            if (paths.Count == 0)
            {
                return;
            }

            for (var attempt = 0; attempt < 8; attempt++)
            {
                if (paths.All(File.Exists))
                {
                    return;
                }

                await Task.Delay(50).ConfigureAwait(true);
            }
        }

        private async Task RunBatchConversionAndRefreshAsync(
            IReadOnlyList<string> pathsToConvert,
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

        private async Task RunBatchResizeAndRefreshAsync(
            IReadOnlyList<string> pathsToResize,
            double scaleFactor)
        {
            await ReplaceConversionCancellationAsync().ConfigureAwait(true);
            var ct = _conversionCts!.Token;
            IProgress<(int Current, int Total, string FileName)> uiProgress = new Progress<(int Current, int Total, string FileName)>(p =>
            {
                toolStripProgressBatch.Value = p.Current;
                SetStatusMessage($"Resizing {p.Current} of {p.Total}: {p.FileName}");
            });

            SetConversionBusy(true);
            toolStripProgressBatch.Visible = true;
            toolStripProgressBatch.Minimum = 0;
            toolStripProgressBatch.Maximum = Math.Max(1, pathsToResize.Count);
            toolStripProgressBatch.Value = 0;
            UseWaitCursor = true;

            BatchImageResizeRunner.RunResult? batchResult = null;
            try
            {
                batchResult = await Task.Run(
                        () => BatchImageResizeRunner.Run(
                            pathsToResize,
                            scaleFactor,
                            ct,
                            uiProgress),
                        ct)
                    .ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                SetStatusMessage("Resize cancelled.");
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
                SetStatusMessage("Resize did not complete.");
                return;
            }

            await ApplyResizeResultAndReloadAsync(batchResult).ConfigureAwait(true);
        }

        private async Task ApplyResizeResultAndReloadAsync(BatchImageResizeRunner.RunResult batchResult)
        {
            var createdOutputs = GetSuccessfulOutputsExistingOnDisk(batchResult.SuccessfulOutputPaths);
            await WaitForOutputsOnDiskAsync(createdOutputs).ConfigureAwait(true);

            if (batchResult.FailCount > 0)
            {
                MessageBox.Show(
                    this,
                    $"{batchResult.FailCount} file(s) could not be resized. Check paths, permissions, and formats.\nSee the status bar for the summary.",
                    "Resize completed with errors",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            RegisterUndoForSuccessfulOutputs(batchResult.SuccessCount, createdOutputs);

            var summary = batchResult.FailCount == 0
                ? $"Resize finished — {batchResult.SuccessCount} file(s) saved as _scaled."
                : $"Resize finished — {batchResult.SuccessCount} succeeded, {batchResult.FailCount} failed.";
            await ReloadSourcePreviewsAsync(
                createdOutputs.Count > 0 ? createdOutputs : null,
                summary).ConfigureAwait(true);
        }

        private async Task PasteClipboardImageAsync()
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

            if (!TryNormalizeDirectoryPath(txtSourceFolder.Text.Trim(), out var folder))
            {
                MessageBox.Show(
                    this,
                    "Choose a valid image folder before pasting.",
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
                    catch (IOException rollbackEx)
                    {
                        Debug.WriteLine(rollbackEx);
                    }
                }

                TryDeleteSessionDirectory(sessionDir);

                MessageBox.Show(this, "Could not move files for delete: " + ex.Message, "Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RegisterUndo(() => TryUndoDeleteMoves(sessionDir, movedPairs));

            await ReloadSourcePreviewsAsync(null, "Deleted selected file(s).");
        }

        private async Task ReloadPreviewKeepingSelectionAsync()
        {
            var selected = GetSelectedSourcePaths();
            await ReloadSourcePreviewsAsync(
                selected.Count > 0 ? selected : null,
                "Review refreshed.");
        }

        private async Task RenameSelectedPreviewFileAsync()
        {
            if (_conversionBusy)
            {
                return;
            }

            if (listViewPreview.SelectedItems.Count != 1)
            {
                MessageBox.Show(
                    this,
                    "Select exactly one image to rename.",
                    "Rename",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var oldPath = GetSelectedSourcePaths()[0];
            if (!File.Exists(oldPath))
            {
                SetStatusMessage("Selected file was not found on disk.");
                await ReloadPreviewKeepingSelectionAsync();
                return;
            }

            var ext = Path.GetExtension(oldPath);
            var currentBaseName = Path.GetFileNameWithoutExtension(oldPath);
            if (!TryPromptForFileRename(currentBaseName, out var newBaseName)
                || string.IsNullOrWhiteSpace(newBaseName)
                || string.Equals(newBaseName, currentBaseName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var dir = Path.GetDirectoryName(oldPath)!;
            var newPath = Path.Combine(dir, newBaseName + ext);
            if (File.Exists(newPath))
            {
                MessageBox.Show(
                    this,
                    $"A file named \"{Path.GetFileName(newPath)}\" already exists in this folder.",
                    "Rename",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                File.Move(oldPath, newPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Rename failed: " + ex.Message, "Rename", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RegisterUndo(() =>
            {
                try
                {
                    if (!File.Exists(newPath) || File.Exists(oldPath))
                    {
                        return false;
                    }

                    File.Move(newPath, oldPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            });

            await ReloadSourcePreviewsAsync([newPath], $"Renamed to {Path.GetFileName(newPath)}.");
        }

        private bool TryPromptForFileRename(string currentBaseName, out string? newBaseName)
        {
            newBaseName = null;
            using var form = new Form
            {
                Text = "Rename",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(420, 120),
                Font = Font
            };

            var label = new Label
            {
                Text = "New name:",
                AutoSize = true,
                Location = new Point(12, 16)
            };
            var textBox = new TextBox
            {
                Text = currentBaseName,
                Location = new Point(12, 44),
                Width = 396
            };
            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(252, 78),
                AutoSize = true
            };
            var btnRenameCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(333, 78),
                AutoSize = true
            };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnRenameCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnRenameCancel;
            form.Shown += (_, _) =>
            {
                textBox.Focus();
                textBox.SelectAll();
            };

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }

            var trimmed = textBox.Text.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                return false;
            }

            newBaseName = SanitizeFileBaseName(trimmed);
            return !string.IsNullOrEmpty(newBaseName);
        }

        private static string SanitizeFileBaseName(string name)
        {
            var trimmed = name.Trim();
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                trimmed = trimmed.Replace(c, '_');
            }

            return trimmed.Trim().TrimEnd('.');
        }

        private bool IsPreviewListFocused()
        {
            return listViewPreview.ContainsFocus || ActiveControl == listViewPreview;
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

            TryDeleteSessionDirectory(sessionDir);

            return true;
        }

        private static void TryDeleteSessionDirectory(string sessionDir)
        {
            try
            {
                if (Directory.Exists(sessionDir))
                {
                    Directory.Delete(sessionDir, true);
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void RegisterUndo(Func<bool> undo)
        {
            _undoLastOperation = undo;
            UpdateUndoButtonEnabled();
        }

        private void UpdateUndoButtonEnabled()
        {
            var enabled = !_conversionBusy && _undoLastOperation is not null;
            btnUndo.Enabled = enabled;
            menuFileUndo.Enabled = enabled;
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
                statusLabelSelection.Visible = true;
                var firstPath = listViewPreview.SelectedItems[0].Tag as string ?? "";
                var name = Path.GetFileName(firstPath);
                var displayName = listViewPreview.SelectedItems.Count == 1
                    ? name
                    : $"{name} (+{listViewPreview.SelectedItems.Count - 1} more)";

                statusLabelSelection.Text = displayName;
                if (!string.IsNullOrEmpty(firstPath))
                {
                    var version = ++_selectionInfoVersion;
                    _ = UpdateSelectionStatusDetailsAsync(firstPath, displayName, version);
                }

                return;
            }

            _selectionInfoVersion++;

            statusLabelSelection.Text = "";
            statusLabelSelection.Visible = false;
        }

        private async Task UpdateSelectionStatusDetailsAsync(string path, string displayName, int version)
        {
            var meta = await Task.Run(() => ImageFileMetadataReader.TryRead(path)).ConfigureAwait(true);
            if (version != _selectionInfoVersion)
            {
                return;
            }

            statusLabelSelection.Text = meta is null
                ? displayName
                : FormatSelectionStatusText(displayName, meta.Value);
        }

        private static string FormatSelectionStatusText(string displayName, ImageFileMetadata meta)
        {
            var dimensions = meta.Width > 0 && meta.Height > 0
                ? $"{meta.Width} × {meta.Height}"
                : "—";
            var modified = meta.LastModifiedLocal.ToString("g");
            return $"{displayName}  ·  {dimensions}  ·  {FormatFileSize(meta.SizeBytes)}  ·  Modified {modified}";
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} B";
            }

            if (bytes < 1024 * 1024)
            {
                return $"{bytes / 1024.0:0.#} KB";
            }

            return $"{bytes / (1024.0 * 1024.0):0.##} MB";
        }

        private void OpenImageFolderInExplorer()
        {
            if (!TryNormalizeDirectoryPath(txtSourceFolder.Text.Trim(), out var folder))
            {
                MessageBox.Show(this, "Image folder is missing or invalid.", "Open folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFolderInExplorer(folder, "Open folder");
        }

        private void OpenAppLocationInExplorer()
        {
            var folder = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            OpenFolderInExplorer(folder, "Open app location");
        }

        private void OpenFolderInExplorer(string folder, string dialogTitle)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = folder,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not open Explorer: " + ex.Message, dialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (hit.Item != null)
            {
                SelectPreviewItemAtClientPoint(pt, clearOtherSelections: !Control.ModifierKeys.HasFlag(Keys.Control));
                PopulatePreviewMenuForSelectedThumbnails();
                return;
            }

            OfferPasteOnReviewEmptyArea(e);
        }

        private void HideAllPreviewContextMenuItems()
        {
            toolStripMenuItemPreviewConvertTo.Visible = false;
            toolStripMenuItemPreviewResize.Visible = false;
            toolStripMenuItemConvertToQuickIcon.Visible = false;
            toolStripSeparatorPreviewAfterConvert.Visible = false;
            toolStripMenuItemPreviewCopy.Visible = false;
            toolStripMenuItemPreviewCopyImagePath.Visible = false;
            toolStripSeparatorPreviewAfterClipboard.Visible = false;
            toolStripMenuItemPreviewRename.Visible = false;
            toolStripMenuItemPreviewDelete.Visible = false;
            toolStripSeparatorPreviewAfterFileOps.Visible = false;
            toolStripMenuItemOpenSourceLocation.Visible = false;
            toolStripSeparatorPreviewOpenWith.Visible = false;
            toolStripMenuItemOpenWithPaint.Visible = false;
            toolStripMenuItemOpenWithPaintDotNet.Visible = false;
            toolStripMenuItemPreviewPaste.Visible = false;
        }

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
            toolStripMenuItemConvertToQuickIcon.Visible = true;
            toolStripMenuItemPreviewConvertTo.Visible = true;
            toolStripMenuItemPreviewResize.Visible = true;
            toolStripSeparatorPreviewAfterConvert.Visible = true;
            toolStripMenuItemPreviewCopy.Visible = true;
            toolStripMenuItemPreviewCopyImagePath.Visible = true;
            toolStripSeparatorPreviewAfterClipboard.Visible = true;
            toolStripMenuItemPreviewRename.Visible = true;
            toolStripMenuItemPreviewDelete.Visible = true;
            toolStripSeparatorPreviewAfterFileOps.Visible = true;
            toolStripMenuItemOpenSourceLocation.Visible = true;
            toolStripSeparatorPreviewOpenWith.Visible = true;
            toolStripMenuItemOpenWithPaint.Visible = true;
            toolStripMenuItemOpenWithPaintDotNet.Visible = true;

            toolStripMenuItemPreviewCopy.Enabled = listViewPreview.SelectedItems.Count > 0;
            toolStripMenuItemPreviewRename.Enabled = listViewPreview.SelectedItems.Count == 1 && !_conversionBusy;
            toolStripMenuItemPreviewDelete.Enabled = listViewPreview.SelectedItems.Count > 0;
            toolStripMenuItemOpenSourceLocation.Enabled = listViewPreview.SelectedItems.Count > 0;
            toolStripMenuItemPreviewCopyImagePath.Enabled = listViewPreview.SelectedItems.Count > 0;
            toolStripMenuItemOpenWithPaint.Enabled = listViewPreview.SelectedItems.Count > 0;
            toolStripMenuItemOpenWithPaintDotNet.Enabled = listViewPreview.SelectedItems.Count > 0
                && ExternalImageEditorLauncher.IsPaintDotNetAvailable();

            var paths = GetSelectedSourcePaths();
            var convertOk = CanConvertSelection() && !_conversionBusy;
            var resizeOk = convertOk && paths.Any(ImageResize.IsResizablePath);
            toolStripMenuItemConvertToQuickIcon.Visible = true;
            toolStripMenuItemConvertToQuickIcon.Enabled = convertOk;
            toolStripMenuItemPreviewResize.Enabled = resizeOk;
            toolStripMenuItemPreviewResize05x.Enabled = resizeOk;
            toolStripMenuItemPreviewResize075x.Enabled = resizeOk;
            toolStripMenuItemPreviewResize2x.Enabled = resizeOk;
            toolStripMenuItemPreviewResize4x.Enabled = resizeOk;

            toolStripMenuItemPreviewConvertTo.DropDownItems.Clear();
            foreach (var formatIdx in BuildAllowedConvertToFormatIndices(paths))
            {
                var captured = formatIdx;
                var sub = new ToolStripMenuItem(SupportedFormats.GetFormatLabel(captured))
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
            if (ShouldUseDefaultCommandKeyHandling(keyData))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            return TryHandleGlobalShortcut(keyData) || base.ProcessCmdKey(ref msg, keyData);
        }

        private bool ShouldUseDefaultCommandKeyHandling(Keys keyData) =>
            ActiveControl is TextBoxBase
            || (ActiveControl is ComboBox && keyData == (Keys.Control | Keys.C));

        private bool TryHandleGlobalShortcut(Keys keyData)
        {
            if (keyData == Keys.F5 && IsPreviewListFocused() && !_conversionBusy)
            {
                _ = ReloadPreviewKeepingSelectionAsync();
                return true;
            }

            if (keyData == Keys.F2 && listViewPreview.SelectedItems.Count == 1 && !_conversionBusy)
            {
                _ = RenameSelectedPreviewFileAsync();
                return true;
            }

            if (keyData == (Keys.Control | Keys.C) && listViewPreview.SelectedItems.Count > 0)
            {
                _ = CopySelectedPreviewFilesToClipboardAsync();
                return true;
            }

            if ((keyData == (Keys.Control | Keys.V) || keyData == (Keys.Control | Keys.P)) && Clipboard.ContainsImage())
            {
                _ = PasteClipboardImageAsync();
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

            return false;
        }

        private void SetStatusMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => SetStatusMessage(message));
                return;
            }

            statusLabelMessage.Text = string.IsNullOrWhiteSpace(message) ? "Ready" : message;
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

        private void grpBackground_Enter(object sender, EventArgs e)
        {
        }
    }
}