using System;
namespace IsengardClient.Backend
{
    /// <summary>
    /// base attribute for names
    /// </summary>
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public class MapTypeDisplayNameAttribute : NameAttribute
    {
        public MapTypeDisplayNameAttribute(string Name)
        {
            this.Name = Name;
        }
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

    #region mob attributes

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
    public class AlignmentAttribute : Attribute
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
    public class AggressiveAttribute : Attribute
    {
        public AggressiveAttribute()
        {
        }
    }

    public class DestroysItemsAttribute : Attribute
    {
        public DestroysItemsAttribute()
        {
        }
    }

    public class InfectsWithDiseaseAttribute : Attribute
    {
        public InfectsWithDiseaseAttribute()
        {
        }
    }

    public class CannotHarmAttribute : Attribute
    {
        public CannotHarmAttribute()
        {
        }
    }

    public class MobVisibilityAttribute : Attribute
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
    public class EquipmentTypeAttribute : Attribute
    {
        public EquipmentType EquipmentType { get; set; }
        public EquipmentTypeAttribute(EquipmentType EquipmentType)
        {
            this.EquipmentType = EquipmentType;
        }
    }

    public class ArmorClassAttribute : Attribute
    {
        public decimal ArmorClass { get; set; }
        public ArmorClassAttribute(double ArmorClass)
        {
            this.ArmorClass = Convert.ToDecimal(ArmorClass);
        }
    }

    public class ItemClassAttribute : Attribute
    {
        public ItemClass ItemClass;
        public ItemClassAttribute(ItemClass ItemClass)
        {
            if (ItemClass == ItemClass.Money ||
                ItemClass == ItemClass.Coins ||
                ItemClass == ItemClass.Bag ||
                ItemClass == ItemClass.Key ||
                ItemClass == ItemClass.Fixed ||
                ItemClass == ItemClass.Chest ||
                ItemClass == ItemClass.Gem ||
                ItemClass == ItemClass.Instrument ||
                ItemClass == ItemClass.HeldItem)
            {
                this.ItemClass = ItemClass;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }

    /// <summary>
    /// weapon type for an item
    /// </summary>
    public class WeaponTypeAttribute : Attribute
    {
        public WeaponType WeaponType { get; set; }
        public WeaponTypeAttribute(WeaponType WeaponType)
        {
            this.WeaponType = WeaponType;
        }
    }

    public class PotionAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public PotionAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    public class ScrollAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public ScrollAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    public class WandAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public WandAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    public class UseAttribute : Attribute
    {
        public SpellsEnum Spell { get; set; }
        public UseAttribute(SpellsEnum Spell)
        {
            this.Spell = Spell;
        }
    }

    #endregion

    public class SpellInformationAttribute : Attribute
    {
        public SpellsEnum SpellType { get; set; }
        public WorkflowSpells? WorkflowSpellType { get; set; }
        public SpellProficiency Proficiency { get; set; }
        public int Mana { get; set; }
        public int Tier { get; set; }
        public string SpellName { get; set; }

        public SpellInformationAttribute(SpellProficiency Proficiency, int Mana, int Tier)
        {
            this.Proficiency = Proficiency;
            this.Mana = Mana;
            this.Tier = Tier;
        }

        public SpellInformationAttribute(SpellProficiency Proficiency, int Mana, int Tier, string SpellName) : this(Proficiency, Mana, Tier)
        {
            this.SpellName = SpellName;
        }

        public int GetMinimumProficiencyForTier()
        {
            int ret;
            switch (Tier)
            {
                case 1:
                    ret = 5;
                    break;
                case 2:
                    ret = 15;
                    break;
                case 3:
                    ret = 35;
                    break;
                case 4:
                    ret = 50;
                    break;
                case 5:
                    ret = 70;
                    break;
                case 6:
                    ret = 85;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }
    }

    public class WeightAttribute : Attribute
    {
        public int Pounds { get; set; }
        public WeightAttribute(int Pounds)
        {
            this.Pounds = Pounds;
        }
    }

    public class SellableAttribute : Attribute
    {
        public int Gold { get; set; }
        public SellableEnum Sellable { get; set; }
        public SellableAttribute(int Gold)
        {
            Sellable = SellableEnum.Sellable;
            this.Gold = Gold;
        }
        public SellableAttribute(SellableEnum Sellable)
        {
            this.Sellable = Sellable;
        }
    }

    public class StrategyStepAttribute : Attribute
    {
        public char Letter { get; set; }
        public bool IsCombat { get; set; }
        public StrategyStepAttribute(char Letter, bool IsCombat)
        {
            this.Letter = Letter;
            this.IsCombat = IsCombat;
        }
    }

    public class DisallowedClassesAttribute : Attribute
    {
        public ClassTypeFlags Classes { get; set; }
        public DisallowedClassesAttribute(ClassTypeFlags Classes)
        {
            this.Classes = Classes;
        }
    }

    public class LookTextTypeAttribute : Attribute
    {
        public LookTextType LookTextType { get; set; }
        public LookTextTypeAttribute(LookTextType LookTextType)
        {
            this.LookTextType = LookTextType;
        }
    }

    public class LookTextAttribute : Attribute
    {
        public string LookText { get; set; }
        public LookTextAttribute(string LookText)
        {
            this.LookText = LookText;
        }
    }

    public class SexRestrictionAttribute : Attribute
    {
        public SexEnum Sex { get; set; }
        public SexRestrictionAttribute(SexEnum Sex)
        {
            this.Sex = Sex;
        }
    }
}
