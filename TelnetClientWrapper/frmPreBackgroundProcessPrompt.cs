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

        public frmPreBackgroundProcessPrompt(IsengardMap gameMap, PromptedSkills skills, Room currentRoom, string currentMob, bool isCombatMacro, Func<GraphInputs> GetGraphInputs, Strategy strategy)
        {
            InitializeComponent();

            _isCombatBackgroundProcess = isCombatMacro;
            _GraphInputs = GetGraphInputs;
            _gameMap = gameMap;
            _currentRoom = currentRoom;
            Strategy = strategy;

            if (currentRoom == null)
            {
                cboRoom.Enabled = false;
            }
            else
            {
                cboRoom.Items.Add(currentRoom);
                foreach (Exit nextEdge in IsengardMap.GetAllRoomExits(currentRoom))
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

            bool showPowerAttack = (skills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
            chkPowerAttack.Visible = showPowerAttack;
            chkPowerAttack.Enabled = showPowerAttack;
            chkPowerAttack.Checked = showPowerAttack;
            bool showManashield = (skills & PromptedSkills.Manashield) == PromptedSkills.Manashield;
            chkManashield.Visible = showManashield;
            chkManashield.Enabled = showManashield;

            btnEditStrategy.Visible = Strategy != null;
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
            Strategy copy = new Strategy(Strategy);
            frmStrategy frm = new frmStrategy(copy);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                Strategy = frm.NewStrategy;
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
    }
}
