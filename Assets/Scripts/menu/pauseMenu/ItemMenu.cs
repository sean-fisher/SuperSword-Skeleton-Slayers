using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : GridOptions {


	// Use this for initialization
	void Start () {
        PauseMenu.itemMenu = this;
	}
	
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

    public override void OpenMenu()
    {
        Debug.Log("openn item menu");
        this.gameObject.SetActive(true);

        cursor = GameManager.gm.cursor;
        currCursor = cursor;

        if (inventoryEntries == null)
        {
            inventoryEntries = transform.GetChild(0).GetComponentsInChildren<InventoryEntry>();
        }
        List<RectTransform> tempList = new List<RectTransform>();
        for (int i = 0; i < inventoryEntries.Length; i++)
        {
            tempList.Add(inventoryEntries[i].GetComponent<RectTransform>());
        }
        listTexts = tempList.ToArray();

        visibleSize = 16;

        UpdateCursor(listTexts, 0);

        InitializeListText<ItemData>(0, Inventory.unsortedList);
        UpdateItemCounts(true);

        canControl = true;  
    }

    protected override void MakeMenuSelection(int menuIndex)
    {
        if (menuIndex < Inventory.unsortedList.Length && Inventory.unsortedList[menuIndex] != null && Inventory.unsortedList[menuIndex].usableOutsideBattle)
        {
            PauseMenu.partySelectMenu.openedFromItemMenu = true;
            PauseMenu.partySelectMenu.OpenMenu();
            PauseMenu.partySelectMenu.itemToUseUnsortedIndex = menuIndex;
            DisableMenuControl();
            //PauseMenu.pauseMenu.UpdateHeroDisplay();
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
            for (int i = 0; i < Inventory.unsortedList.Length; i++)
            {
                ItemData id = Inventory.unsortedList[i];
                if (id != null)
                {
                    int itemStock = Inventory.allItems[id.itemID];
                    if (itemStock > 0)
                    {
                        InventoryEntry ie = inventoryEntries[i];

                        ie.itemNum.text = "" + itemStock;
                    }
                    else
                    {
                        InventoryEntry ie = inventoryEntries[i];
                        ie.itemNum.text = "";
                        ie.itemName.text = "";
                        //listTexts[i].GetComponent<Text>().text += "";
                    }
                } else
                {
                    InventoryEntry ie = inventoryEntries[i];
                    ie.itemNum.text = "";
                    ie.itemName.text = "";
                }
            }
        } else
        {
            ItemData id = Inventory.unsortedList[singleItemIndex];
            if (id != null)
            {
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
            }
        }
    }
}
