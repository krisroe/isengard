using System;
using System.Collections.Generic;
namespace IsengardClient.Backend
{
    public enum FloatRequirement
    {
        None = 0,
        Fly = 1,
        Levitation = 2,
        NoLevitation = 3,
    }

    public enum ExitPresenceType
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

    [Flags]
    public enum PromptedSkills
    {
        None = 0,
        PowerAttack = 1,
        Manashield = 2,
        Fireshield = 4,
    }

    public enum MonsterStatus
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

    public enum InputEchoType
    {
        On,
        OnPassword,
        Off,
    }

    public enum BackgroundProcessPhase
    {
        None,
        Initialization,
        Heal,
        /// <summary>
        /// drink potions, activate skills, remove all equipment
        /// </summary>
        PostHealPreCombatLogic,
        Movement,
        Combat,
        Flee,
        InventoryManagement,
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
    public enum InitializationStep
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
        Uptime = 256,
        RemoveAll = 512,
        BeforeFinalization = 1023,
        Finalization = 1024,
        All = 2047,
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

    public enum MobLocationType
    {
        CurrentRoomMobs,
        PickFromList,
    }

    public enum ItemsToProcessType
    {
        NoProcessing = 0,
        ProcessMonsterDrops = 1,
        ProcessAllItemsInRoom = 2,
    }

    public enum InventoryManagementWorkflow
    {
        ManageSourceItems = 0,
        Ferry = 1,
    }

    public enum MobVisibility
    {
        Visible = 0,
        Hidden = 1,
        Invisible = 2,
    }

    public enum AutoEscapeType
    {
        Flee = 0,
        Hazy = 1,
    }

    public enum BidirectionalExitType
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
        IntenseBlue,
        DullBlue,
        Grey,
        DullRed,
        IntenseRed,
        Red,
    }

    public enum MapType
    {
        [MapTypeDisplayName("Bree Streets")]
        BreeStreets,

        [MapTypeDisplayName("Bree Sewers")]
        BreeSewers,

        [MapTypeDisplayName("Under Bree")]
        UnderBree,

        [MapTypeDisplayName("Millwood Mansion")]
        MillwoodMansion,

        [MapTypeDisplayName("Millwood Mansion Upstairs")]
        MillwoodMansionUpstairs,

        [MapTypeDisplayName("Bree Haunted Mansion")]
        BreeHauntedMansion,

        [MapTypeDisplayName("West of Bree")]
        WestOfBree,

        [MapTypeDisplayName("Bree/Imladris")]
        BreeToImladris,

        [MapTypeDisplayName("Imladris")]
        Imladris,

        [MapTypeDisplayName("Imladris/Tharbad")]
        ImladrisToTharbad,

        [MapTypeDisplayName("East of Imladris")]
        EastOfImladris,

        [MapTypeDisplayName("Shanty Town")]
        ShantyTown,

        [MapTypeDisplayName("Spindril's Castle Level 1")]
        SpindrilsCastleLevel1,

        [MapTypeDisplayName("Tharbad")]
        Tharbad,

        [MapTypeDisplayName("West of Tharbad")]
        WestOfTharbad,

        [MapTypeDisplayName("East of Tharbad")]
        AlliskPlainsEastOfTharbad,

        [MapTypeDisplayName("Esgaroth")]
        Esgaroth,

        [MapTypeDisplayName("Esgaroth Museum")]
        EsgarothMuseum,

        [MapTypeDisplayName("North of Esgaroth")]
        NorthOfEsgaroth,

        [MapTypeDisplayName("Mithlond")]
        Mithlond,

        [MapTypeDisplayName("Nindamos")]
        Nindamos,

        [MapTypeDisplayName("Armenelos")]
        Armenelos,

        [MapTypeDisplayName("Nindamos/Eldemonde")]
        NindamosToEldemonde,

        [MapTypeDisplayName("Eldemonde")]
        Eldemonde,

        [MapTypeDisplayName("Death Valley")]
        DeathValley,

        [MapTypeDisplayName("Intangible")]
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
        [StrategyStep('S', true)]
        Stun,

        [StrategyStep('C', true)]
        OffensiveSpellAuto,

