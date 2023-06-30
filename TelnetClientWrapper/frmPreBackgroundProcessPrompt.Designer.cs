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
            this.cboInventoryFlow = new System.Windows.Forms.ComboBox();
            this.lvlInventoryFlow = new System.Windows.Forms.Label();
            this.grpSkills.SuspendLayout();
            this.grpStrategy.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(434, 283);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(515, 283);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // flp
            // 
            this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp.Location = new System.Drawing.Point(3, 16);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(322, 126);
            this.flp.TabIndex = 0;
            // 
            // grpSkills
            // 
            this.grpSkills.Controls.Add(this.flp);
            this.grpSkills.Location = new System.Drawing.Point(33, 164);
            this.grpSkills.Name = "grpSkills";
            this.grpSkills.Size = new System.Drawing.Size(328, 145);
            this.grpSkills.TabIndex = 3;
            this.grpSkills.TabStop = false;
            this.grpSkills.Text = "Skills";
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(41, 42);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(31, 13);
            this.lblMob.TabIndex = 4;
            this.lblMob.Text = "Mob:";
            // 
            // cboMob
            // 
            this.cboMob.FormattingEnabled = true;
            this.cboMob.Location = new System.Drawing.Point(78, 40);
            this.cboMob.Name = "cboMob";
            this.cboMob.Size = new System.Drawing.Size(283, 21);
            this.cboMob.TabIndex = 5;
            // 
            // cboRoom
            // 
            this.cboRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoom.FormattingEnabled = true;
            this.cboRoom.Location = new System.Drawing.Point(78, 14);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Size = new System.Drawing.Size(283, 21);
            this.cboRoom.TabIndex = 7;
            this.cboRoom.SelectedIndexChanged += new System.EventHandler(this.cboRoom_SelectedIndexChanged);
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new System.Drawing.Point(41, 16);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(38, 13);
            this.lblRoom.TabIndex = 6;
            this.lblRoom.Text = "Room:";
            // 
            // btnEditStrategy
            // 
            this.btnEditStrategy.Location = new System.Drawing.Point(92, 66);
            this.btnEditStrategy.Name = "btnEditStrategy";
            this.btnEditStrategy.Size = new System.Drawing.Size(75, 23);
            this.btnEditStrategy.TabIndex = 8;
            this.btnEditStrategy.Text = "Edit";
            this.btnEditStrategy.UseVisualStyleBackColor = true;
            this.btnEditStrategy.Click += new System.EventHandler(this.btnEditStrategy_Click);
            // 
            // btnGraph
            // 
            this.btnGraph.Location = new System.Drawing.Point(365, 10);
            this.btnGraph.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(71, 26);
            this.btnGraph.TabIndex = 9;
            this.btnGraph.Text = "Graph";
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // btnLocations
            // 
            this.btnLocations.Location = new System.Drawing.Point(441, 10);
            this.btnLocations.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLocations.Name = "btnLocations";
            this.btnLocations.Size = new System.Drawing.Size(71, 26);
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
            this.grpStrategy.Location = new System.Drawing.Point(374, 164);
            this.grpStrategy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpStrategy.Name = "grpStrategy";
            this.grpStrategy.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpStrategy.Size = new System.Drawing.Size(221, 98);
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
            this.cboOnKillMonster.Location = new System.Drawing.Point(92, 41);
            this.cboOnKillMonster.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(126, 21);
            this.cboOnKillMonster.TabIndex = 148;
            this.cboOnKillMonster.SelectedIndexChanged += new System.EventHandler(this.cboOnKillMonster_SelectedIndexChanged);
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(8, 44);
            this.lblOnKillMonster.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(79, 13);
            this.lblOnKillMonster.TabIndex = 147;
            this.lblOnKillMonster.Text = "On kill monster:";
            // 
            // chkPotions
            // 
            this.chkPotions.AutoSize = true;
            this.chkPotions.Location = new System.Drawing.Point(115, 17);
            this.chkPotions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkPotions.Name = "chkPotions";
            this.chkPotions.Size = new System.Drawing.Size(61, 17);
            this.chkPotions.TabIndex = 11;
            this.chkPotions.Text = "Potions";
            this.chkPotions.UseVisualStyleBackColor = true;
            this.chkPotions.CheckedChanged += new System.EventHandler(this.chkPotions_CheckedChanged);
            // 
            // chkMelee
            // 
            this.chkMelee.AutoSize = true;
            this.chkMelee.Location = new System.Drawing.Point(62, 17);
            this.chkMelee.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkMelee.Name = "chkMelee";
            this.chkMelee.Size = new System.Drawing.Size(55, 17);
            this.chkMelee.TabIndex = 10;
            this.chkMelee.Text = "Melee";
            this.chkMelee.UseVisualStyleBackColor = true;
            this.chkMelee.CheckedChanged += new System.EventHandler(this.chkMelee_CheckedChanged);
            // 
            // chkMagic
            // 
            this.chkMagic.AutoSize = true;
            this.chkMagic.Location = new System.Drawing.Point(5, 17);
            this.chkMagic.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkMagic.Name = "chkMagic";
            this.chkMagic.Size = new System.Drawing.Size(55, 17);
            this.chkMagic.TabIndex = 9;
            this.chkMagic.Text = "Magic";
            this.chkMagic.UseVisualStyleBackColor = true;
            this.chkMagic.CheckedChanged += new System.EventHandler(this.chkMagic_CheckedChanged);
            // 
            // cboPawnShoppe
            // 
            this.cboPawnShoppe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPawnShoppe.FormattingEnabled = true;
            this.cboPawnShoppe.Location = new System.Drawing.Point(78, 93);
            this.cboPawnShoppe.Name = "cboPawnShoppe";
            this.cboPawnShoppe.Size = new System.Drawing.Size(283, 21);
            this.cboPawnShoppe.TabIndex = 13;
            this.cboPawnShoppe.SelectedIndexChanged += new System.EventHandler(this.cboPawnShoppe_SelectedIndexChanged);
            // 
            // lblPawnShoppe
            // 
            this.lblPawnShoppe.AutoSize = true;
            this.lblPawnShoppe.Location = new System.Drawing.Point(12, 95);
            this.lblPawnShoppe.Name = "lblPawnShoppe";
            this.lblPawnShoppe.Size = new System.Drawing.Size(63, 13);
            this.lblPawnShoppe.TabIndex = 12;
            this.lblPawnShoppe.Text = "Pawn shop:";
            // 
            // cboTickRoom
            // 
            this.cboTickRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTickRoom.FormattingEnabled = true;
            this.cboTickRoom.Location = new System.Drawing.Point(79, 119);
            this.cboTickRoom.Name = "cboTickRoom";
            this.cboTickRoom.Size = new System.Drawing.Size(283, 21);
            this.cboTickRoom.TabIndex = 15;
            this.cboTickRoom.SelectedIndexChanged += new System.EventHandler(this.cboTickRoom_SelectedIndexChanged);
            // 
            // lblTickRoom
            // 
            this.lblTickRoom.AutoSize = true;
            this.lblTickRoom.Location = new System.Drawing.Point(13, 121);
            this.lblTickRoom.Name = "lblTickRoom";
            this.lblTickRoom.Size = new System.Drawing.Size(57, 13);
            this.lblTickRoom.TabIndex = 14;
            this.lblTickRoom.Text = "Tick room:";
            // 
            // cboInventoryFlow
            // 
            this.cboInventoryFlow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInventoryFlow.FormattingEnabled = true;
            this.cboInventoryFlow.Location = new System.Drawing.Point(79, 66);
            this.cboInventoryFlow.Name = "cboInventoryFlow";
            this.cboInventoryFlow.Size = new System.Drawing.Size(283, 21);
            this.cboInventoryFlow.TabIndex = 17;
            // 
            // lvlInventoryFlow
            // 
            this.lvlInventoryFlow.AutoSize = true;
            this.lvlInventoryFlow.Location = new System.Drawing.Point(26, 69);
            this.lvlInventoryFlow.Name = "lvlInventoryFlow";
            this.lvlInventoryFlow.Size = new System.Drawing.Size(47, 13);
            this.lvlInventoryFlow.TabIndex = 16;
            this.lvlInventoryFlow.Text = "Inv flow:";
            // 
            // frmPreBackgroundProcessPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 328);
            this.ControlBox = false;
            this.Controls.Add(this.cboInventoryFlow);
            this.Controls.Add(this.lvlInventoryFlow);
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
        private System.Windows.Forms.ComboBox cboInventoryFlow;
        private System.Windows.Forms.Label lvlInventoryFlow;
    }
}