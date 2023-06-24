namespace IsengardClient
{
    partial class frmPreBackgroundProcessPrompt
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.grpSkills = new System.Windows.Forms.GroupBox();
            this.lblMob = new System.Windows.Forms.Label();
            this.cboMob = new System.Windows.Forms.ComboBox();
            this.cboRoom = new System.Windows.Forms.ComboBox();
            this.lblRoom = new System.Windows.Forms.Label();
            this.btnEditStrategy = new System.Windows.Forms.Button();
            this.btnGraph = new System.Windows.Forms.Button();
            this.btnLocations = new System.Windows.Forms.Button();
            this.grpSkills.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(265, 275);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(373, 275);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // flp
            // 
            this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp.Location = new System.Drawing.Point(4, 19);
            this.flp.Margin = new System.Windows.Forms.Padding(4);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(429, 155);
            this.flp.TabIndex = 0;
            // 
            // grpSkills
            // 
            this.grpSkills.Controls.Add(this.flp);
            this.grpSkills.Location = new System.Drawing.Point(51, 89);
            this.grpSkills.Margin = new System.Windows.Forms.Padding(4);
            this.grpSkills.Name = "grpSkills";
            this.grpSkills.Padding = new System.Windows.Forms.Padding(4);
            this.grpSkills.Size = new System.Drawing.Size(437, 178);
            this.grpSkills.TabIndex = 3;
            this.grpSkills.TabStop = false;
            this.grpSkills.Text = "Skills";
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(55, 52);
            this.lblMob.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(37, 16);
            this.lblMob.TabIndex = 4;
            this.lblMob.Text = "Mob:";
            // 
            // cboMob
            // 
            this.cboMob.FormattingEnabled = true;
            this.cboMob.Location = new System.Drawing.Point(104, 49);
            this.cboMob.Margin = new System.Windows.Forms.Padding(4);
            this.cboMob.Name = "cboMob";
            this.cboMob.Size = new System.Drawing.Size(376, 24);
            this.cboMob.TabIndex = 5;
            // 
            // cboRoom
            // 
            this.cboRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoom.FormattingEnabled = true;
            this.cboRoom.Location = new System.Drawing.Point(104, 17);
            this.cboRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Size = new System.Drawing.Size(376, 24);
            this.cboRoom.TabIndex = 7;
            this.cboRoom.SelectedIndexChanged += new System.EventHandler(this.cboRoom_SelectedIndexChanged);
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new System.Drawing.Point(55, 20);
            this.lblRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(47, 16);
            this.lblRoom.TabIndex = 6;
            this.lblRoom.Text = "Room:";
            // 
            // btnEditStrategy
            // 
            this.btnEditStrategy.Location = new System.Drawing.Point(51, 275);
            this.btnEditStrategy.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditStrategy.Name = "btnEditStrategy";
            this.btnEditStrategy.Size = new System.Drawing.Size(100, 28);
            this.btnEditStrategy.TabIndex = 8;
            this.btnEditStrategy.Text = "Edit Strategy";
            this.btnEditStrategy.UseVisualStyleBackColor = true;
            this.btnEditStrategy.Click += new System.EventHandler(this.btnEditStrategy_Click);
            // 
            // btnGraph
            // 
            this.btnGraph.Location = new System.Drawing.Point(487, 12);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(95, 32);
            this.btnGraph.TabIndex = 9;
            this.btnGraph.Text = "Graph";
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // btnLocations
            // 
            this.btnLocations.Location = new System.Drawing.Point(588, 12);
            this.btnLocations.Name = "btnLocations";
            this.btnLocations.Size = new System.Drawing.Size(95, 32);
            this.btnLocations.TabIndex = 10;
            this.btnLocations.Text = "Locations";
            this.btnLocations.UseVisualStyleBackColor = true;
            this.btnLocations.Click += new System.EventHandler(this.btnLocations_Click);
            // 
            // frmPreBackgroundProcessPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 319);
            this.ControlBox = false;
            this.Controls.Add(this.btnLocations);
            this.Controls.Add(this.btnGraph);
            this.Controls.Add(this.btnEditStrategy);
            this.Controls.Add(this.cboRoom);
            this.Controls.Add(this.lblRoom);
            this.Controls.Add(this.cboMob);
            this.Controls.Add(this.lblMob);
            this.Controls.Add(this.grpSkills);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPreBackgroundProcessPrompt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Skills";
            this.grpSkills.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpSkills;
        private System.Windows.Forms.Label lblMob;
        private System.Windows.Forms.ComboBox cboMob;
        private System.Windows.Forms.ComboBox cboRoom;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.Button btnEditStrategy;
        private System.Windows.Forms.Button btnGraph;
        private System.Windows.Forms.Button btnLocations;
    }
}