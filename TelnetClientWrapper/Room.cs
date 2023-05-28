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

        public Room(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
        public string Mob1 { get; set; }
        public string Mob2 { get; set; }
        public string Mob3 { get; set; }
        public Dictionary<Variable, string> VariableValues { get; set; }
        public AlignmentType? Alignment { get; set; }
        public int? Experience1 { get; set; }
        public int? Experience2 { get; set; }
        public int? Experience3 { get; set; }
        public bool IsHealingRoom { get; set; }
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
