using IsengardClient.Backend;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPermRun : Form
    {
        private bool _initialized;
        private PermRun _permRun;
        private PermRunEditFlow _permRunEditFlow;
        private Func<GraphInputs> _GraphInputs;
        private IsengardMap _gameMap;
        private IsengardSettingData _settingsData;
        private Room _currentRoom;
        private CheckBox _chkPowerAttack;
        private CurrentEntityInfo _currentEntityInfo;
        private int _areaSelectedIndex;
        private Area _currentArea;
        /// <summary>
        /// current areas when editing a perm run
        /// </summary>
        private HashSet<Area> _currentAreasForEdit;

        private bool IsCombatStrategy { get; set; }
        private string MobText { get; set; }
        private MobTypeEnum? MobType { get; set; }
        private int MobIndex { get; set; }

        /// <summary>
        /// constructor used when initiating a perm run ad hoc from a strategy
        /// </summary>
        public frmPermRun(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, SupportedKeysFlags keys, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, Strategy strategy, ItemsToProcessType invWorkflow, CurrentEntityInfo currentEntityInfo, FullType beforeFull, FullType afterFull, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions, Area currentArea)
        {
            InitializeComponent();
            txtDisplayName.Enabled = false;
            StrategyOverrides stratAndOverrides = new StrategyOverrides();
            stratAndOverrides.AutoSpellLevelMin = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            stratAndOverrides.AutoSpellLevelMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            stratAndOverrides.UseMagicCombat = (strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            stratAndOverrides.UseMeleeCombat = (strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            stratAndOverrides.UsePotionsCombat = (strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            stratAndOverrides.Realms = null;
            stratAndOverrides.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            Initialize(gameMap, settingsData, skills, keys, currentRoom, currentMob, GetGraphInputs, invWorkflow, currentEntityInfo, beforeFull, afterFull, spellsCastOptions, spellsPotionsOptions, currentArea, currentArea, currentArea != null, false, stratAndOverrides, strategy);
        }

        /// <summary>
        /// constructor used when editing a perm run or using the change+run workflow
        /// </summary>
        public frmPermRun(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, SupportedKeysFlags keys, Room currentRoom, Func<GraphInputs> GetGraphInputs, CurrentEntityInfo currentEntityInfo, WorkflowSpells spellsCastOptions, WorkflowSpells spellsPotionsOptions, PermRun permRun, PermRunEditFlow permRunEditFlow, Area currentArea)
        {
            InitializeComponent();
            _permRun = permRun;
            _permRunEditFlow = permRunEditFlow;
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
            Area mostCompatibleArea = _permRun.DetermineMostCompatibleArea(currentArea, gameMap, GetGraphInputs());
            Initialize(gameMap, settingsData, skills, keys, currentRoom, sCurrentMob, GetGraphInputs, _permRun.ItemsToProcessType, currentEntityInfo, _permRun.BeforeFull, _permRun.AfterFull, spellsCastOptions, spellsPotionsOptions, currentArea, mostCompatibleArea, _permRun.Rehome, _permRun.RemoveAllEquipment, _permRun.StrategyOverrides, _permRun.Strategy);
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
        /// <param name="keysToShow">keys to show checkboxes for</param>
        /// <param name="currentArea">current area where the player currently is</param>
        /// <param name="targetArea">target area for the perm run</param>
        private void Initialize(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skillsToShow, SupportedKeysFlags keysToShow, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, ItemsToProcessType invWorkflow, CurrentEntityInfo currentEntityInfo, FullType beforeFull, FullType afterFull, WorkflowSpells spellsCastToShow, WorkflowSpells spellsPotionsToShow, Area currentArea, Area targetArea, bool rehome, bool removeAllEquipment, StrategyOverrides strategyAndOverrides, Strategy strategy)
        {
            _GraphInputs = GetGraphInputs;
            _gameMap = gameMap;
            _settingsData = settingsData;
            _currentRoom = currentRoom;
            _currentEntityInfo = currentEntityInfo;
            _currentArea = currentArea;

            foreach (Enum next in Enum.GetValues(typeof(FullType)))
            {
                cboBeforeFull.Items.Add(next);
                cboAfterFull.Items.Add(next);
            }

            cboBeforeFull.SelectedItem = beforeFull;
            cboAfterFull.SelectedItem = afterFull;

            ucStrategyModifications1.Initialize(strategyAndOverrides, strategy, !ForImmediateRun(), _settingsData, _permRunEditFlow, getSelectedStrategy, RefreshUIFromEffectiveStrategy);

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
                            isChecked = nextSkill == PromptedSkills.PowerAttack && strategy.IsCombatStrategy(CommandType.Melee, ucStrategyModifications1.GetEffectiveCombatTypesEnabled(strategy));
                            isChecked = true;
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
                        AddCheckbox(flpSpellsCast, nextSpell, isChecked);
                    }
                    if ((spellsPotionsToShow & nextSpell) == nextSpell)
                    {
                        if (_permRun == null)
                            isChecked = false;
                        else
                            isChecked = (_permRun.SpellsToPotion & nextSpell) != WorkflowSpells.None;
                        AddCheckbox(flpSpellsPotions, nextSpell, isChecked);
                    }
                }
            }
            foreach (SupportedKeysFlags nextKey in Enum.GetValues(typeof(SupportedKeysFlags)))
            {
                if (nextKey != SupportedKeysFlags.None && nextKey != SupportedKeysFlags.All)
                {
                    if ((keysToShow & nextKey) == nextKey)
                    {
                        if (_permRun == null)
                            isChecked = false;
                        else
                            isChecked = (_permRun.SupportedKeys & nextKey) != SupportedKeysFlags.None;
                        AddCheckbox(flpKeys, nextKey, isChecked);
                    }
                }
            }

            if (_permRunEditFlow == PermRunEditFlow.Edit)
            {
                cboArea.Visible = false;
                txtArea.Visible = true;
                btnEditAreas.Enabled = true;
                _currentAreasForEdit = _permRun.Areas;
                txtArea.Text = PermRun.GetAreaListAsText(_currentAreasForEdit);
            }
            else
            {
                cboArea.Visible = true;
                txtArea.Visible = false;
                btnEditAreas.Enabled = false;
                cboArea.Items.Add(string.Empty);
                foreach (Area a in settingsData.EnumerateAreas()) cboArea.Items.Add(a);
                if (targetArea == null)
                    cboArea.SelectedIndex = 0;
                else
                    cboArea.SelectedItem = targetArea;
                _areaSelectedIndex = cboArea.SelectedIndex;
            }
            chkRehome.Checked = rehome;
            chkRemoveAllEquipment.Checked = removeAllEquipment;

            if (_permRun != null && _permRun.Strategy == null)
            {
                cboStrategy.Items.Add(string.Empty);
            }
            foreach (Strategy s in settingsData.Strategies)
            {
                cboStrategy.Items.Add(s);
            }
            cboStrategy.SelectedItem = _permRun == null ? strategy : _permRun.Strategy;
            RefreshUIFromEffectiveStrategy();
            _initialized = true;
        }

        private Strategy getSelectedStrategy()
        {
            return cboStrategy.SelectedItem as Strategy;
        }

        private CheckBox AddCheckbox(FlowLayoutPanel flp, object tag, bool isChecked)
        {
            CheckBox chk = new CheckBox();
            chk.AutoSize = true;
            chk.Checked = isChecked;
            chk.Margin = new Padding(4);
            chk.Tag = tag;
            chk.Text = tag.ToString();
            chk.UseVisualStyleBackColor = true;
            flp.Controls.Add(chk);
            return chk;
        }

        /// <summary>
        /// refreshes the UI after the strategy dropdown changes
        /// </summary>
        private void RefreshUIFromStrategy()
        {
            Strategy Strategy = (Strategy)cboStrategy.SelectedItem;
            ucStrategyModifications1.RefreshUIFromStrategy(Strategy);
            RefreshUIFromEffectiveStrategy();
        }

        private void RefreshUIFromEffectiveStrategy()
        {
            CommandType eCombatTypes = ucStrategyModifications1.GetSelectedCombatTypes();
            bool showMob = eCombatTypes != CommandType.None;
            lblMob.Visible = showMob;
            cboMob.Visible = showMob;
            Strategy Strategy = (Strategy)cboStrategy.SelectedItem;
            string grpStrategyText;
            if (Strategy == null)
                grpStrategyText = "No Strategy Selected";
            else
                grpStrategyText = "Strategy (" + Strategy.GetToStringForCommandTypes(eCombatTypes) + ")";
            ucStrategyModifications1.SetGroupBoxText(grpStrategyText);
            if (_chkPowerAttack != null)
            {
                _chkPowerAttack.Visible = (eCombatTypes & CommandType.Melee) != CommandType.None;
            }
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

        private SupportedKeysFlags SelectedKeys
        {
            get
            {
                SupportedKeysFlags ret = SupportedKeysFlags.None;
                foreach (CheckBox nextKeyCheckbox in flpKeys.Controls)
                {
                    if (nextKeyCheckbox.Checked)
                    {
                        ret |= (SupportedKeysFlags)nextKeyCheckbox.Tag;
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


        private void btnOK_Click(object sender, EventArgs e)
        {
            Strategy strategySelectedInDropdown = cboStrategy.SelectedItem as Strategy;
            if (_permRun != null && strategySelectedInDropdown == null)
            {
                MessageBox.Show("No strategy selected.", "Perm Run");
                return;
            }
            IsCombatStrategy = strategySelectedInDropdown.IsCombatStrategy(CommandType.All, ucStrategyModifications1.GetEffectiveCombatTypesEnabled(strategySelectedInDropdown));
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

            if (chkRehome.Checked)
            {
                bool hasArea;
                if (_permRunEditFlow == PermRunEditFlow.Edit)
                    hasArea = _currentAreasForEdit != null;
                else
                    hasArea = cboArea.SelectedIndex > 0;
                if (!hasArea)
                {
                    MessageBox.Show("Cannot rehome without a selected area.");
                    return;
                }
            }

            if (ForImmediateRun()) //if running the perm run immediately, validate it can be run
            {
                ItemsToProcessType ipw = (ItemsToProcessType)cboItemsToProcessType.SelectedItem;
                if (ipw != ItemsToProcessType.NoProcessing && (cboArea.SelectedIndex == 0))
                {
                    if (MessageBox.Show("No area room selected. Continue?", "Inventory Processing", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return;
                    }
                }

                PermRun tempPermRun = new PermRun();
                SaveFormDataToPermRun(tempPermRun);
                if (!tempPermRun.IsRunnable(_GraphInputs, _currentEntityInfo, this, _gameMap, _currentArea))
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
            ClearRoomDropdown(cboThresholdRoom);
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

        private void ClearRoomDropdown(ComboBox dropdown)
        {
            dropdown.SelectedItem = null;
            dropdown.Items.Clear();
        }

        private void DisplayLocations(ComboBox roomDropdown)
        {
            frmLocations locationsForm = new frmLocations(_gameMap, _settingsData, _currentRoom, true, _GraphInputs, false);
            locationsForm.ShowDialog();
            UIShared.HandleRoomSelected(locationsForm.SelectedRoom, roomDropdown);
        }

        private void DisplayGraph(ComboBox roomDropdown)
        {
            Room contextRoom = roomDropdown.SelectedItem as Room;
            if (contextRoom == null && roomDropdown != cboTargetRoom) contextRoom = cboTargetRoom.SelectedItem as Room;
            if (contextRoom == null) contextRoom = _currentRoom;
            VertexSelectionRequirement vsr = _permRun == null ? VertexSelectionRequirement.ValidPathFromCurrentLocation : VertexSelectionRequirement.UnambiguousRoomBackendOrDisplayName;
            frmGraph graphForm = new frmGraph(_gameMap, contextRoom, true, _GraphInputs, vsr, false);
            graphForm.ShowDialog();
            UIShared.HandleRoomSelected(graphForm.SelectedRoom, roomDropdown);
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

        public void SaveFormDataToPermRun(PermRun pr)
        {
            pr.Rehome = chkRehome.Checked;
            if (_permRunEditFlow == PermRunEditFlow.Edit)
            {
                pr.Areas = _currentAreasForEdit;
            }
            else
            {
                Area a = cboArea.SelectedItem as Area;
                if (a == null)
                    pr.Areas = null;
                else
                    pr.Areas = new HashSet<Area>() { a };
            }

            ucStrategyModifications1.SaveChanges();
            pr.DisplayName = txtDisplayName.Text;
            pr.BeforeFull = (FullType)cboBeforeFull.SelectedItem;
            pr.AfterFull = (FullType)cboAfterFull.SelectedItem;
            pr.ItemsToProcessType = (ItemsToProcessType)cboItemsToProcessType.SelectedItem;
            pr.MobIndex = MobIndex;
            pr.MobText = MobText;
            pr.MobType = MobType;
            pr.SpellsToCast = GetSelectedSpells(flpSpellsCast);
            pr.SpellsToPotion = GetSelectedSpells(flpSpellsPotions);
            pr.SkillsToRun = SelectedSkills;
            pr.RemoveAllEquipment = chkRemoveAllEquipment.Checked;
            pr.SupportedKeys = SelectedKeys;
            pr.Strategy = (Strategy)cboStrategy.SelectedItem;
            Room rTemp;
            rTemp = (Room)cboTargetRoom.SelectedItem;
            pr.TargetRoomIdentifier = _gameMap.GetRoomTextIdentifier(rTemp);
            pr.TargetRoomObject = rTemp;
            rTemp = cboThresholdRoom.SelectedItem as Room;
            pr.ThresholdRoomIdentifier = rTemp == null ? string.Empty : _gameMap.GetRoomTextIdentifier(rTemp);
            pr.ThresholdRoomObject = rTemp;
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

        private bool ForImmediateRun()
        {
            return _permRun == null || _permRunEditFlow == PermRunEditFlow.ChangeAndRun;
        }

        private void cboArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                int iNewSelectedIndex = cboArea.SelectedIndex;
                if (_areaSelectedIndex == 0 && iNewSelectedIndex > 0)
                {
                    chkRehome.Checked = true;
                }
                _areaSelectedIndex = iNewSelectedIndex;
            }
        }

        private void btnEditAreas_Click(object sender, EventArgs e)
        {
            using (frmChooseAreas frm = new frmChooseAreas(_settingsData, _currentAreasForEdit))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _currentAreasForEdit = frm.SelectedAreas;
                    txtArea.Text = PermRun.GetAreaListAsText(_currentAreasForEdit);
                }
            }
        }
    }
}
