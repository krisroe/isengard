using System;

namespace IsengardClient
{
    internal enum FloatRequirement
    {
        None = 0,
        Fly = 1,
        FlyOrLevitation = 2,
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
        UnknownKnockable,
    }

    [Flags]
    internal enum PromptedSkills
    {
        None = 0,
        PowerAttack = 1,
        Manashield = 2
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
        Stun,
        OffensiveSpell,
        Attack,
        Flee,
        DrinkHazy,
        Score,
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
        Quit,
    }

    [Flags]
    public enum CommandType
    {
        None = 0,
        Melee = 1,
        Magic = 2,
        Potions = 4,
    }

    [Flags]
    internal enum InitializationStep
    {
        None = 0,
        Initialization = 1,
        RemoveAll = 2,
        Score = 4,
        Time = 8,
        Who = 16,
        BeforeFinalization = 31,
        Finalization = 32,
        All = 63,
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
        Item = 3,
    }

    public enum AutoEscapeType
    {
        Flee = 0,
        Hazy = 1,
    }

    internal enum BidirectionalExitType
    {
        WestEast,
        NorthSouth,
        SoutheastNorthwest,
        SouthwestNortheast,
        UpDown,
    }

    internal enum AlignmentType
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
        Mithlond,
        Nindamos,
        Armenelos,
        Eldemonde,
        Intangible,
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
        Hazy,
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
    }

    public enum InformationalMessageType
    {
        InitialLogin,
        Death,
        DayStart,
        NightStart,
        BlessOver,
        ProtectionOver,
        FlyOver,
        ManashieldOff,
        FireDamage,
        WaterDamage,
        EarthDamage,
        WindDamage,
        PoisonDamage,
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
        FallDamage,
        MobArrived,
        MobWanderedAway,
    }

    public enum SkillWithCooldownType
    {
        Unknown,
        PowerAttack,
        Manashield,
    }

    public enum MovementResult
    {
        Success,
        TotalFailure,
        MapFailure,
        StandFailure,
        ClosedDoorFailure,
        FallFailure,
    }

    public enum RealmType
    {
        Earth = 0,
        Wind = 1,
        Water = 2,
        Fire = 3,
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

    internal enum RoomChangeType
    {
        NewRoom,
        AddExit,
        RemoveExit,
        AddMob,
        RemoveMob,
    }
}
