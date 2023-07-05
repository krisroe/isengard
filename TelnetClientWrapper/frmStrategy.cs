using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmStrategy : Form
    {
        private AutoSpellLevelOverrides _autoSpellLevelInfo;

        private Strategy Strategy { get; set; }

        /// <summary>
        /// construvctor
        /// </summary>
        /// <param name="s">strategy. This strategy object is modified in place if the form is accepted.</param>
        public frmStrategy(Strategy s)
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

            txtName.Text = s.Name;
            chkAutogenerateName.Checked = s.AutogenerateName;

            cboOnKillMonster.SelectedIndex = (int)s.AfterKillMonsterAction;

            _autoSpellLevelInfo = new AutoSpellLevelOverrides(s.AutoSpellLevelMin, s.AutoSpellLevelMax, lblAutoSpellLevels);

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

            if (lstMagicSteps.Items.Count > 0)
            {
                Strategy.MagicSteps = new List<MagicStrategyStep>();
                foreach (MagicStrategyStep nextStep in lstMagicSteps.Items)
                {
                    Strategy.MagicSteps.Add(nextStep);
                }
            }
            else
            {
                Strategy.MagicSteps = null;
            }
            if (lstMeleeSteps.Items.Count > 0)
            {
                Strategy.MeleeSteps = new List<MeleeStrategyStep>();
                foreach (MeleeStrategyStep nextStep in lstMeleeSteps.Items)
                {
                    Strategy.MeleeSteps.Add(nextStep);
                }
            }
            else
            {
                Strategy.MeleeSteps = null;
            }
            if (lstPotionsSteps.Items.Count > 0)
            {
                Strategy.PotionsSteps = new List<PotionsStrategyStep>();
                foreach (PotionsStrategyStep nextStep in lstPotionsSteps.Items)
                {
                    Strategy.PotionsSteps.Add(nextStep);
                }
            }
            else
            {
                Strategy.PotionsSteps = null;
            }

            Strategy.AutogenerateName = chkAutogenerateName.Checked;
            Strategy.Name = Strategy.AutogenerateName ? string.Empty : txtName.Text;

            Strategy.AfterKillMonsterAction = (AfterKillMonsterAction)cboOnKillMonster.SelectedIndex;

            Strategy.AutoSpellLevelMin = _autoSpellLevelInfo.Minimum;
            Strategy.AutoSpellLevelMax = _autoSpellLevelInfo.Maximum;

            sInt = txtManaPool.Text;
            Strategy.ManaPool = string.IsNullOrEmpty(sInt) ? 0 : int.Parse(sInt);

            CommandType ct;

            ct = CommandType.None;
            if (chkMagicLastStepIndefinite.Checked) ct |= CommandType.Magic;
            if (chkMeleeRepeatLastStepIndefinitely.Checked) ct |= CommandType.Melee;
            if (chkPotionsRepeatLastStepIndefinitely.Checked) ct |= CommandType.Potions;
            Strategy.TypesToRunLastCommandIndefinitely = ct;

            sInt = txtMagicOnlyWhenStunnedForXMS.Text;
            Strategy.MagicOnlyWhenStunnedForXMS = string.IsNullOrEmpty(sInt) ? (int?)null : int.Parse(sInt);
            sInt = txtMeleeOnlyWhenStunnedForXMS.Text;
            Strategy.MeleeOnlyWhenStunnedForXMS = string.IsNullOrEmpty(sInt) ? (int?)null : int.Parse(sInt);
            sInt = txtPotionsOnlyWhenStunnedForXMS.Text;
            Strategy.PotionsOnlyWhenStunnedForXMS = string.IsNullOrEmpty(sInt) ? (int?)null : int.Parse(sInt);

            ct = CommandType.None;
            if (chkMagicEnabled.Checked) ct |= CommandType.Magic;
            if (chkMeleeEnabled.Checked) ct |= CommandType.Melee;
            if (chkPotionsEnabled.Checked) ct |= CommandType.Potions;
            Strategy.TypesWithStepsEnabled = ct;

            Strategy.FinalMagicAction = (FinalStepAction)cboMagicFinalAction.SelectedIndex;
            Strategy.FinalMeleeAction = (FinalStepAction)cboMeleeFinalAction.SelectedIndex;
            Strategy.FinalPotionsAction = (FinalStepAction)cboPotionsFinalAction.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
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
