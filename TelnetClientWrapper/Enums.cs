using System;
using System.Collections.Generic;
namespace IsengardClient
{
    internal enum FloatRequirement
    {
        None = 0,
        Fly = 1,
        Levitation = 2,
        NoLevitation = 3,
    }

    internal enum ExitPresenceType
    {
        /// <summary>
        /// exits that are always present
        /// </summary>
        Always,

        /// <summary>
        /// exits that are sometimes present (e.g. whether boat is present in port)
        /// </summary>
        Periodic,

        /// <summary>
        /// hidden exits that require search to be usable. These are assumed to be hidden.
        /// </summary>
        RequiresSearch,
    }

    internal enum KeyType
    {
        None,
        GateKey,
        KasnarsRedKey,
        SilverKey,
        BoilerKey,
        BridgeKey,
        UnknownKnockable,
    }

    [Flags]
    internal enum PromptedSkills
    {
        None = 0,
        PowerAttack = 1,
        Manashield = 2,
        Fireshield = 4,
    }

    /// <summary>
    /// result of a single command
    /// </summary>
    public enum CommandResult
    {
        /// <summary>
        /// the command completed successfully
        /// </summary>
        CommandSuccessful,

        /// <summary>
        /// the command was unsuccessful, but could succeed if run again (e.g. flee, search, knock, manashield)
        /// </summary>
        CommandUnsuccessfulThisTime,

        /// <summary>
        /// the command was unsuccessful, and it is expected the command would continue to not work if tried again
        /// </summary>
        CommandUnsuccessfulAlways,

        /// <summary>
        /// additional wait time is needed before the command can be run
        /// </summary>
        CommandMustWait,

        /// <summary>
        /// the background process was aborted, such as the user cancelling the background process or a hazy or flee was triggered
        /// </summary>
        CommandAborted,

        /// <summary>
        /// no response was processed from the server within the timeout interval
        /// </summary>
        CommandTimeout,
    }

    public enum BackgroundCommandType
    {
        Movement,
        OpenDoor,
        Look,
        LookAtMob,
        Prepare,
        Search,
        Knock,
        Vigor,
        MendWounds,
        CurePoison,
        Bless,
        Protection,
        Manashield,
        Fireshield,
        Stun,
        OffensiveSpell,
        Attack,
        Flee,
        DrinkHazy,
        DrinkNonHazyPotion,
        GetItem,
        SellItem,
        DropItem,
        Score,
        Inventory,
        Quit,
    }

    internal enum MonsterStatus
    {
        None,
        ExcellentCondition,
        FewSmallScratches,
        WincingInPain,
        SlightlyBruisedAndBattered,
        SomeMinorWounds,
        BleedingProfusely,
        NastyAndGapingWound,
        ManyGreviousWounds,
        MortallyWounded,
        BarelyClingingToLife,
    }

    internal enum InputEchoType
    {
        On,
        OnPassword,
        Off,
    }

    internal enum BackgroundProcessPhase
    {
        None,
        Initialization,
        Heal,
        ActivateSkills,
        Movement,
        Combat,
        Flee,
        Hazy,
        Score,
    }

    [Flags]
    public enum CommandType
    {
        None = 0,
        Melee = 1,
        Magic = 2,
        Potions = 4,
        All = 7,
    }

    [Flags]
    internal enum InitializationStep
    {
        None = 0,
        Initialization = 1,
        Equipment = 2,
        Score = 4,
        Information = 8,
        Time = 16,
        Who = 32,
        Spells = 64,
        Inventory = 128,
        RemoveAll = 256,
        BeforeFinalization = 511,
        Finalization = 512,
        All = 1023,
    }

    public enum EntityType
    {
        Player,
        Mob,
        Item,
        Unknown,
    }

    [Flags]
    public enum EntityTypeFlags
    {
        None = 0,
        Player = 1,
        Mob = 2,
        Item = 4,
    }

    public enum ItemLocationType
    {
        Inventory,
        Equipment,
        Room,
    }

    [Flags]
    public enum ItemLocationTypeFlags
    {
        None = 0,
        Inventory = 1,
        Equipment = 2,
        Room = 4,
        All = 7,
    }

    public enum AutoEscapeType
    {
        Flee = 0,
        Hazy = 1,
    }

    public enum AutoEscapeActivity
    {
        Active,
        Inactive,
        Inherit,
    }

    internal enum BidirectionalExitType
    {
        WestEast,
        NorthSouth,
        SoutheastNorthwest,
        SouthwestNortheast,
        UpDown,
    }

    public enum AlignmentType
    {
        Blue,
        Grey,
        Red,
    }

