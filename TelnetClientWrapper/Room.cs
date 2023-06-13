using System;
using System.Collections.Generic;
using static IsengardClient.frmMain;

namespace IsengardClient
{
    internal class Room
    {
        public override string ToString()
        {
            string ret = Name;
            if (Experience1.HasValue)
            {
                if (Experience2.HasValue)
                {
                    if (Experience3.HasValue)
                    {
                        ret += (" " + Experience1.Value + "/" + Experience2.Value + "/" + Experience3.Value);
                    }
                    else
                    {
                        ret += (" " + Experience1.Value + "/" + Experience2.Value);
                    }
                }
                else
                {
                    ret += (" " + Experience1.Value);
                }
            }
            if (Alignment.HasValue)
            {
                ret += " ";
                string sAlign = string.Empty;
                switch (Alignment.Value)
                {
                    case AlignmentType.Blue:
                        sAlign = "Bl";
                        break;
                    case AlignmentType.Grey:
                        sAlign = "Gy";
                        break;
                    case AlignmentType.Red:
                        sAlign = "Rd";
                        break;
                }
                ret += sAlign;
            }
            return ret;
        }

        public Room(string name, string backendName)
        {
            this.Name = name;
            this.BackendName = backendName;
        }
        public string Name { get; set; }
        public string BackendName { get; set; }
        public string Mob1 { get; set; }
        public string Mob2 { get; set; }
        public string Mob3 { get; set; }
        public AlignmentType? Alignment { get; set; }
        public int? Experience1 { get; set; }
        public int? Experience2 { get; set; }
        public int? Experience3 { get; set; }
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

        public string GetDefaultMob()
        {
            string defaultMob = this.Mob1;
            if (!string.IsNullOrEmpty(this.Mob1))
            {
                if (!string.IsNullOrEmpty(this.Mob2))
                {
                    if (!string.Equals(defaultMob, this.Mob2, StringComparison.OrdinalIgnoreCase))
                    {
                        defaultMob = string.Empty;
                    }
                    else if (!string.Equals(defaultMob, this.Mob3, StringComparison.OrdinalIgnoreCase))
                    {
                        defaultMob = string.Empty;
                    }
                }
            }
            return defaultMob;
        }
    }
}
