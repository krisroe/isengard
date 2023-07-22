namespace IsengardClient
{
    partial class ucStrategyModifications
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ctxToggleStrategyModificationOverride = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiToggleEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.grpStrategyModifications = new System.Windows.Forms.GroupBox();
            this.pnlStrategyModifications = new System.Windows.Forms.Panel();
            this.lblCurrentRealmValue = new System.Windows.Forms.Label();
            this.chkMagic = new System.Windows.Forms.CheckBox();
            this.chkMelee = new System.Windows.Forms.CheckBox();
            this.lblCurrentAutoSpellLevelsValue = new System.Windows.Forms.Label();
            this.chkPotions = new System.Windows.Forms.CheckBox();
            this.cboOnKillMonster = new System.Windows.Forms.ComboBox();
            this.lblOnKillMonster = new System.Windows.Forms.Label();
            this.ctxToggleStrategyModificationOverride.SuspendLayout();
            this.grpStrategyModifications.SuspendLayout();
            this.pnlStrategyModifications.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxToggleStrategyModificationOverride
            // 
            this.ctxToggleStrategyModificationOverride.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxToggleStrategyModificationOverride.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiToggleEnabled});
            this.ctxToggleStrategyModificationOverride.Name = "ctxCombatType";
            this.ctxToggleStrategyModificationOverride.Size = new System.Drawing.Size(125, 28);
            this.ctxToggleStrategyModificationOverride.Opening += new System.ComponentModel.CancelEventHandler(this.ctxToggleStrategyModificationOverride_Opening);
            // 
            // tsmiToggleEnabled
            // 
            this.tsmiToggleEnabled.Name = "tsmiToggleEnabled";
            this.tsmiToggleEnabled.Size = new System.Drawing.Size(124, 24);
            this.tsmiToggleEnabled.Text = "Toggle";
            this.tsmiToggleEnabled.Click += new System.EventHandler(this.tsmiToggleEnabled_Click);
            // 
            // grpStrategyModifications
            // 
            this.grpStrategyModifications.Controls.Add(this.pnlStrategyModifications);
            this.grpStrategyModifications.Location = new System.Drawing.Point(0, 0);
            this.grpStrategyModifications.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpStrategyModifications.Name = "grpStrategyModifications";
            this.grpStrategyModifications.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpStrategyModifications.Size = new System.Drawing.Size(979, 100);
            this.grpStrategyModifications.TabIndex = 26;
            this.grpStrategyModifications.TabStop = false;
            this.grpStrategyModifications.Text = "Strategy Modifications";
            // 
            // pnlStrategyModifications
            // 
            this.pnlStrategyModifications.Controls.Add(this.lblCurrentRealmValue);
            this.pnlStrategyModifications.Controls.Add(this.chkMagic);
            this.pnlStrategyModifications.Controls.Add(this.chkMelee);
            this.pnlStrategyModifications.Controls.Add(this.lblCurrentAutoSpellLevelsValue);
            this.pnlStrategyModifications.Controls.Add(this.chkPotions);
            this.pnlStrategyModifications.Controls.Add(this.cboOnKillMonster);
            this.pnlStrategyModifications.Controls.Add(this.lblOnKillMonster);
            this.pnlStrategyModifications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStrategyModifications.Location = new System.Drawing.Point(3, 24);
            this.pnlStrategyModifications.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlStrategyModifications.Name = "pnlStrategyModifications";
            this.pnlStrategyModifications.Size = new System.Drawing.Size(973, 72);
            this.pnlStrategyModifications.TabIndex = 151;
            this.pnlStrategyModifications.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlStrategyModifications_MouseUp);
            // 
            // lblCurrentRealmValue
            // 
            this.lblCurrentRealmValue.BackColor = System.Drawing.Color.White;
            this.lblCurrentRealmValue.Location = new System.Drawing.Point(595, 35);
            this.lblCurrentRealmValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentRealmValue.Name = "lblCurrentRealmValue";
            this.lblCurrentRealmValue.Size = new System.Drawing.Size(368, 27);
            this.lblCurrentRealmValue.TabIndex = 149;
            this.lblCurrentRealmValue.Text = "Realm";
            this.lblCurrentRealmValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkMagic
            // 
            this.chkMagic.AutoSize = true;
            this.chkMagic.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.chkMagic.Location = new System.Drawing.Point(14, 4);
            this.chkMagic.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkMagic.Name = "chkMagic";
            this.chkMagic.Size = new System.Drawing.Size(75, 23);
            this.chkMagic.TabIndex = 0;
            this.chkMagic.Text = "Magic";
            this.chkMagic.UseVisualStyleBackColor = true;
            this.chkMagic.CheckedChanged += new System.EventHandler(this.chkCombatType_CheckedChanged);
            // 
            // chkMelee
            // 
            this.chkMelee.AutoSize = true;
            this.chkMelee.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.chkMelee.Location = new System.Drawing.Point(143, 4);
            this.chkMelee.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkMelee.Name = "chkMelee";
            this.chkMelee.Size = new System.Drawing.Size(74, 23);
            this.chkMelee.TabIndex = 1;
            this.chkMelee.Text = "Melee";
            this.chkMelee.UseVisualStyleBackColor = true;
            this.chkMelee.CheckedChanged += new System.EventHandler(this.chkCombatType_CheckedChanged);
            // 
            // lblCurrentAutoSpellLevelsValue
            // 
            this.lblCurrentAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblCurrentAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentAutoSpellLevelsValue.Location = new System.Drawing.Point(595, 5);
            this.lblCurrentAutoSpellLevelsValue.Name = "lblCurrentAutoSpellLevelsValue";
            this.lblCurrentAutoSpellLevelsValue.Size = new System.Drawing.Size(368, 30);
            this.lblCurrentAutoSpellLevelsValue.TabIndex = 6;
            this.lblCurrentAutoSpellLevelsValue.Text = "Min:Max";
            this.lblCurrentAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkPotions
            // 
            this.chkPotions.AutoSize = true;
            this.chkPotions.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.chkPotions.Location = new System.Drawing.Point(285, 4);
            this.chkPotions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkPotions.Name = "chkPotions";
            this.chkPotions.Size = new System.Drawing.Size(85, 23);
            this.chkPotions.TabIndex = 2;
            this.chkPotions.Text = "Potions";
            this.chkPotions.UseVisualStyleBackColor = true;
            this.chkPotions.CheckedChanged += new System.EventHandler(this.chkCombatType_CheckedChanged);
            // 
            // cboOnKillMonster
            // 
            this.cboOnKillMonster.ContextMenuStrip = this.ctxToggleStrategyModificationOverride;
            this.cboOnKillMonster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOnKillMonster.FormattingEnabled = true;
            this.cboOnKillMonster.Location = new System.Drawing.Point(233, 35);
            this.cboOnKillMonster.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboOnKillMonster.Name = "cboOnKillMonster";
            this.cboOnKillMonster.Size = new System.Drawing.Size(329, 27);
            this.cboOnKillMonster.TabIndex = 4;
            // 
            // lblOnKillMonster
            // 
            this.lblOnKillMonster.AutoSize = true;
            this.lblOnKillMonster.Location = new System.Drawing.Point(10, 43);
            this.lblOnKillMonster.Name = "lblOnKillMonster";
            this.lblOnKillMonster.Size = new System.Drawing.Size(121, 19);
            this.lblOnKillMonster.TabIndex = 3;
            this.lblOnKillMonster.Text = "On kill monster:";
            // 
            // ucStrategyModifications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpStrategyModifications);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ucStrategyModifications";
            this.Size = new System.Drawing.Size(999, 100);
            this.ctxToggleStrategyModificationOverride.ResumeLayout(false);
            this.grpStrategyModifications.ResumeLayout(false);
            this.pnlStrategyModifications.ResumeLayout(false);
            this.pnlStrategyModifications.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpStrategyModifications;
        private System.Windows.Forms.Panel pnlStrategyModifications;
        private System.Windows.Forms.Label lblCurrentRealmValue;
        private System.Windows.Forms.CheckBox chkMagic;
        private System.Windows.Forms.CheckBox chkMelee;
        private System.Windows.Forms.Label lblCurrentAutoSpellLevelsValue;
        private System.Windows.Forms.CheckBox chkPotions;
        private System.Windows.Forms.ComboBox cboOnKillMonster;
        private System.Windows.Forms.Label lblOnKillMonster;
        private System.Windows.Forms.ContextMenuStrip ctxToggleStrategyModificationOverride;
        private System.Windows.Forms.ToolStripMenuItem tsmiToggleEnabled;
    }
}
