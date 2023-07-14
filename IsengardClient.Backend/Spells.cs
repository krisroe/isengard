using System;
using System.Collections.Generic;
using System.Linq;
namespace IsengardClient.Backend
{
    public enum SpellProficiency
    {
        Earth,
        Wind,
        Fire,
        Water,
        Divination,
        Arcana,
        Life,
        Sorcery,
    }

    public enum RealmType
    {
        Earth = 0,
        Wind = 1,
        Water = 2,
        Fire = 3,
    }

    [Flags]
    public enum RealmTypeFlags
    {
        None = 0,
        Earth = 1,
        Wind = 2,
        Water = 4,
        Fire = 8,
        All = 15,
    }

    [Flags]
    public enum WorkflowSpells
    {
        None = 0,
        Bless = 1,
        Protection = 2,
        Levitation = 4,
        Fly = 8,
        EndureEarth = 16,
        EndureFire = 32,
        EndureWater = 64,
        EndureCold = 128,
        CurePoison = 256,
    }

    public class SpellsStatic
    {
        public static Dictionary<string, SpellInformationAttribute> SpellsByName = new Dictionary<string, SpellInformationAttribute>();
        public static Dictionary<SpellsEnum, SpellInformationAttribute> SpellsByEnum = new Dictionary<SpellsEnum, SpellInformationAttribute>();
        public static Dictionary<WorkflowSpells, SpellInformationAttribute> WorkflowSpellsByEnum = new Dictionary<WorkflowSpells, SpellInformationAttribute>();

        static SpellsStatic()
        {
            Type t = typeof(SpellsEnum);
            foreach (SpellsEnum nextSpell in Enum.GetValues(t))
            {
                var memberInfos = t.GetMember(nextSpell.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);
                object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SpellInformationAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                {
                    SpellInformationAttribute sia = (SpellInformationAttribute)valueAttributes[0];
                    if (string.IsNullOrEmpty(sia.SpellName)) sia.SpellName = nextSpell.ToString();
                    SpellsByName[sia.SpellName] = sia;
                    SpellsByEnum[nextSpell] = sia;
                    sia.SpellType = nextSpell;
                }
            }
            Dictionary<WorkflowSpells, SpellsEnum> workflowSpellsMapping = new Dictionary<WorkflowSpells, SpellsEnum>();
            workflowSpellsMapping[WorkflowSpells.Bless] = SpellsEnum.bless;
            workflowSpellsMapping[WorkflowSpells.CurePoison] = SpellsEnum.curepoison;
            workflowSpellsMapping[WorkflowSpells.EndureCold] = SpellsEnum.endurecold;
            workflowSpellsMapping[WorkflowSpells.EndureEarth] = SpellsEnum.endureearth;
            workflowSpellsMapping[WorkflowSpells.EndureFire] = SpellsEnum.endurefire;
            workflowSpellsMapping[WorkflowSpells.EndureWater] = SpellsEnum.endurewater;
            workflowSpellsMapping[WorkflowSpells.Fly] = SpellsEnum.fly;
            workflowSpellsMapping[WorkflowSpells.Levitation] = SpellsEnum.levitate;
            workflowSpellsMapping[WorkflowSpells.Protection] = SpellsEnum.protection;
            foreach (var next in workflowSpellsMapping)
            {
                var nextSpell = SpellsByEnum[next.Value];
                var nextWorkflowSpell = next.Key;
                WorkflowSpellsByEnum[nextWorkflowSpell] = nextSpell;
                nextSpell.WorkflowSpellType = nextWorkflowSpell;
            }
        }
    }

    public enum SpellsEnum
    {
        [SpellInformation(SpellProficiency.Earth, 3, 1)]
        rumble,

        [SpellInformation(SpellProficiency.Earth, 7, 2)]
        crush,

        [SpellInformation(SpellProficiency.Earth, 10, 3)]
        shatterstone,

        [SpellInformation(SpellProficiency.Earth, 10, 3, "endure-earth")]
        endureearth,

        [SpellInformation(SpellProficiency.Earth, 15, 4)]
        engulf,

        [SpellInformation(SpellProficiency.Earth, 15, 4, "resist-earth")]
        resistearth,

        [SpellInformation(SpellProficiency.Earth, 25, 5)]
        tremor,

        [SpellInformation(SpellProficiency.Earth, 25, 6)]
        earthquake,

        [SpellInformation(SpellProficiency.Wind, 3, 1)]
        hurt,

        [SpellInformation(SpellProficiency.Wind, 7, 2)]
        dustgust,

        [SpellInformation(SpellProficiency.Wind, 10, 3)]
        shockbolt,

        [SpellInformation(SpellProficiency.Wind, 10, 3, "endure-cold")]
        endurecold,

        [SpellInformation(SpellProficiency.Wind, 15, 4)]
        lightning,

        [SpellInformation(SpellProficiency.Wind, 15, 4, "resist-wind")]
        resistwind,

        [SpellInformation(SpellProficiency.Wind, 25, 5)]
        thunderbolt,

