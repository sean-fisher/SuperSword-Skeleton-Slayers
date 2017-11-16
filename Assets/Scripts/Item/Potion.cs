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
    public int boostMP;

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


    public override string Effect(params BaseCharacter[] targets)
    {
        // For every target character, their stats are boosted
        for (int i = 0; i < targets.Length; i++)
        {
            BaseCharacter bc = targets[i];
            BoostHP(boostHP, bc);
            BoostATK(boostATK, bc);
            BoostDEF(boostDEF, bc);
            BoostSKL(boostSKL, bc);
            BoostMP(boostMP, bc);
        }
        if (boostMP > 0)
        {
            return "{0}'s MP was restored by 100!";
        } else
        {
            return "{0}'s HP was restored by 100!";
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
