﻿using System;
using System.Collections.Generic;
namespace IsengardClient
{
    internal class BackgroundWorkerParameters
    {
        public BackgroundWorkerParameters()
        {
            QueuedCommands = new List<string>();
        }
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
        public MeleeStrategyStep? QueuedMeleeStep { get; set; }
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
        public bool HasCombat()
        {
            return !string.IsNullOrEmpty(MobText) || MobType.HasValue;
        }
        public bool Foreground { get; set; }
        /// <summary>
        /// whether to cure poison if needed. This is used by the standalone cure-poison option.
        /// </summary>
        public bool CureIfPoisoned { get; set; }
        public BackgroundCommandType? SingleCommandType { get; set; }
        public CommandResult SingleCommandResult { get; set; }
        public bool SaveSettingsOnQuit { get; set; }
        public bool LogoutOnQuit { get; set; }
        public bool MonsterKilled { get; set; }
        public MobTypeEnum? MonsterKilledType { get; set; }
        public bool AtDestination { get; set; }
        public bool UsedPawnShoppe { get; set; }
        public bool Success { get; set; }
        /// <summary>
        /// when the perm run started
        /// </summary>
        public DateTime PermRunStart { get; set; }
        /// <summary>
        /// experience before starting the perm run
        /// </summary>
        public int BeforeExperience { get; set; }
        /// <summary>
        /// gold before starting the perm run
        /// </summary>
        public int BeforeGold { get; set; }
        /// <summary>
        /// perm run info
        /// </summary>
        public PermRun PermRun { get; set; }
        /// <summary>
        /// items to process in the inventory management flow
        /// </summary>
        public ItemsToProcessType InventoryProcessInputType { get; set; }
        public bool IsForPermRun()
        {
            return this.TargetRoom != null && (this.MobType.HasValue || !string.IsNullOrEmpty(this.MobText));
        }
        public void SetPermRun(PermRun p, IsengardMap gameMap)
        {
            PermRun = p;
            Strategy = p.Strategy;
            if (p.MobType.HasValue)
            {
                MobType = p.MobType;
                MobTypeCounter = p.MobIndex;
            }
            else
            {
                MobText = p.MobText;
                MobTextCounter = p.MobIndex;
            }
            if (p.TickRoom.HasValue)
            {
                InventorySinkRoom = gameMap.HealingRooms[p.TickRoom.Value];
            }
            InventoryProcessInputType = p.ItemsToProcessType;
            TargetRoom = p.TargetRoomObject;
        }
    }
}
