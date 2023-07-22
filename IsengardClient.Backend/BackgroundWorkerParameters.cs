using System;
using System.Collections.Generic;
namespace IsengardClient.Backend
{
    public class BackgroundWorkerParameters
    {
        public BackgroundWorkerParameters()
        {
            QueuedCommands = new List<string>();
        }
        /// <summary>
        /// current area
        /// </summary>
        public Area CurrentArea { get; set; }
        /// <summary>
        /// common parent area
        /// </summary>
        public Area CommonParentArea { get; set; }
        /// <summary>
        /// new area
        /// </summary>
        public Area NewArea { get; set; }
        /// <summary>
        /// exits to traverse in the background
        /// </summary>
        public List<Exit> Exits { get; set; }
        /// <summary>
        /// target room if not passing a specific set of exits
        /// </summary>
        public Room TargetRoom { get; set; }
        /// <summary>
        /// inventory sink room
        /// </summary>
        public Room InventorySinkRoom { get; set; }
        /// <summary>
        /// inventory management flow
        /// </summary>
        public InventoryManagementWorkflow InventoryManagementFlow { get; set; }
        public bool Cancelled { get; set; }
        public Strategy Strategy { get; set; }
        public List<string> QueuedCommands { get; set; }
        public MagicStrategyStep? QueuedMagicStep { get; set; }
        public PotionsStrategyStep? QueuedPotionsStep { get; set; }
        public bool Hazy { get; set; }
        public bool Hazied { get; set; }
        public bool Flee { get; set; }
        public bool Fled { get; set; }
        public bool DoScore { get; set; }
        /// <summary>
        /// text for a mob identified by a word
        /// </summary>
        public string MobText { get; set; }
        /// <summary>
        /// counter for a mob identified by a word
        /// </summary>
        public int MobTextCounter { get; set; }
        /// <summary>
        /// type for a mob identified by a type
        /// </summary>
        public MobTypeEnum? MobType { get; set; }
        /// <summary>
        /// counter for a mob identified by a type
        /// </summary>
        public int MobTypeCounter { get; set; }
        /// <summary>
        /// inventory items
        /// </summary>
        public List<SelectedInventoryOrEquipmentItem> InventoryItems { get; set; }
        /// <summary>
        /// whether to cure poison if needed. This is used by the standalone cure-poison option.
        /// </summary>
        public bool CureIfPoisoned { get; set; }
        public BackgroundCommandType? SingleCommandType { get; set; }
        public CommandResultObject SingleCommandResultObject { get; set; }
        public bool SaveSettingsOnQuit { get; set; }
        public bool LogoutOnQuit { get; set; }
        public bool MonsterKilled { get; set; }
        public MobTypeEnum? MonsterKilledType { get; set; }
        public bool AtDestination { get; set; }
        public bool Success { get; set; }
        /// <summary>
        /// perm run info
        /// </summary>
        public PermRun PermRun { get; set; }
        /// <summary>
        /// items to process in the inventory management flow
        /// </summary>
        public ItemsToProcessType InventoryProcessInputType { get; set; }
        /// <summary>
        /// whether resuming an existing perm run
        /// </summary>
        public bool Resume { get; set; }
        /// <summary>
        /// whether a mob is targeted for combat
        /// </summary>
        /// <returns>true if a mob is targeted, false otherwise</returns>
        public bool HasTargetMob()
        {
            return !string.IsNullOrEmpty(MobText) || MobType.HasValue || MobTypeCounter >= 1;
        }
        public void SetPermRun(PermRun p, IsengardMap gameMap, GraphInputs graphInputs)
        {
            PermRun = p;

            //modify the strategy with overrides from the perm run.
            if (p.Strategy != null)
            {
                Strategy = new Strategy(p.Strategy);
                if (p.StrategyOverrides.AutoSpellLevelMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && p.StrategyOverrides.AutoSpellLevelMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    Strategy.AutoSpellLevelMin = p.StrategyOverrides.AutoSpellLevelMin;
                    Strategy.AutoSpellLevelMax = p.StrategyOverrides.AutoSpellLevelMax;
                }
                if (p.StrategyOverrides.Realms.HasValue)
                {
                    Strategy.Realms = p.StrategyOverrides.Realms;
                }
                if (p.StrategyOverrides.AfterKillMonsterAction.HasValue)
                {
                    Strategy.AfterKillMonsterAction = p.StrategyOverrides.AfterKillMonsterAction.Value;
                }
                if (p.StrategyOverrides.UseMagicCombat.HasValue)
                {
                    if (p.StrategyOverrides.UseMagicCombat.Value)
                        Strategy.TypesWithStepsEnabled |= CommandType.Magic;
                    else
                        Strategy.TypesWithStepsEnabled &= ~CommandType.Magic;
                }
                if (p.StrategyOverrides.UseMeleeCombat.HasValue)
                {
                    if (p.StrategyOverrides.UseMeleeCombat.Value)
                        Strategy.TypesWithStepsEnabled |= CommandType.Melee;
                    else
                        Strategy.TypesWithStepsEnabled &= ~CommandType.Melee;
                }
                if (p.StrategyOverrides.UsePotionsCombat.HasValue)
                {
                    if (p.StrategyOverrides.UsePotionsCombat.Value)
                        Strategy.TypesWithStepsEnabled |= CommandType.Potions;
                    else
                        Strategy.TypesWithStepsEnabled &= ~CommandType.Potions;
                }
            }

            int iMobIndex = p.MobIndex;
            if (p.MobType.HasValue)
            {
                MobType = p.MobType;
                MobTypeCounter = iMobIndex;
            }
            else if (!string.IsNullOrEmpty(p.MobText))
            {
                MobText = p.MobText;
                MobTextCounter = iMobIndex;
            }
            else if (iMobIndex >= 1)
            {
                MobTypeCounter = MobTextCounter = 1;
            }
            if (p.Areas != null)
            {
                NewArea = p.DetermineMostCompatibleArea(CurrentArea, gameMap, graphInputs);
                if (NewArea != null)
                {
                    if (CurrentArea != null)
                    {
                        CommonParentArea = CurrentArea.DetermineCommonParentArea(NewArea);
                    }
                    InventorySinkRoom = NewArea.InventorySinkRoomObject;
                }
            }
            InventoryProcessInputType = p.ItemsToProcessType;
            TargetRoom = p.TargetRoomObject;
        }
    }
}
