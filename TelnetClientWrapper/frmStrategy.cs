using IsengardClient.Backend;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmStrategy : Form
    {
        private IsengardSettingData _settings;
        private StrategyOverridesUI _strategyOverridesUI;

        private Strategy _strategy { get; set; }

        /// <summary>
        /// construvctor
        /// </summary>
        /// <param name="s">strategy. This strategy object is modified in place if the form is accepted.</param>
        /// <param name="settings">settings object</param>
        public frmStrategy(Strategy s, IsengardSettingData settings)
        {
            InitializeComponent();

            _strategy = s;
            _settings = settings;

            tsmiAddOffensiveAuto.Tag = MagicStrategyStep.OffensiveSpellAuto;
            tsmiAddOffensiveLevel1.Tag = MagicStrategyStep.OffensiveSpellLevel1;
            tsmiAddOffensiveLevel2.Tag = MagicStrategyStep.OffensiveSpellLevel2;
            tsmiAddOffensiveLevel3.Tag = MagicStrategyStep.OffensiveSpellLevel3;
            tsmiAddOffensiveLevel4.Tag = MagicStrategyStep.OffensiveSpellLevel4;
            tsmiAddOffensiveLevel5.Tag = MagicStrategyStep.OffensiveSpellLevel5;
            tsmiAddStun.Tag = MagicStrategyStep.Stun;
            tsmiAddStunWand.Tag = MagicStrategyStep.StunWand;
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

            tsmiChangeToOffensiveAuto.Tag = MagicStrategyStep.OffensiveSpellAuto;
            tsmiChangeToOffensiveLevel1.Tag = MagicStrategyStep.OffensiveSpellLevel1;
            tsmiChangeToOffensiveLevel2.Tag = MagicStrategyStep.OffensiveSpellLevel2;
            tsmiChangeToOffensiveLevel3.Tag = MagicStrategyStep.OffensiveSpellLevel3;
            tsmiChangeToOffensiveLevel4.Tag = MagicStrategyStep.OffensiveSpellLevel4;
            tsmiChangeToOffensiveLevel5.Tag = MagicStrategyStep.OffensiveSpellLevel5;
            tsmiChangeToStun.Tag = MagicStrategyStep.Stun;
            tsmiChangeToStunWand.Tag = MagicStrategyStep.StunWand;
            tsmiMagicChangeToVigor.Tag = MagicStrategyStep.Vigor;
            tsmiMagicChangeToMendWounds.Tag = MagicStrategyStep.MendWounds;
            tsmiMagicChangeToGenericHeal.Tag = MagicStrategyStep.GenericHeal;
            tsmiMagicChangeToCurePoison.Tag = MagicStrategyStep.CurePoison;
            tsmiChangeToRegularAttack.Tag = MeleeStrategyStep.RegularAttack;
            tsmiChangeToPowerAttack.Tag = MeleeStrategyStep.PowerAttack;
            tsmiPotionsChangeToVigor.Tag = PotionsStrategyStep.Vigor;
            tsmiPotionsChangeToMendWounds.Tag = PotionsStrategyStep.MendWounds;
            tsmiPotionsChangeToGenericHeal.Tag = PotionsStrategyStep.GenericHeal;
            tsmiPotionsChangeToCurePoison.Tag = PotionsStrategyStep.CurePoison;

            cboOnKillMonster.Items.Add(string.Empty);
            foreach (AfterKillMonsterAction nextAction in Enum.GetValues(typeof(AfterKillMonsterAction)))
            {
                cboOnKillMonster.Items.Add(nextAction);
            }
            if (s.AfterKillMonsterAction.HasValue)
                cboOnKillMonster.SelectedItem = s.AfterKillMonsterAction.Value;
            else
                cboOnKillMonster.SelectedIndex = 0;

            foreach (var nextFinalAction in Enum.GetValues(typeof(FinalStepAction)))
            {
                cboMagicFinalAction.Items.Add(nextFinalAction.ToString());
                cboMeleeFinalAction.Items.Add(nextFinalAction.ToString());
                cboPotionsFinalAction.Items.Add(nextFinalAction.ToString());
            }

            txtName.Text = s.DisplayName;

            _strategyOverridesUI = new StrategyOverridesUI(IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, s.AutoSpellLevelMin, s.AutoSpellLevelMax, lblAutoSpellLevels, null, s.Realms, lblCurrentRealmValue, StrategyOverridesLevel.Strategy, _settings);

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
            txtMagicLastXStepsRunIndefinitely.Text = hasSteps && s.MagicLastCommandsToRunIndefinitely > 0 ? s.MagicLastCommandsToRunIndefinitely.ToString() : string.Empty;
            txtMagicLastXStepsRunIndefinitely.Enabled = hasSteps;

            hasSteps = s.MeleeSteps != null;
            if (hasSteps)
            {
                foreach (var nextStep in s.MeleeSteps)
                {
                    lstMeleeSteps.Items.Add(nextStep);
                }
            }
            txtMeleeLastXStepsRunIndefinitely.Text = hasSteps && s.MeleeLastCommandsToRunIndefinitely > 0 ? s.MeleeLastCommandsToRunIndefinitely.ToString() : string.Empty;
            txtMeleeLastXStepsRunIndefinitely.Enabled = hasSteps;

            hasSteps = s.PotionsSteps != null;
            if (hasSteps)
            {
                foreach (var nextStep in s.PotionsSteps)
                {
                    lstPotionsSteps.Items.Add(nextStep);
                }
            }
            txtPotionsLastXStepsRunIndefinitely.Text = hasSteps && s.PotionsLastCommandsToRunIndefinitely > 0 ? s.PotionsLastCommandsToRunIndefinitely.ToString() : string.Empty;
            txtPotionsLastXStepsRunIndefinitely.Enabled = hasSteps;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string sInt = txtManaPool.Text;
            int iTemp;
            if (!string.IsNullOrEmpty(sInt) && !int.TryParse(sInt, out _))
            {
                MessageBox.Show("Invalid mana pool", "Strategy");
                txtManaPool.Focus();
                return;
            }
            sInt = txtMagicOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0))
            {
                MessageBox.Show("Invalid magic only when stunned for X MS", "Strategy");
                txtMagicOnlyWhenStunnedForXMS.Focus();
                return;
            }
            sInt = txtMeleeOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0))
            {
                MessageBox.Show("Invalid melee only when stunned for X MS", "Strategy");
                txtMeleeOnlyWhenStunnedForXMS.Focus();
                return;
            }
            sInt = txtPotionsOnlyWhenStunnedForXMS.Text;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0))
            {
                MessageBox.Show("Invalid potions only when stunned for X MS", "Strategy");
                txtPotionsOnlyWhenStunnedForXMS.Focus();
                return;
            }
            sInt = txtMagicLastXStepsRunIndefinitely.Text;
            int iMagicStepsCount = lstMagicSteps.Items.Count;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0 || iTemp > iMagicStepsCount))
            {
                MessageBox.Show("Invalid magic last X steps run indefinitely", "Strategy");
                txtMagicLastXStepsRunIndefinitely.Focus();
                return;
            }
            sInt = txtMeleeLastXStepsRunIndefinitely.Text;
            int iMeleeStepsCount = lstMeleeSteps.Items.Count;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0 || iTemp > iMeleeStepsCount))
            {
                MessageBox.Show("Invalid melee last X steps run indefinitely", "Strategy");
                txtMeleeLastXStepsRunIndefinitely.Focus();
                return;
            }
            sInt = txtPotionsLastXStepsRunIndefinitely.Text;
            int iPotionsStepsCount = lstPotionsSteps.Items.Count;
            if (!string.IsNullOrEmpty(sInt) && (!int.TryParse(sInt, out iTemp) || iTemp < 0 || iTemp > iPotionsStepsCount))
            {
                MessageBox.Show("Invalid potions last X steps run indefinitely", "Strategy");
                txtPotionsLastXStepsRunIndefinitely.Focus();
                return;
            }

            if (iMagicStepsCount > 0)
            {
                _strategy.MagicSteps = new List<MagicStrategyStep>();
                foreach (MagicStrategyStep nextStep in lstMagicSteps.Items)
                {
                    _strategy.MagicSteps.Add(nextStep);
                }
            }
            else
            {
                _strategy.MagicSteps = null;
            }
            if (iMeleeStepsCount > 0)
            {
                _strategy.MeleeSteps = new List<MeleeStrategyStep>();
                foreach (MeleeStrategyStep nextStep in lstMeleeSteps.Items)
                {
                    _strategy.MeleeSteps.Add(nextStep);
                }
            }
            else
            {
                _strategy.MeleeSteps = null;
            }
            if (iPotionsStepsCount > 0)
            {
                _strategy.PotionsSteps = new List<PotionsStrategyStep>();
                foreach (PotionsStrategyStep nextStep in lstPotionsSteps.Items)
                {
                    _strategy.PotionsSteps.Add(nextStep);
                }
            }
            else
            {
                _strategy.PotionsSteps = null;
            }

            _strategy.DisplayName = txtName.Text;
            if (cboOnKillMonster.SelectedIndex == 0)
                _strategy.AfterKillMonsterAction = null;
            else
                _strategy.AfterKillMonsterAction = (AfterKillMonsterAction)cboOnKillMonster.SelectedItem;
            _strategy.AutoSpellLevelMin = _strategyOverridesUI.StrategyAutoSpellLevelMinimum;
            _strategy.AutoSpellLevelMax = _strategyOverridesUI.StrategyAutoSpellLevelMaximum;
            _strategy.Realms = _strategyOverridesUI.StrategyRealms;

            sInt = txtManaPool.Text;
            _strategy.ManaPool = string.IsNullOrEmpty(sInt) ? 0 : int.Parse(sInt);

            sInt = txtMagicOnlyWhenStunnedForXMS.Text;
            _strategy.MagicOnlyWhenStunnedForXMS = string.IsNullOrEmpty(sInt) ? (int?)null : int.Parse(sInt);
            sInt = txtMeleeOnlyWhenStunnedForXMS.Text;
            _strategy.MeleeOnlyWhenStunnedForXMS = string.IsNullOrEmpty(sInt) ? (int?)null : int.Parse(sInt);
            sInt = txtPotionsOnlyWhenStunnedForXMS.Text;
            _strategy.PotionsOnlyWhenStunnedForXMS = string.IsNullOrEmpty(sInt) ? (int?)null : int.Parse(sInt);
            sInt = txtMagicLastXStepsRunIndefinitely.Text;
            _strategy.MagicLastCommandsToRunIndefinitely = string.IsNullOrEmpty(sInt) ? 0 : int.Parse(sInt);
            sInt = txtMeleeLastXStepsRunIndefinitely.Text;
            _strategy.MeleeLastCommandsToRunIndefinitely = string.IsNullOrEmpty(sInt) ? 0 : int.Parse(sInt);
            sInt = txtPotionsLastXStepsRunIndefinitely.Text;
            _strategy.PotionsLastCommandsToRunIndefinitely = string.IsNullOrEmpty(sInt) ? 0 : int.Parse(sInt);

            CommandType ct = CommandType.None;
            if (chkMagicEnabled.Checked) ct |= CommandType.Magic;
            if (chkMeleeEnabled.Checked) ct |= CommandType.Melee;
            if (chkPotionsEnabled.Checked) ct |= CommandType.Potions;
            _strategy.TypesWithStepsEnabled = ct;

            _strategy.FinalMagicAction = (FinalStepAction)cboMagicFinalAction.SelectedIndex;
            _strategy.FinalMeleeAction = (FinalStepAction)cboMeleeFinalAction.SelectedIndex;
            _strategy.FinalPotionsAction = (FinalStepAction)cboPotionsFinalAction.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ctxMagicSteps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleStepsContextMenuOpening(lstMagicSteps, tsmiMagicChangeTo, tsmiMagicRemove, tsmiMagicMoveUp, tsmiMagicMoveDown);
        }

        private void ctxMeleeSteps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleStepsContextMenuOpening(lstMeleeSteps, tsmiMeleeChangeTo, tsmiMeleeRemove, tsmiMeleeMoveUp, tsmiMeleeMoveDown);
        }

        private void ctxPotionsSteps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleStepsContextMenuOpening(lstPotionsSteps, tsmiPotionsChangeTo, tsmiPotionsRemove, tsmiPotionsMoveUp, tsmiPotionsMoveDown);
        }

        private void HandleStepsContextMenuOpening(ListBox lst, ToolStripMenuItem changeTo, ToolStripMenuItem remove, ToolStripMenuItem moveUp, ToolStripMenuItem moveDown)
        {
            int iSelectedIndex = lst.SelectedIndex;
            bool hasSelectedItem = iSelectedIndex >= 0;
            changeTo.Enabled = hasSelectedItem;
            remove.Enabled = hasSelectedItem;
            moveUp.Enabled = hasSelectedItem && iSelectedIndex > 0;
            moveDown.Enabled = hasSelectedItem && iSelectedIndex < lst.Items.Count - 1;
        }

        private void ctxMagicSteps_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleContextMenuItemClicked(e, lstMagicSteps, tsmiMagicRemove, tsmiMagicMoveUp, tsmiMagicMoveDown, txtMagicLastXStepsRunIndefinitely);
        }

        private void ctxMeleeSteps_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleContextMenuItemClicked(e, lstMeleeSteps, tsmiMeleeRemove, tsmiMeleeMoveUp, tsmiMeleeMoveDown,txtMeleeLastXStepsRunIndefinitely);
        }

        private void ctxPotionsSteps_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleContextMenuItemClicked(e, lstPotionsSteps, tsmiPotionsRemove, tsmiPotionsMoveUp, tsmiPotionsMoveDown, txtPotionsLastXStepsRunIndefinitely);
        }

        private void HandleContextMenuItemClicked(ToolStripItemClickedEventArgs e, ListBox lst, ToolStripMenuItem remove, ToolStripMenuItem moveUp, ToolStripMenuItem moveDown, TextBox txtRepeatLastStepsIndefinitely)
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
                        txtRepeatLastStepsIndefinitely.Text = string.Empty;
                        txtRepeatLastStepsIndefinitely.Enabled = false;
                    }
                }
            }
        }

        private void tsmiMagicAdd_Click(object sender, EventArgs e)
        {
            lstMagicSteps.Items.Add(((ToolStripMenuItem)sender).Tag);
            txtMagicLastXStepsRunIndefinitely.Enabled = true;
        }
        private void tsmiMagicChangeTo_Click(object sender, EventArgs e)
        {
            lstMagicSteps.Items[lstMagicSteps.SelectedIndex] = ((ToolStripMenuItem)sender).Tag;
        }

        private void tsmiMeleeAdd_Click(object sender, EventArgs e)
        {
            lstMeleeSteps.Items.Add(((ToolStripMenuItem)sender).Tag);
            txtMeleeLastXStepsRunIndefinitely.Enabled = true;
        }
        private void tsmiMeleeChangeTo_Click(object sender, EventArgs e)
        {
            lstMeleeSteps.Items[lstMeleeSteps.SelectedIndex] = ((ToolStripMenuItem)sender).Tag;
        }

        private void tsmiPotionsAdd_Click(object sender, EventArgs e)
        {
            lstPotionsSteps.Items.Add(((ToolStripMenuItem)sender).Tag);
            txtPotionsLastXStepsRunIndefinitely.Enabled = true;
        }
        private void tsmiPotionsChangeTo_Click(object sender, EventArgs e)
        {
            lstPotionsSteps.Items[lstPotionsSteps.SelectedIndex] = ((ToolStripMenuItem)sender).Tag;
        }
    }
}
