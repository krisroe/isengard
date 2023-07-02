﻿namespace IsengardClient
{
    partial class frmConfiguration
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
            this.ctxRealm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCurrentRealmEarth = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentRealmFire = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentRealmWater = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentRealmWind = new System.Windows.Forms.ToolStripMenuItem();
            this.lblWeapon = new System.Windows.Forms.Label();
            this.lblPreferredAlignment = new System.Windows.Forms.Label();
            this.ctxAutoEscape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetCurrentAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearCurrentAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoEscape1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCurrentAutoEscapeFlee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoEscape2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCurrentAutoEscapeActive = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeInactive = new System.Windows.Forms.ToolStripMenuItem();
            this.chkQueryMonsterStatus = new System.Windows.Forms.CheckBox();
            this.chkVerboseOutput = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPreferredAlignmentValue = new System.Windows.Forms.Label();
            this.ctxPreferredAlignment = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPreferredAlignmentGood = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreferredAlignmentEvil = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentAutoSpellLevelsValue = new System.Windows.Forms.Label();
            this.ctxAutoSpellLevels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetCurrentMinimumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetCurrentMaximumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentRealmValue = new System.Windows.Forms.Label();
            this.txtCurrentWeaponValue = new System.Windows.Forms.TextBox();
            this.lblCurrentAutoEscapeValue = new System.Windows.Forms.Label();
            this.lblEmptyColorValue = new System.Windows.Forms.Label();
            this.lblEmptyColor = new System.Windows.Forms.Label();
            this.lblFullColorValue = new System.Windows.Forms.Label();
            this.lblFullColor = new System.Windows.Forms.Label();
            this.lstStrategies = new System.Windows.Forms.ListBox();
            this.ctxStrategies = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddStrategy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditStrategy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveStrategy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveStrategyUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveStrategyDown = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRestoreDefaultStrategies = new System.Windows.Forms.ToolStripMenuItem();
            this.chkRemoveAllOnStartup = new System.Windows.Forms.CheckBox();
            this.tcConfiguration = new System.Windows.Forms.TabControl();
            this.tabConfiguration = new System.Windows.Forms.TabPage();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.chkDisplayStunLength = new System.Windows.Forms.CheckBox();
            this.btnSelectEmptyColor = new System.Windows.Forms.Button();
            this.btnSelectFullColor = new System.Windows.Forms.Button();
            this.tabStrategies = new System.Windows.Forms.TabPage();
            this.tabDynamicItemData = new System.Windows.Forms.TabPage();
            this.lvItems = new System.Windows.Forms.ListView();
            this.colItemName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colKeepCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTickCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOverflowAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlItemsTop = new System.Windows.Forms.Panel();
            this.btnTick = new System.Windows.Forms.Button();
            this.btnSellOrJunk = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnKeep = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.ctxRealm.SuspendLayout();
            this.ctxAutoEscape.SuspendLayout();
            this.ctxPreferredAlignment.SuspendLayout();
            this.ctxAutoSpellLevels.SuspendLayout();
            this.ctxStrategies.SuspendLayout();
            this.tcConfiguration.SuspendLayout();
            this.tabConfiguration.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            this.tabStrategies.SuspendLayout();
            this.tabDynamicItemData.SuspendLayout();
            this.pnlItemsTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxRealm
            // 
            this.ctxRealm.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxRealm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCurrentRealmEarth,
            this.tsmiCurrentRealmFire,
            this.tsmiCurrentRealmWater,
            this.tsmiCurrentRealmWind});
            this.ctxRealm.Name = "ctxRealm";
            this.ctxRealm.Size = new System.Drawing.Size(104, 92);
            this.ctxRealm.Opening += new System.ComponentModel.CancelEventHandler(this.ctxRealm_Opening);
            // 
            // tsmiCurrentRealmEarth
            // 
            this.tsmiCurrentRealmEarth.Name = "tsmiCurrentRealmEarth";
            this.tsmiCurrentRealmEarth.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmEarth.Text = "earth";
            this.tsmiCurrentRealmEarth.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmFire
            // 
            this.tsmiCurrentRealmFire.Name = "tsmiCurrentRealmFire";
            this.tsmiCurrentRealmFire.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmFire.Text = "fire";
            this.tsmiCurrentRealmFire.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmWater
            // 
            this.tsmiCurrentRealmWater.Name = "tsmiCurrentRealmWater";
            this.tsmiCurrentRealmWater.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmWater.Text = "water";
            this.tsmiCurrentRealmWater.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmWind
            // 
            this.tsmiCurrentRealmWind.Name = "tsmiCurrentRealmWind";
            this.tsmiCurrentRealmWind.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmWind.Text = "wind";
            this.tsmiCurrentRealmWind.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // lblWeapon
            // 
            this.lblWeapon.AutoSize = true;
            this.lblWeapon.Location = new System.Drawing.Point(13, 31);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(51, 13);
            this.lblWeapon.TabIndex = 126;
            this.lblWeapon.Text = "Weapon:";
            // 
            // lblPreferredAlignment
            // 
            this.lblPreferredAlignment.AutoSize = true;
            this.lblPreferredAlignment.Location = new System.Drawing.Point(11, 156);
            this.lblPreferredAlignment.Name = "lblPreferredAlignment";
            this.lblPreferredAlignment.Size = new System.Drawing.Size(101, 13);
            this.lblPreferredAlignment.TabIndex = 128;
            this.lblPreferredAlignment.Text = "Preferred alignment:";
            // 
            // ctxAutoEscape
            // 
            this.ctxAutoEscape.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoEscape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetCurrentAutoEscapeThreshold,
            this.tsmiClearCurrentAutoEscapeThreshold,
            this.sepAutoEscape1,
            this.tsmiCurrentAutoEscapeFlee,
            this.tsmiCurrentAutoEscapeHazy,
            this.sepAutoEscape2,
            this.tsmiCurrentAutoEscapeActive,
            this.tsmiCurrentAutoEscapeInactive});
            this.ctxAutoEscape.Name = "ctxAutoEscape";
            this.ctxAutoEscape.Size = new System.Drawing.Size(200, 148);
            this.ctxAutoEscape.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoEscape_Opening);
            // 
            // tsmiSetCurrentAutoEscapeThreshold
            // 
            this.tsmiSetCurrentAutoEscapeThreshold.Name = "tsmiSetCurrentAutoEscapeThreshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(199, 22);
            this.tsmiSetCurrentAutoEscapeThreshold.Text = "Set Current Threshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiSetCurrentAutoEscapeThreshold_Click);
            // 
            // tsmiClearCurrentAutoEscapeThreshold
            // 
            this.tsmiClearCurrentAutoEscapeThreshold.Name = "tsmiClearCurrentAutoEscapeThreshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(199, 22);
            this.tsmiClearCurrentAutoEscapeThreshold.Text = "Clear Current Threshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiClearCurrentAutoEscapeThreshold_Click);
            // 
            // sepAutoEscape1
            // 
            this.sepAutoEscape1.Name = "sepAutoEscape1";
            this.sepAutoEscape1.Size = new System.Drawing.Size(196, 6);
            // 
            // tsmiCurrentAutoEscapeFlee
            // 
            this.tsmiCurrentAutoEscapeFlee.Name = "tsmiCurrentAutoEscapeFlee";
            this.tsmiCurrentAutoEscapeFlee.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeFlee.Text = "Flee";
            this.tsmiCurrentAutoEscapeFlee.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeFlee_Click);
            // 
            // tsmiCurrentAutoEscapeHazy
            // 
            this.tsmiCurrentAutoEscapeHazy.Name = "tsmiCurrentAutoEscapeHazy";
            this.tsmiCurrentAutoEscapeHazy.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeHazy.Text = "Hazy";
            this.tsmiCurrentAutoEscapeHazy.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeHazy_Click);
            // 
            // sepAutoEscape2
            // 
            this.sepAutoEscape2.Name = "sepAutoEscape2";
            this.sepAutoEscape2.Size = new System.Drawing.Size(196, 6);
            // 
            // tsmiCurrentAutoEscapeActive
            // 
            this.tsmiCurrentAutoEscapeActive.Name = "tsmiCurrentAutoEscapeActive";
            this.tsmiCurrentAutoEscapeActive.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeActive.Text = "Active";
            this.tsmiCurrentAutoEscapeActive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeActive_Click);
            // 
            // tsmiCurrentAutoEscapeInactive
            // 
            this.tsmiCurrentAutoEscapeInactive.Name = "tsmiCurrentAutoEscapeInactive";
            this.tsmiCurrentAutoEscapeInactive.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeInactive.Text = "Inactive";
            this.tsmiCurrentAutoEscapeInactive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeInactive_Click);
            // 
            // chkQueryMonsterStatus
            // 
            this.chkQueryMonsterStatus.AutoSize = true;
            this.chkQueryMonsterStatus.Location = new System.Drawing.Point(114, 179);
            this.chkQueryMonsterStatus.Name = "chkQueryMonsterStatus";
            this.chkQueryMonsterStatus.Size = new System.Drawing.Size(131, 17);
            this.chkQueryMonsterStatus.TabIndex = 135;
            this.chkQueryMonsterStatus.Text = "Query monster status?";
            this.chkQueryMonsterStatus.UseVisualStyleBackColor = true;
            // 
            // chkVerboseOutput
            // 
            this.chkVerboseOutput.AutoSize = true;
            this.chkVerboseOutput.Location = new System.Drawing.Point(114, 202);
            this.chkVerboseOutput.Name = "chkVerboseOutput";
            this.chkVerboseOutput.Size = new System.Drawing.Size(104, 17);
            this.chkVerboseOutput.TabIndex = 136;
            this.chkVerboseOutput.Text = "Verbose output?";
            this.chkVerboseOutput.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(654, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(73, 22);
            this.btnOK.TabIndex = 137;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(733, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 22);
            this.btnCancel.TabIndex = 138;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblPreferredAlignmentValue
            // 
            this.lblPreferredAlignmentValue.BackColor = System.Drawing.Color.Blue;
            this.lblPreferredAlignmentValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblPreferredAlignmentValue.ForeColor = System.Drawing.Color.White;
            this.lblPreferredAlignmentValue.Location = new System.Drawing.Point(111, 154);
            this.lblPreferredAlignmentValue.Name = "lblPreferredAlignmentValue";
            this.lblPreferredAlignmentValue.Size = new System.Drawing.Size(166, 15);
            this.lblPreferredAlignmentValue.TabIndex = 139;
            this.lblPreferredAlignmentValue.Text = "Good";
            this.lblPreferredAlignmentValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxPreferredAlignment
            // 
            this.ctxPreferredAlignment.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxPreferredAlignment.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPreferredAlignmentGood,
            this.tsmiPreferredAlignmentEvil});
            this.ctxPreferredAlignment.Name = "ctxPreferredAlignment";
            this.ctxPreferredAlignment.Size = new System.Drawing.Size(104, 48);
            this.ctxPreferredAlignment.Opening += new System.ComponentModel.CancelEventHandler(this.ctxPreferredAlignment_Opening);
            // 
            // tsmiPreferredAlignmentGood
            // 
            this.tsmiPreferredAlignmentGood.Name = "tsmiPreferredAlignmentGood";
            this.tsmiPreferredAlignmentGood.Size = new System.Drawing.Size(103, 22);
            this.tsmiPreferredAlignmentGood.Text = "Good";
            this.tsmiPreferredAlignmentGood.Click += new System.EventHandler(this.tsmiPreferredAlignmentGood_Click);
            // 
            // tsmiPreferredAlignmentEvil
            // 
            this.tsmiPreferredAlignmentEvil.Name = "tsmiPreferredAlignmentEvil";
            this.tsmiPreferredAlignmentEvil.Size = new System.Drawing.Size(103, 22);
            this.tsmiPreferredAlignmentEvil.Text = "Evil";
            this.tsmiPreferredAlignmentEvil.Click += new System.EventHandler(this.tsmiPreferredAlignmentEvil_Click);
            // 
            // lblCurrentAutoSpellLevelsValue
            // 
            this.lblCurrentAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblCurrentAutoSpellLevelsValue.ContextMenuStrip = this.ctxAutoSpellLevels;
            this.lblCurrentAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentAutoSpellLevelsValue.Location = new System.Drawing.Point(111, 76);
            this.lblCurrentAutoSpellLevelsValue.Name = "lblCurrentAutoSpellLevelsValue";
            this.lblCurrentAutoSpellLevelsValue.Size = new System.Drawing.Size(166, 15);
            this.lblCurrentAutoSpellLevelsValue.TabIndex = 144;
            this.lblCurrentAutoSpellLevelsValue.Text = "Min:Max";
            this.lblCurrentAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetCurrentMinimumAutoSpellLevel,
            this.tsmiSetCurrentMaximumAutoSpellLevel});
            this.ctxAutoSpellLevels.Name = "ctxAutoSpellLevels";
            this.ctxAutoSpellLevels.Size = new System.Drawing.Size(192, 48);
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
            // lblCurrentRealmValue
            // 
            this.lblCurrentRealmValue.BackColor = System.Drawing.Color.White;
            this.lblCurrentRealmValue.ContextMenuStrip = this.ctxRealm;
            this.lblCurrentRealmValue.Location = new System.Drawing.Point(111, 8);
            this.lblCurrentRealmValue.Name = "lblCurrentRealmValue";
            this.lblCurrentRealmValue.Size = new System.Drawing.Size(166, 15);
            this.lblCurrentRealmValue.TabIndex = 141;
            this.lblCurrentRealmValue.Text = "Realm";
            this.lblCurrentRealmValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtCurrentWeaponValue
            // 
            this.txtCurrentWeaponValue.Location = new System.Drawing.Point(111, 28);
            this.txtCurrentWeaponValue.Name = "txtCurrentWeaponValue";
            this.txtCurrentWeaponValue.Size = new System.Drawing.Size(167, 20);
            this.txtCurrentWeaponValue.TabIndex = 142;
            // 
            // lblCurrentAutoEscapeValue
            // 
            this.lblCurrentAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblCurrentAutoEscapeValue.ContextMenuStrip = this.ctxAutoEscape;
            this.lblCurrentAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblCurrentAutoEscapeValue.Location = new System.Drawing.Point(111, 53);
            this.lblCurrentAutoEscapeValue.Name = "lblCurrentAutoEscapeValue";
            this.lblCurrentAutoEscapeValue.Size = new System.Drawing.Size(166, 15);
            this.lblCurrentAutoEscapeValue.TabIndex = 143;
            this.lblCurrentAutoEscapeValue.Text = "Auto Escape";
            this.lblCurrentAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColorValue
            // 
            this.lblEmptyColorValue.BackColor = System.Drawing.Color.Red;
            this.lblEmptyColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblEmptyColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblEmptyColorValue.Location = new System.Drawing.Point(111, 128);
            this.lblEmptyColorValue.Name = "lblEmptyColorValue";
            this.lblEmptyColorValue.Size = new System.Drawing.Size(166, 15);
            this.lblEmptyColorValue.TabIndex = 143;
            this.lblEmptyColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColor
            // 
            this.lblEmptyColor.AutoSize = true;
            this.lblEmptyColor.Location = new System.Drawing.Point(11, 128);
            this.lblEmptyColor.Name = "lblEmptyColor";
            this.lblEmptyColor.Size = new System.Drawing.Size(65, 13);
            this.lblEmptyColor.TabIndex = 142;
            this.lblEmptyColor.Text = "Empty color:";
            // 
            // lblFullColorValue
            // 
            this.lblFullColorValue.BackColor = System.Drawing.Color.Green;
            this.lblFullColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblFullColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblFullColorValue.Location = new System.Drawing.Point(111, 102);
            this.lblFullColorValue.Name = "lblFullColorValue";
            this.lblFullColorValue.Size = new System.Drawing.Size(166, 15);
            this.lblFullColorValue.TabIndex = 141;
            this.lblFullColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFullColor
            // 
            this.lblFullColor.AutoSize = true;
            this.lblFullColor.Location = new System.Drawing.Point(11, 104);
            this.lblFullColor.Name = "lblFullColor";
            this.lblFullColor.Size = new System.Drawing.Size(52, 13);
            this.lblFullColor.TabIndex = 140;
            this.lblFullColor.Text = "Full color:";
            // 
            // lstStrategies
            // 
            this.lstStrategies.ContextMenuStrip = this.ctxStrategies;
            this.lstStrategies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStrategies.FormattingEnabled = true;
            this.lstStrategies.Location = new System.Drawing.Point(2, 2);
            this.lstStrategies.Name = "lstStrategies";
            this.lstStrategies.Size = new System.Drawing.Size(812, 370);
            this.lstStrategies.TabIndex = 0;
            // 
            // ctxStrategies
            // 
            this.ctxStrategies.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxStrategies.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddStrategy,
            this.tsmiEditStrategy,
            this.tsmiRemoveStrategy,
            this.tsmiMoveStrategyUp,
            this.tsmiMoveStrategyDown,
            this.tsmiRestoreDefaultStrategies});
            this.ctxStrategies.Name = "ctxStrategies";
            this.ctxStrategies.Size = new System.Drawing.Size(160, 136);
            this.ctxStrategies.Opening += new System.ComponentModel.CancelEventHandler(this.ctxStrategies_Opening);
            // 
            // tsmiAddStrategy
            // 
            this.tsmiAddStrategy.Name = "tsmiAddStrategy";
            this.tsmiAddStrategy.Size = new System.Drawing.Size(159, 22);
            this.tsmiAddStrategy.Text = "Add";
            this.tsmiAddStrategy.Click += new System.EventHandler(this.tsmiAddStrategy_Click);
            // 
            // tsmiEditStrategy
            // 
            this.tsmiEditStrategy.Name = "tsmiEditStrategy";
            this.tsmiEditStrategy.Size = new System.Drawing.Size(159, 22);
            this.tsmiEditStrategy.Text = "Edit";
            this.tsmiEditStrategy.Click += new System.EventHandler(this.tsmiEditStrategy_Click);
            // 
            // tsmiRemoveStrategy
            // 
            this.tsmiRemoveStrategy.Name = "tsmiRemoveStrategy";
            this.tsmiRemoveStrategy.Size = new System.Drawing.Size(159, 22);
            this.tsmiRemoveStrategy.Text = "Remove";
            this.tsmiRemoveStrategy.Click += new System.EventHandler(this.tsmiRemoveStrategy_Click);
            // 
            // tsmiMoveStrategyUp
            // 
            this.tsmiMoveStrategyUp.Name = "tsmiMoveStrategyUp";
            this.tsmiMoveStrategyUp.Size = new System.Drawing.Size(159, 22);
            this.tsmiMoveStrategyUp.Text = "Move Up";
            this.tsmiMoveStrategyUp.Click += new System.EventHandler(this.tsmiMoveStrategyUp_Click);
            // 
            // tsmiMoveStrategyDown
            // 
            this.tsmiMoveStrategyDown.Name = "tsmiMoveStrategyDown";
            this.tsmiMoveStrategyDown.Size = new System.Drawing.Size(159, 22);
            this.tsmiMoveStrategyDown.Text = "Move Down";
            this.tsmiMoveStrategyDown.Click += new System.EventHandler(this.tsmiMoveStrategyDown_Click);
            // 
            // tsmiRestoreDefaultStrategies
            // 
            this.tsmiRestoreDefaultStrategies.Name = "tsmiRestoreDefaultStrategies";
            this.tsmiRestoreDefaultStrategies.Size = new System.Drawing.Size(159, 22);
            this.tsmiRestoreDefaultStrategies.Text = "Restore Defaults";
            this.tsmiRestoreDefaultStrategies.Click += new System.EventHandler(this.tsmiRestoreDefaultStrategies_Click);
            // 
            // chkRemoveAllOnStartup
            // 
            this.chkRemoveAllOnStartup.AutoSize = true;
            this.chkRemoveAllOnStartup.Location = new System.Drawing.Point(114, 224);
            this.chkRemoveAllOnStartup.Name = "chkRemoveAllOnStartup";
            this.chkRemoveAllOnStartup.Size = new System.Drawing.Size(135, 17);
            this.chkRemoveAllOnStartup.TabIndex = 146;
            this.chkRemoveAllOnStartup.Text = "Remove all on startup?";
            this.chkRemoveAllOnStartup.UseVisualStyleBackColor = true;
            // 
            // tcConfiguration
            // 
            this.tcConfiguration.Controls.Add(this.tabConfiguration);
            this.tcConfiguration.Controls.Add(this.tabStrategies);
            this.tcConfiguration.Controls.Add(this.tabDynamicItemData);
            this.tcConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcConfiguration.Location = new System.Drawing.Point(0, 0);
            this.tcConfiguration.Margin = new System.Windows.Forms.Padding(2);
            this.tcConfiguration.Name = "tcConfiguration";
            this.tcConfiguration.SelectedIndex = 0;
            this.tcConfiguration.Size = new System.Drawing.Size(824, 400);
            this.tcConfiguration.TabIndex = 147;
            // 
            // tabConfiguration
            // 
            this.tabConfiguration.Controls.Add(this.pnlSettings);
            this.tabConfiguration.Location = new System.Drawing.Point(4, 22);
            this.tabConfiguration.Margin = new System.Windows.Forms.Padding(2);
            this.tabConfiguration.Name = "tabConfiguration";
            this.tabConfiguration.Padding = new System.Windows.Forms.Padding(2);
            this.tabConfiguration.Size = new System.Drawing.Size(816, 374);
            this.tabConfiguration.TabIndex = 0;
            this.tabConfiguration.Text = "Settings";
            this.tabConfiguration.UseVisualStyleBackColor = true;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.chkDisplayStunLength);
            this.pnlSettings.Controls.Add(this.btnSelectEmptyColor);
            this.pnlSettings.Controls.Add(this.lblFullColorValue);
            this.pnlSettings.Controls.Add(this.btnSelectFullColor);
            this.pnlSettings.Controls.Add(this.txtCurrentWeaponValue);
            this.pnlSettings.Controls.Add(this.lblCurrentRealmValue);
            this.pnlSettings.Controls.Add(this.lblWeapon);
            this.pnlSettings.Controls.Add(this.chkVerboseOutput);
            this.pnlSettings.Controls.Add(this.lblCurrentAutoEscapeValue);
            this.pnlSettings.Controls.Add(this.chkQueryMonsterStatus);
            this.pnlSettings.Controls.Add(this.lblCurrentAutoSpellLevelsValue);
            this.pnlSettings.Controls.Add(this.lblFullColor);
            this.pnlSettings.Controls.Add(this.lblPreferredAlignment);
            this.pnlSettings.Controls.Add(this.lblEmptyColor);
            this.pnlSettings.Controls.Add(this.lblEmptyColorValue);
            this.pnlSettings.Controls.Add(this.lblPreferredAlignmentValue);
            this.pnlSettings.Controls.Add(this.chkRemoveAllOnStartup);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(2, 2);
            this.pnlSettings.Margin = new System.Windows.Forms.Padding(2);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(812, 370);
            this.pnlSettings.TabIndex = 150;
            // 
            // chkDisplayStunLength
            // 
            this.chkDisplayStunLength.AutoSize = true;
            this.chkDisplayStunLength.Location = new System.Drawing.Point(114, 247);
            this.chkDisplayStunLength.Name = "chkDisplayStunLength";
            this.chkDisplayStunLength.Size = new System.Drawing.Size(121, 17);
            this.chkDisplayStunLength.TabIndex = 149;
            this.chkDisplayStunLength.Text = "Display stun length?";
            this.chkDisplayStunLength.UseVisualStyleBackColor = true;
            // 
            // btnSelectEmptyColor
            // 
            this.btnSelectEmptyColor.Location = new System.Drawing.Point(283, 123);
            this.btnSelectEmptyColor.Name = "btnSelectEmptyColor";
            this.btnSelectEmptyColor.Size = new System.Drawing.Size(55, 23);
            this.btnSelectEmptyColor.TabIndex = 148;
            this.btnSelectEmptyColor.Text = "Select";
            this.btnSelectEmptyColor.UseVisualStyleBackColor = true;
            this.btnSelectEmptyColor.Click += new System.EventHandler(this.btnSelectEmptyColor_Click);
            // 
            // btnSelectFullColor
            // 
            this.btnSelectFullColor.Location = new System.Drawing.Point(283, 99);
            this.btnSelectFullColor.Name = "btnSelectFullColor";
            this.btnSelectFullColor.Size = new System.Drawing.Size(55, 23);
            this.btnSelectFullColor.TabIndex = 147;
            this.btnSelectFullColor.Text = "Select";
            this.btnSelectFullColor.UseVisualStyleBackColor = true;
            this.btnSelectFullColor.Click += new System.EventHandler(this.btnSelectFullColor_Click);
            // 
            // tabStrategies
            // 
            this.tabStrategies.Controls.Add(this.lstStrategies);
            this.tabStrategies.Location = new System.Drawing.Point(4, 22);
            this.tabStrategies.Margin = new System.Windows.Forms.Padding(2);
            this.tabStrategies.Name = "tabStrategies";
            this.tabStrategies.Padding = new System.Windows.Forms.Padding(2);
            this.tabStrategies.Size = new System.Drawing.Size(816, 374);
            this.tabStrategies.TabIndex = 1;
            this.tabStrategies.Text = "Strategies";
            this.tabStrategies.UseVisualStyleBackColor = true;
            // 
            // tabDynamicItemData
            // 
            this.tabDynamicItemData.Controls.Add(this.lvItems);
            this.tabDynamicItemData.Controls.Add(this.pnlItemsTop);
            this.tabDynamicItemData.Location = new System.Drawing.Point(4, 22);
            this.tabDynamicItemData.Margin = new System.Windows.Forms.Padding(2);
            this.tabDynamicItemData.Name = "tabDynamicItemData";
            this.tabDynamicItemData.Size = new System.Drawing.Size(816, 374);
            this.tabDynamicItemData.TabIndex = 2;
            this.tabDynamicItemData.Text = "Items";
            this.tabDynamicItemData.UseVisualStyleBackColor = true;
            // 
            // lvItems
            // 
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colItemName,
            this.colKeepCount,
            this.colTickCount,
            this.colOverflowAction});
            this.lvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvItems.FullRowSelect = true;
            this.lvItems.HideSelection = false;
            this.lvItems.Location = new System.Drawing.Point(0, 36);
            this.lvItems.Margin = new System.Windows.Forms.Padding(2);
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(816, 338);
            this.lvItems.TabIndex = 1;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            this.lvItems.SelectedIndexChanged += new System.EventHandler(this.lvItems_SelectedIndexChanged);
            // 
            // colItemName
            // 
            this.colItemName.Text = "Item";
            this.colItemName.Width = 280;
            // 
            // colKeepCount
            // 
            this.colKeepCount.Text = "Keep #";
            this.colKeepCount.Width = 77;
            // 
            // colTickCount
            // 
            this.colTickCount.Text = "Tick #";
            this.colTickCount.Width = 84;
            // 
            // colOverflowAction
            // 
            this.colOverflowAction.Text = "Overflow Action";
            this.colOverflowAction.Width = 97;
            // 
            // pnlItemsTop
            // 
            this.pnlItemsTop.Controls.Add(this.btnTick);
            this.pnlItemsTop.Controls.Add(this.btnSellOrJunk);
            this.pnlItemsTop.Controls.Add(this.btnClear);
            this.pnlItemsTop.Controls.Add(this.btnKeep);
            this.pnlItemsTop.Controls.Add(this.btnIgnore);
            this.pnlItemsTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlItemsTop.Location = new System.Drawing.Point(0, 0);
            this.pnlItemsTop.Margin = new System.Windows.Forms.Padding(2);
            this.pnlItemsTop.Name = "pnlItemsTop";
            this.pnlItemsTop.Size = new System.Drawing.Size(816, 36);
            this.pnlItemsTop.TabIndex = 0;
            // 
            // btnTick
            // 
            this.btnTick.Location = new System.Drawing.Point(67, 10);
            this.btnTick.Margin = new System.Windows.Forms.Padding(2);
            this.btnTick.Name = "btnTick";
            this.btnTick.Size = new System.Drawing.Size(56, 21);
            this.btnTick.TabIndex = 4;
            this.btnTick.Text = "Tick";
            this.btnTick.UseVisualStyleBackColor = true;
            this.btnTick.Click += new System.EventHandler(this.btnTick_Click);
            // 
            // btnSellOrJunk
            // 
            this.btnSellOrJunk.Location = new System.Drawing.Point(217, 10);
            this.btnSellOrJunk.Margin = new System.Windows.Forms.Padding(2);
            this.btnSellOrJunk.Name = "btnSellOrJunk";
            this.btnSellOrJunk.Size = new System.Drawing.Size(71, 21);
            this.btnSellOrJunk.TabIndex = 3;
            this.btnSellOrJunk.Text = "Sell/Junk";
            this.btnSellOrJunk.UseVisualStyleBackColor = true;
            this.btnSellOrJunk.Click += new System.EventHandler(this.btnSellOrJunk_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(157, 10);
            this.btnClear.Margin = new System.Windows.Forms.Padding(2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(56, 21);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnKeep
            // 
            this.btnKeep.Location = new System.Drawing.Point(7, 10);
            this.btnKeep.Margin = new System.Windows.Forms.Padding(2);
            this.btnKeep.Name = "btnKeep";
            this.btnKeep.Size = new System.Drawing.Size(56, 21);
            this.btnKeep.TabIndex = 1;
            this.btnKeep.Text = "Keep";
            this.btnKeep.UseVisualStyleBackColor = true;
            this.btnKeep.Click += new System.EventHandler(this.btnKeep_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(292, 10);
            this.btnIgnore.Margin = new System.Windows.Forms.Padding(2);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(56, 21);
            this.btnIgnore.TabIndex = 0;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 356);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(824, 44);
            this.pnlBottom.TabIndex = 148;
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 400);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.tcConfiguration);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.ctxRealm.ResumeLayout(false);
            this.ctxAutoEscape.ResumeLayout(false);
            this.ctxPreferredAlignment.ResumeLayout(false);
            this.ctxAutoSpellLevels.ResumeLayout(false);
            this.ctxStrategies.ResumeLayout(false);
            this.tcConfiguration.ResumeLayout(false);
            this.tabConfiguration.ResumeLayout(false);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.tabStrategies.ResumeLayout(false);
            this.tabDynamicItemData.ResumeLayout(false);
            this.pnlItemsTop.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblWeapon;
        private System.Windows.Forms.Label lblPreferredAlignment;
        private System.Windows.Forms.CheckBox chkQueryMonsterStatus;
        private System.Windows.Forms.CheckBox chkVerboseOutput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip ctxAutoEscape;
        private System.Windows.Forms.Label lblPreferredAlignmentValue;
        private System.Windows.Forms.ContextMenuStrip ctxPreferredAlignment;
        private System.Windows.Forms.Label lblFullColorValue;
        private System.Windows.Forms.Label lblFullColor;
        private System.Windows.Forms.Label lblEmptyColorValue;
        private System.Windows.Forms.Label lblEmptyColor;
        private System.Windows.Forms.ListBox lstStrategies;
        private System.Windows.Forms.ContextMenuStrip ctxStrategies;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveStrategyUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveStrategyDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreferredAlignmentGood;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreferredAlignmentEvil;
        private System.Windows.Forms.Label lblCurrentAutoSpellLevelsValue;
        private System.Windows.Forms.Label lblCurrentRealmValue;
        private System.Windows.Forms.TextBox txtCurrentWeaponValue;
        private System.Windows.Forms.Label lblCurrentAutoEscapeValue;
        private System.Windows.Forms.ContextMenuStrip ctxAutoSpellLevels;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMinimumAutoSpellLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMaximumAutoSpellLevel;
        private System.Windows.Forms.ContextMenuStrip ctxRealm;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmEarth;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmFire;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmWater;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmWind;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearCurrentAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeFlee;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeHazy;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeActive;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeInactive;
        private System.Windows.Forms.CheckBox chkRemoveAllOnStartup;
        private System.Windows.Forms.ToolStripSeparator sepAutoEscape1;
        private System.Windows.Forms.ToolStripSeparator sepAutoEscape2;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreDefaultStrategies;
        private System.Windows.Forms.TabControl tcConfiguration;
        private System.Windows.Forms.TabPage tabConfiguration;
        private System.Windows.Forms.Button btnSelectEmptyColor;
        private System.Windows.Forms.Button btnSelectFullColor;
        private System.Windows.Forms.TabPage tabStrategies;
        private System.Windows.Forms.TabPage tabDynamicItemData;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlItemsTop;
        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.ColumnHeader colItemName;
        private System.Windows.Forms.Button btnKeep;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSellOrJunk;
        private System.Windows.Forms.Button btnTick;
        private System.Windows.Forms.CheckBox chkDisplayStunLength;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.ColumnHeader colKeepCount;
        private System.Windows.Forms.ColumnHeader colTickCount;
        private System.Windows.Forms.ColumnHeader colOverflowAction;
    }
}