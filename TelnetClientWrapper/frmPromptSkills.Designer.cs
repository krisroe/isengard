namespace IsengardClient
{
    partial class frmPromptSkills
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
            this.flp.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(53, 109);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(134, 109);
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
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp.Location = new System.Drawing.Point(12, 3);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(197, 100);
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
            // frmPromptSkills
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 139);
            this.ControlBox = false;
            this.Controls.Add(this.flp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "frmPromptSkills";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Skills";
            this.flp.ResumeLayout(false);
            this.flp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flp;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkManashield;
        private System.Windows.Forms.CheckBox chkPowerAttack;
    }
}