namespace IsengardClient
{
    partial class frmMain
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
            this.btnLevel1OffensiveSpell = new System.Windows.Forms.Button();
            this.txtMob = new System.Windows.Forms.TextBox();
            this.ctxMob = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiMob1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMob2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMob3 = new System.Windows.Forms.ToolStripMenuItem();
            this.lblMob = new System.Windows.Forms.Label();
            this.btnLevel2OffensiveSpell = new System.Windows.Forms.Button();
            this.btnFlee = new System.Windows.Forms.Button();
            this.btnDrinkHazy = new System.Windows.Forms.Button();
            this.btnLookAtMob = new System.Windows.Forms.Button();
            this.btnLook = new System.Windows.Forms.Button();
            this.btnCastVigor = new System.Windows.Forms.Button();
            this.btnManashield = new System.Windows.Forms.Button();
            this.btnCastCurePoison = new System.Windows.Forms.Button();
            this.btnTime = new System.Windows.Forms.Button();
            this.btnScore = new System.Windows.Forms.Button();
            this.btnInformation = new System.Windows.Forms.Button();
            this.btnCastProtection = new System.Windows.Forms.Button();
            this.chkIsNight = new System.Windows.Forms.CheckBox();
            this.txtOneOffCommand = new System.Windows.Forms.TextBox();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnAttackMob = new System.Windows.Forms.Button();
            this.txtCurrentRoom = new System.Windows.Forms.TextBox();
            this.lblCurrentRoom = new System.Windows.Forms.Label();
            this.btnDrinkYellow = new System.Windows.Forms.Button();
            this.btnDrinkGreen = new System.Windows.Forms.Button();
            this.txtWeapon = new System.Windows.Forms.TextBox();
            this.lblWeapon = new System.Windows.Forms.Label();
            this.btnWieldWeapon = new System.Windows.Forms.Button();
            this.btnCastBless = new System.Windows.Forms.Button();
            this.btnSet = new System.Windows.Forms.Button();
            this.cboSetOption = new System.Windows.Forms.ComboBox();
            this.chkSetOn = new System.Windows.Forms.CheckBox();
            this.txtWand = new System.Windows.Forms.TextBox();
            this.lblWand = new System.Windows.Forms.Label();
            this.btnUseWandOnMob = new System.Windows.Forms.Button();
            this.btnWho = new System.Windows.Forms.Button();
            this.btnUptime = new System.Windows.Forms.Button();
            this.btnEquipment = new System.Windows.Forms.Button();
            this.btnPowerAttackMob = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.txtSetValue = new System.Windows.Forms.TextBox();
            this.grpRealm = new System.Windows.Forms.GroupBox();
            this.radFire = new System.Windows.Forms.RadioButton();
            this.radWater = new System.Windows.Forms.RadioButton();
            this.radWind = new System.Windows.Forms.RadioButton();
            this.radEarth = new System.Windows.Forms.RadioButton();
            this.treeLocations = new System.Windows.Forms.TreeView();
            this.ctxLocations = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGoToLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.grpLocations = new System.Windows.Forms.GroupBox();
            this.grpOneClickMacros = new System.Windows.Forms.GroupBox();
            this.flpOneClickMacros = new System.Windows.Forms.FlowLayoutPanel();
            this.btnVariables = new System.Windows.Forms.Button();
            this.txtPotion = new System.Windows.Forms.TextBox();
            this.lblPotion = new System.Windows.Forms.Label();
            this.btnRemoveWeapon = new System.Windows.Forms.Button();
            this.btnFumbleMob = new System.Windows.Forms.Button();
            this.btnNortheast = new System.Windows.Forms.Button();
            this.btnNorth = new System.Windows.Forms.Button();
            this.btnNorthwest = new System.Windows.Forms.Button();
            this.btnWest = new System.Windows.Forms.Button();
            this.btnEast = new System.Windows.Forms.Button();
            this.btnSouthwest = new System.Windows.Forms.Button();
            this.btnSouth = new System.Windows.Forms.Button();
            this.btnSoutheast = new System.Windows.Forms.Button();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnClearCurrentLocation = new System.Windows.Forms.Button();
            this.grpSingleMove = new System.Windows.Forms.GroupBox();
            this.btnExitSingleMove = new System.Windows.Forms.Button();
            this.ctxRoomExits = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chkExecuteMove = new System.Windows.Forms.CheckBox();
            this.btnOtherSingleMove = new System.Windows.Forms.Button();
            this.btnDn = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnSetAutoHazyThreshold = new System.Windows.Forms.Button();
            this.txtAutoHazyThreshold = new System.Windows.Forms.TextBox();
            this.lblAutoHazyThreshold = new System.Windows.Forms.Label();
            this.chkAutoHazy = new System.Windows.Forms.CheckBox();
            this.grpSpells = new System.Windows.Forms.GroupBox();
            this.flpSpells = new System.Windows.Forms.FlowLayoutPanel();
            this.txtManashieldTime = new System.Windows.Forms.TextBox();
            this.lblManashieldTime = new System.Windows.Forms.Label();
            this.txtPowerAttackTime = new System.Windows.Forms.TextBox();
            this.lblPowerAttackCooldown = new System.Windows.Forms.Label();
            this.chkAutoMana = new System.Windows.Forms.CheckBox();
            this.txtHitpoints = new System.Windows.Forms.TextBox();
            this.lblHitpoints = new System.Windows.Forms.Label();
            this.cboMaxOffLevel = new System.Windows.Forms.ComboBox();
            this.lblMaxOffensiveLevel = new System.Windows.Forms.Label();
            this.btnManaSet = new System.Windows.Forms.Button();
            this.txtMana = new System.Windows.Forms.TextBox();
            this.lblMana = new System.Windows.Forms.Label();
            this.cboCelduinExpress = new System.Windows.Forms.ComboBox();
            this.lblCelduinExpressLocation = new System.Windows.Forms.Label();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnLevel3OffensiveSpell = new System.Windows.Forms.Button();
            this.btnStunMob = new System.Windows.Forms.Button();
            this.btnCastMend = new System.Windows.Forms.Button();
            this.btnReddishOrange = new System.Windows.Forms.Button();
            this.btnHide = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tabAncillary = new System.Windows.Forms.TabPage();
            this.pnlAncillary = new System.Windows.Forms.Panel();
            this.lblPreferredAlignment = new System.Windows.Forms.Label();
            this.txtPreferredAlignment = new System.Windows.Forms.TextBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.txtLevel = new System.Windows.Forms.TextBox();
            this.lblMacro = new System.Windows.Forms.Label();
            this.cboMacros = new System.Windows.Forms.ComboBox();
            this.btnRunMacro = new System.Windows.Forms.Button();
            this.tabEmotes = new System.Windows.Forms.TabPage();
            this.pnlEmotes = new System.Windows.Forms.Panel();
            this.btnSay = new System.Windows.Forms.Button();
            this.chkShowEmotesWithoutTarget = new System.Windows.Forms.CheckBox();
            this.lblEmoteTarget = new System.Windows.Forms.Label();
            this.txtEmoteTarget = new System.Windows.Forms.TextBox();
            this.lblCommandText = new System.Windows.Forms.Label();
            this.btnEmote = new System.Windows.Forms.Button();
            this.txtCommandText = new System.Windows.Forms.TextBox();
            this.grpEmotes = new System.Windows.Forms.GroupBox();
            this.flpEmotes = new System.Windows.Forms.FlowLayoutPanel();
            this.tabHelp = new System.Windows.Forms.TabPage();
            this.grpHelp = new System.Windows.Forms.GroupBox();
            this.flpHelp = new System.Windows.Forms.FlowLayoutPanel();
            this.tmr = new System.Windows.Forms.Timer(this.components);
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.grpConsole = new System.Windows.Forms.GroupBox();
            this.pnlConsoleHolder = new System.Windows.Forms.Panel();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.ctxConsole = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiClearConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlCommand = new System.Windows.Forms.Panel();
            this.ctxMob.SuspendLayout();
            this.grpRealm.SuspendLayout();
            this.ctxLocations.SuspendLayout();
            this.grpLocations.SuspendLayout();
            this.grpOneClickMacros.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.grpSingleMove.SuspendLayout();
            this.grpSpells.SuspendLayout();
            this.tabAncillary.SuspendLayout();
            this.pnlAncillary.SuspendLayout();
            this.tabEmotes.SuspendLayout();
            this.pnlEmotes.SuspendLayout();
            this.grpEmotes.SuspendLayout();
            this.tabHelp.SuspendLayout();
            this.grpHelp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.grpConsole.SuspendLayout();
            this.pnlConsoleHolder.SuspendLayout();
            this.ctxConsole.SuspendLayout();
            this.pnlCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLevel1OffensiveSpell
            // 
            this.btnLevel1OffensiveSpell.Location = new System.Drawing.Point(64, 200);
            this.btnLevel1OffensiveSpell.Margin = new System.Windows.Forms.Padding(2);
            this.btnLevel1OffensiveSpell.Name = "btnLevel1OffensiveSpell";
            this.btnLevel1OffensiveSpell.Size = new System.Drawing.Size(117, 28);
            this.btnLevel1OffensiveSpell.TabIndex = 0;
            this.btnLevel1OffensiveSpell.Text = "Level 1 Offensive Spell";
            this.btnLevel1OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel1OffensiveSpell.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtMob
            // 
            this.txtMob.ContextMenuStrip = this.ctxMob;
            this.txtMob.Location = new System.Drawing.Point(68, 19);
            this.txtMob.Margin = new System.Windows.Forms.Padding(2);
            this.txtMob.Name = "txtMob";
            this.txtMob.Size = new System.Drawing.Size(179, 20);
            this.txtMob.TabIndex = 4;
            // 
            // ctxMob
            // 
            this.ctxMob.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxMob.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMob1,
            this.tsmiMob2,
            this.tsmiMob3});
            this.ctxMob.Name = "ctxMob";
            this.ctxMob.Size = new System.Drawing.Size(106, 70);
            this.ctxMob.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMob_Opening);
            // 
            // tsmiMob1
            // 
            this.tsmiMob1.Name = "tsmiMob1";
            this.tsmiMob1.Size = new System.Drawing.Size(105, 22);
            this.tsmiMob1.Text = "Mob1";
            this.tsmiMob1.Click += new System.EventHandler(this.tsmiMob_Click);
            // 
            // tsmiMob2
            // 
            this.tsmiMob2.Name = "tsmiMob2";
            this.tsmiMob2.Size = new System.Drawing.Size(105, 22);
            this.tsmiMob2.Text = "Mob2";
            this.tsmiMob2.Click += new System.EventHandler(this.tsmiMob_Click);
            // 
            // tsmiMob3
            // 
            this.tsmiMob3.Name = "tsmiMob3";
            this.tsmiMob3.Size = new System.Drawing.Size(105, 22);
            this.tsmiMob3.Text = "Mob3";
            this.tsmiMob3.Click += new System.EventHandler(this.tsmiMob_Click);
            // 
            // lblMob
            // 
            this.lblMob.Location = new System.Drawing.Point(14, 15);
            this.lblMob.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(52, 27);
            this.lblMob.TabIndex = 3;
            this.lblMob.Text = "Mob:";
            this.lblMob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLevel2OffensiveSpell
            // 
            this.btnLevel2OffensiveSpell.Location = new System.Drawing.Point(64, 233);
            this.btnLevel2OffensiveSpell.Margin = new System.Windows.Forms.Padding(2);
            this.btnLevel2OffensiveSpell.Name = "btnLevel2OffensiveSpell";
            this.btnLevel2OffensiveSpell.Size = new System.Drawing.Size(117, 28);
            this.btnLevel2OffensiveSpell.TabIndex = 5;
            this.btnLevel2OffensiveSpell.Text = "Level 2 Offensive Spell";
            this.btnLevel2OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel2OffensiveSpell.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnFlee
            // 
            this.btnFlee.Location = new System.Drawing.Point(64, 334);
            this.btnFlee.Margin = new System.Windows.Forms.Padding(2);
            this.btnFlee.Name = "btnFlee";
            this.btnFlee.Size = new System.Drawing.Size(117, 28);
            this.btnFlee.TabIndex = 6;
            this.btnFlee.Text = "Flee";
            this.btnFlee.UseVisualStyleBackColor = true;
            this.btnFlee.Click += new System.EventHandler(this.btnFlee_Click);
            // 
            // btnDrinkHazy
            // 
            this.btnDrinkHazy.Location = new System.Drawing.Point(64, 366);
            this.btnDrinkHazy.Margin = new System.Windows.Forms.Padding(2);
            this.btnDrinkHazy.Name = "btnDrinkHazy";
            this.btnDrinkHazy.Size = new System.Drawing.Size(117, 28);
            this.btnDrinkHazy.TabIndex = 7;
            this.btnDrinkHazy.Text = "Drink Hazy";
            this.btnDrinkHazy.UseVisualStyleBackColor = true;
            this.btnDrinkHazy.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLookAtMob
            // 
            this.btnLookAtMob.Location = new System.Drawing.Point(188, 301);
            this.btnLookAtMob.Margin = new System.Windows.Forms.Padding(2);
            this.btnLookAtMob.Name = "btnLookAtMob";
            this.btnLookAtMob.Size = new System.Drawing.Size(117, 28);
            this.btnLookAtMob.TabIndex = 8;
            this.btnLookAtMob.Text = "Look at Mob";
            this.btnLookAtMob.UseVisualStyleBackColor = true;
            this.btnLookAtMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLook
            // 
            this.btnLook.Location = new System.Drawing.Point(308, 200);
            this.btnLook.Margin = new System.Windows.Forms.Padding(2);
            this.btnLook.Name = "btnLook";
            this.btnLook.Size = new System.Drawing.Size(117, 28);
            this.btnLook.TabIndex = 9;
            this.btnLook.Text = "Look";
            this.btnLook.UseVisualStyleBackColor = true;
            this.btnLook.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastVigor
            // 
            this.btnCastVigor.Location = new System.Drawing.Point(429, 167);
            this.btnCastVigor.Margin = new System.Windows.Forms.Padding(2);
            this.btnCastVigor.Name = "btnCastVigor";
            this.btnCastVigor.Size = new System.Drawing.Size(93, 28);
            this.btnCastVigor.TabIndex = 10;
            this.btnCastVigor.Text = "Cast Vigor";
            this.btnCastVigor.UseVisualStyleBackColor = true;
            this.btnCastVigor.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnManashield
            // 
            this.btnManashield.Location = new System.Drawing.Point(64, 300);
            this.btnManashield.Margin = new System.Windows.Forms.Padding(2);
            this.btnManashield.Name = "btnManashield";
            this.btnManashield.Size = new System.Drawing.Size(117, 28);
            this.btnManashield.TabIndex = 11;
            this.btnManashield.Text = "Manashield";
            this.btnManashield.UseVisualStyleBackColor = true;
            this.btnManashield.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastCurePoison
            // 
            this.btnCastCurePoison.Location = new System.Drawing.Point(429, 233);
            this.btnCastCurePoison.Margin = new System.Windows.Forms.Padding(2);
            this.btnCastCurePoison.Name = "btnCastCurePoison";
            this.btnCastCurePoison.Size = new System.Drawing.Size(93, 28);
            this.btnCastCurePoison.TabIndex = 18;
            this.btnCastCurePoison.Text = "Cast Curepoison";
            this.btnCastCurePoison.UseVisualStyleBackColor = true;
            this.btnCastCurePoison.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnTime
            // 
            this.btnTime.Location = new System.Drawing.Point(624, 301);
            this.btnTime.Margin = new System.Windows.Forms.Padding(2);
            this.btnTime.Name = "btnTime";
            this.btnTime.Size = new System.Drawing.Size(102, 28);
            this.btnTime.TabIndex = 19;
            this.btnTime.Text = "Time";
            this.btnTime.UseVisualStyleBackColor = true;
            this.btnTime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnScore
            // 
            this.btnScore.Location = new System.Drawing.Point(624, 334);
            this.btnScore.Margin = new System.Windows.Forms.Padding(2);
            this.btnScore.Name = "btnScore";
            this.btnScore.Size = new System.Drawing.Size(102, 28);
            this.btnScore.TabIndex = 20;
            this.btnScore.Text = "Score";
            this.btnScore.UseVisualStyleBackColor = true;
            this.btnScore.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnInformation
            // 
            this.btnInformation.Location = new System.Drawing.Point(624, 267);
            this.btnInformation.Margin = new System.Windows.Forms.Padding(2);
            this.btnInformation.Name = "btnInformation";
            this.btnInformation.Size = new System.Drawing.Size(102, 28);
            this.btnInformation.TabIndex = 21;
            this.btnInformation.Text = "Information";
            this.btnInformation.UseVisualStyleBackColor = true;
            this.btnInformation.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastProtection
            // 
            this.btnCastProtection.Location = new System.Drawing.Point(429, 266);
            this.btnCastProtection.Margin = new System.Windows.Forms.Padding(2);
            this.btnCastProtection.Name = "btnCastProtection";
            this.btnCastProtection.Size = new System.Drawing.Size(93, 28);
            this.btnCastProtection.TabIndex = 26;
            this.btnCastProtection.Text = "Cast Protection";
            this.btnCastProtection.UseVisualStyleBackColor = true;
            this.btnCastProtection.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // chkIsNight
            // 
            this.chkIsNight.AutoSize = true;
            this.chkIsNight.Location = new System.Drawing.Point(562, 306);
            this.chkIsNight.Margin = new System.Windows.Forms.Padding(2);
            this.chkIsNight.Name = "chkIsNight";
            this.chkIsNight.Size = new System.Drawing.Size(66, 17);
            this.chkIsNight.TabIndex = 28;
            this.chkIsNight.Text = "Is night?";
            this.chkIsNight.UseVisualStyleBackColor = true;
            this.chkIsNight.CheckedChanged += new System.EventHandler(this.chkIsNight_CheckedChanged);
            // 
            // txtOneOffCommand
            // 
            this.txtOneOffCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOneOffCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOneOffCommand.Location = new System.Drawing.Point(0, 0);
            this.txtOneOffCommand.Margin = new System.Windows.Forms.Padding(0);
            this.txtOneOffCommand.Name = "txtOneOffCommand";
            this.txtOneOffCommand.Size = new System.Drawing.Size(724, 26);
            this.txtOneOffCommand.TabIndex = 29;
            this.txtOneOffCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOneOffCommand_KeyPress);
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(624, 233);
            this.btnInventory.Margin = new System.Windows.Forms.Padding(2);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(102, 28);
            this.btnInventory.TabIndex = 31;
            this.btnInventory.Text = "Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(611, 520);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(2);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(116, 21);
            this.btnAbort.TabIndex = 33;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnAttackMob
            // 
            this.btnAttackMob.Location = new System.Drawing.Point(186, 233);
            this.btnAttackMob.Margin = new System.Windows.Forms.Padding(2);
            this.btnAttackMob.Name = "btnAttackMob";
            this.btnAttackMob.Size = new System.Drawing.Size(117, 28);
            this.btnAttackMob.TabIndex = 35;
            this.btnAttackMob.Text = "Attack Mob";
            this.btnAttackMob.UseVisualStyleBackColor = true;
            this.btnAttackMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtCurrentRoom
            // 
            this.txtCurrentRoom.Enabled = false;
            this.txtCurrentRoom.Location = new System.Drawing.Point(512, 470);
            this.txtCurrentRoom.Margin = new System.Windows.Forms.Padding(2);
            this.txtCurrentRoom.Name = "txtCurrentRoom";
            this.txtCurrentRoom.Size = new System.Drawing.Size(216, 20);
            this.txtCurrentRoom.TabIndex = 37;
            // 
            // lblCurrentRoom
            // 
            this.lblCurrentRoom.AutoSize = true;
            this.lblCurrentRoom.Location = new System.Drawing.Point(442, 473);
            this.lblCurrentRoom.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrentRoom.Name = "lblCurrentRoom";
            this.lblCurrentRoom.Size = new System.Drawing.Size(70, 13);
            this.lblCurrentRoom.TabIndex = 38;
            this.lblCurrentRoom.Text = "Current room:";
            // 
            // btnDrinkYellow
            // 
            this.btnDrinkYellow.Location = new System.Drawing.Point(527, 167);
            this.btnDrinkYellow.Margin = new System.Windows.Forms.Padding(2);
            this.btnDrinkYellow.Name = "btnDrinkYellow";
            this.btnDrinkYellow.Size = new System.Drawing.Size(93, 28);
            this.btnDrinkYellow.TabIndex = 39;
            this.btnDrinkYellow.Text = "Yellow pot";
            this.btnDrinkYellow.UseVisualStyleBackColor = true;
            this.btnDrinkYellow.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnDrinkGreen
            // 
            this.btnDrinkGreen.Location = new System.Drawing.Point(527, 233);
            this.btnDrinkGreen.Margin = new System.Windows.Forms.Padding(2);
            this.btnDrinkGreen.Name = "btnDrinkGreen";
            this.btnDrinkGreen.Size = new System.Drawing.Size(93, 28);
            this.btnDrinkGreen.TabIndex = 40;
            this.btnDrinkGreen.Text = "Green pot";
            this.btnDrinkGreen.UseVisualStyleBackColor = true;
            this.btnDrinkGreen.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtWeapon
            // 
            this.txtWeapon.Location = new System.Drawing.Point(68, 41);
            this.txtWeapon.Margin = new System.Windows.Forms.Padding(2);
            this.txtWeapon.Name = "txtWeapon";
            this.txtWeapon.Size = new System.Drawing.Size(179, 20);
            this.txtWeapon.TabIndex = 42;
            this.txtWeapon.TextChanged += new System.EventHandler(this.txtWeapon_TextChanged);
            // 
            // lblWeapon
            // 
            this.lblWeapon.Location = new System.Drawing.Point(14, 39);
            this.lblWeapon.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(51, 27);
            this.lblWeapon.TabIndex = 41;
            this.lblWeapon.Text = "Weapon:";
            this.lblWeapon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnWieldWeapon
            // 
            this.btnWieldWeapon.Location = new System.Drawing.Point(308, 302);
            this.btnWieldWeapon.Margin = new System.Windows.Forms.Padding(2);
            this.btnWieldWeapon.Name = "btnWieldWeapon";
            this.btnWieldWeapon.Size = new System.Drawing.Size(117, 28);
            this.btnWieldWeapon.TabIndex = 43;
            this.btnWieldWeapon.Text = "Wield Weapon";
            this.btnWieldWeapon.UseVisualStyleBackColor = true;
            this.btnWieldWeapon.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastBless
            // 
            this.btnCastBless.Location = new System.Drawing.Point(429, 301);
            this.btnCastBless.Margin = new System.Windows.Forms.Padding(2);
            this.btnCastBless.Name = "btnCastBless";
            this.btnCastBless.Size = new System.Drawing.Size(93, 28);
            this.btnCastBless.TabIndex = 44;
            this.btnCastBless.Text = "Cast Bless";
            this.btnCastBless.UseVisualStyleBackColor = true;
            this.btnCastBless.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(10, 13);
            this.btnSet.Margin = new System.Windows.Forms.Padding(2);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(55, 24);
            this.btnSet.TabIndex = 45;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // cboSetOption
            // 
            this.cboSetOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSetOption.FormattingEnabled = true;
            this.cboSetOption.Items.AddRange(new object[] {
            "compact",
            "roomname",
            "short",
            "long",
            "noauto",
            "wimpy"});
            this.cboSetOption.Location = new System.Drawing.Point(72, 14);
            this.cboSetOption.Margin = new System.Windows.Forms.Padding(2);
            this.cboSetOption.Name = "cboSetOption";
            this.cboSetOption.Size = new System.Drawing.Size(124, 21);
            this.cboSetOption.TabIndex = 46;
            this.cboSetOption.SelectedIndexChanged += new System.EventHandler(this.cboSetOption_SelectedIndexChanged);
            // 
            // chkSetOn
            // 
            this.chkSetOn.AutoSize = true;
            this.chkSetOn.Location = new System.Drawing.Point(200, 17);
            this.chkSetOn.Margin = new System.Windows.Forms.Padding(2);
            this.chkSetOn.Name = "chkSetOn";
            this.chkSetOn.Size = new System.Drawing.Size(46, 17);
            this.chkSetOn.TabIndex = 47;
            this.chkSetOn.Text = "On?";
            this.chkSetOn.UseVisualStyleBackColor = true;
            this.chkSetOn.CheckedChanged += new System.EventHandler(this.chkSetOn_CheckedChanged);
            // 
            // txtWand
            // 
            this.txtWand.Location = new System.Drawing.Point(68, 65);
            this.txtWand.Margin = new System.Windows.Forms.Padding(2);
            this.txtWand.Name = "txtWand";
            this.txtWand.Size = new System.Drawing.Size(179, 20);
            this.txtWand.TabIndex = 49;
            // 
            // lblWand
            // 
            this.lblWand.Location = new System.Drawing.Point(14, 59);
            this.lblWand.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWand.Name = "lblWand";
            this.lblWand.Size = new System.Drawing.Size(51, 27);
            this.lblWand.TabIndex = 48;
            this.lblWand.Text = "Wand:";
            this.lblWand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnUseWandOnMob
            // 
            this.btnUseWandOnMob.Location = new System.Drawing.Point(186, 334);
            this.btnUseWandOnMob.Margin = new System.Windows.Forms.Padding(2);
            this.btnUseWandOnMob.Name = "btnUseWandOnMob";
            this.btnUseWandOnMob.Size = new System.Drawing.Size(117, 28);
            this.btnUseWandOnMob.TabIndex = 50;
            this.btnUseWandOnMob.Text = "Wand Mob";
            this.btnUseWandOnMob.UseVisualStyleBackColor = true;
            this.btnUseWandOnMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnWho
            // 
            this.btnWho.Location = new System.Drawing.Point(624, 167);
            this.btnWho.Margin = new System.Windows.Forms.Padding(2);
            this.btnWho.Name = "btnWho";
            this.btnWho.Size = new System.Drawing.Size(102, 28);
            this.btnWho.TabIndex = 51;
            this.btnWho.Text = "Who";
            this.btnWho.UseVisualStyleBackColor = true;
            this.btnWho.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnUptime
            // 
            this.btnUptime.Location = new System.Drawing.Point(624, 137);
            this.btnUptime.Margin = new System.Windows.Forms.Padding(2);
            this.btnUptime.Name = "btnUptime";
            this.btnUptime.Size = new System.Drawing.Size(102, 24);
            this.btnUptime.TabIndex = 52;
            this.btnUptime.Text = "Uptime";
            this.btnUptime.UseVisualStyleBackColor = true;
            this.btnUptime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnEquipment
            // 
            this.btnEquipment.Location = new System.Drawing.Point(624, 366);
            this.btnEquipment.Margin = new System.Windows.Forms.Padding(2);
            this.btnEquipment.Name = "btnEquipment";
            this.btnEquipment.Size = new System.Drawing.Size(102, 28);
            this.btnEquipment.TabIndex = 53;
            this.btnEquipment.Text = "Equipment";
            this.btnEquipment.UseVisualStyleBackColor = true;
            this.btnEquipment.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnPowerAttackMob
            // 
            this.btnPowerAttackMob.Location = new System.Drawing.Point(188, 267);
            this.btnPowerAttackMob.Margin = new System.Windows.Forms.Padding(2);
            this.btnPowerAttackMob.Name = "btnPowerAttackMob";
            this.btnPowerAttackMob.Size = new System.Drawing.Size(117, 28);
            this.btnPowerAttackMob.TabIndex = 54;
            this.btnPowerAttackMob.Text = "Power Attack Mob";
            this.btnPowerAttackMob.UseVisualStyleBackColor = true;
            this.btnPowerAttackMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(624, 201);
            this.btnQuit.Margin = new System.Windows.Forms.Padding(2);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(102, 28);
            this.btnQuit.TabIndex = 55;
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // txtSetValue
            // 
            this.txtSetValue.Location = new System.Drawing.Point(244, 15);
            this.txtSetValue.Margin = new System.Windows.Forms.Padding(2);
            this.txtSetValue.Name = "txtSetValue";
            this.txtSetValue.Size = new System.Drawing.Size(89, 20);
            this.txtSetValue.TabIndex = 56;
            // 
            // grpRealm
            // 
            this.grpRealm.Controls.Add(this.radFire);
            this.grpRealm.Controls.Add(this.radWater);
            this.grpRealm.Controls.Add(this.radWind);
            this.grpRealm.Controls.Add(this.radEarth);
            this.grpRealm.Location = new System.Drawing.Point(64, 410);
            this.grpRealm.Name = "grpRealm";
            this.grpRealm.Size = new System.Drawing.Size(217, 46);
            this.grpRealm.TabIndex = 62;
            this.grpRealm.TabStop = false;
            this.grpRealm.Text = "Realm";
            // 
            // radFire
            // 
            this.radFire.AutoSize = true;
            this.radFire.Location = new System.Drawing.Point(173, 19);
            this.radFire.Name = "radFire";
            this.radFire.Size = new System.Drawing.Size(42, 17);
            this.radFire.TabIndex = 3;
            this.radFire.TabStop = true;
            this.radFire.Text = "Fire";
            this.radFire.UseVisualStyleBackColor = true;
            // 
            // radWater
            // 
            this.radWater.AutoSize = true;
            this.radWater.Location = new System.Drawing.Point(113, 19);
            this.radWater.Name = "radWater";
            this.radWater.Size = new System.Drawing.Size(54, 17);
            this.radWater.TabIndex = 2;
            this.radWater.TabStop = true;
            this.radWater.Text = "Water";
            this.radWater.UseVisualStyleBackColor = true;
            // 
            // radWind
            // 
            this.radWind.AutoSize = true;
            this.radWind.Checked = true;
            this.radWind.Location = new System.Drawing.Point(62, 19);
            this.radWind.Name = "radWind";
            this.radWind.Size = new System.Drawing.Size(50, 17);
            this.radWind.TabIndex = 1;
            this.radWind.TabStop = true;
            this.radWind.Text = "Wind";
            this.radWind.UseVisualStyleBackColor = true;
            // 
            // radEarth
            // 
            this.radEarth.AutoSize = true;
            this.radEarth.Location = new System.Drawing.Point(6, 19);
            this.radEarth.Name = "radEarth";
            this.radEarth.Size = new System.Drawing.Size(50, 17);
            this.radEarth.TabIndex = 0;
            this.radEarth.TabStop = true;
            this.radEarth.Text = "Earth";
            this.radEarth.UseVisualStyleBackColor = true;
            // 
            // treeLocations
            // 
            this.treeLocations.ContextMenuStrip = this.ctxLocations;
            this.treeLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeLocations.HideSelection = false;
            this.treeLocations.Location = new System.Drawing.Point(3, 16);
            this.treeLocations.Name = "treeLocations";
            this.treeLocations.Size = new System.Drawing.Size(277, 862);
            this.treeLocations.TabIndex = 63;
            this.treeLocations.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeLocations_NodeMouseClick);
            // 
            // ctxLocations
            // 
            this.ctxLocations.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxLocations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGoToLocation,
            this.tsmiSetLocation});
            this.ctxLocations.Name = "ctxLocations";
            this.ctxLocations.Size = new System.Drawing.Size(181, 70);
            this.ctxLocations.Opening += new System.ComponentModel.CancelEventHandler(this.ctxLocations_Opening);
            this.ctxLocations.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxLocations_ItemClicked);
            // 
            // tsmiSetLocation
            // 
            this.tsmiSetLocation.Name = "tsmiSetLocation";
            this.tsmiSetLocation.Size = new System.Drawing.Size(180, 22);
            this.tsmiSetLocation.Text = "Set";
            // 
            // tsmiGoToLocation
            // 
            this.tsmiGoToLocation.Name = "tsmiGoToLocation";
            this.tsmiGoToLocation.Size = new System.Drawing.Size(180, 22);
            this.tsmiGoToLocation.Text = "Go";
            // 
            // grpLocations
            // 
            this.grpLocations.Controls.Add(this.treeLocations);
            this.grpLocations.Location = new System.Drawing.Point(731, 13);
            this.grpLocations.Name = "grpLocations";
            this.grpLocations.Size = new System.Drawing.Size(283, 881);
            this.grpLocations.TabIndex = 64;
            this.grpLocations.TabStop = false;
            this.grpLocations.Text = "Locations";
            // 
            // grpOneClickMacros
            // 
            this.grpOneClickMacros.Controls.Add(this.flpOneClickMacros);
            this.grpOneClickMacros.Location = new System.Drawing.Point(4, 495);
            this.grpOneClickMacros.Name = "grpOneClickMacros";
            this.grpOneClickMacros.Size = new System.Drawing.Size(601, 111);
            this.grpOneClickMacros.TabIndex = 65;
            this.grpOneClickMacros.TabStop = false;
            this.grpOneClickMacros.Text = "One Click Macros";
            // 
            // flpOneClickMacros
            // 
            this.flpOneClickMacros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpOneClickMacros.Location = new System.Drawing.Point(3, 16);
            this.flpOneClickMacros.Name = "flpOneClickMacros";
            this.flpOneClickMacros.Size = new System.Drawing.Size(595, 92);
            this.flpOneClickMacros.TabIndex = 0;
            // 
            // btnVariables
            // 
            this.btnVariables.Location = new System.Drawing.Point(527, 137);
            this.btnVariables.Margin = new System.Windows.Forms.Padding(2);
            this.btnVariables.Name = "btnVariables";
            this.btnVariables.Size = new System.Drawing.Size(93, 24);
            this.btnVariables.TabIndex = 66;
            this.btnVariables.Text = "Variables";
            this.btnVariables.UseVisualStyleBackColor = true;
            this.btnVariables.Click += new System.EventHandler(this.btnVariables_Click);
            // 
            // txtPotion
            // 
            this.txtPotion.Location = new System.Drawing.Point(68, 87);
            this.txtPotion.Margin = new System.Windows.Forms.Padding(2);
            this.txtPotion.Name = "txtPotion";
            this.txtPotion.Size = new System.Drawing.Size(179, 20);
            this.txtPotion.TabIndex = 68;
            // 
            // lblPotion
            // 
            this.lblPotion.Location = new System.Drawing.Point(14, 81);
            this.lblPotion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPotion.Name = "lblPotion";
            this.lblPotion.Size = new System.Drawing.Size(51, 27);
            this.lblPotion.TabIndex = 67;
            this.lblPotion.Text = "Potion:";
            this.lblPotion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemoveWeapon
            // 
            this.btnRemoveWeapon.Location = new System.Drawing.Point(308, 334);
            this.btnRemoveWeapon.Margin = new System.Windows.Forms.Padding(2);
            this.btnRemoveWeapon.Name = "btnRemoveWeapon";
            this.btnRemoveWeapon.Size = new System.Drawing.Size(117, 28);
            this.btnRemoveWeapon.TabIndex = 69;
            this.btnRemoveWeapon.Text = "Remove Weapon";
            this.btnRemoveWeapon.UseVisualStyleBackColor = true;
            this.btnRemoveWeapon.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnFumbleMob
            // 
            this.btnFumbleMob.Location = new System.Drawing.Point(188, 366);
            this.btnFumbleMob.Margin = new System.Windows.Forms.Padding(2);
            this.btnFumbleMob.Name = "btnFumbleMob";
            this.btnFumbleMob.Size = new System.Drawing.Size(117, 28);
            this.btnFumbleMob.TabIndex = 70;
            this.btnFumbleMob.Text = "Fumble Mob";
            this.btnFumbleMob.UseVisualStyleBackColor = true;
            this.btnFumbleMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnNortheast
            // 
            this.btnNortheast.Location = new System.Drawing.Point(90, 40);
            this.btnNortheast.Margin = new System.Windows.Forms.Padding(2);
            this.btnNortheast.Name = "btnNortheast";
            this.btnNortheast.Size = new System.Drawing.Size(34, 23);
            this.btnNortheast.TabIndex = 71;
            this.btnNortheast.Tag = "northeast";
            this.btnNortheast.Text = "NE";
            this.btnNortheast.UseVisualStyleBackColor = true;
            this.btnNortheast.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnNorth
            // 
            this.btnNorth.Location = new System.Drawing.Point(51, 40);
            this.btnNorth.Margin = new System.Windows.Forms.Padding(2);
            this.btnNorth.Name = "btnNorth";
            this.btnNorth.Size = new System.Drawing.Size(34, 23);
            this.btnNorth.TabIndex = 72;
            this.btnNorth.Tag = "north";
            this.btnNorth.Text = "N";
            this.btnNorth.UseVisualStyleBackColor = true;
            this.btnNorth.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnNorthwest
            // 
            this.btnNorthwest.Location = new System.Drawing.Point(14, 40);
            this.btnNorthwest.Margin = new System.Windows.Forms.Padding(2);
            this.btnNorthwest.Name = "btnNorthwest";
            this.btnNorthwest.Size = new System.Drawing.Size(34, 23);
            this.btnNorthwest.TabIndex = 73;
            this.btnNorthwest.Tag = "northwest";
            this.btnNorthwest.Text = "NW";
            this.btnNorthwest.UseVisualStyleBackColor = true;
            this.btnNorthwest.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnWest
            // 
            this.btnWest.Location = new System.Drawing.Point(14, 66);
            this.btnWest.Margin = new System.Windows.Forms.Padding(2);
            this.btnWest.Name = "btnWest";
            this.btnWest.Size = new System.Drawing.Size(34, 23);
            this.btnWest.TabIndex = 75;
            this.btnWest.Tag = "west";
            this.btnWest.Text = "W";
            this.btnWest.UseVisualStyleBackColor = true;
            this.btnWest.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnEast
            // 
            this.btnEast.Location = new System.Drawing.Point(90, 66);
            this.btnEast.Margin = new System.Windows.Forms.Padding(2);
            this.btnEast.Name = "btnEast";
            this.btnEast.Size = new System.Drawing.Size(34, 23);
            this.btnEast.TabIndex = 74;
            this.btnEast.Tag = "east";
            this.btnEast.Text = "E";
            this.btnEast.UseVisualStyleBackColor = true;
            this.btnEast.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnSouthwest
            // 
            this.btnSouthwest.Location = new System.Drawing.Point(14, 93);
            this.btnSouthwest.Margin = new System.Windows.Forms.Padding(2);
            this.btnSouthwest.Name = "btnSouthwest";
            this.btnSouthwest.Size = new System.Drawing.Size(34, 23);
            this.btnSouthwest.TabIndex = 78;
            this.btnSouthwest.Tag = "southwest";
            this.btnSouthwest.Text = "SW";
            this.btnSouthwest.UseVisualStyleBackColor = true;
            this.btnSouthwest.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnSouth
            // 
            this.btnSouth.Location = new System.Drawing.Point(51, 93);
            this.btnSouth.Margin = new System.Windows.Forms.Padding(2);
            this.btnSouth.Name = "btnSouth";
            this.btnSouth.Size = new System.Drawing.Size(34, 23);
            this.btnSouth.TabIndex = 77;
            this.btnSouth.Tag = "south";
            this.btnSouth.Text = "S";
            this.btnSouth.UseVisualStyleBackColor = true;
            this.btnSouth.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnSoutheast
            // 
            this.btnSoutheast.Location = new System.Drawing.Point(90, 93);
            this.btnSoutheast.Margin = new System.Windows.Forms.Padding(2);
            this.btnSoutheast.Name = "btnSoutheast";
            this.btnSoutheast.Size = new System.Drawing.Size(34, 23);
            this.btnSoutheast.TabIndex = 76;
            this.btnSoutheast.Tag = "southeast";
            this.btnSoutheast.Text = "SE";
            this.btnSoutheast.UseVisualStyleBackColor = true;
            this.btnSoutheast.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabMain);
            this.tcMain.Controls.Add(this.tabAncillary);
            this.tcMain.Controls.Add(this.tabEmotes);
            this.tcMain.Controls.Add(this.tabHelp);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Margin = new System.Windows.Forms.Padding(2);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1029, 930);
            this.tcMain.TabIndex = 79;
            this.tcMain.Selected += new System.Windows.Forms.TabControlEventHandler(this.tcMain_Selected);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.pnlMain);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Margin = new System.Windows.Forms.Padding(2);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(2);
            this.tabMain.Size = new System.Drawing.Size(1021, 904);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnClearCurrentLocation);
            this.pnlMain.Controls.Add(this.grpSingleMove);
            this.pnlMain.Controls.Add(this.btnSetAutoHazyThreshold);
            this.pnlMain.Controls.Add(this.txtAutoHazyThreshold);
            this.pnlMain.Controls.Add(this.lblAutoHazyThreshold);
            this.pnlMain.Controls.Add(this.chkAutoHazy);
            this.pnlMain.Controls.Add(this.grpSpells);
            this.pnlMain.Controls.Add(this.txtManashieldTime);
            this.pnlMain.Controls.Add(this.lblManashieldTime);
            this.pnlMain.Controls.Add(this.txtPowerAttackTime);
            this.pnlMain.Controls.Add(this.lblPowerAttackCooldown);
            this.pnlMain.Controls.Add(this.chkAutoMana);
            this.pnlMain.Controls.Add(this.txtHitpoints);
            this.pnlMain.Controls.Add(this.lblHitpoints);
            this.pnlMain.Controls.Add(this.cboMaxOffLevel);
            this.pnlMain.Controls.Add(this.lblMaxOffensiveLevel);
            this.pnlMain.Controls.Add(this.btnManaSet);
            this.pnlMain.Controls.Add(this.txtMana);
            this.pnlMain.Controls.Add(this.lblMana);
            this.pnlMain.Controls.Add(this.cboCelduinExpress);
            this.pnlMain.Controls.Add(this.lblCelduinExpressLocation);
            this.pnlMain.Controls.Add(this.btnRemoveAll);
            this.pnlMain.Controls.Add(this.btnLevel3OffensiveSpell);
            this.pnlMain.Controls.Add(this.btnStunMob);
            this.pnlMain.Controls.Add(this.btnCastMend);
            this.pnlMain.Controls.Add(this.btnReddishOrange);
            this.pnlMain.Controls.Add(this.btnHide);
            this.pnlMain.Controls.Add(this.btnSearch);
            this.pnlMain.Controls.Add(this.btnLevel1OffensiveSpell);
            this.pnlMain.Controls.Add(this.lblMob);
            this.pnlMain.Controls.Add(this.txtMob);
            this.pnlMain.Controls.Add(this.btnLevel2OffensiveSpell);
            this.pnlMain.Controls.Add(this.btnFlee);
            this.pnlMain.Controls.Add(this.btnDrinkHazy);
            this.pnlMain.Controls.Add(this.btnLookAtMob);
            this.pnlMain.Controls.Add(this.btnFumbleMob);
            this.pnlMain.Controls.Add(this.btnLook);
            this.pnlMain.Controls.Add(this.btnRemoveWeapon);
            this.pnlMain.Controls.Add(this.btnCastVigor);
            this.pnlMain.Controls.Add(this.txtPotion);
            this.pnlMain.Controls.Add(this.btnManashield);
            this.pnlMain.Controls.Add(this.lblPotion);
            this.pnlMain.Controls.Add(this.btnCastCurePoison);
            this.pnlMain.Controls.Add(this.btnVariables);
            this.pnlMain.Controls.Add(this.btnTime);
            this.pnlMain.Controls.Add(this.grpOneClickMacros);
            this.pnlMain.Controls.Add(this.btnScore);
            this.pnlMain.Controls.Add(this.grpLocations);
            this.pnlMain.Controls.Add(this.btnInformation);
            this.pnlMain.Controls.Add(this.grpRealm);
            this.pnlMain.Controls.Add(this.btnCastProtection);
            this.pnlMain.Controls.Add(this.chkIsNight);
            this.pnlMain.Controls.Add(this.btnQuit);
            this.pnlMain.Controls.Add(this.btnPowerAttackMob);
            this.pnlMain.Controls.Add(this.btnEquipment);
            this.pnlMain.Controls.Add(this.btnInventory);
            this.pnlMain.Controls.Add(this.btnUptime);
            this.pnlMain.Controls.Add(this.btnWho);
            this.pnlMain.Controls.Add(this.btnAbort);
            this.pnlMain.Controls.Add(this.btnUseWandOnMob);
            this.pnlMain.Controls.Add(this.txtWand);
            this.pnlMain.Controls.Add(this.btnAttackMob);
            this.pnlMain.Controls.Add(this.lblWand);
            this.pnlMain.Controls.Add(this.txtCurrentRoom);
            this.pnlMain.Controls.Add(this.btnCastBless);
            this.pnlMain.Controls.Add(this.lblCurrentRoom);
            this.pnlMain.Controls.Add(this.btnWieldWeapon);
            this.pnlMain.Controls.Add(this.btnDrinkYellow);
            this.pnlMain.Controls.Add(this.txtWeapon);
            this.pnlMain.Controls.Add(this.btnDrinkGreen);
            this.pnlMain.Controls.Add(this.lblWeapon);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(2, 2);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(2);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1017, 900);
            this.pnlMain.TabIndex = 0;
            // 
            // btnClearCurrentLocation
            // 
            this.btnClearCurrentLocation.Location = new System.Drawing.Point(611, 495);
            this.btnClearCurrentLocation.Margin = new System.Windows.Forms.Padding(2);
            this.btnClearCurrentLocation.Name = "btnClearCurrentLocation";
            this.btnClearCurrentLocation.Size = new System.Drawing.Size(116, 21);
            this.btnClearCurrentLocation.TabIndex = 118;
            this.btnClearCurrentLocation.Text = "Clear Location";
            this.btnClearCurrentLocation.UseVisualStyleBackColor = true;
            this.btnClearCurrentLocation.Click += new System.EventHandler(this.btnClearCurrentLocation_Click);
            // 
            // grpSingleMove
            // 
            this.grpSingleMove.Controls.Add(this.btnExitSingleMove);
            this.grpSingleMove.Controls.Add(this.chkExecuteMove);
            this.grpSingleMove.Controls.Add(this.btnOtherSingleMove);
            this.grpSingleMove.Controls.Add(this.btnNortheast);
            this.grpSingleMove.Controls.Add(this.btnNorth);
            this.grpSingleMove.Controls.Add(this.btnNorthwest);
            this.grpSingleMove.Controls.Add(this.btnEast);
            this.grpSingleMove.Controls.Add(this.btnDn);
            this.grpSingleMove.Controls.Add(this.btnWest);
            this.grpSingleMove.Controls.Add(this.btnUp);
            this.grpSingleMove.Controls.Add(this.btnSoutheast);
            this.grpSingleMove.Controls.Add(this.btnSouth);
            this.grpSingleMove.Controls.Add(this.btnSouthwest);
            this.grpSingleMove.Location = new System.Drawing.Point(542, 612);
            this.grpSingleMove.Name = "grpSingleMove";
            this.grpSingleMove.Size = new System.Drawing.Size(181, 168);
            this.grpSingleMove.TabIndex = 117;
            this.grpSingleMove.TabStop = false;
            this.grpSingleMove.Text = "Single Move";
            // 
            // btnExitSingleMove
            // 
            this.btnExitSingleMove.ContextMenuStrip = this.ctxRoomExits;
            this.btnExitSingleMove.Location = new System.Drawing.Point(64, 131);
            this.btnExitSingleMove.Margin = new System.Windows.Forms.Padding(2);
            this.btnExitSingleMove.Name = "btnExitSingleMove";
            this.btnExitSingleMove.Size = new System.Drawing.Size(46, 23);
            this.btnExitSingleMove.TabIndex = 113;
            this.btnExitSingleMove.Tag = "";
            this.btnExitSingleMove.Text = "Exit";
            this.btnExitSingleMove.UseVisualStyleBackColor = true;
            this.btnExitSingleMove.Click += new System.EventHandler(this.btnExitSingleMove_Click);
            // 
            // ctxRoomExits
            // 
            this.ctxRoomExits.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxRoomExits.Name = "ctxOneClickMacro";
            this.ctxRoomExits.Size = new System.Drawing.Size(61, 4);
            this.ctxRoomExits.Opening += new System.ComponentModel.CancelEventHandler(this.ctxRoomExits_Opening);
            this.ctxRoomExits.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxRoomExits_ItemClicked);
            // 
            // chkExecuteMove
            // 
            this.chkExecuteMove.AutoSize = true;
            this.chkExecuteMove.Checked = true;
            this.chkExecuteMove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExecuteMove.Location = new System.Drawing.Point(6, 19);
            this.chkExecuteMove.Name = "chkExecuteMove";
            this.chkExecuteMove.Size = new System.Drawing.Size(95, 17);
            this.chkExecuteMove.TabIndex = 0;
            this.chkExecuteMove.Text = "Execute Move";
            this.chkExecuteMove.UseVisualStyleBackColor = true;
            this.chkExecuteMove.CheckedChanged += new System.EventHandler(this.chkExecuteMove_CheckedChanged);
            // 
            // btnOtherSingleMove
            // 
            this.btnOtherSingleMove.Location = new System.Drawing.Point(14, 131);
            this.btnOtherSingleMove.Margin = new System.Windows.Forms.Padding(2);
            this.btnOtherSingleMove.Name = "btnOtherSingleMove";
            this.btnOtherSingleMove.Size = new System.Drawing.Size(46, 23);
            this.btnOtherSingleMove.TabIndex = 112;
            this.btnOtherSingleMove.Tag = "";
            this.btnOtherSingleMove.Text = "Other";
            this.btnOtherSingleMove.UseVisualStyleBackColor = true;
            this.btnOtherSingleMove.Click += new System.EventHandler(this.btnOtherSingleMove_Click);
            // 
            // btnDn
            // 
            this.btnDn.Location = new System.Drawing.Point(128, 76);
            this.btnDn.Margin = new System.Windows.Forms.Padding(2);
            this.btnDn.Name = "btnDn";
            this.btnDn.Size = new System.Drawing.Size(34, 23);
            this.btnDn.TabIndex = 111;
            this.btnDn.Tag = "down";
            this.btnDn.Text = "Dn";
            this.btnDn.UseVisualStyleBackColor = true;
            this.btnDn.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(128, 50);
            this.btnUp.Margin = new System.Windows.Forms.Padding(2);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(34, 23);
            this.btnUp.TabIndex = 110;
            this.btnUp.Tag = "up";
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnSetAutoHazyThreshold
            // 
            this.btnSetAutoHazyThreshold.Location = new System.Drawing.Point(190, 135);
            this.btnSetAutoHazyThreshold.Margin = new System.Windows.Forms.Padding(2);
            this.btnSetAutoHazyThreshold.Name = "btnSetAutoHazyThreshold";
            this.btnSetAutoHazyThreshold.Size = new System.Drawing.Size(40, 24);
            this.btnSetAutoHazyThreshold.TabIndex = 116;
            this.btnSetAutoHazyThreshold.Text = "Set";
            this.btnSetAutoHazyThreshold.UseVisualStyleBackColor = true;
            this.btnSetAutoHazyThreshold.Click += new System.EventHandler(this.btnSetAutoHazyThreshold_Click);
            // 
            // txtAutoHazyThreshold
            // 
            this.txtAutoHazyThreshold.Location = new System.Drawing.Point(126, 140);
            this.txtAutoHazyThreshold.Margin = new System.Windows.Forms.Padding(2);
            this.txtAutoHazyThreshold.Name = "txtAutoHazyThreshold";
            this.txtAutoHazyThreshold.ReadOnly = true;
            this.txtAutoHazyThreshold.Size = new System.Drawing.Size(61, 20);
            this.txtAutoHazyThreshold.TabIndex = 115;
            // 
            // lblAutoHazyThreshold
            // 
            this.lblAutoHazyThreshold.AutoSize = true;
            this.lblAutoHazyThreshold.Location = new System.Drawing.Point(64, 140);
            this.lblAutoHazyThreshold.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAutoHazyThreshold.Name = "lblAutoHazyThreshold";
            this.lblAutoHazyThreshold.Size = new System.Drawing.Size(57, 13);
            this.lblAutoHazyThreshold.TabIndex = 114;
            this.lblAutoHazyThreshold.Text = "Threshold:";
            this.lblAutoHazyThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkAutoHazy
            // 
            this.chkAutoHazy.AutoSize = true;
            this.chkAutoHazy.Location = new System.Drawing.Point(70, 119);
            this.chkAutoHazy.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoHazy.Name = "chkAutoHazy";
            this.chkAutoHazy.Size = new System.Drawing.Size(75, 17);
            this.chkAutoHazy.TabIndex = 113;
            this.chkAutoHazy.Text = "Auto Hazy";
            this.chkAutoHazy.UseVisualStyleBackColor = true;
            // 
            // grpSpells
            // 
            this.grpSpells.Controls.Add(this.flpSpells);
            this.grpSpells.Location = new System.Drawing.Point(512, 13);
            this.grpSpells.Margin = new System.Windows.Forms.Padding(2);
            this.grpSpells.Name = "grpSpells";
            this.grpSpells.Padding = new System.Windows.Forms.Padding(2);
            this.grpSpells.Size = new System.Drawing.Size(214, 115);
            this.grpSpells.TabIndex = 108;
            this.grpSpells.TabStop = false;
            this.grpSpells.Text = "Active Spells";
            // 
            // flpSpells
            // 
            this.flpSpells.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSpells.Location = new System.Drawing.Point(2, 15);
            this.flpSpells.Margin = new System.Windows.Forms.Padding(2);
            this.flpSpells.Name = "flpSpells";
            this.flpSpells.Size = new System.Drawing.Size(210, 98);
            this.flpSpells.TabIndex = 0;
            // 
            // txtManashieldTime
            // 
            this.txtManashieldTime.Location = new System.Drawing.Point(340, 93);
            this.txtManashieldTime.Margin = new System.Windows.Forms.Padding(2);
            this.txtManashieldTime.Name = "txtManashieldTime";
            this.txtManashieldTime.ReadOnly = true;
            this.txtManashieldTime.Size = new System.Drawing.Size(86, 20);
            this.txtManashieldTime.TabIndex = 107;
            // 
            // lblManashieldTime
            // 
            this.lblManashieldTime.AutoSize = true;
            this.lblManashieldTime.Location = new System.Drawing.Point(250, 93);
            this.lblManashieldTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblManashieldTime.Name = "lblManashieldTime";
            this.lblManashieldTime.Size = new System.Drawing.Size(80, 13);
            this.lblManashieldTime.TabIndex = 106;
            this.lblManashieldTime.Text = "To Manashield:";
            this.lblManashieldTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPowerAttackTime
            // 
            this.txtPowerAttackTime.Location = new System.Drawing.Point(340, 70);
            this.txtPowerAttackTime.Margin = new System.Windows.Forms.Padding(2);
            this.txtPowerAttackTime.Name = "txtPowerAttackTime";
            this.txtPowerAttackTime.ReadOnly = true;
            this.txtPowerAttackTime.Size = new System.Drawing.Size(86, 20);
            this.txtPowerAttackTime.TabIndex = 105;
            // 
            // lblPowerAttackCooldown
            // 
            this.lblPowerAttackCooldown.AutoSize = true;
            this.lblPowerAttackCooldown.Location = new System.Drawing.Point(250, 70);
            this.lblPowerAttackCooldown.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPowerAttackCooldown.Name = "lblPowerAttackCooldown";
            this.lblPowerAttackCooldown.Size = new System.Drawing.Size(90, 13);
            this.lblPowerAttackCooldown.TabIndex = 104;
            this.lblPowerAttackCooldown.Text = "To Power Attack:";
            this.lblPowerAttackCooldown.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkAutoMana
            // 
            this.chkAutoMana.AutoSize = true;
            this.chkAutoMana.Checked = true;
            this.chkAutoMana.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoMana.Location = new System.Drawing.Point(437, 20);
            this.chkAutoMana.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoMana.Name = "chkAutoMana";
            this.chkAutoMana.Size = new System.Drawing.Size(78, 17);
            this.chkAutoMana.TabIndex = 103;
            this.chkAutoMana.Text = "Auto Mana";
            this.chkAutoMana.UseVisualStyleBackColor = true;
            // 
            // txtHitpoints
            // 
            this.txtHitpoints.Location = new System.Drawing.Point(340, 20);
            this.txtHitpoints.Margin = new System.Windows.Forms.Padding(2);
            this.txtHitpoints.Name = "txtHitpoints";
            this.txtHitpoints.ReadOnly = true;
            this.txtHitpoints.Size = new System.Drawing.Size(86, 20);
            this.txtHitpoints.TabIndex = 101;
            // 
            // lblHitpoints
            // 
            this.lblHitpoints.AutoSize = true;
            this.lblHitpoints.Location = new System.Drawing.Point(250, 23);
            this.lblHitpoints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHitpoints.Name = "lblHitpoints";
            this.lblHitpoints.Size = new System.Drawing.Size(51, 13);
            this.lblHitpoints.TabIndex = 100;
            this.lblHitpoints.Text = "Hitpoints:";
            this.lblHitpoints.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboMaxOffLevel
            // 
            this.cboMaxOffLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaxOffLevel.FormattingEnabled = true;
            this.cboMaxOffLevel.Items.AddRange(new object[] {
            "3",
            "2",
            "1"});
            this.cboMaxOffLevel.Location = new System.Drawing.Point(354, 425);
            this.cboMaxOffLevel.Margin = new System.Windows.Forms.Padding(2);
            this.cboMaxOffLevel.Name = "cboMaxOffLevel";
            this.cboMaxOffLevel.Size = new System.Drawing.Size(98, 21);
            this.cboMaxOffLevel.TabIndex = 99;
            // 
            // lblMaxOffensiveLevel
            // 
            this.lblMaxOffensiveLevel.AutoSize = true;
            this.lblMaxOffensiveLevel.Location = new System.Drawing.Point(286, 428);
            this.lblMaxOffensiveLevel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMaxOffensiveLevel.Name = "lblMaxOffensiveLevel";
            this.lblMaxOffensiveLevel.Size = new System.Drawing.Size(70, 13);
            this.lblMaxOffensiveLevel.TabIndex = 98;
            this.lblMaxOffensiveLevel.Text = "Max off level:";
            // 
            // btnManaSet
            // 
            this.btnManaSet.Location = new System.Drawing.Point(435, 41);
            this.btnManaSet.Margin = new System.Windows.Forms.Padding(2);
            this.btnManaSet.Name = "btnManaSet";
            this.btnManaSet.Size = new System.Drawing.Size(40, 24);
            this.btnManaSet.TabIndex = 96;
            this.btnManaSet.Text = "Set";
            this.btnManaSet.UseVisualStyleBackColor = true;
            this.btnManaSet.Click += new System.EventHandler(this.btnManaSet_Click);
            // 
            // txtMana
            // 
            this.txtMana.Location = new System.Drawing.Point(340, 44);
            this.txtMana.Margin = new System.Windows.Forms.Padding(2);
            this.txtMana.Name = "txtMana";
            this.txtMana.ReadOnly = true;
            this.txtMana.Size = new System.Drawing.Size(86, 20);
            this.txtMana.TabIndex = 95;
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Location = new System.Drawing.Point(250, 46);
            this.lblMana.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMana.Name = "lblMana";
            this.lblMana.Size = new System.Drawing.Size(37, 13);
            this.lblMana.TabIndex = 92;
            this.lblMana.Text = "Mana:";
            this.lblMana.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboCelduinExpress
            // 
            this.cboCelduinExpress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCelduinExpress.FormattingEnabled = true;
            this.cboCelduinExpress.Items.AddRange(new object[] {
            "At Sea",
            "Bree",
            "Mithlond"});
            this.cboCelduinExpress.Location = new System.Drawing.Point(611, 426);
            this.cboCelduinExpress.Margin = new System.Windows.Forms.Padding(2);
            this.cboCelduinExpress.Name = "cboCelduinExpress";
            this.cboCelduinExpress.Size = new System.Drawing.Size(119, 21);
            this.cboCelduinExpress.TabIndex = 80;
            this.cboCelduinExpress.SelectedIndexChanged += new System.EventHandler(this.cboCelduinExpress_SelectedIndexChanged);
            // 
            // lblCelduinExpressLocation
            // 
            this.lblCelduinExpressLocation.AutoSize = true;
            this.lblCelduinExpressLocation.Location = new System.Drawing.Point(524, 429);
            this.lblCelduinExpressLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCelduinExpressLocation.Name = "lblCelduinExpressLocation";
            this.lblCelduinExpressLocation.Size = new System.Drawing.Size(85, 13);
            this.lblCelduinExpressLocation.TabIndex = 90;
            this.lblCelduinExpressLocation.Text = "Celduin Express:";
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(308, 366);
            this.btnRemoveAll.Margin = new System.Windows.Forms.Padding(2);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(117, 28);
            this.btnRemoveAll.TabIndex = 87;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLevel3OffensiveSpell
            // 
            this.btnLevel3OffensiveSpell.Location = new System.Drawing.Point(64, 266);
            this.btnLevel3OffensiveSpell.Margin = new System.Windows.Forms.Padding(2);
            this.btnLevel3OffensiveSpell.Name = "btnLevel3OffensiveSpell";
            this.btnLevel3OffensiveSpell.Size = new System.Drawing.Size(117, 28);
            this.btnLevel3OffensiveSpell.TabIndex = 84;
            this.btnLevel3OffensiveSpell.Text = "Level 3 Offensive Spell";
            this.btnLevel3OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel3OffensiveSpell.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnStunMob
            // 
            this.btnStunMob.Location = new System.Drawing.Point(186, 201);
            this.btnStunMob.Margin = new System.Windows.Forms.Padding(2);
            this.btnStunMob.Name = "btnStunMob";
            this.btnStunMob.Size = new System.Drawing.Size(117, 28);
            this.btnStunMob.TabIndex = 83;
            this.btnStunMob.Text = "Stun Mob";
            this.btnStunMob.UseVisualStyleBackColor = true;
            this.btnStunMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastMend
            // 
            this.btnCastMend.Location = new System.Drawing.Point(430, 200);
            this.btnCastMend.Margin = new System.Windows.Forms.Padding(2);
            this.btnCastMend.Name = "btnCastMend";
            this.btnCastMend.Size = new System.Drawing.Size(93, 28);
            this.btnCastMend.TabIndex = 82;
            this.btnCastMend.Text = "Cast Mend";
            this.btnCastMend.UseVisualStyleBackColor = true;
            this.btnCastMend.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnReddishOrange
            // 
            this.btnReddishOrange.Location = new System.Drawing.Point(527, 201);
            this.btnReddishOrange.Margin = new System.Windows.Forms.Padding(2);
            this.btnReddishOrange.Name = "btnReddishOrange";
            this.btnReddishOrange.Size = new System.Drawing.Size(93, 28);
            this.btnReddishOrange.TabIndex = 81;
            this.btnReddishOrange.Text = "Red-Orange pot";
            this.btnReddishOrange.UseVisualStyleBackColor = true;
            this.btnReddishOrange.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(308, 269);
            this.btnHide.Margin = new System.Windows.Forms.Padding(2);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(117, 28);
            this.btnHide.TabIndex = 80;
            this.btnHide.Tag = "hide";
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(308, 233);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(117, 28);
            this.btnSearch.TabIndex = 79;
            this.btnSearch.Tag = "search";
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tabAncillary
            // 
            this.tabAncillary.Controls.Add(this.pnlAncillary);
            this.tabAncillary.Location = new System.Drawing.Point(4, 22);
            this.tabAncillary.Margin = new System.Windows.Forms.Padding(2);
            this.tabAncillary.Name = "tabAncillary";
            this.tabAncillary.Padding = new System.Windows.Forms.Padding(2);
            this.tabAncillary.Size = new System.Drawing.Size(1021, 904);
            this.tabAncillary.TabIndex = 1;
            this.tabAncillary.Text = "Ancillary";
            this.tabAncillary.UseVisualStyleBackColor = true;
            // 
            // pnlAncillary
            // 
            this.pnlAncillary.Controls.Add(this.lblPreferredAlignment);
            this.pnlAncillary.Controls.Add(this.txtPreferredAlignment);
            this.pnlAncillary.Controls.Add(this.lblLevel);
            this.pnlAncillary.Controls.Add(this.txtLevel);
            this.pnlAncillary.Controls.Add(this.lblMacro);
            this.pnlAncillary.Controls.Add(this.cboMacros);
            this.pnlAncillary.Controls.Add(this.btnRunMacro);
            this.pnlAncillary.Controls.Add(this.txtSetValue);
            this.pnlAncillary.Controls.Add(this.btnSet);
            this.pnlAncillary.Controls.Add(this.cboSetOption);
            this.pnlAncillary.Controls.Add(this.chkSetOn);
            this.pnlAncillary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAncillary.Location = new System.Drawing.Point(2, 2);
            this.pnlAncillary.Margin = new System.Windows.Forms.Padding(2);
            this.pnlAncillary.Name = "pnlAncillary";
            this.pnlAncillary.Size = new System.Drawing.Size(1017, 900);
            this.pnlAncillary.TabIndex = 0;
            // 
            // lblPreferredAlignment
            // 
            this.lblPreferredAlignment.AutoSize = true;
            this.lblPreferredAlignment.Location = new System.Drawing.Point(782, 50);
            this.lblPreferredAlignment.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPreferredAlignment.Name = "lblPreferredAlignment";
            this.lblPreferredAlignment.Size = new System.Drawing.Size(77, 13);
            this.lblPreferredAlignment.TabIndex = 92;
            this.lblPreferredAlignment.Text = "Alignment pref:";
            this.lblPreferredAlignment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPreferredAlignment
            // 
            this.txtPreferredAlignment.Enabled = false;
            this.txtPreferredAlignment.Location = new System.Drawing.Point(872, 48);
            this.txtPreferredAlignment.Margin = new System.Windows.Forms.Padding(2);
            this.txtPreferredAlignment.Name = "txtPreferredAlignment";
            this.txtPreferredAlignment.Size = new System.Drawing.Size(102, 20);
            this.txtPreferredAlignment.TabIndex = 93;
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(782, 21);
            this.lblLevel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(36, 13);
            this.lblLevel.TabIndex = 90;
            this.lblLevel.Text = "Level:";
            this.lblLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtLevel
            // 
            this.txtLevel.Enabled = false;
            this.txtLevel.Location = new System.Drawing.Point(872, 20);
            this.txtLevel.Margin = new System.Windows.Forms.Padding(2);
            this.txtLevel.Name = "txtLevel";
            this.txtLevel.Size = new System.Drawing.Size(102, 20);
            this.txtLevel.TabIndex = 91;
            // 
            // lblMacro
            // 
            this.lblMacro.AutoSize = true;
            this.lblMacro.Location = new System.Drawing.Point(12, 54);
            this.lblMacro.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMacro.Name = "lblMacro";
            this.lblMacro.Size = new System.Drawing.Size(40, 13);
            this.lblMacro.TabIndex = 64;
            this.lblMacro.Text = "Macro:";
            // 
            // cboMacros
            // 
            this.cboMacros.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMacros.FormattingEnabled = true;
            this.cboMacros.Location = new System.Drawing.Point(72, 50);
            this.cboMacros.Name = "cboMacros";
            this.cboMacros.Size = new System.Drawing.Size(324, 21);
            this.cboMacros.TabIndex = 63;
            // 
            // btnRunMacro
            // 
            this.btnRunMacro.Enabled = false;
            this.btnRunMacro.Location = new System.Drawing.Point(401, 49);
            this.btnRunMacro.Margin = new System.Windows.Forms.Padding(2);
            this.btnRunMacro.Name = "btnRunMacro";
            this.btnRunMacro.Size = new System.Drawing.Size(86, 24);
            this.btnRunMacro.TabIndex = 62;
            this.btnRunMacro.Text = "Run Macro";
            this.btnRunMacro.UseVisualStyleBackColor = true;
            this.btnRunMacro.Click += new System.EventHandler(this.btnRunMacro_Click);
            // 
            // tabEmotes
            // 
            this.tabEmotes.Controls.Add(this.pnlEmotes);
            this.tabEmotes.Location = new System.Drawing.Point(4, 22);
            this.tabEmotes.Margin = new System.Windows.Forms.Padding(2);
            this.tabEmotes.Name = "tabEmotes";
            this.tabEmotes.Size = new System.Drawing.Size(1021, 904);
            this.tabEmotes.TabIndex = 2;
            this.tabEmotes.Text = "Emotes";
            this.tabEmotes.UseVisualStyleBackColor = true;
            // 
            // pnlEmotes
            // 
            this.pnlEmotes.Controls.Add(this.btnSay);
            this.pnlEmotes.Controls.Add(this.chkShowEmotesWithoutTarget);
            this.pnlEmotes.Controls.Add(this.lblEmoteTarget);
            this.pnlEmotes.Controls.Add(this.txtEmoteTarget);
            this.pnlEmotes.Controls.Add(this.lblCommandText);
            this.pnlEmotes.Controls.Add(this.btnEmote);
            this.pnlEmotes.Controls.Add(this.txtCommandText);
            this.pnlEmotes.Controls.Add(this.grpEmotes);
            this.pnlEmotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEmotes.Location = new System.Drawing.Point(0, 0);
            this.pnlEmotes.Margin = new System.Windows.Forms.Padding(2);
            this.pnlEmotes.Name = "pnlEmotes";
            this.pnlEmotes.Size = new System.Drawing.Size(1021, 904);
            this.pnlEmotes.TabIndex = 12;
            // 
            // btnSay
            // 
            this.btnSay.Location = new System.Drawing.Point(349, 9);
            this.btnSay.Margin = new System.Windows.Forms.Padding(2);
            this.btnSay.Name = "btnSay";
            this.btnSay.Size = new System.Drawing.Size(61, 20);
            this.btnSay.TabIndex = 15;
            this.btnSay.Text = "Say";
            this.btnSay.UseVisualStyleBackColor = true;
            this.btnSay.Click += new System.EventHandler(this.btnSay_Click);
            // 
            // chkShowEmotesWithoutTarget
            // 
            this.chkShowEmotesWithoutTarget.AutoSize = true;
            this.chkShowEmotesWithoutTarget.Checked = true;
            this.chkShowEmotesWithoutTarget.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowEmotesWithoutTarget.Location = new System.Drawing.Point(284, 34);
            this.chkShowEmotesWithoutTarget.Name = "chkShowEmotesWithoutTarget";
            this.chkShowEmotesWithoutTarget.Size = new System.Drawing.Size(162, 17);
            this.chkShowEmotesWithoutTarget.TabIndex = 14;
            this.chkShowEmotesWithoutTarget.Text = "Show Emotes without Target";
            this.chkShowEmotesWithoutTarget.UseVisualStyleBackColor = true;
            this.chkShowEmotesWithoutTarget.CheckedChanged += new System.EventHandler(this.chkShowEmotesWithoutTarget_CheckedChanged);
            // 
            // lblEmoteTarget
            // 
            this.lblEmoteTarget.AutoSize = true;
            this.lblEmoteTarget.Location = new System.Drawing.Point(12, 35);
            this.lblEmoteTarget.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEmoteTarget.Name = "lblEmoteTarget";
            this.lblEmoteTarget.Size = new System.Drawing.Size(41, 13);
            this.lblEmoteTarget.TabIndex = 12;
            this.lblEmoteTarget.Text = "Target:";
            // 
            // txtEmoteTarget
            // 
            this.txtEmoteTarget.Location = new System.Drawing.Point(93, 32);
            this.txtEmoteTarget.Margin = new System.Windows.Forms.Padding(2);
            this.txtEmoteTarget.Name = "txtEmoteTarget";
            this.txtEmoteTarget.Size = new System.Drawing.Size(186, 20);
            this.txtEmoteTarget.TabIndex = 13;
            this.txtEmoteTarget.TextChanged += new System.EventHandler(this.txtEmoteTarget_TextChanged);
            // 
            // lblCommandText
            // 
            this.lblCommandText.AutoSize = true;
            this.lblCommandText.Location = new System.Drawing.Point(12, 13);
            this.lblCommandText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCommandText.Name = "lblCommandText";
            this.lblCommandText.Size = new System.Drawing.Size(77, 13);
            this.lblCommandText.TabIndex = 8;
            this.lblCommandText.Text = "Command text:";
            // 
            // btnEmote
            // 
            this.btnEmote.Location = new System.Drawing.Point(284, 9);
            this.btnEmote.Margin = new System.Windows.Forms.Padding(2);
            this.btnEmote.Name = "btnEmote";
            this.btnEmote.Size = new System.Drawing.Size(61, 20);
            this.btnEmote.TabIndex = 11;
            this.btnEmote.Text = "Emote";
            this.btnEmote.UseVisualStyleBackColor = true;
            this.btnEmote.Click += new System.EventHandler(this.btnEmote_Click);
            // 
            // txtCommandText
            // 
            this.txtCommandText.Location = new System.Drawing.Point(93, 10);
            this.txtCommandText.Margin = new System.Windows.Forms.Padding(2);
            this.txtCommandText.Name = "txtCommandText";
            this.txtCommandText.Size = new System.Drawing.Size(187, 20);
            this.txtCommandText.TabIndex = 9;
            this.txtCommandText.TextChanged += new System.EventHandler(this.txtEmoteText_TextChanged);
            // 
            // grpEmotes
            // 
            this.grpEmotes.Controls.Add(this.flpEmotes);
            this.grpEmotes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpEmotes.Location = new System.Drawing.Point(0, 57);
            this.grpEmotes.Margin = new System.Windows.Forms.Padding(2);
            this.grpEmotes.Name = "grpEmotes";
            this.grpEmotes.Padding = new System.Windows.Forms.Padding(2);
            this.grpEmotes.Size = new System.Drawing.Size(1021, 847);
            this.grpEmotes.TabIndex = 10;
            this.grpEmotes.TabStop = false;
            this.grpEmotes.Text = "Emotes";
            // 
            // flpEmotes
            // 
            this.flpEmotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpEmotes.Location = new System.Drawing.Point(2, 15);
            this.flpEmotes.Margin = new System.Windows.Forms.Padding(2);
            this.flpEmotes.Name = "flpEmotes";
            this.flpEmotes.Size = new System.Drawing.Size(1017, 830);
            this.flpEmotes.TabIndex = 0;
            // 
            // tabHelp
            // 
            this.tabHelp.Controls.Add(this.grpHelp);
            this.tabHelp.Location = new System.Drawing.Point(4, 22);
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Size = new System.Drawing.Size(1021, 904);
            this.tabHelp.TabIndex = 3;
            this.tabHelp.Text = "Help";
            this.tabHelp.UseVisualStyleBackColor = true;
            // 
            // grpHelp
            // 
            this.grpHelp.Controls.Add(this.flpHelp);
            this.grpHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHelp.Location = new System.Drawing.Point(0, 0);
            this.grpHelp.Name = "grpHelp";
            this.grpHelp.Size = new System.Drawing.Size(1021, 904);
            this.grpHelp.TabIndex = 0;
            this.grpHelp.TabStop = false;
            this.grpHelp.Text = "Help";
            // 
            // flpHelp
            // 
            this.flpHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpHelp.Location = new System.Drawing.Point(3, 16);
            this.flpHelp.Name = "flpHelp";
            this.flpHelp.Size = new System.Drawing.Size(1015, 885);
            this.flpHelp.TabIndex = 0;
            // 
            // tmr
            // 
            this.tmr.Enabled = true;
            this.tmr.Interval = 20;
            this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Margin = new System.Windows.Forms.Padding(2);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.tcMain);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.grpConsole);
            this.scMain.Size = new System.Drawing.Size(1760, 930);
            this.scMain.SplitterDistance = 1029;
            this.scMain.SplitterWidth = 3;
            this.scMain.TabIndex = 81;
            // 
            // grpConsole
            // 
            this.grpConsole.Controls.Add(this.pnlConsoleHolder);
            this.grpConsole.Controls.Add(this.pnlCommand);
            this.grpConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConsole.Location = new System.Drawing.Point(0, 0);
            this.grpConsole.Margin = new System.Windows.Forms.Padding(2);
            this.grpConsole.Name = "grpConsole";
            this.grpConsole.Padding = new System.Windows.Forms.Padding(2);
            this.grpConsole.Size = new System.Drawing.Size(728, 930);
            this.grpConsole.TabIndex = 110;
            this.grpConsole.TabStop = false;
            this.grpConsole.Text = "Console";
            // 
            // pnlConsoleHolder
            // 
            this.pnlConsoleHolder.Controls.Add(this.rtbConsole);
            this.pnlConsoleHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConsoleHolder.Location = new System.Drawing.Point(2, 15);
            this.pnlConsoleHolder.Name = "pnlConsoleHolder";
            this.pnlConsoleHolder.Size = new System.Drawing.Size(724, 863);
            this.pnlConsoleHolder.TabIndex = 31;
            // 
            // rtbConsole
            // 
            this.rtbConsole.BackColor = System.Drawing.Color.Black;
            this.rtbConsole.ContextMenuStrip = this.ctxConsole;
            this.rtbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConsole.Font = new System.Drawing.Font("Courier New", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbConsole.ForeColor = System.Drawing.Color.White;
            this.rtbConsole.HideSelection = false;
            this.rtbConsole.Location = new System.Drawing.Point(0, 0);
            this.rtbConsole.Margin = new System.Windows.Forms.Padding(2);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(724, 863);
            this.rtbConsole.TabIndex = 0;
            this.rtbConsole.Text = "";
            // 
            // ctxConsole
            // 
            this.ctxConsole.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxConsole.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiClearConsole});
            this.ctxConsole.Name = "ctxConsole";
            this.ctxConsole.Size = new System.Drawing.Size(102, 26);
            this.ctxConsole.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxConsole_ItemClicked);
            // 
            // tsmiClearConsole
            // 
            this.tsmiClearConsole.Name = "tsmiClearConsole";
            this.tsmiClearConsole.Size = new System.Drawing.Size(101, 22);
            this.tsmiClearConsole.Text = "Clear";
            // 
            // pnlCommand
            // 
            this.pnlCommand.Controls.Add(this.txtOneOffCommand);
            this.pnlCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCommand.Location = new System.Drawing.Point(2, 878);
            this.pnlCommand.Name = "pnlCommand";
            this.pnlCommand.Size = new System.Drawing.Size(724, 50);
            this.pnlCommand.TabIndex = 30;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1760, 930);
            this.Controls.Add(this.scMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ctxMob.ResumeLayout(false);
            this.grpRealm.ResumeLayout(false);
            this.grpRealm.PerformLayout();
            this.ctxLocations.ResumeLayout(false);
            this.grpLocations.ResumeLayout(false);
            this.grpOneClickMacros.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.grpSingleMove.ResumeLayout(false);
            this.grpSingleMove.PerformLayout();
            this.grpSpells.ResumeLayout(false);
            this.tabAncillary.ResumeLayout(false);
            this.pnlAncillary.ResumeLayout(false);
            this.pnlAncillary.PerformLayout();
            this.tabEmotes.ResumeLayout(false);
            this.pnlEmotes.ResumeLayout(false);
            this.pnlEmotes.PerformLayout();
            this.grpEmotes.ResumeLayout(false);
            this.tabHelp.ResumeLayout(false);
            this.grpHelp.ResumeLayout(false);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.grpConsole.ResumeLayout(false);
            this.pnlConsoleHolder.ResumeLayout(false);
            this.ctxConsole.ResumeLayout(false);
            this.pnlCommand.ResumeLayout(false);
            this.pnlCommand.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLevel1OffensiveSpell;
        private System.Windows.Forms.TextBox txtMob;
        private System.Windows.Forms.Label lblMob;
        private System.Windows.Forms.Button btnLevel2OffensiveSpell;
        private System.Windows.Forms.Button btnFlee;
        private System.Windows.Forms.Button btnDrinkHazy;
        private System.Windows.Forms.Button btnLookAtMob;
        private System.Windows.Forms.Button btnLook;
        private System.Windows.Forms.Button btnCastVigor;
        private System.Windows.Forms.Button btnManashield;
        private System.Windows.Forms.Button btnCastCurePoison;
        private System.Windows.Forms.Button btnTime;
        private System.Windows.Forms.Button btnScore;
        private System.Windows.Forms.Button btnInformation;
        private System.Windows.Forms.Button btnCastProtection;
        private System.Windows.Forms.CheckBox chkIsNight;
        private System.Windows.Forms.TextBox txtOneOffCommand;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnAttackMob;
        private System.Windows.Forms.TextBox txtCurrentRoom;
        private System.Windows.Forms.Label lblCurrentRoom;
        private System.Windows.Forms.Button btnDrinkYellow;
        private System.Windows.Forms.Button btnDrinkGreen;
        private System.Windows.Forms.TextBox txtWeapon;
        private System.Windows.Forms.Label lblWeapon;
        private System.Windows.Forms.Button btnWieldWeapon;
        private System.Windows.Forms.Button btnCastBless;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.ComboBox cboSetOption;
        private System.Windows.Forms.CheckBox chkSetOn;
        private System.Windows.Forms.TextBox txtWand;
        private System.Windows.Forms.Label lblWand;
        private System.Windows.Forms.Button btnUseWandOnMob;
        private System.Windows.Forms.Button btnWho;
        private System.Windows.Forms.Button btnUptime;
        private System.Windows.Forms.Button btnEquipment;
        private System.Windows.Forms.Button btnPowerAttackMob;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.TextBox txtSetValue;
        private System.Windows.Forms.GroupBox grpRealm;
        private System.Windows.Forms.RadioButton radFire;
        private System.Windows.Forms.RadioButton radWater;
        private System.Windows.Forms.RadioButton radWind;
        private System.Windows.Forms.RadioButton radEarth;
        private System.Windows.Forms.TreeView treeLocations;
        private System.Windows.Forms.GroupBox grpLocations;
        private System.Windows.Forms.GroupBox grpOneClickMacros;
        private System.Windows.Forms.FlowLayoutPanel flpOneClickMacros;
        private System.Windows.Forms.Button btnVariables;
        private System.Windows.Forms.TextBox txtPotion;
        private System.Windows.Forms.Label lblPotion;
        private System.Windows.Forms.Button btnRemoveWeapon;
        private System.Windows.Forms.Button btnFumbleMob;
        private System.Windows.Forms.Button btnNortheast;
        private System.Windows.Forms.Button btnNorth;
        private System.Windows.Forms.Button btnNorthwest;
        private System.Windows.Forms.Button btnWest;
        private System.Windows.Forms.Button btnEast;
        private System.Windows.Forms.Button btnSouthwest;
        private System.Windows.Forms.Button btnSouth;
        private System.Windows.Forms.Button btnSoutheast;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TabPage tabAncillary;
        private System.Windows.Forms.Panel pnlAncillary;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Label lblMacro;
        private System.Windows.Forms.ComboBox cboMacros;
        private System.Windows.Forms.Button btnRunMacro;
        private System.Windows.Forms.Button btnReddishOrange;
        private System.Windows.Forms.Button btnCastMend;
        private System.Windows.Forms.Button btnStunMob;
        private System.Windows.Forms.Button btnLevel3OffensiveSpell;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Label lblCelduinExpressLocation;
        private System.Windows.Forms.ComboBox cboCelduinExpress;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Timer tmr;
        private System.Windows.Forms.TextBox txtMana;
        private System.Windows.Forms.Button btnManaSet;
        private System.Windows.Forms.ComboBox cboMaxOffLevel;
        private System.Windows.Forms.Label lblMaxOffensiveLevel;
        private System.Windows.Forms.Label lblPreferredAlignment;
        private System.Windows.Forms.TextBox txtPreferredAlignment;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.TextBox txtLevel;
        private System.Windows.Forms.TextBox txtHitpoints;
        private System.Windows.Forms.Label lblHitpoints;
        private System.Windows.Forms.CheckBox chkAutoMana;
        private System.Windows.Forms.Label lblPowerAttackCooldown;
        private System.Windows.Forms.TextBox txtPowerAttackTime;
        private System.Windows.Forms.TextBox txtManashieldTime;
        private System.Windows.Forms.Label lblManashieldTime;
        private System.Windows.Forms.GroupBox grpSpells;
        private System.Windows.Forms.FlowLayoutPanel flpSpells;
        private System.Windows.Forms.Button btnDn;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnOtherSingleMove;
        private System.Windows.Forms.Button btnSetAutoHazyThreshold;
        private System.Windows.Forms.TextBox txtAutoHazyThreshold;
        private System.Windows.Forms.Label lblAutoHazyThreshold;
        private System.Windows.Forms.CheckBox chkAutoHazy;
        private System.Windows.Forms.TabPage tabEmotes;
        private System.Windows.Forms.Button btnEmote;
        private System.Windows.Forms.GroupBox grpEmotes;
        private System.Windows.Forms.TextBox txtCommandText;
        private System.Windows.Forms.Label lblCommandText;
        private System.Windows.Forms.Panel pnlEmotes;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.GroupBox grpConsole;
        private System.Windows.Forms.RichTextBox rtbConsole;
        private System.Windows.Forms.Label lblEmoteTarget;
        private System.Windows.Forms.TextBox txtEmoteTarget;
        private System.Windows.Forms.FlowLayoutPanel flpEmotes;
        private System.Windows.Forms.CheckBox chkShowEmotesWithoutTarget;
        private System.Windows.Forms.ContextMenuStrip ctxConsole;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearConsole;
        private System.Windows.Forms.ContextMenuStrip ctxMob;
        private System.Windows.Forms.ToolStripMenuItem tsmiMob1;
        private System.Windows.Forms.ToolStripMenuItem tsmiMob2;
        private System.Windows.Forms.ToolStripMenuItem tsmiMob3;
        private System.Windows.Forms.ContextMenuStrip ctxRoomExits;
        private System.Windows.Forms.Button btnSay;
        private System.Windows.Forms.GroupBox grpSingleMove;
        private System.Windows.Forms.CheckBox chkExecuteMove;
        private System.Windows.Forms.Button btnExitSingleMove;
        private System.Windows.Forms.Button btnClearCurrentLocation;
        private System.Windows.Forms.TabPage tabHelp;
        private System.Windows.Forms.GroupBox grpHelp;
        private System.Windows.Forms.FlowLayoutPanel flpHelp;
        private System.Windows.Forms.Panel pnlCommand;
        private System.Windows.Forms.Panel pnlConsoleHolder;
        private System.Windows.Forms.ContextMenuStrip ctxLocations;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetLocation;
        private System.Windows.Forms.ToolStripMenuItem tsmiGoToLocation;
    }
}

