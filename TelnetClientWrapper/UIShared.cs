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
        private System.Windows.Forms.ContextMenuStrip ctxAutoSpellLevels;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMinimumAutoSpellLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMaximumAutoSpellLevel;
        private System.Windows.Forms.ToolStripSeparator sepAutoSpellLevels1;
        private System.Windows.Forms.ToolStripMenuItem tsmiToggleOverrideAutoSpellLevels;

        public AutoSpellLevelOverrides(int permRunMin, int permRunMax, int strategyMin, int strategyMax, int settingsMin, int settingsMax, Label lblAutoSpellLevels, AutoSpellLevelOverridesLevel level)
        {
            _permRunAutoSpellLevelMinimum = permRunMin;
            _permRunAutoSpellLevelMaximum = permRunMax;
            _strategyAutoSpellLevelMinimum = strategyMin;
            _strategyAutoSpellLevelMaximum = strategyMax;
            _settingsAutoSpellLevelMinimum = settingsMin;
            _settingsAutoSpellLevelMaximum = settingsMax;
            _lblAutoSpellLevels = lblAutoSpellLevels;
            level = _level;

            this.ctxAutoSpellLevels = new System.Windows.Forms.ContextMenuStrip();
            this.tsmiSetCurrentMinimumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetCurrentMaximumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();

            if (_level != AutoSpellLevelOverridesLevel.Settings)
            {
                this.sepAutoSpellLevels1 = new System.Windows.Forms.ToolStripSeparator();
                this.tsmiToggleOverrideAutoSpellLevels = new System.Windows.Forms.ToolStripMenuItem();
            }

            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.ImageScalingSize = new System.Drawing.Size(20, 20);

            if (_level == AutoSpellLevelOverridesLevel.Settings)
            {
                this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    this.tsmiSetCurrentMinimumAutoSpellLevel,
                    this.tsmiSetCurrentMaximumAutoSpellLevel});
            }
            else
            {
                this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    this.tsmiSetCurrentMinimumAutoSpellLevel,
                    this.tsmiSetCurrentMaximumAutoSpellLevel,
                    this.sepAutoSpellLevels1,
                    this.tsmiToggleOverrideAutoSpellLevels});
            }
            this.ctxAutoSpellLevels.Name = "ctxAutoSpellLevels";
            this.ctxAutoSpellLevels.Size = new System.Drawing.Size(222, 82);
            this.ctxAutoSpellLevels.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoSpellLevels_Opening);
            // 
            // tsmiSetCurrentMinimumAutoSpellLevel
            // 
            this.tsmiSetCurrentMinimumAutoSpellLevel.Name = "tsmiSetCurrentMinimumAutoSpellLevel";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Size = new System.Drawing.Size(221, 24);
            this.tsmiSetCurrentMinimumAutoSpellLevel.Text = "Set Current Minimum";
            this.tsmiSetCurrentMinimumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMinimumAutoSpellLevel_Click);
            // 
            // tsmiSetCurrentMaximumAutoSpellLevel
            // 
            this.tsmiSetCurrentMaximumAutoSpellLevel.Name = "tsmiSetCurrentMaximumAutoSpellLevel";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Size = new System.Drawing.Size(221, 24);
            this.tsmiSetCurrentMaximumAutoSpellLevel.Text = "Set Current Maximum";
            this.tsmiSetCurrentMaximumAutoSpellLevel.Click += new System.EventHandler(this.tsmiSetCurrentMaximumAutoSpellLevel_Click);

            if (_level != AutoSpellLevelOverridesLevel.Settings)
            {
                // 
                // sepAutoSpellLevels1
                // 
                this.sepAutoSpellLevels1.Name = "sepAutoSpellLevels1";
                this.sepAutoSpellLevels1.Size = new System.Drawing.Size(218, 6);
                // 
                // tsmiInheritAutoSpellLevels
                // 
                this.tsmiToggleOverrideAutoSpellLevels.Name = "tsmiInheritAutoSpellLevels";
                this.tsmiToggleOverrideAutoSpellLevels.Size = new System.Drawing.Size(221, 24);
                this.tsmiToggleOverrideAutoSpellLevels.Text = "Inherit?";
                this.tsmiToggleOverrideAutoSpellLevels.Click += new System.EventHandler(this.tsmiToggleOverrideAutoSpellLevels_Click);
            }

            lblAutoSpellLevels.ContextMenuStrip = ctxAutoSpellLevels;
            RefreshAutoSpellLevelUI();
        }

        private void ctxAutoSpellLevels_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_level != AutoSpellLevelOverridesLevel.Settings)
            {
                int iMin, iMax;
                if (_level == AutoSpellLevelOverridesLevel.Strategy)
                {
                    iMin = _strategyAutoSpellLevelMinimum;
                    iMax = _strategyAutoSpellLevelMaximum;
                }
                else if (_level == AutoSpellLevelOverridesLevel.PermRun)
                {
                    iMin = _permRunAutoSpellLevelMinimum;
                    iMax = _permRunAutoSpellLevelMaximum;
                }
                else
                {
                    throw new InvalidOperationException();
                }
                string sText;
                if (iMin == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET || iMax == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                    sText = "Override";
                else
                    sText = "Remove Override";
                tsmiToggleOverrideAutoSpellLevels.Text = sText;
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
