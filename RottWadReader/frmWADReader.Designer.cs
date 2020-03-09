namespace RottWadReader
{
    partial class frmWADReader
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
            this.btnWADOpen = new System.Windows.Forms.Button();
            this.lblWADOpen = new System.Windows.Forms.Label();
            this.txtWADPath = new System.Windows.Forms.TextBox();
            this.picWADView = new System.Windows.Forms.PictureBox();
            this.lstWADEntries = new System.Windows.Forms.ListView();
            this.grpWADInformation = new System.Windows.Forms.GroupBox();
            this.btnListMarkers = new System.Windows.Forms.Button();
            this.lblTotalLumps = new System.Windows.Forms.Label();
            this.lblLumpSize = new System.Windows.Forms.Label();
            this.lblLumpOffset = new System.Windows.Forms.Label();
            this.lblLumpName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkAutoScale = new System.Windows.Forms.CheckBox();
            this.chkWarnOnZeroLump = new System.Windows.Forms.CheckBox();
            this.chkDarkness = new System.Windows.Forms.CheckBox();
            this.nudDarkness = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.picWADView)).BeginInit();
            this.grpWADInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDarkness)).BeginInit();
            this.SuspendLayout();
            // 
            // btnWADOpen
            // 
            this.btnWADOpen.Location = new System.Drawing.Point(472, 32);
            this.btnWADOpen.Name = "btnWADOpen";
            this.btnWADOpen.Size = new System.Drawing.Size(47, 23);
            this.btnWADOpen.TabIndex = 0;
            this.btnWADOpen.Text = "...";
            this.btnWADOpen.UseVisualStyleBackColor = true;
            this.btnWADOpen.Click += new System.EventHandler(this.btnWADOpen_Click);
            // 
            // lblWADOpen
            // 
            this.lblWADOpen.AutoSize = true;
            this.lblWADOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWADOpen.Location = new System.Drawing.Point(12, 18);
            this.lblWADOpen.Name = "lblWADOpen";
            this.lblWADOpen.Size = new System.Drawing.Size(150, 13);
            this.lblWADOpen.TabIndex = 1;
            this.lblWADOpen.Text = "Select a ROTT WAD file:";
            // 
            // txtWADPath
            // 
            this.txtWADPath.Enabled = false;
            this.txtWADPath.Location = new System.Drawing.Point(15, 34);
            this.txtWADPath.Name = "txtWADPath";
            this.txtWADPath.Size = new System.Drawing.Size(451, 20);
            this.txtWADPath.TabIndex = 2;
            // 
            // picWADView
            // 
            this.picWADView.Location = new System.Drawing.Point(198, 70);
            this.picWADView.Name = "picWADView";
            this.picWADView.Size = new System.Drawing.Size(321, 218);
            this.picWADView.TabIndex = 3;
            this.picWADView.TabStop = false;
            // 
            // lstWADEntries
            // 
            this.lstWADEntries.Location = new System.Drawing.Point(15, 70);
            this.lstWADEntries.Name = "lstWADEntries";
            this.lstWADEntries.Size = new System.Drawing.Size(177, 296);
            this.lstWADEntries.TabIndex = 4;
            this.lstWADEntries.UseCompatibleStateImageBehavior = false;
            this.lstWADEntries.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstWADEntries_ItemSelectionChanged);
            this.lstWADEntries.SelectedIndexChanged += new System.EventHandler(this.lstWADEntries_SelectedIndexChanged);
            this.lstWADEntries.DoubleClick += new System.EventHandler(this.lstWADEntries_DoubleClick);
            // 
            // grpWADInformation
            // 
            this.grpWADInformation.Controls.Add(this.btnListMarkers);
            this.grpWADInformation.Controls.Add(this.lblTotalLumps);
            this.grpWADInformation.Controls.Add(this.lblLumpSize);
            this.grpWADInformation.Controls.Add(this.lblLumpOffset);
            this.grpWADInformation.Controls.Add(this.lblLumpName);
            this.grpWADInformation.Location = new System.Drawing.Point(15, 372);
            this.grpWADInformation.Name = "grpWADInformation";
            this.grpWADInformation.Size = new System.Drawing.Size(504, 100);
            this.grpWADInformation.TabIndex = 5;
            this.grpWADInformation.TabStop = false;
            this.grpWADInformation.Text = "WAD File Information:";
            // 
            // btnListMarkers
            // 
            this.btnListMarkers.Location = new System.Drawing.Point(347, 19);
            this.btnListMarkers.Name = "btnListMarkers";
            this.btnListMarkers.Size = new System.Drawing.Size(142, 23);
            this.btnListMarkers.TabIndex = 6;
            this.btnListMarkers.Text = "(DEBUG) List Markers";
            this.btnListMarkers.UseVisualStyleBackColor = true;
            this.btnListMarkers.Visible = false;
            this.btnListMarkers.Click += new System.EventHandler(this.btnListMarkers_Click);
            // 
            // lblTotalLumps
            // 
            this.lblTotalLumps.AutoSize = true;
            this.lblTotalLumps.Location = new System.Drawing.Point(344, 69);
            this.lblTotalLumps.Name = "lblTotalLumps";
            this.lblTotalLumps.Size = new System.Drawing.Size(72, 13);
            this.lblTotalLumps.TabIndex = 3;
            this.lblTotalLumps.Text = "lblTotalLumps";
            // 
            // lblLumpSize
            // 
            this.lblLumpSize.AutoSize = true;
            this.lblLumpSize.Location = new System.Drawing.Point(16, 69);
            this.lblLumpSize.Name = "lblLumpSize";
            this.lblLumpSize.Size = new System.Drawing.Size(63, 13);
            this.lblLumpSize.TabIndex = 2;
            this.lblLumpSize.Text = "lblLumpSize";
            // 
            // lblLumpOffset
            // 
            this.lblLumpOffset.AutoSize = true;
            this.lblLumpOffset.Location = new System.Drawing.Point(16, 47);
            this.lblLumpOffset.Name = "lblLumpOffset";
            this.lblLumpOffset.Size = new System.Drawing.Size(71, 13);
            this.lblLumpOffset.TabIndex = 1;
            this.lblLumpOffset.Text = "lblLumpOffset";
            // 
            // lblLumpName
            // 
            this.lblLumpName.AutoSize = true;
            this.lblLumpName.Location = new System.Drawing.Point(16, 25);
            this.lblLumpName.Name = "lblLumpName";
            this.lblLumpName.Size = new System.Drawing.Size(71, 13);
            this.lblLumpName.TabIndex = 0;
            this.lblLumpName.Text = "lblLumpName";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 484);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(343, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "ROTT2D WAD Reader / Pieter De Ridder / www.rott2d.net";
            // 
            // chkAutoScale
            // 
            this.chkAutoScale.AutoSize = true;
            this.chkAutoScale.Location = new System.Drawing.Point(198, 349);
            this.chkAutoScale.Name = "chkAutoScale";
            this.chkAutoScale.Size = new System.Drawing.Size(160, 17);
            this.chkAutoScale.TabIndex = 7;
            this.chkAutoScale.Text = "Auto scale textures in output";
            this.chkAutoScale.UseVisualStyleBackColor = true;
            // 
            // chkWarnOnZeroLump
            // 
            this.chkWarnOnZeroLump.AutoSize = true;
            this.chkWarnOnZeroLump.Location = new System.Drawing.Point(198, 326);
            this.chkWarnOnZeroLump.Name = "chkWarnOnZeroLump";
            this.chkWarnOnZeroLump.Size = new System.Drawing.Size(150, 17);
            this.chkWarnOnZeroLump.TabIndex = 8;
            this.chkWarnOnZeroLump.Text = "Warn if lump is zero in size";
            this.chkWarnOnZeroLump.UseVisualStyleBackColor = true;
            // 
            // chkDarkness
            // 
            this.chkDarkness.AutoSize = true;
            this.chkDarkness.Location = new System.Drawing.Point(198, 303);
            this.chkDarkness.Name = "chkDarkness";
            this.chkDarkness.Size = new System.Drawing.Size(246, 17);
            this.chkDarkness.TabIndex = 9;
            this.chkDarkness.Text = "Use palette darkness (255 = normal, 0 = black)";
            this.chkDarkness.UseVisualStyleBackColor = true;
            this.chkDarkness.CheckedChanged += new System.EventHandler(this.chkDarkness_CheckedChanged);
            // 
            // nudDarkness
            // 
            this.nudDarkness.Location = new System.Drawing.Point(450, 302);
            this.nudDarkness.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDarkness.Name = "nudDarkness";
            this.nudDarkness.Size = new System.Drawing.Size(61, 20);
            this.nudDarkness.TabIndex = 10;
            this.nudDarkness.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // frmWADReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 507);
            this.Controls.Add(this.nudDarkness);
            this.Controls.Add(this.chkDarkness);
            this.Controls.Add(this.chkWarnOnZeroLump);
            this.Controls.Add(this.chkAutoScale);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpWADInformation);
            this.Controls.Add(this.lstWADEntries);
            this.Controls.Add(this.picWADView);
            this.Controls.Add(this.txtWADPath);
            this.Controls.Add(this.lblWADOpen);
            this.Controls.Add(this.btnWADOpen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmWADReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ROTT2D WAD Reader (v3m)";
            this.Load += new System.EventHandler(this.frmWADReader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picWADView)).EndInit();
            this.grpWADInformation.ResumeLayout(false);
            this.grpWADInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDarkness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWADOpen;
        private System.Windows.Forms.Label lblWADOpen;
        private System.Windows.Forms.TextBox txtWADPath;
        private System.Windows.Forms.PictureBox picWADView;
        private System.Windows.Forms.ListView lstWADEntries;
        private System.Windows.Forms.GroupBox grpWADInformation;
        private System.Windows.Forms.Label lblLumpSize;
        private System.Windows.Forms.Label lblLumpOffset;
        private System.Windows.Forms.Label lblLumpName;
        private System.Windows.Forms.Label lblTotalLumps;
        private System.Windows.Forms.Button btnListMarkers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkAutoScale;
        private System.Windows.Forms.CheckBox chkWarnOnZeroLump;
        private System.Windows.Forms.CheckBox chkDarkness;
        private System.Windows.Forms.NumericUpDown nudDarkness;
    }
}

