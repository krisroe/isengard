using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static IsengardClient.frmMain;

using System.Xml;

namespace IsengardClient
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadConfiguration(out string defaultRealm, out int level, out int totalhp, out int totalmp, out int healtickmp, out AlignmentType preferredAlignment, out string userName, out List<Macro> allMacros, out List<string> startupCommands, out string defaultWeapon, out int autoHazyThreshold, out bool autoHazyDefault, out bool verboseMode, out bool queryMonsterStatus);

            string password;
            using (frmLogin loginForm = new frmLogin(userName))
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                userName = loginForm.UserName;
                password = loginForm.Password;
            }

            Application.Run(new frmMain(defaultRealm, level, totalhp, totalmp, healtickmp, preferredAlignment, userName, password, allMacros, startupCommands, defaultWeapon, autoHazyThreshold, autoHazyDefault, verboseMode, queryMonsterStatus));
        }

        private static void LoadConfiguration(out string defaultRealm, out int level, out int totalhp, out int totalmp, out int healtickmp, out AlignmentType preferredAlignment, out string userName, out List<Macro> allMacros, out List<string> startupCommands, out string defaultWeapon, out int autoHazyThreshold, out bool autoHazyDefault, out bool verboseMode, out bool queryMonsterStatus)
        {
            defaultRealm = string.Empty;
            level = 0;
            totalhp = 0;
            totalmp = 0;
            healtickmp = 0;
            preferredAlignment = AlignmentType.Grey;
            userName = string.Empty;
            allMacros = new List<Macro>();
            startupCommands = new List<string>();
            defaultWeapon = string.Empty;
            autoHazyThreshold = 0;
            autoHazyDefault = false;
            verboseMode = false;
            queryMonsterStatus = false;

            string configurationFile = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "Configuration.xml");
            FileInfo fi = new FileInfo(configurationFile);
            if (!fi.Exists)
            {
                MessageBox.Show("Configuration.xml does not exist!");
                return;
            }
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(configurationFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Configuration.xml: " + ex.ToString());
                return;
            }

            List<string> errorMessages = new List<string>();

            XmlElement docElement = doc.DocumentElement;

            defaultRealm = docElement.GetAttribute("defaultrealm");
            if (!string.IsNullOrEmpty(defaultRealm))
            {
                defaultRealm = defaultRealm.ToLower();
                if (defaultRealm != "earth" &&
                    defaultRealm != "fire" &&
                    defaultRealm != "water" &&
                    defaultRealm != "wind")
                {
                    MessageBox.Show("Invalid default realm: " + defaultRealm);
                    defaultRealm = string.Empty;
                }
            }

            defaultWeapon = docElement.GetAttribute("defaultweapon");

            string sLevel = docElement.GetAttribute("level");
            if (string.IsNullOrEmpty(sLevel))
            {
                MessageBox.Show("No level specified");
                level = 1;
            }
            else if (!int.TryParse(sLevel, out level))
            {
                MessageBox.Show("Invalid level specified: " + sLevel);
                level = 1;
            }

            string sTotalHP = docElement.GetAttribute("totalhp");
            if (string.IsNullOrEmpty(sTotalHP))
            {
                MessageBox.Show("No total HP specified.");
                totalhp = 0;
            }
            else if (!int.TryParse(sTotalHP, out totalhp))
            {
                MessageBox.Show("Invalid total HP specified: " + sTotalHP);
                totalhp = 0;
            }

            string sTotalMP = docElement.GetAttribute("totalmp");
            if (string.IsNullOrEmpty(sTotalMP))
            {
                MessageBox.Show("No total MP specified.");
                totalmp = 0;
            }
            else if (!int.TryParse(sTotalMP, out totalmp))
            {
                MessageBox.Show("Invalid total MP specified: " + sTotalMP);
                totalmp = 0;
            }

            string sHealTickMP = docElement.GetAttribute("healtickmp");
            if (string.IsNullOrEmpty(sHealTickMP))
            {
                MessageBox.Show("No heal tick MP specified.");
                healtickmp = 0;
            }
            else if (!int.TryParse(sHealTickMP, out healtickmp))
            {
                MessageBox.Show("Invalid heal tick MP specified: " + sHealTickMP);
                healtickmp = 0;
            }

            string sAutoHazyThreshold = docElement.GetAttribute("autohazythreshold");
            if (!string.IsNullOrEmpty(sAutoHazyThreshold))
            {
                if (!int.TryParse(sAutoHazyThreshold, out autoHazyThreshold))
                {
                    MessageBox.Show("Invalid auto hazy threshold: " + sAutoHazyThreshold);
                    autoHazyThreshold = 0;
                }
            }

            string sAutoHazyDefault = docElement.GetAttribute("autohazydefault");
            if (!string.IsNullOrEmpty(sAutoHazyDefault))
            {
                if (!bool.TryParse(sAutoHazyDefault, out autoHazyDefault))
                {
                    MessageBox.Show("Invalid auto hazy default: " + sAutoHazyDefault);
                    autoHazyDefault = false;
                }
            }

            string sVerbose = docElement.GetAttribute("verbose");
            if (!string.IsNullOrEmpty(sVerbose))
            {
                if (!bool.TryParse(sVerbose, out verboseMode))
                {
                    MessageBox.Show("Invalid verbose: " + sVerbose);
                    verboseMode = false;
                }
            }

            string sQueryMonsterStatus = docElement.GetAttribute("queryMonsterStatus");
            if (!string.IsNullOrEmpty(sQueryMonsterStatus))
            {
                if (!bool.TryParse(sQueryMonsterStatus, out queryMonsterStatus))
                {
                    MessageBox.Show("Invalid query monster status: " + sQueryMonsterStatus);
                    queryMonsterStatus = false;
                }
            }

            string sPreferredAlignment = docElement.GetAttribute("preferredalignment");
            if (Enum.TryParse(sPreferredAlignment, out AlignmentType ePreferredAlignment))
            {
                preferredAlignment = ePreferredAlignment;
            }
            else
            {
                MessageBox.Show("Invalid preferred alignment type");
                preferredAlignment = AlignmentType.Grey;
            }

            string sStartupCommands = docElement.GetAttribute("startupcommands");
            if (!string.IsNullOrEmpty(sStartupCommands))
            {
                startupCommands.AddRange(sStartupCommands.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            }

            userName = docElement.GetAttribute("username");

            allMacros.Add(Macro.GenerateCannedMacro("C*+A*"));
            allMacros.Add(Macro.GenerateCannedMacro("SC*+A*"));
            allMacros.Add(Macro.GenerateCannedMacro("SCCSC*+A*"));
            allMacros.Add(Macro.GenerateCannedMacro("C*"));
            allMacros.Add(Macro.GenerateCannedMacro("SC*"));
            allMacros.Add(Macro.GenerateCannedMacro("SCCSC*"));
            allMacros.Add(Macro.GenerateCannedMacro("A*"));
            allMacros.Add(Macro.GenerateCannedMacro("Flee"));
            allMacros.Add(Macro.GenerateCannedMacro("Skills"));
            allMacros.Add(Macro.GenerateCannedMacro("Heal"));

            if (errorMessages.Count > 0)
            {
                MessageBox.Show("Errors loading configuration file" + Environment.NewLine + string.Join(Environment.NewLine, errorMessages));
            }
        }
    }
}
