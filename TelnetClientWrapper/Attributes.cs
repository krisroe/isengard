using System;
namespace IsengardClient
{
    /// <summary>
    /// base attribute for names
    /// </summary>
    internal class NameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    internal class MapTypeDisplayNameAttribute : NameAttribute
    {
        public MapTypeDisplayNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// singular name for items/mobs
    /// </summary>
    internal class SingularNameAttribute : NameAttribute
    {
        public SingularNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// singular selection name where the singular name has components that aren't used for selection
    /// </summary>
    internal class SingularSelectionAttribute : NameAttribute
    {
        public SingularSelectionAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// plural name for items/mobs
    /// </summary>
    internal class PluralNameAttribute : NameAttribute
    {
        public PluralNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    #region mob attributes

    /// <summary>
    /// experience gained when killing a mob
    /// </summary>
    internal class ExperienceAttribute : Attribute
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

    internal class MobVisibilityAttribute : Attribute
    {
        public MobVisibility MobVisibility { get; set; }
        public MobVisibilityAttribute(MobVisibility MobVisibility)
        {
            this.MobVisibility = MobVisibility;
        }
    }

    #endregion

    #region item attributes

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

    internal class MoneyAttribute : Attribute
    {
    }
    internal class CoinsAttribute : Attribute
    {
    }
    internal class BagAttribute : Attribute
    {
    }
    internal class KeyAttribute : Attribute
    {
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

    internal class PotionAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public PotionAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    internal class ScrollAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public ScrollAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    internal class WandAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public WandAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    internal class UseAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public UseAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    #endregion

    internal class SpellInformationAttribute : Attribute
    {
        public SpellProficiency Proficiency { get; set; }
        public int Tier { get; set; }
        public string SpellName { get; set; }

        public SpellInformationAttribute(SpellProficiency Proficiency, int Tier)
        {
            this.Proficiency = Proficiency;
            this.Tier = Tier;
        }

        public SpellInformationAttribute(SpellProficiency Proficiency, int Tier, string SpellName) : this(Proficiency, Tier)
        {
            this.SpellName = SpellName;
        }
    }

    internal class WeightAttribute : Attribute
    {
        public int Pounds { get; set; }
        public WeightAttribute(int Pounds)
        {
            this.Pounds = Pounds;
        }
    }

    internal class SellGoldRangeAttribute : Attribute
    {
        public int LowerRange { get; set; }
        public int UpperRange { get; set; }
        public SellGoldRangeAttribute(int SingleValue) : this(SingleValue, SingleValue)
        {
        }
        public SellGoldRangeAttribute(int LowerRange, int UpperRange)
        {
            this.LowerRange = LowerRange;
            this.UpperRange = UpperRange;
        }
    }

    internal class JunkAttribute : Attribute
    {
    }
}
