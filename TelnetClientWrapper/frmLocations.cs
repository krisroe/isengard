using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IsengardClient
{
    internal partial class frmLocations : Form
    {
        private IsengardMap _fullMap;
        private IsengardSettingData _settingsData;
        private Func<GraphInputs> _graphInputs;
        private bool _forRoomSelection;

        public frmLocations(IsengardMap fullMap, IsengardSettingData settingsData, Room currentRoom, bool forRoomSelection, Func<GraphInputs> gi)
        {
            InitializeComponent();

            _fullMap = fullMap;
            _settingsData = settingsData;
            CurrentRoom = currentRoom;
            _graphInputs = gi;
            _forRoomSelection = forRoomSelection;
            btnSet.Visible = !forRoomSelection;

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
            foreach (LocationNode nextNode in _settingsData.Locations)
            {
                treeLocations.Nodes.Add(CreateLocationNode(nextNode));
            }
        }

        private TreeNode CreateLocationNode(LocationNode node)
        {
            TreeNode ret = new TreeNode(node.GetDisplayName(_fullMap));
            ret.Tag = node;
            if (node.Children != null)
            {
                foreach (LocationNode nextLoc in node.Children)
                {
                    ret.Nodes.Add(CreateLocationNode(nextLoc));
                }
            }
            if (ret.Nodes.Count > 0 && node.Expanded)
            {
                ret.Expand();
            }
            return ret;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            Room r = ((LocationNode)treeLocations.SelectedNode.Tag).FindRoom(_fullMap);
            if (r != null)
            {
                CurrentRoom = r;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void treeLocations_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Room nodeRoom = null;
            TreeNode tn = treeLocations.SelectedNode;
            if (tn != null) nodeRoom = ((LocationNode)tn.Tag).FindRoom(_fullMap);
            btnSet.Enabled = nodeRoom != null && nodeRoom != CurrentRoom;
        }

        private TreeNode DisplayNodeForm(LocationNode startingPoint)
        {
            TreeNode ret = null;
            frmLocationNode frm = new frmLocationNode(startingPoint, CurrentRoom, _fullMap, _graphInputs);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                LocationNode ln = new LocationNode();
                ln.DisplayName = frm.DisplayName;
                string sRoom;
                if (frm.SelectedRoom == null)
                {
                    sRoom = string.Empty;
                }
                else if (_fullMap.UnambiguousRoomsByBackendName.ContainsKey(frm.SelectedRoom.BackendName))
                {
                    sRoom = frm.SelectedRoom.BackendName;
                }
                else if (_fullMap.UnambiguousRoomsByDisplayName[frm.SelectedRoom.Name] != null)
                {
                    sRoom = frm.SelectedRoom.Name;
                }
                else //shouldn't happen
                {
                    MessageBox.Show("Unable to disambiguate room.");
                    return null;
                }
                ln.Room = sRoom;
                ret = CreateLocationNode(ln);
            }
            return ret;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TreeNode newNode = DisplayNodeForm(null);
            if (newNode != null)
            {
                treeLocations.Nodes.Add(newNode);
            }
        }

        private void ctxTree_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem tsi = e.ClickedItem;
            TreeNode selectedNode = treeLocations.SelectedNode;
            LocationNode currentLoc = (LocationNode)selectedNode.Tag;
            TreeNode parentNode = selectedNode.Parent;
            TreeNodeCollection parentTreeNodes = parentNode == null ? treeLocations.Nodes : selectedNode.Parent.Nodes;
            List<LocationNode> parentLocationNodes = parentNode == null ? _settingsData.Locations : currentLoc.Parent.Children;
            int iCurrentIndex = parentTreeNodes.IndexOf(selectedNode);
            bool isTopLevel = currentLoc.Parent == null;
            TreeNode newNodeInfo;
            if (tsi == tsmiAddChild || tsi == tsmiAddSiblingAfter || tsi == tsmiAddSiblingBefore)
            {
                newNodeInfo = DisplayNodeForm(null);
                if (newNodeInfo != null)
                {
                    LocationNode newLoc = (LocationNode)newNodeInfo.Tag;
                    if (tsi == tsmiAddChild)
                    {
                        selectedNode.Nodes.Add(newNodeInfo);
                        if (currentLoc.Children == null) currentLoc.Children = new List<LocationNode>();
                        currentLoc.Children.Add(newLoc);
                        newLoc.ParentID = currentLoc.ID;
                        newLoc.Parent = currentLoc;
                        selectedNode.Expand();
                    }
                    else if (tsi == tsmiAddSiblingBefore)
                    {
                        selectedNode.Parent.Nodes.Insert(iCurrentIndex, newNodeInfo);
                        parentLocationNodes.Insert(iCurrentIndex, newLoc);
                        newLoc.Parent = currentLoc.Parent;
                        if (!isTopLevel)
                        {
                            newLoc.ParentID = currentLoc.Parent.ID;
                        }
                    }
                    else if (tsi == tsmiAddSiblingAfter)
                    {
                        if (iCurrentIndex == parentTreeNodes.Count - 1)
                        {
                            parentTreeNodes.Add(newNodeInfo);
                            parentLocationNodes.Add(newLoc);
                        }
                        else
                        {
                            parentTreeNodes.Insert(iCurrentIndex + 1, newNodeInfo);
                            parentLocationNodes.Insert(iCurrentIndex + 1, newLoc);
                        }
                        newLoc.Parent = currentLoc.Parent;
                        if (!isTopLevel)
                        {
                            newLoc.ParentID = currentLoc.Parent.ID;
                        }
                    }
                }
            }
            else if (tsi == tsmiEdit)
            {
                newNodeInfo = DisplayNodeForm(currentLoc);
                if (newNodeInfo != null)
                {
                    LocationNode newLoc = (LocationNode)newNodeInfo.Tag;
                    currentLoc.DisplayName = newLoc.DisplayName;
                    currentLoc.Room = newLoc.Room;
                    selectedNode.Text = newLoc.GetDisplayName(_fullMap);
                }
            }
            else if (tsi == tsmiRemove)
            {
                if (MessageBox.Show("Are you sure you want to remove this node?", "Remove Node", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    parentTreeNodes.Remove(selectedNode);
                    parentLocationNodes.Remove(currentLoc);
                    if (currentLoc.Parent != null && parentLocationNodes.Count == 0)
                    {
                        currentLoc.Parent.Children = null;
                    }
                }
            }
            else if (tsi == tsmiMoveUp)
            {
                parentTreeNodes.Remove(selectedNode);
                parentLocationNodes.Remove(currentLoc);
                parentTreeNodes.Insert(iCurrentIndex - 1, selectedNode);
                parentLocationNodes.Insert(iCurrentIndex - 1, currentLoc);
                treeLocations.SelectedNode = selectedNode;
            }
            else if (tsi == tsmiMoveDown)
            {
                parentTreeNodes.Remove(selectedNode);
                parentLocationNodes.Remove(currentLoc);
                if (iCurrentIndex == parentTreeNodes.Count)
                {
                    parentTreeNodes.Add(selectedNode);
                    parentLocationNodes.Add(currentLoc);
                }
                else
                {
                    parentTreeNodes.Insert(iCurrentIndex + 1, selectedNode);
                    parentLocationNodes.Insert(iCurrentIndex + 1, currentLoc);
                }
                treeLocations.SelectedNode = selectedNode;
            }
        }

        private void ctxTree_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TreeNode selectedNode = treeLocations.SelectedNode;
            bool haveNode = selectedNode != null;
            if (haveNode)
            {
                TreeNodeCollection parentCollection = selectedNode.Parent == null ? treeLocations.Nodes : selectedNode.Parent.Nodes;
                int iIndex = parentCollection.IndexOf(selectedNode);
                tsmiMoveDown.Enabled = iIndex < parentCollection.Count - 1;
                tsmiMoveUp.Enabled = iIndex > 0;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmLocations_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (TreeNode nextNode in GetAllTreeNodes(treeLocations.Nodes))
            {
                LocationNode ln = (LocationNode)nextNode.Tag;
                ln.Expanded = nextNode.Nodes.Count > 0 && nextNode.IsExpanded;
            }
            List<LocationNode> locs = new List<LocationNode>();
            foreach (TreeNode nextNode in treeLocations.Nodes)
            {
                locs.Add((LocationNode)nextNode.Tag);
            }
            _settingsData.Locations = locs;
        }

        private IEnumerable<TreeNode> GetAllTreeNodes(TreeNodeCollection nextCol)
        {
            foreach (TreeNode next in nextCol)
            {
                yield return next;
                foreach (TreeNode nextSub in GetAllTreeNodes(next.Nodes))
                {
                    yield return nextSub;
                }
            }
        }

        private void treeLocations_DoubleClick(object sender, EventArgs e)
        {
            Room r = ((LocationNode)treeLocations.SelectedNode.Tag).FindRoom(_fullMap);
            if (r != null)
            {
                if (!_forRoomSelection)
                {
                    if (CurrentRoom == null)
                    {
                        MessageBox.Show("No current room.");
                        return;
                    }
                    if (r == CurrentRoom)
                    {
                        MessageBox.Show("Already at selected room.");
                        return;
                    }
                    SelectedPath = MapComputation.ComputeLowestCostPath(this.CurrentRoom, r, _graphInputs());
                    if (SelectedPath == null)
                    {
                        MessageBox.Show("No path to target room found.", "Go to Room", MessageBoxButtons.OK);
                        return;
                    }
                }
                SelectedRoom = r;
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
