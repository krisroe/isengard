namespace IsengardClient
{
    internal enum MobTypeEnum
    {
        [SingularName("Alasse")]
        //CSRTODO: no plural
        Alasse,

        [SingularName("alley cat")]
        [PluralName("alley cats")]
        AlleyCat,

        [SingularName("alligator")]
        [PluralName("alligators")]
        Alligator,

        [SingularName("Amlug")]
        //CSRTODO: no plural
        Amlug,

        [SingularName("Amme")]
        //CSRTODO: no plural
        Amme,

        [SingularName("archer")]
        [PluralName("archers")]
        Archer,

        [SingularName("aristocrat")]
        [PluralName("aristocrats")]
        Aristocrat,

        [SingularName("art student")]
        [PluralName("art students")]
        ArtStudent,

        [SingularName("Aurelius the Scholar")]
        //CSRTODO: no plural
        AureliusTheScholar,

        [SingularName("Azgara")]
        //CSRTODO: no plural
        Azgara,

        [SingularName("bandit")]
        [PluralName("bandits")]
        Bandit,

        [SingularName("barbarian")]
        [PluralName("barbarians")]
        Barbarian,

        [SingularName("barbarian guard")]
        [PluralName("barbarian guards")]
        BarbarianGuard,

        [SingularName("barmaid")]
        [PluralName("barmaids")]
        Barmaid,

        [SingularName("barrow wight")]
        [PluralName("barrow wights")]
        BarrowWight,

        [SingularName("bartender")]
        [PluralName("bartenders")]
        [Experience(15)]
        Bartender,

        [SingularName("bear")]
        [PluralName("bears")]
        Bear,

        [SingularName("Bezanthi")]
        //CSRTODO: no plural
        Bezanthi,

        [SingularName("Big Papa")]
        //CSRTODO: no plural?
        [Experience(350)]
        [Alignment(AlignmentType.Blue)]
        BigPapa,

        [SingularName("Bilbo Baggins")]
        //CSRTODO: no plural
        [Experience(260)]
        [Alignment(AlignmentType.Blue)]
        BilboBaggins,

        [SingularName("blade")]
        [PluralName("blades")]
        Blade,

        [SingularName("bluejacket")]
        [PluralName("bluejackets")]
        Bluejacket,

        [SingularName("black cat")]
        [PluralName("black cats")]
        BlackCat,

        [SingularName("black pegasus")]
        //CSRTODO: plural?
        BlackPegasus,

        [SingularName("blind crone")]
        [PluralName("blind crones")]
        BlindCrone,

        [SingularName("blue flying snake")]
        [PluralName("blue flying snakes")]
        BlueFlyingSnake,

        [SingularName("boatswain")]
        [PluralName("boatswains")]
        [Experience(350)]
        Boatswain,

        [SingularName("bosun's mate")]
        [PluralName("bosun's mates")]
        BosunsMate,

        [SingularName("bowyer")]
        [PluralName("bowyers")]
        Bowyer,

        [SingularName("Branco the hobbits' chief")]
        //CSRTODO: no plural
        BrancoTheHobbitsChief,

        [SingularName("Brent Diehard")]
        //CSRTODO: no plural?
        BrentDiehard,

        [SingularName("bride named Princess")]
        //CSRTODO: no plural
        BrideNamedPrincess,

        [SingularName("bugbear")]
        [PluralName("bugbears")]
        Bugbear,

        [SingularName("bull")]
        [PluralName("bulls")]
        Bull,

        [SingularName("The burned remains of Nimrodel")]
        //CSRTODO: no plural.
        [Experience(300)]
        BurnedRemainsOfNimrodel,

        [SingularName("butler")]
        [PluralName("butlers")]
        Butler,

        [SingularName("cabin boy")]
        [PluralName("cabin boys")]
        CabinBoy,

        [SingularName("Captain Felagund")]
        //CSRTODO: no plural
        CaptainFelagund,

        [SingularName("Captain Renton")]
        //CSRTODO: no plural
        CaptainRenton,

        [SingularName("carrion crawler")]
        [PluralName("carrion crawlers")]
        CarrionCrawler,

        [SingularName("catapult crewmember")]
        [PluralName("catapult crewmembers")]
        CatapultCrewmember,

        [SingularName("catapult officer")]
        [PluralName("catapult officers")]
        CatapultOfficer,

        [SingularName("cave troll")]
        [PluralName("cave trolls")]
        CaveTroll,

        [SingularName("centaur")]
        [PluralName("centaurs")]
        Centaur,

        [SingularName("Chancellor of Protection")]
        //CSRTODO: no plural
        [Experience(200)]
        [Alignment(AlignmentType.Blue)]
        ChancellorOfProtection,

        [SingularName("child")]
        //CSRTODO: plural
        Child,

        [SingularName("citizen")]
        [PluralName("citizens")]
        Citizen,

        [SingularName("cook")]
        [PluralName("cooks")]
        Cook,

        [SingularName("coughing man")]
        [PluralName("coughing men")]
        CoughingMan,

        [SingularName("court jester")]
        [PluralName("court jesters")]
        CourtJester,

        [SingularName("Crabbe the class bully")]
        //CSRTODO: no plural
        [Experience(250)]
        CrabbeTheClassBully,

        [SingularName("criminal")]
        [PluralName("criminals")]
        Criminal,

        [SingularName("crow")]
        [PluralName("crows")]
        Crow,

        [SingularName("cutthroat")]
        [PluralName("cutthroats")]
        [Experience(500)]
        Cutthroat,

        [SingularName("Dale Purves")]
        //CSRTODO: no plural?
        DalePurves,

        [SingularName("damaged skeleton")]
        [PluralName("damaged skeletons")]
        DamagedSkeleton,

        [SingularName("dancing bear")]
        [PluralName("dancing bears")]
        DancingBear,

        [SingularName("decaying sailor")]
        [PluralName("decaying sailors")]
        DecayingSailor,

        [SingularName("Denethore the Wise")]
        //CSRTODO: no plural?
        DenethoreTheWise,

        [SingularName("dervish")]
        [PluralName("dervishes")] //CSRTODO: plural?
        Dervish,

        [SingularName("disillusioned drunk")]
        [PluralName("disillusioned drunks")]
        DisillusionedDrunk,

        [SingularName("door mimic")]
        [PluralName("door mimics")]
        DoorMimic,

        [SingularName("Dori")]
        //CSRTODO: no plural?
        Dori,

        [SingularName("Dr. Faramir")]
        //CSRTODO: no plural
        DrFaramir,

        [SingularName("Droolie the troll")]
        //CSRTODO: no plural?
        [Experience(100)]
        [Alignment(AlignmentType.Red)]
        DroolieTheTroll,

        [SingularName("drunk")]
        [PluralName("drunks")]
        [Alignment(AlignmentType.Red)]
        Drunk,

        [SingularName("drunken greek")]
        [PluralName("drunken greeks")]
        DrunkenGreek,

        [SingularName("dryad")]
        [PluralName("dryads")]
        Dryad,

        [SingularName("dungeon guard")]
        [PluralName("dungeon guards")]
        [Experience(120)]
        DungeonGuard,

        [SingularName("dwarf")]
        //CSRTODO: plural?
        Dwarf,

        [SingularName("dwarven acolyte")]
        [PluralName("dwarven acolytes")]
        DwarvenAcolyte,

        [SingularName("dwarven miner")]
        [PluralName("dwarven miners")]
        DwarvenMiner,

        [SingularName("dwarven outcast")]
        [PluralName("dwarven outcasts")]
        DwarvenOutcast,

        [SingularName("Eald the Wise")]
        //CSRTODO: no plural
        EaldTheWise,

        [SingularName("The Earthen Loremaster")]
        //CSRTODO: no plural
        EarthenLoremaster,

        [SingularName("eccentric artist")]
        [PluralName("eccentric artists")]
        EccentricArtist,

        [SingularName("Ele Honor Guard")]
        [PluralName("Ele Honor Guards")]
        EleHonorGuard,

        [SingularName("elven archer")]
        [PluralName("elven archers")]
        ElvenArcher,

        [SingularName("elven dignitary")]
        //CSRTODO: plural
        ElvenDignitary,

        [SingularName("elven guard")]
        [PluralName("elven guards")]
        ElvenGuard,

        [SingularName("evil sorcerer")]
        [PluralName("evil sorcerers")]
        [Experience(210)]
        EvilSorcerer,

        [SingularName("Eugene the Executioner")]
        //CSRTODO: no plural?
        EugeneTheExecutioner,

        [SingularName("Faeldor")]
        //CSRTODO: no plural
        Faeldor,

        [SingularName("Fallon")]
        //CSRTODO: no plural?
        [Experience(350)]
        [Alignment(AlignmentType.Blue)]
        Fallon,

        [SingularName("Faornil the Seer")]
        //CSRTODO: no plural?
        [Experience(250)]
        [Alignment(AlignmentType.Red)]
        FaornilTheSeer,

        [SingularName("farm cat")]
        [PluralName("farm cats")]
        [Experience(550)]
        FarmCat,

        [SingularName("field laborer")]
        [PluralName("field laborers")]
        FieldLaborer,

        [SingularName("fisherman")]
        [PluralName("fishermen")]
        Fisherman,

        [SingularName("forest sprite")]
        [PluralName("forest sprites")]
        ForestSprite,

        [SingularName("Frodo Baggins")]
        //CSRTODO: no plural?
        [Experience(260)]
        [Alignment(AlignmentType.Blue)]
        FrodoBaggins,

        [SingularName("gambler")]
        [PluralName("gamblers")]
        Gambler,

        [SingularName("garbage collector")]
        [PluralName("garbage collectors")]
        GarbageCollector,

        [SingularName("gargoyle")]
        [PluralName("gargoyles")]
        Gargoyle,

        [SingularName("gate guard")]
        [PluralName("gate guards")]
        GateGuard,

        [SingularName("ghast")]
        [PluralName("ghasts")]
        Ghast,

        [SingularName("ghost")]
        [PluralName("ghosts")]
        Ghost,

        [SingularName("ghostly fencer")]
        [PluralName("ghostly fencers")]
        [Experience(82)]
        GhostlyFencer,

        [SingularName("ghost of Muzgash")]
        //CSRTODO: no plural
        GhostOfMuzgash,

        [SingularName("ghoul")]
        [PluralName("ghouls")]
        [Experience(47)]
        Ghoul,

        [SingularName("giant catfish")]
        //CSRTODO: plural?
        GiantCatfish,

        [SingularName("giant centipede")]
        [PluralName("giant centipedes")]
        GiantCentipede,

        //CSRTODO: giant crayfish?

        [SingularName("giant rat")]
        [PluralName("giant rats")]
        GiantRat,

        [SingularName("giant rooster")]
        [PluralName("giant roosters")]
        GiantRooster,

        [SingularName("Gnarbolla")]
        //CSRTODO: no plural
        Gnarbolla,

        [SingularName("Gnibal")]
        //CSRTODO: no plural
        Gnibal,

        [SingularName("Gnimbelle")]
        //CSRTODO: no plural
        Gnimbelle,

        [SingularName("gnomish engineer")]
        [PluralName("gnomish engineers")]
        GnomishEngineer,

        [SingularName("goat")]
        [PluralName("goats")]
        Goat,

        [SingularName("goblin")]
        [PluralName("goblins")]
        [Experience(45)]
        Goblin,

        [SingularName("goblin warrior")]
        [PluralName("goblin warriors")]
        [Experience(70)]
        GoblinWarrior,

        [SingularName("Godfather")]
        //CSRTODO: no plural
        [Experience(1200)]
        Godfather,

        [SingularName("golden eagle")]
        [PluralName("golden eagles")]
        [Experience(350)]
        GoldenEagle,

        [SingularName("Graddy")]
        //CSRTODO: no plural
        [Experience(350)]
        Graddy,

        [SingularName("Granite Knucklebuster")]
        [PluralName("Granite Knucklebusters")] //verified 6/28/23
        GraniteKnucklebuster,

        [SingularName("Grant")]
        //CSRTODO: no plural
        [Experience(170)]
        Grant,

        [SingularName("grave digger")]
        [PluralName("grave diggers")]
        GraveDigger,

        [SingularName("gray elf")]
        [PluralName("gray elves")]
        GrayElf,

        [SingularName("green slime")]
        //CSRTODO: plurals are hard to get since hidden
        [Experience(35)]
        GreenSlime,

        [SingularName("Gregory Hiester")]
        //CSRTODO: no plural?
        [Experience(1200)]
        GregoryHiester,

        [SingularName("grey knight")]
        [PluralName("grey knights")]
        GreyKnight,

        [SingularName("griffon")]
        [PluralName("griffons")]
        [Experience(140)]
        Griffon,

        [SingularName("Grimaxe Grimson")]
        //CSRTODO: no plural
        GrimaxeGrimson,

        [SingularName("grizzly bear")]
        [PluralName("grizzly bears")]
        GrizzlyBear,

        [SingularName("groundskeeper")]
        [PluralName("groundskeepers")]
        Groundskeeper,

        [SingularName("Grunkill")]
        //CSRTODO: no plural?
        Grunkill,

        [SingularName("guard")]
        [PluralName("guards")]
        [Experience(110)]
        Guard,

        [SingularName("Guido")]
        //CSRTODO: no plural?
        [Experience(350)]
        [Alignment(AlignmentType.Red)]
        Guido,

        [SingularName("guildmaster")]
        [PluralName("guildmasters")]
        Guildmaster,

        [SingularName("Guildmaster Ansette")]
        //CSRTODO: no plural?
        [Experience(1200)]
        GuildmasterAnsette,

        [SingularName("gypsy-bard")]
        [PluralName("gypsy-bards")]
        GypsyBard,

        [SingularName("gypsy blademaster")]
        [PluralName("gypsy blademasters")]
        [Experience(160)]
        [Alignment(AlignmentType.Blue)]
        GypsyBlademaster,

        [SingularName("gypsy dancer")]
        [PluralName("gypsy dancers")]
        GypsyDancer,

        [SingularName("gypsy fighter")]
        [PluralName("gypsy fighters")]
        GypsyFighter,

        [SingularName("gypsy fire eater")]
        [PluralName("gypsy fire eaters")]
        GypsyFireEater,

        [SingularName("half-elf")]
        [PluralName("half-elves")]
        HalfElf,

        [SingularName("halfling ghost")]
        [PluralName("halfling ghosts")]
        HalflingGhost,

        [SingularName("harbor master")]
        [PluralName("harbor masters")]
        HarborMaster,

        [SingularName("hawker")]
        [PluralName("hawkers")]
        Hawker,

        [SingularName("herald")]
        [PluralName("heralds")]
        Herald,

        [SingularName("herb vendor")]
        [PluralName("herb vendors")]
        HerbVendor,

        [SingularName("hermit fisher")]
        [PluralName("hermit fishers")]
        [Experience(60)]
        HermitFisher,

        [SingularName("Hesta")]
        //CSRTODO: no plural
        Hesta,

        [SingularName("hill giant")]
        [PluralName("hill giants")]
        HillGiant,

        [SingularName("hippie")]
        [PluralName("hippies")]
        Hippie,

        [SingularName("hobbit")]
        [PluralName("hobbits")]
        Hobbit,

        [SingularName("hobbit chef")]
        [PluralName("hobbit chefs")]
        HobbitChef,

        [SingularName("hobbit cleric")]
        [PluralName("hobbit clerics")]
        HobbitCleric,

        [SingularName("hobbitish doctor")]
        [PluralName("hobbitish doctors")]
        [Alignment(AlignmentType.Blue)]
        HobbitishDoctor,

        [SingularName("hobbit mother")]
        [PluralName("hobbit mothers")]
        HobbitMother,

        [SingularName("hobbit sailor")]
        [PluralName("hobbit sailors")]
        HobbitSailor,

        [SingularName("hobbit warrior")]
        [PluralName("hobbit warriors")]
        HobbitWarrior,

        [SingularName("Hogoth")]
        //CSRTODO: no plural?
        [Experience(260)]
        [Alignment(AlignmentType.Blue)]
        Hogoth,

        [SingularName("hosta warrior")]
        [PluralName("hosta warriors")]
        HostaWarrior,

        [SingularName("Ho-suan the Penniless")]
        //CSRTODO: no plural?
        HoSuanThePenniless,

        [SingularName("hound dog")]
        [PluralName("hound dogs")]
        [Experience(150)]
        [Alignment(AlignmentType.Blue)]
        HoundDog,

        [SingularName("Hummley")]
        //CSRTODO: no plural
        Hummley,

        [SingularName("hunchback servant")]
        [PluralName("hunchback servants")]
        HunchbackServant,

        [SingularName("hunter")]
        [PluralName("hunters")]
        Hunter,

        [SingularName("Igor the Bouncer")]
        //CSRTODO: no plural
        [Experience(130)]
        [Alignment(AlignmentType.Grey)]
        IgorTheBouncer,

        [SingularName("Imrahil")]
        //CSRTODO: no plural
        Imrahil,

        [SingularName("innkeeper")]
        [PluralName("innkeepers")]
        Innkeeper,

        [SingularName("Iorlas the hermit")]
        //CSRTODO: no plural?
        [Experience(200)]
        [Alignment(AlignmentType.Grey)]
        IorlasTheHermit,

        [SingularName("irrigation engineer")]
        [PluralName("irrigation engineers")]
        IrrigationEngineer,

        [SingularName("Isildur")]
        //CSRTODO: no plural
        Isildur,

        [SingularName("Ixell DeSantis")]
        //CSRTODO: no plural
        [Experience(70)]
        [Alignment(AlignmentType.Blue)]
        [SingularSelection("Ixell")]
        IxellDeSantis,

        [SingularName("jongleur")]
        [PluralName("jongleurs")]
        Jongleur,

        [SingularName("Kaivo")]
        //CSRTODO: no plural
        Kaivo,

        [SingularName("Kali")]
        //CSRTODO: no plural
        Kali,

        [SingularName("Kallo")]
        //CSRTODO: no plural
        Kallo,

        [SingularName("Kasnar the Guard")]
        //CSRTODO: no plural
        [Experience(535)]
        KasnarTheGuard,

        [SingularName("Kauka")]
        //CSRTODO: no plural
        Kauka,

        [SingularName("King Brunden")]
        //CSRTODO: no plural
        [Experience(300)]
        KingBrunden,

        [SingularName("king's moneychanger")]
        [PluralName("king's moneychangers")]
        [Experience(150)]
        [Alignment(AlignmentType.Red)]
        KingsMoneychanger,

        [SingularName("knight")]
        [PluralName("knights")]
        Knight,

        [SingularName("Kosta")]
        //CSRTODO: no plural
        Kosta,

        [SingularName("Kralle")]
        //CSRTODO: plural?
        Kralle,

        [SingularName("laborer")]
        [PluralName("laborers")]
        Laborer,

        [SingularName("lady in waiting")]
        //CSRTODO: plural?
        LadyInWaiting,

        [SingularName("lag")]
        [PluralName("lags")] //verified 6/21/23
        Lag,

        [SingularName("large goblin")]
        [PluralName("large goblins")]
        [Experience(50)]
        LargeGoblin,

        [SingularName("Lars Magnus Grunwald")]
        //CSRTODO no plural
        LarsMagnusGrunwald,

        [SingularName("Lathlorien")]
        //CSRTODO: no plural
        Lathlorien,

        [SingularName("leprechaun")]
        [PluralName("leprechauns")]
        Leprechaun,

        [SingularName("little boy")]
        [PluralName("little boys")]
        LittleBoy,

        [SingularName("little mouse")]
        [PluralName("little mice")]
        LittleMouse,

        [SingularName("longshoreman")]
        [PluralName("longshoremen")]
        Longshoreman,

        [SingularName("Lord De'Arnse")]
        //CSRTODO: no plural
        LordDeArnse,

        [SingularName("lumberjack")]
        [PluralName("lumberjacks")]
        Lumberjack,

        [SingularName("Luthic the High Priestess")]
        //CSRTODO: no plural
        LuthicTheHighPriestess,

        [SingularName("Madame Despana")]
        //CSRTODO: no plural
        MadameDespana,

        [SingularName("Madame Nicolov")]
        //CSRTODO: no plural?
        [Experience(180)]
        [Alignment(AlignmentType.Blue)]
        MadameNicolov,

        [SingularName("madman")]
        [PluralName("madmen")]
        Madman,

        [SingularName("Magor the Instructor")]
        //CSRTODO: no plural
        MagorTheInstructor,

        [SingularName("Malika")]
        //CSRTODO: no plural
        [Experience(380)]
        Malika,

        [SingularName("Manager Mulloy")]
        //CSRTODO: no plural?
        [Experience(600)]
        [Alignment(AlignmentType.Blue)]
        ManagerMulloy,

        [SingularName("Mark Frey")]
        //CSRTODO: no plural?
        [Experience(450)]
        MarkFrey,

        [SingularName("marksman archer")]
        //CSRTODO: plural
        MarksmanArcher,

        [SingularName("master assassin")]
        [PluralName("master assassins")]
        [Experience(600)]
        MasterAssassin,

        [SingularName("master chef")]
        [PluralName("master chefs")]
        MasterChef,

        [SingularName("Master Jeweler")]
        //CSRTODO: no plural
        [Experience(170)]
        [Alignment(AlignmentType.Red)]
        MasterJeweler,

        [SingularName("Matriarch Alliyana of Isengard")]
        //CSRTODO: no plural
        MatriarchAlliyanaOfIsengard,

        [SingularName("Max the vegetable vendor")]
        //CSRTODO: no plural
        MaxTheVegetableVendor,

        //immune to stun
        [SingularName("Mayor Millwood")]
        //CSRTODO: no plural
        [Experience(220)]
        [Alignment(AlignmentType.Grey)]
        MayorMillwood,

        [SingularName("meistersinger")]
        [PluralName("meistersingers")]
        Meistersinger,

        [SingularName("member of lower royalty")]
        //CSRTODO: plural
        MemberOfLowerRoyalty,

        [SingularName("mercenary")]
        //CSRTODO: plural?
        Mercenary,

        [SingularName("mercenary captain")]
        [PluralName("mercenary captains")]
        [Experience(150)]
        MercenaryCaptain,

        [SingularName("merchant")]
        [PluralName("merchants")]
        Merchant,

        [SingularName("merchant marine")]
        [PluralName("merchant marines")]
        MerchantMarine,

        [SingularName("migrant worker")]
        [PluralName("migrant workers")]
        MigrantWorker,

        [SingularName("militiaman")]
        [PluralName("militiamen")]
        Militiaman,

        [SingularName("minor lich")]
        //CSRTODO: no plural
        MinorLich,

        [SingularName("minstrel of Esgaroth")]
        [PluralName("minstrels of Esgaroth")]
        MinstrelOfEsgaroth,

        [SingularName("mistress")]
        [PluralName("mistresses")]
        Mistress,

        [SingularName("monk")]
        [PluralName("monks")]
        Monk,

        [SingularName("monster")]
        [PluralName("monsters")]
        Monster,

        [SingularName("morality officer")]
        [PluralName("morality officers")]
        MoralityOfficer,

        [SingularName("Morgatha the Enchantress")]
        //CSRTODO: no plural
        MorgathaTheEnchantress,

        [SingularName("mosquito")]
        [PluralName("mosquitos")] //CSRTODO: dictionary has plural as either mosquito or mosquitoes
        Mosquito,

        [SingularName("mountain climber")]
        [PluralName("mountain climbers")]
        MountainClimber,

        [SingularName("Mountain Dragon")]
        //CSRTODO: plural?
        MountainDragon,

        [SingularName("mountain goat")]
        [PluralName("mountain goats")]
        MountainGoat,

        [SingularName("mountain hiker")]
        [PluralName("mountain hikers")]
        MountainHiker,

        [SingularName("mountain lion")]
        [PluralName("mountain lions")]
        MountainLion,

        [SingularName("mountain raider")]
        [PluralName("mountain raiders")]
        MountainRaider,

        [SingularName("Mr. Wartnose")]
        //CSRTODO: no plural
        [Experience(235)]
        MrWartnose,

        [SingularName("naked hobbit")]
        [PluralName("naked hobbits")]
        NakedHobbit,

        [SingularName("Naugrim")]
        //CSRTODO: no plural
        [Experience(205)]
        [Alignment(AlignmentType.Red)]
        Naugrim,

        [SingularName("netmaker")]
        [PluralName("netmakers")]
        Netmaker,

        [SingularName("nobleman")]
        [PluralName("noblemen")]
        Nobleman,

        [SingularName("Numenorean Captain")]
        [PluralName("Numenorean Captains")]
        NumenoreanCaptain,

        [SingularName("Numenorean Sentry")]
        [PluralName("Numenorean Sentries")]
        NumenoreanSentry,

        [SingularName("Numenorean Warder")]
        [PluralName("Numenorean Warders")]
        [Experience(450)]
        NumenoreanWarder,

        [SingularName("nymph")]
        [PluralName("nymphs")]
        Nymph,

        [SingularName("ogre")]
        [PluralName("ogres")]
        [Experience(150)]
        Ogre,

        [SingularName("old gardener")]
        [PluralName("old gardeners")]
        OldGardener,

        [SingularName("Oliphaunt the Tattoo Artist")]
        //CSRTODO: no plural
        [Experience(310)]
        [Alignment(AlignmentType.Blue)]
        OliphauntTheTattooArtist,

        [SingularName("Oohlgrist")]
        //CSRTODO: no plural
        [Experience(110)]
        Oohlgrist,

        [SingularName("orc")]
        [PluralName("orcs")]
        Orc,

        [SingularName("orc guard")]
        [PluralName("orc guards")]
        OrcGuard,

        [SingularName("orcish corpse")]
        [PluralName("orcish corpses")]
        OrcishCorpse,

        [SingularName("orcish skeleton")]
        [PluralName("orcish skeletons")]
        OrcishSkeleton,

        [SingularName("orc miner")]
        [PluralName("orc miners")]
        OrcMiner,

        [SingularName("otyugh")]
        [PluralName("otyughs")]
        [Experience(75)]
        Otyugh,

        [SingularName("paladin")]
        [PluralName("paladins")]
        Paladin,

        [SingularName("Pansy Smallburrows")]
        //CSRTODO: no plural?
        [Experience(95)]
        [Alignment(AlignmentType.Red)]
        PansySmallburrows,

        [SingularName("peasant")]
        [PluralName("peasants")]
        Peasant,

        [SingularName("pilgrim")]
        [PluralName("pilgrims")]
        Pilgrim,

        [SingularName("pirate")]
        [PluralName("pirates")]
        Pirate,

        [SingularName("pixie")]
        [PluralName("pixies")]
        Pixie,

        [SingularName("policeman")]
        [PluralName("policemen")]
        Policeman,

        [SingularName("pony")]
        //CSRTODO: plural
        Pony,

        [SingularName("poor fisherman")]
        [PluralName("poor fishermen")]
        PoorFisherman,

        [SingularName("pregnant goat")]
        [PluralName("pregnant goats")]
        PregnantGoat,

        [SingularName("priest")]
        [PluralName("priests")]
        Priest,

        [SingularName("Prince Brunden")]
        //CSRTODO: no plural?
        [Experience(150)]
        [Alignment(AlignmentType.Blue)]
        PrinceBrunden,

        [SingularName("Prucilla the Groupie")]
        //CSRTODO: no plural?
        PrucillaTheGroupie,

        [SingularName("rabbit")]
        [PluralName("rabbits")]
        Rabbit,

        [SingularName("raccoon")]
        [PluralName("raccoons")]
        Raccoon,

        [SingularName("ram")]
        [PluralName("rams")]
        Ram,

        [SingularName("ranger")]
        [PluralName("rangers")]
        Ranger,

        [SingularName("Ranier the Librarian")]
        //CSRTODO: no plural
        RanierTheLibrarian,

        [SingularName("raving lunatic")]
        [PluralName("raving lunatics")]
        RavingLunatic,

        [SingularName("Roc")]
        //CSRTODO: no plural
        Roc,

        [SingularName("sailor")]
        [PluralName("sailors")]
        Sailor,

        [SingularName("salamander")]
        [PluralName("salamanders")]
        [Experience(100)]
        [Alignment(AlignmentType.Red)]
        Salamander,

        [SingularName("scallywag")]
        [PluralName("scallywags")]
        Scallywag,

        [SingularName("scholar")]
        [PluralName("scholars")]
        Scholar,

        [SingularName("Scranlin")]
        //CSRTODO: no plural
        [Experience(500)]
        [Alignment(AlignmentType.Red)]
        Scranlin,

        [SingularName("scribe")]
        [PluralName("scribes")]
        Scribe,

        [SingularName("seasoned veteran")]
        [PluralName("seasoned veterans")]
        SeasonedVeteran,

        [SingularName("Sergeant Grimgall")]
        //CSRTODO: no plural?
        [Experience(350)]
        [Alignment(AlignmentType.Blue)]
        SergeantGrimgall,

        [SingularName("servant")]
        [PluralName("servants")]
        Servant,

        [SingularName("sewer demon")]
        [PluralName("sewer demons")]
        SewerDemon,

        [SingularName("sewer orc")]
        [PluralName("sewer orcs")]
        [Aggressive]
        SewerOrc,

        [SingularName("sewer orc guard")]
        [PluralName("sewer orc guards")]
        [Experience(70)]
        [Aggressive]
        SewerOrcGuard,

        [SingularName("sewer rat")]
        [PluralName("sewer rats")]
        SewerRat,

        [SingularName("sewer wolf")]
        [PluralName("sewer wolves")]
        SewerWolf,

        [SingularName("Shadow of Incendius")]
        //CSRTODO: no plural
        ShadowOfIncendius,

        [SingularName("shallow scholar")]
        [PluralName("shallow scholars")]
        ShallowScholar,

        [SingularName("Sharkey")]
        //CSRTODO: no plural?
        Sharkey,

        [SingularName("sheep")]
        [PluralName("sheep")]
        Sheep,

        [SingularName("shepherd")]
        //CSRTODO: no plural?
        [Experience(60)]
        [Alignment(AlignmentType.Blue)]
        Shepherd,

        [SingularName("shirriff")]
        [PluralName("shirriffs")]
        [Experience(325)]
        Shirriff,

        [SingularName("Sival the Artificer")]
        //CSRTODO: no plural
        SivalTheArtificer,

        [SingularName("skald")]
        [PluralName("skalds")]
        Skald,

        [SingularName("skeleton")]
        [PluralName("skeletons")]
        Skeleton,

        [SingularName("small crab")]
        [PluralName("small crabs")]
        SmallCrab,

        [SingularName("small spider")]
        [PluralName("small spiders")]
        SmallSpider,

        [SingularName("Smee")]
        //CSRTODO: no plural
        Smee,

        [SingularName("smithy")]
        //CSRTODO: plural?
        Smithy,

        [SingularName("snarling mutt")]
        [PluralName("snarling mutts")]
        [Experience(50)]
        [Alignment(AlignmentType.Red)]
        SnarlingMutt,

        [SingularName("sobbing girl")]
        [PluralName("sobbing girls")]
        SobbingGirl,

        [SingularName("spectre")]
        [PluralName("spectres")]
        [Experience(140)]
        Spectre,

        [SingularName("sprite guard")]
        [PluralName("sprite guards")]
        [Experience(120)]
        [Alignment(AlignmentType.Blue)]
        SpriteGuard,

        [SingularName("Sssreth the Lizardman")]
        //CSRTODO: no plural
        SssrethTheLizardman,

        [SingularName("stablehand")]
        [PluralName("stablehands")]
        Stablehand,

        [SingularName("stegosaurus")]
        //CSRTODO: plural?
        Stegosaurus,

        [SingularName("stoker")]
        [PluralName("stokers")]
        Stoker,

        [SingularName("strumpet")]
        [PluralName("strumpets")]
        Strumpet,

        [SingularName("student")]
        [PluralName("students")]
        Student,

        [SingularName("student activist")]
        [PluralName("student activists")]
        StudentActivist,

        [SingularName("swarm of flies")]
        [PluralName("swarm of flies")]
        SwarmOfFlies,

        [SingularName("Tamar")]
        //CSRTODO: no plural
        Tamar,

        [SingularName("Tellia the Witch")]
        //CSRTODO: no plural
        TelliaTheWitch,

        [SingularName("thief")]
        [PluralName("thiefs")]
        Thief,

        [SingularName("Thoringil the Holy")]
        //CSRTODO: no plural
        ThoringilTheHoly,

        [SingularName("tour guide")]
        [PluralName("tour guides")]
        TourGuide,

        [SingularName("tourist")]
        [PluralName("tourists")]
        Tourist,

        [SingularName("The Town Crier")]
        //CSRTODO: no plural?
        TheTownCrier,

        [SingularName("tracker")]
        [PluralName("trackers")]
        Tracker,

        [SingularName("traveler")]
        [PluralName("travelers")]
        Traveler,

        [SingularName("traveling horse")]
        [PluralName("traveling horses")]
        TravelingHorse,

        [SingularName("traveling minstrel")]
        [PluralName("traveling minstrels")]
        TravelingMinstrel,

        [SingularName("Trakard ogre ranger")]
        [PluralName("Trakard ogre rangers")]
        TrakardOgreRanger,

        [SingularName("treant")]
        [PluralName("treants")]
        Treant,

        [SingularName("tree sprite")]
        [PluralName("tree sprites")]
        TreeSprite,

        [SingularName("Tyrie")]
        //CSRTODO: no plural
        Tyrie,

        [SingularName("ugly boy")]
        [PluralName("ugly boys")]
        UglyBoy,

        [SingularName("ugly bully")]
        [PluralName("ugly bullys")] //plural verified 6/15/23
        UglyBully,

        [SingularName("ugly kid")]
        [PluralName("ugly kids")]
        UglyKid,

        [SingularName("vagrant")]
        [PluralName("vagrants")]
        Vagrant,

        [SingularName("vampire bat")]
        [PluralName("vampire bats")]
        VampireBat,

        [SingularName("vendor")]
        [PluralName("vendors")]
        Vendor,

        [SingularName("village woman")]
        [PluralName("village women")]
        VillageWoman,

        [SingularName("villager's ghost")]
        [PluralName("villager's ghosts")]
        VillagersGhost,

        [SingularName("Voteli")]
        //CSRTODO: no plural
        Voteli,

        [SingularName("vulture")]
        [PluralName("vultures")]
        Vulture,

        [SingularName("waitress")]
        [PluralName("waitresses")]
        [Experience(7)]
        Waitress,

        [SingularName("warrant officer")]
        [PluralName("warrant officers")]
        WarrantOfficer,

        [SingularName("warrior bard")]
        [PluralName("warrior bards")]
        [Experience(100)]
        [Alignment(AlignmentType.Red)]
        WarriorBard,

        [SingularName("water turtle")]
        [PluralName("water turtles")]
        WaterTurtle,

        [SingularName("werewolf")]
        [PluralName("werewolves")]
        Werewolf,

        [SingularName("wheat harvester")]
        [PluralName("wheat harvesters")]
        WheatHarvester,

        [SingularName("white knight")]
        [PluralName("white knights")]
        WhiteKnight,

        [SingularName("wight")]
        [PluralName("wights")]
        Wight,

        [SingularName("wildman")]
        [PluralName("wildmen")]
        Wildman,

        [SingularName("Wizard of the First Order")]
        //CSRTODO: no plural?
        WizardOfTheFirstOrder,

        [SingularName("wolf")]
        [PluralName("wolves")]
        Wolf,

        [SingularName("worker")]
        [PluralName("workers")]
        Worker,

        [SingularName("worshiper")]
        [PluralName("worshiper")]
        Worshiper,

        [SingularName("Zain")]
        //CSRTODO: no plural
        Zain,

        [SingularName("Zathriel the Minstrel")]
        [SingularSelection("Minstrel")]
        //CSRTODO: no plural?
        [Experience(220)]
        [Alignment(AlignmentType.Blue)]
        ZathrielTheMinstrel,

        [SingularName("zombie")]
        [PluralName("zombies")]
        Zombie,
    }
}
