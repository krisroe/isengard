using System.Collections.Generic;
namespace IsengardClient
{
    internal class LocationNode
    {
        public int ID { get; set; }
        public LocationNode Parent { get; set; }
        public List<LocationNode> Children { get; set; }
        public string Room { get; set; }
        public string DisplayName { get; set; }
        public bool Expanded { get; set; }
    }
}
