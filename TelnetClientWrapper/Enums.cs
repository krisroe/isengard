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
    internal enum CommandResult
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

    internal enum BackgroundCommandType
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
        RemoveEquipment,
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
    internal enum CommandType
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

    internal enum EntityType
    {
        Player,
        Mob,
        Item,
        Unknown,
    }

    [Flags]
    internal enum EntityTypeFlags
    {
        None = 0,
        Player = 1,
        Mob = 2,
        Item = 4,
    }

    internal enum ItemLocationType
    {
        Inventory,
        Equipment,
        Room,
    }

    internal enum MobLocationType
    {
        CurrentRoomMobs,
        PickFromList,
    }

    internal enum InventoryProcessWorkflow
    {
        NoProcessing = 0,
        ProcessMonsterDrops = 1,
        ProcessAllItemsInRoom = 2,
    }

    internal enum MobVisibility
    {
        Visible = 0,
        Hidden = 1,
        Invisible = 2,
    }

    internal enum AutoEscapeType
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
        IntenseBlue,
        DullBlue,
        Grey,
        DullRed,
        IntenseRed,
        Red,
    }

    internal enum MapType
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

    internal enum FinalStepAction
    {
        None = 0,
        Flee = 1,
        Hazy = 2,
        FinishCombat = 3,
    }

    internal enum MagicStrategyStep
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

    internal enum MeleeStrategyStep
    {
        [StrategyStep('A', true)]
        RegularAttack,

        [StrategyStep('P', true)]
        PowerAttack,
    }

    internal enum PotionsStrategyStep
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

    internal enum PotionsCommandChoiceResult
    {
        Drink,
        Skip,
        Fail,
    }

    internal enum MagicCommandChoiceResult
    {
        Cast,
        Skip,
        OutOfMana,
    }

    [Flags]
    internal enum DependentObjectType
    {
        None = 0,
        Mob = 1,
        Weapon = 2,
        Wand = 4,
    }

    internal enum ValidPotionType
    {
        Invalid,
        Primary,
        Secondary,
    }

    internal enum OutputItemSequenceType
    {
        UserNamePrompt,
        PasswordPrompt,
        HPMPStatus,
        ContinueToNextScreen,
        Goodbye,
    }

    internal enum ConstantSequenceMatchType
    {
        ExactMatch,
        StartsWith,
        EndsWith,
        Contains,
    }

    internal enum SkillCooldownStatus
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

    internal enum RoomTransitionType
    {
        Initial,
        Move,
        FleeWithoutDropWeapon,
        FleeWithDropWeapon,
        WordOfRecall,
        Death,
    }

    internal class InformationalMessages
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

    internal enum InformationalMessageType
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
        RoomPoisoned,
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

    internal enum SkillWithCooldownType
    {
        Unknown,
        PowerAttack,
        Manashield,
        Fireshield,
    }

    internal enum MovementResult
    {
        Success,
        TotalFailure,
        MapFailure,
        StandFailure,
        ClosedDoorFailure,
        LockedDoorFailure,
        FallFailure,
    }

    internal enum RealmType
    {
        Earth = 0,
        Wind = 1,
        Water = 2,
        Fire = 3,
    }

    internal enum RoomDamageType
    {
        Earth = 0,
        Wind = 1,
        Water = 2,
        Fire = 3,
        Poison = 4,
    }

    internal enum PawnShoppe
    {
        BreeNortheast,
        BreeSouthwest,
        Imladris,
        Tharbad,
        Esgaroth,
    }

    internal enum HealingRoom
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
    internal enum TrapType
    {
        None = 0,
        PoisonDart = 1,
        Fall = 2,
    }

    internal enum BoatEmbarkOrDisembark
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
        MagicallySentItem,
        DropItem,
        ConsumeItem,
        EquipItem,
        UnequipItem,
        DestroyEquipment,
        CreateRoomItems,
        RemoveRoomItems,
    }

    internal enum ItemManagementAction
    {
        None,
        PickUpItem,
        MagicallySentItem,
        DropItem,
        ConsumeItem,
        SellItem,
        Equip,
        Unequip,
        DestroyEquipment,
    }

    internal enum EquipmentType
    {
        Torso,
        Arms,
        Legs,
        Feet,
        Neck,
        Waist,
        Head,
        Hands, //CSRTODO: hands vs face?
        Face,  //CSRTODO: hands vs face?
        Finger,
        Ears,
        Holding,
        Shield,
        Wielded,
        Unknown,
    }

    internal enum WeaponType
    {
        Slash,
        Stab,
        Blunt,
        Polearm,
        Missile,
        Unknown,
    }

    internal enum EquipmentSlot
    {
        Torso = 0,
        Arms = 1,
        Legs = 2,
        Feet = 3,
        Neck = 4,
        Waist = 5,
        Head = 6,
        Hands = 7, //CSRTODO: hands vs face
        Face = 8, //CSRTODO: hands vs face
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
    internal enum ItemInventoryOverflowAction
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

    internal enum SellableEnum
    {
        Unknown,
        Sellable,
        NotSellable,
        Junk,
    }

    internal enum AfterKillMonsterAction
    {
        ContinueCombat = 0,
        StopCombat = 1,
        SelectFirstMonsterInRoom = 2,
        SelectFirstMonsterInRoomOfSameType = 3,
    }

    internal enum SpellProficiency
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

    internal enum ClassType
    {
        Mage,
        Priest,
        Bard,
        Monk,
        Hunter,
        Rogue,
        Warrior,
    }
    internal enum ClassTypeFlags
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

    internal enum VertexSelectionRequirement
    {
        ValidPathFromCurrentLocation,
        UnambiguousRoomBackendOrDisplayName,
    }

    internal enum SpellsEnum
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

        restore,

        unknown,
    }
}
