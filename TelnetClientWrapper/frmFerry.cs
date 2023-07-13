using IsengardClient.Backend;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmFerry : Form
    {
        private IsengardMap _gameMap;
        private Room _currentRoom;
        private Func<GraphInputs> _GraphInputs;
        private IsengardSettingData _settings;
        public Room SourceRoom
        {
            get
            {
                return ((RoomEntry)cboSourceRoom.SelectedItem).Room;
            }
        }
        public Room SinkRoom
        {
            get
            {
                return ((RoomEntry)cboTargetRoom.SelectedItem).Room;
            }
        }

        public frmFerry(CurrentEntityInfo cei, IsengardMap gameMap, Func<GraphInputs> GraphInputs, IsengardSettingData settings)
        {
            InitializeComponent();

            _gameMap = gameMap;
            _GraphInputs = GraphInputs;
            _settings = settings;

            _currentRoom = cei.CurrentRoom;
            cboSourceRoom.Items.Add(new RoomEntry(_currentRoom));
            cboSourceRoom.SelectedIndex = 0;
            cboTargetRoom.Items.Add(string.Empty);
            HashSet<Room> targetRooms = new HashSet<Room>();
            Room r;
            foreach (Area a in settings.EnumerateAreas())
            {
                r = a.InventorySinkRoomObject;
                if (r != null && !targetRooms.Contains(r))
                {
                    targetRooms.Add(r);
                    cboTargetRoom.Items.Add(new RoomEntry(r));
                }
            }
            foreach (HealingRoom next in Enum.GetValues(typeof(HealingRoom)))
            {
                r = gameMap.HealingRooms[next];
                if (!targetRooms.Contains(r))
                {
                    targetRooms.Add(r);
                    cboTargetRoom.Items.Add(new RoomEntry(r));
                }
            }
            foreach (PawnShoppe next in Enum.GetValues(typeof(PawnShoppe)))
            {
                r = gameMap.PawnShoppes[next];
                if (!targetRooms.Contains(r))
                {
                    targetRooms.Add(r);
                    cboTargetRoom.Items.Add(new RoomEntry(r));
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboTargetRoom.SelectedIndex == 0)
            {
                MessageBox.Show("No target room selected.");
                return;
            }
            
            Room rSource = ((RoomEntry)cboSourceRoom.SelectedItem).Room;
            Room rSink = ((RoomEntry)cboTargetRoom.SelectedItem).Room;
            if (rSource == rSink)
            {
                MessageBox.Show("Source room cannot be the same as the target room.");
                return;
            }

            GraphInputs gi = _GraphInputs();
            if (_currentRoom != rSource &&  MapComputation.ComputeLowestCostPath(_currentRoom, rSource, gi) == null)
            {
                MessageBox.Show("Cannot find path to source room.");
                return;
            }
            if (MapComputation.ComputeLowestCostPath(rSource, rSink, gi) == null)
            {
                MessageBox.Show("Cannot find path from source room to target room.");
                return;
            }
            if (MapComputation.ComputeLowestCostPath(rSink, rSource, gi) == null)
            {
                MessageBox.Show("Cannot find path from target room to source room.");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSourceLocations_Click(object sender, EventArgs e)
        {
            DisplayLocations(true);
        }

        private void btnSourceGraph_Click(object sender, EventArgs e)
        {
            DisplayGraph(true);
        }

        private void btnTargetLocations_Click(object sender, EventArgs e)
        {
            DisplayLocations(false);
        }

        private void btnTargetGraph_Click(object sender, EventArgs e)
        {
            DisplayGraph(false);
        }

        private void DisplayLocations(bool isSource)
        {
            frmLocations locationsForm = new frmLocations(_gameMap, _settings, _currentRoom, true, _GraphInputs, false);
            locationsForm.ShowDialog();
            EnsureRoomSelectedInDropdown(isSource ? cboSourceRoom : cboTargetRoom, locationsForm.SelectedRoom);
        }

        private void DisplayGraph(bool isSource)
        {
            frmGraph graphForm = new frmGraph(_gameMap, _currentRoom, true, _GraphInputs, VertexSelectionRequirement.ValidPathFromCurrentLocation, false);
            graphForm.ShowDialog();
            EnsureRoomSelectedInDropdown(isSource ? cboSourceRoom : cboTargetRoom, graphForm.SelectedRoom);
        }

        private void EnsureRoomSelectedInDropdown(ComboBox cbo, Room selectedRoom)
        {
            if (selectedRoom != null)
            {
                RoomEntry found = null;
                foreach (object nextObj in cbo.Items)
                {
                    RoomEntry next = nextObj as RoomEntry;
                    if (next != null && next.Room == selectedRoom)
                    {
                        found = next;
                        break;
                    }
                }
                if (found == null)
                {
                    found = new RoomEntry(selectedRoom);
                    cbo.Items.Add(found);
                }
                cbo.SelectedItem = found;
            }
        }

        private class RoomEntry
        {
            public override string ToString()
            {
                string sRet;
                if (Room.HealingRoom.HasValue)
                {
                    sRet = "Healing " + Room.HealingRoom.Value.ToString() + " " + Room.BackendName;
                }
                else if (Room.PawnShoppe.HasValue)
                {
                    sRet = "Pawn " + Room.PawnShoppe.Value.ToString() + " " + Room.BackendName;
                }
                else
                {
                    sRet = Room.BackendName;
                }
                return sRet;
            }
            public RoomEntry(Room r)
            {
                this.Room = r;
            }
            public Room Room { get; set; }
        }
    }
}

