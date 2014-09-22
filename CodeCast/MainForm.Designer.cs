namespace CodeCast
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.timerScreenshot = new System.Windows.Forms.Timer(this.components);
            this.cboTimelapse = new System.Windows.Forms.ComboBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.lstDiffs = new System.Windows.Forms.ListView();
            this.picDiff = new System.Windows.Forms.PictureBox();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDiff)).BeginInit();
            this.SuspendLayout();
            // 
            // picPreview
            // 
            this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picPreview.Location = new System.Drawing.Point(12, 52);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(681, 246);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 0;
            this.picPreview.TabStop = false;
            // 
            // timerScreenshot
            // 
            this.timerScreenshot.Enabled = true;
            this.timerScreenshot.Interval = 1000;
            this.timerScreenshot.Tick += new System.EventHandler(this.timerScreenshot_Tick);
            // 
            // cboTimelapse
            // 
            this.cboTimelapse.FormattingEnabled = true;
            this.cboTimelapse.Items.AddRange(new object[] {
            "50",
            "100",
            "250",
            "500",
            "1000",
            "2000",
            "3000",
            "6000",
            "15000",
            "30000",
            "60000"});
            this.cboTimelapse.Location = new System.Drawing.Point(12, 25);
            this.cboTimelapse.Name = "cboTimelapse";
            this.cboTimelapse.Size = new System.Drawing.Size(121, 21);
            this.cboTimelapse.TabIndex = 1;
            this.cboTimelapse.Text = "1000";
            this.cboTimelapse.SelectedIndexChanged += new System.EventHandler(this.cboTimelapse_SelectedIndexChanged);
            this.cboTimelapse.Leave += new System.EventHandler(this.cboTimelapse_Leave);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(12, 9);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(121, 13);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "Timelapse (Milliseconds)";
            // 
            // lstDiffs
            // 
            this.lstDiffs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lstDiffs.Location = new System.Drawing.Point(12, 304);
            this.lstDiffs.Name = "lstDiffs";
            this.lstDiffs.Size = new System.Drawing.Size(346, 206);
            this.lstDiffs.TabIndex = 3;
            this.lstDiffs.UseCompatibleStateImageBehavior = false;
            // 
            // picDiff
            // 
            this.picDiff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picDiff.Location = new System.Drawing.Point(364, 304);
            this.picDiff.Name = "picDiff";
            this.picDiff.Size = new System.Drawing.Size(329, 206);
            this.picDiff.TabIndex = 4;
            this.picDiff.TabStop = false;
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(139, 27);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(61, 17);
            this.chkEnabled.TabIndex = 5;
            this.chkEnabled.Text = "Record";
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 522);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.picDiff);
            this.Controls.Add(this.lstDiffs);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.cboTimelapse);
            this.Controls.Add(this.picPreview);
            this.Name = "MainForm";
            this.Text = "CodeCast";
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDiff)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Timer timerScreenshot;
        private System.Windows.Forms.ComboBox cboTimelapse;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.ListView lstDiffs;
        private System.Windows.Forms.PictureBox picDiff;
        private System.Windows.Forms.CheckBox chkEnabled;
    }
}

