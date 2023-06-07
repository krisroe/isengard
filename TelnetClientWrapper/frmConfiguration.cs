using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace IsengardClient
{
    public partial class frmConfiguration : Form
    {
        private int _autoHazyThreshold;
        private AlignmentType _preferredAlignment;
        private string _defaultRealm;

        public frmConfiguration()
        {
            InitializeComponent();

            IsengardSettings sets = IsengardSettings.Default;

            _defaultRealm = sets.DefaultRealm;
            if (_defaultRealm != "earth" &&
                _defaultRealm != "fire" &&
                _defaultRealm != "water" &&
                _defaultRealm != "wind")
            {
                _defaultRealm = "earth";
            }

            RefreshRealmUI();

            txtDefaultWeapon.Text = sets.DefaultWeapon;

            if (!Enum.TryParse(sets.PreferredAlignment, out _preferredAlignment))
            {
                _preferredAlignment = AlignmentType.Blue;
            }
            RefreshAlignmentTypeUI();

            _autoHazyThreshold = sets.DefaultAutoHazyThreshold;
            if (_autoHazyThreshold < 0) _autoHazyThreshold = 0;
            UIShared.RefreshAutoHazyUI(_autoHazyThreshold, lblAutoHazyValue, tsmiClearAutoHazy, 0);

            chkQueryMonsterStatus.Checked = sets.QueryMonsterStatus;
            chkVerboseOutput.Checked = sets.VerboseMode;
        }

        private void RefreshRealmUI()
        {
            lblRealm.Text = _defaultRealm;
            lblRealm.BackColor = UIShared.GetColorForRealm(_defaultRealm);
            UIShared.UpdateRealmMenu(ctxDefaultRealm, _defaultRealm);
        }

        private void tsmiClearAutoHazy_Click(object sender, EventArgs e)
        {
            _autoHazyThreshold = 0;
            UIShared.RefreshAutoHazyUI(_autoHazyThreshold, lblAutoHazyValue, tsmiClearAutoHazy, 0);
            tsmiClearAutoHazy.Visible = false;
        }

        private void tsmiSetOrClearAutoHazy_Click(object sender, EventArgs e)
        {
            string threshold = Interaction.InputBox("Threshold:", "Enter Threshold", _autoHazyThreshold.ToString());
            if (int.TryParse(threshold, out int iThreshold) && iThreshold > 0)
            {
                _autoHazyThreshold = iThreshold;
                UIShared.RefreshAutoHazyUI(_autoHazyThreshold, lblAutoHazyValue, tsmiClearAutoHazy, 0);
                tsmiClearAutoHazy.Visible = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IsengardSettings sets = IsengardSettings.Default;
            sets.DefaultWeapon = txtDefaultWeapon.Text;
            sets.DefaultRealm = _defaultRealm;
            sets.PreferredAlignment = _preferredAlignment.ToString();
            sets.DefaultAutoHazyThreshold = _autoHazyThreshold;
            sets.QueryMonsterStatus = chkQueryMonsterStatus.Checked;
            sets.VerboseMode = chkVerboseOutput.Checked;
            IsengardSettings.Default.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tsmiTogglePreferredAlignment_Click(object sender, EventArgs e)
        {
            AlignmentType newType;
            if (_preferredAlignment == AlignmentType.Blue)
                newType = AlignmentType.Red;
            else
                newType = AlignmentType.Blue;
            _preferredAlignment = newType;
            RefreshAlignmentTypeUI();
        }

        private void RefreshAlignmentTypeUI()
        {
            string sLabelText;
            Color cLabelBack;
            if (_preferredAlignment == AlignmentType.Blue)
            {
                sLabelText = "Good";
                cLabelBack = Color.Blue;
            }
            else
            {
                sLabelText = "Evil";
                cLabelBack = Color.Red;
            }
            lblPreferredAlignmentValue.BackColor = cLabelBack;
            lblPreferredAlignmentValue.Text = sLabelText;
        }

        private void tsmiRealm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            _defaultRealm = tsmi.Text;
            RefreshRealmUI();
        }
    }
}
