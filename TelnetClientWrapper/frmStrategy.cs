using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace IsengardClient
{
    public partial class frmStrategy : Form
    {
        private int _currentAutoEscapeThreshold;
        private AutoEscapeType _currentAutoEscapeType;
        private AutoEscapeActivity _currentAutoEscapeActivity;

        private int _currentAutoSpellLevelMinimum;
        private int _currentAutoSpellLevelMaximum;

        public Strategy NewStrategy { get; set; }

        /// <summary>
        /// set defaults for a new strategy
        /// </summary>
        public frmStrategy()
        {
            InitializeComponent();

            cboOnKillMonster.SelectedIndex = (int)AfterKillMonsterAction.StopCombat;

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

            foreach (var nextFinalAction in Enum.GetValues(typeof(FinalStepAction)))
            {
                cboMagicFinalAction.Items.Add(nextFinalAction.ToString());
                cboMeleeFinalAction.Items.Add(nextFinalAction.ToString());
                cboPotionsFinalAction.Items.Add(nextFinalAction.ToString());
            }
        }

        public frmStrategy(Strategy s) : this()
        {
            txtName.Text = s.Name;
            chkAutogenerateName.Checked = s.AutogenerateName;

            cboOnKillMonster.SelectedIndex = (int)s.AfterKillMonsterAction;

            _currentAutoEscapeThreshold = s.AutoEscapeThreshold;
            _currentAutoEscapeActivity = s.AutoEscapeActivity;
            _currentAutoEscapeType = s.AutoEscapeType;
            RefreshAutoEscapeUI();

            _currentAutoSpellLevelMinimum = s.AutoSpellLevelMin;
            _currentAutoSpellLevelMaximum = s.AutoSpellLevelMax;
            RefreshAutoSpellLevelUI();

            if (s.ManaPool > 0) txtManaPool.Text = s.ManaPool.ToString();
            chkPromptForManaPool.Checked = s.PromptForManaPool;

            if (s.LastMagicStep.HasValue)
                cboMagicLastStep.SelectedItem = s.LastMagicStep.Value.ToString();
            else
                cboMagicLastStep.SelectedIndex = 0;

            if (s.LastMeleeStep.HasValue)
                cboMeleeLastStep.SelectedItem = s.LastMeleeStep.Value.ToString();
            else
                cboMeleeLastStep.SelectedIndex = 0;

            if (s.LastPotionsStep.HasValue)
                cboPotionsLastStep.SelectedItem = s.LastPotionsStep.Value.ToString();
            else
                cboPotionsLastStep.SelectedIndex = 0;

            chkMagicLastStepIndefinite.Checked = (s.TypesToRunLastCommandIndefinitely & CommandType.Magic) != CommandType.None;
            chkMeleeRepeatLastStepIndefinitely.Checked = (s.TypesToRunLastCommandIndefinitely & CommandType.Melee) != CommandType.None;
            chkPotionsRepeatLastStepIndefinitely.Checked = (s.TypesToRunLastCommandIndefinitely & CommandType.Potions) != CommandType.None;

            cboMagicFinalAction.SelectedItem = s.FinalMagicAction.ToString();
            cboMeleeFinalAction.SelectedItem = s.FinalMeleeAction.ToString();
            cboPotionsFinalAction.SelectedItem = s.FinalPotionsAction.ToString();

            chkMagicOnlyWhenStunned.Checked = (s.TypesToRunOnlyWhenMonsterStunned & CommandType.Magic) != CommandType.None;
            chkMeleeOnlyWhenStunned.Checked = (s.TypesToRunOnlyWhenMonsterStunned & CommandType.Melee) != CommandType.None;
            chkPotionsOnlyWhenStunned.Checked = (s.TypesToRunOnlyWhenMonsterStunned & CommandType.Potions) != CommandType.None;

            chkMagicEnabled.Checked = (s.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            chkMeleeEnabled.Checked = (s.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            chkPotionsEnabled.Checked = (s.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;

            if (s.MagicVigorOnlyWhenDownXHP > 0) txtMagicVigorWhenDownXHP.Text = s.MagicVigorOnlyWhenDownXHP.ToString();
            if (s.MagicMendOnlyWhenDownXHP > 0) txtMagicMendWhenDownXHP.Text = s.MagicMendOnlyWhenDownXHP.ToString();
            if (s.PotionsVigorOnlyWhenDownXHP > 0) txtPotionsVigorWhenDownXHP.Text = s.PotionsVigorOnlyWhenDownXHP.ToString();
            if (s.PotionsMendOnlyWhenDownXHP > 0) txtPotionsMendWhenDownXHP.Text = s.PotionsMendOnlyWhenDownXHP.ToString();

            if (s.MagicSteps != null)
            {
                foreach (AMagicStrategyStep nextStep in s.MagicSteps)
                {
                    lstMagicSteps.Items.Add(nextStep);
                }
            }
            if (s.MeleeSteps != null)
            {
                foreach (AMeleeStrategyStep nextStep in s.MeleeSteps)
                {
                    lstMeleeSteps.Items.Add(nextStep);
                }
            }
            if (s.PotionsSteps != null)
            {
                foreach (APotionsStrategyStep nextStep in s.PotionsSteps)
                {
                    lstPotionsSteps.Items.Add(nextStep);
                }
            }
        }

        private void chkAutogenerateName_CheckedChanged(object sender, EventArgs e)
        {
            txtName.Enabled = !chkAutogenerateName.Checked;
            if (chkAutogenerateName.Checked)
            {
                txtName.Text = string.Empty;
            }
        }

        private void RefreshAutoSpellLevelUI()
        {
            if (_currentAutoSpellLevelMinimum == -1 && _currentAutoSpellLevelMaximum == -1)
            {
                lblAutoSpellLevels.Text = "Inherit";
            }
            else
            {
                lblAutoSpellLevels.Text = _currentAutoSpellLevelMinimum + ":" + _currentAutoSpellLevelMaximum;
            }
        }

        private void RefreshAutoEscapeUI()
        {
            Color autoEscapeBackColor;
            string autoEscapeText;
            if (_currentAutoEscapeActivity == AutoEscapeActivity.Inherit)
            {
                autoEscapeBackColor = Color.Black;
                autoEscapeText = "Inherit Auto Escape";
            }
            else if (_currentAutoEscapeActivity == AutoEscapeActivity.Active)
            {
                string sAutoEscapeType = _currentAutoEscapeType == AutoEscapeType.Hazy ? "Hazy" : "Flee";
                if (_currentAutoEscapeThreshold > 0)
                {
                    autoEscapeText = sAutoEscapeType + " @ " + _currentAutoEscapeThreshold.ToString();
                }
                else
                {
                    autoEscapeText = sAutoEscapeType;
                }
                if (_currentAutoEscapeType == AutoEscapeType.Hazy)
                {
                    autoEscapeBackColor = Color.DarkBlue;
                }
                else //Flee
                {
                    autoEscapeBackColor = Color.DarkRed;
                }
            }
            else //inactive
            {
                autoEscapeBackColor = Color.Black;
                autoEscapeText = "No Auto Escape";
            }
            UIShared.GetForegroundColor(autoEscapeBackColor.R, autoEscapeBackColor.G, autoEscapeBackColor.G, out byte forer, out byte foreg, out byte foreb);
            lblAutoEscapeValue.BackColor = autoEscapeBackColor;
            lblAutoEscapeValue.ForeColor = Color.FromArgb(forer, foreg, foreb);
            lblAutoEscapeValue.Text = autoEscapeText;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string sInt = txtManaPool.Text;
            if (!string.IsNullOrEmpty(sInt) && !int.TryParse(sInt, out _))
            {
                MessageBox.Show("Invalid mana pool", "Strategy");
                txtManaPool.Focus();
            }
            sInt = txtMagicVigorWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt) && !int.TryParse(sInt, out _))
            {
                MessageBox.Show("Invalid magic vigor when down X HP", "Strategy");
                txtMagicVigorWhenDownXHP.Focus();
            }
            sInt = txtMagicMendWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt) && !int.TryParse(sInt, out _))
            {
                MessageBox.Show("Invalid magic mend when down X HP", "Strategy");
                txtMagicMendWhenDownXHP.Focus();
            }
            sInt = txtPotionsVigorWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt) && !int.TryParse(sInt, out _))
            {
                MessageBox.Show("Invalid potions vigor when down X HP", "Strategy");
                txtPotionsVigorWhenDownXHP.Focus();
            }
            sInt = txtPotionsMendWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt) && !int.TryParse(sInt, out _))
            {
                MessageBox.Show("Invalid potions mend when down X HP", "Strategy");
                txtPotionsMendWhenDownXHP.Focus();
            }
            if (!chkAutogenerateName.Checked && string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("No name specified", "Strategy");
                txtName.Focus();
            }

            NewStrategy = new Strategy();
            if (lstMagicSteps.Items.Count > 0)
            {
                NewStrategy.MagicSteps = new List<AMagicStrategyStep>();
                foreach (AMagicStrategyStep nextStep in lstMagicSteps.Items)
                {
                    NewStrategy.MagicSteps.Add(nextStep);
                }
            }
            if (lstMeleeSteps.Items.Count > 0)
            {
                NewStrategy.MeleeSteps = new List<AMeleeStrategyStep>();
                foreach (AMeleeStrategyStep nextStep in lstMeleeSteps.Items)
                {
                    NewStrategy.MeleeSteps.Add(nextStep);
                }
            }
            if (lstPotionsSteps.Items.Count > 0)
            {
                NewStrategy.PotionsSteps = new List<APotionsStrategyStep>();
                foreach (APotionsStrategyStep nextStep in lstPotionsSteps.Items)
                {
                    NewStrategy.PotionsSteps.Add(nextStep);
                }
            }

            NewStrategy.AutogenerateName = chkAutogenerateName.Checked;
            if (!NewStrategy.AutogenerateName) NewStrategy.Name = txtName.Text;

            NewStrategy.AfterKillMonsterAction = (AfterKillMonsterAction)cboOnKillMonster.SelectedIndex;

            NewStrategy.AutoEscapeActivity = _currentAutoEscapeActivity;
            NewStrategy.AutoEscapeThreshold = _currentAutoEscapeThreshold;
            NewStrategy.AutoEscapeType = _currentAutoEscapeType;

            NewStrategy.AutoSpellLevelMin = _currentAutoSpellLevelMinimum;
            NewStrategy.AutoSpellLevelMax = _currentAutoSpellLevelMaximum;

            NewStrategy.PromptForManaPool = chkPromptForManaPool.Checked;

            sInt = txtManaPool.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.ManaPool = int.Parse(sInt);
            sInt = txtMagicVigorWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.MagicVigorOnlyWhenDownXHP = int.Parse(sInt);
            sInt = txtMagicMendWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.MagicMendOnlyWhenDownXHP = int.Parse(sInt);
            sInt = txtPotionsVigorWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.PotionsVigorOnlyWhenDownXHP = int.Parse(sInt);
            sInt = txtPotionsMendWhenDownXHP.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.PotionsMendOnlyWhenDownXHP = int.Parse(sInt);

            if (cboMagicLastStep.SelectedIndex > 0)
            {
                NewStrategy.LastMagicStep = (MagicStrategyStep)Enum.Parse(typeof(MagicStrategyStep), cboMagicLastStep.SelectedItem.ToString());
            }
            if (cboMeleeLastStep.SelectedIndex > 0)
            {
                NewStrategy.LastMeleeStep = (MeleeStrategyStep)Enum.Parse(typeof(MeleeStrategyStep), cboMeleeLastStep.SelectedItem.ToString());
            }
            if (cboPotionsLastStep.SelectedIndex > 0)
            {
                NewStrategy.LastPotionsStep = (PotionsStrategyStep)Enum.Parse(typeof(PotionsStrategyStep), cboPotionsLastStep.SelectedItem.ToString());
            }

            CommandType ct;

            ct = CommandType.None;
            if (chkMagicLastStepIndefinite.Checked) ct |= CommandType.Magic;
            if (chkMeleeRepeatLastStepIndefinitely.Checked) ct |= CommandType.Melee;
            if (chkPotionsRepeatLastStepIndefinitely.Checked) ct |= CommandType.Potions;
            NewStrategy.TypesToRunLastCommandIndefinitely = ct;

            ct = CommandType.None;
            if (chkMagicOnlyWhenStunned.Checked) ct |= CommandType.Magic;
            if (chkMeleeOnlyWhenStunned.Checked) ct |= CommandType.Melee;
            if (chkPotionsOnlyWhenStunned.Checked) ct |= CommandType.Potions;
            NewStrategy.TypesToRunOnlyWhenMonsterStunned = ct;

            ct = CommandType.None;
            if (chkMagicEnabled.Checked) ct |= CommandType.Magic;
            if (chkMeleeEnabled.Checked) ct |= CommandType.Melee;
            if (chkPotionsEnabled.Checked) ct |= CommandType.Potions;
            NewStrategy.TypesWithStepsEnabled = ct;

            NewStrategy.FinalMagicAction = (FinalStepAction)cboMagicFinalAction.SelectedIndex;
            NewStrategy.FinalMeleeAction = (FinalStepAction)cboMeleeFinalAction.SelectedIndex;
            NewStrategy.FinalPotionsAction = (FinalStepAction)cboPotionsFinalAction.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ctxAutoSpellLevels_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tsmiInheritAutoSpellLevels.Checked = _currentAutoSpellLevelMaximum == -1 && _currentAutoSpellLevelMinimum == -1;
        }

        private void tsmiSetCurrentMinimumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string sStart = _currentAutoSpellLevelMinimum == -1 ? string.Empty : _currentAutoSpellLevelMinimum.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMinimum = iLevel;
                if (_currentAutoSpellLevelMaximum == -1)
                {
                    _currentAutoSpellLevelMaximum = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
                }
                else if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMaximum = _currentAutoSpellLevelMinimum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetCurrentMaximumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string sStart = _currentAutoSpellLevelMaximum == -1 ? string.Empty : _currentAutoSpellLevelMaximum.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMaximum = iLevel;
                if (_currentAutoSpellLevelMinimum == -1)
                {
                    _currentAutoSpellLevelMinimum = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
                }
                else if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMinimum = _currentAutoSpellLevelMaximum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiInheritAutoSpellLevels_Click(object sender, EventArgs e)
        {
            if (_currentAutoSpellLevelMinimum == -1 && _currentAutoSpellLevelMaximum == -1)
            {
                _currentAutoSpellLevelMaximum = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
                _currentAutoSpellLevelMinimum = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
            }
            else
            {
                _currentAutoSpellLevelMinimum = -1;
                _currentAutoSpellLevelMaximum = -1;
            }
            RefreshAutoSpellLevelUI();
        }
    }
}
