using System;
using System.Collections.Generic;
using System.Linq;
namespace IsengardClient
{
    public class Entity
    {
        /// <summary>
        /// number of entities present
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// number of sets of that count
        /// </summary>
        public int SetCount { get; set; }
        /// <summary>
        /// type of entity
        /// </summary>
        public EntityType Type
        {
            get
            {
                EntityType ret;
                if (this is MobEntity)
                {
                    ret = EntityType.Mob;
                }
                else if (this is ItemEntity)
                {
                    ret = EntityType.Item;
                }
                else if (this is PlayerEntity)
                {
                    ret = EntityType.Player;
                }
                else if (this is UnknownTypeEntity)
                {
                    ret = EntityType.Unknown;
                }
                else
                {
                    throw new InvalidOperationException();
                }
                return ret;
            }
        }

        public static Entity GetEntity(string fullName, EntityTypeFlags possibleEntityTypes, List<string> errorMessages, HashSet<string> playerNames)
        {
            string remainder = fullName;
            int iSpaceIndex = remainder.IndexOf(' ');
            if (iSpaceIndex < 0)
            {
                return ParseNamedEntity(remainder, possibleEntityTypes, playerNames);
            }
            else if (iSpaceIndex == 0)
            {
                errorMessages.Add("Invalid entity: " + fullName);
                return null; //shouldn't happen
            }
            else
            {
                string firstWord = remainder.Substring(0, iSpaceIndex);
                int? parsedCount = ParseNumberWord(firstWord);
                if (!parsedCount.HasValue)
                {
                    return ParseNamedEntity(remainder, possibleEntityTypes, null);
                }
                int count = parsedCount.Value;
                int remainderLength = remainder.Length;
                int remainderPoint = iSpaceIndex + 1;
                if (remainderLength == remainderPoint)
                {
                    errorMessages.Add("Invalid entity: " + fullName);
                    return null; //shouldn't happen
                }
                remainder = remainder.Substring(remainderPoint).Trim();

                int setCount;
                if (remainder.StartsWith("sets of "))
                {
                    setCount = count;
                    if (remainder.Length == "sets of ".Length)
                    {
                        return null;
                    }
                    remainder = remainder.Substring("sets of ".Length);
                    iSpaceIndex = remainder.IndexOf(' ');
                    if (iSpaceIndex < 0 || iSpaceIndex == 0)
                    {
                        errorMessages.Add("Invalid entity: " + fullName);
                        return null;
                    }
                    else
                    {
                        firstWord = remainder.Substring(0, iSpaceIndex);
                        parsedCount = ParseNumberWord(firstWord);
                        if (!parsedCount.HasValue) //sets of [1] singular thing
                        {
                            Entity e = GetEntity(1, 1, remainder, possibleEntityTypes, errorMessages);
                            e.Count = setCount;
                            return e;
                        }
                        count = parsedCount.Value;
                        remainderLength = remainder.Length;
                        remainderPoint = iSpaceIndex + 1;
                        if (remainderLength == remainderPoint)
                        {
                            errorMessages.Add("Invalid entity: " + fullName);
                            return null;
                        }
                        remainder = remainder.Substring(remainderPoint).Trim();
                    }
                }
                else
                {
                    setCount = 1;
                }
                return GetEntity(setCount, count, remainder, possibleEntityTypes, errorMessages);
            }
        }

        public static Entity ParseNamedEntity(string input, EntityTypeFlags possibleEntityTypes, HashSet<string> playerNames)
        {
            if ((possibleEntityTypes & EntityTypeFlags.Mob) != EntityTypeFlags.None)
            {
                if (MobEntity.SingularMobMapping.TryGetValue(input, out MobTypeEnum mt))
                {
                    return new MobEntity(mt, 1, 1);
                }
            }
            if ((possibleEntityTypes & EntityTypeFlags.Item) != EntityTypeFlags.None)
            {
                if (ItemEntity.SingularItemMapping.TryGetValue(input, out ItemTypeEnum it))
                {
                    return new ItemEntity(it, 1, 1);
                }
            }
            if ((possibleEntityTypes & EntityTypeFlags.Player) != EntityTypeFlags.None)
            {
                if (playerNames != null && playerNames.Contains(input))
                {
                    return new PlayerEntity(input);
                }
            }
            return new UnknownTypeEntity(input, 1, possibleEntityTypes);
        }

