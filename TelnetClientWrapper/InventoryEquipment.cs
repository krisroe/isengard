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
