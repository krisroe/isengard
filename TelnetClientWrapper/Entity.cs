﻿using System;
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

        public static Entity GetEntity(string fullName, EntityTypeFlags possibleEntityTypes, List<string> errorMessages, HashSet<string> playerNames, bool expectCapitalized)
        {
            string remainder = fullName;

            while (remainder.EndsWith(" (H)") || remainder.EndsWith(" (F)") || remainder.EndsWith(" (M)") || remainder.EndsWith(" (+1)") || remainder.EndsWith(" (+2)") || remainder.EndsWith(" (+3)") || remainder.EndsWith(" (+4)") || remainder.EndsWith(" (+5)"))
            {
                bool isHoned = remainder.EndsWith(" (H)");
                bool isForged = remainder.EndsWith(" (F)");
                bool isMagic = remainder.EndsWith(" (M)");
                bool isPlus = remainder.EndsWith(" (+1)") || remainder.EndsWith(" (+2)") || remainder.EndsWith(" (+3)") || remainder.EndsWith(" (+4)") || remainder.EndsWith(" (+5)");
                int extraLength = isPlus ? " (+1)".Length : " (H)".Length;
                bool mustBeItem = isHoned || isForged || isPlus;
                bool mustBeItemOrMob = isMagic;
                if (mustBeItem)
                {
                    if ((possibleEntityTypes & EntityTypeFlags.Item) != EntityTypeFlags.Item)
                        return new UnknownTypeEntity(fullName, 1, possibleEntityTypes);
                    else
                        possibleEntityTypes = EntityTypeFlags.Item;
                }
                else if (mustBeItemOrMob)
                {
                    if (((possibleEntityTypes & EntityTypeFlags.Item) != EntityTypeFlags.Item) &&
                        ((possibleEntityTypes & EntityTypeFlags.Mob) != EntityTypeFlags.Mob))
                    {
                        return new UnknownTypeEntity(fullName, 1, possibleEntityTypes);
                    }
                    else
                    {
                        possibleEntityTypes = possibleEntityTypes & (EntityTypeFlags.Item | EntityTypeFlags.Mob);
                    }
                }
                else
                {
                    return null;
                }
                if (remainder.Length == extraLength)
                    return null;
                else
                    remainder = remainder.Substring(0, remainder.Length - extraLength);
            }

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
                int? parsedCount = ParseNumberWord(firstWord, expectCapitalized);
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
                        parsedCount = ParseNumberWord(firstWord, false);
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
            if (input == "Somebody" & ((possibleEntityTypes & EntityTypeFlags.Player) != EntityTypeFlags.None))
            {
                return new PlayerEntity();
            }
            if ((possibleEntityTypes & EntityTypeFlags.Mob) != EntityTypeFlags.None)
            {
                if (MobEntity.SingularMobMapping.TryGetValue(input, out MobTypeEnum mt))
                {
                    return new MobEntity(mt, 1, 1);
                }
            }
            if ((possibleEntityTypes & EntityTypeFlags.Item) != EntityTypeFlags.None)
            {
                if (ItemEntity.ItemMappingByDisplayName.TryGetValue(input, out StaticItemData it) && it.SingularName == input)
                {
                    return new ItemEntity(it.ItemType, 1, 1);
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

        private static string GetCapitalized(string input, bool expectCapitalized)
        {
            string ret;
            if (expectCapitalized)
            {
                ret = char.ToUpper(input[0]).ToString();
                if (input.Length > 1)
                {
                    ret += input.Substring(1);
                }
            }
            else
            {
                ret = input;
            }
            return ret;
        }

        public static int? ParseNumberWord(string input, bool expectCapitalized)
        {
            int? count = null;
            if (input == GetCapitalized("the", expectCapitalized) || input == GetCapitalized("a", expectCapitalized) || input == GetCapitalized("an", expectCapitalized) || input == GetCapitalized("some", expectCapitalized))
            {
                count = 1;
            }
            else if (input == GetCapitalized("two", expectCapitalized))
            {
                count = 2;
            }
            else if (input == GetCapitalized("three", expectCapitalized))
            {
                count = 3;
            }
            else if (input == GetCapitalized("four", expectCapitalized))
            {
                count = 4;
            }
            else if (input == GetCapitalized("five", expectCapitalized))
            {
                count = 5;
            }
            else if (input == GetCapitalized("six", expectCapitalized))
            {
                count = 6;
            }
            else if (input == GetCapitalized("seven", expectCapitalized))
            {
                count = 7;
            }
            else if (input == GetCapitalized("eight", expectCapitalized))
            {
                count = 8;
            }
            else if (input == GetCapitalized("nine", expectCapitalized))
            {
                count = 9;
            }
            else if (input == GetCapitalized("ten", expectCapitalized))
            {
                count = 10;
            }
            else if (input == GetCapitalized("eleven", expectCapitalized))
            {
                count = 11;
            }
            else if (input == GetCapitalized("twelve", expectCapitalized))
            {
                count = 12;
            }
            else if (input == GetCapitalized("thirteen", expectCapitalized))
            {
                count = 13;
            }
            else if (input == GetCapitalized("fourteen", expectCapitalized))
            {
                count = 14;
            }
            else if (input == GetCapitalized("fifteen", expectCapitalized))
            {
                count = 15;
            }
            else if (input == GetCapitalized("sixteen", expectCapitalized))
            {
                count = 16;
            }
            else if (input == GetCapitalized("seventeen", expectCapitalized))
            {
                count = 17;
            }
            else if (input == GetCapitalized("eighteen", expectCapitalized))
            {
                count = 18;
            }
            else if (input == GetCapitalized("nineteen", expectCapitalized))
            {
                count = 19;
            }
            else if (input == GetCapitalized("twenty", expectCapitalized))
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
                    if (ItemEntity.ItemMappingByDisplayName.TryGetValue(name, out StaticItemData it) && name == it.SingularName)
                    {
                        foundEntityType = EntityType.Item;
                        ret = new ItemEntity(it.ItemType, count, setCount);
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
                    if (ItemEntity.ItemMappingByDisplayName.TryGetValue(name, out StaticItemData it) && name == it.PluralName)
                    {
                        foundEntityType = EntityType.Item;
                        ret = new ItemEntity(it.ItemType, count, setCount);
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
        public static Dictionary<MobTypeEnum, string> MobToSingularMappingForSelection = new Dictionary<MobTypeEnum, string>();
        public static Dictionary<MobTypeEnum, string> MobToSingularMapping = new Dictionary<MobTypeEnum, string>();
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
                    throw new InvalidOperationException();
                MobToSingularMapping[nextEnum] = sSingular;

                string sSingularSelection = null;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SingularSelectionAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sSingularSelection = ((SingularSelectionAttribute)valueAttributes[0]).Name;
                if (!string.IsNullOrEmpty(sSingularSelection)) MobToSingularMappingForSelection[nextEnum] = sSingularSelection;

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

        /// <summary>
        /// pick a word for the mob, choosing the longest word in the singular name.
        /// This isn't necessarily the best since there could be ambiguity with multiple mobs
        /// </summary>
        /// <param name="nextMob">mob to pick</param>
        /// <returns>word for the mob</returns>
        public static string PickWordForMob(MobTypeEnum nextMob)
        {
            string sBestWord;
            if (!MobToSingularMappingForSelection.TryGetValue(nextMob, out sBestWord))
            {
                sBestWord = PickWord(MobToSingularMapping[nextMob]);
            }
            return sBestWord;
        }

        public static string PickWord(string input)
        {
            string sBestWord = string.Empty;
            string[] sWords = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sNextWord in sWords)
            {
                if (string.IsNullOrEmpty(sBestWord) || sNextWord.Length > sBestWord.Length)
                {
                    sBestWord = sNextWord;
                }
            }
            return sBestWord;
        }

        public static string PickMobTextWithinList(MobTypeEnum nextMob, IEnumerable<MobTypeEnum> mobList)
        {
            string sWord = PickWordForMob(nextMob);
            int iCounter = 0;
            foreach (MobTypeEnum nextMobInList in mobList)
            {
                string sSingular = MobToSingularMapping[nextMobInList];
                foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                {
                    if (nextWord.StartsWith(sWord, StringComparison.OrdinalIgnoreCase))
                    {
                        iCounter++;
                        break;
                    }
                }
            }
            if (iCounter > 1)
            {
                sWord += " " + iCounter;
            }
            return sWord;
        }

        public static IEnumerable<MobTypeEnum> IterateThroughMobs(List<MobTypeEnum> mobs, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return mobs[i];
            }
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

    public class StaticItemData
    {
        public StaticItemData()
        {
        }

        public ItemTypeEnum ItemType { get; set; }
        public EquipmentType? EquipmentType { get; set; }
        public WeaponType? WeaponType { get; set; }
        public string SingularName { get; set; }
        public string PluralName { get; set; }
    }

    public class ItemEntity : Entity
    {
        public static Dictionary<string, StaticItemData> ItemMappingByDisplayName = new Dictionary<string, StaticItemData>();
        public static Dictionary<ItemTypeEnum, StaticItemData> StaticItemData = new Dictionary<ItemTypeEnum, StaticItemData>();

        static ItemEntity()
        {
            Type t = typeof(ItemTypeEnum);
            foreach (ItemTypeEnum nextEnum in Enum.GetValues(t))
            {
                StaticItemData sid = new StaticItemData();
                sid.ItemType = nextEnum;
                var memberInfos = t.GetMember(nextEnum.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);

                object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SingularNameAttribute), false);
                string sSingular;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sSingular = ((SingularNameAttribute)valueAttributes[0]).Name;
                else
                    throw new InvalidOperationException();
                sid.SingularName = sSingular;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(PluralNameAttribute), false);
                string sPlural;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sPlural = ((PluralNameAttribute)valueAttributes[0]).Name;
                else
                    sPlural = null;
                sid.PluralName = sPlural;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(EquipmentTypeAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sid.EquipmentType = ((EquipmentTypeAttribute)valueAttributes[0]).EquipmentType;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(WeaponTypeAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sid.WeaponType = ((WeaponTypeAttribute)valueAttributes[0]).WeaponType;
                if (sid.WeaponType.HasValue)
                {
                    if (sid.EquipmentType.HasValue && sid.EquipmentType.Value != EquipmentType.Weapon)
                    {
                        throw new InvalidOperationException();
                    }
                    sid.EquipmentType = EquipmentType.Weapon;
                }

                AddItem(sid);
            }
        }

        public ItemEntity(ItemTypeEnum? itemType, int count, int setCount)
        {
            this.ItemType = itemType;
            this.Count = count;
            this.SetCount = setCount;
        }

        public ItemTypeEnum? ItemType { get; set; }

        private static void AddItem(StaticItemData itemData)
        {
            bool hasSingular = !string.IsNullOrEmpty(itemData.SingularName);
            bool hasPlural = !string.IsNullOrEmpty(itemData.PluralName);
            if (hasSingular)
            {
                ItemMappingByDisplayName[itemData.SingularName] = itemData;
            }
            else
            {
                throw new InvalidOperationException();
            }
            if (hasPlural)
            {
                ItemMappingByDisplayName[itemData.PluralName] = itemData;
            }
            StaticItemData[itemData.ItemType] = itemData;
        }
    }

    public class NamedEntity : Entity
    {
        public string Name { get; set; }
    }

    public class PlayerEntity : NamedEntity
    {
        /// <summary>
        /// creates an unknown player entity
        /// </summary>
        public PlayerEntity()
        {
            this.Name = string.Empty;
            this.Count = 1;
            this.SetCount = 1;
        }

        /// <summary>
        /// creates a player entity with the given name
        /// </summary>
        /// <param name="Name">input name</param>
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

    public class SingularSelectionAttribute : NameAttribute
    {
        public SingularSelectionAttribute(string Name)
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

    public class ExperienceAttribute : Attribute
    {
        public ExperienceAttribute(int Experience)
        {
            this.Experience = Experience;
        }
        public int Experience { get; set; }
    }

    internal class AlignmentAttribute : Attribute
    {
        public AlignmentAttribute(AlignmentType Alignment)
        {
            this.Alignment = Alignment;
        }
        public AlignmentType Alignment { get; set; }
    }

    internal class EquipmentTypeAttribute : Attribute
    {
        public EquipmentType EquipmentType { get; set; }
        public EquipmentTypeAttribute(EquipmentType EquipmentType)
        {
            this.EquipmentType = EquipmentType;
        }
    }

    internal class WeaponTypeAttribute : Attribute
    {
        public WeaponType WeaponType { get; set; }
        public WeaponTypeAttribute(WeaponType WeaponType)
        {
            this.WeaponType = WeaponType;
        }
    }
}