        [SpellInformation(SpellProficiency.Wind, 25, 6)]
        tornado,

        [SpellInformation(SpellProficiency.Fire, 3, 1)]
        burn,

        [SpellInformation(SpellProficiency.Fire, 7, 2)]
        fireball,

        [SpellInformation(SpellProficiency.Fire, 10, 3)]
        burstflame,

        [SpellInformation(SpellProficiency.Fire, 10, 3, "endure-fire")]
        endurefire,

        [SpellInformation(SpellProficiency.Fire, 15, 4)]
        immolate,

        [SpellInformation(SpellProficiency.Fire, 15, 4, "resist-fire")]
        resistfire,

        [SpellInformation(SpellProficiency.Fire, 25, 5)]
        flamefill,

        [SpellInformation(SpellProficiency.Fire, 25, 6)]
        incinerate,

        [SpellInformation(SpellProficiency.Water, 3, 1)]
        blister,

        [SpellInformation(SpellProficiency.Water, 7, 2)]
        waterbolt,

        [SpellInformation(SpellProficiency.Water, 10, 3)]
        steamblast,

        [SpellInformation(SpellProficiency.Water, 10, 3, "endure-water")]
        endurewater,

        [SpellInformation(SpellProficiency.Water, 15, 4)]
        bloodboil,

        [SpellInformation(SpellProficiency.Water, 15, 4, "resist-water")]
        resistwater,

        [SpellInformation(SpellProficiency.Water, 25, 5)]
        iceblade,

        [SpellInformation(SpellProficiency.Water, 25, 6)]
        flood,

        [SpellInformation(SpellProficiency.Divination, 4, 1, "know-aura")]
        knowaura,

        [SpellInformation(SpellProficiency.Divination, 10, 2, "detect-magic")]
        detectmagic,

        [SpellInformation(SpellProficiency.Divination, 0, 2)]
        fortune,

        [SpellInformation(SpellProficiency.Divination, 15, 2)]
        farsight,

        [SpellInformation(SpellProficiency.Divination, 10, 3, "detect-invis")]
        detectinvis,

        [SpellInformation(SpellProficiency.Divination, 1, 3, "detect-relics")]
        detectrelics,

        [SpellInformation(SpellProficiency.Divination, 15, 4)]
        clairvoyance,

        [SpellInformation(SpellProficiency.Divination, 30, 5)]
        summon,

        [SpellInformation(SpellProficiency.Divination, 0, 6)]
        tracking,

        [SpellInformation(SpellProficiency.Arcana, 5, 1)]
        light,

        [SpellInformation(SpellProficiency.Arcana, 10, 2)]
        levitate,

        [SpellInformation(SpellProficiency.Arcana, 15, 3)]
        invisibility,

        [SpellInformation(SpellProficiency.Arcana, 15, 3)]
        fly,

        [SpellInformation(SpellProficiency.Arcana, 15, 4)]
        dispel,

        [SpellInformation(SpellProficiency.Arcana, int.MaxValue, 4)]
        transport,

        [SpellInformation(SpellProficiency.Arcana, 25, 5)]
        teleport,

        [SpellInformation(SpellProficiency.Arcana, 0, 5)]
        knock,

        [SpellInformation(SpellProficiency.Arcana, 25, 6, "word-of-recall")]
        wordofrecall,

        [SpellInformation(SpellProficiency.Life, 2, 1)]
        vigor,

        [SpellInformation(SpellProficiency.Life, 6, 1, "cure-poison")]
        curepoison,

        [SpellInformation(SpellProficiency.Life, 8, 2)]
        bless,

        [SpellInformation(SpellProficiency.Life, 6, 2, "mend-wounds")]
        mend,

        [SpellInformation(SpellProficiency.Life, 8, 2)]
        protection,

        [SpellInformation(SpellProficiency.Life, 12, 3, "cure-disease")]
        curedisease,

        [SpellInformation(SpellProficiency.Life, 12, 4)]
        revive,

        [SpellInformation(SpellProficiency.Life, 25, 5, "cure-malady")]
        curemalady,

        [SpellInformation(SpellProficiency.Life, 30, 6)]
        heal,

        [SpellInformation(SpellProficiency.Sorcery, 7, 1)]
        fumble,

        [SpellInformation(SpellProficiency.Sorcery, 10, 2)]
        stun,

        [SpellInformation(SpellProficiency.Sorcery, 12, 3)]
        drain,

        [SpellInformation(SpellProficiency.Sorcery, 18, 3, "remove-curse")]
        removecurse,

        [SpellInformation(SpellProficiency.Sorcery, 12, 4)]
        curse,

        [SpellInformation(SpellProficiency.Sorcery, 15, 4)]
        fear,

        [SpellInformation(SpellProficiency.Sorcery, int.MaxValue, 5)]
        conjure,

        [SpellInformation(SpellProficiency.Sorcery, 7, 5)]
        mute,

        [SpellInformation(SpellProficiency.Sorcery, 12, 6, "resist-magic")]
        resistmagic,

        restore,

        unknown,
    }
}
