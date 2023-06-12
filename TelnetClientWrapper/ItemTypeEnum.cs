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
        [SingularName("adamantine scale mail gloves")]
        AdamantineScaleMailGloves,

        [SingularName("adamantine scale mail leggings")]
        AdamantineScaleMailLeggings,

        [SingularName("adamantine scale mail sleeves")]
        AdamantineScaleMailSleeves,

        [SingularName("amber scroll")]
        [PluralName("amber scrolls")]
        AmberScroll,

        [SingularName("ancient lyre")]
        [PluralName("ancient lyres")]
        AncientLyre,

        [SingularName("aquamarine potion")]
        [PluralName("aquamarine potions")]
        AquamarinePotion,

        [SingularName("beastmaster's whip")]
        [PluralName("beastmaster's whips")]
        BeastmastersWhip,

        [SingularName("blackened scroll")]
        [PluralName("blackened scrolls")]
        BlackenedScroll,

        [SingularName("black scroll")]
        [PluralName("black scrolls")]
        BlackScroll,

        [SingularName("black iron key")]
        [PluralName("black iron keys")]
        BlackIronKey,

        [SingularName("blue bubbly potion")]
        [PluralName("blue bubbly potions")]
        BlueBubblyPotion,

        [SingularName("bone armor")]
        BoneArmor,

        [SingularName("book of knowledge")]
        [PluralName("books of knowledge")]
        BookOfKnowledge,

        [SingularName("bracers of ogre-strength")]
        BracersOfOgreStrength,

        [SingularName("broad sword")]
        [PluralName("broad swords")]
        BroadSword,

        [SingularName("carved ivory key")]
        [PluralName("carved ivory keys")]
        CarvedIvoryKey,

        [SingularName("cat o' nine tails")]
        [PluralName("cat o' nine tailses")] //CSRTODO: correct plural?
        CatONineTails,

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

        [SingularName("dagger")]
        [PluralName("daggers")]
        Dagger,

        [SingularName("dark blade")]
        [PluralName("dark blades")]
        DarkBlade,

        [SingularName("dark green potion")]
        [PluralName("dark green potions")]
        DarkGreenPotion,

        [SingularName("dildo")]
        //CSRTODO: dildo or dildoes?
        Dildo,

        [SingularName("elven bow")]
        [PluralName("elven bows")]
        ElvenBow,

        [SingularName("emerald collar")]
        [PluralName("emerald collars")]
        EmeraldCollar,

        [SingularName("engagement ring")]
        [PluralName("engagement rings")]
        EngagementRing,

        [SingularName("galvorn ring")]
        [PluralName("galvorn rings")]
        GalvornRing,

        [SingularName("gate warning")]
        [PluralName("gate warnings")]
        GateWarning,

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

        [SingularName("godentag")]
        [PluralName("godentags")]
        Godentag,

        [SingularName("gold coins")]
        [PluralName("gold coins")]
        GoldCoins,

        [SingularName("green potion")]
        [PluralName("green potions")]
        GreenPotion,

        [SingularName("grey scroll")]
        [PluralName("grey scrolls")]
        GreyScroll,

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

        [SingularName("ice blue potion")]
        [PluralName("ice blue potions")]
        IceBluePotion,

        [SingularName("iron ring")]
        [PluralName("iron rings")]
        IronRing,

        [SingularName("lead hammer")]
        [PluralName("lead hammers")]
        LeadHammer,

        [SingularName("leather armor")]
        LeatherArmor,

        [SingularName("leather gloves")]
        LeatherGloves,

        [SingularName("little brown jug")]
        [PluralName("little brown jugs")]
        LittleBrownJug,

        [SingularName("lollipop")]
        [PluralName("lollipops")]
        Lollipop,

        [SingularName("long bow")]
        [PluralName("long bows")]
        LongBow,

        [SingularName("magical tabulator")]
        [PluralName("magical tabulators")]
        MagicalTabulator,

        [SingularName("marble chess set")]
        [PluralName("marble chess sets")]
        MarbleChessSet,

        [SingularName("metal mask")]
        [PluralName("metal masks")]
        MetalMask,

        [SingularName("mithril jo stick")]
        [PluralName("mithril jo sticks")]
        MithrilJoStick,

        [SingularName("mithron helmet")]
        [PluralName("mithron helmets")]
        MithronHelmet,

        [SingularName("mithron shield")]
        [PluralName("mithron shields")]
        MithronShield,

        [SingularName("MOM tattoo")]
        [PluralName("MOM tattoos")]
        MOMTattoo,

        [SingularName("old wooden sign")]
        [PluralName("old wooden signs")]
        OldWoodenSign,

        [SingularName("onyx amulet")]
        [PluralName("onyx amulets")]
        OnyxAmulet,

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

        [SingularName("pipe weed")]
        //CSRTODO: plural?
        PipeWeed,

        [SingularName("pot of gold")]
        [PluralName("pots of gold")]
        PotOfGold,

        [SingularName("purple wand")]
        [PluralName("purple wands")]
        PurpleWand,

        [SingularName("quarterstaff")]
        [PluralName("quarterstaffs")]
        Quarterstaff,

        [SingularName("quartz stone")]
        [PluralName("quartz stones")]
        QuartzStone,

        [SingularName("repair kit")]
        [PluralName("repair kits")]
        RepairKit,

        [SingularName("ring of invisibility")]
        [PluralName("rings of invisibility")]
        RingOfInvisibility,

        [SingularName("rusty key")]
        [PluralName("rusty key")]
        RustyKey,

        [SingularName("sailor's locket")]
        [PluralName("sailor's lockets")]
        SailorsLocket,

        [SingularName("signet ring")]
        [PluralName("signet rings")]
        SignetRing,

        [SingularName("Silver-blue scale")]
        [PluralName("Silver-blue scales")]
        SilverBlueScale,

        [SingularName("silver scimitar")]
        [PluralName("silver scimitars")]
        SilverScimitar,

        [SingularName("silver wand")]
        [PluralName("silver wands")]
        SilverWand,

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

        [SingularName("small silver chest")]
        [PluralName("small silver chests")]
        SmallSilverChest,

        [SingularName("small wooden shield")]
        [PluralName("small wooden shields")]
        SmallWoodenShield,

        [SingularName("sprite boots")]
        SpriteBoots,

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

        [SingularName("taupe scroll")]
        [PluralName("taupe scrolls")]
        TaupeScroll,

        [SingularName("T-bone")]
        [PluralName("T-bones")]
        TBone,

        [SingularName("tiger shark leather armor")]
        TigerSharkLeatherArmor,

        [SingularName("training key")]
        [PluralName("training keys")]
        TrainingKey,

        [SingularName("translucent leggings")]
        TranslucentLeggings,

        [SingularName("translucent sleeves")]
        TranslucentSleeves,

        [SingularName("verdant green scroll")]
        [PluralName("verdant green scrolls")]
        VerdantGreenScroll,

        [SingularName("voulge")]
        [PluralName("voulges")]
        Voulge,

        [SingularName("wagonmaster's whip")]
        [PluralName("wagonmaster's whips")]
        WagonmastersWhip,

        [SingularName("war harness")]
        [PluralName("war harnesses")]
        WarHarness,

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
