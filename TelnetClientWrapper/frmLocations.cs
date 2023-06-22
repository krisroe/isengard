using QuickGraph;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IsengardClient
{
    internal partial class frmLocations : Form
    {
        private IsengardMap _fullMap;
        private bool _flying;
        private bool _levitating;
        private bool _isDay;
        private int _level;

        public frmLocations(IsengardMap fullMap, Room currentRoom, bool flying, bool levitating, bool isDay, int level)
        {
            InitializeComponent();

            _fullMap = fullMap;
            CurrentRoom = currentRoom;
            _flying = flying;
            _levitating = levitating;
            _isDay = isDay;
            _level = level;

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
            SelectedPath = MapComputation.ComputeLowestCostPath(this.CurrentRoom, selectedRoom, _flying, _levitating, _isDay, _level);
            if (SelectedPath == null)
            {
                MessageBox.Show("No path to target room found.", "Go to Room", MessageBoxButtons.OK);
                return;
            }
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
            btnGo.Enabled = CurrentRoom != null && !isCurrentRoom;
        }
    }
}
