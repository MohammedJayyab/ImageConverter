namespace ImageConverter
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            menuStripMain = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            menuFileOpenFolder = new ToolStripMenuItem();
            menuFileRefreshReview = new ToolStripMenuItem();
            menuFileSeparatorAfterWorkspace = new ToolStripSeparator();
            menuFileOpenFolderInExplorer = new ToolStripMenuItem();
            menuFileOpenAppLocation = new ToolStripMenuItem();
            menuFileSeparatorAfterLocations = new ToolStripSeparator();
            menuFilePasteImage = new ToolStripMenuItem();
            menuFileUndo = new ToolStripMenuItem();
            menuFileSeparatorExit = new ToolStripSeparator();
            menuFileExit = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            menuHelpHowToUse = new ToolStripMenuItem();
            menuHelpSupport = new ToolStripMenuItem();
            menuHelpSeparatorAbout = new ToolStripSeparator();
            menuHelpAbout = new ToolStripMenuItem();
            statusStripMain = new StatusStrip();
            statusLabelMessage = new ToolStripStatusLabel();
            statusLabelSelection = new ToolStripStatusLabel();
            toolStripProgressBatch = new ToolStripProgressBar();
            statusLabelSpring = new ToolStripStatusLabel();
            flowActions = new FlowLayoutPanel();
            btnUndo = new Button();
            btnCancel = new Button();
            splitReview = new SplitContainer();
            grpFolders = new GroupBox();
            lblSourceFolder = new Label();
            txtSourceFolder = new TextBox();
            btnBrowseSource = new Button();
            grpBackground = new GroupBox();
            lblCanvasFill = new Label();
            cmbIcoOutputSize = new ComboBox();
            lblIcoOutputSize = new Label();
            cmbSolidColor = new ComboBox();
            grpExplorer = new GroupBox();
            chkEnableExplorerConvertMenu = new CheckBox();
            btnRefreshExplorerConverterMenu = new Button();
            grpPreview = new GroupBox();
            listViewPreview = new ListView();
            contextMenuPreview = new ContextMenuStrip(components);
            toolStripMenuItemConvertToQuickIcon = new ToolStripMenuItem();
            toolStripMenuItemPreviewConvertTo = new ToolStripMenuItem();
            toolStripMenuItemPreviewResize = new ToolStripMenuItem();
            toolStripMenuItemPreviewResize05x = new ToolStripMenuItem();
            toolStripMenuItemPreviewResize075x = new ToolStripMenuItem();
            toolStripSeparatorPreviewResizeScale = new ToolStripSeparator();
            toolStripMenuItemPreviewResize2x = new ToolStripMenuItem();
            toolStripMenuItemPreviewResize4x = new ToolStripMenuItem();
            toolStripSeparatorPreviewAfterConvert = new ToolStripSeparator();
            toolStripMenuItemPreviewCopy = new ToolStripMenuItem();
            toolStripMenuItemPreviewCopyImagePath = new ToolStripMenuItem();
            toolStripSeparatorPreviewAfterClipboard = new ToolStripSeparator();
            toolStripMenuItemPreviewRename = new ToolStripMenuItem();
            toolStripMenuItemPreviewDelete = new ToolStripMenuItem();
            toolStripSeparatorPreviewAfterFileOps = new ToolStripSeparator();
            toolStripMenuItemOpenSourceLocation = new ToolStripMenuItem();
            toolStripSeparatorPreviewOpenWith = new ToolStripSeparator();
            toolStripMenuItemOpenWithPaint = new ToolStripMenuItem();
            toolStripMenuItemOpenWithPaintDotNet = new ToolStripMenuItem();
            toolStripMenuItemPreviewPaste = new ToolStripMenuItem();
            imageListThumbnails = new ImageList(components);
            lblPreviewPlaceholder = new Label();
            flowPreviewToolbar = new FlowLayoutPanel();
            lblPreviewView = new Label();
            cmbPreviewSize = new ComboBox();
            btnRefreshPreview = new Button();
            btnOpenFolder = new Button();
            menuStripMain.SuspendLayout();
            statusStripMain.SuspendLayout();
            flowActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitReview).BeginInit();
            splitReview.Panel1.SuspendLayout();
            splitReview.Panel2.SuspendLayout();
            splitReview.SuspendLayout();
            grpFolders.SuspendLayout();
            grpBackground.SuspendLayout();
            grpExplorer.SuspendLayout();
            grpPreview.SuspendLayout();
            contextMenuPreview.SuspendLayout();
            flowPreviewToolbar.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripMain
            // 
            menuStripMain.ImageScalingSize = new Size(20, 20);
            menuStripMain.Items.AddRange(new ToolStripItem[] { menuFile, menuHelp });
            menuStripMain.Location = new Point(10, 10);
            menuStripMain.Name = "menuStripMain";
            menuStripMain.Size = new Size(882, 33);
            menuStripMain.TabIndex = 3;
            menuStripMain.Text = "menuStripMain";
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileOpenFolder, menuFileRefreshReview, menuFileSeparatorAfterWorkspace, menuFileOpenFolderInExplorer, menuFileOpenAppLocation, menuFileSeparatorAfterLocations, menuFilePasteImage, menuFileUndo, menuFileSeparatorExit, menuFileExit });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(54, 29);
            menuFile.Text = "&File";
            // 
            // menuFileOpenFolder
            // 
            menuFileOpenFolder.Name = "menuFileOpenFolder";
            menuFileOpenFolder.ShortcutKeys = Keys.Control | Keys.O;
            menuFileOpenFolder.Size = new Size(390, 34);
            menuFileOpenFolder.Text = "&Open folder…";
            // 
            // menuFileRefreshReview
            // 
            menuFileRefreshReview.Name = "menuFileRefreshReview";
            menuFileRefreshReview.ShortcutKeys = Keys.F5;
            menuFileRefreshReview.Size = new Size(390, 34);
            menuFileRefreshReview.Text = "&Refresh review";
            // 
            // menuFileSeparatorAfterWorkspace
            // 
            menuFileSeparatorAfterWorkspace.Name = "menuFileSeparatorAfterWorkspace";
            menuFileSeparatorAfterWorkspace.Size = new Size(387, 6);
            // 
            // menuFileOpenFolderInExplorer
            // 
            menuFileOpenFolderInExplorer.Name = "menuFileOpenFolderInExplorer";
            menuFileOpenFolderInExplorer.Size = new Size(390, 34);
            menuFileOpenFolderInExplorer.Text = "Open &image folder in Explorer";
            // 
            // menuFileOpenAppLocation
            // 
            menuFileOpenAppLocation.Name = "menuFileOpenAppLocation";
            menuFileOpenAppLocation.Size = new Size(390, 34);
            menuFileOpenAppLocation.Text = "Open &application folder in Explorer";
            // 
            // menuFileSeparatorAfterLocations
            // 
            menuFileSeparatorAfterLocations.Name = "menuFileSeparatorAfterLocations";
            menuFileSeparatorAfterLocations.Size = new Size(387, 6);
            // 
            // menuFilePasteImage
            // 
            menuFilePasteImage.Name = "menuFilePasteImage";
            menuFilePasteImage.ShortcutKeys = Keys.Control | Keys.V;
            menuFilePasteImage.Size = new Size(390, 34);
            menuFilePasteImage.Text = "&Paste image";
            // 
            // menuFileUndo
            // 
            menuFileUndo.Name = "menuFileUndo";
            menuFileUndo.ShortcutKeys = Keys.Control | Keys.Z;
            menuFileUndo.Size = new Size(390, 34);
            menuFileUndo.Text = "&Undo";
            // 
            // menuFileSeparatorExit
            // 
            menuFileSeparatorExit.Name = "menuFileSeparatorExit";
            menuFileSeparatorExit.Size = new Size(387, 6);
            // 
            // menuFileExit
            // 
            menuFileExit.Name = "menuFileExit";
            menuFileExit.ShortcutKeys = Keys.Alt | Keys.F4;
            menuFileExit.Size = new Size(390, 34);
            menuFileExit.Text = "E&xit";
            // 
            // menuHelp
            // 
            menuHelp.DropDownItems.AddRange(new ToolStripItem[] { menuHelpHowToUse, menuHelpSupport, menuHelpSeparatorAbout, menuHelpAbout });
            menuHelp.Name = "menuHelp";
            menuHelp.Size = new Size(65, 29);
            menuHelp.Text = "&Help";
            // 
            // menuHelpHowToUse
            // 
            menuHelpHowToUse.Name = "menuHelpHowToUse";
            menuHelpHowToUse.Size = new Size(336, 34);
            menuHelpHowToUse.Text = "&How to use…";
            // 
            // menuHelpSupport
            // 
            menuHelpSupport.Name = "menuHelpSupport";
            menuHelpSupport.Size = new Size(336, 34);
            menuHelpSupport.Text = "☕ &Support the developer…";
            // 
            // menuHelpSeparatorAbout
            // 
            menuHelpSeparatorAbout.Name = "menuHelpSeparatorAbout";
            menuHelpSeparatorAbout.Size = new Size(333, 6);
            // 
            // menuHelpAbout
            // 
            menuHelpAbout.Name = "menuHelpAbout";
            menuHelpAbout.Size = new Size(336, 34);
            menuHelpAbout.Text = "&About…";
            // 
            // statusStripMain
            // 
            statusStripMain.ImageScalingSize = new Size(20, 20);
            statusStripMain.Items.AddRange(new ToolStripItem[] { statusLabelMessage, statusLabelSelection, toolStripProgressBatch, statusLabelSpring });
            statusStripMain.Location = new Point(10, 857);
            statusStripMain.Name = "statusStripMain";
            statusStripMain.Padding = new Padding(1, 0, 16, 0);
            statusStripMain.Size = new Size(882, 32);
            statusStripMain.TabIndex = 0;
            statusStripMain.Text = "statusStripMain";
            // 
            // statusLabelMessage
            // 
            statusLabelMessage.Name = "statusLabelMessage";
            statusLabelMessage.Size = new Size(60, 25);
            statusLabelMessage.Text = "Ready";
            statusLabelMessage.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statusLabelSelection
            // 
            statusLabelSelection.BorderSides = ToolStripStatusLabelBorderSides.Left;
            statusLabelSelection.BorderStyle = Border3DStyle.Etched;
            statusLabelSelection.Margin = new Padding(12, 3, 12, 2);
            statusLabelSelection.Name = "statusLabelSelection";
            statusLabelSelection.Padding = new Padding(8, 0, 0, 0);
            statusLabelSelection.Size = new Size(12, 27);
            statusLabelSelection.TextAlign = ContentAlignment.MiddleLeft;
            statusLabelSelection.Visible = false;
            // 
            // toolStripProgressBatch
            // 
            toolStripProgressBatch.Name = "toolStripProgressBatch";
            toolStripProgressBatch.Size = new Size(160, 24);
            toolStripProgressBatch.Style = ProgressBarStyle.Continuous;
            toolStripProgressBatch.Visible = false;
            // 
            // statusLabelSpring
            // 
            statusLabelSpring.Name = "statusLabelSpring";
            statusLabelSpring.Size = new Size(805, 25);
            statusLabelSpring.Spring = true;
            // 
            // flowActions
            // 
            flowActions.AutoSize = true;
            flowActions.Controls.Add(btnUndo);
            flowActions.Controls.Add(btnCancel);
            flowActions.Dock = DockStyle.Bottom;
            flowActions.Location = new Point(10, 797);
            flowActions.Name = "flowActions";
            flowActions.Padding = new Padding(0, 8, 0, 8);
            flowActions.Size = new Size(882, 60);
            flowActions.TabIndex = 2;
            flowActions.WrapContents = false;
            // 
            // btnUndo
            // 
            btnUndo.AutoSize = true;
            btnUndo.Enabled = false;
            btnUndo.Location = new Point(3, 11);
            btnUndo.Margin = new Padding(3, 3, 12, 3);
            btnUndo.MinimumSize = new Size(100, 38);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(100, 38);
            btnUndo.TabIndex = 0;
            btnUndo.Text = "&Undo";
            btnUndo.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.AutoSize = true;
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(118, 11);
            btnCancel.MinimumSize = new Size(120, 38);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 38);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Ca&ncel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // splitReview
            // 
            splitReview.Dock = DockStyle.Fill;
            splitReview.Location = new Point(10, 43);
            splitReview.Name = "splitReview";
            splitReview.Orientation = Orientation.Horizontal;
            // 
            // splitReview.Panel1
            // 
            splitReview.Panel1.Controls.Add(grpFolders);
            splitReview.Panel1.Controls.Add(grpBackground);
            splitReview.Panel1.Controls.Add(grpExplorer);
            splitReview.Panel1MinSize = 210;
            // 
            // splitReview.Panel2
            // 
            splitReview.Panel2.Controls.Add(grpPreview);
            splitReview.Panel2MinSize = 140;
            splitReview.Size = new Size(882, 754);
            splitReview.SplitterDistance = 243;
            splitReview.SplitterWidth = 8;
            splitReview.TabIndex = 1;
            // 
            // grpFolders
            // 
            grpFolders.Controls.Add(lblSourceFolder);
            grpFolders.Controls.Add(txtSourceFolder);
            grpFolders.Controls.Add(btnBrowseSource);
            grpFolders.Dock = DockStyle.Top;
            grpFolders.Location = new Point(0, 164);
            grpFolders.Margin = new Padding(0, 0, 0, 8);
            grpFolders.Name = "grpFolders";
            grpFolders.Padding = new Padding(10, 11, 10, 11);
            grpFolders.Size = new Size(882, 84);
            grpFolders.TabIndex = 0;
            grpFolders.TabStop = false;
            grpFolders.Text = "Image folder";
            // 
            // lblSourceFolder
            // 
            lblSourceFolder.AutoSize = true;
            lblSourceFolder.Location = new Point(13, 40);
            lblSourceFolder.Name = "lblSourceFolder";
            lblSourceFolder.Size = new Size(62, 25);
            lblSourceFolder.TabIndex = 0;
            lblSourceFolder.Text = "Folder";
            // 
            // txtSourceFolder
            // 
            txtSourceFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSourceFolder.Location = new Point(143, 36);
            txtSourceFolder.Margin = new Padding(3, 4, 10, 4);
            txtSourceFolder.Name = "txtSourceFolder";
            txtSourceFolder.ReadOnly = true;
            txtSourceFolder.Size = new Size(609, 31);
            txtSourceFolder.TabIndex = 1;
            txtSourceFolder.TabStop = false;
            // 
            // btnBrowseSource
            // 
            btnBrowseSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseSource.AutoSize = true;
            btnBrowseSource.Location = new Point(769, 35);
            btnBrowseSource.Name = "btnBrowseSource";
            btnBrowseSource.Size = new Size(102, 35);
            btnBrowseSource.TabIndex = 2;
            btnBrowseSource.Text = "&Browse…";
            btnBrowseSource.UseVisualStyleBackColor = true;
            // 
            // grpBackground
            // 
            grpBackground.Controls.Add(lblCanvasFill);
            grpBackground.Controls.Add(cmbIcoOutputSize);
            grpBackground.Controls.Add(lblIcoOutputSize);
            grpBackground.Controls.Add(cmbSolidColor);
            grpBackground.Dock = DockStyle.Top;
            grpBackground.Location = new Point(0, 72);
            grpBackground.Margin = new Padding(0, 0, 0, 8);
            grpBackground.Name = "grpBackground";
            grpBackground.Padding = new Padding(10, 11, 10, 11);
            grpBackground.Size = new Size(882, 92);
            grpBackground.TabIndex = 2;
            grpBackground.TabStop = false;
            grpBackground.Text = "Canvas & background";
            grpBackground.Enter += grpBackground_Enter;
            // 
            // lblCanvasFill
            // 
            lblCanvasFill.AutoSize = true;
            lblCanvasFill.Location = new Point(20, 47);
            lblCanvasFill.Name = "lblCanvasFill";
            lblCanvasFill.Size = new Size(111, 25);
            lblCanvasFill.TabIndex = 0;
            lblCanvasFill.Text = "Background:";
            // 
            // cmbIcoOutputSize
            // 
            cmbIcoOutputSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIcoOutputSize.FormattingEnabled = true;
            cmbIcoOutputSize.Items.AddRange(new object[] { "16 × 16", "32 × 32", "48 × 48", "64 × 64", "128 × 128", "256 × 256" });
            cmbIcoOutputSize.Location = new Point(479, 43);
            cmbIcoOutputSize.Name = "cmbIcoOutputSize";
            cmbIcoOutputSize.Size = new Size(220, 33);
            cmbIcoOutputSize.TabIndex = 2;
            // 
            // lblIcoOutputSize
            // 
            lblIcoOutputSize.AutoSize = true;
            lblIcoOutputSize.Location = new Point(342, 47);
            lblIcoOutputSize.Name = "lblIcoOutputSize";
            lblIcoOutputSize.Size = new Size(139, 25);
            lblIcoOutputSize.TabIndex = 1;
            lblIcoOutputSize.Text = "ICO output size:";
            // 
            // cmbSolidColor
            // 
            cmbSolidColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSolidColor.FormattingEnabled = true;
            cmbSolidColor.Items.AddRange(new object[] { "White", "Black", "Transparent (preserve alpha)" });
            cmbSolidColor.Location = new Point(137, 43);
            cmbSolidColor.Name = "cmbSolidColor";
            cmbSolidColor.Size = new Size(167, 33);
            cmbSolidColor.TabIndex = 1;
            // 
            // grpExplorer
            // 
            grpExplorer.Controls.Add(btnRefreshExplorerConverterMenu);
            grpExplorer.Controls.Add(chkEnableExplorerConvertMenu);
            grpExplorer.Dock = DockStyle.Top;
            grpExplorer.Location = new Point(0, 0);
            grpExplorer.Margin = new Padding(0, 0, 0, 8);
            grpExplorer.Name = "grpExplorer";
            grpExplorer.Padding = new Padding(10, 11, 10, 11);
            grpExplorer.Size = new Size(882, 72);
            grpExplorer.TabIndex = 3;
            grpExplorer.TabStop = false;
            grpExplorer.Text = "Windows Explorer";
            // 
            // chkEnableExplorerConvertMenu
            // 
            chkEnableExplorerConvertMenu.AutoSize = true;
            chkEnableExplorerConvertMenu.Checked = true;
            chkEnableExplorerConvertMenu.CheckState = CheckState.Checked;
            chkEnableExplorerConvertMenu.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkEnableExplorerConvertMenu.Location = new Point(20, 32);
            chkEnableExplorerConvertMenu.Name = "chkEnableExplorerConvertMenu";
            chkEnableExplorerConvertMenu.Size = new Size(530, 29);
            chkEnableExplorerConvertMenu.TabIndex = 0;
            chkEnableExplorerConvertMenu.Text = "Add \"Converter To\" to Windows Explorer right-click menu";
            chkEnableExplorerConvertMenu.UseVisualStyleBackColor = true;
            // 
            // btnRefreshExplorerConverterMenu
            // 
            btnRefreshExplorerConverterMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshExplorerConverterMenu.AutoSize = true;
            btnRefreshExplorerConverterMenu.Location = new Point(668, 28);
            btnRefreshExplorerConverterMenu.Name = "btnRefreshExplorerConverterMenu";
            btnRefreshExplorerConverterMenu.Size = new Size(192, 35);
            btnRefreshExplorerConverterMenu.TabIndex = 1;
            btnRefreshExplorerConverterMenu.Text = "Refresh E&xplorer menu";
            btnRefreshExplorerConverterMenu.UseVisualStyleBackColor = true;
            // 
            // grpPreview
            // 
            grpPreview.Controls.Add(listViewPreview);
            grpPreview.Controls.Add(lblPreviewPlaceholder);
            grpPreview.Controls.Add(flowPreviewToolbar);
            grpPreview.Dock = DockStyle.Fill;
            grpPreview.Location = new Point(0, 0);
            grpPreview.Name = "grpPreview";
            grpPreview.Padding = new Padding(10, 11, 10, 11);
            grpPreview.Size = new Size(882, 503);
            grpPreview.TabIndex = 0;
            grpPreview.TabStop = false;
            grpPreview.Text = "Review / preview";
            // 
            // listViewPreview
            // 
            listViewPreview.AllowDrop = true;
            listViewPreview.ContextMenuStrip = contextMenuPreview;
            listViewPreview.Dock = DockStyle.Fill;
            listViewPreview.LargeImageList = imageListThumbnails;
            listViewPreview.Location = new Point(10, 84);
            listViewPreview.Name = "listViewPreview";
            listViewPreview.OwnerDraw = true;
            listViewPreview.Size = new Size(862, 408);
            listViewPreview.TabIndex = 0;
            listViewPreview.TileSize = new Size(120, 140);
            listViewPreview.UseCompatibleStateImageBehavior = false;
            listViewPreview.View = View.Tile;
            // 
            // contextMenuPreview
            // 
            contextMenuPreview.ImageScalingSize = new Size(24, 24);
            contextMenuPreview.Items.AddRange(new ToolStripItem[] { toolStripMenuItemConvertToQuickIcon, toolStripMenuItemPreviewConvertTo, toolStripMenuItemPreviewResize, toolStripSeparatorPreviewAfterConvert, toolStripMenuItemPreviewCopy, toolStripMenuItemPreviewCopyImagePath, toolStripSeparatorPreviewAfterClipboard, toolStripMenuItemPreviewRename, toolStripMenuItemPreviewDelete, toolStripSeparatorPreviewAfterFileOps, toolStripMenuItemOpenSourceLocation, toolStripSeparatorPreviewOpenWith, toolStripMenuItemOpenWithPaint, toolStripMenuItemOpenWithPaintDotNet, toolStripMenuItemPreviewPaste });
            contextMenuPreview.Name = "contextMenuPreview";
            contextMenuPreview.Size = new Size(232, 380);
            // 
            // toolStripMenuItemConvertToQuickIcon
            // 
            toolStripMenuItemConvertToQuickIcon.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            toolStripMenuItemConvertToQuickIcon.ForeColor = Color.FromArgb(0, 120, 215);
            toolStripMenuItemConvertToQuickIcon.Name = "toolStripMenuItemConvertToQuickIcon";
            toolStripMenuItemConvertToQuickIcon.Size = new Size(231, 32);
            toolStripMenuItemConvertToQuickIcon.Text = "Convert to &Icon";
            toolStripMenuItemConvertToQuickIcon.Visible = false;
            // 
            // toolStripMenuItemPreviewConvertTo
            // 
            toolStripMenuItemPreviewConvertTo.Name = "toolStripMenuItemPreviewConvertTo";
            toolStripMenuItemPreviewConvertTo.Size = new Size(231, 32);
            toolStripMenuItemPreviewConvertTo.Text = "&Convert to";
            toolStripMenuItemPreviewConvertTo.Visible = false;
            // 
            // toolStripMenuItemPreviewResize
            // 
            toolStripMenuItemPreviewResize.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemPreviewResize05x, toolStripMenuItemPreviewResize075x, toolStripSeparatorPreviewResizeScale, toolStripMenuItemPreviewResize2x, toolStripMenuItemPreviewResize4x });
            toolStripMenuItemPreviewResize.Name = "toolStripMenuItemPreviewResize";
            toolStripMenuItemPreviewResize.Size = new Size(231, 32);
            toolStripMenuItemPreviewResize.Text = "&Resize";
            toolStripMenuItemPreviewResize.Visible = false;
            // 
            // toolStripMenuItemPreviewResize05x
            // 
            toolStripMenuItemPreviewResize05x.Name = "toolStripMenuItemPreviewResize05x";
            toolStripMenuItemPreviewResize05x.Size = new Size(160, 34);
            toolStripMenuItemPreviewResize05x.Text = "0.&5×";
            // 
            // toolStripMenuItemPreviewResize075x
            // 
            toolStripMenuItemPreviewResize075x.Name = "toolStripMenuItemPreviewResize075x";
            toolStripMenuItemPreviewResize075x.Size = new Size(160, 34);
            toolStripMenuItemPreviewResize075x.Text = "0.&75×";
            // 
            // toolStripSeparatorPreviewResizeScale
            // 
            toolStripSeparatorPreviewResizeScale.Name = "toolStripSeparatorPreviewResizeScale";
            toolStripSeparatorPreviewResizeScale.Size = new Size(157, 6);
            // 
            // toolStripMenuItemPreviewResize2x
            // 
            toolStripMenuItemPreviewResize2x.Name = "toolStripMenuItemPreviewResize2x";
            toolStripMenuItemPreviewResize2x.Size = new Size(160, 34);
            toolStripMenuItemPreviewResize2x.Text = "&2×";
            // 
            // toolStripMenuItemPreviewResize4x
            // 
            toolStripMenuItemPreviewResize4x.Name = "toolStripMenuItemPreviewResize4x";
            toolStripMenuItemPreviewResize4x.Size = new Size(160, 34);
            toolStripMenuItemPreviewResize4x.Text = "&4×";
            // 
            // toolStripSeparatorPreviewAfterConvert
            // 
            toolStripSeparatorPreviewAfterConvert.Name = "toolStripSeparatorPreviewAfterConvert";
            toolStripSeparatorPreviewAfterConvert.Size = new Size(228, 6);
            toolStripSeparatorPreviewAfterConvert.Visible = false;
            // 
            // toolStripMenuItemPreviewCopy
            // 
            toolStripMenuItemPreviewCopy.Name = "toolStripMenuItemPreviewCopy";
            toolStripMenuItemPreviewCopy.Size = new Size(231, 32);
            toolStripMenuItemPreviewCopy.Text = "&Copy";
            toolStripMenuItemPreviewCopy.Visible = false;
            // 
            // toolStripMenuItemPreviewCopyImagePath
            // 
            toolStripMenuItemPreviewCopyImagePath.Name = "toolStripMenuItemPreviewCopyImagePath";
            toolStripMenuItemPreviewCopyImagePath.Size = new Size(231, 32);
            toolStripMenuItemPreviewCopyImagePath.Text = "Copy image &path";
            toolStripMenuItemPreviewCopyImagePath.Visible = false;
            // 
            // toolStripSeparatorPreviewAfterClipboard
            // 
            toolStripSeparatorPreviewAfterClipboard.Name = "toolStripSeparatorPreviewAfterClipboard";
            toolStripSeparatorPreviewAfterClipboard.Size = new Size(228, 6);
            toolStripSeparatorPreviewAfterClipboard.Visible = false;
            // 
            // toolStripMenuItemPreviewRename
            // 
            toolStripMenuItemPreviewRename.Name = "toolStripMenuItemPreviewRename";
            toolStripMenuItemPreviewRename.Size = new Size(231, 32);
            toolStripMenuItemPreviewRename.Text = "Rena&me";
            toolStripMenuItemPreviewRename.Visible = false;
            // 
            // toolStripMenuItemPreviewDelete
            // 
            toolStripMenuItemPreviewDelete.Name = "toolStripMenuItemPreviewDelete";
            toolStripMenuItemPreviewDelete.Size = new Size(231, 32);
            toolStripMenuItemPreviewDelete.Text = "&Delete";
            toolStripMenuItemPreviewDelete.Visible = false;
            // 
            // toolStripSeparatorPreviewAfterFileOps
            // 
            toolStripSeparatorPreviewAfterFileOps.Name = "toolStripSeparatorPreviewAfterFileOps";
            toolStripSeparatorPreviewAfterFileOps.Size = new Size(228, 6);
            toolStripSeparatorPreviewAfterFileOps.Visible = false;
            // 
            // toolStripMenuItemOpenSourceLocation
            // 
            toolStripMenuItemOpenSourceLocation.Name = "toolStripMenuItemOpenSourceLocation";
            toolStripMenuItemOpenSourceLocation.Size = new Size(231, 32);
            toolStripMenuItemOpenSourceLocation.Text = "Open file &location";
            toolStripMenuItemOpenSourceLocation.Visible = false;
            // 
            // toolStripSeparatorPreviewOpenWith
            // 
            toolStripSeparatorPreviewOpenWith.Name = "toolStripSeparatorPreviewOpenWith";
            toolStripSeparatorPreviewOpenWith.Size = new Size(228, 6);
            toolStripSeparatorPreviewOpenWith.Visible = false;
            // 
            // toolStripMenuItemOpenWithPaint
            // 
            toolStripMenuItemOpenWithPaint.Name = "toolStripMenuItemOpenWithPaint";
            toolStripMenuItemOpenWithPaint.Size = new Size(231, 32);
            toolStripMenuItemOpenWithPaint.Text = "Open by &Paint";
            toolStripMenuItemOpenWithPaint.Visible = false;
            // 
            // toolStripMenuItemOpenWithPaintDotNet
            // 
            toolStripMenuItemOpenWithPaintDotNet.Name = "toolStripMenuItemOpenWithPaintDotNet";
            toolStripMenuItemOpenWithPaintDotNet.Size = new Size(231, 32);
            toolStripMenuItemOpenWithPaintDotNet.Text = "Open by Paint.&NET";
            toolStripMenuItemOpenWithPaintDotNet.Visible = false;
            // 
            // toolStripMenuItemPreviewPaste
            // 
            toolStripMenuItemPreviewPaste.Name = "toolStripMenuItemPreviewPaste";
            toolStripMenuItemPreviewPaste.Size = new Size(231, 32);
            toolStripMenuItemPreviewPaste.Text = "&Paste";
            toolStripMenuItemPreviewPaste.Visible = false;
            // 
            // imageListThumbnails
            // 
            imageListThumbnails.ColorDepth = ColorDepth.Depth32Bit;
            imageListThumbnails.ImageSize = new Size(64, 64);
            imageListThumbnails.TransparentColor = Color.Transparent;
            // 
            // lblPreviewPlaceholder
            // 
            lblPreviewPlaceholder.AllowDrop = true;
            lblPreviewPlaceholder.BackColor = Color.FromArgb(245, 245, 245);
            lblPreviewPlaceholder.ContextMenuStrip = contextMenuPreview;
            lblPreviewPlaceholder.Dock = DockStyle.Fill;
            lblPreviewPlaceholder.ForeColor = SystemColors.GrayText;
            lblPreviewPlaceholder.Location = new Point(10, 84);
            lblPreviewPlaceholder.Name = "lblPreviewPlaceholder";
            lblPreviewPlaceholder.Size = new Size(862, 408);
            lblPreviewPlaceholder.TabIndex = 1;
            lblPreviewPlaceholder.Text = "Choose an image folder or drop files / a folder here. Thumbnails appear when images are loaded.";
            lblPreviewPlaceholder.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // flowPreviewToolbar
            // 
            flowPreviewToolbar.AutoSize = true;
            flowPreviewToolbar.Controls.Add(lblPreviewView);
            flowPreviewToolbar.Controls.Add(cmbPreviewSize);
            flowPreviewToolbar.Controls.Add(btnRefreshPreview);
            flowPreviewToolbar.Controls.Add(btnOpenFolder);
            flowPreviewToolbar.Dock = DockStyle.Top;
            flowPreviewToolbar.Location = new Point(10, 35);
            flowPreviewToolbar.Name = "flowPreviewToolbar";
            flowPreviewToolbar.Padding = new Padding(0, 0, 0, 6);
            flowPreviewToolbar.Size = new Size(862, 49);
            flowPreviewToolbar.TabIndex = 2;
            flowPreviewToolbar.WrapContents = false;
            // 
            // lblPreviewView
            // 
            lblPreviewView.AutoSize = true;
            lblPreviewView.Location = new Point(3, 10);
            lblPreviewView.Margin = new Padding(3, 10, 8, 4);
            lblPreviewView.Name = "lblPreviewView";
            lblPreviewView.Size = new Size(133, 25);
            lblPreviewView.TabIndex = 10;
            lblPreviewView.Text = "Thumbnail size:";
            // 
            // cmbPreviewSize
            // 
            cmbPreviewSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPreviewSize.Location = new Point(147, 4);
            cmbPreviewSize.Margin = new Padding(3, 4, 16, 4);
            cmbPreviewSize.Name = "cmbPreviewSize";
            cmbPreviewSize.Size = new Size(140, 33);
            cmbPreviewSize.TabIndex = 11;
            // 
            // btnRefreshPreview
            // 
            btnRefreshPreview.AutoSize = true;
            btnRefreshPreview.Location = new Point(306, 4);
            btnRefreshPreview.Margin = new Padding(3, 4, 10, 4);
            btnRefreshPreview.Name = "btnRefreshPreview";
            btnRefreshPreview.Size = new Size(142, 35);
            btnRefreshPreview.TabIndex = 0;
            btnRefreshPreview.Text = "&Refresh review";
            btnRefreshPreview.UseVisualStyleBackColor = true;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.AutoSize = true;
            btnOpenFolder.Location = new Point(461, 4);
            btnOpenFolder.Margin = new Padding(3, 4, 3, 4);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(140, 35);
            btnOpenFolder.TabIndex = 1;
            btnOpenFolder.Text = "Open &folder…";
            btnOpenFolder.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(902, 899);
            Controls.Add(splitReview);
            Controls.Add(flowActions);
            Controls.Add(statusStripMain);
            Controls.Add(menuStripMain);
            Font = new Font("Segoe UI", 9F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStripMain;
            MinimumSize = new Size(900, 600);
            Name = "frmMain";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "    Image Converter";
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            statusStripMain.ResumeLayout(false);
            statusStripMain.PerformLayout();
            flowActions.ResumeLayout(false);
            flowActions.PerformLayout();
            splitReview.Panel1.ResumeLayout(false);
            splitReview.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitReview).EndInit();
            splitReview.ResumeLayout(false);
            grpFolders.ResumeLayout(false);
            grpFolders.PerformLayout();
            grpBackground.ResumeLayout(false);
            grpBackground.PerformLayout();
            grpExplorer.ResumeLayout(false);
            grpExplorer.PerformLayout();
            grpPreview.ResumeLayout(false);
            grpPreview.PerformLayout();
            contextMenuPreview.ResumeLayout(false);
            flowPreviewToolbar.ResumeLayout(false);
            flowPreviewToolbar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStripMain;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuFileOpenFolder;
        private ToolStripMenuItem menuFileRefreshReview;
        private ToolStripSeparator menuFileSeparatorAfterWorkspace;
        private ToolStripMenuItem menuFileOpenFolderInExplorer;
        private ToolStripMenuItem menuFileOpenAppLocation;
        private ToolStripSeparator menuFileSeparatorAfterLocations;
        private ToolStripMenuItem menuFilePasteImage;
        private ToolStripMenuItem menuFileUndo;
        private ToolStripSeparator menuFileSeparatorExit;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuHelpHowToUse;
        private ToolStripMenuItem menuHelpSupport;
        private ToolStripSeparator menuHelpSeparatorAbout;
        private ToolStripMenuItem menuHelpAbout;
        private StatusStrip statusStripMain;
        private ToolStripStatusLabel statusLabelMessage;
        private ToolStripStatusLabel statusLabelSelection;
        private ToolStripProgressBar toolStripProgressBatch;
        private ToolStripStatusLabel statusLabelSpring;
        private FlowLayoutPanel flowActions;
        private SplitContainer splitReview;
        private GroupBox grpFolders;
        private Label lblSourceFolder;
        private TextBox txtSourceFolder;
        private Button btnBrowseSource;
        private Label lblIcoOutputSize;
        private ComboBox cmbIcoOutputSize;
        private GroupBox grpExplorer;
        private CheckBox chkEnableExplorerConvertMenu;
        private Button btnRefreshExplorerConverterMenu;
        private GroupBox grpBackground;
        private Label lblCanvasFill;
        private ComboBox cmbSolidColor;
        private GroupBox grpPreview;
        private FlowLayoutPanel flowPreviewToolbar;
        private Label lblPreviewView;
        private ComboBox cmbPreviewSize;
        private Button btnRefreshPreview;
        private Button btnOpenFolder;
        private ContextMenuStrip contextMenuPreview;
        private ToolStripMenuItem toolStripMenuItemPreviewConvertTo;
        private ToolStripMenuItem toolStripMenuItemPreviewResize;
        private ToolStripMenuItem toolStripMenuItemPreviewResize05x;
        private ToolStripMenuItem toolStripMenuItemPreviewResize075x;
        private ToolStripSeparator toolStripSeparatorPreviewResizeScale;
        private ToolStripMenuItem toolStripMenuItemPreviewResize2x;
        private ToolStripMenuItem toolStripMenuItemPreviewResize4x;
        private ToolStripMenuItem toolStripMenuItemConvertToQuickIcon;
        private ToolStripSeparator toolStripSeparatorPreviewAfterConvert;
        private ToolStripMenuItem toolStripMenuItemPreviewCopy;
        private ToolStripMenuItem toolStripMenuItemPreviewCopyImagePath;
        private ToolStripSeparator toolStripSeparatorPreviewAfterClipboard;
        private ToolStripMenuItem toolStripMenuItemPreviewRename;
        private ToolStripMenuItem toolStripMenuItemPreviewPaste;
        private ToolStripMenuItem toolStripMenuItemPreviewDelete;
        private ToolStripSeparator toolStripSeparatorPreviewAfterFileOps;
        private ToolStripMenuItem toolStripMenuItemOpenSourceLocation;
        private ToolStripSeparator toolStripSeparatorPreviewOpenWith;
        private ToolStripMenuItem toolStripMenuItemOpenWithPaint;
        private ToolStripMenuItem toolStripMenuItemOpenWithPaintDotNet;
        private ListView listViewPreview;
        private ImageList imageListThumbnails;
        private Label lblPreviewPlaceholder;
        private Button btnUndo;
        private Button btnCancel;
    }
}
