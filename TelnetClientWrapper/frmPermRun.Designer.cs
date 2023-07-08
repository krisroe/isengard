﻿namespace IsengardClient
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
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.flpSkills = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMob = new System.Windows.Forms.Label();
            this.cboMob = new System.Windows.Forms.ComboBox();
            this.cboTargetRoom = new System.Windows.Forms.ComboBox();
            this.lblTargetRoom = new System.Windows.Forms.Label();
            this.btnTargetGraph = new System.Windows.Forms.Button();
            this.btnTargetLocations = new System.Windows.Forms.Button();
            this.grpStrategyModifications = new System.Windows.Forms.GroupBox();
            this.pnlStrategyModifications = new System.Windows.Forms.Panel();
            this.chkMagic = new System.Windows.Forms.CheckBox();
            this.ctxToggleStrategyModificationOverride = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiToggleEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.chkMelee = new System.Windows.Forms.CheckBox();
            this.lblCurrentAutoSpellLevelsValue = new System.Windows.Forms.Label();
            this.chkPotions = new System.Windows.Forms.CheckBox();
            this.cboOnKillMonster = new System.Windows.Forms.ComboBox();
            this.lblOnKillMonster = new System.Windows.Forms.Label();
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
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.btnThresholdLocations = new System.Windows.Forms.Button();
            this.btnThresholdGraph = new System.Windows.Forms.Button();
            this.cboThresholdRoom = new System.Windows.Forms.ComboBox();
            this.lblThresholdRoom = new System.Windows.Forms.Label();
            this.grpStrategyModifications.SuspendLayout();
            this.pnlStrategyModifications.SuspendLayout();
            this.ctxToggleStrategyModificationOverride.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(677, 561);
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
            this.btnCancel.Location = new System.Drawing.Point(828, 561);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 43);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // flpSkills
            // 
            this.flpSkills.Location = new System.Drawing.Point(122, 261);
            this.flpSkills.Margin = new System.Windows.Forms.Padding(4);
            this.flpSkills.Name = "flpSkills";
            this.flpSkills.Size = new System.Drawing.Size(846, 31);
            this.flpSkills.TabIndex = 0;
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(14, 373);
            this.lblMob.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(45, 19);
            this.lblMob.TabIndex = 4;
            this.lblMob.Text = "Mob:";
            // 
            // cboMob
            // 
            this.cboMob.FormattingEnabled = true;
            this.cboMob.Location = new System.Drawing.Point(123, 370);
            this.cboMob.Margin = new System.Windows.Forms.Padding(4);
            this.cboMob.Name = "cboMob";
            this.cboMob.Size = new System.Drawing.Size(528, 27);
            this.cboMob.TabIndex = 5;
            // 
            // cboTargetRoom
            // 
            this.cboTargetRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetRoom.FormattingEnabled = true;
            this.cboTargetRoom.Location = new System.Drawing.Point(123, 335);
            this.cboTargetRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTargetRoom.Name = "cboTargetRoom";
            this.cboTargetRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTargetRoom.TabIndex = 7;
            this.cboTargetRoom.SelectedIndexChanged += new System.EventHandler(this.cboRoom_SelectedIndexChanged);
            // 
            // lblTargetRoom
            // 
            this.lblTargetRoom.AutoSize = true;
            this.lblTargetRoom.Location = new System.Drawing.Point(14, 338);
            this.lblTargetRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTargetRoom.Name = "lblTargetRoom";
            this.lblTargetRoom.Size = new System.Drawing.Size(58, 19);
            this.lblTargetRoom.TabIndex = 6;
            this.lblTargetRoom.Text = "Target:";
            // 
            // btnTargetGraph
            // 
            this.btnTargetGraph.Location = new System.Drawing.Point(810, 335);
            this.btnTargetGraph.Name = "btnTargetGraph";
            this.btnTargetGraph.Size = new System.Drawing.Size(133, 27);
            this.btnTargetGraph.TabIndex = 9;
            this.btnTargetGraph.Text = "Graph";
            this.btnTargetGraph.UseVisualStyleBackColor = true;
            this.btnTargetGraph.Click += new System.EventHandler(this.btnTargetGraph_Click);
            // 
            // btnTargetLocations
            // 
            this.btnTargetLocations.Location = new System.Drawing.Point(671, 335);
            this.btnTargetLocations.Name = "btnTargetLocations";
            this.btnTargetLocations.Size = new System.Drawing.Size(133, 27);
            this.btnTargetLocations.TabIndex = 10;
            this.btnTargetLocations.Text = "Locations";
            this.btnTargetLocations.UseVisualStyleBackColor = true;
            this.btnTargetLocations.Click += new System.EventHandler(this.btnTargetLocations_Click);
            // 
            // grpStrategyModifications
            // 
            this.grpStrategyModifications.Controls.Add(this.pnlStrategyModifications);
            this.grpStrategyModifications.Location = new System.Drawing.Point(123, 437);
            this.grpStrategyModifications.Name = "grpStrategyModifications";
            this.grpStrategyModifications.Size = new System.Drawing.Size(528, 129);
            this.grpStrategyModifications.TabIndex = 11;
            this.grpStrategyModifications.TabStop = false;
            this.grpStrategyModifications.Text = "Strategy Modifications";
            // 
            // pnlStrategyModifications
            // 
            this.pnlStrategyModifications.Controls.Add(this.chkMagic);
            this.pnlStrategyModifications.Controls.Add(this.lblAutoSpellLevels);
            this.pnlStrategyModifications.Controls.Add(this.chkMelee);
            this.pnlStrategyModifications.Controls.Add(this.lblCurrentAutoSpellLevelsValue);
            this.pnlStrategyModifications.Controls.Add(this.chkPotions);
            this.pnlStrategyModifications.Controls.Add(this.cboOnKillMonster);
            this.pnlStrategyModifications.Controls.Add(this.lblOnKillMonster);
            this.pnlStrategyModifications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStrategyModifications.Location = new System.Drawing.Point(3, 23);
            this.pnlStrategyModifications.Name = "pnlStrategyModifications";
            this.pnlStrategyModifications.Size = new System.Drawing.Size(522, 103);
            this.pnlStrategyModifications.TabIndex = 151;
            this.pnlStrategyModifications.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlStrategyModifications_MouseUp);
            // 
            // chkMagic
            // 
            this.chkMagic.AutoSize = true;
            this.chkMagic.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.chkMagic.Location = new System.Drawing.Point(3, 3);
            this.chkMagic.Name = "chkMagic";
            this.chkMagic.Size = new System.Drawing.Size(75, 23);
            this.chkMagic.TabIndex = 9;
            this.chkMagic.Text = "Magic";
            this.chkMagic.UseVisualStyleBackColor = true;
            this.chkMagic.CheckedChanged += new System.EventHandler(this.chkMagic_CheckedChanged);
            // 
            // ctxToggleStrategyModificationOverride
            // 
            this.ctxToggleStrategyModificationOverride.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxToggleStrategyModificationOverride.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiToggleEnabled});
            this.ctxToggleStrategyModificationOverride.Name = "ctxCombatType";
            this.ctxToggleStrategyModificationOverride.Size = new System.Drawing.Size(125, 28);
            this.ctxToggleStrategyModificationOverride.Opening += new System.ComponentModel.CancelEventHandler(this.ctxToggleStrategyModificationOverride_Opening);
            // 
            // tsmiToggleEnabled
            // 
            this.tsmiToggleEnabled.Name = "tsmiToggleEnabled";
            this.tsmiToggleEnabled.Size = new System.Drawing.Size(124, 24);
            this.tsmiToggleEnabled.Text = "Toggle";
            this.tsmiToggleEnabled.Click += new System.EventHandler(this.tsmiToggleEnabled_Click);
            // 
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.AutoSize = true;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(7, 66);
            this.lblAutoSpellLevels.Name = "lblAutoSpellLevels";
            this.lblAutoSpellLevels.Size = new System.Drawing.Size(128, 19);
            this.lblAutoSpellLevels.TabIndex = 150;
            this.lblAutoSpellLevels.Text = "Auto spell levels:";
            // 
            // chkMelee
            // 
            this.chkMelee.AutoSize = true;
            this.chkMelee.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.chkMelee.Location = new System.Drawing.Point(109, 3);
            this.chkMelee.Name = "chkMelee";
            this.chkMelee.Size = new System.Drawing.Size(74, 23);
            this.chkMelee.TabIndex = 10;
            this.chkMelee.Text = "Melee";
            this.chkMelee.UseVisualStyleBackColor = true;
            this.chkMelee.CheckedChanged += new System.EventHandler(this.chkMelee_CheckedChanged);
            // 
            // lblCurrentAutoSpellLevelsValue
            // 
            this.lblCurrentAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblCurrentAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentAutoSpellLevelsValue.Location = new System.Drawing.Point(165, 62);
            this.lblCurrentAutoSpellLevelsValue.Name = "lblCurrentAutoSpellLevelsValue";
            this.lblCurrentAutoSpellLevelsValue.Size = new System.Drawing.Size(234, 27);
            this.lblCurrentAutoSpellLevelsValue.TabIndex = 149;
            this.lblCurrentAutoSpellLevelsValue.Text = "Min:Max";
            this.lblCurrentAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkPotions
            // 
            this.chkPotions.AutoSize = true;
            this.chkPotions.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.chkPotions.Location = new System.Drawing.Point(208, 3);
            this.chkPotions.Name = "chkPotions";
            this.chkPotions.Size = new System.Drawing.Size(85, 23);
            this.chkPotions.TabIndex = 11;
            this.chkPotions.Text = "Potions";
            this.chkPotions.UseVisualStyleBackColor = true;
            this.chkPotions.CheckedChanged += new System.EventHandler(this.chkPotions_CheckedChanged);
            // 
            // cboOnKillMonster
            // 
            this.cboOnKillMonster.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.cboOnKillMonster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOnKillMonster.FormattingEnabled = true;
            this.cboOnKillMonster.Location = new System.Drawing.Point(165, 32);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(234, 27);
            this.cboOnKillMonster.TabIndex = 148;
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(7, 37);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(121, 19);
            this.lblOnKillMonster.TabIndex = 147;
            this.lblOnKillMonster.Text = "On kill monster:";
            // 
            // cboPawnShoppe
            // 
            this.cboPawnShoppe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPawnShoppe.FormattingEnabled = true;
            this.cboPawnShoppe.Location = new System.Drawing.Point(129, 80);
            this.cboPawnShoppe.Margin = new System.Windows.Forms.Padding(4);
            this.cboPawnShoppe.Name = "cboPawnShoppe";
            this.cboPawnShoppe.Size = new System.Drawing.Size(528, 27);
            this.cboPawnShoppe.TabIndex = 13;
            this.cboPawnShoppe.SelectedIndexChanged += new System.EventHandler(this.cboPawnShoppe_SelectedIndexChanged);
            // 
            // lblPawnShoppe
            // 
            this.lblPawnShoppe.AutoSize = true;
            this.lblPawnShoppe.Location = new System.Drawing.Point(13, 83);
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
            this.cboTickRoom.Location = new System.Drawing.Point(129, 44);
            this.cboTickRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTickRoom.Name = "cboTickRoom";
            this.cboTickRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTickRoom.TabIndex = 15;
            this.cboTickRoom.SelectedIndexChanged += new System.EventHandler(this.cboTickRoom_SelectedIndexChanged);
            // 
            // lblTickRoom
            // 
            this.lblTickRoom.AutoSize = true;
            this.lblTickRoom.Location = new System.Drawing.Point(13, 47);
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
            this.cboInventoryFlow.Location = new System.Drawing.Point(123, 573);
            this.cboInventoryFlow.Margin = new System.Windows.Forms.Padding(4);
            this.cboInventoryFlow.Name = "cboInventoryFlow";
            this.cboInventoryFlow.Size = new System.Drawing.Size(522, 27);
            this.cboInventoryFlow.TabIndex = 17;
            // 
            // lvlInventoryFlow
            // 
            this.lvlInventoryFlow.AutoSize = true;
            this.lvlInventoryFlow.Location = new System.Drawing.Point(18, 579);
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
            this.chkFullBeforeStarting.Location = new System.Drawing.Point(664, 48);
            this.chkFullBeforeStarting.Name = "chkFullBeforeStarting";
            this.chkFullBeforeStarting.Size = new System.Drawing.Size(175, 23);
            this.chkFullBeforeStarting.TabIndex = 18;
            this.chkFullBeforeStarting.Text = "Full before starting?";
            this.chkFullBeforeStarting.UseVisualStyleBackColor = true;
            // 
            // lblSpellsCast
            // 
            this.lblSpellsCast.AutoSize = true;
            this.lblSpellsCast.Location = new System.Drawing.Point(14, 115);
            this.lblSpellsCast.Name = "lblSpellsCast";
            this.lblSpellsCast.Size = new System.Drawing.Size(104, 19);
            this.lblSpellsCast.TabIndex = 19;
            this.lblSpellsCast.Text = "Spells (cast):";
            // 
            // lblSkills
            // 
            this.lblSkills.AutoSize = true;
            this.lblSkills.Location = new System.Drawing.Point(17, 261);
            this.lblSkills.Name = "lblSkills";
            this.lblSkills.Size = new System.Drawing.Size(51, 19);
            this.lblSkills.TabIndex = 20;
            this.lblSkills.Text = "Skills:";
            // 
            // flpSpellsCast
            // 
            this.flpSpellsCast.Location = new System.Drawing.Point(123, 115);
            this.flpSpellsCast.Margin = new System.Windows.Forms.Padding(4);
            this.flpSpellsCast.Name = "flpSpellsCast";
            this.flpSpellsCast.Size = new System.Drawing.Size(846, 65);
            this.flpSpellsCast.TabIndex = 21;
            // 
            // chkFullAfterFinishing
            // 
            this.chkFullAfterFinishing.AutoSize = true;
            this.chkFullAfterFinishing.Checked = true;
            this.chkFullAfterFinishing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFullAfterFinishing.Location = new System.Drawing.Point(664, 79);
            this.chkFullAfterFinishing.Name = "chkFullAfterFinishing";
            this.chkFullAfterFinishing.Size = new System.Drawing.Size(169, 23);
            this.chkFullAfterFinishing.TabIndex = 22;
            this.chkFullAfterFinishing.Text = "Full after finishing?";
            this.chkFullAfterFinishing.UseVisualStyleBackColor = true;
            // 
            // lblStrategy
            // 
            this.lblStrategy.AutoSize = true;
            this.lblStrategy.Location = new System.Drawing.Point(11, 407);
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
            this.cboStrategy.Location = new System.Drawing.Point(123, 404);
            this.cboStrategy.Name = "cboStrategy";
            this.cboStrategy.Size = new System.Drawing.Size(528, 27);
            this.cboStrategy.TabIndex = 24;
            this.cboStrategy.SelectedIndexChanged += new System.EventHandler(this.cboStrategy_SelectedIndexChanged);
            // 
            // flpSpellsPotions
            // 
            this.flpSpellsPotions.Location = new System.Drawing.Point(123, 188);
            this.flpSpellsPotions.Margin = new System.Windows.Forms.Padding(4);
            this.flpSpellsPotions.Name = "flpSpellsPotions";
            this.flpSpellsPotions.Size = new System.Drawing.Size(846, 65);
            this.flpSpellsPotions.TabIndex = 26;
            // 
            // lblSpellsPotions
            // 
            this.lblSpellsPotions.AutoSize = true;
            this.lblSpellsPotions.Location = new System.Drawing.Point(13, 188);
            this.lblSpellsPotions.Name = "lblSpellsPotions";
            this.lblSpellsPotions.Size = new System.Drawing.Size(96, 19);
            this.lblSpellsPotions.TabIndex = 25;
            this.lblSpellsPotions.Text = "Spells (pot):";
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.Location = new System.Drawing.Point(14, 12);
            this.lblDisplayName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(113, 19);
            this.lblDisplayName.TabIndex = 27;
            this.lblDisplayName.Text = "Display name:";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(129, 9);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(528, 27);
            this.txtDisplayName.TabIndex = 28;
            // 
            // btnThresholdLocations
            // 
            this.btnThresholdLocations.Location = new System.Drawing.Point(670, 300);
            this.btnThresholdLocations.Name = "btnThresholdLocations";
            this.btnThresholdLocations.Size = new System.Drawing.Size(133, 27);
            this.btnThresholdLocations.TabIndex = 32;
            this.btnThresholdLocations.Text = "Locations";
            this.btnThresholdLocations.UseVisualStyleBackColor = true;
            this.btnThresholdLocations.Click += new System.EventHandler(this.btnThresholdLocations_Click);
            // 
            // btnThresholdGraph
            // 
            this.btnThresholdGraph.Location = new System.Drawing.Point(809, 300);
            this.btnThresholdGraph.Name = "btnThresholdGraph";
            this.btnThresholdGraph.Size = new System.Drawing.Size(133, 27);
            this.btnThresholdGraph.TabIndex = 31;
            this.btnThresholdGraph.Text = "Graph";
            this.btnThresholdGraph.UseVisualStyleBackColor = true;
            this.btnThresholdGraph.Click += new System.EventHandler(this.btnThresholdGraph_Click);
            // 
            // cboThresholdRoom
            // 
            this.cboThresholdRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboThresholdRoom.FormattingEnabled = true;
            this.cboThresholdRoom.Location = new System.Drawing.Point(122, 300);
            this.cboThresholdRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboThresholdRoom.Name = "cboThresholdRoom";
            this.cboThresholdRoom.Size = new System.Drawing.Size(528, 27);
            this.cboThresholdRoom.TabIndex = 30;
            // 
            // lblThresholdRoom
            // 
            this.lblThresholdRoom.AutoSize = true;
            this.lblThresholdRoom.Location = new System.Drawing.Point(13, 303);
            this.lblThresholdRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblThresholdRoom.Name = "lblThresholdRoom";
            this.lblThresholdRoom.Size = new System.Drawing.Size(85, 19);
            this.lblThresholdRoom.TabIndex = 29;
            this.lblThresholdRoom.Text = "Threshold:";
            // 
            // frmPermRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 612);
            this.ControlBox = false;
            this.Controls.Add(this.btnThresholdLocations);
            this.Controls.Add(this.btnThresholdGraph);
            this.Controls.Add(this.cboThresholdRoom);
            this.Controls.Add(this.lblThresholdRoom);
            this.Controls.Add(this.flpSpellsPotions);
            this.Controls.Add(this.txtDisplayName);
            this.Controls.Add(this.lblDisplayName);
            this.Controls.Add(this.lblSpellsPotions);
            this.Controls.Add(this.cboStrategy);
            this.Controls.Add(this.lblStrategy);
            this.Controls.Add(this.chkFullAfterFinishing);
            this.Controls.Add(this.flpSpellsCast);
            this.Controls.Add(this.lblSkills);
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
            this.Controls.Add(this.btnTargetLocations);
            this.Controls.Add(this.btnTargetGraph);
            this.Controls.Add(this.cboTargetRoom);
            this.Controls.Add(this.lblTargetRoom);
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
            this.pnlStrategyModifications.ResumeLayout(false);
            this.pnlStrategyModifications.PerformLayout();
            this.ctxToggleStrategyModificationOverride.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flpSkills;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMob;
        private System.Windows.Forms.ComboBox cboMob;
        private System.Windows.Forms.ComboBox cboTargetRoom;
        private System.Windows.Forms.Label lblTargetRoom;
        private System.Windows.Forms.Button btnTargetGraph;
        private System.Windows.Forms.Button btnTargetLocations;
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
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.ContextMenuStrip ctxToggleStrategyModificationOverride;
        private System.Windows.Forms.ToolStripMenuItem tsmiToggleEnabled;
        private System.Windows.Forms.Panel pnlStrategyModifications;
        private System.Windows.Forms.Button btnThresholdLocations;
        private System.Windows.Forms.Button btnThresholdGraph;
        private System.Windows.Forms.ComboBox cboThresholdRoom;
        private System.Windows.Forms.Label lblThresholdRoom;
    }
}