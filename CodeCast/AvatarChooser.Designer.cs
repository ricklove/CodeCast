namespace CodeCast
{
    partial class AvatarChooser
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
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.lstNormal = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lstExclaim = new System.Windows.Forms.ListView();
            this.lstQuestion = new System.Windows.Forms.ListView();
            this.lstHappy = new System.Windows.Forms.ListView();
            this.lstSad = new System.Windows.Forms.ListView();
            this.dlgOpenAvatar = new System.Windows.Forms.OpenFileDialog();
            this.btnAddNormal = new System.Windows.Forms.Button();
            this.btnAddExclaim = new System.Windows.Forms.Button();
            this.btnAddQuestion = new System.Windows.Forms.Button();
            this.btnAddHappy = new System.Windows.Forms.Button();
            this.btnAddSad = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // picPreview
            // 
            this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picPreview.Location = new System.Drawing.Point(448, 12);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(167, 359);
            this.picPreview.TabIndex = 0;
            this.picPreview.TabStop = false;
            // 
            // lstNormal
            // 
            this.lstNormal.Location = new System.Drawing.Point(72, 12);
            this.lstNormal.Name = "lstNormal";
            this.lstNormal.Size = new System.Drawing.Size(288, 70);
            this.lstNormal.TabIndex = 1;
            this.lstNormal.TileSize = new System.Drawing.Size(64, 64);
            this.lstNormal.UseCompatibleStateImageBehavior = false;
            this.lstNormal.SelectedIndexChanged += new System.EventHandler(this.lst_SelectedIndexChanged);
            this.lstNormal.Enter += new System.EventHandler(this.lstSad_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Normal";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "!";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "?";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = ":)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 316);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = ":(";
            // 
            // lstExclaim
            // 
            this.lstExclaim.Location = new System.Drawing.Point(72, 88);
            this.lstExclaim.Name = "lstExclaim";
            this.lstExclaim.Size = new System.Drawing.Size(288, 70);
            this.lstExclaim.TabIndex = 11;
            this.lstExclaim.TileSize = new System.Drawing.Size(64, 64);
            this.lstExclaim.UseCompatibleStateImageBehavior = false;
            this.lstExclaim.SelectedIndexChanged += new System.EventHandler(this.lst_SelectedIndexChanged);
            this.lstExclaim.Enter += new System.EventHandler(this.lstSad_Enter);
            // 
            // lstQuestion
            // 
            this.lstQuestion.Location = new System.Drawing.Point(72, 164);
            this.lstQuestion.Name = "lstQuestion";
            this.lstQuestion.Size = new System.Drawing.Size(288, 70);
            this.lstQuestion.TabIndex = 12;
            this.lstQuestion.TileSize = new System.Drawing.Size(64, 64);
            this.lstQuestion.UseCompatibleStateImageBehavior = false;
            this.lstQuestion.SelectedIndexChanged += new System.EventHandler(this.lst_SelectedIndexChanged);
            this.lstQuestion.Enter += new System.EventHandler(this.lstSad_Enter);
            // 
            // lstHappy
            // 
            this.lstHappy.Location = new System.Drawing.Point(72, 240);
            this.lstHappy.Name = "lstHappy";
            this.lstHappy.Size = new System.Drawing.Size(288, 70);
            this.lstHappy.TabIndex = 13;
            this.lstHappy.TileSize = new System.Drawing.Size(64, 64);
            this.lstHappy.UseCompatibleStateImageBehavior = false;
            this.lstHappy.SelectedIndexChanged += new System.EventHandler(this.lst_SelectedIndexChanged);
            this.lstHappy.Enter += new System.EventHandler(this.lstSad_Enter);
            // 
            // lstSad
            // 
            this.lstSad.Location = new System.Drawing.Point(72, 316);
            this.lstSad.Name = "lstSad";
            this.lstSad.Size = new System.Drawing.Size(288, 70);
            this.lstSad.TabIndex = 14;
            this.lstSad.TileSize = new System.Drawing.Size(64, 64);
            this.lstSad.UseCompatibleStateImageBehavior = false;
            this.lstSad.SelectedIndexChanged += new System.EventHandler(this.lst_SelectedIndexChanged);
            this.lstSad.Enter += new System.EventHandler(this.lstSad_Enter);
            // 
            // dlgOpenAvatar
            // 
            this.dlgOpenAvatar.DefaultExt = "png";
            this.dlgOpenAvatar.FileName = "Avatar.png";
            // 
            // btnAddNormal
            // 
            this.btnAddNormal.Location = new System.Drawing.Point(367, 12);
            this.btnAddNormal.Name = "btnAddNormal";
            this.btnAddNormal.Size = new System.Drawing.Size(75, 23);
            this.btnAddNormal.TabIndex = 15;
            this.btnAddNormal.Text = "Add";
            this.btnAddNormal.UseVisualStyleBackColor = true;
            this.btnAddNormal.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddExclaim
            // 
            this.btnAddExclaim.Location = new System.Drawing.Point(367, 88);
            this.btnAddExclaim.Name = "btnAddExclaim";
            this.btnAddExclaim.Size = new System.Drawing.Size(75, 23);
            this.btnAddExclaim.TabIndex = 16;
            this.btnAddExclaim.Text = "Add";
            this.btnAddExclaim.UseVisualStyleBackColor = true;
            this.btnAddExclaim.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddQuestion
            // 
            this.btnAddQuestion.Location = new System.Drawing.Point(367, 164);
            this.btnAddQuestion.Name = "btnAddQuestion";
            this.btnAddQuestion.Size = new System.Drawing.Size(75, 23);
            this.btnAddQuestion.TabIndex = 17;
            this.btnAddQuestion.Text = "Add";
            this.btnAddQuestion.UseVisualStyleBackColor = true;
            this.btnAddQuestion.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddHappy
            // 
            this.btnAddHappy.Location = new System.Drawing.Point(367, 240);
            this.btnAddHappy.Name = "btnAddHappy";
            this.btnAddHappy.Size = new System.Drawing.Size(75, 23);
            this.btnAddHappy.TabIndex = 18;
            this.btnAddHappy.Text = "Add";
            this.btnAddHappy.UseVisualStyleBackColor = true;
            this.btnAddHappy.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddSad
            // 
            this.btnAddSad.Location = new System.Drawing.Point(367, 316);
            this.btnAddSad.Name = "btnAddSad";
            this.btnAddSad.Size = new System.Drawing.Size(75, 23);
            this.btnAddSad.TabIndex = 19;
            this.btnAddSad.Text = "Add";
            this.btnAddSad.UseVisualStyleBackColor = true;
            this.btnAddSad.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(540, 377);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 20;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // AvatarChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 412);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAddSad);
            this.Controls.Add(this.btnAddHappy);
            this.Controls.Add(this.btnAddQuestion);
            this.Controls.Add(this.btnAddExclaim);
            this.Controls.Add(this.btnAddNormal);
            this.Controls.Add(this.lstSad);
            this.Controls.Add(this.lstHappy);
            this.Controls.Add(this.lstQuestion);
            this.Controls.Add(this.lstExclaim);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstNormal);
            this.Controls.Add(this.picPreview);
            this.Name = "AvatarChooser";
            this.Text = "AvatarChooser";
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.ListView lstNormal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView lstExclaim;
        private System.Windows.Forms.ListView lstQuestion;
        private System.Windows.Forms.ListView lstHappy;
        private System.Windows.Forms.ListView lstSad;
        private System.Windows.Forms.OpenFileDialog dlgOpenAvatar;
        private System.Windows.Forms.Button btnAddNormal;
        private System.Windows.Forms.Button btnAddExclaim;
        private System.Windows.Forms.Button btnAddQuestion;
        private System.Windows.Forms.Button btnAddHappy;
        private System.Windows.Forms.Button btnAddSad;
        private System.Windows.Forms.Button btnDelete;
    }
}