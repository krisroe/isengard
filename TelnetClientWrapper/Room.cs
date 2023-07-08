using System.Collections.Generic;
namespace IsengardClient
{
    internal class Room
    {
        public const string UNKNOWN_ROOM = "!@#UNKNOWN$%^";

        public override string ToString()
        {
            return DisplayName;
        }

        public Room(string displayName, string backendName)
        {
            this.DisplayName = displayName;
            this.BackendName = backendName;
            this.Exits = new List<Exit>();
        }
        /// <summary>
        /// room display name, which is a name for display purposes chosen by the client program
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// room backend name, which is the name given to the room by the game
        /// </summary>
        public string BackendName { get; set; }
        /// <summary>
        /// exits
        /// </summary>
        public List<Exit> Exits { get; set; }
        /// <summary>
        /// region for a healing room
        /// </summary>
        public HealingRoom? HealingRoom { get; set; }
        /// <summary>
        /// pawn shoppe
        /// </summary>
        public PawnShoppe? PawnShoppe { get; set; }
        /// <summary>
        /// realm for the damage type for rooms that damage periodically
        /// </summary>
        public RoomDamageType? DamageType { get; set; }
        /// <summary>
        /// is a room that has a trap when the player enters it
        /// </summary>
        public bool IsTrapRoom { get; set; }
        /// <summary>
        /// whether flee is permitted
        /// </summary>
        public bool NoFlee { get; set; }
        public bool Intangible { get; set; }

        public BoatEmbarkOrDisembark? BoatLocationType { get; set; }
        public List<MobTypeEnum> PermanentMobs { get; set; }
        public List<MobTypeEnum> NonPermanentMobs { get; set; }
        public List<ItemTypeEnum> PermanentItems { get; set; }

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

        public void AddPermanentItems(params ItemTypeEnum[] items)
        {
            if (PermanentItems == null) PermanentItems = new List<ItemTypeEnum>();
            foreach (ItemTypeEnum nextItem in items)
            {
                PermanentItems.Add(nextItem);
            }
        }

        public int GetTotalExperience()
        {
            int total = 0;
            foreach (int next in GetExperiences())
            {
                total += next;
            }
            return total;
        }

        public IEnumerable<int> GetExperiences()
        {
            if (PermanentMobs != null && PermanentMobs.Count > 0)
            {
                foreach (MobTypeEnum nextMobType in PermanentMobs)
                {
                    StaticMobData smd = MobEntity.StaticMobData[nextMobType];
                    if (smd.Experience > 0)
                    {
                        yield return smd.Experience;
                    }
                }
            }
        }

        public string GetRoomNameWithExperience()
        {
            string ret = ToString();
            if (PermanentMobs != null && PermanentMobs.Count > 0)
            {
                List<int> exps = new List<int>();
                foreach (int nextExperience in GetExperiences())
                {
                    exps.Add(nextExperience);
                }
                if (exps.Count > 0)
                {
                    ret += " ";
                    exps.Sort();
                    exps.Reverse();
                    bool isFirst = true;
                    foreach (int nextExp in exps)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                        }
                        else
                        {
                            ret += ",";
                        }
                        ret += nextExp.ToString();
                    }
                }
                AlignmentType? alignType = null;
                foreach (MobTypeEnum nextMobType in PermanentMobs)
                {
                    StaticMobData smd = MobEntity.StaticMobData[nextMobType];
                    if (smd.Alignment.HasValue)
                    {
                        if (alignType.HasValue)
                        {
                            if (alignType.Value != smd.Alignment.Value)
                            {
                                alignType = null;
                                break;
                            }
                        }
                        else
                        {
                            alignType = smd.Alignment.Value;
                        }
                    }
                    else
                    {
                        alignType = null;
                        break;
                    }
                }
                if (alignType.HasValue)
                {
                    ret += " " + StaticMobData.GetAlignmentString(alignType.Value);
                }
            }
            return ret;
        }
    }
}
