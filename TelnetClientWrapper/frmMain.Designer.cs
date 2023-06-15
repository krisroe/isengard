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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Obvious Exits");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Other Exits");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnLevel1OffensiveSpell = new System.Windows.Forms.Button();
            this.txtMob = new System.Windows.Forms.TextBox();
            this.ctxMob = new System.Windows.Forms.ContextMenuStrip(this.components);
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
            this.btnDrinkYellow = new System.Windows.Forms.Button();
            this.btnDrinkGreen = new System.Windows.Forms.Button();
            this.txtWeapon = new System.Windows.Forms.TextBox();
            this.lblWeapon = new System.Windows.Forms.Label();
            this.btnWieldWeapon = new System.Windows.Forms.Button();
            this.btnSet = new System.Windows.Forms.Button();
            this.cboSetOption = new System.Windows.Forms.ComboBox();
            this.chkSetOn = new System.Windows.Forms.CheckBox();
            this.txtWand = new System.Windows.Forms.TextBox();
            this.lblWand = new System.Windows.Forms.Label();
            this.btnUseWandOnMob = new System.Windows.Forms.Button();
            this.btnPowerAttackMob = new System.Windows.Forms.Button();
            this.txtSetValue = new System.Windows.Forms.TextBox();
            this.grpOneClickStrategies = new System.Windows.Forms.GroupBox();
            this.flpOneClickStrategies = new System.Windows.Forms.FlowLayoutPanel();
            this.txtPotion = new System.Windows.Forms.TextBox();
            this.lblPotion = new System.Windows.Forms.Label();
            this.btnRemoveWeapon = new System.Windows.Forms.Button();
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
            this.btnGoToHealingRoom = new System.Windows.Forms.Button();
            this.lblTickRoom = new System.Windows.Forms.Label();
            this.cboTickRoom = new System.Windows.Forms.ComboBox();
            this.grpCurrentRoom = new System.Windows.Forms.GroupBox();
            this.treeCurrentRoom = new System.Windows.Forms.TreeView();
            this.ctxCurrentRoom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiGoToRoom = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLocations = new System.Windows.Forms.Button();
            this.btnIncrementWand = new System.Windows.Forms.Button();
            this.grpSkillCooldowns = new System.Windows.Forms.GroupBox();
            this.btnHeal = new System.Windows.Forms.Button();
            this.btnSkills = new System.Windows.Forms.Button();
            this.lblAutoEscapeValue = new System.Windows.Forms.Label();
            this.ctxAutoEscape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAutoEscapeIsActive = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSetAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAutoEscapeFlee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSetDefaultAutoEscape = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRestoreDefaultAutoEscape = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTime = new System.Windows.Forms.Label();
            this.grpCurrentPlayer = new System.Windows.Forms.GroupBox();
            this.lblToNextLevelValue = new System.Windows.Forms.Label();
            this.lblToNextLevel = new System.Windows.Forms.Label();
            this.lblManaValue = new System.Windows.Forms.Label();
            this.lblHitpointsValue = new System.Windows.Forms.Label();
            this.lblMana = new System.Windows.Forms.Label();
            this.lblHitpoints = new System.Windows.Forms.Label();
            this.grpMessages = new System.Windows.Forms.GroupBox();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.grpMob = new System.Windows.Forms.GroupBox();
            this.txtMobStatus = new System.Windows.Forms.TextBox();
            this.lblMobStatus = new System.Windows.Forms.Label();
            this.txtMobDamage = new System.Windows.Forms.TextBox();
            this.lblMobDamage = new System.Windows.Forms.Label();
            this.btnGraph = new System.Windows.Forms.Button();
            this.btnClearCurrentLocation = new System.Windows.Forms.Button();
            this.grpSingleMove = new System.Windows.Forms.GroupBox();
            this.btnOut = new System.Windows.Forms.Button();
            this.btnOtherSingleMove = new System.Windows.Forms.Button();
            this.btnDn = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.grpSpells = new System.Windows.Forms.GroupBox();
            this.flpSpells = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnLevel3OffensiveSpell = new System.Windows.Forms.Button();
            this.btnStunMob = new System.Windows.Forms.Button();
            this.btnCastMend = new System.Windows.Forms.Button();
            this.btnReddishOrange = new System.Windows.Forms.Button();
            this.btnHide = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
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
            this.ctxRoomExits = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmr = new System.Windows.Forms.Timer(this.components);
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.pnlTabControl = new System.Windows.Forms.Panel();
            this.tsTopMenu = new System.Windows.Forms.ToolStrip();
            this.tsbInformation = new System.Windows.Forms.ToolStripButton();
            this.tsbInventory = new System.Windows.Forms.ToolStripButton();
            this.tsbEquipment = new System.Windows.Forms.ToolStripButton();
            this.tsbWho = new System.Windows.Forms.ToolStripButton();
            this.tsbUptime = new System.Windows.Forms.ToolStripButton();
            this.tsbScore = new System.Windows.Forms.ToolStripButton();
            this.tsbTime = new System.Windows.Forms.ToolStripButton();
            this.tsbConfiguration = new System.Windows.Forms.ToolStripButton();
            this.tsbQuit = new System.Windows.Forms.ToolStripButton();
            this.grpConsole = new System.Windows.Forms.GroupBox();
            this.pnlConsoleHolder = new System.Windows.Forms.Panel();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.ctxConsole = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiClearConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlCommand = new System.Windows.Forms.Panel();
            this.grpOneClickStrategies.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.grpCurrentRoom.SuspendLayout();
            this.ctxCurrentRoom.SuspendLayout();
            this.ctxAutoEscape.SuspendLayout();
            this.grpCurrentPlayer.SuspendLayout();
            this.grpMessages.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
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
            this.btnLevel1OffensiveSpell.Location = new System.Drawing.Point(883, 583);
            this.btnLevel1OffensiveSpell.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLevel1OffensiveSpell.Name = "btnLevel1OffensiveSpell";
            this.btnLevel1OffensiveSpell.Size = new System.Drawing.Size(108, 34);
            this.btnLevel1OffensiveSpell.TabIndex = 0;
            this.btnLevel1OffensiveSpell.Text = "Cast Level 1";
            this.btnLevel1OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel1OffensiveSpell.Click += new System.EventHandler(this.btnLevel1OffensiveSpell_Click);
            // 
            // txtMob
            // 
            this.txtMob.ContextMenuStrip = this.ctxMob;
            this.txtMob.Location = new System.Drawing.Point(101, 23);
            this.txtMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMob.Name = "txtMob";
            this.txtMob.Size = new System.Drawing.Size(205, 22);
            this.txtMob.TabIndex = 4;
            this.txtMob.TextChanged += new System.EventHandler(this.txtMob_TextChanged);
            // 
            // ctxMob
            // 
            this.ctxMob.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxMob.Name = "ctxMob";
            this.ctxMob.Size = new System.Drawing.Size(61, 4);
            this.ctxMob.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMob_Opening);
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(17, 27);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(37, 16);
            this.lblMob.TabIndex = 3;
            this.lblMob.Text = "Mob:";
            this.lblMob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLevel2OffensiveSpell
            // 
            this.btnLevel2OffensiveSpell.Location = new System.Drawing.Point(883, 624);
            this.btnLevel2OffensiveSpell.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLevel2OffensiveSpell.Name = "btnLevel2OffensiveSpell";
            this.btnLevel2OffensiveSpell.Size = new System.Drawing.Size(108, 34);
            this.btnLevel2OffensiveSpell.TabIndex = 5;
            this.btnLevel2OffensiveSpell.Text = "Cast Level 2";
            this.btnLevel2OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel2OffensiveSpell.Click += new System.EventHandler(this.btnLevel2OffensiveSpell_Click);
            // 
            // btnFlee
            // 
            this.btnFlee.Location = new System.Drawing.Point(883, 704);
            this.btnFlee.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnFlee.Name = "btnFlee";
            this.btnFlee.Size = new System.Drawing.Size(107, 34);
            this.btnFlee.TabIndex = 6;
            this.btnFlee.Text = "Flee";
            this.btnFlee.UseVisualStyleBackColor = true;
            this.btnFlee.Click += new System.EventHandler(this.btnFlee_Click);
            // 
            // btnDrinkHazy
            // 
            this.btnDrinkHazy.Location = new System.Drawing.Point(884, 743);
            this.btnDrinkHazy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDrinkHazy.Name = "btnDrinkHazy";
            this.btnDrinkHazy.Size = new System.Drawing.Size(107, 34);
            this.btnDrinkHazy.TabIndex = 7;
            this.btnDrinkHazy.Text = "Hazy pot";
            this.btnDrinkHazy.UseVisualStyleBackColor = true;
            this.btnDrinkHazy.Click += new System.EventHandler(this.btnDrinkHazy_Click);
            // 
            // btnLookAtMob
            // 
            this.btnLookAtMob.Location = new System.Drawing.Point(772, 743);
            this.btnLookAtMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLookAtMob.Name = "btnLookAtMob";
            this.btnLookAtMob.Size = new System.Drawing.Size(105, 34);
            this.btnLookAtMob.TabIndex = 8;
            this.btnLookAtMob.Text = "Look at Mob";
            this.btnLookAtMob.UseVisualStyleBackColor = true;
            this.btnLookAtMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLook
            // 
            this.btnLook.Location = new System.Drawing.Point(36, 571);
            this.btnLook.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLook.Name = "btnLook";
            this.btnLook.Size = new System.Drawing.Size(133, 34);
            this.btnLook.TabIndex = 9;
            this.btnLook.Text = "Look";
            this.btnLook.UseVisualStyleBackColor = true;
            this.btnLook.Click += new System.EventHandler(this.btnLook_Click);
            // 
            // btnCastVigor
            // 
            this.btnCastVigor.Location = new System.Drawing.Point(772, 783);
            this.btnCastVigor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCastVigor.Name = "btnCastVigor";
            this.btnCastVigor.Size = new System.Drawing.Size(107, 34);
            this.btnCastVigor.TabIndex = 10;
            this.btnCastVigor.Text = "Vigor";
            this.btnCastVigor.UseVisualStyleBackColor = true;
            this.btnCastVigor.Click += new System.EventHandler(this.btnVigor_Click);
            // 
            // btnCastCurePoison
            // 
            this.btnCastCurePoison.Location = new System.Drawing.Point(173, 688);
            this.btnCastCurePoison.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCastCurePoison.Name = "btnCastCurePoison";
            this.btnCastCurePoison.Size = new System.Drawing.Size(133, 34);
            this.btnCastCurePoison.TabIndex = 18;
            this.btnCastCurePoison.Text = "Cast Curepoison";
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
            this.txtOneOffCommand.Size = new System.Drawing.Size(644, 30);
            this.txtOneOffCommand.TabIndex = 29;
            this.txtOneOffCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOneOffCommand_KeyPress);
            // 
            // btnAbort
            // 
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(996, 782);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(107, 34);
            this.btnAbort.TabIndex = 33;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnAttackMob
            // 
            this.btnAttackMob.Location = new System.Drawing.Point(772, 625);
            this.btnAttackMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAttackMob.Name = "btnAttackMob";
            this.btnAttackMob.Size = new System.Drawing.Size(105, 34);
            this.btnAttackMob.TabIndex = 35;
            this.btnAttackMob.Text = "Atk";
            this.btnAttackMob.UseVisualStyleBackColor = true;
            this.btnAttackMob.Click += new System.EventHandler(this.btnAttackMob_Click);
            // 
            // btnDrinkYellow
            // 
            this.btnDrinkYellow.Location = new System.Drawing.Point(885, 783);
            this.btnDrinkYellow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDrinkYellow.Name = "btnDrinkYellow";
            this.btnDrinkYellow.Size = new System.Drawing.Size(105, 34);
            this.btnDrinkYellow.TabIndex = 39;
            this.btnDrinkYellow.Text = "Yellow pot";
            this.btnDrinkYellow.UseVisualStyleBackColor = true;
            this.btnDrinkYellow.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnDrinkGreen
            // 
            this.btnDrinkGreen.Location = new System.Drawing.Point(173, 730);
            this.btnDrinkGreen.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDrinkGreen.Name = "btnDrinkGreen";
            this.btnDrinkGreen.Size = new System.Drawing.Size(133, 34);
            this.btnDrinkGreen.TabIndex = 40;
            this.btnDrinkGreen.Text = "Green pot";
            this.btnDrinkGreen.UseVisualStyleBackColor = true;
            this.btnDrinkGreen.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtWeapon
            // 
            this.txtWeapon.Location = new System.Drawing.Point(101, 50);
            this.txtWeapon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWeapon.Name = "txtWeapon";
            this.txtWeapon.Size = new System.Drawing.Size(204, 22);
            this.txtWeapon.TabIndex = 42;
            this.txtWeapon.TextChanged += new System.EventHandler(this.txtWeapon_TextChanged);
            // 
            // lblWeapon
            // 
            this.lblWeapon.AutoSize = true;
            this.lblWeapon.Location = new System.Drawing.Point(17, 54);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(62, 16);
            this.lblWeapon.TabIndex = 41;
            this.lblWeapon.Text = "Weapon:";
            this.lblWeapon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnWieldWeapon
            // 
            this.btnWieldWeapon.Location = new System.Drawing.Point(175, 571);
            this.btnWieldWeapon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnWieldWeapon.Name = "btnWieldWeapon";
            this.btnWieldWeapon.Size = new System.Drawing.Size(133, 34);
            this.btnWieldWeapon.TabIndex = 43;
            this.btnWieldWeapon.Text = "Wield Weapon";
            this.btnWieldWeapon.UseVisualStyleBackColor = true;
            this.btnWieldWeapon.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(13, 16);
            this.btnSet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(73, 30);
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
            this.cboSetOption.Location = new System.Drawing.Point(96, 17);
            this.cboSetOption.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboSetOption.Name = "cboSetOption";
            this.cboSetOption.Size = new System.Drawing.Size(164, 24);
            this.cboSetOption.TabIndex = 46;
            this.cboSetOption.SelectedIndexChanged += new System.EventHandler(this.cboSetOption_SelectedIndexChanged);
            // 
            // chkSetOn
            // 
            this.chkSetOn.AutoSize = true;
            this.chkSetOn.Location = new System.Drawing.Point(267, 21);
            this.chkSetOn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkSetOn.Name = "chkSetOn";
            this.chkSetOn.Size = new System.Drawing.Size(53, 20);
            this.chkSetOn.TabIndex = 47;
            this.chkSetOn.Text = "On?";
            this.chkSetOn.UseVisualStyleBackColor = true;
            this.chkSetOn.CheckedChanged += new System.EventHandler(this.chkSetOn_CheckedChanged);
            // 
            // txtWand
            // 
            this.txtWand.Location = new System.Drawing.Point(101, 78);
            this.txtWand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWand.Name = "txtWand";
            this.txtWand.Size = new System.Drawing.Size(204, 22);
            this.txtWand.TabIndex = 49;
            this.txtWand.TextChanged += new System.EventHandler(this.txtWand_TextChanged);
            // 
            // lblWand
            // 
            this.lblWand.AutoSize = true;
            this.lblWand.Location = new System.Drawing.Point(19, 81);
            this.lblWand.Name = "lblWand";
            this.lblWand.Size = new System.Drawing.Size(46, 16);
            this.lblWand.TabIndex = 48;
            this.lblWand.Text = "Wand:";
            this.lblWand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnUseWandOnMob
            // 
            this.btnUseWandOnMob.Location = new System.Drawing.Point(772, 704);
            this.btnUseWandOnMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUseWandOnMob.Name = "btnUseWandOnMob";
            this.btnUseWandOnMob.Size = new System.Drawing.Size(105, 34);
            this.btnUseWandOnMob.TabIndex = 50;
            this.btnUseWandOnMob.Text = "Wand";
            this.btnUseWandOnMob.UseVisualStyleBackColor = true;
            this.btnUseWandOnMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnPowerAttackMob
            // 
            this.btnPowerAttackMob.Location = new System.Drawing.Point(772, 665);
            this.btnPowerAttackMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPowerAttackMob.Name = "btnPowerAttackMob";
            this.btnPowerAttackMob.Size = new System.Drawing.Size(105, 34);
            this.btnPowerAttackMob.TabIndex = 54;
            this.btnPowerAttackMob.Text = "Power Atk";
            this.btnPowerAttackMob.UseVisualStyleBackColor = true;
            this.btnPowerAttackMob.Click += new System.EventHandler(this.btnPowerAttackMob_Click);
            // 
            // txtSetValue
            // 
            this.txtSetValue.Location = new System.Drawing.Point(325, 18);
            this.txtSetValue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSetValue.Name = "txtSetValue";
            this.txtSetValue.Size = new System.Drawing.Size(117, 22);
            this.txtSetValue.TabIndex = 56;
            // 
            // grpOneClickStrategies
            // 
            this.grpOneClickStrategies.Controls.Add(this.flpOneClickStrategies);
            this.grpOneClickStrategies.Location = new System.Drawing.Point(31, 192);
            this.grpOneClickStrategies.Margin = new System.Windows.Forms.Padding(4);
            this.grpOneClickStrategies.Name = "grpOneClickStrategies";
            this.grpOneClickStrategies.Padding = new System.Windows.Forms.Padding(4);
            this.grpOneClickStrategies.Size = new System.Drawing.Size(801, 133);
            this.grpOneClickStrategies.TabIndex = 65;
            this.grpOneClickStrategies.TabStop = false;
            this.grpOneClickStrategies.Text = "Strategies";
            // 
            // flpOneClickStrategies
            // 
            this.flpOneClickStrategies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpOneClickStrategies.Location = new System.Drawing.Point(4, 19);
            this.flpOneClickStrategies.Margin = new System.Windows.Forms.Padding(4);
            this.flpOneClickStrategies.Name = "flpOneClickStrategies";
            this.flpOneClickStrategies.Size = new System.Drawing.Size(793, 110);
            this.flpOneClickStrategies.TabIndex = 0;
            // 
            // txtPotion
            // 
            this.txtPotion.Location = new System.Drawing.Point(101, 105);
            this.txtPotion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPotion.Name = "txtPotion";
            this.txtPotion.Size = new System.Drawing.Size(204, 22);
            this.txtPotion.TabIndex = 68;
            // 
            // lblPotion
            // 
            this.lblPotion.AutoSize = true;
            this.lblPotion.Location = new System.Drawing.Point(19, 108);
            this.lblPotion.Name = "lblPotion";
            this.lblPotion.Size = new System.Drawing.Size(48, 16);
            this.lblPotion.TabIndex = 67;
            this.lblPotion.Text = "Potion:";
            this.lblPotion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemoveWeapon
            // 
            this.btnRemoveWeapon.Location = new System.Drawing.Point(175, 610);
            this.btnRemoveWeapon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRemoveWeapon.Name = "btnRemoveWeapon";
            this.btnRemoveWeapon.Size = new System.Drawing.Size(133, 34);
            this.btnRemoveWeapon.TabIndex = 69;
            this.btnRemoveWeapon.Text = "Remove Weapon";
            this.btnRemoveWeapon.UseVisualStyleBackColor = true;
            this.btnRemoveWeapon.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnNortheast
            // 
            this.btnNortheast.Location = new System.Drawing.Point(116, 22);
            this.btnNortheast.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNortheast.Name = "btnNortheast";
            this.btnNortheast.Size = new System.Drawing.Size(45, 28);
            this.btnNortheast.TabIndex = 71;
            this.btnNortheast.Tag = "northeast";
            this.btnNortheast.Text = "NE";
            this.btnNortheast.UseVisualStyleBackColor = true;
            this.btnNortheast.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnNorth
            // 
            this.btnNorth.Location = new System.Drawing.Point(64, 22);
            this.btnNorth.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNorth.Name = "btnNorth";
            this.btnNorth.Size = new System.Drawing.Size(45, 28);
            this.btnNorth.TabIndex = 72;
            this.btnNorth.Tag = "north";
            this.btnNorth.Text = "N";
            this.btnNorth.UseVisualStyleBackColor = true;
            this.btnNorth.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnNorthwest
            // 
            this.btnNorthwest.Location = new System.Drawing.Point(15, 22);
            this.btnNorthwest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNorthwest.Name = "btnNorthwest";
            this.btnNorthwest.Size = new System.Drawing.Size(45, 28);
            this.btnNorthwest.TabIndex = 73;
            this.btnNorthwest.Tag = "northwest";
            this.btnNorthwest.Text = "NW";
            this.btnNorthwest.UseVisualStyleBackColor = true;
            this.btnNorthwest.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnWest
            // 
            this.btnWest.Location = new System.Drawing.Point(15, 54);
            this.btnWest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnWest.Name = "btnWest";
            this.btnWest.Size = new System.Drawing.Size(45, 28);
            this.btnWest.TabIndex = 75;
            this.btnWest.Tag = "west";
            this.btnWest.Text = "W";
            this.btnWest.UseVisualStyleBackColor = true;
            this.btnWest.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnEast
            // 
            this.btnEast.Location = new System.Drawing.Point(116, 54);
            this.btnEast.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEast.Name = "btnEast";
            this.btnEast.Size = new System.Drawing.Size(45, 28);
            this.btnEast.TabIndex = 74;
            this.btnEast.Tag = "east";
            this.btnEast.Text = "E";
            this.btnEast.UseVisualStyleBackColor = true;
            this.btnEast.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnSouthwest
            // 
            this.btnSouthwest.Location = new System.Drawing.Point(15, 87);
            this.btnSouthwest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSouthwest.Name = "btnSouthwest";
            this.btnSouthwest.Size = new System.Drawing.Size(45, 28);
            this.btnSouthwest.TabIndex = 78;
            this.btnSouthwest.Tag = "southwest";
            this.btnSouthwest.Text = "SW";
            this.btnSouthwest.UseVisualStyleBackColor = true;
            this.btnSouthwest.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnSouth
            // 
            this.btnSouth.Location = new System.Drawing.Point(64, 87);
            this.btnSouth.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSouth.Name = "btnSouth";
            this.btnSouth.Size = new System.Drawing.Size(45, 28);
            this.btnSouth.TabIndex = 77;
            this.btnSouth.Tag = "south";
            this.btnSouth.Text = "S";
            this.btnSouth.UseVisualStyleBackColor = true;
            this.btnSouth.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnSoutheast
            // 
            this.btnSoutheast.Location = new System.Drawing.Point(116, 87);
            this.btnSoutheast.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSoutheast.Name = "btnSoutheast";
            this.btnSoutheast.Size = new System.Drawing.Size(45, 28);
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
            this.tcMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1270, 1013);
            this.tcMain.TabIndex = 79;
            this.tcMain.Selected += new System.Windows.Forms.TabControlEventHandler(this.tcMain_Selected);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.pnlMain);
            this.tabMain.Location = new System.Drawing.Point(4, 25);
            this.tabMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabMain.Size = new System.Drawing.Size(1262, 984);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnGoToHealingRoom);
            this.pnlMain.Controls.Add(this.lblTickRoom);
            this.pnlMain.Controls.Add(this.cboTickRoom);
            this.pnlMain.Controls.Add(this.grpCurrentRoom);
            this.pnlMain.Controls.Add(this.btnLocations);
            this.pnlMain.Controls.Add(this.btnIncrementWand);
            this.pnlMain.Controls.Add(this.grpSkillCooldowns);
            this.pnlMain.Controls.Add(this.btnHeal);
            this.pnlMain.Controls.Add(this.btnSkills);
            this.pnlMain.Controls.Add(this.lblAutoEscapeValue);
            this.pnlMain.Controls.Add(this.lblTime);
            this.pnlMain.Controls.Add(this.grpCurrentPlayer);
            this.pnlMain.Controls.Add(this.grpMessages);
            this.pnlMain.Controls.Add(this.grpMob);
            this.pnlMain.Controls.Add(this.btnGraph);
            this.pnlMain.Controls.Add(this.btnClearCurrentLocation);
            this.pnlMain.Controls.Add(this.grpSingleMove);
            this.pnlMain.Controls.Add(this.grpSpells);
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
            this.pnlMain.Controls.Add(this.btnLook);
            this.pnlMain.Controls.Add(this.btnRemoveWeapon);
            this.pnlMain.Controls.Add(this.btnCastVigor);
            this.pnlMain.Controls.Add(this.txtPotion);
            this.pnlMain.Controls.Add(this.lblPotion);
            this.pnlMain.Controls.Add(this.btnCastCurePoison);
            this.pnlMain.Controls.Add(this.grpOneClickStrategies);
            this.pnlMain.Controls.Add(this.btnPowerAttackMob);
            this.pnlMain.Controls.Add(this.btnAbort);
            this.pnlMain.Controls.Add(this.btnUseWandOnMob);
            this.pnlMain.Controls.Add(this.txtWand);
            this.pnlMain.Controls.Add(this.btnAttackMob);
            this.pnlMain.Controls.Add(this.lblWand);
            this.pnlMain.Controls.Add(this.btnWieldWeapon);
            this.pnlMain.Controls.Add(this.btnDrinkYellow);
            this.pnlMain.Controls.Add(this.txtWeapon);
            this.pnlMain.Controls.Add(this.btnDrinkGreen);
            this.pnlMain.Controls.Add(this.lblWeapon);
            this.pnlMain.Location = new System.Drawing.Point(3, 2);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1356, 1108);
            this.pnlMain.TabIndex = 0;
            // 
            // btnGoToHealingRoom
            // 
            this.btnGoToHealingRoom.Enabled = false;
            this.btnGoToHealingRoom.Location = new System.Drawing.Point(312, 129);
            this.btnGoToHealingRoom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGoToHealingRoom.Name = "btnGoToHealingRoom";
            this.btnGoToHealingRoom.Size = new System.Drawing.Size(45, 28);
            this.btnGoToHealingRoom.TabIndex = 145;
            this.btnGoToHealingRoom.Tag = "";
            this.btnGoToHealingRoom.Text = "Go";
            this.btnGoToHealingRoom.UseVisualStyleBackColor = true;
            this.btnGoToHealingRoom.Click += new System.EventHandler(this.btnGoToHealingRoom_Click);
            // 
            // lblTickRoom
            // 
            this.lblTickRoom.AutoSize = true;
            this.lblTickRoom.Location = new System.Drawing.Point(19, 135);
            this.lblTickRoom.Name = "lblTickRoom";
            this.lblTickRoom.Size = new System.Drawing.Size(70, 16);
            this.lblTickRoom.TabIndex = 144;
            this.lblTickRoom.Text = "Tick room:";
            this.lblTickRoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboTickRoom
            // 
            this.cboTickRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTickRoom.FormattingEnabled = true;
            this.cboTickRoom.Location = new System.Drawing.Point(101, 132);
            this.cboTickRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTickRoom.Name = "cboTickRoom";
            this.cboTickRoom.Size = new System.Drawing.Size(205, 24);
            this.cboTickRoom.TabIndex = 143;
            this.cboTickRoom.SelectedIndexChanged += new System.EventHandler(this.cboTickRoom_SelectedIndexChanged);
            // 
            // grpCurrentRoom
            // 
            this.grpCurrentRoom.Controls.Add(this.treeCurrentRoom);
            this.grpCurrentRoom.Location = new System.Drawing.Point(875, 12);
            this.grpCurrentRoom.Margin = new System.Windows.Forms.Padding(4);
            this.grpCurrentRoom.Name = "grpCurrentRoom";
            this.grpCurrentRoom.Padding = new System.Windows.Forms.Padding(4);
            this.grpCurrentRoom.Size = new System.Drawing.Size(375, 559);
            this.grpCurrentRoom.TabIndex = 141;
            this.grpCurrentRoom.TabStop = false;
            this.grpCurrentRoom.Text = "Current Room";
            // 
            // treeCurrentRoom
            // 
            this.treeCurrentRoom.ContextMenuStrip = this.ctxCurrentRoom;
            this.treeCurrentRoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeCurrentRoom.Location = new System.Drawing.Point(4, 19);
            this.treeCurrentRoom.Name = "treeCurrentRoom";
            treeNode1.Name = "tnObviousExits";
            treeNode1.Text = "Obvious Exits";
            treeNode2.Name = "tnOtherExits";
            treeNode2.Text = "Other Exits";
            this.treeCurrentRoom.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.treeCurrentRoom.Size = new System.Drawing.Size(367, 536);
            this.treeCurrentRoom.TabIndex = 0;
            this.treeCurrentRoom.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeCurrentRoom_NodeMouseClick);
            // 
            // ctxCurrentRoom
            // 
            this.ctxCurrentRoom.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxCurrentRoom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGoToRoom});
            this.ctxCurrentRoom.Name = "ctxCurrentRoom";
            this.ctxCurrentRoom.Size = new System.Drawing.Size(160, 28);
            this.ctxCurrentRoom.Opening += new System.ComponentModel.CancelEventHandler(this.ctxCurrentRoom_Opening);
            // 
            // tsmiGoToRoom
            // 
            this.tsmiGoToRoom.Name = "tsmiGoToRoom";
            this.tsmiGoToRoom.Size = new System.Drawing.Size(159, 24);
            this.tsmiGoToRoom.Text = "Go to Room";
            this.tsmiGoToRoom.Click += new System.EventHandler(this.tsmiGoToRoom_Click);
            // 
            // btnLocations
            // 
            this.btnLocations.Location = new System.Drawing.Point(713, 57);
            this.btnLocations.Margin = new System.Windows.Forms.Padding(4);
            this.btnLocations.Name = "btnLocations";
            this.btnLocations.Size = new System.Drawing.Size(149, 34);
            this.btnLocations.TabIndex = 140;
            this.btnLocations.Text = "Locations";
            this.btnLocations.UseVisualStyleBackColor = true;
            this.btnLocations.Click += new System.EventHandler(this.btnLocations_Click);
            // 
            // btnIncrementWand
            // 
            this.btnIncrementWand.Enabled = false;
            this.btnIncrementWand.Location = new System.Drawing.Point(312, 78);
            this.btnIncrementWand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnIncrementWand.Name = "btnIncrementWand";
            this.btnIncrementWand.Size = new System.Drawing.Size(45, 28);
            this.btnIncrementWand.TabIndex = 139;
            this.btnIncrementWand.Text = "Inc";
            this.btnIncrementWand.UseVisualStyleBackColor = true;
            this.btnIncrementWand.Click += new System.EventHandler(this.btnIncrementWand_Click);
            // 
            // grpSkillCooldowns
            // 
            this.grpSkillCooldowns.Location = new System.Drawing.Point(533, 571);
            this.grpSkillCooldowns.Margin = new System.Windows.Forms.Padding(4);
            this.grpSkillCooldowns.Name = "grpSkillCooldowns";
            this.grpSkillCooldowns.Padding = new System.Windows.Forms.Padding(4);
            this.grpSkillCooldowns.Size = new System.Drawing.Size(221, 188);
            this.grpSkillCooldowns.TabIndex = 138;
            this.grpSkillCooldowns.TabStop = false;
            this.grpSkillCooldowns.Text = "Skill Cooldowns";
            // 
            // btnHeal
            // 
            this.btnHeal.Location = new System.Drawing.Point(36, 730);
            this.btnHeal.Margin = new System.Windows.Forms.Padding(4);
            this.btnHeal.Name = "btnHeal";
            this.btnHeal.Size = new System.Drawing.Size(133, 34);
            this.btnHeal.TabIndex = 136;
            this.btnHeal.Text = "Heal";
            this.btnHeal.UseVisualStyleBackColor = true;
            this.btnHeal.Click += new System.EventHandler(this.btnHeal_Click);
            // 
            // btnSkills
            // 
            this.btnSkills.Location = new System.Drawing.Point(36, 689);
            this.btnSkills.Margin = new System.Windows.Forms.Padding(4);
            this.btnSkills.Name = "btnSkills";
            this.btnSkills.Size = new System.Drawing.Size(133, 34);
            this.btnSkills.TabIndex = 135;
            this.btnSkills.Text = "Skills";
            this.btnSkills.UseVisualStyleBackColor = true;
            this.btnSkills.Click += new System.EventHandler(this.btnSkills_Click);
            // 
            // lblAutoEscapeValue
            // 
            this.lblAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblAutoEscapeValue.ContextMenuStrip = this.ctxAutoEscape;
            this.lblAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblAutoEscapeValue.Location = new System.Drawing.Point(713, 161);
            this.lblAutoEscapeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAutoEscapeValue.Name = "lblAutoEscapeValue";
            this.lblAutoEscapeValue.Size = new System.Drawing.Size(149, 18);
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
            this.tsmiAutoEscapeHazy,
            this.tsmiAutoEscapeSeparator3,
            this.tsmiSetDefaultAutoEscape,
            this.tsmiRestoreDefaultAutoEscape});
            this.ctxAutoEscape.Name = "ctxAutoEscape";
            this.ctxAutoEscape.Size = new System.Drawing.Size(223, 190);
            this.ctxAutoEscape.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoEscape_Opening);
            // 
            // tsmiAutoEscapeIsActive
            // 
            this.tsmiAutoEscapeIsActive.Name = "tsmiAutoEscapeIsActive";
            this.tsmiAutoEscapeIsActive.Size = new System.Drawing.Size(222, 24);
            this.tsmiAutoEscapeIsActive.Text = "Is Active?";
            this.tsmiAutoEscapeIsActive.Click += new System.EventHandler(this.tsmiToggleAutoEscapeActive_Click);
            // 
            // tsmiAutoEscapeSeparator1
            // 
            this.tsmiAutoEscapeSeparator1.Name = "tsmiAutoEscapeSeparator1";
            this.tsmiAutoEscapeSeparator1.Size = new System.Drawing.Size(219, 6);
            // 
            // tsmiSetAutoEscapeThreshold
            // 
            this.tsmiSetAutoEscapeThreshold.Name = "tsmiSetAutoEscapeThreshold";
            this.tsmiSetAutoEscapeThreshold.Size = new System.Drawing.Size(222, 24);
            this.tsmiSetAutoEscapeThreshold.Text = "Set Threshold";
            this.tsmiSetAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiSetAutoEscapeThreshold_Click);
            // 
            // tsmiClearAutoEscapeThreshold
            // 
            this.tsmiClearAutoEscapeThreshold.Name = "tsmiClearAutoEscapeThreshold";
            this.tsmiClearAutoEscapeThreshold.Size = new System.Drawing.Size(222, 24);
            this.tsmiClearAutoEscapeThreshold.Text = "Clear Threshold";
            this.tsmiClearAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiClearAutoEscapeThreshold_Click);
            // 
            // tsmiAutoEscapeSeparator2
            // 
            this.tsmiAutoEscapeSeparator2.Name = "tsmiAutoEscapeSeparator2";
            this.tsmiAutoEscapeSeparator2.Size = new System.Drawing.Size(219, 6);
            // 
            // tsmiAutoEscapeFlee
            // 
            this.tsmiAutoEscapeFlee.Name = "tsmiAutoEscapeFlee";
            this.tsmiAutoEscapeFlee.Size = new System.Drawing.Size(222, 24);
            this.tsmiAutoEscapeFlee.Text = "Flee";
            // 
            // tsmiAutoEscapeHazy
            // 
            this.tsmiAutoEscapeHazy.Name = "tsmiAutoEscapeHazy";
            this.tsmiAutoEscapeHazy.Size = new System.Drawing.Size(222, 24);
            this.tsmiAutoEscapeHazy.Text = "Hazy";
            // 
            // tsmiAutoEscapeSeparator3
            // 
            this.tsmiAutoEscapeSeparator3.Name = "tsmiAutoEscapeSeparator3";
            this.tsmiAutoEscapeSeparator3.Size = new System.Drawing.Size(219, 6);
            // 
            // tsmiSetDefaultAutoEscape
            // 
            this.tsmiSetDefaultAutoEscape.Name = "tsmiSetDefaultAutoEscape";
            this.tsmiSetDefaultAutoEscape.Size = new System.Drawing.Size(222, 24);
            this.tsmiSetDefaultAutoEscape.Text = "Set Current as Default";
            this.tsmiSetDefaultAutoEscape.Click += new System.EventHandler(this.tsmiSetDefaultAutoEscape_Click);
            // 
            // tsmiRestoreDefaultAutoEscape
            // 
            this.tsmiRestoreDefaultAutoEscape.Name = "tsmiRestoreDefaultAutoEscape";
            this.tsmiRestoreDefaultAutoEscape.Size = new System.Drawing.Size(222, 24);
            this.tsmiRestoreDefaultAutoEscape.Text = "Restore Default";
            this.tsmiRestoreDefaultAutoEscape.Click += new System.EventHandler(this.tsmiRestoreDefaultAutoEscape_Click);
            // 
            // lblTime
            // 
            this.lblTime.BackColor = System.Drawing.Color.LightGray;
            this.lblTime.Location = new System.Drawing.Point(713, 137);
            this.lblTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(149, 18);
            this.lblTime.TabIndex = 125;
            this.lblTime.Text = "Time";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpCurrentPlayer
            // 
            this.grpCurrentPlayer.Controls.Add(this.lblToNextLevelValue);
            this.grpCurrentPlayer.Controls.Add(this.lblToNextLevel);
            this.grpCurrentPlayer.Controls.Add(this.lblManaValue);
            this.grpCurrentPlayer.Controls.Add(this.lblHitpointsValue);
            this.grpCurrentPlayer.Controls.Add(this.lblMana);
            this.grpCurrentPlayer.Controls.Add(this.lblHitpoints);
            this.grpCurrentPlayer.Location = new System.Drawing.Point(997, 578);
            this.grpCurrentPlayer.Margin = new System.Windows.Forms.Padding(4);
            this.grpCurrentPlayer.Name = "grpCurrentPlayer";
            this.grpCurrentPlayer.Padding = new System.Windows.Forms.Padding(4);
            this.grpCurrentPlayer.Size = new System.Drawing.Size(251, 105);
            this.grpCurrentPlayer.TabIndex = 122;
            this.grpCurrentPlayer.TabStop = false;
            this.grpCurrentPlayer.Text = "Current Player";
            // 
            // lblToNextLevelValue
            // 
            this.lblToNextLevelValue.BackColor = System.Drawing.Color.LightGray;
            this.lblToNextLevelValue.Location = new System.Drawing.Point(129, 68);
            this.lblToNextLevelValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblToNextLevelValue.Name = "lblToNextLevelValue";
            this.lblToNextLevelValue.Size = new System.Drawing.Size(115, 18);
            this.lblToNextLevelValue.TabIndex = 131;
            this.lblToNextLevelValue.Text = "Value";
            this.lblToNextLevelValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblToNextLevel
            // 
            this.lblToNextLevel.AutoSize = true;
            this.lblToNextLevel.Location = new System.Drawing.Point(7, 68);
            this.lblToNextLevel.Name = "lblToNextLevel";
            this.lblToNextLevel.Size = new System.Drawing.Size(93, 16);
            this.lblToNextLevel.TabIndex = 130;
            this.lblToNextLevel.Text = "To Next Level:";
            this.lblToNextLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblManaValue
            // 
            this.lblManaValue.BackColor = System.Drawing.Color.LightGray;
            this.lblManaValue.Location = new System.Drawing.Point(129, 44);
            this.lblManaValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblManaValue.Name = "lblManaValue";
            this.lblManaValue.Size = new System.Drawing.Size(115, 18);
            this.lblManaValue.TabIndex = 127;
            this.lblManaValue.Text = "Value";
            this.lblManaValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHitpointsValue
            // 
            this.lblHitpointsValue.BackColor = System.Drawing.Color.LightGray;
            this.lblHitpointsValue.Location = new System.Drawing.Point(129, 21);
            this.lblHitpointsValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHitpointsValue.Name = "lblHitpointsValue";
            this.lblHitpointsValue.Size = new System.Drawing.Size(115, 18);
            this.lblHitpointsValue.TabIndex = 126;
            this.lblHitpointsValue.Text = "Value";
            this.lblHitpointsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Location = new System.Drawing.Point(48, 44);
            this.lblMana.Name = "lblMana";
            this.lblMana.Size = new System.Drawing.Size(44, 16);
            this.lblMana.TabIndex = 92;
            this.lblMana.Text = "Mana:";
            this.lblMana.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHitpoints
            // 
            this.lblHitpoints.AutoSize = true;
            this.lblHitpoints.Location = new System.Drawing.Point(48, 21);
            this.lblHitpoints.Name = "lblHitpoints";
            this.lblHitpoints.Size = new System.Drawing.Size(62, 16);
            this.lblHitpoints.TabIndex = 100;
            this.lblHitpoints.Text = "Hitpoints:";
            this.lblHitpoints.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpMessages
            // 
            this.grpMessages.Controls.Add(this.lstMessages);
            this.grpMessages.Location = new System.Drawing.Point(35, 332);
            this.grpMessages.Margin = new System.Windows.Forms.Padding(4);
            this.grpMessages.Name = "grpMessages";
            this.grpMessages.Padding = new System.Windows.Forms.Padding(4);
            this.grpMessages.Size = new System.Drawing.Size(797, 222);
            this.grpMessages.TabIndex = 121;
            this.grpMessages.TabStop = false;
            this.grpMessages.Text = "Messages";
            // 
            // lstMessages
            // 
            this.lstMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.ItemHeight = 16;
            this.lstMessages.Location = new System.Drawing.Point(4, 19);
            this.lstMessages.Margin = new System.Windows.Forms.Padding(4);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(789, 199);
            this.lstMessages.TabIndex = 0;
            // 
            // grpMob
            // 
            this.grpMob.Controls.Add(this.txtMobStatus);
            this.grpMob.Controls.Add(this.lblMobStatus);
            this.grpMob.Controls.Add(this.txtMobDamage);
            this.grpMob.Controls.Add(this.lblMobDamage);
            this.grpMob.Location = new System.Drawing.Point(996, 690);
            this.grpMob.Margin = new System.Windows.Forms.Padding(4);
            this.grpMob.Name = "grpMob";
            this.grpMob.Padding = new System.Windows.Forms.Padding(4);
            this.grpMob.Size = new System.Drawing.Size(255, 74);
            this.grpMob.TabIndex = 120;
            this.grpMob.TabStop = false;
            this.grpMob.Text = "Mob";
            // 
            // txtMobStatus
            // 
            this.txtMobStatus.Location = new System.Drawing.Point(128, 42);
            this.txtMobStatus.Margin = new System.Windows.Forms.Padding(4);
            this.txtMobStatus.Name = "txtMobStatus";
            this.txtMobStatus.ReadOnly = true;
            this.txtMobStatus.Size = new System.Drawing.Size(113, 22);
            this.txtMobStatus.TabIndex = 3;
            // 
            // lblMobStatus
            // 
            this.lblMobStatus.AutoSize = true;
            this.lblMobStatus.Location = new System.Drawing.Point(55, 46);
            this.lblMobStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMobStatus.Name = "lblMobStatus";
            this.lblMobStatus.Size = new System.Drawing.Size(47, 16);
            this.lblMobStatus.TabIndex = 2;
            this.lblMobStatus.Text = "Status:";
            // 
            // txtMobDamage
            // 
            this.txtMobDamage.Location = new System.Drawing.Point(128, 11);
            this.txtMobDamage.Margin = new System.Windows.Forms.Padding(4);
            this.txtMobDamage.Name = "txtMobDamage";
            this.txtMobDamage.ReadOnly = true;
            this.txtMobDamage.Size = new System.Drawing.Size(113, 22);
            this.txtMobDamage.TabIndex = 1;
            // 
            // lblMobDamage
            // 
            this.lblMobDamage.AutoSize = true;
            this.lblMobDamage.Location = new System.Drawing.Point(55, 15);
            this.lblMobDamage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMobDamage.Name = "lblMobDamage";
            this.lblMobDamage.Size = new System.Drawing.Size(63, 16);
            this.lblMobDamage.TabIndex = 0;
            this.lblMobDamage.Text = "Damage:";
            // 
            // btnGraph
            // 
            this.btnGraph.Location = new System.Drawing.Point(713, 17);
            this.btnGraph.Margin = new System.Windows.Forms.Padding(4);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(149, 34);
            this.btnGraph.TabIndex = 119;
            this.btnGraph.Text = "Graph";
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // btnClearCurrentLocation
            // 
            this.btnClearCurrentLocation.Location = new System.Drawing.Point(713, 97);
            this.btnClearCurrentLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClearCurrentLocation.Name = "btnClearCurrentLocation";
            this.btnClearCurrentLocation.Size = new System.Drawing.Size(149, 34);
            this.btnClearCurrentLocation.TabIndex = 118;
            this.btnClearCurrentLocation.Text = "Clear Location";
            this.btnClearCurrentLocation.UseVisualStyleBackColor = true;
            this.btnClearCurrentLocation.Click += new System.EventHandler(this.btnClearCurrentLocation_Click);
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
            this.grpSingleMove.Location = new System.Drawing.Point(364, 22);
            this.grpSingleMove.Margin = new System.Windows.Forms.Padding(4);
            this.grpSingleMove.Name = "grpSingleMove";
            this.grpSingleMove.Padding = new System.Windows.Forms.Padding(4);
            this.grpSingleMove.Size = new System.Drawing.Size(228, 129);
            this.grpSingleMove.TabIndex = 117;
            this.grpSingleMove.TabStop = false;
            this.grpSingleMove.Text = "Single Move";
            // 
            // btnOut
            // 
            this.btnOut.Location = new System.Drawing.Point(167, 87);
            this.btnOut.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(45, 28);
            this.btnOut.TabIndex = 113;
            this.btnOut.Tag = "out";
            this.btnOut.Text = "Out";
            this.btnOut.UseVisualStyleBackColor = true;
            this.btnOut.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnOtherSingleMove
            // 
            this.btnOtherSingleMove.Location = new System.Drawing.Point(64, 53);
            this.btnOtherSingleMove.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOtherSingleMove.Name = "btnOtherSingleMove";
            this.btnOtherSingleMove.Size = new System.Drawing.Size(45, 28);
            this.btnOtherSingleMove.TabIndex = 112;
            this.btnOtherSingleMove.Tag = "";
            this.btnOtherSingleMove.Text = "?";
            this.btnOtherSingleMove.UseVisualStyleBackColor = true;
            this.btnOtherSingleMove.Click += new System.EventHandler(this.btnOtherSingleMove_Click);
            // 
            // btnDn
            // 
            this.btnDn.Location = new System.Drawing.Point(167, 54);
            this.btnDn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDn.Name = "btnDn";
            this.btnDn.Size = new System.Drawing.Size(45, 28);
            this.btnDn.TabIndex = 111;
            this.btnDn.Tag = "down";
            this.btnDn.Text = "Dn";
            this.btnDn.UseVisualStyleBackColor = true;
            this.btnDn.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(167, 22);
            this.btnUp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(45, 28);
            this.btnUp.TabIndex = 110;
            this.btnUp.Tag = "up";
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnDoSingleMove_Click);
            // 
            // grpSpells
            // 
            this.grpSpells.Controls.Add(this.flpSpells);
            this.grpSpells.Location = new System.Drawing.Point(313, 571);
            this.grpSpells.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpSpells.Name = "grpSpells";
            this.grpSpells.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpSpells.Size = new System.Drawing.Size(216, 188);
            this.grpSpells.TabIndex = 108;
            this.grpSpells.TabStop = false;
            this.grpSpells.Text = "Active Spells";
            // 
            // flpSpells
            // 
            this.flpSpells.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSpells.Location = new System.Drawing.Point(3, 17);
            this.flpSpells.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flpSpells.Name = "flpSpells";
            this.flpSpells.Size = new System.Drawing.Size(210, 169);
            this.flpSpells.TabIndex = 0;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(173, 649);
            this.btnRemoveAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(133, 34);
            this.btnRemoveAll.TabIndex = 87;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLevel3OffensiveSpell
            // 
            this.btnLevel3OffensiveSpell.Location = new System.Drawing.Point(883, 665);
            this.btnLevel3OffensiveSpell.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLevel3OffensiveSpell.Name = "btnLevel3OffensiveSpell";
            this.btnLevel3OffensiveSpell.Size = new System.Drawing.Size(108, 34);
            this.btnLevel3OffensiveSpell.TabIndex = 84;
            this.btnLevel3OffensiveSpell.Text = "Cast Level 3";
            this.btnLevel3OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel3OffensiveSpell.Click += new System.EventHandler(this.btnLevel3OffensiveSpell_Click);
            // 
            // btnStunMob
            // 
            this.btnStunMob.Location = new System.Drawing.Point(772, 583);
            this.btnStunMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStunMob.Name = "btnStunMob";
            this.btnStunMob.Size = new System.Drawing.Size(105, 34);
            this.btnStunMob.TabIndex = 83;
            this.btnStunMob.Text = "Stun";
            this.btnStunMob.UseVisualStyleBackColor = true;
            this.btnStunMob.Click += new System.EventHandler(this.btnStun_Click);
            // 
            // btnCastMend
            // 
            this.btnCastMend.Location = new System.Drawing.Point(772, 821);
            this.btnCastMend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCastMend.Name = "btnCastMend";
            this.btnCastMend.Size = new System.Drawing.Size(107, 34);
            this.btnCastMend.TabIndex = 82;
            this.btnCastMend.Text = "Mend";
            this.btnCastMend.UseVisualStyleBackColor = true;
            this.btnCastMend.Click += new System.EventHandler(this.btnMendWounds_Click);
            // 
            // btnReddishOrange
            // 
            this.btnReddishOrange.Location = new System.Drawing.Point(885, 821);
            this.btnReddishOrange.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReddishOrange.Name = "btnReddishOrange";
            this.btnReddishOrange.Size = new System.Drawing.Size(105, 34);
            this.btnReddishOrange.TabIndex = 81;
            this.btnReddishOrange.Text = "R/O pot";
            this.btnReddishOrange.UseVisualStyleBackColor = true;
            this.btnReddishOrange.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(36, 649);
            this.btnHide.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(133, 34);
            this.btnHide.TabIndex = 80;
            this.btnHide.Tag = "hide";
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(36, 610);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(133, 34);
            this.btnSearch.TabIndex = 79;
            this.btnSearch.Tag = "search";
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tabAncillary
            // 
            this.tabAncillary.Controls.Add(this.pnlAncillary);
            this.tabAncillary.Location = new System.Drawing.Point(4, 25);
            this.tabAncillary.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabAncillary.Name = "tabAncillary";
            this.tabAncillary.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabAncillary.Size = new System.Drawing.Size(1262, 984);
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
            this.pnlAncillary.Location = new System.Drawing.Point(3, 2);
            this.pnlAncillary.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlAncillary.Name = "pnlAncillary";
            this.pnlAncillary.Size = new System.Drawing.Size(1256, 980);
            this.pnlAncillary.TabIndex = 0;
            // 
            // tabEmotes
            // 
            this.tabEmotes.Controls.Add(this.pnlEmotes);
            this.tabEmotes.Location = new System.Drawing.Point(4, 25);
            this.tabEmotes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabEmotes.Name = "tabEmotes";
            this.tabEmotes.Size = new System.Drawing.Size(1262, 984);
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
            this.pnlEmotes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlEmotes.Name = "pnlEmotes";
            this.pnlEmotes.Size = new System.Drawing.Size(1262, 984);
            this.pnlEmotes.TabIndex = 12;
            // 
            // btnSay
            // 
            this.btnSay.Location = new System.Drawing.Point(465, 11);
            this.btnSay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSay.Name = "btnSay";
            this.btnSay.Size = new System.Drawing.Size(81, 25);
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
            this.chkShowEmotesWithoutTarget.Location = new System.Drawing.Point(379, 42);
            this.chkShowEmotesWithoutTarget.Margin = new System.Windows.Forms.Padding(4);
            this.chkShowEmotesWithoutTarget.Name = "chkShowEmotesWithoutTarget";
            this.chkShowEmotesWithoutTarget.Size = new System.Drawing.Size(197, 20);
            this.chkShowEmotesWithoutTarget.TabIndex = 14;
            this.chkShowEmotesWithoutTarget.Text = "Show Emotes without Target";
            this.chkShowEmotesWithoutTarget.UseVisualStyleBackColor = true;
            this.chkShowEmotesWithoutTarget.CheckedChanged += new System.EventHandler(this.chkShowEmotesWithoutTarget_CheckedChanged);
            // 
            // lblEmoteTarget
            // 
            this.lblEmoteTarget.AutoSize = true;
            this.lblEmoteTarget.Location = new System.Drawing.Point(16, 43);
            this.lblEmoteTarget.Name = "lblEmoteTarget";
            this.lblEmoteTarget.Size = new System.Drawing.Size(50, 16);
            this.lblEmoteTarget.TabIndex = 12;
            this.lblEmoteTarget.Text = "Target:";
            // 
            // txtEmoteTarget
            // 
            this.txtEmoteTarget.Location = new System.Drawing.Point(124, 39);
            this.txtEmoteTarget.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtEmoteTarget.Name = "txtEmoteTarget";
            this.txtEmoteTarget.Size = new System.Drawing.Size(247, 22);
            this.txtEmoteTarget.TabIndex = 13;
            this.txtEmoteTarget.TextChanged += new System.EventHandler(this.txtEmoteTarget_TextChanged);
            // 
            // lblCommandText
            // 
            this.lblCommandText.AutoSize = true;
            this.lblCommandText.Location = new System.Drawing.Point(16, 16);
            this.lblCommandText.Name = "lblCommandText";
            this.lblCommandText.Size = new System.Drawing.Size(95, 16);
            this.lblCommandText.TabIndex = 8;
            this.lblCommandText.Text = "Command text:";
            // 
            // btnEmote
            // 
            this.btnEmote.Location = new System.Drawing.Point(379, 11);
            this.btnEmote.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEmote.Name = "btnEmote";
            this.btnEmote.Size = new System.Drawing.Size(81, 25);
            this.btnEmote.TabIndex = 11;
            this.btnEmote.Text = "Emote";
            this.btnEmote.UseVisualStyleBackColor = true;
            this.btnEmote.Click += new System.EventHandler(this.btnEmote_Click);
            // 
            // txtCommandText
            // 
            this.txtCommandText.Location = new System.Drawing.Point(124, 12);
            this.txtCommandText.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCommandText.Name = "txtCommandText";
            this.txtCommandText.Size = new System.Drawing.Size(248, 22);
            this.txtCommandText.TabIndex = 9;
            this.txtCommandText.TextChanged += new System.EventHandler(this.txtEmoteText_TextChanged);
            // 
            // grpEmotes
            // 
            this.grpEmotes.Controls.Add(this.flpEmotes);
            this.grpEmotes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpEmotes.Location = new System.Drawing.Point(0, -58);
            this.grpEmotes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpEmotes.Name = "grpEmotes";
            this.grpEmotes.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpEmotes.Size = new System.Drawing.Size(1262, 1042);
            this.grpEmotes.TabIndex = 10;
            this.grpEmotes.TabStop = false;
            this.grpEmotes.Text = "Emotes";
            // 
            // flpEmotes
            // 
            this.flpEmotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpEmotes.Location = new System.Drawing.Point(3, 17);
            this.flpEmotes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flpEmotes.Name = "flpEmotes";
            this.flpEmotes.Size = new System.Drawing.Size(1256, 1023);
            this.flpEmotes.TabIndex = 0;
            // 
            // tabHelp
            // 
            this.tabHelp.Controls.Add(this.grpHelp);
            this.tabHelp.Location = new System.Drawing.Point(4, 25);
            this.tabHelp.Margin = new System.Windows.Forms.Padding(4);
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Size = new System.Drawing.Size(1262, 984);
            this.tabHelp.TabIndex = 3;
            this.tabHelp.Text = "Help";
            this.tabHelp.UseVisualStyleBackColor = true;
            // 
            // grpHelp
            // 
            this.grpHelp.Controls.Add(this.flpHelp);
            this.grpHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHelp.Location = new System.Drawing.Point(0, 0);
            this.grpHelp.Margin = new System.Windows.Forms.Padding(4);
            this.grpHelp.Name = "grpHelp";
            this.grpHelp.Padding = new System.Windows.Forms.Padding(4);
            this.grpHelp.Size = new System.Drawing.Size(1262, 984);
            this.grpHelp.TabIndex = 0;
            this.grpHelp.TabStop = false;
            this.grpHelp.Text = "Help";
            // 
            // flpHelp
            // 
            this.flpHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpHelp.Location = new System.Drawing.Point(4, 19);
            this.flpHelp.Margin = new System.Windows.Forms.Padding(4);
            this.flpHelp.Name = "flpHelp";
            this.flpHelp.Size = new System.Drawing.Size(1254, 961);
            this.flpHelp.TabIndex = 0;
            // 
            // ctxRoomExits
            // 
            this.ctxRoomExits.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxRoomExits.Name = "ctxRoomExits";
            this.ctxRoomExits.Size = new System.Drawing.Size(61, 4);
            this.ctxRoomExits.Opening += new System.ComponentModel.CancelEventHandler(this.ctxRoomExits_Opening);
            this.ctxRoomExits.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxRoomExits_ItemClicked);
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
            this.scMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.pnlTabControl);
            this.scMain.Panel1.Controls.Add(this.tsTopMenu);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.grpConsole);
            this.scMain.Size = new System.Drawing.Size(1924, 1040);
            this.scMain.SplitterDistance = 1270;
            this.scMain.TabIndex = 81;
            // 
            // pnlTabControl
            // 
            this.pnlTabControl.Controls.Add(this.tcMain);
            this.pnlTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTabControl.Location = new System.Drawing.Point(0, 27);
            this.pnlTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTabControl.Name = "pnlTabControl";
            this.pnlTabControl.Size = new System.Drawing.Size(1270, 1013);
            this.pnlTabControl.TabIndex = 81;
            // 
            // tsTopMenu
            // 
            this.tsTopMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbInformation,
            this.tsbInventory,
            this.tsbEquipment,
            this.tsbWho,
            this.tsbUptime,
            this.tsbScore,
            this.tsbTime,
            this.tsbConfiguration,
            this.tsbQuit});
            this.tsTopMenu.Location = new System.Drawing.Point(0, 0);
            this.tsTopMenu.Name = "tsTopMenu";
            this.tsTopMenu.Size = new System.Drawing.Size(1270, 27);
            this.tsTopMenu.TabIndex = 80;
            this.tsTopMenu.Text = "toolStrip1";
            // 
            // tsbInformation
            // 
            this.tsbInformation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInformation.Image = ((System.Drawing.Image)(resources.GetObject("tsbInformation.Image")));
            this.tsbInformation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInformation.Name = "tsbInformation";
            this.tsbInformation.Size = new System.Drawing.Size(91, 24);
            this.tsbInformation.Tag = "information";
            this.tsbInformation.Text = "Information";
            this.tsbInformation.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbInventory
            // 
            this.tsbInventory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInventory.Image = ((System.Drawing.Image)(resources.GetObject("tsbInventory.Image")));
            this.tsbInventory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInventory.Name = "tsbInventory";
            this.tsbInventory.Size = new System.Drawing.Size(74, 24);
            this.tsbInventory.Text = "Inventory";
            this.tsbInventory.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbEquipment
            // 
            this.tsbEquipment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbEquipment.Image = ((System.Drawing.Image)(resources.GetObject("tsbEquipment.Image")));
            this.tsbEquipment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEquipment.Name = "tsbEquipment";
            this.tsbEquipment.Size = new System.Drawing.Size(85, 24);
            this.tsbEquipment.Tag = "equipment";
            this.tsbEquipment.Text = "Equipment";
            this.tsbEquipment.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbWho
            // 
            this.tsbWho.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbWho.Image = ((System.Drawing.Image)(resources.GetObject("tsbWho.Image")));
            this.tsbWho.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWho.Name = "tsbWho";
            this.tsbWho.Size = new System.Drawing.Size(44, 24);
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
            this.tsbUptime.Size = new System.Drawing.Size(62, 24);
            this.tsbUptime.Tag = "uptime";
            this.tsbUptime.Text = "Uptime";
            this.tsbUptime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbScore
            // 
            this.tsbScore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbScore.Image = ((System.Drawing.Image)(resources.GetObject("tsbScore.Image")));
            this.tsbScore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScore.Name = "tsbScore";
            this.tsbScore.Size = new System.Drawing.Size(50, 24);
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
            this.tsbTime.Size = new System.Drawing.Size(46, 24);
            this.tsbTime.Tag = "time";
            this.tsbTime.Text = "Time";
            this.tsbTime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // tsbConfiguration
            // 
            this.tsbConfiguration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbConfiguration.Image = ((System.Drawing.Image)(resources.GetObject("tsbConfiguration.Image")));
            this.tsbConfiguration.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConfiguration.Name = "tsbConfiguration";
            this.tsbConfiguration.Size = new System.Drawing.Size(104, 24);
            this.tsbConfiguration.Text = "Configuration";
            this.tsbConfiguration.Click += new System.EventHandler(this.tsbConfiguration_Click);
            // 
            // tsbQuit
            // 
            this.tsbQuit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbQuit.Image = ((System.Drawing.Image)(resources.GetObject("tsbQuit.Image")));
            this.tsbQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbQuit.Name = "tsbQuit";
            this.tsbQuit.Size = new System.Drawing.Size(41, 24);
            this.tsbQuit.Text = "Quit";
            this.tsbQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // grpConsole
            // 
            this.grpConsole.Controls.Add(this.pnlConsoleHolder);
            this.grpConsole.Controls.Add(this.pnlCommand);
            this.grpConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConsole.Location = new System.Drawing.Point(0, 0);
            this.grpConsole.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpConsole.Name = "grpConsole";
            this.grpConsole.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpConsole.Size = new System.Drawing.Size(650, 1040);
            this.grpConsole.TabIndex = 110;
            this.grpConsole.TabStop = false;
            this.grpConsole.Text = "Console";
            // 
            // pnlConsoleHolder
            // 
            this.pnlConsoleHolder.Controls.Add(this.rtbConsole);
            this.pnlConsoleHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConsoleHolder.Location = new System.Drawing.Point(3, 17);
            this.pnlConsoleHolder.Margin = new System.Windows.Forms.Padding(4);
            this.pnlConsoleHolder.Name = "pnlConsoleHolder";
            this.pnlConsoleHolder.Size = new System.Drawing.Size(644, 959);
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
            this.rtbConsole.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(644, 959);
            this.rtbConsole.TabIndex = 0;
            this.rtbConsole.Text = "";
            // 
            // ctxConsole
            // 
            this.ctxConsole.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxConsole.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiClearConsole});
            this.ctxConsole.Name = "ctxConsole";
            this.ctxConsole.Size = new System.Drawing.Size(113, 28);
            this.ctxConsole.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxConsole_ItemClicked);
            // 
            // tsmiClearConsole
            // 
            this.tsmiClearConsole.Name = "tsmiClearConsole";
            this.tsmiClearConsole.Size = new System.Drawing.Size(112, 24);
            this.tsmiClearConsole.Text = "Clear";
            // 
            // pnlCommand
            // 
            this.pnlCommand.Controls.Add(this.txtOneOffCommand);
            this.pnlCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCommand.Location = new System.Drawing.Point(3, 976);
            this.pnlCommand.Margin = new System.Windows.Forms.Padding(4);
            this.pnlCommand.Name = "pnlCommand";
            this.pnlCommand.Size = new System.Drawing.Size(644, 62);
            this.pnlCommand.TabIndex = 30;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 1040);
            this.Controls.Add(this.scMain);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Isengard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.grpOneClickStrategies.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.grpCurrentRoom.ResumeLayout(false);
            this.ctxCurrentRoom.ResumeLayout(false);
            this.ctxAutoEscape.ResumeLayout(false);
            this.grpCurrentPlayer.ResumeLayout(false);
            this.grpCurrentPlayer.PerformLayout();
            this.grpMessages.ResumeLayout(false);
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
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel1.PerformLayout();
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnDrinkYellow;
        private System.Windows.Forms.Button btnDrinkGreen;
        private System.Windows.Forms.TextBox txtWeapon;
        private System.Windows.Forms.Label lblWeapon;
        private System.Windows.Forms.Button btnWieldWeapon;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.ComboBox cboSetOption;
        private System.Windows.Forms.CheckBox chkSetOn;
        private System.Windows.Forms.TextBox txtWand;
        private System.Windows.Forms.Label lblWand;
        private System.Windows.Forms.Button btnUseWandOnMob;
        private System.Windows.Forms.Button btnPowerAttackMob;
        private System.Windows.Forms.TextBox txtSetValue;
        private System.Windows.Forms.GroupBox grpOneClickStrategies;
        private System.Windows.Forms.FlowLayoutPanel flpOneClickStrategies;
        private System.Windows.Forms.TextBox txtPotion;
        private System.Windows.Forms.Label lblPotion;
        private System.Windows.Forms.Button btnRemoveWeapon;
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
        private System.Windows.Forms.Button btnReddishOrange;
        private System.Windows.Forms.Button btnCastMend;
        private System.Windows.Forms.Button btnStunMob;
        private System.Windows.Forms.Button btnLevel3OffensiveSpell;
        private System.Windows.Forms.Button btnRemoveAll;
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
        private System.Windows.Forms.ContextMenuStrip ctxRoomExits;
        private System.Windows.Forms.Button btnSay;
        private System.Windows.Forms.GroupBox grpSingleMove;
        private System.Windows.Forms.Button btnClearCurrentLocation;
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
        private System.Windows.Forms.ToolStripButton tsbInventory;
        private System.Windows.Forms.ToolStripButton tsbEquipment;
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
        private System.Windows.Forms.ToolStripButton tsbConfiguration;
        private System.Windows.Forms.Label lblAutoEscapeValue;
        private System.Windows.Forms.ContextMenuStrip ctxAutoEscape;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeIsActive;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetDefaultAutoEscape;
        private System.Windows.Forms.Button btnSkills;
        private System.Windows.Forms.Button btnHeal;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreDefaultAutoEscape;
        private System.Windows.Forms.Label lblToNextLevelValue;
        private System.Windows.Forms.Label lblToNextLevel;
        private System.Windows.Forms.GroupBox grpSkillCooldowns;
        private System.Windows.Forms.Button btnIncrementWand;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeFlee;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeHazy;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator2;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator3;
        private System.Windows.Forms.Button btnLocations;
        private System.Windows.Forms.GroupBox grpCurrentRoom;
        private System.Windows.Forms.ComboBox cboTickRoom;
        private System.Windows.Forms.Label lblTickRoom;
        private System.Windows.Forms.Button btnGoToHealingRoom;
        private System.Windows.Forms.TreeView treeCurrentRoom;
        private System.Windows.Forms.ContextMenuStrip ctxCurrentRoom;
        private System.Windows.Forms.ToolStripMenuItem tsmiGoToRoom;
        private System.Windows.Forms.Button btnOut;
    }
}

