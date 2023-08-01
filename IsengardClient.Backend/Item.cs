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
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(SexRestrictionAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0) sid.SexRestriction = ((SexRestrictionAttribute)valueAttributes[0]).Sex;
                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(LookTextAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0)
                {
                    sid.LookText = ((LookTextAttribute)valueAttributes[0]).LookText;
                    sid.LookTextType = LookTextType.Known;
                }

                valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(LookTextTypeAttribute), false);
                if (valueAttributes != null && valueAttributes.Length > 0) sid.LookTextType = ((LookTextTypeAttribute)valueAttributes[0]).LookTextType;

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
            foreach (string s in StringProcessing.PickWords(sSingular, true))
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

        public bool IsItemClass(ItemClass ic)
        {
            bool ret = false;
            if (ItemType.HasValue)
            {
                StaticItemData sid = StaticItemData[ItemType.Value];
                ret = sid.ItemClass == ic;
            }
            return ret;
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
        public SelectedInventoryOrEquipmentItem(ItemEntity ie, ItemTypeEnum? ItemType, int Counter, ItemLocationType LocationType)
        {
            ItemEntity = ie;
            this.ItemType = ItemType;
            this.Counter = Counter;
            this.LocationType = LocationType;
        }
        public ItemEntity ItemEntity;
        public ItemTypeEnum? ItemType;
        public int Counter;
        public ItemLocationType LocationType;
    }

    public enum ColumnType
    {
        None,
        Source,
        Target,
        SellOrJunk,
        Tick,
        Inventory,
        Equipment,
    }

    public class SelectedItemWithTarget
    {
        public ItemEntity ItemEntity;
        public ItemTypeEnum ItemType;
        public int Counter;
        public ItemManagementLocationType LocationType;
        public ColumnType Target;
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
        /// look text for the item
        /// </summary>
        public string LookText { get; set; }
        /// <summary>
        /// what kind of look text it is
        /// </summary>
        public LookTextType LookTextType { get; set; }
        /// <summary>
        /// weight of the item. Null if unknown. It's nullable because a zero pound item has been observed (string of pearls)
        /// </summary>
        public int? Weight { get; set; }
        public decimal ArmorClass { get; set; }
        public SellableEnum Sellable { get; set; }
        public int SellGold { get; set; }
        public ClassTypeFlags DisallowedClasses { get; set; }
        public SexEnum? SexRestriction { get; set; }
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
            else if (ic == ItemClass.Instrument)
            {
                yield return DynamicDataItemClass.Instrument;
            }
            else if (ic == ItemClass.HeldItem)
            {
                yield return DynamicDataItemClass.HeldItem;
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
        Instrument,
        Coins,
        Money,
        Fixed,
        Chest,
        HeldItem,
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
        Instrument,
        Coins,
        Money,
        HeldItem,

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
        FactoryKey = 64,
        RustyKey = 128,
        All = 255,
    }

    /// <summary>
    /// item enums. There are three cases:
    /// 1. ordinary items have singular and plural names. These have both singular and plural attributes.
    /// 2. coin items use "X gold coins" and "sets of X gold coins" formats. These currently have both singular and plural attributes.
    /// 3. collective items only have a singular name, and use "sets of X" for the plural case. These currently only have a singular attribute.
    /// </summary>
    public enum ItemTypeEnum
    {
        [SingularName("adamantine blade")]
        [PluralName("adamantine blades")]
        [LookText("You see an ornate blade that shimmers of pure adamantine.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(5)]
        [Sellable(927)]
        AdamantineBlade,

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
        [LookText("You see some heavy gloves made of scale mail from adamantine ores.")]
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
        [LookText("You see some shiny scale mail sleeves made of strong adamantine.")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(8)]
        [ArmorClass(0.4)]
        [Sellable(185)]
        AdamantineScaleMailSleeves,

        [SingularName("Ahrot's magic string")]
        [PluralName("Ahrot's magic strings")]
        AhrotsMagicString,

        [SingularName("alligator leather")] //verified collective 7/20/23
        [SingularSelection("alligator leather boots")]
        [LookText("hardened alligator leather.")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(3)]
        [ArmorClass(0.6)]
        [Sellable(1039)]
        AlligatorLeather,

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
        [PluralName("Ancient Shield of Adalphis")] //verified 7/23/23
        [LookText("You see a marvelous shield made by a great wizard.")]
        [EquipmentType(EquipmentType.Shield)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(61)]
        AncientShieldOfAdalphi,

        [SingularName("animal hides")] //verified collective 7/29/23
        [LookText("The hide looks as though it could make a decent piece of armor.")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        AnimalHides,

        [SingularName("anvil")]
        [PluralName("anvils")]
        [LookText("A blackened steel forge that is glowing red.")]
        [Weight(12)]
        [Sellable(371)]
        Anvil,

        [SingularName("aquamarine potion")]
        [PluralName("aquamarine potions")]
        [Potion(SpellsEnum.levitate)]
        AquamarinePotion,

        [SingularName("assassin's dagger")]
        [PluralName("assassin's daggers")]
        [LookText("It's a blackened steel blade, perfect for backstabbing.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(3)]
        [Sellable(3168)]
        AssassinsDagger,

        [SingularName("assassin's mask")]
        [PluralName("assassin's masks")]
        [LookText("You see a black mask that would completely cover your face.")]
        [EquipmentType(EquipmentType.Face)]
        [Weight(1)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        AssassinsMask,

        [SingularName("A statuette of Balthazar")]
        [SingularSelection("A")]
        //CSRTODO: no plural?
        [LookTextType(LookTextType.Unknown)] //unknown because "look all" takes precedence over looking at it
        [ItemClass(ItemClass.Fixed)]
        AStatuetteOfBalthazar,

        [SingularName("azure scroll")]
        [PluralName("azure scrolls")]
        [LookText("You see a blue toned scroll with some wierd writing on it.")]
        [Scroll(SpellsEnum.blister)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        AzureScroll,

        [SingularName("bag")]
        [PluralName("bags")]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        Bag,

        [SingularName("banded mail armor")]
        [EquipmentType(EquipmentType.Torso)]
        BandedMailArmor,

        [SingularName("bastard sword")]
        [PluralName("bastard swords")]
        [LookText("You see a large, powerful sword.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(5)]
        [Sellable(137)]
        BastardSword,

        [SingularName("beastmaster's whip")]
        [PluralName("beastmaster's whips")]
        [LookText("You see a heavy dinosaur hide whip with a heavy chain and mithril tip.")]
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
        [LookText("You see nothing special about it.")]
        [Scroll(SpellsEnum.light)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        BlackenedScroll,

        [SingularName("black rune")]
        [PluralName("black runes")]
        [ItemClass(ItemClass.Fixed)]
        BlackRune,

        [SingularName("black scroll")]
        [PluralName("black scrolls")]
        [LookText("The scroll is as black as charcoal, its strange words written in white.")]
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
        [LookText("These vestments are the typical clothing of priests and monks.")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(2)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        BlackVestments,

        [SingularName("blue bubbly potion")]
        [PluralName("blue bubbly potions")]
        [LookText("You see a blue potion that fizzes when you swirl it.")]
        [Potion(SpellsEnum.detectinvis)]
        [Weight(2)]
        [Sellable(SellableEnum.NotSellable)]
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
        [LookText("This is the coveted armor of the troll chieftain, Oohlgrist.")]
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
        [LookTextType(LookTextType.Multiline)]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        BookOfKnowledge,

        [SingularName("boomerang")]
        [PluralName("boomerangs")]
        [LookText("You see nothing special about it.")]
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
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(4)]
        [Sellable(SellableEnum.Junk)]
        BoStick,

        [SingularName("box of strawberries")]
        [LookText("It looks like about a pint of red berries.")]
        [ItemClass(ItemClass.HeldItem)]
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
        [LookText("It looks like it may fit in one of the hatchways.")]
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
        [LookText("You see a large white shinny key.")]
        [ItemClass(ItemClass.Key)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        CarvedIvoryKey,

        [SingularName("carving knife")]
        [PluralName("carving knifes")] //verified 7/29/23
        [LookText("It's similar in size to a pocket knife, but much sharper.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        CarvingKnife,

        //has a strength requirement
        [SingularName("cat o' nine tails")]
        [PluralName("cat o' nine tailses")] //verified 7/3/23
        [LookText("A whip with nine ends, each with pieces of metal or bone at the end.")]
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
        [LookText("You see a gauntlet with sharp claws protruding from the end.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(3)]
        ClawGauntlet,

        [SingularName("cloth armor")] //verified collective 7/18/23
        [LookText("You see nothing special about it.")]
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
        [LookText("You see nothing special about it.")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        ClothHat,

        [SingularName("cloth pants")]
        [LookText("You see nothing special about it.")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(2)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        ClothPants,

        [SingularName("club")]
        [PluralName("clubs")]
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Blunt)]
        [Weight(2)]
        [Sellable(24)]
        Club,

        [SingularName("copper pieces")]
        [PluralName("copper pieces")]
        [LookText("You see nothing special about it.")]
        [ItemClass(ItemClass.Coins)]
        CopperPieces,

        [SingularName("copper ring")]
        [PluralName("copper rings")]
        [LookText("This ring will turn your finger green.")]
        [EquipmentType(EquipmentType.Finger)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        CopperRing,

        [SingularName("cracker")]
        [PluralName("crackers")]
        [LookText("A small party snapper.")]
        [Use(SpellsEnum.rumble)]
        [Weight(1)]
        [Sellable(24)]
        Cracker,

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
        [LookText("You see a brilliant crystal trinket attached to a golden necklace.")]
        [EquipmentType(EquipmentType.Unknown)]
        CrystalAmulet,

        [SingularName("crystal scepter")]
        [PluralName("crypstal scepters")]
        [LookText("You see a brilliant gold scepter with a headpiece of pure white crystal.")]
        [Weight(6)]
        [Sellable(618)]
        CrystalScepter,

        [SingularName("cutlass")]
        [PluralName("cutlasses")]
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(4)]
        [Sellable(91)]
        Cutlass,

        [SingularName("dagger")]
        [PluralName("daggers")]
        [LookText("A well balanced knife.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        Dagger,

        [SingularName("dagger of impaling")]
        //CSRTODO: plural
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(297)]
        DaggerOfImpaling,

        [SingularName("dark blade")]
        [PluralName("dark blades")]
        [WeaponType(WeaponType.Slash)]
        DarkBlade,

        [SingularName("dark goggles")]
        //CSRTODO: plural
        [EquipmentType(EquipmentType.Face)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(123)]
        DarkGoggles,

        [SingularName("dark green potion")]
        [PluralName("dark green potions")]
        [LookText("You see a deep green liquid in a heavy, lead beaker.")]
        [Potion(SpellsEnum.detectmagic)]
        [Weight(2)] //CSRTODO: wiki says this could also be 4
        [Sellable(SellableEnum.NotSellable)]
        DarkGreenPotion,

        [SingularName("dark flask")]
        [PluralName("dark flasks")]
        [LookText("You see a flask of goblin gruel.")]
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
        [LookText("It's a sparkling diamond of at least 18 carats.")]
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

        [SingularName("diamond-studded stiletto")]
        //CSRTODO: plural
        [LookText("It's a beautiful weapon, completely covered in gold and diamonds.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(6)]
        [Sellable(10000)]
        DiamondStuddedStiletto,

        [SingularName("dildo")]
        [PluralName("dildos")]
        Dildo,

        [SingularName("dirk")]
        [PluralName("dirks")]
        [LookText("A small, but very sharp, dagger.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(163)]
        Dirk,

        [SingularName("double bladed axe")]
        [PluralName("double bladed axes")]
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(8)]
        [Sellable(99)]
        DoubleBladedAxe,

        [SingularName("dragon eye")]
        [PluralName("dragon eyes")]
        [LookText("You see a metal band for use around the neck.")]
        [Weight(1)]
        DragonEye,

        //mage training level 15
        [SingularName("dragon key")]
        [PluralName("dragon keys")]
        [LookText("The handle of this key is shaped like a dragon.")]
        [Weight(1)]
        DragonKey,

        [SingularName("dried seaweed")]
        //CSRTODO: plural?
        DriedSeaweed,

        [SingularName("dungeon key")]
        [PluralName("dungeon keys")]
        [ItemClass(ItemClass.Key)]
        DungeonKey,

        [SingularName("dwarven mithril gaiters")] //collective plural, verified 7/21/23
        [SingularSelection("dwarven mithril gaiter")]
        [LookText("You see a shiny pair of long boots, made by the dwarves of Saeros.")]
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
        [LookText("You see some exquisite elven leather armor.")]
        [EquipmentType(EquipmentType.Torso)]
        [Weight(1)]
        [ArmorClass(1.2)]
        [Sellable(247)]
        ElvenCuredLeatherArmor,

        [SingularName("elven cured leather boots")] //collective plural verified 7/6/23
        [LookText("You see some well crafted boots that are so light, you don't feel them on.")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(61)]
        ElvenCuredLeatherBoots,

        [SingularName("elven cured leather gloves")] //collective plural verified 7/6/23
        [LookText("You see a fantastic pair of leather gloves.")]
        [EquipmentType(EquipmentType.Hands)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(61)]
        ElvenCuredLeatherGloves,

        [SingularName("elven cured leather hood")]
        [PluralName("elven cured leather hoods")]
        [LookText("You see an elven hood which fits around your head like a glove.")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(1)]
        [ArmorClass(0.8)]
        [Sellable(185)]
        ElvenCuredLeatherHood,

        [SingularName("elven cured leather leggings")] //collective plural verified 7/6/23
        [LookText("You see a smooth elven pair of leggings to match your armor.")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(123)]
        ElvenCuredLeatherLeggings,

        [SingularName("elven cured leather sleeves")] //collective plural verified 7/6/23
        [LookText("You see some cured leather sleeves to match the elven leather armor.")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(1)]
        [ArmorClass(0.4)]
        [Sellable(123)]
        ElvenCuredLeatherSleeves,

        [SingularName("elven leather whip")]
        [PluralName("elven leather whips")]
        [LookText("You see a finely crafted bull whip used mostly for crowd control.")]
        [WeaponType(WeaponType.Missile)]
        [Weight(5)]
        [Sellable(148)]
        ElvenLeatherWhip,

        [SingularName("emerald")]
        [PluralName("emeralds")]
        [LookText("The green jewel is breath-taking in its simple beauty.")]
        [ItemClass(ItemClass.Gem)]
        [Weight(1)]
        [Sellable(432)]
        Emerald,

        [SingularName("emerald collar")]
        [PluralName("emerald collars")]
        [LookText("You see a leather collar studded with emeralds")]
        [EquipmentType(EquipmentType.Neck)]
        [SexRestriction(SexEnum.Female)]
        [Weight(4)]
        [Sellable(495)]
        EmeraldCollar,

        [SingularName("engagement ring")]
        [PluralName("engagement rings")]
        [LookText("It's a beautifull engagement ring of gold and diamonds.")]
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

        [SingularName("factory key")]
        [PluralName("factory keys")]
        [LookText("A shiny metal key in the shape of a flowing wave.")]
        [ItemClass(ItemClass.Key)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        FactoryKey,

        [SingularName("Faded map")]
        [PluralName("Faded maps")]
        [LookText("You see a map written in a strange language, who could possibly decipher it?")]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        FadedMap,

        [SingularName("firecracker")]
        [PluralName("firecrackers")]
        [LookText("This looks more like a party-popper.")]
        [Use(SpellsEnum.burn)]
        [Weight(1)]
        [Sellable(24)]
        Firecracker,

        [SingularName("fish mail hood")]
        [PluralName("fish mail hoods")]
        [EquipmentType(EquipmentType.Head)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(5)]
        [Sellable(1485)]
        FishMailHood,

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

        [SingularName("galvorn shield")]
        [PluralName("galvorn shields")]
        [EquipmentType(EquipmentType.Shield)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(7)]
        [Sellable(4950)]
        GalvornShield,

        [SingularName("gate key")]
        [PluralName("gate keys")]
        [ItemClass(ItemClass.Key)]
        GateKey,

        [SingularName("gate warning")]
        [PluralName("gate warnings")]
        [LookTextType(LookTextType.Multiline)]
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
        [LookText("You see a large stone key attached to a leather rope.")]
        [ItemClass(ItemClass.Key)]
        GirionsKey,

        [SingularName("glimmering blade")]
        [PluralName("glimmering blades")]
        [WeaponType(WeaponType.Slash)]
        [Weight(15)]
        GlimmeringBlade,

        [SingularName("glitter")]
        //CSRTODO: glitter plural? (does not use some)
        [LookText("This metallic substance reflects light in all directions.")]
        [Wand(SpellsEnum.stun)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
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
        [LookText("My, how it glitters.")]
        [ItemClass(ItemClass.Coins)]
        GoldCoins,

        [SingularName("golden dagger")]
        [PluralName("golden daggers")]
        [LookText("You see a dagger made of gold.  It's the weapon of a lamia.")]
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
        [LookText("You see a shimmering 24 K gold ring.")]
        [EquipmentType(EquipmentType.Finger)] //when reworn, shocks and drops if cursed
        [Weight(2)]
        [ArmorClass(0.1)]
        [Sellable(123)]
        GoldRing,

        [SingularName("gold sword")]
        [PluralName("gold swords")]
        [LookText("You see a remarkable gold sword, used mostly for decoration.")]
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
        [LookText("It's a lightly crinkled document, with archaic words written upon it.")]
        [Scroll(SpellsEnum.vigor)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        GreyScroll,

        [SingularName("grey staff")]
        //CSRTODO: plural
        [LookText("It's a tall, grey staff made of gnarled oak wood.")]
        [Wand(SpellsEnum.vigor)]
        [Weight(3)]
        [Sellable(118)]
        GreyStaff,

        [SingularName("guide to the Ituk Glacier")]
        //CSRTODO: plural
        [Weight(0)]
        [Sellable(SellableEnum.Junk)]
        GuideToTheItukGlacier,

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
        [LookText("A large pole with a blade afixed to the tip ending in a spear.")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(12)]
        [Sellable(176)]
        Halberd,

        [SingularName("half-giant chain mail armor")]
        [EquipmentType(EquipmentType.Torso)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
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
        [LookText("You see some long chain mail leggings, made for very large people.")]
        [EquipmentType(EquipmentType.Legs)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(15)]
        [Sellable(185)]
        HalfGiantChainMailLeggings,

        [SingularName("half-giant chain mail sleeves")]
        [LookText("You see some large chain mail sleeves made specially for half-giants.")]
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
        [Weight(10)]
        [ArmorClass(0.5)]
        [Sellable(247)]
        HardwoodShield,

        [SingularName("hazy potion")]
        [PluralName("hazy potions")]
        [LookText("The potion is a murky grey color, with a slight blue hue.")]
        [Potion(SpellsEnum.wordofrecall)]
        [Weight(5)]
        [Sellable(SellableEnum.NotSellable)]
        HazyPotion,

        [SingularName("head of lettuce")]
        //CSRTODO: plural?
        HeadOfLettuce,

        [SingularName("heavy cloth leggings")]
        //CSRTODO: plural
        [LookText("You see some leggings.  They are very warm for the cold mountain air.")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(8)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        HeavyClothLeggings,

        [SingularName("herbal stimulant")]
        [PluralName("herbal stimulants")]
        HerbalStimulant,

        [SingularName("hood of the high priest")]
        //CSRTODO: plural
        [EquipmentType(EquipmentType.Head)]
        HoodOfTheHighPriest,

        [SingularName("horseman's mace")]
        [PluralName("horseman's maces")]
        [LookText("You see a large mace made specifically for fighting on horseback.")]
        [WeaponType(WeaponType.Blunt)]
        [Sellable(117)]
        HorsemansMace,

        [SingularName("human brain")]
        [PluralName("human brains")]
        HumanBrain,

        [SingularName("human carcass")]
        [PluralName("human carcasses")] //verified 7/20/23
        [LookText("You see the remains of an unlucky human partially consumed by ghouls.")]
        HumanCarcass,

        [SingularName("ice blue potion")]
        [PluralName("ice blue potions")]
        [LookText("You see a vial of some slushy, blue liquid.")]
        [Potion(SpellsEnum.fly)]
        [Weight(2)]
        [Sellable(SellableEnum.NotSellable)]
        IceBluePotion,

        [SingularName("ice cube")]
        [PluralName("ice cubes")]
        [LookText("A cube of ice with a fly in the center.")]
        [Use(SpellsEnum.blister)]
        [Weight(1)]
        [Sellable(24)]
        IceCube,

        [SingularName("information kiosk")]
        [PluralName("information kiosks")]
        [ItemClass(ItemClass.Fixed)]
        InformationKiosk,

        //mage training level 13
        [SingularName("invisible key")]
        [PluralName("invisible keys")]
        [ItemClass(ItemClass.Key)]
        InvisibleKey,

        [SingularName("iron leggings")]
        //CSRTODO: plural
        [LookText("You see some iron leggings from a knight's suit of armor.")]
        [EquipmentType(EquipmentType.Legs)]
        [Weight(2)]
        [ArmorClass(0.4)]
        [Sellable(123)]
        IronLeggings,

        [SingularName("iron ring")]
        [PluralName("iron rings")]
        [LookText("You see a ring made of wrought iron.")]
        [EquipmentType(EquipmentType.Finger)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        IronRing,

        [SingularName("iron skull")]
        [PluralName("iron skulls")]
        [LookText("A iron figurine forged in the shape of a humanoid skull.")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(4)]
        [ArmorClass(1)]
        [Sellable(49)]
        IronSkull,

        [SingularName("iron spear")]
        [PluralName("iron spears")]
        [LookText("You see an iron spear which appears to be glowing faintly.")]
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
        [LookText("a large red iron key, it feals very heavy.")]
        [ItemClass(ItemClass.Key)]
        [Weight(3)]
        [Sellable(SellableEnum.Junk)]
        KasnarsRedKey,

        [SingularName("kelp necklace")]
        [PluralName("kelp necklaces")]
        [LookText("It is a loop of river kelp interlaced with small fresh water pearls.")]
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
        [LookText("This is the precious egg of the mighty condor.")]
        [Use(SpellsEnum.curemalady)]
        [Weight(3)]
        [Sellable(49)]
        LargeEgg,

        [SingularName("large metal shield")]
        [PluralName("large metal shields")]
        [LookText("You see nothing special about it.")]
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
        [LookText("The hammer is a large blunt object capable of doing lots of damage.")]
        [WeaponType(WeaponType.Blunt)]
        [Weight(5)]
        [Sellable(643)]
        LeadHammer,

        [SingularName("leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        LeatherArmor,

        [SingularName("leather boots")]
        [LookText("You see nothing special about it.")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(4)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        LeatherBoots,

        [SingularName("leather cap")]
        [PluralName("leather caps")]
        [LookText("You see nothing special about it.")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(2)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        LeatherCap,

        [SingularName("leather gloves")]
        [LookText("You see nothing special about it.")]
        [EquipmentType(EquipmentType.Hands)]
        [Weight(2)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        LeatherGloves,

        [SingularName("leather pouch")]
        [PluralName("leather pouches")]
        LeatherPouch,

        [SingularName("leather sleeves")]
        [LookText("You see nothing special about it.")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(3)]
        [ArmorClass(0.2)]
        [Sellable(SellableEnum.Junk)]
        LeatherSleeves,

        [SingularName("light leather armor")]
        [EquipmentType(EquipmentType.Torso)]
        LightLeatherArmor,

        [SingularName("little brown jug")]
        [PluralName("little brown jugs")]
        [LookText("You see a small brown jug with a cork in it.")]
        [Potion(SpellsEnum.endurecold)]
        [Weight(3)]
        [Sellable(SellableEnum.Junk)]
        LittleBrownJug,

        [SingularName("lollipop")]
        [PluralName("lollipops")]
        [LookText("This sticky item has just ruined your gloves.")]
        [Potion(SpellsEnum.curedisease)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        Lollipop,

        [SingularName("long bow")]
        [PluralName("long bows")]
        [WeaponType(WeaponType.Unknown)]
        LongBow,

        [SingularName("long sword")]
        [PluralName("long swords")]
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(5)]
        [Sellable(59)]
        LongSword,

        [SingularName("lunch money")]
        [ItemClass(ItemClass.Money)]
        //CSRTODO: plural?
        LunchMoney,

        [SingularName("magical tabulator")]
        [PluralName("magical tabulators")]
        [LookText("Seems to be a mystic calculator.  But what could it calculate, I wonder?")]
        [Wand(SpellsEnum.detectmagic)]
        [Weight(5)]
        [Sellable(120)]
        MagicalTabulator,

        [SingularName("magical temper")]
        //CSRTODO: plural?
        MagicalTemper,

        [SingularName("marble chess set")]
        [PluralName("marble chess sets")]
        [LookText("The pieces are each hand carved and the board is very solid")]
        [Weight(12)]
        [Sellable(49)]
        MarbleChessSet,

        [SingularName("mask of darkness")]
        //CSRTODO: plural?
        [LookText("You see a stylish, black mask made of translucent galvorn.")]
        [EquipmentType(EquipmentType.Face)]
        [Weight(3)]
        [ArmorClass(0.6)]
        [Sellable(742)]
        MaskOfDarkness,

        [SingularName("mask of distortion")]
        [PluralName("mask of distortions")] //verified 7/9/23
        [LookText("You see a strange looking mask that appears to distort your image.")]
        [EquipmentType(EquipmentType.Face)]
        [Weight(3)]
        [ArmorClass(0.5)]
        [Sellable(308)]
        MaskOfDistortion,

        [SingularName("mauve scroll")]
        [PluralName("mauve scrolls")]
        [LookText("The scroll is written upon a burgundy colored paper.")]
        MauveScroll,

        [SingularName("metal helmet")]
        [PluralName("metal helmets")]
        [LookText("It's a simple helmet, forged in normal metal.")]
        [EquipmentType(EquipmentType.Head)]
        [Weight(5)]
        [ArmorClass(0.4)]
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
        [LookText("It is beautiful armor, made by the dwarves")]
        [EquipmentType(EquipmentType.Torso)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(18)]
        [Sellable(6187)]
        MithrilChainArmor,

        [SingularName("mithril jo stick")]
        [PluralName("mithril jo sticks")]
        [LookText("You see a ebony stick inlaid with ornate mithril.")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(5)]
        [Sellable(2376)]
        MithrilJoStick,

        [SingularName("mithril lamella leggings")]
        [LookText("You see a set of mithril armor for the legs.")]
        [EquipmentType(EquipmentType.Legs)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
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
        [LookText("You see a long hood made from an alloy of mithril and iron.")]
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
        [LookText("A tattoo of a heart with MOM written across it.")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(1)]
        [Sellable(1237)]
        [DisallowedClasses(ClassTypeFlags.Mage)] //verified 7/3/23
        MOMTattoo,

        [SingularName("moon pearl")]
        [PluralName("moon pearls")]
        [LookText("A small milky white pearl.")]
        [EquipmentType(EquipmentType.Ears)]
        [Weight(1)]
        [ArmorClass(0.1)]
        [Sellable(35)]
        MoonPearl,

        [SingularName("morning star")]
        [PluralName("morning stars")]
        [WeaponType(WeaponType.Unknown)]
        MorningStar,

        [SingularName("mountain boots")]
        //CSRTODO: plural
        [LookText("You see a pair of boots, ruggedly designed for mountain climbing.")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(2)]
        [ArmorClass(0.1)]
        [Sellable(SellableEnum.Junk)]
        MountainBoots,

        [SingularName("musical score")]
        [PluralName("musical scores")]
        [LookText("These sheets of music could be for a choir but it is hard to tell by looking.")]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        MusicalScore,

        [SingularName("nunchukus")]
        [WeaponType(WeaponType.Polearm)]
        //CSRTODO: plural
        Nunchukus,

        [SingularName("old wooden sign")]
        [PluralName("old wooden signs")]
        [LookText("BEWARE... A boat crew died on this spot with their throats ripped out")]
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
        [LookText("It's an aqueous solution with a slightly orange tint.")]
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
        [LookText("You see a large knife with a bone handle")]
        [WeaponType(WeaponType.Slash)]
        [Weight(5)]
        [Sellable(841)]
        OrkBlade,

        [SingularName("overturned wagon")]
        [PluralName("overturned wagons")]
        [LookText("You see a broken wagon covered in scorchmarks and dried blood.")]
        [ItemClass(ItemClass.Fixed)]
        OverturnedWagon,

        [SingularName("out of order sign")]
        [PluralName("out of order signs")]
        [LookText("The sign says, \"Out of order until further notice\"")]
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

        [SingularName("pearl handled blade")]
        [PluralName("pearl handled blades")]
        [LookText("You see a razor edged blade fitted with a solid pearl handle.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(4)]
        [Sellable(183)]
        PearlHandledBlade,

        [SingularName("pearl handled knife")]
        [PluralName("pearl handled knives")]
        [WeaponType(WeaponType.Unknown)]
        PearlHandledKnife,

        [SingularName("petrified morning star")]
        [PluralName("petrified morning stars")]
        [WeaponType(WeaponType.Unknown)]
        PetrifiedMorningStar,

        [SingularName("philosopher's stone")]
        [PluralName("philosopher's stones")]
        [LookText("You see a mystical stone which is rumored to have magical healing properties.")]
        [Wand(SpellsEnum.heal)] //CSRTODO: not sure about this, says "You feel incredibly better." when zapping.
        PhilosophersStone,

        [SingularName("piece of coal")]
        //CSRTODO: plural?
        [Weight(10)]
        PieceOfCoal,

        [SingularName("pipe weed")]
        //CSRTODO: plural?
        [LookText("You see some dried leaves and flowers.")]
        [Wand(SpellsEnum.stun)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        PipeWeed,

        [SingularName("platinum conch")]
        //CSRTODO: plural
        PlatinumConch,

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
        [LookTextType(LookTextType.Multiline)]
        [ItemClass(ItemClass.Fixed)]
        PortManifest,

        [SingularName("pot helm")]
        [PluralName("pot helms")]
        [LookText("You see nothing special about it.")]
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
        [LookText("It's a purple wand, emitting a glowing light.")]
        [Wand(SpellsEnum.stun)]
        [Weight(5)]
        [Sellable(74)]
        PurpleWand,

        [SingularName("quarterstaff")]
        [PluralName("quarterstaffs")]
        [LookText("a wooden stave.")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        Quarterstaff,

        [SingularName("quartz stone")]
        [PluralName("quartz stones")]
        [LookText("It glows curiously.")]
        [Weight(5)]
        [Wand(SpellsEnum.restore)] //CSRTODO: is this correct?
        [Sellable(495)]
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
        [LookText("You see a strange, reddish-orange viscous liquid in a glass beaker.")]
        [Potion(SpellsEnum.mend)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        ReddishOrangePotion,

        [SingularName("red potion")]
        [PluralName("red potions")]
        [LookText("You see a strange-looking potion in a darkened beaker.")]
        [Potion(SpellsEnum.endurefire)]
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        RedPotion,

        [SingularName("repair kit")]
        [PluralName("repair kits")]
        [LookText("A kit for repairing weapons and armor.")]
        [Weight(2)]
        [Sellable(24)]
        RepairKit,

        [SingularName("ribbed plate armor")]
        //CSRTODO: plural
        [LookText("The armor is heavily reinforced with metal ribbing.")]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(15)]
        [Sellable(496)]
        RibbedPlateArmor,

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
        [LookText("You see a white rod carved from bone.")]
        [Wand(SpellsEnum.removecurse)] //spell per wiki
        [Sellable(24)]
        RodOfTheDead,

        [SingularName("rogue's mask")]
        [PluralName("rogue's masks")]
        [EquipmentType(EquipmentType.Face)]
        [Weight(1)]
        [Sellable(297)]
        RoguesMask,

        [SingularName("rope of snaring")]
        [PluralName("rope of snarings")] //verified 7/29/23
        [LookText("You see a long rope that tangles everything it comes near.")]
        [Use(SpellsEnum.fumble)]
        [Weight(0)]
        [Sellable(SellableEnum.Junk)]
        RopeOfSnaring,

        [SingularName("ruby")]
        [PluralName("rubys")] //verified 6/21/23
        Ruby,

        [SingularName("rusty key")]
        [PluralName("rusty keys")]
        [LookText("The key is completely rusted over.")]
        [ItemClass(ItemClass.Key)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        RustyKey,

        [SingularName("sabre")]
        [PluralName("sabres")]
        [LookText("This straight, single-edged blade is a dangerous weapon.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(193)]
        Sabre,

        [SingularName("sack of potatoes")]
        [PluralName("sack of potatoeses")] //verified 6/14/2023
        SackOfPotatoes,

        [SingularName("sailor's locket")]
        [PluralName("sailor's lockets")]
        [EquipmentType(EquipmentType.Unknown)]
        SailorsLocket,

        [SingularName("sand crystal")]
        [PluralName("sand crystals")]
        [LookText("It gleams like quartz.")]
        SandCrystal,

        [SingularName("sapphire")]
        [PluralName("sapphires")]
        [LookText("You see nothing special about it.")]
        [ItemClass(ItemClass.Gem)]
        [Weight(1)]
        [Sellable(179)]
        Sapphire,

        [SingularName("scythe")]
        [PluralName("scythes")]
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(5)]
        [Sellable(73)]
        Scythe,

        [SingularName("sextant")]
        [PluralName("sextants")]
        Sextant,

        [SingularName("sign")]
        [PluralName("signs")]
        [LookText("Passage to Tharbald.")]
        [ItemClass(ItemClass.Fixed)]
        Sign,

        [SingularName("signet ring")]
        [PluralName("signet rings")]
        [LookText("A token of state power.")]
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
        //CSRTODO: plural
        [LookText("They're made from the finest silver.")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(5)]
        [ArmorClass(0.2)]
        [Sellable(595)]
        SilverArmBands,

        [SingularName("Silver-blue scale")]
        [PluralName("Silver-blue scales")]
        [LookText("The shimmering scale of a large dragon is almost impenetrable.")]
        [EquipmentType(EquipmentType.Unknown)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(4)]
        [Sellable(247)]
        SilverBlueScale,

        [SingularName("silver-blue staff")]
        [PluralName("silver-blue staffs")]
        [LookText("You see a shimmering staff forged from the scales of Smaug the bard killer.")]
        [WeaponType(WeaponType.Polearm)]
        [Weight(6)]
        [Sellable(2103)]
        SilverBlueStaff,

        [SingularName("silver dagger")]
        [PluralName("silver daggers")]
        [WeaponType(WeaponType.Stab)]
        [Sellable(67)]
        SilverDagger,

        [SingularName("silver key")]
        [PluralName("silver keys")]
        [ItemClass(ItemClass.Key)]
        SilverKey,

        [SingularName("silver-nickel neckguard")]
        [PluralName("silver-nickel neckguards")]
        [LookText("You see an ornate protective device for your neck.")]
        SilverNickelNeckguard,

        [SingularName("silver scimitar")]
        [PluralName("silver scimitars")]
        [LookText("You see a sharp single edged blade.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(5)]
        [Sellable(866)]
        SilverScimitar,

        [SingularName("silver wand")]
        [PluralName("silver wands")]
        [Wand(SpellsEnum.hurt)]
        SilverWand,

        [SingularName("slaying sword")]
        [PluralName("slaying swords")]
        [LookText("You see a sword with a wide blade and short hilt.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(5)]
        [Sellable(562)]
        SlayingSword,

        [SingularName("sling")]
        [PluralName("slings")]
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Missile)]
        [Weight(5)]
        [Sellable(SellableEnum.Junk)]
        Sling,

        [SingularName("small ash bow")]
        [PluralName("small ash bows")]
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Missile)]
        [Weight(3)]
        [Sellable(69)]
        SmallAshBow,

        [SingularName("small bag")]
        [PluralName("small bags")]
        [LookText("Several objects can be put inside this bag.")]
        [ItemClass(ItemClass.Bag)]
        [Weight(2)]
        [Sellable(SellableEnum.Junk)]
        SmallBag,

        [SingularName("small knife")]
        [PluralName("small knifes")] //verified 6/21/23
        [LookText("You see nothing special about it.")]
        [WeaponType(WeaponType.Stab)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        SmallKnife,

        [SingularName("small metal shield")]
        [PluralName("small metal shields")]
        [LookText("You see nothing special about it.")]
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
        [LookText("You see a small chest made of silver and finely engraved.")]
        [ItemClass(ItemClass.Money)]
        SmallSilverChest,

        [SingularName("small wooden shield")]
        [PluralName("small wooden shields")]
        [LookText("You see nothing special about it.")]
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
        [LookText("You see a pair of boots, just the right size for a sprite.")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(3)]
        [ArmorClass(0.2)]
        [Sellable(198)]
        SpriteBoots,

        //doesn't fit despug
        [SingularName("sprite leather armor")] //verified collective 7/23/23
        [LookText("You see some leather armor, fashioned by the sprites for smaller races.")]
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
        [LookText("You see nothing special about it.")]
        [EquipmentType(EquipmentType.Unknown)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(15)]
        [Sellable(3093)]
        SplintMail,

        [SingularName("sprite bracelet")]
        [PluralName("sprite bracelets")]
        [EquipmentType(EquipmentType.Unknown)]
        SpriteBracelet,

        [SingularName("spruce-top guitar")]
        [PluralName("spruce-top guitars")]
        [LookText("A handsome instument for making music.")]
        [ItemClass(ItemClass.Instrument)]
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
        [LookText("The key is made completely of stone.  It is quite fragile.")]
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
        [LookText("You see a strand of small pearls that fit snugly around your throat.")]
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
        [LookText("A red and black tattoo of a hissing snake.")]
        [EquipmentType(EquipmentType.Arms)]
        [Weight(1)]
        [DisallowedClasses(ClassTypeFlags.Mage)] //verified 7/6/23
        [Sellable(1237)]
        TattooOfASnake,

        [SingularName("tattoo of a wench")]
        //CSRTODO: plural?
        [LookText("A tattoo of a saucy looking wench.")]
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
        [LookText("You see a large bone from a steak")]
        [Weight(4)]
        [Sellable(SellableEnum.Junk)]
        TBone,

        [SingularName("thieves' boots")]
        [LookText("They look rather stealthy.")]
        [EquipmentType(EquipmentType.Feet)]
        [Weight(3)]
        [ArmorClass(0.2)]
        [Sellable(193)]
        ThievesBoots,

        [SingularName("throwing axe")]
        [PluralName("throwing axes")]
        [LookText("The axe is small and easily handled.")]
        [WeaponType(WeaponType.Missile)]
        [Weight(2)]
        [Sellable(48)]
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
        [LookText("You see a roll of toilet paper with cute little animals adorning it.")]
        [Wand(SpellsEnum.stun)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        ToiletPaper,

        [SingularName("tomb key")]
        [PluralName("tomb keys")]
        [LookText("This key has some badly worn orcish runes on it.")]
        [ItemClass(ItemClass.Key)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        TombKey,

        [SingularName("topaz")]
        [PluralName("topazs")] //verified 7/10/23
        [LookText("You see a glimmering honey-colored gemstone of considerable weight.")]
        [ItemClass(ItemClass.Gem)]
        [Weight(3)]
        [Sellable(308)]
        Topaz,

        [SingularName("torch")]
        [PluralName("torchs")] //verified 6/21/23
        [LookText("You see nothing special about it.")]
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
        [LookText("You see shimmers in the air where the armor should be.")]
        [EquipmentType(EquipmentType.Torso)]
        TranslucentArmor,

        [SingularName("translucent leggings")]
        [EquipmentType(EquipmentType.Legs)]
        TranslucentLeggings,

        [SingularName("translucent sleeves")]
        [EquipmentType(EquipmentType.Arms)]
        TranslucentSleeves,

        //has a strength requirement
        [SingularName("two-handed sword")]
        [PluralName("two-handed swords")]
        [LookText("You see a large sword that requires skill to handle.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(12)]
        [Sellable(693)]
        TwoHandedSword,

        [SingularName("vanishing cream")]
        [PluralName("vanishing creams")]
        [LookText("You don't see that.")] //close to but not exactly the not here text
        [Use(SpellsEnum.invisibility)]
        [Weight(1)]
        [Sellable(36)]
        VanishingCream,

        [SingularName("verdant green scroll")]
        [PluralName("verdant green scrolls")]
        [LookText("You see nothing special about it.")]
        [Scroll(SpellsEnum.protection)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        VerdantGreenScroll,

        [SingularName("very old key")]
        [PluralName("very old keys")]
        [ItemClass(ItemClass.Key)]
        VeryOldKey,

        [SingularName("viscous potion")]
        [PluralName("viscous potions")]
        [LookText("The potion is extremely viscous.  It looks like oil.")]
        [Potion(SpellsEnum.unknown)] //CSRTODO: what is this, causes small amount of damage
        [Weight(1)]
        [Sellable(SellableEnum.NotSellable)]
        ViscousPotion,

        [SingularName("volcanic ash")]
        //CSRTODO: plural
        [LookText("You have scooped up a small handfull of volcanic ash.")]
        [Wand(SpellsEnum.stun)]
        [Weight(1)]
        [Sellable(SellableEnum.Junk)]
        VolcanicAsh,

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
        [LookText("You see a large axe or cleaver-type blade mounted on a short pole.")]
        [WeaponType(WeaponType.Slash)]
        [Weight(1)]
        [Sellable(173)]
        Voulge,

        [SingularName("wagonmaster's whip")]
        [PluralName("wagonmaster's whips")]
        [LookText("Each of the six ends on this whip are tipped with shards of wyvern bone.")]
        [WeaponType(WeaponType.Missile)]
        [Weight(3)]
        [Sellable(1262)]
        WagonmastersWhip,

        [SingularName("wand of the efreeti")]
        //CSRTODO: plural
        [LookText("A fire red wand that burns your hands as you grab it.")]
        [Weight(5)]
        WandOfTheEfreeti,

        [SingularName("warhammer")]
        [PluralName("warhammers")]
        [WeaponType(WeaponType.Unknown)]
        Warhammer,

        [SingularName("war harness")]
        [PluralName("war harnesses")]
        [LookText("A durable weapons belt.")]
        [EquipmentType(EquipmentType.Waist)]
        [Weight(3)]
        [ArmorClass(0.4)]
        [Sellable(SellableEnum.Junk)]
        WarHarness,

        [SingularName("War's flaming axe")]
        [PluralName("War's flaming axes")]
        [WeaponType(WeaponType.Unknown)]
        WarsFlamingAxe,

        [SingularName("welcome sign")]
        [PluralName("welcome signs")]
        [LookTextType(LookTextType.Multiline)]
        [ItemClass(ItemClass.Fixed)]
        WelcomeSign,

        [SingularName("white armor")]
        [PluralName("white armors")] //verified 6/29/23
        [EquipmentType(EquipmentType.Torso)]
        [DisallowedClasses(ClassTypeFlags.Mage)]
        [Weight(15)]
        [Sellable(209)]
        WhiteArmor,

        [SingularName("whoopie cushion")]
        [PluralName("whoopie cushions")]
        [LookText("A brown bladder that looks like a water bottle.")]
        [Use(SpellsEnum.hurt)]
        [Weight(2)]
        [Sellable(61)]
        WhoopieCushion,

        [SingularName("wight mask")]
        [PluralName("wight masks")]
        [LookText("The mask seems to glow with an eerie light.")]
        [Weight(2)]
        [ArmorClass(0.4)]
        [Sellable(402)]
        WightMask,

        [SingularName("yellow beholder's eye")]
        [SingularSelection("yellow beholder eye")]
        [PluralName("yellow beholder's eyes")]
        [LookText("You see a yellow eye of a beholder.")]
        [Wand(SpellsEnum.lightning)]
        YellowBeholdersEye,

        [SingularName("yellow potion")]
        [PluralName("yellow potions")]
        [LookText("You see nothing special about it.")]
        [Potion(SpellsEnum.vigor)]
        [Weight(3)]
        [Sellable(SellableEnum.NotSellable)]
        YellowPotion,

        [SingularName("yo-yo")]
        [PluralName("yo-yos")]
        [LookText("This thing looks more like a dumbell on a rope.")]
        [Use(SpellsEnum.unknown)]
        [Weight(1)]
        YoYo,
    }
}
