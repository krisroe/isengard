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
        AdamantineScaleMailArmor,

        [SingularName("adamantine scale mail gloves")]
        AdamantineScaleMailGloves,

        [SingularName("adamantine scale mail leggings")]
        AdamantineScaleMailLeggings,

        [SingularName("adamantine scale mail sleeves")]
        AdamantineScaleMailSleeves,

        [SingularName("Ahrot's magic string")]
        [PluralName("Ahrot's magic strings")]
        AhrotsMagicString,

        [SingularName("amber scroll")]
        [PluralName("amber scrolls")]
        AmberScroll,

        [SingularName("ancient bag")]
        [PluralName("ancient bags")]
        AncientBag,

        [SingularName("ancient lyre")]
        [PluralName("ancient lyres")]
        AncientLyre,

        [SingularName("aquamarine potion")]
        [PluralName("aquamarine potions")]
        AquamarinePotion,

        [SingularName("banded mail armor")]
        BandedMailArmor,

        [SingularName("beastmaster's whip")]
        [PluralName("beastmaster's whips")]
        BeastmastersWhip,

        [SingularName("black cape")]
        [PluralName("black capes")]
        BlackCape,

        [SingularName("blackened scroll")]
        [PluralName("blackened scrolls")]
        BlackenedScroll,

        [SingularName("black scroll")]
        [PluralName("black scrolls")]
        BlackScroll,

        [SingularName("black iron key")]
        [PluralName("black iron keys")]
        BlackIronKey,

        [SingularName("black vestments")]
        //CSRTODO: plural?
        BlackVestments,

        [SingularName("blue bubbly potion")]
        [PluralName("blue bubbly potions")]
        BlueBubblyPotion,

        [SingularName("boiler key")]
        [PluralName("boiler keys")]
        BoilerKey,

        [SingularName("bone armor")]
        BoneArmor,

        [SingularName("bone shield")]
        [PluralName("bone shields")]
        BoneShield,

        [SingularName("book of knowledge")]
        [PluralName("books of knowledge")]
        BookOfKnowledge,

        [SingularName("box of strawberries")]
        //CSRTODO: plural?
        BoxOfStrawberries,

        [SingularName("bracers of ogre-strength")]
        BracersOfOgreStrength,

        [SingularName("broad sword")]
        [PluralName("broad swords")]
        BroadSword,

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
        CatONineTails,

        [SingularName("chain mail armor")]
        ChainMailArmor,

        [SingularName("chain mail gloves")]
        ChainMailGloves,

        [SingularName("chain mail sleeves")]
        ChainMailSleeves,

        [SingularName("cloth armor")]
        ClothArmor,

        [SingularName("cloth boots")]
        ClothBoots,

        [SingularName("cloth hat")]
        [PluralName("cloth hats")]
        ClothHat,

        [SingularName("cloth pants")]
        ClothPants,

        [SingularName("club")]
        [PluralName("clubs")]
        Club,

        [SingularName("copper pieces")]
        [PluralName("copper pieces")]
        CopperPieces,

        [SingularName("copper ring")]
        [PluralName("copper rings")]
        CopperRing,

        [SingularName("crossbow")]
        [PluralName("crossbows")]
        Crossbow,

        [SingularName("crystal amulet")]
        [PluralName("crystal amulets")]
        CrystalAmulet,

        [SingularName("dagger")]
        [PluralName("daggers")]
        Dagger,

        [SingularName("dark blade")]
        [PluralName("dark blades")]
        DarkBlade,

        [SingularName("dark green potion")]
        [PluralName("dark green potions")]
        DarkGreenPotion,

        [SingularName("dead rat carcass")]
        //CSRTODO: plural?
        DeadRatCarcass,

        [SingularName("Death's galvorn sickle")]
        [PluralName("Death's galvorn sickles")]
        DeathsGalvornSickle,

        [SingularName("dildo")]
        [PluralName("dildos")]
        Dildo,

        [SingularName("dirk")]
        [PluralName("dirks")]
        Dirk,

        [SingularName("dried seaweed")]
        //CSRTODO: plural?
        DriedSeaweed,

        [SingularName("dwarven mithril gaiters")]
        DwarvenMithrilGaiters,

        [SingularName("ear lobe plug")]
        [PluralName("ear lobe plugs")]
        EarLobePlug,

        [SingularName("Eat At Denethore's decorative mug")]
        [PluralName("Eat At Denethore's decorative mugs")]
        EatAtDenethoresDecorativeMug,

        [SingularName("elven bow")]
        [PluralName("elven bows")]
        ElvenBow,

        [SingularName("elven cured leather gloves")]
        ElvenCuredLeatherGloves,

        [SingularName("elven leather whip")]
        [PluralName("elven leather whips")]
        ElvenLeatherWhip,

        [SingularName("emerald")]
        [PluralName("emeralds")]
        Emerald,

        [SingularName("emerald collar")]
        [PluralName("emerald collars")]
        EmeraldCollar,

        [SingularName("engagement ring")]
        [PluralName("engagement rings")]
        EngagementRing,

        [SingularName("eye of newt")]
        //CSRTODO: plural?
        EyeOfNewt,

        [SingularName("furry sack")]
        [PluralName("furry sacks")]
        FurrySack,

        [SingularName("gaff")]
        [PluralName("gaffs")]
        Gaff,

        [SingularName("galvorn ring")]
        [PluralName("galvorn rings")]
        GalvornRing,

        [SingularName("gate warning")]
        [PluralName("gate warnings")]
        GateWarning,

        [SingularName("gaudy scepter")]
        [PluralName("gaudy scepters")]
        GaudyScepter,

        [SingularName("gawdy ear hoop")]
        [PluralName("gawdy ear hoops")]
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
        GlowingPendant,

        [SingularName("godentag")]
        [PluralName("godentags")]
        Godentag,

        [SingularName("gold coins")]
        [PluralName("gold coins")]
        GoldCoins,

        [SingularName("golden dagger")]
        [PluralName("golden daggers")]
        GoldenDagger,

        [SingularName("golden mask of the gods")]
        //CSRTODO: plural?
        GoldenMaskOfTheGods,

        [SingularName("green potion")]
        [PluralName("green potions")]
        GreenPotion,

        [SingularName("grey scroll")]
        [PluralName("grey scrolls")]
        GreyScroll,

        [SingularName("gypsy cape")]
        [PluralName("gypsy capes")]
        GypsyCape,

        [SingularName("halberd")]
        [PluralName("halberds")]
        Halberd,

        [SingularName("hand axe")]
        [PluralName("hand axes")]
        HandAxe,

        [SingularName("hardwood shield")]
        [PluralName("hardwood shields")]
        HardwoodShield,

        [SingularName("hazy potion")]
        [PluralName("hazy potions")]
        HazyPotion,

        [SingularName("head of lettuce")]
        //CSRTODO: plural?
        HeadOfLettuce,

        [SingularName("hood of the high priest")]
        //CSRTODO: plural
        HoodOfTheHighPriest,

        [SingularName("ice blue potion")]
        [PluralName("ice blue potions")]
        IceBluePotion,

        [SingularName("iron ring")]
        [PluralName("iron rings")]
        IronRing,

        [SingularName("Kasnar's red key")]
        [PluralName("Kasnar's red keys")]
        KasnarsRedKey,

        [SingularName("kelp necklace")]
        [PluralName("kelp necklaces")]
        KelpNecklace,

        [SingularName("khopesh sword")]
        [PluralName("khopesh swords")]
        KhopeshSword,

        [SingularName("knapsack")]
        [PluralName("knapsacks")]
        Knapsack,

        [SingularName("large egg")]
        [PluralName("large eggs")]
        LargeEgg,

        [SingularName("large metal shield")]
        [PluralName("large metal shields")]
        LargeMetalShield,

        [SingularName("lead hammer")]
        [PluralName("lead hammers")]
        LeadHammer,

        [SingularName("leather armor")]
        LeatherArmor,

        [SingularName("leather gloves")]
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
        LongBow,

        [SingularName("lunch money")]
        //CSRTODO: plural?
        LunchMoney,

        [SingularName("magical tabulator")]
        [PluralName("magical tabulators")]
        MagicalTabulator,

        [SingularName("marble chess set")]
        [PluralName("marble chess sets")]
        MarbleChessSet,

        [SingularName("mask of darkness")]
        //CSRTODO: plural?
        MaskOfDarkness,

        [SingularName("metal helmet")]
        [PluralName("metal helmets")]
        MetalHelmet,

        [SingularName("metal mask")]
        [PluralName("metal masks")]
        MetalMask,

        [SingularName("mithril chain armor")]
        MithrilChainArmor,

        [SingularName("mithril jo stick")]
        [PluralName("mithril jo sticks")]
        MithrilJoStick,

        [SingularName("mithril lamella leggings")]
        MithrilLamellaLeggings,

        [SingularName("mithron blade")]
        [PluralName("mithron blades")]
        MithronBlade,

        [SingularName("mithron helmet")]
        [PluralName("mithron helmets")]
        MithronHelmet,

        [SingularName("mithron hood")]
        [PluralName("mithron hoods")]
        MithronHood,

        [SingularName("mithron shield")]
        [PluralName("mithron shields")]
        MithronShield,

        [SingularName("molten iron key")]
        [PluralName("molten iron keys")]
        MoltenIronKey,

        [SingularName("MOM tattoo")]
        [PluralName("MOM tattoos")]
        MOMTattoo,

        [SingularName("morning star")]
        [PluralName("morning stars")]
        MorningStar,

        [SingularName("old wooden sign")]
        [PluralName("old wooden signs")]
        OldWoodenSign,

        [SingularName("onyx amulet")]
        [PluralName("onyx amulets")]
        OnyxAmulet,

        [SingularName("orc's sword")]
        [PluralName("orc's swords")]
        OrcsSword,

        [SingularName("ork blade")]
        [PluralName("ork blades")]
        OrkBlade,

        [SingularName("out of order sign")]
        [PluralName("out of order signs")]
        OutOfOrderSign,

        [SingularName("parched scroll")]
        [PluralName("parched scrolls")]
        ParchedScroll,

        [SingularName("pearl encrusted diadem")]
        //CSRTODO: plural?
        PearlEncrustedDiadem,

        [SingularName("pearl handled knife")]
        [PluralName("pearl handled knives")]
        PearlHandledKnife,

        [SingularName("petrified morning star")]
        [PluralName("petrified morning stars")]
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
        PotHelm,

        [SingularName("pot of gold")]
        [PluralName("pots of gold")]
        PotOfGold,

        [SingularName("pure white cape")]
        [PluralName("pure white capes")]
        PureWhiteCape,

        [SingularName("purple wand")]
        [PluralName("purple wands")]
        PurpleWand,

        [SingularName("quarterstaff")]
        [PluralName("quarterstaffs")]
        Quarterstaff,

        [SingularName("quartz stone")]
        [PluralName("quartz stones")]
        QuartzStone,

        [SingularName("rakshasan eviscerator")]
        [PluralName("rakshasan eviscerators")]
        RakshasanEviscerator,

        [SingularName("red bubbly potion")]
        [PluralName("red bubbly potions")]
        RedBubblyPotion,

        [SingularName("reddish-orange potion")]
        [PluralName("reddish-orange potions")]
        ReddishOrangePotion,

        [SingularName("red potion")]
        [PluralName("red potions")]
        RedPotion,

        [SingularName("repair kit")]
        [PluralName("repair kits")]
        RepairKit,

        [SingularName("ribbed plate boots")]
        RibbedPlateBoots,

        [SingularName("ribbed plate hood")]
        [PluralName("ribbed plate hoods")]
        RibbedPlateHood,

        [SingularName("ribbed plate shield")]
        [PluralName("ribbed plate shields")]
        RibbedPlateShield,

        [SingularName("ribbed plate sleeves")]
        RibbedPlateSleeves,

        [SingularName("ring of invisibility")]
        [PluralName("rings of invisibility")]
        RingOfInvisibility,

        [SingularName("rusty key")]
        [PluralName("rusty key")]
        RustyKey,

        [SingularName("sack of potatoes")]
        [PluralName("sack of potatoeses")] //verified 6/14/2023
        SackOfPotatoes,

        [SingularName("sailor's locket")]
        [PluralName("sailor's lockets")]
        SailorsLocket,

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
        SignetRing,

        [SingularName("silima blade")]
        [PluralName("silima blades")]
        SilimaBlade,

        [SingularName("silver arm-bands")]
        SilverArmBands,

        [SingularName("Silver-blue scale")]
        [PluralName("Silver-blue scales")]
        SilverBlueScale,

        [SingularName("silver dagger")]
        [PluralName("silver daggers")]
        SilverDagger,

        [SingularName("silver scimitar")]
        [PluralName("silver scimitars")]
        SilverScimitar,

        [SingularName("silver wand")]
        [PluralName("silver wands")]
        SilverWand,

        [SingularName("slaying sword")]
        [PluralName("slaying swords")]
        SlayingSword,

        [SingularName("sling")]
        [PluralName("slings")]
        Sling,

        [SingularName("small ash bow")]
        [PluralName("small ash bows")]
        SmallAshBow,

        [SingularName("small bag")]
        [PluralName("small bags")]
        SmallBag,

        [SingularName("small knife")]
        [PluralName("small knives")] //CSRTODO: correct plural?
        SmallKnife,

        [SingularName("small metal shield")]
        [PluralName("small metal shields")]
        SmallMetalShield,

        [SingularName("small pearl")]
        [PluralName("small pearls")]
        SmallPearl,

        [SingularName("small silver chest")]
        [PluralName("small silver chests")]
        SmallSilverChest,

        [SingularName("small wooden shield")]
        [PluralName("small wooden shields")]
        SmallWoodenShield,

        [SingularName("speckled potion")]
        [PluralName("speckled potions")]
        SpeckledPotion,

        [SingularName("sprite boots")]
        SpriteBoots,

        [SingularName("sprite leather armor")]
        SpriteLeatherArmor,

        [SingularName("splint mail")]
        SplintMail,

        [SingularName("sprite bracelet")]
        [PluralName("sprite bracelets")]
        SpriteBracelet,

        [SingularName("spyglass")]
        [PluralName("spyglasses")]
        Spyglass,

        [SingularName("statuette of Balthazar")]
        [PluralName("statuettes of Balthazar")]
        StatuetteOfBalthazar,

        [SingularName("steel-chain armor")]
        SteelChainArmor,

        [SingularName("stilleto")]
        //CSRTODO: plural?
        Stilleto,

        [SingularName("stone hammer")]
        [PluralName("stone hammers")]
        StoneHammer,

        [SingularName("stone key")]
        [PluralName("stone keys")]
        StoneKey,

        [SingularName("sundorian tassle")]
        [PluralName("sundorian tassles")]
        SundorianTassle,

        [SingularName("tattoo of a snake")]
        //CSRTODO: plural?
        TattooOfASnake,

        [SingularName("tattoo of a wench")]
        //CSRTODO: plural?
        TattooOfAWench,

        [SingularName("taupe scroll")]
        [PluralName("taupe scrolls")]
        TaupeScroll,

        [SingularName("T-bone")]
        [PluralName("T-bones")]
        TBone,

        [SingularName("throwing axe")]
        [PluralName("throwing axes")]
        ThrowingAxe,

        [SingularName("tiger shark leather armor")]
        TigerSharkLeatherArmor,

        [SingularName("toilet paper")]
        //CSRTODO: collective plural?
        ToiletPaper,

        [SingularName("torch")]
        [PluralName("torches")]
        Torch,

        [SingularName("training key")]
        [PluralName("training keys")]
        TrainingKey,

        [SingularName("translucent armor")]
        TranslucentArmor,

        [SingularName("translucent leggings")]
        TranslucentLeggings,

        [SingularName("translucent sleeves")]
        TranslucentSleeves,

        [SingularName("verdant green scroll")]
        [PluralName("verdant green scrolls")]
        VerdantGreenScroll,

        [SingularName("viscous potion")]
        [PluralName("viscous potions")]
        ViscousPotion,

        [SingularName("volcanic boots")]
        VolcanicBoots,

        [SingularName("volcanic gauntlets")]
        VolcanicGauntlets,

        [SingularName("volcanic shield")]
        [PluralName("volcanic shields")]
        VolcanicShield,

        [SingularName("voulge")]
        [PluralName("voulges")]
        Voulge,

        [SingularName("wagonmaster's whip")]
        [PluralName("wagonmaster's whips")]
        WagonmastersWhip,

        [SingularName("warhammer")]
        [PluralName("warhammers")]
        Warhammer,

        [SingularName("war harness")]
        [PluralName("war harnesses")]
        WarHarness,

        [SingularName("War's flaming axe")]
        [PluralName("War's flaming axes")]
        WarsFlamingAxe,

        [SingularName("welcome sign")]
        [PluralName("welcome signs")]
        WelcomeSign,

        [SingularName("yellow beholder's eye")]
        [PluralName("yellow beholder's eyes")]
        YellowBeholdersEye,

        [SingularName("yellow potion")]
        [PluralName("yellow potions")]
        YellowPotion,
    }
}
