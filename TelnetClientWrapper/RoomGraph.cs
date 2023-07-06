using System.Collections.Generic;
using System.Windows;
namespace IsengardClient
{
    internal class RoomGraph
    {
        public RoomGraph(MapType mapType, string Name)
        {
            MapType = mapType;
            Rooms = new Dictionary<Room, Point>();
            this.Name = Name;
        }
        public override string ToString()
        {
            return Name;
        }
        public MapType MapType { get; set; }
        public Dictionary<Room, Point> Rooms { get; set; }
        public string Name { get; set; }
        public int ScalingFactor { get; set; }
    }
}
