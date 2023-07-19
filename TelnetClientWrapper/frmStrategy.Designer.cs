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
            this.lblCurrentRealmValue = new System.Windows.Forms.Label();
            this.txtMagicOnlyWhenStunnedForXMS = new System.Windows.Forms.TextBox();
            this.lblMagicOnlyWhenStunnedForXMS = new System.Windows.Forms.Label();
            this.chkMagicEnabled = new System.Windows.Forms.CheckBox();
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.lblMagicFinalAction = new System.Windows.Forms.Label();
            this.cboMagicFinalAction = new System.Windows.Forms.ComboBox();
            this.txtManaPool = new System.Windows.Forms.TextBox();
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblOnKillMonster = new System.Windows.Forms.Label();
            this.cboOnKillMonster = new System.Windows.Forms.ComboBox();
            this.txtMagicLastXStepsRunIndefinitely = new System.Windows.Forms.TextBox();
            this.lblMagicLastXStepsRunIndefinitely = new System.Windows.Forms.Label();
            this.txtMeleeLastXStepsRunIndefinitely = new System.Windows.Forms.TextBox();
            this.lblMeleeLastXStepsRunIndefinitely = new System.Windows.Forms.Label();
            this.txtPotionsLastXStepsRunIndefinitely = new System.Windows.Forms.TextBox();
            this.lblPotionsLastXStepsRunIndefinitely = new System.Windows.Forms.Label();
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
            this.grpMagic.Controls.Add(this.txtMagicLastXStepsRunIndefinitely);
            this.grpMagic.Controls.Add(this.lblMagicLastXStepsRunIndefinitely);
            this.grpMagic.Controls.Add(this.lblCurrentRealmValue);
            this.grpMagic.Controls.Add(this.txtMagicOnlyWhenStunnedForXMS);
            this.grpMagic.Controls.Add(this.lblMagicOnlyWhenStunnedForXMS);
            this.grpMagic.Controls.Add(this.chkMagicEnabled);
            this.grpMagic.Controls.Add(this.lblAutoSpellLevels);
            this.grpMagic.Controls.Add(this.lblMagicFinalAction);
            this.grpMagic.Controls.Add(this.cboMagicFinalAction);
            this.grpMagic.Controls.Add(this.txtManaPool);
            this.grpMagic.Controls.Add(this.lblManaPool);
            this.grpMagic.Controls.Add(this.lstMagicSteps);
            this.grpMagic.Location = new System.Drawing.Point(40, 12);
            this.grpMagic.Margin = new System.Windows.Forms.Padding(4);
            this.grpMagic.Name = "grpMagic";
            this.grpMagic.Padding = new System.Windows.Forms.Padding(4);
            this.grpMagic.Size = new System.Drawing.Size(300, 410);
            this.grpMagic.TabIndex = 0;
            this.grpMagic.TabStop = false;
            this.grpMagic.Text = "Magic";
            // 
            // lblCurrentRealmValue
            // 
            this.lblCurrentRealmValue.BackColor = System.Drawing.Color.White;
            this.lblCurrentRealmValue.Location = new System.Drawing.Point(5, 382);
            this.lblCurrentRealmValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentRealmValue.Name = "lblCurrentRealmValue";
            this.lblCurrentRealmValue.Size = new System.Drawing.Size(291, 18);
            this.lblCurrentRealmValue.TabIndex = 148;
            this.lblCurrentRealmValue.Text = "Realm";
            this.lblCurrentRealmValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtMagicOnlyWhenStunnedForXMS
            // 
            this.txtMagicOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(189, 281);
            this.txtMagicOnlyWhenStunnedForXMS.Margin = new System.Windows.Forms.Padding(4);
            this.txtMagicOnlyWhenStunnedForXMS.Name = "txtMagicOnlyWhenStunnedForXMS";
            this.txtMagicOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(81, 22);
            this.txtMagicOnlyWhenStunnedForXMS.TabIndex = 147;
            // 
            // lblMagicOnlyWhenStunnedForXMS
            // 
            this.lblMagicOnlyWhenStunnedForXMS.AutoSize = true;
            this.lblMagicOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(5, 281);
            this.lblMagicOnlyWhenStunnedForXMS.Name = "lblMagicOnlyWhenStunnedForXMS";
            this.lblMagicOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(171, 16);
            this.lblMagicOnlyWhenStunnedForXMS.TabIndex = 146;
            this.lblMagicOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkMagicEnabled
            // 
            this.chkMagicEnabled.AutoSize = true;
            this.chkMagicEnabled.Location = new System.Drawing.Point(57, 304);
            this.chkMagicEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.chkMagicEnabled.Name = "chkMagicEnabled";
            this.chkMagicEnabled.Size = new System.Drawing.Size(87, 20);
            this.chkMagicEnabled.TabIndex = 145;
            this.chkMagicEnabled.Text = "Enabled?";
            this.chkMagicEnabled.UseVisualStyleBackColor = true;
            // 
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.BackColor = System.Drawing.Color.Silver;
            this.lblAutoSpellLevels.ForeColor = System.Drawing.Color.Black;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(5, 361);
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
            // cboMagicFinalAction
            // 
            this.cboMagicFinalAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMagicFinalAction.FormattingEnabled = true;
            this.cboMagicFinalAction.Items.AddRange(new object[] {
            "",
            "Flee",
            "End Combat"});
            this.cboMagicFinalAction.Location = new System.Drawing.Point(111, 250);
            this.cboMagicFinalAction.Margin = new System.Windows.Forms.Padding(4);
            this.cboMagicFinalAction.Name = "cboMagicFinalAction";
            this.cboMagicFinalAction.Size = new System.Drawing.Size(160, 24);
            this.cboMagicFinalAction.TabIndex = 3;
            // 
            // txtManaPool
            // 
            this.txtManaPool.Location = new System.Drawing.Point(91, 332);
            this.txtManaPool.Margin = new System.Windows.Forms.Padding(4);
            this.txtManaPool.Name = "txtManaPool";
            this.txtManaPool.Size = new System.Drawing.Size(177, 22);
            this.txtManaPool.TabIndex = 9;
            // 
            // lblManaPool
            // 
            this.lblManaPool.AutoSize = true;
            this.lblManaPool.Location = new System.Drawing.Point(3, 337);
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
            this.lstMagicSteps.Size = new System.Drawing.Size(292, 196);
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
            this.ctxMagicSteps.Size = new System.Drawing.Size(211, 128);
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
            this.tsmiMagicRemove.Size = new System.Drawing.Size(210, 24);
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
            this.grpMelee.Controls.Add(this.txtMeleeLastXStepsRunIndefinitely);
            this.grpMelee.Controls.Add(this.lblMeleeLastXStepsRunIndefinitely);
            this.grpMelee.Controls.Add(this.txtMeleeOnlyWhenStunnedForXMS);
            this.grpMelee.Controls.Add(this.lblMeleeOnlyWhenStunnedForXMS);
            this.grpMelee.Controls.Add(this.chkMeleeEnabled);
            this.grpMelee.Controls.Add(this.lblMeleeFinalAction);
            this.grpMelee.Controls.Add(this.cboMeleeFinalAction);
            this.grpMelee.Controls.Add(this.lstMeleeSteps);
            this.grpMelee.Location = new System.Drawing.Point(355, 12);
            this.grpMelee.Margin = new System.Windows.Forms.Padding(4);
            this.grpMelee.Name = "grpMelee";
            this.grpMelee.Padding = new System.Windows.Forms.Padding(4);
            this.grpMelee.Size = new System.Drawing.Size(307, 335);
            this.grpMelee.TabIndex = 1;
            this.grpMelee.TabStop = false;
            this.grpMelee.Text = "Melee";
            // 
            // txtMeleeOnlyWhenStunnedForXMS
            // 
            this.txtMeleeOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(199, 281);
            this.txtMeleeOnlyWhenStunnedForXMS.Margin = new System.Windows.Forms.Padding(4);
            this.txtMeleeOnlyWhenStunnedForXMS.Name = "txtMeleeOnlyWhenStunnedForXMS";
            this.txtMeleeOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(81, 22);
            this.txtMeleeOnlyWhenStunnedForXMS.TabIndex = 149;
            // 
            // lblMeleeOnlyWhenStunnedForXMS
            // 
            this.lblMeleeOnlyWhenStunnedForXMS.AutoSize = true;
            this.lblMeleeOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(15, 284);
            this.lblMeleeOnlyWhenStunnedForXMS.Name = "lblMeleeOnlyWhenStunnedForXMS";
            this.lblMeleeOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(171, 16);
            this.lblMeleeOnlyWhenStunnedForXMS.TabIndex = 148;
            this.lblMeleeOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkMeleeEnabled
            // 
            this.chkMeleeEnabled.AutoSize = true;
            this.chkMeleeEnabled.Location = new System.Drawing.Point(67, 304);
            this.chkMeleeEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.chkMeleeEnabled.Name = "chkMeleeEnabled";
            this.chkMeleeEnabled.Size = new System.Drawing.Size(87, 20);
            this.chkMeleeEnabled.TabIndex = 147;
            this.chkMeleeEnabled.Text = "Enabled?";
            this.chkMeleeEnabled.UseVisualStyleBackColor = true;
            // 
            // lblMeleeFinalAction
            // 
            this.lblMeleeFinalAction.AutoSize = true;
            this.lblMeleeFinalAction.Location = new System.Drawing.Point(15, 254);
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
            this.cboMeleeFinalAction.Location = new System.Drawing.Point(120, 250);
            this.cboMeleeFinalAction.Margin = new System.Windows.Forms.Padding(4);
            this.cboMeleeFinalAction.Name = "cboMeleeFinalAction";
            this.cboMeleeFinalAction.Size = new System.Drawing.Size(160, 24);
            this.cboMeleeFinalAction.TabIndex = 5;
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
            this.lstMeleeSteps.Size = new System.Drawing.Size(299, 196);
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
            this.grpPotions.Controls.Add(this.txtPotionsLastXStepsRunIndefinitely);
            this.grpPotions.Controls.Add(this.lblPotionsLastXStepsRunIndefinitely);
            this.grpPotions.Controls.Add(this.txtPotionsOnlyWhenStunnedForXMS);
            this.grpPotions.Controls.Add(this.lblPotionsOnlyWhenStunnedForXMS);
            this.grpPotions.Controls.Add(this.chkPotionsEnabled);
            this.grpPotions.Controls.Add(this.lblPotionsFinalAction);
            this.grpPotions.Controls.Add(this.cboPotionsFinalAction);
            this.grpPotions.Controls.Add(this.lstPotionsSteps);
            this.grpPotions.Location = new System.Drawing.Point(669, 12);
            this.grpPotions.Margin = new System.Windows.Forms.Padding(4);
            this.grpPotions.Name = "grpPotions";
            this.grpPotions.Padding = new System.Windows.Forms.Padding(4);
            this.grpPotions.Size = new System.Drawing.Size(307, 335);
            this.grpPotions.TabIndex = 2;
            this.grpPotions.TabStop = false;
            this.grpPotions.Text = "Potions";
            // 
            // txtPotionsOnlyWhenStunnedForXMS
            // 
            this.txtPotionsOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(195, 281);
            this.txtPotionsOnlyWhenStunnedForXMS.Margin = new System.Windows.Forms.Padding(4);
            this.txtPotionsOnlyWhenStunnedForXMS.Name = "txtPotionsOnlyWhenStunnedForXMS";
            this.txtPotionsOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(81, 22);
            this.txtPotionsOnlyWhenStunnedForXMS.TabIndex = 151;
            // 
            // lblPotionsOnlyWhenStunnedForXMS
            // 
            this.lblPotionsOnlyWhenStunnedForXMS.AutoSize = true;
            this.lblPotionsOnlyWhenStunnedForXMS.Location = new System.Drawing.Point(11, 283);
            this.lblPotionsOnlyWhenStunnedForXMS.Name = "lblPotionsOnlyWhenStunnedForXMS";
            this.lblPotionsOnlyWhenStunnedForXMS.Size = new System.Drawing.Size(171, 16);
            this.lblPotionsOnlyWhenStunnedForXMS.TabIndex = 150;
            this.lblPotionsOnlyWhenStunnedForXMS.Text = "Only when stunned for X ms:";
            // 
            // chkPotionsEnabled
            // 
            this.chkPotionsEnabled.AutoSize = true;
            this.chkPotionsEnabled.Location = new System.Drawing.Point(67, 304);
            this.chkPotionsEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.chkPotionsEnabled.Name = "chkPotionsEnabled";
            this.chkPotionsEnabled.Size = new System.Drawing.Size(87, 20);
            this.chkPotionsEnabled.TabIndex = 149;
            this.chkPotionsEnabled.Text = "Enabled?";
            this.chkPotionsEnabled.UseVisualStyleBackColor = true;
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
            this.cboPotionsFinalAction.Location = new System.Drawing.Point(111, 250);
            this.cboPotionsFinalAction.Margin = new System.Windows.Forms.Padding(4);
            this.cboPotionsFinalAction.Name = "cboPotionsFinalAction";
            this.cboPotionsFinalAction.Size = new System.Drawing.Size(165, 24);
            this.cboPotionsFinalAction.TabIndex = 7;
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
            this.lstPotionsSteps.Size = new System.Drawing.Size(299, 196);
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
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(475, 354);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 22);
            this.txtName.TabIndex = 4;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(352, 357);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(47, 16);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(768, 383);
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
            this.btnCancel.Location = new System.Drawing.Point(872, 384);
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
            this.lblOnKillMonster.Location = new System.Drawing.Point(347, 390);
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
            this.cboOnKillMonster.Location = new System.Drawing.Point(475, 385);
            this.cboOnKillMonster.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(200, 24);
            this.cboOnKillMonster.TabIndex = 146;
            // 
            // txtMagicLastXStepsRunIndefinitely
            // 
            this.txtMagicLastXStepsRunIndefinitely.Location = new System.Drawing.Point(189, 220);
            this.txtMagicLastXStepsRunIndefinitely.Margin = new System.Windows.Forms.Padding(4);
            this.txtMagicLastXStepsRunIndefinitely.Name = "txtMagicLastXStepsRunIndefinitely";
            this.txtMagicLastXStepsRunIndefinitely.Size = new System.Drawing.Size(81, 22);
            this.txtMagicLastXStepsRunIndefinitely.TabIndex = 150;
            // 
            // lblMagicLastXStepsRunIndefinitely
            // 
            this.lblMagicLastXStepsRunIndefinitely.AutoSize = true;
            this.lblMagicLastXStepsRunIndefinitely.Location = new System.Drawing.Point(3, 223);
            this.lblMagicLastXStepsRunIndefinitely.Name = "lblMagicLastXStepsRunIndefinitely";
            this.lblMagicLastXStepsRunIndefinitely.Size = new System.Drawing.Size(211, 20);
            this.lblMagicLastXStepsRunIndefinitely.TabIndex = 149;
            this.lblMagicLastXStepsRunIndefinitely.Text = "Last X steps run indefinitely:";
            // 
            // txtMeleeLastXStepsRunIndefinitely
            // 
            this.txtMeleeLastXStepsRunIndefinitely.Location = new System.Drawing.Point(199, 220);
            this.txtMeleeLastXStepsRunIndefinitely.Margin = new System.Windows.Forms.Padding(4);
            this.txtMeleeLastXStepsRunIndefinitely.Name = "txtMeleeLastXStepsRunIndefinitely";
            this.txtMeleeLastXStepsRunIndefinitely.Size = new System.Drawing.Size(81, 22);
            this.txtMeleeLastXStepsRunIndefinitely.TabIndex = 151;
            // 
            // lblMeleeLastXStepsRunIndefinitely
            // 
            this.lblMeleeLastXStepsRunIndefinitely.AutoSize = true;
            this.lblMeleeLastXStepsRunIndefinitely.Location = new System.Drawing.Point(15, 223);
            this.lblMeleeLastXStepsRunIndefinitely.Name = "lblMeleeLastXStepsRunIndefinitely";
            this.lblMeleeLastXStepsRunIndefinitely.Size = new System.Drawing.Size(211, 20);
            this.lblMeleeLastXStepsRunIndefinitely.TabIndex = 150;
            this.lblMeleeLastXStepsRunIndefinitely.Text = "Last X steps run indefinitely:";
            // 
            // txtPotionsLastXStepsRunIndefinitely
            // 
            this.txtPotionsLastXStepsRunIndefinitely.Location = new System.Drawing.Point(195, 220);
            this.txtPotionsLastXStepsRunIndefinitely.Margin = new System.Windows.Forms.Padding(4);
            this.txtPotionsLastXStepsRunIndefinitely.Name = "txtPotionsLastXStepsRunIndefinitely";
            this.txtPotionsLastXStepsRunIndefinitely.Size = new System.Drawing.Size(81, 22);
            this.txtPotionsLastXStepsRunIndefinitely.TabIndex = 153;
            // 
            // lblPotionsLastXStepsRunIndefinitely
            // 
            this.lblPotionsLastXStepsRunIndefinitely.AutoSize = true;
            this.lblPotionsLastXStepsRunIndefinitely.Location = new System.Drawing.Point(11, 222);
            this.lblPotionsLastXStepsRunIndefinitely.Name = "lblPotionsLastXStepsRunIndefinitely";
            this.lblPotionsLastXStepsRunIndefinitely.Size = new System.Drawing.Size(211, 20);
            this.lblPotionsLastXStepsRunIndefinitely.TabIndex = 152;
            this.lblPotionsLastXStepsRunIndefinitely.Text = "Last X steps run indefinitely:";
            // 
            // frmStrategy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 427);
            this.Controls.Add(this.cboOnKillMonster);
            this.Controls.Add(this.lblOnKillMonster);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
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
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtManaPool;
        private System.Windows.Forms.Label lblManaPool;
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
        private System.Windows.Forms.Label lblCurrentRealmValue;
        private System.Windows.Forms.TextBox txtMagicLastXStepsRunIndefinitely;
        private System.Windows.Forms.Label lblMagicLastXStepsRunIndefinitely;
        private System.Windows.Forms.TextBox txtMeleeLastXStepsRunIndefinitely;
        private System.Windows.Forms.Label lblMeleeLastXStepsRunIndefinitely;
        private System.Windows.Forms.TextBox txtPotionsLastXStepsRunIndefinitely;
        private System.Windows.Forms.Label lblPotionsLastXStepsRunIndefinitely;
    }
}