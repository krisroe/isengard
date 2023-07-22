using IsengardClient.Backend;
using System;
using System.Windows.Forms;

namespace IsengardClient
{
    public partial class ucStrategyModifications : UserControl
    {
        private StrategyOverridesUI _strategyOverridesUI;
        private StrategyOverrides _strategyOverrides;
        private bool _allowOverrides;
        private Control _rightClickControl;
        private PermRunEditFlow _permRunEditFlow;
        private Func<Strategy> _getStrategy;
        private Action _onCombatTypeCheckedOrUnchecked;
        private bool _initialized;

        public ucStrategyModifications()
        {
            InitializeComponent();

            foreach (Enum next in Enum.GetValues(typeof(AfterKillMonsterAction)))
            {
                cboOnKillMonster.Items.Add(next);
            }
        }

        public void Initialize(StrategyOverrides strategyOverrides, Strategy strategy, bool allowOverrides, IsengardSettingData settingsData, PermRunEditFlow permRunEditFlow, Func<Strategy> getStrategy, Action onCombatTypeCheckedOrUnchecked)
        {
            _strategyOverrides = strategyOverrides;
            _allowOverrides = allowOverrides;
            _permRunEditFlow = permRunEditFlow;
            _getStrategy = getStrategy;
            _onCombatTypeCheckedOrUnchecked = onCombatTypeCheckedOrUnchecked;
            int autoSpellLevelMin = strategyOverrides.AutoSpellLevelMin;
            int autoSpellLevelMax = strategyOverrides.AutoSpellLevelMax;
            RealmTypeFlags? realms = strategyOverrides.Realms;
            bool? useMagicCombat = strategyOverrides.UseMagicCombat;
            bool? useMeleeCombat = strategyOverrides.UseMeleeCombat;
            bool? usePotionsCombat = strategyOverrides.UsePotionsCombat;
            AfterKillMonsterAction? afterMonsterKillAction = strategyOverrides.AfterKillMonsterAction;

            bool useMagicCombatFromStrategy;
            bool useMeleeCombatFromStrategy;
            bool usePotionsCombatFromStrategy;
            AfterKillMonsterAction afterSkillMonsterActionFromStrategy;
            int strategyAutoSpellLevelMin, strategyAutoSpellLevelMax;
            RealmTypeFlags? strategyRealms;
            if (strategy == null)
            {
                useMagicCombatFromStrategy = useMeleeCombatFromStrategy = usePotionsCombatFromStrategy = false;
                afterSkillMonsterActionFromStrategy = AfterKillMonsterAction.StopCombat;
                strategyAutoSpellLevelMin = strategyAutoSpellLevelMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
                strategyRealms = null;
            }
            else
            {
                useMagicCombatFromStrategy = (strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
                useMeleeCombatFromStrategy = (strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
                usePotionsCombatFromStrategy = (strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
                afterSkillMonsterActionFromStrategy = strategy.AfterKillMonsterAction.GetValueOrDefault(AfterKillMonsterAction.StopCombat);
                strategyAutoSpellLevelMin = strategy.AutoSpellLevelMin;
                strategyAutoSpellLevelMax = strategy.AutoSpellLevelMax;
                strategyRealms = strategy.Realms;
            }
            if (allowOverrides)
            {
                cboOnKillMonster.Enabled = afterMonsterKillAction.HasValue;
                chkMagic.Enabled = useMagicCombat.HasValue;
                chkMelee.Enabled = useMeleeCombat.HasValue;
                chkPotions.Enabled = usePotionsCombat.HasValue;
            }
            else //add/edit a perm run
            {
                cboOnKillMonster.Enabled = true;
                chkMagic.Enabled = true;
                chkMelee.Enabled = true;
                chkPotions.Enabled = true;
            }
            chkMagic.Checked = useMagicCombat.GetValueOrDefault(useMagicCombatFromStrategy);
            chkMelee.Checked = useMeleeCombat.GetValueOrDefault(useMeleeCombatFromStrategy);
            chkPotions.Checked = usePotionsCombat.GetValueOrDefault(usePotionsCombatFromStrategy);
            cboOnKillMonster.SelectedIndex = (int)afterMonsterKillAction.GetValueOrDefault(afterSkillMonsterActionFromStrategy);
            _strategyOverridesUI = new StrategyOverridesUI(autoSpellLevelMin, autoSpellLevelMax, strategyAutoSpellLevelMin, strategyAutoSpellLevelMax, lblCurrentAutoSpellLevelsValue, realms, strategyRealms, lblCurrentRealmValue, StrategyOverridesLevel.PermRun, settingsData);
            _strategyOverridesUI.RefreshUI();
            _initialized = true;
        }

        private bool? UseMagicCombat
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

        private bool? UseMeleeCombat
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

        private bool? UsePotionsCombat
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

        public CommandType GetSelectedCombatTypes()
        {
            CommandType eCombatTypes = CommandType.None;
            if (chkMagic.Checked) eCombatTypes |= CommandType.Magic;
            if (chkMelee.Checked) eCombatTypes |= CommandType.Melee;
            if (chkPotions.Checked) eCombatTypes |= CommandType.Potions;
            return eCombatTypes;
        }

        public void RefreshUIFromStrategy(Strategy Strategy)
        {
            bool isChecked;
            if (!_allowOverrides || !chkMagic.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
                chkMagic.Checked = isChecked;
            }
            if (!_allowOverrides || !chkMelee.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
                chkMelee.Checked = isChecked;
            }
            if (!_allowOverrides || !chkPotions.Enabled)
            {
                if (Strategy == null)
                    isChecked = false;
                else
                    isChecked = (Strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
                chkPotions.Checked = isChecked;
            }
            AfterKillMonsterAction action;
            if (!_allowOverrides || !cboOnKillMonster.Enabled)
            {
                if (Strategy == null || !Strategy.AfterKillMonsterAction.HasValue)
                    action = AfterKillMonsterAction.StopCombat;
                else
                    action = Strategy.AfterKillMonsterAction.Value;
            }
            else
            {
                action = _strategyOverrides.AfterKillMonsterAction.Value;
            }
            cboOnKillMonster.SelectedIndex = (int)action;
            if (Strategy != null)
            {
                _strategyOverridesUI.StrategyAutoSpellLevelMinimum = Strategy.AutoSpellLevelMin;
                _strategyOverridesUI.StrategyAutoSpellLevelMaximum = Strategy.AutoSpellLevelMax;
                _strategyOverridesUI.StrategyRealms = Strategy.Realms;
            }
            else
            {
                _strategyOverridesUI.StrategyAutoSpellLevelMinimum = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
                _strategyOverridesUI.StrategyAutoSpellLevelMaximum = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
                _strategyOverridesUI.StrategyRealms = null;
            }
            _strategyOverridesUI.RefreshUI();
        }

        public CommandType GetEffectiveCombatTypesEnabled(Strategy s)
        {
            CommandType typesWithStepsEnabled = s.TypesWithStepsEnabled;
            if (_allowOverrides)
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

        private void pnlStrategyModifications_MouseUp(object sender, MouseEventArgs e)
        {
            Control ctl = pnlStrategyModifications.GetChildAtPoint(e.Location);
            if (ctl != null && !ctl.Enabled && (ctl == chkMagic || ctl == chkMelee || ctl == chkPotions || ctl == cboOnKillMonster))
            {
                _rightClickControl = ctl;
                ctl.ContextMenuStrip.Show(pnlStrategyModifications, e.Location);
            }
        }

        public void SaveChanges()
        {
            if (_permRunEditFlow == PermRunEditFlow.Edit)
            {
                _strategyOverrides.AutoSpellLevelMin = _strategyOverridesUI.PermRunAutoSpellLevelMinimum;
                _strategyOverrides.AutoSpellLevelMax = _strategyOverridesUI.PermRunAutoSpellLevelMaximum;
                _strategyOverrides.Realms = _strategyOverridesUI.PermRunRealms;
            }
            else
            {
                _strategyOverridesUI.GetEffectiveAutoSpellLevelsMinMax(out int iMin, out int iMax);
                _strategyOverrides.AutoSpellLevelMin = iMin;
                _strategyOverrides.AutoSpellLevelMax = iMax;
                _strategyOverrides.Realms = _strategyOverridesUI.GetEffectiveRealms();
            }
            _strategyOverrides.AfterKillMonsterAction = cboOnKillMonster.Enabled ? (AfterKillMonsterAction?)cboOnKillMonster.SelectedIndex : null;
            _strategyOverrides.UseMagicCombat = UseMagicCombat;
            _strategyOverrides.UseMeleeCombat = UseMeleeCombat;
            _strategyOverrides.UsePotionsCombat = UsePotionsCombat;
        }

        private void ctxToggleStrategyModificationOverride_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_allowOverrides)
            {
                Control sourceControl = ctxToggleStrategyModificationOverride.SourceControl;
                if (sourceControl == chkMagic || sourceControl == chkMelee || sourceControl == chkPotions || sourceControl == cboOnKillMonster)
                {
                    _rightClickControl = sourceControl;
                }
                tsmiToggleEnabled.Text = _rightClickControl.Enabled ? "Remove Override" : "Override";
            }
            else
            {
                e.Cancel = true;
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
                Strategy s = _getStrategy();
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

        public void SetGroupBoxText(String text)
        {
            grpStrategyModifications.Text = text;
        }

        private void chkCombatType_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _onCombatTypeCheckedOrUnchecked();
            }
        }
    }
}