        [StrategyStep('1', true)]
        OffensiveSpellLevel1,

        [StrategyStep('2', true)]
        OffensiveSpellLevel2,

        [StrategyStep('3', true)]
        OffensiveSpellLevel3,

        [StrategyStep('4', true)]
        OffensiveSpellLevel4,

        [StrategyStep('5', true)]
        OffensiveSpellLevel5,

        [StrategyStep('V', false)]
        Vigor,

        [StrategyStep('M', false)]
        MendWounds,

        [StrategyStep('H', false)]
        GenericHeal,

        [StrategyStep('P', false)]
        CurePoison,
    }

    public enum MeleeStrategyStep
    {
        [StrategyStep('A', true)]
        RegularAttack,

        [StrategyStep('P', true)]
        PowerAttack,
    }

    public enum PotionsStrategyStep
    {
        [StrategyStep('v', false)]
        Vigor,

        [StrategyStep('m', false)]
        MendWounds,

        [StrategyStep('h', false)]
        GenericHeal,

        [StrategyStep('p', false)]
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
        Wand = 2,
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
        StartsWithAndEndsWith,
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
        FleeWithoutDropWeapon,
        FleeWithDropWeapon,
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
        KnowAuraOver,
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
        SomethingPoisoned,
        DiseaseDamage,
        SomethingDiseased,
        FleeWithoutDropWeapon,
        FleeWithDropWeapon,
        FleeFailed,
        WordOfRecall,
        BullroarerInMithlond,
        BullroarerInNindamos,
        BullroarerReadyForBoarding,
        CelduinExpressInBree,
        CelduinExpressLeftBree,
        CelduinExpressLeftMithlond,
        OmaniPrincessInMithlond,
        OmaniPrincessInUmbar,
        OmaniPrincessReadyForBoarding,
        HarbringerInPort,
        HarbringerSailed,
        EnemyAttacksYou,
        EnemyCirclesYou,
        EnemyTriesToCircleYou,
        EnemyActivatesSanctuary,
        FallDamage,
        MobArrived,
        MobWanderedAway,
        ReceiveVampyricTouch,
        EquipmentDestroyed,
        EquipmentFellApart,
        WeaponIsBroken,
        FireshieldInflictsDamageAndDissipates,
        MobPickedUpItem,
        StunCastOnEnemy,
        ItemMagicallySentToYou,
    }

    public enum SkillWithCooldownType
    {
        Unknown,
        PowerAttack,
        Manashield,
        Fireshield,
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
        Esgaroth,
    }

    public enum HealingRoom
    {
        BreeNortheast,
        BreeSouthwest,
        Imladris,
        Tharbad,
        Esgaroth,
        BreeArena,
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

    [Flags]
    public enum PlayerStatusFlags
    {
        None = 0,
        Poisoned = 1,
        Prone = 2,
        Diseased = 4,
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
        OmaniPrincessMithlondBoat,
        OmaniPrincessMithlondDock,
        OmaniPrincessUmbarBoat,
        OmaniPrincessUmbarDock,
    }

    public enum EntityChangeType
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
        MagicallySentItem,
        DropItem,
        ConsumeItem,
        Trade,
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
        MagicallySentItem,
        DropItem,
        ConsumeItem,
        SellItem,
        WieldItem,
        HoldItem,
        WearItem,
        Unequip,
        DestroyEquipment,
        Trade,
    }

    public enum EquipmentType
    {
        Unknown,
        Torso,
        Arms,
        Legs,
        Feet,
        Neck,
        Waist,
        Head,
        Face,
        Hands,
        Finger,
        Ears,
        Holding,
        Shield,
        Wielded,
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
        Face = 7,
        Hands = 8,
        Finger1 = 9,
        Finger2 = 10,
        Ears = 11,
        Held = 12,
        Shield = 13,
        Weapon1 = 14,
        Weapon2 = 15,
        Count = 16,
    }

    /// <summary>
    /// what action to take for an item beyond that needed to keep in inventory or in tick room
    /// </summary>
    public enum ItemInventoryOverflowAction
    {
        /// <summary>
        /// no action listed
        /// </summary>
        None = 0,
        /// <summary>
        /// skip the item
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// sell or junk the item
        /// </summary>
        SellOrJunk = 2,
    }

