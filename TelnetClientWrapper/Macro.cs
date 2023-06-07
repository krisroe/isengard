using System;
using System.Collections.Generic;
namespace IsengardClient
{
    public class Macro
    {
        public Macro(string Name)
        {
            this.Name = Name;
        }
        public override string ToString()
        {
            return this.Name;
        }

        public string Name { get; set; }
        public CommandType CombatCommandTypes
        {
            get
            {
                CommandType types = CommandType.None;
                if (MagicCombatSteps != null)
                    types |= CommandType.Magic;
                if (MeleeCombatSteps != null)
                    types |= CommandType.Melee;
                return types;
            }
        }

        public IEnumerable<MagicCombatStep?> GetMagicSteps()
        {
            MagicCombatStep? lastStep = null;
            if (MagicCombatSteps != null)
            {
                foreach (MagicCombatStep next in MagicCombatSteps)
                {
                    yield return next;
                    lastStep = next;
                }
            }
            if (MagicEnd == CombatStepEnd.RepeatLastStep)
            {
                MagicCombatStep lastStepValue = lastStep.Value;
                while (true)
                {
                    yield return lastStepValue;
                }
            }
            yield break;
        }

        public IEnumerable<MeleeCombatStep?> GetMeleeSteps(bool powerAttack)
        {
            MeleeCombatStep? lastStep = null;
            if (MeleeCombatSteps != null)
            {
                foreach (MeleeCombatStep next in MeleeCombatSteps)
                {
                    MeleeCombatStep nextStepActual;
                    if (next == MeleeCombatStep.RegularAttack && powerAttack)
                    {
                        powerAttack = false;
                        nextStepActual =  MeleeCombatStep.PowerAttack;
                    }
                    else
                    {
                        nextStepActual = next;
                    }
                    lastStep = next; //never power attack
                    yield return nextStepActual; //could be power attack or regular attack
                }
            }
            if (MeleeEnd == CombatStepEnd.RepeatLastStep)
            {
                MeleeCombatStep lastStepValue = lastStep.Value;
                while (true)
                {
                    yield return lastStepValue;
                }
            }
            yield break;
        }

        public bool Heal { get; set; }
        public bool ShowPreForm { get; set; }
        public List<MagicCombatStep> MagicCombatSteps { get; set; }
        public List<MeleeCombatStep> MeleeCombatSteps { get; set; }
        public CombatStepEnd MagicEnd { get; set; }
        public CombatStepEnd MeleeEnd { get; set; }
        public CommandType OnlyRunWhenStunned { get; set; }

        public static Macro GenerateCannedMacro(string Name)
        {
            Macro m = new Macro(Name);
            switch (Name)
            {
                case "C*+A*":
                    m.ShowPreForm = true;
                    m.MagicCombatSteps = new List<MagicCombatStep>() { MagicCombatStep.OffensiveSpellAuto };
                    m.MagicEnd = CombatStepEnd.RepeatLastStep;
                    m.MeleeCombatSteps = new List<MeleeCombatStep>() { MeleeCombatStep.RegularAttack };
                    m.MeleeEnd = CombatStepEnd.RepeatLastStep;
                    m.OnlyRunWhenStunned = CommandType.Melee;
                    break;
                case "SC*+A*":
                    m.ShowPreForm = true;
                    m.MagicCombatSteps = new List<MagicCombatStep>() { MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto };
                    m.MagicEnd = CombatStepEnd.RepeatLastStep;
                    m.MeleeCombatSteps = new List<MeleeCombatStep>() { MeleeCombatStep.RegularAttack };
                    m.MeleeEnd = CombatStepEnd.RepeatLastStep;
                    m.OnlyRunWhenStunned = CommandType.Melee;
                    break;
                case "SCCSC*+A*":
                    m.ShowPreForm = true;
                    m.MagicCombatSteps = new List<MagicCombatStep>() { MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto, MagicCombatStep.OffensiveSpellAuto, MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto };
                    m.MagicEnd = CombatStepEnd.RepeatLastStep;
                    m.MeleeCombatSteps = new List<MeleeCombatStep>() { MeleeCombatStep.RegularAttack };
                    m.MeleeEnd = CombatStepEnd.RepeatLastStep;
                    m.OnlyRunWhenStunned = CommandType.Melee;
                    break;
                case "SCCSCCF+A*":
                    m.ShowPreForm = true;
                    m.MagicCombatSteps = new List<MagicCombatStep>() { MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto, MagicCombatStep.OffensiveSpellAuto, MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto, MagicCombatStep.OffensiveSpellAuto };
                    m.MagicEnd = CombatStepEnd.Flee;
                    m.MeleeCombatSteps = new List<MeleeCombatStep>() { MeleeCombatStep.RegularAttack };
                    m.MeleeEnd = CombatStepEnd.RepeatLastStep;
                    m.OnlyRunWhenStunned = CommandType.Melee;
                    break;
                case "C*":
                    m.ShowPreForm = true;
                    m.MagicCombatSteps = new List<MagicCombatStep>() { MagicCombatStep.OffensiveSpellAuto };
                    m.MagicEnd = CombatStepEnd.RepeatLastStep;
                    break;
                case "SC*":
                    m.ShowPreForm = true;
                    m.MagicCombatSteps = new List<MagicCombatStep>() { MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto };
                    m.MagicEnd = CombatStepEnd.RepeatLastStep;
                    break;
                case "SCCSC*":
                    m.ShowPreForm = true;
                    m.MagicCombatSteps = new List<MagicCombatStep>() { MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto, MagicCombatStep.OffensiveSpellAuto, MagicCombatStep.Stun, MagicCombatStep.OffensiveSpellAuto };
                    m.MagicEnd = CombatStepEnd.RepeatLastStep;
                    break;
                case "A*":
                    m.ShowPreForm = true;
                    m.MeleeCombatSteps = new List<MeleeCombatStep>() { MeleeCombatStep.RegularAttack };
                    m.MeleeEnd = CombatStepEnd.RepeatLastStep;
                    break;
                case "Skills":
                    m.ShowPreForm = true;
                    break;
                case "Heal":
                    m.Heal = true;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return m;
        }
    }

    public enum MagicCombatStep
    {
        Stun,
        OffensiveSpellAuto,
        OffensiveSpellLevel1,
        OffensiveSpellLevel2,
        OffensiveSpellLevel3,
        Vigor,
        MendWounds,
    }

    public enum MeleeCombatStep
    {
        PowerAttack,
        RegularAttack,
    }

    public enum CombatStepEnd
    {
        None,
        Flee,
        RepeatLastStep,
    }
}
