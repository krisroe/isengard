using System.Collections.Generic;
namespace IsengardClient
{
    internal class LocationNode
    {
        public bool IsValid { get; set; }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public LocationNode Parent { get; set; }
        public List<LocationNode> Children { get; set; }
        public string Room { get; set; }
        public Room RoomObject { get; set; }
        public string DisplayName { get; set; }
        public bool Expanded { get; set; }
        public LocationNode(LocationNode Parent)
        {
            this.Parent = Parent;
            if (Parent != null) ParentID = Parent.ID;
        }
        public LocationNode(LocationNode copied, LocationNode parent) : this(parent)
        {
            ID = copied.ID;
            if (copied.Children != null)
            {
                Children = new List<LocationNode>();
                foreach (var next in copied.Children)
                {
                    Children.Add(new LocationNode(next, this));
                }
            }
            Room = copied.Room;
            RoomObject = copied.RoomObject;
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

        public string GetDisplayName()
        {
            string sDisplayName = DisplayName;
            if (string.IsNullOrEmpty(sDisplayName) && RoomObject != null)
            {
                sDisplayName = RoomObject.GetRoomNameWithExperience();
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
