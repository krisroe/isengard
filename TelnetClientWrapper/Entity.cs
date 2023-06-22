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
                //since it is not a named entity, it cannot be a player at this point
                possibleEntityTypes = possibleEntityTypes & ~EntityTypeFlags.Player;

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
                if (MobEntity.MobMappingByDisplayName.TryGetValue(input, out StaticMobData smd) && smd.SingularName == input)
                {
                    return new MobEntity(smd.MobType, 1, 1);
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
                    if (MobEntity.MobMappingByDisplayName.TryGetValue(name, out StaticMobData smd) && smd.SingularName == name)
                    {
                        foundEntityType = EntityType.Mob;
                        ret = new MobEntity(smd.MobType, count, setCount);
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
                    if (MobEntity.MobMappingByDisplayName.TryGetValue(name, out StaticMobData smd) && smd.PluralName == name)
                    {
                        foundEntityType = EntityType.Mob;
                        ret = new MobEntity(smd.MobType, count, setCount);
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
        public static Dictionary<string, StaticMobData> MobMappingByDisplayName = new Dictionary<string, StaticMobData>();
        public static Dictionary<MobTypeEnum, StaticMobData> StaticMobData = new Dictionary<MobTypeEnum, StaticMobData>();

        public MobTypeEnum? MobType { get; set; }

        public MobEntity(MobTypeEnum? mt, int count, int setCount)
        {
            this.MobType = mt;
            this.Count = count;
            this.SetCount = setCount;
        }

        static MobEntity()
        {
            Type t = typeof(MobTypeEnum);
            foreach (MobTypeEnum nextEnum in Enum.GetValues(t))
            {
                StaticMobData smd = new StaticMobData();
                smd.MobType = nextEnum;

                var memberInfos = t.GetMember(nextEnum.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);
                object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SingularNameAttribute), false);
                string sSingular;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sSingular = ((SingularNameAttribute)valueAttributes[0]).Name;
                else
                    throw new InvalidOperationException();
                smd.SingularName = sSingular;

                string sSingularSelection = null;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SingularSelectionAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sSingularSelection = ((SingularSelectionAttribute)valueAttributes[0]).Name;
                if (!string.IsNullOrEmpty(sSingularSelection)) smd.SingularSelection = sSingularSelection;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(PluralNameAttribute), false);
                string sPlural;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sPlural = ((PluralNameAttribute)valueAttributes[0]).Name;
                else
                    sPlural = null;
                if (!string.IsNullOrEmpty(sPlural)) smd.PluralName = sPlural;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(AggressiveAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    smd.Aggressive = ((AggressiveAttribute)valueAttributes[0]).Aggressive;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(ExperienceAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    smd.Experience = ((ExperienceAttribute)valueAttributes[0]).Experience;

                bool hasSingular = !string.IsNullOrEmpty(smd.SingularName);
                bool hasPlural = !string.IsNullOrEmpty(smd.PluralName);
                if (hasSingular)
                {
                    MobMappingByDisplayName[smd.SingularName] = smd;
                }
                else
                {
                    throw new InvalidOperationException();
                }
                if (hasPlural)
                {
                    MobMappingByDisplayName[smd.PluralName] = smd;
                }
                StaticMobData[smd.MobType] = smd;
            }
        }

        /// <summary>
        /// pick a word for the mob, choosing the longest word in the singular name.
        /// This isn't necessarily the best since there could be ambiguity with multiple mobs
        /// </summary>
        /// <param name="nextMob">mob to pick</param>
        /// <returns>word for the mob</returns>
        public static IEnumerable<string> GetMobWords(MobTypeEnum nextMob)
        {
            StaticMobData smd = StaticMobData[nextMob];
            string sSingular = smd.SingularSelection;
            if (string.IsNullOrEmpty(sSingular))
            {
                sSingular = smd.SingularName;
            }
            foreach (string s in StringProcessing.PickWords(sSingular))
            {
                yield return s;
            }
        }

        public static string PickMobTextWithinList(MobTypeEnum nextMob, IEnumerable<MobTypeEnum> mobList)
        {
            var wordEnumerator = GetMobWords(nextMob).GetEnumerator();
            wordEnumerator.MoveNext();
            string sWord = wordEnumerator.Current;
            int iCounter = 0;
            foreach (MobTypeEnum nextMobInList in mobList)
            {
                string sSingular = StaticMobData[nextMobInList].SingularName;
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
    }

    public class StaticItemData
    {
        public ItemTypeEnum ItemType { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public WeaponType? WeaponType { get; set; }
        public string SingularName { get; set; }
        public string PluralName { get; set; }
    }

    public class StaticMobData
    {
        public MobTypeEnum MobType { get; set; }
        public string SingularName { get; set; }
        public string SingularSelection { get; set; }
        public string PluralName { get; set; }
        public bool Aggressive { get; set; }
        public int Experience { get; set; }
    }

    public class ItemEntity : Entity
    {
        public static Dictionary<string, StaticItemData> ItemMappingByDisplayName = new Dictionary<string, StaticItemData>();
        public static Dictionary<ItemTypeEnum, StaticItemData> StaticItemData = new Dictionary<ItemTypeEnum, StaticItemData>();

        public ItemTypeEnum? ItemType { get; set; }

        public ItemEntity(ItemTypeEnum? itemType, int count, int setCount)
        {
            this.ItemType = itemType;
            this.Count = count;
            this.SetCount = setCount;
        }

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

                EquipmentType eEquipmentType = EquipmentType.Held;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(EquipmentTypeAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    eEquipmentType = ((EquipmentTypeAttribute)valueAttributes[0]).EquipmentType;
                sid.EquipmentType = eEquipmentType;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(WeaponTypeAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sid.WeaponType = ((WeaponTypeAttribute)valueAttributes[0]).WeaponType;
                if (sid.WeaponType.HasValue) sid.EquipmentType = EquipmentType.Weapon;

                bool hasSingular = !string.IsNullOrEmpty(sid.SingularName);
                bool hasPlural = !string.IsNullOrEmpty(sid.PluralName);
                if (hasSingular)
                {
                    ItemMappingByDisplayName[sid.SingularName] = sid;
                }
                else
                {
                    throw new InvalidOperationException();
                }
                if (hasPlural)
                {
                    ItemMappingByDisplayName[sid.PluralName] = sid;
                }
                StaticItemData[sid.ItemType] = sid;
            }
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

    /// <summary>
    /// base attribute for names
    /// </summary>
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// singular name for items/mobs
    /// </summary>
    public class SingularNameAttribute : NameAttribute
    {
        public SingularNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// singular selection name where the singular name has components that aren't used for selection
    /// </summary>
    public class SingularSelectionAttribute : NameAttribute
    {
        public SingularSelectionAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// plural name for items/mobs
    /// </summary>
    public class PluralNameAttribute : NameAttribute
    {
        public PluralNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// experience gained when killing a mob
    /// </summary>
    public class ExperienceAttribute : Attribute
    {
        public ExperienceAttribute(int Experience)
        {
            this.Experience = Experience;
        }
        public int Experience { get; set; }
    }

    /// <summary>
    /// alignment of a mob
    /// </summary>
    internal class AlignmentAttribute : Attribute
    {
        public AlignmentAttribute(AlignmentType Alignment)
        {
            this.Alignment = Alignment;
        }
        public AlignmentType Alignment { get; set; }
    }

    /// <summary>
    /// whether a mob is aggressive. This is binary and so doesn't capture cases where mobs may be aggressive (e.g. warrior bards)
    /// </summary>
    internal class AggressiveAttribute : Attribute
    {
        public bool Aggressive { get; set; }
        public AggressiveAttribute()
        {
            this.Aggressive = true;
        }
    }

    /// <summary>
    /// equipment type for an item
    /// </summary>
    internal class EquipmentTypeAttribute : Attribute
    {
        public EquipmentType EquipmentType { get; set; }
        public EquipmentTypeAttribute(EquipmentType EquipmentType)
        {
            this.EquipmentType = EquipmentType;
        }
    }

    /// <summary>
    /// weapon type for an item
    /// </summary>
    internal class WeaponTypeAttribute : Attribute
    {
        public WeaponType WeaponType { get; set; }
        public WeaponTypeAttribute(WeaponType WeaponType)
        {
            this.WeaponType = WeaponType;
        }
    }
}
