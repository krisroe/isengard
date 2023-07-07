using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPermRun : Form
    {
        private bool _initialized;
        private PermRun _permRun;
        private bool _forChangeAndRun;
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

        public ItemsToProcessType InventoryFlow
        {
            get
            {
                return (ItemsToProcessType)cboInventoryFlow.SelectedItem;
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

        public AfterKillMonsterAction? AfterKillMonsterAction
        {
            get
            {
                return cboOnKillMonster.Enabled ? (AfterKillMonsterAction?)cboOnKillMonster.SelectedIndex : null;
            }
        }

        public int AutoSpellLevelMin
        {
            get
            {
                return _autoSpellLevelInfo.PermRunMinimum;
            }
        }

        public int AutoSpellLevelMax
        {
            get
            {
                return _autoSpellLevelInfo.PermRunMaximum;
            }
        }

        public string DisplayName
        {
            get
            {
                return txtDisplayName.Text;
            }
        }

        public bool? UseMagicCombat
        {
            get
            {
                bool? ret;
                if (chkMagic.Enabled)
                    ret = chkMagic.Checked;
                else
                    ret = null;
                return ret;
            }
        }

        public bool? UseMeleeCombat
        {
            get
            {
                bool? ret;
                if (chkMelee.Enabled)
                    ret = chkMelee.Checked;
                else
                    ret = null;
                return ret;
            }
        }

        public bool? UsePotionsCombat
        {
            get
            {
                bool? ret;
                if (chkPotions.Enabled)
                    ret = chkPotions.Checked;
                else
                    ret = null;
                return ret;
            }
        }

        public bool IsCombatStrategy { get; set; }
        public string MobText { get; set; }
        public MobTypeEnum? MobType { get; set; }
        public int MobIndex { get; set; }

        /// <summary>
        /// constructor used when initiating a perm run ad hoc from a strategy
        /// </summary>
        public frmPermRun(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, Strategy strategy, HealingRoom? healingRoom, PawnShoppe? pawnShop, ItemsToProcessType invWorkflow, CurrentEntityInfo currentEntityInfo, bool fullBeforeStarting, bool fullAfterFinishing, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions)
        {
            InitializeComponent();
            txtDisplayName.Enabled = false;
            bool useMagic = (strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            bool useMelee = (strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            bool usePotions = (strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            Initialize(gameMap, settingsData, skills, currentRoom, currentMob, GetGraphInputs, strategy, healingRoom, pawnShop, invWorkflow, currentEntityInfo, fullBeforeStarting, fullAfterFinishing, spellsCastOptions, spellsPotionsOptions, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, useMagic, useMelee, usePotions, IsengardClient.AfterKillMonsterAction.StopCombat);
        }

        public frmPermRun(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, Room currentRoom, Func<GraphInputs> GetGraphInputs, CurrentEntityInfo currentEntityInfo, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions, PermRun permRun, bool forChangeAndRun)
        {
            InitializeComponent();
            _permRun = permRun;
            _forChangeAndRun = forChangeAndRun;
            txtDisplayName.Text = permRun.DisplayName;
            string sCurrentMob;
            if (_permRun.MobType.HasValue)
            {
                sCurrentMob = _permRun.MobType.Value.ToString();
            }
            else
            {
                sCurrentMob = _permRun.MobText;
            }
            if (!string.IsNullOrEmpty(sCurrentMob) && _permRun.MobIndex > 1)
            {
                sCurrentMob += " " + _permRun.MobIndex.ToString();
            }
            sCurrentMob = sCurrentMob ?? string.Empty;
            Initialize(gameMap, settingsData, skills, currentRoom, sCurrentMob, GetGraphInputs, _permRun.Strategy, _permRun.TickRoom, _permRun.PawnShop, _permRun.ItemsToProcessType, currentEntityInfo, _permRun.FullBeforeStarting, _permRun.FullAfterFinishing, spellsCastOptions, spellsPotionsOptions, _permRun.AutoSpellLevelMin, _permRun.AutoSpellLevelMax, _permRun.UseMagicCombat, _permRun.UseMeleeCombat, _permRun.UsePotionsCombat, _permRun.AfterKillMonsterAction);
        }

        private void Initialize(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, Strategy strategy, HealingRoom? healingRoom, PawnShoppe? pawnShop, ItemsToProcessType invWorkflow, CurrentEntityInfo currentEntityInfo, bool fullBeforeStarting, bool fullAfterFinishing, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions, int autoSpellLevelMin, int autoSpellLevelMax, bool? useMagicCombat, bool? useMeleeCombat, bool? usePotionsCombat, AfterKillMonsterAction? afterMonsterKillAction)
        {
            _GraphInputs = GetGraphInputs;
            _gameMap = gameMap;
            _settingsData = settingsData;
            _currentRoom = currentRoom;
            _currentEntityInfo = currentEntityInfo;

            chkFullBeforeStarting.Checked = fullBeforeStarting;
            chkFullAfterFinishing.Checked = fullAfterFinishing;

            foreach (Enum next in Enum.GetValues(typeof(AfterKillMonsterAction)))
            {
                cboOnKillMonster.Items.Add(next);
            }

            if (_permRun != null && !_forChangeAndRun)
            {
                cboOnKillMonster.Enabled = afterMonsterKillAction.HasValue;
                chkMagic.Enabled = useMagicCombat.HasValue;
                chkMagic.Checked = useMagicCombat.GetValueOrDefault(false);
                chkMelee.Enabled = useMeleeCombat.HasValue;
                chkMelee.Checked = useMeleeCombat.GetValueOrDefault(false);
                chkPotions.Enabled = usePotionsCombat.HasValue;
                chkPotions.Checked = usePotionsCombat.GetValueOrDefault(false);
            }
            else
            {
                cboOnKillMonster.Enabled = true;
                chkMagic.Enabled = true;
                chkMelee.Enabled = true;
                chkPotions.Enabled = true;
                chkMagic.Checked = useMagicCombat.Value;
                chkMelee.Checked = useMeleeCombat.Value;
                chkPotions.Checked = usePotionsCombat.Value;
            }

            int strategyAutoSpellLevelMin, strategyAutoSpellLevelMax;
            if (strategy == null)
            {
                strategyAutoSpellLevelMin = strategyAutoSpellLevelMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            }
            else
            {
                strategyAutoSpellLevelMin = strategy.AutoSpellLevelMin;
                strategyAutoSpellLevelMax = strategy.AutoSpellLevelMax;
            }
            _autoSpellLevelInfo = new AutoSpellLevelOverrides(autoSpellLevelMin, autoSpellLevelMax, strategyAutoSpellLevelMin, strategyAutoSpellLevelMax, _settingsData.AutoSpellLevelMin, _settingsData.AutoSpellLevelMax, lblCurrentAutoSpellLevelsValue, AutoSpellLevelOverridesLevel.PermRun);

            //populate tick room and pawn shop dropdowns
            cboTickRoom.Items.Add(string.Empty);
            cboPawnShoppe.Items.Add(string.Empty);
            foreach (var nextHealingRoom in Enum.GetValues(typeof(HealingRoom)))
            {
                cboTickRoom.Items.Add(nextHealingRoom);
            }
            foreach (var nextPawnShop in Enum.GetValues(typeof(PawnShoppe)))
            {
                cboPawnShoppe.Items.Add(nextPawnShop);
            }

            if (healingRoom.HasValue)
                cboTickRoom.SelectedItem = healingRoom.Value;
            else
                cboTickRoom.SelectedIndex = 0;
            if (pawnShop.HasValue)
                cboPawnShoppe.SelectedItem = pawnShop;
            else
                cboPawnShoppe.SelectedIndex = 0;

            cboInventoryFlow.Items.Add(ItemsToProcessType.NoProcessing);
            cboInventoryFlow.Items.Add(ItemsToProcessType.ProcessMonsterDrops);
            cboInventoryFlow.Items.Add(ItemsToProcessType.ProcessAllItemsInRoom);
            cboInventoryFlow.SelectedItem = invWorkflow;

            if (_permRun == null)
            {
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
            }
            else
            {
                if (currentRoom != null)
                {
                    cboRoom.Items.Add(currentRoom);
                    cboRoom.SelectedItem = currentRoom;
                }
            }
            if (cboMob.Items.Contains(currentMob))
            {
                cboMob.SelectedItem = currentMob;
            }
            else
            {
                cboMob.Text = currentMob;
            }

            bool isChecked;
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
                            _chkPowerAttack = chk;
                        }
                        if (_permRun == null)
                        {
                            isChecked = nextSkill == PromptedSkills.PowerAttack && strategy.IsCombatStrategy(CommandType.Melee, GetEffectiveCombatTypesEnabled(strategy));
                            if (nextSkill == PromptedSkills.PowerAttack)
                            {
                                chk.Visible = isChecked;
                            }
                        }
                        else
                        {
                            isChecked = (_permRun.SkillsToRun & nextSkill) != PromptedSkills.None;
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
                        if (_permRun == null)
                            isChecked = nextSpell == WorkflowSpells.Bless || nextSpell == WorkflowSpells.Protection || nextSpell == WorkflowSpells.CurePoison;
                        else
                            isChecked = (_permRun.SpellsToCast & nextSpell) != WorkflowSpells.None;
                        AddSpellCheckbox(flpSpellsCast, nextSpell, isChecked);
                    }
                    if ((spellsPotionsOptions & nextSpell) == nextSpell)
                    {
                        if (_permRun == null)
                            isChecked = false;
                        else
                            isChecked = (_permRun.SpellsToPotion & nextSpell) != WorkflowSpells.None;
                        AddSpellCheckbox(flpSpellsPotions, nextSpell, isChecked);
                    }
                }
            }

            if (_permRun != null)
            {
                cboStrategy.Items.Add(string.Empty);
            }
            foreach (Strategy s in settingsData.Strategies)
            {
                cboStrategy.Items.Add(s);
            }
            cboStrategy.SelectedItem = _permRun == null ? strategy : _permRun.Strategy;
            RefreshUIFromStrategy();
            _initialized = true;
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
            bool isChecked;
            if (_permRun == null || _forChangeAndRun || !chkMagic.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
                chkMagic.Checked = isChecked;
            }
            if (_permRun == null || _forChangeAndRun || !chkMelee.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
                chkMelee.Checked = isChecked;
            }
            if (_permRun == null || _forChangeAndRun || !chkPotions.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
                chkPotions.Checked = isChecked;
            }
            if (_permRun == null || _forChangeAndRun || !cboOnKillMonster.Enabled)
            {
                AfterKillMonsterAction action;
                if (Strategy == null)
                    action = IsengardClient.AfterKillMonsterAction.StopCombat;
                else
                    action = Strategy.AfterKillMonsterAction;
                cboOnKillMonster.SelectedIndex = (int)action;
            }
            if (Strategy != null)
            {
                _autoSpellLevelInfo.StrategyMinimum = Strategy.AutoSpellLevelMin;
                _autoSpellLevelInfo.StrategyMaximum = Strategy.AutoSpellLevelMax;
            }
            else
            {
                _autoSpellLevelInfo.StrategyMinimum = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
                _autoSpellLevelInfo.StrategyMaximum = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            }
            _autoSpellLevelInfo.RefreshAutoSpellLevelUI();

            RefreshUIFromEffectiveStrategy();
        }

        private void RefreshUIFromEffectiveStrategy()
        {
            CommandType eCombatTypes = GetSelectedCombatTypes();
            bool showMob = eCombatTypes != CommandType.None;
            lblMob.Visible = showMob;
            cboMob.Visible = showMob;
            Strategy Strategy = (Strategy)cboStrategy.SelectedItem;
            string grpStrategyText;
            if (Strategy == null)
                grpStrategyText = "No Strategy Selected";
            else
                grpStrategyText = "Strategy (" + Strategy.GetToStringForCommandTypes(eCombatTypes) + ")";
            grpStrategyModifications.Text = grpStrategyText;
            if (_chkPowerAttack != null)
            {
                _chkPowerAttack.Visible = (eCombatTypes & CommandType.Melee) != CommandType.None;
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

        private CommandType GetEffectiveCombatTypesEnabled(Strategy s)
        {
            CommandType typesWithStepsEnabled = s.TypesWithStepsEnabled;
            if (_permRun != null)
            {
                if (chkMagic.Enabled)
                {
                    if (chkMagic.Checked)
                        typesWithStepsEnabled |= CommandType.Magic;
                    else
                        typesWithStepsEnabled &= ~CommandType.Magic;
                }
                if (chkMelee.Enabled)
                {
                    if (chkMelee.Checked)
                        typesWithStepsEnabled |= CommandType.Melee;
                    else
                        typesWithStepsEnabled &= ~CommandType.Melee;
                }
                if (chkPotions.Enabled)
                {
                    if (chkPotions.Checked)
                        typesWithStepsEnabled |= CommandType.Potions;
                    else
                        typesWithStepsEnabled &= ~CommandType.Potions;
                }
            }
            return typesWithStepsEnabled;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_permRun != null && cboStrategy.SelectedIndex == 0)
            {
                MessageBox.Show("No strategy selected.", "Perm Run");
                return;
            }
            Strategy strategySelectedInDropdown = (Strategy)cboStrategy.SelectedItem;
            IsCombatStrategy = strategySelectedInDropdown.IsCombatStrategy(CommandType.All, GetEffectiveCombatTypesEnabled(strategySelectedInDropdown));
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
            else
            {
                MobText = string.Empty;
                MobType = null;
                MobIndex = 0;
            }

            Room targetRoom = cboRoom.SelectedItem as Room;
            if (targetRoom == null)
            {
                MessageBox.Show("No target room selected.");
                return;
            }

            if (_permRun == null)
            {
                Strategy selectedStrategy = new Strategy(strategySelectedInDropdown);
                selectedStrategy.AfterKillMonsterAction = (AfterKillMonsterAction)cboOnKillMonster.SelectedIndex;
                selectedStrategy.TypesWithStepsEnabled = GetSelectedCombatTypes();
                int iAutoSpellMin, iAutoSpellMax;
                _autoSpellLevelInfo.GetEffectiveMinMax(out iAutoSpellMin, out iAutoSpellMax);
                selectedStrategy.AutoSpellLevelMin = iAutoSpellMin;
                selectedStrategy.AutoSpellLevelMax = iAutoSpellMax;

                SelectedStrategy = selectedStrategy;

                ItemsToProcessType ipw = (ItemsToProcessType)cboInventoryFlow.SelectedItem;
                if (ipw != ItemsToProcessType.NoProcessing && (cboPawnShoppe.SelectedIndex == 0 || cboTickRoom.SelectedIndex == 0))
                {
                    if (MessageBox.Show("No pawn/tick room selected. Continue?", "Inventory Processing", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return;
                    }
                }

                GraphInputs graphInputs = _GraphInputs();

                //verify the target room is reachable
                if (targetRoom != _currentRoom)
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
                    if (targetRoom != healingRoom)
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
                    if (targetRoom != pawnShop)
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
            }
            else
            {
                SelectedStrategy = strategySelectedInDropdown;
                string sRoomTextIdentifier = _gameMap.GetRoomTextIdentifier(targetRoom);
                if (string.IsNullOrEmpty(sRoomTextIdentifier))
                {
                    MessageBox.Show("Cannot use this room because the backend and display names are ambiguous.", "Perm Run");
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            VertexSelectionRequirement vsr = _permRun == null ? VertexSelectionRequirement.ValidPathFromCurrentLocation : VertexSelectionRequirement.UnambiguousRoomBackendOrDisplayName;
            frmGraph graphForm = new frmGraph(_gameMap, _currentRoom, true, _GraphInputs, vsr, false);
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
            frmLocations locationsForm = new frmLocations(_gameMap, _settingsData, _currentRoom, true, _GraphInputs, false);
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
            if (_initialized)
            {
                RefreshUIFromEffectiveStrategy();
            }
        }

        private void chkMelee_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                RefreshUIFromEffectiveStrategy();
            }
        }

        private void chkPotions_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                RefreshUIFromEffectiveStrategy();
            }
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

        private Control _rightClickControl;

        private void ctxToggleStrategyModificationOverride_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_permRun == null || _forChangeAndRun)
            {
                e.Cancel = true;
            }
            else
            {
                tsmiToggleEnabled.Text = _rightClickControl.Enabled ? "Remove Override" : "Override";
            }
        }

        private void tsmiToggleEnabled_Click(object sender, EventArgs e)
        {
            if (tsmiToggleEnabled.Text == "Override")
            {
                _rightClickControl.Enabled = true;
            }
            else
            {
                _rightClickControl.Enabled = false;
                Strategy s = cboStrategy.SelectedItem as Strategy;
                bool haveStrategy = s != null;
                if (_rightClickControl == chkMagic)
                {
                    chkMagic.Checked = haveStrategy && ((s.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None);
                }
                else if (_rightClickControl == chkMelee)
                {
                    chkMelee.Checked = haveStrategy && ((s.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None);
                }
                else if (_rightClickControl == chkPotions)
                {
                    chkPotions.Checked = haveStrategy && ((s.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None);
                }
                else if (_rightClickControl == cboOnKillMonster)
                {
                    cboOnKillMonster.SelectedIndex = (int)(haveStrategy ? s.AfterKillMonsterAction : IsengardClient.AfterKillMonsterAction.StopCombat);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void SaveFormDataToPermRun(PermRun pr)
        {
            pr.AfterKillMonsterAction = AfterKillMonsterAction;
            pr.AutoSpellLevelMax = AutoSpellLevelMin;
            pr.AutoSpellLevelMax = AutoSpellLevelMax;
            pr.DisplayName = DisplayName;
            pr.FullBeforeStarting = FullBeforeStarting;
            pr.FullAfterFinishing = FullAfterFinishing;
            pr.ItemsToProcessType = InventoryFlow;
            pr.MobIndex = MobIndex;
            pr.MobText = MobText;
            pr.MobType = MobType;
            pr.PawnShop = PawnShop;
            pr.TickRoom = HealingRoom;
            pr.SpellsToCast = SelectedCastSpells;
            pr.SpellsToPotion = SelectedPotionsSpells;
            pr.SkillsToRun = SelectedSkills;
            pr.Strategy = SelectedStrategy;
            pr.TargetRoom = _gameMap.GetRoomTextIdentifier(TargetRoom);
            pr.TargetRoomObject = TargetRoom;
            pr.UseMagicCombat = UseMagicCombat;
            pr.UseMeleeCombat = UseMeleeCombat;
            pr.UsePotionsCombat = UsePotionsCombat;
        }

        /// <summary>
        /// removes the blank strategy item once a strategy is selected
        /// </summary>
        private void cboStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            Strategy s = cboStrategy.SelectedItem as Strategy;
            if (s != null)
            {
                Strategy sFirst = cboStrategy.Items[0] as Strategy;
                if (sFirst == null)
                {
                    cboStrategy.Items.RemoveAt(0);
                }
            }
            if (_initialized)
            {
                RefreshUIFromStrategy();
            }
        }

        private void pnlStrategyModifications_MouseUp(object sender, MouseEventArgs e)
        {
            Control ctl = pnlStrategyModifications.GetChildAtPoint(e.Location);
            if (ctl != null && !ctl.Enabled && (ctl == chkMagic || ctl == chkMelee || ctl == chkPotions || ctl == cboOnKillMonster))
            {
                _rightClickControl = ctl;
                ctl.ContextMenuStrip.Show(pnlStrategyModifications, e.Location);
            }
        }
    }
}
