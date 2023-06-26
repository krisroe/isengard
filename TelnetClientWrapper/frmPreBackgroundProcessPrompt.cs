using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPreBackgroundProcessPrompt : Form
    {
        private bool _isCombatBackgroundProcess;
        private Func<GraphInputs> _GraphInputs;
        private IsengardMap _gameMap;
        private Room _currentRoom;

        public List<Exit> SelectedPath { get; set; }

        public Strategy Strategy { get; set; }
        public HealingRoom? HealingRoom
        {
            get
            {
                return cboTickRoom.SelectedIndex == 0 ? (HealingRoom?)null : (HealingRoom)cboTickRoom.SelectedItem;
            }
        }
        public PawnShoppe? PawnShop
        {
            get
            {
                return cboPawnShoppe.SelectedIndex == 0 ? (PawnShoppe?)null : (PawnShoppe)cboPawnShoppe.SelectedItem;
            }
        }

        public frmPreBackgroundProcessPrompt(IsengardMap gameMap, PromptedSkills skills, Room currentRoom, string currentMob, bool isCombatMacro, Func<GraphInputs> GetGraphInputs, Strategy strategy, HealingRoom? healingRoom, PawnShoppe? pawnShop)
        {
            InitializeComponent();

            cboTickRoom.Items.Add(string.Empty);
            foreach (var nextHealingRoom in Enum.GetValues(typeof(HealingRoom)))
            {
                cboTickRoom.Items.Add(nextHealingRoom);
            }
            if (healingRoom.HasValue)
            {
                cboTickRoom.SelectedItem = healingRoom.Value;
            }
            else
            {
                cboTickRoom.SelectedIndex = 0;
            }
            cboPawnShoppe.Items.Add(string.Empty);
            foreach (var nextPawnShop in Enum.GetValues(typeof(PawnShoppe)))
            {
                cboPawnShoppe.Items.Add(nextPawnShop);
            }
            if (pawnShop.HasValue)
            {
                cboPawnShoppe.SelectedItem = pawnShop;
            }
            else
            {
                cboPawnShoppe.SelectedIndex = 0;
            }
            _isCombatBackgroundProcess = isCombatMacro;
            _GraphInputs = GetGraphInputs;
            _gameMap = gameMap;
            _currentRoom = currentRoom;
            if (strategy != null)
            {
                Strategy = new Strategy(strategy);
                RefreshUIFromStrategy();
            }
            else
            {
                grpStrategy.Visible = false;
            }

            if (currentRoom == null)
            {
                cboRoom.Enabled = false;
            }
            else
            {
                cboRoom.Items.Add(currentRoom);
                foreach (Exit nextEdge in currentRoom.Exits)
                {
                    Room targetRoom = nextEdge.Target;
                    if (currentRoom != targetRoom)
                    {
                        cboRoom.Items.Add(targetRoom);
                    }
                }
                cboRoom.SelectedItem = currentRoom;
            }
            cboMob.Text = currentMob;

            foreach (PromptedSkills nextSkill in Enum.GetValues(typeof(PromptedSkills)))
            {
                if (nextSkill != PromptedSkills.None)
                {
                    if ((skills & nextSkill) == nextSkill)
                    {
                        CheckBox chk = new CheckBox();
                        chk.AutoSize = true;
                        if (nextSkill == PromptedSkills.PowerAttack)
                        {
                            chk.Checked = true;
                        }
                        chk.Margin = new Padding(4);
                        chk.Tag = nextSkill;
                        chk.Text = nextSkill.ToString();
                        chk.UseVisualStyleBackColor = true;
                        flp.Controls.Add(chk);
                    }
                }
            }

            btnEditStrategy.Visible = Strategy != null;
        }

        private void RefreshUIFromStrategy()
        {
            chkMagic.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            chkMelee.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            chkPotions.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            cboOnKillMonster.SelectedIndex = (int)Strategy.AfterKillMonsterAction;
            grpStrategy.Text = "Strategy (" + Strategy.ToString() + ")";
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
                foreach (CheckBox nextSkillCheckbox in flp.Controls)
                {
                    if (nextSkillCheckbox.Checked)
                    {
                        ret |= (PromptedSkills)nextSkillCheckbox.Tag;
                    }
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

            //verify the selected room is still reachable
            if (cboRoom.SelectedItem != null && cboRoom.SelectedItem != _currentRoom)
            {
                GraphInputs graphInputs = _GraphInputs();
                SelectedPath = MapComputation.ComputeLowestCostPath(_currentRoom, (Room)cboRoom.SelectedItem, graphInputs);
                if (SelectedPath == null)
                {
                    MessageBox.Show("Cannot find path to selected room");
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            GraphInputs graphInputs = _GraphInputs();
            frmGraph graphForm = new frmGraph(_gameMap, _currentRoom, true, graphInputs);
            graphForm.ShowDialog();
            if (graphForm.SelectedRoom != null)
            {
                if (!cboRoom.Items.Contains(graphForm.SelectedRoom))
                {
                    cboRoom.Items.Add(graphForm.SelectedRoom);
                }
                cboRoom.SelectedItem = graphForm.SelectedRoom;
            }
        }

        private void btnLocations_Click(object sender, EventArgs e)
        {
            GraphInputs graphInputs = _GraphInputs();
            frmLocations locationsForm = new frmLocations(_gameMap, _currentRoom, true, graphInputs);
            locationsForm.ShowDialog();
            if (locationsForm.SelectedRoom != null)
            {
                if (!cboRoom.Items.Contains(locationsForm.SelectedRoom))
                {
                    cboRoom.Items.Add(locationsForm.SelectedRoom);
                }
                cboRoom.SelectedItem = locationsForm.SelectedRoom;
            }
        }

        private void btnEditStrategy_Click(object sender, EventArgs e)
        {
            frmStrategy frm = new frmStrategy(Strategy);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                Strategy = frm.NewStrategy;
                RefreshUIFromStrategy();
            }
        }

        private void cboRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            Room r = (Room)cboRoom.SelectedItem;
            cboMob.Items.Clear();
            if (r.PermanentMobs != null)
            {
                for (int i = 0; i < r.PermanentMobs.Count; i++)
                {
                    MobTypeEnum eNextPerm = r.PermanentMobs[i];
                    string sMobText = MobEntity.PickMobTextWithinList(eNextPerm, MobEntity.IterateThroughMobs(r.PermanentMobs, i + 1));
                    cboMob.Items.Add(sMobText);
                    if (i == 0)
                    {
                        cboMob.SelectedItem = sMobText;
                    }
                }
            }
        }

        private void chkMagic_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMagic.Checked)
            {
                Strategy.TypesWithStepsEnabled |= CommandType.Magic;
            }
            else
            {
                Strategy.TypesWithStepsEnabled &= ~CommandType.Magic;
            }
            RefreshUIFromStrategy();
        }

        private void chkMelee_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMelee.Checked)
            {
                Strategy.TypesWithStepsEnabled |= CommandType.Melee;
            }
            else
            {
                Strategy.TypesWithStepsEnabled &= ~CommandType.Melee;
            }
            RefreshUIFromStrategy();
        }

        private void chkPotions_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPotions.Checked)
            {
                Strategy.TypesWithStepsEnabled |= CommandType.Potions;
            }
            else
            {
                Strategy.TypesWithStepsEnabled &= ~CommandType.Potions;
            }
            RefreshUIFromStrategy();
        }

        private void cboOnKillMonster_SelectedIndexChanged(object sender, EventArgs e)
        {
            Strategy.AfterKillMonsterAction = (AfterKillMonsterAction)cboOnKillMonster.SelectedIndex;
        }
    }
}
