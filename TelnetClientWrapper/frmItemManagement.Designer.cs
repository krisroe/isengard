namespace IsengardClient
{
    partial class frmItemManagement
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
            this.btnTargetLocations = new System.Windows.Forms.Button();
            this.btnTargetGraph = new System.Windows.Forms.Button();
            this.lblTarget = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPawn = new System.Windows.Forms.Label();
            this.cboPawn = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboTick = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.cboTargetRoom = new System.Windows.Forms.ComboBox();
            this.lblTickRoom = new System.Windows.Forms.Label();
            this.cboTickRoom = new System.Windows.Forms.ComboBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.colItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSource = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTarget = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colSellOrJunk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colInventory = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colEquipment = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlBottom.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // btnTargetLocations
            // 
            this.btnTargetLocations.Location = new System.Drawing.Point(693, 12);
            this.btnTargetLocations.Name = "btnTargetLocations";
            this.btnTargetLocations.Size = new System.Drawing.Size(133, 38);
            this.btnTargetLocations.TabIndex = 18;
            this.btnTargetLocations.Text = "Locations";
            this.btnTargetLocations.UseVisualStyleBackColor = true;
            this.btnTargetLocations.Click += new System.EventHandler(this.btnTargetLocations_Click);
            // 
            // btnTargetGraph
            // 
            this.btnTargetGraph.Location = new System.Drawing.Point(832, 12);
            this.btnTargetGraph.Name = "btnTargetGraph";
            this.btnTargetGraph.Size = new System.Drawing.Size(133, 38);
            this.btnTargetGraph.TabIndex = 17;
            this.btnTargetGraph.Text = "Graph";
            this.btnTargetGraph.UseVisualStyleBackColor = true;
            this.btnTargetGraph.Click += new System.EventHandler(this.btnTargetGraph_Click);
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(40, 22);
            this.lblTarget.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(58, 19);
            this.lblTarget.TabIndex = 15;
            this.lblTarget.Text = "Target:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(861, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(133, 38);
            this.btnOK.TabIndex = 20;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(1000, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(133, 38);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 785);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1145, 61);
            this.pnlBottom.TabIndex = 21;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.panel1);
            this.pnlTop.Controls.Add(this.lblTickRoom);
            this.pnlTop.Controls.Add(this.cboTickRoom);
            this.pnlTop.Controls.Add(this.btnTargetGraph);
            this.pnlTop.Controls.Add(this.lblTarget);
            this.pnlTop.Controls.Add(this.btnTargetLocations);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1145, 139);
            this.pnlTop.TabIndex = 22;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblPawn);
            this.panel1.Controls.Add(this.cboPawn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cboTick);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.cboTargetRoom);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1145, 139);
            this.panel1.TabIndex = 23;
            // 
            // lblPawn
            // 
            this.lblPawn.AutoSize = true;
            this.lblPawn.Location = new System.Drawing.Point(40, 92);
            this.lblPawn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPawn.Name = "lblPawn";
            this.lblPawn.Size = new System.Drawing.Size(54, 19);
            this.lblPawn.TabIndex = 21;
            this.lblPawn.Text = "Pawn:";
            // 
            // cboPawn
            // 
            this.cboPawn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPawn.FormattingEnabled = true;
            this.cboPawn.Location = new System.Drawing.Point(149, 89);
            this.cboPawn.Margin = new System.Windows.Forms.Padding(4);
            this.cboPawn.Name = "cboPawn";
            this.cboPawn.Size = new System.Drawing.Size(528, 27);
            this.cboPawn.TabIndex = 22;
            this.cboPawn.SelectedIndexChanged += new System.EventHandler(this.cboPawn_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 57);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 19);
            this.label1.TabIndex = 19;
            this.label1.Text = "Tick:";
            // 
            // cboTick
            // 
            this.cboTick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTick.FormattingEnabled = true;
            this.cboTick.Location = new System.Drawing.Point(149, 54);
            this.cboTick.Margin = new System.Windows.Forms.Padding(4);
            this.cboTick.Name = "cboTick";
            this.cboTick.Size = new System.Drawing.Size(528, 27);
            this.cboTick.TabIndex = 20;
            this.cboTick.SelectedIndexChanged += new System.EventHandler(this.cboTick_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(832, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 38);
            this.button1.TabIndex = 17;
            this.button1.Text = "Graph";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnTargetLocations_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 19);
            this.label2.TabIndex = 15;
            this.label2.Text = "Target:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(693, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(133, 38);
            this.button2.TabIndex = 18;
            this.button2.Text = "Locations";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnTargetGraph_Click);
            // 
            // cboTargetRoom
            // 
            this.cboTargetRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetRoom.FormattingEnabled = true;
            this.cboTargetRoom.Location = new System.Drawing.Point(149, 19);
            this.cboTargetRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTargetRoom.Name = "cboTargetRoom";
            this.cboTargetRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTargetRoom.TabIndex = 16;
            // 
            // lblTickRoom
            // 
            this.lblTickRoom.AutoSize = true;
            this.lblTickRoom.Location = new System.Drawing.Point(40, 57);
            this.lblTickRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTickRoom.Name = "lblTickRoom";
            this.lblTickRoom.Size = new System.Drawing.Size(43, 19);
            this.lblTickRoom.TabIndex = 19;
            this.lblTickRoom.Text = "Tick:";
            // 
            // cboTickRoom
            // 
            this.cboTickRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTickRoom.FormattingEnabled = true;
            this.cboTickRoom.Location = new System.Drawing.Point(149, 54);
            this.cboTickRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTickRoom.Name = "cboTickRoom";
            this.cboTickRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTickRoom.TabIndex = 20;
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItem,
            this.colLocation,
            this.colSource,
            this.colTarget,
            this.colTick,
            this.colSellOrJunk,
            this.colInventory,
            this.colEquipment});
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.Location = new System.Drawing.Point(0, 139);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.RowHeadersWidth = 51;
            this.dgvItems.RowTemplate.Height = 24;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(1145, 646);
            this.dgvItems.TabIndex = 23;
            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentClick);
            this.dgvItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            // 
            // colItem
            // 
            this.colItem.HeaderText = "Item";
            this.colItem.MinimumWidth = 6;
            this.colItem.Name = "colItem";
            this.colItem.ReadOnly = true;
            this.colItem.Width = 250;
            // 
            // colLocation
            // 
            this.colLocation.HeaderText = "Location";
            this.colLocation.MinimumWidth = 6;
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;
            this.colLocation.Width = 125;
            // 
            // colSource
            // 
            this.colSource.HeaderText = "Source";
            this.colSource.MinimumWidth = 6;
            this.colSource.Name = "colSource";
            this.colSource.Width = 125;
            // 
            // colTarget
            // 
            this.colTarget.HeaderText = "Target";
            this.colTarget.MinimumWidth = 6;
            this.colTarget.Name = "colTarget";
            this.colTarget.Width = 125;
            // 
            // colTick
            // 
            this.colTick.HeaderText = "Tick";
            this.colTick.MinimumWidth = 6;
            this.colTick.Name = "colTick";
            this.colTick.Width = 125;
            // 
            // colSellOrJunk
            // 
            this.colSellOrJunk.HeaderText = "Sell/Junk";
            this.colSellOrJunk.MinimumWidth = 6;
            this.colSellOrJunk.Name = "colSellOrJunk";
            this.colSellOrJunk.Width = 125;
            // 
            // colInventory
            // 
            this.colInventory.HeaderText = "Inventory";
            this.colInventory.MinimumWidth = 6;
            this.colInventory.Name = "colInventory";
            this.colInventory.Width = 125;
            // 
            // colEquipment
            // 
            this.colEquipment.HeaderText = "Equipment";
            this.colEquipment.MinimumWidth = 6;
            this.colEquipment.Name = "colEquipment";
            this.colEquipment.Width = 125;
            // 
            // frmItemManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1145, 846);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "frmItemManagement";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Item Management";
            this.pnlBottom.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnTargetLocations;
        private System.Windows.Forms.Button btnTargetGraph;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSource;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTarget;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTick;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSellOrJunk;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colInventory;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colEquipment;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblPawn;
        private System.Windows.Forms.ComboBox cboPawn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTick;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cboTargetRoom;
        private System.Windows.Forms.Label lblTickRoom;
        private System.Windows.Forms.ComboBox cboTickRoom;
    }
}