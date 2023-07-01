﻿using System;
using System.Windows.Forms;

namespace IsengardClient
{
    internal partial class frmLocationNode : Form
    {
        private Room _currentRoom;
        private IsengardMap _fullMap;
        private Room _selectedRoom;
        private GraphInputs _gi;

        public frmLocationNode(LocationNode input, Room currentRoom, IsengardMap fullMap, GraphInputs gi)
        {
            InitializeComponent();

            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.DisplayName))
                {
                    txtDisplayName.Text = input.DisplayName;
                }
                string sRoom = input.Room;
                if (!string.IsNullOrEmpty(sRoom))
                {
                    if (!_fullMap.UnambiguousRoomsByBackendName.TryGetValue(sRoom, out _selectedRoom))
                    {
                        _fullMap.UnambiguousRoomsByDisplayName.TryGetValue(sRoom, out _selectedRoom);
                    }
                    if (_selectedRoom != null)
                    {
                        txtRoom.Text = _selectedRoom.GetRoomNameWithExperience();
                    }
                }
            }

            _currentRoom = currentRoom;
            _fullMap = fullMap;
            _gi = gi;
        }

        public Room SelectedRoom
        {
            get
            {
                return _selectedRoom;
            }
        }

        public string DisplayName
        {
            get
            {
                return txtDisplayName.Text;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_currentRoom == null && string.IsNullOrEmpty(txtDisplayName.Text))
            {
                MessageBox.Show("Either a display name or room must be specified.");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _selectedRoom = null;
            txtRoom.Text = string.Empty;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Room starterRoom = _selectedRoom ?? _currentRoom;
            frmGraph fg = new frmGraph(_fullMap, starterRoom, true, _gi, VertexSelectionRequirement.UnambiguousRoomBackendOrDisplayName);
            if (fg.ShowDialog().GetValueOrDefault(false))
            {
                _selectedRoom = fg.SelectedRoom;
                txtRoom.Text = _selectedRoom.GetRoomNameWithExperience();
            }
        }
    }
}