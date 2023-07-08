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

            DataGridViewCellStyle oAlternatingRowsStyle = new DataGridViewCellStyle();
            oAlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(225, 255, 255);
            oAlternatingRowsStyle.ForeColor = System.Drawing.SystemColors.ControlText;
            oAlternatingRowsStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            oAlternatingRowsStyle.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dgvPermRuns.AlternatingRowsDefaultCellStyle = oAlternatingRowsStyle;

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
                colGo.HeaderText = "Go";
                colGo.MinimumWidth = 6;
                colGo.Name = "colGo";
                colGo.ReadOnly = true;
                colGo.Width = 75;
                dgvPermRuns.Columns.Add(colChangeAndRun);
                dgvPermRuns.Columns.Add(colRun);
                dgvPermRuns.Columns.Add(colGo);
            }
        }

        private void ctxPermRuns_Opening(object sender, CancelEventArgs e)
        {
            int iSelectedCount = dgvPermRuns.SelectedRows.Count;
            tsmiRemove.Visible = iSelectedCount > 0;
            bool showMoveUp = false;
            bool showMoveDown = false;
            if (iSelectedCount == 1)
            {
                DataGridViewRow r = dgvPermRuns.SelectedRows[0];
                int iIndex = r.Index;
                showMoveDown = iIndex < dgvPermRuns.RowCount - 1;
                showMoveUp = iIndex > 0;
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
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow r in dgvPermRuns.SelectedRows)
            {
                rows.Add(r);
            }
            foreach (DataGridViewRow r in rows)
            {
                dgvPermRuns.Rows.Remove(r);
            }
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
                    PermRun prToRun = null;
                    List<Exit> targetRoomToGo = null;
                    if (col == colEdit)
                    {
                        PromptedSkills skills = _currentEntityInfo.GetAvailableSkills(true);
                        WorkflowSpells castableSpells = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.Castable);
                        WorkflowSpells potionableSpells = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.All);
                        using (frmPermRun frm = new frmPermRun(_gameMap, _settings, skills, pr.TargetRoomObject, _getGraphInputs, _currentEntityInfo, castableSpells, potionableSpells, pr, false))
                        {
                            if (frm.ShowDialog(this) == DialogResult.OK)
                            {
                                frm.SaveFormDataToPermRun(pr);
                                UpdatePermRunDisplay(pr, iRowIndex);
                            }
                        }
                    }
                    else if (col == colChangeAndRun)
                    {
                        PermRun prChanged = new PermRun(pr);
                        PromptedSkills availableSkills = _currentEntityInfo.GetAvailableSkills(false);
                        WorkflowSpells castableSpells = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.Castable);
                        WorkflowSpells availablePotions = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.HavePotions);
                        if (ValidateAvailableSkillsAndSpellsAgainstPermRun(prChanged, ref availableSkills, ref castableSpells, ref availablePotions, false))
                        {
                            using (frmPermRun frm = new frmPermRun(_gameMap, _settings, availableSkills, pr.TargetRoomObject, _getGraphInputs, _currentEntityInfo, castableSpells, availablePotions, pr, true))
                            {
                                if (frm.ShowDialog(this) == DialogResult.OK)
                                {
                                    frm.SaveFormDataToPermRun(prChanged);
                                    prToRun = prChanged;
                                }
                            }
                        }
                    }
                    else if (col == colRun)
                    {
                        PermRun prChanged = new PermRun(pr);

                        //if power attack is missing, that isn't critical, just turn power attack off
                        PromptedSkills availableSkills = _currentEntityInfo.GetAvailableSkills(false);
                        if (((pr.SkillsToRun & PromptedSkills.PowerAttack) != PromptedSkills.None) &&
                            ((availableSkills & PromptedSkills.PowerAttack) == PromptedSkills.None))
                        {
                            pr.SkillsToRun &= ~PromptedSkills.PowerAttack;
                        }

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

        public bool ValidateAvailableSkillsAndSpellsAgainstPermRun(PermRun pr, ref PromptedSkills availableSkills, ref WorkflowSpells castableSpells, ref WorkflowSpells availablePotions, bool hardStop)
        {
            List<string> errorMessages = new List<string>();

            //if power attack is missing, that isn't critical, just turn power attack off
            if (((pr.SkillsToRun & PromptedSkills.PowerAttack) != PromptedSkills.None) && 
                ((availableSkills & PromptedSkills.PowerAttack) == PromptedSkills.None))
            {
                pr.SkillsToRun &= ~PromptedSkills.PowerAttack;
            }
            PromptedSkills requiredSkills = pr.SkillsToRun;
            PromptedSkills missingSkills = requiredSkills & ~availableSkills;
            if (missingSkills != PromptedSkills.None)
            {
                errorMessages.Add("Missing required skills: " + missingSkills.ToString().Replace(" ", ""));
            }
            WorkflowSpells requiredCastableSpells = pr.SpellsToCast;
            WorkflowSpells missingCastableSpells = requiredCastableSpells & ~castableSpells;
            if (missingCastableSpells != WorkflowSpells.None)
            {
                errorMessages.Add("Missing castable spells: " + missingCastableSpells.ToString().Replace(" ", ""));
            }
            WorkflowSpells requiredPotionSpells = pr.SpellsToPotion;
            WorkflowSpells missingPotionSpells = requiredPotionSpells & ~availablePotions;
            if (missingPotionSpells != WorkflowSpells.None)
            {
                errorMessages.Add("Missing potions for: " + missingPotionSpells.ToString().Replace(" ", ""));
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
                    if (MessageBox.Show(string.Join(Environment.NewLine, errorMessages.ToArray()) + Environment.NewLine + "Proceed anyway?", "Perm Run", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        availableSkills &= ~missingSkills;
                        castableSpells &= ~missingCastableSpells;
                        availablePotions &= ~missingPotionSpells;
                    }
                    else
                    {
                        ret = false;
                    }
                }
            }
            return ret;
        }

        public PermRun PermRunToRun { get; set; }
        public List<Exit> NavigateToRoom { get; set; }

        private void frmPermRuns_FormClosed(object sender, FormClosedEventArgs e)
        {
            _settings.PermRuns.Clear();
            foreach (DataGridViewRow r in dgvPermRuns.Rows)
            {
                _settings.PermRuns.Add((PermRun)r.Tag);
            }
        }
    }
}