    public enum SellableEnum
    {
        Unknown,
        Sellable,
        NotSellable,
        Junk,
    }

    public enum AfterKillMonsterAction
    {
        ContinueCombat = 0,
        StopCombat = 1,
        SelectFirstMonsterInRoom = 2,
        SelectFirstMonsterInRoomOfSameType = 3,
    }

    public enum ClassType
    {
        Mage,
        Priest,
        Bard,
        Monk,
        Hunter,
        Rogue,
        Warrior,
    }
    public enum ClassTypeFlags
    {
        None = 0,
        Mage = 1,
        Priest = 2,
        Bard = 4,
        Monk = 8,
        Hunter = 16,
        Rogue = 32,
        Warrior = 64,
    }

    public enum VertexSelectionRequirement
    {
        ValidPathFromCurrentLocation,
        UnambiguousRoomBackendOrDisplayName,
    }

    public enum DisconnectedAction
    {
        Reconnect,
        Quit,
        Logout,
    }

    public enum AvailableSpellTypes
    {
        All,
        Castable,
        HavePotions,
    }

    /// <summary>
    /// level at which strategy properties are specified. some strategy properties have a default at the settings level.
    /// strategy properties can also be specified at strategy or perm run levels
    /// </summary>
    public enum StrategyOverridesLevel
    {
        Settings,
        Strategy,
        PermRun,
    }

    public enum FullType
    {
        None,
        Total,
        Almost,
    }

    public enum PermRunFlow
    {
        AdHocStrategy,
        ChangeAndRun,
        Run,
    }

    public enum PermRunEditFlow
    {
        AdHocStrategy,
        Edit,
        ChangeAndRun,
    }

    public enum ConsoleOutputVerbosity
    {
        Minimum = 0,
        Default = 1,
        Maximum = 2,
    }

    public enum AreaRoomType
    {
        Tick,
        Pawn,
        InventorySink,
    }

    public enum OverrideStrategyPropertyType
    {
        AutoSpellLevels,
        Realms,
    }

    /// <summary>
    /// one minute cycles for the celduin express
    /// </summary>
    public enum CelduinExpressCycle
    {
        /// <summary>
        /// in bree, global notification
        /// </summary>
        InBree,
        ToMithlond,
        InMithlond,
        ToBree,
    }

    /// <summary>
    /// one minute cycles for bullroarer
    /// </summary>
    public enum BullroarerCycle
    {
        /// <summary>
        /// Enter and exit in Nindamos. This is synchronized with Celduin Express in Bree.
        /// </summary>
        InNindamos,
        /// <summary>
        /// at sea
        /// </summary>
        AtSea1,
        /// <summary>
        /// can exit to Mithlond but not board
        /// </summary>
        ExitToMithlond1,
        /// <summary>
        /// at sea
        /// </summary>
        AtSea2,
        /// <summary>
        /// Enter at mithlond, exit to Nindamos (glitched). This is synchronized with Celduin express in Bree.
        /// </summary>
        EnterInMithlondExitToNindamos,
        /// <summary>
        /// at sea
        /// </summary>
        AtSea3,
        /// <summary>
        /// can exit to Mithlond but not board
        /// </summary>
        ExitToMithlond2,
        /// <summary>
        /// at sea
        /// </summary>
        AtSea4,
    }

    public enum HarbringerCycle
    {
        /// <summary>
        /// board in tharbad, not possible to disembark. This is synchronized with the celduin express in bree message.
        /// </summary>
        BoardInTharbad,
        /// <summary>
        /// at sea
        /// </summary>
        ToMithlond,
        /// <summary>
        /// enter and exit in mithlond
        /// </summary>
        InMithlond,
        /// <summary>
        /// at sea
        /// </summary>
        ToTharbad,
    }

    public enum OmaniPrincessCycle
    {
        /// <summary>
        /// embark/disembark in Mithlond. This is synchronized with the celduin express in bree message.
        /// </summary>
        InMithlond,

        /// <summary>
        /// at sea
        /// </summary>
        ToUmber,
    }
}
