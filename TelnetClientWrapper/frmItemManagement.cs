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
                Room ret;
                if (cboTargetRoom.SelectedIndex <= 0)
                    ret = null;
                else
                    ret = ((RoomEntry)cboTargetRoom.SelectedItem).Room;
                return ret;
            }
        }

        public HealingRoom? TickRoom
        {
            get
            {
                HealingRoom? ret;
                if (cboTick.SelectedIndex == 0)
                {
                    ret = null;
                }
                else
                {
                    ret = (HealingRoom)cboTick.SelectedItem;
                }
                return ret;
            }
        }

        public PawnShoppe? PawnShop
        {
            get
            {
                PawnShoppe? ret;
                if (cboPawn.SelectedIndex == 0)
                {
                    ret = null;
                }
                else
                {
                    ret = (PawnShoppe)cboPawn.SelectedItem;
                }
                return ret;
            }
        }

        public List<SelectedItemWithTarget> ItemsToChange
        {
            get;
            set;
        }

        public frmItemManagement(CurrentEntityInfo cei, IsengardMap gameMap, Func<GraphInputs> GraphInputs, IsengardSettingData settings, Area currentArea)
        {
            InitializeComponent();

            dgvItems.AlternatingRowsDefaultCellStyle = UIShared.GetAlternatingDataGridViewCellStyle();

            cboTick.Items.Add(string.Empty);
            foreach (HealingRoom next in Enum.GetValues(typeof(HealingRoom))) cboTick.Items.Add(next);
            if (currentArea != null && currentArea.TickRoom.HasValue)
                cboTick.SelectedItem = currentArea.TickRoom.Value;
            else
                cboTick.SelectedIndex = 0;

            cboPawn.Items.Add(string.Empty);
            foreach (PawnShoppe next in Enum.GetValues(typeof(PawnShoppe))) cboPawn.Items.Add(next);
            if (currentArea != null && currentArea.PawnShop.HasValue)
                cboPawn.SelectedItem = currentArea.PawnShop.Value;
            else
                cboPawn.SelectedIndex = 0;

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
                        AddItemToGrid(new SelectedInventoryOrEquipmentItem(ie, nextItemType, iCounter, ItemLocationType.Room), true);
                    }
                }
                foreach (SelectedInventoryOrEquipmentItem sioei in cei.GetInvEqItems(null, true, true))
                {
                    AddItemToGrid(sioei, false);
                }
            }

            _currentRoom = cei.CurrentRoom;
            cboTargetRoom.Items.Add(string.Empty);
            HashSet<Room> targetRooms = new HashSet<Room>();
            Room r;
            RoomEntry roomEntryToSelect = null;
            foreach (Area a in settings.EnumerateAreas())
            {
                r = a.InventorySinkRoomObject;
                if (r != null && r != _currentRoom && !targetRooms.Contains(r))
                {
                    targetRooms.Add(r);
                    RoomEntry newEntry = new RoomEntry(r);
                    if (roomEntryToSelect == null && currentArea != null && currentArea.InventorySinkRoomObject == r)
                    {
                        roomEntryToSelect = newEntry;
                    }
                    cboTargetRoom.Items.Add(newEntry);
                }
            }
            if (roomEntryToSelect != null)
            {
                cboTargetRoom.SelectedItem = roomEntryToSelect;
            }
        }

        private void AddItemToGrid(SelectedInventoryOrEquipmentItem sioei, bool visible)
        {
            ItemTypeEnum itemType = sioei.ItemType.Value;
            StaticItemData sid = ItemEntity.StaticItemData[itemType];
            bool isSource = sioei.LocationType == ItemLocationType.Room;
            bool isInventory = sioei.LocationType == ItemLocationType.Inventory;
            bool isEquipment = sioei.LocationType == ItemLocationType.Equipment;
            int iIndex = dgvItems.Rows.Add(sid.SingularName, sioei.LocationType.ToString(), isSource, false, false, false, isInventory, isEquipment);
            DataGridViewRow dgvr = dgvItems.Rows[iIndex];
            int iColIndex;
            if (isSource)
                iColIndex = colSource.Index;
            else if (isInventory)
                iColIndex = colInventory.Index;
            else if (isEquipment)
                iColIndex = colEquipment.Index;
            else
                throw new InvalidOperationException();
            dgvr.Visible = visible;
            dgvr.Cells[iColIndex].ReadOnly = true;
            dgvr.Tag = sioei;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool haveTargetItems = false;
            bool haveTickItems = false;
            bool haveSellOrJunkItems = false;
            ItemsToChange = new List<SelectedItemWithTarget>();
            foreach (DataGridViewRow dgvr in dgvItems.Rows)
            {
                SelectedInventoryOrEquipmentItem sioei = (SelectedInventoryOrEquipmentItem)dgvr.Tag;
                ColumnType ct = ColumnType.None;
                ColumnType ctCheck = ColumnType.None;
                switch (sioei.LocationType)
                {
                    case ItemLocationType.Room:
                        ctCheck = ColumnType.Source;
                        break;
                    case ItemLocationType.Inventory:
                        ctCheck = ColumnType.Inventory;
                        break;
                    case ItemLocationType.Equipment:
                        ctCheck = ColumnType.Equipment;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                if (Convert.ToBoolean(dgvr.Cells[colTarget.Index].Value))
                {
                    ct = ColumnType.Target;
                    haveTargetItems = true;
                }
                else if (Convert.ToBoolean(dgvr.Cells[colSource.Index].Value))
                {
                    ct = ColumnType.Source;
                }
                else if (Convert.ToBoolean(dgvr.Cells[colSellOrJunk.Index].Value))
                {
                    ct = ColumnType.SellOrJunk;
                    haveSellOrJunkItems = true;
                }
                else if (Convert.ToBoolean(dgvr.Cells[colTick.Index].Value))
                {
                    ct = ColumnType.Tick;
                    haveTickItems = true;
                }
                else if (Convert.ToBoolean(dgvr.Cells[colInventory.Index].Value))
                {
                    ct = ColumnType.Inventory;
                }
                else if (Convert.ToBoolean(dgvr.Cells[colEquipment.Index].Value))
                {
                    ct = ColumnType.Equipment;
                }
                if (ct != ColumnType.None && ct != ctCheck)
                {
                    SelectedItemWithTarget siwt = new SelectedItemWithTarget();
                    siwt.ItemEntity = sioei.ItemEntity;
                    siwt.ItemType = sioei.ItemType.Value;
                    siwt.Counter = sioei.Counter;
                    switch (sioei.LocationType)
                    {
                        case ItemLocationType.Inventory:
                            siwt.LocationType = ItemManagementLocationType.Inventory;
                            break;
                        case ItemLocationType.Equipment:
                            siwt.LocationType = ItemManagementLocationType.Equipment;
                            break;
                        case ItemLocationType.Room:
                            siwt.LocationType = ItemManagementLocationType.SourceRoom;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    siwt.Target = ct;
                    ItemsToChange.Add(siwt);
                }
            }

            Room rSource = _cei.CurrentRoom;
            if (haveTargetItems)
            {
                if (cboTargetRoom.SelectedIndex <= 0)
                {
                    MessageBox.Show("No target room selected.");
                    return;
                }
                Room rSink = ((RoomEntry)cboTargetRoom.SelectedItem).Room;
                if (rSource == rSink)
                {
                    MessageBox.Show("Source room cannot be the same as the target room.");
                    return;
                }
                GraphInputs gi = _GraphInputs();
                if (_currentRoom != rSource && MapComputation.ComputeLowestCostPath(_currentRoom, rSource, gi) == null)
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
            }

            if (haveTickItems)
            {
                if (cboTick.SelectedIndex == 0)
                {
                    MessageBox.Show("No healing room selected.");
                    return;
                }
            }
            if (haveSellOrJunkItems)
            {
                if (cboPawn.SelectedIndex == 0)
                {
                    MessageBox.Show("No pawn shop selected.");
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTargetLocations_Click(object sender, EventArgs e)
        {
            frmLocations locationsForm = new frmLocations(_gameMap, _settings, _currentRoom, true, _GraphInputs, false);
            locationsForm.ShowDialog();
            EnsureRoomSelectedInDropdown(cboTargetRoom, locationsForm.SelectedRoom);
        }

        private void btnTargetGraph_Click(object sender, EventArgs e)
        {
#if DEBUG
            frmGraph graphForm = new frmGraph(_gameMap, _currentRoom, true, _GraphInputs, VertexSelectionRequirement.ValidPathFromCurrentLocation, false);
            graphForm.ShowDialog();
            EnsureRoomSelectedInDropdown(cboTargetRoom, graphForm.SelectedRoom);
#else
            MessageBox.Show("Not supported in release mode!");
#endif
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
            int iRowIndex = e.RowIndex;
            if (iRowIndex >= 0)
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
                    SelectedInventoryOrEquipmentItem sioei = (SelectedInventoryOrEquipmentItem)dgvr.Tag;
                    DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgvr.Cells[iColIndex];
                    if (Convert.ToBoolean(cell.Value))
                    {
                        if (iColIndex != colTarget.Index) dgvr.Cells[colTarget.Index].Value = false;
                        if (iColIndex != colSource.Index)
                        {
                            dgvr.Cells[colSource.Index].ReadOnly = false;
                            dgvr.Cells[colSource.Index].Value = false;
                        }
                        else
                        {
                            dgvr.Cells[colSource.Index].ReadOnly = true;
                        }
                        if (iColIndex != colSellOrJunk.Index) dgvr.Cells[colSellOrJunk.Index].Value = false;
                        if (iColIndex != colTick.Index) dgvr.Cells[colTick.Index].Value = false;
                        if (iColIndex != colInventory.Index)
                        {
                            dgvr.Cells[colInventory.Index].ReadOnly = false;
                            dgvr.Cells[colInventory.Index].Value = false;
                        }
                        else
                        {
                            dgvr.Cells[colInventory.Index].ReadOnly = true;
                        }
                        if (iColIndex != colEquipment.Index)
                        {
                            dgvr.Cells[colEquipment.Index].ReadOnly = false;
                            dgvr.Cells[colEquipment.Index].Value = false;
                        }
                        else
                        {
                            dgvr.Cells[colEquipment.Index].ReadOnly = true;
                        }
                    }
                    else if (!Convert.ToBoolean(dgvr.Cells[colTarget.Index].Value) 
                          && !Convert.ToBoolean(dgvr.Cells[colSource.Index].Value)
                          && !Convert.ToBoolean(dgvr.Cells[colSellOrJunk.Index].Value)
                          && !Convert.ToBoolean(dgvr.Cells[colTick.Index].Value)
                          && !Convert.ToBoolean(dgvr.Cells[colInventory.Index].Value)
                          && !Convert.ToBoolean(dgvr.Cells[colEquipment.Index].Value))
                    {
                        switch (sioei.LocationType)
                        {
                            case ItemLocationType.Room:
                                dgvr.Cells[colSource.Index].Value = true;
                                break;
                            case ItemLocationType.Inventory:
                                dgvr.Cells[colInventory.Index].Value = true;
                                break;
                            case ItemLocationType.Equipment:
                                dgvr.Cells[colEquipment.Index].Value = true;
                                break;
                        }
                    }
                }
            }
        }

        private void cboTick_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTick.SelectedIndex > 0)
            {
                HealingRoom entry = (HealingRoom)cboTick.SelectedItem;
                if (Enum.TryParse(entry.ToString(), out PawnShoppe pawnShoppe))
                {
                    cboPawn.SelectedItem = pawnShoppe;
                }
            }
        }

        private void cboPawn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPawn.SelectedIndex > 0)
            {
                PawnShoppe entry = (PawnShoppe)cboPawn.SelectedItem;
                if (Enum.TryParse(entry.ToString(), out HealingRoom healingRoom))
                {
                    cboTick.SelectedItem = healingRoom;
                }
            }
        }

        private void chkRoomItems_CheckedChanged(object sender, EventArgs e)
        {
            RefreshVisible();
        }

        private void chkInventory_CheckedChanged(object sender, EventArgs e)
        {
            RefreshVisible();
        }

        private void chkEquipment_CheckedChanged(object sender, EventArgs e)
        {
            RefreshVisible();
        }

        private void RefreshVisible()
        {
            bool showRoomItems = chkRoomItems.Checked;
            bool showInventory = chkInventory.Checked;
            bool showEquipment = chkEquipment.Checked;
            foreach (DataGridViewRow dgvr in dgvItems.Rows)
            {
                SelectedInventoryOrEquipmentItem sioei = (SelectedInventoryOrEquipmentItem)dgvr.Tag;
                bool visible;
                switch (sioei.LocationType)
                {
                    case ItemLocationType.Equipment:
                        visible = showEquipment;
                        break;
                    case ItemLocationType.Inventory:
                        visible = showInventory;
                        break;
                    case ItemLocationType.Room:
                        visible = showRoomItems;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                dgvr.Visible = visible;
            }
        }

        private void ctxItems_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool hasVisible = false;
            foreach (DataGridViewRow dgvr in dgvItems.Rows)
            {
                if (dgvr.Visible)
                {
                    hasVisible = true;
                    break;
                }
            }
            if (hasVisible)
            {
                if (dgvItems.SelectedRows.Count == 0)
                {
                    tsmiTargetAll.Text = "Target All";
                    tsmiTickAll.Text = "Tick All";
                    tsmiSellOrJunkAll.Text = "Sell/Junk All";
                    tsmiInventoryAll.Text = "Inventory All";
                    tsmiEquipmentAll.Text = "Equipment All";
                }
                else
                {
                    tsmiTargetAll.Text = "Target Selected";
                    tsmiTickAll.Text = "Tick Selected";
                    tsmiSellOrJunkAll.Text = "Sell/Junk Selected";
                    tsmiInventoryAll.Text = "Inventory Selected";
                    tsmiEquipmentAll.Text = "Equipment Selected";
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ctxItems_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            object clickedItem = e.ClickedItem;
            DataGridViewCheckBoxColumn col;
            if (clickedItem == tsmiTargetAll) col = colTarget;
            else if (clickedItem == tsmiTickAll) col = colTick;
            else if (clickedItem == tsmiSellOrJunkAll) col = colSellOrJunk;
            else if (clickedItem == tsmiInventoryAll) col = colInventory;
            else if (clickedItem == tsmiEquipmentAll) col = colEquipment;
            else throw new InvalidOperationException();
            int iIndex = col.Index;
            foreach (DataGridViewRow dgvr in GetRowsToCheck())
            {
                dgvr.Cells[iIndex].Value = true;
            }
        }

        private IEnumerable<DataGridViewRow> GetRowsToCheck()
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow dgvr in dgvItems.SelectedRows)
                {
                    if (dgvr.Visible)
                    {
                        yield return dgvr;
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow dgvr in dgvItems.Rows)
                {
                    if (dgvr.Visible)
                    {
                        yield return dgvr;
                    }
                }
            }
        }
    }
}

