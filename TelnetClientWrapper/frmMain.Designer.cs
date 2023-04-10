﻿namespace IsengardClient
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
            this.btnPowerAttackMob = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.txtSetValue = new System.Windows.Forms.TextBox();
            this.btnRunMacro = new System.Windows.Forms.Button();
            this.cboMacros = new System.Windows.Forms.ComboBox();
            this.lblMacro = new System.Windows.Forms.Label();
            this.grpRealm = new System.Windows.Forms.GroupBox();
            this.radFire = new System.Windows.Forms.RadioButton();
            this.radWater = new System.Windows.Forms.RadioButton();
            this.radWind = new System.Windows.Forms.RadioButton();
            this.radEarth = new System.Windows.Forms.RadioButton();
            this.treeLocations = new System.Windows.Forms.TreeView();
            this.grpLocations = new System.Windows.Forms.GroupBox();
            this.grpOneClickMacros = new System.Windows.Forms.GroupBox();
            this.flpOneClickMacros = new System.Windows.Forms.FlowLayoutPanel();
            this.btnVariables = new System.Windows.Forms.Button();
            this.txtPotion = new System.Windows.Forms.TextBox();
            this.lblPotion = new System.Windows.Forms.Label();
            this.grpRealm.SuspendLayout();
            this.grpLocations.SuspendLayout();
            this.grpOneClickMacros.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLevel1OffensiveSpell
            // 
            this.btnLevel1OffensiveSpell.Location = new System.Drawing.Point(109, 170);
            this.btnLevel1OffensiveSpell.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLevel1OffensiveSpell.Name = "btnLevel1OffensiveSpell";
            this.btnLevel1OffensiveSpell.Size = new System.Drawing.Size(156, 34);
            this.btnLevel1OffensiveSpell.TabIndex = 0;
            this.btnLevel1OffensiveSpell.Tag = "cast {realm1spell} {mob}";
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
            this.txtWindow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWindow.Name = "txtWindow";
            this.txtWindow.Size = new System.Drawing.Size(237, 22);
            this.txtWindow.TabIndex = 2;
            this.txtWindow.Text = "Telnet isengard.nazgul.com";
            // 
            // txtMob
            // 
            this.txtMob.Location = new System.Drawing.Point(109, 54);
            this.txtMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMob.Name = "txtMob";
            this.txtMob.Size = new System.Drawing.Size(237, 22);
            this.txtMob.TabIndex = 4;
            // 
            // lblMob
            // 
            this.lblMob.Location = new System.Drawing.Point(35, 50);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(69, 33);
            this.lblMob.TabIndex = 3;
            this.lblMob.Text = "Mob:";
            this.lblMob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLevel2OffensiveSpell
            // 
            this.btnLevel2OffensiveSpell.Location = new System.Drawing.Point(109, 211);
            this.btnLevel2OffensiveSpell.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLevel2OffensiveSpell.Name = "btnLevel2OffensiveSpell";
            this.btnLevel2OffensiveSpell.Size = new System.Drawing.Size(156, 34);
            this.btnLevel2OffensiveSpell.TabIndex = 5;
            this.btnLevel2OffensiveSpell.Tag = "cast {realm2spell} {mob}";
            this.btnLevel2OffensiveSpell.Text = "Level 2 Offensive Spell";
            this.btnLevel2OffensiveSpell.UseVisualStyleBackColor = true;
            this.btnLevel2OffensiveSpell.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnFlee
            // 
            this.btnFlee.Location = new System.Drawing.Point(109, 294);
            this.btnFlee.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnFlee.Name = "btnFlee";
            this.btnFlee.Size = new System.Drawing.Size(156, 34);
            this.btnFlee.TabIndex = 6;
            this.btnFlee.Tag = "flee";
            this.btnFlee.Text = "Flee";
            this.btnFlee.UseVisualStyleBackColor = true;
            this.btnFlee.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnDrinkHazy
            // 
            this.btnDrinkHazy.Location = new System.Drawing.Point(109, 334);
            this.btnDrinkHazy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDrinkHazy.Name = "btnDrinkHazy";
            this.btnDrinkHazy.Size = new System.Drawing.Size(156, 34);
            this.btnDrinkHazy.TabIndex = 7;
            this.btnDrinkHazy.Tag = "drink hazy";
            this.btnDrinkHazy.Text = "Drink Hazy";
            this.btnDrinkHazy.UseVisualStyleBackColor = true;
            this.btnDrinkHazy.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLookAtMob
            // 
            this.btnLookAtMob.Location = new System.Drawing.Point(273, 294);
            this.btnLookAtMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLookAtMob.Name = "btnLookAtMob";
            this.btnLookAtMob.Size = new System.Drawing.Size(156, 34);
            this.btnLookAtMob.TabIndex = 8;
            this.btnLookAtMob.Tag = "look {mob}";
            this.btnLookAtMob.Text = "Look at Mob";
            this.btnLookAtMob.UseVisualStyleBackColor = true;
            this.btnLookAtMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnLook
            // 
            this.btnLook.Location = new System.Drawing.Point(434, 170);
            this.btnLook.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLook.Name = "btnLook";
            this.btnLook.Size = new System.Drawing.Size(156, 34);
            this.btnLook.TabIndex = 9;
            this.btnLook.Tag = "look";
            this.btnLook.Text = "Look";
            this.btnLook.UseVisualStyleBackColor = true;
            this.btnLook.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastVigor
            // 
            this.btnCastVigor.Location = new System.Drawing.Point(595, 170);
            this.btnCastVigor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCastVigor.Name = "btnCastVigor";
            this.btnCastVigor.Size = new System.Drawing.Size(124, 34);
            this.btnCastVigor.TabIndex = 10;
            this.btnCastVigor.Tag = "cast vigor";
            this.btnCastVigor.Text = "Cast Vigor";
            this.btnCastVigor.UseVisualStyleBackColor = true;
            this.btnCastVigor.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnManashield
            // 
            this.btnManashield.Location = new System.Drawing.Point(109, 252);
            this.btnManashield.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnManashield.Name = "btnManashield";
            this.btnManashield.Size = new System.Drawing.Size(156, 34);
            this.btnManashield.TabIndex = 11;
            this.btnManashield.Tag = "manashield";
            this.btnManashield.Text = "Manashield";
            this.btnManashield.UseVisualStyleBackColor = true;
            this.btnManashield.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastCurePoison
            // 
            this.btnCastCurePoison.Location = new System.Drawing.Point(595, 211);
            this.btnCastCurePoison.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCastCurePoison.Name = "btnCastCurePoison";
            this.btnCastCurePoison.Size = new System.Drawing.Size(124, 34);
            this.btnCastCurePoison.TabIndex = 18;
            this.btnCastCurePoison.Tag = "cast cure-poison";
            this.btnCastCurePoison.Text = "Cast Curepoison";
            this.btnCastCurePoison.UseVisualStyleBackColor = true;
            this.btnCastCurePoison.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnTime
            // 
            this.btnTime.Location = new System.Drawing.Point(435, 130);
            this.btnTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTime.Name = "btnTime";
            this.btnTime.Size = new System.Drawing.Size(156, 34);
            this.btnTime.TabIndex = 19;
            this.btnTime.Tag = "time";
            this.btnTime.Text = "Time";
            this.btnTime.UseVisualStyleBackColor = true;
            this.btnTime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnScore
            // 
            this.btnScore.Location = new System.Drawing.Point(434, 212);
            this.btnScore.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnScore.Name = "btnScore";
            this.btnScore.Size = new System.Drawing.Size(156, 34);
            this.btnScore.TabIndex = 20;
            this.btnScore.Tag = "score";
            this.btnScore.Text = "Score";
            this.btnScore.UseVisualStyleBackColor = true;
            this.btnScore.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnInformation
            // 
            this.btnInformation.Location = new System.Drawing.Point(434, 253);
            this.btnInformation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnInformation.Name = "btnInformation";
            this.btnInformation.Size = new System.Drawing.Size(156, 34);
            this.btnInformation.TabIndex = 21;
            this.btnInformation.Tag = "information";
            this.btnInformation.Text = "Information";
            this.btnInformation.UseVisualStyleBackColor = true;
            this.btnInformation.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnSetCurrentLocation
            // 
            this.btnSetCurrentLocation.Location = new System.Drawing.Point(681, 499);
            this.btnSetCurrentLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSetCurrentLocation.Name = "btnSetCurrentLocation";
            this.btnSetCurrentLocation.Size = new System.Drawing.Size(155, 26);
            this.btnSetCurrentLocation.TabIndex = 24;
            this.btnSetCurrentLocation.Text = "Set Current Location";
            this.btnSetCurrentLocation.UseVisualStyleBackColor = true;
            this.btnSetCurrentLocation.Click += new System.EventHandler(this.btnSetCurrentLocation_Click);
            // 
            // btnGoToLocation
            // 
            this.btnGoToLocation.Location = new System.Drawing.Point(681, 531);
            this.btnGoToLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGoToLocation.Name = "btnGoToLocation";
            this.btnGoToLocation.Size = new System.Drawing.Size(155, 26);
            this.btnGoToLocation.TabIndex = 25;
            this.btnGoToLocation.Text = "Go to Location";
            this.btnGoToLocation.UseVisualStyleBackColor = true;
            this.btnGoToLocation.Click += new System.EventHandler(this.btnGoToLocation_Click);
            // 
            // btnCastProtection
            // 
            this.btnCastProtection.Location = new System.Drawing.Point(595, 252);
            this.btnCastProtection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCastProtection.Name = "btnCastProtection";
            this.btnCastProtection.Size = new System.Drawing.Size(124, 34);
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
            this.chkIsNight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkIsNight.Name = "chkIsNight";
            this.chkIsNight.Size = new System.Drawing.Size(77, 20);
            this.chkIsNight.TabIndex = 28;
            this.chkIsNight.Text = "Is night?";
            this.chkIsNight.UseVisualStyleBackColor = true;
            this.chkIsNight.CheckedChanged += new System.EventHandler(this.chkIsNight_CheckedChanged);
            // 
            // txtOneOffCommand
            // 
            this.txtOneOffCommand.Location = new System.Drawing.Point(107, 452);
            this.txtOneOffCommand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtOneOffCommand.Name = "txtOneOffCommand";
            this.txtOneOffCommand.Size = new System.Drawing.Size(249, 22);
            this.txtOneOffCommand.TabIndex = 29;
            this.txtOneOffCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOneOffCommand_KeyPress);
            // 
            // lblOneOffCommand
            // 
            this.lblOneOffCommand.AutoSize = true;
            this.lblOneOffCommand.Location = new System.Drawing.Point(37, 455);
            this.lblOneOffCommand.Name = "lblOneOffCommand";
            this.lblOneOffCommand.Size = new System.Drawing.Size(52, 16);
            this.lblOneOffCommand.TabIndex = 30;
            this.lblOneOffCommand.Text = "One-Off";
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(434, 294);
            this.btnInventory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(156, 34);
            this.btnInventory.TabIndex = 31;
            this.btnInventory.Tag = "inventory";
            this.btnInventory.Text = "Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnClearOneOff
            // 
            this.btnClearOneOff.Location = new System.Drawing.Point(363, 452);
            this.btnClearOneOff.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClearOneOff.Name = "btnClearOneOff";
            this.btnClearOneOff.Size = new System.Drawing.Size(85, 30);
            this.btnClearOneOff.TabIndex = 32;
            this.btnClearOneOff.Text = "Clear";
            this.btnClearOneOff.UseVisualStyleBackColor = true;
            this.btnClearOneOff.Click += new System.EventHandler(this.btnClearOneOff_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(681, 566);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(155, 26);
            this.btnAbort.TabIndex = 33;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnOneOffExecute
            // 
            this.btnOneOffExecute.Location = new System.Drawing.Point(454, 452);
            this.btnOneOffExecute.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOneOffExecute.Name = "btnOneOffExecute";
            this.btnOneOffExecute.Size = new System.Drawing.Size(85, 30);
            this.btnOneOffExecute.TabIndex = 34;
            this.btnOneOffExecute.Text = "Execute";
            this.btnOneOffExecute.UseVisualStyleBackColor = true;
            this.btnOneOffExecute.Click += new System.EventHandler(this.btnOneOffExecute_Click);
            // 
            // btnAttackMob
            // 
            this.btnAttackMob.Location = new System.Drawing.Point(271, 170);
            this.btnAttackMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAttackMob.Name = "btnAttackMob";
            this.btnAttackMob.Size = new System.Drawing.Size(156, 34);
            this.btnAttackMob.TabIndex = 35;
            this.btnAttackMob.Tag = "kill {mob}";
            this.btnAttackMob.Text = "Attack Mob";
            this.btnAttackMob.UseVisualStyleBackColor = true;
            this.btnAttackMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtCurrentRoom
            // 
            this.txtCurrentRoom.Enabled = false;
            this.txtCurrentRoom.Location = new System.Drawing.Point(678, 470);
            this.txtCurrentRoom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCurrentRoom.Name = "txtCurrentRoom";
            this.txtCurrentRoom.Size = new System.Drawing.Size(156, 22);
            this.txtCurrentRoom.TabIndex = 37;
            // 
            // lblCurrentRoom
            // 
            this.lblCurrentRoom.AutoSize = true;
            this.lblCurrentRoom.Location = new System.Drawing.Point(583, 473);
            this.lblCurrentRoom.Name = "lblCurrentRoom";
            this.lblCurrentRoom.Size = new System.Drawing.Size(86, 16);
            this.lblCurrentRoom.TabIndex = 38;
            this.lblCurrentRoom.Text = "Current room:";
            // 
            // btnDrinkYellow
            // 
            this.btnDrinkYellow.Location = new System.Drawing.Point(725, 170);
            this.btnDrinkYellow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDrinkYellow.Name = "btnDrinkYellow";
            this.btnDrinkYellow.Size = new System.Drawing.Size(124, 34);
            this.btnDrinkYellow.TabIndex = 39;
            this.btnDrinkYellow.Tag = "drink yellow";
            this.btnDrinkYellow.Text = "Yellow pot";
            this.btnDrinkYellow.UseVisualStyleBackColor = true;
            this.btnDrinkYellow.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(725, 211);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 34);
            this.button1.TabIndex = 40;
            this.button1.Tag = "drink green";
            this.button1.Text = "Green pot";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtWeapon
            // 
            this.txtWeapon.Location = new System.Drawing.Point(109, 82);
            this.txtWeapon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
            this.btnWieldWeapon.Location = new System.Drawing.Point(273, 252);
            this.btnWieldWeapon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnWieldWeapon.Name = "btnWieldWeapon";
            this.btnWieldWeapon.Size = new System.Drawing.Size(156, 34);
            this.btnWieldWeapon.TabIndex = 43;
            this.btnWieldWeapon.Tag = "wield {weapon}";
            this.btnWieldWeapon.Text = "Wield Weapon";
            this.btnWieldWeapon.UseVisualStyleBackColor = true;
            this.btnWieldWeapon.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnCastBless
            // 
            this.btnCastBless.Location = new System.Drawing.Point(595, 294);
            this.btnCastBless.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCastBless.Name = "btnCastBless";
            this.btnCastBless.Size = new System.Drawing.Size(124, 34);
            this.btnCastBless.TabIndex = 44;
            this.btnCastBless.Tag = "cast bless";
            this.btnCastBless.Text = "Cast Bless";
            this.btnCastBless.UseVisualStyleBackColor = true;
            this.btnCastBless.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(110, 494);
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
            "noauto",
            "wimpy"});
            this.cboSetOption.Location = new System.Drawing.Point(193, 495);
            this.cboSetOption.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboSetOption.Name = "cboSetOption";
            this.cboSetOption.Size = new System.Drawing.Size(164, 24);
            this.cboSetOption.TabIndex = 46;
            this.cboSetOption.SelectedIndexChanged += new System.EventHandler(this.cboSetOption_SelectedIndexChanged);
            // 
            // chkSetOn
            // 
            this.chkSetOn.AutoSize = true;
            this.chkSetOn.Location = new System.Drawing.Point(363, 499);
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
            this.txtWand.Location = new System.Drawing.Point(109, 110);
            this.txtWand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWand.Name = "txtWand";
            this.txtWand.Size = new System.Drawing.Size(237, 22);
            this.txtWand.TabIndex = 49;
            // 
            // lblWand
            // 
            this.lblWand.Location = new System.Drawing.Point(35, 105);
            this.lblWand.Name = "lblWand";
            this.lblWand.Size = new System.Drawing.Size(68, 33);
            this.lblWand.TabIndex = 48;
            this.lblWand.Text = "Wand:";
            this.lblWand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnUseWandOnMob
            // 
            this.btnUseWandOnMob.Location = new System.Drawing.Point(271, 334);
            this.btnUseWandOnMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUseWandOnMob.Name = "btnUseWandOnMob";
            this.btnUseWandOnMob.Size = new System.Drawing.Size(156, 34);
            this.btnUseWandOnMob.TabIndex = 50;
            this.btnUseWandOnMob.Tag = "zap {wand} {mob}";
            this.btnUseWandOnMob.Text = "Wand Mob";
            this.btnUseWandOnMob.UseVisualStyleBackColor = true;
            this.btnUseWandOnMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnWho
            // 
            this.btnWho.Location = new System.Drawing.Point(435, 90);
            this.btnWho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnWho.Name = "btnWho";
            this.btnWho.Size = new System.Drawing.Size(156, 34);
            this.btnWho.TabIndex = 51;
            this.btnWho.Tag = "who";
            this.btnWho.Text = "Who";
            this.btnWho.UseVisualStyleBackColor = true;
            this.btnWho.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnUptime
            // 
            this.btnUptime.Location = new System.Drawing.Point(434, 49);
            this.btnUptime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUptime.Name = "btnUptime";
            this.btnUptime.Size = new System.Drawing.Size(156, 34);
            this.btnUptime.TabIndex = 52;
            this.btnUptime.Tag = "uptime";
            this.btnUptime.Text = "Uptime";
            this.btnUptime.UseVisualStyleBackColor = true;
            this.btnUptime.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnEquipment
            // 
            this.btnEquipment.Location = new System.Drawing.Point(433, 335);
            this.btnEquipment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEquipment.Name = "btnEquipment";
            this.btnEquipment.Size = new System.Drawing.Size(156, 34);
            this.btnEquipment.TabIndex = 53;
            this.btnEquipment.Tag = "equipment";
            this.btnEquipment.Text = "Equipment";
            this.btnEquipment.UseVisualStyleBackColor = true;
            this.btnEquipment.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnPowerAttackMob
            // 
            this.btnPowerAttackMob.Location = new System.Drawing.Point(273, 212);
            this.btnPowerAttackMob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPowerAttackMob.Name = "btnPowerAttackMob";
            this.btnPowerAttackMob.Size = new System.Drawing.Size(156, 34);
            this.btnPowerAttackMob.TabIndex = 54;
            this.btnPowerAttackMob.Tag = "power {mob}";
            this.btnPowerAttackMob.Text = "Power Attack Mob";
            this.btnPowerAttackMob.UseVisualStyleBackColor = true;
            this.btnPowerAttackMob.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(596, 49);
            this.btnQuit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(124, 34);
            this.btnQuit.TabIndex = 55;
            this.btnQuit.Tag = "quit";
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // txtSetValue
            // 
            this.txtSetValue.Location = new System.Drawing.Point(422, 497);
            this.txtSetValue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSetValue.Name = "txtSetValue";
            this.txtSetValue.Size = new System.Drawing.Size(117, 22);
            this.txtSetValue.TabIndex = 56;
            // 
            // btnRunMacro
            // 
            this.btnRunMacro.Enabled = false;
            this.btnRunMacro.Location = new System.Drawing.Point(549, 561);
            this.btnRunMacro.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRunMacro.Name = "btnRunMacro";
            this.btnRunMacro.Size = new System.Drawing.Size(115, 30);
            this.btnRunMacro.TabIndex = 59;
            this.btnRunMacro.Text = "Run Macro";
            this.btnRunMacro.UseVisualStyleBackColor = true;
            this.btnRunMacro.Click += new System.EventHandler(this.btnRunMacro_Click);
            // 
            // cboMacros
            // 
            this.cboMacros.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMacros.FormattingEnabled = true;
            this.cboMacros.Location = new System.Drawing.Point(110, 562);
            this.cboMacros.Margin = new System.Windows.Forms.Padding(4);
            this.cboMacros.Name = "cboMacros";
            this.cboMacros.Size = new System.Drawing.Size(431, 24);
            this.cboMacros.TabIndex = 60;
            this.cboMacros.SelectedIndexChanged += new System.EventHandler(this.cboMacros_SelectedIndexChanged);
            // 
            // lblMacro
            // 
            this.lblMacro.AutoSize = true;
            this.lblMacro.Location = new System.Drawing.Point(19, 566);
            this.lblMacro.Name = "lblMacro";
            this.lblMacro.Size = new System.Drawing.Size(48, 16);
            this.lblMacro.TabIndex = 61;
            this.lblMacro.Text = "Macro:";
            // 
            // grpRealm
            // 
            this.grpRealm.Controls.Add(this.radFire);
            this.grpRealm.Controls.Add(this.radWater);
            this.grpRealm.Controls.Add(this.radWind);
            this.grpRealm.Controls.Add(this.radEarth);
            this.grpRealm.Location = new System.Drawing.Point(109, 386);
            this.grpRealm.Margin = new System.Windows.Forms.Padding(4);
            this.grpRealm.Name = "grpRealm";
            this.grpRealm.Padding = new System.Windows.Forms.Padding(4);
            this.grpRealm.Size = new System.Drawing.Size(289, 57);
            this.grpRealm.TabIndex = 62;
            this.grpRealm.TabStop = false;
            this.grpRealm.Text = "Realm";
            // 
            // radFire
            // 
            this.radFire.AutoSize = true;
            this.radFire.Location = new System.Drawing.Point(231, 23);
            this.radFire.Margin = new System.Windows.Forms.Padding(4);
            this.radFire.Name = "radFire";
            this.radFire.Size = new System.Drawing.Size(51, 20);
            this.radFire.TabIndex = 3;
            this.radFire.TabStop = true;
            this.radFire.Text = "Fire";
            this.radFire.UseVisualStyleBackColor = true;
            // 
            // radWater
            // 
            this.radWater.AutoSize = true;
            this.radWater.Location = new System.Drawing.Point(151, 23);
            this.radWater.Margin = new System.Windows.Forms.Padding(4);
            this.radWater.Name = "radWater";
            this.radWater.Size = new System.Drawing.Size(64, 20);
            this.radWater.TabIndex = 2;
            this.radWater.TabStop = true;
            this.radWater.Text = "Water";
            this.radWater.UseVisualStyleBackColor = true;
            // 
            // radWind
            // 
            this.radWind.AutoSize = true;
            this.radWind.Checked = true;
            this.radWind.Location = new System.Drawing.Point(83, 23);
            this.radWind.Margin = new System.Windows.Forms.Padding(4);
            this.radWind.Name = "radWind";
            this.radWind.Size = new System.Drawing.Size(59, 20);
            this.radWind.TabIndex = 1;
            this.radWind.TabStop = true;
            this.radWind.Text = "Wind";
            this.radWind.UseVisualStyleBackColor = true;
            // 
            // radEarth
            // 
            this.radEarth.AutoSize = true;
            this.radEarth.Location = new System.Drawing.Point(8, 23);
            this.radEarth.Margin = new System.Windows.Forms.Padding(4);
            this.radEarth.Name = "radEarth";
            this.radEarth.Size = new System.Drawing.Size(59, 20);
            this.radEarth.TabIndex = 0;
            this.radEarth.TabStop = true;
            this.radEarth.Text = "Earth";
            this.radEarth.UseVisualStyleBackColor = true;
            // 
            // treeLocations
            // 
            this.treeLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeLocations.Location = new System.Drawing.Point(4, 19);
            this.treeLocations.Margin = new System.Windows.Forms.Padding(4);
            this.treeLocations.Name = "treeLocations";
            this.treeLocations.Size = new System.Drawing.Size(327, 580);
            this.treeLocations.TabIndex = 63;
            // 
            // grpLocations
            // 
            this.grpLocations.Controls.Add(this.treeLocations);
            this.grpLocations.Location = new System.Drawing.Point(862, 22);
            this.grpLocations.Margin = new System.Windows.Forms.Padding(4);
            this.grpLocations.Name = "grpLocations";
            this.grpLocations.Padding = new System.Windows.Forms.Padding(4);
            this.grpLocations.Size = new System.Drawing.Size(335, 603);
            this.grpLocations.TabIndex = 64;
            this.grpLocations.TabStop = false;
            this.grpLocations.Text = "Locations";
            // 
            // grpOneClickMacros
            // 
            this.grpOneClickMacros.Controls.Add(this.flpOneClickMacros);
            this.grpOneClickMacros.Location = new System.Drawing.Point(18, 629);
            this.grpOneClickMacros.Margin = new System.Windows.Forms.Padding(4);
            this.grpOneClickMacros.Name = "grpOneClickMacros";
            this.grpOneClickMacros.Padding = new System.Windows.Forms.Padding(4);
            this.grpOneClickMacros.Size = new System.Drawing.Size(1175, 80);
            this.grpOneClickMacros.TabIndex = 65;
            this.grpOneClickMacros.TabStop = false;
            this.grpOneClickMacros.Text = "One Click Macros";
            // 
            // flpOneClickMacros
            // 
            this.flpOneClickMacros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpOneClickMacros.Location = new System.Drawing.Point(4, 19);
            this.flpOneClickMacros.Margin = new System.Windows.Forms.Padding(4);
            this.flpOneClickMacros.Name = "flpOneClickMacros";
            this.flpOneClickMacros.Size = new System.Drawing.Size(1167, 57);
            this.flpOneClickMacros.TabIndex = 0;
            // 
            // btnVariables
            // 
            this.btnVariables.Location = new System.Drawing.Point(726, 51);
            this.btnVariables.Name = "btnVariables";
            this.btnVariables.Size = new System.Drawing.Size(123, 32);
            this.btnVariables.TabIndex = 66;
            this.btnVariables.Text = "Variables";
            this.btnVariables.UseVisualStyleBackColor = true;
            this.btnVariables.Click += new System.EventHandler(this.btnVariables_Click);
            // 
            // txtPotion
            // 
            this.txtPotion.Location = new System.Drawing.Point(109, 138);
            this.txtPotion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPotion.Name = "txtPotion";
            this.txtPotion.Size = new System.Drawing.Size(237, 22);
            this.txtPotion.TabIndex = 68;
            // 
            // lblPotion
            // 
            this.lblPotion.Location = new System.Drawing.Point(35, 131);
            this.lblPotion.Name = "lblPotion";
            this.lblPotion.Size = new System.Drawing.Size(68, 33);
            this.lblPotion.TabIndex = 67;
            this.lblPotion.Text = "Potion:";
            this.lblPotion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1210, 728);
            this.Controls.Add(this.txtPotion);
            this.Controls.Add(this.lblPotion);
            this.Controls.Add(this.btnVariables);
            this.Controls.Add(this.grpOneClickMacros);
            this.Controls.Add(this.grpLocations);
            this.Controls.Add(this.grpRealm);
            this.Controls.Add(this.lblMacro);
            this.Controls.Add(this.cboMacros);
            this.Controls.Add(this.btnRunMacro);
            this.Controls.Add(this.txtSetValue);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPowerAttackMob);
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
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.grpRealm.ResumeLayout(false);
            this.grpRealm.PerformLayout();
            this.grpLocations.ResumeLayout(false);
            this.grpOneClickMacros.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnPowerAttackMob;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.TextBox txtSetValue;
        private System.Windows.Forms.Button btnRunMacro;
        private System.Windows.Forms.ComboBox cboMacros;
        private System.Windows.Forms.Label lblMacro;
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
    }
}

