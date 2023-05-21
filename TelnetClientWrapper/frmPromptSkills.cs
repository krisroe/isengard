using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPromptSkills : Form
    {
        public frmPromptSkills(PromptedSkills skills)
        {
            InitializeComponent();

            bool showPowerAttack = (skills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
            chkPowerAttack.Visible = showPowerAttack;
            chkPowerAttack.Enabled = showPowerAttack;
            chkPowerAttack.Checked = showPowerAttack;
            bool showManashield = (skills & PromptedSkills.Manashield) == PromptedSkills.Manashield;
            chkManashield.Visible = showManashield;
            chkManashield.Enabled = showManashield;
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
    }
}
