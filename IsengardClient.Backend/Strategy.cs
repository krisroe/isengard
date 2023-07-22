using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace IsengardClient.Backend
{
    public class Strategy
    {
        public const string CAST_VIGOR_SPELL = "cast vigor";
        public const string CAST_MENDWOUNDS_SPELL = "cast mend-wounds";
        public const string CAST_CUREPOISON_SPELL = "cast cure-poison";

        public int ID { get; set; }
        public bool IsValid { get; set; }
        public string DisplayName { get; set; }
        public AfterKillMonsterAction? AfterKillMonsterAction { get; set; }
        public int ManaPool { get; set; }

        public List<MagicStrategyStep> MagicSteps { get; set; }
        public FinalStepAction FinalMagicAction { get; set; }
        public int AutoSpellLevelMin { get; set; }
        public int AutoSpellLevelMax { get; set; }
        public RealmTypeFlags? Realms { get; set; }
        public int? MagicOnlyWhenStunnedForXMS { get; set; }
        public int MagicLastCommandsToRunIndefinitely { get; set; }

        public List<MeleeStrategyStep> MeleeSteps { get; set; }
        public FinalStepAction FinalMeleeAction { get; set; }
        public int? MeleeOnlyWhenStunnedForXMS { get; set; }
        public int MeleeLastCommandsToRunIndefinitely { get; set; }

        public List<PotionsStrategyStep> PotionsSteps { get; set; }
        public FinalStepAction FinalPotionsAction { get; set; }
        public int? PotionsOnlyWhenStunnedForXMS { get; set; }
        public int PotionsLastCommandsToRunIndefinitely { get; set; }

        public CommandType TypesWithStepsEnabled { get; set; }

        public Strategy()
        {
            TypesWithStepsEnabled = CommandType.Magic | CommandType.Melee | CommandType.Potions;
            AutoSpellLevelMax = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            AutoSpellLevelMin = IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET;
            Realms = null;
        }

        public Strategy(Strategy copied)
        {
            CopyFromStrategy(copied, false);
        }

        private void CopyFromStrategy(Strategy copied, bool forInheritance)
        {
            DisplayName = copied.DisplayName;
            if (!forInheritance || copied.AfterKillMonsterAction.HasValue)
            {
                AfterKillMonsterAction = copied.AfterKillMonsterAction;
            }
            ManaPool = copied.ManaPool;
            FinalMagicAction = copied.FinalMagicAction;
            if (!forInheritance || copied.AutoSpellLevelMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                AutoSpellLevelMin = copied.AutoSpellLevelMin;
                AutoSpellLevelMax = copied.AutoSpellLevelMax;
            }
            if (!forInheritance || copied.Realms.HasValue)
            {
                Realms = copied.Realms;
            }
            MagicOnlyWhenStunnedForXMS = copied.MagicOnlyWhenStunnedForXMS;
            MagicLastCommandsToRunIndefinitely = copied.MagicLastCommandsToRunIndefinitely;
            if (copied.MagicSteps != null)
            {
                MagicSteps = new List<MagicStrategyStep>(copied.MagicSteps);
            }

            FinalMeleeAction = copied.FinalMeleeAction;
            MeleeOnlyWhenStunnedForXMS = copied.MeleeOnlyWhenStunnedForXMS;
            MeleeLastCommandsToRunIndefinitely = copied.MeleeLastCommandsToRunIndefinitely;
            if (copied.MeleeSteps != null)
            {
                MeleeSteps = new List<MeleeStrategyStep>(copied.MeleeSteps);
            }

            FinalPotionsAction = copied.FinalPotionsAction;
            PotionsOnlyWhenStunnedForXMS = copied.PotionsOnlyWhenStunnedForXMS;
            PotionsLastCommandsToRunIndefinitely = copied.PotionsLastCommandsToRunIndefinitely;
            if (copied.PotionsSteps != null)
            {
                PotionsSteps = new List<PotionsStrategyStep>(copied.PotionsSteps);
            }

            TypesWithStepsEnabled = copied.TypesWithStepsEnabled;
        }

        public Strategy(Strategy baseStrategy, DynamicMobData dmd) : this(baseStrategy ?? dmd.Strategy)
        {
            if (baseStrategy != null && dmd.Strategy != null)
            {
                CopyFromStrategy(dmd.Strategy, true);
            }
            ApplyStrategyOverrides(dmd.StrategyOverrides);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(DisplayName) ? GetToStringForCommandTypes(TypesWithStepsEnabled) : DisplayName;
        }

        public void ApplyStrategyOverrides(StrategyOverrides overrides)
        {
            if (overrides.AutoSpellLevelMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && overrides.AutoSpellLevelMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
            {
                AutoSpellLevelMin = overrides.AutoSpellLevelMin;
                AutoSpellLevelMax = overrides.AutoSpellLevelMax;
            }
            if (overrides.Realms.HasValue)
            {
                Realms = overrides.Realms;
            }
            if (overrides.AfterKillMonsterAction.HasValue)
            {
                AfterKillMonsterAction = overrides.AfterKillMonsterAction.Value;
            }
            if (overrides.UseMagicCombat.HasValue)
            {
                if (overrides.UseMagicCombat.Value)
                    TypesWithStepsEnabled |= CommandType.Magic;
                else
                    TypesWithStepsEnabled &= ~CommandType.Magic;
            }
            if (overrides.UseMeleeCombat.HasValue)
            {
                if (overrides.UseMeleeCombat.Value)
                    TypesWithStepsEnabled |= CommandType.Melee;
                else
                    TypesWithStepsEnabled &= ~CommandType.Melee;
            }
            if (overrides.UsePotionsCombat.HasValue)
            {
                if (overrides.UsePotionsCombat.Value)
                    TypesWithStepsEnabled |= CommandType.Potions;
                else
                    TypesWithStepsEnabled &= ~CommandType.Potions;
            }
        }

        public string GetToStringForCommandTypes(CommandType Types)
        {
            StringBuilder sb;
            List<string> parts = new List<string>();
            bool supportsSteps = (Types & CommandType.Magic) != CommandType.None;
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
                if (MagicLastCommandsToRunIndefinitely == 1)
                {
                    sb.Append("*");
                }
                else if (MagicLastCommandsToRunIndefinitely > 1)
                {
                    sb.Insert(sb.Length - MagicLastCommandsToRunIndefinitely, "(");
                    sb.Append(")*");
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
            supportsSteps = (Types & CommandType.Melee) != CommandType.None;
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
                if (MeleeLastCommandsToRunIndefinitely == 1)
                {
                    sb.Append("*");
                }
                else if (MeleeLastCommandsToRunIndefinitely > 1)
                {
                    sb.Insert(sb.Length - MeleeLastCommandsToRunIndefinitely, "(");
                    sb.Append(")*");
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
            supportsSteps = (Types & CommandType.Potions) != CommandType.None;
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
                if (PotionsLastCommandsToRunIndefinitely == 1)
                {
                    sb.Append("*");
                }
                else if (PotionsLastCommandsToRunIndefinitely > 1)
                {
                    sb.Insert(sb.Length - PotionsLastCommandsToRunIndefinitely, "(");
                    sb.Append(")*");
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
            string ret;
            if (parts.Count == 0)
                ret = "No Steps";
            else
                ret = string.Join("+", parts.ToArray());
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
        /// <param name="CombatTypesToConsiderEnabled">which combat types to consider enabled</param>
        /// <returns>true if a combat strategy, false otherwise</returns>
        public bool IsCombatStrategy(CommandType types, CommandType CombatTypesToConsiderEnabled)
        {
            bool ret = false;
            bool checkMagic = (types & CommandType.Magic) != CommandType.None;
            bool checkMelee = (types & CommandType.Melee) != CommandType.None;
            bool checkPotions = (types & CommandType.Potions) != CommandType.None;
            bool magicEnabled = (CombatTypesToConsiderEnabled & CommandType.Magic) != CommandType.None;
            bool meleeEnabled = (CombatTypesToConsiderEnabled & CommandType.Melee) != CommandType.None;
            bool potionsEnabled = (CombatTypesToConsiderEnabled & CommandType.Potions) != CommandType.None;
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

        public bool HasAnyMagicSteps(MagicStrategyStep? stepType)
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Magic) == CommandType.None)
                ret = false;
            else
                ret = MagicSteps != null && (!stepType.HasValue || MagicSteps.Contains(stepType.Value));
            return ret;
        }

        public bool HasAnyMeleeSteps(MeleeStrategyStep? stepType)
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Melee) == CommandType.None)
                ret = false;
            else
                ret = MeleeSteps != null && (!stepType.HasValue || MeleeSteps.Contains(stepType.Value));
            return ret;
        }

        public bool HasAnyPotionsSteps(PotionsStrategyStep? stepType)
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Potions) == CommandType.None)
                ret = false;
            else
                ret = PotionsSteps != null && (!stepType.HasValue || PotionsSteps.Contains(stepType.Value));
            return ret;
        }

        public IEnumerable<MagicStrategyStep> GetMagicSteps()
        {
            if ((TypesWithStepsEnabled & CommandType.Magic) != CommandType.None && MagicSteps != null)
            {
                foreach (var nextStep in MagicSteps)
                {
                    yield return nextStep;
                }
                if (MagicLastCommandsToRunIndefinitely > 0)
                {
                    int iCount = MagicSteps.Count;
                    while (true)
                    {
                        for (int i = iCount - MagicLastCommandsToRunIndefinitely; i < iCount; i++)
                        {
                            yield return MagicSteps[i];
                        }
                    }
                }
            }
        }

        public IEnumerable<MeleeStrategyStep> GetMeleeSteps(bool powerAttack)
        {
            if ((TypesWithStepsEnabled & CommandType.Melee) != CommandType.None && MeleeSteps != null)
            {
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
                    yield return nextStepActual; //could be power attack or regular attack
                }
                if (MeleeLastCommandsToRunIndefinitely > 0)
                {
                    int iCount = MeleeSteps.Count;
                    while (true)
                    {
                        for (int i = iCount - MeleeLastCommandsToRunIndefinitely; i < iCount; i++)
                        {
                            yield return MeleeSteps[i];
                        }
                    }
                }
            }
        }

        public IEnumerable<PotionsStrategyStep> GetPotionsSteps()
        {
            if ((TypesWithStepsEnabled & CommandType.Potions) != CommandType.None && PotionsSteps != null)
            {
                foreach (var nextStep in PotionsSteps)
                {
                    yield return nextStep;
                }
                if (PotionsLastCommandsToRunIndefinitely > 0)
                {
                    int iCount = PotionsSteps.Count;
                    while (true)
                    {
                        for (int i = iCount - PotionsLastCommandsToRunIndefinitely; i < iCount; i++)
                        {
                            yield return PotionsSteps[i];
                        }
                    }
                }
            }
        }

        public static bool GetMagicStrategyStepIsCombat(MagicStrategyStep step)
        {
            return GetStrategyStepIsCombatFromAttribute(typeof(MagicStrategyStep), step.ToString());
        }

        public static bool GetMeleeStrategyStepIsCombat(MeleeStrategyStep step)
        {
            return GetStrategyStepIsCombatFromAttribute(typeof(MeleeStrategyStep), step.ToString());
        }

        public static bool GetPotionsStrategyStepIsCombat(PotionsStrategyStep step)
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

            //no combat
            s = new Strategy();
            s.FinalMeleeAction = FinalStepAction.FinishCombat;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.FinalPotionsAction = FinalStepAction.FinishCombat;
            s.AfterKillMonsterAction = Backend.AfterKillMonsterAction.StopCombat;
            s.TypesWithStepsEnabled = CommandType.None;
            yield return s;

            //attack with melee, heal with magic/potions
            s = new Strategy();
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.MagicSteps = new List<MagicStrategyStep>() { MagicStrategyStep.GenericHeal };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.MagicLastCommandsToRunIndefinitely = 1;
            s.MeleeLastCommandsToRunIndefinitely = 1;
            s.PotionsLastCommandsToRunIndefinitely = 1;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;

            //attack with melee/magic, heal with potions
            s = new Strategy();
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.MagicSteps = new List<MagicStrategyStep>() { MagicStrategyStep.OffensiveSpellAuto };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.AfterKillMonsterAction = Backend.AfterKillMonsterAction.StopCombat;
            s.MagicLastCommandsToRunIndefinitely = 1;
            s.MeleeLastCommandsToRunIndefinitely = 1;
            s.PotionsLastCommandsToRunIndefinitely = 1;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;

            //attack with melee, stun+attack with magic, heal with potions
            s = new Strategy();
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<MagicStrategyStep>()
            {
                MagicStrategyStep.Stun,
                MagicStrategyStep.OffensiveSpellAuto,
            };
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.MeleeOnlyWhenStunnedForXMS = stunWaitMS;
            s.AfterKillMonsterAction = Backend.AfterKillMonsterAction.StopCombat;
            s.MagicLastCommandsToRunIndefinitely = 1;
            s.MeleeLastCommandsToRunIndefinitely = 1;
            s.PotionsLastCommandsToRunIndefinitely = 1;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;

            //attack with melee, (stunwand+cast)* with magic, heal with potions
            s = new Strategy();
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<MagicStrategyStep>()
            {
                MagicStrategyStep.StunWand,
                MagicStrategyStep.OffensiveSpellAuto,
            };
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.RegularAttack };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.GenericHeal };
            s.MeleeOnlyWhenStunnedForXMS = stunWaitMS;
            s.AfterKillMonsterAction = Backend.AfterKillMonsterAction.StopCombat;
            s.MagicLastCommandsToRunIndefinitely = 2;
            s.MeleeLastCommandsToRunIndefinitely = 1;
            s.PotionsLastCommandsToRunIndefinitely = 1;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            yield return s;
        }
    }
}
