using System;
using System.Collections.Generic;
namespace IsengardClient
{
    internal class Room
    {
        public override string ToString()
        {
            return Name; //CSRTODO: we used to display experience and alignment here
        }

        public Room(string name, string backendName)
        {
            this.Name = name;
            this.BackendName = backendName;
        }
        public string Name { get; set; }
        public string BackendName { get; set; }
        /// <summary>
        /// region for a healing room
        /// </summary>
        public HealingRoom? HealingRoom { get; set; }
        /// <summary>
        /// realm for the damage type for rooms that damage periodically
        /// </summary>
        public RealmType? DamageType { get; set; }
        /// <summary>
        /// is a room that has a trap when the player enters it
        /// </summary>
        public bool IsTrapRoom { get; set; }
        public bool Intangible { get; set; }

        public BoatEmbarkOrDisembark? BoatLocationType { get; set; }
        public List<MobTypeEnum> PermanentMobs { get; set; }
        public List<MobTypeEnum> NonPermanentMobs { get; set; }

        public void AddPermanentMobs(params MobTypeEnum[] mobs)
        {
            if (PermanentMobs == null) PermanentMobs = new List<MobTypeEnum>();
            foreach (MobTypeEnum nextMob in mobs)
            {
                PermanentMobs.Add(nextMob);
            }
        }

        public void AddNonPermanentMobs(params MobTypeEnum[] mobs)
        {
            if (NonPermanentMobs == null) NonPermanentMobs = new List<MobTypeEnum>();
            foreach (MobTypeEnum nextMob in mobs)
            {
                NonPermanentMobs.Add(nextMob);
            }
        }

        public string GetDefaultMob()
        {
            string ret = string.Empty;
            if (PermanentMobs != null)
            {
                ret = MobEntity.PickWordForMob(PermanentMobs[0]);
            }
            return ret;
        }

        public static int ProcessTrapDamage(string prefix, string suffix, string nextLine)
        {
            int ret = 0;
            if (nextLine.StartsWith(prefix) && nextLine.EndsWith(suffix))
            {
                int iPrefixLen = prefix.Length;
                int iSuffixLen = suffix.Length;
                int iTotalLen = nextLine.Length;
                int iDamageLen = iTotalLen - iPrefixLen - iSuffixLen;
                if (iDamageLen > 0)
                {
                    string sDamage = nextLine.Substring(iPrefixLen, iDamageLen);
                    if (int.TryParse(sDamage, out int iThisDamage) && iThisDamage > 0)
                    {
                        ret = iThisDamage;
                    }
                }
            }
            return ret;
        }
    }

    internal class RoomChange
    {
        public RoomChange()
        {
            this.Exits = new List<string>();
            this.MappedExits = new Dictionary<string, Exit>();
        }
        public RoomChangeType ChangeType { get; set; }
        public Room Room { get; set; }
        public List<string> Exits { get; set; }
        public Dictionary<string, Exit> MappedExits { get; set; }
        public List<Exit> OtherExits = new List<Exit>();
        public int GlobalCounter { get; set; }
    }
}
