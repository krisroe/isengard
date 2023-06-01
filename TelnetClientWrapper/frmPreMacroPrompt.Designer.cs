namespace IsengardClient
{
    partial class frmPreMacroPrompt
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.chkPowerAttack = new System.Windows.Forms.CheckBox();
            this.chkManashield = new System.Windows.Forms.CheckBox();
            this.grpSkills = new System.Windows.Forms.GroupBox();
            this.lblMob = new System.Windows.Forms.Label();
            this.cboMob = new System.Windows.Forms.ComboBox();
            this.flp.SuspendLayout();
            this.grpSkills.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(200, 198);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(281, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // flp
            // 
            this.flp.Controls.Add(this.chkPowerAttack);
            this.flp.Controls.Add(this.chkManashield);
            this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp.Location = new System.Drawing.Point(3, 16);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(322, 126);
            this.flp.TabIndex = 0;
            // 
            // chkPowerAttack
            // 
            this.chkPowerAttack.AutoSize = true;
            this.chkPowerAttack.Location = new System.Drawing.Point(3, 3);
            this.chkPowerAttack.Name = "chkPowerAttack";
            this.chkPowerAttack.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.chkPowerAttack.Size = new System.Drawing.Size(95, 22);
            this.chkPowerAttack.TabIndex = 1;
            this.chkPowerAttack.Text = "Power Attack";
            this.chkPowerAttack.UseVisualStyleBackColor = true;
            // 
            // chkManashield
            // 
            this.chkManashield.AutoSize = true;
            this.chkManashield.Location = new System.Drawing.Point(3, 31);
            this.chkManashield.Name = "chkManashield";
            this.chkManashield.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.chkManashield.Size = new System.Drawing.Size(85, 22);
            this.chkManashield.TabIndex = 0;
            this.chkManashield.Text = "Manashield";
            this.chkManashield.UseVisualStyleBackColor = true;
            // 
            // grpSkills
            // 
            this.grpSkills.Controls.Add(this.flp);
            this.grpSkills.Location = new System.Drawing.Point(39, 47);
            this.grpSkills.Name = "grpSkills";
            this.grpSkills.Size = new System.Drawing.Size(328, 145);
            this.grpSkills.TabIndex = 3;
            this.grpSkills.TabStop = false;
            this.grpSkills.Text = "Skills";
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(36, 23);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(31, 13);
            this.lblMob.TabIndex = 4;
            this.lblMob.Text = "Mob:";
            // 
            // cboMob
            // 
            this.cboMob.FormattingEnabled = true;
            this.cboMob.Location = new System.Drawing.Point(73, 20);
            this.cboMob.Name = "cboMob";
            this.cboMob.Size = new System.Drawing.Size(283, 21);
            this.cboMob.TabIndex = 5;
            // 
            // frmPreMacroPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 238);
            this.ControlBox = false;
            this.Controls.Add(this.cboMob);
            this.Controls.Add(this.lblMob);
            this.Controls.Add(this.grpSkills);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "frmPreMacroPrompt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Skills";
            this.flp.ResumeLayout(false);
            this.flp.PerformLayout();
            this.grpSkills.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkManashield;
        private System.Windows.Forms.CheckBox chkPowerAttack;
        private System.Windows.Forms.GroupBox grpSkills;
        private System.Windows.Forms.Label lblMob;
        private System.Windows.Forms.ComboBox cboMob;
    }
}