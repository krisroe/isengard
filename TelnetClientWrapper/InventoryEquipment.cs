using System.Collections.Generic;
namespace IsengardClient
{
    internal class InventoryEquipment
    {
        public List<ItemTypeEnum> InventoryItems { get; set; }
        public List<InventoryEquipmentChange> InventoryEquipmentChanges { get; set; }
        public object InventoryEquipmentLock { get; set; }
        public int InventoryEquipmentCounter { get; set; }
        public int InventoryEquipmentCounterUI { get; set; }
        public InventoryEquipment()
        {
            this.InventoryItems = new List<ItemTypeEnum>();
            this.InventoryEquipmentChanges = new List<InventoryEquipmentChange>();
            this.InventoryEquipmentLock = new object();
        }

        public int FindNewItemInsertionPoint(ItemTypeEnum newItem)
        {
           string sSingular = ItemEntity.ItemToSingularString[newItem];
            bool isCapitalized = char.IsUpper(sSingular[0]);
            int i = 0;
            int iFoundIndex = -1;
            foreach (ItemTypeEnum nextItem in InventoryItems)
            {
                string sNextSingular = ItemEntity.ItemToSingularString[nextItem];
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
    }

    internal class InventoryEquipmentChange
    {
        public InventoryEquipmentChangeType ChangeType { get; set; }
        /// <summary>
        /// items being changed
        /// </summary>
        public List<ItemTypeEnum> InventoryItems { get; set; }
        /// <summary>
        /// equipment items
        /// </summary>
        public List<ItemTypeEnum> EquipmentItems { get; set; }
        /// <summary>
        /// index where the object should be inserted/removed. This is -1 when inserted at the end of the list.
        /// </summary>
        public int InventoryIndex { get; set; }
        public int GlobalCounter { get; set; }
    }

    internal class ItemInList
    {
        public ItemTypeEnum ItemType { get; set; }
        public ItemInList(ItemTypeEnum itemType)
        {
            this.ItemType = itemType;
        }
        public override string ToString()
        {
            return ItemEntity.ItemToSingularString[ItemType];
        }
    }
}
