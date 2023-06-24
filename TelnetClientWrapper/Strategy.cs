using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace IsengardClient
{
    public class Strategy
    {
        public const string CAST_VIGOR_SPELL = "cast vigor";
        public const string CAST_MENDWOUNDS_SPELL = "cast mend-wounds";
        public const string CAST_CUREPOISON_SPELL = "cast cure-poison";

        public string Name { get; set; }
        public bool AutogenerateName { get; set; }
        public AfterKillMonsterAction AfterKillMonsterAction { get; set; }
        public AutoEscapeActivity AutoEscapeActivity { get; set; }
        public AutoEscapeType AutoEscapeType { get; set; }
        public int AutoEscapeThreshold { get; set; }
        public int ManaPool { get; set; }
        public bool PromptForManaPool { get; set; }

        public MagicStrategyStep? LastMagicStep { get; set; }
        public int MagicVigorOnlyWhenDownXHP { get; set; }
        public int MagicMendOnlyWhenDownXHP { get; set; }
        public FinalStepAction FinalMagicAction { get; set; }
        public List<AMagicStrategyStep> MagicSteps { get; set; }
        public int AutoSpellLevelMin { get; set; }
        public int AutoSpellLevelMax { get; set; }

        public MeleeStrategyStep? LastMeleeStep { get; set; }
        public FinalStepAction FinalMeleeAction { get; set; }
        public List<AMeleeStrategyStep> MeleeSteps { get; set; }

        public PotionsStrategyStep? LastPotionsStep { get; set; }
        public int PotionsVigorOnlyWhenDownXHP { get; set; }
        public int PotionsMendOnlyWhenDownXHP { get; set; }
        public FinalStepAction FinalPotionsAction { get; set; }
        public List<APotionsStrategyStep> PotionsSteps { get; set; }

        public CommandType TypesToRunOnlyWhenMonsterStunned { get; set; }
        public CommandType TypesToRunLastCommandIndefinitely { get; set; }
        public CommandType TypesWithStepsEnabled { get; set; }

        public Strategy()
        {
            this.AutogenerateName = true;
            this.TypesWithStepsEnabled = CommandType.Magic | CommandType.Melee | CommandType.Potions;
            this.AutoSpellLevelMax = -1;
            this.AutoSpellLevelMin = -1;
            this.AutoEscapeActivity = AutoEscapeActivity.Inherit;
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
            this.AutoEscapeActivity = copied.AutoEscapeActivity;
            this.AutoEscapeThreshold = copied.AutoEscapeThreshold;
            this.AutoEscapeType = copied.AutoEscapeType;
            this.ManaPool = copied.ManaPool;
            this.PromptForManaPool = copied.PromptForManaPool;
            this.LastMagicStep = copied.LastMagicStep;
            this.MagicVigorOnlyWhenDownXHP = copied.MagicVigorOnlyWhenDownXHP;
            this.MagicMendOnlyWhenDownXHP = copied.MagicMendOnlyWhenDownXHP;
            this.FinalMagicAction = copied.FinalMagicAction;
            this.AutoSpellLevelMin = copied.AutoSpellLevelMin;
            this.AutoSpellLevelMax = copied.AutoSpellLevelMax;

            if (copied.MagicSteps != null)
            {
                this.MagicSteps = new List<AMagicStrategyStep>();
                foreach (var next in copied.MagicSteps)
                {
                    this.MagicSteps.Add(next.Clone());
                }
            }

            this.LastMeleeStep = copied.LastMeleeStep;
            this.FinalMeleeAction = copied.FinalMeleeAction;
            if (copied.MeleeSteps != null)
            {
                this.MeleeSteps = new List<AMeleeStrategyStep>();
                foreach (var next in copied.MeleeSteps)
                {
                    this.MeleeSteps.Add(next.Clone());
                }
            }

            this.LastPotionsStep = copied.LastPotionsStep;
            this.PotionsVigorOnlyWhenDownXHP = copied.PotionsVigorOnlyWhenDownXHP;
            this.PotionsMendOnlyWhenDownXHP = copied.PotionsMendOnlyWhenDownXHP;
            this.FinalPotionsAction = copied.FinalPotionsAction;
            if (copied.PotionsSteps != null)
            {
                this.PotionsSteps = new List<APotionsStrategyStep>();
                foreach (var next in copied.PotionsSteps)
                {
                    this.PotionsSteps.Add(next.Clone());
                }
            }

            this.TypesToRunLastCommandIndefinitely = copied.TypesToRunLastCommandIndefinitely;
            this.TypesToRunOnlyWhenMonsterStunned = copied.TypesToRunOnlyWhenMonsterStunned;
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
                bool hasLastStep = LastMagicStep.HasValue;
                if (supportsSteps && (hasSteps || hasLastStep))
                {
                    sb = new StringBuilder();
                    if (hasSteps)
                    {
                        foreach (var next in MagicSteps)
                        {
                            sb.Append(next.ToString());
                        }
                    }
                    if (hasLastStep)
                    {
                        sb.Append(SingleMagicStrategyStep.GetStrategyStep(LastMagicStep.Value));
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
                hasLastStep = LastMeleeStep.HasValue;
                hasSteps = MeleeSteps != null;
                if (supportsSteps && (hasSteps || hasLastStep))
                {
                    sb = new StringBuilder();
                    if (hasSteps)
                    {
                        foreach (var next in MeleeSteps)
                        {
                            sb.Append(next.ToString());
                        }
                    }
                    if (hasLastStep)
                    {
                        sb.Append(SingleMeleeStrategyStep.GetStrategyStep(LastMeleeStep.Value));
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
                hasLastStep = LastPotionsStep.HasValue;
                hasSteps = PotionsSteps != null;
                if (supportsSteps && (hasSteps || hasLastStep))
                {
                    sb = new StringBuilder();
                    if (hasSteps)
                    {
                        foreach (var next in PotionsSteps)
                        {
                            sb.Append(next.ToString());
                        }
                    }
                    if (hasLastStep)
                    {
                        sb.Append(SinglePotionsStrategyStep.GetStrategyStep(LastPotionsStep.Value));
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
                if (((TypesWithStepsEnabled & CommandType.Magic) != CommandType.None) && (MagicSteps != null || LastMagicStep.HasValue))
                    types |= CommandType.Magic;
                if (((TypesWithStepsEnabled & CommandType.Melee) != CommandType.None) && (MeleeSteps != null || LastMeleeStep.HasValue))
                    types |= CommandType.Melee;
                if (((TypesWithStepsEnabled & CommandType.Potions) != CommandType.None) && (PotionsSteps != null || LastPotionsStep.HasValue))
                    types |= CommandType.Potions;
                return types;
            }
        }

        public bool HasAnyMagicSteps()
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Magic) == CommandType.None)
                ret = false;
            else if (LastMagicStep.HasValue)
                ret = true;
            else
                ret = GetMagicSteps().GetEnumerator().MoveNext();
            return ret;
        }

        public bool HasAnyMeleeSteps()
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Melee) == CommandType.None)
                ret = false;
            else if (LastMeleeStep.HasValue)
                ret = true;
            else
                ret = GetMeleeSteps(false).GetEnumerator().MoveNext();
            return ret;
        }

        public bool HasAnyPotionsSteps()
        {
            bool ret;
            if ((TypesWithStepsEnabled & CommandType.Potions) == CommandType.None)
                ret = false;
            else if (LastPotionsStep.HasValue)
                ret = true;
            else
                ret = GetPotionsSteps().GetEnumerator().MoveNext();
            return ret;
        }

        public IEnumerable<MagicStrategyStep> GetMagicSteps()
        {
            if ((TypesWithStepsEnabled & CommandType.Magic) != CommandType.None)
            {
                bool haveAnySteps = false;
                MagicStrategyStep eLastStepValue = MagicStrategyStep.GenericHeal;
                if (MagicSteps != null)
                {
                    foreach (var nextStep in MagicSteps)
                    {
                        foreach (var nextAction in nextStep.GetSteps())
                        {
                            haveAnySteps = true;
                            eLastStepValue = nextAction;
                            yield return nextAction;
                        }
                    }
                }
                if (LastMagicStep.HasValue)
                {
                    haveAnySteps = true;
                    eLastStepValue = LastMagicStep.Value;
                    yield return eLastStepValue;
                }
                if (haveAnySteps && ((TypesToRunLastCommandIndefinitely & CommandType.Magic) != CommandType.None))
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
            if ((TypesWithStepsEnabled & CommandType.Melee) != CommandType.None)
            {
                bool haveAnySteps = false;
                MeleeStrategyStep eLastStepValue = MeleeStrategyStep.RegularAttack;
                if (MeleeSteps != null)
                {
                    foreach (var nextStep in MeleeSteps)
                    {
                        foreach (var nextAction in nextStep.GetSteps())
                        {
                            MeleeStrategyStep nextStepActual;
                            if (nextAction == MeleeStrategyStep.RegularAttack && powerAttack)
                            {
                                powerAttack = false;
                                nextStepActual = MeleeStrategyStep.PowerAttack;
                            }
                            else
                            {
                                nextStepActual = nextAction;
                            }
                            haveAnySteps = true;
                            eLastStepValue = nextAction; //never power attack
                            yield return nextStepActual; //could be power attack or regular attack
                        }
                    }
                }
                if (LastMeleeStep.HasValue)
                {
                    eLastStepValue = LastMeleeStep.Value;
                    bool switchToPowerAttack = !haveAnySteps && powerAttack && eLastStepValue == MeleeStrategyStep.RegularAttack;
                    if (switchToPowerAttack)
                    {
                        eLastStepValue = MeleeStrategyStep.PowerAttack;
                    }
                    haveAnySteps = true;
                    yield return eLastStepValue;
                    if (switchToPowerAttack)
                    {
                        eLastStepValue = MeleeStrategyStep.RegularAttack;
                    }
                }
                if (haveAnySteps && ((TypesToRunLastCommandIndefinitely & CommandType.Melee) != CommandType.None))
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
            if ((TypesWithStepsEnabled & CommandType.Potions) != CommandType.None)
            {
                bool haveAnySteps = false;
                PotionsStrategyStep eLastStepValue = PotionsStrategyStep.GenericHeal;
                if (PotionsSteps != null)
                {
                    foreach (var nextStep in PotionsSteps)
                    {
                        foreach (var nextAction in nextStep.GetSteps())
                        {
                            haveAnySteps = true;
                            eLastStepValue = nextAction;
                            yield return nextAction;
                        }
                    }
                }
                if (LastPotionsStep.HasValue)
                {
                    haveAnySteps = true;
                    eLastStepValue = LastPotionsStep.Value;
                    yield return eLastStepValue;
                }
                if (haveAnySteps && ((TypesToRunLastCommandIndefinitely & CommandType.Potions) != CommandType.None))
                {
                    while (true)
                    {
                        yield return eLastStepValue;
                    }
                }
            }
        }

        public static List<Strategy> GetDefaultStrategies()
        {
            List<Strategy> allStrategies = new List<Strategy>();
            Strategy s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
            s.LastMeleeStep = MeleeStrategyStep.RegularAttack;
            s.LastPotionsStep = PotionsStrategyStep.GenericHeal;
            s.PotionsMendOnlyWhenDownXHP = 12;
            s.PotionsVigorOnlyWhenDownXHP = 6;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            allStrategies.Add(s);

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<AMagicStrategyStep>()
            {
                SingleMagicStrategyStep.MagicStepStun,
            };
            s.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
            s.LastMeleeStep = MeleeStrategyStep.RegularAttack;
            s.LastPotionsStep = PotionsStrategyStep.GenericHeal;
            s.PotionsMendOnlyWhenDownXHP = 12;
            s.PotionsVigorOnlyWhenDownXHP = 6;
            s.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            allStrategies.Add(s);

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<AMagicStrategyStep>()
            {
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepStun,
            };
            s.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
            s.LastMeleeStep = MeleeStrategyStep.RegularAttack;
            s.LastPotionsStep = PotionsStrategyStep.GenericHeal;
            s.PotionsMendOnlyWhenDownXHP = 12;
            s.PotionsVigorOnlyWhenDownXHP = 6;
            s.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            allStrategies.Add(s);

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.MagicSteps = new List<AMagicStrategyStep>()
            {
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
            };
            s.FinalMagicAction = FinalStepAction.Flee;
            s.LastMeleeStep = MeleeStrategyStep.RegularAttack;
            s.LastPotionsStep = PotionsStrategyStep.GenericHeal;
            s.PotionsMendOnlyWhenDownXHP = 12;
            s.PotionsVigorOnlyWhenDownXHP = 6;
            s.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Potions;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Magic;
            allStrategies.Add(s);

            return allStrategies;
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

    public abstract class AMagicStrategyStep
    {
        public int RepeatCount { get; set; }

        public IEnumerable<MagicStrategyStep> GetSteps()
        {
            for (int i = 0; i < RepeatCount; i++)
            {
                foreach (var nextStep in GetBaseSteps())
                {
                    yield return nextStep;
                }
            }
        }

        public abstract AMagicStrategyStep Clone();

        public abstract IEnumerable<MagicStrategyStep> GetBaseSteps();
    }

    public class SingleMagicStrategyStep : AMagicStrategyStep
    {
        public static SingleMagicStrategyStep MagicStepStun = new SingleMagicStrategyStep(MagicStrategyStep.Stun, 'S');
        public static SingleMagicStrategyStep MagicStepOffensiveSpellAuto = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellAuto, 'C');
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel1 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel1, '1');
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel2 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel2, '2');
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel3 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel3, '3');
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel4 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel4, '4');
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel5 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel5, '5');
        public static SingleMagicStrategyStep MagicStepVigor = new SingleMagicStrategyStep(MagicStrategyStep.Vigor, 'V');
        public static SingleMagicStrategyStep MagicStepMend = new SingleMagicStrategyStep(MagicStrategyStep.MendWounds, 'M');
        public static SingleMagicStrategyStep MagicStepGenericHeal = new SingleMagicStrategyStep(MagicStrategyStep.GenericHeal, 'H');
        public static SingleMagicStrategyStep MagicStepCurePoison = new SingleMagicStrategyStep(MagicStrategyStep.CurePoison, 'P');

        public char Letter { get; set; }

        private SingleMagicStrategyStep(MagicStrategyStep step, char Letter)
        {
            Action = step;
            RepeatCount = 1;
            this.Letter = Letter;
        }

        public override AMagicStrategyStep Clone()
        {
            return this; //singleton object, doesn't need to be cloned
        }

        public override string ToString()
        {
            return this.Letter.ToString();
        }

        public static SingleMagicStrategyStep GetStrategyStep(MagicStrategyStep step)
        {
            SingleMagicStrategyStep ret;
            switch (step)
            {
                case MagicStrategyStep.Stun:
                    ret = MagicStepStun;
                    break;
                case MagicStrategyStep.OffensiveSpellAuto:
                    ret = MagicStepOffensiveSpellAuto;
                    break;
                case MagicStrategyStep.OffensiveSpellLevel1:
                    ret = MagicStepOffensiveSpellLevel1;
                    break;
                case MagicStrategyStep.OffensiveSpellLevel2:
                    ret = MagicStepOffensiveSpellLevel2;
                    break;
                case MagicStrategyStep.OffensiveSpellLevel3:
                    ret = MagicStepOffensiveSpellLevel3;
                    break;
                case MagicStrategyStep.OffensiveSpellLevel4:
                    ret = MagicStepOffensiveSpellLevel4;
                    break;
                case MagicStrategyStep.OffensiveSpellLevel5:
                    ret = MagicStepOffensiveSpellLevel5;
                    break;
                case MagicStrategyStep.Vigor:
                    ret = MagicStepVigor;
                    break;
                case MagicStrategyStep.MendWounds:
                    ret = MagicStepMend;
                    break;
                case MagicStrategyStep.GenericHeal:
                    ret = MagicStepGenericHeal;
                    break;
                case MagicStrategyStep.CurePoison:
                    ret = MagicStepCurePoison;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }

        public MagicStrategyStep Action { get; set; }

        public override IEnumerable<MagicStrategyStep> GetBaseSteps()
        {
            yield return Action;
        }
    }

    public class MultipleMagicStrategyStep : AMagicStrategyStep
    {
        public List<AMagicStrategyStep> SubSteps { get; private set; }

        public MultipleMagicStrategyStep()
        {
            this.SubSteps = new List<AMagicStrategyStep>();
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append("(");
            foreach (var next in this.SubSteps)
            {
                ret.Append(next.ToString());
            }
            ret.Append(")");
            if (RepeatCount > 1)
            {
                ret.Append("*");
                ret.Append(this.RepeatCount.ToString());
            }
            return ret.ToString();
        }

        public override IEnumerable<MagicStrategyStep> GetBaseSteps()
        {
            foreach (var nextStep in SubSteps)
            {
                foreach (var nextAction in nextStep.GetBaseSteps())
                {
                    yield return nextAction;
                }
            }
        }

        public override AMagicStrategyStep Clone()
        {
            MultipleMagicStrategyStep ret = new MultipleMagicStrategyStep();
            foreach (var next in this.SubSteps)
            {
                ret.SubSteps.Add(next.Clone());
            }
            return ret;
        }
    }

    public abstract class AMeleeStrategyStep
    {
        public int RepeatCount { get; set; }

        public IEnumerable<MeleeStrategyStep> GetSteps()
        {
            for (int i = 0; i < RepeatCount; i++)
            {
                foreach (var nextStep in GetBaseSteps())
                {
                    yield return nextStep;
                }
            }
        }

        public abstract IEnumerable<MeleeStrategyStep> GetBaseSteps();

        public abstract AMeleeStrategyStep Clone();
    }

    public class SingleMeleeStrategyStep : AMeleeStrategyStep
    {
        public static SingleMeleeStrategyStep MeleeStepRegularAttack = new SingleMeleeStrategyStep(MeleeStrategyStep.RegularAttack, 'A');
        public static SingleMeleeStrategyStep MeleeStepPowerAttack = new SingleMeleeStrategyStep(MeleeStrategyStep.PowerAttack, 'P');

        public char Letter { get; set; }

        private SingleMeleeStrategyStep(MeleeStrategyStep step, char Letter)
        {
            Action = step;
            RepeatCount = 1;
            this.Letter = Letter;
        }

        public MeleeStrategyStep Action { get; set; }

        public static SingleMeleeStrategyStep GetStrategyStep(MeleeStrategyStep step)
        {
            SingleMeleeStrategyStep ret;
            switch (step)
            {
                case MeleeStrategyStep.RegularAttack:
                    ret = MeleeStepRegularAttack;
                    break;
                case MeleeStrategyStep.PowerAttack:
                    ret = MeleeStepPowerAttack;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }

        public override string ToString()
        {
            return this.Letter.ToString();
        }

        public override IEnumerable<MeleeStrategyStep> GetBaseSteps()
        {
            yield return Action;
        }

        public override AMeleeStrategyStep Clone()
        {
            return this; //singleton object, doesn't need to be cloned
        }
    }

    public class MultipleMeleeStrategyStep : AMeleeStrategyStep
    {
        public List<AMeleeStrategyStep> SubSteps { get; private set; }

        public MultipleMeleeStrategyStep()
        {
            this.SubSteps = new List<AMeleeStrategyStep>();
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append("(");
            foreach (var next in this.SubSteps)
            {
                ret.Append(next.ToString());
            }
            ret.Append(")");
            if (RepeatCount > 1)
            {
                ret.Append("*");
                ret.Append(this.RepeatCount.ToString());
            }
            return ret.ToString();
        }

        public override IEnumerable<MeleeStrategyStep> GetBaseSteps()
        {
            foreach (var nextStep in SubSteps)
            {
                foreach (var nextAction in nextStep.GetBaseSteps())
                {
                    yield return nextAction;
                }
            }
        }

        public override AMeleeStrategyStep Clone()
        {
            MultipleMeleeStrategyStep ret = new MultipleMeleeStrategyStep();
            foreach (var next in this.SubSteps)
            {
                ret.SubSteps.Add(next.Clone());
            }
            return ret;
        }
    }

    public abstract class APotionsStrategyStep
    {
        public int RepeatCount { get; set; }

        public IEnumerable<PotionsStrategyStep> GetSteps()
        {
            for (int i = 0; i < RepeatCount; i++)
            {
                foreach (var nextStep in GetBaseSteps())
                {
                    yield return nextStep;
                }
            }
        }

        public abstract IEnumerable<PotionsStrategyStep> GetBaseSteps();

        public abstract APotionsStrategyStep Clone();
    }

    public class SinglePotionsStrategyStep : APotionsStrategyStep
    {
        public static SinglePotionsStrategyStep PotionsStepVigor = new SinglePotionsStrategyStep(PotionsStrategyStep.Vigor, 'v');
        public static SinglePotionsStrategyStep PotionsStepMendWounds = new SinglePotionsStrategyStep(PotionsStrategyStep.MendWounds, 'm');
        public static SinglePotionsStrategyStep PotionsStepGenericHeal = new SinglePotionsStrategyStep(PotionsStrategyStep.GenericHeal, 'h');
        public static SinglePotionsStrategyStep PotionsStepCurePoison = new SinglePotionsStrategyStep(PotionsStrategyStep.CurePoison, 'p');

        public PotionsStrategyStep Action { get; set; }

        public char Letter { get; set; }

        private SinglePotionsStrategyStep(PotionsStrategyStep step, char Letter)
        {
            Action = step;
            RepeatCount = 1;
            this.Letter = Letter;
        }

        public static SinglePotionsStrategyStep GetStrategyStep(PotionsStrategyStep step)
        {
            SinglePotionsStrategyStep ret;
            switch (step)
            {
                case PotionsStrategyStep.Vigor:
                    ret = PotionsStepVigor;
                    break;
                case PotionsStrategyStep.MendWounds:
                    ret = PotionsStepMendWounds;
                    break;
                case PotionsStrategyStep.GenericHeal:
                    ret = PotionsStepGenericHeal;
                    break;
                case PotionsStrategyStep.CurePoison:
                    ret = PotionsStepCurePoison;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }

        public override string ToString()
        {
            return this.Letter.ToString();
        }

        public override IEnumerable<PotionsStrategyStep> GetBaseSteps()
        {
            yield return Action;
        }

        public override APotionsStrategyStep Clone()
        {
            return this; //singleton object, doesn't need to be cloned
        }
    }

    public class MultiplePotionsStrategyStep : APotionsStrategyStep
    {
        public List<APotionsStrategyStep> SubSteps { get; private set; }

        public MultiplePotionsStrategyStep()
        {
            this.SubSteps = new List<APotionsStrategyStep>();
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append("(");
            foreach (var next in this.SubSteps)
            {
                ret.Append(next.ToString());
            }
            ret.Append(")");
            if (RepeatCount > 1)
            {
                ret.Append("*");
                ret.Append(this.RepeatCount.ToString());
            }
            return ret.ToString();
        }

        public override IEnumerable<PotionsStrategyStep> GetBaseSteps()
        {
            foreach (var nextStep in SubSteps)
            {
                foreach (var nextAction in nextStep.GetBaseSteps())
                {
                    yield return nextAction;
                }
            }
        }

        public override APotionsStrategyStep Clone()
        {
            MultiplePotionsStrategyStep ret = new MultiplePotionsStrategyStep();
            foreach (var next in this.SubSteps)
            {
                ret.SubSteps.Add(next.Clone());
            }
            return ret;
        }
    }
}
