using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : BaseItem {

    public EquipData equipData;

    


    


}
[System.Serializable]
public enum EquipType
{
    HEADGEAR,
    ARMOR,
    ACCESSORY,
    WEAPON
}
[System.Serializable]
public abstract class EquipData : ItemData
{
    public int boostHP;
    public int boostATK;
    public int boostDEF;
    public int boostSKL;
    public int boostEVA;

    public EquipType equipType;

    public List<HeroClasses> equippableBy;

    // Decreases Damage done by these
    public List<AttackType> strongAgainst;
    // Completely blocks attacks of these types
    public List<AttackType> blockTypes;

    public override int GetValue()
    {
        int value = 0;
        value += boostATK;
        value += boostDEF;
        value += boostHP;
        value += boostSKL;
        value += boostEVA;
        return value;
    }
    public abstract override string Effect(params BaseCharacter[] targets);

    public override int CompareTo(ItemData other)
    {
        ItemTypes otherType = other.itemType;
        if (otherType == ItemTypes.EQUIPMENT)
        {
            return this.GetValue() - other.GetValue();
        }
        else return 1;
    }
}
