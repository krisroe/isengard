namespace IsengardClient
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
            return this.Item.GetItemString();
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
            string s = sid.SingularName + "(" + sid.EquipmentType.ToString() + ")";
            if (sid.ArmorClass > 0) s += sid.ArmorClass.ToString("N1");
            return s;
        }
    }
}
