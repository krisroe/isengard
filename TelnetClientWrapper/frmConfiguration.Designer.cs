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
            this.lblAutoHazy = new System.Windows.Forms.Label();
            this.lblAutoHazyValue = new System.Windows.Forms.Label();
            this.ctxAutoHazy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetAutoHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearAutoHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.chkQueryMonsterStatus = new System.Windows.Forms.CheckBox();
            this.chkVerboseOutput = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPreferredAlignmentValue = new System.Windows.Forms.Label();
            this.ctxPreferredAlignment = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiTogglePreferredAlignment = new System.Windows.Forms.ToolStripMenuItem();
            this.grpDefaults = new System.Windows.Forms.GroupBox();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.btnSelectEmptyColor = new System.Windows.Forms.Button();
            this.btnSelectFullColor = new System.Windows.Forms.Button();
            this.lblEmptyColorValue = new System.Windows.Forms.Label();
            this.lblEmptyColor = new System.Windows.Forms.Label();
            this.lblFullColorValue = new System.Windows.Forms.Label();
            this.lblFullColor = new System.Windows.Forms.Label();
            this.lblAutoSpellLevelsValue = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ctxAutoSpellLevels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetMinimumSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetMaximumSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDefaultRealm.SuspendLayout();
            this.ctxAutoHazy.SuspendLayout();
            this.ctxPreferredAlignment.SuspendLayout();
            this.grpDefaults.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.ctxAutoSpellLevels.SuspendLayout();
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
            // lblAutoHazy
            // 
            this.lblAutoHazy.AutoSize = true;
            this.lblAutoHazy.Location = new System.Drawing.Point(15, 67);
            this.lblAutoHazy.Name = "lblAutoHazy";
            this.lblAutoHazy.Size = new System.Drawing.Size(57, 13);
            this.lblAutoHazy.TabIndex = 132;
            this.lblAutoHazy.Text = "Auto hazy:";
            // 
            // lblAutoHazyValue
            // 
            this.lblAutoHazyValue.BackColor = System.Drawing.Color.Black;
            this.lblAutoHazyValue.ContextMenuStrip = this.ctxAutoHazy;
            this.lblAutoHazyValue.ForeColor = System.Drawing.Color.White;
            this.lblAutoHazyValue.Location = new System.Drawing.Point(129, 65);
            this.lblAutoHazyValue.Name = "lblAutoHazyValue";
            this.lblAutoHazyValue.Size = new System.Drawing.Size(166, 15);
            this.lblAutoHazyValue.TabIndex = 133;
            this.lblAutoHazyValue.Text = "Auto Hazy";
            this.lblAutoHazyValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxAutoHazy
            // 
            this.ctxAutoHazy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetAutoHazy,
            this.tsmiClearAutoHazy});
            this.ctxAutoHazy.Name = "ctxAutoHazy";
            this.ctxAutoHazy.Size = new System.Drawing.Size(102, 48);
            // 
            // tsmiSetAutoHazy
            // 
            this.tsmiSetAutoHazy.Name = "tsmiSetAutoHazy";
            this.tsmiSetAutoHazy.Size = new System.Drawing.Size(101, 22);
            this.tsmiSetAutoHazy.Text = "Set";
            this.tsmiSetAutoHazy.Click += new System.EventHandler(this.tsmiSetOrClearAutoHazy_Click);
            // 
            // tsmiClearAutoHazy
            // 
            this.tsmiClearAutoHazy.Name = "tsmiClearAutoHazy";
            this.tsmiClearAutoHazy.Size = new System.Drawing.Size(101, 22);
            this.tsmiClearAutoHazy.Text = "Clear";
            this.tsmiClearAutoHazy.Click += new System.EventHandler(this.tsmiClearAutoHazy_Click);
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
            this.btnOK.Location = new System.Drawing.Point(139, 300);
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
            this.btnCancel.Location = new System.Drawing.Point(218, 300);
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
            this.grpDefaults.Controls.Add(this.lblAutoHazyValue);
            this.grpDefaults.Controls.Add(this.lblAutoHazy);
            this.grpDefaults.Location = new System.Drawing.Point(12, 12);
            this.grpDefaults.Name = "grpDefaults";
            this.grpDefaults.Size = new System.Drawing.Size(366, 119);
            this.grpDefaults.TabIndex = 140;
            this.grpDefaults.TabStop = false;
            this.grpDefaults.Text = "Defaults";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 139;
            this.label1.Text = "Auto spell levels:";
            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetMinimumSpellLevel,
            this.tsmiSetMaximumSpellLevel});
            this.ctxAutoSpellLevels.Name = "ctxAutoSpellLevels";
            this.ctxAutoSpellLevels.Size = new System.Drawing.Size(181, 70);
            // 
            // tsmiSetMinimumSpellLevel
            // 
            this.tsmiSetMinimumSpellLevel.Name = "tsmiSetMinimumSpellLevel";
            this.tsmiSetMinimumSpellLevel.Size = new System.Drawing.Size(180, 22);
            this.tsmiSetMinimumSpellLevel.Text = "Set Minimum";
            this.tsmiSetMinimumSpellLevel.Click += new System.EventHandler(this.tsmiSetMinimumSpellLevel_Click);
            // 
            // tsmiSetMaximumSpellLevel
            // 
            this.tsmiSetMaximumSpellLevel.Name = "tsmiSetMaximumSpellLevel";
            this.tsmiSetMaximumSpellLevel.Size = new System.Drawing.Size(180, 22);
            this.tsmiSetMaximumSpellLevel.Text = "Set Maximum";
            this.tsmiSetMaximumSpellLevel.Click += new System.EventHandler(this.tsmiSetMaximumSpellLevel_Click);
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 347);
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
            this.ctxAutoHazy.ResumeLayout(false);
            this.ctxPreferredAlignment.ResumeLayout(false);
            this.grpDefaults.ResumeLayout(false);
            this.grpDefaults.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.ctxAutoSpellLevels.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDefaultRealm;
        private System.Windows.Forms.Label lblRealm;
        private System.Windows.Forms.Label lblDefaultWeapon;
        private System.Windows.Forms.TextBox txtDefaultWeapon;
        private System.Windows.Forms.Label lblPreferredAlignment;
        private System.Windows.Forms.Label lblAutoHazy;
        private System.Windows.Forms.Label lblAutoHazyValue;
        private System.Windows.Forms.CheckBox chkQueryMonsterStatus;
        private System.Windows.Forms.CheckBox chkVerboseOutput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip ctxDefaultRealm;
        private System.Windows.Forms.ToolStripMenuItem tsmiEarth;
        private System.Windows.Forms.ToolStripMenuItem tsmiFire;
        private System.Windows.Forms.ToolStripMenuItem tsmiWater;
        private System.Windows.Forms.ToolStripMenuItem tsmiWind;
        private System.Windows.Forms.ContextMenuStrip ctxAutoHazy;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetAutoHazy;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearAutoHazy;
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
    }
}