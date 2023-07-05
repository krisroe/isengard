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
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.lblMagicFinalAction = new System.Windows.Forms.Label();
            this.cboMagicFinalAction = new System.Windows.Forms.ComboBox();
            this.txtManaPool = new System.Windows.Forms.TextBox();
            this.chkMagicLastStepIndefinite = new System.Windows.Forms.CheckBox();
            this.lblManaPool = new System.Windows.Forms.Label();
            this.lstMagicSteps = new System.Windows.Forms.ListBox();
            this.ctxMagicSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiMagicAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddStun = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddOffensiveAuto = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddOffensiveLevel1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddOffensiveLevel2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddOffensiveLevel3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddOffensiveLevel4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddOffensiveLevel5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMagicAddVigor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMagicAddMendWounds = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMagicAddGenericHeal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMagicAddCurePoison = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMagicRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMagicMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMagicMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.grpMelee = new System.Windows.Forms.GroupBox();
            this.txtMeleeOnlyWhenStunnedForXMS = new System.Windows.Forms.TextBox();
            this.lblMeleeOnlyWhenStunnedForXMS = new System.Windows.Forms.Label();
            this.chkMeleeEnabled = new System.Windows.Forms.CheckBox();
            this.lblMeleeFinalAction = new System.Windows.Forms.Label();
            this.cboMeleeFinalAction = new System.Windows.Forms.ComboBox();
            this.chkMeleeRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.lstMeleeSteps = new System.Windows.Forms.ListBox();
            this.ctxMeleeSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiMeleeAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddRegularAttack = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddPowerAttack = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMeleeRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMeleeMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMeleeMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.grpPotions = new System.Windows.Forms.GroupBox();
            this.txtPotionsOnlyWhenStunnedForXMS = new System.Windows.Forms.TextBox();
            this.lblPotionsOnlyWhenStunnedForXMS = new System.Windows.Forms.Label();
            this.chkPotionsEnabled = new System.Windows.Forms.CheckBox();
            this.lblPotionsFinalAction = new System.Windows.Forms.Label();
            this.cboPotionsFinalAction = new System.Windows.Forms.ComboBox();
            this.chkPotionsRepeatLastStepIndefinitely = new System.Windows.Forms.CheckBox();
            this.lstPotionsSteps = new System.Windows.Forms.ListBox();
            this.ctxPotionsSteps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPotionsAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPotionsAddVigor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPotionsAddMendWounds = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPotionsAddGenericHeal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPotionsAddCurePoison = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPotionsRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPotionsMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPotionsMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.chkAutogenerateName = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblOnKillMonster = new System.Windows.Forms.Label();
            this.cboOnKillMonster = new System.Windows.Forms.ComboBox();
            this.grpMagic.SuspendLayout();
            this.ctxMagicSteps.SuspendLayout();
            this.grpMelee.SuspendLayout();
            this.ctxMeleeSteps.SuspendLayout();
            this.grpPotions.SuspendLayout();
            this.ctxPotionsSteps.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMagic
            // 
            this.grpMagic.Controls.Add(this.txtMagicOnlyWhenStunnedForXMS);
            this.grpMagic.Controls.Add(this.lblMagicOnlyWhenStunnedForXMS);
            this.grpMagic.Controls.Add(this.chkMagicEnabled);
            this.grpMagic.Controls.Add(this.lblAutoSpellLevels);
            this.grpMagic.Controls.Add(this.lblMagicFinalAction);
            this.grpMagic.Controls.Add(this.cboMagicFinalAction);
            this.grpMagic.Controls.Add(this.txtManaPool);
            this.grpMagic.Controls.Add(this.chkMagicLastStepIndefinite);
            this.grpMagic.Controls.Add(this.lblManaPool);
            this.grpMagic.Controls.Add(this.lstMagicSteps);
            this.grpMagic.Location = new System.Drawing.Point(30, 10);
            this.grpMagic.Name = "grpMagic";
            this.grpMagic.Size = new System.Drawing.Size(225, 319);
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
            this.lblMagicOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(162, 15);
            this.lblMagicOnlyWhenStunnedForXMS.TabIndex = 146;
            this.lblMagicOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkMagicEnabled
            // 
            this.chkMagicEnabled.AutoSize = true;
            this.chkMagicEnabled.Location = new System.Drawing.Point(43, 247);
            this.chkMagicEnabled.Name = "chkMagicEnabled";
            this.chkMagicEnabled.Size = new System.Drawing.Size(82, 19);
            this.chkMagicEnabled.TabIndex = 145;
            this.chkMagicEnabled.Text = "Enabled?";
            this.chkMagicEnabled.UseVisualStyleBackColor = true;
            // 
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.BackColor = System.Drawing.Color.Silver;
            this.lblAutoSpellLevels.ForeColor = System.Drawing.Color.Black;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(4, 293);
            this.lblAutoSpellLevels.Name = "lblAutoSpellLevels";
            this.lblAutoSpellLevels.Size = new System.Drawing.Size(218, 15);
            this.lblAutoSpellLevels.TabIndex = 138;
            this.lblAutoSpellLevels.Text = "AutoSpell lvls";
            this.lblAutoSpellLevels.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMagicFinalAction
            // 
            this.lblMagicFinalAction.AutoSize = true;
            this.lblMagicFinalAction.Location = new System.Drawing.Point(4, 206);
            this.lblMagicFinalAction.Name = "lblMagicFinalAction";
            this.lblMagicFinalAction.Size = new System.Drawing.Size(73, 15);
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
            this.cboMagicFinalAction.Location = new System.Drawing.Point(83, 203);
            this.cboMagicFinalAction.Name = "cboMagicFinalAction";
            this.cboMagicFinalAction.Size = new System.Drawing.Size(121, 21);
            this.cboMagicFinalAction.TabIndex = 3;
            // 
            // txtManaPool
            // 
            this.txtManaPool.Location = new System.Drawing.Point(68, 270);
            this.txtManaPool.Name = "txtManaPool";
            this.txtManaPool.Size = new System.Drawing.Size(134, 20);
            this.txtManaPool.TabIndex = 9;
            // 
            // chkMagicLastStepIndefinite
            // 
            this.chkMagicLastStepIndefinite.AutoSize = true;
            this.chkMagicLastStepIndefinite.Location = new System.Drawing.Point(43, 181);
            this.chkMagicLastStepIndefinite.Name = "chkMagicLastStepIndefinite";
            this.chkMagicLastStepIndefinite.Size = new System.Drawing.Size(185, 19);
            this.chkMagicLastStepIndefinite.TabIndex = 2;
            this.chkMagicLastStepIndefinite.Text = "Repeat last step indefinitely?";
            this.chkMagicLastStepIndefinite.UseVisualStyleBackColor = true;
            // 
            // lblManaPool
            // 
            this.lblManaPool.AutoSize = true;
            this.lblManaPool.Location = new System.Drawing.Point(2, 274);
            this.lblManaPool.Name = "lblManaPool";
            this.lblManaPool.Size = new System.Drawing.Size(69, 15);
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
            this.ctxMagicSteps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMagicAdd,
            this.tsmiMagicRemove,
            this.tsmiMagicMoveUp,
            this.tsmiMagicMoveDown});
            this.ctxMagicSteps.Name = "ctxMagicSteps";
            this.ctxMagicSteps.Size = new System.Drawing.Size(159, 100);
            this.ctxMagicSteps.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMagicSteps_Opening);
            this.ctxMagicSteps.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxMagicSteps_ItemClicked);
            // 
            // tsmiMagicAdd
            // 
            this.tsmiMagicAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddStun,
            this.tsmiAddOffensiveAuto,
            this.tsmiAddOffensiveLevel1,
            this.tsmiAddOffensiveLevel2,
            this.tsmiAddOffensiveLevel3,
            this.tsmiAddOffensiveLevel4,
            this.tsmiAddOffensiveLevel5,
            this.tsmiMagicAddVigor,
            this.tsmiMagicAddMendWounds,
            this.tsmiMagicAddGenericHeal,
            this.tsmiMagicAddCurePoison});
            this.tsmiMagicAdd.Name = "tsmiMagicAdd";
            this.tsmiMagicAdd.Size = new System.Drawing.Size(158, 24);
            this.tsmiMagicAdd.Text = "Add";
            // 
            // tsmiAddStun
            // 
            this.tsmiAddStun.Name = "tsmiAddStun";
            this.tsmiAddStun.Size = new System.Drawing.Size(241, 26);
            this.tsmiAddStun.Text = "Stun";
            this.tsmiAddStun.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiAddOffensiveAuto
            // 
            this.tsmiAddOffensiveAuto.Name = "tsmiAddOffensiveAuto";
            this.tsmiAddOffensiveAuto.Size = new System.Drawing.Size(241, 26);
            this.tsmiAddOffensiveAuto.Text = "Offensive Spell Auto";
            this.tsmiAddOffensiveAuto.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiAddOffensiveLevel1
            // 
            this.tsmiAddOffensiveLevel1.Name = "tsmiAddOffensiveLevel1";
            this.tsmiAddOffensiveLevel1.Size = new System.Drawing.Size(241, 26);
            this.tsmiAddOffensiveLevel1.Text = "Offensive Spell Level 1";
            this.tsmiAddOffensiveLevel1.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiAddOffensiveLevel2
            // 
            this.tsmiAddOffensiveLevel2.Name = "tsmiAddOffensiveLevel2";
            this.tsmiAddOffensiveLevel2.Size = new System.Drawing.Size(241, 26);
            this.tsmiAddOffensiveLevel2.Text = "Offensive Spell Level 2";
            this.tsmiAddOffensiveLevel2.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiAddOffensiveLevel3
            // 
            this.tsmiAddOffensiveLevel3.Name = "tsmiAddOffensiveLevel3";
            this.tsmiAddOffensiveLevel3.Size = new System.Drawing.Size(241, 26);
            this.tsmiAddOffensiveLevel3.Text = "Offensive Spell Level 3";
            this.tsmiAddOffensiveLevel3.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiAddOffensiveLevel4
            // 
            this.tsmiAddOffensiveLevel4.Name = "tsmiAddOffensiveLevel4";
            this.tsmiAddOffensiveLevel4.Size = new System.Drawing.Size(241, 26);
            this.tsmiAddOffensiveLevel4.Text = "Offensive Spell Level 4";
            this.tsmiAddOffensiveLevel4.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiAddOffensiveLevel5
            // 
            this.tsmiAddOffensiveLevel5.Name = "tsmiAddOffensiveLevel5";
            this.tsmiAddOffensiveLevel5.Size = new System.Drawing.Size(241, 26);
            this.tsmiAddOffensiveLevel5.Text = "Offensive Spell Level 5";
            this.tsmiAddOffensiveLevel5.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiMagicAddVigor
            // 
            this.tsmiMagicAddVigor.Name = "tsmiMagicAddVigor";
            this.tsmiMagicAddVigor.Size = new System.Drawing.Size(241, 26);
            this.tsmiMagicAddVigor.Text = "Vigor";
            this.tsmiMagicAddVigor.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiMagicAddMendWounds
            // 
            this.tsmiMagicAddMendWounds.Name = "tsmiMagicAddMendWounds";
            this.tsmiMagicAddMendWounds.Size = new System.Drawing.Size(241, 26);
            this.tsmiMagicAddMendWounds.Text = "Mend-Wounds";
            this.tsmiMagicAddMendWounds.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiMagicAddGenericHeal
            // 
            this.tsmiMagicAddGenericHeal.Name = "tsmiMagicAddGenericHeal";
            this.tsmiMagicAddGenericHeal.Size = new System.Drawing.Size(241, 26);
            this.tsmiMagicAddGenericHeal.Text = "Generic Heal";
            this.tsmiMagicAddGenericHeal.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiMagicAddCurePoison
            // 
            this.tsmiMagicAddCurePoison.Name = "tsmiMagicAddCurePoison";
            this.tsmiMagicAddCurePoison.Size = new System.Drawing.Size(241, 26);
            this.tsmiMagicAddCurePoison.Text = "Cure-Poison";
            this.tsmiMagicAddCurePoison.Click += new System.EventHandler(this.tsmiMagicAdd_Click);
            // 
            // tsmiMagicRemove
            // 
            this.tsmiMagicRemove.Name = "tsmiMagicRemove";
            this.tsmiMagicRemove.Size = new System.Drawing.Size(158, 24);
            this.tsmiMagicRemove.Text = "Remove";
            // 
            // tsmiMagicMoveUp
            // 
            this.tsmiMagicMoveUp.Name = "tsmiMagicMoveUp";
            this.tsmiMagicMoveUp.Size = new System.Drawing.Size(158, 24);
            this.tsmiMagicMoveUp.Text = "Move Up";
            // 
            // tsmiMagicMoveDown
            // 
            this.tsmiMagicMoveDown.Name = "tsmiMagicMoveDown";
            this.tsmiMagicMoveDown.Size = new System.Drawing.Size(158, 24);
            this.tsmiMagicMoveDown.Text = "Move Down";
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
            this.grpMelee.Size = new System.Drawing.Size(230, 272);
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
            this.lblMeleeOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(162, 15);
            this.lblMeleeOnlyWhenStunnedForXMS.TabIndex = 148;
            this.lblMeleeOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkMeleeEnabled
            // 
            this.chkMeleeEnabled.AutoSize = true;
            this.chkMeleeEnabled.Location = new System.Drawing.Point(50, 247);
            this.chkMeleeEnabled.Name = "chkMeleeEnabled";
            this.chkMeleeEnabled.Size = new System.Drawing.Size(82, 19);
            this.chkMeleeEnabled.TabIndex = 147;
            this.chkMeleeEnabled.Text = "Enabled?";
            this.chkMeleeEnabled.UseVisualStyleBackColor = true;
            // 
            // lblMeleeFinalAction
            // 
            this.lblMeleeFinalAction.AutoSize = true;
            this.lblMeleeFinalAction.Location = new System.Drawing.Point(11, 206);
            this.lblMeleeFinalAction.Name = "lblMeleeFinalAction";
            this.lblMeleeFinalAction.Size = new System.Drawing.Size(73, 15);
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
            this.cboMeleeFinalAction.Location = new System.Drawing.Point(90, 203);
            this.cboMeleeFinalAction.Name = "cboMeleeFinalAction";
            this.cboMeleeFinalAction.Size = new System.Drawing.Size(121, 21);
            this.cboMeleeFinalAction.TabIndex = 5;
            // 
            // chkMeleeRepeatLastStepIndefinitely
            // 
            this.chkMeleeRepeatLastStepIndefinitely.AutoSize = true;
            this.chkMeleeRepeatLastStepIndefinitely.Location = new System.Drawing.Point(50, 181);
            this.chkMeleeRepeatLastStepIndefinitely.Name = "chkMeleeRepeatLastStepIndefinitely";
            this.chkMeleeRepeatLastStepIndefinitely.Size = new System.Drawing.Size(185, 19);
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
            this.ctxMeleeSteps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMeleeAdd,
            this.tsmiMeleeRemove,
            this.tsmiMeleeMoveUp,
            this.tsmiMeleeMoveDown});
            this.ctxMeleeSteps.Name = "ctxMeleeSteps";
            this.ctxMeleeSteps.Size = new System.Drawing.Size(159, 100);
            this.ctxMeleeSteps.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMeleeSteps_Opening);
            this.ctxMeleeSteps.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxMeleeSteps_ItemClicked);
            // 
            // tsmiMeleeAdd
            // 
            this.tsmiMeleeAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddRegularAttack,
            this.tsmiAddPowerAttack});
            this.tsmiMeleeAdd.Name = "tsmiMeleeAdd";
            this.tsmiMeleeAdd.Size = new System.Drawing.Size(158, 24);
            this.tsmiMeleeAdd.Text = "Add";
            // 
            // tsmiAddRegularAttack
            // 
            this.tsmiAddRegularAttack.Name = "tsmiAddRegularAttack";
            this.tsmiAddRegularAttack.Size = new System.Drawing.Size(189, 26);
            this.tsmiAddRegularAttack.Text = "Regular Attack";
            this.tsmiAddRegularAttack.Click += new System.EventHandler(this.tsmiMeleeAdd_Click);
            // 
            // tsmiAddPowerAttack
            // 
            this.tsmiAddPowerAttack.Name = "tsmiAddPowerAttack";
            this.tsmiAddPowerAttack.Size = new System.Drawing.Size(189, 26);
            this.tsmiAddPowerAttack.Text = "Power Attack";
            this.tsmiAddPowerAttack.Click += new System.EventHandler(this.tsmiMeleeAdd_Click);
            // 
            // tsmiMeleeRemove
            // 
            this.tsmiMeleeRemove.Name = "tsmiMeleeRemove";
            this.tsmiMeleeRemove.Size = new System.Drawing.Size(158, 24);
            this.tsmiMeleeRemove.Text = "Remove";
            // 
            // tsmiMeleeMoveUp
            // 
            this.tsmiMeleeMoveUp.Name = "tsmiMeleeMoveUp";
            this.tsmiMeleeMoveUp.Size = new System.Drawing.Size(158, 24);
            this.tsmiMeleeMoveUp.Text = "Move Up";
            // 
            // tsmiMeleeMoveDown
            // 
            this.tsmiMeleeMoveDown.Name = "tsmiMeleeMoveDown";
            this.tsmiMeleeMoveDown.Size = new System.Drawing.Size(158, 24);
            this.tsmiMeleeMoveDown.Text = "Move Down";
            // 
            // grpPotions
            // 
            this.grpPotions.Controls.Add(this.txtPotionsOnlyWhenStunnedForXMS);
            this.grpPotions.Controls.Add(this.lblPotionsOnlyWhenStunnedForXMS);
            this.grpPotions.Controls.Add(this.chkPotionsEnabled);
            this.grpPotions.Controls.Add(this.lblPotionsFinalAction);
            this.grpPotions.Controls.Add(this.cboPotionsFinalAction);
            this.grpPotions.Controls.Add(this.chkPotionsRepeatLastStepIndefinitely);
            this.grpPotions.Controls.Add(this.lstPotionsSteps);
            this.grpPotions.Location = new System.Drawing.Point(502, 10);
            this.grpPotions.Name = "grpPotions";
            this.grpPotions.Size = new System.Drawing.Size(230, 272);
            this.grpPotions.TabIndex = 2;
            this.grpPotions.TabStop = false;
            this.grpPotions.Text = "Potions";
            // 
            // txtPotionsOnlyWhenStunnedForXMS
            // 
            this.txtPotionsOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(146, 228);
            this.txtPotionsOnlyWhenStunnedForXMS.Name = "txtPotionsOnlyWhenStunnedForXMS";
            this.txtPotionsOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(62, 20);
            this.txtPotionsOnlyWhenStunnedForXMS.TabIndex = 151;
            // 
            // lblPotionsOnlyWhenStunnedForXMS
            // 
            this.lblPotionsOnlyWhenStunnedForXMS.AutoSize = true;
            this.lblPotionsOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(8, 230);
            this.lblPotionsOnlyWhenStunnedForXMS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPotionsOnlyWhenStunnedForXMS.Name = "lblPotionsOnlyWhenStunnedForXMS";
            this.lblPotionsOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(162, 15);
            this.lblPotionsOnlyWhenStunnedForXMS.TabIndex = 150;
            this.lblPotionsOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkPotionsEnabled
            // 
            this.chkPotionsEnabled.AutoSize = true;
            this.chkPotionsEnabled.Location = new System.Drawing.Point(50, 247);
            this.chkPotionsEnabled.Name = "chkPotionsEnabled";
            this.chkPotionsEnabled.Size = new System.Drawing.Size(82, 19);
            this.chkPotionsEnabled.TabIndex = 149;
            this.chkPotionsEnabled.Text = "Enabled?";
            this.chkPotionsEnabled.UseVisualStyleBackColor = true;
            // 
            // lblPotionsFinalAction
            // 
            this.lblPotionsFinalAction.AutoSize = true;
            this.lblPotionsFinalAction.Location = new System.Drawing.Point(4, 206);
            this.lblPotionsFinalAction.Name = "lblPotionsFinalAction";
            this.lblPotionsFinalAction.Size = new System.Drawing.Size(73, 15);
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
            this.cboPotionsFinalAction.Location = new System.Drawing.Point(83, 203);
            this.cboPotionsFinalAction.Name = "cboPotionsFinalAction";
            this.cboPotionsFinalAction.Size = new System.Drawing.Size(125, 21);
            this.cboPotionsFinalAction.TabIndex = 7;
            // 
            // chkPotionsRepeatLastStepIndefinitely
            // 
            this.chkPotionsRepeatLastStepIndefinitely.AutoSize = true;
            this.chkPotionsRepeatLastStepIndefinitely.Location = new System.Drawing.Point(50, 180);
            this.chkPotionsRepeatLastStepIndefinitely.Name = "chkPotionsRepeatLastStepIndefinitely";
            this.chkPotionsRepeatLastStepIndefinitely.Size = new System.Drawing.Size(185, 19);
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
            this.ctxPotionsSteps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPotionsAdd,
            this.tsmiPotionsRemove,
            this.tsmiPotionsMoveUp,
            this.tsmiPotionsMoveDown});
            this.ctxPotionsSteps.Name = "ctxPotionsSteps";
            this.ctxPotionsSteps.Size = new System.Drawing.Size(159, 100);
            this.ctxPotionsSteps.Opening += new System.ComponentModel.CancelEventHandler(this.ctxPotionsSteps_Opening);
            this.ctxPotionsSteps.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxPotionsSteps_ItemClicked);
            // 
            // tsmiPotionsAdd
            // 
            this.tsmiPotionsAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPotionsAddVigor,
            this.tsmiPotionsAddMendWounds,
            this.tsmiPotionsAddGenericHeal,
            this.tsmiPotionsAddCurePoison});
            this.tsmiPotionsAdd.Name = "tsmiPotionsAdd";
            this.tsmiPotionsAdd.Size = new System.Drawing.Size(158, 24);
            this.tsmiPotionsAdd.Text = "Add";
            // 
            // tsmiPotionsAddVigor
            // 
            this.tsmiPotionsAddVigor.Name = "tsmiPotionsAddVigor";
            this.tsmiPotionsAddVigor.Size = new System.Drawing.Size(189, 26);
            this.tsmiPotionsAddVigor.Text = "Vigor";
            this.tsmiPotionsAddVigor.Click += new System.EventHandler(this.tsmiPotionsAdd_Click);
            // 
            // tsmiPotionsAddMendWounds
            // 
            this.tsmiPotionsAddMendWounds.Name = "tsmiPotionsAddMendWounds";
            this.tsmiPotionsAddMendWounds.Size = new System.Drawing.Size(189, 26);
            this.tsmiPotionsAddMendWounds.Text = "Mend-Wounds";
            this.tsmiPotionsAddMendWounds.Click += new System.EventHandler(this.tsmiPotionsAdd_Click);
            // 
            // tsmiPotionsAddGenericHeal
            // 
            this.tsmiPotionsAddGenericHeal.Name = "tsmiPotionsAddGenericHeal";
            this.tsmiPotionsAddGenericHeal.Size = new System.Drawing.Size(189, 26);
            this.tsmiPotionsAddGenericHeal.Text = "Generic Heal";
            this.tsmiPotionsAddGenericHeal.Click += new System.EventHandler(this.tsmiPotionsAdd_Click);
            // 
            // tsmiPotionsAddCurePoison
            // 
            this.tsmiPotionsAddCurePoison.Name = "tsmiPotionsAddCurePoison";
            this.tsmiPotionsAddCurePoison.Size = new System.Drawing.Size(189, 26);
            this.tsmiPotionsAddCurePoison.Text = "Cure-Poison";
            this.tsmiPotionsAddCurePoison.Click += new System.EventHandler(this.tsmiPotionsAdd_Click);
            // 
            // tsmiPotionsRemove
            // 
            this.tsmiPotionsRemove.Name = "tsmiPotionsRemove";
            this.tsmiPotionsRemove.Size = new System.Drawing.Size(158, 24);
            this.tsmiPotionsRemove.Text = "Remove";
            // 
            // tsmiPotionsMoveUp
            // 
            this.tsmiPotionsMoveUp.Name = "tsmiPotionsMoveUp";
            this.tsmiPotionsMoveUp.Size = new System.Drawing.Size(158, 24);
            this.tsmiPotionsMoveUp.Text = "Move Up";
            // 
            // tsmiPotionsMoveDown
            // 
            this.tsmiPotionsMoveDown.Name = "tsmiPotionsMoveDown";
            this.tsmiPotionsMoveDown.Size = new System.Drawing.Size(158, 24);
            this.tsmiPotionsMoveDown.Text = "Move Down";
            // 
            // chkAutogenerateName
            // 
            this.chkAutogenerateName.AutoSize = true;
            this.chkAutogenerateName.Location = new System.Drawing.Point(358, 286);
            this.chkAutogenerateName.Name = "chkAutogenerateName";
            this.chkAutogenerateName.Size = new System.Drawing.Size(144, 19);
            this.chkAutogenerateName.TabIndex = 3;
            this.chkAutogenerateName.Text = "Autogenerate name?";
            this.chkAutogenerateName.UseVisualStyleBackColor = true;
            this.chkAutogenerateName.CheckedChanged += new System.EventHandler(this.chkAutogenerateName_CheckedChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(356, 309);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(151, 20);
            this.txtName.TabIndex = 4;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(264, 311);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(44, 15);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(576, 338);
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
            this.btnCancel.Location = new System.Drawing.Point(654, 339);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(260, 344);
            this.lblOnKillMonster.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(92, 15);
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
            this.cboOnKillMonster.Location = new System.Drawing.Point(356, 340);
            this.cboOnKillMonster.Margin = new System.Windows.Forms.Padding(2);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(151, 21);
            this.cboOnKillMonster.TabIndex = 146;
            // 
            // frmStrategy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 374);
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
            this.ctxMagicSteps.ResumeLayout(false);
            this.grpMelee.ResumeLayout(false);
            this.grpMelee.PerformLayout();
            this.ctxMeleeSteps.ResumeLayout(false);
            this.grpPotions.ResumeLayout(false);
            this.grpPotions.PerformLayout();
            this.ctxPotionsSteps.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtMagicOnlyWhenStunnedForXMS;
        private System.Windows.Forms.Label lblMagicOnlyWhenStunnedForXMS;
        private System.Windows.Forms.TextBox txtMeleeOnlyWhenStunnedForXMS;
        private System.Windows.Forms.Label lblMeleeOnlyWhenStunnedForXMS;
        private System.Windows.Forms.TextBox txtPotionsOnlyWhenStunnedForXMS;
        private System.Windows.Forms.Label lblPotionsOnlyWhenStunnedForXMS;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicRemove;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicMoveUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicMoveDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddStun;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddOffensiveAuto;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddOffensiveLevel1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddOffensiveLevel2;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddOffensiveLevel3;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddOffensiveLevel4;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddOffensiveLevel5;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicAddVigor;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicAddMendWounds;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicAddGenericHeal;
        private System.Windows.Forms.ToolStripMenuItem tsmiMagicAddCurePoison;
        private System.Windows.Forms.ToolStripMenuItem tsmiMeleeAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddRegularAttack;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddPowerAttack;
        private System.Windows.Forms.ToolStripMenuItem tsmiMeleeRemove;
        private System.Windows.Forms.ToolStripMenuItem tsmiMeleeMoveUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMeleeMoveDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsRemove;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsMoveUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsMoveDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsAddVigor;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsAddMendWounds;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsAddGenericHeal;
        private System.Windows.Forms.ToolStripMenuItem tsmiPotionsAddCurePoison;
    }
}