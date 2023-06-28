using System;
namespace IsengardClient
{
    /// <summary>
    /// base attribute for names
    /// </summary>
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// singular name for items/mobs
    /// </summary>
    public class SingularNameAttribute : NameAttribute
    {
        public SingularNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// singular selection name where the singular name has components that aren't used for selection
    /// </summary>
    public class SingularSelectionAttribute : NameAttribute
    {
        public SingularSelectionAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// plural name for items/mobs
    /// </summary>
    public class PluralNameAttribute : NameAttribute
    {
        public PluralNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// experience gained when killing a mob
    /// </summary>
    public class ExperienceAttribute : Attribute
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
}
