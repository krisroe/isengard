using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IsengardClient
{
    internal class Entity
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
                bool mustBeItem;
                int? parsedCount = ParseNumberWord(firstWord, expectCapitalized, out mustBeItem);
                if (mustBeItem)
                {
                    possibleEntityTypes = possibleEntityTypes & EntityTypeFlags.Item;
                }
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
                    if (iSpaceIndex == 0)
                    {
                        errorMessages.Add("Invalid entity: " + fullName);
                    }
                    else if (iSpaceIndex > 0)
                    {
                        firstWord = remainder.Substring(0, iSpaceIndex);
                        parsedCount = ParseNumberWord(firstWord, false, out mustBeItem);
                        if (mustBeItem)
                        {
                            possibleEntityTypes = possibleEntityTypes & EntityTypeFlags.Item;
                        }
                        if (parsedCount.HasValue) //sets of [1] singular thing
                        {
                            count = parsedCount.Value;
                            remainder = remainder.Substring(iSpaceIndex + 1).Trim();
                        }
                        else
                        {
                            count = setCount;
                            setCount = 1;
                        }
                    }
                    else
                    {
                        count = setCount;
                        setCount = 1;
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

        public static int? ParseNumberWord(string input, bool expectCapitalized, out bool mustBeItem)
        {
            mustBeItem = false;
            int? count = null;
            if (input == GetCapitalized("some", expectCapitalized))
            {
                mustBeItem = true;
                count = 1;
            }
            else if (input == GetCapitalized("the", expectCapitalized) || input == GetCapitalized("a", expectCapitalized) || input == GetCapitalized("an", expectCapitalized))
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
                    if (ItemEntity.ItemMappingByDisplayName.TryGetValue(name, out StaticItemData it) && (name == it.PluralName || (it.PluralName == null && name == it.SingularName)))
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

    internal class MobEntity : Entity
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

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(MobVisibilityAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    smd.Visibility = ((MobVisibilityAttribute)valueAttributes[0]).MobVisibility;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(AlignmentAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    smd.Alignment = ((AlignmentAttribute)valueAttributes[0]).Alignment;

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
        /// iterate through words for the mob
        /// </summary>
        /// <param name="nextMob">mob to use</param>
        /// <returns>words for the mob</returns>
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

        /// <summary>
        /// retrieves mob info from text
        /// </summary>
        /// <param name="inputMobText">
        /// input mob text. the first word could be a mob enum (if starts with upper case letter).
        /// or could be a mob word (if starts with a lower case letter)
        /// </param>
        /// <param name="mobText">returns the mob word in case of a word-based mob specification</param>
        /// <param name="mobType">returns the mob type in case of a type-based mob specification</param>
        /// <param name="mobCounter">returns the mob counter from the second piece of the text if present. returns 1 if no mob counter was specified.</param>
        /// <returns>true if the input text was successfully processed, false otherwise</returns>
        public static bool GetMobInfo(string inputMobText, out string mobText, out MobTypeEnum? mobType, out int mobCounter)
        {
            mobType = null;
            bool ret = false;
            mobText = string.Empty;
            mobCounter = 1;
            if (!string.IsNullOrEmpty(inputMobText))
            {
                string[] split = inputMobText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int len = split.Length;
                if (len >= 1 && len <= 2)
                {
                    mobText = split[0];
                    if (int.TryParse(mobText, out _)) return false;
                    if (len == 2 && (!int.TryParse(split[1], out mobCounter) || mobCounter < 1)) return false;
                    if (len == 1) mobCounter = 1;
                    ret = true;
                }
            }
            if (!ret) return false;
            if (char.IsUpper(mobText[0]))
            {
                MobTypeEnum mt;
                if (Enum.TryParse(mobText, out mt))
                {
                    mobType = mt;
                    mobText = string.Empty;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }

    internal class StaticMobData
    {
        public MobTypeEnum MobType { get; set; }
        public string SingularName { get; set; }
        public string SingularSelection { get; set; }
        public string PluralName { get; set; }
        public bool Aggressive { get; set; }
        public int Experience { get; set; }
        public MobVisibility Visibility { get; set; }
        public AlignmentType? Alignment { get; set; }

        public static string GetAlignmentString(AlignmentType alignment)
        {
            string sAlign;
            switch (alignment)
            {
                case AlignmentType.IntenseBlue:
                    sAlign = "Bl+";
                    break;
                case AlignmentType.Blue:
                    sAlign = "Bl";
                    break;
                case AlignmentType.DullBlue:
                    sAlign = "Bl-";
                    break;
                case AlignmentType.Grey:
                    sAlign = "Gy";
                    break;
                case AlignmentType.IntenseRed:
                    sAlign = "Rd+";
                    break;
                case AlignmentType.Red:
                    sAlign = "Rd";
                    break;
                case AlignmentType.DullRed:
                    sAlign = "Rd-";
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return sAlign;
        }
    }

    internal class NamedEntity : Entity
    {
        public string Name { get; set; }
    }

    internal class PlayerEntity : NamedEntity
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

    internal class UnknownMobEntity : MobEntity
    {
        public string Name { get; set; }
        public UnknownMobEntity(string Name, int count, int setCount) : base(null, count, setCount)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// entity whose type is not unknown
    /// </summary>
    internal class UnknownTypeEntity : NamedEntity
    {
        private EntityTypeFlags PossibleTypes { get; set; }
        public UnknownTypeEntity(string Name, int count, EntityTypeFlags possibleTypes)
        {
            this.Name = Name;
            this.Count = count;
            this.PossibleTypes = possibleTypes;
        }

        public static IEnumerable<UnknownTypeEntity> SplitUnknownTypeEntity(UnknownTypeEntity input, bool expectSingleItem, List<string> errorMessages)
        {
            for (int i = 0; i < input.Count; i++)
            {
                yield return input;
            }
        }
    }


    internal class EntityChange
    {
        public EntityChange()
        {
            this.Changes = new List<EntityChangeEntry>();
            this.Exits = new List<string>();
            this.MappedExits = new Dictionary<string, Exit>();
        }
        public EntityChangeType ChangeType { get; set; }
        public Room Room { get; set; }
        public List<string> Exits { get; set; }
        /// <summary>
        /// items being changed
        /// </summary>
        public List<EntityChangeEntry> Changes { get; set; }
        /// <summary>
        /// index where the object should be inserted/removed. This is -1 when inserted at the end of the list.
        /// </summary>
        public Dictionary<string, Exit> MappedExits { get; set; }
        public List<Exit> OtherExits = new List<Exit>();

        public bool AddOrRemoveEntityItemFromRoomItems(CurrentEntityInfo entityInfo, ItemEntity nextItemEntity, bool isAdd, EntityChangeEntry changeInfo)
        {
            ItemTypeEnum? specifiedItemType = nextItemEntity.ItemType;
            StaticItemData sid = null;
            if (specifiedItemType.HasValue)
            {
                sid = ItemEntity.StaticItemData[specifiedItemType.Value];
            }
            bool isCoins = sid != null && sid.ItemClass == ItemClass.Coins;
            List<ItemEntity> itemList = entityInfo.CurrentRoomItems;
            int foundRoomItemsIndex = -1;
            int foundUnknownItemsIndex = -1;
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                bool matches = false;
                ItemEntity nextIt = itemList[i];
                if (nextIt.ItemType.HasValue)
                {
                    matches = specifiedItemType == nextIt.ItemType.Value;
                    if (matches && isCoins)
                    {
                        matches = nextItemEntity.Count == nextIt.Count;
                    }
                }
                else if (nextItemEntity is UnknownItemEntity)
                {
                    matches = ((UnknownItemEntity)nextItemEntity).Name == ((UnknownItemEntity)nextIt).Name;
                }
                if (matches)
                {
                    foundRoomItemsIndex = i;
                    break;
                }
            }
            if (foundRoomItemsIndex == -1 && !isAdd && nextItemEntity is UnknownItemEntity)
            {
                UnknownItemEntity uie = (UnknownItemEntity)nextItemEntity;
                for (int i = entityInfo.CurrentUnknownEntities.Count - 1; i >= 0; i--)
                {
                    UnknownTypeEntity ute = entityInfo.CurrentUnknownEntities[i];
                    if (ute.Name == uie.Name)
                    {
                        foundUnknownItemsIndex = i;
                        break;
                    }
                }
            }
            bool effectChange = true;
            if (isAdd)
            {
                changeInfo.RoomItemIndex = entityInfo.FindNewRoomItemInsertionPoint(nextItemEntity);
                changeInfo.RoomItemAction = true;
                if (changeInfo.RoomItemIndex == -1)
                    itemList.Add(nextItemEntity);
                else
                    itemList.Insert(changeInfo.RoomItemIndex, nextItemEntity);
            }
            else
            {
                if (foundRoomItemsIndex >= 0)
                {
                    itemList.RemoveAt(foundRoomItemsIndex);
                    changeInfo.RoomItemIndex = foundRoomItemsIndex;
                    changeInfo.RoomItemAction = false;
                }
                else if (foundUnknownItemsIndex >= 0)
                {
                    entityInfo.CurrentUnknownEntities.RemoveAt(foundUnknownItemsIndex);
                    changeInfo.RoomUnknownEntityIndex = foundUnknownItemsIndex;
                    changeInfo.RoomUnknownEntityAction = false;
                }
                else
                {
                    effectChange = false;
                }
            }
            return effectChange;
        }

        public bool AddOrRemoveEntityItemFromInventory(CurrentEntityInfo inventoryEquipment, ItemEntity nextItemEntity, bool isAdd, EntityChangeEntry changeInfo)
        {
            List<ItemEntity> itemList = inventoryEquipment.InventoryItems;
            int foundIndex = -1;
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                ItemEntity nextIE = itemList[i];
                if (nextIE.ItemType.HasValue == nextItemEntity.ItemType.HasValue)
                {
                    bool matches;
                    if (nextIE.ItemType.HasValue)
                        matches = nextIE.ItemType.Value == nextItemEntity.ItemType.Value;
                    else
                        matches = ((UnknownItemEntity)nextIE).Name == ((UnknownItemEntity)nextItemEntity).Name;
                    if (matches)
                    {
                        foundIndex = i;
                        break;
                    }
                }
            }
            int changeIndex;
            bool effectChange = false;
            if (isAdd)
            {
                changeIndex = inventoryEquipment.FindNewInventoryItemInsertionPoint(nextItemEntity);
                effectChange = true;
                if (changeIndex == -1)
                {
                    itemList.Add(nextItemEntity);
                }
                else
                {
                    itemList.Insert(changeIndex, nextItemEntity);
                }
            }
            else
            {
                changeIndex = foundIndex;
                if (foundIndex >= 0)
                {
                    itemList.RemoveAt(foundIndex);
                    effectChange = true;
                }
            }
            if (effectChange)
            {
                changeInfo.InventoryIndex = changeIndex;
                changeInfo.InventoryAction = isAdd;
            }
            return effectChange;
        }
    }

    internal class CurrentEntityInfo
    {
        public bool UIProcessed { get; set; }
        public Room CurrentRoom { get; set; }
        public Room CurrentRoomUI { get; set; }
        public List<EntityChange> CurrentEntityChanges { get; set; }
        public List<MobTypeEnum> CurrentRoomMobs { get; set; }
        public List<ItemEntity> CurrentRoomItems { get; set; }
        public List<UnknownTypeEntity> CurrentUnknownEntities { get; set; }
        public List<ItemEntity> InventoryItems { get; set; }
        public int? TotalInventoryWeight { get; set; }
        public ItemTypeEnum?[] Equipment { get; set; }
        public List<string> CurrentObviousExits { get; set; }
        public TreeNode tnObviousMobs { get; set; }
        public bool ObviousMobsTNExpanded { get; set; }
        public TreeNode tnObviousItems { get; set; }
        public bool ObviousItemsTNExpanded { get; set; }
        public TreeNode tnObviousExits { get; set; }
        public bool ObviousExitsTNExpanded { get; set; }
        public TreeNode tnOtherExits { get; set; }
        public bool OtherExitsTNExpanded { get; set; }
        public TreeNode tnPermanentMobs { get; set; }
        public bool PermMobsTNExpanded { get; set; }
        public TreeNode tnUnknownEntities { get; set; }
        public bool UnknownEntitiesExpanded { get; set; }

        public CurrentEntityInfo()
        {
            CurrentEntityChanges = new List<EntityChange>();
            CurrentRoomMobs = new List<MobTypeEnum>();
            CurrentRoomItems = new List<ItemEntity>();
            CurrentUnknownEntities = new List<UnknownTypeEntity>();
            InventoryItems = new List<ItemEntity>();
            Equipment = new ItemTypeEnum?[(int)EquipmentSlot.Count];
            CurrentObviousExits = new List<string>();
            ObviousExitsTNExpanded = true;
            ObviousMobsTNExpanded = true;
            ObviousItemsTNExpanded = true;
            OtherExitsTNExpanded = true;
            PermMobsTNExpanded = true;
            UnknownEntitiesExpanded = true;

            tnObviousMobs = new TreeNode("Obvious Mobs");
            tnObviousMobs.Name = "tnObviousMobs";
            tnObviousMobs.Text = "Obvious Mobs";
            tnObviousItems = new TreeNode("Obvious Items");
            tnObviousItems.Name = "tnObviousItems";
            tnObviousItems.Text = "Obvious Items";
            tnObviousExits = new TreeNode("Obvious Exits");
            tnObviousExits.Name = "tnObviousExits";
            tnObviousExits.Text = "Obvious Exits";
            tnOtherExits = new TreeNode("Other Exits");
            tnOtherExits.Name = "tnOtherExits";
            tnOtherExits.Text = "Other Exits";
            tnPermanentMobs = new TreeNode("Permanent Mobs");
            tnPermanentMobs.Name = "tnPermanentMobs";
            tnPermanentMobs.Text = "Permanent Mobs";
            tnUnknownEntities = new TreeNode("Unknown Entities");
            tnUnknownEntities.Name = "tnUnknownEntities";
            tnUnknownEntities.Text = "Unknown Entities";
        }

        public bool GetTopLevelTreeNodeExpanded(TreeNode topLevelTreeNode)
        {
            bool ret = false;
            if (topLevelTreeNode == tnObviousMobs)
            {
                ret = ObviousMobsTNExpanded;
            }
            else if (topLevelTreeNode == tnObviousItems)
            {
                ret = ObviousItemsTNExpanded;
            }
            else if (topLevelTreeNode == tnObviousExits)
            {
                ret = ObviousExitsTNExpanded;
            }
            else if (topLevelTreeNode == tnOtherExits)
            {
                ret = OtherExitsTNExpanded;
            }
            else if (topLevelTreeNode == tnPermanentMobs)
            {
                ret = PermMobsTNExpanded;
            }
            else if (topLevelTreeNode == tnUnknownEntities)
            {
                ret = UnknownEntitiesExpanded;
            }
            return ret;
        }

        public int GetTopLevelTreeNodeLogicalIndex(TreeNode topLevelTreeNode)
        {
            int i = 0;
            if (topLevelTreeNode == tnObviousMobs)
            {
                i = 1;
            }
            else if (topLevelTreeNode == tnObviousItems)
            {
                i = 2;
            }
            else if (topLevelTreeNode == tnObviousExits)
            {
                i = 3;
            }
            else if (topLevelTreeNode == tnOtherExits)
            {
                i = 4;
            }
            else if (topLevelTreeNode == tnPermanentMobs)
            {
                i = 5;
            }
            else if (topLevelTreeNode == tnUnknownEntities)
            {
                i = 6;
            }
            return i;
        }

        public int FindNewMobInsertionPoint(MobTypeEnum newMob)
        {
            string sSingular = MobEntity.StaticMobData[newMob].SingularName;
            bool isCapitalized = char.IsUpper(sSingular[0]);
            int i = 0;
            int iFoundIndex = -1;
            foreach (MobTypeEnum nextMob in CurrentRoomMobs)
            {
                string sNextSingular = MobEntity.StaticMobData[nextMob].SingularName;
                bool nextIsCapitalized = char.IsUpper(sNextSingular[0]);
                bool isBefore = false;
                if (isCapitalized != nextIsCapitalized)
                {
                    isBefore = isCapitalized;
                }
                else
                {
                    isBefore = sSingular.CompareTo(sNextSingular) < 0;
                }
                if (isBefore)
                {
                    iFoundIndex = i;
                    break;
                }
                i++;
            }
            return iFoundIndex;
        }

        public void SetExpandFlag(TreeNode node, bool expanded)
        {
            if (node == tnObviousMobs)
            {
                ObviousMobsTNExpanded = expanded;
            }
            else if (node == tnObviousItems)
            {
                ObviousItemsTNExpanded = expanded;
            }
            else if (node == tnObviousExits)
            {
                ObviousExitsTNExpanded = expanded;
            }
            else if (node == tnOtherExits)
            {
                OtherExitsTNExpanded = expanded;
            }
            else if (node == tnPermanentMobs)
            {
                PermMobsTNExpanded = expanded;
            }
            else if (node == tnUnknownEntities)
            {
                UnknownEntitiesExpanded = expanded;
            }
        }

        public int FindEquipmentRemovalPoint(EquipmentSlot slot)
        {
            int iSlot = (int)slot;
            int previousEntries = 0;
            for (int i = 0; i < iSlot; i++)
            {
                if (Equipment[i] != null)
                {
                    previousEntries++;
                }
            }
            return previousEntries;
        }

        public int FindNewEquipmentInsertionPoint(EquipmentSlot slot)
        {
            int iSlot = (int)slot;
            int previousEntries = 0;
            bool haveAfter = false;
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null)
                {
                    if (i < iSlot)
                    {
                        previousEntries++;
                    }
                    else if (i > iSlot)
                    {
                        haveAfter = true;
                        break;
                    }
                }
            }
            int iRet;
            if (haveAfter)
            {
                iRet = previousEntries;
            }
            else
            {
                iRet = -1;
            }
            return iRet;
        }

        /// <summary>
        /// finds the insertion point for a new room item. Unknown items are placed at the end of the list.
        /// </summary>
        /// <param name="newItemEntity">item to add</param>
        /// <returns>insertion point for the item</returns>
        public int FindNewRoomItemInsertionPoint(ItemEntity newItemEntity)
        {
            int iFoundIndex = -1;
            if (newItemEntity.ItemType.HasValue)
            {
                StaticItemData newSID = ItemEntity.StaticItemData[newItemEntity.ItemType.Value];
                string sSingular = newSID.SingularName;
                bool isCapitalized = char.IsUpper(sSingular[0]);
                int i = 0;
                List<ItemEntity> itemList = CurrentRoomItems;
                foreach (ItemEntity nextItemEntity in itemList)
                {
                    if (nextItemEntity.ItemType.HasValue)
                    {
                        StaticItemData nextSID = ItemEntity.StaticItemData[nextItemEntity.ItemType.Value];
                        bool isBefore = false;
                        if ((nextSID.ItemClass == ItemClass.Coins) != (newSID.ItemClass == ItemClass.Coins))
                        {
                            isBefore = newSID.ItemClass == ItemClass.Coins;
                        }
                        else if (nextItemEntity.ItemType.Value == newItemEntity.ItemType.Value)
                        {
                            isBefore = newItemEntity.Count < nextItemEntity.Count;
                        }
                        else
                        {
                            string sNextSingular = ItemEntity.StaticItemData[nextItemEntity.ItemType.Value].SingularName;
                            bool nextIsCapitalized = char.IsUpper(sNextSingular[0]);
                            if (isCapitalized != nextIsCapitalized)
                            {
                                isBefore = isCapitalized;
                            }
                            else
                            {
                                isBefore = sSingular.CompareTo(sNextSingular) < 0;
                            }
                        }
                        if (isBefore)
                        {
                            iFoundIndex = i;
                            break;
                        }
                    }
                    i++;
                }
            }
            return iFoundIndex;
        }

        public int FindNewInventoryItemInsertionPoint(ItemEntity newItem)
        {
            string sSingular;
            if (newItem.ItemType.HasValue)
                sSingular = ItemEntity.StaticItemData[newItem.ItemType.Value].SingularName;
            else
                sSingular = ((UnknownItemEntity)newItem).Name;
            bool isCapitalized = char.IsUpper(sSingular[0]);
            int i = 0;
            int iFoundIndex = -1;
            List<ItemEntity> itemList = InventoryItems;
            foreach (ItemEntity nextItem in itemList)
            {
                string sNextSingular;
                if (nextItem.ItemType.HasValue)
                    sNextSingular = ItemEntity.StaticItemData[nextItem.ItemType.Value].SingularName;
                else
                    sNextSingular = ((UnknownItemEntity)nextItem).Name;
                bool nextIsCapitalized = char.IsUpper(sNextSingular[0]);
                bool isBefore = false;
                if (isCapitalized != nextIsCapitalized)
                {
                    isBefore = isCapitalized;
                }
                else
                {
                    isBefore = sSingular.CompareTo(sNextSingular) < 0;
                }
                if (isBefore)
                {
                    iFoundIndex = i;
                    break;
                }
                i++;
            }
            return iFoundIndex;
        }

        public static IEnumerable<EquipmentSlot> GetSlotsForEquipmentType(EquipmentType type, bool reverse)
        {
            switch (type)
            {
                case EquipmentType.Torso:
                    yield return EquipmentSlot.Torso;
                    break;
                case EquipmentType.Arms:
                    yield return EquipmentSlot.Arms;
                    break;
                case EquipmentType.Legs:
                    yield return EquipmentSlot.Legs;
                    break;
                case EquipmentType.Neck:
                    yield return EquipmentSlot.Neck;
                    break;
                case EquipmentType.Waist:
                    yield return EquipmentSlot.Waist;
                    break;
                case EquipmentType.Feet:
                    yield return EquipmentSlot.Feet;
                    break;
                case EquipmentType.Head:
                    yield return EquipmentSlot.Head;
                    break;
                case EquipmentType.Hands:
                    yield return EquipmentSlot.Hands;
                    break;
                case EquipmentType.Face:
                    yield return EquipmentSlot.Face;
                    break;
                case EquipmentType.Finger:
                    if (reverse)
                    {
                        yield return EquipmentSlot.Finger2;
                        yield return EquipmentSlot.Finger1;
                    }
                    else
                    {
                        yield return EquipmentSlot.Finger1;
                        yield return EquipmentSlot.Finger2;
                    }
                    break;
                case EquipmentType.Ears:
                    yield return EquipmentSlot.Ears;
                    break;
                case EquipmentType.Holding:
                    yield return EquipmentSlot.Held;
                    break;
                case EquipmentType.Shield:
                    yield return EquipmentSlot.Shield;
                    break;
                case EquipmentType.Wielded:
                    if (reverse)
                    {
                        yield return EquipmentSlot.Weapon2;
                        yield return EquipmentSlot.Weapon1;
                    }
                    else
                    {
                        yield return EquipmentSlot.Weapon1;
                        yield return EquipmentSlot.Weapon2;
                    }
                    break;
            }
        }

        /// <summary>
        /// picks mob text from a mob counter
        /// </summary>
        /// <param name="mobList">list of mobs (for the pick from list case)</param>
        /// <param name="mobLocType">whether to pick the mob from the current room mobs list or the specified list</param>
        /// <param name="mobType">mob type</param>
        /// <param name="mobCounter">mob counter within the list</param>
        /// <param name="reverseOrder">whether to search mobs in forward or backward order</param>
        /// <param name="validateAgainstOtherSources">whether to validate the mob selection against other sources (inventory/equipment/roomitems)</param>
        /// <returns>mob text if selection could be constructed, otherwise blank</returns>
        public string PickMobTextFromMobCounter(List<MobTypeEnum> mobList, MobLocationType mobLocType, MobTypeEnum mobType, int mobCounter, bool reverseOrder, bool validateAgainstOtherSources)
        {
            int iActualIndex = -1;
            int iCounter = 0;
            int iIncrement = reverseOrder ? -1 : 1;
            List<MobTypeEnum> sourceList;
            if (mobLocType == MobLocationType.CurrentRoomMobs)
                sourceList = CurrentRoomMobs;
            else if (mobLocType == MobLocationType.PickFromList)
                sourceList = mobList;
            else
                throw new InvalidOperationException();
            for (int i = reverseOrder ? sourceList.Count - 1 : 0; reverseOrder ? i >= 0 : i < sourceList.Count; i+= iIncrement)
            {
                MobTypeEnum nextMobType = sourceList[i];
                if (nextMobType == mobType)
                {
                    iCounter++;
                    if (mobCounter == iCounter)
                    {
                        iActualIndex = i;
                        break;
                    }
                }
            }
            return iActualIndex < 0 ? null : PickMobTextFromActualIndex(mobType, sourceList, iActualIndex, validateAgainstOtherSources);
        }

        /// <summary>
        /// pick selection text for an inventory/equipment/room item, assumes the entity lock is present
        /// </summary>
        /// <param name="locationType">where to look for the item</param>
        /// <param name="itemType">item type</param>
        /// <param name="itemCounter">item counter of that type</param>
        /// <param name="reverseOrder">whether to search in reverse order</param>
        /// <param name="validateAgainstOtherSources">whether to validate for duplicates in higher priority locations</param>
        /// <returns>selection text for the item</returns>
        public string PickItemTextFromItemCounter(ItemLocationType locationType, ItemTypeEnum itemType, int itemCounter, bool reverseOrder, bool validateAgainstOtherSources)
        {
            int iActualIndex = -1;
            int iCounter = 0;
            int iIncrement = reverseOrder ? -1 : 1;
            if (locationType == ItemLocationType.Inventory)
            {
                for (int i = reverseOrder ? InventoryItems.Count - 1 : 0; reverseOrder ? i >= 0 : i < InventoryItems.Count; i += iIncrement)
                {
                    if (InventoryItems[i].ItemType == itemType)
                    {
                        iCounter++;
                        if (itemCounter == iCounter)
                        {
                            iActualIndex = i;
                            break;
                        }
                    }
                }
            }
            else if (locationType == ItemLocationType.Equipment)
            {
                for (int i = reverseOrder ? Equipment.Length - 1 : 0; reverseOrder ? i >= 0 : i < Equipment.Length; i += iIncrement)
                {
                    if (Equipment[i] == itemType)
                    {
                        iCounter++;
                        if (itemCounter == iCounter)
                        {
                            iActualIndex = i;
                            break;
                        }
                    }
                }
            }
            else if (locationType == ItemLocationType.Room)
            {
                for (int i = reverseOrder ? CurrentRoomItems.Count - 1 : 0; reverseOrder ? i >= 0 : i < CurrentRoomItems.Count; i += iIncrement)
                {
                    ItemTypeEnum? it = CurrentRoomItems[i].ItemType;
                    if (it.HasValue && it.Value == itemType)
                    {
                        iCounter++;
                        if (itemCounter == iCounter)
                        {
                            iActualIndex = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
            return iActualIndex < 0 ? null : PickItemTextFromActualIndex(locationType, itemType, iActualIndex, validateAgainstOtherSources);
        }

        /// <summary>
        /// pick selectio text for a mob, assumes the entity lock is present
        /// </summary>
        /// <param name="mobType">mob type</param>
        /// <param name="sourceList">source mob list (either room mobs or permanent mobs)</param>
        /// <param name="iActualIndex">actual index in the list to look for the mob at</param>
        /// <param name="validateAgainstOtherSources">whether to validate for duplicates in higher priority locations</param>
        /// <returns>selection text for the mob</returns>
        public string PickMobTextFromActualIndex(MobTypeEnum mobType, List<MobTypeEnum> sourceList, int iActualIndex, bool validateAgainstOtherSources)
        {
            string ret = null;
            foreach (string word in MobEntity.GetMobWords(mobType))
            {
                string sSingular;

                //find word index within the list of mobs
                int iCounter = 0;
                for (int i = 0; i <= iActualIndex; i++)
                {
                    MobTypeEnum eMobType = sourceList[i];
                    bool matches = false;
                    if (eMobType == mobType)
                    {
                        matches = true;
                    }
                    else
                    {
                        sSingular = MobEntity.StaticMobData[eMobType].SingularName;
                        foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                        {
                            if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                            {
                                matches = true;
                                break;
                            }
                        }
                    }
                    if (matches)
                    {
                        iCounter++;
                    }
                }

                bool isDuplicate = false;
                int iDuplicateCounter;
                if (validateAgainstOtherSources)
                {
                    iDuplicateCounter = 0;
                    foreach (ItemEntity nextItemEntity in InventoryItems)
                    {
                        if (nextItemEntity.ItemType.HasValue)
                        {
                            sSingular = ItemEntity.StaticItemData[nextItemEntity.ItemType.Value].SingularName;
                            foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                            {
                                if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase) && ++iDuplicateCounter == iCounter)
                                    break;
                            }
                            if (iDuplicateCounter == iCounter) break;
                        }
                    }
                    isDuplicate = iDuplicateCounter == iCounter;

                    if (!isDuplicate)
                    {
                        iDuplicateCounter = 0;
                        foreach (ItemTypeEnum? nextItem in Equipment)
                        {
                            if (nextItem.HasValue)
                            {
                                sSingular = ItemEntity.StaticItemData[nextItem.Value].SingularName;
                                foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                                {
                                    if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase) && ++iDuplicateCounter == iCounter)
                                        break;
                                }
                                if (iDuplicateCounter == iCounter) break;
                            }
                        }
                        isDuplicate = iDuplicateCounter == iCounter;
                    }

                    if (!isDuplicate)
                    {
                        iDuplicateCounter = 0;
                        foreach (ItemEntity nextItem in CurrentRoomItems)
                        {
                            if (nextItem.ItemType.HasValue)
                            {
                                sSingular = ItemEntity.StaticItemData[nextItem.ItemType.Value].SingularName;
                                foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                                {
                                    if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase) && ++iDuplicateCounter == iCounter)
                                        break;
                                }
                                if (iDuplicateCounter == iCounter) break;
                            }
                        }
                        isDuplicate = iDuplicateCounter == iCounter;
                    }
                }

                if (!isDuplicate)
                {
                    if (iCounter == 1)
                        ret = word;
                    else
                        ret = word + " " + iCounter;
                    break;
                }
            }
            return ret;
        }

        public string PickItemTextFromActualIndex(ItemLocationType locationType, ItemTypeEnum itemType, int iActualIndex, bool validateAgainstOtherSources)
        {
            string ret = null;
            foreach (string word in ItemEntity.GetItemWords(itemType))
            {
                string sSingular;

                //find word index within the list of items
                int iCounter = 0;
                for (int i = 0; i <= iActualIndex; i++)
                {
                    ItemTypeEnum? eItem;
                    if (locationType == ItemLocationType.Inventory)
                    {
                        eItem = InventoryItems[i].ItemType;
                    }
                    else if (locationType == ItemLocationType.Equipment)
                    {
                        eItem = Equipment[i];
                    }
                    else if (locationType == ItemLocationType.Room)
                    {
                        eItem = CurrentRoomItems[i].ItemType;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    if (eItem.HasValue)
                    {
                        ItemTypeEnum eItemValue = eItem.Value;
                        bool matches = false;
                        if (eItemValue == itemType)
                        {
                            matches = true;
                        }
                        else
                        {
                            sSingular = ItemEntity.StaticItemData[eItemValue].SingularName;
                            foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                            {
                                if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                                {
                                    matches = true;
                                    break;
                                }
                            }
                        }
                        if (matches)
                        {
                            iCounter++;
                        }
                    }
                }

                bool isDuplicate = false;
                int iDuplicateCounter;
                if (validateAgainstOtherSources)
                {
                    if (locationType != ItemLocationType.Inventory)
                    {
                        //validate for a duplicate in inventory
                        iDuplicateCounter = 0;
                        foreach (ItemEntity nextItemEntity in InventoryItems)
                        {
                            if (nextItemEntity.ItemType.HasValue)
                            {
                                sSingular = ItemEntity.StaticItemData[nextItemEntity.ItemType.Value].SingularName;
                                foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                                {
                                    if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase) && ++iDuplicateCounter == iCounter)
                                        break;
                                }
                                if (iDuplicateCounter == iCounter) break;
                            }
                        }
                        isDuplicate = iDuplicateCounter == iCounter;

                        //validate for a duplicate in equipment
                        if (!isDuplicate && locationType != ItemLocationType.Equipment)
                        {
                            iDuplicateCounter = 0;
                            foreach (ItemTypeEnum? nextItem in Equipment)
                            {
                                if (nextItem.HasValue)
                                {
                                    sSingular = ItemEntity.StaticItemData[nextItem.Value].SingularName;
                                    foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                                    {
                                        if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase) && ++iDuplicateCounter == iCounter)
                                            break;
                                    }
                                    if (iDuplicateCounter == iCounter) break;
                                }
                            }
                            isDuplicate = iDuplicateCounter == iCounter;
                        }
                    }
                }

                if (!isDuplicate)
                {
                    if (iCounter == 1)
                        ret = word;
                    else
                        ret = word + " " + iCounter;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// gets the total inventory count for an item type. assumes the entity lock.
        /// </summary>
        /// <param name="itemType">item type</param>
        /// <returns>number of those item type possessed</returns>
        public int GetTotalInventoryCount(ItemTypeEnum itemType)
        {
            int iCount = 0;
            foreach (ItemEntity nextItemEntity in InventoryItems)
            {
                if (itemType == nextItemEntity.ItemType) iCount++;
            }
            foreach (ItemTypeEnum? nextItemType in Equipment)
            {
                if (nextItemType == itemType) iCount++;
            }
            return iCount;
        }

        public bool InventoryContainsItemType(ItemTypeEnum itemType)
        {
            bool ret = false;
            foreach (ItemEntity ie in InventoryItems)
            {
                if (itemType == ie.ItemType)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// gets the total room items count for an item type, assumes the entity lock
        /// </summary>
        /// <param name="itemType">item type</param>
        /// <returns>number of that item type in the room</returns>
        public int GetTotalRoomItemsCount(ItemTypeEnum itemType)
        {
            int iCount = 0;
            foreach (ItemEntity ie in CurrentRoomItems)
            {
                if (ie.ItemType == itemType)
                {
                    iCount++;
                }
            }
            return iCount;
        }

        /// <summary>
        /// enumerates inventory and equipment items, assumes the entity lock
        /// </summary>
        /// <returns>inventory and equipment items</returns>
        public IEnumerable<ItemTypeEnum> EnumerateInventoryAndEquipmentItems()
        {
            foreach (ItemEntity next in InventoryItems)
            {
                if (next.ItemType.HasValue)
                {
                    yield return next.ItemType.Value;
                }
            }
            foreach (ItemTypeEnum? next in Equipment)
            {
                if (next.HasValue)
                {
                    yield return next.Value;
                }
            }
        }

        public bool HasPotionForSpell(SpellsEnum spell, out ItemTypeEnum? foundItem, out bool? inInventory)
        {
            bool ret = false;
            foundItem = null;
            inInventory = null;

            foreach (ItemEntity next in InventoryItems)
            {
                if (next.ItemType.HasValue)
                {
                    StaticItemData sid = ItemEntity.StaticItemData[next.ItemType.Value];
                    if (sid.ItemClass == ItemClass.Potion && sid.Spell.Value == spell)
                    {
                        foundItem = next.ItemType.Value;
                        inInventory = true;
                        ret = true;
                    }
                }
            }

            foreach (ItemTypeEnum? next in Equipment)
            {
                if (next.HasValue)
                {
                    StaticItemData sid = ItemEntity.StaticItemData[next.Value];
                    if (sid.ItemClass == ItemClass.Potion && sid.Spell.Value == spell)
                    {
                        foundItem = next.Value;
                        inInventory = false;
                        ret = true;
                    }
                }
            }

            return ret;
        }
    }
    internal class EntityChangeEntry
    {
        /// <summary>
        /// unknown type entity
        /// </summary>
        public UnknownTypeEntity UnknownTypeEntity { get; set; }
        /// <summary>
        /// type of item being updated
        /// </summary>
        public ItemEntity Item { get; set; }
        /// <summary>
        /// type of mob being updated
        /// </summary>
        public MobTypeEnum? MobType { get; set; }
        /// <summary>
        /// inventory index to add/remove at
        /// </summary>
        public int InventoryIndex { get; set; }
        /// <summary>
        /// true to add to inventory, false to remove from inventory, null for no action
        /// </summary>
        public bool? InventoryAction { get; set; }
        /// <summary>
        /// equipment index to add/remove at
        /// </summary>
        public int EquipmentIndex { get; set; }
        /// <summary>
        /// true to add to equipment, false to remove from equipment, null for no action
        /// </summary>
        public bool? EquipmentAction { get; set; }
        /// <summary>
        /// room item index to add/remove at
        /// </summary>
        public int RoomItemIndex { get; set; }
        /// <summary>
        /// true to add to the room items, false to remove from room items, null for no action
        /// </summary>
        public bool? RoomItemAction { get; set; }
        /// <summary>
        /// room mob index to add/remove at
        /// </summary>
        public int RoomMobIndex { get; set; }
        /// <summary>
        /// true to add to the room mobs, false to remove from room mobs, null for no action
        /// </summary>
        public bool? RoomMobAction { get; set; }
        /// <summary>
        /// true to add to unknown room entities, false to remove from unknown room entities, null for no action
        /// </summary>
        public bool? RoomUnknownEntityAction { get; set; }
        /// <summary>
        /// room unknown entity index to add/remove at
        /// </summary>
        public int RoomUnknownEntityIndex { get; set; }
    }
}
