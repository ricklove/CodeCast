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
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.chkPreview = new System.Windows.Forms.CheckBox();
            this.picOutput = new System.Windows.Forms.PictureBox();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChooseAvatars = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // picPreview
            // 
            this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPreview.Location = new System.Drawing.Point(12, 70);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(258, 418);
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
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(139, 27);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(63, 17);
            this.chkEnabled.TabIndex = 5;
            this.chkEnabled.Text = "Capture";
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // chkPreview
            // 
            this.chkPreview.AutoSize = true;
            this.chkPreview.Location = new System.Drawing.Point(206, 27);
            this.chkPreview.Name = "chkPreview";
            this.chkPreview.Size = new System.Drawing.Size(64, 17);
            this.chkPreview.TabIndex = 6;
            this.chkPreview.Text = "Preview";
            this.chkPreview.UseVisualStyleBackColor = true;
            this.chkPreview.CheckedChanged += new System.EventHandler(this.chkPreview_CheckedChanged);
            // 
            // picOutput
            // 
            this.picOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picOutput.Location = new System.Drawing.Point(276, 70);
            this.picOutput.Name = "picOutput";
            this.picOutput.Size = new System.Drawing.Size(417, 418);
            this.picOutput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picOutput.TabIndex = 7;
            this.picOutput.TabStop = false;
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.DefaultExt = "mp4";
            this.dlgSaveFile.FileName = "Video";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(618, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Record";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtComment
            // 
            this.txtComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComment.Location = new System.Drawing.Point(412, 4);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(200, 60);
            this.txtComment.TabIndex = 9;
            this.txtComment.TextChanged += new System.EventHandler(this.txtComment_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(355, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Comment";
            // 
            // btnChooseAvatars
            // 
            this.btnChooseAvatars.Location = new System.Drawing.Point(331, 41);
            this.btnChooseAvatars.Name = "btnChooseAvatars";
            this.btnChooseAvatars.Size = new System.Drawing.Size(75, 23);
            this.btnChooseAvatars.TabIndex = 11;
            this.btnChooseAvatars.Text = "Avatars";
            this.btnChooseAvatars.UseVisualStyleBackColor = true;
            this.btnChooseAvatars.Click += new System.EventHandler(this.btnChooseAvatars_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 500);
            this.Controls.Add(this.btnChooseAvatars);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.picOutput);
            this.Controls.Add(this.chkPreview);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.cboTimelapse);
            this.Controls.Add(this.picPreview);
            this.Name = "MainForm";
            this.Text = "CodeCast";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOutput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Timer timerScreenshot;
        private System.Windows.Forms.ComboBox cboTimelapse;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.CheckBox chkPreview;
        private System.Windows.Forms.PictureBox picOutput;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnChooseAvatars;
    }
}

