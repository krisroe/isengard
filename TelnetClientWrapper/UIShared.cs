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
        private int? _currentAutoSpellLevelMaximum;
        private int? _currentAutoSpellLevelMinimum;
        private Label _lblAutoSpellLevels;
        private System.Windows.Forms.ContextMenuStrip ctxAutoSpellLevels;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMinimumAutoSpellLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentMaximumAutoSpellLevel;
        private System.Windows.Forms.ToolStripSeparator sepAutoSpellLevels1;
        private System.Windows.Forms.ToolStripMenuItem tsmiInheritAutoSpellLevels;

        public AutoSpellLevelOverrides(int? min, int? max, Label lblAutoSpellLevels)
        {
            _currentAutoSpellLevelMinimum = min;
            _currentAutoSpellLevelMaximum = max;
            _lblAutoSpellLevels = lblAutoSpellLevels;

            this.ctxAutoSpellLevels = new System.Windows.Forms.ContextMenuStrip();
            this.tsmiSetCurrentMinimumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetCurrentMaximumAutoSpellLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoSpellLevels1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiInheritAutoSpellLevels = new System.Windows.Forms.ToolStripMenuItem();

            // 
            // ctxAutoSpellLevels
            // 
            this.ctxAutoSpellLevels.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoSpellLevels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetCurrentMinimumAutoSpellLevel,
            this.tsmiSetCurrentMaximumAutoSpellLevel,
            this.sepAutoSpellLevels1,
            this.tsmiInheritAutoSpellLevels});
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
            // 
            // sepAutoSpellLevels1
            // 
            this.sepAutoSpellLevels1.Name = "sepAutoSpellLevels1";
            this.sepAutoSpellLevels1.Size = new System.Drawing.Size(218, 6);
            // 
            // tsmiInheritAutoSpellLevels
            // 
            this.tsmiInheritAutoSpellLevels.Name = "tsmiInheritAutoSpellLevels";
            this.tsmiInheritAutoSpellLevels.Size = new System.Drawing.Size(221, 24);
            this.tsmiInheritAutoSpellLevels.Text = "Inherit?";
            this.tsmiInheritAutoSpellLevels.Click += new System.EventHandler(this.tsmiInheritAutoSpellLevels_Click);

            lblAutoSpellLevels.ContextMenuStrip = ctxAutoSpellLevels;

            RefreshAutoSpellLevelUI();
        }

        public int? Minimum
        {
            get
            {
                return _currentAutoSpellLevelMinimum;
            }
            set
            {
                _currentAutoSpellLevelMinimum = value;
            }
        }

        public int? Maximum
        {
            get
            {
                return _currentAutoSpellLevelMaximum;
            }
            set
            {
                _currentAutoSpellLevelMaximum = value;
            }
        }

        private void ctxAutoSpellLevels_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tsmiInheritAutoSpellLevels.Checked = _currentAutoSpellLevelMaximum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && _currentAutoSpellLevelMinimum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
        }

        private void tsmiSetCurrentMinimumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string sStart = _currentAutoSpellLevelMinimum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET ? string.Empty : _currentAutoSpellLevelMinimum.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMinimum = iLevel;
                if (_currentAutoSpellLevelMaximum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    _currentAutoSpellLevelMaximum = IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM;
                }
                else if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMaximum = _currentAutoSpellLevelMinimum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiSetCurrentMaximumAutoSpellLevel_Click(object sender, EventArgs e)
        {
            string sStart = _currentAutoSpellLevelMaximum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET ? string.Empty : _currentAutoSpellLevelMaximum.ToString();
            string level = Interaction.InputBox("Level:", "Enter Level", sStart);
            if (int.TryParse(level, out int iLevel) && iLevel >= IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM && iLevel <= IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _currentAutoSpellLevelMaximum = iLevel;
                if (_currentAutoSpellLevelMinimum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    _currentAutoSpellLevelMinimum = IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM;
                }
                else if (_currentAutoSpellLevelMaximum < _currentAutoSpellLevelMinimum)
                {
                    _currentAutoSpellLevelMinimum = _currentAutoSpellLevelMaximum;
                }
                RefreshAutoSpellLevelUI();
            }
        }

        private void tsmiInheritAutoSpellLevels_Click(object sender, EventArgs e)
        {
            if (_currentAutoSpellLevelMinimum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && _currentAutoSpellLevelMaximum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                _currentAutoSpellLevelMaximum = IsengardSettingData.AUTO_SPELL_LEVEL_MAXIMUM;
                _currentAutoSpellLevelMinimum = IsengardSettingData.AUTO_SPELL_LEVEL_MINIMUM;
            }
            else
            {
                _currentAutoSpellLevelMinimum = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
                _currentAutoSpellLevelMaximum = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            }
            RefreshAutoSpellLevelUI();
        }

        public void RefreshAutoSpellLevelUI()
        {
            if (_currentAutoSpellLevelMinimum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && _currentAutoSpellLevelMaximum == IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                _lblAutoSpellLevels.Text = "Inherit";
            }
            else
            {
                _lblAutoSpellLevels.Text = _currentAutoSpellLevelMinimum + ":" + _currentAutoSpellLevelMaximum;
            }
        }
    }
}
