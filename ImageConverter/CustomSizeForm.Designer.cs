namespace ImageConverter;

partial class CustomSizeForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblSize = new Label();
        nudWidth = new NumericUpDown();
        lblSizeSeparator = new Label();
        nudHeight = new NumericUpDown();
        lblOutputFolder = new Label();
        txtOutputFolder = new TextBox();
        btnBrowse = new Button();
        btnOK = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudHeight).BeginInit();
        SuspendLayout();
        //
        // lblSize
        //
        lblSize.AutoSize = true;
        lblSize.Location = new Point(20, 27);
        lblSize.Name = "lblSize";
        lblSize.Size = new Size(132, 25);
        lblSize.TabIndex = 0;
        lblSize.Text = "Width × height:";
        //
        // nudWidth
        //
        nudWidth.Location = new Point(160, 24);
        nudWidth.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        nudWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudWidth.Name = "nudWidth";
        nudWidth.Size = new Size(110, 31);
        nudWidth.TabIndex = 1;
        nudWidth.Value = new decimal(new int[] { 32, 0, 0, 0 });
        //
        // lblSizeSeparator
        //
        lblSizeSeparator.AutoSize = true;
        lblSizeSeparator.Location = new Point(280, 27);
        lblSizeSeparator.Name = "lblSizeSeparator";
        lblSizeSeparator.Size = new Size(22, 25);
        lblSizeSeparator.TabIndex = 2;
        lblSizeSeparator.Text = "×";
        //
        // nudHeight
        //
        nudHeight.Location = new Point(312, 24);
        nudHeight.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        nudHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudHeight.Name = "nudHeight";
        nudHeight.Size = new Size(110, 31);
        nudHeight.TabIndex = 3;
        nudHeight.Value = new decimal(new int[] { 32, 0, 0, 0 });
        //
        // lblOutputFolder
        //
        lblOutputFolder.AutoSize = true;
        lblOutputFolder.Location = new Point(20, 78);
        lblOutputFolder.Name = "lblOutputFolder";
        lblOutputFolder.Size = new Size(126, 25);
        lblOutputFolder.TabIndex = 4;
        lblOutputFolder.Text = "Output folder:";
        //
        // txtOutputFolder
        //
        txtOutputFolder.Location = new Point(160, 75);
        txtOutputFolder.Name = "txtOutputFolder";
        txtOutputFolder.Size = new Size(352, 31);
        txtOutputFolder.TabIndex = 5;
        //
        // btnBrowse
        //
        btnBrowse.Location = new Point(520, 74);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new Size(93, 34);
        btnBrowse.TabIndex = 6;
        btnBrowse.Text = "Browse…";
        btnBrowse.UseVisualStyleBackColor = true;
        btnBrowse.Click += btnBrowse_Click;
        //
        // btnOK
        //
        btnOK.Location = new Point(407, 133);
        btnOK.Name = "btnOK";
        btnOK.Size = new Size(100, 36);
        btnOK.TabIndex = 7;
        btnOK.Text = "OK";
        btnOK.UseVisualStyleBackColor = true;
        btnOK.Click += btnOK_Click;
        //
        // btnCancel
        //
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(513, 133);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 8;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        //
        // CustomSizeForm
        //
        AcceptButton = btnOK;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(633, 187);
        Controls.Add(btnCancel);
        Controls.Add(btnOK);
        Controls.Add(btnBrowse);
        Controls.Add(txtOutputFolder);
        Controls.Add(lblOutputFolder);
        Controls.Add(nudHeight);
        Controls.Add(lblSizeSeparator);
        Controls.Add(nudWidth);
        Controls.Add(lblSize);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CustomSizeForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Set Custom Size";
        ((System.ComponentModel.ISupportInitialize)nudWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudHeight).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblSize;
    private NumericUpDown nudWidth;
    private Label lblSizeSeparator;
    private NumericUpDown nudHeight;
    private Label lblOutputFolder;
    private TextBox txtOutputFolder;
    private Button btnBrowse;
    private Button btnOK;
    private Button btnCancel;
}
