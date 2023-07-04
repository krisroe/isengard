using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace IsengardClient
{
    internal class Strategy
    {
        public const string CAST_VIGOR_SPELL = "cast vigor";
        public const string CAST_MENDWOUNDS_SPELL = "cast mend-wounds";
        public const string CAST_CUREPOISON_SPELL = "cast cure-poison";

        public string Name { get; set; }
        public bool AutogenerateName { get; set; }
        public AfterKillMonsterAction AfterKillMonsterAction { get; set; }
        public int ManaPool { get; set; }

        public List<MagicStrategyStep> MagicSteps { get; set; }
        public FinalStepAction FinalMagicAction { get; set; }
        public int AutoSpellLevelMin { get; set; }
        public int AutoSpellLevelMax { get; set; }
        public int? MagicOnlyWhenStunnedForXMS { get; set; }

        public List<MeleeStrategyStep> MeleeSteps { get; set; }
        public FinalStepAction FinalMeleeAction { get; set; }
        public int? MeleeOnlyWhenStunnedForXMS { get; set; }

        public List<PotionsStrategyStep> PotionsSteps { get; set; }
        public FinalStepAction FinalPotionsAction { get; set; }
        public int? PotionsOnlyWhenStunnedForXMS { get; set; }

        public CommandType TypesToRunLastCommandIndefinitely { get; set; }
        public CommandType TypesWithStepsEnabled { get; set; }

        public Strategy()
        {
            this.AutogenerateName = true;
            this.TypesWithStepsEnabled = CommandType.Magic | CommandType.Melee | CommandType.Potions;
            this.AutoSpellLevelMax = -1;
            this.AutoSpellLevelMin = -1;
        }

        public Strategy(string Name)
        {
            this.Name = Name;
            this.AutogenerateName = false;
            this.TypesWithStepsEnabled = CommandType.Magic | CommandType.Melee | CommandType.Potions;
        }

        public Strategy(Strategy copied)
        {
            this.Name = copied.Name;
            this.AutogenerateName = copied.AutogenerateName;
            this.AfterKillMonsterAction = copied.AfterKillMonsterAction;
            this.ManaPool = copied.ManaPool;
            this.FinalMagicAction = copied.FinalMagicAction;
            this.AutoSpellLevelMin = copied.AutoSpellLevelMin;
            this.AutoSpellLevelMax = copied.AutoSpellLevelMax;
            this.MagicOnlyWhenStunnedForXMS = copied.MagicOnlyWhenStunnedForXMS;
            if (copied.MagicSteps != null)
            {
                this.MagicSteps = new List<MagicStrategyStep>(copied.MagicSteps);
            }

            this.FinalMeleeAction = copied.FinalMeleeAction;
            this.MeleeOnlyWhenStunnedForXMS = copied.MeleeOnlyWhenStunnedForXMS;
            if (copied.MeleeSteps != null)
            {
                this.MeleeSteps = new List<MeleeStrategyStep>(copied.MeleeSteps);
            }

            this.FinalPotionsAction = copied.FinalPotionsAction;
            this.PotionsOnlyWhenStunnedForXMS = copied.PotionsOnlyWhenStunnedForXMS;
            if (copied.PotionsSteps != null)
            {
                this.PotionsSteps = new List<PotionsStrategyStep>(copied.PotionsSteps);
            }

            this.TypesToRunLastCommandIndefinitely = copied.TypesToRunLastCommandIndefinitely;
            this.TypesWithStepsEnabled = copied.TypesWithStepsEnabled;
        }

        public override string ToString()
        {
            string ret;
            if (AutogenerateName)
            {
                StringBuilder sb;
                List<string> parts = new List<string>();
                bool supportsSteps = (TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
                bool hasSteps = MagicSteps != null;
                if (supportsSteps && hasSteps)
                {
                    sb = new StringBuilder();
                    if (hasSteps)
                    {
                        foreach (var next in MagicSteps)
                        {
                            sb.Append(GetMagicStrategyStepCharacter(next));
                        }
                    }
                    if ((TypesToRunLastCommandIndefinitely & CommandType.Magic) != CommandType.None)
                    {
                        sb.Append("*");
                    }
                    if (FinalMagicAction == FinalStepAction.Flee)
                    {
                        sb.Append("F");
                    }
                    else if (FinalMagicAction == FinalStepAction.Hazy)
                    {
                        sb.Append("w");
                    }
                    else if (FinalMagicAction == FinalStepAction.FinishCombat)
                    {
                        sb.Append("X");
                    }
                    parts.Add(sb.ToString());
                }
                supportsSteps = (TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
                hasSteps = MeleeSteps != null;
                if (supportsSteps && hasSteps)
                {
                    sb = new StringBuilder();
                    if (hasSteps)
                    {
                        foreach (var next in MeleeSteps)
                        {
                            sb.Append(GetMeleeStrategyStepCharacter(next));
                        }
                    }
                    if ((TypesToRunLastCommandIndefinitely & CommandType.Melee) != CommandType.None)
                    {
                        sb.Append("*");
                    }
                    if (FinalMeleeAction == FinalStepAction.Flee)
                    {
                        sb.Append("F");
                    }
                    else if (FinalMeleeAction == FinalStepAction.Hazy)
                    {
                        sb.Append("w");
                    }
                    else if (FinalMeleeAction == FinalStepAction.FinishCombat)
                    {
                        sb.Append("X");
                    }
                    parts.Add(sb.ToString());
                }
                supportsSteps = (TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
                hasSteps = PotionsSteps != null;
                if (supportsSteps && hasSteps)
                {
                    sb = new StringBuilder();
                    if (hasSteps)
                    {
                        foreach (var next in PotionsSteps)
                        {
                            sb.Append(GetPotionsStrategyStepCharacter(next));
                        }
                    }
                    if ((TypesToRunLastCommandIndefinitely & CommandType.Potions) != CommandType.None)
                    {
                        sb.Append("*");
                    }
                    if (FinalPotionsAction == FinalStepAction.Flee)
                    {
                        sb.Append("F");
                    }
                    else if (FinalPotionsAction == FinalStepAction.Hazy)
                    {
                        sb.Append("w");
                    }
                    else if (FinalPotionsAction == FinalStepAction.FinishCombat)
                    {
                        sb.Append("X");
                    }
                    parts.Add(sb.ToString());
                }
                if (parts.Count == 0)
                    ret = "No Steps";
                else
                    ret = string.Join("+", parts.ToArray());
            }
            else
            {
                ret = Name ?? string.Empty;
            }
            return ret;
        }

        public CommandType CombatCommandTypes
        {
            get
            {
                CommandType types = CommandType.None;
                if (((TypesWithStepsEnabled & CommandType.Magic) != CommandType.None) && MagicSteps != null)
                    types |= CommandType.Magic;
                if (((TypesWithStepsEnabled & CommandType.Melee) != CommandType.None) && MeleeSteps != null)
                    types |= CommandType.Melee;
                if (((TypesWithStepsEnabled & CommandType.Potions) != CommandType.None) && PotionsSteps != null)
                    types |= CommandType.Potions;
                return types;
            }
        }

        /// <summary>
        /// whether the strategy involves combat
        /// </summary>
        /// <param name="types">which command types to check</param>
        /// <returns>true if a combat strategy, false otherwise</returns>
        public bool IsCombatStrategy(CommandType types)
        {
            bool ret = false;
            bool checkMagic = (types & CommandType.Magic) != CommandType.None;
            bool checkMelee = (types & CommandType.Melee) != CommandType.None;
            bool checkPotions = (types & CommandType.Potions) != CommandType.None;
            bool magicEnabled = (TypesWithStepsEnabled & CommandType.Magic) != CommandType.None;
            bool meleeEnabled = (TypesWithStepsEnabled & CommandType.Melee) != CommandType.None;
            bool potionsEnabled = (TypesWithStepsEnabled & CommandType.Potions) != CommandType.None;
            if (MagicSteps != null && magicEnabled && checkMagic)
            {
                foreach (var nextStep in MagicSteps)
                {
                    if (GetMagicStrategyStepIsCombat(nextStep))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            if (!ret && MeleeSteps != null && meleeEnabled && checkMelee)
            {
                foreach (var nextStep in MeleeSteps)
                {
                    if (GetMeleeStrategyStepIsCombat(nextStep))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            if (!ret && PotionsSteps != null && potionsEnabled && checkPotions)
            {
                foreach (var nextStep in PotionsSteps)
                {
                    if (GetPotionsStrategyStepIsCombat(nextStep))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        public bool HasAnyMagicSteps()
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Magic) == CommandType.None)
                ret = false;
            else
                ret = MagicSteps != null;
            return ret;
        }

        public bool HasAnyMeleeSteps()
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Melee) == CommandType.None)
                ret = false;
            else
                ret = MeleeSteps != null;
            return ret;
        }

        public bool HasAnyPotionsSteps()
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Potions) == CommandType.None)
                ret = false;
            else
                ret = PotionsSteps != null;
            return ret;
        }

        public IEnumerable<MagicStrategyStep> GetMagicSteps()
        {
            if ((TypesWithStepsEnabled & CommandType.Magic) != CommandType.None && MagicSteps != null)
            {
                MagicStrategyStep eLastStepValue = MagicStrategyStep.GenericHeal;
                foreach (var nextStep in MagicSteps)
                {
                    eLastStepValue = nextStep;
                    yield return nextStep;
                }
                if ((TypesToRunLastCommandIndefinitely & CommandType.Magic) != CommandType.None)
                {
                    while (true)
                    {
                        yield return eLastStepValue;
                    }
                }
            }
        }

        public IEnumerable<MeleeStrategyStep> GetMeleeSteps(bool powerAttack)
        {
            if ((TypesWithStepsEnabled & CommandType.Melee) != CommandType.None && MeleeSteps != null)
            {
                MeleeStrategyStep eLastStepValue = MeleeStrategyStep.RegularAttack;
                foreach (var nextStep in MeleeSteps)
                {
                    MeleeStrategyStep nextStepActual;
                    if (nextStep == MeleeStrategyStep.RegularAttack && powerAttack)
                    {
                        powerAttack = false;
                        nextStepActual = MeleeStrategyStep.PowerAttack;
                    }
                    else
                    {
                        nextStepActual = nextStep;
                    }
                    eLastStepValue = nextStep; //never power attack
                    yield return nextStepActual; //could be power attack or regular attack
                }
                if ((TypesToRunLastCommandIndefinitely & CommandType.Melee) != CommandType.None)
                {
                    while (true)
                    {
                        yield return eLastStepValue;
                    }
                }
            }
        }

        public IEnumerable<PotionsStrategyStep> GetPotionsSteps()
        {
            if ((TypesWithStepsEnabled & CommandType.Potions) != CommandType.None && PotionsSteps != null)
            {
                PotionsStrategyStep eLastStepValue = PotionsStrategyStep.GenericHeal;
                foreach (var nextStep in PotionsSteps)
                {
                    eLastStepValue = nextStep;
                    yield return nextStep;
                }
                if ((TypesToRunLastCommandIndefinitely & CommandType.Potions) != CommandType.None)
                {
                    while (true)
                    {
                        yield return eLastStepValue;
                    }
                }
            }
        }

        private static bool GetMagicStrategyStepIsCombat(MagicStrategyStep step)
        {
            return GetStrategyStepIsCombatFromAttribute(typeof(MagicStrategyStep), step.ToString());
        }

        private static bool GetMeleeStrategyStepIsCombat(MeleeStrategyStep step)
        {
            return GetStrategyStepIsCombatFromAttribute(typeof(MeleeStrategyStep), step.ToString());
        }

        private static bool GetPotionsStrategyStepIsCombat(PotionsStrategyStep step)
        {
            return GetStrategyStepIsCombatFromAttribute(typeof(PotionsStrategyStep), step.ToString());
        }

        private static char GetMagicStrategyStepCharacter(MagicStrategyStep step)
        {
            return GetStrategyStepLetterFromAttribute(typeof(MagicStrategyStep), step.ToString());
        }

        private static char GetMeleeStrategyStepCharacter(MeleeStrategyStep step)
        {
            return GetStrategyStepLetterFromAttribute(typeof(MeleeStrategyStep), step.ToString());
        }

        private static char GetPotionsStrategyStepCharacter(PotionsStrategyStep step)
        {
            return GetStrategyStepLetterFromAttribute(typeof(PotionsStrategyStep), step.ToString());
        }

        private static bool GetStrategyStepIsCombatFromAttribute(Type t, string step)
        {
            var memberInfos = t.GetMember(step);
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);
            object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(StrategyStepAttribute), false);
            return ((StrategyStepAttribute)valueAttributes[0]).IsCombat;
        }

        private static char GetStrategyStepLetterFromAttribute(Type t, string step)
        {
            var memberInfos = t.GetMember(step);
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);
            object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(StrategyStepAttribute), false);
            return ((StrategyStepAttribute)valueAttributes[0]).Letter;
        }

        public static IEnumerable<Strategy> GetDefaultStrategies()
        {
            int stunWaitMS = 250;

            Strategy s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMeleeAction = FinalStepAction.FinishCombat;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.FinalPotionsAction = FinalStepAction.FinishCombat;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesWithStepsEnabled = CommandType.None;
            yield return s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.MagicSteps = new List<MagicStrategyStep>() { MagicStrategyStep.GenericHeal };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.MagicSteps = new List<MagicStrategyStep>() { MagicStrategyStep.OffensiveSpellAuto };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<MagicStrategyStep>()
            {
                MagicStrategyStep.Stun,
                MagicStrategyStep.OffensiveSpellAuto
            };
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.MeleeOnlyWhenStunnedForXMS = stunWaitMS;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<MagicStrategyStep>()
            {
                MagicStrategyStep.Stun,
                MagicStrategyStep.OffensiveSpellAuto,
                MagicStrategyStep.OffensiveSpellAuto,
                MagicStrategyStep.Stun,
                MagicStrategyStep.OffensiveSpellAuto
            };
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.MeleeOnlyWhenStunnedForXMS = stunWaitMS;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<MagicStrategyStep>()
            {
                MagicStrategyStep.Stun,
                MagicStrategyStep.OffensiveSpellAuto,
                MagicStrategyStep.OffensiveSpellAuto,
                MagicStrategyStep.Stun,
                MagicStrategyStep.Stun,
                MagicStrategyStep.OffensiveSpellAuto,
                MagicStrategyStep.OffensiveSpellAuto,
            };
            s.FinalMagicAction = FinalStepAction.Flee;
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.MeleeOnlyWhenStunnedForXMS = stunWaitMS;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;
        }

        public static void GetMinMaxOffensiveSpellLevels(Strategy strategy, int currentMinLevel, int currentMaxLevel, List<string> knownSpells, List<string> realmSpells, out int? calculatedMinLevel, out int? calculatedMaxLevel)
        {
            if (strategy == null || strategy.AutoSpellLevelMin <= 0 || strategy.AutoSpellLevelMax <= 0 || strategy.AutoSpellLevelMax < strategy.AutoSpellLevelMin)
            {
                calculatedMinLevel = currentMinLevel;
                calculatedMaxLevel = currentMaxLevel;
            }
            else
            {
                calculatedMinLevel = strategy.AutoSpellLevelMin;
                calculatedMaxLevel = strategy.AutoSpellLevelMax;
            }
            while (true)
            {
                if (knownSpells.Contains(realmSpells[calculatedMaxLevel.Value - 1]))
                {
                    break;
                }
                else
                {
                    calculatedMaxLevel = calculatedMaxLevel.Value - 1;
                    if (calculatedMaxLevel.Value < calculatedMinLevel.Value)
                    {
                        calculatedMaxLevel = null;
                        calculatedMinLevel = null;
                        break;
                    }
                }
            }
        }
    }
}
