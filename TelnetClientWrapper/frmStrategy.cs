using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmStrategy : Form
    {
        private int _currentAutoSpellLevelMinimum;
        private int _currentAutoSpellLevelMaximum;

        public Strategy NewStrategy { get; set; }

        /// <summary>
        /// set defaults for a new strategy
        /// </summary>
        public frmStrategy()
        {
            InitializeComponent();

            tsmiAddOffensiveAuto.Tag = MagicStrategyStep.OffensiveSpellAuto;
            tsmiAddOffensiveLevel1.Tag = MagicStrategyStep.OffensiveSpellLevel1;
            tsmiAddOffensiveLevel2.Tag = MagicStrategyStep.OffensiveSpellLevel2;
            tsmiAddOffensiveLevel3.Tag = MagicStrategyStep.OffensiveSpellLevel3;
            tsmiAddOffensiveLevel4.Tag = MagicStrategyStep.OffensiveSpellLevel4;
            tsmiAddOffensiveLevel5.Tag = MagicStrategyStep.OffensiveSpellLevel5;
            tsmiAddStun.Tag = MagicStrategyStep.Stun;
            tsmiMagicAddVigor.Tag = MagicStrategyStep.Vigor;
            tsmiMagicAddMendWounds.Tag = MagicStrategyStep.MendWounds;
            tsmiMagicAddGenericHeal.Tag = MagicStrategyStep.GenericHeal;
            tsmiMagicAddCurePoison.Tag = MagicStrategyStep.CurePoison;
            tsmiAddRegularAttack.Tag = MeleeStrategyStep.RegularAttack;
            tsmiAddPowerAttack.Tag = MeleeStrategyStep.PowerAttack;
            tsmiPotionsAddVigor.Tag = PotionsStrategyStep.Vigor;
            tsmiPotionsAddMendWounds.Tag = PotionsStrategyStep.MendWounds;
            tsmiPotionsAddGenericHeal.Tag = PotionsStrategyStep.GenericHeal;
            tsmiPotionsAddCurePoison.Tag = PotionsStrategyStep.CurePoison;

            cboOnKillMonster.SelectedIndex = (int)AfterKillMonsterAction.StopCombat;

            foreach (var nextFinalAction in Enum.GetValues(typeof(FinalStepAction)))
            {
                cboMagicFinalAction.Items.Add(nextFinalAction.ToString());
                cboMeleeFinalAction.Items.Add(nextFinalAction.ToString());
                cboPotionsFinalAction.Items.Add(nextFinalAction.ToString());
            }

            chkMagicLastStepIndefinite.Enabled = false;
            chkMeleeRepeatLastStepIndefinitely.Enabled = false;
            chkPotionsRepeatLastStepIndefinitely.Enabled = false;
        }

        public frmStrategy(Strategy s) : this()
        {
            txtName.Text = s.Name;
            chkAutogenerateName.Checked = s.AutogenerateName;

            cboOnKillMonster.SelectedIndex = (int)s.AfterKillMonsterAction;

            _currentAutoSpellLevelMinimum = s.AutoSpellLevelMin;
            _currentAutoSpellLevelMaximum = s.AutoSpellLevelMax;
            RefreshAutoSpellLevelUI();

            if (s.ManaPool > 0) txtManaPool.Text = s.ManaPool.ToString();

            cboMagicFinalAction.SelectedItem = s.FinalMagicAction.ToString();
            cboMeleeFinalAction.SelectedItem = s.FinalMeleeAction.ToString();
            cboPotionsFinalAction.SelectedItem = s.FinalPotionsAction.ToString();

            txtMagicOnlyWhenStunnedForXMS.Text = s.MagicOnlyWhenStunnedForXMS.HasValue ? s.MagicOnlyWhenStunnedForXMS.ToString() : string.Empty;
            txtMeleeOnlyWhenStunnedForXMS.Text = s.MeleeOnlyWhenStunnedForXMS.HasValue ? s.MeleeOnlyWhenStunnedForXMS.ToString() : string.Empty;
            txtPotionsOnlyWhenStunnedForXMS.Text = s.PotionsOnlyWhenStunnedForXMS.HasValue ? s.PotionsOnlyWhenStunnedForXMS.ToString() : string.Empty;

            chkMagicEnabled.Checked = (s.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            chkMeleeEnabled.Checked = (s.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            chkPotionsEnabled.Checked = (s.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;

            bool hasSteps;
            
            hasSteps = s.MagicSteps != null;
            if (hasSteps)
            {
                foreach (var nextStep in s.MagicSteps)
                {
                    lstMagicSteps.Items.Add(nextStep);
                }
            }
            chkMagicLastStepIndefinite.Checked = hasSteps && (s.TypesToRunLastCommandIndefinitely & CommandType.Magic) != CommandType.None;
            chkMagicLastStepIndefinite.Enabled = hasSteps;

            hasSteps = s.MeleeSteps != null;
            if (hasSteps)
            {
                foreach (var nextStep in s.MeleeSteps)
                {
                    lstMeleeSteps.Items.Add(nextStep);
                }
            }
            chkMeleeRepeatLastStepIndefinitely.Checked = hasSteps && (s.TypesToRunLastCommandIndefinitely & CommandType.Melee) != CommandType.None;
            chkMeleeRepeatLastStepIndefinitely.Enabled = hasSteps;

            hasSteps = s.PotionsSteps != null;
            if (hasSteps)
            {
                foreach (var nextStep in s.PotionsSteps)
                {
                    lstPotionsSteps.Items.Add(nextStep);
                }
            }
            chkPotionsRepeatLastStepIndefinitely.Checked = hasSteps && (s.TypesToRunLastCommandIndefinitely & CommandType.Potions) != CommandType.None;
            chkPotionsRepeatLastStepIndefinitely.Enabled = hasSteps;
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            string sInt = txtManaPool.Text;
            int iTemp;
            if (!string.IsNullOrEmpty(sInt) && !int.TryParse(sInt, out _))
            {
                MessageBox.Show("Invalid mana pool", "Strategy");
                txtManaPool.Focus();
            }
            sInt = txtMagicOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0))
            {
                MessageBox.Show("Invalid magic only when stunned for X MS", "Strategy");
                txtMagicOnlyWhenStunnedForXMS.Focus();
            }
            sInt = txtMeleeOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0))
            {
                MessageBox.Show("Invalid melee only when stunned for X MS", "Strategy");
                txtMeleeOnlyWhenStunnedForXMS.Focus();
            }
            sInt = txtPotionsOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0))
            {
                MessageBox.Show("Invalid potions only when stunned for X MS", "Strategy");
                txtPotionsOnlyWhenStunnedForXMS.Focus();
            }
            if (!chkAutogenerateName.Checked && string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("No name specified", "Strategy");
                txtName.Focus();
            }

            NewStrategy = new Strategy();
            if (lstMagicSteps.Items.Count > 0)
            {
                NewStrategy.MagicSteps = new List<MagicStrategyStep>();
                foreach (MagicStrategyStep nextStep in lstMagicSteps.Items)
                {
                    NewStrategy.MagicSteps.Add(nextStep);
                }
            }
            if (lstMeleeSteps.Items.Count > 0)
            {
                NewStrategy.MeleeSteps = new List<MeleeStrategyStep>();
                foreach (MeleeStrategyStep nextStep in lstMeleeSteps.Items)
                {
                    NewStrategy.MeleeSteps.Add(nextStep);
                }
            }
            if (lstPotionsSteps.Items.Count > 0)
            {
                NewStrategy.PotionsSteps = new List<PotionsStrategyStep>();
                foreach (PotionsStrategyStep nextStep in lstPotionsSteps.Items)
                {
                    NewStrategy.PotionsSteps.Add(nextStep);
                }
            }

            NewStrategy.AutogenerateName = chkAutogenerateName.Checked;
            if (!NewStrategy.AutogenerateName) NewStrategy.Name = txtName.Text;

            NewStrategy.AfterKillMonsterAction = (AfterKillMonsterAction)cboOnKillMonster.SelectedIndex;

            NewStrategy.AutoSpellLevelMin = _currentAutoSpellLevelMinimum;
            NewStrategy.AutoSpellLevelMax = _currentAutoSpellLevelMaximum;

            sInt = txtManaPool.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.ManaPool = int.Parse(sInt);

            CommandType ct;

            ct = CommandType.None;
            if (chkMagicLastStepIndefinite.Checked) ct |= CommandType.Magic;
            if (chkMeleeRepeatLastStepIndefinitely.Checked) ct |= CommandType.Melee;
            if (chkPotionsRepeatLastStepIndefinitely.Checked) ct |= CommandType.Potions;
            NewStrategy.TypesToRunLastCommandIndefinitely = ct;

            sInt = txtMagicOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.MagicOnlyWhenStunnedForXMS = int.Parse(sInt);
            sInt = txtMeleeOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.MeleeOnlyWhenStunnedForXMS = int.Parse(sInt);
            sInt = txtPotionsOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt)) NewStrategy.PotionsOnlyWhenStunnedForXMS = int.Parse(sInt);

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

        private void ctxMagicSteps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleStepsContextMenuOpening(lstMagicSteps, tsmiMagicRemove, tsmiMagicMoveUp, tsmiMagicMoveDown);
        }

        private void ctxMeleeSteps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleStepsContextMenuOpening(lstMeleeSteps, tsmiMeleeRemove, tsmiMeleeMoveUp, tsmiMeleeMoveDown);
        }

        private void ctxPotionsSteps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleStepsContextMenuOpening(lstPotionsSteps, tsmiPotionsRemove, tsmiPotionsMoveUp, tsmiPotionsMoveDown);
        }

        private void HandleStepsContextMenuOpening(ListBox lst, ToolStripMenuItem remove, ToolStripMenuItem moveUp, ToolStripMenuItem moveDown)
        {
            int iSelectedIndex = lst.SelectedIndex;
            bool hasSelectedItem = iSelectedIndex >= 0;
            remove.Enabled = hasSelectedItem;
            moveUp.Enabled = hasSelectedItem && iSelectedIndex > 0;
            moveDown.Enabled = hasSelectedItem && iSelectedIndex < lst.Items.Count - 1;
        }

        private void ctxMagicSteps_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleContextMenuItemClicked(e, lstMagicSteps, tsmiMagicRemove, tsmiMagicMoveUp, tsmiMagicMoveDown, chkMagicLastStepIndefinite);
        }

        private void ctxMeleeSteps_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleContextMenuItemClicked(e, lstMeleeSteps, tsmiMeleeRemove, tsmiMeleeMoveUp, tsmiMeleeMoveDown,chkMeleeRepeatLastStepIndefinitely);
        }

        private void ctxPotionsSteps_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleContextMenuItemClicked(e, lstPotionsSteps, tsmiPotionsRemove, tsmiPotionsMoveUp, tsmiPotionsMoveDown, chkPotionsRepeatLastStepIndefinitely);
        }

        private void HandleContextMenuItemClicked(ToolStripItemClickedEventArgs e, ListBox lst, ToolStripMenuItem remove, ToolStripMenuItem moveUp, ToolStripMenuItem moveDown, CheckBox repeatLastStepIndefinitely)
        {
            ToolStripItem clickedItem = e.ClickedItem;
            object selectedItem = lst.SelectedItem;
            int iSelectedIndex = lst.SelectedIndex;
            if (clickedItem == remove || clickedItem == moveUp || clickedItem == moveDown)
            {
                lst.Items.RemoveAt(iSelectedIndex);
                if (clickedItem == moveUp)
                {
                    lst.Items.Insert(iSelectedIndex - 1, selectedItem);
                }
                else if (clickedItem == moveDown)
                {
                    if (iSelectedIndex == lst.Items.Count - 1)
                        lst.Items.Add(selectedItem);
                    else
                        lst.Items.Insert(iSelectedIndex + 1, selectedItem);
                }
                else //remove
                {
                    if (lst.Items.Count == 0)
                    {
                        repeatLastStepIndefinitely.Checked = false;
                        repeatLastStepIndefinitely.Enabled = false;
                    }
                }
            }
        }

        private void tsmiMagicAdd_Click(object sender, EventArgs e)
        {
            lstMagicSteps.Items.Add(((ToolStripMenuItem)sender).Tag);
            chkMagicLastStepIndefinite.Enabled = true;
        }
        private void tsmiMeleeAdd_Click(object sender, EventArgs e)
        {
            lstMeleeSteps.Items.Add(((ToolStripMenuItem)sender).Tag);
            chkMeleeRepeatLastStepIndefinitely.Enabled = true;
        }
        private void tsmiPotionsAdd_Click(object sender, EventArgs e)
        {
            lstPotionsSteps.Items.Add(((ToolStripMenuItem)sender).Tag);
            chkPotionsRepeatLastStepIndefinitely.Enabled = true;
        }
    }
}
