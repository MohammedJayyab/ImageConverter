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
            menuFileOpenFolderInExplorer = new ToolStripMenuItem();
            menuFilePasteImage = new ToolStripMenuItem();
            menuFileUndo = new ToolStripMenuItem();
            menuFileSeparatorExit = new ToolStripSeparator();
            menuFileExit = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            menuHelpHowToUse = new ToolStripMenuItem();
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
            grpPreview = new GroupBox();
            listViewPreview = new ListView();
            contextMenuPreview = new ContextMenuStrip(components);
            toolStripMenuItemConvertToQuickIcon = new ToolStripMenuItem();
            toolStripMenuItemPreviewConvertTo = new ToolStripMenuItem();
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
            menuStripMain.Padding = new Padding(6, 2, 0, 2);
            menuStripMain.Size = new Size(1121, 32);
            menuStripMain.TabIndex = 3;
            menuStripMain.Text = "menuStripMain";
            //
            // menuFile
            //
            menuFile.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuFileOpenFolder,
                menuFileRefreshReview,
                menuFileOpenFolderInExplorer,
                menuFilePasteImage,
                menuFileUndo,
                menuFileSeparatorExit,
                menuFileExit
            });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(46, 28);
            menuFile.Text = "&File";
            //
            // menuFileOpenFolder
            //
            menuFileOpenFolder.Name = "menuFileOpenFolder";
            menuFileOpenFolder.ShortcutKeys = Keys.Control | Keys.O;
            menuFileOpenFolder.Size = new Size(280, 34);
            menuFileOpenFolder.Text = "&Open folder…";
            //
            // menuFileRefreshReview
            //
            menuFileRefreshReview.Name = "menuFileRefreshReview";
            menuFileRefreshReview.ShortcutKeys = Keys.F5;
            menuFileRefreshReview.Size = new Size(280, 34);
            menuFileRefreshReview.Text = "&Refresh review";
            //
            // menuFileOpenFolderInExplorer
            //
            menuFileOpenFolderInExplorer.Name = "menuFileOpenFolderInExplorer";
            menuFileOpenFolderInExplorer.Size = new Size(280, 34);
            menuFileOpenFolderInExplorer.Text = "Open folder in &Explorer…";
            //
            // menuFilePasteImage
            //
            menuFilePasteImage.Name = "menuFilePasteImage";
            menuFilePasteImage.ShortcutKeys = Keys.Control | Keys.V;
            menuFilePasteImage.Size = new Size(280, 34);
            menuFilePasteImage.Text = "&Paste image";
            //
            // menuFileUndo
            //
            menuFileUndo.Name = "menuFileUndo";
            menuFileUndo.ShortcutKeys = Keys.Control | Keys.Z;
            menuFileUndo.Size = new Size(280, 34);
            menuFileUndo.Text = "&Undo";
            //
            // menuFileSeparatorExit
            //
            menuFileSeparatorExit.Name = "menuFileSeparatorExit";
            menuFileSeparatorExit.Size = new Size(277, 6);
            //
            // menuFileExit
            //
            menuFileExit.Name = "menuFileExit";
            menuFileExit.ShortcutKeys = Keys.Alt | Keys.F4;
            menuFileExit.Size = new Size(280, 34);
            menuFileExit.Text = "E&xit";
            //
            // menuHelp
            //
            menuHelp.DropDownItems.AddRange(new ToolStripItem[] { menuHelpHowToUse, menuHelpAbout });
            menuHelp.Name = "menuHelp";
            menuHelp.Size = new Size(55, 28);
            menuHelp.Text = "&Help";
            //
            // menuHelpHowToUse
            //
            menuHelpHowToUse.Name = "menuHelpHowToUse";
            menuHelpHowToUse.Size = new Size(240, 34);
            menuHelpHowToUse.Text = "&How to use…";
            //
            // menuHelpAbout
            //
            menuHelpAbout.Name = "menuHelpAbout";
            menuHelpAbout.Size = new Size(240, 34);
            menuHelpAbout.Text = "&About…";
            // 
            // statusStripMain
            // 
            statusStripMain.ImageScalingSize = new Size(20, 20);
            statusStripMain.Items.AddRange(new ToolStripItem[] { statusLabelMessage, statusLabelSelection, toolStripProgressBatch, statusLabelSpring });
            statusStripMain.Location = new Point(10, 856);
            statusStripMain.Name = "statusStripMain";
            statusStripMain.Padding = new Padding(1, 0, 16, 0);
            statusStripMain.Size = new Size(1121, 32);
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
            statusLabelSpring.Size = new Size(1044, 25);
            statusLabelSpring.Spring = true;
            // 
            // flowActions
            // 
            flowActions.AutoSize = true;
            flowActions.Controls.Add(btnUndo);
            flowActions.Controls.Add(btnCancel);
            flowActions.Dock = DockStyle.Bottom;
            flowActions.Location = new Point(10, 796);
            flowActions.Name = "flowActions";
            flowActions.Padding = new Padding(0, 8, 0, 8);
            flowActions.Size = new Size(1121, 60);
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
            splitReview.Location = new Point(10, 10);
            splitReview.Name = "splitReview";
            splitReview.Orientation = Orientation.Horizontal;
            // 
            // splitReview.Panel1
            // 
            splitReview.Panel1.Controls.Add(grpFolders);
            splitReview.Panel1.Controls.Add(grpBackground);
            splitReview.Panel1MinSize = 140;
            // 
            // splitReview.Panel2
            // 
            splitReview.Panel2.Controls.Add(grpPreview);
            splitReview.Panel2MinSize = 140;
            splitReview.Size = new Size(1121, 786);
            splitReview.SplitterDistance = 198;
            splitReview.SplitterWidth = 8;
            splitReview.TabIndex = 1;
            // 
            // grpFolders
            // 
            grpFolders.Controls.Add(lblSourceFolder);
            grpFolders.Controls.Add(txtSourceFolder);
            grpFolders.Controls.Add(btnBrowseSource);
            grpFolders.Dock = DockStyle.Top;
            grpFolders.Location = new Point(0, 72);
            grpFolders.Margin = new Padding(0, 0, 0, 8);
            grpFolders.Name = "grpFolders";
            grpFolders.Padding = new Padding(10, 11, 10, 11);
            grpFolders.Size = new Size(1121, 84);
            grpFolders.TabIndex = 0;
            grpFolders.TabStop = false;
            grpFolders.Text = "Image folder";
            // 
            // lblSourceFolder
            // 
            lblSourceFolder.AutoSize = true;
            lblSourceFolder.Location = new Point(13, 40);
            lblSourceFolder.Name = "lblSourceFolder";
            lblSourceFolder.Size = new Size(58, 25);
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
            txtSourceFolder.Size = new Size(848, 31);
            txtSourceFolder.TabIndex = 1;
            txtSourceFolder.TabStop = false;
            // 
            // btnBrowseSource
            // 
            btnBrowseSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseSource.AutoSize = true;
            btnBrowseSource.Location = new Point(1008, 35);
            btnBrowseSource.Name = "btnBrowseSource";
            btnBrowseSource.Size = new Size(97, 35);
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
            grpBackground.Location = new Point(0, 0);
            grpBackground.Margin = new Padding(0, 0, 0, 8);
            grpBackground.Name = "grpBackground";
            grpBackground.Padding = new Padding(10, 11, 10, 11);
            grpBackground.Size = new Size(1121, 72);
            grpBackground.TabIndex = 2;
            grpBackground.TabStop = false;
            grpBackground.Text = "ICO canvas (letterbox)";
            // 
            // lblCanvasFill
            // 
            lblCanvasFill.AutoSize = true;
            lblCanvasFill.Location = new Point(13, 31);
            lblCanvasFill.Name = "lblCanvasFill";
            lblCanvasFill.Size = new Size(135, 25);
            lblCanvasFill.TabIndex = 0;
            lblCanvasFill.Text = "Letterbox color:";
            // 
            // cmbIcoOutputSize
            // 
            cmbIcoOutputSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIcoOutputSize.FormattingEnabled = true;
            cmbIcoOutputSize.Items.AddRange(new object[] { "16 × 16", "32 × 32", "48 × 48", "64 × 64", "128 × 128", "256 × 256" });
            cmbIcoOutputSize.Location = new Point(444, 24);
            cmbIcoOutputSize.Name = "cmbIcoOutputSize";
            cmbIcoOutputSize.Size = new Size(220, 33);
            cmbIcoOutputSize.TabIndex = 2;
            // 
            // lblIcoOutputSize
            // 
            lblIcoOutputSize.AutoSize = true;
            lblIcoOutputSize.Location = new Point(293, 28);
            lblIcoOutputSize.Name = "lblIcoOutputSize";
            lblIcoOutputSize.Size = new Size(139, 25);
            lblIcoOutputSize.TabIndex = 1;
            lblIcoOutputSize.Text = "ICO output size:";
            // 
            // cmbSolidColor
            // 
            cmbSolidColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSolidColor.FormattingEnabled = true;
            cmbSolidColor.Items.AddRange(new object[] { "White", "Black" });
            cmbSolidColor.Location = new Point(158, 27);
            cmbSolidColor.Name = "cmbSolidColor";
            cmbSolidColor.Size = new Size(120, 33);
            cmbSolidColor.TabIndex = 1;
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
            grpPreview.Size = new Size(1121, 540);
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
            listViewPreview.Size = new Size(1101, 445);
            listViewPreview.TabIndex = 0;
            listViewPreview.TileSize = new Size(120, 140);
            listViewPreview.UseCompatibleStateImageBehavior = false;
            listViewPreview.View = View.Tile;
            // 
            // contextMenuPreview
            // 
            contextMenuPreview.ImageScalingSize = new Size(24, 24);
            contextMenuPreview.Items.AddRange(new ToolStripItem[]
            {
                toolStripMenuItemConvertToQuickIcon,
                toolStripMenuItemPreviewConvertTo,
                toolStripSeparatorPreviewAfterConvert,
                toolStripMenuItemPreviewCopy,
                toolStripMenuItemPreviewCopyImagePath,
                toolStripSeparatorPreviewAfterClipboard,
                toolStripMenuItemPreviewRename,
                toolStripMenuItemPreviewDelete,
                toolStripSeparatorPreviewAfterFileOps,
                toolStripMenuItemOpenSourceLocation,
                toolStripSeparatorPreviewOpenWith,
                toolStripMenuItemOpenWithPaint,
                toolStripMenuItemOpenWithPaintDotNet,
                toolStripMenuItemPreviewPaste
            });
            contextMenuPreview.Name = "contextMenuPreview";
            contextMenuPreview.Size = new Size(232, 298);
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
            lblPreviewPlaceholder.Size = new Size(1101, 445);
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
            flowPreviewToolbar.Size = new Size(1101, 49);
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
            ClientSize = new Size(1141, 898);
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
            Text = "Image Converter";
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
        private ToolStripMenuItem menuFileOpenFolderInExplorer;
        private ToolStripMenuItem menuFilePasteImage;
        private ToolStripMenuItem menuFileUndo;
        private ToolStripSeparator menuFileSeparatorExit;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuHelpHowToUse;
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
