using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMenu : GridOptions {

    public Text heroNameText;
    public Text armorText;
    public Text weaponText;
    public Text accessoryText;


    // Update is called once per frame
    void Update () {
		if (canControl)
        {
            CheckInput<RectTransform>(listTexts, 2, 8);

            if (waitFrame && bPressed)
            {
                CloseMenu();
                PauseMenu.pauseMenu.OpenMenu();
            } else if (!waitFrame)
            {
                waitFrame = true;
            }
        }
	}

    bool waitFrame = false;
    
    InventoryEntry[] inventoryEntries;
    List<ItemData> equips;

    public override void OpenMenu()
    {
        this.gameObject.SetActive(true);

        cursor = GameManager.gm.cursor;
        currCursor = cursor;

        inventoryEntries = transform.GetChild(0).GetComponentsInChildren<InventoryEntry>();

        List<RectTransform> tempList = new List<RectTransform>();
        for (int i = 0; i < inventoryEntries.Length; i++)
        {
            tempList.Add(inventoryEntries[i].itemName.rectTransform);
        }
        listTexts = tempList.ToArray();

        visibleSize = 16;

        UpdateCursor(listTexts, 0);

        equips = Inventory.GetEquipList();
        InitializeListText<ItemData>(0, equips);
        UpdateItemCounts(true);

        canControl = true;
        UpdateHeroEquipsWindow(0);
    }

    public void UpdateHeroEquipsWindow(int heroIndex)
    {
        BaseCharacter currHero = BattleManager.hpm.activePartyMembers[heroIndex];

        heroNameText.text = currHero.characterName;

        if (currHero.armor != null) {
            armorText.text = currHero.armor.itemName;
        } else
        {
            armorText.text = emptyText;
        }
        if (currHero.weapon != null)
        {
            weaponText.text = currHero.weapon.itemName;
        }
        else
        {
            weaponText.text = emptyText;
        }
        if (currHero.accessory != null)
        {
            accessoryText.text = currHero.accessory.itemName;
        }
        else
        {
            accessoryText.text = emptyText;
        }
    }

    protected override void MakeMenuSelection(int menuIndex)
    {
        if (menuIndex < Inventory.equipList.Count && Inventory.equipList[menuIndex] != null)
        {
            PauseMenu.partySelectMenu.openedFromEquipMenu = true;
            PauseMenu.partySelectMenu.OpenMenu();
            PauseMenu.partySelectMenu.itemToUseUnsortedIndex = menuIndex;
            DisableMenuControl();
        } else
        {
            // No item there
            Debug.Log("No Item there!");
        }
    }

    public override void CloseMenu()
    {
        gameObject.SetActive(false);
        canControl = false;
        waitFrame = false;
    }

    public void UpdateItemCounts(bool checkAllItems, int singleItemIndex = 0)
    {
        if (checkAllItems)
        {
            for (int i = 0; i < 14; i++)
            {
                if (i < equips.Count)
                {
                    ItemData id = equips[i];
                    try
                    {
                        int itemStock = Inventory.allItems[id.itemID];
                        if (itemStock > 0)
                        {
                            InventoryEntry ie = inventoryEntries[i];

                            //ie.itemName.text = ";
                            ie.itemNum.text = "" + itemStock;
                        }
                        else
                        {
                            InventoryEntry ie = inventoryEntries[i];
                            ie.itemNum.text = "";
                            ie.itemName.text = "";
                            //listTexts[i].GetComponent<Text>().text += "";
                        }
                    } catch (KeyNotFoundException e)
                    {

                    }
                }
                else
                {
                    InventoryEntry ie = inventoryEntries[i];
                    ie.itemNum.text = "";
                    ie.itemName.text = "";
                }
            }
        }
        else 
        {
            Debug.Log("UPdate entry " + singleItemIndex);
            ItemData id = equips[singleItemIndex];

            if (id != null)
            {
                try { 
                int itemStock = Inventory.allItems[id.itemID];
                if (itemStock > 0)
                {
                    InventoryEntry ie = inventoryEntries[singleItemIndex];
                    ie.itemNum.text = "" + itemStock;
                }
                else
                {
                    InventoryEntry ie = inventoryEntries[singleItemIndex];
                    ie.itemNum.text = "";
                    ie.itemName.text = "";
                }
                } catch(KeyNotFoundException e)
                {
                    e = null;
                    Debug.Log("Key not found");
                }
            }
        }
    }
}
