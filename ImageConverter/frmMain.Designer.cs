namespace ImageConverter
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            folderBrowserDialog = new FolderBrowserDialog();
            statusStripMain = new StatusStrip();
            statusLabelMessage = new ToolStripStatusLabel();
            statusLabelSelection = new ToolStripStatusLabel();
            toolStripProgressBatch = new ToolStripProgressBar();
            statusLabelSpring = new ToolStripStatusLabel();
            panelMain = new Panel();
            tableLayoutOuter = new TableLayoutPanel();
            tableLayoutRoot = new TableLayoutPanel();
            splitReview = new SplitContainer();
            tableLayoutSettings = new TableLayoutPanel();
            grpFolders = new GroupBox();
            tableLayoutFolders = new TableLayoutPanel();
            lblSourceFolder = new Label();
            txtSourceFolder = new TextBox();
            btnBrowseSource = new Button();
            lblDestFolder = new Label();
            txtDestFolder = new TextBox();
            btnBrowseDest = new Button();
            grpFormats = new GroupBox();
            tableLayoutFormats = new TableLayoutPanel();
            lblIcoHint = new Label();
            panelIcoSizes = new FlowLayoutPanel();
            lblIcoOutputSize = new Label();
            cmbIcoOutputSize = new ComboBox();
            grpBackground = new GroupBox();
            flowBackground = new FlowLayoutPanel();
            lblCanvasFill = new Label();
            cmbSolidColor = new ComboBox();
            grpPreview = new GroupBox();
            panelPreviewToolbar = new FlowLayoutPanel();
            lblPreviewView = new Label();
            cmbPreviewSize = new ComboBox();
            btnRefreshPreview = new Button();
            btnOpenDestination = new Button();
            contextMenuPreview = new ContextMenuStrip(components);
            toolStripMenuItemPreviewCopy = new ToolStripMenuItem();
            toolStripMenuItemPreviewPaste = new ToolStripMenuItem();
            toolStripMenuItemPreviewConvertTo = new ToolStripMenuItem();
            toolStripMenuItemConvertToQuickIcon = new ToolStripMenuItem();
            toolStripMenuItemPreviewDelete = new ToolStripMenuItem();
            toolStripMenuItemOpenSourceLocation = new ToolStripMenuItem();
            listViewPreview = new ListView();
            imageListThumbnails = new ImageList(components);
            lblPreviewPlaceholder = new Label();
            panelActions = new Panel();
            btnUndo = new Button();
            btnCancel = new Button();
            statusStripMain.SuspendLayout();
            panelMain.SuspendLayout();
            tableLayoutOuter.SuspendLayout();
            tableLayoutRoot.SuspendLayout();
            splitReview.SuspendLayout();
            tableLayoutSettings.SuspendLayout();
            grpFolders.SuspendLayout();
            tableLayoutFolders.SuspendLayout();
            grpFormats.SuspendLayout();
            tableLayoutFormats.SuspendLayout();
            panelIcoSizes.SuspendLayout();
            grpBackground.SuspendLayout();
            flowBackground.SuspendLayout();
            grpPreview.SuspendLayout();
            panelPreviewToolbar.SuspendLayout();
            panelActions.SuspendLayout();
            SuspendLayout();
            // 
            // folderBrowserDialog
            // 
            folderBrowserDialog.AutoUpgradeEnabled = false;
            folderBrowserDialog.Description = "Select a folder";
            folderBrowserDialog.UseDescriptionForTitle = true;
            // 
            // statusStripMain
            // 
            statusStripMain.ImageScalingSize = new Size(20, 20);
            statusStripMain.Items.AddRange(new ToolStripItem[] { statusLabelMessage, statusLabelSelection, toolStripProgressBatch, statusLabelSpring });
            statusStripMain.Location = new Point(0, 713);
            statusStripMain.Name = "statusStripMain";
            statusStripMain.Padding = new Padding(1, 0, 16, 0);
            statusStripMain.Size = new Size(1100, 32);
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
            statusLabelSelection.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            statusLabelSelection.Margin = new Padding(12, 3, 12, 2);
            statusLabelSelection.Name = "statusLabelSelection";
            statusLabelSelection.Padding = new Padding(8, 0, 0, 0);
            statusLabelSelection.Size = new Size(4, 25);
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
            statusLabelSpring.Size = new Size(1023, 25);
            statusLabelSpring.Spring = true;
            // 
            // panelMain
            // 
            panelMain.Controls.Add(tableLayoutOuter);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Margin = new Padding(3, 4, 3, 4);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(10, 11, 10, 11);
            panelMain.Size = new Size(1100, 713);
            panelMain.TabIndex = 1;
            // 
            // tableLayoutOuter
            // 
            tableLayoutOuter.ColumnCount = 1;
            tableLayoutOuter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutOuter.Controls.Add(tableLayoutRoot, 0, 0);
            tableLayoutOuter.Controls.Add(panelActions, 0, 1);
            tableLayoutOuter.Dock = DockStyle.Fill;
            tableLayoutOuter.Location = new Point(10, 11);
            tableLayoutOuter.Margin = new Padding(0);
            tableLayoutOuter.Name = "tableLayoutOuter";
            tableLayoutOuter.RowCount = 2;
            tableLayoutOuter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutOuter.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableLayoutOuter.Size = new Size(1080, 691);
            tableLayoutOuter.TabIndex = 0;
            // 
            // tableLayoutRoot
            // 
            tableLayoutRoot.ColumnCount = 1;
            tableLayoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutRoot.Controls.Add(splitReview, 0, 0);
            tableLayoutRoot.Dock = DockStyle.Fill;
            tableLayoutRoot.Location = new Point(3, 4);
            tableLayoutRoot.Margin = new Padding(3, 4, 3, 4);
            tableLayoutRoot.Name = "tableLayoutRoot";
            tableLayoutRoot.RowCount = 1;
            tableLayoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutRoot.Size = new Size(1074, 603);
            tableLayoutRoot.TabIndex = 0;
            // 
            // splitReview
            // 
            splitReview.Dock = DockStyle.Fill;
            splitReview.Location = new Point(3, 4);
            splitReview.Margin = new Padding(3, 4, 3, 4);
            splitReview.Name = "splitReview";
            splitReview.Orientation = Orientation.Horizontal;
            splitReview.Panel1MinSize = 180;
            splitReview.Panel2MinSize = 140;
            splitReview.Size = new Size(1074, 603);
            splitReview.SplitterDistance = 410;
            splitReview.SplitterWidth = 8;
            splitReview.TabIndex = 0;
            splitReview.Panel1.Controls.Add(tableLayoutSettings);
            splitReview.Panel2.Controls.Add(grpPreview);
            // 
            // tableLayoutSettings
            // 
            tableLayoutSettings.ColumnCount = 1;
            tableLayoutSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutSettings.Controls.Add(grpFolders, 0, 0);
            tableLayoutSettings.Controls.Add(grpFormats, 0, 1);
            tableLayoutSettings.Controls.Add(grpBackground, 0, 2);
            tableLayoutSettings.Dock = DockStyle.Fill;
            tableLayoutSettings.Location = new Point(0, 0);
            tableLayoutSettings.Margin = new Padding(0);
            tableLayoutSettings.Name = "tableLayoutSettings";
            tableLayoutSettings.RowCount = 3;
            tableLayoutSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutSettings.Size = new Size(1074, 434);
            tableLayoutSettings.TabIndex = 0;
            // 
            // grpFolders
            // 
            grpFolders.AutoSize = true;
            grpFolders.Controls.Add(tableLayoutFolders);
            grpFolders.Dock = DockStyle.Fill;
            grpFolders.Location = new Point(3, 4);
            grpFolders.Margin = new Padding(3, 4, 3, 8);
            grpFolders.Name = "grpFolders";
            grpFolders.Padding = new Padding(10, 11, 10, 11);
            grpFolders.Size = new Size(1068, 126);
            grpFolders.TabIndex = 0;
            grpFolders.TabStop = false;
            grpFolders.Text = "Source && destination";
            // 
            // tableLayoutFolders
            // 
            tableLayoutFolders.AutoSize = true;
            tableLayoutFolders.ColumnCount = 3;
            tableLayoutFolders.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tableLayoutFolders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutFolders.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tableLayoutFolders.Controls.Add(lblSourceFolder, 0, 0);
            tableLayoutFolders.Controls.Add(txtSourceFolder, 1, 0);
            tableLayoutFolders.Controls.Add(btnBrowseSource, 2, 0);
            tableLayoutFolders.Controls.Add(lblDestFolder, 0, 1);
            tableLayoutFolders.Controls.Add(txtDestFolder, 1, 1);
            tableLayoutFolders.Controls.Add(btnBrowseDest, 2, 1);
            tableLayoutFolders.Dock = DockStyle.Fill;
            tableLayoutFolders.Location = new Point(10, 35);
            tableLayoutFolders.Margin = new Padding(3, 4, 3, 4);
            tableLayoutFolders.Name = "tableLayoutFolders";
            tableLayoutFolders.RowCount = 2;
            tableLayoutFolders.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutFolders.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutFolders.Size = new Size(1048, 80);
            tableLayoutFolders.TabIndex = 0;
            // 
            // lblSourceFolder
            // 
            lblSourceFolder.Anchor = AnchorStyles.Left;
            lblSourceFolder.AutoSize = true;
            lblSourceFolder.Location = new Point(3, 7);
            lblSourceFolder.Name = "lblSourceFolder";
            lblSourceFolder.Size = new Size(118, 25);
            lblSourceFolder.TabIndex = 0;
            lblSourceFolder.Text = "Source folder";
            lblSourceFolder.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSourceFolder
            // 
            txtSourceFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtSourceFolder.Location = new Point(133, 4);
            txtSourceFolder.Margin = new Padding(3, 4, 10, 4);
            txtSourceFolder.Name = "txtSourceFolder";
            txtSourceFolder.ReadOnly = true;
            txtSourceFolder.Size = new Size(795, 31);
            txtSourceFolder.TabIndex = 1;
            txtSourceFolder.TabStop = false;
            // 
            // btnBrowseSource
            // 
            btnBrowseSource.Anchor = AnchorStyles.Right;
            btnBrowseSource.AutoSize = true;
            btnBrowseSource.Location = new Point(948, 3);
            btnBrowseSource.Name = "btnBrowseSource";
            btnBrowseSource.Size = new Size(97, 34);
            btnBrowseSource.TabIndex = 2;
            btnBrowseSource.Text = "&Browse…";
            btnBrowseSource.UseVisualStyleBackColor = true;
            // 
            // lblDestFolder
            // 
            lblDestFolder.Anchor = AnchorStyles.Left;
            lblDestFolder.AutoSize = true;
            lblDestFolder.Location = new Point(3, 47);
            lblDestFolder.Name = "lblDestFolder";
            lblDestFolder.Size = new Size(102, 25);
            lblDestFolder.TabIndex = 3;
            lblDestFolder.Text = "Destination";
            lblDestFolder.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDestFolder
            // 
            txtDestFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtDestFolder.Location = new Point(133, 44);
            txtDestFolder.Margin = new Padding(3, 4, 10, 4);
            txtDestFolder.Name = "txtDestFolder";
            txtDestFolder.ReadOnly = true;
            txtDestFolder.Size = new Size(795, 31);
            txtDestFolder.TabIndex = 4;
            txtDestFolder.TabStop = false;
            // 
            // btnBrowseDest
            // 
            btnBrowseDest.Anchor = AnchorStyles.Right;
            btnBrowseDest.AutoSize = true;
            btnBrowseDest.Location = new Point(948, 43);
            btnBrowseDest.Name = "btnBrowseDest";
            btnBrowseDest.Size = new Size(97, 34);
            btnBrowseDest.TabIndex = 5;
            btnBrowseDest.Text = "B&rowse…";
            btnBrowseDest.UseVisualStyleBackColor = true;
            // 
            // grpFormats
            // 
            grpFormats.AutoSize = true;
            grpFormats.Controls.Add(tableLayoutFormats);
            grpFormats.Dock = DockStyle.Fill;
            grpFormats.Location = new Point(3, 142);
            grpFormats.Margin = new Padding(3, 4, 3, 8);
            grpFormats.Name = "grpFormats";
            grpFormats.Padding = new Padding(10, 11, 10, 11);
            grpFormats.Size = new Size(1068, 170);
            grpFormats.TabIndex = 1;
            grpFormats.TabStop = false;
            grpFormats.Text = "Icon output (.ico)";
            // 
            // tableLayoutFormats
            // 
            tableLayoutFormats.AutoSize = true;
            tableLayoutFormats.ColumnCount = 1;
            tableLayoutFormats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutFormats.Controls.Add(lblIcoHint, 0, 0);
            tableLayoutFormats.Controls.Add(panelIcoSizes, 0, 1);
            tableLayoutFormats.Dock = DockStyle.Fill;
            tableLayoutFormats.Location = new Point(10, 35);
            tableLayoutFormats.Margin = new Padding(3, 4, 3, 4);
            tableLayoutFormats.Name = "tableLayoutFormats";
            tableLayoutFormats.RowCount = 2;
            tableLayoutFormats.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutFormats.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutFormats.Size = new Size(1048, 124);
            tableLayoutFormats.TabIndex = 0;
            // 
            // lblIcoHint
            // 
            lblIcoHint.AutoSize = true;
            lblIcoHint.Dock = DockStyle.Fill;
            lblIcoHint.ForeColor = SystemColors.GrayText;
            lblIcoHint.Margin = new Padding(3, 4, 3, 0);
            lblIcoHint.Name = "lblIcoHint";
            lblIcoHint.TabIndex = 0;
            lblIcoHint.Text = "These settings apply when you convert to ICO from the thumbnail menu.";
            // 
            // panelIcoSizes
            // 
            panelIcoSizes.AutoSize = true;
            panelIcoSizes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelIcoSizes.Controls.Add(lblIcoOutputSize);
            panelIcoSizes.Controls.Add(cmbIcoOutputSize);
            panelIcoSizes.Dock = DockStyle.Fill;
            panelIcoSizes.Margin = new Padding(3, 2, 3, 0);
            panelIcoSizes.Name = "panelIcoSizes";
            panelIcoSizes.Padding = new Padding(0, 2, 0, 6);
            panelIcoSizes.TabIndex = 1;
            panelIcoSizes.WrapContents = false;
            // 
            // lblIcoOutputSize
            // 
            lblIcoOutputSize.Anchor = AnchorStyles.Left;
            lblIcoOutputSize.AutoSize = true;
            lblIcoOutputSize.Location = new Point(0, 10);
            lblIcoOutputSize.Margin = new Padding(0, 0, 10, 0);
            lblIcoOutputSize.Name = "lblIcoOutputSize";
            lblIcoOutputSize.Size = new Size(139, 25);
            lblIcoOutputSize.TabIndex = 0;
            lblIcoOutputSize.Text = "ICO output size:";
            lblIcoOutputSize.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbIcoOutputSize
            // 
            cmbIcoOutputSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIcoOutputSize.FormattingEnabled = true;
            cmbIcoOutputSize.Items.AddRange(new object[] { "16 × 16", "32 × 32", "48 × 48", "64 × 64", "128 × 128", "256 × 256" });
            cmbIcoOutputSize.Location = new Point(152, 6);
            cmbIcoOutputSize.Margin = new Padding(3, 4, 3, 4);
            cmbIcoOutputSize.Name = "cmbIcoOutputSize";
            cmbIcoOutputSize.Size = new Size(220, 33);
            cmbIcoOutputSize.TabIndex = 1;
            // 
            // grpBackground
            // 
            grpBackground.AutoSize = true;
            grpBackground.Controls.Add(flowBackground);
            grpBackground.Dock = DockStyle.Fill;
            grpBackground.Location = new Point(3, 324);
            grpBackground.Margin = new Padding(3, 4, 3, 8);
            grpBackground.Name = "grpBackground";
            grpBackground.Padding = new Padding(10, 11, 10, 11);
            grpBackground.Size = new Size(1068, 85);
            grpBackground.TabIndex = 2;
            grpBackground.TabStop = false;
            grpBackground.Text = "ICO canvas (letterbox)";
            // 
            // flowBackground
            // 
            flowBackground.AutoSize = true;
            flowBackground.Controls.Add(lblCanvasFill);
            flowBackground.Controls.Add(cmbSolidColor);
            flowBackground.Dock = DockStyle.Fill;
            flowBackground.Location = new Point(10, 35);
            flowBackground.Margin = new Padding(3, 4, 3, 4);
            flowBackground.Name = "flowBackground";
            flowBackground.Padding = new Padding(0, 2, 0, 0);
            flowBackground.Size = new Size(1048, 39);
            flowBackground.TabIndex = 0;
            flowBackground.WrapContents = false;
            // 
            // lblCanvasFill
            // 
            lblCanvasFill.Anchor = AnchorStyles.Left;
            lblCanvasFill.AutoSize = true;
            lblCanvasFill.Location = new Point(3, 6);
            lblCanvasFill.Margin = new Padding(3, 4, 12, 4);
            lblCanvasFill.Name = "lblCanvasFill";
            lblCanvasFill.Size = new Size(128, 25);
            lblCanvasFill.TabIndex = 0;
            lblCanvasFill.Text = "Letterbox color:";
            lblCanvasFill.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbSolidColor
            // 
            cmbSolidColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSolidColor.FormattingEnabled = true;
            cmbSolidColor.Items.AddRange(new object[] { "White", "Black" });
            cmbSolidColor.Location = new Point(146, 2);
            cmbSolidColor.Margin = new Padding(3, 0, 3, 4);
            cmbSolidColor.Name = "cmbSolidColor";
            cmbSolidColor.Size = new Size(120, 33);
            cmbSolidColor.TabIndex = 1;
            // 
            // grpPreview
            // 
            grpPreview.Controls.Add(panelPreviewToolbar);
            grpPreview.Controls.Add(listViewPreview);
            grpPreview.Controls.Add(lblPreviewPlaceholder);
            grpPreview.Dock = DockStyle.Fill;
            grpPreview.Location = new Point(3, 421);
            grpPreview.Margin = new Padding(3, 4, 3, 8);
            grpPreview.Name = "grpPreview";
            grpPreview.Padding = new Padding(10, 11, 10, 11);
            grpPreview.Size = new Size(1068, 174);
            grpPreview.TabIndex = 3;
            grpPreview.TabStop = false;
            grpPreview.Text = "Review / preview";
            // 
            // panelPreviewToolbar
            // 
            panelPreviewToolbar.AutoSize = true;
            panelPreviewToolbar.Dock = DockStyle.Top;
            panelPreviewToolbar.Location = new Point(10, 35);
            panelPreviewToolbar.Margin = new Padding(3, 4, 3, 8);
            panelPreviewToolbar.Name = "panelPreviewToolbar";
            panelPreviewToolbar.Padding = new Padding(0, 0, 0, 6);
            panelPreviewToolbar.Size = new Size(1048, 46);
            panelPreviewToolbar.TabIndex = 2;
            panelPreviewToolbar.WrapContents = false;
            // 
            // lblPreviewView
            // 
            lblPreviewView.AutoSize = true;
            lblPreviewView.Margin = new Padding(3, 10, 8, 4);
            lblPreviewView.Name = "lblPreviewView";
            lblPreviewView.TabIndex = 10;
            lblPreviewView.Text = "Thumbnail size:";
            // 
            // cmbPreviewSize
            // 
            cmbPreviewSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPreviewSize.Margin = new Padding(3, 4, 16, 4);
            cmbPreviewSize.Name = "cmbPreviewSize";
            cmbPreviewSize.Size = new Size(140, 33);
            cmbPreviewSize.TabIndex = 11;
            panelPreviewToolbar.Controls.Add(lblPreviewView);
            panelPreviewToolbar.Controls.Add(cmbPreviewSize);
            panelPreviewToolbar.Controls.Add(btnRefreshPreview);
            panelPreviewToolbar.Controls.Add(btnOpenDestination);
            // 
            // contextMenuPreview
            // 
            contextMenuPreview.Items.AddRange(new ToolStripItem[] { toolStripMenuItemPreviewCopy, toolStripMenuItemPreviewConvertTo, toolStripMenuItemConvertToQuickIcon, toolStripMenuItemPreviewDelete, toolStripMenuItemOpenSourceLocation, toolStripMenuItemPreviewPaste });
            contextMenuPreview.Name = "contextMenuPreview";
            // 
            // toolStripMenuItemPreviewCopy
            // 
            toolStripMenuItemPreviewCopy.Name = "toolStripMenuItemPreviewCopy";
            toolStripMenuItemPreviewCopy.Size = new Size(224, 32);
            toolStripMenuItemPreviewCopy.Text = "&Copy";
            toolStripMenuItemPreviewCopy.Visible = false;
            // 
            // toolStripMenuItemPreviewConvertTo
            // 
            toolStripMenuItemPreviewConvertTo.Name = "toolStripMenuItemPreviewConvertTo";
            toolStripMenuItemPreviewConvertTo.Size = new Size(224, 32);
            toolStripMenuItemPreviewConvertTo.Text = "&Convert to";
            toolStripMenuItemPreviewConvertTo.Visible = false;
            // 
            // toolStripMenuItemConvertToQuickIcon
            // 
            toolStripMenuItemConvertToQuickIcon.Name = "toolStripMenuItemConvertToQuickIcon";
            toolStripMenuItemConvertToQuickIcon.Size = new Size(224, 32);
            toolStripMenuItemConvertToQuickIcon.Text = "Convert to &Icon";
            toolStripMenuItemConvertToQuickIcon.Visible = false;
            // 
            // toolStripMenuItemPreviewPaste
            // 
            toolStripMenuItemPreviewPaste.Name = "toolStripMenuItemPreviewPaste";
            toolStripMenuItemPreviewPaste.Size = new Size(224, 32);
            toolStripMenuItemPreviewPaste.Text = "&Paste";
            toolStripMenuItemPreviewPaste.Visible = false;
            // 
            // toolStripMenuItemPreviewDelete
            // 
            toolStripMenuItemPreviewDelete.Name = "toolStripMenuItemPreviewDelete";
            toolStripMenuItemPreviewDelete.Size = new Size(224, 32);
            toolStripMenuItemPreviewDelete.Text = "&Delete";
            toolStripMenuItemPreviewDelete.Visible = false;
            // 
            // toolStripMenuItemOpenSourceLocation
            // 
            toolStripMenuItemOpenSourceLocation.Name = "toolStripMenuItemOpenSourceLocation";
            toolStripMenuItemOpenSourceLocation.Size = new Size(224, 32);
            toolStripMenuItemOpenSourceLocation.Text = "Open file &location";
            toolStripMenuItemOpenSourceLocation.Visible = false;
            // 
            // btnRefreshPreview
            // 
            btnRefreshPreview.AutoSize = true;
            btnRefreshPreview.Location = new Point(3, 4);
            btnRefreshPreview.Margin = new Padding(3, 4, 10, 4);
            btnRefreshPreview.Name = "btnRefreshPreview";
            btnRefreshPreview.Size = new Size(142, 34);
            btnRefreshPreview.TabIndex = 0;
            btnRefreshPreview.Text = "&Refresh review";
            btnRefreshPreview.UseVisualStyleBackColor = true;
            // 
            // btnOpenDestination
            // 
            btnOpenDestination.AutoSize = true;
            btnOpenDestination.Location = new Point(161, 4);
            btnOpenDestination.Margin = new Padding(3, 4, 3, 4);
            btnOpenDestination.Name = "btnOpenDestination";
            btnOpenDestination.Size = new Size(180, 34);
            btnOpenDestination.TabIndex = 1;
            btnOpenDestination.Text = "Open &destination…";
            btnOpenDestination.UseVisualStyleBackColor = true;
            // 
            // listViewPreview
            // 
            listViewPreview.AllowDrop = true;
            listViewPreview.ContextMenuStrip = contextMenuPreview;
            listViewPreview.Dock = DockStyle.Fill;
            listViewPreview.MultiSelect = true;
            listViewPreview.LargeImageList = imageListThumbnails;
            listViewPreview.Location = new Point(10, 35);
            listViewPreview.Margin = new Padding(3, 4, 3, 4);
            listViewPreview.Name = "listViewPreview";
            listViewPreview.Size = new Size(1048, 128);
            listViewPreview.SmallImageList = imageListThumbnails;
            listViewPreview.TabIndex = 0;
            listViewPreview.TileSize = new Size(180, 200);
            listViewPreview.UseCompatibleStateImageBehavior = false;
            listViewPreview.View = View.Tile;
            // 
            // imageListThumbnails
            // 
            imageListThumbnails.ColorDepth = ColorDepth.Depth32Bit;
            imageListThumbnails.ImageSize = new Size(128, 128);
            imageListThumbnails.TransparentColor = Color.Transparent;
            // 
            // lblPreviewPlaceholder
            // 
            lblPreviewPlaceholder.AllowDrop = true;
            lblPreviewPlaceholder.BackColor = Color.FromArgb(245, 245, 245);
            lblPreviewPlaceholder.ContextMenuStrip = contextMenuPreview;
            lblPreviewPlaceholder.Dock = DockStyle.Fill;
            lblPreviewPlaceholder.ForeColor = SystemColors.GrayText;
            lblPreviewPlaceholder.Location = new Point(10, 35);
            lblPreviewPlaceholder.Name = "lblPreviewPlaceholder";
            lblPreviewPlaceholder.Size = new Size(1048, 128);
            lblPreviewPlaceholder.TabIndex = 1;
            lblPreviewPlaceholder.Text = "Choose a source folder or drop files / a folder here. Thumbnails appear when images are loaded.";
            lblPreviewPlaceholder.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelActions
            // 
            panelActions.Controls.Add(btnUndo);
            panelActions.Controls.Add(btnCancel);
            panelActions.Dock = DockStyle.Fill;
            panelActions.Location = new Point(0, 611);
            panelActions.Margin = new Padding(0);
            panelActions.Name = "panelActions";
            panelActions.Size = new Size(1080, 80);
            panelActions.TabIndex = 1;
            // 
            // btnUndo
            // 
            btnUndo.AutoSize = true;
            btnUndo.Enabled = false;
            btnUndo.Location = new Point(0, 12);
            btnUndo.Margin = new Padding(3, 4, 12, 4);
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
            btnCancel.Location = new Point(118, 12);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.MinimumSize = new Size(120, 38);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 38);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Ca&ncel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 745);
            Controls.Add(panelMain);
            Controls.Add(statusStripMain);
            Font = new Font("Segoe UI", 9F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(900, 600);
            KeyPreview = true;
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Image Converter";
            statusStripMain.ResumeLayout(false);
            statusStripMain.PerformLayout();
            panelMain.ResumeLayout(false);
            tableLayoutOuter.ResumeLayout(false);
            tableLayoutRoot.ResumeLayout(false);
            tableLayoutRoot.PerformLayout();
            splitReview.ResumeLayout(false);
            tableLayoutSettings.ResumeLayout(false);
            grpFolders.ResumeLayout(false);
            grpFolders.PerformLayout();
            tableLayoutFolders.ResumeLayout(false);
            tableLayoutFolders.PerformLayout();
            grpFormats.ResumeLayout(false);
            grpFormats.PerformLayout();
            tableLayoutFormats.ResumeLayout(false);
            tableLayoutFormats.PerformLayout();
            panelIcoSizes.ResumeLayout(false);
            panelIcoSizes.PerformLayout();
            grpBackground.ResumeLayout(false);
            grpBackground.PerformLayout();
            flowBackground.ResumeLayout(false);
            flowBackground.PerformLayout();
            panelPreviewToolbar.ResumeLayout(false);
            panelPreviewToolbar.PerformLayout();
            grpPreview.ResumeLayout(false);
            panelActions.ResumeLayout(false);
            panelActions.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FolderBrowserDialog folderBrowserDialog;
        private StatusStrip statusStripMain;
        private ToolStripStatusLabel statusLabelMessage;
        private ToolStripStatusLabel statusLabelSelection;
        private ToolStripProgressBar toolStripProgressBatch;
        private ToolStripStatusLabel statusLabelSpring;
        private Panel panelMain;
        private TableLayoutPanel tableLayoutOuter;
        private TableLayoutPanel tableLayoutRoot;
        private SplitContainer splitReview;
        private TableLayoutPanel tableLayoutSettings;
        private GroupBox grpFolders;
        private TableLayoutPanel tableLayoutFolders;
        private Label lblSourceFolder;
        private TextBox txtSourceFolder;
        private Button btnBrowseSource;
        private Label lblDestFolder;
        private TextBox txtDestFolder;
        private Button btnBrowseDest;
        private GroupBox grpFormats;
        private TableLayoutPanel tableLayoutFormats;
        private Label lblIcoHint;
        private FlowLayoutPanel panelIcoSizes;
        private Label lblIcoOutputSize;
        private ComboBox cmbIcoOutputSize;
        private GroupBox grpBackground;
        private FlowLayoutPanel flowBackground;
        private Label lblCanvasFill;
        private ComboBox cmbSolidColor;
        private GroupBox grpPreview;
        private FlowLayoutPanel panelPreviewToolbar;
        private Label lblPreviewView;
        private ComboBox cmbPreviewSize;
        private Button btnRefreshPreview;
        private Button btnOpenDestination;
        private ContextMenuStrip contextMenuPreview;
        private ToolStripMenuItem toolStripMenuItemPreviewCopy;
        private ToolStripMenuItem toolStripMenuItemPreviewConvertTo;
        private ToolStripMenuItem toolStripMenuItemConvertToQuickIcon;
        private ToolStripMenuItem toolStripMenuItemPreviewPaste;
        private ToolStripMenuItem toolStripMenuItemPreviewDelete;
        private ToolStripMenuItem toolStripMenuItemOpenSourceLocation;
        private ListView listViewPreview;
        private ImageList imageListThumbnails;
        private Label lblPreviewPlaceholder;
        private Panel panelActions;
        private Button btnUndo;
        private Button btnCancel;
    }
}
