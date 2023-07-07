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
            if (copied.Strategy != null)
            {
                this.Strategy = new Strategy(copied.Strategy);
            }
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
        public MobTypeEnum? MobType { get; set; }
        public string MobText { get; set; }
        public int MobIndex { get; set; }
        public Strategy Strategy { get; set; }
        public bool? UseMagicCombat { get; set; }
        public bool? UseMeleeCombat { get; set; }
        public bool? UsePotionsCombat { get; set; }
        public AfterKillMonsterAction? AfterKillMonsterAction { get; set; }
        public int? AutoSpellLevelMin { get; set; }
        public int? AutoSpellLevelMax { get; set; }
        public ItemsToProcessType ItemsToProcessType { get; set; }
    }
}
