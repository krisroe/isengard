using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmConfiguration : Form
    {
        private List<Strategy> _strategies;
        private int _autoHazyThreshold;
        private AlignmentType _preferredAlignment;
        private string _defaultRealm;
        private Color _fullColor;
        private Color _emptyColor;
        private int _autoSpellLevelMinimum;
        private int _autoSpellLevelMaximum;
        internal const int AUTO_SPELL_LEVEL_MINIMUM = 1;
        internal const int AUTO_SPELL_LEVEL_MAXIMUM = 3;

        public frmConfiguration(List<Strategy> strategies)
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

            _autoSpellLevelMinimum = sets.DefaultAutoSpellLevelMin;
            _autoSpellLevelMaximum = sets.DefaultAutoSpellLevelMax;
            if (_autoSpellLevelMinimum > _autoSpellLevelMaximum || _autoSpellLevelMinimum < AUTO_SPELL_LEVEL_MINIMUM || _autoSpellLevelMinimum > AUTO_SPELL_LEVEL_MAXIMUM || _autoSpellLevelMaximum < AUTO_SPELL_LEVEL_MINIMUM || _autoSpellLevelMaximum > AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _autoSpellLevelMinimum = AUTO_SPELL_LEVEL_MINIMUM;
                _autoSpellLevelMaximum = AUTO_SPELL_LEVEL_MAXIMUM;
            }
            RefreshAutoSpellLevelUI();

            txtDefaultWeapon.Text = sets.DefaultWeapon;
            _preferredAlignment = ParseAlignment(sets.PreferredAlignment);
            RefreshAlignmentTypeUI();

            _autoHazyThreshold = sets.DefaultAutoHazyThreshold;
            if (_autoHazyThreshold < 0) _autoHazyThreshold = 0;
            UIShared.RefreshAutoHazyUI(_autoHazyThreshold, lblAutoHazyValue, tsmiClearAutoHazy, 0);

            chkQueryMonsterStatus.Checked = sets.QueryMonsterStatus;
            chkVerboseOutput.Checked = sets.VerboseMode;

            _fullColor = sets.FullColor;
            SetColorUI(lblFullColorValue, _fullColor);
            _emptyColor = sets.EmptyColor;
            SetColorUI(lblEmptyColorValue, _emptyColor);

            _strategies = new List<Strategy>();
            foreach (Strategy s in strategies)
            {
                Strategy sClone = new Strategy(s);
                _strategies.Add(sClone);
                lstStrategies.Items.Add(sClone);
            }
        }

        public bool ChangedStrategies { get; set; }

        public List<Strategy> Strategies
        {
            get
            {
                return _strategies;
            }
        }

        private void SetColorUI(Label lbl, Color c)
        {
            lbl.BackColor = c;
            GetRGBString(c, out string colorText, out Color fgColor);
            lbl.Text = colorText;
            lbl.ForeColor = fgColor;
        }

        internal void GetRGBString(Color c, out string text, out Color foregroundColor)
        {
            text = c.R + "," + c.G + "," + c.B;
            int iForegroundR = c.R <= 127 ? 255 : 0;
            int iForegroundG = c.G <= 127 ? 255 : 0;
            int iForegroundB = c.B <= 127 ? 255 : 0;
            foregroundColor = Color.FromArgb(iForegroundR, iForegroundG, iForegroundB);
        }

        internal AlignmentType PreferredAlignment
        {
            get
            {
                return _preferredAlignment;
            }
        }

        private static AlignmentType ParseAlignment(string alignment)
        {
            if (!Enum.TryParse(alignment, out AlignmentType at))
            {
                at = AlignmentType.Blue;
            }
            return at;
        }

        private void RefreshRealmUI()
        {
            lblRealm.Text = _defaultRealm;
            lblRealm.BackColor = UIShared.GetColorForRealm(_defaultRealm);
            UIShared.UpdateRealmMenu(ctxDefaultRealm, _defaultRealm);
        }

        private void RefreshAutoSpellLevelUI()
        {
            lblAutoSpellLevelsValue.Text = _autoSpellLevelMinimum + ":" + _autoSpellLevelMaximum;
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
            sets.FullColor = _fullColor;
            sets.EmptyColor = _emptyColor;
            sets.DefaultAutoSpellLevelMin = _autoSpellLevelMinimum;
            sets.DefaultAutoSpellLevelMax = _autoSpellLevelMaximum;
            IsengardSettings.Default.Save();

            //CSRTODO: save changes to strategies

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

        private void btnSelectFullColor_Click(object sender, EventArgs e)
        {
            Color? selected = PromptColor(_fullColor);
            if (selected.HasValue)
            {
                _fullColor = selected.Value;
                SetColorUI(lblFullColorValue, _fullColor);
            }
        }

        private void btnSelectEmptyColor_Click(object sender, EventArgs e)
        {
            Color? selected = PromptColor(_emptyColor);
            if (selected.HasValue)
            {
                _emptyColor = selected.Value;
                SetColorUI(lblEmptyColorValue, _emptyColor);
            }
        }

        private Color? PromptColor(Color initColor)
        {
            Color? ret = null;
            ColorDialog clg = new ColorDialog();
            clg.Color = initColor;
            if (clg.ShowDialog(this) == DialogResult.OK)
            {
                ret = clg.Color;
            }
            return ret;
        }

        private void tsmiSetMinimumSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _autoSpellLevelMinimum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _autoSpellLevelMinimum = iLevel;
                if (_autoSpellLevelMaximum < _autoSpellLevelMinimum)
                {
                    _autoSpellLevelMaximum = _autoSpellLevelMinimum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetMaximumSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _autoSpellLevelMaximum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _autoSpellLevelMaximum = iLevel;
                if (_autoSpellLevelMaximum < _autoSpellLevelMinimum)
                {
                    _autoSpellLevelMinimum = _autoSpellLevelMaximum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void btnAddStrategy_Click(object sender, EventArgs e)
        {
            //CSRTODO: implement me!
        }

        private void btnEditStrategy_Click(object sender, EventArgs e)
        {
            //CSRTODO: implement me!
        }

        private void btnRemoveStrategy_Click(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            _strategies.RemoveAt(iIndex);
            lstStrategies.Items.RemoveAt(iIndex);
            ChangedStrategies = true;
        }

        private void btnMoveStrategyUp_Click(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            Strategy s = (Strategy)lstStrategies.SelectedItem;
            lstStrategies.Items.RemoveAt(iIndex);
            lstStrategies.Items.Insert(iIndex - 1, s);
            _strategies.RemoveAt(iIndex);
            _strategies.Insert(iIndex - 1, s);
            lstStrategies.SelectedIndex = iIndex - 1;
            ChangedStrategies = true;
        }

        private void btnMoveStrategyDown_Click(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            bool isLastIndex = iIndex == lstStrategies.Items.Count - 2;
            Strategy s = (Strategy)lstStrategies.SelectedItem;
            lstStrategies.Items.RemoveAt(iIndex);
            _strategies.RemoveAt(iIndex);
            if (isLastIndex)
            {
                lstStrategies.Items.Add(s);
                _strategies.Add(s);
            }
            else
            {
                lstStrategies.Items.Insert(iIndex + 1, s);
                _strategies.Insert(iIndex + 1, s);
            }
            lstStrategies.SelectedIndex = iIndex + 1;
            ChangedStrategies = true;
        }

        private void lstStrategies_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            if (iIndex < 0)
            {
                btnEditStrategy.Enabled = false;
                btnRemoveStrategy.Enabled = false;
                btnMoveStrategyUp.Enabled = false;
                btnMoveStrategyDown.Enabled = false;
            }
            else
            {
                btnEditStrategy.Enabled = true;
                btnRemoveStrategy.Enabled = true;
                btnMoveStrategyUp.Enabled = iIndex > 0;
                btnMoveStrategyDown.Enabled = iIndex < lstStrategies.Items.Count - 1;
            }
        }
    }
}
