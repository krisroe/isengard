using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IsengardClient
{
    internal partial class frmConfiguration : Form
    {
        private List<Strategy> _strategies;

        private int _defaultAutoEscapeThreshold;
        private AutoEscapeType _defaultAutoEscapeType;
        private bool _defaultAutoEscapeActive;
        private int _defaultAutoEscapeThresholdOriginal;
        private AutoEscapeType _defaultAutoEscapeTypeOriginal;
        private bool _defaultAutoEscapeActiveOriginal;
        private int _currentAutoEscapeThreshold;
        private AutoEscapeType _currentAutoEscapeType;
        private bool _currentAutoEscapeActive;
        private int _currentAutoEscapeThresholdOriginal;
        private AutoEscapeType _currentAutoEscapeTypeOriginal;
        private bool _currentAutoEscapeActiveOriginal;

        private AlignmentType _preferredAlignment;
        private Color _fullColor;
        private Color _emptyColor;

        private string _defaultRealm;
        private string _defaultRealmOriginal;
        private string _currentRealm;
        private string _currentRealmOriginal;

        private int _defaultAutoSpellLevelMinimum;
        private int _defaultAutoSpellLevelMaximum;
        private int _defaultAutoSpellLevelMinimumOriginal;
        private int _defaultAutoSpellLevelMaximumOriginal;
        private int _currentAutoSpellLevelMinimum;
        private int _currentAutoSpellLevelMaximum;
        private int _currentAutoSpellLevelMinimumOriginal;
        private int _currentAutoSpellLevelMaximumOriginal;
        internal const int AUTO_SPELL_LEVEL_MINIMUM = 1;
        internal const int AUTO_SPELL_LEVEL_MAXIMUM = 3;

        private string _defaultWeaponOriginal;
        private string _currentWeaponOriginal;

        public frmConfiguration(string currentRealm, int currentAutoSpellLevelMin, int currentAutoSpellLevelMax, string weapon, int autoEscapeThreshold, AutoEscapeType autoEscapeType, bool autoEscapeActive, List<Strategy> strategies)
        {
            InitializeComponent();

            IsengardSettings sets = IsengardSettings.Default;

            _currentRealmOriginal = _currentRealm = currentRealm;
            _defaultRealm = IsengardSettings.Default.DefaultRealm;
            _defaultRealmOriginal = _defaultRealm;
            RefreshRealmUI();

            _currentAutoSpellLevelMaximum = _currentAutoSpellLevelMaximumOriginal = currentAutoSpellLevelMax;
            _currentAutoSpellLevelMinimum = _currentAutoSpellLevelMinimumOriginal = currentAutoSpellLevelMin;
            _defaultAutoSpellLevelMinimum = sets.DefaultAutoSpellLevelMin;
            _defaultAutoSpellLevelMaximum = sets.DefaultAutoSpellLevelMax;
            _defaultAutoSpellLevelMaximumOriginal = _defaultAutoSpellLevelMaximum;
            _defaultAutoSpellLevelMinimumOriginal = _defaultAutoSpellLevelMinimum;
            RefreshAutoSpellLevelUI();

            txtCurrentWeaponValue.Text = _currentWeaponOriginal = weapon;
            txtDefaultWeaponValue.Text = _defaultWeaponOriginal = sets.DefaultWeapon;

            _preferredAlignment = ParseAlignment(sets.PreferredAlignment);
            RefreshAlignmentTypeUI();

            _defaultAutoEscapeType = (AutoEscapeType)sets.DefaultAutoEscapeType;
            _defaultAutoEscapeThreshold = sets.DefaultAutoEscapeThreshold;
            _defaultAutoEscapeActive = sets.DefaultAutoEscapeOnByDefault;
            if (_defaultAutoEscapeType != AutoEscapeType.Flee && _defaultAutoEscapeType != AutoEscapeType.Hazy) _defaultAutoEscapeType = AutoEscapeType.Flee;
            if (_defaultAutoEscapeThreshold < 0) _defaultAutoEscapeThreshold = 0;
            if (_defaultAutoEscapeThreshold == 0) _defaultAutoEscapeActive = false;
            _defaultAutoEscapeActiveOriginal = _defaultAutoEscapeActive;
            _defaultAutoEscapeThresholdOriginal = _defaultAutoEscapeThreshold;
            _defaultAutoEscapeTypeOriginal = _defaultAutoEscapeType;
            _currentAutoEscapeActive = _currentAutoEscapeActiveOriginal = CurrentAutoEscapeActive;
            _currentAutoEscapeThreshold = _currentAutoEscapeThresholdOriginal = CurrentAutoEscapeThreshold;
            _currentAutoEscapeType = _currentAutoEscapeTypeOriginal = CurrentAutoEscapeType;
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            IsengardSettings sets = IsengardSettings.Default;
            sets.DefaultWeapon = txtDefaultWeaponValue.Text;
            sets.DefaultRealm = _defaultRealm;
            sets.PreferredAlignment = _preferredAlignment.ToString();
            sets.DefaultAutoEscapeOnByDefault = _defaultAutoEscapeActive;
            sets.DefaultAutoEscapeThreshold = _defaultAutoEscapeThreshold;
            sets.DefaultAutoEscapeType = Convert.ToInt32(_defaultAutoEscapeType);
            sets.QueryMonsterStatus = chkQueryMonsterStatus.Checked;
            sets.VerboseMode = chkVerboseOutput.Checked;
            sets.FullColor = _fullColor;
            sets.EmptyColor = _emptyColor;
            sets.DefaultAutoSpellLevelMin = _defaultAutoSpellLevelMinimum;
            sets.DefaultAutoSpellLevelMax = _defaultAutoSpellLevelMaximum;
            IsengardSettings.Default.Save();

            //CSRTODO: save changes to strategies

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region preferred alignment

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
            tsmiPreferredAlignmentGood.Checked = _preferredAlignment == AlignmentType.Blue;
            tsmiPreferredAlignmentEvil.Checked = _preferredAlignment == AlignmentType.Red;
        }

        private void ctxPreferredAlignment_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tsmiPreferredAlignmentRestoreOriginalValue.Enabled = _preferredAlignment != (AlignmentType)Enum.Parse(typeof(AlignmentType), IsengardSettings.Default.PreferredAlignment);
        }

        private void tsmiPreferredAlignmentRestoreOriginalValue_Click(object sender, EventArgs e)
        {
            _preferredAlignment = (AlignmentType)Enum.Parse(typeof(AlignmentType), IsengardSettings.Default.PreferredAlignment);
            RefreshAlignmentTypeUI();
        }

        private static AlignmentType ParseAlignment(string alignment)
        {
            if (!Enum.TryParse(alignment, out AlignmentType at))
            {
                at = AlignmentType.Blue;
            }
            return at;
        }

        private void tsmiPreferredAlignmentGood_Click(object sender, EventArgs e)
        {
            _preferredAlignment = AlignmentType.Blue;
            RefreshAlignmentTypeUI();
        }

        private void tsmiPreferredAlignmentEvil_Click(object sender, EventArgs e)
        {
            _preferredAlignment = AlignmentType.Red;
            RefreshAlignmentTypeUI();
        }

        #endregion

        #region current/default realm

        public string CurrentRealm
        {
            get
            {
                return _currentRealm;
            }
        }

        private void RefreshRealmUI()
        {
            lblDefaultRealmValue.Text = _defaultRealm;
            lblDefaultRealmValue.BackColor = UIShared.GetColorForRealm(_defaultRealm);
            lblCurrentRealmValue.Text = _currentRealm;
            lblCurrentRealmValue.BackColor = UIShared.GetColorForRealm(_currentRealm);
        }

        private void tsmiDefaultRealm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            _defaultRealm = tsmi.Tag.ToString();
            RefreshRealmUI();
        }

        private void tsmiCurrentRealm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            _currentRealm = tsmi.Tag.ToString();
            RefreshRealmUI();
        }

        private void ctxRealm_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (ToolStripMenuItem tsmi in new List<ToolStripMenuItem>() { tsmiCurrentRealmEarth, tsmiCurrentRealmFire, tsmiCurrentRealmWater, tsmiCurrentRealmWind })
            {
                string sTag = tsmi.Tag.ToString();
                bool isSelected = sTag == _currentRealm;
                tsmi.Checked = isSelected;
                tsmi.Text = isSelected ? sTag + " (Current)" : sTag;
            }
            foreach (ToolStripMenuItem tsmi in new List<ToolStripMenuItem>() { tsmiDefaultRealmEarth, tsmiDefaultRealmFire, tsmiDefaultRealmWater, tsmiDefaultRealmWind })
            {
                string sTag = tsmi.Tag.ToString();
                bool isSelected = sTag == _defaultRealm;
                tsmi.Checked = isSelected;
                tsmi.Text = isSelected ? sTag + " (Default)" : sTag;
            }
            tsmiSetCurrentRealmAsDefault.Enabled = tsmiSetDefaultRealmAsCurrent.Enabled = _defaultRealm != _currentRealm;
            tsmiRestoreCurrentRealm.Enabled = _currentRealm != _currentRealmOriginal;
            tsmiRestoreDefaultRealm.Enabled = _defaultRealm != _defaultRealmOriginal;
        }

        private void tsmiSetCurrentRealmAsDefault_Click(object sender, EventArgs e)
        {
            _defaultRealm = _currentRealm;
            RefreshRealmUI();
        }

        private void tsmiSetDefaultRealmAsCurrent_Click(object sender, EventArgs e)
        {
            _currentRealm = _defaultRealm;
            RefreshRealmUI();
        }

        private void tsmiRestoreDefaultRealm_Click(object sender, EventArgs e)
        {
            _defaultRealm = _defaultRealmOriginal;
            RefreshRealmUI();
        }

        private void tsmiRestoreCurrentRealm_Click(object sender, EventArgs e)
        {
            _currentRealm = _currentRealmOriginal;
            RefreshRealmUI();
        }

        #endregion

        #region auto spell levels

        public int CurrentAutoSpellLevelMin
        {
            get
            {
                return _currentAutoSpellLevelMinimum;
            }
        }

        public int CurrentAutoSpellLevelMax
        {
            get
            {
                return _currentAutoSpellLevelMaximum;
            }
        }

        private void RefreshAutoSpellLevelUI()
        {
            lblDefaultAutoSpellLevelsValue.Text = _defaultAutoSpellLevelMinimum + ":" + _defaultAutoSpellLevelMaximum;
            lblCurrentAutoSpellLevelsValue.Text = _currentAutoSpellLevelMinimum + ":" + _currentAutoSpellLevelMaximum;
        }

        private void tsmiSetCurrentMinimumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _currentAutoSpellLevelMinimum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMinimum = iLevel;
                if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMaximum = _currentAutoSpellLevelMinimum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetCurrentMaximumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _currentAutoSpellLevelMaximum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMaximum = iLevel;
                if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMinimum = _currentAutoSpellLevelMaximum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetDefaultMinimumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _defaultAutoSpellLevelMinimum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _defaultAutoSpellLevelMinimum = iLevel;
                if (_defaultAutoSpellLevelMaximum < _defaultAutoSpellLevelMinimum)
                {
                    _defaultAutoSpellLevelMaximum = _defaultAutoSpellLevelMinimum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetDefaultMaximumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _defaultAutoSpellLevelMaximum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _defaultAutoSpellLevelMaximum = iLevel;
                if (_defaultAutoSpellLevelMaximum < _defaultAutoSpellLevelMinimum)
                {
                    _defaultAutoSpellLevelMinimum = _defaultAutoSpellLevelMaximum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void ctxAutoSpellLevels_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tsmiSetCurrentAutoSpellLevelsAsDefault.Enabled = tsmiSetDefaultAutoSpellLevelsAsCurrent.Enabled = _defaultAutoSpellLevelMaximum != _currentAutoSpellLevelMaximum || _defaultAutoSpellLevelMinimum != _currentAutoSpellLevelMinimum;
            tsmiRestoreDefaultAutoSpellLevels.Enabled = _defaultAutoSpellLevelMaximum != _defaultAutoSpellLevelMaximumOriginal || _defaultAutoSpellLevelMinimum != _defaultAutoSpellLevelMinimumOriginal;
            tsmiRestoreCurrentAutoSpellLevels.Enabled = _currentAutoSpellLevelMaximum != _currentAutoSpellLevelMaximumOriginal || _currentAutoSpellLevelMinimum != _currentAutoSpellLevelMinimumOriginal;
        }

        private void tsmiSetCurrentAutoSpellLevelsAsDefault_Click(object sender, EventArgs e)
        {
            _defaultAutoSpellLevelMaximum = _currentAutoSpellLevelMaximum;
            _defaultAutoSpellLevelMinimum = _currentAutoSpellLevelMinimum;
            RefreshAutoSpellLevelUI();
        }

        private void tsmiSetDefaultAutoSpellLevelsAsCurrent_Click(object sender, EventArgs e)
        {
            _currentAutoSpellLevelMaximum = _defaultAutoSpellLevelMaximum;
            _currentAutoSpellLevelMinimum = _defaultAutoSpellLevelMinimum;
            RefreshAutoSpellLevelUI();
        }

        private void tsmiRestoreCurrentAutoSpellLevels_Click(object sender, EventArgs e)
        {
            _currentAutoSpellLevelMaximum = _currentAutoSpellLevelMaximumOriginal;
            _currentAutoSpellLevelMinimum = _currentAutoSpellLevelMinimumOriginal;
            RefreshAutoSpellLevelUI();
        }

        private void tsmiRestoreDefaultAutoSpellLevels_Click(object sender, EventArgs e)
        {
            _defaultAutoSpellLevelMaximum = _defaultAutoSpellLevelMaximumOriginal;
            _defaultAutoSpellLevelMinimum = _defaultAutoSpellLevelMinimumOriginal;
            RefreshAutoSpellLevelUI();
        }

        #endregion

        #region auto escape

        public int CurrentAutoEscapeThreshold
        {
            get
            {
                return _currentAutoEscapeThreshold;
            }
        }

        public AutoEscapeType CurrentAutoEscapeType
        {
            get
            {
                return _currentAutoEscapeType;
            }
        }

        public bool CurrentAutoEscapeActive
        {
            get
            {
                return _currentAutoEscapeActive;
            }
        }

        private void tsmiClearDefaultAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            _defaultAutoEscapeThreshold = 0;
            _defaultAutoEscapeActive = false;
            RefreshAutoEscapeUI();
        }

        private void tsmiSetDefaultAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            string sInitialValue = _defaultAutoEscapeThreshold > 0 ? _defaultAutoEscapeThreshold.ToString() : string.Empty;
            string threshold = Interaction.InputBox("Threshold:", "Enter Threshold", sInitialValue);
            if (int.TryParse(threshold, out int iThreshold) && iThreshold > 0)
            {
                _defaultAutoEscapeThreshold = iThreshold;
                RefreshAutoEscapeUI();
            }
        }

        private void RefreshAutoEscapeUI()
        {
            SetAutoEscapeLabel(lblDefaultAutoEscapeValue, _defaultAutoEscapeType, _defaultAutoEscapeThreshold, _defaultAutoEscapeActive);
            SetAutoEscapeLabel(lblCurrentAutoEscapeValue, _currentAutoEscapeType, _currentAutoEscapeThreshold, _currentAutoEscapeActive);
        }

        private static void SetAutoEscapeLabel(Label lbl, AutoEscapeType autoEscapeType, int autoEscapeThreshold, bool autoEscapeActive)
        {
            Color autoEscapeBackColor;
            string autoEscapeText;
            string sAutoEscapeType = autoEscapeType == AutoEscapeType.Hazy ? "Hazy" : "Flee";
            if (autoEscapeThreshold > 0)
            {
                autoEscapeText = sAutoEscapeType + " @ " + autoEscapeThreshold.ToString();
            }
            else
            {
                autoEscapeText = sAutoEscapeType;
            }
            if (autoEscapeActive)
            {
                if (autoEscapeType == AutoEscapeType.Hazy)
                {
                    autoEscapeBackColor = Color.DarkBlue;
                }
                else //Flee
                {
                    autoEscapeBackColor = Color.DarkRed;
                }
            }
            else if (autoEscapeThreshold > 0)
            {
                autoEscapeBackColor = Color.LightGray;
            }
            else
            {
                autoEscapeBackColor = Color.Black;
            }
            if (autoEscapeActive)
            {
                autoEscapeText += " (On)";
            }
            else
            {
                autoEscapeText += " (Off)";
            }

            UIShared.GetForegroundColor(autoEscapeBackColor.R, autoEscapeBackColor.G, autoEscapeBackColor.G, out byte forer, out byte foreg, out byte foreb);
            lbl.BackColor = autoEscapeBackColor;
            lbl.ForeColor = Color.FromArgb(forer, foreg, foreb);
            lbl.Text = autoEscapeText;
        }

        private void ctxAutoEscape_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tsmiDefaultAutoEscapeFlee.Checked = _defaultAutoEscapeType == AutoEscapeType.Flee;
            tsmiDefaultAutoEscapeHazy.Checked = _defaultAutoEscapeType == AutoEscapeType.Hazy;
            tsmiDefaultAutoEscapeActive.Checked = _defaultAutoEscapeActive;
            tsmiDefaultAutoEscapeInactive.Checked = !_defaultAutoEscapeActive;

            tsmiCurrentAutoEscapeFlee.Checked = _currentAutoEscapeType == AutoEscapeType.Flee;
            tsmiCurrentAutoEscapeHazy.Checked = _currentAutoEscapeType == AutoEscapeType.Hazy;
            tsmiCurrentAutoEscapeActive.Checked = _currentAutoEscapeActive;
            tsmiCurrentAutoEscapeInactive.Checked = !_currentAutoEscapeActive;

            bool haveDefaultThreshold = _defaultAutoEscapeThreshold > 0;
            tsmiClearDefaultAutoEscapeThreshold.Enabled = haveDefaultThreshold;
            tsmiDefaultAutoEscapeActive.Enabled = haveDefaultThreshold;
            tsmiDefaultAutoEscapeInactive.Enabled = haveDefaultThreshold;

            bool haveCurrentThreshold = _currentAutoEscapeThreshold > 0;
            tsmiClearCurrentAutoEscapeThreshold.Enabled = haveCurrentThreshold;
            tsmiCurrentAutoEscapeActive.Enabled = haveCurrentThreshold;
            tsmiCurrentAutoEscapeInactive.Enabled = haveCurrentThreshold;

            bool currentAndDefaultDiffer = _defaultAutoEscapeActive != _currentAutoEscapeActive || _defaultAutoEscapeThreshold != _currentAutoEscapeThreshold || _defaultAutoEscapeType != _currentAutoEscapeType;
            tsmiSetCurrentAutoEscapeAsDefault.Enabled = tsmiSetDefaultAutoEscapeAsCurrent.Enabled = currentAndDefaultDiffer;
            tsmiRestoreDefaultAutoEscape.Enabled = _defaultAutoEscapeActive != _defaultAutoEscapeActiveOriginal || _defaultAutoEscapeThreshold != _defaultAutoEscapeThresholdOriginal || _defaultAutoEscapeType != _defaultAutoEscapeTypeOriginal;
            tsmiRestoreCurrentAutoEscape.Enabled = _currentAutoEscapeActive != _currentAutoEscapeActiveOriginal || _currentAutoEscapeThreshold != _currentAutoEscapeThresholdOriginal || _currentAutoEscapeType != _currentAutoEscapeTypeOriginal;
        }

        private void tsmiDefaultAutoEscapeFlee_Click(object sender, EventArgs e)
        {
            _defaultAutoEscapeType = AutoEscapeType.Flee;
            RefreshAutoEscapeUI();
        }

        private void tsmiDefaultAutoEscapeHazy_Click(object sender, EventArgs e)
        {
            _defaultAutoEscapeType = AutoEscapeType.Hazy;
            RefreshAutoEscapeUI();
        }

        private void tsmiDefaultAutoEscapeActive_Click(object sender, EventArgs e)
        {
            _defaultAutoEscapeActive = true;
            RefreshAutoEscapeUI();
        }

        private void tsmiDefaultAutoEscapeInactive_Click(object sender, EventArgs e)
        {
            _defaultAutoEscapeActive = false;
            RefreshAutoEscapeUI();
        }

        private void tsmiSetCurrentAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            string sInitialValue = _currentAutoEscapeThreshold > 0 ? _currentAutoEscapeThreshold.ToString() : string.Empty;
            string threshold = Interaction.InputBox("Threshold:", "Enter Threshold", sInitialValue);
            if (int.TryParse(threshold, out int iThreshold) && iThreshold > 0)
            {
                _currentAutoEscapeThreshold = iThreshold;
                RefreshAutoEscapeUI();
            }
        }

        private void tsmiClearCurrentAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeThreshold = 0;
            _currentAutoEscapeActive = false;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeFlee_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeType = AutoEscapeType.Flee;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeHazy_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeType = AutoEscapeType.Hazy;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeActive_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeActive = true;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeInactive_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeActive = false;
            RefreshAutoEscapeUI();
        }

        private void tsmiSetCurrentAutoEscapeAsDefault_Click(object sender, EventArgs e)
        {
            _defaultAutoEscapeActive = _currentAutoEscapeActive;
            _defaultAutoEscapeThreshold = _currentAutoEscapeThreshold;
            _defaultAutoEscapeType = _currentAutoEscapeType;
            RefreshAutoEscapeUI();
        }

        private void tsmiSetDefaultAutoEscapeAsCurrent_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeActive = _defaultAutoEscapeActive;
            _currentAutoEscapeThreshold = _defaultAutoEscapeThreshold;
            _currentAutoEscapeType = _defaultAutoEscapeType;
            RefreshAutoEscapeUI();
        }

        private void tsmiRestoreCurrentAutoEscape_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeActive = _currentAutoEscapeActiveOriginal;
            _currentAutoEscapeThreshold = _currentAutoEscapeThresholdOriginal;
            _currentAutoEscapeType = _currentAutoEscapeTypeOriginal;
            RefreshAutoEscapeUI();
        }

        private void tsmiRestoreDefaultAutoEscape_Click(object sender, EventArgs e)
        {
            _defaultAutoEscapeActive = _defaultAutoEscapeActiveOriginal;
            _defaultAutoEscapeThreshold = _defaultAutoEscapeThresholdOriginal;
            _defaultAutoEscapeType = _defaultAutoEscapeTypeOriginal;
            RefreshAutoEscapeUI();
        }

        #endregion

        #region weapon

        public string CurrentWeapon
        {
            get
            {
                return txtCurrentWeaponValue.Text ?? string.Empty;
            }
        }

        private void ctxWeapon_Opening(object sender, CancelEventArgs e)
        {
            tsmiSetCurrentWeaponAsDefault.Enabled = tsmiSetDefaultWeaponAsCurrent.Enabled = !string.Equals(txtCurrentWeaponValue.Text, txtDefaultWeaponValue.Text);
            tsmiRestoreDefaultWeapon.Enabled = !string.Equals(txtDefaultWeaponValue.Text, _defaultWeaponOriginal);
            tsmiRestoreCurrentWeapon.Enabled = !string.Equals(txtCurrentWeaponValue.Text, _currentWeaponOriginal);
        }

        private void tsmiSetCurrentWeaponAsDefault_Click(object sender, EventArgs e)
        {
            txtDefaultWeaponValue.Text = txtCurrentWeaponValue.Text;
        }

        private void tsmiSetDefaultWeaponAsCurrent_Click(object sender, EventArgs e)
        {
            txtCurrentWeaponValue.Text = txtDefaultWeaponValue.Text;
        }

        private void tsmiRestoreCurrentWeapon_Click(object sender, EventArgs e)
        {
            txtCurrentWeaponValue.Text = _currentWeaponOriginal;
        }

        private void tsmiRestoreDefaultWeapon_Click(object sender, EventArgs e)
        {
            txtDefaultWeaponValue.Text = _defaultWeaponOriginal;
        }

        #endregion

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
    }
}
