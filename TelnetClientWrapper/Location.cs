using System.Collections.Generic;
namespace IsengardClient
{
    internal class LocationNode
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public LocationNode Parent { get; set; }
        public List<LocationNode> Children { get; set; }
        public string Room { get; set; }
        public string DisplayName { get; set; }
        public bool Expanded { get; set; }
        public LocationNode()
        {
        }
        public LocationNode(LocationNode copied, LocationNode parent)
        {
            ID = copied.ID;
            Parent = parent;
            if (copied.Children != null)
            {
                Children = new List<LocationNode>();
                foreach (var next in copied.Children)
                {
                    Children.Add(new LocationNode(next, this));
                }
            }
            Room = copied.Room;
            DisplayName = copied.DisplayName;
            Expanded = copied.Expanded;
        }
        public override string ToString()
        {
            string ret = this.DisplayName;
            if (string.IsNullOrEmpty(ret))
            {
                ret = Room;
            }
            return ret;
        }
        public Room FindRoom(IsengardMap fullMap)
        {
            Room associatedRoom = null;
            if (!string.IsNullOrEmpty(Room))
            {
                if (!fullMap.UnambiguousRoomsByBackendName.TryGetValue(Room, out associatedRoom))
                {
                    fullMap.UnambiguousRoomsByDisplayName.TryGetValue(Room, out associatedRoom);
                }
            }
            return associatedRoom;
        }

        public string GetDisplayName(IsengardMap fullMap)
        {
            Room associatedRoom = FindRoom(fullMap);
            string sDisplayName = DisplayName;
            if (string.IsNullOrEmpty(sDisplayName) && associatedRoom != null)
            {
                sDisplayName = associatedRoom.GetRoomNameWithExperience();
            }
            return sDisplayName;
        }

        public IEnumerable<LocationNode> GetChildNodes()
        {
            if (Children != null)
            {
                foreach (LocationNode next in Children)
                {
                    yield return next;
                    foreach (LocationNode nextSub in next.GetChildNodes())
                    {
                        yield return nextSub;
                    }
                }
            }
        }
    }
}
