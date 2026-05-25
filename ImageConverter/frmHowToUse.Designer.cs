namespace ImageConverter;

partial class frmHowToUse
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
        richTextHelp = new RichTextBox();
        btnClose = new Button();
        SuspendLayout();
        // 
        // richTextHelp
        // 
        richTextHelp.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        richTextHelp.BackColor = SystemColors.Window;
        richTextHelp.BorderStyle = BorderStyle.FixedSingle;
        richTextHelp.Font = new Font("Segoe UI", 10F);
        richTextHelp.Location = new Point(16, 16);
        richTextHelp.Name = "richTextHelp";
        richTextHelp.ReadOnly = true;
        richTextHelp.Size = new Size(891, 565);
        richTextHelp.TabIndex = 0;
        // 
        // btnClose
        // 
        btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnClose.DialogResult = DialogResult.OK;
        btnClose.Location = new Point(807, 593);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(100, 36);
        btnClose.TabIndex = 1;
        btnClose.Text = "Close";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += btnClose_Click;
        // 
        // frmHowToUse
        // 
        AcceptButton = btnClose;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnClose;
        ClientSize = new Size(923, 641);
        Controls.Add(btnClose);
        Controls.Add(richTextHelp);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(500, 420);
        Name = "frmHowToUse";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "How to use Image Converter";
        ResumeLayout(false);
    }

    private RichTextBox richTextHelp;
    private Button btnClose;
}
