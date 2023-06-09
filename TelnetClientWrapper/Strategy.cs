using System;
using System.Collections.Generic;
namespace IsengardClient
{
    public class Strategy
    {
        public const string CAST_VIGOR_SPELL = "cast vigor";
        public const string CAST_MENDWOUNDS_SPELL = "cast mend-wounds";

        public Strategy(string Name)
        {
            this.Name = Name;
            this.AutogenerateName = false;
        }

        public string Name { get; set; }
        public bool AutogenerateName { get; set; }
        public bool StopWhenKillMonster { get; set; }
        public int FleeHPThreshold { get; set; }
        public bool ShowPreForm { get; set; }
        public int ManaPool { get; set; }
        public bool PromptForManaPool { get; set; }

        public MagicStrategyStep? LastMagicStep { get; set; }
        public int VigorOnlyWhenDownXHP { get; set; }
        public int MendOnlyWhenDownXHP { get; set; }
        public FinalStepAction FinalMagicAction { get; set; }
        public List<AMagicStrategyStep> MagicSteps { get; set; }
        public int AutoSpellLevelMin { get; set; }
        public int AutoSpellLevelMax { get; set; }

        public MeleeStrategyStep? LastMeleeStep { get; set; }
        public FinalStepAction FinalMeleeAction { get; set; }
        public List<AMeleeStrategyStep> MeleeSteps { get; set; }

        public PotionsStrategyStep? LastPotionsStep { get; set; }
        public int YellowOnlyWhenDownXHP { get; set; }
        public int RedOrangeOnlyWhenDownXHP { get; set; }
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

        public static Strategy GenerateCannedStrategy(string Name)
        {
            Strategy m = new Strategy(Name);
            switch (Name)
            {
                case "C*+A*":
                    m.ShowPreForm = true;
                    m.FinalMagicAction = FinalStepAction.FinishCombat;
                    m.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
                    m.LastMeleeStep = MeleeStrategyStep.RegularAttack;
                    m.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic;
                    break;
                case "SC*+A*":
                    m.ShowPreForm = true;
                    m.FinalMagicAction = FinalStepAction.FinishCombat;
                    m.MagicSteps = new List<AMagicStrategyStep>()
                    {
                        SingleMagicStrategyStep.MagicStepStun,
                    };
                    m.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
                    m.LastMeleeStep = MeleeStrategyStep.RegularAttack;
                    m.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic;
                    break;
                case "SCCSC*+A*":
                    m.ShowPreForm = true;
                    m.FinalMagicAction = FinalStepAction.FinishCombat;
                    m.MagicSteps = new List<AMagicStrategyStep>()
                    {
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepStun,
                    };
                    m.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
                    m.LastMeleeStep = MeleeStrategyStep.RegularAttack;
                    m.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Melee | CommandType.Magic;
                    break;
                case "SCCSCCF+A*":
                    m.ShowPreForm = true;
                    m.FinalMagicAction = FinalStepAction.FinishCombat;
                    m.MagicSteps = new List<AMagicStrategyStep>()
                    {
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                    };
                    m.FinalMagicAction = FinalStepAction.Flee;
                    m.LastMeleeStep = MeleeStrategyStep.RegularAttack;
                    m.TypesToRunOnlyWhenMonsterStunned = CommandType.Melee;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Melee;
                    break;
                case "C*":
                    m.ShowPreForm = true;
                    m.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Magic;
                    break;
                case "SC*":
                    m.ShowPreForm = true;
                    m.MagicSteps = new List<AMagicStrategyStep>()
                    {
                        SingleMagicStrategyStep.MagicStepStun,
                    };
                    m.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Magic;
                    break;
                case "SCCSC*":
                    m.ShowPreForm = true;
                    m.MagicSteps = new List<AMagicStrategyStep>()
                    {
                        SingleMagicStrategyStep.MagicStepStun,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepOffensiveSpellAuto,
                        SingleMagicStrategyStep.MagicStepStun,
                    };
                    m.LastMagicStep = MagicStrategyStep.OffensiveSpellAuto;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Magic;
                    break;
                case "A*":
                    m.ShowPreForm = true;
                    m.LastMeleeStep = MeleeStrategyStep.RegularAttack;
                    m.StopWhenKillMonster = true;
                    m.TypesToRunLastCommandIndefinitely = CommandType.Melee;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return m;
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

        public abstract IEnumerable<MagicStrategyStep> GetBaseSteps();
    }

    public class SingleMagicStrategyStep : AMagicStrategyStep
    {
        public static SingleMagicStrategyStep MagicStepStun = new SingleMagicStrategyStep(MagicStrategyStep.Stun);
        public static SingleMagicStrategyStep MagicStepOffensiveSpellAuto = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellAuto);
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel1 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel1);
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel2 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel2);
        public static SingleMagicStrategyStep MagicStepOffensiveSpellLevel3 = new SingleMagicStrategyStep(MagicStrategyStep.OffensiveSpellLevel3);
        public static SingleMagicStrategyStep MagicStepVigor = new SingleMagicStrategyStep(MagicStrategyStep.Vigor);
        public static SingleMagicStrategyStep MagicStepMend = new SingleMagicStrategyStep(MagicStrategyStep.MendWounds);
        public static SingleMagicStrategyStep MagicStepGenericHeal = new SingleMagicStrategyStep(MagicStrategyStep.GenericHeal);

