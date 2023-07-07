using System;
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
        public int ManaPool { get; set; }
        /// <summary>
        /// skills to use in this background process
        /// </summary>
        public PromptedSkills UsedSkills { get; set; }
        /// <summary>
        /// spells that can be cast during the workflow
        /// </summary>
        public WorkflowSpells SpellsToCast { get; set; }
        /// <summary>
        /// spells that can be potioned during the workflow
        /// </summary>
        public WorkflowSpells SpellsToPotion { get; set; }
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
        /// whether to get full before starting
        /// </summary>
        public bool FullBeforeStarting { get; set; }
        /// <summary>
        /// whether to get full after finishing
        /// </summary>
        public bool FullAfterFinishing { get; set; }
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
        public PawnShoppe? PawnShop { get; set; }
        public bool UsedPawnShoppe { get; set; }
        public HealingRoom? TickRoom { get; set; }
        public bool Success { get; set; }
        public ItemsToProcessType InventoryProcessInputType { get; set; }
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

        public bool IsForPermRun()
        {
            return this.TargetRoom != null && (this.MobType.HasValue || !string.IsNullOrEmpty(this.MobText));
        }
    }
}