        public static int? ParseNumberWord(string input)
        {
            int? count = null;
            if (input == "a" || input == "an" || input == "some")
            {
                count = 1;
            }
            else if (input == "two")
            {
                count = 2;
            }
            else if (input == "three")
            {
                count = 3;
            }
            else if (input == "four")
            {
                count = 4;
            }
            else if (input == "five")
            {
                count = 5;
            }
            else if (input == "six")
            {
                count = 6;
            }
            else if (input == "seven")
            {
                count = 7;
            }
            else if (input == "eight")
            {
                count = 8;
            }
            else if (input == "nine")
            {
                count = 9;
            }
            else if (input == "ten")
            {
                count = 10;
            }
            else if (input == "eleven")
            {
                count = 11;
            }
            else if (input == "twelve")
            {
                count = 12;
            }
            else if (input == "thirteen")
            {
                count = 13;
            }
            else if (input == "fourteen")
            {
                count = 14;
            }
            else if (input == "fifteen")
            {
                count = 15;
            }
            else if (input == "sixteen")
            {
                count = 16;
            }
            else if (input == "seventeen")
            {
                count = 17;
            }
            else if (input == "eighteen")
            {
                count = 18;
            }
            else if (input == "nineteen")
            {
                count = 19;
            }
            else if (input == "twenty")
            {
                count = 20;
            }
            else
            {
                int iCount;
                if (int.TryParse(input, out iCount))
                {
                    count = iCount;
                }
            }
            return count;
        }

        /// <summary>
        /// retrieves an entity
        /// </summary>
        /// <param name="setCount">set count</param>
        /// <param name="count">number of entities</param>
        /// <param name="name">entity name (singular or plural)</param>
        /// <param name="possibleEntityTypes">possible entity types</param>
        /// <param name="errorMessages">error messages</param>
        /// <returns>entity object</returns>
        public static Entity GetEntity(int setCount, int count, string name, EntityTypeFlags possibleEntityTypes, List<string> errorMessages)
        {
            Entity ret = null;
            EntityType? foundEntityType = null;
            if (count == 1)
            {
                if ((possibleEntityTypes & EntityTypeFlags.Mob) != EntityTypeFlags.None)
                {
                    if (MobEntity.SingularMobMapping.TryGetValue(name, out MobTypeEnum mt))
                    {
                        foundEntityType = EntityType.Mob;
                        ret = new MobEntity(mt, count, setCount);
                    }
                }
                if ((possibleEntityTypes & EntityTypeFlags.Item) != EntityTypeFlags.None)
                {
                    if (ItemEntity.SingularItemMapping.TryGetValue(name, out ItemTypeEnum it))
                    {
                        foundEntityType = EntityType.Item;
                        ret = new ItemEntity(it, count, setCount);
                    }
                }
            }
            else
            {
                if ((possibleEntityTypes & EntityTypeFlags.Mob) != EntityTypeFlags.None)
                {
                    if (MobEntity.PluralMobMapping.TryGetValue(name, out MobTypeEnum mt))
                    {
                        foundEntityType = EntityType.Mob;
                        ret = new MobEntity(mt, count, setCount);
                    }
                }
                if ((possibleEntityTypes & EntityTypeFlags.Item) != EntityTypeFlags.None)
                {
                    if (ItemEntity.PluralItemMapping.TryGetValue(name, out ItemTypeEnum it))
                    {
                        foundEntityType = EntityType.Item;
                        ret = new ItemEntity(it, count, setCount);
                    }
                }
            }
            if (foundEntityType.HasValue)
            {
                EntityTypeFlags foundEntityTypeFlags;
                switch (foundEntityType.Value)
                {
                    case EntityType.Item:
                        foundEntityTypeFlags = EntityTypeFlags.Item;
                        break;
                    case EntityType.Mob:
                        foundEntityTypeFlags = EntityTypeFlags.Mob;
                        break;
                    case EntityType.Player:
                        foundEntityTypeFlags = EntityTypeFlags.Player;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if ((foundEntityTypeFlags & possibleEntityTypes) == EntityTypeFlags.None)
                {
                    errorMessages.Add("Invalid entity: " + name + ". Expected " + possibleEntityTypes + " but found " + foundEntityTypeFlags);
                    return null;
                }
            }
            else if (possibleEntityTypes == EntityTypeFlags.Item)
            {
                ret = new UnknownItemEntity(name, count, setCount);
            }
            else if (possibleEntityTypes == EntityTypeFlags.Mob)
            {
                ret = new UnknownMobEntity(name, count, setCount);
            }
            else if (possibleEntityTypes == EntityTypeFlags.Player)
            {
                ret = new PlayerEntity(name);
            }
            else
            {
                return new UnknownTypeEntity(name, count, possibleEntityTypes);
            }
            return ret;
        }
    }

