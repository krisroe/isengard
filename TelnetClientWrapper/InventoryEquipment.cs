using System;
using System.Collections.Generic;
namespace IsengardClient
{
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
