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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnLevel1OffensiveSpell = new System.Windows.Forms.Button();
            this.txtMob = new System.Windows.Forms.TextBox();
            this.lblMob = new System.Windows.Forms.Label();
            this.btnLevel2OffensiveSpell = new System.Windows.Forms.Button();
            this.btnFlee = new System.Windows.Forms.Button();
            this.btnDrinkHazy = new System.Windows.Forms.Button();
            this.btnLookAtMob = new System.Windows.Forms.Button();
            this.btnLook = new System.Windows.Forms.Button();
            this.btnCastVigor = new System.Windows.Forms.Button();
            this.btnCastCurePoison = new System.Windows.Forms.Button();
            this.txtOneOffCommand = new System.Windows.Forms.TextBox();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnAttackMob = new System.Windows.Forms.Button();
            this.btnDrinkVigor = new System.Windows.Forms.Button();
            this.btnDrinkCurepoison = new System.Windows.Forms.Button();
            this.btnSet = new System.Windows.Forms.Button();
            this.cboSetOption = new System.Windows.Forms.ComboBox();
            this.chkSetOn = new System.Windows.Forms.CheckBox();
            this.txtWand = new System.Windows.Forms.TextBox();
            this.lblWand = new System.Windows.Forms.Label();
            this.btnUseWandOnMob = new System.Windows.Forms.Button();
            this.btnPowerAttackMob = new System.Windows.Forms.Button();
            this.txtSetValue = new System.Windows.Forms.TextBox();
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
            this.grpPermRuns = new System.Windows.Forms.GroupBox();
            this.btnFightAll = new System.Windows.Forms.Button();
            this.btnFightOne = new System.Windows.Forms.Button();
            this.btnAdHocPermRun = new System.Windows.Forms.Button();
            this.btnNonCombatPermRun = new System.Windows.Forms.Button();
            this.btnResumeCurrentPermRun = new System.Windows.Forms.Button();
            this.btnCompleteCurrentPermRun = new System.Windows.Forms.Button();
            this.btnRemoveNextPermRun = new System.Windows.Forms.Button();
            this.btnRemoveCurrentPermRun = new System.Windows.Forms.Button();
            this.btnPermRuns = new System.Windows.Forms.Button();
            this.txtNextPermRun = new System.Windows.Forms.TextBox();
            this.lblNextPermRun = new System.Windows.Forms.Label();
            this.txtCurrentPermRun = new System.Windows.Forms.TextBox();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.btnGoToInventorySink = new System.Windows.Forms.Button();
            this.btnGoToPawnShop = new System.Windows.Forms.Button();
            this.lblGold = new System.Windows.Forms.Label();
            this.lblToNextLevelValue = new System.Windows.Forms.Label();
            this.grpInventory = new System.Windows.Forms.GroupBox();
            this.lstInventory = new System.Windows.Forms.ListBox();
            this.ctxInventoryOrEquipmentItem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.grpEquipment = new System.Windows.Forms.GroupBox();
            this.lstEquipment = new System.Windows.Forms.ListBox();
            this.btnGoToHealingRoom = new System.Windows.Forms.Button();
            this.lblArea = new System.Windows.Forms.Label();
            this.cboArea = new System.Windows.Forms.ComboBox();
            this.grpCurrentRoom = new System.Windows.Forms.GroupBox();
            this.treeCurrentRoom = new System.Windows.Forms.TreeView();
            this.ctxCurrentRoom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnLocations = new System.Windows.Forms.Button();
            this.btnIncrementWand = new System.Windows.Forms.Button();
            this.grpSkillCooldowns = new System.Windows.Forms.GroupBox();
            this.lblAutoEscapeValue = new System.Windows.Forms.Label();
            this.ctxAutoEscape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAutoEscapeIsActive = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSetAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAutoEscapeFlee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTime = new System.Windows.Forms.Label();
            this.grpCurrentPlayer = new System.Windows.Forms.GroupBox();
            this.lblManaValue = new System.Windows.Forms.Label();
            this.lblHitpointsValue = new System.Windows.Forms.Label();
            this.lblMana = new System.Windows.Forms.Label();
            this.lblHitpoints = new System.Windows.Forms.Label();
            this.grpMessages = new System.Windows.Forms.GroupBox();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.ctxMessages = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyMessages = new System.Windows.Forms.ToolStripMenuItem();
            this.grpMob = new System.Windows.Forms.GroupBox();
            this.txtMobStatus = new System.Windows.Forms.TextBox();
            this.lblMobStatus = new System.Windows.Forms.Label();
            this.txtMobDamage = new System.Windows.Forms.TextBox();
            this.lblMobDamage = new System.Windows.Forms.Label();
            this.btnGraph = new System.Windows.Forms.Button();
            this.grpSingleMove = new System.Windows.Forms.GroupBox();
            this.btnOut = new System.Windows.Forms.Button();
            this.btnOtherSingleMove = new System.Windows.Forms.Button();
            this.btnDn = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.grpSpells = new System.Windows.Forms.GroupBox();
            this.flpSpells = new System.Windows.Forms.FlowLayoutPanel();
            this.btnLevel3OffensiveSpell = new System.Windows.Forms.Button();
            this.btnStunMob = new System.Windows.Forms.Button();
            this.btnCastMend = new System.Windows.Forms.Button();
            this.btnDrinkMend = new System.Windows.Forms.Button();
            this.tabAncillary = new System.Windows.Forms.TabPage();
            this.pnlAncillary = new System.Windows.Forms.Panel();
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
            this.pnlOverallLeft = new System.Windows.Forms.Panel();
            this.pnlTabControl = new System.Windows.Forms.Panel();
            this.tsTopMenu = new System.Windows.Forms.ToolStrip();
            this.tsbInformation = new System.Windows.Forms.ToolStripButton();
            this.tsbInventoryAndEquipment = new System.Windows.Forms.ToolStripButton();
            this.tsbRemoveAll = new System.Windows.Forms.ToolStripButton();
            this.tsbWearAll = new System.Windows.Forms.ToolStripButton();
            this.tsbWho = new System.Windows.Forms.ToolStripButton();
            this.tsbUptime = new System.Windows.Forms.ToolStripButton();
            this.tsbSpells = new System.Windows.Forms.ToolStripButton();
            this.tsbScore = new System.Windows.Forms.ToolStripButton();
            this.tsbTime = new System.Windows.Forms.ToolStripButton();
            this.tsddActions = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHide = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiItemManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTimeInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddbSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiEditSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportXML = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportXML = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportFromPlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQuitWithoutSaving = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRestoreDefaults = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenLogFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbReloadMap = new System.Windows.Forms.ToolStripButton();
            this.tsbQuit = new System.Windows.Forms.ToolStripButton();
            this.tsbLogout = new System.Windows.Forms.ToolStripButton();
            this.tmr = new System.Windows.Forms.Timer(this.components);
            this.grpConsole = new System.Windows.Forms.GroupBox();
            this.pnlConsoleHolder = new System.Windows.Forms.Panel();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.ctxConsole = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiClearConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlCommand = new System.Windows.Forms.Panel();
            this.tcMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.grpPermRuns.SuspendLayout();
            this.grpInventory.SuspendLayout();
            this.grpEquipment.SuspendLayout();
            this.grpCurrentRoom.SuspendLayout();
            this.ctxAutoEscape.SuspendLayout();
            this.grpCurrentPlayer.SuspendLayout();
            this.grpMessages.SuspendLayout();
            this.ctxMessages.SuspendLayout();
            this.grpMob.SuspendLayout();
            this.grpSingleMove.SuspendLayout();
            this.grpSpells.SuspendLayout();
            this.tabAncillary.SuspendLayout();
            this.pnlAncillary.SuspendLayout();
            this.tabEmotes.SuspendLayout();
            this.pnlEmotes.SuspendLayout();
            this.grpEmotes.SuspendLayout();
            this.tabHelp.SuspendLayout();
            this.grpHelp.SuspendLayout();
            this.pnlOverallLeft.SuspendLayout();
            this.pnlTabControl.SuspendLayout();
            this.tsTopMenu.SuspendLayout();
            this.grpConsole.SuspendLayout();
            this.pnlConsoleHolder.SuspendLayout();
            this.ctxConsole.SuspendLayout();
            this.pnlCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLevel1OffensiveSpell
            // 
            this.btnLevel1OffensiveSpell.Location = new System.Drawing.Point(662, 474);
            this.btnLevel1OffensiveSpell.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLevel1OffensiveSpell.Name = "btnLevel1OffensiveSpell";
            this.btnLevel1OffensiveSpell.Size = new System.Drawing.Size(81, 28);
            this.btnLevel1OffensiveSpell.TabIndex = 0;
            this.btnLevel1OffensiveSpell.Text = "Cast Level 1";
            this.btnLevel1OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel1OffensiveSpell.Click += new System.EventHandler(this.btnLevel1OffensiveSpell_Click);
            // 
            // txtMob
            // 
            this.txtMob.Location = new System.Drawing.Point(71, 18);
            this.txtMob.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtMob.Name = "txtMob";
            this.txtMob.Size = new System.Drawing.Size(161, 20);
            this.txtMob.TabIndex = 4;
            this.txtMob.TextChanged += new System.EventHandler(this.txtMob_TextChanged);
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(13, 22);
            this.lblMob.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(31, 13);
            this.lblMob.TabIndex = 3;
            this.lblMob.Text = "Mob:";
            this.lblMob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLevel2OffensiveSpell
            // 
            this.btnLevel2OffensiveSpell.Location = new System.Drawing.Point(662, 505);
            this.btnLevel2OffensiveSpell.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLevel2OffensiveSpell.Name = "btnLevel2OffensiveSpell";
            this.btnLevel2OffensiveSpell.Size = new System.Drawing.Size(81, 28);
            this.btnLevel2OffensiveSpell.TabIndex = 5;
            this.btnLevel2OffensiveSpell.Text = "Cast Level 2";
            this.btnLevel2OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel2OffensiveSpell.Click += new System.EventHandler(this.btnLevel2OffensiveSpell_Click);
            // 
            // btnFlee
            // 
            this.btnFlee.Location = new System.Drawing.Point(662, 566);
            this.btnFlee.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnFlee.Name = "btnFlee";
            this.btnFlee.Size = new System.Drawing.Size(80, 28);
            this.btnFlee.TabIndex = 6;
            this.btnFlee.Text = "Flee";
            this.btnFlee.UseVisualStyleBackColor = true;
            this.btnFlee.Click += new System.EventHandler(this.btnFlee_Click);
            // 
            // btnDrinkHazy
            // 
            this.btnDrinkHazy.Location = new System.Drawing.Point(663, 597);
            this.btnDrinkHazy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDrinkHazy.Name = "btnDrinkHazy";
            this.btnDrinkHazy.Size = new System.Drawing.Size(80, 28);
            this.btnDrinkHazy.TabIndex = 7;
            this.btnDrinkHazy.Text = "Hazy pot";
            this.btnDrinkHazy.UseVisualStyleBackColor = true;
            this.btnDrinkHazy.Click += new System.EventHandler(this.btnDrinkHazy_Click);
            // 
            // btnLookAtMob
            // 
            this.btnLookAtMob.Location = new System.Drawing.Point(579, 597);
            this.btnLookAtMob.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLookAtMob.Name = "btnLookAtMob";
            this.btnLookAtMob.Size = new System.Drawing.Size(79, 28);
            this.btnLookAtMob.TabIndex = 8;
            this.btnLookAtMob.Text = "Look at Mob";
            this.btnLookAtMob.UseVisualStyleBackColor = true;
            this.btnLookAtMob.Click += new System.EventHandler(this.btnLookAtMob_Click);
            // 
            // btnLook
            // 
            this.btnLook.Location = new System.Drawing.Point(450, 73);
            this.btnLook.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLook.Name = "btnLook";
            this.btnLook.Size = new System.Drawing.Size(79, 28);
            this.btnLook.TabIndex = 9;
            this.btnLook.Text = "Look";
            this.btnLook.UseVisualStyleBackColor = true;
            this.btnLook.Click += new System.EventHandler(this.btnLook_Click);
            // 
            // btnCastVigor
            // 
            this.btnCastVigor.Location = new System.Drawing.Point(579, 628);
            this.btnCastVigor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCastVigor.Name = "btnCastVigor";
            this.btnCastVigor.Size = new System.Drawing.Size(80, 28);
            this.btnCastVigor.TabIndex = 10;
            this.btnCastVigor.Text = "Vigor";
            this.btnCastVigor.UseVisualStyleBackColor = true;
            this.btnCastVigor.Click += new System.EventHandler(this.btnVigor_Click);
            // 
            // btnCastCurePoison
            // 
            this.btnCastCurePoison.Location = new System.Drawing.Point(579, 690);
            this.btnCastCurePoison.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCastCurePoison.Name = "btnCastCurePoison";
            this.btnCastCurePoison.Size = new System.Drawing.Size(80, 28);
            this.btnCastCurePoison.TabIndex = 18;
            this.btnCastCurePoison.Text = "Curepoison";
            this.btnCastCurePoison.UseVisualStyleBackColor = true;
            this.btnCastCurePoison.Click += new System.EventHandler(this.btnCurePoison_Click);
            // 
            // txtOneOffCommand
            // 
            this.txtOneOffCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOneOffCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOneOffCommand.Location = new System.Drawing.Point(0, 0);
            this.txtOneOffCommand.Margin = new System.Windows.Forms.Padding(0);
            this.txtOneOffCommand.Name = "txtOneOffCommand";
            this.txtOneOffCommand.Size = new System.Drawing.Size(204, 26);
            this.txtOneOffCommand.TabIndex = 29;
            this.txtOneOffCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOneOffCommand_KeyPress);
            // 
            // btnAbort
            // 
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(746, 628);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(80, 28);
            this.btnAbort.TabIndex = 33;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnAttackMob
            // 
            this.btnAttackMob.Location = new System.Drawing.Point(579, 505);
            this.btnAttackMob.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAttackMob.Name = "btnAttackMob";
            this.btnAttackMob.Size = new System.Drawing.Size(79, 28);
            this.btnAttackMob.TabIndex = 35;
            this.btnAttackMob.Text = "Atk";
            this.btnAttackMob.UseVisualStyleBackColor = true;
            this.btnAttackMob.Click += new System.EventHandler(this.btnAttackMob_Click);
            // 
            // btnDrinkVigor
            // 
            this.btnDrinkVigor.Location = new System.Drawing.Point(664, 628);
            this.btnDrinkVigor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDrinkVigor.Name = "btnDrinkVigor";
            this.btnDrinkVigor.Size = new System.Drawing.Size(79, 28);
            this.btnDrinkVigor.TabIndex = 39;
            this.btnDrinkVigor.Text = "potion";
            this.btnDrinkVigor.UseVisualStyleBackColor = true;
            this.btnDrinkVigor.Click += new System.EventHandler(this.btnVigorPotion_Click);
            // 
            // btnDrinkCurepoison
            // 
            this.btnDrinkCurepoison.Location = new System.Drawing.Point(664, 690);
            this.btnDrinkCurepoison.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDrinkCurepoison.Name = "btnDrinkCurepoison";
            this.btnDrinkCurepoison.Size = new System.Drawing.Size(80, 28);
            this.btnDrinkCurepoison.TabIndex = 40;
            this.btnDrinkCurepoison.Text = "potion";
            this.btnDrinkCurepoison.UseVisualStyleBackColor = true;
            this.btnDrinkCurepoison.Click += new System.EventHandler(this.btnCurePoisonPotion_Click);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(10, 13);
            this.btnSet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.cboSetOption.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboSetOption.Name = "cboSetOption";
            this.cboSetOption.Size = new System.Drawing.Size(124, 21);
            this.cboSetOption.TabIndex = 46;
            this.cboSetOption.SelectedIndexChanged += new System.EventHandler(this.cboSetOption_SelectedIndexChanged);
            // 
            // chkSetOn
            // 
            this.chkSetOn.AutoSize = true;
            this.chkSetOn.Location = new System.Drawing.Point(200, 17);
            this.chkSetOn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkSetOn.Name = "chkSetOn";
            this.chkSetOn.Size = new System.Drawing.Size(46, 17);
            this.chkSetOn.TabIndex = 47;
            this.chkSetOn.Text = "On?";
            this.chkSetOn.UseVisualStyleBackColor = true;
            this.chkSetOn.CheckedChanged += new System.EventHandler(this.chkSetOn_CheckedChanged);
            // 
            // txtWand
            // 
            this.txtWand.Location = new System.Drawing.Point(71, 42);
            this.txtWand.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtWand.Name = "txtWand";
            this.txtWand.Size = new System.Drawing.Size(161, 20);
            this.txtWand.TabIndex = 49;
            this.txtWand.TextChanged += new System.EventHandler(this.txtWand_TextChanged);
            // 
            // lblWand
            // 
            this.lblWand.AutoSize = true;
            this.lblWand.Location = new System.Drawing.Point(13, 45);
            this.lblWand.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWand.Name = "lblWand";
            this.lblWand.Size = new System.Drawing.Size(39, 13);
            this.lblWand.TabIndex = 48;
            this.lblWand.Text = "Wand:";
            this.lblWand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnUseWandOnMob
            // 
            this.btnUseWandOnMob.Location = new System.Drawing.Point(579, 566);
            this.btnUseWandOnMob.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnUseWandOnMob.Name = "btnUseWandOnMob";
            this.btnUseWandOnMob.Size = new System.Drawing.Size(79, 28);
            this.btnUseWandOnMob.TabIndex = 50;
            this.btnUseWandOnMob.Text = "Wand";
            this.btnUseWandOnMob.UseVisualStyleBackColor = true;
            this.btnUseWandOnMob.Click += new System.EventHandler(this.btnUseWandOnMob_Click);
            // 
            // btnPowerAttackMob
            // 
            this.btnPowerAttackMob.Location = new System.Drawing.Point(579, 535);
            this.btnPowerAttackMob.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPowerAttackMob.Name = "btnPowerAttackMob";
            this.btnPowerAttackMob.Size = new System.Drawing.Size(79, 28);
            this.btnPowerAttackMob.TabIndex = 54;
            this.btnPowerAttackMob.Text = "Power Atk";
            this.btnPowerAttackMob.UseVisualStyleBackColor = true;
            this.btnPowerAttackMob.Click += new System.EventHandler(this.btnPowerAttackMob_Click);
            // 
            // txtSetValue
            // 
            this.txtSetValue.Location = new System.Drawing.Point(244, 15);
            this.txtSetValue.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtSetValue.Name = "txtSetValue";
            this.txtSetValue.Size = new System.Drawing.Size(89, 20);
            this.txtSetValue.TabIndex = 56;
            // 
            // btnNortheast
            // 
            this.btnNortheast.Location = new System.Drawing.Point(87, 18);
            this.btnNortheast.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnNorth.Location = new System.Drawing.Point(48, 18);
            this.btnNorth.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnNorthwest.Location = new System.Drawing.Point(11, 18);
            this.btnNorthwest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnWest.Location = new System.Drawing.Point(11, 44);
            this.btnWest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnEast.Location = new System.Drawing.Point(87, 44);
            this.btnEast.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnSouthwest.Location = new System.Drawing.Point(11, 71);
            this.btnSouthwest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnSouth.Location = new System.Drawing.Point(48, 71);
            this.btnSouth.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnSoutheast.Location = new System.Drawing.Point(87, 71);
            this.btnSoutheast.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.tcMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(947, 832);
            this.tcMain.TabIndex = 79;
            this.tcMain.Selected += new System.Windows.Forms.TabControlEventHandler(this.tcMain_Selected);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.pnlMain);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabMain.Size = new System.Drawing.Size(939, 806);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.grpPermRuns);
            this.pnlMain.Controls.Add(this.btnGoToInventorySink);
            this.pnlMain.Controls.Add(this.btnGoToPawnShop);
            this.pnlMain.Controls.Add(this.lblGold);
            this.pnlMain.Controls.Add(this.lblToNextLevelValue);
            this.pnlMain.Controls.Add(this.grpInventory);
            this.pnlMain.Controls.Add(this.grpEquipment);
            this.pnlMain.Controls.Add(this.btnGoToHealingRoom);
            this.pnlMain.Controls.Add(this.lblArea);
            this.pnlMain.Controls.Add(this.cboArea);
            this.pnlMain.Controls.Add(this.grpCurrentRoom);
            this.pnlMain.Controls.Add(this.btnLocations);
            this.pnlMain.Controls.Add(this.btnIncrementWand);
            this.pnlMain.Controls.Add(this.grpSkillCooldowns);
            this.pnlMain.Controls.Add(this.lblAutoEscapeValue);
            this.pnlMain.Controls.Add(this.lblTime);
            this.pnlMain.Controls.Add(this.grpCurrentPlayer);
            this.pnlMain.Controls.Add(this.grpMessages);
            this.pnlMain.Controls.Add(this.grpMob);
            this.pnlMain.Controls.Add(this.btnGraph);
            this.pnlMain.Controls.Add(this.grpSingleMove);
            this.pnlMain.Controls.Add(this.grpSpells);
            this.pnlMain.Controls.Add(this.btnLevel3OffensiveSpell);
            this.pnlMain.Controls.Add(this.btnStunMob);
            this.pnlMain.Controls.Add(this.btnCastMend);
            this.pnlMain.Controls.Add(this.btnDrinkMend);
            this.pnlMain.Controls.Add(this.btnLevel1OffensiveSpell);
            this.pnlMain.Controls.Add(this.lblMob);
            this.pnlMain.Controls.Add(this.txtMob);
            this.pnlMain.Controls.Add(this.btnLevel2OffensiveSpell);
            this.pnlMain.Controls.Add(this.btnFlee);
            this.pnlMain.Controls.Add(this.btnDrinkHazy);
            this.pnlMain.Controls.Add(this.btnLookAtMob);
            this.pnlMain.Controls.Add(this.btnLook);
            this.pnlMain.Controls.Add(this.btnCastVigor);
            this.pnlMain.Controls.Add(this.btnCastCurePoison);
            this.pnlMain.Controls.Add(this.btnPowerAttackMob);
            this.pnlMain.Controls.Add(this.btnAbort);
            this.pnlMain.Controls.Add(this.btnUseWandOnMob);
            this.pnlMain.Controls.Add(this.txtWand);
            this.pnlMain.Controls.Add(this.btnAttackMob);
            this.pnlMain.Controls.Add(this.lblWand);
            this.pnlMain.Controls.Add(this.btnDrinkVigor);
            this.pnlMain.Controls.Add(this.btnDrinkCurepoison);
            this.pnlMain.Location = new System.Drawing.Point(2, 2);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1017, 900);
            this.pnlMain.TabIndex = 0;
            // 
            // grpPermRuns
            // 
            this.grpPermRuns.Controls.Add(this.btnFightAll);
            this.grpPermRuns.Controls.Add(this.btnFightOne);
            this.grpPermRuns.Controls.Add(this.btnAdHocPermRun);
            this.grpPermRuns.Controls.Add(this.btnNonCombatPermRun);
            this.grpPermRuns.Controls.Add(this.btnResumeCurrentPermRun);
            this.grpPermRuns.Controls.Add(this.btnCompleteCurrentPermRun);
            this.grpPermRuns.Controls.Add(this.btnRemoveNextPermRun);
            this.grpPermRuns.Controls.Add(this.btnRemoveCurrentPermRun);
            this.grpPermRuns.Controls.Add(this.btnPermRuns);
            this.grpPermRuns.Controls.Add(this.txtNextPermRun);
            this.grpPermRuns.Controls.Add(this.lblNextPermRun);
            this.grpPermRuns.Controls.Add(this.txtCurrentPermRun);
            this.grpPermRuns.Controls.Add(this.lblCurrent);
            this.grpPermRuns.Location = new System.Drawing.Point(190, 128);
            this.grpPermRuns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpPermRuns.Name = "grpPermRuns";
            this.grpPermRuns.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpPermRuns.Size = new System.Drawing.Size(456, 100);
            this.grpPermRuns.TabIndex = 156;
            this.grpPermRuns.TabStop = false;
            this.grpPermRuns.Text = "Perm Runs";
            // 
            // btnFightAll
            // 
            this.btnFightAll.Location = new System.Drawing.Point(369, 47);
            this.btnFightAll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnFightAll.Name = "btnFightAll";
            this.btnFightAll.Size = new System.Drawing.Size(80, 28);
            this.btnFightAll.TabIndex = 160;
            this.btnFightAll.Tag = "";
            this.btnFightAll.Text = "Fight All";
            this.btnFightAll.UseVisualStyleBackColor = true;
            this.btnFightAll.Click += new System.EventHandler(this.btnFightAll_Click);
            // 
            // btnFightOne
            // 
            this.btnFightOne.Location = new System.Drawing.Point(369, 16);
            this.btnFightOne.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnFightOne.Name = "btnFightOne";
            this.btnFightOne.Size = new System.Drawing.Size(80, 28);
            this.btnFightOne.TabIndex = 159;
            this.btnFightOne.Tag = "";
            this.btnFightOne.Text = "Fight One";
            this.btnFightOne.UseVisualStyleBackColor = true;
            this.btnFightOne.Click += new System.EventHandler(this.btnFightOne_Click);
            // 
            // btnAdHocPermRun
            // 
            this.btnAdHocPermRun.Location = new System.Drawing.Point(284, 47);
            this.btnAdHocPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAdHocPermRun.Name = "btnAdHocPermRun";
            this.btnAdHocPermRun.Size = new System.Drawing.Size(80, 28);
            this.btnAdHocPermRun.TabIndex = 158;
            this.btnAdHocPermRun.Tag = "";
            this.btnAdHocPermRun.Text = "Ad Hoc";
            this.btnAdHocPermRun.UseVisualStyleBackColor = true;
            this.btnAdHocPermRun.Click += new System.EventHandler(this.btnAdHocPermRun_Click);
            // 
            // btnNonCombatPermRun
            // 
            this.btnNonCombatPermRun.Location = new System.Drawing.Point(284, 16);
            this.btnNonCombatPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnNonCombatPermRun.Name = "btnNonCombatPermRun";
            this.btnNonCombatPermRun.Size = new System.Drawing.Size(80, 28);
            this.btnNonCombatPermRun.TabIndex = 157;
            this.btnNonCombatPermRun.Tag = "";
            this.btnNonCombatPermRun.Text = "Non Combat";
            this.btnNonCombatPermRun.UseVisualStyleBackColor = true;
            this.btnNonCombatPermRun.Click += new System.EventHandler(this.btnNonCombatPermRun_Click);
            // 
            // btnResumeCurrentPermRun
            // 
            this.btnResumeCurrentPermRun.Location = new System.Drawing.Point(194, 62);
            this.btnResumeCurrentPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnResumeCurrentPermRun.Name = "btnResumeCurrentPermRun";
            this.btnResumeCurrentPermRun.Size = new System.Drawing.Size(66, 28);
            this.btnResumeCurrentPermRun.TabIndex = 156;
            this.btnResumeCurrentPermRun.Tag = "";
            this.btnResumeCurrentPermRun.Text = "Resume";
            this.btnResumeCurrentPermRun.UseVisualStyleBackColor = true;
            this.btnResumeCurrentPermRun.Click += new System.EventHandler(this.btnResumeCurrentPermRun_Click);
            // 
            // btnCompleteCurrentPermRun
            // 
            this.btnCompleteCurrentPermRun.Location = new System.Drawing.Point(123, 62);
            this.btnCompleteCurrentPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCompleteCurrentPermRun.Name = "btnCompleteCurrentPermRun";
            this.btnCompleteCurrentPermRun.Size = new System.Drawing.Size(66, 28);
            this.btnCompleteCurrentPermRun.TabIndex = 155;
            this.btnCompleteCurrentPermRun.Tag = "";
            this.btnCompleteCurrentPermRun.Text = "Complete";
            this.btnCompleteCurrentPermRun.UseVisualStyleBackColor = true;
            this.btnCompleteCurrentPermRun.Click += new System.EventHandler(this.btnCompleteCurrentPermRun_Click);
            // 
            // btnRemoveNextPermRun
            // 
            this.btnRemoveNextPermRun.Location = new System.Drawing.Point(260, 40);
            this.btnRemoveNextPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRemoveNextPermRun.Name = "btnRemoveNextPermRun";
            this.btnRemoveNextPermRun.Size = new System.Drawing.Size(20, 18);
            this.btnRemoveNextPermRun.TabIndex = 5;
            this.btnRemoveNextPermRun.Text = "X";
            this.btnRemoveNextPermRun.UseVisualStyleBackColor = true;
            this.btnRemoveNextPermRun.Click += new System.EventHandler(this.btnRemoveNextPermRun_Click);
            // 
            // btnRemoveCurrentPermRun
            // 
            this.btnRemoveCurrentPermRun.Location = new System.Drawing.Point(260, 17);
            this.btnRemoveCurrentPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRemoveCurrentPermRun.Name = "btnRemoveCurrentPermRun";
            this.btnRemoveCurrentPermRun.Size = new System.Drawing.Size(20, 18);
            this.btnRemoveCurrentPermRun.TabIndex = 4;
            this.btnRemoveCurrentPermRun.Text = "X";
            this.btnRemoveCurrentPermRun.UseVisualStyleBackColor = true;
            this.btnRemoveCurrentPermRun.Click += new System.EventHandler(this.btnRemoveCurrentPermRun_Click);
            // 
            // btnPermRuns
            // 
            this.btnPermRuns.Location = new System.Drawing.Point(52, 62);
            this.btnPermRuns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPermRuns.Name = "btnPermRuns";
            this.btnPermRuns.Size = new System.Drawing.Size(66, 28);
            this.btnPermRuns.TabIndex = 154;
            this.btnPermRuns.Tag = "";
            this.btnPermRuns.Text = "Edit";
            this.btnPermRuns.UseVisualStyleBackColor = true;
            this.btnPermRuns.Click += new System.EventHandler(this.btnPermRuns_Click);
            // 
            // txtNextPermRun
            // 
            this.txtNextPermRun.Enabled = false;
            this.txtNextPermRun.Location = new System.Drawing.Point(52, 40);
            this.txtNextPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtNextPermRun.Name = "txtNextPermRun";
            this.txtNextPermRun.Size = new System.Drawing.Size(204, 20);
            this.txtNextPermRun.TabIndex = 3;
            // 
            // lblNextPermRun
            // 
            this.lblNextPermRun.AutoSize = true;
            this.lblNextPermRun.Location = new System.Drawing.Point(8, 42);
            this.lblNextPermRun.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNextPermRun.Name = "lblNextPermRun";
            this.lblNextPermRun.Size = new System.Drawing.Size(32, 13);
            this.lblNextPermRun.TabIndex = 2;
            this.lblNextPermRun.Text = "Next:";
            // 
            // txtCurrentPermRun
            // 
            this.txtCurrentPermRun.Enabled = false;
            this.txtCurrentPermRun.Location = new System.Drawing.Point(52, 17);
            this.txtCurrentPermRun.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtCurrentPermRun.Name = "txtCurrentPermRun";
            this.txtCurrentPermRun.Size = new System.Drawing.Size(204, 20);
            this.txtCurrentPermRun.TabIndex = 1;
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Location = new System.Drawing.Point(8, 20);
            this.lblCurrent.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(44, 13);
            this.lblCurrent.TabIndex = 0;
            this.lblCurrent.Text = "Current:";
            // 
            // btnGoToInventorySink
            // 
            this.btnGoToInventorySink.Enabled = false;
            this.btnGoToInventorySink.Location = new System.Drawing.Point(180, 89);
            this.btnGoToInventorySink.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGoToInventorySink.Name = "btnGoToInventorySink";
            this.btnGoToInventorySink.Size = new System.Drawing.Size(51, 23);
            this.btnGoToInventorySink.TabIndex = 155;
            this.btnGoToInventorySink.Tag = "";
            this.btnGoToInventorySink.Text = "Inv";
            this.btnGoToInventorySink.UseVisualStyleBackColor = true;
            this.btnGoToInventorySink.Click += new System.EventHandler(this.btnGoToInventorySink_Click);
            // 
            // btnGoToPawnShop
            // 
            this.btnGoToPawnShop.Enabled = false;
            this.btnGoToPawnShop.Location = new System.Drawing.Point(125, 89);
            this.btnGoToPawnShop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGoToPawnShop.Name = "btnGoToPawnShop";
            this.btnGoToPawnShop.Size = new System.Drawing.Size(51, 23);
            this.btnGoToPawnShop.TabIndex = 151;
            this.btnGoToPawnShop.Tag = "";
            this.btnGoToPawnShop.Text = "Pawn";
            this.btnGoToPawnShop.UseVisualStyleBackColor = true;
            this.btnGoToPawnShop.Click += new System.EventHandler(this.btnGoToPawnShop_Click);
            // 
            // lblGold
            // 
            this.lblGold.BackColor = System.Drawing.Color.YellowGreen;
            this.lblGold.Location = new System.Drawing.Point(538, 52);
            this.lblGold.Name = "lblGold";
            this.lblGold.Size = new System.Drawing.Size(108, 18);
            this.lblGold.TabIndex = 148;
            this.lblGold.Text = "Value";
            this.lblGold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblToNextLevelValue
            // 
            this.lblToNextLevelValue.BackColor = System.Drawing.Color.LightGray;
            this.lblToNextLevelValue.Location = new System.Drawing.Point(538, 32);
            this.lblToNextLevelValue.Name = "lblToNextLevelValue";
            this.lblToNextLevelValue.Size = new System.Drawing.Size(108, 18);
            this.lblToNextLevelValue.TabIndex = 131;
            this.lblToNextLevelValue.Text = "Value";
            this.lblToNextLevelValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpInventory
            // 
            this.grpInventory.Controls.Add(this.lstInventory);
            this.grpInventory.Location = new System.Drawing.Point(227, 464);
            this.grpInventory.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpInventory.Name = "grpInventory";
            this.grpInventory.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpInventory.Size = new System.Drawing.Size(182, 309);
            this.grpInventory.TabIndex = 147;
            this.grpInventory.TabStop = false;
            this.grpInventory.Text = "Inventory";
            // 
            // lstInventory
            // 
            this.lstInventory.ContextMenuStrip = this.ctxInventoryOrEquipmentItem;
            this.lstInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstInventory.FormattingEnabled = true;
            this.lstInventory.Location = new System.Drawing.Point(2, 15);
            this.lstInventory.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstInventory.Name = "lstInventory";
            this.lstInventory.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstInventory.Size = new System.Drawing.Size(178, 292);
            this.lstInventory.TabIndex = 0;
            // 
            // ctxInventoryOrEquipmentItem
            // 
            this.ctxInventoryOrEquipmentItem.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxInventoryOrEquipmentItem.Name = "ctxInventoryOrEquipmentItem";
            this.ctxInventoryOrEquipmentItem.Size = new System.Drawing.Size(61, 4);
            this.ctxInventoryOrEquipmentItem.Opening += new System.ComponentModel.CancelEventHandler(this.ctxInventoryOrEquipmentItem_Opening);
            this.ctxInventoryOrEquipmentItem.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxInventoryOrEquipmentItem_ItemClicked);
            // 
            // grpEquipment
            // 
            this.grpEquipment.Controls.Add(this.lstEquipment);
            this.grpEquipment.Location = new System.Drawing.Point(23, 464);
            this.grpEquipment.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpEquipment.Name = "grpEquipment";
            this.grpEquipment.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpEquipment.Size = new System.Drawing.Size(200, 309);
            this.grpEquipment.TabIndex = 146;
            this.grpEquipment.TabStop = false;
            this.grpEquipment.Text = "Equipment";
            // 
            // lstEquipment
            // 
            this.lstEquipment.ContextMenuStrip = this.ctxInventoryOrEquipmentItem;
            this.lstEquipment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEquipment.FormattingEnabled = true;
            this.lstEquipment.Location = new System.Drawing.Point(2, 15);
            this.lstEquipment.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstEquipment.Name = "lstEquipment";
            this.lstEquipment.Size = new System.Drawing.Size(196, 292);
            this.lstEquipment.TabIndex = 0;
            // 
            // btnGoToHealingRoom
            // 
            this.btnGoToHealingRoom.Enabled = false;
            this.btnGoToHealingRoom.Location = new System.Drawing.Point(70, 89);
            this.btnGoToHealingRoom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGoToHealingRoom.Name = "btnGoToHealingRoom";
            this.btnGoToHealingRoom.Size = new System.Drawing.Size(51, 23);
            this.btnGoToHealingRoom.TabIndex = 145;
            this.btnGoToHealingRoom.Tag = "";
            this.btnGoToHealingRoom.Text = "Tick";
            this.btnGoToHealingRoom.UseVisualStyleBackColor = true;
            this.btnGoToHealingRoom.Click += new System.EventHandler(this.btnGoToHealingRoom_Click);
            // 
            // lblArea
            // 
            this.lblArea.AutoSize = true;
            this.lblArea.Location = new System.Drawing.Point(14, 67);
            this.lblArea.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(32, 13);
            this.lblArea.TabIndex = 144;
            this.lblArea.Text = "Area:";
            this.lblArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboArea
            // 
            this.cboArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboArea.FormattingEnabled = true;
            this.cboArea.Location = new System.Drawing.Point(70, 63);
            this.cboArea.Name = "cboArea";
            this.cboArea.Size = new System.Drawing.Size(161, 21);
            this.cboArea.TabIndex = 143;
            this.cboArea.SelectedIndexChanged += new System.EventHandler(this.cboArea_SelectedIndexChanged);
            // 
            // grpCurrentRoom
            // 
            this.grpCurrentRoom.Controls.Add(this.treeCurrentRoom);
            this.grpCurrentRoom.Location = new System.Drawing.Point(656, 10);
            this.grpCurrentRoom.Name = "grpCurrentRoom";
            this.grpCurrentRoom.Size = new System.Drawing.Size(281, 454);
            this.grpCurrentRoom.TabIndex = 141;
            this.grpCurrentRoom.TabStop = false;
            this.grpCurrentRoom.Text = "Current Room";
            // 
            // treeCurrentRoom
            // 
            this.treeCurrentRoom.ContextMenuStrip = this.ctxCurrentRoom;
            this.treeCurrentRoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeCurrentRoom.Location = new System.Drawing.Point(3, 16);
            this.treeCurrentRoom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.treeCurrentRoom.Name = "treeCurrentRoom";
            this.treeCurrentRoom.Size = new System.Drawing.Size(275, 435);
            this.treeCurrentRoom.TabIndex = 0;
            this.treeCurrentRoom.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeCurrentRoom_AfterCollapse);
            this.treeCurrentRoom.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeCurrentRoom_AfterExpand);
            this.treeCurrentRoom.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeCurrentRoom_AfterSelect);
            this.treeCurrentRoom.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeCurrentRoom_NodeMouseClick);
            this.treeCurrentRoom.DoubleClick += new System.EventHandler(this.treeCurrentRoom_DoubleClick);
            // 
            // ctxCurrentRoom
            // 
            this.ctxCurrentRoom.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxCurrentRoom.Name = "ctxCurrentRoom";
            this.ctxCurrentRoom.Size = new System.Drawing.Size(61, 4);
            this.ctxCurrentRoom.Opening += new System.ComponentModel.CancelEventHandler(this.ctxCurrentRoom_Opening);
            this.ctxCurrentRoom.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxCurrentRoom_ItemClicked);
            // 
            // btnLocations
            // 
            this.btnLocations.Location = new System.Drawing.Point(450, 42);
            this.btnLocations.Name = "btnLocations";
            this.btnLocations.Size = new System.Drawing.Size(79, 28);
            this.btnLocations.TabIndex = 140;
            this.btnLocations.Text = "Locations";
            this.btnLocations.UseVisualStyleBackColor = true;
            this.btnLocations.Click += new System.EventHandler(this.btnLocations_Click);
            // 
            // btnIncrementWand
            // 
            this.btnIncrementWand.Enabled = false;
            this.btnIncrementWand.Location = new System.Drawing.Point(234, 41);
            this.btnIncrementWand.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnIncrementWand.Name = "btnIncrementWand";
            this.btnIncrementWand.Size = new System.Drawing.Size(34, 23);
            this.btnIncrementWand.TabIndex = 139;
            this.btnIncrementWand.Text = "Inc";
            this.btnIncrementWand.UseVisualStyleBackColor = true;
            this.btnIncrementWand.Click += new System.EventHandler(this.btnIncrementWand_Click);
            // 
            // grpSkillCooldowns
            // 
            this.grpSkillCooldowns.Location = new System.Drawing.Point(413, 464);
            this.grpSkillCooldowns.Name = "grpSkillCooldowns";
            this.grpSkillCooldowns.Size = new System.Drawing.Size(153, 153);
            this.grpSkillCooldowns.TabIndex = 138;
            this.grpSkillCooldowns.TabStop = false;
            this.grpSkillCooldowns.Text = "Skill Cooldowns";
            // 
            // lblAutoEscapeValue
            // 
            this.lblAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblAutoEscapeValue.ContextMenuStrip = this.ctxAutoEscape;
            this.lblAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblAutoEscapeValue.Location = new System.Drawing.Point(538, 72);
            this.lblAutoEscapeValue.Name = "lblAutoEscapeValue";
            this.lblAutoEscapeValue.Size = new System.Drawing.Size(108, 18);
            this.lblAutoEscapeValue.TabIndex = 134;
            this.lblAutoEscapeValue.Text = "Auto Escape";
            this.lblAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxAutoEscape
            // 
            this.ctxAutoEscape.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoEscape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAutoEscapeIsActive,
            this.tsmiAutoEscapeSeparator1,
            this.tsmiSetAutoEscapeThreshold,
            this.tsmiClearAutoEscapeThreshold,
            this.tsmiAutoEscapeSeparator2,
            this.tsmiAutoEscapeFlee,
            this.tsmiAutoEscapeHazy});
            this.ctxAutoEscape.Name = "ctxAutoEscape";
            this.ctxAutoEscape.Size = new System.Drawing.Size(157, 126);
            this.ctxAutoEscape.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoEscape_Opening);
            // 
            // tsmiAutoEscapeIsActive
            // 
            this.tsmiAutoEscapeIsActive.Name = "tsmiAutoEscapeIsActive";
            this.tsmiAutoEscapeIsActive.Size = new System.Drawing.Size(156, 22);
            this.tsmiAutoEscapeIsActive.Text = "Is Active?";
            this.tsmiAutoEscapeIsActive.Click += new System.EventHandler(this.tsmiToggleAutoEscapeActive_Click);
            // 
            // tsmiAutoEscapeSeparator1
            // 
            this.tsmiAutoEscapeSeparator1.Name = "tsmiAutoEscapeSeparator1";
            this.tsmiAutoEscapeSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // tsmiSetAutoEscapeThreshold
            // 
            this.tsmiSetAutoEscapeThreshold.Name = "tsmiSetAutoEscapeThreshold";
            this.tsmiSetAutoEscapeThreshold.Size = new System.Drawing.Size(156, 22);
            this.tsmiSetAutoEscapeThreshold.Text = "Set Threshold";
            this.tsmiSetAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiSetAutoEscapeThreshold_Click);
            // 
            // tsmiClearAutoEscapeThreshold
            // 
            this.tsmiClearAutoEscapeThreshold.Name = "tsmiClearAutoEscapeThreshold";
            this.tsmiClearAutoEscapeThreshold.Size = new System.Drawing.Size(156, 22);
            this.tsmiClearAutoEscapeThreshold.Text = "Clear Threshold";
            this.tsmiClearAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiClearAutoEscapeThreshold_Click);
            // 
            // tsmiAutoEscapeSeparator2
            // 
            this.tsmiAutoEscapeSeparator2.Name = "tsmiAutoEscapeSeparator2";
            this.tsmiAutoEscapeSeparator2.Size = new System.Drawing.Size(153, 6);
            // 
            // tsmiAutoEscapeFlee
            // 
            this.tsmiAutoEscapeFlee.Name = "tsmiAutoEscapeFlee";
            this.tsmiAutoEscapeFlee.Size = new System.Drawing.Size(156, 22);
            this.tsmiAutoEscapeFlee.Text = "Flee";
            this.tsmiAutoEscapeFlee.Click += new System.EventHandler(this.tsmiAutoEscapeFlee_Click);
            // 
            // tsmiAutoEscapeHazy
            // 
            this.tsmiAutoEscapeHazy.Name = "tsmiAutoEscapeHazy";
            this.tsmiAutoEscapeHazy.Size = new System.Drawing.Size(156, 22);
            this.tsmiAutoEscapeHazy.Text = "Hazy";
            this.tsmiAutoEscapeHazy.Click += new System.EventHandler(this.tsmiAutoEscapeHazy_Click);
            // 
            // lblTime
            // 
            this.lblTime.BackColor = System.Drawing.Color.LightGray;
            this.lblTime.Location = new System.Drawing.Point(538, 11);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(108, 18);
            this.lblTime.TabIndex = 125;
            this.lblTime.Text = "Time";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpCurrentPlayer
            // 
            this.grpCurrentPlayer.Controls.Add(this.lblManaValue);
            this.grpCurrentPlayer.Controls.Add(this.lblHitpointsValue);
            this.grpCurrentPlayer.Controls.Add(this.lblMana);
            this.grpCurrentPlayer.Controls.Add(this.lblHitpoints);
            this.grpCurrentPlayer.Location = new System.Drawing.Point(748, 470);
            this.grpCurrentPlayer.Name = "grpCurrentPlayer";
            this.grpCurrentPlayer.Size = new System.Drawing.Size(188, 63);
            this.grpCurrentPlayer.TabIndex = 122;
            this.grpCurrentPlayer.TabStop = false;
            this.grpCurrentPlayer.Text = "Current Player";
            // 
            // lblManaValue
            // 
            this.lblManaValue.BackColor = System.Drawing.Color.LightGray;
            this.lblManaValue.Location = new System.Drawing.Point(97, 36);
            this.lblManaValue.Name = "lblManaValue";
            this.lblManaValue.Size = new System.Drawing.Size(86, 15);
            this.lblManaValue.TabIndex = 127;
            this.lblManaValue.Text = "Value";
            this.lblManaValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHitpointsValue
            // 
            this.lblHitpointsValue.BackColor = System.Drawing.Color.LightGray;
            this.lblHitpointsValue.Location = new System.Drawing.Point(97, 17);
            this.lblHitpointsValue.Name = "lblHitpointsValue";
            this.lblHitpointsValue.Size = new System.Drawing.Size(86, 15);
            this.lblHitpointsValue.TabIndex = 126;
            this.lblHitpointsValue.Text = "Value";
            this.lblHitpointsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Location = new System.Drawing.Point(36, 36);
            this.lblMana.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMana.Name = "lblMana";
            this.lblMana.Size = new System.Drawing.Size(37, 13);
            this.lblMana.TabIndex = 92;
            this.lblMana.Text = "Mana:";
            this.lblMana.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHitpoints
            // 
            this.lblHitpoints.AutoSize = true;
            this.lblHitpoints.Location = new System.Drawing.Point(36, 17);
            this.lblHitpoints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHitpoints.Name = "lblHitpoints";
            this.lblHitpoints.Size = new System.Drawing.Size(51, 13);
            this.lblHitpoints.TabIndex = 100;
            this.lblHitpoints.Text = "Hitpoints:";
            this.lblHitpoints.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpMessages
            // 
            this.grpMessages.Controls.Add(this.lstMessages);
            this.grpMessages.Location = new System.Drawing.Point(26, 234);
            this.grpMessages.Name = "grpMessages";
            this.grpMessages.Size = new System.Drawing.Size(598, 216);
            this.grpMessages.TabIndex = 121;
            this.grpMessages.TabStop = false;
            this.grpMessages.Text = "Messages";
            // 
            // lstMessages
            // 
            this.lstMessages.ContextMenuStrip = this.ctxMessages;
            this.lstMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMessages.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.Location = new System.Drawing.Point(3, 16);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMessages.Size = new System.Drawing.Size(592, 197);
            this.lstMessages.TabIndex = 0;
            // 
            // ctxMessages
            // 
            this.ctxMessages.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxMessages.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyMessages});
            this.ctxMessages.Name = "ctxMessages";
            this.ctxMessages.Size = new System.Drawing.Size(103, 26);
            this.ctxMessages.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMessages_Opening);
            this.ctxMessages.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxMessages_ItemClicked);
            // 
            // tsmiCopyMessages
            // 
            this.tsmiCopyMessages.Name = "tsmiCopyMessages";
            this.tsmiCopyMessages.Size = new System.Drawing.Size(102, 22);
            this.tsmiCopyMessages.Text = "Copy";
            // 
            // grpMob
            // 
            this.grpMob.Controls.Add(this.txtMobStatus);
            this.grpMob.Controls.Add(this.lblMobStatus);
            this.grpMob.Controls.Add(this.txtMobDamage);
            this.grpMob.Controls.Add(this.lblMobDamage);
            this.grpMob.Location = new System.Drawing.Point(747, 531);
            this.grpMob.Name = "grpMob";
            this.grpMob.Size = new System.Drawing.Size(191, 74);
            this.grpMob.TabIndex = 120;
            this.grpMob.TabStop = false;
            this.grpMob.Text = "Mob";
            // 
            // txtMobStatus
            // 
            this.txtMobStatus.Location = new System.Drawing.Point(98, 44);
            this.txtMobStatus.Name = "txtMobStatus";
            this.txtMobStatus.ReadOnly = true;
            this.txtMobStatus.Size = new System.Drawing.Size(86, 20);
            this.txtMobStatus.TabIndex = 3;
            // 
            // lblMobStatus
            // 
            this.lblMobStatus.AutoSize = true;
            this.lblMobStatus.Location = new System.Drawing.Point(37, 47);
            this.lblMobStatus.Name = "lblMobStatus";
            this.lblMobStatus.Size = new System.Drawing.Size(40, 13);
            this.lblMobStatus.TabIndex = 2;
            this.lblMobStatus.Text = "Status:";
            // 
            // txtMobDamage
            // 
            this.txtMobDamage.Location = new System.Drawing.Point(98, 19);
            this.txtMobDamage.Name = "txtMobDamage";
            this.txtMobDamage.ReadOnly = true;
            this.txtMobDamage.Size = new System.Drawing.Size(86, 20);
            this.txtMobDamage.TabIndex = 1;
            // 
            // lblMobDamage
            // 
            this.lblMobDamage.AutoSize = true;
            this.lblMobDamage.Location = new System.Drawing.Point(37, 22);
            this.lblMobDamage.Name = "lblMobDamage";
            this.lblMobDamage.Size = new System.Drawing.Size(50, 13);
            this.lblMobDamage.TabIndex = 0;
            this.lblMobDamage.Text = "Damage:";
            // 
            // btnGraph
            // 
            this.btnGraph.Location = new System.Drawing.Point(450, 11);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(79, 28);
            this.btnGraph.TabIndex = 119;
            this.btnGraph.Text = "Graph";
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // grpSingleMove
            // 
            this.grpSingleMove.Controls.Add(this.btnOut);
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
            this.grpSingleMove.Location = new System.Drawing.Point(273, 18);
            this.grpSingleMove.Name = "grpSingleMove";
            this.grpSingleMove.Size = new System.Drawing.Size(171, 105);
            this.grpSingleMove.TabIndex = 117;
            this.grpSingleMove.TabStop = false;
            this.grpSingleMove.Text = "Single Move";
            // 
            // btnOut
            // 
            this.btnOut.Location = new System.Drawing.Point(125, 71);
            this.btnOut.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(34, 23);
            this.btnOut.TabIndex = 113;
            this.btnOut.Tag = "out";
            this.btnOut.Text = "Out";
            this.btnOut.UseVisualStyleBackColor = true;
            this.btnOut.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnOtherSingleMove
            // 
            this.btnOtherSingleMove.Location = new System.Drawing.Point(48, 43);
            this.btnOtherSingleMove.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOtherSingleMove.Name = "btnOtherSingleMove";
            this.btnOtherSingleMove.Size = new System.Drawing.Size(34, 23);
            this.btnOtherSingleMove.TabIndex = 112;
            this.btnOtherSingleMove.Tag = "";
            this.btnOtherSingleMove.Text = "?";
            this.btnOtherSingleMove.UseVisualStyleBackColor = true;
            this.btnOtherSingleMove.Click += new System.EventHandler(this.btnOtherSingleMove_Click);
            // 
            // btnDn
            // 
            this.btnDn.Location = new System.Drawing.Point(125, 44);
            this.btnDn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnUp.Location = new System.Drawing.Point(125, 18);
            this.btnUp.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(34, 23);
            this.btnUp.TabIndex = 110;
            this.btnUp.Tag = "up";
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // grpSpells
            // 
            this.grpSpells.Controls.Add(this.flpSpells);
            this.grpSpells.Location = new System.Drawing.Point(413, 622);
            this.grpSpells.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpSpells.Name = "grpSpells";
            this.grpSpells.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpSpells.Size = new System.Drawing.Size(153, 153);
            this.grpSpells.TabIndex = 108;
            this.grpSpells.TabStop = false;
            this.grpSpells.Text = "Active Spells";
            // 
            // flpSpells
            // 
            this.flpSpells.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSpells.Location = new System.Drawing.Point(2, 15);
            this.flpSpells.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpSpells.Name = "flpSpells";
            this.flpSpells.Size = new System.Drawing.Size(149, 136);
            this.flpSpells.TabIndex = 0;
            // 
            // btnLevel3OffensiveSpell
            // 
            this.btnLevel3OffensiveSpell.Location = new System.Drawing.Point(662, 535);
            this.btnLevel3OffensiveSpell.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLevel3OffensiveSpell.Name = "btnLevel3OffensiveSpell";
            this.btnLevel3OffensiveSpell.Size = new System.Drawing.Size(81, 28);
            this.btnLevel3OffensiveSpell.TabIndex = 84;
            this.btnLevel3OffensiveSpell.Text = "Cast Level 3";
            this.btnLevel3OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel3OffensiveSpell.Click += new System.EventHandler(this.btnLevel3OffensiveSpell_Click);
            // 
            // btnStunMob
            // 
            this.btnStunMob.Location = new System.Drawing.Point(579, 474);
            this.btnStunMob.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStunMob.Name = "btnStunMob";
            this.btnStunMob.Size = new System.Drawing.Size(79, 28);
            this.btnStunMob.TabIndex = 83;
            this.btnStunMob.Text = "Stun";
            this.btnStunMob.UseVisualStyleBackColor = true;
            this.btnStunMob.Click += new System.EventHandler(this.btnStun_Click);
            // 
            // btnCastMend
            // 
            this.btnCastMend.Location = new System.Drawing.Point(579, 659);
            this.btnCastMend.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCastMend.Name = "btnCastMend";
            this.btnCastMend.Size = new System.Drawing.Size(80, 28);
            this.btnCastMend.TabIndex = 82;
            this.btnCastMend.Text = "Mend";
            this.btnCastMend.UseVisualStyleBackColor = true;
            this.btnCastMend.Click += new System.EventHandler(this.btnMendWounds_Click);
            // 
            // btnDrinkMend
            // 
            this.btnDrinkMend.Location = new System.Drawing.Point(664, 659);
            this.btnDrinkMend.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDrinkMend.Name = "btnDrinkMend";
            this.btnDrinkMend.Size = new System.Drawing.Size(79, 28);
            this.btnDrinkMend.TabIndex = 81;
            this.btnDrinkMend.Text = "potion";
            this.btnDrinkMend.UseVisualStyleBackColor = true;
            this.btnDrinkMend.Click += new System.EventHandler(this.btnMendPotion_Click);
            // 
            // tabAncillary
            // 
            this.tabAncillary.Controls.Add(this.pnlAncillary);
            this.tabAncillary.Location = new System.Drawing.Point(4, 22);
            this.tabAncillary.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabAncillary.Name = "tabAncillary";
            this.tabAncillary.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabAncillary.Size = new System.Drawing.Size(939, 811);
            this.tabAncillary.TabIndex = 1;
            this.tabAncillary.Text = "Ancillary";
            this.tabAncillary.UseVisualStyleBackColor = true;
            // 
            // pnlAncillary
            // 
            this.pnlAncillary.Controls.Add(this.txtSetValue);
            this.pnlAncillary.Controls.Add(this.btnSet);
            this.pnlAncillary.Controls.Add(this.cboSetOption);
            this.pnlAncillary.Controls.Add(this.chkSetOn);
            this.pnlAncillary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAncillary.Location = new System.Drawing.Point(2, 2);
            this.pnlAncillary.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlAncillary.Name = "pnlAncillary";
            this.pnlAncillary.Size = new System.Drawing.Size(935, 807);
            this.pnlAncillary.TabIndex = 0;
            // 
            // tabEmotes
            // 
            this.tabEmotes.Controls.Add(this.pnlEmotes);
            this.tabEmotes.Location = new System.Drawing.Point(4, 22);
            this.tabEmotes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabEmotes.Name = "tabEmotes";
            this.tabEmotes.Size = new System.Drawing.Size(939, 811);
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
            this.pnlEmotes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlEmotes.Name = "pnlEmotes";
            this.pnlEmotes.Size = new System.Drawing.Size(939, 811);
            this.pnlEmotes.TabIndex = 12;
            // 
            // btnSay
            // 
            this.btnSay.Location = new System.Drawing.Point(349, 9);
            this.btnSay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.txtEmoteTarget.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.btnEmote.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.txtCommandText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtCommandText.Name = "txtCommandText";
            this.txtCommandText.Size = new System.Drawing.Size(187, 20);
            this.txtCommandText.TabIndex = 9;
            this.txtCommandText.TextChanged += new System.EventHandler(this.txtEmoteText_TextChanged);
            // 
            // grpEmotes
            // 
            this.grpEmotes.Controls.Add(this.flpEmotes);
            this.grpEmotes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpEmotes.Location = new System.Drawing.Point(0, -36);
            this.grpEmotes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpEmotes.Name = "grpEmotes";
            this.grpEmotes.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpEmotes.Size = new System.Drawing.Size(939, 847);
            this.grpEmotes.TabIndex = 10;
            this.grpEmotes.TabStop = false;
            this.grpEmotes.Text = "Emotes";
            // 
            // flpEmotes
            // 
            this.flpEmotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpEmotes.Location = new System.Drawing.Point(2, 15);
            this.flpEmotes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpEmotes.Name = "flpEmotes";
            this.flpEmotes.Size = new System.Drawing.Size(935, 830);
            this.flpEmotes.TabIndex = 0;
            // 
            // tabHelp
            // 
            this.tabHelp.Controls.Add(this.grpHelp);
            this.tabHelp.Location = new System.Drawing.Point(4, 22);
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Size = new System.Drawing.Size(939, 811);
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
            this.grpHelp.Size = new System.Drawing.Size(939, 811);
            this.grpHelp.TabIndex = 0;
            this.grpHelp.TabStop = false;
            this.grpHelp.Text = "Help";
            // 
            // flpHelp
            // 
            this.flpHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpHelp.Location = new System.Drawing.Point(3, 16);
            this.flpHelp.Name = "flpHelp";
            this.flpHelp.Size = new System.Drawing.Size(933, 792);
            this.flpHelp.TabIndex = 0;
            // 
            // pnlOverallLeft
            // 
            this.pnlOverallLeft.Controls.Add(this.pnlTabControl);
            this.pnlOverallLeft.Controls.Add(this.tsTopMenu);
            this.pnlOverallLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlOverallLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlOverallLeft.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlOverallLeft.Name = "pnlOverallLeft";
            this.pnlOverallLeft.Size = new System.Drawing.Size(947, 857);
            this.pnlOverallLeft.TabIndex = 0;
            // 
            // pnlTabControl
            // 
            this.pnlTabControl.Controls.Add(this.tcMain);
            this.pnlTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTabControl.Location = new System.Drawing.Point(0, 25);
            this.pnlTabControl.Name = "pnlTabControl";
            this.pnlTabControl.Size = new System.Drawing.Size(947, 832);
            this.pnlTabControl.TabIndex = 81;
            // 
            // tsTopMenu
            // 
            this.tsTopMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbInformation,
            this.tsbInventoryAndEquipment,
            this.tsbRemoveAll,
            this.tsbWearAll,
            this.tsbWho,
            this.tsbUptime,
            this.tsbSpells,
            this.tsbScore,
            this.tsbTime,
            this.tsddActions,
            this.tsddbSettings,
            this.tsbReloadMap,
            this.tsbQuit,
            this.tsbLogout});
            this.tsTopMenu.Location = new System.Drawing.Point(0, 0);
            this.tsTopMenu.Name = "tsTopMenu";
            this.tsTopMenu.Size = new System.Drawing.Size(947, 25);
            this.tsTopMenu.TabIndex = 80;
            this.tsTopMenu.Text = "toolStrip1";
            // 
            // tsbInformation
            // 
            this.tsbInformation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInformation.Image = ((System.Drawing.Image)(resources.GetObject("tsbInformation.Image")));
            this.tsbInformation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInformation.Name = "tsbInformation";
            this.tsbInformation.Size = new System.Drawing.Size(74, 22);
            this.tsbInformation.Tag = "information";
            this.tsbInformation.Text = "Information";
            this.tsbInformation.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbInventoryAndEquipment
            // 
            this.tsbInventoryAndEquipment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInventoryAndEquipment.Image = ((System.Drawing.Image)(resources.GetObject("tsbInventoryAndEquipment.Image")));
            this.tsbInventoryAndEquipment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInventoryAndEquipment.Name = "tsbInventoryAndEquipment";
            this.tsbInventoryAndEquipment.Size = new System.Drawing.Size(127, 22);
            this.tsbInventoryAndEquipment.Text = "Inventory+Equipment";
            this.tsbInventoryAndEquipment.Click += new System.EventHandler(this.tsbInventoryAndEquipment_Click);
            // 
            // tsbRemoveAll
            // 
            this.tsbRemoveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbRemoveAll.Image = ((System.Drawing.Image)(resources.GetObject("tsbRemoveAll.Image")));
            this.tsbRemoveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemoveAll.Name = "tsbRemoveAll";
            this.tsbRemoveAll.Size = new System.Drawing.Size(71, 22);
            this.tsbRemoveAll.Tag = "remove all";
            this.tsbRemoveAll.Text = "Remove All";
            this.tsbRemoveAll.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbWearAll
            // 
            this.tsbWearAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbWearAll.Image = ((System.Drawing.Image)(resources.GetObject("tsbWearAll.Image")));
            this.tsbWearAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWearAll.Name = "tsbWearAll";
            this.tsbWearAll.Size = new System.Drawing.Size(55, 22);
            this.tsbWearAll.Tag = "wear all";
            this.tsbWearAll.Text = "Wear All";
            this.tsbWearAll.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbWho
            // 
            this.tsbWho.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbWho.Image = ((System.Drawing.Image)(resources.GetObject("tsbWho.Image")));
            this.tsbWho.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWho.Name = "tsbWho";
            this.tsbWho.Size = new System.Drawing.Size(36, 22);
            this.tsbWho.Tag = "who";
            this.tsbWho.Text = "Who";
            this.tsbWho.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbUptime
            // 
            this.tsbUptime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbUptime.Image = ((System.Drawing.Image)(resources.GetObject("tsbUptime.Image")));
            this.tsbUptime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUptime.Name = "tsbUptime";
            this.tsbUptime.Size = new System.Drawing.Size(50, 22);
            this.tsbUptime.Tag = "uptime";
            this.tsbUptime.Text = "Uptime";
            this.tsbUptime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbSpells
            // 
            this.tsbSpells.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSpells.Image = ((System.Drawing.Image)(resources.GetObject("tsbSpells.Image")));
            this.tsbSpells.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSpells.Name = "tsbSpells";
            this.tsbSpells.Size = new System.Drawing.Size(41, 22);
            this.tsbSpells.Tag = "spells";
            this.tsbSpells.Text = "Spells";
            this.tsbSpells.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbScore
            // 
            this.tsbScore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbScore.Image = ((System.Drawing.Image)(resources.GetObject("tsbScore.Image")));
            this.tsbScore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScore.Name = "tsbScore";
            this.tsbScore.Size = new System.Drawing.Size(40, 22);
            this.tsbScore.Tag = "";
            this.tsbScore.Text = "Score";
            this.tsbScore.Click += new System.EventHandler(this.btnScore_Click);
            // 
            // tsbTime
            // 
            this.tsbTime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbTime.Image = ((System.Drawing.Image)(resources.GetObject("tsbTime.Image")));
            this.tsbTime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTime.Name = "tsbTime";
            this.tsbTime.Size = new System.Drawing.Size(37, 22);
            this.tsbTime.Tag = "time";
            this.tsbTime.Text = "Time";
            this.tsbTime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsddActions
            // 
            this.tsddActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSearch,
            this.tsmiHide,
            this.tsmiItemManagement,
            this.tsmiTimeInfo});
            this.tsddActions.Image = ((System.Drawing.Image)(resources.GetObject("tsddActions.Image")));
            this.tsddActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddActions.Name = "tsddActions";
            this.tsddActions.Size = new System.Drawing.Size(60, 22);
            this.tsddActions.Text = "Actions";
            // 
            // tsmiSearch
            // 
            this.tsmiSearch.Name = "tsmiSearch";
            this.tsmiSearch.Size = new System.Drawing.Size(172, 22);
            this.tsmiSearch.Text = "Search";
            this.tsmiSearch.Click += new System.EventHandler(this.tsmiSearch_Click);
            // 
            // tsmiHide
            // 
            this.tsmiHide.Name = "tsmiHide";
            this.tsmiHide.Size = new System.Drawing.Size(172, 22);
            this.tsmiHide.Text = "Hide";
            this.tsmiHide.Click += new System.EventHandler(this.tsmiHide_Click);
            // 
            // tsmiItemManagement
            // 
            this.tsmiItemManagement.Name = "tsmiItemManagement";
            this.tsmiItemManagement.Size = new System.Drawing.Size(172, 22);
            this.tsmiItemManagement.Text = "Item Management";
            this.tsmiItemManagement.Click += new System.EventHandler(this.tsmiItemManagement_Click);
            // 
            // tsmiTimeInfo
            // 
            this.tsmiTimeInfo.Name = "tsmiTimeInfo";
            this.tsmiTimeInfo.Size = new System.Drawing.Size(172, 22);
            this.tsmiTimeInfo.Text = "Time Info";
            this.tsmiTimeInfo.Click += new System.EventHandler(this.tsmiShipInfo_Click);
            // 
            // tsddbSettings
            // 
            this.tsddbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditSettings,
            this.tsmiExportXML,
            this.tsmiImportXML,
            this.tsmiSaveSettings,
            this.tsmiImportFromPlayer,
            this.tsmiQuitWithoutSaving,
            this.tsmiRestoreDefaults,
            this.tsmiOpenLogFolder});
            this.tsddbSettings.Image = ((System.Drawing.Image)(resources.GetObject("tsddbSettings.Image")));
            this.tsddbSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbSettings.Name = "tsddbSettings";
            this.tsddbSettings.Size = new System.Drawing.Size(62, 22);
            this.tsddbSettings.Text = "Settings";
            // 
            // tsmiEditSettings
            // 
            this.tsmiEditSettings.Name = "tsmiEditSettings";
            this.tsmiEditSettings.Size = new System.Drawing.Size(179, 22);
            this.tsmiEditSettings.Text = "Edit";
            this.tsmiEditSettings.Click += new System.EventHandler(this.tsmiEditSettings_Click);
            // 
            // tsmiExportXML
            // 
            this.tsmiExportXML.Name = "tsmiExportXML";
            this.tsmiExportXML.Size = new System.Drawing.Size(179, 22);
            this.tsmiExportXML.Text = "Export XML";
            this.tsmiExportXML.Click += new System.EventHandler(this.tsmiExportXML_Click);
            // 
            // tsmiImportXML
            // 
            this.tsmiImportXML.Name = "tsmiImportXML";
            this.tsmiImportXML.Size = new System.Drawing.Size(179, 22);
            this.tsmiImportXML.Text = "Import XML";
            this.tsmiImportXML.Click += new System.EventHandler(this.tsmiImportXML_Click);
            // 
            // tsmiSaveSettings
            // 
            this.tsmiSaveSettings.Name = "tsmiSaveSettings";
            this.tsmiSaveSettings.Size = new System.Drawing.Size(179, 22);
            this.tsmiSaveSettings.Text = "Save Settings";
            this.tsmiSaveSettings.Click += new System.EventHandler(this.tsmiSaveSettings_Click);
            // 
            // tsmiImportFromPlayer
            // 
            this.tsmiImportFromPlayer.Name = "tsmiImportFromPlayer";
            this.tsmiImportFromPlayer.Size = new System.Drawing.Size(179, 22);
            this.tsmiImportFromPlayer.Text = "Import from Player";
            this.tsmiImportFromPlayer.Click += new System.EventHandler(this.tsmiImportFromPlayer_Click);
            // 
            // tsmiQuitWithoutSaving
            // 
            this.tsmiQuitWithoutSaving.Name = "tsmiQuitWithoutSaving";
            this.tsmiQuitWithoutSaving.Size = new System.Drawing.Size(179, 22);
            this.tsmiQuitWithoutSaving.Text = "Quit without Saving";
            this.tsmiQuitWithoutSaving.Click += new System.EventHandler(this.tsmiQuitWithoutSaving_Click);
            // 
            // tsmiRestoreDefaults
            // 
            this.tsmiRestoreDefaults.Name = "tsmiRestoreDefaults";
            this.tsmiRestoreDefaults.Size = new System.Drawing.Size(179, 22);
            this.tsmiRestoreDefaults.Text = "Restore Defaults";
            this.tsmiRestoreDefaults.Click += new System.EventHandler(this.tsmiRestoreDefaults_Click);
            // 
            // tsmiOpenLogFolder
            // 
            this.tsmiOpenLogFolder.Name = "tsmiOpenLogFolder";
            this.tsmiOpenLogFolder.Size = new System.Drawing.Size(179, 22);
            this.tsmiOpenLogFolder.Text = "Open Log Folder";
            this.tsmiOpenLogFolder.Click += new System.EventHandler(this.tsmiOpenLogFolder_Click);
            // 
            // tsbReloadMap
            // 
            this.tsbReloadMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbReloadMap.Image = ((System.Drawing.Image)(resources.GetObject("tsbReloadMap.Image")));
            this.tsbReloadMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReloadMap.Name = "tsbReloadMap";
            this.tsbReloadMap.Size = new System.Drawing.Size(74, 22);
            this.tsbReloadMap.Text = "Reload Map";
            this.tsbReloadMap.Click += new System.EventHandler(this.tsbReloadMap_Click);
            // 
            // tsbQuit
            // 
            this.tsbQuit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbQuit.Image = ((System.Drawing.Image)(resources.GetObject("tsbQuit.Image")));
            this.tsbQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbQuit.Name = "tsbQuit";
            this.tsbQuit.Size = new System.Drawing.Size(34, 22);
            this.tsbQuit.Text = "Quit";
            this.tsbQuit.Click += new System.EventHandler(this.tsbQuit_Click);
            // 
            // tsbLogout
            // 
            this.tsbLogout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbLogout.Image = ((System.Drawing.Image)(resources.GetObject("tsbLogout.Image")));
            this.tsbLogout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLogout.Name = "tsbLogout";
            this.tsbLogout.Size = new System.Drawing.Size(49, 22);
            this.tsbLogout.Text = "Logout";
            this.tsbLogout.Click += new System.EventHandler(this.tsbLogout_Click);
            // 
            // tmr
            // 
            this.tmr.Enabled = true;
            this.tmr.Interval = 20;
            this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
            // 
            // grpConsole
            // 
            this.grpConsole.Controls.Add(this.pnlConsoleHolder);
            this.grpConsole.Controls.Add(this.pnlCommand);
            this.grpConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConsole.Location = new System.Drawing.Point(947, 0);
            this.grpConsole.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpConsole.Name = "grpConsole";
            this.grpConsole.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpConsole.Size = new System.Drawing.Size(208, 857);
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
            this.pnlConsoleHolder.Size = new System.Drawing.Size(204, 790);
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
            this.rtbConsole.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(204, 790);
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
            this.pnlCommand.Location = new System.Drawing.Point(2, 805);
            this.pnlCommand.Name = "pnlCommand";
            this.pnlCommand.Size = new System.Drawing.Size(204, 50);
            this.pnlCommand.TabIndex = 30;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 857);
            this.Controls.Add(this.grpConsole);
            this.Controls.Add(this.pnlOverallLeft);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Isengard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.tcMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.grpPermRuns.ResumeLayout(false);
            this.grpPermRuns.PerformLayout();
            this.grpInventory.ResumeLayout(false);
            this.grpEquipment.ResumeLayout(false);
            this.grpCurrentRoom.ResumeLayout(false);
            this.ctxAutoEscape.ResumeLayout(false);
            this.grpCurrentPlayer.ResumeLayout(false);
            this.grpCurrentPlayer.PerformLayout();
            this.grpMessages.ResumeLayout(false);
            this.ctxMessages.ResumeLayout(false);
            this.grpMob.ResumeLayout(false);
            this.grpMob.PerformLayout();
            this.grpSingleMove.ResumeLayout(false);
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
            this.pnlOverallLeft.ResumeLayout(false);
            this.pnlOverallLeft.PerformLayout();
            this.pnlTabControl.ResumeLayout(false);
            this.tsTopMenu.ResumeLayout(false);
            this.tsTopMenu.PerformLayout();
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
        private System.Windows.Forms.Button btnCastCurePoison;
        private System.Windows.Forms.TextBox txtOneOffCommand;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnAttackMob;
        private System.Windows.Forms.Button btnDrinkVigor;
        private System.Windows.Forms.Button btnDrinkCurepoison;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.ComboBox cboSetOption;
        private System.Windows.Forms.CheckBox chkSetOn;
        private System.Windows.Forms.TextBox txtWand;
        private System.Windows.Forms.Label lblWand;
        private System.Windows.Forms.Button btnUseWandOnMob;
        private System.Windows.Forms.Button btnPowerAttackMob;
        private System.Windows.Forms.TextBox txtSetValue;
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
        private System.Windows.Forms.Panel pnlOverallLeft;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TabPage tabAncillary;
        private System.Windows.Forms.Panel pnlAncillary;
        private System.Windows.Forms.Button btnDrinkMend;
        private System.Windows.Forms.Button btnCastMend;
        private System.Windows.Forms.Button btnStunMob;
        private System.Windows.Forms.Button btnLevel3OffensiveSpell;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Timer tmr;
        private System.Windows.Forms.Label lblHitpoints;
        private System.Windows.Forms.GroupBox grpSpells;
        private System.Windows.Forms.FlowLayoutPanel flpSpells;
        private System.Windows.Forms.Button btnDn;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnOtherSingleMove;
        private System.Windows.Forms.TabPage tabEmotes;
        private System.Windows.Forms.Button btnEmote;
        private System.Windows.Forms.GroupBox grpEmotes;
        private System.Windows.Forms.TextBox txtCommandText;
        private System.Windows.Forms.Label lblCommandText;
        private System.Windows.Forms.Panel pnlEmotes;
        private System.Windows.Forms.GroupBox grpConsole;
        private System.Windows.Forms.RichTextBox rtbConsole;
        private System.Windows.Forms.Label lblEmoteTarget;
        private System.Windows.Forms.TextBox txtEmoteTarget;
        private System.Windows.Forms.FlowLayoutPanel flpEmotes;
        private System.Windows.Forms.CheckBox chkShowEmotesWithoutTarget;
        private System.Windows.Forms.ContextMenuStrip ctxConsole;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearConsole;
        private System.Windows.Forms.Button btnSay;
        private System.Windows.Forms.GroupBox grpSingleMove;
        private System.Windows.Forms.TabPage tabHelp;
        private System.Windows.Forms.GroupBox grpHelp;
        private System.Windows.Forms.FlowLayoutPanel flpHelp;
        private System.Windows.Forms.Panel pnlCommand;
        private System.Windows.Forms.Panel pnlConsoleHolder;
        private System.Windows.Forms.Button btnGraph;
        private System.Windows.Forms.GroupBox grpMob;
        private System.Windows.Forms.TextBox txtMobDamage;
        private System.Windows.Forms.Label lblMobDamage;
        private System.Windows.Forms.TextBox txtMobStatus;
        private System.Windows.Forms.Label lblMobStatus;
        private System.Windows.Forms.GroupBox grpMessages;
        private System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.ToolStrip tsTopMenu;
        private System.Windows.Forms.ToolStripButton tsbInformation;
        private System.Windows.Forms.ToolStripButton tsbInventoryAndEquipment;
        private System.Windows.Forms.ToolStripButton tsbWho;
        private System.Windows.Forms.ToolStripButton tsbUptime;
        private System.Windows.Forms.ToolStripButton tsbScore;
        private System.Windows.Forms.ToolStripButton tsbTime;
        private System.Windows.Forms.ToolStripButton tsbQuit;
        private System.Windows.Forms.GroupBox grpCurrentPlayer;
        private System.Windows.Forms.Panel pnlTabControl;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblManaValue;
        private System.Windows.Forms.Label lblHitpointsValue;
        private System.Windows.Forms.Label lblAutoEscapeValue;
        private System.Windows.Forms.ContextMenuStrip ctxAutoEscape;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeIsActive;
        private System.Windows.Forms.Label lblToNextLevelValue;
        private System.Windows.Forms.GroupBox grpSkillCooldowns;
        private System.Windows.Forms.Button btnIncrementWand;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeFlee;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeHazy;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator2;
        private System.Windows.Forms.Button btnLocations;
        private System.Windows.Forms.GroupBox grpCurrentRoom;
        private System.Windows.Forms.ComboBox cboArea;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.Button btnGoToHealingRoom;
        private System.Windows.Forms.TreeView treeCurrentRoom;
        private System.Windows.Forms.Button btnOut;
        private System.Windows.Forms.ToolStripButton tsbSpells;
        private System.Windows.Forms.GroupBox grpEquipment;
        private System.Windows.Forms.ListBox lstEquipment;
        private System.Windows.Forms.GroupBox grpInventory;
        private System.Windows.Forms.ListBox lstInventory;
        private System.Windows.Forms.Label lblGold;
        private System.Windows.Forms.ToolStripButton tsbRemoveAll;
        private System.Windows.Forms.ToolStripButton tsbWearAll;
        private System.Windows.Forms.ContextMenuStrip ctxInventoryOrEquipmentItem;
        private System.Windows.Forms.Button btnGoToPawnShop;
        private System.Windows.Forms.ToolStripButton tsbReloadMap;
        private System.Windows.Forms.ContextMenuStrip ctxMessages;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyMessages;
        private System.Windows.Forms.ToolStripDropDownButton tsddbSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportXML;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportXML;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportFromPlayer;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuitWithoutSaving;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreDefaults;
        private System.Windows.Forms.ToolStripButton tsbLogout;
        private System.Windows.Forms.Button btnPermRuns;
        private System.Windows.Forms.ContextMenuStrip ctxCurrentRoom;
        private System.Windows.Forms.Button btnGoToInventorySink;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenLogFolder;
        private System.Windows.Forms.ToolStripDropDownButton tsddActions;
        private System.Windows.Forms.ToolStripMenuItem tsmiSearch;
        private System.Windows.Forms.ToolStripMenuItem tsmiHide;
        private System.Windows.Forms.ToolStripMenuItem tsmiItemManagement;
        private System.Windows.Forms.GroupBox grpPermRuns;
        private System.Windows.Forms.TextBox txtNextPermRun;
        private System.Windows.Forms.Label lblNextPermRun;
        private System.Windows.Forms.TextBox txtCurrentPermRun;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Button btnRemoveCurrentPermRun;
        private System.Windows.Forms.Button btnRemoveNextPermRun;
        private System.Windows.Forms.Button btnCompleteCurrentPermRun;
        private System.Windows.Forms.Button btnResumeCurrentPermRun;
        private System.Windows.Forms.ToolStripMenuItem tsmiTimeInfo;
        private System.Windows.Forms.Button btnNonCombatPermRun;
        private System.Windows.Forms.Button btnAdHocPermRun;
        private System.Windows.Forms.Button btnFightAll;
        private System.Windows.Forms.Button btnFightOne;
    }
}

