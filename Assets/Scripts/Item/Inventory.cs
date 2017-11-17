using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    // The key is the ItemID, the second is the number of that item.

    // Every item ID in the player's inventory along with the number of said item
    public static Dictionary<int, int> allItems = new Dictionary<int, int>(); 
    public static Dictionary<int, int> equipment = new Dictionary<int, int>();

    // Items the player has, in the order they appear in the item window
    public static ItemData[] unsortedList = new ItemData[16]; 

    int emptySpot = 0;

    public static int partyGold;

    // Equippables for each character stored here for equip menu access. 
    // The number represents a particular hero ID, not his party position
    public List<EquipData> knightEquips = new List<EquipData>();
    public List<EquipData> mageEquips = new List<EquipData>();
    public List<EquipData> monkEquips = new List<EquipData>();
    public List<EquipData> ninjaEquips = new List<EquipData>();
    public List<EquipData> chefEquips = new List<EquipData>();


    // If items are removed from the list such that there is an empty space in unsortedList, 
    // this keeps track of where the item should be added to unsortedList.
    // @return true if the inventory already had this item or was null, false if new item

    private void Start()
    {
        partyGold = 200;
    }

    public bool ContainsItem(ItemData item)
    {
        return allItems.ContainsKey(item.itemID);
    }

    public bool AddToInventory(ItemData newItem)
    {
        if (newItem == null)
        {
            return true;
        }

        bool arrContains = false;
        bool isNew = false;
        if (allItems.ContainsKey(newItem.itemID))
        {
            arrContains = true;
            isNew = true;
            allItems[newItem.itemID]++;
        } else // The player doesn't have any of this item
        {
            allItems.Add(newItem.itemID, 1);
        }

        // Adds to unsorted list
        if (!arrContains)
        {
            while (unsortedList[emptySpot] != null)
            {
                emptySpot++;
            }
            unsortedList[emptySpot] = newItem;
        }
        bool ESFound = false;
        for (int i = 0; i < unsortedList.Length && !ESFound; i++)
        {
            if (unsortedList[i] == null)
            {
                ESFound = true;
                emptySpot = i;
            }
        }

        arrContains = false;
        if (newItem is EquipData)
        {
            if (equipment.ContainsKey(newItem.itemID))
            {
                arrContains = true;
                equipment[newItem.itemID]++;
            }
            else // The player doesn't have any of this item
            {
                equipment.Add(newItem.itemID, 1);
                
                if (((EquipData)newItem).equippableBy.Count > 0)
                {
                    // If equippableBy is greater than 0, the equip will only be added to the correct characters
                    foreach (HeroClasses job in ((EquipData)newItem).equippableBy)
                    {
                        switch (job)
                        {
                            case (HeroClasses.KNIGHT):
                                knightEquips.Add((EquipData)newItem);
                                break;
                            case (HeroClasses.MAGE):
                                mageEquips.Add((EquipData)newItem);
                                break;
                            case (HeroClasses.MONK):
                                monkEquips.Add((EquipData)newItem);
                                break;
                            case (HeroClasses.NINJA):
                                ninjaEquips.Add((EquipData)newItem);
                                break;
                            case (HeroClasses.CHEF):
                                chefEquips.Add((EquipData)newItem);
                                break;
                        }
                    }
                } else
                {
                    knightEquips.Add((EquipData)newItem);
                    mageEquips.Add((EquipData)newItem);
                    monkEquips.Add((EquipData)newItem);
                    ninjaEquips.Add((EquipData)newItem);
                    chefEquips.Add((EquipData)newItem);
                }           
            }
        }
        return isNew;
    }

    /**
     * @return true if the item still remains in the player's inventory
     * */
    public bool DecrementSupply(int unsortedIndex)
    {
        ItemData decrementItem = unsortedList[unsortedIndex];

        Debug.Log("Remove: " + decrementItem.itemName);

        if (--allItems[decrementItem.itemID] < 1)
        {
            allItems.Remove(decrementItem.itemID);
            unsortedList[unsortedIndex] = null;
            if (unsortedIndex < emptySpot)
            {
                emptySpot = unsortedIndex;
            }

            if (decrementItem is EquipData)
            {
                EquipData eq = (EquipData)decrementItem;

                if (knightEquips.Contains((EquipData)decrementItem))
                {
                    knightEquips.Remove((EquipData)decrementItem);
                }
                if (mageEquips.Contains(eq))
                {
                    mageEquips.Remove(eq);
                }
                if (monkEquips.Contains(eq))
                {
                    monkEquips.Remove(eq);
                }
                if (ninjaEquips.Contains(eq))
                {
                    ninjaEquips.Remove(eq);
                }
                if (chefEquips.Contains(eq))
                {
                    chefEquips.Remove(eq);
                }
            }

            return false;
        }
        else return true;
    }

    /**
     * returns 0 if there are none left,
     *      1 if there are some left, 
     *      2 if it is null
     *      3 if it never contained any
     * */
    public int DecrementSupply(ItemData decrementItem)
    {
        Debug.Log("Remove: " + decrementItem.itemName);
        if (decrementItem == null) {
            return 2;
        }
        if (!allItems.ContainsKey(decrementItem.itemID))
        {
            return 3;
        }
        
        if (--allItems[decrementItem.itemID] < 1)
        {
            // no more of this item
            allItems.Remove(decrementItem.itemID);
            int i = 0;
            bool itemFound = false;
            for (; !itemFound && i < unsortedList.Length; i++)
            {
                if (unsortedList[i] == decrementItem)
                {
                    unsortedList[i] = null;
                    itemFound = true;
                }
            }
            if (i < emptySpot)
            {
                emptySpot = i;
            }
            if (decrementItem is EquipData)
            {
                EquipData eq = (EquipData)decrementItem;

                if (knightEquips.Contains(eq))
                {
                    knightEquips.Remove(eq);
                }
                if (knightEquips.Contains(eq))
                {
                    knightEquips.Remove(eq);
                }
                if (knightEquips.Contains(eq))
                {
                    knightEquips.Remove(eq);
                }
                if (knightEquips.Contains(eq))
                {
                    knightEquips.Remove(eq);
                }
                if (knightEquips.Contains(eq))
                {
                    knightEquips.Remove(eq);
                }
            }
            return 0;
        }
        else return 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            //AddToInventory(new SimpleItem.Medicine());
            partyGold += 600;
        }
    }

    public class ItemNumPair
    {
        public BaseItem item;
        public int num;

        public ItemNumPair(BaseItem item, int num)
        {
            this.item = item;
            this.num = num;
        }
    }

    public static bool SpendGold(int goldSpent)
    {
        partyGold -= goldSpent;

        if (partyGold < 0)
        {
            partyGold = 0;
            return false;
        }
        return true;
    }


    public static bool GainGold(int goldSpent)
    {
        if (goldSpent < 0)
        {
            return false;
        }
        else
        {
            partyGold += goldSpent;

            return true;
        }
    }

    public static List<ItemData> equipList;

    public static List<ItemData> GetEquipList()
    {
        List<ItemData> equips = new List<ItemData>();

        for (int i = 0; i < unsortedList.Length; i++)
        {
            if (unsortedList[i] != null && unsortedList[i].itemType == ItemTypes.EQUIPMENT)
            {
                equips.Add(unsortedList[i]);
            }
        }
        equipList = equips;
        return equips;
    }
}
[System.Serializable]
public class InventorySaveData
{
    public List<int> allItemsIDs;
    public List<int> allItemsAmounts;

    public List<int> equipmentIDs;
    public List<int> equipmentAmounts;

    public ItemData[] unsortedList = new ItemData[150];

    public void SaveMembers()
    {
        allItemsIDs = new List<int>(Inventory.allItems.Keys);

        allItemsAmounts = new List<int>(Inventory.allItems.Values);

        equipmentIDs = new List<int>(Inventory.equipment.Values);
        equipmentAmounts = new List<int>(Inventory.equipment.Keys);

        unsortedList = Inventory.unsortedList;
    }

    public void LoadMembers()
    {
        // Convert int lists to allItems dictionary
        Inventory.allItems = new Dictionary<int, int>();
        int i = 0;
        foreach (int id in allItemsIDs)
        {
            Inventory.allItems.Add(id, allItemsAmounts[i++]);
        }
        // Convert int lists to equipment dictionary
        Inventory.equipment = new Dictionary<int, int>();
        i = 0;
        foreach (int id in equipmentIDs)
        {
            Inventory.equipment.Add(id, equipmentAmounts[i++]);
        }

        Inventory.unsortedList = unsortedList;
    }
}
