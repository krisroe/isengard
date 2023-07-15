using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace IsengardClient.Backend
{
    public class PermRun
    {
        public PermRun()
        {
            Rehome = true;
        }
        public PermRun(PermRun copied)
        {
            ID = copied.ID;
            DisplayName = copied.DisplayName;
            Rehome = copied.Rehome;
            if (copied.Areas != null) Areas = new HashSet<Area>(copied.Areas);
            BeforeFull = copied.BeforeFull;
            AfterFull = copied.AfterFull;
            SpellsToCast = copied.SpellsToCast;
            SpellsToPotion = copied.SpellsToPotion;
            SkillsToRun = copied.SkillsToRun;
            TargetRoomIdentifier = copied.TargetRoomIdentifier;
            TargetRoomObject = copied.TargetRoomObject;
            ThresholdRoomIdentifier = copied.ThresholdRoomIdentifier;
            ThresholdRoomObject = copied.ThresholdRoomObject;
            MobType = copied.MobType;
            MobText = copied.MobText;
            MobIndex = copied.MobIndex;
            UseMagicCombat = copied.UseMagicCombat;
            UseMeleeCombat = copied.UseMeleeCombat;
            UsePotionsCombat = copied.UsePotionsCombat;
            AfterKillMonsterAction = copied.AfterKillMonsterAction;
            AutoSpellLevelMin = copied.AutoSpellLevelMin;
            AutoSpellLevelMax = copied.AutoSpellLevelMax;
            Realms = copied.Realms;
            ItemsToProcessType = copied.ItemsToProcessType;
            Strategy = copied.Strategy;
        }
        public bool IsValid { get; set; }
        public int ID { get; set; }
        public int OrderValue { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// whether to rehome to the area before beginning the workflow
        /// </summary>
        public bool Rehome { get; set; }
        /// <summary>
        /// area(s) for the perm run
        /// </summary>
        public HashSet<Area> Areas { get; set; }
        public FullType BeforeFull { get; set; }
        public FullType AfterFull { get; set; }
        public WorkflowSpells SpellsToCast { get; set; }
        public WorkflowSpells SpellsToPotion { get; set; }
        public PromptedSkills SkillsToRun { get; set; }
        /// <summary>
        /// identifier for the target room
        /// </summary>
        public string TargetRoomIdentifier { get; set; }
        /// <summary>
        /// target room object
        /// </summary>
        public Room TargetRoomObject { get; set; }
        /// <summary>
        /// identifier for the threshold room
        /// </summary>
        public string ThresholdRoomIdentifier { get; set; }
        /// <summary>
        /// threshold room object
        /// </summary>
        public Room ThresholdRoomObject { get; set; }
        /// <summary>
        /// specific mob type
        /// </summary>
        public MobTypeEnum? MobType { get; set; }
        /// <summary>
        /// mob text to use for the mob when not specifying a specific type. The first character of this
        /// text must be lower case.
        /// </summary>
        public string MobText { get; set; }
        /// <summary>
        /// mob index if there is a specifiy mob type/text and not the first mob
        /// mob index for any mob in the room if no mob type/text is specified
        /// </summary>
        public int MobIndex { get; set; }
        public Strategy Strategy { get; set; }
        public bool? UseMagicCombat { get; set; }
        public bool? UseMeleeCombat { get; set; }
        public bool? UsePotionsCombat { get; set; }
        public AfterKillMonsterAction? AfterKillMonsterAction { get; set; }
        public int AutoSpellLevelMin { get; set; }
        public int AutoSpellLevelMax { get; set; }
        public RealmTypeFlags? Realms { get; set; }
        public ItemsToProcessType ItemsToProcessType { get; set; }
        /// <summary>
        /// flow used to initiate a perm run. This is not saved.
        /// </summary>
        public PermRunFlow Flow { get; set; }

        public Area DetermineMostCompatibleArea(Area currentArea)
        {
            Area aBest = null;
            if (Areas != null)
            {
                int? iValue = null;
                if (currentArea == null)
                {
                    foreach (Area nextArea in Areas)
                    {
                        List<Area> pathsBackToHome = nextArea.GetAreaPathBackToHome();
                        if (!iValue.HasValue || pathsBackToHome.Count < iValue.Value)
                        {
                            aBest = nextArea;
                        }
                    }
                }
                else
                {
                    List<Area> currentAreasFromHome = currentArea.GetAreaPathBackToHome();
                    foreach (Area nextArea in Areas)
                    {
                        Area commonParent = nextArea.DetermineCommonParentArea(currentArea);
                        int iIndex = currentAreasFromHome.IndexOf(commonParent);
                        if (!iValue.HasValue || iIndex < iValue.Value)
                        {
                            iValue = iIndex;
                            aBest = nextArea;
                        }
                    }
                }
            }
            return aBest;
        }

        public string GetAreaListAsText()
        {
            return GetAreaListAsText(Areas);
        }
        
        public static string GetAreaListAsText(HashSet<Area> Areas)
        {
            string sAreas;
            if (Areas == null)
            {
                sAreas = "None";
            }
            else
            {
                List<string> lst = new List<string>();
                foreach (Area a in Areas)
                {
                    lst.Add(a.DisplayName);
                }
                lst.Sort();
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (string s in lst)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(",");
                    sb.Append(s);
                }
                sAreas = sb.ToString();
            }
            return sAreas;
        }

        public bool IsRunnable(Func<GraphInputs> GetGraphInputs, CurrentEntityInfo cei, IWin32Window parent, IsengardMap gameMap, Area currentArea)
        {
            Room healingRoom = null;
            Room pawnShop = null;
            Room inventorySinkRoom = null;
            Area afterArea = null;
            if (Areas != null)
            {
                afterArea = DetermineMostCompatibleArea(currentArea);
                if (afterArea.TickRoom.HasValue)
                {
                    healingRoom = gameMap.HealingRooms[afterArea.TickRoom.Value];
                }
                if (afterArea.PawnShop.HasValue)
                {
                    pawnShop = gameMap.PawnShoppes[afterArea.PawnShop.Value];
                }
                inventorySinkRoom = afterArea.InventorySinkRoomObject;
            }

            GraphInputs graphInputs = GetGraphInputs();
            Room currentRoom = cei.CurrentRoom;

            if (currentRoom == null)
            {
                MessageBox.Show(parent, "No current room.");
                return false;
            }

            Room testRoom = currentRoom;

            if (healingRoom == null && (BeforeFull != FullType.None || AfterFull != FullType.None))
            {
                MessageBox.Show(parent, "No tick room specified.");
                return false;
            }

            if (Rehome && currentArea != null && currentArea != afterArea && currentArea.InventorySinkRoomObject != null)
            {
                Area commonParentArea = currentArea.DetermineCommonParentArea(afterArea);
                if (currentArea != commonParentArea && commonParentArea.InventorySinkRoomObject != null && commonParentArea.InventorySinkRoomObject != currentArea.InventorySinkRoomObject)
                {
                    Room rehomeRoom = currentArea.InventorySinkRoomObject;
                    if (rehomeRoom != testRoom && MapComputation.ComputeLowestCostPath(testRoom, rehomeRoom, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path to current area inventory sink room.");
                        return false;
                    }
                    testRoom = rehomeRoom;

                    rehomeRoom = commonParentArea.InventorySinkRoomObject;
                    if (rehomeRoom != testRoom && MapComputation.ComputeLowestCostPath(testRoom, rehomeRoom, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path to common parent area inventory sink room.");
                        return false;
                    }
                    testRoom = rehomeRoom;
                }
            }

            if (BeforeFull != FullType.None)
            {
                if (testRoom != healingRoom && MapComputation.ComputeLowestCostPath(testRoom, healingRoom, graphInputs) == null)
                {
                    MessageBox.Show(parent, "Cannot find path to tick room.");
                    return false;
                }
                testRoom = healingRoom;
            }

            if (ThresholdRoomObject != null && testRoom != ThresholdRoomObject) //verify the threshold room is reachable
            {
                if (MapComputation.ComputeLowestCostPath(testRoom, ThresholdRoomObject, graphInputs) == null)
                {
                    MessageBox.Show(parent, "Cannot find path to threshold room.");
                    return false;
                }
                testRoom = ThresholdRoomObject;
            }

            if (testRoom != TargetRoomObject) //verify the target room is reachable
            {
                if (MapComputation.ComputeLowestCostPath(testRoom, TargetRoomObject, graphInputs) == null)
                {
                    MessageBox.Show(parent, "Cannot find path to target room.");
                    return false;
                }
            }

            if (ItemsToProcessType != ItemsToProcessType.NoProcessing)
            {
                if (inventorySinkRoom != null && testRoom != inventorySinkRoom)
                {
                    if (MapComputation.ComputeLowestCostPath(testRoom, inventorySinkRoom, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from target to inventory sink room.");
                        return false;
                    }
                    if (MapComputation.ComputeLowestCostPath(inventorySinkRoom, testRoom, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from target to inventory sink room.");
                        return false;
                    }
                    testRoom = inventorySinkRoom;
                }

                if (pawnShop != null && testRoom != pawnShop) //verify can get to and from the pawn shop
                {
                    if (MapComputation.ComputeLowestCostPath(testRoom, pawnShop, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path to pawn shop.");
                        return false;
                    }
                    if (MapComputation.ComputeLowestCostPath(pawnShop, testRoom, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from pawn shop.");
                        return false;
                    }
                }
            }

            if (AfterFull != FullType.None && testRoom != healingRoom) //verify healing room is reachable after the workflow
            {
                if (MapComputation.ComputeLowestCostPath(testRoom, healingRoom, graphInputs) == null)
                {
                    MessageBox.Show(parent, "Cannot find path to tick room.");
                    return false;
                }
            }

            //check that skills are actually available
            PromptedSkills skillsToCheck = SkillsToRun;
            if (skillsToCheck != PromptedSkills.None)
            {
                PromptedSkills skillsAvailable = cei.GetAvailableSkills(false);
                PromptedSkills missingSkills = skillsToCheck & ~skillsAvailable;
                if (missingSkills != PromptedSkills.None)
                {
                    MessageBox.Show(parent, "Missing skills: " + StringProcessing.TrimFlagsEnumToString(missingSkills));
                    return false;
                }
            }

            WorkflowSpells missingSpells;

            //check that spells can actually be casted
            if (SpellsToCast != WorkflowSpells.None)
            {
                WorkflowSpells castableSpells = cei.GetAvailableWorkflowSpells(AvailableSpellTypes.Castable);
                missingSpells = SpellsToCast & ~castableSpells;
                if (missingSpells != WorkflowSpells.None)
                {
                    MessageBox.Show(parent, "Cannot cast spells: " + StringProcessing.TrimFlagsEnumToString(missingSpells));
                    return false;
                }
            }

            //verify potions are actually in inventory
            if (SpellsToPotion != WorkflowSpells.None)
            {
                WorkflowSpells availablePotions = cei.GetAvailableWorkflowSpells(AvailableSpellTypes.HavePotions);
                missingSpells = SpellsToPotion & ~availablePotions;
                if (missingSpells != WorkflowSpells.None)
                {
                    MessageBox.Show(parent, "Missing potions: " + StringProcessing.TrimFlagsEnumToString(missingSpells));
                    return false;
                }
            }
            
            return true;
        }
    }
}
