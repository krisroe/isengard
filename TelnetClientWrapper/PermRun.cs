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
            this.ID = copied.ID;
            this.DisplayName = copied.DisplayName;
            this.TickRoom = copied.TickRoom;
            this.PawnShop = copied.PawnShop;
            this.BeforeFull = copied.BeforeFull;
            this.AfterFull = copied.AfterFull;
            this.SpellsToCast = copied.SpellsToCast;
            this.SpellsToPotion = copied.SpellsToPotion;
            this.SkillsToRun = copied.SkillsToRun;
            this.TargetRoomIdentifier = copied.TargetRoomIdentifier;
            this.TargetRoomObject = copied.TargetRoomObject;
            this.ThresholdRoomIdentifier = copied.ThresholdRoomIdentifier;
            this.ThresholdRoomObject = copied.ThresholdRoomObject;
            this.MobType = copied.MobType;
            this.MobText = copied.MobText;
            this.MobIndex = copied.MobIndex;
            this.UseMagicCombat = copied.UseMagicCombat;
            this.UseMeleeCombat = copied.UseMeleeCombat;
            this.UsePotionsCombat = copied.UsePotionsCombat;
            this.AfterKillMonsterAction = copied.AfterKillMonsterAction;
            this.AutoSpellLevelMin = copied.AutoSpellLevelMin;
            this.AutoSpellLevelMax = copied.AutoSpellLevelMax;
            this.ItemsToProcessType = copied.ItemsToProcessType;
            this.Strategy = copied.Strategy;
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

            if (currentRoom == null)
            {
                MessageBox.Show(parent, "No current room.");
                return false;
            }

            if (ThresholdRoomObject != null) //verify the threshold room is reachable from the current room
            {
                if (currentRoom != ThresholdRoomObject)
                {
                    if (MapComputation.ComputeLowestCostPath(currentRoom, ThresholdRoomObject, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path to threshold room.");
                        return false;
                    }
                }
                if (MapComputation.ComputeLowestCostPath(ThresholdRoomObject, TargetRoomObject, graphInputs) == null)
                {
                    MessageBox.Show(parent, "Cannot find path from threshold room to target room.");
                    return false;
                }
            }
            else if (TargetRoomObject != currentRoom) //verify the target room is reachable from the current room
            {
                if (MapComputation.ComputeLowestCostPath(currentRoom, TargetRoomObject, graphInputs) == null)
                {
                    MessageBox.Show(parent, "Cannot find path to selected room.");
                    return false;
                }
            }

            //verify healing room is reachable back and forth from the target room
            if (TickRoom.HasValue && (ItemsToProcessType != ItemsToProcessType.NoProcessing || AfterFull != FullType.None))
            {
                Room healingRoom = gameMap.HealingRooms[TickRoom.Value];
                if (TargetRoomObject != healingRoom)
                {
                    if (MapComputation.ComputeLowestCostPath(TargetRoomObject, healingRoom, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from target room to tick room.");
                        return false;
                    }
                    if (MapComputation.ComputeLowestCostPath(healingRoom, TargetRoomObject, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from tick room to healing room.");
                        return false;
                    }
                }
            }
            //verify pawn shop is reachable back and forth from the target room
            if (PawnShop.HasValue && ItemsToProcessType != ItemsToProcessType.NoProcessing)
            {
                Room pawnShop = gameMap.PawnShoppes[PawnShop.Value];
                if (TargetRoomObject != pawnShop)
                {
                    if (MapComputation.ComputeLowestCostPath(TargetRoomObject, pawnShop, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from target room to pawn shop.");
                        return false;
                    }
                    if (MapComputation.ComputeLowestCostPath(pawnShop, TargetRoomObject, graphInputs) == null)
                    {
                        MessageBox.Show(parent, "Cannot find path from pawn shop to target room.");
                        return false;
                    }
                }
            }

            //check that skills are actually available
            PromptedSkills skillsToCheck = SkillsToRun & ~PromptedSkills.PowerAttack;
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
