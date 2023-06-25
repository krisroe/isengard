﻿namespace IsengardClient
{
    internal class ItemInInventoryList
    {
        public ItemEntity Item { get; set; }
        public ItemInInventoryList(ItemEntity item)
        {
            this.Item = item;
        }
        public override string ToString()
        {
            return ItemEntity.StaticItemData[Item.ItemType.Value].SingularName;
        }
    }

    internal class ItemInEquipmentList
    {
        public ItemEntity Item { get; set; }
        public ItemInEquipmentList(ItemEntity item)
        {
            this.Item = item;
        }
        public override string ToString()
        {
            StaticItemData sid = ItemEntity.StaticItemData[Item.ItemType.Value];
            return sid.SingularName + "(" + sid.EquipmentType.ToString() + ")";
        }
    }
}
