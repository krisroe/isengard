using IsengardClient.Backend;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmItemManagement : Form
    {
        private IsengardMap _gameMap;
        private Room _currentRoom;
        private Func<GraphInputs> _GraphInputs;
        private IsengardSettingData _settings;
        private CurrentEntityInfo _cei;
        public Room SinkRoom
        {
            get
            {
                return ((RoomEntry)cboTargetRoom.SelectedItem).Room;
            }
        }

        public frmItemManagement(CurrentEntityInfo cei, IsengardMap gameMap, Func<GraphInputs> GraphInputs, IsengardSettingData settings)
        {
            InitializeComponent();

            _cei = cei;
            _gameMap = gameMap;
            _GraphInputs = GraphInputs;
            _settings = settings;

            lock (cei.EntityLock)
            {
                ItemTypeEnum? currentItemType = null;
                int iCounter = 0;
                for (int i = 0; i < cei.CurrentRoomItems.Count; i++)
                {
                    ItemEntity ie = cei.CurrentRoomItems[i];
                    if (ie.ItemType.HasValue)
                    {
                        ItemTypeEnum nextItemType = ie.ItemType.Value;
                        if (!currentItemType.HasValue || currentItemType.Value != ie.ItemType.Value)
                        {
                            iCounter = 0;
                        }
                        iCounter++;
                        AddItemToGrid(ie, new SelectedInventoryOrEquipmentItem(ie, nextItemType, iCounter, ItemLocationType.Room));
                    }
                }
                foreach (SelectedInventoryOrEquipmentItem sioei in cei.GetInvEqItems(null, true, true))
                {
                    AddItemToGrid(sioei.ItemEntity, sioei);
                }
            }

            _currentRoom = cei.CurrentRoom;
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

        private void AddItemToGrid(ItemEntity ie, SelectedInventoryOrEquipmentItem sioei)
        {
            ItemTypeEnum itemType = sioei.ItemType;
            StaticItemData sid = ItemEntity.StaticItemData[itemType];
            bool isSource = sioei.LocationType == ItemLocationType.Room;
            bool isInventory = sioei.LocationType == ItemLocationType.Inventory;
            bool isEquipment = sioei.LocationType == ItemLocationType.Equipment;
            int iIndex = dgvItems.Rows.Add(sid.SingularName, sioei.LocationType.ToString(), isSource, false, false, false, isInventory, isEquipment);
            DataGridViewRow dgvr = dgvItems.Rows[iIndex];
            dgvr.Tag = sioei;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboTargetRoom.SelectedIndex == 0)
            {
                MessageBox.Show("No target room selected.");
                return;
            }
            
            Room rSource = _cei.CurrentRoom;
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

        private void btnTargetLocations_Click(object sender, EventArgs e)
        {
            DisplayLocations();
        }

        private void btnTargetGraph_Click(object sender, EventArgs e)
        {
            DisplayGraph();
        }

        private void DisplayLocations()
        {
            frmLocations locationsForm = new frmLocations(_gameMap, _settings, _currentRoom, true, _GraphInputs, false);
            locationsForm.ShowDialog();
            EnsureRoomSelectedInDropdown(cboTargetRoom, locationsForm.SelectedRoom);
        }

        private void DisplayGraph()
        {
            frmGraph graphForm = new frmGraph(_gameMap, _currentRoom, true, _GraphInputs, VertexSelectionRequirement.ValidPathFromCurrentLocation, false);
            graphForm.ShowDialog();
            EnsureRoomSelectedInDropdown(cboTargetRoom, graphForm.SelectedRoom);
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

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int iColIndex = e.ColumnIndex;
            if (iColIndex == colTarget.Index ||
                iColIndex == colSource.Index ||
                iColIndex == colSellOrJunk.Index ||
                iColIndex == colTick.Index ||
                iColIndex == colInventory.Index ||
                iColIndex == colEquipment.Index)
            {
                DataGridViewRow dgvr = dgvItems.Rows[e.RowIndex];
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgvr.Cells[iColIndex];
                if (!Convert.ToBoolean(cell.Value))
                {
                    dgvItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int iColIndex = e.ColumnIndex;
            if (iColIndex == colTarget.Index ||
                iColIndex == colSource.Index ||
                iColIndex == colSellOrJunk.Index ||
                iColIndex == colTick.Index ||
                iColIndex == colInventory.Index ||
                iColIndex == colEquipment.Index)
            {
                DataGridViewRow dgvr = dgvItems.Rows[e.RowIndex];
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgvr.Cells[iColIndex];
                if (Convert.ToBoolean(cell.Value))
                {
                    if (iColIndex != colTarget.Index) dgvr.Cells[colTarget.Index].Value = false;
                    if (iColIndex != colSource.Index) dgvr.Cells[colSource.Index].Value = false;
                    if (iColIndex != colSellOrJunk.Index) dgvr.Cells[colSellOrJunk.Index].Value = false;
                    if (iColIndex != colTick.Index) dgvr.Cells[colTick.Index].Value = false;
                    if (iColIndex != colInventory.Index) dgvr.Cells[colInventory.Index].Value = false;
                    if (iColIndex != colEquipment.Index) dgvr.Cells[colEquipment.Index].Value = false;
                }
            }
        }
    }
}

