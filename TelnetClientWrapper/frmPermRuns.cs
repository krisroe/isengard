using System;
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
        private bool _inBackgroundProcess;

        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colTickRoom;
        private DataGridViewTextBoxColumn colRoom;
        private DataGridViewTextBoxColumn colMob;
        private DataGridViewButtonColumn colEdit;
        private DataGridViewButtonColumn colChangeAndRun;
        private DataGridViewButtonColumn colRun;
        private DataGridViewButtonColumn colGo;

        public frmPermRuns(IsengardSettingData settings, IsengardMap gameMap, CurrentEntityInfo entityInfo, Func<GraphInputs> getGraphInputs, bool inBackgroundProcess)
        {
            InitializeComponent();
            _settings = settings;
            _gameMap = gameMap;
            _currentEntityInfo = entityInfo;
            _getGraphInputs = getGraphInputs;
            _inBackgroundProcess = inBackgroundProcess;
            InitializeColumns(inBackgroundProcess);
            foreach (PermRun nextPermRun in settings.PermRuns)
            {
                UpdatePermRunDisplay(nextPermRun, null);
            }
        }

        private void InitializeColumns(bool inBackgroundProcess)
        {
            colName = new DataGridViewTextBoxColumn();
            colTickRoom = new DataGridViewTextBoxColumn();
            colRoom = new DataGridViewTextBoxColumn();
            colMob = new DataGridViewTextBoxColumn();
            colEdit = new DataGridViewButtonColumn();
            colName.HeaderText = "Name";
            colName.MinimumWidth = 6;
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.Width = 300;
            colTickRoom.HeaderText = "Tick";
            colTickRoom.MinimumWidth = 6;
            colTickRoom.Name = "colTickRoom";
            colTickRoom.ReadOnly = true;
            colTickRoom.Width = 150;
            colRoom.HeaderText = "Room";
            colRoom.MinimumWidth = 6;
            colRoom.Name = "colRoom";
            colRoom.ReadOnly = true;
            colRoom.Width = 300;
            colMob.HeaderText = "Mob";
            colMob.MinimumWidth = 6;
            colMob.Name = "colMob";
            colMob.ReadOnly = true;
            colMob.Width = 200;
            colEdit.HeaderText = "Edit";
            colEdit.MinimumWidth = 6;
            colEdit.Name = "colEdit";
            colEdit.ReadOnly = true;
            colEdit.Width = 75;
            dgvPermRuns.Columns.Add(colName);
            dgvPermRuns.Columns.Add(colTickRoom);
            dgvPermRuns.Columns.Add(colRoom);
            dgvPermRuns.Columns.Add(colMob);
            dgvPermRuns.Columns.Add(colEdit);
            if (!inBackgroundProcess)
            {
                colChangeAndRun = new DataGridViewButtonColumn();
                colRun = new DataGridViewButtonColumn();
                colGo = new DataGridViewButtonColumn();
                colChangeAndRun.HeaderText = "Change+Run";
                colChangeAndRun.MinimumWidth = 6;
                colChangeAndRun.Name = "colChangeAndRun";
                colChangeAndRun.ReadOnly = true;
                colChangeAndRun.Width = 125;
                colRun.HeaderText = "Run";
                colRun.MinimumWidth = 6;
                colRun.Name = "colRun";
                colRun.ReadOnly = true;
                colRun.Width = 75;
                colGo.Name = "colGo";
                colGo.MinimumWidth = 6;
                colGo.ReadOnly = true;
                colGo.Width = 75;
                dgvPermRuns.Columns.Add(colChangeAndRun);
                dgvPermRuns.Columns.Add(colRun);
                dgvPermRuns.Columns.Add(colGo);
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
                showMoveUp = iIndex < dgvPermRuns.RowCount - 1;
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
            pr.BeforeFull = FullType.Total;
            pr.AfterFull = FullType.Almost;
            PromptedSkills skills = _currentEntityInfo.GetAvailableSkills(true);
            WorkflowSpells castableSpells = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.Castable);
            pr.SpellsToCast = castableSpells & (WorkflowSpells.Bless | WorkflowSpells.Protection | WorkflowSpells.CurePoison);
            pr.SpellsToPotion = WorkflowSpells.None;
            pr.SkillsToRun = PromptedSkills.PowerAttack;
            pr.AutoSpellLevelMin = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            pr.AutoSpellLevelMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
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
            string sMob = string.Empty;
            bool hasMobType = false;
            int iMobIndex = nextPermRun.MobIndex;
            if (nextPermRun.MobType.HasValue)
            {
                sMob = nextPermRun.MobType.Value.ToString();
                hasMobType = true;
            }
            else if (!string.IsNullOrEmpty(nextPermRun.MobText))
            {
                sMob = nextPermRun.MobText;
                hasMobType = true;
            }
            else if (iMobIndex >= 1)
            {
                sMob = iMobIndex.ToString();
            }
            if (hasMobType && iMobIndex > 1)
            {
                sMob += " " + iMobIndex;
            }
            string sRoom = nextPermRun.TargetRoomObject.BackendName;
            DataGridViewRow r;
            if (rowIndex.HasValue)
            {
                r = dgvPermRuns.Rows[rowIndex.Value];
                if (_inBackgroundProcess)
                    r.SetValues(sDisplayName, sTickRoom, sRoom, sMob, "Edit");
                else
                    r.SetValues(sDisplayName, sTickRoom, sRoom, sMob, "Edit", "Change+Run", "Run", "Go");
            }
            else
            {
                if (_inBackgroundProcess)
                    rowIndex = dgvPermRuns.Rows.Add(sDisplayName, sTickRoom, sRoom, sMob, "Edit");
                else
                    rowIndex = dgvPermRuns.Rows.Add(sDisplayName, sTickRoom, sRoom, sMob, "Edit", "Change+Run", "Run", "Go");
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
                    List<Exit> targetRoomToGo = null;
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
                            using (frmPermRun frm = new frmPermRun(_gameMap, _settings, skills, pr.TargetRoomObject, _getGraphInputs, _currentEntityInfo, castableSpells, potionableSpells, pr, true))
                            {
                                if (frm.ShowDialog(this) == DialogResult.OK)
                                {
                                    prToRun = new PermRun();
                                    frm.SaveFormDataToPermRun(prToRun);
                                }
                            }
                        }
                    }
                    else if (col == colRun)
                    {
                        if (pr.IsRunnable(_getGraphInputs, _currentEntityInfo, this, _gameMap))
                        {
                            prToRun = new PermRun(pr);
                        }
                    }
                    else if (col == colGo)
                    {
                        targetRoomToGo = MapComputation.ComputeLowestCostPath(_currentEntityInfo.CurrentRoom, pr.TargetRoomObject, _getGraphInputs());
                        if (targetRoomToGo == null)
                        {
                            MessageBox.Show("Unable to navigate to room.");
                        }
                    }
                    if (prToRun != null || targetRoomToGo != null)
                    {
                        PermRunToRun = prToRun;
                        NavigateToRoom = targetRoomToGo;
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
        public List<Exit> NavigateToRoom { get; set; }
    }
}
