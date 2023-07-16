using System;
using System.Collections.Generic;
using System.Linq;
namespace IsengardClient.Backend
{
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
                ItemClass eItemClass = ItemClass.Other;
                SpellsEnum? eSpell = null;
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

                string sSingularSelection = null;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SingularSelectionAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sSingularSelection = ((SingularSelectionAttribute)valueAttributes[0]).Name;
                if (!string.IsNullOrEmpty(sSingularSelection)) sid.SingularSelection = sSingularSelection;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(PluralNameAttribute), false);
                string sPlural;
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sPlural = ((PluralNameAttribute)valueAttributes[0]).Name;
                else
                    sPlural = null;
                sid.PluralName = sPlural;

                EquipmentType eEquipmentType = EquipmentType.Holding;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(EquipmentTypeAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    eEquipmentType = ((EquipmentTypeAttribute)valueAttributes[0]).EquipmentType;
                sid.EquipmentType = eEquipmentType;
                if (eEquipmentType != EquipmentType.Holding)
                {
                    eItemClass = ItemClass.Equipment;
                }

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(WeaponTypeAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                    sid.WeaponType = ((WeaponTypeAttribute)valueAttributes[0]).WeaponType;
                if (sid.WeaponType.HasValue)
                {
                    sid.EquipmentType = EquipmentType.Wielded;
                    eItemClass = ItemClass.Weapon;
                }

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(PotionAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                {
                    eSpell = ((PotionAttribute)valueAttributes[0]).Spell;
                    eItemClass = ItemClass.Potion;
                }
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(ScrollAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                {
                    eSpell = ((ScrollAttribute)valueAttributes[0]).Spell;
                    eItemClass = ItemClass.Scroll;
                }
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(WandAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                {
                    eSpell = ((WandAttribute)valueAttributes[0]).Spell;
                    eItemClass = ItemClass.Wand;
                }
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(UseAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                {
                    eSpell = ((UseAttribute)valueAttributes[0]).Spell;
                    eItemClass = ItemClass.Usable;
                }

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(ItemClassAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0) eItemClass = ((ItemClassAttribute)valueAttributes[0]).ItemClass;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(WeightAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0) sid.Weight = ((WeightAttribute)valueAttributes[0]).Pounds;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(ArmorClassAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0) sid.ArmorClass = ((ArmorClassAttribute)valueAttributes[0]).ArmorClass;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DisallowedClassesAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0) sid.DisallowedClasses = ((DisallowedClassesAttribute)valueAttributes[0]).Classes;

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SellableAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                {
                    SellableAttribute attr = (SellableAttribute)valueAttributes[0];
                    sid.Sellable = attr.Sellable;
                    sid.SellGold = attr.Gold;
                }

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
                sid.ItemClass = eItemClass;
                sid.Spell = eSpell;
                StaticItemData[sid.ItemType] = sid;
            }
        }

        /// <summary>
        /// iterate through words for the item
        /// </summary>
        /// <param name="nextItem">item to use</param>
        /// <returns>words for the item</returns>
        public static IEnumerable<string> GetItemWords(ItemTypeEnum nextItem)
        {
            StaticItemData sid = StaticItemData[nextItem];
            string sSingular = sid.SingularSelection;
            if (string.IsNullOrEmpty(sSingular))
            {
                sSingular = sid.SingularName;
            }
            foreach (string s in StringProcessing.PickWords(sSingular))
            {
                yield return s;
            }
        }

        /// <summary>
        /// retrieves the display for an item (for display of room items or inventory).
        /// Item type is assumed to have a value
        /// </summary>
        /// <param name="itemType">item type</param>
        /// <param name="count">count, relevant for coins items</param>
        /// <returns>text for the item</returns>
        public string GetItemString()
        {
            string sText;
            if (this is UnknownItemEntity)
            {
                sText = ((UnknownItemEntity)this).Name;
            }
            else
            {
                StaticItemData sid = StaticItemData[this.ItemType.Value];
                ItemClass eItemClass = sid.ItemClass;
                sText = sid.SingularName;
                if (eItemClass == ItemClass.Coins)
                {
                    sText = this.Count + " " + sText;
                }
                if (!sid.IsCurrency())
                {
                    if (sid.Weight.HasValue)
                    {
                        sText = sText + " #" + sid.Weight.Value.ToString();
                    }
                    if (sid.ArmorClass > 0)
                    {
                        sText = sText + " " + sid.ArmorClass.ToString("N1") + "ac";
                    }
                    if (sid.Sellable == SellableEnum.Junk)
                    {
                        sText = sText + " junk";
                    }
                    else if (sid.SellGold != 0)
                    {
                        sText = sText + " $" + sid.SellGold;
                    }
                }
            }
            return sText;
        }

        public static IEnumerable<ItemEntity> SplitItemEntity(ItemEntity input, bool expectSingleItem, List<string> errorMessages)
        {
            int iEntityCount = input.Count;
            if (input.ItemType.HasValue)
            {
                ItemTypeEnum eItemType = input.ItemType.Value;
                StaticItemData sid = StaticItemData[eItemType];
                int iSplitCount;
                if (sid.ItemClass == ItemClass.Coins)
                {
                    iEntityCount = input.SetCount;
                    iSplitCount = input.Count;
                }
                else
                {
                    iSplitCount = 1;
                    if (iEntityCount != 1 && expectSingleItem)
                    {
                        errorMessages.Add("Unexpected item count for " + eItemType.ToString() + ": " + input.Count);
                    }
                }
                if (iEntityCount == 1)
                {
                    yield return input;
                }
                else
                {
                    for (int i = 0; i < iEntityCount; i++)
                    {
                        yield return new ItemEntity(eItemType, iSplitCount, 1);
                    }
                }
            }
            else
            {
                //this assumes the unknown item isn't coins, but there's no way to know at this point
                //so the best guess is treat is as not coins
                UnknownItemEntity uie = (UnknownItemEntity)input;
                for (int i = 0; i < iEntityCount; i++)
                {
                    yield return new UnknownItemEntity(uie.Name, 1, 1);
                }
            }
        }

        public static void ProcessAndSplitItemEntity(ItemEntity ie, ref List<ItemEntity> itemList, FeedLineParameters flp, bool expectSingleItem)
        {
            if (ie != null)
            {
                if (itemList == null) itemList = new List<ItemEntity>();
                if (ie is UnknownItemEntity)
                {
                    flp.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)ie).Name);
                    itemList.Add(ie);
                }
                else
                {
                    foreach (ItemEntity nextIE in ItemEntity.SplitItemEntity(ie, expectSingleItem, flp.ErrorMessages))
                    {
                        itemList.Add(nextIE);
                    }
                }
            }
        }

        /// <summary>
        /// retrieves an item entity from an object text. A single item is expected.
        /// </summary>
        /// <param name="ObjectText">object text</param>
        /// <param name="itemList">list of items, created if null</param>
        /// <param name="flp">feed line parameters</param>
        /// <param name="expectCapitalized">whether to expect the first word to be capitalized</param>
        public static void GetItemEntityFromObjectText(string ObjectText, ref List<ItemEntity> itemList, FeedLineParameters flp, bool expectCapitalized)
        {
            ItemEntity ie = Entity.GetEntity(ObjectText, EntityTypeFlags.Item, flp.ErrorMessages, null, expectCapitalized) as ItemEntity;
            ProcessAndSplitItemEntity(ie, ref itemList, flp, true);
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

    public class ItemInInventoryOrEquipmentList
    {
        public ItemEntity Item { get; set; }
        public bool IsInventory { get; set; }
        public ItemInInventoryOrEquipmentList(ItemEntity item, bool IsInventory)
        {
            this.Item = item;
            this.IsInventory = IsInventory;
        }
        public override string ToString()
        {
            string ret;
            if (IsInventory)
            {
                ret = this.Item.GetItemString();
            }
            else
            {
                if (this.Item is UnknownItemEntity)
                {
                    ret = ((UnknownItemEntity)this.Item).Name + "(Unknown)";
                }
                else
                {
                    StaticItemData sid = ItemEntity.StaticItemData[Item.ItemType.Value];
                    ret = sid.SingularName + "(" + sid.EquipmentType.ToString() + ")";
                    if (sid.ArmorClass > 0) ret += sid.ArmorClass.ToString("N1");
                }
            }
            return ret;
        }
    }


    public class SelectedInventoryOrEquipmentItem
    {
        public SelectedInventoryOrEquipmentItem(ItemTypeEnum ItemType, int Counter, bool IsInventory)
        {
            this.ItemType = ItemType;
            this.Counter = Counter;
            this.IsInventory = IsInventory;
        }
        public ItemTypeEnum ItemType;
        public int Counter;
        public bool IsInventory;
    }

    public class StaticItemData
    {
        public bool IsCurrency()
        {
            return ItemClass == ItemClass.Money || ItemClass == ItemClass.Coins;
        }
        public ItemClass ItemClass { get; set; }
        public ItemTypeEnum ItemType { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public WeaponType? WeaponType { get; set; }
        public SpellsEnum? Spell { get; set; }
        public string SingularName { get; set; }
        public string SingularSelection { get; set; }
        public string PluralName { get; set; }
        /// <summary>
        /// weight of the item. Null if unknown. It's nullable because a zero pound item has been observed (string of pearls)
        /// </summary>
        public int? Weight { get; set; }
        public decimal ArmorClass { get; set; }
        public SellableEnum Sellable { get; set; }
        public int SellGold { get; set; }
        public ClassTypeFlags DisallowedClasses { get; set; }
    }

    public class DynamicItemDataWithInheritance : DynamicItemData
    {
        public DynamicDataItemClass? KeepCountInheritance;
        public DynamicDataItemClass? SinkCountInheritance;
        public DynamicDataItemClass? OverflowActionInheritance;

        public DynamicItemDataWithInheritance(IsengardSettingData settings, ItemTypeEnum itemType)
        {
            DynamicItemData did;
            if (settings.DynamicItemData.TryGetValue(itemType, out did))
            {
                KeepCount = did.KeepCount;
                SinkCount = did.SinkCount;
                OverflowAction = did.OverflowAction;
            }
            ProcessInheritance(settings, GetInheritanceClasses(itemType));
        }

        public DynamicItemDataWithInheritance(IsengardSettingData settings, DynamicDataItemClass itemClass)
        {
            DynamicItemData did;
            if (settings.DynamicItemClassData.TryGetValue(itemClass, out did))
            {
                KeepCount = did.KeepCount;
                SinkCount = did.SinkCount;
                OverflowAction = did.OverflowAction;
            }
            ProcessInheritance(settings, GetInheritanceClasses(itemClass));
        }

        private void ProcessInheritance(IsengardSettingData settings, IEnumerable<DynamicDataItemClass> InheritanceClasses)
        {
            foreach (DynamicDataItemClass nextInheritanceClass in InheritanceClasses)
            {
                if (settings.DynamicItemClassData.TryGetValue(nextInheritanceClass, out DynamicItemData did))
                {
                    if (KeepCount < 0 && did.KeepCount >= 0)
                    {
                        KeepCount = did.KeepCount;
                        KeepCountInheritance = nextInheritanceClass;
                    }
                    if (SinkCount < 0 && did.SinkCount >= 0)
                    {
                        SinkCount = did.SinkCount;
                        SinkCountInheritance = nextInheritanceClass;
                    }
                    if (OverflowAction == ItemInventoryOverflowAction.None && did.OverflowAction != ItemInventoryOverflowAction.None)
                    {
                        OverflowAction = did.OverflowAction;
                        OverflowActionInheritance = nextInheritanceClass;
                    }
                }
            }
        }

        public static IEnumerable<DynamicDataItemClass> GetInheritanceClasses(DynamicDataItemClass ItemClass)
        {
            yield return DynamicDataItemClass.Item;
        }

        public static IEnumerable<DynamicDataItemClass> GetInheritanceClasses(ItemTypeEnum ItemType)
        {
            StaticItemData sid = ItemEntity.StaticItemData[ItemType];
            ItemClass ic = sid.ItemClass;
            if (ic == ItemClass.Equipment)
            {
                switch (sid.EquipmentType)
                {
                    case EquipmentType.Arms:
                        yield return DynamicDataItemClass.EquipmentArms;
                        break;
                    case EquipmentType.Ears:
                        yield return DynamicDataItemClass.EquipmentEars;
                        break;
                    case EquipmentType.Face:
                        yield return DynamicDataItemClass.EquipmentFace;
                        break;
                    case EquipmentType.Feet:
                        yield return DynamicDataItemClass.EquipmentFeet;
                        break;
                    case EquipmentType.Finger:
                        yield return DynamicDataItemClass.EquipmentFinger;
                        break;
                    case EquipmentType.Hands:
                        yield return DynamicDataItemClass.EquipmentHands;
                        break;
                    case EquipmentType.Head:
                        yield return DynamicDataItemClass.EquipmentHead;
                        break;
                    case EquipmentType.Legs:
                        yield return DynamicDataItemClass.EquipmentLegs;
                        break;
                    case EquipmentType.Neck:
                        yield return DynamicDataItemClass.EquipmentNeck;
                        break;
                    case EquipmentType.Shield:
                        yield return DynamicDataItemClass.EquipmentShield;
                        break;
                    case EquipmentType.Torso:
                        yield return DynamicDataItemClass.EquipmentTorso;
                        break;
                    case EquipmentType.Waist:
                        yield return DynamicDataItemClass.EquipmentWaist;
                        break;
                    case EquipmentType.Unknown:
                        yield return DynamicDataItemClass.EquipmentUnknown;
                        break;
                }
                yield return DynamicDataItemClass.Equipment;
            }
            else if (ic == ItemClass.Weapon)
            {
                switch (sid.WeaponType)
                {
                    case WeaponType.Slash:
                        yield return DynamicDataItemClass.WeaponSlash;
                        break;
                    case WeaponType.Stab:
                        yield return DynamicDataItemClass.WeaponStab;
                        break;
                    case WeaponType.Blunt:
                        yield return DynamicDataItemClass.WeaponBlunt;
                        break;
                    case WeaponType.Polearm:
                        yield return DynamicDataItemClass.WeaponPolearm;
                        break;
                    case WeaponType.Missile:
                        yield return DynamicDataItemClass.WeaponMissile;
                        break;
                    case WeaponType.Unknown:
                        yield return DynamicDataItemClass.WeaponUnknown;
                        break;
                }
                yield return DynamicDataItemClass.Weapon;
            }
            else if (ic == ItemClass.Coins)
            {
                yield return DynamicDataItemClass.Coins;
            }
            else if (ic == ItemClass.Money)
            {
                yield return DynamicDataItemClass.Money;
            }
            else if (ic == ItemClass.Bag)
            {
                yield return DynamicDataItemClass.BagClass;
            }
            else if (ic == ItemClass.Key)
            {
                yield return DynamicDataItemClass.Key;
            }
            else if (ic == ItemClass.Potion)
            {
                yield return DynamicDataItemClass.Potion;
            }
            else if (ic == ItemClass.Scroll)
            {
                yield return DynamicDataItemClass.Scroll;
            }
            else if (ic == ItemClass.Wand)
            {
                yield return DynamicDataItemClass.Wand;
            }
            else if (ic == ItemClass.Usable)
            {
                yield return DynamicDataItemClass.Usable;
            }
            else if (ic == ItemClass.Fixed)
            {
                yield return DynamicDataItemClass.Fixed;
            }
            else if (ic == ItemClass.Chest)
            {
                yield return DynamicDataItemClass.Chest;
            }
            else if (ic == ItemClass.Gem)
            {
                yield return DynamicDataItemClass.Gem;
            }
            yield return DynamicDataItemClass.Item;
        }
    }

    public class DynamicItemData
    {
        /// <summary>
        /// number of items to keep in inventory
        /// </summary>
        public int KeepCount { get; set; }
        /// <summary>
        /// number of items to keep at the inventory sink
        /// </summary>
        public int SinkCount { get; set; }
        public ItemInventoryOverflowAction OverflowAction { get; set; }
        public DynamicItemData()
        {
            KeepCount = -1;
            SinkCount = -1;
            OverflowAction = ItemInventoryOverflowAction.None;
        }
        public DynamicItemData(DynamicItemData copied)
        {
            this.KeepCount = copied.KeepCount;
            this.SinkCount = copied.SinkCount;
            this.OverflowAction = copied.OverflowAction;
        }
        public bool HasData()
        {
            return KeepCount >=0 || SinkCount >= 0 || OverflowAction != ItemInventoryOverflowAction.None;
        }
    }

    public enum ItemClass
    {
        Equipment,
        Weapon,
        Potion,
        Scroll,
        Wand,
        Usable,
        Bag,
        Key,
        Gem,
        Coins,
        Money,
        Fixed,
        Chest,
        Other,
    }

    public enum DynamicDataItemClass
    {
        /// <summary>
        /// catchall default for any item
        /// </summary>
        Item,

        Equipment,
        EquipmentTorso,
        EquipmentArms,
        EquipmentLegs,
        EquipmentFeet,
        EquipmentNeck,
        EquipmentWaist,
        EquipmentHead,
        EquipmentHands,
        EquipmentFace,
        EquipmentFinger,
        EquipmentEars,
        EquipmentShield,
        EquipmentUnknown,

        Weapon,
        WeaponSlash,
        WeaponStab,
        WeaponBlunt,
        WeaponPolearm,
        WeaponMissile,
        WeaponUnknown,

        Potion,
        Scroll,
        Wand,
        Usable,
        BagClass,
        Key,
        Fixed,
        Chest,
        Gem,
        Coins,
        Money,

        /// <summary>
        /// item class not covered by other item classes
        /// </summary>
        Other,
    }

    [Flags]
    public enum SupportedKeysFlags
    {
        None = 0,
        GateKey = 1,
        BridgeKey = 2,
        BoilerKey = 4,
        SilverKey = 8,
        KasnarsRedKey = 16,
        TombKey = 32,
        All = 63,
    }

    /// <summary>
    /// item enums. There are three cases:
    /// 1. ordinary items have singular and plural names. These have both singular and plural attributes.
    /// 2. coin items use "X gold coins" and "sets of X gold coins" formats. These currently have both singular and plural attributes.
    /// 3. collective items only have a singular name, and use "sets of X" for the plural case. These currently only have a singular attribute.
    /// </summary>
    public enum ItemTypeEnum
    {
        [SingularName("adamantine dart")]
        [PluralName("adamantine darts")]
        [WeaponType(WeaponType.Unknown)]
        [Weight(3)]
        [Sellable(81)]
        AdamantineDart,

        [SingularName("adamantine scale mail armor")] //verified collective 7/8/23
        [EquipmentType(EquipmentType.Torso)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(10)]
        [Sellable(247)]
        AdamantineScaleMailArmor,

        [SingularName("adamantine scale mail gloves")]
        [EquipmentType(EquipmentType.Hands)]
        [Weight(5)]
        [ArmorClass(0.4)]
        [Sellable(86)]
        AdamantineScaleMailGloves,

        [SingularName("adamantine scale mail leggings")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(7)]
        [ArmorClass(0.4)]
        AdamantineScaleMailLeggings,

        [SingularName("adamantine scale mail sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(8)]
        [ArmorClass(0.4)]
        AdamantineScaleMailSleeves,

        [SingularName("Ahrot's magic string")]
        [PluralName("Ahrot's magic strings")]
        AhrotsMagicString,

        [SingularName("amber scroll")]
        [PluralName("amber scrolls")]
        [Scroll(SpellsEnum.burn)]
        AmberScroll,

        //destroys whatever is put in it (e.g. "some toilet paper is devoured by an ancient bag!")
        [SingularName("ancient bag")]
        [PluralName("ancient bags")]
        [ItemClass(ItemClass.Bag)]
        [Weight(1)]
        [Sellable(123)]
        AncientBag,

        [SingularName("ancient lyre")]
        [PluralName("ancient lyres")]
        [Weight(4)]
        AncientLyre,

        [SingularName("Ancient Shield of Adalphi")]
        //CSRTODO: plural?
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(61)]
        AncientShieldOfAdalphi,

        [SingularName("animal hides")]
        //CSRTODO: plural
        [EquipmentType(EquipmentType.Torso)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        AnimalHides,

        [SingularName("anvil")]
        [PluralName("anvils")]
        Anvil,

        [SingularName("aquamarine potion")]
        [PluralName("aquamarine potions")]
        [Potion(SpellsEnum.levitate)]
        AquamarinePotion,

        [SingularName("assassin's dagger")]
        [PluralName("assassin's daggers")]
        [WeaponType(WeaponType.Stab)]
        [Weight(3)]
        [Sellable(3168)]
        AssassinsDagger,

        [SingularName("assassin's mask")]
        [PluralName("assassin's masks")]
        [EquipmentType(EquipmentType.Face)]
        AssassinsMask,

        [SingularName("A statuette of Balthazar")]
        [SingularSelection("A")]
        //CSRTODO: no plural?
        [ItemClass(ItemClass.Fixed)]
        AStatuetteOfBalthazar,

        [SingularName("bag")]
        [PluralName("bags")]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        Bag,

        [SingularName("banded mail armor")]
        [EquipmentType(EquipmentType.Torso)]
        BandedMailArmor,

        [SingularName("beastmaster's whip")]
        [PluralName("beastmaster's whips")]
        [WeaponType(WeaponType.Missile)]
        [Weight(2)]
        [Sellable(3093)]
        BeastmastersWhip,

        [SingularName("bec de corbin")]
        [PluralName("bec de corbins")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(5)]
        [Sellable(99)]
        BecDeCorbin,

        [SingularName("black bag")]
        [PluralName("black bags")]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        BlackBag,

        [SingularName("black cape")]
        [PluralName("black capes")]
        [EquipmentType(EquipmentType.Unknown)]
        BlackCape,

        [SingularName("blackened scroll")]
        [PluralName("blackened scrolls")]
        [Scroll(SpellsEnum.light)]
        BlackenedScroll,

        [SingularName("black rune")]
        [PluralName("black runes")]
        [ItemClass(ItemClass.Fixed)]
        BlackRune,

        [SingularName("black scroll")]
        [PluralName("black scrolls")]
        [Scroll(SpellsEnum.hurt)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        BlackScroll,

        [SingularName("black iron key")]
        [PluralName("black iron keys")]
        [ItemClass(ItemClass.Key)]
        BlackIronKey,

        [SingularName("black vestments")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Torso)]
        BlackVestments,

        [SingularName("blue bubbly potion")]
        [PluralName("blue bubbly potions")]
        [Potion(SpellsEnum.detectinvis)]
        [Weight(2)]
        BlueBubblyPotion,

        [SingularName("blue rune")]
        [PluralName("blue runes")]
        [ItemClass(ItemClass.Fixed)]
        BlueRune,

        [SingularName("boiler key")]
        [PluralName("boiler keys")]
        [ItemClass(ItemClass.Key)]
        [Weight(10)]
        [Sellable(SellableEnum.Junk)]
        BoilerKey,

        [SingularName("bone armor")]
        [PluralName("bone armors")] //verified 6/21/23
        [EquipmentType(EquipmentType.Torso)]
        [Weight(10)]
        [ArmorClass(0.7)]
        [Sellable(247)]
        BoneArmor,

        [SingularName("bone shield")]
        [PluralName("bone shields")]
        [EquipmentType(EquipmentType.Shield)]
        [Weight(5)]
        [Sellable(24)]
        BoneShield,

        [SingularName("book of knowledge")]
        [PluralName("books of knowledge")]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        BookOfKnowledge,

        [SingularName("boomerang")]
        [PluralName("boomerangs")]
        [WeaponType(WeaponType.Missile)]
        [Weight(5)]
        [Sellable(54)]
        Boomerang,

        //first use casts levitation
        [SingularName("boots of levitation")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(2)]
        BootsOfLevitation,

        [SingularName("bo stick")]
        [PluralName("bo sticks")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(4)]
        [Sellable(SellableEnum.Junk)]
        BoStick,

        [SingularName("box of strawberries")]
        //CSRTODO: plural?
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        BoxOfStrawberries,

        [SingularName("bracers of ogre-strength")]
        [EquipmentType(EquipmentType.Unknown)]
        [Weight(8)]
        [Sellable(618)]
        BracersOfOgreStrength,

        [SingularName("brass knuckles")]
        //CSRTODO: plural?
        [WeaponType(WeaponType.Blunt)]
        BrassKnuckles,

        [SingularName("bridge key")]
        [PluralName("bridge keys")]
        [ItemClass(ItemClass.Key)]
        [Weight(5)]
        [Sellable(SellableEnum.Junk)]
        BridgeKey,

        [SingularName("broad sword")]
        [PluralName("broad swords")]
        [WeaponType(WeaponType.Slash)]
        BroadSword,

        [SingularName("bronze gauntlets")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Hands)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(198)]
        BronzeGauntlets,

        [SingularName("brown bag")]
        [PluralName("brown bags")]
        BrownBag,

        [SingularName("bucket")]
        [PluralName("buckets")]
        Bucket,

        //mage training level 14
        [SingularName("bugbear key")]
        [PluralName("bugbear keys")]
        [ItemClass(ItemClass.Key)]
        BugbearKey,

        [SingularName("bundle of wheat")]
        [PluralName("bundle of wheats")] //verified 6/14/2023
        BundleOfWheat,

        [SingularName("carbon scroll")]
        [PluralName("carbon scrolls")]
        [Scroll(SpellsEnum.rumble)]
        CarbonScroll,

        [SingularName("carved ivory key")]
        [PluralName("carved ivory keys")]
        [ItemClass(ItemClass.Key)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        CarvedIvoryKey,

        [SingularName("cat o' nine tails")]
        [PluralName("cat o' nine tailses")] //verified 7/3/23
        [WeaponType(WeaponType.Slash)]
        [Weight(12)]
        [Sellable(396)]
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

        [SingularName("chest")]
        [PluralName("chests")]
        [ItemClass(ItemClass.Chest)]
        Chest,

        [SingularName("claw gauntlet")]
        [PluralName("claw gauntlets")]
        [WeaponType(WeaponType.Slash)]
        [Weight(3)]
        ClawGauntlet,

        [SingularName("cloth armor")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(5)]
        [ArmorClass(0.3)]
        [Sellable(SellableEnum.Junk)]
        ClothArmor,

        [SingularName("cloth boots")]
        [EquipmentType(EquipmentType.Feet)]
        ClothBoots,

        [SingularName("cloth hat")]
        [PluralName("cloth hats")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        ClothHat,

        [SingularName("cloth pants")]
        [EquipmentType(EquipmentType.Legs)]
        ClothPants,

        [SingularName("club")]
        [PluralName("clubs")]
        [WeaponType(WeaponType.Blunt)]
        [Weight(2)]
        [Sellable(24)]
        Club,

        [SingularName("copper pieces")]
        [PluralName("copper pieces")]
        [ItemClass(ItemClass.Coins)]
        CopperPieces,

        [SingularName("copper ring")]
        [PluralName("copper rings")]
        [EquipmentType(EquipmentType.Finger)]
        CopperRing,

        [SingularName("criminal records book")]
        [PluralName("criminal records books")]
        [Weight(20)]
        [Sellable(1237)]
        CriminalRecordsBook,

        [SingularName("crossbow")]
        [PluralName("crossbows")]
        [WeaponType(WeaponType.Missile)]
        [Weight(5)]
        [Sellable(207)]
        Crossbow,

        [SingularName("crystal amulet")]
        [PluralName("crystal amulets")]
        [EquipmentType(EquipmentType.Unknown)]
        CrystalAmulet,

        [SingularName("cutlass")]
        [PluralName("cutlasses")]
        [WeaponType(WeaponType.Stab)]
        [Weight(4)]
        [Sellable(91)]
        Cutlass,

        [SingularName("dagger")]
        [PluralName("daggers")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        Dagger,

        [SingularName("dark blade")]
        [PluralName("dark blades")]
        [WeaponType(WeaponType.Slash)]
        DarkBlade,

        [SingularName("dark green potion")]
        [PluralName("dark green potions")]
        [Potion(SpellsEnum.detectmagic)]
        [Weight(2)] //CSRTODO: wiki says this could also be 4
        DarkGreenPotion,

        [SingularName("dark flask")]
        [PluralName("dark flasks")]
        [Potion(SpellsEnum.mend)]
        [Weight(3)]
        [Sellable(SellableEnum.Junk)]
        DarkFlask,

        [SingularName("dead rat carcass")]
        //CSRTODO: plural?
        [Weight(1)]
        DeadRatCarcass,

        [SingularName("Death's galvorn sickle")]
        [PluralName("Death's galvorn sickles")]
        [WeaponType(WeaponType.Unknown)]
        DeathsGalvornSickle,

        [SingularName("diamond")]
        [PluralName("diamonds")]
        [ItemClass(ItemClass.Gem)]
        [Weight(1)]
        [Sellable(371)]
        Diamond,

        [SingularName("diamond laurel ring")]
        [PluralName("diamond laurel rings")]
        [EquipmentType(EquipmentType.Finger)]
        [Weight(2)]
        [Sellable(148)]
        DiamondLaurelRing,

        [SingularName("dildo")]
        [PluralName("dildos")]
        Dildo,

        [SingularName("dirk")]
        [PluralName("dirks")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(163)]
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
        [ItemClass(ItemClass.Key)]
        DungeonKey,

        [SingularName("dwarven mithril gaiters")] //CSRTODO: plural
        [SingularSelection("dwarven mithril gaiter")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(4)]
        [Sellable(742)]
        DwarvenMithrilGaiters,

        [SingularName("ear lobe plug")]
        [PluralName("ear lobe plugs")]
        [EquipmentType(EquipmentType.Unknown)]
        EarLobePlug,

        [SingularName("earth pipe")]
        [PluralName("earth pipes")]
        EarthPipe,

        [SingularName("Eat At Denethore's decorative mug")]
        [PluralName("Eat At Denethore's decorative mugs")]
        EatAtDenethoresDecorativeMug,

        [SingularName("elven bow")]
        [PluralName("elven bows")]
        [WeaponType(WeaponType.Missile)]
        ElvenBow,

        [SingularName("elven chain mail")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Unknown)]
        [Sellable(185)]
        ElvenChainMail,

        [SingularName("elven chain mail gloves")]
        [EquipmentType(EquipmentType.Hands)]
        [Sellable(86)]
        ElvenChainMailGloves,

        [SingularName("elven cured leather armor")] //collective plural verified 7/6/23
        [EquipmentType(EquipmentType.Torso)]
        [Weight(1)]
        [ArmorClass(1.2)]
        [Sellable(247)]
        ElvenCuredLeatherArmor,

        [SingularName("elven cured leather boots")] //collective plural verified 7/6/23
        [EquipmentType(EquipmentType.Feet)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(61)]
        ElvenCuredLeatherBoots,

        [SingularName("elven cured leather gloves")] //collective plural verified 7/6/23
        [EquipmentType(EquipmentType.Hands)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(61)]
        ElvenCuredLeatherGloves,

        [SingularName("elven cured leather hood")]
        [PluralName("elven cured leather hoods")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(1)]
        [ArmorClass(0.8)]
        [Sellable(185)]
        ElvenCuredLeatherHood,

        [SingularName("elven cured leather leggings")] //collective plural verified 7/6/23
        [EquipmentType(EquipmentType.Legs)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(123)]
        ElvenCuredLeatherLeggings,

        [SingularName("elven cured leather sleeves")] //collective plural verified 7/6/23
        [EquipmentType(EquipmentType.Arms)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(123)]
        ElvenCuredLeatherSleeves,

        [SingularName("elven leather whip")]
        [PluralName("elven leather whips")]
        [WeaponType(WeaponType.Missile)]
        [Weight(5)]
        [Sellable(148)]
        ElvenLeatherWhip,

        [SingularName("emerald")]
        [PluralName("emeralds")]
        [ItemClass(ItemClass.Gem)]
        [Weight(1)]
        [Sellable(432)]
        Emerald,

        [SingularName("emerald collar")]
        [PluralName("emerald collars")]
        [EquipmentType(EquipmentType.Neck)]
        [Weight(4)]
        [Sellable(495)]
        EmeraldCollar,

        [SingularName("engagement ring")]
        [PluralName("engagement rings")]
        [EquipmentType(EquipmentType.Finger)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(198)]
        EngagementRing,

        [SingularName("epee sword")]
        [PluralName("epee swords")]
        [WeaponType(WeaponType.Unknown)]
        EpeeSword,

        [SingularName("eye of newt")]
        //CSRTODO: plural?
        EyeOfNewt,

        [SingularName("Faded map")]
        [PluralName("Faded maps")]
        [Weight(1)]
        FadedMap,

        [SingularName("flint blade")]
        [PluralName("flint blades")]
        [WeaponType(WeaponType.Slash)]
        [Weight(6)]
        [Sellable(224)]
        FlintBlade,

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

        [SingularName("gate key")]
        [PluralName("gate keys")]
        [ItemClass(ItemClass.Key)]
        GateKey,

        [SingularName("gate warning")]
        [PluralName("gate warnings")]
        [ItemClass(ItemClass.Fixed)]
        GateWarning,

        [SingularName("gaudy scepter")]
        [PluralName("gaudy scepters")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(3)]
        [Sellable(1212)]
        GaudyScepter,

        [SingularName("gawdy ear hoop")]
        [PluralName("gawdy ear hoops")]
        [EquipmentType(EquipmentType.Ears)]
        [Weight(2)]
        [ArmorClass(0.2)]
        [Sellable(99)]
        GawdyEarHoop,

        [SingularName("giant stylus")]
        [PluralName("giant styluses")] //CSRTODO: correct plural
        [WeaponType(WeaponType.Polearm)]
        [Weight(3)]
        [Sellable(2783)]
        GiantStylus,

        [SingularName("Girion's key")]
        [PluralName("Girion's keys")]
        [ItemClass(ItemClass.Key)]
        GirionsKey,

        [SingularName("glimmering blade")]
        [PluralName("glimmering blades")]
        [WeaponType(WeaponType.Slash)]
        [Weight(15)]
        GlimmeringBlade,

        [SingularName("glitter")]
        //CSRTODO: glitter plural? (does not use some)
        [Wand(SpellsEnum.stun)]
        [Weight(1)]
        Glitter,

        [SingularName("glowing pendant")]
        [PluralName("glowing pendants")]
        [EquipmentType(EquipmentType.Unknown)]
        GlowingPendant,

        [SingularName("glowing potion")]
        [PluralName("glowing potions")]
        [Potion(SpellsEnum.bless)]
        GlowingPotion,

        [SingularName("glowing talisman")]
        //CSRTODO: plural
        GlowingTalisman,

        [SingularName("goblin blade")]
        [PluralName("goblin blades")]
        [WeaponType(WeaponType.Slash)]
        [Weight(5)]
        [Sellable(116)]
        GoblinBlade,

        [SingularName("godentag")]
        [PluralName("godentags")]
        [WeaponType(WeaponType.Blunt)]
        Godentag,

        [SingularName("gold coins")]
        [PluralName("gold coins")]
        [ItemClass(ItemClass.Coins)]
        GoldCoins,

        [SingularName("golden dagger")]
        [PluralName("golden daggers")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(278)]
        GoldenDagger,

        [SingularName("golden mask of the gods")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Face)]
        GoldenMaskOfTheGods,

        [SingularName("gold ring")]
        [PluralName("gold rings")]
        [EquipmentType(EquipmentType.Finger)] //when worn, shocks and drops if cursed
        [Weight(2)]
        [ArmorClass(0.1)]
        [Sellable(123)]
        GoldRing,

        [SingularName("gold sword")]
        [PluralName("gold swords")]
        [WeaponType(WeaponType.Stab)]
        [Weight(5)]
        [Sellable(1267)]
        GoldSword,

        [SingularName("grate key")]
        [PluralName("grate keys")]
        [ItemClass(ItemClass.Key)]
        GrateKey,

        [SingularName("green potion")]
        [PluralName("green potions")]
        [Potion(SpellsEnum.curepoison)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        GreenPotion,

        [SingularName("green rune")]
        [PluralName("green runes")]
        [ItemClass(ItemClass.Fixed)]
        GreenRune,

        [SingularName("grey cloak")]
        [PluralName("grey cloaks")]
        [EquipmentType(EquipmentType.Neck)]
        [Weight(2)]
        [ArmorClass(0.1)]
        [Sellable(50)]
        GreyCloak,

        [SingularName("grey rune")]
        [PluralName("grey runes")]
        [ItemClass(ItemClass.Fixed)]
        GreyRune,

        [SingularName("grey scroll")]
        [PluralName("grey scrolls")]
        [Scroll(SpellsEnum.vigor)]
        [Weight(1)]
        GreyScroll,

        [SingularName("gypsy battle crescent")]
        [PluralName("gypsy battle crescents")]
        [WeaponType(WeaponType.Slash)]
        [Weight(3)]
        [Sellable(965)]
        GypsyBattleCrescent,

        [SingularName("gypsy cape")]
        [PluralName("gypsy capes")]
        [EquipmentType(EquipmentType.Unknown)]
        [Weight(2)]
        [Sellable(148)]
        GypsyCape,

        [SingularName("gypsy crown")]
        [PluralName("gypsy crowns")]
        [EquipmentType(EquipmentType.Unknown)]
        [Sellable(247)]
        [Weight(7)]
        GypsyCrown,

        [SingularName("halberd")]
        [PluralName("halberds")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(12)]
        [Sellable(176)]
        Halberd,

        [SingularName("half giant chain mail armor")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(24)]
        [Sellable(371)]
        HalfGiantChainMailArmor,

        [SingularName("half-giant chain mail gloves")]
        [EquipmentType(EquipmentType.Hands)]
        [Weight(10)]
        [Sellable(123)]
        HalfGiantChainMailGloves,

        [SingularName("half-giant chain mail hood")]
        [PluralName("half-giant chain mail hoods")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(13)]
        [Sellable(185)]
        HalfGiantChainMailHood,

        [SingularName("half-giant chain mail leggings")]
        [EquipmentType(EquipmentType.Legs)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(15)]
        [Sellable(185)]
        HalfGiantChainMailLeggings,

        [SingularName("half-giant chain mail sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(15)]
        [Sellable(185)]
        HalfGiantChainMailSleeves,

        [SingularName("hand axe")]
        [PluralName("hand axes")]
        [WeaponType(WeaponType.Slash)]
        [Weight(2)]
        [Sellable(39)]
        HandAxe,

        [SingularName("hardwood shield")]
        [PluralName("hardwood shields")]
        [EquipmentType(EquipmentType.Shield)]
        HardwoodShield,

        [SingularName("hazy potion")]
        [PluralName("hazy potions")]
        [Potion(SpellsEnum.wordofrecall)]
        [Weight(5)]
        [Sellable(SellableEnum.NotSellable)]
        HazyPotion,

        [SingularName("head of lettuce")]
        //CSRTODO: plural?
        HeadOfLettuce,

        [SingularName("herbal stimulant")]
        [PluralName("herbal stimulants")]
        HerbalStimulant,

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
        [Weight(2)]
        [Sellable(SellableEnum.NotSellable)]
        IceBluePotion,

        [SingularName("information kiosk")]
        [PluralName("information kiosks")]
        [ItemClass(ItemClass.Fixed)]
        InformationKiosk,

        //mage training level 13
        [SingularName("invisible key")]
        [PluralName("invisible keys")]
        [ItemClass(ItemClass.Key)]
        InvisibleKey,

        [SingularName("iron ring")]
        [PluralName("iron rings")]
        [EquipmentType(EquipmentType.Finger)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        IronRing,

        [SingularName("iron spear")]
        [PluralName("iron spears")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(3)]
        [Sellable(203)]
        IronSpear,

        [SingularName("jewelled armlets")]
        //CSRTODO: plural
        [EquipmentType(EquipmentType.Arms)]
        [Weight(2)]
        [Sellable(122)]
        JewelledArmlets,

        [SingularName("juggling pin")]
        [PluralName("juggling pins")]
        [WeaponType(WeaponType.Blunt)]
        JugglingPin,

        [SingularName("Kasnar's red key")]
        [PluralName("Kasnar's red keys")]
        [ItemClass(ItemClass.Key)]
        [Weight(3)]
        KasnarsRedKey,

        [SingularName("kelp necklace")]
        [PluralName("kelp necklaces")]
        [EquipmentType(EquipmentType.Neck)]
        [Weight(2)]
        [ArmorClass(0.3)]
        [Sellable(SellableEnum.Junk)]
        KelpNecklace,

        [SingularName("key of the elements")]
        //CSRTODO: plural?
        [ItemClass(ItemClass.Key)]
        KeyOfTheElements,

        [SingularName("khopesh sword")]
        [PluralName("khopesh swords")]
        [WeaponType(WeaponType.Unknown)]
        KhopeshSword,

        [SingularName("knapsack")]
        [PluralName("knapsacks")]
        [ItemClass(ItemClass.Bag)]
        Knapsack,

        [SingularName("lance")]
        [PluralName("lances")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(1)]
        [Sellable(188)]
        Lance,

        [SingularName("lancette")]
        [PluralName("lancettes")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(59)]
        Lancette,

        [SingularName("lantern")]
        [PluralName("lanterns")]
        [Weight(4)]
        [Sellable(SellableEnum.Junk)]
        Lantern,

        [SingularName("large egg")]
        [PluralName("large eggs")]
        [Use(SpellsEnum.curemalady)]
        [Weight(3)]
        [Sellable(49)]
        LargeEgg,

        [SingularName("large metal shield")]
        [PluralName("large metal shields")]
        [EquipmentType(EquipmentType.Shield)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(12)]
        [Sellable(49)]
        LargeMetalShield,

        [SingularName("large wooden shield")]
        [PluralName("large wooden shields")]
        [EquipmentType(EquipmentType.Shield)]
        [Weight(7)]
        [ArmorClass(0.3)]
        [Sellable(SellableEnum.Junk)]
        LargeWoodenShield,

        [SingularName("lead hammer")]
        [PluralName("lead hammers")]
        [WeaponType(WeaponType.Blunt)]
        [Weight(5)]
        [Sellable(643)]
        LeadHammer,

        [SingularName("leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        LeatherArmor,

        [SingularName("leather boots")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(4)]
        [Sellable(SellableEnum.Junk)]
        LeatherBoots,

        [SingularName("leather cap")]
        [PluralName("leather caps")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        LeatherCap,

        [SingularName("leather gloves")]
        [EquipmentType(EquipmentType.Hands)]
        [Weight(2)]
        LeatherGloves,

        [SingularName("leather pouch")]
        [PluralName("leather pouches")]
        LeatherPouch,

        [SingularName("leather sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(3)]
        LeatherSleeves,

        [SingularName("light leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        LightLeatherArmor,

        [SingularName("little brown jug")]
        [PluralName("little brown jugs")]
        [Potion(SpellsEnum.endurecold)]
        [Weight(3)]
        LittleBrownJug,

        [SingularName("lollipop")]
        [PluralName("lollipops")]
        [Potion(SpellsEnum.curedisease)] //per wiki
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
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
        [ItemClass(ItemClass.Money)]
        //CSRTODO: plural?
        LunchMoney,

        [SingularName("magical tabulator")]
        [PluralName("magical tabulators")]
        [Weight(5)]
        [Wand(SpellsEnum.detectmagic)]
        MagicalTabulator,

        [SingularName("magical temper")]
        //CSRTODO: plural?
        MagicalTemper,

        [SingularName("marble chess set")]
        [PluralName("marble chess sets")]
        [Weight(12)]
        [Sellable(49)]
        MarbleChessSet,

        [SingularName("mask of darkness")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Face)]
        [Weight(3)]
        [ArmorClass(0.6)]
        [Sellable(742)]
        MaskOfDarkness,

        [SingularName("mask of distortion")]
        [PluralName("mask of distortions")] //verified 7/9/23
        [EquipmentType(EquipmentType.Face)]
        [Weight(3)]
        [Sellable(308)]
        MaskOfDistortion,

        [SingularName("metal helmet")]
        [PluralName("metal helmets")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(5)]
        [Sellable(49)]
        MetalHelmet,

        [SingularName("metal mask")]
        [PluralName("metal masks")]
        [EquipmentType(EquipmentType.Unknown)]
        MetalMask,

        [SingularName("mithril breastplate")]
        [PluralName("mithril breastplates")]
        [EquipmentType(EquipmentType.Unknown)]
        MithrilBreastplate,

        [SingularName("mithril chain armor")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(18)]
        [Sellable(6187)]
        MithrilChainArmor,

        [SingularName("mithril jo stick")]
        [PluralName("mithril jo sticks")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(5)]
        [Sellable(2376)]
        MithrilJoStick,

        [SingularName("mithril lamella leggings")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(5)]
        [Sellable(3217)]
        MithrilLamellaLeggings,

        [SingularName("mithron blade")]
        [PluralName("mithron blades")]
        [WeaponType(WeaponType.Unknown)]
        MithronBlade,

        [SingularName("mithron helmet")]
        [PluralName("mithron helmets")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(4)]
        MithronHelmet,

        [SingularName("mithron hood")]
        [PluralName("mithron hoods")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(5)]
        [Sellable(247)]
        [DisallowedClasses(ClassTypeFlags.Mage)] //verified 7/3/23
        MithronHood,

        [SingularName("mithron shield")]
        [PluralName("mithron shields")]
        [EquipmentType(EquipmentType.Shield)]
        [Weight(8)]
        [Sellable(185)]
        MithronShield,

        [SingularName("molten iron key")]
        [PluralName("molten iron keys")]
        [ItemClass(ItemClass.Key)]
        MoltenIronKey,

        [SingularName("MOM tattoo")]
        [PluralName("MOM tattoo")] //verified 7/5/23
        [EquipmentType(EquipmentType.Arms)]
        [Weight(1)]
        [Sellable(1237)]
        [DisallowedClasses(ClassTypeFlags.Mage)] //verified 7/3/23
        MOMTattoo,

        [SingularName("morning star")]
        [PluralName("morning stars")]
        [WeaponType(WeaponType.Unknown)]
        MorningStar,

        [SingularName("mountain boots")]
        //CSRTODO: plural
        [EquipmentType(EquipmentType.Feet)]
        [Weight(2)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        MountainBoots,

        [SingularName("nunchukus")]
        [WeaponType(WeaponType.Polearm)]
        //CSRTODO: plural
        Nunchukus,

        //CSRTODO: where does this live?
        [SingularName("old wooden sign")]
        [PluralName("old wooden signs")]
        [ItemClass(ItemClass.Fixed)]
        OldWoodenSign,

        [SingularName("onyx amulet")]
        [PluralName("onyx amulets")]
        [EquipmentType(EquipmentType.Neck)]
        [Weight(4)]
        OnyxAmulet,

        [SingularName("opal staff")]
        [PluralName("opal staffs")]
        [Wand(SpellsEnum.mend)]
        OpalStaff,

        [SingularName("orange potion")]
        [PluralName("orange potions")]
        [Potion(SpellsEnum.knowaura)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        OrangePotion,

        [SingularName("orc's sword")]
        [PluralName("orc's swords")]
        [WeaponType(WeaponType.Unknown)]
        OrcsSword,

        [SingularName("ork blade")]
        [PluralName("ork blades")]
        [WeaponType(WeaponType.Slash)]
        [Weight(5)]
        [Sellable(841)]
        OrkBlade,

        [SingularName("out of order sign")]
        [PluralName("out of order signs")]
        [ItemClass(ItemClass.Fixed)]
        OutOfOrderSign,

        [SingularName("parched scroll")]
        [PluralName("parched scrolls")]
        [Scroll(SpellsEnum.curepoison)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
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

        [SingularName("piece of coal")]
        //CSRTODO: plural?
        [Weight(10)]
        PieceOfCoal,

        [SingularName("pipe weed")]
        //CSRTODO: plural?
        [Wand(SpellsEnum.stun)]
        [Weight(1)]
        PipeWeed,

        [SingularName("platinum pieces")]
        [PluralName("platinum pieces")]
        [ItemClass(ItemClass.Coins)]
        PlatinumPieces,

        [SingularName("platinum ring")]
        [PluralName("platinum rings")]
        [EquipmentType(EquipmentType.Finger)]
        [Weight(1)]
        [Sellable(247)]
        PlatinumRing,

        [SingularName("port manifest")]
        [PluralName("port manifests")]
        [ItemClass(ItemClass.Fixed)]
        PortManifest,

        [SingularName("pot helm")]
        [PluralName("pot helms")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(7)]
        [ArmorClass(0.3)]
        [Sellable(23)]
        PotHelm,

        [SingularName("pot of gold")]
        [PluralName("pots of gold")]
        [ItemClass(ItemClass.Money)]
        PotOfGold,

        [SingularName("pure white cape")]
        [PluralName("pure white capes")]
        [EquipmentType(EquipmentType.Unknown)]
        PureWhiteCape,

        [SingularName("purple wand")]
        [PluralName("purple wands")]
        [Wand(SpellsEnum.stun)]
        [Weight(5)]
        //CSRTODO: wand
        PurpleWand,

        [SingularName("quarterstaff")]
        [PluralName("quarterstaffs")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        Quarterstaff,

        [SingularName("quartz stone")]
        [PluralName("quartz stones")]
        [Weight(5)]
        [Wand(SpellsEnum.restore)] //CSRTODO: is this correct?
        QuartzStone,

        [SingularName("rainbow key")]
        [PluralName("rainbow keys")]
        [Weight(5)]
        [ItemClass(ItemClass.Key)]
        RainbowKey,

        [SingularName("rake")]
        [PluralName("rakes")]
        [WeaponType(WeaponType.Polearm)]
        Rake,

        [SingularName("rakshasan eviscerator")]
        [PluralName("rakshasan eviscerators")]
        [WeaponType(WeaponType.Unknown)]
        RakshasanEviscerator,

        [SingularName("Ranger's compass")]
        //CSRTODO: plural
        RangersCompass,

        [SingularName("red bubbly potion")]
        [PluralName("red bubbly potions")]
        [Potion(SpellsEnum.invisibility)]
        [Weight(1)]
        RedBubblyPotion,

        [SingularName("reddish-orange potion")]
        [PluralName("reddish-orange potions")]
        [Potion(SpellsEnum.mend)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        ReddishOrangePotion,

        [SingularName("red potion")]
        [PluralName("red potions")]
        [Potion(SpellsEnum.endurefire)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        RedPotion,

        [SingularName("repair kit")]
        [PluralName("repair kits")]
        RepairKit,

        [SingularName("ribbed plate boots")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(4)]
        [Sellable(185)]
        RibbedPlateBoots,

        [SingularName("ribbed plate gloves")]
        [EquipmentType(EquipmentType.Hands)]
        [Weight(4)]
        [Sellable(123)]
        RibbedPlateGloves,

        [SingularName("ribbed plate hood")]
        [PluralName("ribbed plate hoods")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(6)]
        [Sellable(371)]
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
        [Weight(1)]
        [Sellable(247)]
        RingOfInvisibility,

        //mages cannot use
        [SingularName("rod of the dead")]
        //CSRTODO: plural
        [Wand(SpellsEnum.removecurse)] //spell per wiki
        [Sellable(24)]
        RodOfTheDead,

        [SingularName("rogue's mask")]
        [PluralName("rogue's masks")]
        [EquipmentType(EquipmentType.Face)]
        [Weight(1)]
        [Sellable(297)]
        RoguesMask,

        [SingularName("ruby")]
        [PluralName("rubys")] //verified 6/21/23
        Ruby,

        [SingularName("rusty key")]
        [PluralName("rusty key")]
        [ItemClass(ItemClass.Key)]
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
        [ItemClass(ItemClass.Gem)]
        [Weight(1)]
        [Sellable(179)]
        Sapphire,

        [SingularName("scythe")]
        [PluralName("scythes")]
        [WeaponType(WeaponType.Unknown)]
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
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(272)]
        SignetRing,

        [SingularName("silima blade")]
        [PluralName("silima blades")]
        [WeaponType(WeaponType.Slash)]
        SilimaBlade,

        [SingularName("silk vest")]
        [PluralName("silk vests")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        SilkVest,

        [SingularName("silver arm-bands")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(595)]
        SilverArmBands,

        [SingularName("Silver-blue scale")]
        [PluralName("Silver-blue scales")]
        [Weight(4)]
        SilverBlueScale,

        [SingularName("silver dagger")]
        [PluralName("silver daggers")]
        [WeaponType(WeaponType.Stab)]
        [Sellable(67)]
        SilverDagger,

        [SingularName("silver key")]
        [PluralName("silver keys")]
        [ItemClass(ItemClass.Key)]
        SilverKey,

        [SingularName("silver scimitar")]
        [PluralName("silver scimitars")]
        [WeaponType(WeaponType.Slash)]
        [Weight(5)]
        SilverScimitar,

        [SingularName("silver wand")]
        [PluralName("silver wands")]
        [Wand(SpellsEnum.hurt)]
        SilverWand,

        [SingularName("slaying sword")]
        [PluralName("slaying swords")]
        [WeaponType(WeaponType.Stab)]
        [Weight(5)]
        [Sellable(562)]
        SlayingSword,

        [SingularName("sling")]
        [PluralName("slings")]
        [WeaponType(WeaponType.Missile)]
        [Weight(5)]
        [Sellable(SellableEnum.Junk)]
        Sling,

        [SingularName("small ash bow")]
        [PluralName("small ash bows")]
        [WeaponType(WeaponType.Missile)]
        SmallAshBow,

        [SingularName("small bag")]
        [PluralName("small bags")]
        [ItemClass(ItemClass.Bag)]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        SmallBag,

        [SingularName("small knife")]
        [PluralName("small knifes")] //verified 6/21/23
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        SmallKnife,

        [SingularName("small metal shield")]
        [PluralName("small metal shields")]
        [EquipmentType(EquipmentType.Shield)]
        [Weight(6)]
        [ArmorClass(0.4)]
        [Sellable(SellableEnum.Junk)]
        SmallMetalShield,

        [SingularName("small pearl")]
        [PluralName("small pearls")]
        SmallPearl,

        [SingularName("small silver chest")]
        [PluralName("small silver chests")]
        [ItemClass(ItemClass.Money)]
        SmallSilverChest,

        [SingularName("small wooden shield")]
        [PluralName("small wooden shields")]
        [EquipmentType(EquipmentType.Shield)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        SmallWoodenShield,

        [SingularName("speckled potion")]
        [PluralName("speckled potions")]
        [Potion(SpellsEnum.unknown)] //stun? "You feel dizzy."
        SpeckledPotion,

        [SingularName("sprite boots")]
        [PluralName("sprite bootses")] //verified 7/5/23
        [EquipmentType(EquipmentType.Feet)]
        [Weight(3)]
        [Sellable(198)]
        SpriteBoots,

        [SingularName("sprite leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(7)]
        [Sellable(185)]
        SpriteLeatherArmor,

        [SingularName("sprite leather boots")]
        [EquipmentType(EquipmentType.Feet)]
        SpriteLeatherBoots,

        [SingularName("sprite leather leggings")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(3)]
        [Sellable(185)]
        SpriteLeatherLeggings,

        [SingularName("splint mail")]
        [EquipmentType(EquipmentType.Unknown)]
        [Weight(15)]
        [Sellable(3093)]
        SplintMail,

        [SingularName("sprite bracelet")]
        [PluralName("sprite bracelets")]
        [EquipmentType(EquipmentType.Unknown)]
        SpriteBracelet,

        [SingularName("spruce-top guitar")]
        [PluralName("spruce-top guitars")]
        [Weight(17)]
        [Sellable(123)]
        SpruceTopGuitar,

        [SingularName("spyglass")]
        [PluralName("spyglasses")]
        Spyglass,

        [SingularName("staff of force")]
        //CSRTODO: plural?
        [WeaponType(WeaponType.Polearm)]
        StaffOfForce,

        [SingularName("steel-chain armor")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(18)]
        [Sellable(569)]
        SteelChainArmor,

        [SingularName("steel reinforced shield")]
        [PluralName("steel reinforced shields")]
        [Weight(8)]
        [Sellable(202)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        SteelReinforcedShield,

        [SingularName("stilleto")]
        //CSRTODO: plural?
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(164)]
        Stilleto,

        [SingularName("stone hammer")]
        [PluralName("stone hammers")]
        [WeaponType(WeaponType.Blunt)]
        [Weight(6)]
        [Sellable(1485)]
        StoneHammer,

        //dropped by sewer orcs
        [SingularName("stone key")]
        [PluralName("stone keys")]
        [ItemClass(ItemClass.Key)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        StoneKey,

        [SingularName("storage sign")]
        [PluralName("storage signs")]
        [ItemClass(ItemClass.Fixed)]
        StorageSign,

        [SingularName("string of pearls")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Neck)]
        [Weight(0)]
        [ArmorClass(0.1)]
        [Sellable(123)]
        StringOfPearls,

        [SingularName("sundorian tassle")]
        [PluralName("sundorian tassles")]
        [EquipmentType(EquipmentType.Unknown)]
        SundorianTassle,

        [SingularName("swirly potion")]
        [PluralName("swirly potions")]
        [Potion(SpellsEnum.protection)]
        SwirlyPotion,

        [SingularName("tattoo of a snake")]
        [PluralName("tattoo of a snake")] //verified 7/5/23
        [EquipmentType(EquipmentType.Arms)]
        [Weight(1)]
        [DisallowedClasses(ClassTypeFlags.Mage)] //verified 7/6/23
        [Sellable(1237)]
        TattooOfASnake,

        [SingularName("tattoo of a wench")]
        //CSRTODO: plural?
        [EquipmentType(EquipmentType.Arms)]
        [Weight(1)]
        [Sellable(1237)]
        [DisallowedClasses(ClassTypeFlags.Mage)] //verified 7/3/23
        TattooOfAWench,

        [SingularName("taupe scroll")]
        [PluralName("taupe scrolls")]
        [Scroll(SpellsEnum.shatterstone)]
        TaupeScroll,

        [SingularName("T-bone")]
        [PluralName("T-bones")]
        [Weight(4)]
        [Sellable(SellableEnum.Junk)]
        TBone,

        [SingularName("thieves' boots")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(3)]
        [ArmorClass(0.2)]
        [Sellable(193)]
        ThievesBoots,

        [SingularName("throwing axe")]
        [PluralName("throwing axes")]
        [WeaponType(WeaponType.Missile)]
        ThrowingAxe,

        [SingularName("tiger shark leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        TigerSharkLeatherArmor,

        //dropped by caretaker
        [SingularName("tin key")]
        [PluralName("tin keys")]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        TinKey,

        [SingularName("toilet paper")] //verified collective 7/5/23
        [Wand(SpellsEnum.stun)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        ToiletPaper,

        [SingularName("tomb key")]
        [PluralName("tomb keys")]
        [ItemClass(ItemClass.Key)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        TombKey,

        [SingularName("topaz")]
        [PluralName("topazs")] //verified 7/10/23
        [ItemClass(ItemClass.Gem)]
        [Weight(3)]
        [Sellable(308)]
        Topaz,

        [SingularName("torch")]
        [PluralName("torchs")] //verified 6/21/23
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        Torch,

        [SingularName("town map")]
        [PluralName("town maps")]
        TownMap,

        [SingularName("training key")]
        [PluralName("training keys")]
        [ItemClass(ItemClass.Key)]
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

        [SingularName("vanishing cream")]
        [PluralName("vanishing creams")]
        VanishingCream,

        [SingularName("verdant green scroll")]
        [PluralName("verdant green scrolls")]
        [Scroll(SpellsEnum.protection)]
        VerdantGreenScroll,

        [SingularName("very old key")]
        [PluralName("very old keys")]
        [ItemClass(ItemClass.Key)]
        VeryOldKey,

        [SingularName("viscous potion")]
        [PluralName("viscous potions")]
        [Potion(SpellsEnum.unknown)] //CSRTODO: what is this, causes small amount of damage
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
        [WeaponType(WeaponType.Slash)]
        [Weight(1)]
        [Sellable(173)]
        Voulge,

        [SingularName("wagonmaster's whip")]
        [PluralName("wagonmaster's whips")]
        [WeaponType(WeaponType.Missile)]
        [Weight(3)]
        [Sellable(1262)]
        WagonmastersWhip,

        [SingularName("warhammer")]
        [PluralName("warhammers")]
        [WeaponType(WeaponType.Unknown)]
        Warhammer,

        [SingularName("war harness")]
        [PluralName("war harnesses")]
        [EquipmentType(EquipmentType.Waist)]
        [Weight(3)]
        [ArmorClass(0.4)]
        WarHarness,

        [SingularName("War's flaming axe")]
        [PluralName("War's flaming axes")]
        [WeaponType(WeaponType.Unknown)]
        WarsFlamingAxe,

        [SingularName("welcome sign")]
        [PluralName("welcome signs")]
        [ItemClass(ItemClass.Fixed)]
        WelcomeSign,

        [SingularName("white armor")]
        [PluralName("white armors")] //verified 6/29/23
        [EquipmentType(EquipmentType.Torso)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(15)]
        WhiteArmor,

        [SingularName("yellow beholder's eye")]
        [PluralName("yellow beholder's eyes")]
        [Wand(SpellsEnum.lightning)]
        YellowBeholdersEye,

        [SingularName("yellow potion")]
        [PluralName("yellow potions")]
        [Potion(SpellsEnum.vigor)]
        [Weight(3)]
        YellowPotion,
    }
}
