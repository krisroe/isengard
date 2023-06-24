using System;
using System.Collections.Generic;
namespace IsengardClient
{
    public class InventoryEquipment
    {
        public List<ItemTypeEnum> InventoryItems { get; set; }
        public List<InventoryEquipmentChange> InventoryEquipmentChanges { get; set; }
        public int InventoryEquipmentCounter { get; set; }
        public int InventoryEquipmentCounterUI { get; set; }
        public ItemTypeEnum?[] Equipment { get; set; }

        public InventoryEquipment()
        {
            this.InventoryItems = new List<ItemTypeEnum>();
            this.InventoryEquipmentChanges = new List<InventoryEquipmentChange>();
            this.Equipment = new ItemTypeEnum?[(int)EquipmentSlot.Count];
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

        public int FindNewItemInsertionPoint(ItemTypeEnum newItem)
        {
            string sSingular = ItemEntity.StaticItemData[newItem].SingularName;
            bool isCapitalized = char.IsUpper(sSingular[0]);
            int i = 0;
            int iFoundIndex = -1;
            foreach (ItemTypeEnum nextItem in InventoryItems)
            {
                string sNextSingular = ItemEntity.StaticItemData[nextItem].SingularName;
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
        /// pick selection text for an inventory/equipment item, assumes the entity lock is present
        /// </summary>
        /// <param name="isInventory">true for inventory, false for equipment</param>
        /// <param name="itemType">item type</param>
        /// <param name="itemCounter">item counter of that type</param>
        /// <returns></returns>
        public string PickItemTextFromItemCounter(bool isInventory, ItemTypeEnum itemType, int itemCounter)
        {
            int iActualIndex = -1;
            int iCounter = 0;
            if (isInventory)
            {
                for (int i = 0; i < InventoryItems.Count; i++)
                {
                    if (InventoryItems[i] == itemType)
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
                for (int i = 0; i < Equipment.Length; i++)
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
            return iActualIndex < 0 ? null : PickItemTextFromActualIndex(isInventory, itemType, iActualIndex);
        }

        public string PickItemTextFromActualIndex(bool isInventory, ItemTypeEnum itemType, int iActualIndex)
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
                    if (isInventory)
                    {
                        eItem = InventoryItems[i];
                    }
                    else
                    {
                        eItem = Equipment[i];
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

                //for equipment, check for a duplicate in inventory
                bool isDuplicate = false;
                if (!isInventory)
                {
                    int iInventoryCounter = 0;
                    foreach (ItemTypeEnum nextItem in InventoryItems)
                    {
                        sSingular = ItemEntity.StaticItemData[nextItem].SingularName;
                        foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                        {
                            if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                            {
                                iInventoryCounter++;
                                if (iInventoryCounter == iCounter)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    isDuplicate = iInventoryCounter == iCounter;
                }

                if (!isDuplicate)
                {
                    if (iCounter == 1)
                    {
                        ret = word;
                    }
                    else
                    {
                        ret = word + " " + iCounter;
                    }
                    break;
                }
            }
            return ret;
        }
    }

    public class InventoryEquipmentEntry
    {
        public ItemTypeEnum ItemType { get; set; }
        public int InventoryIndex { get; set; }
        /// <summary>
        /// true to add to inventory, false to remove from inventory, null for no action
        /// </summary>
        public bool? InventoryAction { get; set; }
        public int EquipmentIndex { get; set; }
        /// <summary>
        /// true to add to equipment, false to remove from equipment, null for no action
        /// </summary>
        public bool? EquipmentAction { get; set; }
    }

    public class InventoryEquipmentChange
    {
        public InventoryEquipmentChangeType ChangeType { get; set; }
        /// <summary>
        /// items being changed
        /// </summary>
        public List<InventoryEquipmentEntry> Changes { get; set; }
        public int GlobalCounter { get; set; }
        public InventoryEquipmentChange()
        {
            this.Changes = new List<InventoryEquipmentEntry>();
        }

        public bool AddOrRemoveInventoryItem(InventoryEquipment inventoryEquipment, ItemTypeEnum nextItem, bool isAdd, InventoryEquipmentEntry changeInfo)
        {
            int foundIndex = inventoryEquipment.InventoryItems.LastIndexOf(nextItem);
            int changeIndex;
            bool effectChange = false;
            if (isAdd)
            {
                changeIndex = inventoryEquipment.FindNewItemInsertionPoint(nextItem);
                effectChange = true;
                if (changeIndex == -1)
                {
                    inventoryEquipment.InventoryItems.Add(nextItem);
                }
                else
                {
                    inventoryEquipment.InventoryItems.Insert(changeIndex, nextItem);
                }
            }
            else
            {
                changeIndex = foundIndex;
                if (foundIndex >= 0)
                {
                    inventoryEquipment.InventoryItems.RemoveAt(foundIndex);
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

    internal class ItemInInventoryList
    {
        public ItemTypeEnum ItemType { get; set; }
        public ItemInInventoryList(ItemTypeEnum itemType)
        {
            this.ItemType = itemType;
        }
        public override string ToString()
        {
            return ItemEntity.StaticItemData[ItemType].SingularName;
        }
    }

    internal class ItemInEquipmentList
    {
        public ItemTypeEnum ItemType { get; set; }
        public ItemInEquipmentList(ItemTypeEnum itemType)
        {
            this.ItemType = itemType;
        }
        public override string ToString()
        {
            StaticItemData sid = ItemEntity.StaticItemData[ItemType];
            return sid.SingularName + "(" + sid.EquipmentType.ToString() + ")";
        }
    }
}
