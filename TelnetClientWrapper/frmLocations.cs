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
        private bool _readOnly;

        public frmLocations(IsengardMap fullMap, IsengardSettingData settingsData, Room currentRoom, bool forRoomSelection, Func<GraphInputs> gi, bool readOnly)
        {
            InitializeComponent();

            _fullMap = fullMap;
            _settingsData = settingsData;
            _readOnly = readOnly;
            CurrentRoom = currentRoom;
            _graphInputs = gi;
            _forRoomSelection = forRoomSelection;
            SetFormTitle();
            PopulateTree();
        }

        private void SetFormTitle()
        {
            string sText;
            if (CurrentRoom == null)
                sText = "Locations (None)";
            else
                sText = "Locations (" + CurrentRoom.DisplayName + ")";
            this.Text = sText;
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
            TreeNode ret = new TreeNode(node.GetDisplayName());
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

        private TreeNode DisplayNodeForm(LocationNode startingPoint)
        {
            TreeNode ret = null;
            frmLocationNode frm = new frmLocationNode(startingPoint, CurrentRoom, _fullMap, _graphInputs);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                LocationNode ln = new LocationNode();
                ln.DisplayName = frm.DisplayName;
                ln.RoomObject = frm.SelectedRoom;
                ln.Room = _fullMap.GetRoomTextIdentifier(ln.RoomObject);
                ret = CreateLocationNode(ln);
            }
            return ret;
        }

        private void ctxTree_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem tsi = e.ClickedItem;
            TreeNode selectedNode = treeLocations.SelectedNode;
            LocationNode currentLoc = null;
            TreeNode parentNode = null;
            if (selectedNode != null)
            {
                currentLoc = (LocationNode)selectedNode.Tag;
                parentNode = selectedNode.Parent;
            }
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
                        if (selectedNode == null) //add top level node
                        {
                            treeLocations.Nodes.Add(newNodeInfo);
                        }
                        else //add sub node to the current node
                        {
                            selectedNode.Nodes.Add(newNodeInfo);
                            if (currentLoc.Children == null) currentLoc.Children = new List<LocationNode>();
                            currentLoc.Children.Add(newLoc);
                            newLoc.ParentID = currentLoc.ID;
                            newLoc.Parent = currentLoc;
                            selectedNode.Expand();
                        }
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
                    currentLoc.RoomObject = newLoc.RoomObject;
                    selectedNode.Text = newLoc.GetDisplayName();
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
            else if (tsi == tsmiSetAsCurrentLocation)
            {
                CurrentRoom = currentLoc.RoomObject;
                SetFormTitle();
            }
        }

        private void ctxTree_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TreeNode selectedNode = treeLocations.SelectedNode;
            bool haveNode = selectedNode != null;
            bool showSetCurrentLocation = false;
            if (haveNode)
            {
                TreeNodeCollection parentCollection = selectedNode.Parent == null ? treeLocations.Nodes : selectedNode.Parent.Nodes;
                int iIndex = parentCollection.IndexOf(selectedNode);
                tsmiMoveDown.Enabled = iIndex < parentCollection.Count - 1;
                tsmiMoveUp.Enabled = iIndex > 0;
                if (!_readOnly && !_forRoomSelection)
                {
                    Room r = ((LocationNode)selectedNode.Tag).RoomObject;
                    showSetCurrentLocation = r != null && r != CurrentRoom;
                }
            }
            tsmiAddSiblingAfter.Visible = haveNode;
            tsmiAddSiblingBefore.Visible = haveNode;
            tsmiEdit.Visible = haveNode;
            tsmiMoveDown.Visible = haveNode;
            tsmiMoveUp.Visible = haveNode;
            tsmiRemove.Visible = haveNode;
            tsmiSetAsCurrentLocation.Visible = showSetCurrentLocation;
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
            if (!_readOnly)
            {
                Room r = ((LocationNode)treeLocations.SelectedNode.Tag).RoomObject;
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
}
