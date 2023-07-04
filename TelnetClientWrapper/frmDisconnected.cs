using System;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmDisconnected : Form
    {
        public frmDisconnected(bool saveSettingsDefault)
        {
            InitializeComponent();
            chkSaveSettings.Checked = saveSettingsDefault;
        }

        public DisconnectedAction Action { get; set; }


        public bool SaveSettings
        {
            get
            {
                return chkSaveSettings.Checked;
            }
        }

        private void btnReconnect_Click(object sender, EventArgs e)
        {
            Action = DisconnectedAction.Reconnect;
            Close();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Action = DisconnectedAction.Quit;
            Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Action = DisconnectedAction.Logout;
            Close();
        }
    }
}
