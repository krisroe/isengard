using IsengardClient.Backend;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace IsengardClient
{
    internal class UIShared
    {
        internal static Color GetColorForRealm(RealmType realm)
        {
            Color ret = Color.Transparent;
            switch (realm)
            {
                case RealmType.Earth:
                    ret = Color.Tan;
                    break;
                case RealmType.Fire:
                    ret = Color.LightSalmon;
                    break;
                case RealmType.Water:
                    ret = Color.LightBlue;
                    break;
                case RealmType.Wind:
                    ret = Color.LightGray;
                    break;
            }
            return ret;
        }

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

    internal class AutoSpellLevelOverrides
    {
        private AutoSpellLevelOverridesLevel _level;
        private int _permRunAutoSpellLevelMaximum;
        private int _permRunAutoSpellLevelMinimum;
        private int _strategyAutoSpellLevelMaximum;
        private int _strategyAutoSpellLevelMinimum;
        private int _settingsAutoSpellLevelMinimum;
        private int _settingsAutoSpellLevelMaximum;
        private Label _lblAutoSpellLevels;
        private ContextMenuStrip ctxAutoSpellLevels;
        private ToolStripMenuItem tsmiSetCurrentMinimumAutoSpellLevel;
        private ToolStripMenuItem tsmiSetCurrentMaximumAutoSpellLevel;
        private ToolStripMenuItem tsmiClearOverrideAutoSpellLevels;

        public AutoSpellLevelOverrides(int permRunMin, int permRunMax, int strategyMin, int strategyMax, int settingsMin, int settingsMax, Label lblAutoSpellLevels, AutoSpellLevelOverridesLevel level)
        {
            _permRunAutoSpellLevelMinimum = permRunMin;
            _permRunAutoSpellLevelMaximum = permRunMax;
            _strategyAutoSpellLevelMinimum = strategyMin;
            _strategyAutoSpellLevelMaximum = strategyMax;
            _settingsAutoSpellLevelMinimum = settingsMin;
            _settingsAutoSpellLevelMaximum = settingsMax;
            _lblAutoSpellLevels = lblAutoSpellLevels;
            _level = level;

            this.ctxAutoSpellLevels = new ContextMenuStrip();
            this.tsmiSetCurrentMinimumAutoSpellLevel = new ToolStripMenuItem();
            this.tsmiSetCurrentMaximumAutoSpellLevel = new ToolStripMenuItem();

            if (_level != AutoSpellLevelOverridesLevel.Settings)
            {
                this.tsmiClearOverrideAutoSpellLevels = new ToolStripMenuItem();
            }

            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.ImageScalingSize = new System.Drawing.Size(20, 20);

            this.ctxAutoSpellLevels.Items.Add(tsmiSetCurrentMinimumAutoSpellLevel);
            this.ctxAutoSpellLevels.Items.Add(tsmiSetCurrentMaximumAutoSpellLevel);
            if (_level != AutoSpellLevelOverridesLevel.Settings)
            {
                this.ctxAutoSpellLevels.Items.Add(tsmiClearOverrideAutoSpellLevels);
            }
            this.ctxAutoSpellLevels.Name = "ctxAutoSpellLevels";
            this.ctxAutoSpellLevels.Size = new Size(222, 82);
            this.ctxAutoSpellLevels.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoSpellLevels_Opening);

            this.tsmiSetCurrentMinimumAutoSpellLevel.Name = "tsmiSetCurrentMinimumAutoSpellLevel";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Size = new Size(221, 24);
            this.tsmiSetCurrentMinimumAutoSpellLevel.Text = "Set Current Minimum";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMinimumAutoSpellLevel_Click);

            this.tsmiSetCurrentMaximumAutoSpellLevel.Name = "tsmiSetCurrentMaximumAutoSpellLevel";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Size = new Size(221, 24);
            this.tsmiSetCurrentMaximumAutoSpellLevel.Text = "Set Current Maximum";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMaximumAutoSpellLevel_Click);

            if (_level != AutoSpellLevelOverridesLevel.Settings)
            {
                this.tsmiClearOverrideAutoSpellLevels.Name = "tsmiClearOverrideAutoSpellLevels";
                this.tsmiClearOverrideAutoSpellLevels.Size = new Size(221, 24);
                this.tsmiClearOverrideAutoSpellLevels.Text = "Clear Override";
                this.tsmiClearOverrideAutoSpellLevels.Click += new EventHandler(this.tsmiToggleOverrideAutoSpellLevels_Click);
            }

            lblAutoSpellLevels.ContextMenuStrip = ctxAutoSpellLevels;
            RefreshAutoSpellLevelUI();
        }

        private void ctxAutoSpellLevels_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_level != AutoSpellLevelOverridesLevel.Settings)
            {
                GetCurrentValues(out int iMin, out int iMax);
                tsmiClearOverrideAutoSpellLevels.Visible = iMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && iMax != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            }
        }

        private void tsmiSetCurrentMinimumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            GetCurrentValues(out int iMin, out int iMax);
            string sStart = iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET ? string.Empty : iMin.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                iMin = iLevel;
                if (iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                    iMax = IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM;
                else if (iMax < iMin)
                    iMax = iMin;
                SetCurrentValues(iMin, iMax);
                RefreshAutoSpellLevelUI();
            }
        }

        private void GetCurrentValues(out int iMin, out int iMax)
        {
            iMin = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            iMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            switch (_level)
            {
                case AutoSpellLevelOverridesLevel.Settings:
                    iMin = _settingsAutoSpellLevelMinimum;
                    iMax = _settingsAutoSpellLevelMaximum;
                    break;
                case AutoSpellLevelOverridesLevel.Strategy:
                    iMin = _strategyAutoSpellLevelMinimum;
                    iMax = _strategyAutoSpellLevelMaximum;
                    break;
                case AutoSpellLevelOverridesLevel.PermRun:
                    iMin = _permRunAutoSpellLevelMinimum;
                    iMax = _permRunAutoSpellLevelMaximum;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetCurrentValues(int iMin, int iMax)
        {
            switch (_level)
            {
                case AutoSpellLevelOverridesLevel.Settings:
                    _settingsAutoSpellLevelMinimum = iMin;
                    _settingsAutoSpellLevelMaximum = iMax;
                    break;
                case AutoSpellLevelOverridesLevel.Strategy:
                    _strategyAutoSpellLevelMinimum = iMin;
                    _strategyAutoSpellLevelMaximum = iMax;
                    break;
                case AutoSpellLevelOverridesLevel.PermRun:
                    _permRunAutoSpellLevelMinimum = iMin;
                    _permRunAutoSpellLevelMaximum = iMax;
                    break;
            }
        }

        public int PermRunMinimum
        {
            get
            {
                return _permRunAutoSpellLevelMinimum;
            }
        }

        public int PermRunMaximum
        {
            get
            {
                return _permRunAutoSpellLevelMaximum;
            }
        }

        public int StrategyMinimum
        {
            set
            {
                _strategyAutoSpellLevelMinimum = value;
            }
        }

        public int StrategyMaximum
        {
            set
            {
                _strategyAutoSpellLevelMaximum = value;
            }
        }


        private void tsmiSetCurrentMaximumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            GetCurrentValues(out int iMin, out int iMax);
            string sStart = iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET ? string.Empty : iMax.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                iMax = iLevel;
                if (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                    iMin = IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM;
                else if (iMax < iMin)
                    iMin = iMax;
                SetCurrentValues(iMin, iMax);
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiToggleOverrideAutoSpellLevels_Click(object sender, EventArgs e)
        {
            GetCurrentValues(out int iMin, out int iMax);
            if (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                if (_level == AutoSpellLevelOverridesLevel.PermRun)
                {
                    iMin = _strategyAutoSpellLevelMinimum;
                    iMax = _strategyAutoSpellLevelMaximum;
                }
                if (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    iMin = _settingsAutoSpellLevelMinimum;
                    iMax = _settingsAutoSpellLevelMaximum;
                }
            }
            else
            {
                iMin = iMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            }
            SetCurrentValues(iMin, iMax);
            RefreshAutoSpellLevelUI();
        }

        public void RefreshAutoSpellLevelUI()
        {
            string sText = null;
            GetCurrentValues(out int iMin, out int iMax);
            if (iMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && iMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                sText = iMin + ":" + iMax;
            }
            if (_level == AutoSpellLevelOverridesLevel.PermRun && string.IsNullOrEmpty(sText))
            {
                if (_strategyAutoSpellLevelMinimum != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && _strategyAutoSpellLevelMaximum != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    sText = "Strategy: " + _strategyAutoSpellLevelMinimum + ":" + _strategyAutoSpellLevelMaximum;
                }
            }
            if ((_level == AutoSpellLevelOverridesLevel.PermRun || _level == AutoSpellLevelOverridesLevel.Strategy) && string.IsNullOrEmpty(sText))
            {
                sText = "Settings: " + _settingsAutoSpellLevelMinimum + ":" + _settingsAutoSpellLevelMaximum;
            }
            _lblAutoSpellLevels.Text = sText;
        }

        public void GetEffectiveMinMax(out int iMin, out int iMax)
        {
            iMin = iMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            if (_level == AutoSpellLevelOverridesLevel.PermRun)
            {
                iMin = _permRunAutoSpellLevelMinimum;
                iMax = _permRunAutoSpellLevelMaximum;
            }
            if ((_level == AutoSpellLevelOverridesLevel.PermRun || _level == AutoSpellLevelOverridesLevel.Strategy) && (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET))
            {
                iMin = _strategyAutoSpellLevelMinimum;
                iMax = _strategyAutoSpellLevelMaximum;
            }
            if (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                iMin = _settingsAutoSpellLevelMinimum;
                iMax = _settingsAutoSpellLevelMaximum;
            }
        }
    }
}
