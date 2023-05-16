using System;
using System.Collections.Generic;
using static IsengardClient.frmMain;

namespace IsengardClient
{
    internal class Macro
    {
        public Macro(string Name)
        {
            this.Name = Name;
            this.Steps = new List<MacroStepBase>();
        }
        public override string ToString()
        {
            return this.Name;
        }

        public string Name { get; set; }
        public List<MacroStepBase> Steps { get; set; }
        public bool SetParentLocation { get; set; }
        public CommandType CombatCommandTypes { get; set; }
        public string FinalCommand { get; set; }
        public Variable FinalCommandConditionVariable { get; set; }
        public string FinalCommand2 { get; set; }
        public Variable FinalCommand2ConditionVariable { get; set; }
        public bool OneClick { get; set; }

        public static bool IsValidMacroName(string name)
        {
            foreach (ObjectType ot in Enum.GetValues(typeof(ObjectType)))
            {
                if (ot != ObjectType.Weapon && ot.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            if (bool.TryParse(name, out _))
            {
                return false;
            }
            if (int.TryParse(name, out _))
            {
                return false;
            }
            return true;
        }
    }

    internal class MacroStepSequence : MacroStepBase
    {
        public List<MacroStepBase> SubCommands { get; set; }
        public MacroStepSequence()
        {
            this.SubCommands = new List<MacroStepBase>();
        }
    }

    internal class MacroStepSetVariable : MacroStepBase
    {
        public Variable Variable { get; set; }

        public MacroStepSetVariable()
        {
        }
    }

    internal class MacroManaSpellCommand : MacroStepBase
    {
    }

    internal class MacroManaSpellStun : MacroManaSpellCommand
    {
    }

    internal class MacroManaSpellOffensive : MacroManaSpellCommand
    {
    }

    internal class MacroCommand : MacroStepBase
    {
        public string RawCommand { get; set; }
        public string Command { get; set; }
        public ManaDrainType ManaDrain { get; set; }
        public MacroCommand(string RawCommand, string Command)
        {
            this.RawCommand = RawCommand;
            this.Command = Command;
        }
    }

    internal class MacroStepSetNextCommandWaitMS : MacroStepBase
    {
        public MacroStepSetNextCommandWaitMS()
        {
        }
    }

    internal enum ManaDrainType
    {
        None = 0,
        Stun = 1,
        Offensive = 2,
    }

    internal enum ObjectType
    {
        Mob,
        Weapon,
        Wand,
        Potion,
        Realm1Spell,
        Realm2Spell,
        Realm3Spell,
    }
}
