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
            this.FullBeforeStarting = copied.FullBeforeStarting;
            this.FullAfterFinishing = copied.FullAfterFinishing;
            this.SpellsToCast = copied.SpellsToCast;
            this.SpellsToPotion = copied.SpellsToPotion;
            this.SkillsToRun = copied.SkillsToRun;
            this.TargetRoom = copied.TargetRoom;
            this.TargetRoomObject = copied.TargetRoomObject;
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
        public bool FullBeforeStarting { get; set; }
        public bool FullAfterFinishing { get; set; }
        public WorkflowSpells SpellsToCast { get; set; }
        public WorkflowSpells SpellsToPotion { get; set; }
        public PromptedSkills SkillsToRun { get; set; }
        public string TargetRoom { get; set; }
        public Room TargetRoomObject { get; set; }
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
    }
}
