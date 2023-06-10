using System;
using System.Collections.Generic;
using System.Text;

namespace IsengardClient
{
    public class Strategy
    {
        public const string CAST_VIGOR_SPELL = "cast vigor";
        public const string CAST_MENDWOUNDS_SPELL = "cast mend-wounds";

        public Strategy()
        {
            this.AutogenerateName = true;
        }

        public Strategy(string Name)
        {
            this.Name = Name;
            this.AutogenerateName = false;
        }

        public Strategy(Strategy copied)
        {
            this.Name = copied.Name;
            this.AutogenerateName = copied.AutogenerateName;
            this.StopWhenKillMonster = copied.StopWhenKillMonster;
            this.FleeHPThreshold = copied.FleeHPThreshold;

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
        }

        public override string ToString()
        {
            string ret;
            if (AutogenerateName)
            {
                StringBuilder sb;
                List<string> parts = new List<string>();
                bool hasSteps = MagicSteps != null;
                bool hasLastStep = LastMagicStep.HasValue;
                if (hasSteps || hasLastStep)
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
                hasLastStep = LastMeleeStep.HasValue;
                hasSteps = MeleeSteps != null;
                if (hasSteps || hasLastStep)
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
                hasLastStep = LastPotionsStep.HasValue;
                hasSteps = PotionsSteps != null;
                if (hasSteps || hasLastStep)
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

        public string Name { get; set; }
        public bool AutogenerateName { get; set; }
        public bool StopWhenKillMonster { get; set; }
        public int FleeHPThreshold { get; set; }
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

        public CommandType CombatCommandTypes
        {
            get
            {
                CommandType types = CommandType.None;
                if (MagicSteps != null || LastMagicStep.HasValue)
                    types |= CommandType.Magic;
                if (MeleeSteps != null || LastMeleeStep.HasValue)
                    types |= CommandType.Melee;
                if (PotionsSteps != null || LastPotionsStep.HasValue)
                    types |= CommandType.Potions;
                return types;
            }
        }

        public bool HasAnyMagicSteps()
        {
            bool ret;
            if (LastMagicStep.HasValue)
                ret = true;
            else
                ret = GetMagicSteps().GetEnumerator().MoveNext();
            return ret;
        }

        public bool HasAnyMeleeSteps()
        {
            bool ret;
            if (LastMeleeStep.HasValue)
                ret = true;
            else
                ret = GetMeleeSteps(false).GetEnumerator().MoveNext();
            return ret;
        }

        public bool HasAnyPotionsSteps()
        {
            bool ret;
            if (LastPotionsStep.HasValue)
                ret = true;
            else
                ret = GetPotionsSteps().GetEnumerator().MoveNext();
            return ret;
        }

        public IEnumerable<MagicStrategyStep> GetMagicSteps()
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

        public IEnumerable<MeleeStrategyStep> GetMeleeSteps(bool powerAttack)
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

        public IEnumerable<PotionsStrategyStep> GetPotionsSteps()
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

        public static List<Strategy> GetDefaultStrategies()
        {
            List<Strategy> allStrategies = new List<Strategy>();
            Strategy s;

            s = new Strategy();
            s.AutogenerateName = true;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
            s.LastMeleeStep = MeleeStrategyStep.RegularAttack;
            s.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic;
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
            s.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic;
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
            s.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic;
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
            s.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee;
            allStrategies.Add(s);

            s = new Strategy();
            s.AutogenerateName = true;
            s.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Magic;
            allStrategies.Add(s);

            s = new Strategy();
            s.AutogenerateName = true;
            s.MagicSteps = new List<AMagicStrategyStep>()
            {
                        SingleMagicStrategyStep.MagicStepStun,
            };
            s.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Magic;
            allStrategies.Add(s);

            s = new Strategy();
            s.AutogenerateName = true;
            s.MagicSteps = new List<AMagicStrategyStep>()
            {
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepStun,
            };
            s.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Magic;
            allStrategies.Add(s);

            s = new Strategy();
            s.AutogenerateName = true;
            s.LastMeleeStep = MeleeStrategyStep.RegularAttack;
            s.StopWhenKillMonster = true;
            s.TypesToRunLastCommandIndefinitely = CommandType.Melee;
            allStrategies.Add(s);

            return allStrategies;
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
        public static SingleMagicStrategyStep MagicStepVigor = new SingleMagicStrategyStep(MagicStrategyStep.Vigor, 'V');
        public static SingleMagicStrategyStep MagicStepMend = new SingleMagicStrategyStep(MagicStrategyStep.MendWounds, 'M');
        public static SingleMagicStrategyStep MagicStepGenericHeal = new SingleMagicStrategyStep(MagicStrategyStep.GenericHeal, 'H');

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
                case MagicStrategyStep.Vigor:
                    ret = MagicStepVigor;
                    break;
                case MagicStrategyStep.MendWounds:
                    ret = MagicStepMend;
                    break;
                case MagicStrategyStep.GenericHeal:
                    ret = MagicStepGenericHeal;
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

    public class StrategyInstance
    {
        private Strategy Strategy;
        private int minAutoSpellLevel;
        private int maxAutoSpellLevel;
        private string currentMob;
        private string _realm1spell;
        private string _realm2spell;
        private string _realm3spell;
        public StrategyInstance(Strategy strategy, int systemMinAutoSpellLevel, int systemMaxAutoSpellLevel, string currentlyFightingMob, string realm1spell, string realm2spell, string realm3spell)
        {
            Strategy = strategy;
            if (strategy.AutoSpellLevelMin <= 0 || strategy.AutoSpellLevelMax <= 0 || strategy.AutoSpellLevelMax < strategy.AutoSpellLevelMin)
            {
                minAutoSpellLevel = systemMinAutoSpellLevel;
                maxAutoSpellLevel = systemMaxAutoSpellLevel;
            }
            else
            {
                minAutoSpellLevel = strategy.AutoSpellLevelMin;
                maxAutoSpellLevel = strategy.AutoSpellLevelMax;
            }
            currentMob = currentlyFightingMob;
            _realm1spell = realm1spell;
            _realm2spell = realm2spell;
            _realm3spell = realm3spell;
        }

        public void GetMeleeCommand(MeleeStrategyStep nextMeleeStep, out string command)
        {
            string sAttackType;
            if (nextMeleeStep == MeleeStrategyStep.PowerAttack)
            {
                sAttackType = "power";
            }
            else if (nextMeleeStep == MeleeStrategyStep.RegularAttack)
            {
                sAttackType = "attack";
            }
            else
            {
                throw new InvalidOperationException();
            }
            command = sAttackType + " " + currentMob;
        }

        public MagicCommandChoiceResult GetMagicCommand(MagicStrategyStep nextMagicStep, int currentHP, int totalHP, int currentMP,  out int manaDrain, out BackgroundCommandType? bct, out string command)
        {
            MagicCommandChoiceResult ret = MagicCommandChoiceResult.Cast;
            bool doCast;
            command = null;
            manaDrain = 0;
            bct = null;
            if (nextMagicStep == MagicStrategyStep.Stun)
            {
                command = "cast stun " + currentMob;
                manaDrain = 10;
                bct = BackgroundCommandType.Stun;
            }
            else if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.Vigor || nextMagicStep == MagicStrategyStep.MendWounds)
            {
                if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    if (Strategy.MagicMendOnlyWhenDownXHP > 0)
                        doCast = currentHP + Strategy.MagicMendOnlyWhenDownXHP <= totalHP;
                    else
                        doCast = currentHP < totalHP;
                    if (doCast)
                    {
                        nextMagicStep = MagicStrategyStep.MendWounds;
                    }
                }
                if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    if (Strategy.MagicVigorOnlyWhenDownXHP > 0)
                        doCast = currentHP + Strategy.MagicVigorOnlyWhenDownXHP <= totalHP;
                    else
                        doCast = currentHP < totalHP;
                    if (doCast)
                    {
                        nextMagicStep = MagicStrategyStep.Vigor;
                    }
                }
                if (nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    command = Strategy.CAST_MENDWOUNDS_SPELL;
                    manaDrain = 6;
                    bct = BackgroundCommandType.MendWounds;
                }
                else if (nextMagicStep == MagicStrategyStep.Vigor)
                {
                    command = Strategy.CAST_VIGOR_SPELL;
                    manaDrain = 2;
                    bct = BackgroundCommandType.Vigor;
                }
                else
                {
                    ret = MagicCommandChoiceResult.Skip;
                }
            }
            else
            {
                if (nextMagicStep == MagicStrategyStep.OffensiveSpellAuto)
                {
                    int iMaxOffLevel = maxAutoSpellLevel;
                    int iMinOffLevel = minAutoSpellLevel;
                    if (currentMP >= 10 && iMinOffLevel <= 3 && iMaxOffLevel >= 3)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel3;
                    }
                    else if (currentMP >= 7 && iMinOffLevel <= 2 && iMaxOffLevel >= 2)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel2;
                    }
                    else if (currentMP >= 3 && iMinOffLevel <= 1 && iMaxOffLevel >= 1)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel1;
                    }
                    else //out of mana
                    {
                        ret = MagicCommandChoiceResult.OutOfMana;
                    }
                }
                if (ret == MagicCommandChoiceResult.Cast)
                {
                    if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel3)
                    {
                        command = "cast " + _realm3spell + " " + currentMob;
                        manaDrain = 10;
                        bct = BackgroundCommandType.OffensiveSpell;
                    }
                    else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel2)
                    {
                        command = "cast " + _realm2spell + " " + currentMob;
                        manaDrain = 7;
                        bct = BackgroundCommandType.OffensiveSpell;
                    }
                    else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel1)
                    {
                        command = "cast " + _realm1spell + " " + currentMob;
                        manaDrain = 3;
                        bct = BackgroundCommandType.OffensiveSpell;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            if (manaDrain > 0 && manaDrain > currentMP)
            {
                manaDrain = 0;
                bct = null;
                ret = MagicCommandChoiceResult.OutOfMana;
            }
            return ret;
        }
    }

    public enum FinalStepAction
    {
        None = 0,
        Flee = 2,
        Hazy = 3,
        FinishCombat = 4,
    }

    public enum MagicStrategyStep
    {
        Stun,
        OffensiveSpellAuto,
        OffensiveSpellLevel1,
        OffensiveSpellLevel2,
        OffensiveSpellLevel3,
        Vigor,
        MendWounds,
        GenericHeal,
    }

    public enum MeleeStrategyStep
    {
        RegularAttack,
        PowerAttack,
    }

    public enum PotionsStrategyStep
    {
        Vigor,
        MendWounds,
        GenericHeal,
    }

    public enum MagicCommandChoiceResult
    {
        Cast,
        Skip,
        OutOfMana,
    }
}
