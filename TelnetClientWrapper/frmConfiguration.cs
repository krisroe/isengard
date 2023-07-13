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
        private IsengardMap _gameMap;
        private Func<GraphInputs> _getGraphInputs;
        private CurrentEntityInfo _cei;

        private int _currentAutoEscapeThreshold;
        private AutoEscapeType _currentAutoEscapeType;
        private bool _currentAutoEscapeActive;

        private AlignmentType _preferredAlignment;
        private Color _fullColor;
        private Color _emptyColor;

        private RealmType _currentRealm;

        private AutoSpellLevelOverrides _autoSpellLevelOverrides;


        public frmConfiguration(IsengardSettingData settingsData, int autoEscapeThreshold, AutoEscapeType autoEscapeType, bool autoEscapeActive, IsengardMap gameMap, Func<GraphInputs> getGraphInputs, CurrentEntityInfo cei)
        {
            InitializeComponent();

            _settings = settingsData;
            _gameMap = gameMap;
            _getGraphInputs = getGraphInputs;
            _cei = cei;

            tsmiCurrentRealmEarth.Tag = RealmType.Earth;
            tsmiCurrentRealmFire.Tag = RealmType.Fire;
            tsmiCurrentRealmWater.Tag = RealmType.Water;
            tsmiCurrentRealmWind.Tag = RealmType.Wind;

            _currentRealm = settingsData.Realm;
            RefreshRealmUI();

            _autoSpellLevelOverrides = new AutoSpellLevelOverrides(IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, settingsData.AutoSpellLevelMin, settingsData.AutoSpellLevelMax, lblCurrentAutoSpellLevelsValue, AutoSpellLevelOverridesLevel.Settings);

            txtCurrentWeaponValue.Text = settingsData.Weapon.HasValue ? settingsData.Weapon.Value.ToString() : string.Empty;
            txtHeldItem.Text = settingsData.HeldItem.HasValue ? settingsData.HeldItem.Value.ToString() : string.Empty;

            _preferredAlignment = settingsData.PreferredAlignment;
            RefreshAlignmentTypeUI();

            _currentAutoEscapeActive = autoEscapeActive;
            _currentAutoEscapeThreshold = autoEscapeThreshold;
            _currentAutoEscapeType = autoEscapeType;
            RefreshAutoEscapeUI();

            chkQueryMonsterStatus.Checked = settingsData.QueryMonsterStatus;
            chkGetNewPermRunOnBoatExitMissing.Checked = settingsData.GetNewPermRunOnBoatExitMissing;
            cboConsoleVerbosity.SelectedIndex = (int)settingsData.ConsoleVerbosity;
            chkRemoveAllOnStartup.Checked = settingsData.RemoveAllOnStartup;
            chkDisplayStunLength.Checked = settingsData.DisplayStunLength;
            chkSaveSettingsOnQuit.Checked = settingsData.SaveSettingsOnQuit;

            txtMagicVigorWhenDownXHP.Text = settingsData.MagicVigorOnlyWhenDownXHP <= 0 ? string.Empty : settingsData.MagicVigorOnlyWhenDownXHP.ToString();
            txtMagicMendWhenDownXHP.Text = settingsData.MagicMendOnlyWhenDownXHP <= 0 ? string.Empty :  settingsData.MagicMendOnlyWhenDownXHP.ToString();
            txtPotionsVigorWhenDownXHP.Text = settingsData.PotionsVigorOnlyWhenDownXHP <= 0 ? string.Empty : settingsData.PotionsVigorOnlyWhenDownXHP.ToString();
            txtPotionsMendWhenDownXHP.Text = settingsData.PotionsMendOnlyWhenDownXHP <= 0 ? string.Empty : settingsData.PotionsMendOnlyWhenDownXHP.ToString();

            _fullColor = settingsData.FullColor;
            SetColorUI(lblFullColorValue, _fullColor);
            _emptyColor = settingsData.EmptyColor;
            SetColorUI(lblEmptyColorValue, _emptyColor);

            foreach (Strategy s in settingsData.Strategies)
            {
                lstStrategies.Items.Add(s);
            }

            foreach (DynamicDataItemClass nextItemClass in Enum.GetValues(typeof(DynamicDataItemClass)))
            {
                AddNewListEntry(nextItemClass);
            }
            foreach (ItemTypeEnum nextItem in Enum.GetValues(typeof(ItemTypeEnum)))
            {
                AddNewListEntry(nextItem);
            }
            RefreshAllItemEntries();
            PopulateAreaTree();
        }

        private void PopulateAreaTree()
        {
            if (_settings.HomeArea != null)
            {
                treeAreas.Nodes.Add(CreateAreaNode(_settings.HomeArea));
            }
        }

        private TreeNode CreateAreaNode(Area node)
        {
            TreeNode ret = new TreeNode(node.DisplayName);
            ret.Tag = node;
            if (node.Children != null)
            {
                foreach (Area nextObj in node.Children)
                {
                    ret.Nodes.Add(CreateAreaNode(nextObj));
                }
            }
            if (ret.Nodes.Count > 0)
            {
                ret.Expand();
            }
            return ret;
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
            lvi.SubItems[2].Text = listInfo.SinkCountText;
            lvi.SubItems[3].Text = listInfo.OverflowActionText;
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            ItemTypeEnum? eWeapon = null;
            ItemTypeEnum? eHeldItem = null;

            string sItem = txtCurrentWeaponValue.Text;
            if (!string.IsNullOrEmpty(sItem))
            {
                eWeapon = (ItemTypeEnum)Enum.Parse(typeof(ItemTypeEnum), sItem);
            }

            sItem = txtHeldItem.Text;
            if (!string.IsNullOrEmpty(sItem))
            {
                eHeldItem = (ItemTypeEnum)Enum.Parse(typeof(ItemTypeEnum), sItem);
            }

            string sWhenDownXHP;
            int iMagicVigorWhenDownXHP, iMagicMendWhenDownXHP, iPotionsVigorWhenDownXHP, iPotionsMendWhenDownXHP;

            sWhenDownXHP = txtMagicVigorWhenDownXHP.Text;
            if (string.IsNullOrEmpty(sWhenDownXHP))
            {
                iMagicVigorWhenDownXHP = 0;
            }
            else if (!int.TryParse(sWhenDownXHP, out iMagicVigorWhenDownXHP) || iMagicVigorWhenDownXHP <= 0)
            {
                MessageBox.Show("Invalid magic vigor when down X HP");
                txtMagicVigorWhenDownXHP.Focus();
                return;
            }

            sWhenDownXHP = txtMagicMendWhenDownXHP.Text;
            if (string.IsNullOrEmpty(sWhenDownXHP))
            {
                iMagicMendWhenDownXHP = 0;
            }
            else if (!int.TryParse(sWhenDownXHP, out iMagicMendWhenDownXHP) || iMagicMendWhenDownXHP <= 0)
            {
                MessageBox.Show("Invalid magic mend when down X HP");
                txtMagicMendWhenDownXHP.Focus();
                return;
            }

            sWhenDownXHP = txtPotionsVigorWhenDownXHP.Text;
            if (string.IsNullOrEmpty(sWhenDownXHP))
            {
                iPotionsVigorWhenDownXHP = 0;
            }
            else if (!int.TryParse(sWhenDownXHP, out iPotionsVigorWhenDownXHP) || iPotionsVigorWhenDownXHP <= 0)
            {
                MessageBox.Show("Invalid potions vigor when down X HP");
                txtPotionsVigorWhenDownXHP.Focus();
                return;
            }

            sWhenDownXHP = txtPotionsMendWhenDownXHP.Text;
            if (string.IsNullOrEmpty(sWhenDownXHP))
            {
                iPotionsMendWhenDownXHP = 0;
            }
            else if (!int.TryParse(sWhenDownXHP, out iPotionsMendWhenDownXHP) || iPotionsMendWhenDownXHP <= 0)
            {
                MessageBox.Show("Invalid potions mend when down X HP");
                txtPotionsMendWhenDownXHP.Focus();
                return;
            }

            _settings.QueryMonsterStatus = chkQueryMonsterStatus.Checked;
            _settings.GetNewPermRunOnBoatExitMissing = chkGetNewPermRunOnBoatExitMissing.Checked;
            _settings.ConsoleVerbosity = (ConsoleOutputVerbosity)cboConsoleVerbosity.SelectedIndex;
            _settings.RemoveAllOnStartup = chkRemoveAllOnStartup.Checked;
            _settings.DisplayStunLength = chkDisplayStunLength.Checked;
            _settings.SaveSettingsOnQuit = chkSaveSettingsOnQuit.Checked;
            _settings.FullColor = _fullColor;
            _settings.EmptyColor = _emptyColor;
            _settings.Realm = _currentRealm;
            _settings.PreferredAlignment = _preferredAlignment;
            SaveAutoSpellToSettingsObject(_settings);
            _settings.Weapon = eWeapon;
            _settings.HeldItem = eHeldItem;
            _settings.MagicVigorOnlyWhenDownXHP = iMagicVigorWhenDownXHP;
            _settings.MagicMendOnlyWhenDownXHP = iMagicMendWhenDownXHP;
            _settings.PotionsVigorOnlyWhenDownXHP = iPotionsVigorWhenDownXHP;
            _settings.PotionsMendOnlyWhenDownXHP = iPotionsMendWhenDownXHP;

            _settings.Strategies.Clear();
            foreach (Strategy s in lstStrategies.Items)
            {
                _settings.Strategies.Add(s);
            }

            if (treeAreas.Nodes.Count == 0)
                _settings.HomeArea = null;
            else
                _settings.HomeArea = GetAreaFromNode(null, treeAreas.Nodes[0]);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private Area GetAreaFromNode(Area parentArea, TreeNode tn)
        {
            Area a = (Area)tn.Tag;
            a.Parent = parentArea;
            a.ParentID = parentArea == null ? 0 : parentArea.ID;
            if (tn.Nodes.Count > 0)
            {
                a.Children = new List<Area>();
                foreach (TreeNode nextTN in tn.Nodes)
                {
                    a.Children.Add(GetAreaFromNode(a, nextTN));
                }
            }
            else
            {
                a.Children = null;
            }
            return a;
        }

        private IsengardSettingData CreateTempSettingsObjectWithAutoSpell()
        {
            IsengardSettingData ret = new IsengardSettingData();
            SaveAutoSpellToSettingsObject(ret);
            return ret;
        }

        private void SaveAutoSpellToSettingsObject(IsengardSettingData settings)
        {
            int iAutoSpellMin, iAutoSpellMax;
            _autoSpellLevelOverrides.GetEffectiveMinMax(out iAutoSpellMin, out iAutoSpellMax);
            settings.AutoSpellLevelMin = iAutoSpellMin;
            settings.AutoSpellLevelMax = iAutoSpellMax;
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

        private void ctxListModification_Opening(object sender, CancelEventArgs e)
        {
            ListBox lb;
            DataGridView dgv;
            object oSourceControl = ctxListModification.SourceControl;
            lb = oSourceControl as ListBox;
            dgv = oSourceControl as DataGridView;
            int iCount;
            List<int> indexes = new List<int>();
            if (lb != null)
            {
                foreach (int nextIndex in lb.SelectedIndices)
                {
                    indexes.Add(nextIndex);
                }
                iCount = lb.Items.Count;
            }
            else if (dgv != null)
            {
                foreach (DataGridViewRow r in dgv.SelectedRows)
                {
                    indexes.Add(r.Index);
                }
                iCount = dgv.Rows.Count;
            }
            else
            {
                throw new InvalidOperationException();
            }
            bool hasOne = indexes.Count == 1;
            bool hasMultiple = indexes.Count > 1;
            int iFirst = hasOne ? indexes[0] : 0;
            tsmiEditEntry.Visible = hasOne;
            tsmiRemoveEntry.Visible = hasOne || hasMultiple;
            tsmiMoveEntryDown.Visible = hasOne && (iFirst < iCount - 1);
            tsmiMoveEntryUp.Visible = hasOne && iFirst > 0;
        }

        private void tsmiAddEntry_Click(object sender, EventArgs e)
        {
            Strategy s = new Strategy();
            using (frmStrategy frm = new frmStrategy(s, CreateTempSettingsObjectWithAutoSpell()))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    lstStrategies.Items.Add(s);
                }
            }
        }

        private void tsmiEditEntry_Click(object sender, EventArgs e)
        {
            int index = lstStrategies.SelectedIndex;
            Strategy s = (Strategy)lstStrategies.Items[index];
            using (frmStrategy frm = new frmStrategy(s, CreateTempSettingsObjectWithAutoSpell()))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    lstStrategies.Items[index] = s;
                }
            }
        }

        private void tsmiRemoveEntry_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            List<int> indexesToRemove = new List<int>();
            foreach (int nextIndex in lstStrategies.SelectedIndices)
            {
                indexesToRemove.Add(nextIndex);
                Strategy s = (Strategy)lstStrategies.Items[nextIndex];
                foreach (PermRun pr in _settings.PermRuns)
                {
                    if (pr.Strategy == s)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!isValid) break;
            }
            if (isValid && MessageBox.Show("Are you sure you want to remove these strategy(s)?", "Remove Strategy", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                indexesToRemove.Reverse();
                foreach (int nextIndex in indexesToRemove)
                {
                    lstStrategies.Items.RemoveAt(nextIndex);
                }
            }
            if (!isValid)
            {
                MessageBox.Show("Objects associated to perm runs cannot be removed.");
                return;
            }
        }

        private void MoveEntryUp(int iIndex, int iIndexToSelect)
        {
            Strategy s = (Strategy)lstStrategies.Items[iIndex];
            lstStrategies.Items.RemoveAt(iIndex);
            lstStrategies.Items.Insert(iIndex - 1, s);
            lstStrategies.SelectedIndex = iIndexToSelect;
        }

        private void tsmiMoveEntryUp_Click(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            MoveEntryUp(iIndex, iIndex - 1);
        }

        private void tsmiMoveEntryDown_Click(object sender, EventArgs e)
        {
            int iIndex = lstStrategies.SelectedIndex;
            iIndex++;
            MoveEntryUp(iIndex, iIndex);
        }

        private void ctxPreferredAlignment_Opening(object sender, CancelEventArgs e)
        {
            tsmiPreferredAlignmentGood.Checked = _preferredAlignment == AlignmentType.Blue;
            tsmiPreferredAlignmentEvil.Checked = _preferredAlignment == AlignmentType.Red;
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
            public string SinkCountText { get; set; }
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

            if (did.SinkCount < 0)
                sNext = "None";
            else if (did.SinkCount == int.MaxValue)
                sNext = "All";
            else
                sNext = did.SinkCount.ToString();
            if (did.SinkCount >= 0 && did.SinkCountInheritance.HasValue)
                sNext += " (" + did.SinkCountInheritance.Value.ToString() + ")";
            ret.SinkCountText = sNext;

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

        private void btnSink_Click(object sender, EventArgs e)
        {
            string count = Interaction.InputBox("Sink Count:", "Enter count, negative to clear, all for no limit", string.Empty);
            int iCount;
            if (count.Equals("all", StringComparison.OrdinalIgnoreCase))
                iCount = int.MaxValue;
            else if (!int.TryParse(count, out iCount))
                return;
            SetItemProperty((did) => { did.SinkCount = iCount < 0 ? -1 : iCount; });
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

        private void btnClearWeapon_Click(object sender, EventArgs e)
        {
            txtCurrentWeaponValue.Text = string.Empty;
        }

        private void btnClearHeldItem_Click(object sender, EventArgs e)
        {
            txtHeldItem.Text = string.Empty;
        }

        private void ctxItems_Opening(object sender, CancelEventArgs e)
        {
            ctxItems.Items.Clear();
            bool singleItemSelected = lvItems.SelectedItems.Count == 1;
            if (singleItemSelected)
            {
                ListViewItem lvi = lvItems.SelectedItems[0];
                if (lvi.Tag is ItemTypeEnum)
                {
                    ItemTypeEnum itemType = (ItemTypeEnum)lvi.Tag;
                    StaticItemData sid = ItemEntity.StaticItemData[itemType];
                    ToolStripMenuItem tsmi;
                    if (sid.ItemClass == ItemClass.Weapon)
                    {
                        tsmi = new ToolStripMenuItem();
                        tsmi.Text = "Set Weapon";
                        ctxItems.Items.Add(tsmi);
                    }
                    else if (sid.EquipmentType == EquipmentType.Holding)
                    {
                        tsmi = new ToolStripMenuItem();
                        tsmi.Text = "Set Held Item";
                        ctxItems.Items.Add(tsmi);
                    }
                }
            }
            if (ctxItems.Items.Count == 0)
            {
                e.Cancel = true;
            }
        }

        private void ctxItems_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string itemTypeText = lvItems.SelectedItems[0].Tag.ToString();
            string sText = e.ClickedItem.Text;
            TextBox txtItem;
            if (sText == "Set Weapon")
                txtItem = txtCurrentWeaponValue;
            else if (sText == "Set Held Item")
                txtItem = txtHeldItem;
            else
                throw new InvalidOperationException();
            txtItem.Text = itemTypeText;
        }

        private void ctxAreas_Opening(object sender, CancelEventArgs e)
        {
            TreeNode selectedNode = treeAreas.SelectedNode;
            bool haveNode = selectedNode != null;
            bool isHome = false;
            bool allowAdd;
            bool allowRemove;
            if (haveNode)
            {
                isHome = selectedNode.Parent == null;
                TreeNodeCollection parentCollection = isHome ? treeAreas.Nodes : selectedNode.Parent.Nodes;
                int iIndex = parentCollection.IndexOf(selectedNode);
                tsmiMoveDown.Enabled = iIndex < parentCollection.Count - 1;
                tsmiMoveUp.Enabled = iIndex > 0;
                allowAdd = true;
                allowRemove = true;
                List<Area> areasToDelete = new List<Area>();
                GetExistingAreas(selectedNode, areasToDelete);
                foreach (PermRun pr in _settings.PermRuns)
                {
                    if (pr.Areas != null)
                    {
                        foreach (Area a in pr.Areas)
                        {
                            if (areasToDelete.Contains(a))
                            {
                                allowRemove = false;
                                break;
                            }
                        }
                        if (!allowRemove) break;
                    }
                }
            }
            else
            {
                allowAdd = treeAreas.Nodes.Count == 0;
                allowRemove = false;
            }
            tsmiAddChild.Visible = allowAdd;
            tsmiAddSiblingAfter.Visible = haveNode && !isHome;
            tsmiAddSiblingBefore.Visible = haveNode && !isHome;
            tsmiEdit.Visible = haveNode;
            tsmiMoveDown.Visible = haveNode;
            tsmiMoveUp.Visible = haveNode;
            tsmiRemove.Visible = allowRemove;

            if (!allowAdd && !haveNode)
            {
                e.Cancel = true;
            }
        }

        private void ctxAreas_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem tsi = e.ClickedItem;
            TreeNode selectedNode = treeAreas.SelectedNode;
            Area currentObj = null;
            TreeNode parentNode = null;
            if (selectedNode != null)
            {
                currentObj = (Area)selectedNode.Tag;
                parentNode = selectedNode.Parent;
            }
            TreeNodeCollection parentTreeNodes = parentNode == null ? treeAreas.Nodes : selectedNode.Parent.Nodes;
            List<Area> parentObjectNodes = parentNode == null ? null : currentObj.Parent.Children;
            int iCurrentIndex = parentTreeNodes.IndexOf(selectedNode);
            TreeNode newNodeInfo;
            if (tsi == tsmiAddChild || tsi == tsmiAddSiblingAfter || tsi == tsmiAddSiblingBefore)
            {
                Area parentObj;
                if (tsi == tsmiAddChild)
                    parentObj = currentObj;
                else
                    parentObj = currentObj.Parent;
                newNodeInfo = DisplayAreaForm(new Area(parentObj));
                if (newNodeInfo != null)
                {
                    Area newObj = (Area)newNodeInfo.Tag;
                    if (tsi == tsmiAddChild)
                    {
                        if (selectedNode == null) //add top level node
                        {
                            treeAreas.Nodes.Add(newNodeInfo);
                        }
                        else //add sub node to the current node
                        {
                            selectedNode.Nodes.Add(newNodeInfo);
                            if (currentObj.Children == null) currentObj.Children = new List<Area>();
                            currentObj.Children.Add(newObj);
                            selectedNode.Expand();
                        }
                    }
                    else if (tsi == tsmiAddSiblingBefore)
                    {
                        selectedNode.Parent.Nodes.Insert(iCurrentIndex, newNodeInfo);
                        parentObjectNodes.Insert(iCurrentIndex, newObj);
                    }
                    else if (tsi == tsmiAddSiblingAfter)
                    {
                        if (iCurrentIndex == parentTreeNodes.Count - 1)
                        {
                            parentTreeNodes.Add(newNodeInfo);
                            parentObjectNodes.Add(newObj);
                        }
                        else
                        {
                            parentTreeNodes.Insert(iCurrentIndex + 1, newNodeInfo);
                            parentObjectNodes.Insert(iCurrentIndex + 1, newObj);
                        }
                    }
                }
            }
            else if (tsi == tsmiEdit)
            {
                newNodeInfo = DisplayAreaForm(currentObj);
                if (newNodeInfo != null)
                {
                    selectedNode.Text = newNodeInfo.Text;
                }
            }
            else if (tsi == tsmiRemove)
            {
                if (MessageBox.Show("Are you sure you want to remove this area?", "Remove Area", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    parentTreeNodes.Remove(selectedNode);
                    parentObjectNodes.Remove(currentObj);
                    if (currentObj.Parent != null && parentObjectNodes.Count == 0)
                    {
                        currentObj.Parent.Children = null;
                    }
                }
            }
            else if (tsi == tsmiMoveUp)
            {
                parentTreeNodes.Remove(selectedNode);
                parentObjectNodes.Remove(currentObj);
                parentTreeNodes.Insert(iCurrentIndex - 1, selectedNode);
                parentObjectNodes.Insert(iCurrentIndex - 1, currentObj);
                treeAreas.SelectedNode = selectedNode;
            }
            else if (tsi == tsmiMoveDown)
            {
                parentTreeNodes.Remove(selectedNode);
                parentObjectNodes.Remove(currentObj);
                if (iCurrentIndex == parentTreeNodes.Count)
                {
                    parentTreeNodes.Add(selectedNode);
                    parentObjectNodes.Add(currentObj);
                }
                else
                {
                    parentTreeNodes.Insert(iCurrentIndex + 1, selectedNode);
                    parentObjectNodes.Insert(iCurrentIndex + 1, currentObj);
                }
                treeAreas.SelectedNode = selectedNode;
            }
        }

        private TreeNode DisplayAreaForm(Area startingPoint)
        {
            TreeNode ret = null;
            List<Area> existingAreas = new List<Area>();
            if (treeAreas.Nodes.Count > 0)
            {
                GetExistingAreas(treeAreas.Nodes[0], existingAreas);
            }
            frmArea frm = new frmArea(startingPoint, _gameMap, _settings, existingAreas, _getGraphInputs, _cei);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                ret = CreateAreaNode(startingPoint);
            }
            return ret;
        }

        private void GetExistingAreas(TreeNode tn, List<Area> areas)
        {
            if (tn != null)
            {
                areas.Add((Area)tn.Tag);
                foreach (TreeNode tnChild in tn.Nodes)
                {
                    GetExistingAreas(tnChild, areas);
                }
            }
        }

        private void treeAreas_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
