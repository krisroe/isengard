using System;
using System.Windows.Forms;
namespace IsengardClient
{
    public partial class frmStrategy : Form
    {
        /// <summary>
        /// set defaults for a new strategy
        /// </summary>
        public frmStrategy()
        {
            InitializeComponent();

            chkStopWhenKillMonster.Checked = true;

            cboMagicLastStep.Items.Add(string.Empty);
            foreach (var nextMagicStep in Enum.GetValues(typeof(MagicStrategyStep)))
            {
                cboMagicLastStep.Items.Add(nextMagicStep.ToString());
            }

            cboMeleeLastStep.Items.Add(string.Empty);
            foreach (var nextMeleeStep in Enum.GetValues(typeof(MeleeStrategyStep)))
            {
                cboMeleeLastStep.Items.Add(nextMeleeStep.ToString());
            }

            cboPotionsLastStep.Items.Add(string.Empty);
            foreach (var nextPotionsStep in Enum.GetValues(typeof(PotionsStrategyStep)))
            {
                cboPotionsLastStep.Items.Add(nextPotionsStep.ToString());
            }
        }

        public frmStrategy(Strategy s) : this()
        {
            txtName.Text = s.Name;
            chkAutogenerateName.Checked = s.AutogenerateName;

            chkStopWhenKillMonster.Checked = s.StopWhenKillMonster;
            if (s.FleeHPThreshold > 0)
            {
                txtFleeThreshold.Text = s.FleeHPThreshold.ToString();
            }
            if (s.ManaPool > 0)
            {
                txtManaPool.Text = s.ManaPool.ToString();
            }
            chkPromptForManaPool.Checked = s.PromptForManaPool;

            //CSRTODO: finish me!
        }

        private void chkAutogenerateName_CheckedChanged(object sender, EventArgs e)
        {
            txtName.Enabled = !chkAutogenerateName.Checked;
            if (chkAutogenerateName.Checked)
            {
                txtName.Text = string.Empty;
            }
        }
    }
}