    internal enum MapType
    {
        BreeStreets,
        BreeSewers,
        UnderBree,
        MillwoodMansion,
        MillwoodMansionUpstairs,
        BreeHauntedMansion,
        WestOfBree,
        BreeToImladris,
        Imladris,
        ImladrisToTharbad,
        EastOfImladris,
        ShantyTown,
        SpindrilsCastleLevel1,
        Tharbad,
        WestOfTharbad,
        AlliskPlainsEastOfTharbad,
        Esgaroth,
        NorthOfEsgaroth,
        Mithlond,
        Nindamos,
        Armenelos,
        NindamosToEldemonde,
        Eldemonde,
        DeathValley,
        Intangible,
    }

    public enum FinalStepAction
    {
        None = 0,
        Flee = 1,
        Hazy = 2,
        FinishCombat = 3,
    }

    public enum MagicStrategyStep
    {
        Stun,
        OffensiveSpellAuto,
        OffensiveSpellLevel1,
        OffensiveSpellLevel2,
        OffensiveSpellLevel3,
        OffensiveSpellLevel4,
        OffensiveSpellLevel5,
        Vigor,
        MendWounds,
        GenericHeal,
        CurePoison,
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
        CurePoison,
    }

    public enum PotionsCommandChoiceResult
    {
        Drink,
        Skip,
        Fail,
    }

    public enum MagicCommandChoiceResult
    {
        Cast,
        Skip,
        OutOfMana,
    }

    [Flags]
    public enum DependentObjectType
    {
        None = 0,
        Mob = 1,
        Weapon = 2,
        Wand = 4,
    }

    public enum ValidPotionType
    {
        Invalid,
        Primary,
        Secondary,
    }

    public enum OutputItemSequenceType
    {
        UserNamePrompt,
        PasswordPrompt,
        HPMPStatus,
        ContinueToNextScreen,
        Goodbye,
    }

    public enum ConstantSequenceMatchType
    {
        ExactMatch,
        StartsWith,
        EndsWith,
        Contains,
    }

    public enum SkillCooldownStatus
    {
        /// <summary>
        /// currently active
        /// </summary>
        Active,

        /// <summary>
        /// currently available
        /// </summary>
        Available,

        /// <summary>
        /// not currently available and waiting to become available
        /// </summary>
        Waiting,

        /// <summary>
        /// not currently available and the next time it will be available is unknown
        /// </summary>
        Inactive,
    }

    public enum RoomTransitionType
    {
        Initial,
        Move,
        Flee,
        WordOfRecall,
        Death,
    }

    public class InformationalMessages
    {
        public InformationalMessages(InformationalMessageType msgType)
        {
            this.MessageType = msgType;
        }

        public InformationalMessageType MessageType { get; set; }
        public int Damage { get; set; }
        public int MobCount { get; set; }
        public MobTypeEnum Mob { get; set; }
        public ItemEntity Item { get; set; }
        public int WaitSeconds { get; set; }

