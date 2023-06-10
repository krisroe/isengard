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

        private int _autoEscapeThreshold;
        private AutoEscapeType _autoEscapeType;
        private bool _autoEscapeOnByDefault;
        private int _autoEscapeThresholdOriginal;
        private AutoEscapeType _autoEscapeTypeOriginal;
        private bool _autoEscapeOnByDefaultOriginal;


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

            _autoEscapeType = (AutoEscapeType)sets.DefaultAutoEscapeType;
            _autoEscapeThreshold = sets.DefaultAutoEscapeThreshold;
            _autoEscapeOnByDefault = sets.DefaultAutoEscapeOnByDefault;
            if (_autoEscapeType != AutoEscapeType.Flee && _autoEscapeType != AutoEscapeType.Hazy) _autoEscapeType = AutoEscapeType.Flee;
            if (_autoEscapeThreshold < 0) _autoEscapeThreshold = 0;
            if (_autoEscapeThreshold == 0) _autoEscapeOnByDefault = false;
            _autoEscapeOnByDefaultOriginal = _autoEscapeOnByDefault;
            _autoEscapeThresholdOriginal = _autoEscapeThreshold;
            _autoEscapeTypeOriginal = _autoEscapeType;
            RefreshAutoEscapeUI();

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

        private void btnOK_Click(object sender, EventArgs e)
        {
            IsengardSettings sets = IsengardSettings.Default;
            sets.DefaultWeapon = txtDefaultWeapon.Text;
            sets.DefaultRealm = _defaultRealm;
            sets.PreferredAlignment = _preferredAlignment.ToString();
            sets.DefaultAutoEscapeOnByDefault = _autoEscapeOnByDefault;
            sets.DefaultAutoEscapeThreshold = _autoEscapeThreshold;
            sets.DefaultAutoEscapeType = Convert.ToInt32(_autoEscapeType);
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

        private void ctxStrategies_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            if (iIndex < 0)
            {
                tsmiEditStrategy.Visible = false;
                tsmiRemoveStrategy.Visible = false;
                tsmiMoveStrategyUp.Visible = false;
                tsmiMoveStrategyDown.Visible = false;
            }
            else
            {
                tsmiEditStrategy.Visible = true;
                tsmiRemoveStrategy.Visible = true;
                tsmiMoveStrategyUp.Visible = iIndex > 0;
                tsmiMoveStrategyDown.Visible = iIndex < lstStrategies.Items.Count - 1;
            }
        }

        private void tsmiAddStrategy_Click(object sender, EventArgs e)
        {
            //CSRTODO: implement me!
        }

        private void tsmiEditStrategy_Click(object sender, EventArgs e)
        {
            //CSRTODO: implement me!
        }

        private void tsmiRemoveStrategy_Click(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            _strategies.RemoveAt(iIndex);
            lstStrategies.Items.RemoveAt(iIndex);
            ChangedStrategies = true;
        }

        private void MoveStrategyUp(int iIndex)
        {
            Strategy s = (Strategy)lstStrategies.SelectedItem;
            lstStrategies.Items.RemoveAt(iIndex);
            lstStrategies.Items.Insert(iIndex - 1, s);
            _strategies.RemoveAt(iIndex);
            _strategies.Insert(iIndex - 1, s);
            lstStrategies.SelectedIndex = iIndex - 1;
            ChangedStrategies = true;
        }

        private void tsmiMoveStrategyUp_Click(object sender, EventArgs e)
        {
            MoveStrategyUp(lstStrategies.SelectedIndex);
        }

        private void tsmiMoveStrategyDown_Click(object sender, EventArgs e)
        {
            MoveStrategyUp(lstStrategies.SelectedIndex + 1);
        }

        private void tsmiClearAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            _autoEscapeThreshold = 0;
            _autoEscapeOnByDefault = false;
            RefreshAutoEscapeUI();
        }

        private void tsmiSetAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            string sInitialValue = _autoEscapeThreshold > 0 ? _autoEscapeThreshold.ToString() : string.Empty;
            string threshold = Interaction.InputBox("Threshold:", "Enter Threshold", sInitialValue);
            if (int.TryParse(threshold, out int iThreshold) && iThreshold > 0)
            {
                _autoEscapeThreshold = iThreshold;
                RefreshAutoEscapeUI();
            }
        }

        private void RefreshAutoEscapeUI()
        {
            Color autoEscapeBackColor;
            string autoEscapeText;
            string sAutoEscapeType = _autoEscapeType == AutoEscapeType.Hazy ? "Hazy" : "Flee";
            if (_autoEscapeThreshold > 0)
            {
                autoEscapeText = sAutoEscapeType + " @ " + _autoEscapeThreshold.ToString();
            }
            else
            {
                autoEscapeText = sAutoEscapeType;
            }
            if (_autoEscapeOnByDefault)
            {
                if (_autoEscapeType == AutoEscapeType.Hazy)
                {
                    autoEscapeBackColor = Color.DarkBlue;
                }
                else //Flee
                {
                    autoEscapeBackColor = Color.DarkRed;
                }
            }
            else if (_autoEscapeThreshold > 0)
            {
                autoEscapeBackColor = Color.LightGray;
            }
            else
            {
                autoEscapeBackColor = Color.Black;
            }
            if (_autoEscapeOnByDefault)
            {
                autoEscapeText += " (On)";
            }
            else
            {
                autoEscapeText += " (Off)";
            }

            UIShared.GetForegroundColor(autoEscapeBackColor.R, autoEscapeBackColor.G, autoEscapeBackColor.G, out byte forer, out byte foreg, out byte foreb);
            lblAutoEscapeValue.BackColor = autoEscapeBackColor;
            lblAutoEscapeValue.ForeColor = Color.FromArgb(forer, foreg, foreb);
            lblAutoEscapeValue.Text = autoEscapeText;

            tsmiAutoEscapeFlee.Checked = _autoEscapeType == AutoEscapeType.Flee;
            tsmiAutoEscapeHazy.Checked = _autoEscapeType == AutoEscapeType.Hazy;

            tsmiAutoEscapeOnByDefault.Checked = _autoEscapeOnByDefault;
            tsmiAutoEscapeOffByDefault.Checked = !_autoEscapeOnByDefault;
        }

        private void ctxAutoEscape_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool haveThreshold = _autoEscapeThreshold > 0;
            tsmiClearAutoEscapeThreshold.Enabled = haveThreshold;
            tsmiAutoEscapeOnByDefault.Enabled = haveThreshold;
            tsmiAutoEscapeOffByDefault.Enabled = haveThreshold;
            tsmiAutoEscapeRestoreOriginalValue.Enabled = _autoEscapeOnByDefault != _autoEscapeOnByDefaultOriginal || _autoEscapeThreshold != _autoEscapeThresholdOriginal || _autoEscapeType != _autoEscapeTypeOriginal;
        }

        private void tsmiAutoEscapeFlee_Click(object sender, EventArgs e)
        {
            _autoEscapeType = AutoEscapeType.Flee;
            RefreshAutoEscapeUI();
        }

        private void tsmiAutoEscapeHazy_Click(object sender, EventArgs e)
        {
            _autoEscapeType = AutoEscapeType.Hazy;
            RefreshAutoEscapeUI();
        }

        private void tsmiAutoEscapeOnByDefault_Click(object sender, EventArgs e)
        {
            _autoEscapeOnByDefault = true;
            RefreshAutoEscapeUI();
        }

        private void tsmiAutoEscapeOffByDefault_Click(object sender, EventArgs e)
        {
            _autoEscapeOnByDefault = false;
            RefreshAutoEscapeUI();
        }

        private void tsmiAutoEscapeRestoreOriginalValue_Click(object sender, EventArgs e)
        {
            _autoEscapeOnByDefault = _autoEscapeOnByDefaultOriginal;
            _autoEscapeThreshold = _autoEscapeThresholdOriginal;
            _autoEscapeType = _autoEscapeTypeOriginal;
            RefreshAutoEscapeUI();
        }
    }

    public enum AutoEscapeType
    {
        Flee = 0,
        Hazy = 1,
    }
}
