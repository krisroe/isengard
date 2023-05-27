using GraphSharp.Algorithms.Layout;
using GraphSharp.Controls;
using QuickGraph;
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

        internal frmGraph(AdjacencyGraph<Room, Exit> map, List<RoomGraph> graphs, Room currentRoom)
        {
            InitializeComponent();

            graphLayout.LayoutAlgorithmType = string.Empty;
            graphLayout.LayoutAlgorithmFactory = new RoomLayoutAlgorithmFactory();
            _map = map;
            CurrentRoom = currentRoom;

            Dictionary<RoomGraph, ComboBoxItem> graphItems = new Dictionary<RoomGraph, ComboBoxItem>();
            foreach (RoomGraph nextGraph in graphs)
            {
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
                foreach (var nextRoomGraph in graphs)
                {
                    if (nextRoomGraph.Rooms.ContainsKey(currentRoom))
                    {
                        startingGraph = nextRoomGraph;
                        break;
                    }
                }
            }
            if (startingGraph == null)
            {
                startingGraph = graphs[0];
            }
            cboGraphs.SelectedItem = graphItems[startingGraph];
        }

        private void cboGraph_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RoomGraph rg = (RoomGraph)((ComboBoxItem)cboGraphs.SelectedItem).Tag;
            RoomBidirectionalGraph rbg = new RoomBidirectionalGraph();
            rbg.ComputedPositions = rg.Rooms;
            HashSet<Room> addedRooms = new HashSet<Room>();
            foreach (KeyValuePair<Room, Point> next in rg.Rooms)
            {
                Room nextRoom = next.Key;
                if (_map.TryGetOutEdges(nextRoom, out IEnumerable<Exit> edges))
                {
                    foreach (Exit nextExit in edges)
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

        public Room GoToRoom
        {
            get;
            set;
        }

        private void Window_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VertexControl vc = e.Source as VertexControl;
            if (vc != null)
            {
                ContextMenu ctx = new ContextMenu();
                MenuItem mnu;
                if (CurrentRoom != null)
                {
                    mnu = new MenuItem();
                    mnu.Header = "Go";
                    mnu.Click += mnuGoToRoom_Click;
                    ctx.Items.Add(mnu);
                }
                mnu = new MenuItem();
                mnu.Header = "Set";
                mnu.Click += mnuSetRoom_Click;
                ctx.Items.Add(mnu);
                vc.ContextMenu = ctx;
                vc.ContextMenuClosing += Vc_ContextMenuClosing;
                _currentVertexControl = vc;
            }
        }

        private void mnuGoToRoom_Click(object sender, RoutedEventArgs e)
        {
            GoToRoom = (Room)_currentVertexControl.Vertex;
            Close();
        }

        private void mnuSetRoom_Click(object sender, RoutedEventArgs e)
        {
            CurrentRoom = (Room)_currentVertexControl.Vertex;
            txtCurrentRoom.Text = CurrentRoom.ToString();
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
