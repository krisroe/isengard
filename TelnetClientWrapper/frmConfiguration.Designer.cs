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
            this.ctxDefaultRealm.SuspendLayout();
            this.ctxAutoHazy.SuspendLayout();
            this.ctxPreferredAlignment.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDefaultRealm
            // 
            this.lblDefaultRealm.AutoSize = true;
            this.lblDefaultRealm.Location = new System.Drawing.Point(15, 11);
            this.lblDefaultRealm.Name = "lblDefaultRealm";
            this.lblDefaultRealm.Size = new System.Drawing.Size(72, 13);
            this.lblDefaultRealm.TabIndex = 0;
            this.lblDefaultRealm.Text = "Default realm:";
            // 
            // lblRealm
            // 
            this.lblRealm.ContextMenuStrip = this.ctxDefaultRealm;
            this.lblRealm.Location = new System.Drawing.Point(127, 9);
            this.lblRealm.Name = "lblRealm";
            this.lblRealm.Size = new System.Drawing.Size(169, 15);
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
            this.lblDefaultWeapon.Location = new System.Drawing.Point(15, 36);
            this.lblDefaultWeapon.Name = "lblDefaultWeapon";
            this.lblDefaultWeapon.Size = new System.Drawing.Size(85, 13);
            this.lblDefaultWeapon.TabIndex = 126;
            this.lblDefaultWeapon.Text = "Default weapon:";
            // 
            // txtDefaultWeapon
            // 
            this.txtDefaultWeapon.Location = new System.Drawing.Point(130, 33);
            this.txtDefaultWeapon.Name = "txtDefaultWeapon";
            this.txtDefaultWeapon.Size = new System.Drawing.Size(166, 20);
            this.txtDefaultWeapon.TabIndex = 127;
            // 
            // lblPreferredAlignment
            // 
            this.lblPreferredAlignment.AutoSize = true;
            this.lblPreferredAlignment.Location = new System.Drawing.Point(15, 64);
            this.lblPreferredAlignment.Name = "lblPreferredAlignment";
            this.lblPreferredAlignment.Size = new System.Drawing.Size(101, 13);
            this.lblPreferredAlignment.TabIndex = 128;
            this.lblPreferredAlignment.Text = "Preferred alignment:";
            // 
            // lblAutoHazy
            // 
            this.lblAutoHazy.AutoSize = true;
            this.lblAutoHazy.Location = new System.Drawing.Point(15, 90);
            this.lblAutoHazy.Name = "lblAutoHazy";
            this.lblAutoHazy.Size = new System.Drawing.Size(93, 13);
            this.lblAutoHazy.TabIndex = 132;
            this.lblAutoHazy.Text = "Default auto hazy:";
            // 
            // lblAutoHazyValue
            // 
            this.lblAutoHazyValue.BackColor = System.Drawing.Color.Black;
            this.lblAutoHazyValue.ContextMenuStrip = this.ctxAutoHazy;
            this.lblAutoHazyValue.ForeColor = System.Drawing.Color.White;
            this.lblAutoHazyValue.Location = new System.Drawing.Point(127, 88);
            this.lblAutoHazyValue.Name = "lblAutoHazyValue";
            this.lblAutoHazyValue.Size = new System.Drawing.Size(169, 15);
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
            this.chkQueryMonsterStatus.Location = new System.Drawing.Point(127, 111);
            this.chkQueryMonsterStatus.Name = "chkQueryMonsterStatus";
            this.chkQueryMonsterStatus.Size = new System.Drawing.Size(131, 17);
            this.chkQueryMonsterStatus.TabIndex = 135;
            this.chkQueryMonsterStatus.Text = "Query monster status?";
            this.chkQueryMonsterStatus.UseVisualStyleBackColor = true;
            // 
            // chkVerboseOutput
            // 
            this.chkVerboseOutput.AutoSize = true;
            this.chkVerboseOutput.Location = new System.Drawing.Point(127, 134);
            this.chkVerboseOutput.Name = "chkVerboseOutput";
            this.chkVerboseOutput.Size = new System.Drawing.Size(104, 17);
            this.chkVerboseOutput.TabIndex = 136;
            this.chkVerboseOutput.Text = "Verbose output?";
            this.chkVerboseOutput.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(127, 157);
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
            this.btnCancel.Location = new System.Drawing.Point(206, 157);
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
            this.lblPreferredAlignmentValue.Location = new System.Drawing.Point(127, 64);
            this.lblPreferredAlignmentValue.Name = "lblPreferredAlignmentValue";
            this.lblPreferredAlignmentValue.Size = new System.Drawing.Size(169, 15);
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
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 193);
            this.Controls.Add(this.lblPreferredAlignmentValue);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkVerboseOutput);
            this.Controls.Add(this.chkQueryMonsterStatus);
            this.Controls.Add(this.lblAutoHazyValue);
            this.Controls.Add(this.lblAutoHazy);
            this.Controls.Add(this.lblPreferredAlignment);
            this.Controls.Add(this.txtDefaultWeapon);
            this.Controls.Add(this.lblDefaultWeapon);
            this.Controls.Add(this.lblRealm);
            this.Controls.Add(this.lblDefaultRealm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.ctxDefaultRealm.ResumeLayout(false);
            this.ctxAutoHazy.ResumeLayout(false);
            this.ctxPreferredAlignment.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}