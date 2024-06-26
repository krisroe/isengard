﻿namespace IsengardClient.Backend
{
    public enum MobTypeEnum
    {
        [SingularName("accuser")]
        [PluralName("accusers")]
        Accuser,

        [SingularName("advisor")]
        [PluralName("advisors")]
        Advisor,

        [SingularName("aged bard")]
        [PluralName("aged bards")]
        AgedBard,

        [SingularName("aged monk")]
        [PluralName("aged monks")]
        AgedMonk,

        [SingularName("Alasse")]
        //CSRTODO: no plural
        Alasse,

        [SingularName("alley cat")]
        [PluralName("alley cats")]
        [Alignment(AlignmentType.DullBlue)]
        [Experience(50)]
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

        [SingularName("architectural student")]
        [PluralName("architectural students")]
        ArchitecturalStudent,

        [SingularName("aristocrat")]
        [PluralName("aristocrats")]
        Aristocrat,

        [SingularName("art student")]
        [PluralName("art students")]
        ArtStudent,

        [SingularName("Asimele the Pleasure Priestess")]
        AsimeleThePleasurePriestess,

        [SingularName("auctioneer")]
        [PluralName("auctioneers")]
        Auctioneer,

        [SingularName("Aurelius the Scholar")]
        //CSRTODO: no plural
        AureliusTheScholar,

        [SingularName("Azgara")]
        //CSRTODO: no plural
        Azgara,

        [SingularName("baby roc")]
        [PluralName("baby rocs")]
        BabyRoc,

        [SingularName("bandit")]
        [PluralName("bandits")]
        Bandit,

        [SingularName("barbarian")]
        [PluralName("barbarians")]
        Barbarian,

        [SingularName("barbarian guard")]
        [PluralName("barbarian guards")]
        BarbarianGuard,

        [SingularName("barbarian raider")]
        [PluralName("barbarian raiders")]
        BarbarianRaider,

        [SingularName("barmaid")]
        [PluralName("barmaids")]
        Barmaid,

        [SingularName("barrow wight")]
        [PluralName("barrow wights")]
        BarrowWight,

        [SingularName("bartender")]
        [PluralName("bartenders")]
        [Experience(15)]
        [Alignment(AlignmentType.DullRed)]
        Bartender,

        [SingularName("bear")]
        [PluralName("bears")]
        Bear,

        [SingularName("Belladona Took")]
        //CSRTODO: no plural
        BelladonaTook,

        [SingularName("Bezanthi")]
        //CSRTODO: no plural
        Bezanthi,

        [SingularName("Big Papa")]
        //CSRTODO: no plural?
        [Experience(350)]
        [Alignment(AlignmentType.DullBlue)]
        BigPapa,

        [SingularName("Bilbo Baggins")]
        //CSRTODO: no plural
        [Experience(260)]
        [Alignment(AlignmentType.DullBlue)]
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

        [SingularName("blue crab")]
        [PluralName("blue crabs")]
        BlueCrab,

        [SingularName("blue flying snake")]
        [PluralName("blue flying snakes")]
        BlueFlyingSnake,

        [SingularName("boatswain")]
        [PluralName("boatswains")]
        [Experience(350)]
        [Alignment(AlignmentType.DullBlue)]
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

        [SingularName("bridge guard")]
        [PluralName("bridge guards")]
        [Experience(400)]
        BridgeGuard,

        [SingularName("bugbear")]
        [PluralName("bugbears")]
        Bugbear,

        [SingularName("bull")]
        [PluralName("bulls")]
        Bull,

        [SingularName("The burned remains of Nimrodel")]
        //CSRTODO: no plural.
        [Experience(300)]
        [Alignment(AlignmentType.DullRed)]
        [MobVisibility(MobVisibility.Hidden)]
        BurnedRemainsOfNimrodel,

        [SingularName("butcher")]
        [PluralName("butchers")]
        Butcher,

        [SingularName("butler")]
        [PluralName("butlers")]
        Butler,

        [SingularName("cabin boy")]
        [PluralName("cabin boys")]
        CabinBoy,

        [SingularName("campus officer")]
        [PluralName("campus officers")]
        CampusOfficer,

        [SingularName("Captain Belfalas")]
        CaptainBelfalas,

        [SingularName("Captain Felagund")]
        //CSRTODO: no plural
        [Alignment(AlignmentType.Blue)]
        [Experience(1000)]
        CaptainFelagund,

        [SingularName("Captain Renton")]
        //CSRTODO: no plural
        CaptainRenton,

        [SingularName("caretaker")]
        [PluralName("caretakers")]
        [Experience(40)]
        Caretaker,

        [SingularName("Carl Arl")]
        //CSRTODO: no plural
        [Experience(450)]
        CarlArl,

        [SingularName("carrion crawler")]
        [PluralName("carrion crawlers")]
        CarrionCrawler,

        [SingularName("cartographer")]
        [PluralName("cartographers")]
        Cartographer,

        [SingularName("cat")]
        [PluralName("cats")]
        Cat,

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
        //[Alignment(AlignmentType.Blue)] //CSRTODO: determine actual alignment
        ChancellorOfProtection,

        [SingularName("child")]
        //CSRTODO: plural
        Child,

        [SingularName("citizen")]
        [PluralName("citizens")]
        Citizen,

        [SingularName("citizen of tharbad")]
        //CSRTODO: plural
        CitizenOfTharbad,

        [SingularName("city guard")]
        [PluralName("city guards")]
        CityGuard,

        [SingularName("clockwork dragon")]
        [PluralName("clockwork dragons")]
        ClockworkDragon,

        [SingularName("cook")]
        [PluralName("cooks")]
        Cook,

        [SingularName("corn husker")]
        [PluralName("corn huskers")]
        CornHusker,

        [SingularName("coughing man")]
        [PluralName("coughing men")]
        CoughingMan,

        [SingularName("court jester")]
        [PluralName("court jesters")]
        [Experience(115)]
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
        [Alignment(AlignmentType.Red)]
        Cutthroat,

        [SingularName("Dale Purves")]
        //CSRTODO: no plural?
        [Experience(800)]
        [Alignment(AlignmentType.Blue)]
        DalePurves,

        [SingularName("damaged skeleton")]
        [PluralName("damaged skeletons")]
        DamagedSkeleton,

        [SingularName("dancing bear")]
        [PluralName("dancing bears")]
        DancingBear,

        [SingularName("dancing girl")]
        [PluralName("dancing girls")]
        DancingGirl,

        [SingularName("Deathbringer")]
        //CSRTODO: no plural
        Deathbringer,

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

        [SingularName("dolphin")]
        [PluralName("dolphins")]
        Dolphin,

        [SingularName("door mimic")]
        [PluralName("door mimics")]
        [MobVisibility(MobVisibility.Hidden)]
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
        [Alignment(AlignmentType.DullRed)]
        DroolieTheTroll,

        [SingularName("drow elf")]
        [PluralName("drow elfs")] //verified 7/5/23
        [Aggressive]
        DrowElf,

        [SingularName("drunk")]
        [PluralName("drunks")]
        [Alignment(AlignmentType.DullRed)]
        [Experience(7)]
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

        [SingularName("dwarven shopper")]
        [PluralName("dwarven shoppers")]
        DwarvenShopper,

        [SingularName("dwarven wagoneer")]
        [PluralName("dwarven wagoneers")]
        DwarvenWagoneer,

        [SingularName("eagle")]
        [PluralName("eagles")]
        Eagle,

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
        [Alignment(AlignmentType.DullBlue)]
        ElvenGuard,

        [SingularName("elven wight")]
        [PluralName("elven wights")]
        ElvenWight,

        [SingularName("Erech")]
        //CSRTODO: no plural
        Erech,

        [SingularName("evil high priestess")]
        //CSRTODO: no plural
        EvilHighPriestess,

        [SingularName("evil sorcerer")]
        [PluralName("evil sorcerers")]
        [Experience(210)]
        EvilSorcerer,

        //doesn't appear stunnable
        [SingularName("Eugene the Executioner")]
        //CSRTODO: no plural?
        [Experience(555)]
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
        [Alignment(AlignmentType.DullRed)]
        FaornilTheSeer,

        [SingularName("farm cat")]
        [PluralName("farm cats")]
        [Experience(550)]
        [Alignment(AlignmentType.DullRed)]
        FarmCat,

        [SingularName("farmer")]
        [PluralName("farmers")]
        Farmer,

        [SingularName("farm maid")]
        [PluralName("farm maids")]
        FarmMaid,

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
        [Alignment(AlignmentType.DullBlue)]
        FrodoBaggins,

        [SingularName("Galdor")]
        //CSRTODO: no plural
        Galdor,

        [SingularName("gambler")]
        [PluralName("gamblers")]
        Gambler,

        [SingularName("garbage collector")]
        [PluralName("garbage collectors")]
        GarbageCollector,

        [SingularName("gargoyle")]
        [PluralName("gargoyles")]
        Gargoyle,

        [SingularName("garter snake")]
        [PluralName("garter snakes")]
        GarterSnake,

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

        [SingularName("ghost of Evan Nildredge")]
        //CSRTODO: no plural
        GhostOfEvanNildredge,

        [SingularName("ghost of Muzgash")]
        //CSRTODO: no plural
        GhostOfMuzgash,

        [SingularName("ghoul")]
        [PluralName("ghouls")]
        [Experience(47)]
        Ghoul,

        [SingularName("giant catfish")]
        [PluralName("giant catfish")]
        GiantCatfish,

        [SingularName("giant crab")]
        [PluralName("giant crabs")]
        GiantCrab,

        [SingularName("giant crayfish")]
        [PluralName("giant crayfish")]
        GiantCrayfish,

        [SingularName("giant centipede")]
        [PluralName("giant centipedes")]
        GiantCentipede,

        [SingularName("giant grasshopper")]
        [PluralName("giant grasshoppers")]
        GiantGrasshopper,

        [SingularName("giant octopus")]
        //CSRTODO: plural?
        GiantOctopus,

        [SingularName("giant rat")]
        [PluralName("giant rats")]
        GiantRat,

        [SingularName("giant rooster")]
        [PluralName("giant roosters")]
        GiantRooster,

        [SingularName("giant stingray")]
        [PluralName("giant stingrays")]
        GiantStingray,

        [SingularName("giant venus fly-trap")]
        [PluralName("giant venus fly-traps")]
        [Experience(150)]
        GiantVenusFlyTrap,

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
        [Alignment(AlignmentType.Red)]
        Godfather,

        [SingularName("golden eagle")]
        [PluralName("golden eagles")]
        [Experience(350)]
        GoldenEagle,

        [SingularName("Graddy")]
        //CSRTODO: no plural
        [Experience(350)]
        [Alignment(AlignmentType.DullRed)]
        Graddy,

        [SingularName("graduate student")]
        [PluralName("graduate students")]
        GraduateStudent,

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
        [Alignment(AlignmentType.DullBlue)]
        [Experience(75)]
        GrayElf,

        [SingularName("gray shark")]
        [PluralName("gray sharks")]
        GrayShark,

        [SingularName("greater spider")]
        [PluralName("greater spiders")]
        [Experience(150)]
        GreaterSpider,

        [SingularName("great spider")]
        [PluralName("great spiders")]
        GreatSpider,

        [SingularName("green slime")]
        //CSRTODO: plurals are hard to get since hidden
        [MobVisibility(MobVisibility.Hidden)]
        [Experience(35)]
        [DestroysItems]
        GreenSlime,

        [SingularName("Gregory Hiester")]
        //CSRTODO: no plural?
        [Experience(1200)]
        [Alignment(AlignmentType.Grey)]
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
        [Experience(40)]
        GrizzlyBear,

        [SingularName("groundskeeper")]
        [PluralName("groundskeepers")]
        Groundskeeper,

        [SingularName("Grunkill")]
        //CSRTODO: no plural?
        [Experience(600)]
        Grunkill,

        [SingularName("guard")]
        [PluralName("guards")]
        [Experience(110)]
        [Alignment(AlignmentType.DullBlue)]
        Guard,

        [SingularName("Guido")]
        //CSRTODO: no plural?
        [Experience(350)]
        [Alignment(AlignmentType.DullRed)]
        Guido,

        [SingularName("guildmaster")]
        [PluralName("guildmasters")]
        [Experience(850)]
        [Alignment(AlignmentType.DullRed)]
        Guildmaster,

        [SingularName("Guildmaster Ansette")]
        //CSRTODO: no plural?
        [Experience(1200)]
        [Alignment(AlignmentType.Blue)]
        GuildmasterAnsette,

        [SingularName("gypsy-bard")]
        [PluralName("gypsy-bards")]
        GypsyBard,

        [SingularName("gypsy blademaster")]
        [PluralName("gypsy blademasters")]
        [Experience(160)]
        [Alignment(AlignmentType.DullBlue)]
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

        [SingularName("headless horseman")]
        [PluralName("headless horsemen")]
        HeadlessHorseman,

        [SingularName("herald")]
        [PluralName("heralds")]
        Herald,

        [SingularName("herb dealer")]
        [PluralName("herb dealers")]
        HerbDealer,

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

        [SingularName("hill giant worker")]
        [PluralName("hill giant workers")]
        HillGiantWorker,

        [SingularName("hippie")]
        [PluralName("hippies")]
        Hippie,

        [SingularName("hobbit")]
        [PluralName("hobbits")]
        [Experience(7)]
        Hobbit,

        [SingularName("hobbit chef")]
        [PluralName("hobbit chefs")]
        HobbitChef,

        [SingularName("hobbit cleric")]
        [PluralName("hobbit clerics")]
        HobbitCleric,

        [SingularName("hobbitish doctor")]
        [PluralName("hobbitish doctors")]
        [Alignment(AlignmentType.DullBlue)]
        [Experience(15)]
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
        [Alignment(AlignmentType.DullBlue)]
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
        [Alignment(AlignmentType.DullBlue)]
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

        [SingularName("hydra")]
        //CSRTODO: plural?
        Hydra,

        [SingularName("Igor the Bouncer")]
        //CSRTODO: no plural
        [Experience(130)]
        [Alignment(AlignmentType.Grey)]
        IgorTheBouncer,

        [SingularName("Imrahil")]
        //CSRTODO: no plural
        Imrahil,

        [SingularName("incorporeal ghost")]
        [PluralName("incorporeal ghosts")]
        IncorporealGhost,

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
        [Alignment(AlignmentType.DullBlue)]
        [SingularSelection("Ixell")]
        IxellDeSantis,

        [SingularName("jeweler")]
        [PluralName("jewelers")]
        Jeweler,

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
        [Alignment(AlignmentType.Red)]
        KasnarTheGuard,

        [SingularName("Kauka")]
        //CSRTODO: no plural
        Kauka,

        [SingularName("killer bee")]
        [PluralName("killer bees")]
        KillerBee,

        [SingularName("King Brunden")]
        //CSRTODO: no plural
        [Experience(300)]
        [Alignment(AlignmentType.Grey)]
        KingBrunden,

        [SingularName("king's moneychanger")]
        [PluralName("king's moneychangers")]
        [Experience(150)]
        [Alignment(AlignmentType.DullRed)]
        KingsMoneychanger,

        [SingularName("knight")]
        [PluralName("knights")]
        [Alignment(AlignmentType.DullBlue)]
        [Experience(70)]
        Knight,

        [SingularName("Kosta")]
        //CSRTODO: no plural
        Kosta,

        [SingularName("Kralle")]
        //CSRTODO: plural?
        Kralle,

        [SingularName("laborer")]
        [PluralName("laborers")]
        [Alignment(AlignmentType.DullBlue)]
        Laborer,

        [SingularName("lab technician")]
        [PluralName("lab technicians")]
        LabTechnician,

        [SingularName("Lady Gwyneth the Chaste")]
        //CSRTODO: no plural
        LadyGwynethTheChaste,

        [SingularName("lady in waiting")]
        //CSRTODO: plural?
        LadyInWaiting,

        [SingularName("lag")]
        [PluralName("lags")] //verified 6/21/23
        Lag,

        [SingularName("large crayfish")]
        //CSRTODO: plural?
        LargeCrayfish,

        [SingularName("large goblin")]
        [PluralName("large goblins")]
        [Experience(50)]
        LargeGoblin,

        [SingularName("large spider")]
        [PluralName("large spiders")]
        LargeSpider,

        [SingularName("Lars Magnus Grunwald")]
        //CSRTODO no plural
        [Experience(200)]
        LarsMagnusGrunwald,

        [SingularName("Lathlorien")]
        //CSRTODO: no plural
        Lathlorien,

        [SingularName("leprechaun")]
        [PluralName("leprechauns")]
        [Alignment(AlignmentType.DullBlue)]
        [Experience(75)]
        Leprechaun,

        [SingularName("little boy")]
        [PluralName("little boys")]
        LittleBoy,

        [SingularName("little mouse")]
        [PluralName("little mice")]
        [Alignment(AlignmentType.DullBlue)]
        [Experience(1)]
        LittleMouse,

        [SingularName("llama")]
        [PluralName("llamas")]
        Llama,

        [SingularName("Lodz")]
        //CSRTODO: no plural
        Lodz,

        [SingularName("longshoreman")]
        [PluralName("longshoremen")]
        Longshoreman,

        [SingularName("Lord De'Arnse")]
        //CSRTODO: no plural
        LordDeArnse,

        [SingularName("Luistrin the Architect")]
        //CSRTODO: no plural
        LuistrinTheArchitect,

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

        [SingularName("mad scientist")]
        [PluralName("mad scientists")]
        MadScientist,

        [SingularName("Magor the Instructor")]
        //CSRTODO: no plural
        MagorTheInstructor,

        [SingularName("maid")]
        [PluralName("maids")]
        Maid,

        [SingularName("maiden")]
        [PluralName("maidens")]
        Maiden,

        [SingularName("Malika")]
        //CSRTODO: no plural
        [Experience(380)]
        Malika,

        [SingularName("Manager Mulloy")]
        //CSRTODO: no plural?
        [Experience(600)]
        [Alignment(AlignmentType.Blue)]
        [DestroysItems]
        ManagerMulloy,

        [SingularName("Mark Frey")]
        //CSRTODO: no plural?
        [Experience(450)]
        [Alignment(AlignmentType.DullRed)]
        MarkFrey,

        [SingularName("marksman archer")]
        //CSRTODO: plural
        MarksmanArcher,

        [SingularName("master assassin")]
        [PluralName("master assassins")]
        [MobVisibility(MobVisibility.Hidden)]
        [Experience(600)]
        [Alignment(AlignmentType.Red)]
        MasterAssassin,

        [SingularName("master chef")]
        [PluralName("master chefs")]
        MasterChef,

        [SingularName("Master Jeweler")]
        //CSRTODO: no plural
        [Experience(170)]
        [Alignment(AlignmentType.DullRed)]
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
        [Alignment(AlignmentType.DullRed)]
        [Experience(6)]
        Merchant,

        [SingularName("merchant marine")]
        [PluralName("merchant marines")]
        [Alignment(AlignmentType.DullBlue)]
        MerchantMarine,

        [SingularName("migrant worker")]
        [PluralName("migrant workers")]
        MigrantWorker,

        [SingularName("militiaman")]
        [PluralName("militiamen")]
        Militiaman,

        [SingularName("minor lich")]
        //CSRTODO: no plural
        [MobVisibility(MobVisibility.Invisible)]
        MinorLich,

        [SingularName("minstrel of Esgaroth")]
        [PluralName("minstrels of Esgaroth")]
        [Experience(1000)]
        MinstrelOfEsgaroth,

        [SingularName("mistress")]
        [PluralName("mistresses")]
        Mistress,

        [SingularName("monk")]
        [PluralName("monks")]
        Monk,

        [SingularName("monster")]
        [PluralName("monsters")]
        [Experience(45)]
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
        [Experience(45)]
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

        [SingularName("Nathalin the Trader")]
        //CSRTODO: no plural
        [Alignment(AlignmentType.Grey)]
        [Experience(200)]
        NathalinTheTrader,

        [SingularName("Naugrim")]
        //CSRTODO: no plural
        [Experience(205)]
        [Alignment(AlignmentType.DullRed)]
        Naugrim,

        [SingularName("netmaker")]
        [PluralName("netmakers")]
        Netmaker,

        [SingularName("nobleman")]
        [PluralName("noblemen")]
        [Alignment(AlignmentType.DullRed)]
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

        [SingularName("nutcracker")]
        [PluralName("nutcrackers")]
        Nutcracker,

        [SingularName("nymph")]
        [PluralName("nymphs")]
        Nymph,

        [SingularName("oarsman")]
        [PluralName("oarsmen")]
        Oarsman,

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
        [Alignment(AlignmentType.DullBlue)]
        [Experience(31)]
        Paladin,

        [SingularName("Pansy Smallburrows")]
        //CSRTODO: no plural?
        [Experience(95)]
        [Alignment(AlignmentType.DullRed)]
        PansySmallburrows,

        [SingularName("parlor maid")]
        [PluralName("parlor maids")]
        ParlorMaid,

        [SingularName("peasant")]
        [PluralName("peasants")]
        [Alignment(AlignmentType.DullRed)]
        Peasant,

        [SingularName("phrenologist")]
        [PluralName("phrenologists")]
        Phrenologist,

        [SingularName("pilgrim")]
        [PluralName("pilgrims")]
        Pilgrim,

        [SingularName("pilot")]
        [PluralName("pilots")]
        Pilot,

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

        [SingularName("potion dealer")]
        [PluralName("potion dealers")]
        PotionDealer,

        [SingularName("pregnant goat")]
        [PluralName("pregnant goats")]
        PregnantGoat,

        [SingularName("priest")]
        [PluralName("priests")]
        Priest,

        [SingularName("Prince Brunden")]
        //CSRTODO: no plural?
        [Experience(150)]
        [Alignment(AlignmentType.DullBlue)]
        PrinceBrunden,

        [SingularName("Prucilla the Groupie")]
        //CSRTODO: no plural?
        [Alignment(AlignmentType.Blue)]
        [CannotHarm]
        PrucillaTheGroupie,

        [SingularName("python")]
        [PluralName("pythons")]
        Python,

        [SingularName("rabbit")]
        [PluralName("rabbits")]
        Rabbit,

        [SingularName("raccoon")]
        [PluralName("raccoons")]
        Raccoon,

        [SingularName("Radbug")]
        //CSRTODO: no plural
        [Aggressive]
        [InfectsWithDisease]
        Radbug,

        [SingularName("raider guard")]
        [PluralName("raider guards")]
        RaiderGuard,

        [SingularName("ram")]
        [PluralName("rams")]
        Ram,

        [SingularName("ranger")]
        [PluralName("rangers")]
        [Alignment(AlignmentType.DullBlue)]
        [Experience(38)]
        Ranger,

        [SingularName("Ranier the Librarian")]
        //CSRTODO: no plural
        RanierTheLibrarian,

        [SingularName("raving lunatic")]
        [PluralName("raving lunatics")]
        RavingLunatic,

        [SingularName("registrar")]
        [PluralName("registrars")]
        Registrar,

        [SingularName("researcher")]
        [PluralName("researchers")]
        Researcher,

        [SingularName("Rex")]
        //CSRTODO: no plural
        Rex,

        [SingularName("Roc")]
        //CSRTODO: no plural
        Roc,

        [SingularName("sadist")]
        [PluralName("sadists")]
        Sadist,

        [SingularName("sailor")]
        [PluralName("sailors")]
        Sailor,

        [SingularName("salamander")]
        [PluralName("salamanders")]
        [Experience(100)]
        [Alignment(AlignmentType.DullRed)]
        Salamander,

        [SingularName("scallywag")]
        [PluralName("scallywags")]
        Scallywag,

        [SingularName("scarecrow")]
        [PluralName("scarecrows")]
        Scarecrow,

        [SingularName("scholar")]
        [PluralName("scholars")]
        [Alignment(AlignmentType.Grey)]
        Scholar,

        [SingularName("Scranlin")]
        //CSRTODO: no plural
        [Experience(500)]
        [Alignment(AlignmentType.DullRed)]
        Scranlin,

        [SingularName("scribe")]
        [PluralName("scribes")]
        [Alignment(AlignmentType.DullRed)]
        [Experience(10)]
        Scribe,

        [SingularName("seasoned veteran")]
        [PluralName("seasoned veterans")]
        [Alignment(AlignmentType.Red)]
        SeasonedVeteran,

        [SingularName("sea turtle")]
        [PluralName("sea turtles")]
        SeaTurtle,

        [SingularName("secretary")]
        //CSRTODO: plural?
        Secretary,

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
        [Experience(20)]
        SewerOrc,

        [SingularName("sewer orc guard")]
        [PluralName("sewer orc guards")]
        [Experience(70)]
        [Aggressive]
        SewerOrcGuard,

        [SingularName("sewer orc mummy")]
        [Experience(1000)]
        [Aggressive]
        [InfectsWithDisease]
        SewerOrcMummy,

        [SingularName("sewer rat")]
        [PluralName("sewer rats")]
        [Alignment(AlignmentType.DullRed)]
        [Experience(10)]
        SewerRat,

        [SingularName("sewer wolf")]
        [PluralName("sewer wolves")]
        [Experience(475)]
        SewerWolf,

        [SingularName("Shadow of Incendius")]
        //CSRTODO: no plural
        [MobVisibility(MobVisibility.Hidden)]
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
        [Alignment(AlignmentType.DullBlue)]
        Shepherd,

        [SingularName("shirriff")]
        [PluralName("shirriffs")]
        [Experience(325)]
        [Alignment(AlignmentType.Blue)]
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
        [Alignment(AlignmentType.DullRed)]
        SmallSpider,

        [SingularName("small tree elf")]
        //CSRTODO: plural
        SmallTreeElf,

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

        [SingularName("Snar Slystone")]
        //CSRTODO: no plural
        [Alignment(AlignmentType.Red)]
        SnarSlystone,

        [SingularName("sobbing girl")]
        [PluralName("sobbing girls")]
        SobbingGirl,

        [SingularName("sociopath")]
        [PluralName("sociopaths")]
        Sociopath,

        [SingularName("The Soul of Claudia")]
        SoulOfClaudia,

        [SingularName("spectre")]
        [PluralName("spectres")]
        [Experience(140)]
        Spectre,

        [SingularName("sprite guard")]
        [PluralName("sprite guards")]
        [Experience(120)]
        [Alignment(AlignmentType.DullBlue)]
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

        [SingularName("street busker")]
        [PluralName("street buskers")]
        StreetBusker,

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
        [Alignment(AlignmentType.DullRed)]
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

        [SingularName("toy soldier")]
        [PluralName("toy soldiers")]
        ToySoldier,

        [SingularName("The Town Crier")]
        //CSRTODO: no plural?
        [CannotHarm]
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

        //resists magic (at least wind/earth) but does not resist stun
        [SingularName("Trakard ogre ranger")]
        [PluralName("Trakard ogre rangers")]
        [MobVisibility(MobVisibility.Invisible)]
        [Experience(700)]
        TrakardOgreRanger,

        [SingularName("treant")]
        [PluralName("treants")]
        Treant,

        [SingularName("tree elf")]
        TreeElf,

        [SingularName("tree sprite")]
        [PluralName("tree sprites")]
        TreeSprite,

        [SingularName("triton")]
        [PluralName("tritons")]
        Triton,

        [SingularName("troll")]
        [PluralName("trolls")]
        Troll,

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

        [SingularName("Umbar Sailor")]
        [PluralName("Umbar Sailors")]
        UmbarSailor,

        [SingularName("vagrant")]
        [PluralName("vagrants")]
        [Alignment(AlignmentType.DullBlue)]
        [Experience(6)]
        Vagrant,

        [SingularName("vampire")]
        [PluralName("vampires")]
        Vampire,

        [SingularName("vampire bat")]
        [PluralName("vampire bats")]
        VampireBat,

        [SingularName("vendor")]
        [PluralName("vendors")]
        Vendor,

        [SingularName("Veristria the Librarian")]
        //CSRTODO: no plural
        VeristriaTheLibrarian,

        [SingularName("village woman")]
        [PluralName("village women")]
        VillageWoman,

        [SingularName("villager's ghost")]
        [PluralName("villager's ghosts")]
        VillagersGhost,

        [SingularName("visiting dignitary")]
        //CSRTODO: plural
        VisitingDignitary,

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

        [SingularName("warg")]
        [PluralName("wargs")]
        Warg,

        [SingularName("warrant officer")]
        [PluralName("warrant officers")]
        [Alignment(AlignmentType.DullBlue)]
        WarrantOfficer,

        [SingularName("warrior")]
        [PluralName("warriors")]
        Warrior,

        [SingularName("warrior bard")]
        [PluralName("warrior bards")]
        [Experience(100)]
        [Alignment(AlignmentType.DullRed)]
        WarriorBard,

        [SingularName("water turtle")]
        [PluralName("water turtles")]
        [CannotHarm]
        WaterTurtle,

        [SingularName("weaver")]
        [PluralName("weavers")]
        Weaver,

        [SingularName("werewolf")]
        [PluralName("werewolfs")] //verified 7/28/23
        Werewolf,

        [SingularName("wheat harvester")]
        [PluralName("wheat harvesters")]
        WheatHarvester,

        [SingularName("white knight")]
        [PluralName("white knights")]
        WhiteKnight,

        [SingularName("White Knight")]
        //CSRTODO: plural
        WhiteKnightPerm,

        [SingularName("wight")]
        [PluralName("wights")]
        Wight,

        [SingularName("wildman")]
        [PluralName("wildmen")]
        Wildman,

        [SingularName("William Tasker")]
        //CSRTODO: no plural
        WilliamTasker,

        [SingularName("Wizard of the First Order")]
        //CSRTODO: no plural?
        WizardOfTheFirstOrder,

        [SingularName("wolf")]
        [PluralName("wolves")]
        Wolf,

        [SingularName("wooly mammoth")]
        [PluralName("wooly mammoths")]
        WoolyMammoth,

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
