using IsengardClient.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
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
            string userName;
            using (frmLogin loginForm = new frmLogin(IsengardSettings.Default.UserName))
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                userName = loginForm.UserName;
                password = loginForm.Password;
            }

            Application.Run(new frmMain(userName, password));
        }
    }
}
