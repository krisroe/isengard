namespace IsengardClient
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
            this.sepRealm1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRestoreCurrentRealm = new System.Windows.Forms.ToolStripMenuItem();
            this.lblWeapon = new System.Windows.Forms.Label();
            this.ctxWeapon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiRestoreCurrentWeapon = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPreferredAlignment = new System.Windows.Forms.Label();
            this.ctxAutoEscape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetCurrentAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearCurrentAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeFlee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeActive = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeInactive = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoEscape1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRestoreCurrentAutoEscape = new System.Windows.Forms.ToolStripMenuItem();
            this.chkQueryMonsterStatus = new System.Windows.Forms.CheckBox();
            this.chkVerboseOutput = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPreferredAlignmentValue = new System.Windows.Forms.Label();
            this.ctxPreferredAlignment = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPreferredAlignmentGood = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreferredAlignmentEvil = new System.Windows.Forms.ToolStripMenuItem();
            this.sepPreferredAlignment1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiPreferredAlignmentRestoreOriginalValue = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentAutoSpellLevelsValue = new System.Windows.Forms.Label();
            this.ctxAutoSpellLevels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetCurrentMinimumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetCurrentMaximumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoSpellLevels1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRestoreCurrentAutoSpellLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentRealmValue = new System.Windows.Forms.Label();
            this.txtCurrentWeaponValue = new System.Windows.Forms.TextBox();
            this.lblCurrentAutoEscapeValue = new System.Windows.Forms.Label();
            this.btnSelectEmptyColor = new System.Windows.Forms.Button();
            this.btnSelectFullColor = new System.Windows.Forms.Button();
            this.lblEmptyColorValue = new System.Windows.Forms.Label();
            this.lblEmptyColor = new System.Windows.Forms.Label();
            this.lblFullColorValue = new System.Windows.Forms.Label();
            this.lblFullColor = new System.Windows.Forms.Label();
            this.grpStrategies = new System.Windows.Forms.GroupBox();
            this.lstStrategies = new System.Windows.Forms.ListBox();
            this.ctxStrategies = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddStrategy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditStrategy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveStrategy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveStrategyUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveStrategyDown = new System.Windows.Forms.ToolStripMenuItem();
            this.chkRemoveAllOnStartup = new System.Windows.Forms.CheckBox();
            this.ctxRealm.SuspendLayout();
            this.ctxWeapon.SuspendLayout();
            this.ctxAutoEscape.SuspendLayout();
            this.ctxPreferredAlignment.SuspendLayout();
            this.ctxAutoSpellLevels.SuspendLayout();
            this.grpStrategies.SuspendLayout();
            this.ctxStrategies.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxRealm
            // 
            this.ctxRealm.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxRealm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCurrentRealmEarth,
            this.tsmiCurrentRealmFire,
            this.tsmiCurrentRealmWater,
            this.tsmiCurrentRealmWind,
            this.sepRealm1,
            this.tsmiRestoreCurrentRealm});
            this.ctxRealm.Name = "ctxRealm";
            this.ctxRealm.Size = new System.Drawing.Size(181, 130);
            this.ctxRealm.Opening += new System.ComponentModel.CancelEventHandler(this.ctxRealm_Opening);
            // 
            // tsmiCurrentRealmEarth
            // 
            this.tsmiCurrentRealmEarth.Name = "tsmiCurrentRealmEarth";
            this.tsmiCurrentRealmEarth.Size = new System.Drawing.Size(222, 24);
            this.tsmiCurrentRealmEarth.Text = "earth";
            this.tsmiCurrentRealmEarth.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmFire
            // 
            this.tsmiCurrentRealmFire.Name = "tsmiCurrentRealmFire";
            this.tsmiCurrentRealmFire.Size = new System.Drawing.Size(222, 24);
            this.tsmiCurrentRealmFire.Text = "fire";
            this.tsmiCurrentRealmFire.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmWater
            // 
            this.tsmiCurrentRealmWater.Name = "tsmiCurrentRealmWater";
            this.tsmiCurrentRealmWater.Size = new System.Drawing.Size(222, 24);
            this.tsmiCurrentRealmWater.Text = "water";
            this.tsmiCurrentRealmWater.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmWind
            // 
            this.tsmiCurrentRealmWind.Name = "tsmiCurrentRealmWind";
            this.tsmiCurrentRealmWind.Size = new System.Drawing.Size(222, 24);
            this.tsmiCurrentRealmWind.Text = "wind";
            this.tsmiCurrentRealmWind.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // sepRealm1
            // 
            this.sepRealm1.Name = "sepRealm1";
            this.sepRealm1.Size = new System.Drawing.Size(219, 6);
            // 
            // tsmiRestoreCurrentRealm
            // 
            this.tsmiRestoreCurrentRealm.Name = "tsmiRestoreCurrentRealm";
            this.tsmiRestoreCurrentRealm.Size = new System.Drawing.Size(222, 24);
            this.tsmiRestoreCurrentRealm.Text = "Restore Current";
            this.tsmiRestoreCurrentRealm.Click += new System.EventHandler(this.tsmiRestoreCurrentRealm_Click);
            // 
            // lblWeapon
            // 
            this.lblWeapon.AutoSize = true;
            this.lblWeapon.ContextMenuStrip = this.ctxWeapon;
            this.lblWeapon.Location = new System.Drawing.Point(14, 58);
            this.lblWeapon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(62, 16);
            this.lblWeapon.TabIndex = 126;
            this.lblWeapon.Text = "Weapon:";
            // 
            // ctxWeapon
            // 
            this.ctxWeapon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxWeapon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRestoreCurrentWeapon});
            this.ctxWeapon.Name = "ctxWeapon";
            this.ctxWeapon.Size = new System.Drawing.Size(211, 56);
            this.ctxWeapon.Opening += new System.ComponentModel.CancelEventHandler(this.ctxWeapon_Opening);
            // 
            // tsmiRestoreCurrentWeapon
            // 
            this.tsmiRestoreCurrentWeapon.Name = "tsmiRestoreCurrentWeapon";
            this.tsmiRestoreCurrentWeapon.Size = new System.Drawing.Size(222, 24);
            this.tsmiRestoreCurrentWeapon.Text = "Restore Current";
            this.tsmiRestoreCurrentWeapon.Click += new System.EventHandler(this.tsmiRestoreCurrentWeapon_Click);
            // 
            // lblPreferredAlignment
            // 
            this.lblPreferredAlignment.AutoSize = true;
            this.lblPreferredAlignment.Location = new System.Drawing.Point(12, 212);
            this.lblPreferredAlignment.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPreferredAlignment.Name = "lblPreferredAlignment";
            this.lblPreferredAlignment.Size = new System.Drawing.Size(127, 16);
            this.lblPreferredAlignment.TabIndex = 128;
            this.lblPreferredAlignment.Text = "Preferred alignment:";
            // 
            // ctxAutoEscape
            // 
            this.ctxAutoEscape.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoEscape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetCurrentAutoEscapeThreshold,
            this.tsmiClearCurrentAutoEscapeThreshold,
            this.tsmiCurrentAutoEscapeFlee,
            this.tsmiCurrentAutoEscapeHazy,
            this.tsmiCurrentAutoEscapeActive,
            this.tsmiCurrentAutoEscapeInactive,
            this.sepAutoEscape1,
            this.tsmiRestoreCurrentAutoEscape});
            this.ctxAutoEscape.Name = "ctxAutoEscape";
            this.ctxAutoEscape.Size = new System.Drawing.Size(234, 178);
            this.ctxAutoEscape.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoEscape_Opening);
            // 
            // tsmiSetCurrentAutoEscapeThreshold
            // 
            this.tsmiSetCurrentAutoEscapeThreshold.Name = "tsmiSetCurrentAutoEscapeThreshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(233, 24);
            this.tsmiSetCurrentAutoEscapeThreshold.Text = "Set Current Threshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiSetCurrentAutoEscapeThreshold_Click);
            // 
            // tsmiClearCurrentAutoEscapeThreshold
            // 
            this.tsmiClearCurrentAutoEscapeThreshold.Name = "tsmiClearCurrentAutoEscapeThreshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(233, 24);
            this.tsmiClearCurrentAutoEscapeThreshold.Text = "Clear Current Threshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiClearCurrentAutoEscapeThreshold_Click);
            // 
            // tsmiCurrentAutoEscapeFlee
            // 
            this.tsmiCurrentAutoEscapeFlee.Name = "tsmiCurrentAutoEscapeFlee";
            this.tsmiCurrentAutoEscapeFlee.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeFlee.Text = "Flee";
            this.tsmiCurrentAutoEscapeFlee.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeFlee_Click);
            // 
            // tsmiCurrentAutoEscapeHazy
            // 
            this.tsmiCurrentAutoEscapeHazy.Name = "tsmiCurrentAutoEscapeHazy";
            this.tsmiCurrentAutoEscapeHazy.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeHazy.Text = "Hazy";
            this.tsmiCurrentAutoEscapeHazy.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeHazy_Click);
            // 
            // tsmiCurrentAutoEscapeActive
            // 
            this.tsmiCurrentAutoEscapeActive.Name = "tsmiCurrentAutoEscapeActive";
            this.tsmiCurrentAutoEscapeActive.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeActive.Text = "Active";
            this.tsmiCurrentAutoEscapeActive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeActive_Click);
            // 
            // tsmiCurrentAutoEscapeInactive
            // 
            this.tsmiCurrentAutoEscapeInactive.Name = "tsmiCurrentAutoEscapeInactive";
            this.tsmiCurrentAutoEscapeInactive.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeInactive.Text = "Inactive";
            this.tsmiCurrentAutoEscapeInactive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeInactive_Click);
            // 
            // sepAutoEscape1
            // 
            this.sepAutoEscape1.Name = "sepAutoEscape1";
            this.sepAutoEscape1.Size = new System.Drawing.Size(230, 6);
            // 
            // tsmiRestoreCurrentAutoEscape
            // 
            this.tsmiRestoreCurrentAutoEscape.Name = "tsmiRestoreCurrentAutoEscape";
            this.tsmiRestoreCurrentAutoEscape.Size = new System.Drawing.Size(233, 24);
            this.tsmiRestoreCurrentAutoEscape.Text = "Restore Current";
            this.tsmiRestoreCurrentAutoEscape.Click += new System.EventHandler(this.tsmiRestoreCurrentAutoEscape_Click);
            // 
            // chkQueryMonsterStatus
            // 
            this.chkQueryMonsterStatus.AutoSize = true;
            this.chkQueryMonsterStatus.Location = new System.Drawing.Point(149, 240);
            this.chkQueryMonsterStatus.Margin = new System.Windows.Forms.Padding(4);
            this.chkQueryMonsterStatus.Name = "chkQueryMonsterStatus";
            this.chkQueryMonsterStatus.Size = new System.Drawing.Size(161, 20);
            this.chkQueryMonsterStatus.TabIndex = 135;
            this.chkQueryMonsterStatus.Text = "Query monster status?";
            this.chkQueryMonsterStatus.UseVisualStyleBackColor = true;
            // 
            // chkVerboseOutput
            // 
            this.chkVerboseOutput.AutoSize = true;
            this.chkVerboseOutput.Location = new System.Drawing.Point(149, 268);
            this.chkVerboseOutput.Margin = new System.Windows.Forms.Padding(4);
            this.chkVerboseOutput.Name = "chkVerboseOutput";
            this.chkVerboseOutput.Size = new System.Drawing.Size(127, 20);
            this.chkVerboseOutput.TabIndex = 136;
            this.chkVerboseOutput.Text = "Verbose output?";
            this.chkVerboseOutput.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(700, 368);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(97, 27);
            this.btnOK.TabIndex = 137;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(805, 368);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 27);
            this.btnCancel.TabIndex = 138;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblPreferredAlignmentValue
            // 
            this.lblPreferredAlignmentValue.BackColor = System.Drawing.Color.Blue;
            this.lblPreferredAlignmentValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblPreferredAlignmentValue.ForeColor = System.Drawing.Color.White;
            this.lblPreferredAlignmentValue.Location = new System.Drawing.Point(145, 210);
            this.lblPreferredAlignmentValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPreferredAlignmentValue.Name = "lblPreferredAlignmentValue";
            this.lblPreferredAlignmentValue.Size = new System.Drawing.Size(221, 18);
            this.lblPreferredAlignmentValue.TabIndex = 139;
            this.lblPreferredAlignmentValue.Text = "Good";
            this.lblPreferredAlignmentValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxPreferredAlignment
            // 
            this.ctxPreferredAlignment.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxPreferredAlignment.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPreferredAlignmentGood,
            this.tsmiPreferredAlignmentEvil,
            this.sepPreferredAlignment1,
            this.tsmiPreferredAlignmentRestoreOriginalValue});
            this.ctxPreferredAlignment.Name = "ctxPreferredAlignment";
            this.ctxPreferredAlignment.Size = new System.Drawing.Size(226, 82);
            this.ctxPreferredAlignment.Opening += new System.ComponentModel.CancelEventHandler(this.ctxPreferredAlignment_Opening);
            // 
            // tsmiPreferredAlignmentGood
            // 
            this.tsmiPreferredAlignmentGood.Name = "tsmiPreferredAlignmentGood";
            this.tsmiPreferredAlignmentGood.Size = new System.Drawing.Size(225, 24);
            this.tsmiPreferredAlignmentGood.Text = "Good";
            this.tsmiPreferredAlignmentGood.Click += new System.EventHandler(this.tsmiPreferredAlignmentGood_Click);
            // 
            // tsmiPreferredAlignmentEvil
            // 
            this.tsmiPreferredAlignmentEvil.Name = "tsmiPreferredAlignmentEvil";
            this.tsmiPreferredAlignmentEvil.Size = new System.Drawing.Size(225, 24);
            this.tsmiPreferredAlignmentEvil.Text = "Evil";
            this.tsmiPreferredAlignmentEvil.Click += new System.EventHandler(this.tsmiPreferredAlignmentEvil_Click);
            // 
            // sepPreferredAlignment1
            // 
            this.sepPreferredAlignment1.Name = "sepPreferredAlignment1";
            this.sepPreferredAlignment1.Size = new System.Drawing.Size(222, 6);
            // 
            // tsmiPreferredAlignmentRestoreOriginalValue
            // 
            this.tsmiPreferredAlignmentRestoreOriginalValue.Name = "tsmiPreferredAlignmentRestoreOriginalValue";
            this.tsmiPreferredAlignmentRestoreOriginalValue.Size = new System.Drawing.Size(225, 24);
            this.tsmiPreferredAlignmentRestoreOriginalValue.Text = "Restore Original Value";
            this.tsmiPreferredAlignmentRestoreOriginalValue.Click += new System.EventHandler(this.tsmiPreferredAlignmentRestoreOriginalValue_Click);
            // 
            // lblCurrentAutoSpellLevelsValue
            // 
            this.lblCurrentAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblCurrentAutoSpellLevelsValue.ContextMenuStrip = this.ctxAutoSpellLevels;
            this.lblCurrentAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentAutoSpellLevelsValue.Location = new System.Drawing.Point(145, 114);
            this.lblCurrentAutoSpellLevelsValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentAutoSpellLevelsValue.Name = "lblCurrentAutoSpellLevelsValue";
            this.lblCurrentAutoSpellLevelsValue.Size = new System.Drawing.Size(221, 18);
            this.lblCurrentAutoSpellLevelsValue.TabIndex = 144;
            this.lblCurrentAutoSpellLevelsValue.Text = "Min:Max";
            this.lblCurrentAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetCurrentMinimumAutoSpellLevel,
            this.tsmiSetCurrentMaximumAutoSpellLevel,
            this.sepAutoSpellLevels1,
            this.tsmiRestoreCurrentAutoSpellLevels});
            this.ctxAutoSpellLevels.Name = "ctxAutoSpellLevels";
            this.ctxAutoSpellLevels.Size = new System.Drawing.Size(222, 82);
            // 
            // tsmiSetCurrentMinimumAutoSpellLevel
            // 
            this.tsmiSetCurrentMinimumAutoSpellLevel.Name = "tsmiSetCurrentMinimumAutoSpellLevel";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Size = new System.Drawing.Size(222, 24);
            this.tsmiSetCurrentMinimumAutoSpellLevel.Text = "Set Current Minimum";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMinimumAutoSpellLevel_Click);
            // 
            // tsmiSetCurrentMaximumAutoSpellLevel
            // 
            this.tsmiSetCurrentMaximumAutoSpellLevel.Name = "tsmiSetCurrentMaximumAutoSpellLevel";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Size = new System.Drawing.Size(222, 24);
            this.tsmiSetCurrentMaximumAutoSpellLevel.Text = "Set Current Maximum";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMaximumAutoSpellLevel_Click);
            // 
            // sepAutoSpellLevels1
            // 
            this.sepAutoSpellLevels1.Name = "sepAutoSpellLevels1";
            this.sepAutoSpellLevels1.Size = new System.Drawing.Size(219, 6);
            // 
            // tsmiRestoreCurrentAutoSpellLevels
            // 
            this.tsmiRestoreCurrentAutoSpellLevels.Name = "tsmiRestoreCurrentAutoSpellLevels";
            this.tsmiRestoreCurrentAutoSpellLevels.Size = new System.Drawing.Size(222, 24);
            this.tsmiRestoreCurrentAutoSpellLevels.Text = "Restore Current";
            this.tsmiRestoreCurrentAutoSpellLevels.Click += new System.EventHandler(this.tsmiRestoreCurrentAutoSpellLevels_Click);
            // 
            // lblCurrentRealmValue
            // 
            this.lblCurrentRealmValue.BackColor = System.Drawing.Color.White;
            this.lblCurrentRealmValue.ContextMenuStrip = this.ctxRealm;
            this.lblCurrentRealmValue.Location = new System.Drawing.Point(145, 30);
            this.lblCurrentRealmValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentRealmValue.Name = "lblCurrentRealmValue";
            this.lblCurrentRealmValue.Size = new System.Drawing.Size(221, 18);
            this.lblCurrentRealmValue.TabIndex = 141;
            this.lblCurrentRealmValue.Text = "Realm";
            this.lblCurrentRealmValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtCurrentWeaponValue
            // 
            this.txtCurrentWeaponValue.ContextMenuStrip = this.ctxWeapon;
            this.txtCurrentWeaponValue.Location = new System.Drawing.Point(145, 55);
            this.txtCurrentWeaponValue.Margin = new System.Windows.Forms.Padding(4);
            this.txtCurrentWeaponValue.Name = "txtCurrentWeaponValue";
            this.txtCurrentWeaponValue.Size = new System.Drawing.Size(221, 22);
            this.txtCurrentWeaponValue.TabIndex = 142;
            // 
            // lblCurrentAutoEscapeValue
            // 
            this.lblCurrentAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblCurrentAutoEscapeValue.ContextMenuStrip = this.ctxAutoEscape;
            this.lblCurrentAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblCurrentAutoEscapeValue.Location = new System.Drawing.Point(145, 85);
            this.lblCurrentAutoEscapeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentAutoEscapeValue.Name = "lblCurrentAutoEscapeValue";
            this.lblCurrentAutoEscapeValue.Size = new System.Drawing.Size(221, 18);
            this.lblCurrentAutoEscapeValue.TabIndex = 143;
            this.lblCurrentAutoEscapeValue.Text = "Auto Escape";
            this.lblCurrentAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSelectEmptyColor
            // 
            this.btnSelectEmptyColor.Location = new System.Drawing.Point(374, 172);
            this.btnSelectEmptyColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectEmptyColor.Name = "btnSelectEmptyColor";
            this.btnSelectEmptyColor.Size = new System.Drawing.Size(73, 28);
            this.btnSelectEmptyColor.TabIndex = 145;
            this.btnSelectEmptyColor.Text = "Select";
            this.btnSelectEmptyColor.UseVisualStyleBackColor = true;
            this.btnSelectEmptyColor.Click += new System.EventHandler(this.btnSelectEmptyColor_Click);
            // 
            // btnSelectFullColor
            // 
            this.btnSelectFullColor.Location = new System.Drawing.Point(374, 143);
            this.btnSelectFullColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectFullColor.Name = "btnSelectFullColor";
            this.btnSelectFullColor.Size = new System.Drawing.Size(73, 28);
            this.btnSelectFullColor.TabIndex = 144;
            this.btnSelectFullColor.Text = "Select";
            this.btnSelectFullColor.UseVisualStyleBackColor = true;
            this.btnSelectFullColor.Click += new System.EventHandler(this.btnSelectFullColor_Click);
            // 
            // lblEmptyColorValue
            // 
            this.lblEmptyColorValue.BackColor = System.Drawing.Color.Red;
            this.lblEmptyColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblEmptyColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblEmptyColorValue.Location = new System.Drawing.Point(145, 177);
            this.lblEmptyColorValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmptyColorValue.Name = "lblEmptyColorValue";
            this.lblEmptyColorValue.Size = new System.Drawing.Size(221, 18);
            this.lblEmptyColorValue.TabIndex = 143;
            this.lblEmptyColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColor
            // 
            this.lblEmptyColor.AutoSize = true;
            this.lblEmptyColor.Location = new System.Drawing.Point(12, 178);
            this.lblEmptyColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmptyColor.Name = "lblEmptyColor";
            this.lblEmptyColor.Size = new System.Drawing.Size(81, 16);
            this.lblEmptyColor.TabIndex = 142;
            this.lblEmptyColor.Text = "Empty color:";
            // 
            // lblFullColorValue
            // 
            this.lblFullColorValue.BackColor = System.Drawing.Color.Green;
            this.lblFullColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblFullColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblFullColorValue.Location = new System.Drawing.Point(145, 146);
            this.lblFullColorValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFullColorValue.Name = "lblFullColorValue";
            this.lblFullColorValue.Size = new System.Drawing.Size(221, 18);
            this.lblFullColorValue.TabIndex = 141;
            this.lblFullColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFullColor
            // 
            this.lblFullColor.AutoSize = true;
            this.lblFullColor.Location = new System.Drawing.Point(12, 148);
            this.lblFullColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFullColor.Name = "lblFullColor";
            this.lblFullColor.Size = new System.Drawing.Size(64, 16);
            this.lblFullColor.TabIndex = 140;
            this.lblFullColor.Text = "Full color:";
            // 
            // grpStrategies
            // 
            this.grpStrategies.Controls.Add(this.lstStrategies);
            this.grpStrategies.Location = new System.Drawing.Point(484, 15);
            this.grpStrategies.Margin = new System.Windows.Forms.Padding(4);
            this.grpStrategies.Name = "grpStrategies";
            this.grpStrategies.Padding = new System.Windows.Forms.Padding(4);
            this.grpStrategies.Size = new System.Drawing.Size(440, 346);
            this.grpStrategies.TabIndex = 142;
            this.grpStrategies.TabStop = false;
            this.grpStrategies.Text = "Strategies";
            // 
            // lstStrategies
            // 
            this.lstStrategies.ContextMenuStrip = this.ctxStrategies;
            this.lstStrategies.FormattingEnabled = true;
            this.lstStrategies.ItemHeight = 16;
            this.lstStrategies.Location = new System.Drawing.Point(8, 23);
            this.lstStrategies.Margin = new System.Windows.Forms.Padding(4);
            this.lstStrategies.Name = "lstStrategies";
            this.lstStrategies.Size = new System.Drawing.Size(409, 308);
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
            this.tsmiMoveStrategyDown});
            this.ctxStrategies.Name = "ctxStrategies";
            this.ctxStrategies.Size = new System.Drawing.Size(159, 124);
            this.ctxStrategies.Opening += new System.ComponentModel.CancelEventHandler(this.ctxStrategies_Opening);
            // 
            // tsmiAddStrategy
            // 
            this.tsmiAddStrategy.Name = "tsmiAddStrategy";
            this.tsmiAddStrategy.Size = new System.Drawing.Size(158, 24);
            this.tsmiAddStrategy.Text = "Add";
            this.tsmiAddStrategy.Click += new System.EventHandler(this.tsmiAddStrategy_Click);
            // 
            // tsmiEditStrategy
            // 
            this.tsmiEditStrategy.Name = "tsmiEditStrategy";
            this.tsmiEditStrategy.Size = new System.Drawing.Size(158, 24);
            this.tsmiEditStrategy.Text = "Edit";
            this.tsmiEditStrategy.Click += new System.EventHandler(this.tsmiEditStrategy_Click);
            // 
            // tsmiRemoveStrategy
            // 
            this.tsmiRemoveStrategy.Name = "tsmiRemoveStrategy";
            this.tsmiRemoveStrategy.Size = new System.Drawing.Size(158, 24);
            this.tsmiRemoveStrategy.Text = "Remove";
            this.tsmiRemoveStrategy.Click += new System.EventHandler(this.tsmiRemoveStrategy_Click);
            // 
            // tsmiMoveStrategyUp
            // 
            this.tsmiMoveStrategyUp.Name = "tsmiMoveStrategyUp";
            this.tsmiMoveStrategyUp.Size = new System.Drawing.Size(158, 24);
            this.tsmiMoveStrategyUp.Text = "Move Up";
            this.tsmiMoveStrategyUp.Click += new System.EventHandler(this.tsmiMoveStrategyUp_Click);
            // 
            // tsmiMoveStrategyDown
            // 
            this.tsmiMoveStrategyDown.Name = "tsmiMoveStrategyDown";
            this.tsmiMoveStrategyDown.Size = new System.Drawing.Size(158, 24);
            this.tsmiMoveStrategyDown.Text = "Move Down";
            this.tsmiMoveStrategyDown.Click += new System.EventHandler(this.tsmiMoveStrategyDown_Click);
            // 
            // chkRemoveAllOnStartup
            // 
            this.chkRemoveAllOnStartup.AutoSize = true;
            this.chkRemoveAllOnStartup.Location = new System.Drawing.Point(148, 296);
            this.chkRemoveAllOnStartup.Margin = new System.Windows.Forms.Padding(4);
            this.chkRemoveAllOnStartup.Name = "chkRemoveAllOnStartup";
            this.chkRemoveAllOnStartup.Size = new System.Drawing.Size(166, 20);
            this.chkRemoveAllOnStartup.TabIndex = 146;
            this.chkRemoveAllOnStartup.Text = "Remove all on startup?";
            this.chkRemoveAllOnStartup.UseVisualStyleBackColor = true;
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 416);
            this.Controls.Add(this.chkRemoveAllOnStartup);
            this.Controls.Add(this.btnSelectEmptyColor);
            this.Controls.Add(this.lblCurrentAutoSpellLevelsValue);
            this.Controls.Add(this.lblCurrentRealmValue);
            this.Controls.Add(this.lblFullColorValue);
            this.Controls.Add(this.txtCurrentWeaponValue);
            this.Controls.Add(this.btnSelectFullColor);
            this.Controls.Add(this.lblWeapon);
            this.Controls.Add(this.lblCurrentAutoEscapeValue);
            this.Controls.Add(this.lblPreferredAlignment);
            this.Controls.Add(this.lblEmptyColorValue);
            this.Controls.Add(this.lblPreferredAlignmentValue);
            this.Controls.Add(this.lblEmptyColor);
            this.Controls.Add(this.lblFullColor);
            this.Controls.Add(this.grpStrategies);
            this.Controls.Add(this.chkQueryMonsterStatus);
            this.Controls.Add(this.chkVerboseOutput);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.ctxRealm.ResumeLayout(false);
            this.ctxWeapon.ResumeLayout(false);
            this.ctxAutoEscape.ResumeLayout(false);
            this.ctxPreferredAlignment.ResumeLayout(false);
            this.ctxAutoSpellLevels.ResumeLayout(false);
            this.grpStrategies.ResumeLayout(false);
            this.ctxStrategies.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button btnSelectFullColor;
        private System.Windows.Forms.Button btnSelectEmptyColor;
        private System.Windows.Forms.GroupBox grpStrategies;
        private System.Windows.Forms.ListBox lstStrategies;
        private System.Windows.Forms.ContextMenuStrip ctxStrategies;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveStrategyUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveStrategyDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreferredAlignmentGood;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreferredAlignmentEvil;
        private System.Windows.Forms.ToolStripSeparator sepPreferredAlignment1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreferredAlignmentRestoreOriginalValue;
        private System.Windows.Forms.Label lblCurrentAutoSpellLevelsValue;
        private System.Windows.Forms.Label lblCurrentRealmValue;
        private System.Windows.Forms.TextBox txtCurrentWeaponValue;
        private System.Windows.Forms.Label lblCurrentAutoEscapeValue;
        private System.Windows.Forms.ContextMenuStrip ctxAutoSpellLevels;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMinimumAutoSpellLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMaximumAutoSpellLevel;
        private System.Windows.Forms.ToolStripSeparator sepAutoSpellLevels1;
        private System.Windows.Forms.ContextMenuStrip ctxRealm;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmEarth;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmFire;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmWater;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmWind;
        private System.Windows.Forms.ToolStripSeparator sepRealm1;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreCurrentRealm;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreCurrentAutoSpellLevels;
        private System.Windows.Forms.ContextMenuStrip ctxWeapon;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreCurrentWeapon;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearCurrentAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeFlee;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeHazy;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeActive;
        private System.Windows.Forms.ToolStripSeparator sepAutoEscape1;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeInactive;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreCurrentAutoEscape;
        private System.Windows.Forms.CheckBox chkRemoveAllOnStartup;
    }
}