namespace IsengardClient
{
    partial class frmPermRun
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
            this.flpSkills = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMob = new System.Windows.Forms.Label();
            this.cboMob = new System.Windows.Forms.ComboBox();
            this.cboRoom = new System.Windows.Forms.ComboBox();
            this.lblRoom = new System.Windows.Forms.Label();
            this.btnEditStrategy = new System.Windows.Forms.Button();
            this.btnGraph = new System.Windows.Forms.Button();
            this.btnLocations = new System.Windows.Forms.Button();
            this.grpStrategyModifications = new System.Windows.Forms.GroupBox();
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.lblCurrentAutoSpellLevelsValue = new System.Windows.Forms.Label();
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
            this.chkFullBeforeStarting = new System.Windows.Forms.CheckBox();
            this.lblSpellsCast = new System.Windows.Forms.Label();
            this.lblSkills = new System.Windows.Forms.Label();
            this.flpSpellsCast = new System.Windows.Forms.FlowLayoutPanel();
            this.chkFullAfterFinishing = new System.Windows.Forms.CheckBox();
            this.lblStrategy = new System.Windows.Forms.Label();
            this.cboStrategy = new System.Windows.Forms.ComboBox();
            this.flpSpellsPotions = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSpellsPotions = new System.Windows.Forms.Label();
            this.grpStrategyModifications.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(676, 438);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(140, 43);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(827, 438);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 43);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // flpSkills
            // 
            this.flpSkills.Location = new System.Drawing.Point(122, 162);
            this.flpSkills.Margin = new System.Windows.Forms.Padding(4);
            this.flpSkills.Name = "flpSkills";
            this.flpSkills.Size = new System.Drawing.Size(846, 31);
            this.flpSkills.TabIndex = 0;
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(13, 251);
            this.lblMob.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(45, 19);
            this.lblMob.TabIndex = 4;
            this.lblMob.Text = "Mob:";
            // 
            // cboMob
            // 
            this.cboMob.FormattingEnabled = true;
            this.cboMob.Location = new System.Drawing.Point(122, 248);
            this.cboMob.Margin = new System.Windows.Forms.Padding(4);
            this.cboMob.Name = "cboMob";
            this.cboMob.Size = new System.Drawing.Size(528, 27);
            this.cboMob.TabIndex = 5;
            // 
            // cboRoom
            // 
            this.cboRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoom.FormattingEnabled = true;
            this.cboRoom.Location = new System.Drawing.Point(122, 213);
            this.cboRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Size = new System.Drawing.Size(528, 27);
            this.cboRoom.TabIndex = 7;
            this.cboRoom.SelectedIndexChanged += new System.EventHandler(this.cboRoom_SelectedIndexChanged);
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new System.Drawing.Point(13, 216);
            this.lblRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(56, 19);
            this.lblRoom.TabIndex = 6;
            this.lblRoom.Text = "Room:";
            // 
            // btnEditStrategy
            // 
            this.btnEditStrategy.Location = new System.Drawing.Point(670, 275);
            this.btnEditStrategy.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditStrategy.Name = "btnEditStrategy";
            this.btnEditStrategy.Size = new System.Drawing.Size(133, 38);
            this.btnEditStrategy.TabIndex = 8;
            this.btnEditStrategy.Text = "Edit";
            this.btnEditStrategy.UseVisualStyleBackColor = true;
            this.btnEditStrategy.Click += new System.EventHandler(this.btnEditStrategy_Click);
            // 
            // btnGraph
            // 
            this.btnGraph.Location = new System.Drawing.Point(670, 206);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(133, 38);
            this.btnGraph.TabIndex = 9;
            this.btnGraph.Text = "Graph";
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // btnLocations
            // 
            this.btnLocations.Location = new System.Drawing.Point(813, 206);
            this.btnLocations.Name = "btnLocations";
            this.btnLocations.Size = new System.Drawing.Size(133, 38);
            this.btnLocations.TabIndex = 10;
            this.btnLocations.Text = "Locations";
            this.btnLocations.UseVisualStyleBackColor = true;
            this.btnLocations.Click += new System.EventHandler(this.btnLocations_Click);
            // 
            // grpStrategyModifications
            // 
            this.grpStrategyModifications.Controls.Add(this.lblAutoSpellLevels);
            this.grpStrategyModifications.Controls.Add(this.lblCurrentAutoSpellLevelsValue);
            this.grpStrategyModifications.Controls.Add(this.cboOnKillMonster);
            this.grpStrategyModifications.Controls.Add(this.lblOnKillMonster);
            this.grpStrategyModifications.Controls.Add(this.chkPotions);
            this.grpStrategyModifications.Controls.Add(this.chkMelee);
            this.grpStrategyModifications.Controls.Add(this.chkMagic);
            this.grpStrategyModifications.Location = new System.Drawing.Point(122, 315);
            this.grpStrategyModifications.Name = "grpStrategyModifications";
            this.grpStrategyModifications.Size = new System.Drawing.Size(528, 129);
            this.grpStrategyModifications.TabIndex = 11;
            this.grpStrategyModifications.TabStop = false;
            this.grpStrategyModifications.Text = "Strategy Modifications";
            // 
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.AutoSize = true;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(14, 94);
            this.lblAutoSpellLevels.Name = "lblAutoSpellLevels";
            this.lblAutoSpellLevels.Size = new System.Drawing.Size(128, 19);
            this.lblAutoSpellLevels.TabIndex = 150;
            this.lblAutoSpellLevels.Text = "Auto spell levels:";
            // 
            // lblCurrentAutoSpellLevelsValue
            // 
            this.lblCurrentAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblCurrentAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentAutoSpellLevelsValue.Location = new System.Drawing.Point(172, 90);
            this.lblCurrentAutoSpellLevelsValue.Name = "lblCurrentAutoSpellLevelsValue";
            this.lblCurrentAutoSpellLevelsValue.Size = new System.Drawing.Size(234, 27);
            this.lblCurrentAutoSpellLevelsValue.TabIndex = 149;
            this.lblCurrentAutoSpellLevelsValue.Text = "Min:Max";
            this.lblCurrentAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.cboOnKillMonster.Location = new System.Drawing.Point(172, 60);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(234, 27);
            this.cboOnKillMonster.TabIndex = 148;
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(14, 65);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(121, 19);
            this.lblOnKillMonster.TabIndex = 147;
            this.lblOnKillMonster.Text = "On kill monster:";
            // 
            // chkPotions
            // 
            this.chkPotions.AutoSize = true;
            this.chkPotions.Location = new System.Drawing.Point(215, 31);
            this.chkPotions.Name = "chkPotions";
            this.chkPotions.Size = new System.Drawing.Size(85, 23);
            this.chkPotions.TabIndex = 11;
            this.chkPotions.Text = "Potions";
            this.chkPotions.UseVisualStyleBackColor = true;
            this.chkPotions.CheckedChanged += new System.EventHandler(this.chkPotions_CheckedChanged);
            // 
            // chkMelee
            // 
            this.chkMelee.AutoSize = true;
            this.chkMelee.Location = new System.Drawing.Point(116, 31);
            this.chkMelee.Name = "chkMelee";
            this.chkMelee.Size = new System.Drawing.Size(74, 23);
            this.chkMelee.TabIndex = 10;
            this.chkMelee.Text = "Melee";
            this.chkMelee.UseVisualStyleBackColor = true;
            this.chkMelee.CheckedChanged += new System.EventHandler(this.chkMelee_CheckedChanged);
            // 
            // chkMagic
            // 
            this.chkMagic.AutoSize = true;
            this.chkMagic.Location = new System.Drawing.Point(10, 31);
            this.chkMagic.Name = "chkMagic";
            this.chkMagic.Size = new System.Drawing.Size(75, 23);
            this.chkMagic.TabIndex = 9;
            this.chkMagic.Text = "Magic";
            this.chkMagic.UseVisualStyleBackColor = true;
            this.chkMagic.CheckedChanged += new System.EventHandler(this.chkMagic_CheckedChanged);
            // 
            // cboPawnShoppe
            // 
            this.cboPawnShoppe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPawnShoppe.FormattingEnabled = true;
            this.cboPawnShoppe.Location = new System.Drawing.Point(128, 49);
            this.cboPawnShoppe.Margin = new System.Windows.Forms.Padding(4);
            this.cboPawnShoppe.Name = "cboPawnShoppe";
            this.cboPawnShoppe.Size = new System.Drawing.Size(528, 27);
            this.cboPawnShoppe.TabIndex = 13;
            this.cboPawnShoppe.SelectedIndexChanged += new System.EventHandler(this.cboPawnShoppe_SelectedIndexChanged);
            // 
            // lblPawnShoppe
            // 
            this.lblPawnShoppe.AutoSize = true;
            this.lblPawnShoppe.Location = new System.Drawing.Point(12, 52);
            this.lblPawnShoppe.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPawnShoppe.Name = "lblPawnShoppe";
            this.lblPawnShoppe.Size = new System.Drawing.Size(94, 19);
            this.lblPawnShoppe.TabIndex = 12;
            this.lblPawnShoppe.Text = "Pawn shop:";
            // 
            // cboTickRoom
            // 
            this.cboTickRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTickRoom.FormattingEnabled = true;
            this.cboTickRoom.Location = new System.Drawing.Point(128, 13);
            this.cboTickRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTickRoom.Name = "cboTickRoom";
            this.cboTickRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTickRoom.TabIndex = 15;
            this.cboTickRoom.SelectedIndexChanged += new System.EventHandler(this.cboTickRoom_SelectedIndexChanged);
            // 
            // lblTickRoom
            // 
            this.lblTickRoom.AutoSize = true;
            this.lblTickRoom.Location = new System.Drawing.Point(12, 16);
            this.lblTickRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTickRoom.Name = "lblTickRoom";
            this.lblTickRoom.Size = new System.Drawing.Size(85, 19);
            this.lblTickRoom.TabIndex = 14;
            this.lblTickRoom.Text = "Tick room:";
            // 
            // cboInventoryFlow
            // 
            this.cboInventoryFlow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInventoryFlow.FormattingEnabled = true;
            this.cboInventoryFlow.Location = new System.Drawing.Point(122, 451);
            this.cboInventoryFlow.Margin = new System.Windows.Forms.Padding(4);
            this.cboInventoryFlow.Name = "cboInventoryFlow";
            this.cboInventoryFlow.Size = new System.Drawing.Size(522, 27);
            this.cboInventoryFlow.TabIndex = 17;
            // 
            // lvlInventoryFlow
            // 
            this.lvlInventoryFlow.AutoSize = true;
            this.lvlInventoryFlow.Location = new System.Drawing.Point(17, 457);
            this.lvlInventoryFlow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lvlInventoryFlow.Name = "lvlInventoryFlow";
            this.lvlInventoryFlow.Size = new System.Drawing.Size(68, 19);
            this.lvlInventoryFlow.TabIndex = 16;
            this.lvlInventoryFlow.Text = "Inv flow:";
            // 
            // chkFullBeforeStarting
            // 
            this.chkFullBeforeStarting.AutoSize = true;
            this.chkFullBeforeStarting.Checked = true;
            this.chkFullBeforeStarting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFullBeforeStarting.Location = new System.Drawing.Point(663, 17);
            this.chkFullBeforeStarting.Name = "chkFullBeforeStarting";
            this.chkFullBeforeStarting.Size = new System.Drawing.Size(175, 23);
            this.chkFullBeforeStarting.TabIndex = 18;
            this.chkFullBeforeStarting.Text = "Full before starting?";
            this.chkFullBeforeStarting.UseVisualStyleBackColor = true;
            // 
            // lblSpellsCast
            // 
            this.lblSpellsCast.AutoSize = true;
            this.lblSpellsCast.Location = new System.Drawing.Point(13, 84);
            this.lblSpellsCast.Name = "lblSpellsCast";
            this.lblSpellsCast.Size = new System.Drawing.Size(104, 19);
            this.lblSpellsCast.TabIndex = 19;
            this.lblSpellsCast.Text = "Spells (cast):";
            // 
            // lblSkills
            // 
            this.lblSkills.AutoSize = true;
            this.lblSkills.Location = new System.Drawing.Point(13, 162);
            this.lblSkills.Name = "lblSkills";
            this.lblSkills.Size = new System.Drawing.Size(51, 19);
            this.lblSkills.TabIndex = 20;
            this.lblSkills.Text = "Skills:";
            // 
            // flpSpellsCast
            // 
            this.flpSpellsCast.Location = new System.Drawing.Point(122, 84);
            this.flpSpellsCast.Margin = new System.Windows.Forms.Padding(4);
            this.flpSpellsCast.Name = "flpSpellsCast";
            this.flpSpellsCast.Size = new System.Drawing.Size(846, 31);
            this.flpSpellsCast.TabIndex = 21;
            // 
            // chkFullAfterFinishing
            // 
            this.chkFullAfterFinishing.AutoSize = true;
            this.chkFullAfterFinishing.Checked = true;
            this.chkFullAfterFinishing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFullAfterFinishing.Location = new System.Drawing.Point(663, 48);
            this.chkFullAfterFinishing.Name = "chkFullAfterFinishing";
            this.chkFullAfterFinishing.Size = new System.Drawing.Size(169, 23);
            this.chkFullAfterFinishing.TabIndex = 22;
            this.chkFullAfterFinishing.Text = "Full after finishing?";
            this.chkFullAfterFinishing.UseVisualStyleBackColor = true;
            // 
            // lblStrategy
            // 
            this.lblStrategy.AutoSize = true;
            this.lblStrategy.Location = new System.Drawing.Point(12, 285);
            this.lblStrategy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStrategy.Name = "lblStrategy";
            this.lblStrategy.Size = new System.Drawing.Size(75, 19);
            this.lblStrategy.TabIndex = 23;
            this.lblStrategy.Text = "Strategy:";
            // 
            // cboStrategy
            // 
            this.cboStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStrategy.FormattingEnabled = true;
            this.cboStrategy.Location = new System.Drawing.Point(122, 282);
            this.cboStrategy.Name = "cboStrategy";
            this.cboStrategy.Size = new System.Drawing.Size(528, 27);
            this.cboStrategy.TabIndex = 24;
            // 
            // flpSpellsPotions
            // 
            this.flpSpellsPotions.Location = new System.Drawing.Point(122, 123);
            this.flpSpellsPotions.Margin = new System.Windows.Forms.Padding(4);
            this.flpSpellsPotions.Name = "flpSpellsPotions";
            this.flpSpellsPotions.Size = new System.Drawing.Size(846, 31);
            this.flpSpellsPotions.TabIndex = 26;
            // 
            // lblSpellsPotions
            // 
            this.lblSpellsPotions.AutoSize = true;
            this.lblSpellsPotions.Location = new System.Drawing.Point(13, 123);
            this.lblSpellsPotions.Name = "lblSpellsPotions";
            this.lblSpellsPotions.Size = new System.Drawing.Size(96, 19);
            this.lblSpellsPotions.TabIndex = 25;
            this.lblSpellsPotions.Text = "Spells (pot):";
            // 
            // frmPermRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 495);
            this.ControlBox = false;
            this.Controls.Add(this.flpSpellsPotions);
            this.Controls.Add(this.lblSpellsPotions);
            this.Controls.Add(this.cboStrategy);
            this.Controls.Add(this.lblStrategy);
            this.Controls.Add(this.chkFullAfterFinishing);
            this.Controls.Add(this.flpSpellsCast);
            this.Controls.Add(this.lblSkills);
            this.Controls.Add(this.btnEditStrategy);
            this.Controls.Add(this.flpSkills);
            this.Controls.Add(this.lblSpellsCast);
            this.Controls.Add(this.chkFullBeforeStarting);
            this.Controls.Add(this.cboInventoryFlow);
            this.Controls.Add(this.lvlInventoryFlow);
            this.Controls.Add(this.cboTickRoom);
            this.Controls.Add(this.lblTickRoom);
            this.Controls.Add(this.cboPawnShoppe);
            this.Controls.Add(this.lblPawnShoppe);
            this.Controls.Add(this.grpStrategyModifications);
            this.Controls.Add(this.btnLocations);
            this.Controls.Add(this.btnGraph);
            this.Controls.Add(this.cboRoom);
            this.Controls.Add(this.lblRoom);
            this.Controls.Add(this.cboMob);
            this.Controls.Add(this.lblMob);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPermRun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Perm Run";
            this.grpStrategyModifications.ResumeLayout(false);
            this.grpStrategyModifications.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flpSkills;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMob;
        private System.Windows.Forms.ComboBox cboMob;
        private System.Windows.Forms.ComboBox cboRoom;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.Button btnEditStrategy;
        private System.Windows.Forms.Button btnGraph;
        private System.Windows.Forms.Button btnLocations;
        private System.Windows.Forms.GroupBox grpStrategyModifications;
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
        private System.Windows.Forms.CheckBox chkFullBeforeStarting;
        private System.Windows.Forms.Label lblSpellsCast;
        private System.Windows.Forms.Label lblSkills;
        private System.Windows.Forms.FlowLayoutPanel flpSpellsCast;
        private System.Windows.Forms.CheckBox chkFullAfterFinishing;
        private System.Windows.Forms.Label lblStrategy;
        private System.Windows.Forms.ComboBox cboStrategy;
        private System.Windows.Forms.Label lblCurrentAutoSpellLevelsValue;
        private System.Windows.Forms.Label lblAutoSpellLevels;
        private System.Windows.Forms.FlowLayoutPanel flpSpellsPotions;
        private System.Windows.Forms.Label lblSpellsPotions;
    }
}