namespace IsengardClient
{
    partial class frmStrategy
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
            this.grpMagic = new System.Windows.Forms.GroupBox();
            this.txtMagicOnlyWhenStunnedForXMS = new System.Windows.Forms.TextBox();
            this.lblMagicOnlyWhenStunnedForXMS = new System.Windows.Forms.Label();
            this.chkMagicEnabled = new System.Windows.Forms.CheckBox();
            this.txtMagicMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblMagicMendWhenDownXHP = new System.Windows.Forms.Label();
            this.txtMagicVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblMagicVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.ctxAutoSpellLevels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetCurrentMinimumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetCurrentMaximumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoSpellLevels1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiInheritAutoSpellLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.lblMagicFinalAction = new System.Windows.Forms.Label();
            this.cboMagicFinalAction = new System.Windows.Forms.ComboBox();
            this.txtManaPool = new System.Windows.Forms.TextBox();
            this.chkMagicLastStepIndefinite = new System.Windows.Forms.CheckBox();
            this.lblManaPool = new System.Windows.Forms.Label();
            this.lstMagicSteps = new System.Windows.Forms.ListBox();
            this.ctxMagicSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.grpMelee = new System.Windows.Forms.GroupBox();
            this.txtMeleeOnlyWhenStunnedForXMS = new System.Windows.Forms.TextBox();
            this.lblMeleeOnlyWhenStunnedForXMS = new System.Windows.Forms.Label();
            this.chkMeleeEnabled = new System.Windows.Forms.CheckBox();
            this.lblMeleeFinalAction = new System.Windows.Forms.Label();
            this.cboMeleeFinalAction = new System.Windows.Forms.ComboBox();
            this.chkMeleeRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.lstMeleeSteps = new System.Windows.Forms.ListBox();
            this.ctxMeleeSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.grpPotions = new System.Windows.Forms.GroupBox();
            this.txtPotionsOnlyWhenStunnedForXMS = new System.Windows.Forms.TextBox();
            this.lblPotionsOnlyWhenStunnedForXMS = new System.Windows.Forms.Label();
            this.chkPotionsEnabled = new System.Windows.Forms.CheckBox();
            this.txtPotionsMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsMendWhenDownXHP = new System.Windows.Forms.Label();
            this.txtPotionsVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.lblPotionsFinalAction = new System.Windows.Forms.Label();
            this.cboPotionsFinalAction = new System.Windows.Forms.ComboBox();
            this.chkPotionsRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.lstPotionsSteps = new System.Windows.Forms.ListBox();
            this.ctxPotionsSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chkAutogenerateName = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblOnKillMonster = new System.Windows.Forms.Label();
            this.cboOnKillMonster = new System.Windows.Forms.ComboBox();
            this.lblAutoEscapeValue = new System.Windows.Forms.Label();
            this.lblAutoEscape = new System.Windows.Forms.Label();
            this.grpMagic.SuspendLayout();
            this.ctxAutoSpellLevels.SuspendLayout();
            this.grpMelee.SuspendLayout();
            this.grpPotions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMagic
            // 
            this.grpMagic.Controls.Add(this.txtMagicOnlyWhenStunnedForXMS);
            this.grpMagic.Controls.Add(this.lblMagicOnlyWhenStunnedForXMS);
            this.grpMagic.Controls.Add(this.chkMagicEnabled);
            this.grpMagic.Controls.Add(this.txtMagicMendWhenDownXHP);
            this.grpMagic.Controls.Add(this.lblMagicMendWhenDownXHP);
            this.grpMagic.Controls.Add(this.txtMagicVigorWhenDownXHP);
            this.grpMagic.Controls.Add(this.lblMagicVigorWhenDownXHP);
            this.grpMagic.Controls.Add(this.lblAutoSpellLevels);
            this.grpMagic.Controls.Add(this.lblMagicFinalAction);
            this.grpMagic.Controls.Add(this.cboMagicFinalAction);
            this.grpMagic.Controls.Add(this.txtManaPool);
            this.grpMagic.Controls.Add(this.chkMagicLastStepIndefinite);
            this.grpMagic.Controls.Add(this.lblManaPool);
            this.grpMagic.Controls.Add(this.lstMagicSteps);
            this.grpMagic.Location = new System.Drawing.Point(30, 10);
            this.grpMagic.Name = "grpMagic";
            this.grpMagic.Size = new System.Drawing.Size(225, 368);
            this.grpMagic.TabIndex = 0;
            this.grpMagic.TabStop = false;
            this.grpMagic.Text = "Magic";
            // 
            // txtMagicOnlyWhenStunnedForXMS
            // 
            this.txtMagicOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(142, 228);
            this.txtMagicOnlyWhenStunnedForXMS.Name = "txtMagicOnlyWhenStunnedForXMS";
            this.txtMagicOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(62, 20);
            this.txtMagicOnlyWhenStunnedForXMS.TabIndex = 147;
            // 
            // lblMagicOnlyWhenStunnedForXMS
            // 
            this.lblMagicOnlyWhenStunnedForXMS.AutoSize = true;
            this.lblMagicOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(4, 228);
            this.lblMagicOnlyWhenStunnedForXMS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMagicOnlyWhenStunnedForXMS.Name = "lblMagicOnlyWhenStunnedForXMS";
            this.lblMagicOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(142, 13);
            this.lblMagicOnlyWhenStunnedForXMS.TabIndex = 146;
            this.lblMagicOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkMagicEnabled
            // 
            this.chkMagicEnabled.AutoSize = true;
            this.chkMagicEnabled.Location = new System.Drawing.Point(43, 342);
            this.chkMagicEnabled.Name = "chkMagicEnabled";
            this.chkMagicEnabled.Size = new System.Drawing.Size(71, 17);
            this.chkMagicEnabled.TabIndex = 145;
            this.chkMagicEnabled.Text = "Enabled?";
            this.chkMagicEnabled.UseVisualStyleBackColor = true;
            // 
            // txtMagicMendWhenDownXHP
            // 
            this.txtMagicMendWhenDownXHP.Location = new System.Drawing.Point(130, 317);
            this.txtMagicMendWhenDownXHP.Name = "txtMagicMendWhenDownXHP";
            this.txtMagicMendWhenDownXHP.Size = new System.Drawing.Size(74, 20);
            this.txtMagicMendWhenDownXHP.TabIndex = 142;
            // 
            // lblMagicMendWhenDownXHP
            // 
            this.lblMagicMendWhenDownXHP.AutoSize = true;
            this.lblMagicMendWhenDownXHP.Location = new System.Drawing.Point(4, 321);
            this.lblMagicMendWhenDownXHP.Name = "lblMagicMendWhenDownXHP";
            this.lblMagicMendWhenDownXHP.Size = new System.Drawing.Size(123, 13);
            this.lblMagicMendWhenDownXHP.TabIndex = 141;
            this.lblMagicMendWhenDownXHP.Text = "Mend when down X HP:";
            // 
            // txtMagicVigorWhenDownXHP
            // 
            this.txtMagicVigorWhenDownXHP.Location = new System.Drawing.Point(130, 292);
            this.txtMagicVigorWhenDownXHP.Name = "txtMagicVigorWhenDownXHP";
            this.txtMagicVigorWhenDownXHP.Size = new System.Drawing.Size(74, 20);
            this.txtMagicVigorWhenDownXHP.TabIndex = 140;
            // 
            // lblMagicVigorWhenDownXHP
            // 
            this.lblMagicVigorWhenDownXHP.AutoSize = true;
            this.lblMagicVigorWhenDownXHP.Location = new System.Drawing.Point(4, 296);
            this.lblMagicVigorWhenDownXHP.Name = "lblMagicVigorWhenDownXHP";
            this.lblMagicVigorWhenDownXHP.Size = new System.Drawing.Size(120, 13);
            this.lblMagicVigorWhenDownXHP.TabIndex = 139;
            this.lblMagicVigorWhenDownXHP.Text = "Vigor when down X HP:";
            // 
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.BackColor = System.Drawing.Color.Silver;
            this.lblAutoSpellLevels.ContextMenuStrip = this.ctxAutoSpellLevels;
            this.lblAutoSpellLevels.ForeColor = System.Drawing.Color.Black;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(6, 274);
            this.lblAutoSpellLevels.Name = "lblAutoSpellLevels";
            this.lblAutoSpellLevels.Size = new System.Drawing.Size(218, 15);
            this.lblAutoSpellLevels.TabIndex = 138;
            this.lblAutoSpellLevels.Text = "AutoSpell lvls";
            this.lblAutoSpellLevels.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetCurrentMinimumAutoSpellLevel,
            this.tsmiSetCurrentMaximumAutoSpellLevel,
            this.sepAutoSpellLevels1,
            this.tsmiInheritAutoSpellLevels});
            this.ctxAutoSpellLevels.Name = "ctxAutoSpellLevels";
            this.ctxAutoSpellLevels.Size = new System.Drawing.Size(192, 76);
            this.ctxAutoSpellLevels.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoSpellLevels_Opening);
            // 
            // tsmiSetCurrentMinimumAutoSpellLevel
            // 
            this.tsmiSetCurrentMinimumAutoSpellLevel.Name = "tsmiSetCurrentMinimumAutoSpellLevel";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Size = new System.Drawing.Size(191, 22);
            this.tsmiSetCurrentMinimumAutoSpellLevel.Text = "Set Current Minimum";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMinimumAutoSpellLevel_Click);
            // 
            // tsmiSetCurrentMaximumAutoSpellLevel
            // 
            this.tsmiSetCurrentMaximumAutoSpellLevel.Name = "tsmiSetCurrentMaximumAutoSpellLevel";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Size = new System.Drawing.Size(191, 22);
            this.tsmiSetCurrentMaximumAutoSpellLevel.Text = "Set Current Maximum";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMaximumAutoSpellLevel_Click);
            // 
            // sepAutoSpellLevels1
            // 
            this.sepAutoSpellLevels1.Name = "sepAutoSpellLevels1";
            this.sepAutoSpellLevels1.Size = new System.Drawing.Size(188, 6);
            // 
            // tsmiInheritAutoSpellLevels
            // 
            this.tsmiInheritAutoSpellLevels.Name = "tsmiInheritAutoSpellLevels";
            this.tsmiInheritAutoSpellLevels.Size = new System.Drawing.Size(191, 22);
            this.tsmiInheritAutoSpellLevels.Text = "Inherit?";
            this.tsmiInheritAutoSpellLevels.Click += new System.EventHandler(this.tsmiInheritAutoSpellLevels_Click);
            // 
            // lblMagicFinalAction
            // 
            this.lblMagicFinalAction.AutoSize = true;
            this.lblMagicFinalAction.Location = new System.Drawing.Point(4, 206);
            this.lblMagicFinalAction.Name = "lblMagicFinalAction";
            this.lblMagicFinalAction.Size = new System.Drawing.Size(64, 13);
            this.lblMagicFinalAction.TabIndex = 4;
            this.lblMagicFinalAction.Text = "Final action:";
            // 
            // cboMagicFinalAction
            // 
            this.cboMagicFinalAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMagicFinalAction.FormattingEnabled = true;
            this.cboMagicFinalAction.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboMagicFinalAction.Location = new System.Drawing.Point(70, 203);
            this.cboMagicFinalAction.Name = "cboMagicFinalAction";
            this.cboMagicFinalAction.Size = new System.Drawing.Size(134, 21);
            this.cboMagicFinalAction.TabIndex = 3;
            // 
            // txtManaPool
            // 
            this.txtManaPool.Location = new System.Drawing.Point(70, 251);
            this.txtManaPool.Name = "txtManaPool";
            this.txtManaPool.Size = new System.Drawing.Size(134, 20);
            this.txtManaPool.TabIndex = 9;
            // 
            // chkMagicLastStepIndefinite
            // 
            this.chkMagicLastStepIndefinite.AutoSize = true;
            this.chkMagicLastStepIndefinite.Location = new System.Drawing.Point(43, 181);
            this.chkMagicLastStepIndefinite.Name = "chkMagicLastStepIndefinite";
            this.chkMagicLastStepIndefinite.Size = new System.Drawing.Size(161, 17);
            this.chkMagicLastStepIndefinite.TabIndex = 2;
            this.chkMagicLastStepIndefinite.Text = "Repeat last step indefinitely?";
            this.chkMagicLastStepIndefinite.UseVisualStyleBackColor = true;
            // 
            // lblManaPool
            // 
            this.lblManaPool.AutoSize = true;
            this.lblManaPool.Location = new System.Drawing.Point(4, 255);
            this.lblManaPool.Name = "lblManaPool";
            this.lblManaPool.Size = new System.Drawing.Size(60, 13);
            this.lblManaPool.TabIndex = 8;
            this.lblManaPool.Text = "Mana pool:";
            // 
            // lstMagicSteps
            // 
            this.lstMagicSteps.ContextMenuStrip = this.ctxMagicSteps;
            this.lstMagicSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstMagicSteps.FormattingEnabled = true;
            this.lstMagicSteps.Location = new System.Drawing.Point(3, 16);
            this.lstMagicSteps.Name = "lstMagicSteps";
            this.lstMagicSteps.Size = new System.Drawing.Size(219, 160);
            this.lstMagicSteps.TabIndex = 0;
            // 
            // ctxMagicSteps
            // 
            this.ctxMagicSteps.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxMagicSteps.Name = "ctxMagicSteps";
            this.ctxMagicSteps.Size = new System.Drawing.Size(61, 4);
            // 
            // grpMelee
            // 
            this.grpMelee.Controls.Add(this.txtMeleeOnlyWhenStunnedForXMS);
            this.grpMelee.Controls.Add(this.lblMeleeOnlyWhenStunnedForXMS);
            this.grpMelee.Controls.Add(this.chkMeleeEnabled);
            this.grpMelee.Controls.Add(this.lblMeleeFinalAction);
            this.grpMelee.Controls.Add(this.cboMeleeFinalAction);
            this.grpMelee.Controls.Add(this.chkMeleeRepeatLastStepIndefinitely);
            this.grpMelee.Controls.Add(this.lstMeleeSteps);
            this.grpMelee.Location = new System.Drawing.Point(266, 10);
            this.grpMelee.Name = "grpMelee";
            this.grpMelee.Size = new System.Drawing.Size(230, 281);
            this.grpMelee.TabIndex = 1;
            this.grpMelee.TabStop = false;
            this.grpMelee.Text = "Melee";
            // 
            // txtMeleeOnlyWhenStunnedForXMS
            // 
            this.txtMeleeOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(149, 228);
            this.txtMeleeOnlyWhenStunnedForXMS.Name = "txtMeleeOnlyWhenStunnedForXMS";
            this.txtMeleeOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(62, 20);
            this.txtMeleeOnlyWhenStunnedForXMS.TabIndex = 149;
            // 
            // lblMeleeOnlyWhenStunnedForXMS
            // 
            this.lblMeleeOnlyWhenStunnedForXMS.AutoSize = true;
            this.lblMeleeOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(11, 231);
            this.lblMeleeOnlyWhenStunnedForXMS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMeleeOnlyWhenStunnedForXMS.Name = "lblMeleeOnlyWhenStunnedForXMS";
            this.lblMeleeOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(142, 13);
            this.lblMeleeOnlyWhenStunnedForXMS.TabIndex = 148;
            this.lblMeleeOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkMeleeEnabled
            // 
            this.chkMeleeEnabled.AutoSize = true;
            this.chkMeleeEnabled.Location = new System.Drawing.Point(50, 254);
            this.chkMeleeEnabled.Name = "chkMeleeEnabled";
            this.chkMeleeEnabled.Size = new System.Drawing.Size(71, 17);
            this.chkMeleeEnabled.TabIndex = 147;
            this.chkMeleeEnabled.Text = "Enabled?";
            this.chkMeleeEnabled.UseVisualStyleBackColor = true;
            // 
            // lblMeleeFinalAction
            // 
            this.lblMeleeFinalAction.AutoSize = true;
            this.lblMeleeFinalAction.Location = new System.Drawing.Point(11, 206);
            this.lblMeleeFinalAction.Name = "lblMeleeFinalAction";
            this.lblMeleeFinalAction.Size = new System.Drawing.Size(64, 13);
            this.lblMeleeFinalAction.TabIndex = 6;
            this.lblMeleeFinalAction.Text = "Final action:";
            // 
            // cboMeleeFinalAction
            // 
            this.cboMeleeFinalAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMeleeFinalAction.FormattingEnabled = true;
            this.cboMeleeFinalAction.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboMeleeFinalAction.Location = new System.Drawing.Point(77, 203);
            this.cboMeleeFinalAction.Name = "cboMeleeFinalAction";
            this.cboMeleeFinalAction.Size = new System.Drawing.Size(134, 21);
            this.cboMeleeFinalAction.TabIndex = 5;
            // 
            // chkMeleeRepeatLastStepIndefinitely
            // 
            this.chkMeleeRepeatLastStepIndefinitely.AutoSize = true;
            this.chkMeleeRepeatLastStepIndefinitely.Location = new System.Drawing.Point(50, 183);
            this.chkMeleeRepeatLastStepIndefinitely.Name = "chkMeleeRepeatLastStepIndefinitely";
            this.chkMeleeRepeatLastStepIndefinitely.Size = new System.Drawing.Size(161, 17);
            this.chkMeleeRepeatLastStepIndefinitely.TabIndex = 3;
            this.chkMeleeRepeatLastStepIndefinitely.Text = "Repeat last step indefinitely?";
            this.chkMeleeRepeatLastStepIndefinitely.UseVisualStyleBackColor = true;
            // 
            // lstMeleeSteps
            // 
            this.lstMeleeSteps.ContextMenuStrip = this.ctxMeleeSteps;
            this.lstMeleeSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstMeleeSteps.FormattingEnabled = true;
            this.lstMeleeSteps.Location = new System.Drawing.Point(3, 16);
            this.lstMeleeSteps.Name = "lstMeleeSteps";
            this.lstMeleeSteps.Size = new System.Drawing.Size(224, 160);
            this.lstMeleeSteps.TabIndex = 1;
            // 
            // ctxMeleeSteps
            // 
            this.ctxMeleeSteps.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxMeleeSteps.Name = "ctxMeleeSteps";
            this.ctxMeleeSteps.Size = new System.Drawing.Size(61, 4);
            // 
            // grpPotions
            // 
            this.grpPotions.Controls.Add(this.txtPotionsOnlyWhenStunnedForXMS);
            this.grpPotions.Controls.Add(this.lblPotionsOnlyWhenStunnedForXMS);
            this.grpPotions.Controls.Add(this.chkPotionsEnabled);
            this.grpPotions.Controls.Add(this.txtPotionsMendWhenDownXHP);
            this.grpPotions.Controls.Add(this.lblPotionsMendWhenDownXHP);
            this.grpPotions.Controls.Add(this.txtPotionsVigorWhenDownXHP);
            this.grpPotions.Controls.Add(this.lblPotionsVigorWhenDownXHP);
            this.grpPotions.Controls.Add(this.lblPotionsFinalAction);
            this.grpPotions.Controls.Add(this.cboPotionsFinalAction);
            this.grpPotions.Controls.Add(this.chkPotionsRepeatLastStepIndefinitely);
            this.grpPotions.Controls.Add(this.lstPotionsSteps);
            this.grpPotions.Location = new System.Drawing.Point(502, 10);
            this.grpPotions.Name = "grpPotions";
            this.grpPotions.Size = new System.Drawing.Size(230, 334);
            this.grpPotions.TabIndex = 2;
            this.grpPotions.TabStop = false;
            this.grpPotions.Text = "Potions";
            // 
            // txtPotionsOnlyWhenStunnedForXMS
            // 
            this.txtPotionsOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(146, 231);
            this.txtPotionsOnlyWhenStunnedForXMS.Name = "txtPotionsOnlyWhenStunnedForXMS";
            this.txtPotionsOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(62, 20);
            this.txtPotionsOnlyWhenStunnedForXMS.TabIndex = 151;
            // 
            // lblPotionsOnlyWhenStunnedForXMS
            // 
            this.lblPotionsOnlyWhenStunnedForXMS.AutoSize = true;
            this.lblPotionsOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(8, 233);
            this.lblPotionsOnlyWhenStunnedForXMS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPotionsOnlyWhenStunnedForXMS.Name = "lblPotionsOnlyWhenStunnedForXMS";
            this.lblPotionsOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(142, 13);
            this.lblPotionsOnlyWhenStunnedForXMS.TabIndex = 150;
            this.lblPotionsOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkPotionsEnabled
            // 
            this.chkPotionsEnabled.AutoSize = true;
            this.chkPotionsEnabled.Location = new System.Drawing.Point(50, 306);
            this.chkPotionsEnabled.Name = "chkPotionsEnabled";
            this.chkPotionsEnabled.Size = new System.Drawing.Size(71, 17);
            this.chkPotionsEnabled.TabIndex = 149;
            this.chkPotionsEnabled.Text = "Enabled?";
            this.chkPotionsEnabled.UseVisualStyleBackColor = true;
            // 
            // txtPotionsMendWhenDownXHP
            // 
            this.txtPotionsMendWhenDownXHP.Location = new System.Drawing.Point(137, 279);
            this.txtPotionsMendWhenDownXHP.Name = "txtPotionsMendWhenDownXHP";
            this.txtPotionsMendWhenDownXHP.Size = new System.Drawing.Size(74, 20);
            this.txtPotionsMendWhenDownXHP.TabIndex = 146;
            // 
            // lblPotionsMendWhenDownXHP
            // 
            this.lblPotionsMendWhenDownXHP.AutoSize = true;
            this.lblPotionsMendWhenDownXHP.Location = new System.Drawing.Point(11, 283);
            this.lblPotionsMendWhenDownXHP.Name = "lblPotionsMendWhenDownXHP";
            this.lblPotionsMendWhenDownXHP.Size = new System.Drawing.Size(123, 13);
            this.lblPotionsMendWhenDownXHP.TabIndex = 145;
            this.lblPotionsMendWhenDownXHP.Text = "Mend when down X HP:";
            // 
            // txtPotionsVigorWhenDownXHP
            // 
            this.txtPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(137, 254);
            this.txtPotionsVigorWhenDownXHP.Name = "txtPotionsVigorWhenDownXHP";
            this.txtPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(74, 20);
            this.txtPotionsVigorWhenDownXHP.TabIndex = 144;
            // 
            // lblPotionsVigorWhenDownXHP
            // 
            this.lblPotionsVigorWhenDownXHP.AutoSize = true;
            this.lblPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(11, 258);
            this.lblPotionsVigorWhenDownXHP.Name = "lblPotionsVigorWhenDownXHP";
            this.lblPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(120, 13);
            this.lblPotionsVigorWhenDownXHP.TabIndex = 143;
            this.lblPotionsVigorWhenDownXHP.Text = "Vigor when down X HP:";
            // 
            // lblPotionsFinalAction
            // 
            this.lblPotionsFinalAction.AutoSize = true;
            this.lblPotionsFinalAction.Location = new System.Drawing.Point(4, 206);
            this.lblPotionsFinalAction.Name = "lblPotionsFinalAction";
            this.lblPotionsFinalAction.Size = new System.Drawing.Size(64, 13);
            this.lblPotionsFinalAction.TabIndex = 8;
            this.lblPotionsFinalAction.Text = "Final action:";
            // 
            // cboPotionsFinalAction
            // 
            this.cboPotionsFinalAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPotionsFinalAction.FormattingEnabled = true;
            this.cboPotionsFinalAction.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboPotionsFinalAction.Location = new System.Drawing.Point(74, 203);
            this.cboPotionsFinalAction.Name = "cboPotionsFinalAction";
            this.cboPotionsFinalAction.Size = new System.Drawing.Size(134, 21);
            this.cboPotionsFinalAction.TabIndex = 7;
            // 
            // chkPotionsRepeatLastStepIndefinitely
            // 
            this.chkPotionsRepeatLastStepIndefinitely.AutoSize = true;
            this.chkPotionsRepeatLastStepIndefinitely.Location = new System.Drawing.Point(50, 183);
            this.chkPotionsRepeatLastStepIndefinitely.Name = "chkPotionsRepeatLastStepIndefinitely";
            this.chkPotionsRepeatLastStepIndefinitely.Size = new System.Drawing.Size(161, 17);
            this.chkPotionsRepeatLastStepIndefinitely.TabIndex = 4;
            this.chkPotionsRepeatLastStepIndefinitely.Text = "Repeat last step indefinitely?";
            this.chkPotionsRepeatLastStepIndefinitely.UseVisualStyleBackColor = true;
            // 
            // lstPotionsSteps
            // 
            this.lstPotionsSteps.ContextMenuStrip = this.ctxPotionsSteps;
            this.lstPotionsSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstPotionsSteps.FormattingEnabled = true;
            this.lstPotionsSteps.Location = new System.Drawing.Point(3, 16);
            this.lstPotionsSteps.Name = "lstPotionsSteps";
            this.lstPotionsSteps.Size = new System.Drawing.Size(224, 160);
            this.lstPotionsSteps.TabIndex = 2;
            // 
            // ctxPotionsSteps
            // 
            this.ctxPotionsSteps.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxPotionsSteps.Name = "ctxPotionsSteps";
            this.ctxPotionsSteps.Size = new System.Drawing.Size(61, 4);
            // 
            // chkAutogenerateName
            // 
            this.chkAutogenerateName.AutoSize = true;
            this.chkAutogenerateName.Location = new System.Drawing.Point(346, 303);
            this.chkAutogenerateName.Name = "chkAutogenerateName";
            this.chkAutogenerateName.Size = new System.Drawing.Size(125, 17);
            this.chkAutogenerateName.TabIndex = 3;
            this.chkAutogenerateName.Text = "Autogenerate name?";
            this.chkAutogenerateName.UseVisualStyleBackColor = true;
            this.chkAutogenerateName.CheckedChanged += new System.EventHandler(this.chkAutogenerateName_CheckedChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(344, 326);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(151, 20);
            this.txtName.TabIndex = 4;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(264, 328);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(576, 372);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(654, 373);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(260, 379);
            this.lblOnKillMonster.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(79, 13);
            this.lblOnKillMonster.TabIndex = 14;
            this.lblOnKillMonster.Text = "On kill monster:";
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
            this.cboOnKillMonster.Location = new System.Drawing.Point(344, 375);
            this.cboOnKillMonster.Margin = new System.Windows.Forms.Padding(2);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(151, 21);
            this.cboOnKillMonster.TabIndex = 146;
            // 
            // lblAutoEscapeValue
            // 
            this.lblAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblAutoEscapeValue.Location = new System.Drawing.Point(344, 351);
            this.lblAutoEscapeValue.Name = "lblAutoEscapeValue";
            this.lblAutoEscapeValue.Size = new System.Drawing.Size(150, 15);
            this.lblAutoEscapeValue.TabIndex = 147;
            this.lblAutoEscapeValue.Text = "Auto Escape";
            this.lblAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAutoEscape
            // 
            this.lblAutoEscape.AutoSize = true;
            this.lblAutoEscape.Location = new System.Drawing.Point(260, 351);
            this.lblAutoEscape.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAutoEscape.Name = "lblAutoEscape";
            this.lblAutoEscape.Size = new System.Drawing.Size(70, 13);
            this.lblAutoEscape.TabIndex = 148;
            this.lblAutoEscape.Text = "Auto escape:";
            // 
            // frmStrategy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 407);
            this.Controls.Add(this.lblAutoEscape);
            this.Controls.Add(this.lblAutoEscapeValue);
            this.Controls.Add(this.cboOnKillMonster);
            this.Controls.Add(this.lblOnKillMonster);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.chkAutogenerateName);
            this.Controls.Add(this.grpPotions);
            this.Controls.Add(this.grpMelee);
            this.Controls.Add(this.grpMagic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStrategy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Strategy";
            this.grpMagic.ResumeLayout(false);
            this.grpMagic.PerformLayout();
            this.ctxAutoSpellLevels.ResumeLayout(false);
            this.grpMelee.ResumeLayout(false);
            this.grpMelee.PerformLayout();
            this.grpPotions.ResumeLayout(false);
            this.grpPotions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMagic;
        private System.Windows.Forms.ListBox lstMagicSteps;
        private System.Windows.Forms.GroupBox grpMelee;
        private System.Windows.Forms.ListBox lstMeleeSteps;
        private System.Windows.Forms.GroupBox grpPotions;
        private System.Windows.Forms.ListBox lstPotionsSteps;
        private System.Windows.Forms.CheckBox chkAutogenerateName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtManaPool;
        private System.Windows.Forms.Label lblManaPool;
        private System.Windows.Forms.CheckBox chkMagicLastStepIndefinite;
        private System.Windows.Forms.CheckBox chkMeleeRepeatLastStepIndefinitely;
        private System.Windows.Forms.CheckBox chkPotionsRepeatLastStepIndefinitely;
        private System.Windows.Forms.Label lblMagicFinalAction;
        private System.Windows.Forms.ComboBox cboMagicFinalAction;
        private System.Windows.Forms.Label lblMeleeFinalAction;
        private System.Windows.Forms.ComboBox cboMeleeFinalAction;
        private System.Windows.Forms.Label lblPotionsFinalAction;
        private System.Windows.Forms.ComboBox cboPotionsFinalAction;
        private System.Windows.Forms.Label lblAutoSpellLevels;
        private System.Windows.Forms.TextBox txtMagicMendWhenDownXHP;
        private System.Windows.Forms.Label lblMagicMendWhenDownXHP;
        private System.Windows.Forms.TextBox txtMagicVigorWhenDownXHP;
        private System.Windows.Forms.Label lblMagicVigorWhenDownXHP;
        private System.Windows.Forms.TextBox txtPotionsMendWhenDownXHP;
        private System.Windows.Forms.Label lblPotionsMendWhenDownXHP;
        private System.Windows.Forms.TextBox txtPotionsVigorWhenDownXHP;
        private System.Windows.Forms.Label lblPotionsVigorWhenDownXHP;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip ctxMagicSteps;
        private System.Windows.Forms.ContextMenuStrip ctxMeleeSteps;
        private System.Windows.Forms.ContextMenuStrip ctxPotionsSteps;
        private System.Windows.Forms.CheckBox chkMagicEnabled;
        private System.Windows.Forms.CheckBox chkMeleeEnabled;
        private System.Windows.Forms.CheckBox chkPotionsEnabled;
        private System.Windows.Forms.Label lblOnKillMonster;
        private System.Windows.Forms.ComboBox cboOnKillMonster;
        private System.Windows.Forms.Label lblAutoEscapeValue;
        private System.Windows.Forms.Label lblAutoEscape;
        private System.Windows.Forms.ContextMenuStrip ctxAutoSpellLevels;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMinimumAutoSpellLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMaximumAutoSpellLevel;
        private System.Windows.Forms.ToolStripSeparator sepAutoSpellLevels1;
        private System.Windows.Forms.ToolStripMenuItem tsmiInheritAutoSpellLevels;
        private System.Windows.Forms.TextBox txtMagicOnlyWhenStunnedForXMS;
        private System.Windows.Forms.Label lblMagicOnlyWhenStunnedForXMS;
        private System.Windows.Forms.TextBox txtMeleeOnlyWhenStunnedForXMS;
        private System.Windows.Forms.Label lblMeleeOnlyWhenStunnedForXMS;
        private System.Windows.Forms.TextBox txtPotionsOnlyWhenStunnedForXMS;
        private System.Windows.Forms.Label lblPotionsOnlyWhenStunnedForXMS;
    }
}