        public static bool ContainsType(List<InformationalMessages> messages, InformationalMessageType type)
        {
            bool ret = false;
            foreach (var nextMessage in messages)
            {
                if (nextMessage.MessageType == type)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
    }

    public enum InformationalMessageType
    {
        InitialLogin,
        Death,
        PleaseWait,
        DayStart,
        NightStart,
        BlessOver,
        ProtectionOver,
        FlyOver,
        LevitationOver,
        InvisibilityOver,
        DetectInvisibleOver,
        LightOver,
        EndureFireOver,
        EndureColdOver,
        EndureWaterOver,
        EndureEarthOver,
        DetectMagicOver,
        ManashieldOff,
        FireshieldOff,
        FireDamage,
        WaterDamage,
        EarthDamage,
        WindDamage,
        PoisonDamage,
        RoomPoisoned,
        Flee,
        FleeFailed,
        WordOfRecall,
        BullroarerInMithlond,
        BullroarerInNindamos,
        BullroarerReadyForBoarding,
        CelduinExpressInBree,
        CelduinExpressLeftBree,
        CelduinExpressLeftMithlond,
        HarbringerInPort,
        HarbringerSailed,
        EnemyAttacksYou,
        EnemyCirclesYou,
        EnemyTriesToCircleYou,
        FallDamage,
        MobArrived,
        MobWanderedAway,
        ReceiveVampyricTouch,
        EquipmentDestroyed,
        EquipmentFellApart,
        FireshieldInflictsDamageAndDissipates,
        MobPickedUpItem,
    }

    public enum SkillWithCooldownType
    {
        Unknown,
        PowerAttack,
        Manashield,
        Fireshield,
    }

    public enum MovementResult
    {
        Success,
        TotalFailure,
        MapFailure,
        StandFailure,
        ClosedDoorFailure,
        LockedDoorFailure,
        FallFailure,
    }

    public enum RealmType
    {
        Earth = 0,
        Wind = 1,
        Water = 2,
        Fire = 3,
    }

    public enum RoomDamageType
    {
        Earth = 0,
        Wind = 1,
        Water = 2,
        Fire = 3,
        Poison = 4,
    }

    public enum PawnShoppe
    {
        BreeNortheast,
        BreeSouthwest,
        Imladris,
        Tharbad,
    }

    public enum HealingRoom
    {
        BreeNortheast,
        BreeSouthwest,
        Imladris,
        Tharbad,
        SpindrilsCastle,
        Nindamos,
        Underhalls,
        MillwoodMansion,
        DeathValley,
    }

    [Flags]
    public enum TrapType
    {
        None = 0,
        PoisonDart = 1,
        Fall = 2,
    }

    public enum BoatEmbarkOrDisembark
    {
        CelduinExpress,
        CelduinExpressMithlond,
        CelduinExpressBree,
        Bullroarer,
        BullroarerMithlond,
        BullroarerNindamos,
        Harbringer,
        HarbringerMithlond,
        HarbringerTharbad,
    }

    internal enum EntityChangeType
    {
        RefreshRoom,
        RefreshInventory,
        RefreshEquipment,
        RefreshRoomItems,
        AddExit,
        RemoveExit,
        AddMob,
        RemoveMob,
        PickUpItem,
        DropItem,
        ConsumeItem,
        EquipItem,
        UnequipItem,
        DestroyEquipment,
        CreateRoomItems,
        RemoveRoomItems,
    }

    public enum ItemManagementAction
    {
        None,
        PickUpItem,
        DropItem,
        ConsumeItem,
        SellItem,
        Equip,
        Unequip,
        DestroyEquipment,
    }

    public enum EquipmentType
    {
        Torso,
        Arms,
        Legs,
        Feet,
        Neck,
        Waist,
        Head,
        Hands,
        Finger,
        Ears,
        Holding,
        Shield,
        Wielded,
        Unknown,
    }

    public enum WeaponType
    {
        Slash,
        Stab,
        Blunt,
        Polearm,
        Missile,
        Unknown,
    }

    public enum EquipmentSlot
    {
        Torso = 0,
        Arms = 1,
        Legs = 2,
        Feet = 3,
        Neck = 4,
        Waist = 5,
        Head = 6,
        Hands = 7,
        Finger1 = 8,
        Finger2 = 9,
        Ears = 10,
        Held = 11,
        Shield = 12,
        Weapon1 = 13,
        Weapon2 = 14,
        Count = 15,
    }

    public enum ItemInventoryAction
    {
        /// <summary>
        /// no action listed
        /// </summary>
        None,
        /// <summary>
        /// skip the item
        /// </summary>
        Ignore,
        /// <summary>
        /// pick up the item and do nothing with it
        /// </summary>
        Take,
        /// <summary>
        /// sell the item
        /// </summary>
        Sell,
        /// <summary>
        /// take the item to the tick room
        /// </summary>
        Tick,
    }

    public enum AfterKillMonsterAction
    {
        ContinueCombat = 0,
        StopCombat = 1,
        SelectFirstMonsterInRoom = 2,
        SelectFirstMonsterInRoomOfSameType = 3,
    }

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

    public enum SpellsEnum
    {
        [SpellInformation(SpellProficiency.Earth, 1)]
        rumble,

        [SpellInformation(SpellProficiency.Earth, 2)]
        crush,

        [SpellInformation(SpellProficiency.Earth, 3)]
        shatterstone,

        [SpellInformation(SpellProficiency.Earth, 3, "endure-earth")]
        endureearth,

        [SpellInformation(SpellProficiency.Earth, 4)]
        engulf,

        [SpellInformation(SpellProficiency.Earth, 4, "resist-earth")]
        resistearth,

        [SpellInformation(SpellProficiency.Earth, 5)]
        tremor,

        [SpellInformation(SpellProficiency.Earth, 6)]
        earthquake,

        [SpellInformation(SpellProficiency.Wind, 1)]
        hurt,

        [SpellInformation(SpellProficiency.Wind, 2)]
        dustgust,

        [SpellInformation(SpellProficiency.Wind, 3)]
        shockbolt,

        [SpellInformation(SpellProficiency.Wind, 3, "endure-cold")]
        endurecold,

        [SpellInformation(SpellProficiency.Wind, 4)]
        lightning,

        [SpellInformation(SpellProficiency.Wind, 4, "resist-wind")]
        resistwind,

        [SpellInformation(SpellProficiency.Wind, 5)]
        thunderbolt,

        [SpellInformation(SpellProficiency.Wind, 6)]
        tornado,

        [SpellInformation(SpellProficiency.Fire, 1)]
        burn,

        [SpellInformation(SpellProficiency.Fire, 2)]
        fireball,

        [SpellInformation(SpellProficiency.Fire, 3)]
        burstflame,

        [SpellInformation(SpellProficiency.Fire, 3, "endure-fire")]
        endurefire,

        [SpellInformation(SpellProficiency.Fire, 4)]
        immolate,

        [SpellInformation(SpellProficiency.Fire, 4, "resist-fire")]
        resistfire,

        [SpellInformation(SpellProficiency.Fire, 5)]
        flamefill,

        [SpellInformation(SpellProficiency.Fire, 6)]
        incinerate,

        [SpellInformation(SpellProficiency.Water, 1)]
        blister,

        [SpellInformation(SpellProficiency.Water, 2)]
        waterbolt,

        [SpellInformation(SpellProficiency.Water, 3)]
        steamblast,

        [SpellInformation(SpellProficiency.Water, 3, "endure-water")]
        endurewater,

        [SpellInformation(SpellProficiency.Water, 4)]
        bloodboil,

        [SpellInformation(SpellProficiency.Water, 4, "resist-water")]
        resistwater,

        [SpellInformation(SpellProficiency.Water, 5)]
        iceblade,

        [SpellInformation(SpellProficiency.Water, 6)]
        flood,

        [SpellInformation(SpellProficiency.Divination, 1, "know-aura")]
        knowaura,

        [SpellInformation(SpellProficiency.Divination, 2, "detect-magic")]
        detectmagic,

        [SpellInformation(SpellProficiency.Divination, 2)]
        fortune,

        [SpellInformation(SpellProficiency.Divination, 2)]
        farsight,

        [SpellInformation(SpellProficiency.Divination, 3, "detect-invis")]
        detectinvis,

        [SpellInformation(SpellProficiency.Divination, 3, "detect-relics")]
        detectrelics,

        [SpellInformation(SpellProficiency.Divination, 4)]
        clairvoyance,

        [SpellInformation(SpellProficiency.Divination, 5)]
        summon,

        [SpellInformation(SpellProficiency.Divination, 6)]
        tracking,

        [SpellInformation(SpellProficiency.Arcana, 1)]
        light,

        [SpellInformation(SpellProficiency.Arcana, 2)]
        levitate,

        [SpellInformation(SpellProficiency.Arcana, 3)]
        invisibility,

        [SpellInformation(SpellProficiency.Arcana, 3)]
        fly,

        [SpellInformation(SpellProficiency.Arcana, 4)]
        dispel,

        [SpellInformation(SpellProficiency.Arcana, 4)]
        transport,

        [SpellInformation(SpellProficiency.Arcana, 5)]
        teleport,

        [SpellInformation(SpellProficiency.Arcana, 5)]
        knock,

        [SpellInformation(SpellProficiency.Arcana, 6, "word-of-recall")]
        wordofrecall,

        [SpellInformation(SpellProficiency.Life, 1)]
        vigor,

        [SpellInformation(SpellProficiency.Life, 1, "cure-poison")]
        curepoison,

        [SpellInformation(SpellProficiency.Life, 2)]
        bless,

        [SpellInformation(SpellProficiency.Life, 2)]
        mend,

        [SpellInformation(SpellProficiency.Life, 2)]
        protection,

        [SpellInformation(SpellProficiency.Life, 3, "cure-disease")]
        curedisease,

        [SpellInformation(SpellProficiency.Life, 4)]
        revive,

        [SpellInformation(SpellProficiency.Life, 5, "cure-malady")]
        curemalady,

        [SpellInformation(SpellProficiency.Life, 6)]
        heal,

        [SpellInformation(SpellProficiency.Sorcery, 1)]
        fumble,

        [SpellInformation(SpellProficiency.Sorcery, 2)]
        stun,

        [SpellInformation(SpellProficiency.Sorcery, 3)]
        drain,

        [SpellInformation(SpellProficiency.Sorcery, 3, "remove-curse")]
        removecurse,

        [SpellInformation(SpellProficiency.Sorcery, 4)]
        curse,

        [SpellInformation(SpellProficiency.Sorcery, 4)]
        fear,

        [SpellInformation(SpellProficiency.Sorcery, 5)]
        conjure,

        [SpellInformation(SpellProficiency.Sorcery, 5)]
        mute,

        [SpellInformation(SpellProficiency.Sorcery, 6, "resist-magic")]
        resistmagic,
    }
}
