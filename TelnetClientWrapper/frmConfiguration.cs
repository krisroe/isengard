using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IsengardClient
{
    internal partial class frmConfiguration : Form
    {
        private IsengardSettingData _settings;
        private List<Strategy> _strategies;

        private int _currentAutoEscapeThreshold;
        private AutoEscapeType _currentAutoEscapeType;
        private bool _currentAutoEscapeActive;

        private AlignmentType _preferredAlignment;
        private Color _fullColor;
        private Color _emptyColor;

        private RealmType _currentRealm;

        private int _currentAutoSpellLevelMinimum;
        private int _currentAutoSpellLevelMaximum;
        internal const int AUTO_SPELL_LEVEL_MINIMUM = 1;
        internal const int AUTO_SPELL_LEVEL_MAXIMUM = 5;

        public frmConfiguration(IsengardSettingData settingsData, int autoEscapeThreshold, AutoEscapeType autoEscapeType, bool autoEscapeActive, List<Strategy> strategies)
        {
            InitializeComponent();

            _settings = settingsData;
            tsmiCurrentRealmEarth.Tag = RealmType.Earth;
            tsmiCurrentRealmFire.Tag = RealmType.Fire;
            tsmiCurrentRealmWater.Tag = RealmType.Water;
            tsmiCurrentRealmWind.Tag = RealmType.Wind;

            _currentRealm = settingsData.Realm;
            RefreshRealmUI();

            _currentAutoSpellLevelMaximum = settingsData.AutoSpellLevelMax;
            _currentAutoSpellLevelMinimum = settingsData.AutoSpellLevelMin;
            RefreshAutoSpellLevelUI();

            txtCurrentWeaponValue.Text = settingsData.Weapon.HasValue ? settingsData.Weapon.Value.ToString() : string.Empty;

            _preferredAlignment = settingsData.PreferredAlignment;
            RefreshAlignmentTypeUI();

            _currentAutoEscapeActive = autoEscapeActive;
            _currentAutoEscapeThreshold = autoEscapeThreshold;
            _currentAutoEscapeType = autoEscapeType;
            RefreshAutoEscapeUI();

            chkQueryMonsterStatus.Checked = settingsData.QueryMonsterStatus;
            chkVerboseOutput.Checked = settingsData.VerboseMode;
            chkRemoveAllOnStartup.Checked = settingsData.RemoveAllOnStartup;
            chkDisplayStunLength.Checked = settingsData.DisplayStunLength;

            _fullColor = settingsData.FullColor;
            SetColorUI(lblFullColorValue, _fullColor);
            _emptyColor = settingsData.EmptyColor;
            SetColorUI(lblEmptyColorValue, _emptyColor);

            //clone the strategies passed in
            _strategies = new List<Strategy>();
            foreach (Strategy s in strategies)
            {
                _strategies.Add(new Strategy(s));
            }

            RefreshStrategyList();

            foreach (DynamicDataItemClass nextItemClass in Enum.GetValues(typeof(DynamicDataItemClass)))
            {
                AddNewListEntry(nextItemClass);
            }
            foreach (ItemTypeEnum nextItem in Enum.GetValues(typeof(ItemTypeEnum)))
            {
                AddNewListEntry(nextItem);
            }
            RefreshAllItemEntries();
        }

        private void AddNewListEntry(object enumValue)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = enumValue.ToString();
            lvi.Tag = enumValue;
            lvi.SubItems.Add(string.Empty); //keep count
            lvi.SubItems.Add(string.Empty); //tick count
            lvi.SubItems.Add(string.Empty); //overflow action
            lvItems.Items.Add(lvi);
        }

        private void RefreshAllItemEntries()
        {
            foreach (ListViewItem lvi in lvItems.Items)
            {
                object oTag = lvi.Tag;
                DynamicDataItemListInfo listInfo;
                if (oTag is ItemTypeEnum)
                    listInfo = GetDynamicItemDataListInfo((ItemTypeEnum)oTag);
                else if (oTag is DynamicDataItemClass)
                    listInfo = GetDynamicItemDataListInfo((DynamicDataItemClass)oTag);
                else
                    throw new InvalidOperationException();
                SetListViewItemInfo(lvi, listInfo);
            }
        }
        
        private void SetListViewItemInfo(ListViewItem lvi, DynamicDataItemListInfo listInfo)
        {
            lvi.SubItems[1].Text = listInfo.KeepCountText;
            lvi.SubItems[2].Text = listInfo.TickCountText;
            lvi.SubItems[3].Text = listInfo.OverflowActionText;
        }

        private void RefreshStrategyList()
        {
            lstStrategies.Items.Clear();
            foreach (Strategy s in _strategies)
            {
                lstStrategies.Items.Add(s);
            }
        }

        public bool ChangedStrategies { get; set; }

        public List<Strategy> Strategies
        {
            get
            {
                return _strategies;
            }
        }

        private void SetColorUI(Label lbl, Color c)
        {
            lbl.BackColor = c;
            GetRGBString(c, out string colorText, out Color fgColor);
            lbl.Text = colorText;
            lbl.ForeColor = fgColor;
        }

        internal void GetRGBString(Color c, out string text, out Color foregroundColor)
        {
            text = c.R + "," + c.G + "," + c.B;
            int iForegroundR = c.R <= 127 ? 255 : 0;
            int iForegroundG = c.G <= 127 ? 255 : 0;
            int iForegroundB = c.B <= 127 ? 255 : 0;
            foregroundColor = Color.FromArgb(iForegroundR, iForegroundG, iForegroundB);
        }

        internal AlignmentType PreferredAlignment
        {
            get
            {
                return _preferredAlignment;
            }
        }

        internal ItemTypeEnum? Weapon
        {
            get;
            set;
        }

        internal bool QueryMonsterStatus
        {
            get
            {
                return chkQueryMonsterStatus.Checked;
            }
        }

        public bool VerboseOutput
        {
            get
            {
                return chkVerboseOutput.Checked;
            }
        }

        public bool RemoveAllOnStartup
        {
            get
            {
                return chkRemoveAllOnStartup.Checked;
            }
        }

        public bool DisplayStunLength
        {
            get
            {
                return chkDisplayStunLength.Checked;
            }
        }

        public Color FullColor
        {
            get
            {
                return _fullColor;
            }
        }

        public Color EmptyColor
        {
            get
            {
                return _emptyColor;
            }
        }

        public RealmType Realm
        {
            get
            {
                return _currentRealm;
            }
        }

        public int AutoSpellLevelMinimum
        {
            get
            {
                return _currentAutoSpellLevelMinimum;
            }
        }

        public int AutoSpellLevelMaximum
        {
            get
            {
                return _currentAutoSpellLevelMaximum;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IsengardSettings sets = IsengardSettings.Default;

            string sWeapon = txtCurrentWeaponValue.Text;
            if (string.IsNullOrEmpty(sWeapon))
            {
                Weapon = null;
            }
            else
            {
                if (!Enum.TryParse(sWeapon, out ItemTypeEnum weapon))
                {
                    MessageBox.Show("Invalid weapon");
                    txtCurrentWeaponValue.Focus();
                    return;
                }
                Weapon = weapon;
            }

            //CSRTODO: save changes to strategies

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region preferred alignment

        private void RefreshAlignmentTypeUI()
        {
            string sLabelText;
            Color cLabelBack;
            if (_preferredAlignment == AlignmentType.Blue)
            {
                sLabelText = "Good";
                cLabelBack = Color.Blue;
            }
            else
            {
                sLabelText = "Evil";
                cLabelBack = Color.Red;
            }
            lblPreferredAlignmentValue.BackColor = cLabelBack;
            lblPreferredAlignmentValue.Text = sLabelText;
        }

        private void tsmiPreferredAlignmentGood_Click(object sender, EventArgs e)
        {
            _preferredAlignment = AlignmentType.Blue;
            RefreshAlignmentTypeUI();
        }

        private void tsmiPreferredAlignmentEvil_Click(object sender, EventArgs e)
        {
            _preferredAlignment = AlignmentType.Red;
            RefreshAlignmentTypeUI();
        }

        #endregion

        #region current realm

        public RealmType CurrentRealm
        {
            get
            {
                return _currentRealm;
            }
        }

        private void RefreshRealmUI()
        {
            lblCurrentRealmValue.Text = _currentRealm.ToString();
            lblCurrentRealmValue.BackColor = UIShared.GetColorForRealm(_currentRealm);
        }

        private void tsmiCurrentRealm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            _currentRealm = (RealmType)tsmi.Tag;
            RefreshRealmUI();
        }

        private void ctxRealm_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (ToolStripMenuItem tsmi in new List<ToolStripMenuItem>() { tsmiCurrentRealmEarth, tsmiCurrentRealmFire, tsmiCurrentRealmWater, tsmiCurrentRealmWind })
            {
                RealmType eTag = (RealmType)tsmi.Tag;
                bool isSelected = eTag == _currentRealm;
                tsmi.Checked = isSelected;
                tsmi.Text = isSelected ? eTag + " (Current)" : eTag.ToString();
            }
        }

        #endregion

        #region auto spell levels

        public int CurrentAutoSpellLevelMin
        {
            get
            {
                return _currentAutoSpellLevelMinimum;
            }
        }

        public int CurrentAutoSpellLevelMax
        {
            get
            {
                return _currentAutoSpellLevelMaximum;
            }
        }

        private void RefreshAutoSpellLevelUI()
        {
            lblCurrentAutoSpellLevelsValue.Text = _currentAutoSpellLevelMinimum + ":" + _currentAutoSpellLevelMaximum;
        }

        private void tsmiSetCurrentMinimumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _currentAutoSpellLevelMinimum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMinimum = iLevel;
                if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMaximum = _currentAutoSpellLevelMinimum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetCurrentMaximumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string level = Interaction.InputBox("Level:", "Enter Level", _currentAutoSpellLevelMaximum.ToString());
            if (int.TryParse(level, out int iLevel) && iLevel >= AUTO_SPELL_LEVEL_MINIMUM && iLevel <= AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMaximum = iLevel;
                if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMinimum = _currentAutoSpellLevelMaximum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        #endregion

        #region auto escape

        public int CurrentAutoEscapeThreshold
        {
            get
            {
                return _currentAutoEscapeThreshold;
            }
        }

        public AutoEscapeType CurrentAutoEscapeType
        {
            get
            {
                return _currentAutoEscapeType;
            }
        }

        public bool CurrentAutoEscapeActive
        {
            get
            {
                return _currentAutoEscapeActive;
            }
        }

        private void RefreshAutoEscapeUI()
        {
            Color autoEscapeBackColor;
            string autoEscapeText;
            string sAutoEscapeType = _currentAutoEscapeType == AutoEscapeType.Hazy ? "Hazy" : "Flee";
            if (_currentAutoEscapeThreshold > 0)
            {
                autoEscapeText = sAutoEscapeType + " @ " + _currentAutoEscapeThreshold.ToString();
            }
            else
            {
                autoEscapeText = sAutoEscapeType;
            }
            if (_currentAutoEscapeActive)
            {
                if (_currentAutoEscapeType == AutoEscapeType.Hazy)
                {
                    autoEscapeBackColor = Color.DarkBlue;
                }
                else //Flee
                {
                    autoEscapeBackColor = Color.DarkRed;
                }
            }
            else if (_currentAutoEscapeThreshold > 0)
            {
                autoEscapeBackColor = Color.LightGray;
            }
            else
            {
                autoEscapeBackColor = Color.Black;
            }
            if (_currentAutoEscapeActive)
            {
                autoEscapeText += " (On)";
            }
            else
            {
                autoEscapeText += " (Off)";
            }

            UIShared.GetForegroundColor(autoEscapeBackColor.R, autoEscapeBackColor.G, autoEscapeBackColor.G, out byte forer, out byte foreg, out byte foreb);
            lblCurrentAutoEscapeValue.BackColor = autoEscapeBackColor;
            lblCurrentAutoEscapeValue.ForeColor = Color.FromArgb(forer, foreg, foreb);
            lblCurrentAutoEscapeValue.Text = autoEscapeText;
        }

        private void ctxAutoEscape_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tsmiCurrentAutoEscapeFlee.Checked = _currentAutoEscapeType == AutoEscapeType.Flee;
            tsmiCurrentAutoEscapeHazy.Checked = _currentAutoEscapeType == AutoEscapeType.Hazy;
            tsmiCurrentAutoEscapeActive.Checked = _currentAutoEscapeActive;
            tsmiCurrentAutoEscapeInactive.Checked = !_currentAutoEscapeActive;

            bool haveCurrentThreshold = _currentAutoEscapeThreshold > 0;
            tsmiClearCurrentAutoEscapeThreshold.Enabled = haveCurrentThreshold;
            tsmiCurrentAutoEscapeActive.Enabled = haveCurrentThreshold;
            tsmiCurrentAutoEscapeInactive.Enabled = haveCurrentThreshold;
        }

        private void tsmiSetCurrentAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            string sInitialValue = _currentAutoEscapeThreshold > 0 ? _currentAutoEscapeThreshold.ToString() : string.Empty;
            string threshold = Interaction.InputBox("Threshold:", "Enter Threshold", sInitialValue);
            if (int.TryParse(threshold, out int iThreshold) && iThreshold > 0)
            {
                _currentAutoEscapeThreshold = iThreshold;
                RefreshAutoEscapeUI();
            }
        }

        private void tsmiClearCurrentAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeThreshold = 0;
            _currentAutoEscapeActive = false;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeFlee_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeType = AutoEscapeType.Flee;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeHazy_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeType = AutoEscapeType.Hazy;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeActive_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeActive = true;
            RefreshAutoEscapeUI();
        }

        private void tsmiCurrentAutoEscapeInactive_Click(object sender, EventArgs e)
        {
            _currentAutoEscapeActive = false;
            RefreshAutoEscapeUI();
        }

        #endregion

        public string CurrentWeapon
        {
            get
            {
                return txtCurrentWeaponValue.Text ?? string.Empty;
            }
        }

        private void btnSelectFullColor_Click(object sender, EventArgs e)
        {
            Color? selected = PromptColor(_fullColor);
            if (selected.HasValue)
            {
                _fullColor = selected.Value;
                SetColorUI(lblFullColorValue, _fullColor);
            }
        }

        private void btnSelectEmptyColor_Click(object sender, EventArgs e)
        {
            Color? selected = PromptColor(_emptyColor);
            if (selected.HasValue)
            {
                _emptyColor = selected.Value;
                SetColorUI(lblEmptyColorValue, _emptyColor);
            }
        }

        private Color? PromptColor(Color initColor)
        {
            Color? ret = null;
            ColorDialog clg = new ColorDialog();
            clg.Color = initColor;
            if (clg.ShowDialog(this) == DialogResult.OK)
            {
                ret = clg.Color;
            }
            return ret;
        }

        private void ctxStrategies_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            if (iIndex < 0)
            {
                tsmiEditStrategy.Visible = false;
                tsmiRemoveStrategy.Visible = false;
                tsmiMoveStrategyUp.Visible = false;
                tsmiMoveStrategyDown.Visible = false;
            }
            else
            {
                tsmiEditStrategy.Visible = true;
                tsmiRemoveStrategy.Visible = true;
                tsmiMoveStrategyUp.Visible = iIndex > 0;
                tsmiMoveStrategyDown.Visible = iIndex < lstStrategies.Items.Count - 1;
            }
        }

        private void tsmiAddStrategy_Click(object sender, EventArgs e)
        {
            //CSRTODO: implement me!
            MessageBox.Show("Not currently implemented");
        }

        private void tsmiEditStrategy_Click(object sender, EventArgs e)
        {
            int index = lstStrategies.SelectedIndex;
            Strategy s = (Strategy)lstStrategies.Items[index];
            frmStrategy frm = new frmStrategy(s);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                s = frm.NewStrategy;
                _strategies[index] = s;
                lstStrategies.Items[index] = s;
                ChangedStrategies = true;
            }
        }

        private void tsmiRemoveStrategy_Click(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            _strategies.RemoveAt(iIndex);
            lstStrategies.Items.RemoveAt(iIndex);
            ChangedStrategies = true;
        }

        private void MoveStrategyUp(int iIndex)
        {
            Strategy s = (Strategy)lstStrategies.SelectedItem;
            lstStrategies.Items.RemoveAt(iIndex);
            lstStrategies.Items.Insert(iIndex - 1, s);
            _strategies.RemoveAt(iIndex);
            _strategies.Insert(iIndex - 1, s);
            lstStrategies.SelectedIndex = iIndex - 1;
            ChangedStrategies = true;
        }

        private void tsmiMoveStrategyUp_Click(object sender, EventArgs e)
        {
            MoveStrategyUp(lstStrategies.SelectedIndex);
        }

        private void tsmiMoveStrategyDown_Click(object sender, EventArgs e)
        {
            MoveStrategyUp(lstStrategies.SelectedIndex + 1);
        }

        private void ctxPreferredAlignment_Opening(object sender, CancelEventArgs e)
        {
            tsmiPreferredAlignmentGood.Checked = _preferredAlignment == AlignmentType.Blue;
            tsmiPreferredAlignmentEvil.Checked = _preferredAlignment == AlignmentType.Red;
        }

        private void tsmiRestoreDefaultStrategies_Click(object sender, EventArgs e)
        {
            _strategies = new List<Strategy>();
            foreach (Strategy s in Strategy.GetDefaultStrategies())
            {
                _strategies.Add(new Strategy(s));
            }
            RefreshStrategyList();
            ChangedStrategies = true;
        }

        private void lvItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnClear.Enabled = btnIgnore.Enabled = btnKeep.Enabled = lvItems.SelectedItems.Count > 0;
        }

        private void SetItemProperty(Action<DynamicItemData> SetPropertyAction)
        {
            bool refreshAllItems = false;
            foreach (ListViewItem lvi in lvItems.SelectedItems)
            {
                object oTag = lvi.Tag;
                bool hasData;
                if (oTag is ItemTypeEnum)
                {
                    ItemTypeEnum itemType = (ItemTypeEnum)oTag;
                    bool existed = _settings.DynamicItemData.TryGetValue(itemType, out DynamicItemData did);
                    if (!existed) did = new DynamicItemData();
                    SetPropertyAction(did);
                    hasData = did.HasData();
                    if (existed)
                    {
                        if (!hasData)
                        {
                            _settings.DynamicItemData.Remove(itemType);
                        }
                    }
                    else //new entry
                    {
                        if (hasData)
                        {
                            _settings.DynamicItemData[itemType] = did;
                        }
                    }
                    SetListViewItemInfo(lvi, GetDynamicItemDataListInfo(itemType));
                }
                else
                {
                    DynamicDataItemClass itemClass = (DynamicDataItemClass)oTag;
                    bool existed = _settings.DynamicItemClassData.TryGetValue(itemClass, out DynamicItemData did);
                    if (!existed) did = new DynamicItemData();
                    SetPropertyAction(did);
                    hasData = did.HasData();
                    if (existed)
                    {
                        if (!hasData)
                        {
                            _settings.DynamicItemClassData.Remove(itemClass);
                        }
                    }
                    else //new entry
                    {
                        if (hasData)
                        {
                            _settings.DynamicItemClassData[itemClass] = did;
                        }
                    }
                    refreshAllItems = true;
                }
            }
            if (refreshAllItems)
            {
                RefreshAllItemEntries();
            }
        }

        private class DynamicDataItemListInfo
        {
            public string KeepCountText { get; set; }
            public string TickCountText { get; set; }
            public string OverflowActionText { get; set; }
        }

        private DynamicDataItemListInfo GetDynamicItemDataListInfo(ItemTypeEnum itemType)
        {
            return GetDynamicItemDataListInfo(new DynamicItemDataWithInheritance(_settings, itemType));
        }

        private DynamicDataItemListInfo GetDynamicItemDataListInfo(DynamicDataItemClass itemClass)
        {
            return GetDynamicItemDataListInfo(new DynamicItemDataWithInheritance(_settings, itemClass));
        }

        private DynamicDataItemListInfo GetDynamicItemDataListInfo(DynamicItemDataWithInheritance did)
        {
            DynamicDataItemListInfo ret = new DynamicDataItemListInfo();
            string sNext;

            if (did.KeepCount < 0)
                sNext = "None";
            else if (did.KeepCount == int.MaxValue)
                sNext = "All";
            else
                sNext = did.KeepCount.ToString();
            if (did.KeepCount >= 0 && did.KeepCountInheritance.HasValue)
                sNext += " (" + did.KeepCountInheritance.Value.ToString() + ")";
            ret.KeepCountText = sNext;

            if (did.TickCount < 0)
                sNext = "None";
            else if (did.TickCount == int.MaxValue)
                sNext = "All";
            else
                sNext = did.TickCount.ToString();
            if (did.TickCount >= 0 && did.TickCountInheritance.HasValue)
                sNext += " (" + did.TickCountInheritance.Value.ToString() + ")";
            ret.TickCountText = sNext;

            sNext = did.OverflowAction.ToString();
            if (did.OverflowAction != ItemInventoryOverflowAction.None && did.OverflowActionInheritance.HasValue)
                sNext += " (" + did.OverflowActionInheritance.Value.ToString() + ")";
            ret.OverflowActionText = sNext;

            return ret;
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            SetItemProperty((did) => { did.OverflowAction = ItemInventoryOverflowAction.Ignore; });
        }

        private void btnSellOrJunk_Click(object sender, EventArgs e)
        {
            SetItemProperty((did) => { did.OverflowAction = ItemInventoryOverflowAction.SellOrJunk; });
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SetItemProperty((did) => { did.OverflowAction = ItemInventoryOverflowAction.None; });
        }

        private void btnTick_Click(object sender, EventArgs e)
        {
            string count = Interaction.InputBox("Tick Count:", "Enter count, negative to clear, all for no limit", string.Empty);
            int iCount;
            if (count.Equals("all", StringComparison.OrdinalIgnoreCase))
                iCount = int.MaxValue;
            else if (!int.TryParse(count, out iCount))
                return;
            SetItemProperty((did) => { did.TickCount = iCount < 0 ? -1 : iCount; });
        }

        private void btnKeep_Click(object sender, EventArgs e)
        {
            string count = Interaction.InputBox("Keep Count:", "Enter count, negative to clear, all for no limit", string.Empty);
            int iCount;
            if (count.Equals("all", StringComparison.OrdinalIgnoreCase))
                iCount = int.MaxValue;
            else if (!int.TryParse(count, out iCount))
                return;
            SetItemProperty((did) => { did.KeepCount = iCount < 0 ? -1 : iCount; });
        }
    }
}
