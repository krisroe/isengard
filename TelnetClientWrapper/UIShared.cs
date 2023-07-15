using IsengardClient.Backend;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace IsengardClient
{
    internal class UIShared
    {
        internal static void GetForegroundColor(byte r, byte g, byte b, out byte forer, out byte foreg, out byte foreb)
        {
            forer = (byte)(r <= 128 ? 255 : 0);
            foreg = (byte)(g <= 128 ? 255 : 0);
            foreb = (byte)(b <= 128 ? 255 : 0);
        }

        internal static DataGridViewCellStyle GetAlternatingDataGridViewCellStyle()
        {
            DataGridViewCellStyle ret = new DataGridViewCellStyle();
            ret.BackColor = Color.FromArgb(225, 255, 255);
            ret.ForeColor = SystemColors.ControlText;
            ret.SelectionBackColor = SystemColors.Highlight;
            ret.SelectionForeColor = SystemColors.WindowText;
            return ret;
        }

        /// <summary>
        /// handler for when a room is selected for a dropdown that auto-adds the room on selection
        /// </summary>
        /// <param name="selectedRoom">selected room</param>
        /// <param name="roomDropdown">dropdown</param>
        public static void HandleRoomSelected(Room selectedRoom, ComboBox roomDropdown)
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
    }

    internal class StrategyOverridesUI
    {
        private StrategyOverridesLevel _level;
        private OverrideStrategyPropertyType _contextMenuProperty;

        public int PermRunAutoSpellLevelMaximum { get; set; }
        public int PermRunAutoSpellLevelMinimum { get; set; }
        public int StrategyAutoSpellLevelMaximum { get; set; }
        public int StrategyAutoSpellLevelMinimum { get; set; }
        private int _settingsAutoSpellLevelMinimum;
        private int _settingsAutoSpellLevelMaximum;

        public RealmTypeFlags? PermRunRealms { get; set; }
        public RealmTypeFlags? StrategyRealms { get; set; }
        private RealmTypeFlags _settingsRealms;
        private bool _realmCycle;
        
        private Label _lblAutoSpellLevels;
        private ContextMenuStrip ctxContextMenu;
        private ToolStripMenuItem tsmiSetCurrentMinimumAutoSpellLevel;
        private ToolStripMenuItem tsmiSetCurrentMaximumAutoSpellLevel;

        private Label _lblRealms;
        private ToolStripMenuItem tsmiRealmsEarth;
        private ToolStripMenuItem tsmiRealmsWind;
        private ToolStripMenuItem tsmiRealmsFire;
        private ToolStripMenuItem tsmiRealmsWater;
        private ToolStripMenuItem tsmiRealmsCycle;

        private ToolStripMenuItem tsmiClearOverride;

        public StrategyOverridesUI(int permRunMin, int permRunMax, int strategyMin, int strategyMax, Label lblAutoSpellLevels, RealmTypeFlags? permRunRealms, RealmTypeFlags? strategyRealms, Label lblRealms, StrategyOverridesLevel level, IsengardSettingData settings)
        {
            _level = level;

            PermRunAutoSpellLevelMinimum = permRunMin;
            PermRunAutoSpellLevelMaximum = permRunMax;
            StrategyAutoSpellLevelMinimum = strategyMin;
            StrategyAutoSpellLevelMaximum = strategyMax;
            _settingsAutoSpellLevelMinimum = settings.AutoSpellLevelMin;
            _settingsAutoSpellLevelMaximum = settings.AutoSpellLevelMax;
            _lblAutoSpellLevels = lblAutoSpellLevels;

            PermRunRealms = permRunRealms;
            StrategyRealms = strategyRealms;
            _settingsRealms = settings.Realms;
            _lblRealms = lblRealms;
            RealmTypeFlags? eCurrentRealms = GetCurrentRealms();
            _realmCycle = eCurrentRealms.HasValue && eCurrentRealms.Value != RealmTypeFlags.Earth && eCurrentRealms.Value != RealmTypeFlags.Wind && eCurrentRealms.Value != RealmTypeFlags.Fire && eCurrentRealms.Value != RealmTypeFlags.Water;

            ctxContextMenu = new ContextMenuStrip();


            ctxContextMenu.ImageScalingSize = new Size(20, 20);
            ctxContextMenu.Name = "ctxContextMenu";
            ctxContextMenu.Size = new Size(222, 82);
            ctxContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ctxContextMenu_Opening);

            tsmiSetCurrentMinimumAutoSpellLevel = new ToolStripMenuItem();
            tsmiSetCurrentMinimumAutoSpellLevel.Name = "tsmiSetCurrentMinimumAutoSpellLevel";
            tsmiSetCurrentMinimumAutoSpellLevel.Size = new Size(221, 24);
            tsmiSetCurrentMinimumAutoSpellLevel.Text = "Set Current Minimum";
            tsmiSetCurrentMinimumAutoSpellLevel.Click += new EventHandler(this.tsmiSetCurrentMinimumAutoSpellLevel_Click);
            tsmiSetCurrentMaximumAutoSpellLevel = new ToolStripMenuItem();
            tsmiSetCurrentMaximumAutoSpellLevel.Name = "tsmiSetCurrentMaximumAutoSpellLevel";
            tsmiSetCurrentMaximumAutoSpellLevel.Size = new Size(221, 24);
            tsmiSetCurrentMaximumAutoSpellLevel.Text = "Set Current Maximum";
            tsmiSetCurrentMaximumAutoSpellLevel.Click += new EventHandler(this.tsmiSetCurrentMaximumAutoSpellLevel_Click);

            tsmiRealmsEarth = new ToolStripMenuItem();
            tsmiRealmsEarth.Name = "tsmiRealmsEarth";
            tsmiRealmsEarth.Size = new Size(210, 24);
            tsmiRealmsEarth.Tag = RealmTypeFlags.Earth;
            tsmiRealmsEarth.Text = "earth";
            tsmiRealmsEarth.Click += new EventHandler(this.tsmiCurrentRealm_Click);
            tsmiRealmsWind = new ToolStripMenuItem();
            tsmiRealmsWind.Name = "tsmiRealmsWind";
            tsmiRealmsWind.Size = new Size(210, 24);
            tsmiRealmsWind.Tag = RealmTypeFlags.Wind;
            tsmiRealmsWind.Text = "wind";
            tsmiRealmsWind.Click += new EventHandler(this.tsmiCurrentRealm_Click);
            tsmiRealmsFire = new ToolStripMenuItem();
            tsmiRealmsFire.Name = "tsmiRealmsFire";
            tsmiRealmsFire.Size = new Size(210, 24);
            tsmiRealmsFire.Tag = RealmTypeFlags.Fire;
            tsmiRealmsFire.Text = "fire";
            tsmiRealmsFire.Click += new EventHandler(this.tsmiCurrentRealm_Click);
            tsmiRealmsWater = new ToolStripMenuItem();
            tsmiRealmsWater.Name = "tsmiRealmsWater";
            tsmiRealmsWater.Size = new Size(210, 24);
            tsmiRealmsWater.Tag = RealmTypeFlags.Water;
            tsmiRealmsWater.Text = "water";
            tsmiRealmsWater.Click += new EventHandler(this.tsmiCurrentRealm_Click);
            tsmiRealmsCycle = new ToolStripMenuItem();
            tsmiRealmsCycle.Name = "tsmiRealmsCycle";
            tsmiRealmsCycle.Size = new Size(210, 24);
            tsmiRealmsCycle.Text = "cycle";
            tsmiRealmsCycle.Click += new EventHandler(this.tsmiCurrentRealmCycle_Click);

            if (_level != StrategyOverridesLevel.Settings)
            {
                tsmiClearOverride = new ToolStripMenuItem();
                tsmiClearOverride.Name = "tsmiClearOverrideAutoSpellLevels";
                tsmiClearOverride.Size = new Size(221, 24);
                tsmiClearOverride.Text = "Clear Override";
                tsmiClearOverride.Click += new EventHandler(this.tsmiClearOverride_Click);
            }

            lblAutoSpellLevels.ContextMenuStrip = ctxContextMenu;
            lblRealms.ContextMenuStrip = ctxContextMenu;

            RefreshUI();
        }

        private void ctxContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ctxContextMenu.Items.Clear();
            object oSourceControl = ctxContextMenu.SourceControl;
            if (oSourceControl == _lblAutoSpellLevels)
            {
                _contextMenuProperty = OverrideStrategyPropertyType.AutoSpellLevels;
                ctxContextMenu.Items.Add(tsmiSetCurrentMinimumAutoSpellLevel);
                ctxContextMenu.Items.Add(tsmiSetCurrentMaximumAutoSpellLevel);
            }
            else if (oSourceControl == _lblRealms)
            {
                _contextMenuProperty = OverrideStrategyPropertyType.Realms;
                RealmTypeFlags? currentRealms = GetCurrentRealms();
                ctxContextMenu.Items.Add(tsmiRealmsEarth);
                tsmiRealmsEarth.Checked = currentRealms.HasValue && ((currentRealms.Value & RealmTypeFlags.Earth) != RealmTypeFlags.None);
                tsmiRealmsEarth.Enabled = !currentRealms.HasValue || currentRealms.Value != RealmTypeFlags.Earth;
                ctxContextMenu.Items.Add(tsmiRealmsWind);
                tsmiRealmsWind.Checked = currentRealms.HasValue && ((currentRealms.Value & RealmTypeFlags.Wind) != RealmTypeFlags.None);
                tsmiRealmsWind.Enabled = !currentRealms.HasValue || currentRealms.Value != RealmTypeFlags.Wind;
                ctxContextMenu.Items.Add(tsmiRealmsFire);
                tsmiRealmsFire.Checked = currentRealms.HasValue && ((currentRealms.Value & RealmTypeFlags.Fire) != RealmTypeFlags.None);
                tsmiRealmsFire.Enabled = !currentRealms.HasValue || currentRealms.Value != RealmTypeFlags.Fire;
                ctxContextMenu.Items.Add(tsmiRealmsWater);
                tsmiRealmsWater.Checked = currentRealms.HasValue && ((currentRealms.Value & RealmTypeFlags.Water) != RealmTypeFlags.None);
                tsmiRealmsWater.Enabled = !currentRealms.HasValue || currentRealms.Value != RealmTypeFlags.Water;
                ctxContextMenu.Items.Add(tsmiRealmsCycle);
                tsmiRealmsCycle.Checked = _realmCycle;
            }
            else
            {
                throw new InvalidOperationException();
            }
            if (_level != StrategyOverridesLevel.Settings)
            {
                bool hasOverride = false;
                if (_contextMenuProperty == OverrideStrategyPropertyType.AutoSpellLevels)
                {
                    GetCurrentAutoSpellLevels(out int iMin, out int iMax);
                    hasOverride = iMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && iMax != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
                }
                else if (_contextMenuProperty == OverrideStrategyPropertyType.Realms)
                {
                    RealmTypeFlags? currentRealms = GetCurrentRealms();
                    hasOverride = currentRealms.HasValue;
                }
                if (hasOverride)
                {
                    ctxContextMenu.Items.Add(tsmiClearOverride);
                }
            }
        }

        private void tsmiClearOverride_Click(object sender, EventArgs e)
        {
            if (_contextMenuProperty == OverrideStrategyPropertyType.AutoSpellLevels)
            {
                GetCurrentAutoSpellLevels(out int iMin, out int iMax);
                if (iMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    SetCurrentAutoSpellLevelValues(IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET, IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET);
                    RefreshAutoSpellLevelUI();
                }
            }
            else if (_contextMenuProperty == OverrideStrategyPropertyType.Realms)
            {
                RealmTypeFlags? currentRealms = GetCurrentRealms();
                if (currentRealms.HasValue)
                {
                    SetCurrentRealms(null);
                    RefreshRealmsUI();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void RefreshUI()
        {
            RefreshAutoSpellLevelUI();
            RefreshRealmsUI();
        }

        #region auto spell levels

        private void tsmiSetCurrentMinimumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            GetCurrentAutoSpellLevels(out int iMin, out int iMax);
            string sStart = iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET ? string.Empty : iMin.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                iMin = iLevel;
                if (iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                    iMax = IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM;
                else if (iMax < iMin)
                    iMax = iMin;
                SetCurrentAutoSpellLevelValues(iMin, iMax);
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetCurrentMaximumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            GetCurrentAutoSpellLevels(out int iMin, out int iMax);
            string sStart = iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET ? string.Empty : iMax.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                iMax = iLevel;
                if (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                    iMin = IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM;
                else if (iMax < iMin)
                    iMin = iMax;
                SetCurrentAutoSpellLevelValues(iMin, iMax);
                RefreshAutoSpellLevelUI();
            }
        }

        public void GetCurrentAutoSpellLevels(out int iMin, out int iMax)
        {
            iMin = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            iMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            switch (_level)
            {
                case StrategyOverridesLevel.Settings:
                    iMin = _settingsAutoSpellLevelMinimum;
                    iMax = _settingsAutoSpellLevelMaximum;
                    break;
                case StrategyOverridesLevel.Strategy:
                    iMin = StrategyAutoSpellLevelMinimum;
                    iMax = StrategyAutoSpellLevelMaximum;
                    break;
                case StrategyOverridesLevel.PermRun:
                    iMin = PermRunAutoSpellLevelMinimum;
                    iMax = PermRunAutoSpellLevelMaximum;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void GetEffectiveAutoSpellLevelsMinMax(out int iMin, out int iMax)
        {
            iMin = iMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            if (_level == StrategyOverridesLevel.PermRun)
            {
                iMin = PermRunAutoSpellLevelMinimum;
                iMax = PermRunAutoSpellLevelMaximum;
            }
            if ((_level == StrategyOverridesLevel.PermRun || _level == StrategyOverridesLevel.Strategy) && (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET))
            {
                iMin = StrategyAutoSpellLevelMinimum;
                iMax = StrategyAutoSpellLevelMaximum;
            }
            if (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                iMin = _settingsAutoSpellLevelMinimum;
                iMax = _settingsAutoSpellLevelMaximum;
            }
        }

        private void SetCurrentAutoSpellLevelValues(int iMin, int iMax)
        {
            switch (_level)
            {
                case StrategyOverridesLevel.Settings:
                    _settingsAutoSpellLevelMinimum = iMin;
                    _settingsAutoSpellLevelMaximum = iMax;
                    break;
                case StrategyOverridesLevel.Strategy:
                    StrategyAutoSpellLevelMinimum = iMin;
                    StrategyAutoSpellLevelMaximum = iMax;
                    break;
                case StrategyOverridesLevel.PermRun:
                    PermRunAutoSpellLevelMinimum = iMin;
                    PermRunAutoSpellLevelMaximum = iMax;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void RefreshAutoSpellLevelUI()
        {
            string sText = null;
            GetCurrentAutoSpellLevels(out int iMin, out int iMax);
            if (iMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && iMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                sText = iMin + ":" + iMax;
            }
            if (_level == StrategyOverridesLevel.PermRun && string.IsNullOrEmpty(sText))
            {
                if (StrategyAutoSpellLevelMinimum != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && StrategyAutoSpellLevelMaximum != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    sText = "Strategy: " + StrategyAutoSpellLevelMinimum + ":" + StrategyAutoSpellLevelMaximum;
                }
            }
            if ((_level == StrategyOverridesLevel.PermRun || _level == StrategyOverridesLevel.Strategy) && string.IsNullOrEmpty(sText))
            {
                sText = "Settings: " + _settingsAutoSpellLevelMinimum + ":" + _settingsAutoSpellLevelMaximum;
            }
            if (_level != StrategyOverridesLevel.Settings)
            {
                sText = "Auto spell levels: " + sText;
            }
            _lblAutoSpellLevels.Text = sText;
        }

        #endregion

        #region realms

        private void tsmiCurrentRealm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            RealmTypeFlags selectedRealm = (RealmTypeFlags)tsmi.Tag;
            RealmTypeFlags? currentRealms = GetCurrentRealms();
            RealmTypeFlags newValue;
            if (tsmi.Checked) //unchecking (only available when cycling)
            {
                newValue = currentRealms.Value & ~selectedRealm;
            }
            else //checking
            {
                if (_realmCycle)
                    newValue = currentRealms.Value | selectedRealm;
                else
                    newValue = selectedRealm;
            }
            SetCurrentRealms(newValue);
            RefreshRealmsUI();
        }

        private void tsmiCurrentRealmCycle_Click(object sender, EventArgs e)
        {
            _realmCycle = !tsmiRealmsCycle.Checked;
            if (_realmCycle)
            {
                SetCurrentRealms(GetEffectiveRealms());
            }
            else
            {
                RealmTypeFlags eCurrentRealms = GetCurrentRealms().Value;
                foreach (RealmTypeFlags nextRealm in new RealmTypeFlags[] { RealmTypeFlags.Earth, RealmTypeFlags.Wind, RealmTypeFlags.Fire, RealmTypeFlags.Water })
                {
                    if ((nextRealm & eCurrentRealms) != RealmTypeFlags.None)
                    {
                        SetCurrentRealms(nextRealm);
                        break;
                    }
                }
            }
            RefreshRealmsUI();
        }

        public RealmTypeFlags? GetCurrentRealms()
        {
            RealmTypeFlags? ret;
            switch (_level)
            {
                case StrategyOverridesLevel.Settings:
                    ret = _settingsRealms;
                    break;
                case StrategyOverridesLevel.Strategy:
                    ret = StrategyRealms;
                    break;
                case StrategyOverridesLevel.PermRun:
                    ret = PermRunRealms;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }

        public RealmTypeFlags GetEffectiveRealms()
        {
            RealmTypeFlags? ret = null;
            if (_level == StrategyOverridesLevel.PermRun)
            {
                ret = PermRunRealms;
            }
            if ((_level == StrategyOverridesLevel.PermRun || _level == StrategyOverridesLevel.Strategy) && !ret.HasValue)
            {
                ret = StrategyRealms;
            }
            if (!ret.HasValue)
            {
                ret = _settingsRealms;
            }
            return ret.Value;
        }

        private void SetCurrentRealms(RealmTypeFlags? realms)
        {
            switch (_level)
            {
                case StrategyOverridesLevel.Settings:
                    _settingsRealms = realms.Value;
                    break;
                case StrategyOverridesLevel.Strategy:
                    StrategyRealms = realms;
                    break;
                case StrategyOverridesLevel.PermRun:
                    PermRunRealms = realms;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void RefreshRealmsUI()
        {
            RealmTypeFlags? realmsCurrent = GetCurrentRealms();
            string sText = null;
            if (realmsCurrent.HasValue)
            {
                sText = StringProcessing.TrimFlagsEnumToString(realmsCurrent.Value);
            }
            if (_level == StrategyOverridesLevel.PermRun && string.IsNullOrEmpty(sText))
            {
                if (StrategyRealms.HasValue)
                {
                    sText = "Strategy: " + StringProcessing.TrimFlagsEnumToString(StrategyRealms.Value);
                }
            }
            if ((_level == StrategyOverridesLevel.PermRun || _level == StrategyOverridesLevel.Strategy) && string.IsNullOrEmpty(sText))
            {
                sText = "Settings: " + StringProcessing.TrimFlagsEnumToString(_settingsRealms);
            }
            if (_level != StrategyOverridesLevel.Settings)
            {
                sText = "Realms: " + sText;
            }
            _lblRealms.Text = sText;

            Color backColor;
            if (realmsCurrent.HasValue)
            {
                RealmTypeFlags eCurrentRealm = realmsCurrent.Value;
                if (_realmCycle) backColor = Color.Lavender;
                else if (eCurrentRealm == RealmTypeFlags.Earth) backColor = Color.Tan;
                else if (eCurrentRealm == RealmTypeFlags.Wind) backColor = Color.LightGray;
                else if (eCurrentRealm == RealmTypeFlags.Fire) backColor = Color.LightSalmon;
                else if (eCurrentRealm == RealmTypeFlags.Water) backColor = Color.LightBlue;
                else backColor = Color.Silver;
            }
            else
            {
                backColor = Color.Silver;
            }
            _lblRealms.BackColor = backColor;

            UIShared.GetForegroundColor(backColor.R, backColor.G, backColor.B, out byte forer, out byte foreg, out byte foreb);
            _lblRealms.ForeColor = Color.FromArgb(forer, foreg, foreb);
        }

        #endregion
    }
}
