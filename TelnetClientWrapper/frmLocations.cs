using QuickGraph;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IsengardClient
{
    internal partial class frmLocations : Form
    {
        private IsengardMap _fullMap;
        private GraphInputs _graphInputs;
        private bool _forRoomSelection;

        public frmLocations(IsengardMap fullMap, Room currentRoom, bool forRoomSelection, GraphInputs gi)
        {
            InitializeComponent();

            _fullMap = fullMap;
            CurrentRoom = currentRoom;
            _graphInputs = gi;
            _forRoomSelection = forRoomSelection;

            if (forRoomSelection)
            {
                btnSet.Visible = false;
                btnGo.Text = "Select";
            }

            PopulateTree();
        }

        public Room CurrentRoom
        {
            get;
            set;
        }

        public List<Exit> SelectedPath
        {
            get;
            set;
        }

        public Room SelectedRoom
        {
            get;
            set;
        }

        private void PopulateTree()
        {
            foreach (Area a in _fullMap.Areas)
            {
                TreeNode tArea = new TreeNode(a.Name);
                tArea.Tag = a;
                treeLocations.Nodes.Add(tArea);
                tArea.Expand();
                foreach (Room r in a.Locations)
                {
                    TreeNode tRoom = new TreeNode(r.GetRoomNameWithExperience());
                    tRoom.Tag = r;
                    tArea.Nodes.Add(tRoom);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();

        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            CurrentRoom = (Room)treeLocations.SelectedNode.Tag;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            Room selectedRoom = (Room)treeLocations.SelectedNode.Tag;
            if (!_forRoomSelection)
            {
                SelectedPath = MapComputation.ComputeLowestCostPath(this.CurrentRoom, selectedRoom, _graphInputs);
                if (SelectedPath == null)
                {
                    MessageBox.Show("No path to target room found.", "Go to Room", MessageBoxButtons.OK);
                    return;
                }
            }
            SelectedRoom = selectedRoom;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void treeLocations_AfterSelect(object sender, TreeViewEventArgs e)
        {
            bool isCurrentRoom = false;
            TreeNode tn = treeLocations.SelectedNode;
            if (tn != null)
            {
                Room r = tn.Tag as Room;
                if (r != null)
                {
                    isCurrentRoom = r == CurrentRoom;
                }
            }
            btnSet.Enabled = !isCurrentRoom;
            btnGo.Enabled = CurrentRoom != null && (!isCurrentRoom || _forRoomSelection);
        }
    }
}
