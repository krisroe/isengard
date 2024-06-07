using GraphShape.Algorithms.Layout;
using GraphShape.Controls;
using IsengardClient.Backend;
using QuikGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace IsengardClient
{
    /// <summary>
    /// Interaction logic for frmGraph.xaml
    /// </summary>
    internal partial class frmGraph : Window
    {
        private VertexControl _currentVertexControl;
        private IsengardMap _fullMap;
        private bool _forVertexSelection;
        private Func<GraphInputs> _graphInputs;
        private VertexSelectionRequirement _selectionRequirement;
        private bool _readOnly;

        internal frmGraph(IsengardMap fullMap, Room currentRoom, bool forVertexSelection, Func<GraphInputs> graphInputs, VertexSelectionRequirement selectionRequirement, bool readOnly)
        {
            InitializeComponent();

            _graphInputs = graphInputs;
            _fullMap = fullMap;
            _readOnly = readOnly;
            graphLayout.LayoutAlgorithmType = string.Empty;
            graphLayout.LayoutAlgorithmFactory = new RoomLayoutAlgorithmFactory();
            CurrentRoom = currentRoom;
            _forVertexSelection = forVertexSelection;
            _selectionRequirement = selectionRequirement;

            Dictionary<RoomGraph, ComboBoxItem> graphItems = new Dictionary<RoomGraph, ComboBoxItem>();
            foreach (MapType mt in Enum.GetValues(typeof(MapType)))
            {
                RoomGraph nextGraph = fullMap.Graphs[mt];
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = nextGraph.Name;
                cbi.Tag = nextGraph;
                cboGraphs.Items.Add(cbi);
                graphItems[nextGraph] = cbi;
            }

            RoomGraph startingGraph = null;
            if (currentRoom != null)
            {
                txtCurrentRoom.Text = currentRoom.ToString();
                if (fullMap.RoomsToMaps.TryGetValue(currentRoom, out MapType currentRoomMapType))
                {
                    startingGraph = fullMap.Graphs[currentRoomMapType];
                }
            }
            if (startingGraph == null)
            {
                startingGraph = fullMap.Graphs[0];
            }
            cboGraphs.SelectedItem = graphItems[startingGraph];
        }

        private void cboGraph_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GraphInputs actualInputs = _graphInputs();
            RoomGraph rg = (RoomGraph)((ComboBoxItem)cboGraphs.SelectedItem).Tag;
            MapType mt = rg.MapType;
            RoomBidirectionalGraph rbg = new RoomBidirectionalGraph();
            rbg.ComputedPositions = rg.Rooms;
            HashSet<Room> addedRooms = new HashSet<Room>();
            foreach (KeyValuePair<Room, PointF> next in rg.Rooms)
            {
                Room nextRoom = next.Key;
                MapType nextMapType = _fullMap.RoomsToMaps[nextRoom];
                foreach (Exit nextExit in nextRoom.Exits)
                {
                    Room targetRoom = nextExit.Target;
                    //there's an ugly link between tree of life and guild street that doesn't show well on the bree streets graph
                    //to avoid that, do not display links between rooms not in the map type for the intangible graph
                    if (_fullMap.RoomsToMaps.TryGetValue(targetRoom, out MapType nextTargetMapType) && 
                        (nextMapType == mt || nextTargetMapType == mt || nextMapType != MapType.Intangible))
                    {
                        if (rg.Rooms.ContainsKey(targetRoom))
                        {
                            if (!addedRooms.Contains(nextRoom))
                            {
                                rbg.AddVertex(nextRoom);
                                addedRooms.Add(nextRoom);
                            }
                            if (!addedRooms.Contains(targetRoom))
                            {
                                rbg.AddVertex(targetRoom);
                                addedRooms.Add(targetRoom);
                            }
                            nextExit.ShowAsRedOnGraph = nextExit.GetCost(actualInputs) == int.MaxValue;
                            rbg.AddEdge(nextExit);
                        }
                    }
                }
            }
            graphLayout.Graph = rbg;
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

        private void Window_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VertexControl vc = e.Source as VertexControl;
            if (vc != null)
            {
                Room oRoom = (Room)vc.Vertex;
                ContextMenu ctx = new ContextMenu();
                MenuItem mnu;
                if (!_readOnly && CurrentRoom != oRoom)
                {
                    if (_forVertexSelection || CurrentRoom != null)
                    {
                        mnu = new MenuItem();
                        mnu.Header = _forVertexSelection ? "Select" : "Go";
                        mnu.Click += mnuGoToOrSelectRoom_Click;
                        ctx.Items.Add(mnu);
                    }
                    if (!_forVertexSelection)
                    {
                        mnu = new MenuItem();
                        mnu.Header = "Set";
                        mnu.Click += mnuSetRoom_Click;
                        ctx.Items.Add(mnu);
                        if (CurrentRoom != null)
                        {
                            mnu = new MenuItem();
                            mnu.Header = "View Path";
                            mnu.Click += mnuViewPath_Click;
                            ctx.Items.Add(mnu);
                        }
                    }
                }

                //add menu items for other graphs containing the room
                RoomGraph currentRoomGraph = (RoomGraph)((ComboBoxItem)cboGraphs.SelectedItem).Tag;
                foreach (KeyValuePair<MapType, RoomGraph> next in _fullMap.Graphs)
                {
                    RoomGraph rg = next.Value;
                    if (rg != currentRoomGraph && rg.Rooms.ContainsKey(oRoom))
                    {
                        mnu = new MenuItem();
                        mnu.Header = rg.Name;
                        mnu.Click += mnuChooseGraph_Click;
                        mnu.Tag = rg;
                        ctx.Items.Add(mnu);
                    }
                }

                if (ctx.Items.Count == 0)
                {
                    e.Handled = true;
                }
                else
                {
                    vc.ContextMenu = ctx;
                    vc.ContextMenuClosing += Vc_ContextMenuClosing;
                    _currentVertexControl = vc;
                }
            }
        }

        private void mnuChooseGraph_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = (MenuItem)e.OriginalSource;
            object oTag = mnu.Tag;
            foreach (ComboBoxItem cbi in cboGraphs.Items)
            {
                if (cbi.Tag == oTag)
                {
                    cboGraphs.SelectedItem = cbi;
                    break;
                }
            }
        }

        private void mnuViewPath_Click(object sender, RoutedEventArgs e)
        {
            Room selectedRoom = (Room)_currentVertexControl.Vertex;
            List<Exit> path = MapComputation.ComputeLowestCostPath(CurrentRoom, selectedRoom, _graphInputs());
            if (path == null)
            {
                MessageBox.Show("No path to target room found.", "View Path", MessageBoxButton.OK);
            }
            else
            {
                StringBuilder sbMessage = new StringBuilder();
                foreach (Exit nextExit in path)
                {
                    sbMessage.AppendLine(nextExit.ExitText);
                }
                MessageBox.Show(sbMessage.ToString(), "View Path");
            }
        }

        private void mnuGoToOrSelectRoom_Click(object sender, RoutedEventArgs e)
        {
            Room selectedRoom = (Room)_currentVertexControl.Vertex;
            if (_selectionRequirement == VertexSelectionRequirement.ValidPathFromCurrentLocation)
            {
                SelectedPath = MapComputation.ComputeLowestCostPath(this.CurrentRoom, selectedRoom, _graphInputs());
                if (SelectedPath == null)
                {
                    MessageBox.Show("No path to target room found.", "Select Room", MessageBoxButton.OK);
                    return;
                }
            }
            else if (_selectionRequirement == VertexSelectionRequirement.UnambiguousRoomBackendOrDisplayName)
            {
                string sRoomTextIdentifier = _fullMap.GetRoomTextIdentifier(selectedRoom);
                if (string.IsNullOrEmpty(sRoomTextIdentifier))
                {
                    MessageBox.Show("Cannot select this room because the backend and display names are ambiguous.", "Select Room", MessageBoxButton.OK);
                    return;
                }
            }
            SelectedRoom = selectedRoom;
            this.DialogResult = true;
            Close();
        }

        private void mnuSetRoom_Click(object sender, RoutedEventArgs e)
        {
            CurrentRoom = (Room)_currentVertexControl.Vertex;
            this.DialogResult = true;
            Close();
        }

        private void Vc_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            ((VertexControl)sender).ContextMenu = null;
        }
    }

    internal class RoomGraphLayoutAlgorithmParameters : LayoutParametersBase
    {
    }

    internal class RoomGraphLayoutAlgorithm : DefaultParameterizedLayoutAlgorithmBase<Room, Exit, RoomBidirectionalGraph, RoomGraphLayoutAlgorithmParameters>
    {
        public RoomGraphLayoutAlgorithm(RoomBidirectionalGraph g, IDictionary<Room, GraphShape.Point> vertexPositions, RoomGraphLayoutAlgorithmParameters oldParameters) : base(g, vertexPositions, oldParameters)
        {
        }
        protected override void InternalCompute()
        {
            foreach (var next in this.VisitedGraph.ComputedPositions)
            {
                VerticesPositions[next.Key] = new GraphShape.Point(next.Value.X, next.Value.Y);
            }
        }
    }

    internal class RoomLayoutAlgorithmFactory : ILayoutAlgorithmFactory<Room, Exit, RoomBidirectionalGraph>
    {
        public IEnumerable<string> AlgorithmTypes
        {
            get { return new[] { string.Empty }; }
        }

        public ILayoutAlgorithm<Room, Exit, RoomBidirectionalGraph> CreateAlgorithm(string newAlgorithmType, ILayoutContext<Room, Exit, RoomBidirectionalGraph> context, ILayoutParameters parameters)
        {
            return new RoomGraphLayoutAlgorithm(context.Graph, context.Positions, null);
        }

        public ILayoutParameters CreateParameters(string algorithmType, ILayoutParameters oldParameters)
        {
            return null;
        }

        public string GetAlgorithmType(ILayoutAlgorithm<Room, Exit, RoomBidirectionalGraph> algorithm)
        {
            return string.Empty;
        }

        public bool IsValidAlgorithm(string algorithmType)
        {
            return true;
        }

        public bool NeedEdgeRouting(string algorithmType)
        {
            return false;
        }

        public bool NeedOverlapRemoval(string algorithmType)
        {
            return false;
        }
    }

    internal class RoomBidirectionalGraph : BidirectionalGraph<Room, Exit>
    {
        public Dictionary<Room, PointF> ComputedPositions { get; set; }

    }

    internal class RoomLayout : GraphLayout<Room, Exit, RoomBidirectionalGraph>
    {
    }
}
