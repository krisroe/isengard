﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPreBackgroundProcessPrompt : Form
    {
        private bool _isCombatBackgroundProcess;

        public frmPreBackgroundProcessPrompt(PromptedSkills skills, Room targetRoom, string currentMob, bool isCombatMacro)
        {
            InitializeComponent();

            _isCombatBackgroundProcess = isCombatMacro;

            if (targetRoom != null && targetRoom.PermanentMobs != null)
            {
                for (int i = 0; i < targetRoom.PermanentMobs.Count; i++)
                {
                    MobTypeEnum eNextPerm = targetRoom.PermanentMobs[i];
                    cboMob.Items.Add(MobEntity.PickMobTextWithinList(eNextPerm, MobEntity.IterateThroughMobs(targetRoom.PermanentMobs, i + 1)));
                }
            }
            cboMob.Text = currentMob;

            bool showPowerAttack = (skills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
            chkPowerAttack.Visible = showPowerAttack;
            chkPowerAttack.Enabled = showPowerAttack;
            chkPowerAttack.Checked = showPowerAttack;
            bool showManashield = (skills & PromptedSkills.Manashield) == PromptedSkills.Manashield;
            chkManashield.Visible = showManashield;
            chkManashield.Enabled = showManashield;
        }

        public string Mob
        {
            get
            {
                string ret;
                if (cboMob.SelectedItem == null)
                {
                    ret = cboMob.Text;
                }
                else
                {
                    ret = cboMob.SelectedItem.ToString();
                }
                return ret;
            }
        }

        public PromptedSkills SelectedSkills
        {
            get
            {
                PromptedSkills ret = PromptedSkills.None;
                if (chkPowerAttack.Enabled && chkPowerAttack.Checked)
                {
                    ret |= PromptedSkills.PowerAttack;
                }
                if (chkManashield.Enabled && chkManashield.Checked)
                {
                    ret |= PromptedSkills.Manashield;
                }
                return ret;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_isCombatBackgroundProcess && string.IsNullOrEmpty(this.Mob))
            {
                MessageBox.Show("No mob specified.");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
