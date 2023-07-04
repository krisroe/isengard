namespace IsengardClient
{
    partial class frmDisconnected
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
            this.lblDisconnected = new System.Windows.Forms.Label();
            this.btnReconnect = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.chkSaveSettings = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblDisconnected
            // 
            this.lblDisconnected.AutoSize = true;
            this.lblDisconnected.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisconnected.Location = new System.Drawing.Point(13, 31);
            this.lblDisconnected.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisconnected.Name = "lblDisconnected";
            this.lblDisconnected.Size = new System.Drawing.Size(458, 19);
            this.lblDisconnected.TabIndex = 0;
            this.lblDisconnected.Text = "You have been disconnected. How would you like to proceed?";
            // 
            // btnReconnect
            // 
            this.btnReconnect.Location = new System.Drawing.Point(117, 71);
            this.btnReconnect.Name = "btnReconnect";
            this.btnReconnect.Size = new System.Drawing.Size(110, 37);
            this.btnReconnect.TabIndex = 1;
            this.btnReconnect.Text = "Reconnect";
            this.btnReconnect.UseVisualStyleBackColor = true;
            this.btnReconnect.Click += new System.EventHandler(this.btnReconnect_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(233, 71);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(110, 37);
            this.btnQuit.TabIndex = 2;
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(349, 71);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(110, 37);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // chkSaveSettings
            // 
            this.chkSaveSettings.AutoSize = true;
            this.chkSaveSettings.Location = new System.Drawing.Point(287, 127);
            this.chkSaveSettings.Name = "chkSaveSettings";
            this.chkSaveSettings.Size = new System.Drawing.Size(136, 23);
            this.chkSaveSettings.TabIndex = 4;
            this.chkSaveSettings.Text = "Save settings?";
            this.chkSaveSettings.UseVisualStyleBackColor = true;
            // 
            // frmDisconnected
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 178);
            this.ControlBox = false;
            this.Controls.Add(this.chkSaveSettings);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnReconnect);
            this.Controls.Add(this.lblDisconnected);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmDisconnected";
            this.Text = "Disconnected";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDisconnected;
        private System.Windows.Forms.Button btnReconnect;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.CheckBox chkSaveSettings;
    }
}