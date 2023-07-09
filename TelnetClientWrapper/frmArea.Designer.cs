namespace IsengardClient
{
    partial class frmArea
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
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.cboTickRoom = new System.Windows.Forms.ComboBox();
            this.lblTickRoom = new System.Windows.Forms.Label();
            this.cboPawnShoppe = new System.Windows.Forms.ComboBox();
            this.lblPawnShoppe = new System.Windows.Forms.Label();
            this.cboInventorySinkRoom = new System.Windows.Forms.ComboBox();
            this.lblInventorySinkRoom = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnInventorySinkClear = new System.Windows.Forms.Button();
            this.btnInventorySinkLocations = new System.Windows.Forms.Button();
            this.btnInventorySinkGraph = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(127, 12);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(528, 27);
            this.txtDisplayName.TabIndex = 34;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.Location = new System.Drawing.Point(12, 15);
            this.lblDisplayName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(113, 19);
            this.lblDisplayName.TabIndex = 33;
            this.lblDisplayName.Text = "Display name:";
            // 
            // cboTickRoom
            // 
            this.cboTickRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTickRoom.FormattingEnabled = true;
            this.cboTickRoom.Location = new System.Drawing.Point(127, 47);
            this.cboTickRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTickRoom.Name = "cboTickRoom";
            this.cboTickRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTickRoom.TabIndex = 32;
            this.cboTickRoom.SelectedIndexChanged += new System.EventHandler(this.cboTickRoom_SelectedIndexChanged);
            // 
            // lblTickRoom
            // 
            this.lblTickRoom.AutoSize = true;
            this.lblTickRoom.Location = new System.Drawing.Point(11, 50);
            this.lblTickRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTickRoom.Name = "lblTickRoom";
            this.lblTickRoom.Size = new System.Drawing.Size(85, 19);
            this.lblTickRoom.TabIndex = 31;
            this.lblTickRoom.Text = "Tick room:";
            // 
            // cboPawnShoppe
            // 
            this.cboPawnShoppe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPawnShoppe.FormattingEnabled = true;
            this.cboPawnShoppe.Location = new System.Drawing.Point(127, 80);
            this.cboPawnShoppe.Margin = new System.Windows.Forms.Padding(4);
            this.cboPawnShoppe.Name = "cboPawnShoppe";
            this.cboPawnShoppe.Size = new System.Drawing.Size(528, 27);
            this.cboPawnShoppe.TabIndex = 30;
            this.cboPawnShoppe.SelectedIndexChanged += new System.EventHandler(this.cboPawnShoppe_SelectedIndexChanged);
            // 
            // lblPawnShoppe
            // 
            this.lblPawnShoppe.AutoSize = true;
            this.lblPawnShoppe.Location = new System.Drawing.Point(11, 83);
            this.lblPawnShoppe.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPawnShoppe.Name = "lblPawnShoppe";
            this.lblPawnShoppe.Size = new System.Drawing.Size(94, 19);
            this.lblPawnShoppe.TabIndex = 29;
            this.lblPawnShoppe.Text = "Pawn shop:";
            // 
            // cboInventorySinkRoom
            // 
            this.cboInventorySinkRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInventorySinkRoom.FormattingEnabled = true;
            this.cboInventorySinkRoom.Location = new System.Drawing.Point(127, 115);
            this.cboInventorySinkRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboInventorySinkRoom.Name = "cboInventorySinkRoom";
            this.cboInventorySinkRoom.Size = new System.Drawing.Size(528, 27);
            this.cboInventorySinkRoom.TabIndex = 41;
            // 
            // lblInventorySinkRoom
            // 
            this.lblInventorySinkRoom.AutoSize = true;
            this.lblInventorySinkRoom.Location = new System.Drawing.Point(9, 119);
            this.lblInventorySinkRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInventorySinkRoom.Name = "lblInventorySinkRoom";
            this.lblInventorySinkRoom.Size = new System.Drawing.Size(115, 19);
            this.lblInventorySinkRoom.TabIndex = 40;
            this.lblInventorySinkRoom.Text = "Inventory sink:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(515, 162);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 43);
            this.btnCancel.TabIndex = 43;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(364, 162);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(140, 43);
            this.btnOK.TabIndex = 42;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnInventorySinkClear
            // 
            this.btnInventorySinkClear.Location = new System.Drawing.Point(862, 114);
            this.btnInventorySinkClear.Name = "btnInventorySinkClear";
            this.btnInventorySinkClear.Size = new System.Drawing.Size(95, 27);
            this.btnInventorySinkClear.TabIndex = 46;
            this.btnInventorySinkClear.Text = "Clear";
            this.btnInventorySinkClear.UseVisualStyleBackColor = true;
            this.btnInventorySinkClear.Click += new System.EventHandler(this.btnInventorySinkClear_Click);
            // 
            // btnInventorySinkLocations
            // 
            this.btnInventorySinkLocations.Location = new System.Drawing.Point(660, 114);
            this.btnInventorySinkLocations.Name = "btnInventorySinkLocations";
            this.btnInventorySinkLocations.Size = new System.Drawing.Size(95, 27);
            this.btnInventorySinkLocations.TabIndex = 45;
            this.btnInventorySinkLocations.Text = "Locations";
            this.btnInventorySinkLocations.UseVisualStyleBackColor = true;
            this.btnInventorySinkLocations.Click += new System.EventHandler(this.btnInventorySinkLocations_Click);
            // 
            // btnInventorySinkGraph
            // 
            this.btnInventorySinkGraph.Location = new System.Drawing.Point(761, 114);
            this.btnInventorySinkGraph.Name = "btnInventorySinkGraph";
            this.btnInventorySinkGraph.Size = new System.Drawing.Size(95, 27);
            this.btnInventorySinkGraph.TabIndex = 44;
            this.btnInventorySinkGraph.Text = "Graph";
            this.btnInventorySinkGraph.UseVisualStyleBackColor = true;
            this.btnInventorySinkGraph.Click += new System.EventHandler(this.btnInventorySinkGraph_Click);
            // 
            // frmArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 218);
            this.Controls.Add(this.btnInventorySinkClear);
            this.Controls.Add(this.btnInventorySinkLocations);
            this.Controls.Add(this.btnInventorySinkGraph);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboInventorySinkRoom);
            this.Controls.Add(this.lblInventorySinkRoom);
            this.Controls.Add(this.txtDisplayName);
            this.Controls.Add(this.lblDisplayName);
            this.Controls.Add(this.cboTickRoom);
            this.Controls.Add(this.lblTickRoom);
            this.Controls.Add(this.cboPawnShoppe);
            this.Controls.Add(this.lblPawnShoppe);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmArea";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Area";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.ComboBox cboTickRoom;
        private System.Windows.Forms.Label lblTickRoom;
        private System.Windows.Forms.ComboBox cboPawnShoppe;
        private System.Windows.Forms.Label lblPawnShoppe;
        private System.Windows.Forms.ComboBox cboInventorySinkRoom;
        private System.Windows.Forms.Label lblInventorySinkRoom;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnInventorySinkClear;
        private System.Windows.Forms.Button btnInventorySinkLocations;
        private System.Windows.Forms.Button btnInventorySinkGraph;
    }
}