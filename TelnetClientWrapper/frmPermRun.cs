using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPermRun : Form
    {
        private Func<GraphInputs> _GraphInputs;
        private IsengardMap _gameMap;
        private IsengardSettingData _settingsData;
        private Room _currentRoom;
        private CheckBox _chkPowerAttack;
        private CurrentEntityInfo _currentEntityInfo;
        private AutoSpellLevelOverrides _autoSpellLevelInfo;

        public Strategy SelectedStrategy { get; set; }

        public HealingRoom? HealingRoom
        {
            get
            {
                return cboTickRoom.SelectedIndex == 0 ? (HealingRoom?)null : (HealingRoom)cboTickRoom.SelectedItem;
            }
        }
        public PawnShoppe? PawnShop
        {
            get
            {
                return cboPawnShoppe.SelectedIndex == 0 ? (PawnShoppe?)null : (PawnShoppe)cboPawnShoppe.SelectedItem;
            }
        }

        public InventoryProcessWorkflow InventoryFlow
        {
            get
            {
                return (InventoryProcessWorkflow)cboInventoryFlow.SelectedItem;
            }
        }

        public bool FullBeforeStarting
        {
            get
            {
                return chkFullBeforeStarting.Checked;
            }
        }

        public bool FullAfterFinishing
        {
            get
            {
                return chkFullAfterFinishing.Checked;
            }
        }

        public Room TargetRoom
        {
            get
            {
                return (Room)cboRoom.SelectedItem;
            }
        }

        public bool IsCombatStrategy { get; set; }
        public string MobText { get; set; }
        public MobTypeEnum? MobType { get; set; }
        public int MobIndex { get; set; }

        public frmPermRun(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, Strategy strategy, HealingRoom? healingRoom, PawnShoppe? pawnShop, InventoryProcessWorkflow invWorkflow, CurrentEntityInfo currentEntityInfo, bool fullBeforeStarting, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions)
        {
            InitializeComponent();

            chkFullBeforeStarting.Checked = fullBeforeStarting;

            cboTickRoom.Items.Add(string.Empty);
            foreach (var nextHealingRoom in Enum.GetValues(typeof(HealingRoom)))
            {
                cboTickRoom.Items.Add(nextHealingRoom);
            }
            if (healingRoom.HasValue)
            {
                cboTickRoom.SelectedItem = healingRoom.Value;
            }
            else
            {
                cboTickRoom.SelectedIndex = 0;
            }
            cboPawnShoppe.Items.Add(string.Empty);
            foreach (var nextPawnShop in Enum.GetValues(typeof(PawnShoppe)))
            {
                cboPawnShoppe.Items.Add(nextPawnShop);
            }
            if (pawnShop.HasValue)
            {
                cboPawnShoppe.SelectedItem = pawnShop;
            }
            else
            {
                cboPawnShoppe.SelectedIndex = 0;
            }

            cboInventoryFlow.Items.Add(InventoryProcessWorkflow.NoProcessing);
            cboInventoryFlow.Items.Add(InventoryProcessWorkflow.ProcessMonsterDrops);
            cboInventoryFlow.Items.Add(InventoryProcessWorkflow.ProcessAllItemsInRoom);
            cboInventoryFlow.SelectedItem = invWorkflow;

            _GraphInputs = GetGraphInputs;
            _gameMap = gameMap;
            _settingsData = settingsData;
            _currentRoom = currentRoom;
            _currentEntityInfo = currentEntityInfo;

            if (currentRoom == null)
            {
                cboRoom.Enabled = false;
            }
            else
            {
                cboRoom.Items.Add(currentRoom);
                foreach (Exit nextEdge in currentRoom.Exits)
                {
                    Room targetRoom = nextEdge.Target;
                    if (currentRoom != targetRoom)
                    {
                        cboRoom.Items.Add(targetRoom);
                    }
                }
                cboRoom.SelectedItem = currentRoom;
            }
            if (cboMob.Items.Contains(currentMob))
            {
                cboMob.SelectedItem = currentMob;
            }
            else
            {
                cboMob.Text = currentMob;
            }

            foreach (PromptedSkills nextSkill in Enum.GetValues(typeof(PromptedSkills)))
            {
                if (nextSkill != PromptedSkills.None)
                {
                    if ((skills & nextSkill) == nextSkill)
                    {
                        CheckBox chk = new CheckBox();
                        chk.AutoSize = true;
                        if (nextSkill == PromptedSkills.PowerAttack)
                        {
                            chk.Checked = chk.Visible = strategy.IsCombatStrategy(CommandType.Melee, strategy.TypesWithStepsEnabled);
                            _chkPowerAttack = chk;
                        }
                        chk.Margin = new Padding(4);
                        chk.Tag = nextSkill;
                        chk.Text = nextSkill.ToString();
                        chk.UseVisualStyleBackColor = true;
                        flpSkills.Controls.Add(chk);
                    }
                }
            }
            foreach (WorkflowSpells nextSpell in Enum.GetValues(typeof(WorkflowSpells)))
            {
                if (nextSpell != WorkflowSpells.None)
                {
                    if ((spellsCastOptions & nextSpell) == nextSpell)
                    {
                        AddSpellCheckbox(flpSpellsCast, nextSpell, nextSpell == WorkflowSpells.Bless || nextSpell == WorkflowSpells.Protection || nextSpell == WorkflowSpells.CurePoison);
                    }
                    if ((spellsPotionsOptions & nextSpell) == nextSpell)
                    {
                        AddSpellCheckbox(flpSpellsPotions, nextSpell, false);
                    }
                }
            }

            _autoSpellLevelInfo = new AutoSpellLevelOverrides(strategy.AutoSpellLevelMin, strategy.AutoSpellLevelMax, lblCurrentAutoSpellLevelsValue);
            foreach (Strategy s in settingsData.Strategies)
            {
                cboStrategy.Items.Add(s);
            }
            cboStrategy.SelectedItem = strategy;
            RefreshUIFromStrategy();
        }

        private void AddSpellCheckbox(FlowLayoutPanel flp, WorkflowSpells spell, bool isChecked)
        {
            CheckBox chk = new CheckBox();
            chk.AutoSize = true;
            chk.Checked = isChecked;
            chk.Margin = new Padding(4);
            chk.Tag = spell;
            chk.Text = spell.ToString();
            chk.UseVisualStyleBackColor = true;
            flp.Controls.Add(chk);
        }

        private void RefreshUIFromStrategy()
        {
            Strategy Strategy = (Strategy)cboStrategy.SelectedItem;
            chkMagic.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            chkMelee.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            chkPotions.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            cboOnKillMonster.SelectedIndex = (int)Strategy.AfterKillMonsterAction;
            _autoSpellLevelInfo.Minimum = Strategy.AutoSpellLevelMin;
            _autoSpellLevelInfo.Maximum = Strategy.AutoSpellLevelMax;
            _autoSpellLevelInfo.RefreshAutoSpellLevelUI();
            RefreshUIFromEffectiveStrategy();
        }

        private void RefreshUIFromEffectiveStrategy()
        {
            CommandType eCombatTypes = GetSelectedCombatTypes();
            Strategy Strategy = (Strategy)cboStrategy.SelectedItem;
            bool showMob = Strategy.IsCombatStrategy(CommandType.All, eCombatTypes);
            lblMob.Visible = showMob;
            cboMob.Visible = showMob;
            grpStrategyModifications.Text = "Strategy (" + Strategy.GetToStringForCommandTypes(eCombatTypes) + ")";
            if (_chkPowerAttack != null)
            {
                _chkPowerAttack.Visible = Strategy.IsCombatStrategy(CommandType.Melee, eCombatTypes);
            }
        }

        private CommandType GetSelectedCombatTypes()
        {
            CommandType eCombatTypes = CommandType.None;
            if (chkMagic.Checked) eCombatTypes |= CommandType.Magic;
            if (chkMelee.Checked) eCombatTypes |= CommandType.Melee;
            if (chkPotions.Checked) eCombatTypes |= CommandType.Potions;
            return eCombatTypes;
        }

        private string GetInputMobText()
        {
            string full;
            if (cboMob.SelectedItem == null)
                full = cboMob.Text;
            else
                full = cboMob.SelectedItem.ToString();
            return full;
        }

        public PromptedSkills SelectedSkills
        {
            get
            {
                PromptedSkills ret = PromptedSkills.None;
                foreach (CheckBox nextSkillCheckbox in flpSkills.Controls)
                {
                    if (nextSkillCheckbox.Checked)
                    {
                        ret |= (PromptedSkills)nextSkillCheckbox.Tag;
                    }
                }
                return ret;
            }
        }

        public WorkflowSpells SelectedCastSpells
        {
            get
            {
                return GetSelectedSpells(flpSpellsCast);
            }
        }

        public WorkflowSpells SelectedPotionsSpells
        {
            get
            {
                return GetSelectedSpells(flpSpellsPotions);
            }
        }

        private WorkflowSpells GetSelectedSpells(FlowLayoutPanel flp)
        {
            WorkflowSpells ret = WorkflowSpells.None;
            foreach (CheckBox nextSpellCheckbox in flp.Controls)
            {
                if (nextSpellCheckbox.Checked)
                {
                    ret |= (WorkflowSpells)nextSpellCheckbox.Tag;
                }
            }
            return ret;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            Strategy selectedStrategy = new Strategy((Strategy)cboStrategy.SelectedItem);
            selectedStrategy.AfterKillMonsterAction = (AfterKillMonsterAction)cboOnKillMonster.SelectedIndex;
            selectedStrategy.TypesWithStepsEnabled = GetSelectedCombatTypes();
            selectedStrategy.AutoSpellLevelMin = _autoSpellLevelInfo.Minimum;
            selectedStrategy.AutoSpellLevelMax = _autoSpellLevelInfo.Maximum;

            IsCombatStrategy = selectedStrategy.IsCombatStrategy(CommandType.All, selectedStrategy.TypesWithStepsEnabled);
            if (IsCombatStrategy)
            {
                if (MobEntity.GetMobInfo(GetInputMobText(), out string mobText, out MobTypeEnum? mobType, out int mobIndex))
                {
                    MobText = mobText;
                    MobType = mobType;
                    MobIndex = mobIndex;
                }
                else
                {
                    MessageBox.Show("No mob specified.");
                    return;
                }
            }

            SelectedStrategy = selectedStrategy;

            InventoryProcessWorkflow ipw = (InventoryProcessWorkflow)cboInventoryFlow.SelectedItem;
            if (ipw != InventoryProcessWorkflow.NoProcessing && (cboPawnShoppe.SelectedIndex == 0 || cboTickRoom.SelectedIndex == 0))
            {
                if (MessageBox.Show("No pawn/tick room selected. Continue?", "Inventory Processing", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
            }

            GraphInputs graphInputs = _GraphInputs();

            //verify the target room is reachable
            Room targetRoom = cboRoom.SelectedItem as Room;
            if (targetRoom == null)
            {
                MessageBox.Show("No target room selected.");
                return;
            }
            if (targetRoom != null && targetRoom != _currentRoom)
            {
                if (MapComputation.ComputeLowestCostPath(_currentRoom, (Room)cboRoom.SelectedItem, graphInputs) == null)
                {
                    MessageBox.Show("Cannot find path to selected room.");
                    return;
                }
            }
            //verify healing room is reachable
            if (cboTickRoom.SelectedIndex > 0)
            {
                Room healingRoom = _gameMap.HealingRooms[(HealingRoom)cboTickRoom.SelectedItem];
                if (targetRoom != null && targetRoom != healingRoom)
                {
                    if (MapComputation.ComputeLowestCostPath(targetRoom, healingRoom, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from target room to tick room.");
                        return;
                    }
                    if (MapComputation.ComputeLowestCostPath(healingRoom, targetRoom, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from tick room to healing room.");
                        return;
                    }
                }
            }
            //verify pawn shop is reachable
            if (cboPawnShoppe.SelectedIndex > 0)
            {
                Room pawnShop = _gameMap.PawnShoppes[(PawnShoppe)cboPawnShoppe.SelectedItem];
                if (targetRoom != null && targetRoom != pawnShop)
                {
                    if (MapComputation.ComputeLowestCostPath(targetRoom, pawnShop, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from target room to pawn shop.");
                        return;
                    }
                    if (MapComputation.ComputeLowestCostPath(pawnShop, targetRoom, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from pawn shop to target room.");
                        return;
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            frmGraph graphForm = new frmGraph(_gameMap, _currentRoom, true, _GraphInputs, VertexSelectionRequirement.ValidPathFromCurrentLocation);
            graphForm.ShowDialog();
            if (graphForm.SelectedRoom != null)
            {
                if (!cboRoom.Items.Contains(graphForm.SelectedRoom))
                {
                    cboRoom.Items.Add(graphForm.SelectedRoom);
                }
                cboRoom.SelectedItem = graphForm.SelectedRoom;
            }
        }

        private void btnLocations_Click(object sender, EventArgs e)
        {
            frmLocations locationsForm = new frmLocations(_gameMap, _settingsData, _currentRoom, true, _GraphInputs);
            locationsForm.ShowDialog();
            if (locationsForm.SelectedRoom != null)
            {
                if (!cboRoom.Items.Contains(locationsForm.SelectedRoom))
                {
                    cboRoom.Items.Add(locationsForm.SelectedRoom);
                }
                cboRoom.SelectedItem = locationsForm.SelectedRoom;
            }
        }

        private void btnEditStrategy_Click(object sender, EventArgs e)
        {
            Strategy chosenStrategy = (Strategy)cboStrategy.SelectedItem;
            bool isBaseStrategy = _settingsData.Strategies.Contains(chosenStrategy);
            if (isBaseStrategy) chosenStrategy = new Strategy(chosenStrategy);
            using (frmStrategy frm = new frmStrategy(chosenStrategy))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    if (isBaseStrategy)
                    {
                        chosenStrategy.Name = $"Modified ({chosenStrategy})";
                        cboStrategy.Items.Add(chosenStrategy);
                    }
                    cboStrategy.SelectedItem = chosenStrategy;
                    RefreshUIFromStrategy();
                }
            }
        }

        private void cboRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            Room r = (Room)cboRoom.SelectedItem;
            cboMob.Items.Clear();
            HashSet<string> entries = new HashSet<string>();
            lock (_currentEntityInfo)
            {
                int iCounter = 0;
                MobTypeEnum? prevMob = null;
                bool isCurrentRoom = _currentEntityInfo.CurrentRoom == r;
                if (isCurrentRoom)
                {
                    foreach (MobTypeEnum nextMob in _currentEntityInfo.CurrentRoomMobs)
                    {
                        if (prevMob.HasValue && prevMob.Value == nextMob)
                            iCounter++;
                        else
                            iCounter = 1;
                        string sMobText = nextMob.ToString();
                        if (iCounter > 1) sMobText += " 1";
                        prevMob = nextMob;
                        cboMob.Items.Add(sMobText);
                        entries.Add(sMobText);
                    }
                }
                if (r.PermanentMobs != null)
                {
                    prevMob = null;
                    foreach (MobTypeEnum nextMob in r.PermanentMobs)
                    {
                        StaticMobData smb = MobEntity.StaticMobData[nextMob];
                        if (smb.Visibility != MobVisibility.Visible || !isCurrentRoom)
                        {
                            if (prevMob.HasValue && prevMob.Value == nextMob)
                                iCounter++;
                            else
                                iCounter = 1;
                            string sMobText = nextMob.ToString();
                            if (iCounter > 1) sMobText += " 1";
                            prevMob = nextMob;
                            if (!entries.Contains(sMobText))
                            {
                                cboMob.Items.Add(sMobText);
                                entries.Add(sMobText);
                            }
                        }
                    }
                }
                if (!isCurrentRoom && cboMob.Items.Count > 0)
                {
                    cboMob.SelectedIndex = 0;
                }
            }
        }

        private void chkMagic_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUIFromEffectiveStrategy();
        }

        private void chkMelee_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUIFromEffectiveStrategy();
        }

        private void chkPotions_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUIFromEffectiveStrategy();
        }

        private void cboPawnShoppe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPawnShoppe.SelectedIndex > 0)
            {
                PawnShoppe ePawnShoppe = (PawnShoppe)cboPawnShoppe.SelectedItem;
                if (Enum.TryParse(ePawnShoppe.ToString(), out HealingRoom healingRoom))
                {
                    cboTickRoom.SelectedItem = healingRoom;
                }
            }
        }

        private void cboTickRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTickRoom.SelectedIndex > 0)
            {
                HealingRoom eHealingRoom = (HealingRoom)cboTickRoom.SelectedItem;
                if (Enum.TryParse(eHealingRoom.ToString(), out PawnShoppe pawnShoppe))
                {
                    cboPawnShoppe.SelectedItem = pawnShoppe;
                }
            }
        }
    }
}
