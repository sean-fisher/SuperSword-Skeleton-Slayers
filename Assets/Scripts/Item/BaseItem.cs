using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseItem : MonoBehaviour {
    
    public abstract ItemData GetItemData();
}

[System.Serializable]
public enum ItemTypes
{
    EQUIPMENT,
    WEAPON,
    ITEM
}

[System.Serializable]
public abstract class ItemData
{
    public string itemName;
    public string itemDescription;
    public int itemID;
    public int itemCost;
    public bool usableOutsideBattle;
    public bool usableInsideBattle;

    public ItemTypes itemType;

    public abstract int GetValue();

    public abstract string Effect(params BaseCharacter[] targets);

    public abstract int CompareTo(ItemData other);
    public void BoostHP(int increaseVal, BaseCharacter target)
    {
        if (increaseVal > 0)
        {
            // TODO: Visual effects related to Boosts
            target.currentHP += increaseVal;
            if (target.currentHP > target.baseHP)
            {
                target.currentHP = target.baseHP;
            }
        }
    }

    public void BoostATK(int increaseVal, BaseCharacter target)
    {
        if (increaseVal > 0)
        {
            // TODO: Visual effects related to Boosts
            target.currentATK += increaseVal;
        }
    }

    public void BoostDEF(int increaseVal, BaseCharacter target)
    {
        if (increaseVal > 0)
        {
            // TODO: Visual effects related to Boosts
            target.currentDEF += increaseVal;
        }
    }

    public void BoostSKL(int increaseVal, BaseCharacter target)
    {
        if (increaseVal > 0)
        {
            // TODO: Visual effects related to Boosts
            target.currentSKL += increaseVal;
        }
    }

    public void BoostEVA(int increaseVal, BaseCharacter target)
    {
        if (increaseVal > 0)
        {
            // TODO: Visual effects related to Boosts
            target.currentEVA += increaseVal;
        }
    }

    public void BoostMP(int increaseVal, BaseCharacter target)
    {
        if (increaseVal > 0)
        {
            // TODO: Visual effects related to Boosts
            target.currentMP += increaseVal;
        }
    }
}
