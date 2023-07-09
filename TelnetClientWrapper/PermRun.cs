using System;
using System.Windows.Forms;

namespace IsengardClient
{
    internal class PermRun
    {
        public PermRun()
        {

        }
        public PermRun(PermRun copied)
        {
            ID = copied.ID;
            DisplayName = copied.DisplayName;
            TickRoom = copied.TickRoom;
            PawnShop = copied.PawnShop;
            BeforeFull = copied.BeforeFull;
            AfterFull = copied.AfterFull;
            SpellsToCast = copied.SpellsToCast;
            SpellsToPotion = copied.SpellsToPotion;
            SkillsToRun = copied.SkillsToRun;
            TargetRoomIdentifier = copied.TargetRoomIdentifier;
            TargetRoomObject = copied.TargetRoomObject;
            ThresholdRoomIdentifier = copied.ThresholdRoomIdentifier;
            ThresholdRoomObject = copied.ThresholdRoomObject;
            InventorySinkRoomIdentifier = copied.InventorySinkRoomIdentifier;
            InventorySinkRoomObject = copied.InventorySinkRoomObject;
            MobType = copied.MobType;
            MobText = copied.MobText;
            MobIndex = copied.MobIndex;
            UseMagicCombat = copied.UseMagicCombat;
            UseMeleeCombat = copied.UseMeleeCombat;
            UsePotionsCombat = copied.UsePotionsCombat;
            AfterKillMonsterAction = copied.AfterKillMonsterAction;
            AutoSpellLevelMin = copied.AutoSpellLevelMin;
            AutoSpellLevelMax = copied.AutoSpellLevelMax;
            ItemsToProcessType = copied.ItemsToProcessType;
            Strategy = copied.Strategy;
        }
        public bool IsValid { get; set; }
        public int ID { get; set; }
        public int OrderValue { get; set; }
        public string DisplayName { get; set; }
        public HealingRoom? TickRoom { get; set; }
        public PawnShoppe? PawnShop { get; set; }
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
        /// inventory sink room identifier
        /// </summary>
        public string InventorySinkRoomIdentifier { get; set; }
        /// <summary>
        /// inventory sink room object
        /// </summary>
        public Room InventorySinkRoomObject { get; set; }
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
        public ItemsToProcessType ItemsToProcessType { get; set; }

        public bool IsRunnable(Func<GraphInputs> GetGraphInputs, CurrentEntityInfo cei, IWin32Window parent, IsengardMap gameMap)
        {
            GraphInputs graphInputs = GetGraphInputs();
            Room currentRoom = cei.CurrentRoom;
            bool haveTickRoom = TickRoom.HasValue;
            Room healingRoom = haveTickRoom ? gameMap.HealingRooms[TickRoom.Value]: null;

            if (currentRoom == null)
            {
                MessageBox.Show(parent, "No current room.");
                return false;
            }

            Room testRoom = currentRoom;

            if (haveTickRoom && BeforeFull != FullType.None && testRoom != healingRoom) //verify healing room is reachable if needed
            {
                if (MapComputation.ComputeLowestCostPath(testRoom, healingRoom, graphInputs) == null)
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
                if (InventorySinkRoomObject != null && testRoom != InventorySinkRoomObject)
                {
                    if (MapComputation.ComputeLowestCostPath(testRoom, InventorySinkRoomObject, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from target to inventory sink room.");
                        return false;
                    }
                    if (MapComputation.ComputeLowestCostPath(InventorySinkRoomObject, testRoom, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from target to inventory sink room.");
                        return false;
                    }
                }
                testRoom = InventorySinkRoomObject;

                if (PawnShop.HasValue) //verify can get to and from the pawn shop
                {
                    Room pawnShop = gameMap.PawnShoppes[PawnShop.Value];
                    if (TargetRoomObject != pawnShop && testRoom != pawnShop)
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
            }

            if (haveTickRoom && AfterFull != FullType.None && testRoom != healingRoom) //verify healing room is reachable after the workflow
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
                    MessageBox.Show(parent, "Missing skills: " + missingSkills.ToString().Replace(" ", ""));
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
                    MessageBox.Show(parent, "Cannot cast spells: " + missingSpells.ToString().Replace(" ", ""));
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
                    MessageBox.Show(parent, "Missing potions: " + missingSpells.ToString().Replace(" ", ""));
                    return false;
                }
            }
            
            return true;
        }
    }
}
