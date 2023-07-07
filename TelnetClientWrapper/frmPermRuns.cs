using System;
using System.ComponentModel;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPermRuns : Form
    {
        private IsengardSettingData _settings;
        private IsengardMap _gameMap;
        private CurrentEntityInfo _currentEntityInfo;
        private Func<GraphInputs> _getGraphInputs;
        public frmPermRuns(IsengardSettingData settings, IsengardMap gameMap, CurrentEntityInfo entityInfo, Func<GraphInputs> getGraphInputs)
        {
            InitializeComponent();
            _settings = settings;
            _gameMap = gameMap;
            _currentEntityInfo = entityInfo;
            _getGraphInputs = getGraphInputs;
            foreach (PermRun nextPermRun in settings.PermRuns)
            {
                AddPermRunToDisplay(nextPermRun);
            }
        }

        private void ctxPermRuns_Opening(object sender, CancelEventArgs e)
        {
            bool hasSelected = dgvPermRuns.SelectedRows.Count > 0;
            tsmiRemove.Visible = hasSelected;
            bool showMoveUp = false;
            bool showMoveDown = false;
            if (hasSelected)
            {
                DataGridViewRow r = dgvPermRuns.SelectedRows[0];
                int iIndex = r.Index;
                showMoveDown = iIndex > 0;
                showMoveUp = iIndex < dgvPermRuns.RowCount;
            }
            tsmiMoveUp.Visible = showMoveUp;
            tsmiMoveDown.Visible = showMoveDown;
        }

        private void tsmiAdd_Click(object sender, EventArgs e)
        {
            Room currentRoom = _currentEntityInfo.CurrentRoom;
            PermRun pr = new PermRun();
            string sMob = string.Empty;
            if (currentRoom != null)
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    if (_currentEntityInfo.CurrentRoomMobs.Count > 0)
                    {
                        pr.MobType = _currentEntityInfo.CurrentRoomMobs[0];
                        pr.MobIndex = 1;
                    }
                }
            }
            pr.TickRoom = currentRoom.HealingRoom;
            pr.PawnShop = currentRoom.PawnShoppe;
            pr.FullBeforeStarting = true;
            pr.FullAfterFinishing = true;
            PromptedSkills skills = _currentEntityInfo.GetAvailableSkills(true);
            WorkflowSpells spellsToCast = _currentEntityInfo.GetAvailableSpellsToCast(false);
            WorkflowSpells spellsToPotion = _currentEntityInfo.GetAvailableSpellsToCast(true);
            using (frmPermRun frm = new frmPermRun(_gameMap, _settings, skills, currentRoom, _getGraphInputs, _currentEntityInfo, spellsToCast, spellsToPotion, pr))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    pr.AfterKillMonsterAction = frm.AfterKillMonsterAction;
                    pr.AutoSpellLevelMax = frm.AutoSpellLevelMin;
                    pr.AutoSpellLevelMax = frm.AutoSpellLevelMax;
                    pr.DisplayName = frm.DisplayName;
                    pr.FullBeforeStarting = frm.FullBeforeStarting;
                    pr.FullAfterFinishing = frm.FullAfterFinishing;
                    pr.ItemsToProcessType = frm.InventoryFlow;
                    pr.MobIndex = frm.MobIndex;
                    pr.MobText = frm.MobText;
                    pr.MobType = frm.MobType;
                    pr.PawnShop = frm.PawnShop;
                    pr.TickRoom = frm.HealingRoom;
                    pr.SpellsToCast = frm.SelectedCastSpells;
                    pr.SpellsToPotion = frm.SelectedPotionsSpells;
                    pr.SkillsToRun = frm.SelectedSkills;
                    pr.Strategy = frm.SelectedStrategy;
                    pr.TargetRoom = frm.TargetRoomText;
                    pr.UseMagicCombat = frm.UseMagicCombat;
                    pr.UseMeleeCombat = frm.UseMeleeCombat;
                    pr.UsePotionsCombat = frm.UsePotionsCombat;
                    _settings.PermRuns.Add(pr);
                    AddPermRunToDisplay(pr);
                }
            }
        }

        private void AddPermRunToDisplay(PermRun nextPermRun)
        {
            string sDisplayName = nextPermRun.DisplayName;
            if (string.IsNullOrEmpty(sDisplayName))
            {
                if (nextPermRun.MobType.HasValue)
                    sDisplayName = nextPermRun.MobType.ToString();
                else if (!string.IsNullOrEmpty(nextPermRun.MobText))
                    sDisplayName = nextPermRun.MobText;
                else
                    sDisplayName = "No display name";
            }
            int iIndex = dgvPermRuns.Rows.Add(sDisplayName, "Edit", "Change+Run", "Run");
            DataGridViewRow oNewRow = dgvPermRuns.Rows[iIndex];
            oNewRow.Tag = nextPermRun;
        }

        private void tsmiRemove_Click(object sender, EventArgs e)
        {
            DataGridViewRow r = dgvPermRuns.SelectedRows[0];
            dgvPermRuns.Rows.Remove(r);
            _settings.PermRuns.Remove((PermRun)r.Tag);
        }

        private void tsmiMoveUp_Click(object sender, EventArgs e)
        {
            DataGridViewRow r = dgvPermRuns.SelectedRows[0];
            MoveDown(dgvPermRuns.Rows[r.Index - 1]);
        }

        private void tsmiMoveDown_Click(object sender, EventArgs e)
        {
            MoveDown(dgvPermRuns.SelectedRows[0]);
        }

        private void MoveDown(DataGridViewRow r)
        {
            int iHigherIndex = r.Index + 1;
            bool isLast = iHigherIndex == dgvPermRuns.Rows.Count - 1;
            DataGridViewRow rNext = dgvPermRuns.Rows[iHigherIndex];
            dgvPermRuns.Rows.Remove(r);
            if (isLast)
                dgvPermRuns.Rows.Add(r);
            else
                dgvPermRuns.Rows.Insert(iHigherIndex, r);
        }
    }
}
