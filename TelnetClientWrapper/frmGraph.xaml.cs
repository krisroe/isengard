using GraphSharp.Algorithms.Layout;
using GraphSharp.Controls;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace IsengardClient
{
    /// <summary>
    /// Interaction logic for frmGraph.xaml
    /// </summary>
    internal partial class frmGraph : Window
    {
        private AdjacencyGraph<Room, Exit> _map;
        private VertexControl _currentVertexControl;
        private Dictionary<MapType, RoomGraph> _graphs;
        private bool _forVertexSelection;
        private bool _flying;
        private bool _levitating;
        private bool _isDay;
        private int _level;

        internal frmGraph(IsengardMap fullMap, Room currentRoom, bool forVertexSelection, bool flying, bool levitating, bool isDay, int level)
        {
            InitializeComponent();

            _flying = flying;
            _levitating = levitating;
            _isDay = isDay;
            _level = level;
            _graphs = fullMap.Graphs;
            graphLayout.LayoutAlgorithmType = string.Empty;
            graphLayout.LayoutAlgorithmFactory = new RoomLayoutAlgorithmFactory();
            _map = fullMap.MapGraph;
            CurrentRoom = currentRoom;
            _forVertexSelection = forVertexSelection;

            Dictionary<RoomGraph, ComboBoxItem> graphItems = new Dictionary<RoomGraph, ComboBoxItem>();
            foreach (MapType mt in Enum.GetValues(typeof(MapType)))
            {
                RoomGraph nextGraph = _graphs[mt];
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
                foreach (KeyValuePair<MapType, RoomGraph> next in _graphs)
                {
                    RoomGraph nextRoomGraph = next.Value;
                    if (nextRoomGraph.Rooms.ContainsKey(currentRoom))
                    {
                        startingGraph = nextRoomGraph;
                        break;
                    }
                }
            }
            if (startingGraph == null)
            {
                startingGraph = _graphs[0];
            }
            cboGraphs.SelectedItem = graphItems[startingGraph];
        }

        private void cboGraph_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RoomGraph rg = (RoomGraph)((ComboBoxItem)cboGraphs.SelectedItem).Tag;
            RoomBidirectionalGraph rbg = new RoomBidirectionalGraph();
            rbg.ComputedPositions = rg.Rooms;
            HashSet<Room> addedRooms = new HashSet<Room>();
            foreach (KeyValuePair<Room, Point> next in rg.Rooms)
            {
                Room nextRoom = next.Key;
                foreach (Exit nextExit in IsengardMap.GetAllRoomExits(_map, nextRoom))
                {
                    Room targetRoom = nextExit.Target;
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
                        nextExit.ShowAsRedOnGraph = !nextExit.ExitIsUsable(_flying, _levitating, _isDay, _level);
                        rbg.AddEdge(nextExit);
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

        private void Window_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VertexControl vc = e.Source as VertexControl;
            if (vc != null)
            {
                Room oRoom = (Room)vc.Vertex;
                ContextMenu ctx = new ContextMenu();
                MenuItem mnu;
                if (CurrentRoom != oRoom)
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
                    }
                }

                //add menu items for other graphs containing the room
                RoomGraph currentRoomGraph = (RoomGraph)((ComboBoxItem)cboGraphs.SelectedItem).Tag;
                foreach (KeyValuePair<MapType, RoomGraph> next in _graphs)
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

        private void mnuGoToOrSelectRoom_Click(object sender, RoutedEventArgs e)
        {
            Room selectedRoom = (Room)_currentVertexControl.Vertex;
            SelectedPath = MapComputation.ComputeLowestCostPath(this.CurrentRoom, selectedRoom, _map, _flying, _levitating, _isDay, _level);
            if (SelectedPath == null)
            {
                MessageBox.Show("No path to target room found.", "Go to Room", MessageBoxButton.OK);
                return;
            }
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
        public RoomGraphLayoutAlgorithm(RoomBidirectionalGraph g, IDictionary<Room, Point> vertexPositions, RoomGraphLayoutAlgorithmParameters oldParameters) : base(g, vertexPositions, oldParameters)
        {
        }
        protected override void InternalCompute()
        {
            foreach (var next in this.VisitedGraph.ComputedPositions)
            {
                VertexPositions[next.Key] = next.Value;
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
        public Dictionary<Room, Point> ComputedPositions { get; set; }

    }

    internal class RoomLayout : GraphLayout<Room, Exit, RoomBidirectionalGraph>
    {
    }
}
