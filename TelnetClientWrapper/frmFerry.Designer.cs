namespace IsengardClient
{
    partial class frmFerry
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
            this.btnSourceLocations = new System.Windows.Forms.Button();
            this.btnSourceGraph = new System.Windows.Forms.Button();
            this.cboSourceRoom = new System.Windows.Forms.ComboBox();
            this.lblSourceRoom = new System.Windows.Forms.Label();
            this.btnTargetLocations = new System.Windows.Forms.Button();
            this.btnTargetGraph = new System.Windows.Forms.Button();
            this.cboTargetRoom = new System.Windows.Forms.ComboBox();
            this.lblTarget = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSourceLocations
            // 
            this.btnSourceLocations.Location = new System.Drawing.Point(667, 18);
            this.btnSourceLocations.Name = "btnSourceLocations";
            this.btnSourceLocations.Size = new System.Drawing.Size(133, 38);
            this.btnSourceLocations.TabIndex = 14;
            this.btnSourceLocations.Text = "Locations";
            this.btnSourceLocations.UseVisualStyleBackColor = true;
            this.btnSourceLocations.Click += new System.EventHandler(this.btnSourceLocations_Click);
            // 
            // btnSourceGraph
            // 
            this.btnSourceGraph.Location = new System.Drawing.Point(806, 18);
            this.btnSourceGraph.Name = "btnSourceGraph";
            this.btnSourceGraph.Size = new System.Drawing.Size(133, 38);
            this.btnSourceGraph.TabIndex = 13;
            this.btnSourceGraph.Text = "Graph";
            this.btnSourceGraph.UseVisualStyleBackColor = true;
            this.btnSourceGraph.Click += new System.EventHandler(this.btnSourceGraph_Click);
            // 
            // cboSourceRoom
            // 
            this.cboSourceRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourceRoom.FormattingEnabled = true;
            this.cboSourceRoom.Location = new System.Drawing.Point(123, 25);
            this.cboSourceRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboSourceRoom.Name = "cboSourceRoom";
            this.cboSourceRoom.Size = new System.Drawing.Size(528, 27);
            this.cboSourceRoom.TabIndex = 12;
            // 
            // lblSourceRoom
            // 
            this.lblSourceRoom.AutoSize = true;
            this.lblSourceRoom.Location = new System.Drawing.Point(14, 28);
            this.lblSourceRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSourceRoom.Name = "lblSourceRoom";
            this.lblSourceRoom.Size = new System.Drawing.Size(84, 24);
            this.lblSourceRoom.TabIndex = 11;
            this.lblSourceRoom.Text = "Source:";
            // 
            // btnTargetLocations
            // 
            this.btnTargetLocations.Location = new System.Drawing.Point(667, 62);
            this.btnTargetLocations.Name = "btnTargetLocations";
            this.btnTargetLocations.Size = new System.Drawing.Size(133, 38);
            this.btnTargetLocations.TabIndex = 18;
            this.btnTargetLocations.Text = "Locations";
            this.btnTargetLocations.UseVisualStyleBackColor = true;
            this.btnTargetLocations.Click += new System.EventHandler(this.btnTargetLocations_Click);
            // 
            // btnTargetGraph
            // 
            this.btnTargetGraph.Location = new System.Drawing.Point(806, 62);
            this.btnTargetGraph.Name = "btnTargetGraph";
            this.btnTargetGraph.Size = new System.Drawing.Size(133, 38);
            this.btnTargetGraph.TabIndex = 17;
            this.btnTargetGraph.Text = "Graph";
            this.btnTargetGraph.UseVisualStyleBackColor = true;
            this.btnTargetGraph.Click += new System.EventHandler(this.btnTargetGraph_Click);
            // 
            // cboTargetRoom
            // 
            this.cboTargetRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetRoom.FormattingEnabled = true;
            this.cboTargetRoom.Location = new System.Drawing.Point(123, 69);
            this.cboTargetRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTargetRoom.Name = "cboTargetRoom";
            this.cboTargetRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTargetRoom.TabIndex = 16;
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(14, 72);
            this.lblTarget.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(73, 24);
            this.lblTarget.TabIndex = 15;
            this.lblTarget.Text = "Target:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(667, 131);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(133, 38);
            this.btnOK.TabIndex = 20;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(806, 131);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(133, 38);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmFerry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 181);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTargetLocations);
            this.Controls.Add(this.btnTargetGraph);
            this.Controls.Add(this.cboTargetRoom);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.btnSourceLocations);
            this.Controls.Add(this.btnSourceGraph);
            this.Controls.Add(this.cboSourceRoom);
            this.Controls.Add(this.lblSourceRoom);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFerry";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ferry Items from Source to Target";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSourceLocations;
        private System.Windows.Forms.Button btnSourceGraph;
        private System.Windows.Forms.ComboBox cboSourceRoom;
        private System.Windows.Forms.Label lblSourceRoom;
        private System.Windows.Forms.Button btnTargetLocations;
        private System.Windows.Forms.Button btnTargetGraph;
        private System.Windows.Forms.ComboBox cboTargetRoom;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}