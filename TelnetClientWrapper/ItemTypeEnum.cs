namespace IsengardClient
{
    /// <summary>
    /// item enums. There are three cases:
    /// 1. ordinary items have singular and plural names. These have both singular and plural attributes.
    /// 2. coin items use "X gold coins" and "sets of X gold coins" formats. These currently have both singular and plural attributes.
    /// 3. collective items only have a singular name, and use "sets of X" for the plural case. These currently only have a singular attribute.
    /// </summary>
    public enum ItemTypeEnum
    {
        [SingularName("adamantine scale mail armor")]
        [EquipmentType(EquipmentType.Torso)]
        AdamantineScaleMailArmor,

        [SingularName("adamantine scale mail gloves")]
        [EquipmentType(EquipmentType.Hands)]
        AdamantineScaleMailGloves,

        [SingularName("adamantine scale mail leggings")]
        [EquipmentType(EquipmentType.Legs)]
        AdamantineScaleMailLeggings,

        [SingularName("adamantine scale mail sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        AdamantineScaleMailSleeves,

        [SingularName("Ahrot's magic string")]
        [PluralName("Ahrot's magic strings")]
        AhrotsMagicString,

        [SingularName("amber scroll")]
        [PluralName("amber scrolls")]
        [Scroll(SpellsEnum.burn)]
        AmberScroll,

        [SingularName("ancient bag")]
        [PluralName("ancient bags")]
        AncientBag,

        [SingularName("ancient lyre")]
        [PluralName("ancient lyres")]
        AncientLyre,

        [SingularName("aquamarine potion")]
        [PluralName("aquamarine potions")]
        [Potion(SpellsEnum.levitate)]
        AquamarinePotion,

        [SingularName("banded mail armor")]
        [EquipmentType(EquipmentType.Torso)]
        BandedMailArmor,

        [SingularName("beastmaster's whip")]
        [PluralName("beastmaster's whips")]
        [WeaponType(WeaponType.Unknown)]
        BeastmastersWhip,

        [SingularName("bec de corbin")]
        //CSRTODO: plural?
        [WeaponType(WeaponType.Polearm)]
        BecDeCorbin,

        [SingularName("black cape")]
        [PluralName("black capes")]
        [EquipmentType(EquipmentType.Unknown)]
        BlackCape,

        [SingularName("blackened scroll")]
        [PluralName("blackened scrolls")]
        [Scroll(SpellsEnum.light)]
        BlackenedScroll,

        [SingularName("black scroll")]
        [PluralName("black scrolls")]
        [Scroll(SpellsEnum.hurt)]
        BlackScroll,

        [SingularName("black iron key")]
        [PluralName("black iron keys")]
        BlackIronKey,

        [SingularName("black vestments")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Torso)]
        BlackVestments,

        [SingularName("blue bubbly potion")]
        [PluralName("blue bubbly potions")]
        [Potion(SpellsEnum.detectinvis)]
        BlueBubblyPotion,

        [SingularName("boiler key")]
        [PluralName("boiler keys")]
        BoilerKey,

        [SingularName("bone armor")]
        [PluralName("bone armors")] //verified 6/21/23
        [EquipmentType(EquipmentType.Torso)]
        BoneArmor,

        [SingularName("bone shield")]
        [PluralName("bone shields")]
        [EquipmentType(EquipmentType.Shield)]
        BoneShield,

        [SingularName("book of knowledge")]
        [PluralName("books of knowledge")]
        BookOfKnowledge,

        [SingularName("boomerang")]
        [PluralName("boomerangs")]
        [WeaponType(WeaponType.Missile)]
        Boomerang,

        [SingularName("box of strawberries")]
        //CSRTODO: plural?
        BoxOfStrawberries,

        [SingularName("bracers of ogre-strength")]
        [EquipmentType(EquipmentType.Unknown)]
        BracersOfOgreStrength,

        [SingularName("broad sword")]
        [PluralName("broad swords")]
        [WeaponType(WeaponType.Slash)]
        BroadSword,

        [SingularName("bronze gauntlets")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Hands)]
        BronzeGauntlets,

        [SingularName("brown bag")]
        [PluralName("brown bags")]
        BrownBag,

        [SingularName("bucket")]
        [PluralName("buckets")]
        Bucket,

        [SingularName("bundle of wheat")]
        [PluralName("bundle of wheats")] //verified 6/14/2023
        BundleOfWheat,

        [SingularName("carved ivory key")]
        [PluralName("carved ivory keys")]
        CarvedIvoryKey,

        [SingularName("cat o' nine tails")]
        [PluralName("cat o' nine tailses")] //CSRTODO: correct plural?
        [WeaponType(WeaponType.Slash)]
        CatONineTails,

        [SingularName("chain mail armor")]
        [EquipmentType(EquipmentType.Torso)]
        ChainMailArmor,

        [SingularName("chain mail gloves")]
        [EquipmentType(EquipmentType.Hands)]
        ChainMailGloves,

        [SingularName("chain mail hood")]
        [PluralName("chain mail hoods")]
        [EquipmentType(EquipmentType.Head)]
        ChainMailHood,

        [SingularName("chain mail sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        ChainMailSleeves,

        [SingularName("cloth armor")]
        [EquipmentType(EquipmentType.Torso)]
        ClothArmor,

        [SingularName("cloth boots")]
        [EquipmentType(EquipmentType.Feet)]
        ClothBoots,

        [SingularName("cloth hat")]
        [PluralName("cloth hats")]
        [EquipmentType(EquipmentType.Head)]
        ClothHat,

        [SingularName("cloth pants")]
        [EquipmentType(EquipmentType.Legs)]
        ClothPants,

        [SingularName("club")]
        [PluralName("clubs")]
        [WeaponType(WeaponType.Blunt)]
        Club,

        [SingularName("copper pieces")]
        [PluralName("copper pieces")]
        CopperPieces,

        [SingularName("copper ring")]
        [PluralName("copper rings")]
        CopperRing,

        [SingularName("crossbow")]
        [PluralName("crossbows")]
        [WeaponType(WeaponType.Missile)]
        Crossbow,

        [SingularName("crystal amulet")]
        [PluralName("crystal amulets")]
        CrystalAmulet,

        [SingularName("cutlass")]
        [PluralName("cutlasses")]
        [WeaponType(WeaponType.Stab)]
        Cutlass,

        [SingularName("dagger")]
        [PluralName("daggers")]
        [WeaponType(WeaponType.Stab)]
        Dagger,

        [SingularName("dark blade")]
        [PluralName("dark blades")]
        [WeaponType(WeaponType.Slash)]
        DarkBlade,

        [SingularName("dark green potion")]
        [PluralName("dark green potions")]
        [Potion(SpellsEnum.detectmagic)]
        DarkGreenPotion,

        [SingularName("dark flask")]
        [PluralName("dark flasks")]
        [Potion(SpellsEnum.mend)]
        DarkFlask,

        [SingularName("dead rat carcass")]
        //CSRTODO: plural?
        DeadRatCarcass,

        [SingularName("Death's galvorn sickle")]
        [PluralName("Death's galvorn sickles")]
        [WeaponType(WeaponType.Unknown)]
        DeathsGalvornSickle,

        [SingularName("dildo")]
        [PluralName("dildos")]
        Dildo,

        [SingularName("dirk")]
        [PluralName("dirks")]
        [WeaponType(WeaponType.Unknown)]
        Dirk,

        [SingularName("double bladed axe")]
        [PluralName("double bladed axes")]
        [WeaponType(WeaponType.Slash)]
        DoubleBladedAxe,

        [SingularName("dried seaweed")]
        //CSRTODO: plural?
        DriedSeaweed,

        [SingularName("dungeon key")]
        [PluralName("dungeon keys")]
        DungeonKey,

        [SingularName("dwarven mithril gaiters")]
        [EquipmentType(EquipmentType.Unknown)]
        DwarvenMithrilGaiters,

        [SingularName("ear lobe plug")]
        [PluralName("ear lobe plugs")]
        [EquipmentType(EquipmentType.Unknown)]
        EarLobePlug,

        [SingularName("Eat At Denethore's decorative mug")]
        [PluralName("Eat At Denethore's decorative mugs")]
        EatAtDenethoresDecorativeMug,

        [SingularName("elven bow")]
        [PluralName("elven bows")]
        [WeaponType(WeaponType.Missile)]
        ElvenBow,

        [SingularName("elven cured leather gloves")]
        [EquipmentType(EquipmentType.Hands)]
        ElvenCuredLeatherGloves,

        [SingularName("elven cured leather hood")]
        [PluralName("elven cured leather hoods")]
        [EquipmentType(EquipmentType.Head)]
        ElvenCuredLeatherHood,

        [SingularName("elven leather whip")]
        [PluralName("elven leather whips")]
        [WeaponType(WeaponType.Unknown)]
        ElvenLeatherWhip,

        [SingularName("emerald")]
        [PluralName("emeralds")]
        Emerald,

        [SingularName("emerald collar")]
        [PluralName("emerald collars")]
        [EquipmentType(EquipmentType.Neck)]
        EmeraldCollar,

        [SingularName("engagement ring")]
        [PluralName("engagement rings")]
        [EquipmentType(EquipmentType.Finger)]
        EngagementRing,

        [SingularName("eye of newt")]
        //CSRTODO: plural?
        EyeOfNewt,

        [SingularName("furry sack")]
        [PluralName("furry sacks")]
        FurrySack,

        [SingularName("gaff")]
        [PluralName("gaffs")]
        [WeaponType(WeaponType.Unknown)]
        Gaff,

        [SingularName("galvorn ring")]
        [PluralName("galvorn rings")]
        [EquipmentType(EquipmentType.Finger)]
        GalvornRing,

        [SingularName("gate warning")]
        [PluralName("gate warnings")]
        GateWarning,

        [SingularName("gaudy scepter")]
        [PluralName("gaudy scepters")]
        [WeaponType(WeaponType.Polearm)]
        GaudyScepter,

        [SingularName("gawdy ear hoop")]
        [PluralName("gawdy ear hoops")]
        [EquipmentType(EquipmentType.Ears)]
        GawdyEarHoop,

        [SingularName("giant stylus")]
        [PluralName("giant styluses")] //CSRTODO: correct plural
        GiantStylus,

        [SingularName("Girion's key")]
        [PluralName("Girion's keys")]
        GirionsKey,

        [SingularName("glitter")]
        //CSRTODO: glitter (does not use some)
        Glitter,

        [SingularName("glowing pendant")]
        [PluralName("glowing pendants")]
        [EquipmentType(EquipmentType.Unknown)]
        GlowingPendant,

        [SingularName("glowing potion")]
        [PluralName("glowing potions")]
        [Potion(SpellsEnum.bless)]
        GlowingPotion,

        [SingularName("godentag")]
        [PluralName("godentags")]
        [WeaponType(WeaponType.Unknown)]
        Godentag,

        [SingularName("gold coins")]
        [PluralName("gold coins")]
        GoldCoins,

        [SingularName("golden dagger")]
        [PluralName("golden daggers")]
        [WeaponType(WeaponType.Stab)]
        GoldenDagger,

        [SingularName("golden mask of the gods")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Unknown)]
        GoldenMaskOfTheGods,

        [SingularName("grate key")]
        [PluralName("grate keys")]
        GrateKey,

        [SingularName("green potion")]
        [PluralName("green potions")]
        [Potion(SpellsEnum.curepoison)]
        GreenPotion,

        [SingularName("grey scroll")]
        [PluralName("grey scrolls")]
        [Scroll(SpellsEnum.vigor)]
        GreyScroll,

        [SingularName("gypsy cape")]
        [PluralName("gypsy capes")]
        [EquipmentType(EquipmentType.Unknown)]
        GypsyCape,

        [SingularName("gypsy crown")]
        [PluralName("gypsy crowns")]
        [EquipmentType(EquipmentType.Unknown)]
        GypsyCrown,

        [SingularName("halberd")]
        [PluralName("halberds")]
        [WeaponType(WeaponType.Polearm)]
        Halberd,

        [SingularName("hand axe")]
        [PluralName("hand axes")]
        [WeaponType(WeaponType.Slash)]
        HandAxe,

        [SingularName("hardwood shield")]
        [PluralName("hardwood shields")]
        [EquipmentType(EquipmentType.Shield)]
        HardwoodShield,

        [SingularName("hazy potion")]
        [PluralName("hazy potions")]
        [Potion(SpellsEnum.wordofrecall)]
        HazyPotion,

        [SingularName("head of lettuce")]
        //CSRTODO: plural?
        HeadOfLettuce,

        [SingularName("hood of the high priest")]
        //CSRTODO: plural
        [EquipmentType(EquipmentType.Head)]
        HoodOfTheHighPriest,

        [SingularName("human brain")]
        [PluralName("human brains")]
        HumanBrain,

        [SingularName("human carcass")]
        [PluralName("human carcasses")]
        HumanCarcass,

        [SingularName("ice blue potion")]
        [PluralName("ice blue potions")]
        [Potion(SpellsEnum.fly)]
        IceBluePotion,

        [SingularName("iron ring")]
        [PluralName("iron rings")]
        [EquipmentType(EquipmentType.Finger)]
        IronRing,

        [SingularName("iron spear")]
        [PluralName("iron spears")]
        [WeaponType(WeaponType.Polearm)]
        IronSpear,

        [SingularName("Kasnar's red key")]
        [PluralName("Kasnar's red keys")]
        KasnarsRedKey,

        [SingularName("kelp necklace")]
        [PluralName("kelp necklaces")]
        [EquipmentType(EquipmentType.Unknown)]
        KelpNecklace,

        [SingularName("key of the elements")]
        //CSRTODO: plural?
        KeyOfTheElements,

        [SingularName("khopesh sword")]
        [PluralName("khopesh swords")]
        [WeaponType(WeaponType.Unknown)]
        KhopeshSword,

        [SingularName("knapsack")]
        [PluralName("knapsacks")]
        Knapsack,

        [SingularName("lancette")]
        [PluralName("lancettes")]
        [WeaponType(WeaponType.Stab)]
        Lancette,

        [SingularName("large egg")]
        [PluralName("large eggs")]
        LargeEgg,

        [SingularName("large metal shield")]
        [PluralName("large metal shields")]
        [EquipmentType(EquipmentType.Shield)]
        LargeMetalShield,

        [SingularName("lead hammer")]
        [PluralName("lead hammers")]
        [WeaponType(WeaponType.Unknown)]
        LeadHammer,

        [SingularName("leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        LeatherArmor,

        [SingularName("leather boots")]
        [EquipmentType(EquipmentType.Feet)]
        LeatherBoots,

        [SingularName("leather cap")]
        [EquipmentType(EquipmentType.Unknown)]
        LeatherCap,

        [SingularName("leather gloves")]
        [EquipmentType(EquipmentType.Hands)]
        LeatherGloves,

        [SingularName("leather pouch")]
        [PluralName("leather pouches")]
        LeatherPouch,

        [SingularName("little brown jug")]
        [PluralName("little brown jugs")]
        LittleBrownJug,

        [SingularName("lollipop")]
        [PluralName("lollipops")]
        Lollipop,

        [SingularName("long bow")]
        [PluralName("long bows")]
        [WeaponType(WeaponType.Unknown)]
        LongBow,

        [SingularName("long sword")]
        [PluralName("long swords")]
        [WeaponType(WeaponType.Stab)]
        LongSword,

        [SingularName("lunch money")]
        //CSRTODO: plural?
        LunchMoney,

        [SingularName("magical tabulator")]
        [PluralName("magical tabulators")]
        MagicalTabulator,

        [SingularName("magical temper")]
        //CSRTODO: plural?
        MagicalTemper,

        [SingularName("marble chess set")]
        [PluralName("marble chess sets")]
        MarbleChessSet,

        [SingularName("mask of darkness")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Unknown)]
        MaskOfDarkness,

        [SingularName("metal helmet")]
        [PluralName("metal helmets")]
        [EquipmentType(EquipmentType.Head)]
        MetalHelmet,

        [SingularName("metal mask")]
        [PluralName("metal masks")]
        [EquipmentType(EquipmentType.Unknown)]
        MetalMask,

        [SingularName("mithril chain armor")]
        [EquipmentType(EquipmentType.Torso)]
        MithrilChainArmor,

        [SingularName("mithril jo stick")]
        [PluralName("mithril jo sticks")]
        [WeaponType(WeaponType.Unknown)]
        MithrilJoStick,

        [SingularName("mithril lamella leggings")]
        [EquipmentType(EquipmentType.Legs)]
        MithrilLamellaLeggings,

        [SingularName("mithron blade")]
        [PluralName("mithron blades")]
        [WeaponType(WeaponType.Unknown)]
        MithronBlade,

        [SingularName("mithron helmet")]
        [PluralName("mithron helmets")]
        [EquipmentType(EquipmentType.Head)]
        MithronHelmet,

        [SingularName("mithron hood")]
        [PluralName("mithron hoods")]
        [EquipmentType(EquipmentType.Head)]
        MithronHood,

        [SingularName("mithron shield")]
        [PluralName("mithron shields")]
        [EquipmentType(EquipmentType.Shield)]
        MithronShield,

        [SingularName("molten iron key")]
        [PluralName("molten iron keys")]
        MoltenIronKey,

        [SingularName("MOM tattoo")]
        [PluralName("MOM tattoos")]
        [EquipmentType(EquipmentType.Unknown)]
        MOMTattoo,

        [SingularName("morning star")]
        [PluralName("morning stars")]
        [WeaponType(WeaponType.Unknown)]
        MorningStar,

        [SingularName("nunchukus")]
        [WeaponType(WeaponType.Polearm)]
        //CSRTODO: plural
        Nunchukus,

        [SingularName("old wooden sign")]
        [PluralName("old wooden signs")]
        OldWoodenSign,

        [SingularName("onyx amulet")]
        [PluralName("onyx amulets")]
        [EquipmentType(EquipmentType.Neck)]
        OnyxAmulet,

        [SingularName("orc's sword")]
        [PluralName("orc's swords")]
        OrcsSword,

        [SingularName("ork blade")]
        [PluralName("ork blades")]
        [WeaponType(WeaponType.Unknown)]
        OrkBlade,

        [SingularName("out of order sign")]
        [PluralName("out of order signs")]
        OutOfOrderSign,

        [SingularName("parched scroll")]
        [PluralName("parched scrolls")]
        [Scroll(SpellsEnum.curepoison)]
        ParchedScroll,

        [SingularName("pearl encrusted diadem")]
        //CSRTODO: plural?
        PearlEncrustedDiadem,

        [SingularName("pearl handled knife")]
        [PluralName("pearl handled knives")]
        [WeaponType(WeaponType.Unknown)]
        PearlHandledKnife,

        [SingularName("petrified morning star")]
        [PluralName("petrified morning stars")]
        [WeaponType(WeaponType.Unknown)]
        PetrifiedMorningStar,

        [SingularName("pipe weed")]
        //CSRTODO: plural?
        PipeWeed,

        [SingularName("platinum pieces")]
        [PluralName("platinum pieces")]
        PlatinumPieces,

        [SingularName("port manifest")]
        [PluralName("port manifests")]
        PortManifest,

        [SingularName("pot helm")]
        [PluralName("pot helms")]
        [EquipmentType(EquipmentType.Head)]
        PotHelm,

        [SingularName("pot of gold")]
        [PluralName("pots of gold")]
        PotOfGold,

        [SingularName("pure white cape")]
        [PluralName("pure white capes")]
        [EquipmentType(EquipmentType.Unknown)]
        PureWhiteCape,

        [SingularName("purple wand")]
        [PluralName("purple wands")]
        PurpleWand,

        [SingularName("quarterstaff")]
        [PluralName("quarterstaffs")]
        [WeaponType(WeaponType.Polearm)]
        Quarterstaff,

        [SingularName("quartz stone")]
        [PluralName("quartz stones")]
        QuartzStone,

        [SingularName("rake")]
        [PluralName("rakes")]
        [WeaponType(WeaponType.Polearm)]
        Rake,

        [SingularName("rakshasan eviscerator")]
        [PluralName("rakshasan eviscerators")]
        RakshasanEviscerator,

        [SingularName("red bubbly potion")]
        [PluralName("red bubbly potions")]
        [Potion(SpellsEnum.invisibility)]
        RedBubblyPotion,

        [SingularName("reddish-orange potion")]
        [PluralName("reddish-orange potions")]
        [Potion(SpellsEnum.mend)]
        ReddishOrangePotion,

        [SingularName("red potion")]
        [PluralName("red potions")]
        [Potion(SpellsEnum.endurefire)]
        RedPotion,

        [SingularName("repair kit")]
        [PluralName("repair kits")]
        RepairKit,

        [SingularName("ribbed plate boots")]
        [EquipmentType(EquipmentType.Feet)]
        RibbedPlateBoots,

        [SingularName("ribbed plate hood")]
        [PluralName("ribbed plate hoods")]
        [EquipmentType(EquipmentType.Head)]
        RibbedPlateHood,

        [SingularName("ribbed plate shield")]
        [PluralName("ribbed plate shields")]
        [EquipmentType(EquipmentType.Shield)]
        RibbedPlateShield,

        [SingularName("ribbed plate sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        RibbedPlateSleeves,

        [SingularName("ring of invisibility")]
        [PluralName("rings of invisibility")]
        RingOfInvisibility,

        [SingularName("ruby")]
        [PluralName("rubys")] //verified 6/21/23
        Ruby,

        [SingularName("rusty key")]
        [PluralName("rusty key")]
        RustyKey,

        [SingularName("sack of potatoes")]
        [PluralName("sack of potatoeses")] //verified 6/14/2023
        SackOfPotatoes,

        [SingularName("sailor's locket")]
        [PluralName("sailor's lockets")]
        [EquipmentType(EquipmentType.Unknown)]
        SailorsLocket,

        [SingularName("sapphire")]
        [PluralName("sapphires")]
        Sapphire,

        [SingularName("scythe")]
        [PluralName("scythes")]
        Scythe,

        [SingularName("sextant")]
        [PluralName("sextants")]
        Sextant,

        [SingularName("sign")]
        [PluralName("signs")]
        Sign,

        [SingularName("signet ring")]
        [PluralName("signet rings")]
        [EquipmentType(EquipmentType.Finger)]
        SignetRing,

        [SingularName("silima blade")]
        [PluralName("silima blades")]
        [WeaponType(WeaponType.Slash)]
        SilimaBlade,

        [SingularName("silk vest")]
        [PluralName("silk vests")]
        [EquipmentType(EquipmentType.Torso)]
        SilkVest,

        [SingularName("silver arm-bands")]
        [EquipmentType(EquipmentType.Arms)]
        SilverArmBands,

        [SingularName("Silver-blue scale")]
        [PluralName("Silver-blue scales")]
        SilverBlueScale,

        [SingularName("silver dagger")]
        [PluralName("silver daggers")]
        [WeaponType(WeaponType.Unknown)]
        SilverDagger,

        [SingularName("silver scimitar")]
        [PluralName("silver scimitars")]
        [WeaponType(WeaponType.Slash)]
        SilverScimitar,

        [SingularName("silver wand")]
        [PluralName("silver wands")]
        SilverWand,

        [SingularName("slaying sword")]
        [PluralName("slaying swords")]
        [WeaponType(WeaponType.Stab)]
        SlayingSword,

        [SingularName("sling")]
        [PluralName("slings")]
        [WeaponType(WeaponType.Missile)]
        Sling,

        [SingularName("small ash bow")]
        [PluralName("small ash bows")]
        [WeaponType(WeaponType.Missile)]
        SmallAshBow,

        [SingularName("small bag")]
        [PluralName("small bags")]
        SmallBag,

        [SingularName("small knife")]
        [PluralName("small knifes")] //verified 6/21/23
        [WeaponType(WeaponType.Stab)]
        SmallKnife,

        [SingularName("small metal shield")]
        [PluralName("small metal shields")]
        [EquipmentType(EquipmentType.Shield)]
        SmallMetalShield,

        [SingularName("small pearl")]
        [PluralName("small pearls")]
        SmallPearl,

        [SingularName("small silver chest")]
        [PluralName("small silver chests")]
        SmallSilverChest,

        [SingularName("small wooden shield")]
        [PluralName("small wooden shields")]
        [EquipmentType(EquipmentType.Shield)]
        SmallWoodenShield,

        [SingularName("speckled potion")]
        [PluralName("speckled potions")]
        [Potion(SpellsEnum.stun)]
        SpeckledPotion,

        [SingularName("sprite boots")]
        [EquipmentType(EquipmentType.Feet)]
        SpriteBoots,

        [SingularName("sprite leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        SpriteLeatherArmor,

        [SingularName("sprite leather boots")]
        [EquipmentType(EquipmentType.Feet)]
        SpriteLeatherBoots,

        [SingularName("sprite leather leggings")]
        [EquipmentType(EquipmentType.Legs)]
        SpriteLeatherLeggings,

        [SingularName("splint mail")]
        [EquipmentType(EquipmentType.Unknown)]
        SplintMail,

        [SingularName("sprite bracelet")]
        [PluralName("sprite bracelets")]
        [EquipmentType(EquipmentType.Unknown)]
        SpriteBracelet,

        [SingularName("spyglass")]
        [PluralName("spyglasses")]
        Spyglass,

        [SingularName("statuette of Balthazar")]
        [PluralName("statuettes of Balthazar")]
        StatuetteOfBalthazar,

        [SingularName("steel-chain armor")]
        [EquipmentType(EquipmentType.Unknown)]
        SteelChainArmor,

        [SingularName("stilleto")]
        //CSRTODO: plural?
        [WeaponType(WeaponType.Unknown)]
        Stilleto,

        [SingularName("stone hammer")]
        [PluralName("stone hammers")]
        [WeaponType(WeaponType.Unknown)]
        StoneHammer,

        [SingularName("stone key")]
        [PluralName("stone keys")]
        StoneKey,

        [SingularName("sundorian tassle")]
        [PluralName("sundorian tassles")]
        [EquipmentType(EquipmentType.Unknown)]
        SundorianTassle,

        [SingularName("swirly potion")]
        [PluralName("swirly potions")]
        [Potion(SpellsEnum.protection)]
        SwirlyPotion,

        [SingularName("tattoo of a snake")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Unknown)]
        TattooOfASnake,

        [SingularName("tattoo of a wench")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Unknown)]
        TattooOfAWench,

        [SingularName("taupe scroll")]
        [PluralName("taupe scrolls")]
        [Scroll(SpellsEnum.shatterstone)]
        TaupeScroll,

        [SingularName("T-bone")]
        [PluralName("T-bones")]
        TBone,

        [SingularName("throwing axe")]
        [PluralName("throwing axes")]
        [WeaponType(WeaponType.Missile)]
        ThrowingAxe,

        [SingularName("tiger shark leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        TigerSharkLeatherArmor,

        [SingularName("toilet paper")]
        //CSRTODO: collective plural?
        ToiletPaper,

        [SingularName("torch")]
        [PluralName("torchs")] //verified 6/21/23
        Torch,

        [SingularName("training key")]
        [PluralName("training keys")]
        TrainingKey,

        [SingularName("translucent armor")]
        [EquipmentType(EquipmentType.Torso)]
        TranslucentArmor,

        [SingularName("translucent leggings")]
        [EquipmentType(EquipmentType.Legs)]
        TranslucentLeggings,

        [SingularName("translucent sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        TranslucentSleeves,

        [SingularName("verdant green scroll")]
        [PluralName("verdant green scrolls")]
        VerdantGreenScroll,

        [SingularName("viscous potion")]
        [PluralName("viscous potions")]
        //CSRTODO: what is this?
        ViscousPotion,

        [SingularName("volcanic boots")]
        [EquipmentType(EquipmentType.Feet)]
        VolcanicBoots,

        [SingularName("volcanic gauntlets")]
        [EquipmentType(EquipmentType.Unknown)]
        VolcanicGauntlets,

        [SingularName("volcanic shield")]
        [PluralName("volcanic shields")]
        [EquipmentType(EquipmentType.Shield)]
        VolcanicShield,

        [SingularName("voulge")]
        [PluralName("voulges")]
        [WeaponType(WeaponType.Unknown)]
        Voulge,

        [SingularName("wagonmaster's whip")]
        [PluralName("wagonmaster's whips")]
        [WeaponType(WeaponType.Unknown)]
        WagonmastersWhip,

        [SingularName("warhammer")]
        [PluralName("warhammers")]
        [WeaponType(WeaponType.Unknown)]
        Warhammer,

        [SingularName("war harness")]
        [PluralName("war harnesses")]
        [EquipmentType(EquipmentType.Waist)]
        WarHarness,

        [SingularName("War's flaming axe")]
        [PluralName("War's flaming axes")]
        [WeaponType(WeaponType.Unknown)]
        WarsFlamingAxe,

        [SingularName("welcome sign")]
        [PluralName("welcome signs")]
        WelcomeSign,

        [SingularName("yellow beholder's eye")]
        [PluralName("yellow beholder's eyes")]
        YellowBeholdersEye,

        [SingularName("yellow potion")]
        [PluralName("yellow potions")]
        [Potion(SpellsEnum.vigor)]
        YellowPotion,
    }
}
