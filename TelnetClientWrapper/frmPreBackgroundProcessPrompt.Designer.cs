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
            this.grpStrategy = new System.Windows.Forms.GroupBox();
            this.cboOnKillMonster = new System.Windows.Forms.ComboBox();
            this.lblOnKillMonster = new System.Windows.Forms.Label();
            this.chkPotions = new System.Windows.Forms.CheckBox();
            this.chkMelee = new System.Windows.Forms.CheckBox();
            this.chkMagic = new System.Windows.Forms.CheckBox();
            this.cboPawnShoppe = new System.Windows.Forms.ComboBox();
            this.lblPawnShoppe = new System.Windows.Forms.Label();
            this.cboTickRoom = new System.Windows.Forms.ComboBox();
            this.lblTickRoom = new System.Windows.Forms.Label();
            this.chkProcessAllItemsInRoom = new System.Windows.Forms.CheckBox();
            this.grpSkills.SuspendLayout();
            this.grpStrategy.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(579, 293);
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
            this.btnCancel.Location = new System.Drawing.Point(687, 293);
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
            this.grpSkills.Location = new System.Drawing.Point(44, 147);
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
            this.btnEditStrategy.Location = new System.Drawing.Point(122, 81);
            this.btnEditStrategy.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditStrategy.Name = "btnEditStrategy";
            this.btnEditStrategy.Size = new System.Drawing.Size(100, 28);
            this.btnEditStrategy.TabIndex = 8;
            this.btnEditStrategy.Text = "Edit";
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
            // grpStrategy
            // 
            this.grpStrategy.Controls.Add(this.cboOnKillMonster);
            this.grpStrategy.Controls.Add(this.lblOnKillMonster);
            this.grpStrategy.Controls.Add(this.chkPotions);
            this.grpStrategy.Controls.Add(this.chkMelee);
            this.grpStrategy.Controls.Add(this.chkMagic);
            this.grpStrategy.Controls.Add(this.btnEditStrategy);
            this.grpStrategy.Location = new System.Drawing.Point(498, 147);
            this.grpStrategy.Name = "grpStrategy";
            this.grpStrategy.Size = new System.Drawing.Size(295, 121);
            this.grpStrategy.TabIndex = 11;
            this.grpStrategy.TabStop = false;
            this.grpStrategy.Text = "Strategy";
            // 
            // cboOnKillMonster
            // 
            this.cboOnKillMonster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOnKillMonster.FormattingEnabled = true;
            this.cboOnKillMonster.Items.AddRange(new object[] {
            "Continue Combat",
            "Stop Combat",
            "Fight First Monster",
            "Fight First Same Monster"});
            this.cboOnKillMonster.Location = new System.Drawing.Point(122, 50);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(167, 24);
            this.cboOnKillMonster.TabIndex = 148;
            this.cboOnKillMonster.SelectedIndexChanged += new System.EventHandler(this.cboOnKillMonster_SelectedIndexChanged);
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(10, 54);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(97, 16);
            this.lblOnKillMonster.TabIndex = 147;
            this.lblOnKillMonster.Text = "On kill monster:";
            // 
            // chkPotions
            // 
            this.chkPotions.AutoSize = true;
            this.chkPotions.Location = new System.Drawing.Point(153, 21);
            this.chkPotions.Name = "chkPotions";
            this.chkPotions.Size = new System.Drawing.Size(71, 20);
            this.chkPotions.TabIndex = 11;
            this.chkPotions.Text = "Potions";
            this.chkPotions.UseVisualStyleBackColor = true;
            this.chkPotions.CheckedChanged += new System.EventHandler(this.chkPotions_CheckedChanged);
            // 
            // chkMelee
            // 
            this.chkMelee.AutoSize = true;
            this.chkMelee.Location = new System.Drawing.Point(83, 21);
            this.chkMelee.Name = "chkMelee";
            this.chkMelee.Size = new System.Drawing.Size(64, 20);
            this.chkMelee.TabIndex = 10;
            this.chkMelee.Text = "Melee";
            this.chkMelee.UseVisualStyleBackColor = true;
            this.chkMelee.CheckedChanged += new System.EventHandler(this.chkMelee_CheckedChanged);
            // 
            // chkMagic
            // 
            this.chkMagic.AutoSize = true;
            this.chkMagic.Location = new System.Drawing.Point(7, 21);
            this.chkMagic.Name = "chkMagic";
            this.chkMagic.Size = new System.Drawing.Size(63, 20);
            this.chkMagic.TabIndex = 9;
            this.chkMagic.Text = "Magic";
            this.chkMagic.UseVisualStyleBackColor = true;
            this.chkMagic.CheckedChanged += new System.EventHandler(this.chkMagic_CheckedChanged);
            // 
            // cboPawnShoppe
            // 
            this.cboPawnShoppe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPawnShoppe.FormattingEnabled = true;
            this.cboPawnShoppe.Location = new System.Drawing.Point(104, 81);
            this.cboPawnShoppe.Margin = new System.Windows.Forms.Padding(4);
            this.cboPawnShoppe.Name = "cboPawnShoppe";
            this.cboPawnShoppe.Size = new System.Drawing.Size(376, 24);
            this.cboPawnShoppe.TabIndex = 13;
            this.cboPawnShoppe.SelectedIndexChanged += new System.EventHandler(this.cboPawnShoppe_SelectedIndexChanged);
            // 
            // lblPawnShoppe
            // 
            this.lblPawnShoppe.AutoSize = true;
            this.lblPawnShoppe.Location = new System.Drawing.Point(16, 84);
            this.lblPawnShoppe.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPawnShoppe.Name = "lblPawnShoppe";
            this.lblPawnShoppe.Size = new System.Drawing.Size(76, 16);
            this.lblPawnShoppe.TabIndex = 12;
            this.lblPawnShoppe.Text = "Pawn shop:";
            // 
            // cboTickRoom
            // 
            this.cboTickRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTickRoom.FormattingEnabled = true;
            this.cboTickRoom.Location = new System.Drawing.Point(105, 113);
            this.cboTickRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTickRoom.Name = "cboTickRoom";
            this.cboTickRoom.Size = new System.Drawing.Size(376, 24);
            this.cboTickRoom.TabIndex = 15;
            this.cboTickRoom.SelectedIndexChanged += new System.EventHandler(this.cboTickRoom_SelectedIndexChanged);
            // 
            // lblTickRoom
            // 
            this.lblTickRoom.AutoSize = true;
            this.lblTickRoom.Location = new System.Drawing.Point(17, 116);
            this.lblTickRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTickRoom.Name = "lblTickRoom";
            this.lblTickRoom.Size = new System.Drawing.Size(70, 16);
            this.lblTickRoom.TabIndex = 14;
            this.lblTickRoom.Text = "Tick room:";
            // 
            // chkProcessAllItemsInRoom
            // 
            this.chkProcessAllItemsInRoom.AutoSize = true;
            this.chkProcessAllItemsInRoom.Location = new System.Drawing.Point(498, 53);
            this.chkProcessAllItemsInRoom.Name = "chkProcessAllItemsInRoom";
            this.chkProcessAllItemsInRoom.Size = new System.Drawing.Size(182, 20);
            this.chkProcessAllItemsInRoom.TabIndex = 1;
            this.chkProcessAllItemsInRoom.Text = "Process all items in room?";
            this.chkProcessAllItemsInRoom.UseVisualStyleBackColor = true;
            // 
            // frmPreBackgroundProcessPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 346);
            this.ControlBox = false;
            this.Controls.Add(this.chkProcessAllItemsInRoom);
            this.Controls.Add(this.cboTickRoom);
            this.Controls.Add(this.lblTickRoom);
            this.Controls.Add(this.cboPawnShoppe);
            this.Controls.Add(this.lblPawnShoppe);
            this.Controls.Add(this.grpStrategy);
            this.Controls.Add(this.btnLocations);
            this.Controls.Add(this.btnGraph);
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
            this.grpStrategy.ResumeLayout(false);
            this.grpStrategy.PerformLayout();
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
        private System.Windows.Forms.GroupBox grpStrategy;
        private System.Windows.Forms.CheckBox chkPotions;
        private System.Windows.Forms.CheckBox chkMelee;
        private System.Windows.Forms.CheckBox chkMagic;
        private System.Windows.Forms.ComboBox cboOnKillMonster;
        private System.Windows.Forms.Label lblOnKillMonster;
        private System.Windows.Forms.ComboBox cboPawnShoppe;
        private System.Windows.Forms.Label lblPawnShoppe;
        private System.Windows.Forms.ComboBox cboTickRoom;
        private System.Windows.Forms.Label lblTickRoom;
        private System.Windows.Forms.CheckBox chkProcessAllItemsInRoom;
    }
}