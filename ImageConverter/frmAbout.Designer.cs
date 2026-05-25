namespace ImageConverter
{
    partial class frmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            pictureBox1 = new PictureBox();
            btnOK = new Button();
            txtAbout = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(22, 46);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(153, 157);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnOK
            // 
            btnOK.FlatStyle = FlatStyle.Popup;
            btnOK.Location = new Point(446, 231);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(109, 41);
            btnOK.TabIndex = 1;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // txtAbout
            // 
            txtAbout.BackColor = SystemColors.ControlLight;
            txtAbout.BorderStyle = BorderStyle.FixedSingle;
            txtAbout.Location = new Point(181, 47);
            txtAbout.Multiline = true;
            txtAbout.Name = "txtAbout";
            txtAbout.ReadOnly = true;
            txtAbout.ScrollBars = ScrollBars.Vertical;
            txtAbout.Size = new Size(387, 165);
            txtAbout.TabIndex = 2;
            txtAbout.Text = "Image Converter V1.0\r\n\r\nDeveloped by: Mohammed Abujayyab\r\nhttps://github.com/MohammedJayyab\r\n\r\nEmail:  moh.abujiab@gmail.com";
            // 
            // frmAbout
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 292);
            Controls.Add(txtAbout);
            Controls.Add(btnOK);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAbout";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About Image Converter";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button btnOK;
        private TextBox txtAbout;
    }
}