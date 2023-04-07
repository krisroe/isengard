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
            this.btnLevel1OffensiveSpell = new System.Windows.Forms.Button();
            this.lblWindow = new System.Windows.Forms.Label();
            this.txtWindow = new System.Windows.Forms.TextBox();
            this.txtMob = new System.Windows.Forms.TextBox();
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
            this.cboRoom = new System.Windows.Forms.ComboBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.btnSetCurrentLocation = new System.Windows.Forms.Button();
            this.btnGoToLocation = new System.Windows.Forms.Button();
            this.btnCastProtection = new System.Windows.Forms.Button();
            this.chkIsNight = new System.Windows.Forms.CheckBox();
            this.txtOneOffCommand = new System.Windows.Forms.TextBox();
            this.lblOneOffCommand = new System.Windows.Forms.Label();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnClearOneOff = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnOneOffExecute = new System.Windows.Forms.Button();
            this.btnAttackMob = new System.Windows.Forms.Button();
            this.cboArea = new System.Windows.Forms.ComboBox();
            this.txtCurrentRoom = new System.Windows.Forms.TextBox();
            this.lblCurrentRoom = new System.Windows.Forms.Label();
            this.btnDrinkYellow = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
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
            this.SuspendLayout();
            // 
            // btnLevel1OffensiveSpell
            // 
            this.btnLevel1OffensiveSpell.Location = new System.Drawing.Point(111, 146);
            this.btnLevel1OffensiveSpell.Name = "btnLevel1OffensiveSpell";
            this.btnLevel1OffensiveSpell.Size = new System.Drawing.Size(156, 35);
            this.btnLevel1OffensiveSpell.TabIndex = 0;
            this.btnLevel1OffensiveSpell.Tag = "cast hurt {0}";
            this.btnLevel1OffensiveSpell.Text = "Level 1 Offensive Spell";
            this.btnLevel1OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel1OffensiveSpell.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // lblWindow
            // 
            this.lblWindow.Location = new System.Drawing.Point(35, 22);
            this.lblWindow.Name = "lblWindow";
            this.lblWindow.Size = new System.Drawing.Size(68, 26);
            this.lblWindow.TabIndex = 1;
            this.lblWindow.Text = "Window:";
            this.lblWindow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtWindow
            // 
            this.txtWindow.Location = new System.Drawing.Point(109, 26);
            this.txtWindow.Name = "txtWindow";
            this.txtWindow.Size = new System.Drawing.Size(237, 22);
            this.txtWindow.TabIndex = 2;
            this.txtWindow.Text = "Telnet isengard.nazgul.com";
            // 
            // txtMob
            // 
            this.txtMob.Location = new System.Drawing.Point(109, 54);
            this.txtMob.Name = "txtMob";
            this.txtMob.Size = new System.Drawing.Size(237, 22);
            this.txtMob.TabIndex = 4;
            // 
            // lblMob
            // 
            this.lblMob.Location = new System.Drawing.Point(35, 50);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(70, 33);
            this.lblMob.TabIndex = 3;
            this.lblMob.Text = "Mob:";
            this.lblMob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLevel2OffensiveSpell
            // 
            this.btnLevel2OffensiveSpell.Location = new System.Drawing.Point(111, 187);
            this.btnLevel2OffensiveSpell.Name = "btnLevel2OffensiveSpell";
            this.btnLevel2OffensiveSpell.Size = new System.Drawing.Size(156, 35);
            this.btnLevel2OffensiveSpell.TabIndex = 5;
            this.btnLevel2OffensiveSpell.Tag = "cast dustgust {0}";
            this.btnLevel2OffensiveSpell.Text = "Level 2 Offensive Spell";
            this.btnLevel2OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel2OffensiveSpell.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnFlee
            // 
            this.btnFlee.Location = new System.Drawing.Point(111, 228);
            this.btnFlee.Name = "btnFlee";
            this.btnFlee.Size = new System.Drawing.Size(156, 35);
            this.btnFlee.TabIndex = 6;
            this.btnFlee.Tag = "flee";
            this.btnFlee.Text = "Flee";
            this.btnFlee.UseVisualStyleBackColor = true;
            this.btnFlee.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnDrinkHazy
            // 
            this.btnDrinkHazy.Location = new System.Drawing.Point(111, 269);
            this.btnDrinkHazy.Name = "btnDrinkHazy";
            this.btnDrinkHazy.Size = new System.Drawing.Size(156, 35);
            this.btnDrinkHazy.TabIndex = 7;
            this.btnDrinkHazy.Tag = "drink hazy";
            this.btnDrinkHazy.Text = "Drink Hazy";
            this.btnDrinkHazy.UseVisualStyleBackColor = true;
            this.btnDrinkHazy.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLookAtMob
            // 
            this.btnLookAtMob.Location = new System.Drawing.Point(273, 146);
            this.btnLookAtMob.Name = "btnLookAtMob";
            this.btnLookAtMob.Size = new System.Drawing.Size(156, 35);
            this.btnLookAtMob.TabIndex = 8;
            this.btnLookAtMob.Tag = "look {0}";
            this.btnLookAtMob.Text = "Look at Mob";
            this.btnLookAtMob.UseVisualStyleBackColor = true;
            this.btnLookAtMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLook
            // 
            this.btnLook.Location = new System.Drawing.Point(436, 146);
            this.btnLook.Name = "btnLook";
            this.btnLook.Size = new System.Drawing.Size(156, 35);
            this.btnLook.TabIndex = 9;
            this.btnLook.Tag = "look";
            this.btnLook.Text = "Look";
            this.btnLook.UseVisualStyleBackColor = true;
            this.btnLook.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastVigor
            // 
            this.btnCastVigor.Location = new System.Drawing.Point(597, 146);
            this.btnCastVigor.Name = "btnCastVigor";
            this.btnCastVigor.Size = new System.Drawing.Size(124, 35);
            this.btnCastVigor.TabIndex = 10;
            this.btnCastVigor.Tag = "cast vigor";
            this.btnCastVigor.Text = "Cast Vigor";
            this.btnCastVigor.UseVisualStyleBackColor = true;
            this.btnCastVigor.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnManashield
            // 
            this.btnManashield.Location = new System.Drawing.Point(273, 310);
            this.btnManashield.Name = "btnManashield";
            this.btnManashield.Size = new System.Drawing.Size(156, 35);
            this.btnManashield.TabIndex = 11;
            this.btnManashield.Tag = "manashield";
            this.btnManashield.Text = "Manashield";
            this.btnManashield.UseVisualStyleBackColor = true;
            this.btnManashield.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastCurePoison
            // 
            this.btnCastCurePoison.Location = new System.Drawing.Point(597, 187);
            this.btnCastCurePoison.Name = "btnCastCurePoison";
            this.btnCastCurePoison.Size = new System.Drawing.Size(124, 35);
            this.btnCastCurePoison.TabIndex = 18;
            this.btnCastCurePoison.Tag = "cast cure-poison";
            this.btnCastCurePoison.Text = "Cast Curepoison";
            this.btnCastCurePoison.UseVisualStyleBackColor = true;
            this.btnCastCurePoison.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnTime
            // 
            this.btnTime.Location = new System.Drawing.Point(436, 104);
            this.btnTime.Name = "btnTime";
            this.btnTime.Size = new System.Drawing.Size(156, 35);
            this.btnTime.TabIndex = 19;
            this.btnTime.Tag = "time";
            this.btnTime.Text = "Time";
            this.btnTime.UseVisualStyleBackColor = true;
            this.btnTime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnScore
            // 
            this.btnScore.Location = new System.Drawing.Point(436, 188);
            this.btnScore.Name = "btnScore";
            this.btnScore.Size = new System.Drawing.Size(156, 35);
            this.btnScore.TabIndex = 20;
            this.btnScore.Tag = "score";
            this.btnScore.Text = "Score";
            this.btnScore.UseVisualStyleBackColor = true;
            this.btnScore.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnInformation
            // 
            this.btnInformation.Location = new System.Drawing.Point(436, 229);
            this.btnInformation.Name = "btnInformation";
            this.btnInformation.Size = new System.Drawing.Size(156, 35);
            this.btnInformation.TabIndex = 21;
            this.btnInformation.Tag = "information";
            this.btnInformation.Text = "Information";
            this.btnInformation.UseVisualStyleBackColor = true;
            this.btnInformation.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // cboRoom
            // 
            this.cboRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoom.FormattingEnabled = true;
            this.cboRoom.Location = new System.Drawing.Point(262, 439);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Size = new System.Drawing.Size(156, 24);
            this.cboRoom.TabIndex = 22;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(33, 442);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(61, 16);
            this.lblLocation.TabIndex = 23;
            this.lblLocation.Text = "Location:";
            // 
            // btnSetCurrentLocation
            // 
            this.btnSetCurrentLocation.Location = new System.Drawing.Point(436, 441);
            this.btnSetCurrentLocation.Name = "btnSetCurrentLocation";
            this.btnSetCurrentLocation.Size = new System.Drawing.Size(155, 26);
            this.btnSetCurrentLocation.TabIndex = 24;
            this.btnSetCurrentLocation.Text = "Set Current Location";
            this.btnSetCurrentLocation.UseVisualStyleBackColor = true;
            this.btnSetCurrentLocation.Click += new System.EventHandler(this.btnSetCurrentLocation_Click);
            // 
            // btnGoToLocation
            // 
            this.btnGoToLocation.Location = new System.Drawing.Point(436, 473);
            this.btnGoToLocation.Name = "btnGoToLocation";
            this.btnGoToLocation.Size = new System.Drawing.Size(155, 26);
            this.btnGoToLocation.TabIndex = 25;
            this.btnGoToLocation.Text = "Go to Location";
            this.btnGoToLocation.UseVisualStyleBackColor = true;
            this.btnGoToLocation.Click += new System.EventHandler(this.btnGoToLocation_Click);
            // 
            // btnCastProtection
            // 
            this.btnCastProtection.Location = new System.Drawing.Point(597, 228);
            this.btnCastProtection.Name = "btnCastProtection";
            this.btnCastProtection.Size = new System.Drawing.Size(124, 35);
            this.btnCastProtection.TabIndex = 26;
            this.btnCastProtection.Tag = "cast protection";
            this.btnCastProtection.Text = "Cast Protection";
            this.btnCastProtection.UseVisualStyleBackColor = true;
            this.btnCastProtection.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // chkIsNight
            // 
            this.chkIsNight.AutoSize = true;
            this.chkIsNight.Location = new System.Drawing.Point(352, 28);
            this.chkIsNight.Name = "chkIsNight";
            this.chkIsNight.Size = new System.Drawing.Size(77, 20);
            this.chkIsNight.TabIndex = 28;
            this.chkIsNight.Text = "Is night?";
            this.chkIsNight.UseVisualStyleBackColor = true;
            this.chkIsNight.CheckedChanged += new System.EventHandler(this.chkIsNight_CheckedChanged);
            // 
            // txtOneOffCommand
            // 
            this.txtOneOffCommand.Location = new System.Drawing.Point(168, 558);
            this.txtOneOffCommand.Name = "txtOneOffCommand";
            this.txtOneOffCommand.Size = new System.Drawing.Size(250, 22);
            this.txtOneOffCommand.TabIndex = 29;
            this.txtOneOffCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOneOffCommand_KeyPress);
            // 
            // lblOneOffCommand
            // 
            this.lblOneOffCommand.AutoSize = true;
            this.lblOneOffCommand.Location = new System.Drawing.Point(97, 560);
            this.lblOneOffCommand.Name = "lblOneOffCommand";
            this.lblOneOffCommand.Size = new System.Drawing.Size(52, 16);
            this.lblOneOffCommand.TabIndex = 30;
            this.lblOneOffCommand.Text = "One-Off";
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(436, 270);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(156, 35);
            this.btnInventory.TabIndex = 31;
            this.btnInventory.Tag = "inventory";
            this.btnInventory.Text = "Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnClearOneOff
            // 
            this.btnClearOneOff.Location = new System.Drawing.Point(424, 557);
            this.btnClearOneOff.Name = "btnClearOneOff";
            this.btnClearOneOff.Size = new System.Drawing.Size(85, 30);
            this.btnClearOneOff.TabIndex = 32;
            this.btnClearOneOff.Text = "Clear";
            this.btnClearOneOff.UseVisualStyleBackColor = true;
            this.btnClearOneOff.Click += new System.EventHandler(this.btnClearOneOff_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(436, 507);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(155, 26);
            this.btnAbort.TabIndex = 33;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnOneOffExecute
            // 
            this.btnOneOffExecute.Location = new System.Drawing.Point(515, 557);
            this.btnOneOffExecute.Name = "btnOneOffExecute";
            this.btnOneOffExecute.Size = new System.Drawing.Size(85, 30);
            this.btnOneOffExecute.TabIndex = 34;
            this.btnOneOffExecute.Text = "Execute";
            this.btnOneOffExecute.UseVisualStyleBackColor = true;
            this.btnOneOffExecute.Click += new System.EventHandler(this.btnOneOffExecute_Click);
            // 
            // btnAttackMob
            // 
            this.btnAttackMob.Location = new System.Drawing.Point(273, 187);
            this.btnAttackMob.Name = "btnAttackMob";
            this.btnAttackMob.Size = new System.Drawing.Size(156, 35);
            this.btnAttackMob.TabIndex = 35;
            this.btnAttackMob.Tag = "kill {0}";
            this.btnAttackMob.Text = "Attack Mob";
            this.btnAttackMob.UseVisualStyleBackColor = true;
            this.btnAttackMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // cboArea
            // 
            this.cboArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboArea.FormattingEnabled = true;
            this.cboArea.Location = new System.Drawing.Point(100, 439);
            this.cboArea.Name = "cboArea";
            this.cboArea.Size = new System.Drawing.Size(156, 24);
            this.cboArea.TabIndex = 36;
            this.cboArea.SelectedIndexChanged += new System.EventHandler(this.cboArea_SelectedIndexChanged);
            // 
            // txtCurrentRoom
            // 
            this.txtCurrentRoom.Enabled = false;
            this.txtCurrentRoom.Location = new System.Drawing.Point(262, 473);
            this.txtCurrentRoom.Name = "txtCurrentRoom";
            this.txtCurrentRoom.Size = new System.Drawing.Size(156, 22);
            this.txtCurrentRoom.TabIndex = 37;
            // 
            // lblCurrentRoom
            // 
            this.lblCurrentRoom.AutoSize = true;
            this.lblCurrentRoom.Location = new System.Drawing.Point(167, 476);
            this.lblCurrentRoom.Name = "lblCurrentRoom";
            this.lblCurrentRoom.Size = new System.Drawing.Size(86, 16);
            this.lblCurrentRoom.TabIndex = 38;
            this.lblCurrentRoom.Text = "Current room:";
            // 
            // btnDrinkYellow
            // 
            this.btnDrinkYellow.Location = new System.Drawing.Point(727, 146);
            this.btnDrinkYellow.Name = "btnDrinkYellow";
            this.btnDrinkYellow.Size = new System.Drawing.Size(124, 35);
            this.btnDrinkYellow.TabIndex = 39;
            this.btnDrinkYellow.Tag = "drink yellow";
            this.btnDrinkYellow.Text = "Yellow pot";
            this.btnDrinkYellow.UseVisualStyleBackColor = true;
            this.btnDrinkYellow.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(727, 187);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 35);
            this.button1.TabIndex = 40;
            this.button1.Tag = "drink green";
            this.button1.Text = "Green pot";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtWeapon
            // 
            this.txtWeapon.Location = new System.Drawing.Point(109, 82);
            this.txtWeapon.Name = "txtWeapon";
            this.txtWeapon.Size = new System.Drawing.Size(237, 22);
            this.txtWeapon.TabIndex = 42;
            // 
            // lblWeapon
            // 
            this.lblWeapon.Location = new System.Drawing.Point(35, 78);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(68, 33);
            this.lblWeapon.TabIndex = 41;
            this.lblWeapon.Text = "Weapon:";
            this.lblWeapon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnWieldWeapon
            // 
            this.btnWieldWeapon.Location = new System.Drawing.Point(273, 228);
            this.btnWieldWeapon.Name = "btnWieldWeapon";
            this.btnWieldWeapon.Size = new System.Drawing.Size(156, 35);
            this.btnWieldWeapon.TabIndex = 43;
            this.btnWieldWeapon.Tag = "wield {1}";
            this.btnWieldWeapon.Text = "Wield Weapon";
            this.btnWieldWeapon.UseVisualStyleBackColor = true;
            this.btnWieldWeapon.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastBless
            // 
            this.btnCastBless.Location = new System.Drawing.Point(597, 269);
            this.btnCastBless.Name = "btnCastBless";
            this.btnCastBless.Size = new System.Drawing.Size(124, 35);
            this.btnCastBless.TabIndex = 44;
            this.btnCastBless.Tag = "cast bless";
            this.btnCastBless.Text = "Cast Bless";
            this.btnCastBless.UseVisualStyleBackColor = true;
            this.btnCastBless.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(170, 599);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(73, 29);
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
            "noauto"});
            this.cboSetOption.Location = new System.Drawing.Point(254, 600);
            this.cboSetOption.Name = "cboSetOption";
            this.cboSetOption.Size = new System.Drawing.Size(164, 24);
            this.cboSetOption.TabIndex = 46;
            // 
            // chkSetOn
            // 
            this.chkSetOn.AutoSize = true;
            this.chkSetOn.Location = new System.Drawing.Point(424, 604);
            this.chkSetOn.Name = "chkSetOn";
            this.chkSetOn.Size = new System.Drawing.Size(53, 20);
            this.chkSetOn.TabIndex = 47;
            this.chkSetOn.Text = "On?";
            this.chkSetOn.UseVisualStyleBackColor = true;
            // 
            // txtWand
            // 
            this.txtWand.Location = new System.Drawing.Point(109, 110);
            this.txtWand.Name = "txtWand";
            this.txtWand.Size = new System.Drawing.Size(237, 22);
            this.txtWand.TabIndex = 49;
            // 
            // lblWand
            // 
            this.lblWand.Location = new System.Drawing.Point(35, 106);
            this.lblWand.Name = "lblWand";
            this.lblWand.Size = new System.Drawing.Size(68, 33);
            this.lblWand.TabIndex = 48;
            this.lblWand.Text = "Wand:";
            this.lblWand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnUseWandOnMob
            // 
            this.btnUseWandOnMob.Location = new System.Drawing.Point(273, 269);
            this.btnUseWandOnMob.Name = "btnUseWandOnMob";
            this.btnUseWandOnMob.Size = new System.Drawing.Size(156, 35);
            this.btnUseWandOnMob.TabIndex = 50;
            this.btnUseWandOnMob.Tag = "zap {2} {0}";
            this.btnUseWandOnMob.Text = "Wand Mob";
            this.btnUseWandOnMob.UseVisualStyleBackColor = true;
            this.btnUseWandOnMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnWho
            // 
            this.btnWho.Location = new System.Drawing.Point(436, 63);
            this.btnWho.Name = "btnWho";
            this.btnWho.Size = new System.Drawing.Size(156, 35);
            this.btnWho.TabIndex = 51;
            this.btnWho.Tag = "who";
            this.btnWho.Text = "Who";
            this.btnWho.UseVisualStyleBackColor = true;
            this.btnWho.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnUptime
            // 
            this.btnUptime.Location = new System.Drawing.Point(435, 22);
            this.btnUptime.Name = "btnUptime";
            this.btnUptime.Size = new System.Drawing.Size(156, 35);
            this.btnUptime.TabIndex = 52;
            this.btnUptime.Tag = "uptime";
            this.btnUptime.Text = "Uptime";
            this.btnUptime.UseVisualStyleBackColor = true;
            this.btnUptime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnEquipment
            // 
            this.btnEquipment.Location = new System.Drawing.Point(435, 311);
            this.btnEquipment.Name = "btnEquipment";
            this.btnEquipment.Size = new System.Drawing.Size(156, 35);
            this.btnEquipment.TabIndex = 53;
            this.btnEquipment.Tag = "equipment";
            this.btnEquipment.Text = "Equipment";
            this.btnEquipment.UseVisualStyleBackColor = true;
            this.btnEquipment.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 693);
            this.Controls.Add(this.btnEquipment);
            this.Controls.Add(this.btnUptime);
            this.Controls.Add(this.btnWho);
            this.Controls.Add(this.btnUseWandOnMob);
            this.Controls.Add(this.txtWand);
            this.Controls.Add(this.lblWand);
            this.Controls.Add(this.chkSetOn);
            this.Controls.Add(this.cboSetOption);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.btnCastBless);
            this.Controls.Add(this.btnWieldWeapon);
            this.Controls.Add(this.txtWeapon);
            this.Controls.Add(this.lblWeapon);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnDrinkYellow);
            this.Controls.Add(this.lblCurrentRoom);
            this.Controls.Add(this.txtCurrentRoom);
            this.Controls.Add(this.cboArea);
            this.Controls.Add(this.btnAttackMob);
            this.Controls.Add(this.btnOneOffExecute);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnClearOneOff);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.lblOneOffCommand);
            this.Controls.Add(this.txtOneOffCommand);
            this.Controls.Add(this.chkIsNight);
            this.Controls.Add(this.btnCastProtection);
            this.Controls.Add(this.btnGoToLocation);
            this.Controls.Add(this.btnSetCurrentLocation);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.cboRoom);
            this.Controls.Add(this.btnInformation);
            this.Controls.Add(this.btnScore);
            this.Controls.Add(this.btnTime);
            this.Controls.Add(this.btnCastCurePoison);
            this.Controls.Add(this.btnManashield);
            this.Controls.Add(this.btnCastVigor);
            this.Controls.Add(this.btnLook);
            this.Controls.Add(this.btnLookAtMob);
            this.Controls.Add(this.btnDrinkHazy);
            this.Controls.Add(this.btnFlee);
            this.Controls.Add(this.btnLevel2OffensiveSpell);
            this.Controls.Add(this.txtMob);
            this.Controls.Add(this.lblMob);
            this.Controls.Add(this.txtWindow);
            this.Controls.Add(this.lblWindow);
            this.Controls.Add(this.btnLevel1OffensiveSpell);
            this.Name = "frmMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLevel1OffensiveSpell;
        private System.Windows.Forms.Label lblWindow;
        private System.Windows.Forms.TextBox txtWindow;
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
        private System.Windows.Forms.ComboBox cboRoom;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Button btnSetCurrentLocation;
        private System.Windows.Forms.Button btnGoToLocation;
        private System.Windows.Forms.Button btnCastProtection;
        private System.Windows.Forms.CheckBox chkIsNight;
        private System.Windows.Forms.TextBox txtOneOffCommand;
        private System.Windows.Forms.Label lblOneOffCommand;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnClearOneOff;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnOneOffExecute;
        private System.Windows.Forms.Button btnAttackMob;
        private System.Windows.Forms.ComboBox cboArea;
        private System.Windows.Forms.TextBox txtCurrentRoom;
        private System.Windows.Forms.Label lblCurrentRoom;
        private System.Windows.Forms.Button btnDrinkYellow;
        private System.Windows.Forms.Button button1;
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
    }
}

