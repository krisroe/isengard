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
            this.lblDefaultRealm = new System.Windows.Forms.Label();
            this.lblRealm = new System.Windows.Forms.Label();
            this.ctxDefaultRealm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiEarth = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFire = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWater = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWind = new System.Windows.Forms.ToolStripMenuItem();
            this.lblDefaultWeapon = new System.Windows.Forms.Label();
            this.txtDefaultWeapon = new System.Windows.Forms.TextBox();
            this.lblPreferredAlignment = new System.Windows.Forms.Label();
            this.lblAutoEscape = new System.Windows.Forms.Label();
            this.lblAutoEscapeValue = new System.Windows.Forms.Label();
            this.ctxAutoEscape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAutoEscapeFlee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAutoEscapeOnByDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoEscapeOffByDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.chkQueryMonsterStatus = new System.Windows.Forms.CheckBox();
            this.chkVerboseOutput = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPreferredAlignmentValue = new System.Windows.Forms.Label();
            this.ctxPreferredAlignment = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiTogglePreferredAlignment = new System.Windows.Forms.ToolStripMenuItem();
            this.grpDefaults = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAutoSpellLevelsValue = new System.Windows.Forms.Label();
            this.ctxAutoSpellLevels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetMinimumSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetMaximumSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.grpSettings = new System.Windows.Forms.GroupBox();
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
            this.tsmiAutoEscapeSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAutoEscapeRestoreOriginalValue = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDefaultRealm.SuspendLayout();
            this.ctxAutoEscape.SuspendLayout();
            this.ctxPreferredAlignment.SuspendLayout();
            this.grpDefaults.SuspendLayout();
            this.ctxAutoSpellLevels.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpStrategies.SuspendLayout();
            this.ctxStrategies.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDefaultRealm
            // 
            this.lblDefaultRealm.AutoSize = true;
            this.lblDefaultRealm.Location = new System.Drawing.Point(14, 22);
            this.lblDefaultRealm.Name = "lblDefaultRealm";
            this.lblDefaultRealm.Size = new System.Drawing.Size(40, 13);
            this.lblDefaultRealm.TabIndex = 0;
            this.lblDefaultRealm.Text = "Realm:";
            // 
            // lblRealm
            // 
            this.lblRealm.BackColor = System.Drawing.Color.White;
            this.lblRealm.ContextMenuStrip = this.ctxDefaultRealm;
            this.lblRealm.Location = new System.Drawing.Point(129, 20);
            this.lblRealm.Name = "lblRealm";
            this.lblRealm.Size = new System.Drawing.Size(166, 15);
            this.lblRealm.TabIndex = 125;
            this.lblRealm.Text = "Realm";
            this.lblRealm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxDefaultRealm
            // 
            this.ctxDefaultRealm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEarth,
            this.tsmiFire,
            this.tsmiWater,
            this.tsmiWind});
            this.ctxDefaultRealm.Name = "ctxDefaultRealm";
            this.ctxDefaultRealm.Size = new System.Drawing.Size(104, 92);
            // 
            // tsmiEarth
            // 
            this.tsmiEarth.Name = "tsmiEarth";
            this.tsmiEarth.Size = new System.Drawing.Size(103, 22);
            this.tsmiEarth.Text = "earth";
            this.tsmiEarth.Click += new System.EventHandler(this.tsmiRealm_Click);
            // 
            // tsmiFire
            // 
            this.tsmiFire.Name = "tsmiFire";
            this.tsmiFire.Size = new System.Drawing.Size(103, 22);
            this.tsmiFire.Text = "fire";
            this.tsmiFire.Click += new System.EventHandler(this.tsmiRealm_Click);
            // 
            // tsmiWater
            // 
            this.tsmiWater.Name = "tsmiWater";
            this.tsmiWater.Size = new System.Drawing.Size(103, 22);
            this.tsmiWater.Text = "water";
            this.tsmiWater.Click += new System.EventHandler(this.tsmiRealm_Click);
            // 
            // tsmiWind
            // 
            this.tsmiWind.Name = "tsmiWind";
            this.tsmiWind.Size = new System.Drawing.Size(103, 22);
            this.tsmiWind.Text = "wind";
            this.tsmiWind.Click += new System.EventHandler(this.tsmiRealm_Click);
            // 
            // lblDefaultWeapon
            // 
            this.lblDefaultWeapon.AutoSize = true;
            this.lblDefaultWeapon.Location = new System.Drawing.Point(14, 43);
            this.lblDefaultWeapon.Name = "lblDefaultWeapon";
            this.lblDefaultWeapon.Size = new System.Drawing.Size(51, 13);
            this.lblDefaultWeapon.TabIndex = 126;
            this.lblDefaultWeapon.Text = "Weapon:";
            // 
            // txtDefaultWeapon
            // 
            this.txtDefaultWeapon.Location = new System.Drawing.Point(129, 40);
            this.txtDefaultWeapon.Name = "txtDefaultWeapon";
            this.txtDefaultWeapon.Size = new System.Drawing.Size(166, 20);
            this.txtDefaultWeapon.TabIndex = 127;
            // 
            // lblPreferredAlignment
            // 
            this.lblPreferredAlignment.AutoSize = true;
            this.lblPreferredAlignment.Location = new System.Drawing.Point(14, 68);
            this.lblPreferredAlignment.Name = "lblPreferredAlignment";
            this.lblPreferredAlignment.Size = new System.Drawing.Size(101, 13);
            this.lblPreferredAlignment.TabIndex = 128;
            this.lblPreferredAlignment.Text = "Preferred alignment:";
            // 
            // lblAutoEscape
            // 
            this.lblAutoEscape.AutoSize = true;
            this.lblAutoEscape.Location = new System.Drawing.Point(15, 67);
            this.lblAutoEscape.Name = "lblAutoEscape";
            this.lblAutoEscape.Size = new System.Drawing.Size(70, 13);
            this.lblAutoEscape.TabIndex = 132;
            this.lblAutoEscape.Text = "Auto escape:";
            // 
            // lblAutoEscapeValue
            // 
            this.lblAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblAutoEscapeValue.ContextMenuStrip = this.ctxAutoEscape;
            this.lblAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblAutoEscapeValue.Location = new System.Drawing.Point(129, 65);
            this.lblAutoEscapeValue.Name = "lblAutoEscapeValue";
            this.lblAutoEscapeValue.Size = new System.Drawing.Size(166, 15);
            this.lblAutoEscapeValue.TabIndex = 133;
            this.lblAutoEscapeValue.Text = "Auto Escape";
            this.lblAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxAutoEscape
            // 
            this.ctxAutoEscape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetAutoEscapeThreshold,
            this.tsmiClearAutoEscapeThreshold,
            this.tsmiAutoEscapeSeparator1,
            this.tsmiAutoEscapeFlee,
            this.tsmiAutoEscapeHazy,
            this.tsmiAutoEscapeSeparator2,
            this.tsmiAutoEscapeOnByDefault,
            this.tsmiAutoEscapeOffByDefault,
            this.tsmiAutoEscapeSeparator3,
            this.tsmiAutoEscapeRestoreOriginalValue});
            this.ctxAutoEscape.Name = "ctxAutoEscape";
            this.ctxAutoEscape.Size = new System.Drawing.Size(190, 198);
            this.ctxAutoEscape.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoEscape_Opening);
            // 
            // tsmiSetAutoEscapeThreshold
            // 
            this.tsmiSetAutoEscapeThreshold.Name = "tsmiSetAutoEscapeThreshold";
            this.tsmiSetAutoEscapeThreshold.Size = new System.Drawing.Size(189, 22);
            this.tsmiSetAutoEscapeThreshold.Text = "Set Threshold";
            this.tsmiSetAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiSetAutoEscapeThreshold_Click);
            // 
            // tsmiClearAutoEscapeThreshold
            // 
            this.tsmiClearAutoEscapeThreshold.Name = "tsmiClearAutoEscapeThreshold";
            this.tsmiClearAutoEscapeThreshold.Size = new System.Drawing.Size(189, 22);
            this.tsmiClearAutoEscapeThreshold.Text = "Clear Threshold";
            this.tsmiClearAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiClearAutoEscapeThreshold_Click);
            // 
            // tsmiAutoEscapeSeparator1
            // 
            this.tsmiAutoEscapeSeparator1.Name = "tsmiAutoEscapeSeparator1";
            this.tsmiAutoEscapeSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // tsmiAutoEscapeFlee
            // 
            this.tsmiAutoEscapeFlee.Checked = true;
            this.tsmiAutoEscapeFlee.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiAutoEscapeFlee.Name = "tsmiAutoEscapeFlee";
            this.tsmiAutoEscapeFlee.Size = new System.Drawing.Size(189, 22);
            this.tsmiAutoEscapeFlee.Text = "Flee";
            this.tsmiAutoEscapeFlee.Click += new System.EventHandler(this.tsmiAutoEscapeFlee_Click);
            // 
            // tsmiAutoEscapeHazy
            // 
            this.tsmiAutoEscapeHazy.Name = "tsmiAutoEscapeHazy";
            this.tsmiAutoEscapeHazy.Size = new System.Drawing.Size(189, 22);
            this.tsmiAutoEscapeHazy.Text = "Hazy";
            this.tsmiAutoEscapeHazy.Click += new System.EventHandler(this.tsmiAutoEscapeHazy_Click);
            // 
            // tsmiAutoEscapeSeparator2
            // 
            this.tsmiAutoEscapeSeparator2.Name = "tsmiAutoEscapeSeparator2";
            this.tsmiAutoEscapeSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // tsmiAutoEscapeOnByDefault
            // 
            this.tsmiAutoEscapeOnByDefault.Name = "tsmiAutoEscapeOnByDefault";
            this.tsmiAutoEscapeOnByDefault.Size = new System.Drawing.Size(189, 22);
            this.tsmiAutoEscapeOnByDefault.Text = "On by Default";
            this.tsmiAutoEscapeOnByDefault.Click += new System.EventHandler(this.tsmiAutoEscapeOnByDefault_Click);
            // 
            // tsmiAutoEscapeOffByDefault
            // 
            this.tsmiAutoEscapeOffByDefault.Checked = true;
            this.tsmiAutoEscapeOffByDefault.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiAutoEscapeOffByDefault.Name = "tsmiAutoEscapeOffByDefault";
            this.tsmiAutoEscapeOffByDefault.Size = new System.Drawing.Size(189, 22);
            this.tsmiAutoEscapeOffByDefault.Text = "Off by Default";
            this.tsmiAutoEscapeOffByDefault.Click += new System.EventHandler(this.tsmiAutoEscapeOffByDefault_Click);
            // 
            // chkQueryMonsterStatus
            // 
            this.chkQueryMonsterStatus.AutoSize = true;
            this.chkQueryMonsterStatus.Location = new System.Drawing.Point(126, 96);
            this.chkQueryMonsterStatus.Name = "chkQueryMonsterStatus";
            this.chkQueryMonsterStatus.Size = new System.Drawing.Size(131, 17);
            this.chkQueryMonsterStatus.TabIndex = 135;
            this.chkQueryMonsterStatus.Text = "Query monster status?";
            this.chkQueryMonsterStatus.UseVisualStyleBackColor = true;
            // 
            // chkVerboseOutput
            // 
            this.chkVerboseOutput.AutoSize = true;
            this.chkVerboseOutput.Location = new System.Drawing.Point(126, 119);
            this.chkVerboseOutput.Name = "chkVerboseOutput";
            this.chkVerboseOutput.Size = new System.Drawing.Size(104, 17);
            this.chkVerboseOutput.TabIndex = 136;
            this.chkVerboseOutput.Text = "Verbose output?";
            this.chkVerboseOutput.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(550, 299);
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
            this.btnCancel.Location = new System.Drawing.Point(629, 299);
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
            this.lblPreferredAlignmentValue.Location = new System.Drawing.Point(129, 68);
            this.lblPreferredAlignmentValue.Name = "lblPreferredAlignmentValue";
            this.lblPreferredAlignmentValue.Size = new System.Drawing.Size(166, 15);
            this.lblPreferredAlignmentValue.TabIndex = 139;
            this.lblPreferredAlignmentValue.Text = "Good";
            this.lblPreferredAlignmentValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxPreferredAlignment
            // 
            this.ctxPreferredAlignment.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTogglePreferredAlignment});
            this.ctxPreferredAlignment.Name = "ctxPreferredAlignment";
            this.ctxPreferredAlignment.Size = new System.Drawing.Size(110, 26);
            // 
            // tsmiTogglePreferredAlignment
            // 
            this.tsmiTogglePreferredAlignment.Name = "tsmiTogglePreferredAlignment";
            this.tsmiTogglePreferredAlignment.Size = new System.Drawing.Size(109, 22);
            this.tsmiTogglePreferredAlignment.Text = "Toggle";
            this.tsmiTogglePreferredAlignment.Click += new System.EventHandler(this.tsmiTogglePreferredAlignment_Click);
            // 
            // grpDefaults
            // 
            this.grpDefaults.Controls.Add(this.label1);
            this.grpDefaults.Controls.Add(this.lblAutoSpellLevelsValue);
            this.grpDefaults.Controls.Add(this.lblRealm);
            this.grpDefaults.Controls.Add(this.lblDefaultRealm);
            this.grpDefaults.Controls.Add(this.lblDefaultWeapon);
            this.grpDefaults.Controls.Add(this.txtDefaultWeapon);
            this.grpDefaults.Controls.Add(this.lblAutoEscapeValue);
            this.grpDefaults.Controls.Add(this.lblAutoEscape);
            this.grpDefaults.Location = new System.Drawing.Point(12, 12);
            this.grpDefaults.Name = "grpDefaults";
            this.grpDefaults.Size = new System.Drawing.Size(366, 119);
            this.grpDefaults.TabIndex = 140;
            this.grpDefaults.TabStop = false;
            this.grpDefaults.Text = "Defaults";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 139;
            this.label1.Text = "Auto spell levels:";
            // 
            // lblAutoSpellLevelsValue
            // 
            this.lblAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblAutoSpellLevelsValue.ContextMenuStrip = this.ctxAutoSpellLevels;
            this.lblAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblAutoSpellLevelsValue.Location = new System.Drawing.Point(129, 88);
            this.lblAutoSpellLevelsValue.Name = "lblAutoSpellLevelsValue";
            this.lblAutoSpellLevelsValue.Size = new System.Drawing.Size(166, 15);
            this.lblAutoSpellLevelsValue.TabIndex = 138;
            this.lblAutoSpellLevelsValue.Text = "Min:Max";
            this.lblAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetMinimumSpellLevel,
            this.tsmiSetMaximumSpellLevel});
            this.ctxAutoSpellLevels.Name = "ctxAutoSpellLevels";
            this.ctxAutoSpellLevels.Size = new System.Drawing.Size(149, 48);
            // 
            // tsmiSetMinimumSpellLevel
            // 
            this.tsmiSetMinimumSpellLevel.Name = "tsmiSetMinimumSpellLevel";
            this.tsmiSetMinimumSpellLevel.Size = new System.Drawing.Size(148, 22);
            this.tsmiSetMinimumSpellLevel.Text = "Set Minimum";
            this.tsmiSetMinimumSpellLevel.Click += new System.EventHandler(this.tsmiSetMinimumSpellLevel_Click);
            // 
            // tsmiSetMaximumSpellLevel
            // 
            this.tsmiSetMaximumSpellLevel.Name = "tsmiSetMaximumSpellLevel";
            this.tsmiSetMaximumSpellLevel.Size = new System.Drawing.Size(148, 22);
            this.tsmiSetMaximumSpellLevel.Text = "Set Maximum";
            this.tsmiSetMaximumSpellLevel.Click += new System.EventHandler(this.tsmiSetMaximumSpellLevel_Click);
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.btnSelectEmptyColor);
            this.grpSettings.Controls.Add(this.btnSelectFullColor);
            this.grpSettings.Controls.Add(this.lblEmptyColorValue);
            this.grpSettings.Controls.Add(this.lblEmptyColor);
            this.grpSettings.Controls.Add(this.lblFullColorValue);
            this.grpSettings.Controls.Add(this.lblFullColor);
            this.grpSettings.Controls.Add(this.lblPreferredAlignmentValue);
            this.grpSettings.Controls.Add(this.lblPreferredAlignment);
            this.grpSettings.Controls.Add(this.chkQueryMonsterStatus);
            this.grpSettings.Controls.Add(this.chkVerboseOutput);
            this.grpSettings.Location = new System.Drawing.Point(12, 147);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(366, 147);
            this.grpSettings.TabIndex = 141;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // btnSelectEmptyColor
            // 
            this.btnSelectEmptyColor.Location = new System.Drawing.Point(301, 37);
            this.btnSelectEmptyColor.Name = "btnSelectEmptyColor";
            this.btnSelectEmptyColor.Size = new System.Drawing.Size(55, 23);
            this.btnSelectEmptyColor.TabIndex = 145;
            this.btnSelectEmptyColor.Text = "Select";
            this.btnSelectEmptyColor.UseVisualStyleBackColor = true;
            this.btnSelectEmptyColor.Click += new System.EventHandler(this.btnSelectEmptyColor_Click);
            // 
            // btnSelectFullColor
            // 
            this.btnSelectFullColor.Location = new System.Drawing.Point(301, 13);
            this.btnSelectFullColor.Name = "btnSelectFullColor";
            this.btnSelectFullColor.Size = new System.Drawing.Size(55, 23);
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
            this.lblEmptyColorValue.Location = new System.Drawing.Point(129, 41);
            this.lblEmptyColorValue.Name = "lblEmptyColorValue";
            this.lblEmptyColorValue.Size = new System.Drawing.Size(166, 15);
            this.lblEmptyColorValue.TabIndex = 143;
            this.lblEmptyColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColor
            // 
            this.lblEmptyColor.AutoSize = true;
            this.lblEmptyColor.Location = new System.Drawing.Point(14, 41);
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
            this.lblFullColorValue.Location = new System.Drawing.Point(129, 16);
            this.lblFullColorValue.Name = "lblFullColorValue";
            this.lblFullColorValue.Size = new System.Drawing.Size(166, 15);
            this.lblFullColorValue.TabIndex = 141;
            this.lblFullColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFullColor
            // 
            this.lblFullColor.AutoSize = true;
            this.lblFullColor.Location = new System.Drawing.Point(14, 16);
            this.lblFullColor.Name = "lblFullColor";
            this.lblFullColor.Size = new System.Drawing.Size(52, 13);
            this.lblFullColor.TabIndex = 140;
            this.lblFullColor.Text = "Full color:";
            // 
            // grpStrategies
            // 
            this.grpStrategies.Controls.Add(this.lstStrategies);
            this.grpStrategies.Location = new System.Drawing.Point(388, 12);
            this.grpStrategies.Name = "grpStrategies";
            this.grpStrategies.Size = new System.Drawing.Size(330, 281);
            this.grpStrategies.TabIndex = 142;
            this.grpStrategies.TabStop = false;
            this.grpStrategies.Text = "Strategies";
            // 
            // lstStrategies
            // 
            this.lstStrategies.ContextMenuStrip = this.ctxStrategies;
            this.lstStrategies.FormattingEnabled = true;
            this.lstStrategies.Location = new System.Drawing.Point(6, 19);
            this.lstStrategies.Name = "lstStrategies";
            this.lstStrategies.Size = new System.Drawing.Size(308, 251);
            this.lstStrategies.TabIndex = 0;
            // 
            // ctxStrategies
            // 
            this.ctxStrategies.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddStrategy,
            this.tsmiEditStrategy,
            this.tsmiRemoveStrategy,
            this.tsmiMoveStrategyUp,
            this.tsmiMoveStrategyDown});
            this.ctxStrategies.Name = "ctxStrategies";
            this.ctxStrategies.Size = new System.Drawing.Size(139, 114);
            this.ctxStrategies.Opening += new System.ComponentModel.CancelEventHandler(this.ctxStrategies_Opening);
            // 
            // tsmiAddStrategy
            // 
            this.tsmiAddStrategy.Name = "tsmiAddStrategy";
            this.tsmiAddStrategy.Size = new System.Drawing.Size(138, 22);
            this.tsmiAddStrategy.Text = "Add";
            this.tsmiAddStrategy.Click += new System.EventHandler(this.tsmiAddStrategy_Click);
            // 
            // tsmiEditStrategy
            // 
            this.tsmiEditStrategy.Name = "tsmiEditStrategy";
            this.tsmiEditStrategy.Size = new System.Drawing.Size(138, 22);
            this.tsmiEditStrategy.Text = "Edit";
            this.tsmiEditStrategy.Click += new System.EventHandler(this.tsmiEditStrategy_Click);
            // 
            // tsmiRemoveStrategy
            // 
            this.tsmiRemoveStrategy.Name = "tsmiRemoveStrategy";
            this.tsmiRemoveStrategy.Size = new System.Drawing.Size(138, 22);
            this.tsmiRemoveStrategy.Text = "Remove";
            this.tsmiRemoveStrategy.Click += new System.EventHandler(this.tsmiRemoveStrategy_Click);
            // 
            // tsmiMoveStrategyUp
            // 
            this.tsmiMoveStrategyUp.Name = "tsmiMoveStrategyUp";
            this.tsmiMoveStrategyUp.Size = new System.Drawing.Size(138, 22);
            this.tsmiMoveStrategyUp.Text = "Move Up";
            this.tsmiMoveStrategyUp.Click += new System.EventHandler(this.tsmiMoveStrategyUp_Click);
            // 
            // tsmiMoveStrategyDown
            // 
            this.tsmiMoveStrategyDown.Name = "tsmiMoveStrategyDown";
            this.tsmiMoveStrategyDown.Size = new System.Drawing.Size(138, 22);
            this.tsmiMoveStrategyDown.Text = "Move Down";
            this.tsmiMoveStrategyDown.Click += new System.EventHandler(this.tsmiMoveStrategyDown_Click);
            // 
            // tsmiAutoEscapeSeparator3
            // 
            this.tsmiAutoEscapeSeparator3.Name = "tsmiAutoEscapeSeparator3";
            this.tsmiAutoEscapeSeparator3.Size = new System.Drawing.Size(186, 6);
            // 
            // tsmiAutoEscapeRestoreOriginalValue
            // 
            this.tsmiAutoEscapeRestoreOriginalValue.Name = "tsmiAutoEscapeRestoreOriginalValue";
            this.tsmiAutoEscapeRestoreOriginalValue.Size = new System.Drawing.Size(189, 22);
            this.tsmiAutoEscapeRestoreOriginalValue.Text = "Restore Original Value";
            this.tsmiAutoEscapeRestoreOriginalValue.Click += new System.EventHandler(this.tsmiAutoEscapeRestoreOriginalValue_Click);
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 335);
            this.Controls.Add(this.grpStrategies);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.grpDefaults);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.ctxDefaultRealm.ResumeLayout(false);
            this.ctxAutoEscape.ResumeLayout(false);
            this.ctxPreferredAlignment.ResumeLayout(false);
            this.grpDefaults.ResumeLayout(false);
            this.grpDefaults.PerformLayout();
            this.ctxAutoSpellLevels.ResumeLayout(false);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpStrategies.ResumeLayout(false);
            this.ctxStrategies.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDefaultRealm;
        private System.Windows.Forms.Label lblRealm;
        private System.Windows.Forms.Label lblDefaultWeapon;
        private System.Windows.Forms.TextBox txtDefaultWeapon;
        private System.Windows.Forms.Label lblPreferredAlignment;
        private System.Windows.Forms.Label lblAutoEscape;
        private System.Windows.Forms.Label lblAutoEscapeValue;
        private System.Windows.Forms.CheckBox chkQueryMonsterStatus;
        private System.Windows.Forms.CheckBox chkVerboseOutput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip ctxDefaultRealm;
        private System.Windows.Forms.ToolStripMenuItem tsmiEarth;
        private System.Windows.Forms.ToolStripMenuItem tsmiFire;
        private System.Windows.Forms.ToolStripMenuItem tsmiWater;
        private System.Windows.Forms.ToolStripMenuItem tsmiWind;
        private System.Windows.Forms.ContextMenuStrip ctxAutoEscape;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearAutoEscapeThreshold;
        private System.Windows.Forms.Label lblPreferredAlignmentValue;
        private System.Windows.Forms.ContextMenuStrip ctxPreferredAlignment;
        private System.Windows.Forms.ToolStripMenuItem tsmiTogglePreferredAlignment;
        private System.Windows.Forms.GroupBox grpDefaults;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Label lblFullColorValue;
        private System.Windows.Forms.Label lblFullColor;
        private System.Windows.Forms.Label lblEmptyColorValue;
        private System.Windows.Forms.Label lblEmptyColor;
        private System.Windows.Forms.Button btnSelectFullColor;
        private System.Windows.Forms.Button btnSelectEmptyColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAutoSpellLevelsValue;
        private System.Windows.Forms.ContextMenuStrip ctxAutoSpellLevels;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetMinimumSpellLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetMaximumSpellLevel;
        private System.Windows.Forms.GroupBox grpStrategies;
        private System.Windows.Forms.ListBox lstStrategies;
        private System.Windows.Forms.ContextMenuStrip ctxStrategies;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveStrategy;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveStrategyUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveStrategyDown;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeFlee;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeHazy;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeOnByDefault;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeOffByDefault;
        private System.Windows.Forms.ToolStripSeparator tsmiAutoEscapeSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoEscapeRestoreOriginalValue;
    }
}