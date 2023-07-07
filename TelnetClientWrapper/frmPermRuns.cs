﻿using System;
using System.Collections.Generic;
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
                UpdatePermRunDisplay(nextPermRun, null);
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
            WorkflowSpells castableSpells = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.Castable);
            pr.SpellsToCast = castableSpells & (WorkflowSpells.Bless | WorkflowSpells.Protection | WorkflowSpells.CurePoison);
            pr.SpellsToPotion = WorkflowSpells.None;
            pr.SkillsToRun = PromptedSkills.PowerAttack;
            WorkflowSpells spellsToPotion = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.All);
            using (frmPermRun frm = new frmPermRun(_gameMap, _settings, skills, currentRoom, _getGraphInputs, _currentEntityInfo, castableSpells, spellsToPotion, pr, false))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    frm.SaveFormDataToPermRun(pr);
                    _settings.PermRuns.Add(pr);
                    UpdatePermRunDisplay(pr, null);
                }
            }
        }

        private void UpdatePermRunDisplay(PermRun nextPermRun, int? rowIndex)
        {
            string sDisplayName = string.IsNullOrEmpty(nextPermRun.DisplayName) ? "None" : nextPermRun.DisplayName;
            string sTickRoom = nextPermRun.TickRoom.HasValue ? nextPermRun.TickRoom.Value.ToString() : string.Empty;
            string sMob = nextPermRun.MobType.HasValue ? nextPermRun.MobType.Value.ToString() : nextPermRun.MobText;
            DataGridViewRow r;
            if (rowIndex.HasValue)
            {
                r = dgvPermRuns.Rows[rowIndex.Value];
                r.SetValues(sDisplayName, sTickRoom, sMob, "Edit", "Change+Run", "Run");
            }
            else
            {
                rowIndex = dgvPermRuns.Rows.Add(sDisplayName, sTickRoom, sMob, "Edit", "Change+Run", "Run");
                r = dgvPermRuns.Rows[rowIndex.Value];
            }
            r.Tag = nextPermRun;
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
            dgvPermRuns.Rows.Remove(r);
            if (isLast)
                dgvPermRuns.Rows.Add(r);
            else
                dgvPermRuns.Rows.Insert(iHigherIndex, r);
        }

        private void dgvPermRuns_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRowIndex = e.RowIndex;
            if (iRowIndex >= 0)
            {
                int iColumnIndex = e.ColumnIndex;
                DataGridViewButtonColumn col = dgvPermRuns.Columns[iColumnIndex] as DataGridViewButtonColumn;
                if (col != null)
                {
                    PermRun pr = (PermRun)dgvPermRuns.Rows[iRowIndex].Tag;
                    PromptedSkills skills = _currentEntityInfo.GetAvailableSkills(true);
                    WorkflowSpells castableSpells = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.Castable);
                    WorkflowSpells potionableSpells = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.All);
                    PermRun prToRun = null;
                    if (col == colEdit)
                    {
                        using (frmPermRun frm = new frmPermRun(_gameMap, _settings, skills, pr.TargetRoomObject, _getGraphInputs, _currentEntityInfo, castableSpells, potionableSpells, pr, false))
                        {
                            if (frm.ShowDialog(this) == DialogResult.OK)
                            {
                                frm.SaveFormDataToPermRun(pr);
                                _settings.PermRuns[iRowIndex] = pr;
                                UpdatePermRunDisplay(pr, iRowIndex);
                            }
                        }
                    }
                    else if (col == colChangeAndRun)
                    {
                        if (ValidateAvailableSkillsAndSpellsAgainstPermRun(pr, skills, castableSpells, false))
                        {
                            PermRun clone = new PermRun(pr);
                            using (frmPermRun frm = new frmPermRun(_gameMap, _settings, skills, pr.TargetRoomObject, _getGraphInputs, _currentEntityInfo, castableSpells, potionableSpells, clone, true))
                            {
                                frm.SaveFormDataToPermRun(clone);
                                prToRun = clone;
                            }
                        }
                    }
                    else if (col == colRun)
                    {
                        if (ValidateAvailableSkillsAndSpellsAgainstPermRun(pr, skills, castableSpells, true))
                        {
                            prToRun = pr;
                        }
                    }
                    if (prToRun != null)
                    {
                        PermRunToRun = prToRun;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
        }

        public bool ValidateAvailableSkillsAndSpellsAgainstPermRun(PermRun pr, PromptedSkills skills, WorkflowSpells castableSpells, bool hardStop)
        {
            PromptedSkills missingSkills;
            WorkflowSpells missingSpells;
            List<string> errorMessages = new List<string>();
            PromptedSkills requiredSkills = pr.SkillsToRun & ~PromptedSkills.PowerAttack;
            if ((skills & requiredSkills) != requiredSkills)
            {
                missingSkills = requiredSkills & ~skills;
                errorMessages.Add("Missing required skills: " + missingSkills.ToString().Replace(" ", ""));
            }
            WorkflowSpells requiredSpells = pr.SpellsToCast;
            if ((castableSpells & requiredSpells) != requiredSpells)
            {
                missingSpells = requiredSpells & ~castableSpells;
                errorMessages.Add("Missing castable spells: " + missingSpells.ToString().Replace(" ", ""));
            }
            WorkflowSpells availablePotions = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.HavePotions);
            requiredSpells = pr.SpellsToPotion;
            if ((availablePotions & requiredSpells) != requiredSpells)
            {
                missingSpells = requiredSpells & ~availablePotions;
                errorMessages.Add("Missing potions for: " + missingSpells.ToString().Replace(" ", ""));
            }
            bool ret = errorMessages.Count == 0;
            if (!ret)
            {
                if (hardStop)
                {
                    MessageBox.Show(string.Join(Environment.NewLine, errorMessages.ToArray()));
                }
                else
                {
                    ret = MessageBox.Show(string.Join(Environment.NewLine, errorMessages.ToArray()) + Environment.NewLine + "Proceed anyway?", "Perm Run", MessageBoxButtons.OKCancel) == DialogResult.OK;
                }
            }
            return ret;
        }

        public PermRun PermRunToRun { get; set; }
    }
}