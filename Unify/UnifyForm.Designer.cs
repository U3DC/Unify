namespace Unify
{
    partial class UnifyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnifyForm));
            this.gbCharacterLocation = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbCameras = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbCameras = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbProjects = new System.Windows.Forms.ComboBox();
            this.btnNewProject = new System.Windows.Forms.Button();
            this.tbProjectName = new System.Windows.Forms.TextBox();
            this.btnDeleteProject = new System.Windows.Forms.Button();
            this.gbProjectLocation = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tbFolderPath = new System.Windows.Forms.TextBox();
            this.btnFolderPath = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.gbCharacterLocation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.gbProjectLocation.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // gbCharacterLocation
            // 
            this.gbCharacterLocation.Controls.Add(this.panel1);
            this.gbCharacterLocation.Enabled = false;
            this.gbCharacterLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbCharacterLocation.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gbCharacterLocation.Location = new System.Drawing.Point(12, 149);
            this.gbCharacterLocation.Name = "gbCharacterLocation";
            this.gbCharacterLocation.Size = new System.Drawing.Size(520, 201);
            this.gbCharacterLocation.TabIndex = 0;
            this.gbCharacterLocation.TabStop = false;
            this.gbCharacterLocation.Text = "Character Locations:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbCameras);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cbCameras);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(508, 175);
            this.panel1.TabIndex = 1;
            // 
            // cbCameras
            // 
            this.cbCameras.FormattingEnabled = true;
            this.cbCameras.Location = new System.Drawing.Point(212, 12);
            this.cbCameras.Name = "cbCameras";
            this.cbCameras.Size = new System.Drawing.Size(200, 21);
            this.cbCameras.TabIndex = 2;
            this.cbCameras.Text = "Cameras";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Player Start Location:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Player Jump Locations:";
            // 
            // lbCameras
            // 
            this.lbCameras.CheckOnClick = true;
            this.lbCameras.FormattingEnabled = true;
            this.lbCameras.Location = new System.Drawing.Point(212, 44);
            this.lbCameras.Name = "lbCameras";
            this.lbCameras.Size = new System.Drawing.Size(200, 124);
            this.lbCameras.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(520, 60);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Unify Projects:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnDeleteProject);
            this.panel2.Controls.Add(this.tbProjectName);
            this.panel2.Controls.Add(this.btnNewProject);
            this.panel2.Controls.Add(this.cbProjects);
            this.panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.panel2.Location = new System.Drawing.Point(6, 19);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(508, 35);
            this.panel2.TabIndex = 1;
            // 
            // cbProjects
            // 
            this.cbProjects.FormattingEnabled = true;
            this.cbProjects.Location = new System.Drawing.Point(6, 6);
            this.cbProjects.Name = "cbProjects";
            this.cbProjects.Size = new System.Drawing.Size(200, 21);
            this.cbProjects.TabIndex = 6;
            this.cbProjects.Text = "Projects";
            this.cbProjects.SelectionChangeCommitted += new System.EventHandler(this.cbProjects_SelectionChangeCommitted);
            // 
            // btnNewProject
            // 
            this.btnNewProject.Location = new System.Drawing.Point(418, 4);
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(37, 23);
            this.btnNewProject.TabIndex = 3;
            this.btnNewProject.Text = "+";
            this.btnNewProject.UseVisualStyleBackColor = true;
            this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
            // 
            // tbProjectName
            // 
            this.tbProjectName.Location = new System.Drawing.Point(212, 6);
            this.tbProjectName.Name = "tbProjectName";
            this.tbProjectName.Size = new System.Drawing.Size(200, 20);
            this.tbProjectName.TabIndex = 7;
            this.tbProjectName.Text = "Project Name";
            // 
            // btnDeleteProject
            // 
            this.btnDeleteProject.Location = new System.Drawing.Point(461, 4);
            this.btnDeleteProject.Name = "btnDeleteProject";
            this.btnDeleteProject.Size = new System.Drawing.Size(37, 23);
            this.btnDeleteProject.TabIndex = 8;
            this.btnDeleteProject.Text = "-";
            this.btnDeleteProject.UseVisualStyleBackColor = true;
            this.btnDeleteProject.Click += new System.EventHandler(this.btnDeleteProject_Click);
            // 
            // gbProjectLocation
            // 
            this.gbProjectLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProjectLocation.Controls.Add(this.panel3);
            this.gbProjectLocation.Enabled = false;
            this.gbProjectLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbProjectLocation.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gbProjectLocation.Location = new System.Drawing.Point(12, 78);
            this.gbProjectLocation.Name = "gbProjectLocation";
            this.gbProjectLocation.Size = new System.Drawing.Size(520, 65);
            this.gbProjectLocation.TabIndex = 43;
            this.gbProjectLocation.TabStop = false;
            this.gbProjectLocation.Text = "Unity Project Location:";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.tbFolderPath);
            this.panel3.Controls.Add(this.btnFolderPath);
            this.panel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.panel3.Location = new System.Drawing.Point(6, 18);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(508, 41);
            this.panel3.TabIndex = 32;
            // 
            // tbFolderPath
            // 
            this.tbFolderPath.Location = new System.Drawing.Point(45, 12);
            this.tbFolderPath.Name = "tbFolderPath";
            this.tbFolderPath.Size = new System.Drawing.Size(367, 20);
            this.tbFolderPath.TabIndex = 21;
            this.tbFolderPath.Text = "FolderPath";
            // 
            // btnFolderPath
            // 
            this.btnFolderPath.Location = new System.Drawing.Point(6, 9);
            this.btnFolderPath.Name = "btnFolderPath";
            this.btnFolderPath.Size = new System.Drawing.Size(33, 23);
            this.btnFolderPath.TabIndex = 23;
            this.btnFolderPath.Text = "...";
            this.btnFolderPath.UseVisualStyleBackColor = true;
            this.btnFolderPath.Click += new System.EventHandler(this.btnFolderPath_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(457, 514);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 36);
            this.btnCancel.TabIndex = 49;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(376, 514);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 36);
            this.btnExport.TabIndex = 48;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(12, 450);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(231, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 47;
            this.pictureBox1.TabStop = false;
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(334, 514);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(36, 36);
            this.btnHelp.TabIndex = 50;
            this.btnHelp.Text = "?";
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // UnifyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 562);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.gbProjectLocation);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbCharacterLocation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(560, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 600);
            this.Name = "UnifyForm";
            this.Text = "Unify";
            this.gbCharacterLocation.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.gbProjectLocation.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCharacterLocation;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckedListBox lbCameras;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbCameras;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tbProjectName;
        private System.Windows.Forms.Button btnNewProject;
        private System.Windows.Forms.ComboBox cbProjects;
        private System.Windows.Forms.Button btnDeleteProject;
        private System.Windows.Forms.GroupBox gbProjectLocation;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox tbFolderPath;
        private System.Windows.Forms.Button btnFolderPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnHelp;
    }
}