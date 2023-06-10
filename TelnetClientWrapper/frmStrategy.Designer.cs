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
            this.grpMelee = new System.Windows.Forms.GroupBox();
            this.grpPotions = new System.Windows.Forms.GroupBox();
            this.lstMagicSteps = new System.Windows.Forms.ListBox();
            this.lstMeleeSteps = new System.Windows.Forms.ListBox();
            this.lstPotionsSteps = new System.Windows.Forms.ListBox();
            this.chkAutogenerateName = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblFleeThreshold = new System.Windows.Forms.Label();
            this.txtFleeThreshold = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtManaPool = new System.Windows.Forms.TextBox();
            this.lblManaPool = new System.Windows.Forms.Label();
            this.chkPromptForManaPool = new System.Windows.Forms.CheckBox();
            this.chkStopWhenKillMonster = new System.Windows.Forms.CheckBox();
            this.chkMagicOnlyWhenStunned = new System.Windows.Forms.CheckBox();
            this.chkMeleeOnlyWhenStunned = new System.Windows.Forms.CheckBox();
            this.chkPotionsOnlyWhenStunned = new System.Windows.Forms.CheckBox();
            this.chkMagicLastStepIndefinite = new System.Windows.Forms.CheckBox();
            this.chkMeleeRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.chkPotionsRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.cboMagicFinalAction = new System.Windows.Forms.ComboBox();
            this.lblMagicFinalAction = new System.Windows.Forms.Label();
            this.lblMeleeFinalAction = new System.Windows.Forms.Label();
            this.cboMeleeFinalAction = new System.Windows.Forms.ComboBox();
            this.lblPotionsFinalAction = new System.Windows.Forms.Label();
            this.cboPotionsFinalAction = new System.Windows.Forms.ComboBox();
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.txtMagicVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblMagicVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.txtMagicMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblMagicMendWhenDownXHP = new System.Windows.Forms.Label();
            this.txtPotionsMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsMendWhenDownXHP = new System.Windows.Forms.Label();
            this.txtPotionsVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ctxMagicSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxMeleeSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxPotionsSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblMagicLastStep = new System.Windows.Forms.Label();
            this.cboMagicLastStep = new System.Windows.Forms.ComboBox();
            this.lblMeleeLastStep = new System.Windows.Forms.Label();
            this.cboMeleeLastStep = new System.Windows.Forms.ComboBox();
            this.lblPotionsLastStep = new System.Windows.Forms.Label();
            this.cboPotionsLastStep = new System.Windows.Forms.ComboBox();
            this.grpMagic.SuspendLayout();
            this.grpMelee.SuspendLayout();
            this.grpPotions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMagic
            // 
            this.grpMagic.Controls.Add(this.lblMagicLastStep);
            this.grpMagic.Controls.Add(this.cboMagicLastStep);
            this.grpMagic.Controls.Add(this.txtMagicMendWhenDownXHP);
            this.grpMagic.Controls.Add(this.lblMagicMendWhenDownXHP);
            this.grpMagic.Controls.Add(this.txtMagicVigorWhenDownXHP);
            this.grpMagic.Controls.Add(this.lblMagicVigorWhenDownXHP);
            this.grpMagic.Controls.Add(this.lblAutoSpellLevels);
            this.grpMagic.Controls.Add(this.lblMagicFinalAction);
            this.grpMagic.Controls.Add(this.chkPromptForManaPool);
            this.grpMagic.Controls.Add(this.cboMagicFinalAction);
            this.grpMagic.Controls.Add(this.txtManaPool);
            this.grpMagic.Controls.Add(this.chkMagicOnlyWhenStunned);
            this.grpMagic.Controls.Add(this.chkMagicLastStepIndefinite);
            this.grpMagic.Controls.Add(this.lblManaPool);
            this.grpMagic.Controls.Add(this.lstMagicSteps);
            this.grpMagic.Location = new System.Drawing.Point(30, 10);
            this.grpMagic.Name = "grpMagic";
            this.grpMagic.Size = new System.Drawing.Size(225, 350);
            this.grpMagic.TabIndex = 0;
            this.grpMagic.TabStop = false;
            this.grpMagic.Text = "Magic";
            // 
            // grpMelee
            // 
            this.grpMelee.Controls.Add(this.lblMeleeLastStep);
            this.grpMelee.Controls.Add(this.cboMeleeLastStep);
            this.grpMelee.Controls.Add(this.lblMeleeFinalAction);
            this.grpMelee.Controls.Add(this.cboMeleeFinalAction);
            this.grpMelee.Controls.Add(this.chkMeleeRepeatLastStepIndefinitely);
            this.grpMelee.Controls.Add(this.chkMeleeOnlyWhenStunned);
            this.grpMelee.Controls.Add(this.lstMeleeSteps);
            this.grpMelee.Location = new System.Drawing.Point(266, 10);
            this.grpMelee.Name = "grpMelee";
            this.grpMelee.Size = new System.Drawing.Size(230, 281);
            this.grpMelee.TabIndex = 1;
            this.grpMelee.TabStop = false;
            this.grpMelee.Text = "Melee";
            // 
            // grpPotions
            // 
            this.grpPotions.Controls.Add(this.lblPotionsLastStep);
            this.grpPotions.Controls.Add(this.cboPotionsLastStep);
            this.grpPotions.Controls.Add(this.txtPotionsMendWhenDownXHP);
            this.grpPotions.Controls.Add(this.lblPotionsMendWhenDownXHP);
            this.grpPotions.Controls.Add(this.txtPotionsVigorWhenDownXHP);
            this.grpPotions.Controls.Add(this.lblPotionsVigorWhenDownXHP);
            this.grpPotions.Controls.Add(this.lblPotionsFinalAction);
            this.grpPotions.Controls.Add(this.cboPotionsFinalAction);
            this.grpPotions.Controls.Add(this.chkPotionsRepeatLastStepIndefinitely);
            this.grpPotions.Controls.Add(this.chkPotionsOnlyWhenStunned);
            this.grpPotions.Controls.Add(this.lstPotionsSteps);
            this.grpPotions.Location = new System.Drawing.Point(502, 10);
            this.grpPotions.Name = "grpPotions";
            this.grpPotions.Size = new System.Drawing.Size(230, 312);
            this.grpPotions.TabIndex = 2;
            this.grpPotions.TabStop = false;
            this.grpPotions.Text = "Potions";
            // 
            // lstMagicSteps
            // 
            this.lstMagicSteps.ContextMenuStrip = this.ctxMagicSteps;
            this.lstMagicSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstMagicSteps.FormattingEnabled = true;
            this.lstMagicSteps.Location = new System.Drawing.Point(3, 16);
            this.lstMagicSteps.Name = "lstMagicSteps";
            this.lstMagicSteps.Size = new System.Drawing.Size(219, 134);
            this.lstMagicSteps.TabIndex = 0;
            // 
            // lstMeleeSteps
            // 
            this.lstMeleeSteps.ContextMenuStrip = this.ctxMeleeSteps;
            this.lstMeleeSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstMeleeSteps.FormattingEnabled = true;
            this.lstMeleeSteps.Location = new System.Drawing.Point(3, 16);
            this.lstMeleeSteps.Name = "lstMeleeSteps";
            this.lstMeleeSteps.Size = new System.Drawing.Size(224, 134);
            this.lstMeleeSteps.TabIndex = 1;
            // 
            // lstPotionsSteps
            // 
            this.lstPotionsSteps.ContextMenuStrip = this.ctxPotionsSteps;
            this.lstPotionsSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstPotionsSteps.FormattingEnabled = true;
            this.lstPotionsSteps.Location = new System.Drawing.Point(3, 16);
            this.lstPotionsSteps.Name = "lstPotionsSteps";
            this.lstPotionsSteps.Size = new System.Drawing.Size(224, 134);
            this.lstPotionsSteps.TabIndex = 2;
            // 
            // chkAutogenerateName
            // 
            this.chkAutogenerateName.AutoSize = true;
            this.chkAutogenerateName.Location = new System.Drawing.Point(540, 331);
            this.chkAutogenerateName.Name = "chkAutogenerateName";
            this.chkAutogenerateName.Size = new System.Drawing.Size(96, 17);
            this.chkAutogenerateName.TabIndex = 3;
            this.chkAutogenerateName.Text = "Autogenerate?";
            this.chkAutogenerateName.UseVisualStyleBackColor = true;
            this.chkAutogenerateName.CheckedChanged += new System.EventHandler(this.chkAutogenerateName_CheckedChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(379, 328);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(155, 20);
            this.txtName.TabIndex = 4;
            // 
            // lblFleeThreshold
            // 
            this.lblFleeThreshold.AutoSize = true;
            this.lblFleeThreshold.Location = new System.Drawing.Point(274, 357);
            this.lblFleeThreshold.Name = "lblFleeThreshold";
            this.lblFleeThreshold.Size = new System.Drawing.Size(94, 13);
            this.lblFleeThreshold.TabIndex = 5;
            this.lblFleeThreshold.Text = "Flee HP threshold:";
            // 
            // txtFleeThreshold
            // 
            this.txtFleeThreshold.Location = new System.Drawing.Point(379, 354);
            this.txtFleeThreshold.Name = "txtFleeThreshold";
            this.txtFleeThreshold.Size = new System.Drawing.Size(155, 20);
            this.txtFleeThreshold.TabIndex = 6;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(274, 331);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name:";
            // 
            // txtManaPool
            // 
            this.txtManaPool.Location = new System.Drawing.Point(70, 251);
            this.txtManaPool.Name = "txtManaPool";
            this.txtManaPool.Size = new System.Drawing.Size(66, 20);
            this.txtManaPool.TabIndex = 9;
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
            // chkPromptForManaPool
            // 
            this.chkPromptForManaPool.AutoSize = true;
            this.chkPromptForManaPool.Location = new System.Drawing.Point(142, 254);
            this.chkPromptForManaPool.Name = "chkPromptForManaPool";
            this.chkPromptForManaPool.Size = new System.Drawing.Size(65, 17);
            this.chkPromptForManaPool.TabIndex = 10;
            this.chkPromptForManaPool.Text = "Prompt?";
            this.chkPromptForManaPool.UseVisualStyleBackColor = true;
            // 
            // chkStopWhenKillMonster
            // 
            this.chkStopWhenKillMonster.AutoSize = true;
            this.chkStopWhenKillMonster.Location = new System.Drawing.Point(355, 302);
            this.chkStopWhenKillMonster.Name = "chkStopWhenKillMonster";
            this.chkStopWhenKillMonster.Size = new System.Drawing.Size(138, 17);
            this.chkStopWhenKillMonster.TabIndex = 11;
            this.chkStopWhenKillMonster.Text = "Stop when kill monster?";
            this.chkStopWhenKillMonster.UseVisualStyleBackColor = true;
            // 
            // chkMagicOnlyWhenStunned
            // 
            this.chkMagicOnlyWhenStunned.AutoSize = true;
            this.chkMagicOnlyWhenStunned.Location = new System.Drawing.Point(43, 230);
            this.chkMagicOnlyWhenStunned.Name = "chkMagicOnlyWhenStunned";
            this.chkMagicOnlyWhenStunned.Size = new System.Drawing.Size(123, 17);
            this.chkMagicOnlyWhenStunned.TabIndex = 1;
            this.chkMagicOnlyWhenStunned.Text = "Only when stunned?";
            this.chkMagicOnlyWhenStunned.UseVisualStyleBackColor = true;
            // 
            // chkMeleeOnlyWhenStunned
            // 
            this.chkMeleeOnlyWhenStunned.AutoSize = true;
            this.chkMeleeOnlyWhenStunned.Location = new System.Drawing.Point(50, 230);
            this.chkMeleeOnlyWhenStunned.Name = "chkMeleeOnlyWhenStunned";
            this.chkMeleeOnlyWhenStunned.Size = new System.Drawing.Size(123, 17);
            this.chkMeleeOnlyWhenStunned.TabIndex = 2;
            this.chkMeleeOnlyWhenStunned.Text = "Only when stunned?";
            this.chkMeleeOnlyWhenStunned.UseVisualStyleBackColor = true;
            // 
            // chkPotionsOnlyWhenStunned
            // 
            this.chkPotionsOnlyWhenStunned.AutoSize = true;
            this.chkPotionsOnlyWhenStunned.Location = new System.Drawing.Point(50, 230);
            this.chkPotionsOnlyWhenStunned.Name = "chkPotionsOnlyWhenStunned";
            this.chkPotionsOnlyWhenStunned.Size = new System.Drawing.Size(123, 17);
            this.chkPotionsOnlyWhenStunned.TabIndex = 3;
            this.chkPotionsOnlyWhenStunned.Text = "Only when stunned?";
            this.chkPotionsOnlyWhenStunned.UseVisualStyleBackColor = true;
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
            // lblMagicFinalAction
            // 
            this.lblMagicFinalAction.AutoSize = true;
            this.lblMagicFinalAction.Location = new System.Drawing.Point(4, 206);
            this.lblMagicFinalAction.Name = "lblMagicFinalAction";
            this.lblMagicFinalAction.Size = new System.Drawing.Size(64, 13);
            this.lblMagicFinalAction.TabIndex = 4;
            this.lblMagicFinalAction.Text = "Final action:";
            // 
            // lblMeleeFinalAction
            // 
            this.lblMeleeFinalAction.AutoSize = true;
            this.lblMeleeFinalAction.Location = new System.Drawing.Point(7, 206);
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
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.BackColor = System.Drawing.Color.Silver;
            this.lblAutoSpellLevels.ForeColor = System.Drawing.Color.Black;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(6, 274);
            this.lblAutoSpellLevels.Name = "lblAutoSpellLevels";
            this.lblAutoSpellLevels.Size = new System.Drawing.Size(218, 15);
            this.lblAutoSpellLevels.TabIndex = 138;
            this.lblAutoSpellLevels.Text = "AutoSpell lvls";
            this.lblAutoSpellLevels.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(565, 358);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(646, 358);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ctxMagicSteps
            // 
            this.ctxMagicSteps.Name = "ctxMagicSteps";
            this.ctxMagicSteps.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxMeleeSteps
            // 
            this.ctxMeleeSteps.Name = "ctxMeleeSteps";
            this.ctxMeleeSteps.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxPotionsSteps
            // 
            this.ctxPotionsSteps.Name = "ctxPotionsSteps";
            this.ctxPotionsSteps.Size = new System.Drawing.Size(61, 4);
            // 
            // lblMagicLastStep
            // 
            this.lblMagicLastStep.AutoSize = true;
            this.lblMagicLastStep.Location = new System.Drawing.Point(4, 159);
            this.lblMagicLastStep.Name = "lblMagicLastStep";
            this.lblMagicLastStep.Size = new System.Drawing.Size(53, 13);
            this.lblMagicLastStep.TabIndex = 144;
            this.lblMagicLastStep.Text = "Last step:";
            // 
            // cboMagicLastStep
            // 
            this.cboMagicLastStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMagicLastStep.FormattingEnabled = true;
            this.cboMagicLastStep.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboMagicLastStep.Location = new System.Drawing.Point(70, 156);
            this.cboMagicLastStep.Name = "cboMagicLastStep";
            this.cboMagicLastStep.Size = new System.Drawing.Size(134, 21);
            this.cboMagicLastStep.TabIndex = 143;
            // 
            // lblMeleeLastStep
            // 
            this.lblMeleeLastStep.AutoSize = true;
            this.lblMeleeLastStep.Location = new System.Drawing.Point(11, 159);
            this.lblMeleeLastStep.Name = "lblMeleeLastStep";
            this.lblMeleeLastStep.Size = new System.Drawing.Size(53, 13);
            this.lblMeleeLastStep.TabIndex = 146;
            this.lblMeleeLastStep.Text = "Last step:";
            // 
            // cboMeleeLastStep
            // 
            this.cboMeleeLastStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMeleeLastStep.FormattingEnabled = true;
            this.cboMeleeLastStep.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboMeleeLastStep.Location = new System.Drawing.Point(77, 156);
            this.cboMeleeLastStep.Name = "cboMeleeLastStep";
            this.cboMeleeLastStep.Size = new System.Drawing.Size(134, 21);
            this.cboMeleeLastStep.TabIndex = 145;
            // 
            // lblPotionsLastStep
            // 
            this.lblPotionsLastStep.AutoSize = true;
            this.lblPotionsLastStep.Location = new System.Drawing.Point(8, 159);
            this.lblPotionsLastStep.Name = "lblPotionsLastStep";
            this.lblPotionsLastStep.Size = new System.Drawing.Size(53, 13);
            this.lblPotionsLastStep.TabIndex = 148;
            this.lblPotionsLastStep.Text = "Last step:";
            // 
            // cboPotionsLastStep
            // 
            this.cboPotionsLastStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPotionsLastStep.FormattingEnabled = true;
            this.cboPotionsLastStep.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboPotionsLastStep.Location = new System.Drawing.Point(74, 156);
            this.cboPotionsLastStep.Name = "cboPotionsLastStep";
            this.cboPotionsLastStep.Size = new System.Drawing.Size(134, 21);
            this.cboPotionsLastStep.TabIndex = 147;
            // 
            // frmStrategy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 393);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkStopWhenKillMonster);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtFleeThreshold);
            this.Controls.Add(this.lblFleeThreshold);
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
        private System.Windows.Forms.Label lblFleeThreshold;
        private System.Windows.Forms.TextBox txtFleeThreshold;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtManaPool;
        private System.Windows.Forms.Label lblManaPool;
        private System.Windows.Forms.CheckBox chkPromptForManaPool;
        private System.Windows.Forms.CheckBox chkStopWhenKillMonster;
        private System.Windows.Forms.CheckBox chkMagicOnlyWhenStunned;
        private System.Windows.Forms.CheckBox chkMeleeOnlyWhenStunned;
        private System.Windows.Forms.CheckBox chkPotionsOnlyWhenStunned;
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
        private System.Windows.Forms.Label lblMagicLastStep;
        private System.Windows.Forms.ComboBox cboMagicLastStep;
        private System.Windows.Forms.Label lblMeleeLastStep;
        private System.Windows.Forms.ComboBox cboMeleeLastStep;
        private System.Windows.Forms.Label lblPotionsLastStep;
        private System.Windows.Forms.ComboBox cboPotionsLastStep;
    }
}