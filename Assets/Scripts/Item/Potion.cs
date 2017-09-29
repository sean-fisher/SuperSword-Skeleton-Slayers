using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : BaseItem
{

    [SerializeField]
    private SimpleItemData sitemData;

    /*// Default Constructor; Sets all ints to 0 (incl. ID) and bools to false.
    public SimpleItem() : this(0, 0, 0, 0, 0, 0,
        false, false, false, false, "Default Constructor Item", "")
    {
    }

    // Constructor used for items that only change HP
    public SimpleItem(int ID, int bHP, String itemName, String desc) :
        this(ID, bHP, 0, 0, 0, 0,
        false, false, false, false, itemName, desc)
    {
    }

    // Constructor that takes parameters for all possible variables
    public SimpleItem(int ID, int bHP, int bATK, int bDEF, int bSKL, int bEVA,
        bool revives, bool hPoison, bool hParal, bool hBurn,
        String itemName, string itemDesc)
    {
        sitemData.itemID = ID;

        sitemData.boostHP = bHP;
        sitemData.boostATK = bATK;
        sitemData.boostDEF = bDEF;
        sitemData.boostSKL = bSKL;
        sitemData.boostEVA = bEVA;
        
        sitemData.revives = revives;
        sitemData.healsPoison = hPoison;
        sitemData.healsParalysis = hParal;
        sitemData.healsBurn = hBurn;
        
        sitemData.itemName = itemName;
        sitemData.itemDescription = itemDesc;
    }*/

    public override ItemData GetItemData()
    {
        return sitemData;
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

    public bool usableOutsideBattle;

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
