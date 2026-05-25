namespace ImageConverter;

partial class SplashForm
{
    private System.ComponentModel.IContainer components = null!;
    private PictureBox pictureSplash;
    private Label lblProgressFill;
    private Label lblLoading;
    private Label lblDevelopedBy;
    private System.Windows.Forms.Timer timerSplash;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
            }

            pictureSplash.Image?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pictureSplash = new PictureBox();
        lblProgressFill = new Label();
        lblLoading = new Label();
        lblDevelopedBy = new Label();
        timerSplash = new System.Windows.Forms.Timer(components);
        ((System.ComponentModel.ISupportInitialize)pictureSplash).BeginInit();
        SuspendLayout();
        // 
        // pictureSplash
        // 
        pictureSplash.BackColor = SystemColors.Control;
        pictureSplash.Location = new Point(0, 0);
        pictureSplash.Margin = new Padding(0);
        pictureSplash.Name = "pictureSplash";
        pictureSplash.Size = new Size(560, 200);
        pictureSplash.SizeMode = PictureBoxSizeMode.Zoom;
        pictureSplash.TabIndex = 0;
        pictureSplash.TabStop = false;
        // 
        // lblProgressFill
        // 
        lblProgressFill.BackColor = Color.FromArgb(0, 140, 55);
        lblProgressFill.Location = new Point(0, 200);
        lblProgressFill.Margin = new Padding(0);
        lblProgressFill.Name = "lblProgressFill";
        lblProgressFill.Size = new Size(0, 11);
        lblProgressFill.TabIndex = 1;
        // 
        // lblLoading
        // 
        lblLoading.BackColor = SystemColors.Control;
        lblLoading.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblLoading.ForeColor = Color.FromArgb(50, 49, 48);
        lblLoading.Location = new Point(24, 223);
        lblLoading.Margin = new Padding(0);
        lblLoading.Name = "lblLoading";
        lblLoading.Size = new Size(512, 26);
        lblLoading.TabIndex = 2;
        lblLoading.Text = "Loading";
        lblLoading.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblDevelopedBy
        // 
        lblDevelopedBy.BackColor = SystemColors.Control;
        lblDevelopedBy.Font = new Font("Segoe UI", 8.25F);
        lblDevelopedBy.ForeColor = Color.FromArgb(96, 94, 92);
        lblDevelopedBy.Location = new Point(24, 261);
        lblDevelopedBy.Margin = new Padding(0);
        lblDevelopedBy.Name = "lblDevelopedBy";
        lblDevelopedBy.Size = new Size(512, 22);
        lblDevelopedBy.TabIndex = 3;
        lblDevelopedBy.Text = "Developed by Mohammed Jayyab";
        lblDevelopedBy.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // timerSplash
        // 
        timerSplash.Interval = 20;
        // 
        // SplashForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = SystemColors.Control;
        ClientSize = new Size(560, 295);
        Controls.Add(pictureSplash);
        Controls.Add(lblProgressFill);
        Controls.Add(lblLoading);
        Controls.Add(lblDevelopedBy);
        FormBorderStyle = FormBorderStyle.None;
        Margin = new Padding(0);
        Name = "SplashForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Image Converter";
        TopMost = true;
        ((System.ComponentModel.ISupportInitialize)pictureSplash).EndInit();
        ResumeLayout(false);
    }
}
