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
        public List<MobTypeEnum> Mobs { get; set; }
        /// <summary>
        /// index where the object should be inserted/removed. This is -1 when inserted at the end of the list.
        /// </summary>
        public int Index { get; set; }
        public Dictionary<string, Exit> MappedExits { get; set; }
        public List<Exit> OtherExits = new List<Exit>();
        public int GlobalCounter { get; set; }
    }

    internal class CurrentRoomInfo
    {
        public Room CurrentRoom { get; set; }
        public Room CurrentRoomUI { get; set; }
        public List<RoomChange> CurrentRoomChanges { get; set; }
        public List<MobTypeEnum> CurrentRoomMobs { get; set; }
        public int RoomChangeCounter { get; set; }
        public int RoomChangeCounterUI { get; set; }
        public List<string> CurrentObviousExits { get; set; }
        public TreeNode tnObviousMobs { get; set; }
        public bool ObviousMobsTNExpanded { get; set; }
        public TreeNode tnObviousExits { get; set; }
        public bool ObviousExitsTNExpanded { get; set; }
        public TreeNode tnOtherExits { get; set; }
        public bool OtherExitsTNExpanded { get; set; }
        public TreeNode tnPermanentMobs { get; set; }
        public bool PermMobsTNExpanded { get; set; }

        public CurrentRoomInfo()
        {
            CurrentRoomChanges = new List<RoomChange>();
            CurrentRoomMobs = new List<MobTypeEnum>();
            RoomChangeCounter = -1;
            RoomChangeCounterUI = -1;
            CurrentObviousExits = new List<string>();
            ObviousExitsTNExpanded = true;
            ObviousMobsTNExpanded = true;
            OtherExitsTNExpanded = true;
            PermMobsTNExpanded = true;

            tnObviousMobs = new TreeNode("Obvious Mobs");
            tnObviousMobs.Name = "tnObviousMobs";
            tnObviousMobs.Text = "Obvious Mobs";
            tnObviousExits = new TreeNode("Obvious Exits");
            tnObviousExits.Name = "tnObviousExits";
            tnObviousExits.Text = "Obvious Exits";
            tnOtherExits = new TreeNode("Other Exits");
            tnOtherExits.Name = "tnOtherExits";
            tnOtherExits.Text = "Other Exits";
            tnPermanentMobs = new TreeNode("Permanent Mobs");
            tnPermanentMobs.Name = "tnPermanentMobs";
            tnPermanentMobs.Text = "Permanent Mobs";
        }

        public bool GetTopLevelTreeNodeExpanded(TreeNode topLevelTreeNode)
        {
            bool ret = false;
            if (topLevelTreeNode == tnObviousMobs)
            {
                ret = ObviousMobsTNExpanded;
            }
            else if (topLevelTreeNode == tnObviousExits)
            {
                ret = ObviousExitsTNExpanded;
            }
            else if (topLevelTreeNode == tnOtherExits)
            {
                ret = OtherExitsTNExpanded;
            }
            else if (topLevelTreeNode == tnPermanentMobs)
            {
                ret = PermMobsTNExpanded;
            }
            return ret;
        }

        public int GetTopLevelTreeNodeLogicalIndex(TreeNode topLevelTreeNode)
        {
            int i = 0;
            if (topLevelTreeNode == tnObviousMobs)
            {
                i = 1;
            }
            else if (topLevelTreeNode == tnObviousExits)
            {
                i = 2;
            }
            else if (topLevelTreeNode == tnOtherExits)
            {
                i = 3;
            }
            else if (topLevelTreeNode == tnPermanentMobs)
            {
                i = 4;
            }
            return i;
        }

        public int FindNewMobInsertionPoint(MobTypeEnum newMob)
        {
            string sSingular = MobEntity.StaticMobData[newMob].SingularName;
            bool isCapitalized = char.IsUpper(sSingular[0]);
            int i = 0;
            int iFoundIndex = -1;
            foreach (MobTypeEnum nextMob in CurrentRoomMobs)
            {
                string sNextSingular = MobEntity.StaticMobData[nextMob].SingularName;
                bool nextIsCapitalized = char.IsUpper(sNextSingular[0]);
                bool isBefore = false;
                if (isCapitalized != nextIsCapitalized)
                {
                    isBefore = isCapitalized;
                }
                else
                {
                    isBefore = sSingular.CompareTo(sNextSingular) < 0;
                }
                if (isBefore)
                {
                    iFoundIndex = i;
                    break;
                }
                i++;
            }
            return iFoundIndex;
        }

        public void SetExpandFlag(TreeNode node, bool expanded)
        {
            if (node == tnObviousMobs)
            {
                ObviousMobsTNExpanded = expanded;
            }
            else if (node == tnObviousExits)
            {
                ObviousExitsTNExpanded = expanded;
            }
            else if (node == tnOtherExits)
            {
                OtherExitsTNExpanded = expanded;
            }
            else if (node == tnPermanentMobs)
            {
                PermMobsTNExpanded = expanded;
            }
        }
    }
}
