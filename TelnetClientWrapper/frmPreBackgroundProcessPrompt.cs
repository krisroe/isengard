using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmPreBackgroundProcessPrompt : Form
    {
        private Func<GraphInputs> _GraphInputs;
        private IsengardMap _gameMap;
        private IsengardSettingData _settingsData;
        private Room _currentRoom;
        private CheckBox _chkPowerAttack;
        private CurrentEntityInfo _currentEntityInfo;

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

        public InventoryProcessWorkflow InventoryFlow
        {
            get
            {
                return (InventoryProcessWorkflow)cboInventoryFlow.SelectedItem;
            }
        }

        public bool IsCombatStrategy { get; set; }
        public string MobText { get; set; }
        public MobTypeEnum? MobType { get; set; }
        public int MobIndex { get; set; }

        public frmPreBackgroundProcessPrompt(IsengardMap gameMap, IsengardSettingData settingsData, PromptedSkills skills, Room currentRoom, string currentMob, Func<GraphInputs> GetGraphInputs, Strategy strategy, HealingRoom? healingRoom, PawnShoppe? pawnShop, InventoryProcessWorkflow invWorkflow, CurrentEntityInfo currentEntityInfo)
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

            cboInventoryFlow.Items.Add(InventoryProcessWorkflow.NoProcessing);
            cboInventoryFlow.Items.Add(InventoryProcessWorkflow.ProcessMonsterDrops);
            cboInventoryFlow.Items.Add(InventoryProcessWorkflow.ProcessAllItemsInRoom);
            cboInventoryFlow.SelectedItem = invWorkflow;

            _GraphInputs = GetGraphInputs;
            _gameMap = gameMap;
            _settingsData = settingsData;
            _currentRoom = currentRoom;
            _currentEntityInfo = currentEntityInfo;

            Strategy = new Strategy(strategy);
            RefreshUIFromStrategy();

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
            if (cboMob.Items.Contains(currentMob))
            {
                cboMob.SelectedItem = currentMob;
            }
            else
            {
                cboMob.Text = currentMob;
            }

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
                            chk.Checked = chk.Visible = Strategy.IsCombatStrategy(CommandType.Melee);
                            _chkPowerAttack = chk;
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
            bool showMob = Strategy.IsCombatStrategy(CommandType.All);
            lblMob.Visible = showMob;
            cboMob.Visible = showMob;
            chkMagic.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            chkMelee.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            chkPotions.Checked = (Strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            cboOnKillMonster.SelectedIndex = (int)Strategy.AfterKillMonsterAction;
            grpStrategy.Text = "Strategy (" + Strategy.ToString() + ")";
            if (_chkPowerAttack != null)
            {
                _chkPowerAttack.Visible = Strategy.IsCombatStrategy(CommandType.Melee);
            }
        }

        private string GetInputMobText()
        {
            string full;
            if (cboMob.SelectedItem == null)
                full = cboMob.Text;
            else
                full = cboMob.SelectedItem.ToString();
            return full;
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
            IsCombatStrategy = Strategy.IsCombatStrategy(CommandType.All);
            if (IsCombatStrategy)
            {
                if (MobEntity.GetMobInfo(GetInputMobText(), out string mobText, out MobTypeEnum? mobType, out int mobIndex))
                {
                    MobText = mobText;
                    MobType = mobType;
                    MobIndex = mobIndex;
                }
                else
                {
                    MessageBox.Show("No mob specified.");
                    return;
                }
            }

            InventoryProcessWorkflow ipw = (InventoryProcessWorkflow)cboInventoryFlow.SelectedItem;
            if (ipw != InventoryProcessWorkflow.NoProcessing && (cboPawnShoppe.SelectedIndex == 0 || cboTickRoom.SelectedIndex == 0))
            {
                if (MessageBox.Show("No pawn/tick room selected. Continue?", "Inventory Processing", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
            }

            GraphInputs graphInputs = _GraphInputs();

            //verify the selected room is still reachable
            Room targetRoom = cboRoom.SelectedItem as Room;
            if (targetRoom != null && targetRoom != _currentRoom)
            {
                SelectedPath = MapComputation.ComputeLowestCostPath(_currentRoom, (Room)cboRoom.SelectedItem, graphInputs);
                if (SelectedPath == null)
                {
                    MessageBox.Show("Cannot find path to selected room.");
                    return;
                }
            }
            //verify healing room is reachable
            if (cboTickRoom.SelectedIndex > 0)
            {
                Room healingRoom = _gameMap.HealingRooms[(HealingRoom)cboTickRoom.SelectedItem];
                if (targetRoom != null && targetRoom != healingRoom)
                {
                    if (MapComputation.ComputeLowestCostPath(targetRoom, healingRoom, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from target room to tick room.");
                        return;
                    }
                    if (MapComputation.ComputeLowestCostPath(healingRoom, targetRoom, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from tick room to healing room.");
                        return;
                    }
                }
            }
            //verify pawn shop is reachable
            if (cboPawnShoppe.SelectedIndex > 0)
            {
                Room pawnShop = _gameMap.PawnShoppes[(PawnShoppe)cboPawnShoppe.SelectedItem];
                if (targetRoom != null && targetRoom != pawnShop)
                {
                    if (MapComputation.ComputeLowestCostPath(targetRoom, pawnShop, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from target room to pawn shop.");
                        return;
                    }
                    if (MapComputation.ComputeLowestCostPath(pawnShop, targetRoom, graphInputs) == null)
                    {
                        MessageBox.Show("Cannot find path from pawn shop to target room.");
                        return;
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            GraphInputs graphInputs = _GraphInputs();
            frmGraph graphForm = new frmGraph(_gameMap, _currentRoom, true, graphInputs, VertexSelectionRequirement.ValidPathFromCurrentLocation);
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
            frmLocations locationsForm = new frmLocations(_gameMap, _settingsData, _currentRoom, true, graphInputs);
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
            HashSet<string> entries = new HashSet<string>();
            lock (_currentEntityInfo)
            {
                int iCounter = 0;
                MobTypeEnum? prevMob = null;
                bool isCurrentRoom = _currentEntityInfo.CurrentRoom == r;
                if (isCurrentRoom)
                {
                    foreach (MobTypeEnum nextMob in _currentEntityInfo.CurrentRoomMobs)
                    {
                        if (prevMob.HasValue && prevMob.Value == nextMob)
                            iCounter++;
                        else
                            iCounter = 1;
                        string sMobText = nextMob.ToString();
                        if (iCounter > 1) sMobText += " 1";
                        prevMob = nextMob;
                        cboMob.Items.Add(sMobText);
                        entries.Add(sMobText);
                    }
                }
                if (r.PermanentMobs != null)
                {
                    prevMob = null;
                    foreach (MobTypeEnum nextMob in r.PermanentMobs)
                    {
                        StaticMobData smb = MobEntity.StaticMobData[nextMob];
                        if (smb.Visibility != MobVisibility.Visible || !isCurrentRoom)
                        {
                            if (prevMob.HasValue && prevMob.Value == nextMob)
                                iCounter++;
                            else
                                iCounter = 1;
                            string sMobText = nextMob.ToString();
                            if (iCounter > 1) sMobText += " 1";
                            prevMob = nextMob;
                            if (!entries.Contains(sMobText))
                            {
                                cboMob.Items.Add(sMobText);
                                entries.Add(sMobText);
                            }
                        }
                    }
                }
                if (!isCurrentRoom && cboMob.Items.Count > 0)
                {
                    cboMob.SelectedIndex = 0;
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

        private void cboPawnShoppe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPawnShoppe.SelectedIndex > 0)
            {
                PawnShoppe ePawnShoppe = (PawnShoppe)cboPawnShoppe.SelectedItem;
                if (Enum.TryParse(ePawnShoppe.ToString(), out HealingRoom healingRoom))
                {
                    cboTickRoom.SelectedItem = healingRoom;
                }
            }
        }

        private void cboTickRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTickRoom.SelectedIndex > 0)
            {
                HealingRoom eHealingRoom = (HealingRoom)cboTickRoom.SelectedItem;
                if (Enum.TryParse(eHealingRoom.ToString(), out PawnShoppe pawnShoppe))
                {
                    cboPawnShoppe.SelectedItem = pawnShoppe;
                }
            }
        }
    }
}