    public class MobEntity : Entity
    {
        public static Dictionary<string, MobTypeEnum> SingularMobMapping = new Dictionary<string, MobTypeEnum>();
        public static Dictionary<string, MobTypeEnum> PluralMobMapping = new Dictionary<string, MobTypeEnum>();

        static MobEntity()
        {
            Type t = typeof(MobTypeEnum);
            foreach (MobTypeEnum nextEnum in Enum.GetValues(t))
            {
                var memberInfos = t.GetMember(nextEnum.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);
                object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SingularNameAttribute), false);
                string sSingular;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sSingular = ((SingularNameAttribute)valueAttributes[0]).Name;
                else
                    sSingular = null;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(PluralNameAttribute), false);
                string sPlural;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sPlural = ((PluralNameAttribute)valueAttributes[0]).Name;
                else
                    sPlural = null;
                AddMob(nextEnum, sSingular, sPlural);
            }
        }

        public MobEntity(MobTypeEnum? mt, int count, int setCount)
        {
            this.MobType = mt;
            this.Count = count;
            this.SetCount = setCount;
        }

        private static void AddMob(MobTypeEnum type, string singular, string plural)
        {
            bool hasSingular = !string.IsNullOrEmpty(singular);
            bool hasPlural = !string.IsNullOrEmpty(plural);
            if (hasSingular && hasPlural)
            {
                if (SingularMobMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                if (PluralMobMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                SingularMobMapping[singular] = type;
                PluralMobMapping[plural] = type;
            }
            else if (hasSingular)
            {
                //CSRTODO: is this even valid?
                if (SingularMobMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                if (PluralMobMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                SingularMobMapping[singular] = type;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public MobTypeEnum? MobType { get; set; }
    }

    public class ItemEntity : Entity
    {
        public static Dictionary<string, ItemTypeEnum> SingularItemMapping = new Dictionary<string, ItemTypeEnum>();
        public static Dictionary<string, ItemTypeEnum> PluralItemMapping = new Dictionary<string, ItemTypeEnum>();

        static ItemEntity()
        {
            Type t = typeof(ItemTypeEnum);
            foreach (ItemTypeEnum nextEnum in Enum.GetValues(t))
            {
                var memberInfos = t.GetMember(nextEnum.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);
                object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SingularNameAttribute), false);
                string sSingular;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sSingular = ((SingularNameAttribute)valueAttributes[0]).Name;
                else
                    sSingular = null;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(PluralNameAttribute), false);
                string sPlural;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sPlural = ((PluralNameAttribute)valueAttributes[0]).Name;
                else
                    sPlural = null;
                AddItem(nextEnum, sSingular, sPlural);
            }
        }

        public ItemEntity(ItemTypeEnum? itemType, int count, int setCount)
        {
            this.ItemType = itemType;
            this.Count = count;
            this.SetCount = setCount;
        }

        public ItemTypeEnum? ItemType { get; set; }

        private static void AddItem(ItemTypeEnum type, string singular, string plural)
        {
            bool hasSingular = !string.IsNullOrEmpty(singular);
            bool hasPlural = !string.IsNullOrEmpty(plural);
            if (hasSingular && hasPlural)
            {
                if (SingularItemMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                if (PluralItemMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                SingularItemMapping[singular] = type;
                PluralItemMapping[plural] = type;
            }
            else if (hasSingular)
            {
                if (SingularItemMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                if (PluralItemMapping.ContainsKey(singular))
                {
                    throw new InvalidOperationException();
                }
                SingularItemMapping[singular] = type;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }

    public class NamedEntity : Entity
    {
        public string Name { get; set; }
    }

    public class PlayerEntity : NamedEntity
    {
        public PlayerEntity(string Name)
        {
            this.Name = Name;
            this.Count = 1;
            this.SetCount = 1;
        }

        /// <summary>
        /// checks if a player name is valid.
        /// Player names always start with an uppercase letter and the remainder
        /// of the letters are lowercase
        /// </summary>
        /// <param name="input">input player name</param>
        /// <returns>true if the player name is valid, false otherwise</returns>
        public static bool IsValidPlayerName(string input)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(input))
            {
                ret = true;
                int index = 0;
                foreach (char c in input)
                {
                    bool ok;
                    if (index == 0)
                        ok = char.IsUpper(c);
                    else
                        ok = char.IsLower(c);
                    if (!ok)
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }
    }

    public class UnknownMobEntity : MobEntity
    {
        public string Name { get; set; }
        public UnknownMobEntity(string Name, int count, int setCount) : base(null, count, setCount)
        {
            this.Name = Name;
        }
    }

    public class UnknownItemEntity : ItemEntity
    {
        public string Name { get; set; }
        public UnknownItemEntity(string Name, int count, int setCount) : base(null, count, setCount)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// entity whose type is not unknown
    /// </summary>
    public class UnknownTypeEntity : NamedEntity
    {
        private EntityTypeFlags PossibleTypes { get; set; }
        public UnknownTypeEntity(string Name, int count, EntityTypeFlags possibleTypes)
        {
            this.Name = Name;
            this.Count = count;
            this.PossibleTypes = possibleTypes;
        }
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

    public enum MobTypeEnum
    {
        [SingularName("alley cat")]
        [PluralName("alley cats")]
        AlleyCat,

        [SingularName("aristocrat")]
        [PluralName("aristocrats")]
        Aristocrat,

        [SingularName("barmaid")]
        [PluralName("barmaids")]
        Barmaid,

        [SingularName("bartender")]
        [PluralName("bartenders")]
        Bartender,

        [SingularName("Big Papa")]
        //CSRTODO: no plural?
        BigPapa,

        [SingularName("Bilbo Baggins")]
        //CSRTODO: no plural
        BilboBaggins,

        [SingularName("black pegasus")]
        //CSRTODO: plural?
        BlackPegasus,

        [SingularName("blue flying snake")]
        [PluralName("blue flying snakes")]
        BlueFlyingSnake,

        [SingularName("book of knowledge")]
        [PluralName("books of knowledge")]
        BookOfKnowledge,

        [SingularName("dancing bear")]
        [PluralName("dancing bears")]
        DancingBear,

        [SingularName("Denethore the Wise")]
        //CSRTODO: no plural?
        DenethoreTheWise,

        [SingularName("Droolie the troll")]
        //CSRTODO: no plural?
        DroolieTheTroll,

        [SingularName("drunk")]
        [PluralName("drunks")]
        Drunk,

        [SingularName("eccentric artist")]
        [PluralName("eccentric artists")]
        EccentricArtist,

        [SingularName("elven guard")]
        [PluralName("elven guards")]
        ElvenGuard,

        [SingularName("Eugene the Executioner")]
        //CSRTODO: no plural?
        EugeneTheExecutioner,

        [SingularName("Faornil the Seer")]
        //CSRTODO: no plural?
        FaornilTheSeer,

        [SingularName("farm cat")]
        [PluralName("farm cats")]
        FarmCat,

        [SingularName("field laborer")]
        [PluralName("field laborers")]
        FieldLaborer,

        [SingularName("Frodo Baggins")]
        //CSRTODO: no plural?
        FrodoBaggins,

        [SingularName("goat")]
        [PluralName("goats")]
        Goat,

        [SingularName("goblin")]
        [PluralName("goblins")]
        Goblin,

        [SingularName("Godfather")]
        //CSRTODO: no plural
        Godfather,

        [SingularName("gray elf")]
        [PluralName("gray elves")]
        GrayElf,

        [SingularName("Gregory Hiester")]
        //CSRTODO: no plural?
        GregoryHiester,

        [SingularName("griffon")]
        [PluralName("griffons")]
        Griffon,

        [SingularName("grizzly bear")]
        [PluralName("grizzly bears")]
        GrizzlyBear,

        [SingularName("guard")]
        [PluralName("guards")]
        Guard,

        [SingularName("Guido")]
        //CSRTODO: no plural?
        Guido,

        [SingularName("guildmaster")]
        [PluralName("guildmasters")]
        Guildmaster,

        [SingularName("Guildmaster Ansette")]
        //CSRTODO: no plural?
        GuildmasterAnsette,

        [SingularName("gypsy-bard")]
        [PluralName("gypsy-bards")]
        GypsyBard,

        [SingularName("gypsy blademaster")]
        [PluralName("gypsy blademasters")]
        GypsyBlademaster,

        [SingularName("gypsy dancer")]
        [PluralName("gypsy dancers")]
        GypsyDancer,

        [SingularName("gypsy fighter")]
        [PluralName("gypsy fighters")]
        GypsyFighter,

        [SingularName("Fallon")]
        //CSRTODO: no plural?
        Fallon,

        [SingularName("half-elf")]
        [PluralName("half-elves")]
        HalfElf,

        [SingularName("hippie")]
        [PluralName("hippies")]
        Hippie,

        [SingularName("hobbit")]
        [PluralName("hobbits")]
        Hobbit,

        [SingularName("hobbitish doctor")]
        [PluralName("hobbitish doctors")]
        HobbitishDoctor,

        [SingularName("hobbit mother")]
        [PluralName("hobbit mothers")]
        HobbitMother,

        [SingularName("Hogoth")]
        //CSRTODO: no plural?
        Hogoth,

        [SingularName("hound dog")]
        [PluralName("hound dogs")]
        HoundDog,

        [SingularName("hunchback servant")]
        [PluralName("hunchback servants")]
        HunchbackServant,

        [SingularName("Igor the Bouncer")]
        //CSRTODO: no plural
        IgorTheBouncer,

        [SingularName("Iorlas the hermit")]
        //CSRTODO: no plural?
        IorlasTheHermit,

        [SingularName("Ixell DeSantis")]
        //CSRTODO: no plural
        IxellDeSantis,

        [SingularName("Kasnar the Guard")]
        //CSRTODO: no plural
        KasnarTheGuard,

        [SingularName("King Brunden")]
        //CSRTODO: no plural
        KingBrunden,

        [SingularName("king's moneychanger")]
        [PluralName("king's moneychangers")]
        KingsMoneychanger,

        [SingularName("knight")]
        [PluralName("knights")]
        Knight,

        [SingularName("laborer")]
        [PluralName("laborers")]
        Laborer,

        [SingularName("large goblin")]
        [PluralName("large goblins")]
        LargeGoblin,

        [SingularName("little mouse")]
        [PluralName("little mice")]
        LittleMouse,

        [SingularName("Madame Nicolov")]
        //CSRTODO: no plural?
        MadameNicolov,

        [SingularName("Manager Mulloy")]
        //CSRTODO: no plural?
        ManagerMulloy,

        [SingularName("Mark Frey")]
        //CSRTODO: no plural?
        MarkFrey,

        [SingularName("Master Jeweler")]
        //CSRTODO: no plural
        MasterJeweler,

        [SingularName("merchant")]
        [PluralName("merchants")]
        Merchant,

        [SingularName("mistress")]
        [PluralName("mistresses")]
        Mistress,

        [SingularName("monk")]
        [PluralName("monks")]
        Monk,

        [SingularName("mosquito")]
        [PluralName("mosquitos")] //CSRTODO: dictionary has plural as either mosquito or mosquitoes
        Mosquito,

        [SingularName("Naugrim")]
        //CSRTODO: no plural
        Naugrim,

        [SingularName("nobleman")]
        [PluralName("noblemen")]
        Nobleman,

        [SingularName("Oliphaunt the Tattoo Artist")]
        //CSRTODO: no plural
        OliphauntTheTattooArtist,

        [SingularName("paladin")]
        [PluralName("paladins")]
        Paladin,

        [SingularName("Pansy Smallburrows")]
        //CSRTODO: no plural?
        PansySmallburrows,

        [SingularName("pixie")]
        [PluralName("pixies")]
        Pixie,

        [SingularName("Prince Brunden")]
        //CSRTODO: no plural?
        PrinceBrunden,

        [SingularName("Prucilla the Groupie")]
        //CSRTODO: no plural?
        PrucillaTheGroupie,

        [SingularName("rabbit")]
        [PluralName("rabbits")]
        Rabbit,

        [SingularName("ram")]
        [PluralName("rams")]
        Ram,

        [SingularName("ranger")]
        [PluralName("rangers")]
        Ranger,

        [SingularName("raving lunatic")]
        [PluralName("raving lunatics")]
        RavingLunatic,

        [SingularName("sailor")]
        [PluralName("sailors")]
        Sailor,

        [SingularName("scholar")]
        [PluralName("scholars")]
        Scholar,

        [SingularName("Scranlin")]
        //CSRTODO: no plural
        Scranlin,

        [SingularName("scribe")]
        [PluralName("scribes")]
        Scribe,

        [SingularName("seasoned veteran")]
        [PluralName("seasoned veterans")]
        SeasonedVeteran,

        [SingularName("Sergeant Grimgall")]
        //CSRTODO: no plural?
        SergeantGrimgall,

        [SingularName("the shepherd")]
        //CSRTODO: no plural?
        Shepherd,

        [SingularName("small spider")]
        [PluralName("small spiders")]
        SmallSpider,

        [SingularName("snarling mutt")]
        [PluralName("snarling mutts")]
        SnarlingMutt,

        [SingularName("The Town Crier")]
        //CSRTODO: no plural?
        TheTownCrier,

        [SingularName("thief")]
        [PluralName("thiefs")]
        Thief,

        [SingularName("tourist")]
        [PluralName("tourists")]
        Tourist,

        [SingularName("traveler")]
        [PluralName("travelers")]
        Traveler,

        [SingularName("traveling horse")]
        [PluralName("traveling horses")]
        TravelingHorse,

        [SingularName("Tyrie")]
        //CSRTODO: no plural
        Tyrie,

        [SingularName("ugly kid")]
        [PluralName("ugly kids")]
        UglyKid,

        [SingularName("vagrant")]
        [PluralName("vagrants")]
        Vagrant,

        [SingularName("villager's ghost")]
        [PluralName("villager's ghosts")]
        VillagersGhost,

        [SingularName("waitress")]
        [PluralName("waitresses")]
        Waitress,

        [SingularName("warrior bard")]
        [PluralName("warrior bards")]
        WarriorBard,

        [SingularName("water turtle")]
        [PluralName("water turtles")]
        WaterTurtle,

        [SingularName("wolf")]
        [PluralName("wolves")]
        Wolf,

        [SingularName("Zathriel the Minstrel")]
        //CSRTODO: no plural?
        ZathrielTheMinstrel,
    }

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

        [SingularName("ancient lyre")]
        [PluralName("ancient lyres")]
        AncientLyre,

        [SingularName("aquamarine potion")]
        [PluralName("aquamarine potions")]
        AquamarinePotion,

        [SingularName("beastmaster's whip")]
        [PluralName("beastmaster's whips")]
        BeastmastersWhip,

        [SingularName("black iron key")]
        [PluralName("black iron keys")]
        BlackIronKey,

        [SingularName("blue bubbly potion")]
        [PluralName("blue bubbly potions")]
        BlueBubblyPotion,

        [SingularName("bone armor")]
        BoneArmor,

        [SingularName("broad sword")]
        [PluralName("broad swords")]
        BroadSword,

        [SingularName("carved ivory key")]
        [PluralName("carved ivory keys")]
        CarvedIvoryKey,

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

        [SingularName("dagger")]
        [PluralName("daggers")]
        Dagger,

        [SingularName("dark green potion")]
        [PluralName("dark green potions")]
        DarkGreenPotion,

        [SingularName("emerald collar")]
        [PluralName("emerald collars")]
        EmeraldCollar,

        [SingularName("engagement ring")]
        [PluralName("engagement rings")]
        EngagementRing,

        [SingularName("gate warning")]
        [PluralName("gate warnings")]
        GateWarning,

        [SingularName("gawdy ear hoop")]
        [PluralName("gawdy ear hoops")]
        GawdyEarHoop,

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

        [SingularName("leather gloves")]
        LeatherGloves,

        [SingularName("little brown jug")]
        [PluralName("little brown jugs")]
        LittleBrownJug,

        [SingularName("lollipop")]
        [PluralName("lollipops")]
        Lollipop,

        [SingularName("marble chess set")]
        [PluralName("marble chess sets")]
        MarbleChessSet,

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

        [SingularName("ork blade")]
        [PluralName("ork blades")]
        OrkBlade,

        [SingularName("out of order sign")]
        [PluralName("out of order signs")]
        OutOfOrderSign,

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

        [SingularName("statuette of Balthazar")]
        [PluralName("statuettes of Balthazar")]
        StatuetteOfBalthazar,

        [SingularName("steel-chain armor")]
        SteelChainArmor,

        [SingularName("T-bone")]
        [PluralName("T-bones")]
        TBone,

        [SingularName("tiger shark leather armor")]
        TigerSharkLeatherArmor,

        [SingularName("training key")]
        [PluralName("training keys")]
        TrainingKey,

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

    public class NameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public class SingularNameAttribute : NameAttribute
    {
        public SingularNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    public class PluralNameAttribute : NameAttribute
    {
        public PluralNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
