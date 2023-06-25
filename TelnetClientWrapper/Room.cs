using System.Collections.Generic;
using System.Windows.Forms;

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
            this.Exits = new List<Exit>();
        }
        public string Name { get; set; }
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
            }
            return ret;
        }
    }
}
