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

        private bool IsCombatStrategy { get; set; }
        private string MobText { get; set; }
        private MobTypeEnum? MobType { get; set; }
        private int MobIndex { get; set; }

        /// <summary>
        /// constructor used when initiating a perm run ad hoc from a strategy
        /// </summary>
        public frmPermRun(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, Strategy strategy, HealingRoom? healingRoom, PawnShoppe? pawnShop, ItemsToProcessType invWorkflow, CurrentEntityInfo currentEntityInfo, FullType beforeFull, FullType afterFull, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions)
        {
            InitializeComponent();
            txtDisplayName.Enabled = false;
            bool useMagic = (strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            bool useMelee = (strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            bool usePotions = (strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            Initialize(gameMap, settingsData, skills, currentRoom, currentMob, GetGraphInputs, strategy, healingRoom, pawnShop, invWorkflow, currentEntityInfo, beforeFull, afterFull, spellsCastOptions, spellsPotionsOptions, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, useMagic, useMelee, usePotions, IsengardClient.AfterKillMonsterAction.StopCombat);
        }

        public frmPermRun(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, Room currentRoom, Func<GraphInputs> GetGraphInputs, CurrentEntityInfo currentEntityInfo, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions, PermRun permRun, bool forChangeAndRun)
        {
            InitializeComponent();
            _permRun = permRun;
            _forChangeAndRun = forChangeAndRun;
            txtDisplayName.Text = permRun.DisplayName;
            string sCurrentMob = string.Empty;
            bool hasMob = false;
            int iMobIndex = _permRun.MobIndex;
            if (_permRun.MobType.HasValue)
            {
                sCurrentMob = _permRun.MobType.Value.ToString();
                hasMob = true;
            }
            else if (!string.IsNullOrEmpty(_permRun.MobText))
            {
                sCurrentMob = _permRun.MobText;
                hasMob = true;
            }
            else if (iMobIndex >= 1)
            {
                sCurrentMob = iMobIndex.ToString();
            }
            if (hasMob && iMobIndex > 1)
            {
                sCurrentMob += " " + iMobIndex.ToString();
            }
            sCurrentMob = sCurrentMob ?? string.Empty;
            Initialize(gameMap, settingsData, skills, currentRoom, sCurrentMob, GetGraphInputs, _permRun.Strategy, _permRun.TickRoom, _permRun.PawnShop, _permRun.ItemsToProcessType, currentEntityInfo, _permRun.BeforeFull, _permRun.AfterFull, spellsCastOptions, spellsPotionsOptions, _permRun.AutoSpellLevelMin, _permRun.AutoSpellLevelMax, _permRun.UseMagicCombat, _permRun.UseMeleeCombat, _permRun.UsePotionsCombat, _permRun.AfterKillMonsterAction);
        }

        /// <summary>
        /// initializes the form
        /// </summary>
        /// <param name="gameMap">game map</param>
        /// <param name="settingsData">settings data</param>
        /// <param name="currentRoom">current room for an ad hoc or new perm run, target room for existing perm run</param>
        /// <param name="skillsToShow">skills to show checkboxes for</param>
        /// <param name="spellsCastToShow">cast spells to show checkboxes for</param>
        /// <param name="spellsPotionsToShow">potions spells to show checkboxes for</param>
        private void Initialize(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skillsToShow, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, Strategy strategy, HealingRoom? healingRoom, PawnShoppe? pawnShop, ItemsToProcessType invWorkflow, CurrentEntityInfo currentEntityInfo, FullType beforeFull, FullType afterFull, WorkflowSpells spellsCastToShow, WorkflowSpells spellsPotionsToShow, int autoSpellLevelMin, int autoSpellLevelMax, bool? useMagicCombat, bool? useMeleeCombat, bool? usePotionsCombat, AfterKillMonsterAction? afterMonsterKillAction)
        {
            _GraphInputs = GetGraphInputs;
            _gameMap = gameMap;
            _settingsData = settingsData;
            _currentRoom = currentRoom;
            _currentEntityInfo = currentEntityInfo;

            foreach (Enum next in Enum.GetValues(typeof(FullType)))
            {
                cboBeforeFull.Items.Add(next);
                cboAfterFull.Items.Add(next);
            }
            foreach (Enum next in Enum.GetValues(typeof(AfterKillMonsterAction)))
            {
                cboOnKillMonster.Items.Add(next);
            }

            cboBeforeFull.SelectedItem = beforeFull;
            cboAfterFull.SelectedItem = afterFull;

            bool useMagicCombatFromStrategy;
            bool useMeleeCombatFromStrategy;
            bool usePotionsCombatFromStrategy;
            if (strategy == null)
            {
                useMagicCombatFromStrategy = useMeleeCombatFromStrategy = usePotionsCombatFromStrategy = false;
            }
            else
            {
                useMagicCombatFromStrategy = (strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
                useMeleeCombatFromStrategy = (strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
                usePotionsCombatFromStrategy = (strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            }
            if (ForImmediateRun())
            {
                cboOnKillMonster.Enabled = true;
                chkMagic.Enabled = true;
                chkMelee.Enabled = true;
                chkPotions.Enabled = true;
            }
            else //add/edit a perm run
            {
                cboOnKillMonster.Enabled = afterMonsterKillAction.HasValue;
                chkMagic.Enabled = useMagicCombat.HasValue;
                chkMelee.Enabled = useMeleeCombat.HasValue;
                chkPotions.Enabled = usePotionsCombat.HasValue;
            }
            chkMagic.Checked = useMagicCombat.GetValueOrDefault(useMagicCombatFromStrategy);
            chkMelee.Checked = useMeleeCombat.GetValueOrDefault(useMeleeCombatFromStrategy);
            chkPotions.Checked = usePotionsCombat.GetValueOrDefault(usePotionsCombatFromStrategy);

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

            cboItemsToProcessType.Items.Add(ItemsToProcessType.NoProcessing);
            cboItemsToProcessType.Items.Add(ItemsToProcessType.ProcessMonsterDrops);
            cboItemsToProcessType.Items.Add(ItemsToProcessType.ProcessAllItemsInRoom);
            cboItemsToProcessType.SelectedItem = invWorkflow;

            if (_permRun == null)
            {
                if (currentRoom == null) //no current room means no way of traveling anywhere
                {
                    cboTargetRoom.Enabled = false;
                    cboThresholdRoom.Enabled = false;
                }
                else
                {
                    cboTargetRoom.Items.Add(currentRoom);
                    foreach (Exit nextEdge in currentRoom.Exits)
                    {
                        Room targetRoom = nextEdge.Target;
                        if (currentRoom != targetRoom)
                        {
                            cboTargetRoom.Items.Add(targetRoom);
                        }
                    }
                    cboTargetRoom.SelectedItem = currentRoom;
                }
            }
            else //add the existing perm run rooms to the dropdowns
            {
                if (currentRoom != null)
                {
                    cboTargetRoom.Items.Add(currentRoom);
                    cboTargetRoom.SelectedItem = currentRoom;
                }
                if (_permRun.ThresholdRoomObject != null)
                {
                    cboThresholdRoom.Items.Add(_permRun.ThresholdRoomObject);
                    cboThresholdRoom.SelectedItem = _permRun.ThresholdRoomObject;
                }
            }
            if (cboMob.Items.Contains(currentMob))
            {
                cboMob.SelectedItem = currentMob;
            }
            else if (!string.IsNullOrEmpty(currentMob))
            {
                cboMob.Text = currentMob;
            }

            bool isChecked;
            foreach (PromptedSkills nextSkill in Enum.GetValues(typeof(PromptedSkills)))
            {
                if (nextSkill != PromptedSkills.None)
                {
                    if ((skillsToShow & nextSkill) == nextSkill)
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
                        chk.Checked = isChecked;
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
                    if ((spellsCastToShow & nextSpell) == nextSpell)
                    {
                        if (_permRun == null)
                            isChecked = nextSpell == WorkflowSpells.Bless || nextSpell == WorkflowSpells.Protection || nextSpell == WorkflowSpells.CurePoison;
                        else
                            isChecked = (_permRun.SpellsToCast & nextSpell) != WorkflowSpells.None;
                        AddSpellCheckbox(flpSpellsCast, nextSpell, isChecked);
                    }
                    if ((spellsPotionsToShow & nextSpell) == nextSpell)
                    {
                        if (_permRun == null)
                            isChecked = false;
                        else
                            isChecked = (_permRun.SpellsToPotion & nextSpell) != WorkflowSpells.None;
                        AddSpellCheckbox(flpSpellsPotions, nextSpell, isChecked);
                    }
                }
            }

            if (_permRun != null && _permRun.Strategy == null)
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
            bool forImmediateRun = ForImmediateRun();
            if (forImmediateRun || !chkMagic.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
                chkMagic.Checked = isChecked;
            }
            if (forImmediateRun || !chkMelee.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
                chkMelee.Checked = isChecked;
            }
            if (forImmediateRun || !chkPotions.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
                chkPotions.Checked = isChecked;
            }
            AfterKillMonsterAction action;
            if (forImmediateRun || !cboOnKillMonster.Enabled)
            {
                if (Strategy == null)
                    action = AfterKillMonsterAction.StopCombat;
                else
                    action = Strategy.AfterKillMonsterAction;
            }
            else
            {
                action = _permRun.AfterKillMonsterAction.Value;
            }
            cboOnKillMonster.SelectedIndex = (int)action;
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

        private PromptedSkills SelectedSkills
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
            Strategy strategySelectedInDropdown = cboStrategy.SelectedItem as Strategy;
            if (_permRun != null && strategySelectedInDropdown == null)
            {
                MessageBox.Show("No strategy selected.", "Perm Run");
                return;
            }
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

            Room targetRoom = cboTargetRoom.SelectedItem as Room;
            if (targetRoom == null)
            {
                MessageBox.Show("No target room selected.");
                return;
            }

            Room thresholdRoom = cboThresholdRoom.SelectedItem as Room;
            if (thresholdRoom == targetRoom)
            {
                MessageBox.Show("Threshold room cannot be the same as the target room.");
                return;
            }

            if (ForImmediateRun()) //if running the perm run immediately, validate it can be run
            {
                ItemsToProcessType ipw = (ItemsToProcessType)cboItemsToProcessType.SelectedItem;
                if (ipw != ItemsToProcessType.NoProcessing && (cboPawnShoppe.SelectedIndex == 0 || cboTickRoom.SelectedIndex == 0))
                {
                    if (MessageBox.Show("No pawn/tick room selected. Continue?", "Inventory Processing", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return;
                    }
                }

                PermRun tempPermRun = new PermRun();
                SaveFormDataToPermRun(tempPermRun);
                if (!tempPermRun.IsRunnable(_GraphInputs, _currentEntityInfo, this, _gameMap))
                {
                    return;
                }
            }
            else
            {
                string sRoomTextIdentifier = _gameMap.GetRoomTextIdentifier(targetRoom);
                if (string.IsNullOrEmpty(sRoomTextIdentifier))
                {
                    MessageBox.Show("Cannot use this target room because the backend and display names are ambiguous.", "Perm Run");
                    return;
                }
                if (thresholdRoom != null)
                {
                    sRoomTextIdentifier = _gameMap.GetRoomTextIdentifier(thresholdRoom);
                    if (string.IsNullOrEmpty(sRoomTextIdentifier))
                    {
                        MessageBox.Show("Cannot use this threshold room because the backend and display names are ambiguous.", "Perm Run");
                        return;
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnThresholdClear_Click(object sender, EventArgs e)
        {
            cboThresholdRoom.SelectedItem = null;
            cboThresholdRoom.Items.Clear();
        }

        private void btnThresholdLocations_Click(object sender, EventArgs e)
        {
            DisplayLocations(cboThresholdRoom);
        }

        private void btnTargetLocations_Click(object sender, EventArgs e)
        {
            DisplayLocations(cboTargetRoom);
        }

        private void btnThresholdGraph_Click(object sender, EventArgs e)
        {
            DisplayGraph(cboThresholdRoom);
        }

        private void btnTargetGraph_Click(object sender, EventArgs e)
        {
            DisplayGraph(cboTargetRoom);
        }

        private void DisplayLocations(ComboBox roomDropdown)
        {
            frmLocations locationsForm = new frmLocations(_gameMap, _settingsData, _currentRoom, true, _GraphInputs, false);
            locationsForm.ShowDialog();
            HandleRoomSelected(locationsForm.SelectedRoom, roomDropdown);
        }

        private void DisplayGraph(ComboBox roomDropdown)
        {
            Room contextRoom = roomDropdown.SelectedItem as Room;
            if (contextRoom == null && roomDropdown != cboTargetRoom) contextRoom = cboTargetRoom.SelectedItem as Room;
            if (contextRoom == null) contextRoom = _currentRoom;
            VertexSelectionRequirement vsr = _permRun == null ? VertexSelectionRequirement.ValidPathFromCurrentLocation : VertexSelectionRequirement.UnambiguousRoomBackendOrDisplayName;
            frmGraph graphForm = new frmGraph(_gameMap, contextRoom, true, _GraphInputs, vsr, false);
            graphForm.ShowDialog();
            HandleRoomSelected(graphForm.SelectedRoom, roomDropdown);
        }

        private void HandleRoomSelected(Room selectedRoom, ComboBox roomDropdown)
        {
            if (selectedRoom != null)
            {
                if (!roomDropdown.Items.Contains(selectedRoom))
                {
                    roomDropdown.Items.Add(selectedRoom);
                }
                roomDropdown.SelectedItem = selectedRoom;
            }
        }

        private void cboRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            Room r = (Room)cboTargetRoom.SelectedItem;
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
                if (_initialized)
                {
                    if (cboMob.Items.Count > 0)
                    {
                        cboMob.SelectedIndex = 0;
                    }
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
                Control sourceControl = ctxToggleStrategyModificationOverride.SourceControl;
                if (sourceControl == chkMagic || sourceControl == chkMelee || sourceControl == chkPotions || sourceControl == cboOnKillMonster)
                {
                    _rightClickControl = sourceControl;
                }
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
                    cboOnKillMonster.SelectedIndex = (int)(haveStrategy ? s.AfterKillMonsterAction : AfterKillMonsterAction.StopCombat);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void SaveFormDataToPermRun(PermRun pr)
        {
            pr.AfterKillMonsterAction = cboOnKillMonster.Enabled ? (AfterKillMonsterAction?)cboOnKillMonster.SelectedIndex : null;
            pr.AutoSpellLevelMin = _autoSpellLevelInfo.PermRunMinimum;
            pr.AutoSpellLevelMax = _autoSpellLevelInfo.PermRunMaximum;
            pr.DisplayName = txtDisplayName.Text;
            pr.BeforeFull = (FullType)cboBeforeFull.SelectedItem;
            pr.AfterFull = (FullType)cboAfterFull.SelectedItem;
            pr.ItemsToProcessType = (ItemsToProcessType)cboItemsToProcessType.SelectedItem;
            pr.MobIndex = MobIndex;
            pr.MobText = MobText;
            pr.MobType = MobType;
            pr.PawnShop = cboPawnShoppe.SelectedIndex == 0 ? (PawnShoppe?)null : (PawnShoppe)cboPawnShoppe.SelectedItem;
            pr.TickRoom = cboTickRoom.SelectedIndex == 0 ? (HealingRoom?)null : (HealingRoom)cboTickRoom.SelectedItem;
            pr.SpellsToCast = GetSelectedSpells(flpSpellsCast);
            pr.SpellsToPotion = GetSelectedSpells(flpSpellsPotions);
            pr.SkillsToRun = SelectedSkills;
            pr.Strategy = (Strategy)cboStrategy.SelectedItem;
            Room oTargetRoom = (Room)cboTargetRoom.SelectedItem;
            pr.TargetRoomIdentifier = _gameMap.GetRoomTextIdentifier(oTargetRoom);
            pr.TargetRoomObject = oTargetRoom;
            Room oThresholdRoom = cboThresholdRoom.SelectedItem as Room;
            pr.ThresholdRoomIdentifier = oThresholdRoom == null ? string.Empty : _gameMap.GetRoomTextIdentifier(oThresholdRoom);
            pr.ThresholdRoomObject = oThresholdRoom;
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
                    ItemsToProcessType itemsToProcessTypeToSelect;
                    if (s.TypesWithStepsEnabled == CommandType.None)
                        itemsToProcessTypeToSelect = ItemsToProcessType.ProcessAllItemsInRoom;
                    else
                        itemsToProcessTypeToSelect = ItemsToProcessType.ProcessMonsterDrops;
                    cboItemsToProcessType.SelectedIndex = (int)itemsToProcessTypeToSelect;
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

        private bool ForImmediateRun()
        {
            return _permRun == null || _forChangeAndRun;
        }
    }
}
