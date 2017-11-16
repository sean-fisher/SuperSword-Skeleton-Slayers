using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEquip : BaseItem
{

    [SerializeField]
    private SimpleEquipData equipData;
    

    public override ItemData GetItemData()
    {
        return equipData;
    }
    
}

[System.Serializable]
public class SimpleEquipData : EquipData
{

    public bool revives;
    public bool healsPoison;
    public bool healsParalysis;
    public bool healsBurn;
    

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


    public override string Effect(params BaseCharacter[] targets)
    {
        return "Equipment used";
    }
    public override int CompareTo(ItemData other)
    {
        ItemTypes otherType = other.itemType;
        if (otherType == ItemTypes.ITEM)
        {
            return this.GetValue() - other.GetValue();
        }
        else if (otherType == ItemTypes.WEAPON)
        {
            return 1;
        }
        else if (otherType == ItemTypes.EQUIPMENT)
        {
            return 1;
        }
        return 1;
    }
}
