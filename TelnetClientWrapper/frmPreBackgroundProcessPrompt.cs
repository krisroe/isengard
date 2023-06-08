using System;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPreBackgroundProcessPrompt : Form
    {
        private bool _isCombatBackgroundProcess;

        public frmPreBackgroundProcessPrompt(PromptedSkills skills, Room targetRoom, string currentMob, bool isCombatMacro)
        {
            InitializeComponent();

            _isCombatBackgroundProcess = isCombatMacro;

            string sCurrentMob;
            if (targetRoom == null || string.IsNullOrEmpty(targetRoom.Mob1))
            {
                sCurrentMob = currentMob;
            }
            else if (currentMob.Equals(targetRoom.Mob1, StringComparison.OrdinalIgnoreCase))
            {
                sCurrentMob = currentMob;
            }
            else if (currentMob.Equals(targetRoom.Mob2, StringComparison.OrdinalIgnoreCase))
            {
                sCurrentMob = currentMob;
            }
            else if (currentMob.Equals(targetRoom.Mob3, StringComparison.OrdinalIgnoreCase))
            {
                sCurrentMob = currentMob;
            }
            else
            {
                sCurrentMob = targetRoom.Mob1;
            }
            if (targetRoom != null)
            {
                if (!string.IsNullOrEmpty(targetRoom.Mob1))
                {
                    cboMob.Items.Add(targetRoom.Mob1);
                }
                if (!string.IsNullOrEmpty(targetRoom.Mob2))
                {
                    cboMob.Items.Add(targetRoom.Mob2);
                }
                if (!string.IsNullOrEmpty(targetRoom.Mob3))
                {
                    cboMob.Items.Add(targetRoom.Mob3);
                }
            }
            if (cboMob.Items.Contains(sCurrentMob))
            {
                cboMob.SelectedItem = sCurrentMob;
            }
            else
            {
                cboMob.Text = sCurrentMob;
            }

            bool showPowerAttack = (skills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
            chkPowerAttack.Visible = showPowerAttack;
            chkPowerAttack.Enabled = showPowerAttack;
            chkPowerAttack.Checked = showPowerAttack;
            bool showManashield = (skills & PromptedSkills.Manashield) == PromptedSkills.Manashield;
            chkManashield.Visible = showManashield;
            chkManashield.Enabled = showManashield;
        }

        public string Mob
        {
            get
            {
                string ret;
                if (cboMob.SelectedItem == null)
                {
                    ret = cboMob.Text;
                }
                else
                {
                    ret = cboMob.SelectedItem.ToString();
                }
                return ret;
            }
        }

        public PromptedSkills SelectedSkills
        {
            get
            {
                PromptedSkills ret = PromptedSkills.None;
                if (chkPowerAttack.Enabled && chkPowerAttack.Checked)
                {
                    ret |= PromptedSkills.PowerAttack;
                }
                if (chkManashield.Enabled && chkManashield.Checked)
                {
                    ret |= PromptedSkills.Manashield;
                }
                return ret;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_isCombatBackgroundProcess && string.IsNullOrEmpty(this.Mob))
            {
                MessageBox.Show("No mob specified.");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
