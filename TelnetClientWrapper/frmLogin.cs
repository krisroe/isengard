using System;
using System.Windows.Forms;

namespace IsengardClient
{
    public partial class frmLogin : Form
    {
        public frmLogin(string userName)
        {
            InitializeComponent();

            txtUserName.Text = userName;
            chkGenerateFullLog.Checked = IsengardSettings.Default.GenerateFullLog;
        }

        public string UserName
        {
            get
            {
                return txtUserName.Text;
            }
        }

        public string Password
        {
            get
            {
                return txtPassword.Text;
            }
        }

        public bool GenerateFullLog
        {
            get
            {
                return chkGenerateFullLog.Checked;
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            OKButtonEnabled();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            OKButtonEnabled();
        }

        private void OKButtonEnabled()
        {
            btnOK.Enabled = !string.IsNullOrEmpty(txtUserName.Text) && !string.IsNullOrEmpty(txtPassword.Text);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IsengardSettings sets = IsengardSettings.Default;
            IsengardSettings.Default.GenerateFullLog = chkGenerateFullLog.Checked;
            sets.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmLogin_Shown(object sender, EventArgs e)
        {
            TextBox initialTextBox;
            if (string.IsNullOrEmpty(txtUserName.Text))
                initialTextBox = txtUserName;
            else
                initialTextBox = txtPassword;
            initialTextBox.Focus();
        }
    }
}
