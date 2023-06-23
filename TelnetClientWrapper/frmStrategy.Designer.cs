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
            this.chkMagicEnabled = new System.Windows.Forms.CheckBox();
            this.lblMagicLastStep = new System.Windows.Forms.Label();
            this.cboMagicLastStep = new System.Windows.Forms.ComboBox();
            this.txtMagicMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblMagicMendWhenDownXHP = new System.Windows.Forms.Label();
            this.txtMagicVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblMagicVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.lblMagicFinalAction = new System.Windows.Forms.Label();
            this.chkPromptForManaPool = new System.Windows.Forms.CheckBox();
            this.cboMagicFinalAction = new System.Windows.Forms.ComboBox();
            this.txtManaPool = new System.Windows.Forms.TextBox();
            this.chkMagicOnlyWhenStunned = new System.Windows.Forms.CheckBox();
            this.chkMagicLastStepIndefinite = new System.Windows.Forms.CheckBox();
            this.lblManaPool = new System.Windows.Forms.Label();
            this.lstMagicSteps = new System.Windows.Forms.ListBox();
            this.ctxMagicSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.grpMelee = new System.Windows.Forms.GroupBox();
            this.chkMeleeEnabled = new System.Windows.Forms.CheckBox();
            this.lblMeleeLastStep = new System.Windows.Forms.Label();
            this.cboMeleeLastStep = new System.Windows.Forms.ComboBox();
            this.lblMeleeFinalAction = new System.Windows.Forms.Label();
            this.cboMeleeFinalAction = new System.Windows.Forms.ComboBox();
            this.chkMeleeRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.chkMeleeOnlyWhenStunned = new System.Windows.Forms.CheckBox();
            this.lstMeleeSteps = new System.Windows.Forms.ListBox();
            this.ctxMeleeSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.grpPotions = new System.Windows.Forms.GroupBox();
            this.chkPotionsEnabled = new System.Windows.Forms.CheckBox();
            this.lblPotionsLastStep = new System.Windows.Forms.Label();
            this.cboPotionsLastStep = new System.Windows.Forms.ComboBox();
            this.txtPotionsMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsMendWhenDownXHP = new System.Windows.Forms.Label();
            this.txtPotionsVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.lblPotionsFinalAction = new System.Windows.Forms.Label();
            this.cboPotionsFinalAction = new System.Windows.Forms.ComboBox();
            this.chkPotionsRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.chkPotionsOnlyWhenStunned = new System.Windows.Forms.CheckBox();
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
            this.grpMelee.SuspendLayout();
            this.grpPotions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMagic
            // 
            this.grpMagic.Controls.Add(this.chkMagicEnabled);
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
            this.grpMagic.Location = new System.Drawing.Point(40, 12);
            this.grpMagic.Margin = new System.Windows.Forms.Padding(4);
            this.grpMagic.Name = "grpMagic";
            this.grpMagic.Padding = new System.Windows.Forms.Padding(4);
            this.grpMagic.Size = new System.Drawing.Size(300, 453);
            this.grpMagic.TabIndex = 0;
            this.grpMagic.TabStop = false;
            this.grpMagic.Text = "Magic";
            // 
            // chkMagicEnabled
            // 
            this.chkMagicEnabled.AutoSize = true;
            this.chkMagicEnabled.Location = new System.Drawing.Point(57, 421);
            this.chkMagicEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.chkMagicEnabled.Name = "chkMagicEnabled";
            this.chkMagicEnabled.Size = new System.Drawing.Size(87, 20);
            this.chkMagicEnabled.TabIndex = 145;
            this.chkMagicEnabled.Text = "Enabled?";
            this.chkMagicEnabled.UseVisualStyleBackColor = true;
            // 
            // lblMagicLastStep
            // 
            this.lblMagicLastStep.AutoSize = true;
            this.lblMagicLastStep.Location = new System.Drawing.Point(5, 196);
            this.lblMagicLastStep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMagicLastStep.Name = "lblMagicLastStep";
            this.lblMagicLastStep.Size = new System.Drawing.Size(64, 16);
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
            this.cboMagicLastStep.Location = new System.Drawing.Point(93, 192);
            this.cboMagicLastStep.Margin = new System.Windows.Forms.Padding(4);
            this.cboMagicLastStep.Name = "cboMagicLastStep";
            this.cboMagicLastStep.Size = new System.Drawing.Size(177, 24);
            this.cboMagicLastStep.TabIndex = 143;
            // 
            // txtMagicMendWhenDownXHP
            // 
            this.txtMagicMendWhenDownXHP.Location = new System.Drawing.Point(173, 390);
            this.txtMagicMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtMagicMendWhenDownXHP.Name = "txtMagicMendWhenDownXHP";
            this.txtMagicMendWhenDownXHP.Size = new System.Drawing.Size(97, 22);
            this.txtMagicMendWhenDownXHP.TabIndex = 142;
            // 
            // lblMagicMendWhenDownXHP
            // 
            this.lblMagicMendWhenDownXHP.AutoSize = true;
            this.lblMagicMendWhenDownXHP.Location = new System.Drawing.Point(5, 395);
            this.lblMagicMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMagicMendWhenDownXHP.Name = "lblMagicMendWhenDownXHP";
            this.lblMagicMendWhenDownXHP.Size = new System.Drawing.Size(146, 16);
            this.lblMagicMendWhenDownXHP.TabIndex = 141;
            this.lblMagicMendWhenDownXHP.Text = "Mend when down X HP:";
            // 
            // txtMagicVigorWhenDownXHP
            // 
            this.txtMagicVigorWhenDownXHP.Location = new System.Drawing.Point(173, 359);
            this.txtMagicVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtMagicVigorWhenDownXHP.Name = "txtMagicVigorWhenDownXHP";
            this.txtMagicVigorWhenDownXHP.Size = new System.Drawing.Size(97, 22);
            this.txtMagicVigorWhenDownXHP.TabIndex = 140;
            // 
            // lblMagicVigorWhenDownXHP
            // 
            this.lblMagicVigorWhenDownXHP.AutoSize = true;
            this.lblMagicVigorWhenDownXHP.Location = new System.Drawing.Point(5, 364);
            this.lblMagicVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMagicVigorWhenDownXHP.Name = "lblMagicVigorWhenDownXHP";
            this.lblMagicVigorWhenDownXHP.Size = new System.Drawing.Size(144, 16);
            this.lblMagicVigorWhenDownXHP.TabIndex = 139;
            this.lblMagicVigorWhenDownXHP.Text = "Vigor when down X HP:";
            // 
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.BackColor = System.Drawing.Color.Silver;
            this.lblAutoSpellLevels.ForeColor = System.Drawing.Color.Black;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(8, 337);
            this.lblAutoSpellLevels.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAutoSpellLevels.Name = "lblAutoSpellLevels";
            this.lblAutoSpellLevels.Size = new System.Drawing.Size(291, 18);
            this.lblAutoSpellLevels.TabIndex = 138;
            this.lblAutoSpellLevels.Text = "AutoSpell lvls";
            this.lblAutoSpellLevels.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMagicFinalAction
            // 
            this.lblMagicFinalAction.AutoSize = true;
            this.lblMagicFinalAction.Location = new System.Drawing.Point(5, 254);
            this.lblMagicFinalAction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMagicFinalAction.Name = "lblMagicFinalAction";
            this.lblMagicFinalAction.Size = new System.Drawing.Size(78, 16);
            this.lblMagicFinalAction.TabIndex = 4;
            this.lblMagicFinalAction.Text = "Final action:";
            // 
            // chkPromptForManaPool
            // 
            this.chkPromptForManaPool.AutoSize = true;
            this.chkPromptForManaPool.Location = new System.Drawing.Point(189, 313);
            this.chkPromptForManaPool.Margin = new System.Windows.Forms.Padding(4);
            this.chkPromptForManaPool.Name = "chkPromptForManaPool";
            this.chkPromptForManaPool.Size = new System.Drawing.Size(79, 20);
            this.chkPromptForManaPool.TabIndex = 10;
            this.chkPromptForManaPool.Text = "Prompt?";
            this.chkPromptForManaPool.UseVisualStyleBackColor = true;
            // 
            // cboMagicFinalAction
            // 
            this.cboMagicFinalAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMagicFinalAction.FormattingEnabled = true;
            this.cboMagicFinalAction.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboMagicFinalAction.Location = new System.Drawing.Point(93, 250);
            this.cboMagicFinalAction.Margin = new System.Windows.Forms.Padding(4);
            this.cboMagicFinalAction.Name = "cboMagicFinalAction";
            this.cboMagicFinalAction.Size = new System.Drawing.Size(177, 24);
            this.cboMagicFinalAction.TabIndex = 3;
            // 
            // txtManaPool
            // 
            this.txtManaPool.Location = new System.Drawing.Point(93, 309);
            this.txtManaPool.Margin = new System.Windows.Forms.Padding(4);
            this.txtManaPool.Name = "txtManaPool";
            this.txtManaPool.Size = new System.Drawing.Size(87, 22);
            this.txtManaPool.TabIndex = 9;
            // 
            // chkMagicOnlyWhenStunned
            // 
            this.chkMagicOnlyWhenStunned.AutoSize = true;
            this.chkMagicOnlyWhenStunned.Location = new System.Drawing.Point(57, 283);
            this.chkMagicOnlyWhenStunned.Margin = new System.Windows.Forms.Padding(4);
            this.chkMagicOnlyWhenStunned.Name = "chkMagicOnlyWhenStunned";
            this.chkMagicOnlyWhenStunned.Size = new System.Drawing.Size(147, 20);
            this.chkMagicOnlyWhenStunned.TabIndex = 1;
            this.chkMagicOnlyWhenStunned.Text = "Only when stunned?";
            this.chkMagicOnlyWhenStunned.UseVisualStyleBackColor = true;
            // 
            // chkMagicLastStepIndefinite
            // 
            this.chkMagicLastStepIndefinite.AutoSize = true;
            this.chkMagicLastStepIndefinite.Location = new System.Drawing.Point(57, 223);
            this.chkMagicLastStepIndefinite.Margin = new System.Windows.Forms.Padding(4);
            this.chkMagicLastStepIndefinite.Name = "chkMagicLastStepIndefinite";
            this.chkMagicLastStepIndefinite.Size = new System.Drawing.Size(200, 20);
            this.chkMagicLastStepIndefinite.TabIndex = 2;
            this.chkMagicLastStepIndefinite.Text = "Repeat last step indefinitely?";
            this.chkMagicLastStepIndefinite.UseVisualStyleBackColor = true;
            // 
            // lblManaPool
            // 
            this.lblManaPool.AutoSize = true;
            this.lblManaPool.Location = new System.Drawing.Point(5, 314);
            this.lblManaPool.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblManaPool.Name = "lblManaPool";
            this.lblManaPool.Size = new System.Drawing.Size(74, 16);
            this.lblManaPool.TabIndex = 8;
            this.lblManaPool.Text = "Mana pool:";
            // 
            // lstMagicSteps
            // 
            this.lstMagicSteps.ContextMenuStrip = this.ctxMagicSteps;
            this.lstMagicSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstMagicSteps.FormattingEnabled = true;
            this.lstMagicSteps.ItemHeight = 16;
            this.lstMagicSteps.Location = new System.Drawing.Point(4, 19);
            this.lstMagicSteps.Margin = new System.Windows.Forms.Padding(4);
            this.lstMagicSteps.Name = "lstMagicSteps";
            this.lstMagicSteps.Size = new System.Drawing.Size(292, 164);
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
            this.grpMelee.Controls.Add(this.chkMeleeEnabled);
            this.grpMelee.Controls.Add(this.lblMeleeLastStep);
            this.grpMelee.Controls.Add(this.cboMeleeLastStep);
            this.grpMelee.Controls.Add(this.lblMeleeFinalAction);
            this.grpMelee.Controls.Add(this.cboMeleeFinalAction);
            this.grpMelee.Controls.Add(this.chkMeleeRepeatLastStepIndefinitely);
            this.grpMelee.Controls.Add(this.chkMeleeOnlyWhenStunned);
            this.grpMelee.Controls.Add(this.lstMeleeSteps);
            this.grpMelee.Location = new System.Drawing.Point(355, 12);
            this.grpMelee.Margin = new System.Windows.Forms.Padding(4);
            this.grpMelee.Name = "grpMelee";
            this.grpMelee.Padding = new System.Windows.Forms.Padding(4);
            this.grpMelee.Size = new System.Drawing.Size(307, 346);
            this.grpMelee.TabIndex = 1;
            this.grpMelee.TabStop = false;
            this.grpMelee.Text = "Melee";
            // 
            // chkMeleeEnabled
            // 
            this.chkMeleeEnabled.AutoSize = true;
            this.chkMeleeEnabled.Location = new System.Drawing.Point(67, 313);
            this.chkMeleeEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.chkMeleeEnabled.Name = "chkMeleeEnabled";
            this.chkMeleeEnabled.Size = new System.Drawing.Size(87, 20);
            this.chkMeleeEnabled.TabIndex = 147;
            this.chkMeleeEnabled.Text = "Enabled?";
            this.chkMeleeEnabled.UseVisualStyleBackColor = true;
            // 
            // lblMeleeLastStep
            // 
            this.lblMeleeLastStep.AutoSize = true;
            this.lblMeleeLastStep.Location = new System.Drawing.Point(15, 196);
            this.lblMeleeLastStep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMeleeLastStep.Name = "lblMeleeLastStep";
            this.lblMeleeLastStep.Size = new System.Drawing.Size(64, 16);
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
            this.cboMeleeLastStep.Location = new System.Drawing.Point(103, 192);
            this.cboMeleeLastStep.Margin = new System.Windows.Forms.Padding(4);
            this.cboMeleeLastStep.Name = "cboMeleeLastStep";
            this.cboMeleeLastStep.Size = new System.Drawing.Size(177, 24);
            this.cboMeleeLastStep.TabIndex = 145;
            // 
            // lblMeleeFinalAction
            // 
            this.lblMeleeFinalAction.AutoSize = true;
            this.lblMeleeFinalAction.Location = new System.Drawing.Point(9, 254);
            this.lblMeleeFinalAction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMeleeFinalAction.Name = "lblMeleeFinalAction";
            this.lblMeleeFinalAction.Size = new System.Drawing.Size(78, 16);
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
            this.cboMeleeFinalAction.Location = new System.Drawing.Point(103, 250);
            this.cboMeleeFinalAction.Margin = new System.Windows.Forms.Padding(4);
            this.cboMeleeFinalAction.Name = "cboMeleeFinalAction";
            this.cboMeleeFinalAction.Size = new System.Drawing.Size(177, 24);
            this.cboMeleeFinalAction.TabIndex = 5;
            // 
            // chkMeleeRepeatLastStepIndefinitely
            // 
            this.chkMeleeRepeatLastStepIndefinitely.AutoSize = true;
            this.chkMeleeRepeatLastStepIndefinitely.Location = new System.Drawing.Point(67, 225);
            this.chkMeleeRepeatLastStepIndefinitely.Margin = new System.Windows.Forms.Padding(4);
            this.chkMeleeRepeatLastStepIndefinitely.Name = "chkMeleeRepeatLastStepIndefinitely";
            this.chkMeleeRepeatLastStepIndefinitely.Size = new System.Drawing.Size(200, 20);
            this.chkMeleeRepeatLastStepIndefinitely.TabIndex = 3;
            this.chkMeleeRepeatLastStepIndefinitely.Text = "Repeat last step indefinitely?";
            this.chkMeleeRepeatLastStepIndefinitely.UseVisualStyleBackColor = true;
            // 
            // chkMeleeOnlyWhenStunned
            // 
            this.chkMeleeOnlyWhenStunned.AutoSize = true;
            this.chkMeleeOnlyWhenStunned.Location = new System.Drawing.Point(67, 283);
            this.chkMeleeOnlyWhenStunned.Margin = new System.Windows.Forms.Padding(4);
            this.chkMeleeOnlyWhenStunned.Name = "chkMeleeOnlyWhenStunned";
            this.chkMeleeOnlyWhenStunned.Size = new System.Drawing.Size(147, 20);
            this.chkMeleeOnlyWhenStunned.TabIndex = 2;
            this.chkMeleeOnlyWhenStunned.Text = "Only when stunned?";
            this.chkMeleeOnlyWhenStunned.UseVisualStyleBackColor = true;
            // 
            // lstMeleeSteps
            // 
            this.lstMeleeSteps.ContextMenuStrip = this.ctxMeleeSteps;
            this.lstMeleeSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstMeleeSteps.FormattingEnabled = true;
            this.lstMeleeSteps.ItemHeight = 16;
            this.lstMeleeSteps.Location = new System.Drawing.Point(4, 19);
            this.lstMeleeSteps.Margin = new System.Windows.Forms.Padding(4);
            this.lstMeleeSteps.Name = "lstMeleeSteps";
            this.lstMeleeSteps.Size = new System.Drawing.Size(299, 164);
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
            this.grpPotions.Controls.Add(this.chkPotionsEnabled);
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
            this.grpPotions.Location = new System.Drawing.Point(669, 12);
            this.grpPotions.Margin = new System.Windows.Forms.Padding(4);
            this.grpPotions.Name = "grpPotions";
            this.grpPotions.Padding = new System.Windows.Forms.Padding(4);
            this.grpPotions.Size = new System.Drawing.Size(307, 411);
            this.grpPotions.TabIndex = 2;
            this.grpPotions.TabStop = false;
            this.grpPotions.Text = "Potions";
            // 
            // chkPotionsEnabled
            // 
            this.chkPotionsEnabled.AutoSize = true;
            this.chkPotionsEnabled.Location = new System.Drawing.Point(67, 376);
            this.chkPotionsEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.chkPotionsEnabled.Name = "chkPotionsEnabled";
            this.chkPotionsEnabled.Size = new System.Drawing.Size(87, 20);
            this.chkPotionsEnabled.TabIndex = 149;
            this.chkPotionsEnabled.Text = "Enabled?";
            this.chkPotionsEnabled.UseVisualStyleBackColor = true;
            // 
            // lblPotionsLastStep
            // 
            this.lblPotionsLastStep.AutoSize = true;
            this.lblPotionsLastStep.Location = new System.Drawing.Point(11, 196);
            this.lblPotionsLastStep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPotionsLastStep.Name = "lblPotionsLastStep";
            this.lblPotionsLastStep.Size = new System.Drawing.Size(64, 16);
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
            this.cboPotionsLastStep.Location = new System.Drawing.Point(99, 192);
            this.cboPotionsLastStep.Margin = new System.Windows.Forms.Padding(4);
            this.cboPotionsLastStep.Name = "cboPotionsLastStep";
            this.cboPotionsLastStep.Size = new System.Drawing.Size(177, 24);
            this.cboPotionsLastStep.TabIndex = 147;
            // 
            // txtPotionsMendWhenDownXHP
            // 
            this.txtPotionsMendWhenDownXHP.Location = new System.Drawing.Point(183, 343);
            this.txtPotionsMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtPotionsMendWhenDownXHP.Name = "txtPotionsMendWhenDownXHP";
            this.txtPotionsMendWhenDownXHP.Size = new System.Drawing.Size(97, 22);
            this.txtPotionsMendWhenDownXHP.TabIndex = 146;
            // 
            // lblPotionsMendWhenDownXHP
            // 
            this.lblPotionsMendWhenDownXHP.AutoSize = true;
            this.lblPotionsMendWhenDownXHP.Location = new System.Drawing.Point(15, 348);
            this.lblPotionsMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPotionsMendWhenDownXHP.Name = "lblPotionsMendWhenDownXHP";
            this.lblPotionsMendWhenDownXHP.Size = new System.Drawing.Size(146, 16);
            this.lblPotionsMendWhenDownXHP.TabIndex = 145;
            this.lblPotionsMendWhenDownXHP.Text = "Mend when down X HP:";
            // 
            // txtPotionsVigorWhenDownXHP
            // 
            this.txtPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(183, 313);
            this.txtPotionsVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtPotionsVigorWhenDownXHP.Name = "txtPotionsVigorWhenDownXHP";
            this.txtPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(97, 22);
            this.txtPotionsVigorWhenDownXHP.TabIndex = 144;
            // 
            // lblPotionsVigorWhenDownXHP
            // 
            this.lblPotionsVigorWhenDownXHP.AutoSize = true;
            this.lblPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(15, 318);
            this.lblPotionsVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPotionsVigorWhenDownXHP.Name = "lblPotionsVigorWhenDownXHP";
            this.lblPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(144, 16);
            this.lblPotionsVigorWhenDownXHP.TabIndex = 143;
            this.lblPotionsVigorWhenDownXHP.Text = "Vigor when down X HP:";
            // 
            // lblPotionsFinalAction
            // 
            this.lblPotionsFinalAction.AutoSize = true;
            this.lblPotionsFinalAction.Location = new System.Drawing.Point(5, 254);
            this.lblPotionsFinalAction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPotionsFinalAction.Name = "lblPotionsFinalAction";
            this.lblPotionsFinalAction.Size = new System.Drawing.Size(78, 16);
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
            this.cboPotionsFinalAction.Location = new System.Drawing.Point(99, 250);
            this.cboPotionsFinalAction.Margin = new System.Windows.Forms.Padding(4);
            this.cboPotionsFinalAction.Name = "cboPotionsFinalAction";
            this.cboPotionsFinalAction.Size = new System.Drawing.Size(177, 24);
            this.cboPotionsFinalAction.TabIndex = 7;
            // 
            // chkPotionsRepeatLastStepIndefinitely
            // 
            this.chkPotionsRepeatLastStepIndefinitely.AutoSize = true;
            this.chkPotionsRepeatLastStepIndefinitely.Location = new System.Drawing.Point(67, 225);
            this.chkPotionsRepeatLastStepIndefinitely.Margin = new System.Windows.Forms.Padding(4);
            this.chkPotionsRepeatLastStepIndefinitely.Name = "chkPotionsRepeatLastStepIndefinitely";
            this.chkPotionsRepeatLastStepIndefinitely.Size = new System.Drawing.Size(200, 20);
            this.chkPotionsRepeatLastStepIndefinitely.TabIndex = 4;
            this.chkPotionsRepeatLastStepIndefinitely.Text = "Repeat last step indefinitely?";
            this.chkPotionsRepeatLastStepIndefinitely.UseVisualStyleBackColor = true;
            // 
            // chkPotionsOnlyWhenStunned
            // 
            this.chkPotionsOnlyWhenStunned.AutoSize = true;
            this.chkPotionsOnlyWhenStunned.Location = new System.Drawing.Point(67, 283);
            this.chkPotionsOnlyWhenStunned.Margin = new System.Windows.Forms.Padding(4);
            this.chkPotionsOnlyWhenStunned.Name = "chkPotionsOnlyWhenStunned";
            this.chkPotionsOnlyWhenStunned.Size = new System.Drawing.Size(147, 20);
            this.chkPotionsOnlyWhenStunned.TabIndex = 3;
            this.chkPotionsOnlyWhenStunned.Text = "Only when stunned?";
            this.chkPotionsOnlyWhenStunned.UseVisualStyleBackColor = true;
            // 
            // lstPotionsSteps
            // 
            this.lstPotionsSteps.ContextMenuStrip = this.ctxPotionsSteps;
            this.lstPotionsSteps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstPotionsSteps.FormattingEnabled = true;
            this.lstPotionsSteps.ItemHeight = 16;
            this.lstPotionsSteps.Location = new System.Drawing.Point(4, 19);
            this.lstPotionsSteps.Margin = new System.Windows.Forms.Padding(4);
            this.lstPotionsSteps.Name = "lstPotionsSteps";
            this.lstPotionsSteps.Size = new System.Drawing.Size(299, 164);
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
            this.chkAutogenerateName.Location = new System.Drawing.Point(461, 373);
            this.chkAutogenerateName.Margin = new System.Windows.Forms.Padding(4);
            this.chkAutogenerateName.Name = "chkAutogenerateName";
            this.chkAutogenerateName.Size = new System.Drawing.Size(154, 20);
            this.chkAutogenerateName.TabIndex = 3;
            this.chkAutogenerateName.Text = "Autogenerate name?";
            this.chkAutogenerateName.UseVisualStyleBackColor = true;
            this.chkAutogenerateName.CheckedChanged += new System.EventHandler(this.chkAutogenerateName_CheckedChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(458, 401);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 22);
            this.txtName.TabIndex = 4;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(352, 404);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(47, 16);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(768, 458);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(872, 459);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(346, 466);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(97, 16);
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
            this.cboOnKillMonster.Location = new System.Drawing.Point(458, 462);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(200, 24);
            this.cboOnKillMonster.TabIndex = 146;
            // 
            // lblAutoEscapeValue
            // 
            this.lblAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblAutoEscapeValue.Location = new System.Drawing.Point(458, 432);
            this.lblAutoEscapeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAutoEscapeValue.Name = "lblAutoEscapeValue";
            this.lblAutoEscapeValue.Size = new System.Drawing.Size(200, 18);
            this.lblAutoEscapeValue.TabIndex = 147;
            this.lblAutoEscapeValue.Text = "Auto Escape";
            this.lblAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAutoEscape
            // 
            this.lblAutoEscape.AutoSize = true;
            this.lblAutoEscape.Location = new System.Drawing.Point(346, 432);
            this.lblAutoEscape.Name = "lblAutoEscape";
            this.lblAutoEscape.Size = new System.Drawing.Size(86, 16);
            this.lblAutoEscape.TabIndex = 148;
            this.lblAutoEscape.Text = "Auto escape:";
            // 
            // frmStrategy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 501);
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
            this.Margin = new System.Windows.Forms.Padding(4);
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
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtManaPool;
        private System.Windows.Forms.Label lblManaPool;
        private System.Windows.Forms.CheckBox chkPromptForManaPool;
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
        private System.Windows.Forms.CheckBox chkMagicEnabled;
        private System.Windows.Forms.CheckBox chkMeleeEnabled;
        private System.Windows.Forms.CheckBox chkPotionsEnabled;
        private System.Windows.Forms.Label lblOnKillMonster;
        private System.Windows.Forms.ComboBox cboOnKillMonster;
        private System.Windows.Forms.Label lblAutoEscapeValue;
        private System.Windows.Forms.Label lblAutoEscape;
    }
}