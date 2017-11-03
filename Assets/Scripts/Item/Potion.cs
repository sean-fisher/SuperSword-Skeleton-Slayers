using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : BaseItem
{

    [SerializeField]
    private SimpleItemData itemData;
    

    public override ItemData GetItemData()
    {
        return itemData;
    }
    
}

[System.Serializable]
public class SimpleItemData : ItemData
{

    public int boostHP;
    public int boostATK;
    public int boostDEF;
    public int boostSKL;
    public int boostEVA;

    public bool revives;
    public bool healsPoison;
    public bool healsParalysis;
    public bool healsBurn;


    // Format: {0} == user's name, {1} == target's name, {2} == item name, {3} == hp restored number
    public string messageWhenUsedInBattle;

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


    public override void Effect(params BaseCharacter[] targets)
    {
        // For every target character, their stats are boosted
        for (int i = 0; i < targets.Length; i++)
        {
            BaseCharacter bc = targets[i];
            BoostHP(boostHP, bc);
            BoostATK(boostATK, bc);
            BoostDEF(boostDEF, bc);
            BoostSKL(boostSKL, bc);
            BoostEVA(boostEVA, bc);
        }
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
