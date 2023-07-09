using IsengardClient.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IsengardClient
{
    internal partial class frmArea : Form
    {
        private Area _area;
        private IsengardMap _gameMap;
        private IsengardSettingData _settings;
        private List<Area> _existingAreas;
        private Func<GraphInputs> _getGraphInputs;
        private CurrentEntityInfo _cei;

        public frmArea(Area area, IsengardMap gameMap, IsengardSettingData settings, List<Area> existingAreas, Func<GraphInputs> getGraphInputs, CurrentEntityInfo cei)
        {
            InitializeComponent();

            _area = area;
            _gameMap = gameMap;
            _settings = settings;
            _getGraphInputs = getGraphInputs;
            _existingAreas = existingAreas;
            _cei = cei;

            txtDisplayName.Text = area.DisplayName;

            //populate tick room and pawn shop dropdowns
            cboTickRoom.Items.Add(string.Empty);
            cboPawnShoppe.Items.Add(string.Empty);
            foreach (var nextHealingRoom in Enum.GetValues(typeof(HealingRoom)))
            {
                cboTickRoom.Items.Add(nextHealingRoom);
            }
            foreach (var nextPawnShop in Enum.GetValues(typeof(PawnShoppe)))
            {
                cboPawnShoppe.Items.Add(nextPawnShop);
            }

            if (area.TickRoom.HasValue)
                cboTickRoom.SelectedItem = area.TickRoom.Value;
            else
                cboTickRoom.SelectedIndex = 0;
            if (area.PawnShop.HasValue)
                cboPawnShoppe.SelectedItem = area.PawnShop.Value;
            else
                cboPawnShoppe.SelectedIndex = 0;

            if (area.InventorySinkRoomObject != null)
            {
                cboInventorySinkRoom.Items.Add(area.InventorySinkRoomObject);
                cboInventorySinkRoom.SelectedItem = area.InventorySinkRoomObject;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string sDisplayName = txtDisplayName.Text;
            if (string.IsNullOrEmpty(sDisplayName))
            {
                MessageBox.Show("No display name specified.");
                return;
            }
            else
            {
                Area aExisting = _existingAreas.Find((a) => { return a.DisplayName == sDisplayName; });
                if (aExisting != null && aExisting != _area)
                {
                    MessageBox.Show("Duplicate area display name specified.");
                    return;
                }
            }

            Room inventorySinkRoom = cboInventorySinkRoom.SelectedItem as Room;
            if (inventorySinkRoom != null)
            {
                string sRoomTextIdentifier = _gameMap.GetRoomTextIdentifier(inventorySinkRoom);
                if (string.IsNullOrEmpty(sRoomTextIdentifier))
                {
                    MessageBox.Show("Cannot use this inventory sink room because the backend and display names are ambiguous.", "Perm Run");
                    return;
                }
            }

            _area.DisplayName = txtDisplayName.Text;
            _area.TickRoom = cboTickRoom.SelectedIndex > 0 ? (HealingRoom?)cboTickRoom.SelectedItem : null;
            _area.PawnShop = cboPawnShoppe.SelectedIndex > 0 ? (PawnShoppe?)cboPawnShoppe.SelectedItem : null;

            Room rTemp = cboInventorySinkRoom.SelectedItem as Room;
            _area.InventorySinkRoomIdentifier = rTemp == null ? string.Empty : _gameMap.GetRoomTextIdentifier(rTemp);
            _area.InventorySinkRoomObject = rTemp;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnInventorySinkLocations_Click(object sender, EventArgs e)
        {
            frmLocations locationsForm = new frmLocations(_gameMap, _settings, _cei.CurrentRoom, true, _getGraphInputs, false);
            locationsForm.ShowDialog();
            UIShared.HandleRoomSelected(locationsForm.SelectedRoom, cboInventorySinkRoom);
        }

        private void btnInventorySinkGraph_Click(object sender, EventArgs e)
        {
            Room contextRoom;
            if (cboTickRoom.SelectedIndex > 0)
                contextRoom = _gameMap.HealingRooms[(HealingRoom)cboTickRoom.SelectedItem];
            else if (cboPawnShoppe.SelectedIndex > 0)
                contextRoom = _gameMap.PawnShoppes[(PawnShoppe)cboPawnShoppe.SelectedItem];
            else
                contextRoom = _cei.CurrentRoom;
            frmGraph graphForm = new frmGraph(_gameMap, contextRoom, true, _getGraphInputs, VertexSelectionRequirement.UnambiguousRoomBackendOrDisplayName, false);
            graphForm.ShowDialog();
            UIShared.HandleRoomSelected(graphForm.SelectedRoom, cboInventorySinkRoom);
        }

        private void btnInventorySinkClear_Click(object sender, EventArgs e)
        {
            cboInventorySinkRoom.SelectedItem = null;
            cboInventorySinkRoom.Items.Clear();
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
    }
}