        private SingleMagicStrategyStep(MagicStrategyStep step)
        {
            Action = step;
            RepeatCount = 1;
        }

        public MagicStrategyStep Action { get; set; }

        public override IEnumerable<MagicStrategyStep> GetBaseSteps()
        {
            yield return Action;
        }
    }

    public class MultipleMagicStrategyStep : AMagicStrategyStep
    {
        public List<AMagicStrategyStep> SubSteps { get; set; }

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
    }

    public class SingleMeleeStrategyStep : AMeleeStrategyStep
    {
        public static SingleMeleeStrategyStep MeleeStepRegularAttack = new SingleMeleeStrategyStep(MeleeStrategyStep.RegularAttack);
        public static SingleMeleeStrategyStep MeleeStepPowerAttack = new SingleMeleeStrategyStep(MeleeStrategyStep.PowerAttack);

        private SingleMeleeStrategyStep(MeleeStrategyStep step)
        {
            Action = step;
            RepeatCount = 1;
        }

        public MeleeStrategyStep Action { get; set; }

        public override IEnumerable<MeleeStrategyStep> GetBaseSteps()
        {
            yield return Action;
        }
    }

    public class MultipleMeleeStrategyStep : AMeleeStrategyStep
    {
        public List<AMeleeStrategyStep> SubSteps { get; set; }

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
    }

    public class SinglePotionsStrategyStep : APotionsStrategyStep
    {
        public static SinglePotionsStrategyStep PotionsStepYellow = new SinglePotionsStrategyStep(PotionsStrategyStep.Yellow);
        public static SinglePotionsStrategyStep PotionsStepRedOrange = new SinglePotionsStrategyStep(PotionsStrategyStep.RedOrange);
        public static SinglePotionsStrategyStep PotionsStepGenericHeal = new SinglePotionsStrategyStep(PotionsStrategyStep.GenericHeal);

        public PotionsStrategyStep Action { get; set; }

        private SinglePotionsStrategyStep(PotionsStrategyStep step)
        {
            Action = step;
            RepeatCount = 1;
        }

        public override IEnumerable<PotionsStrategyStep> GetBaseSteps()
        {
            yield return Action;
        }
    }

    public class MultiplePotionsStrategyStep : APotionsStrategyStep
    {
        public List<APotionsStrategyStep> SubSteps { get; set; }

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
                    if (Strategy.MendOnlyWhenDownXHP > 0)
                        doCast = currentHP + Strategy.MendOnlyWhenDownXHP <= totalHP;
                    else
                        doCast = currentHP < totalHP;
                    if (doCast)
                    {
                        nextMagicStep = MagicStrategyStep.MendWounds;
                    }
                }
                if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    if (Strategy.VigorOnlyWhenDownXHP > 0)
                        doCast = currentHP + Strategy.VigorOnlyWhenDownXHP <= totalHP;
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
            return ret;
        }
    }

    public enum FinalStepAction
    {
        None = 0,
        Flee = 2,
        FinishCombat = 3,
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
        Yellow,
        RedOrange,
        GenericHeal,
    }

    public enum MagicCommandChoiceResult
    {
        Cast,
        Skip,
        OutOfMana,
    }
}
