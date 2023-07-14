using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
namespace IsengardClient
{
    internal static class Program
    {
        internal const string HOST_NAME = "isengard.nazgul.com";
        internal const int PORT = 4040;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Ping p = new Ping();
            PingReply pr = p.Send(HOST_NAME);
            if (pr.Status != IPStatus.Success)
            {
                MessageBox.Show("Ping failed: " + pr.Status);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string password;
            string userName = IsengardSettings.Default.UserName;
            while (true)
            {
                bool generateFullLog;
                using (frmLogin loginForm = new frmLogin(userName))
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    userName = loginForm.UserName;
                    password = loginForm.Password;
                    generateFullLog = loginForm.GenerateFullLog;
                }
                bool logout;
                using (frmMain frm = new frmMain(userName, password, generateFullLog))
                {
                    frm.ShowDialog();
                    logout = frm.Logout;
                }
                if (logout)
                {
                    userName = string.Empty;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
