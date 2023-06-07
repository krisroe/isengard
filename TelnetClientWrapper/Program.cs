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

            LoadConfiguration(out List<Macro> allMacros);

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

            Application.Run(new frmMain(userName, password, allMacros));
        }

        private static void LoadConfiguration(out List<Macro> allMacros)
        {
            allMacros = new List<Macro>();

            List<string> errorMessages = new List<string>();

            allMacros.Add(Macro.GenerateCannedMacro("C*+A*"));
            allMacros.Add(Macro.GenerateCannedMacro("SC*+A*"));
            allMacros.Add(Macro.GenerateCannedMacro("SCCSC*+A*"));
            allMacros.Add(Macro.GenerateCannedMacro("SCCSCCF+A*"));
            allMacros.Add(Macro.GenerateCannedMacro("C*"));
            allMacros.Add(Macro.GenerateCannedMacro("SC*"));
            allMacros.Add(Macro.GenerateCannedMacro("SCCSC*"));
            allMacros.Add(Macro.GenerateCannedMacro("A*"));
            allMacros.Add(Macro.GenerateCannedMacro("Skills"));
            allMacros.Add(Macro.GenerateCannedMacro("Heal"));

            if (errorMessages.Count > 0)
            {
                MessageBox.Show("Errors loading configuration file" + Environment.NewLine + string.Join(Environment.NewLine, errorMessages));
            }
        }
    }
}
