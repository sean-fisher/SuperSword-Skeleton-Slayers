using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySelectMenu : GridOptions {


	// Use this for initialization
	void Start ()
    {
        PauseMenu.partySelectMenu = this;
        if (cursor == null)
        {
            cursor = GameManager.gm.cursor;
            currCursor = cursor;
        }

        if (cursor2 == null)
        {
            cursor2 = GameObject.Instantiate(cursor, cursor.transform.parent);
        }
        if (cursor3 == null)
        {
            cursor3 = GameObject.Instantiate(cursor, cursor.transform.parent);
        }
    }

    public bool openedFromItemMenu = false;
    public bool openedFromEquipMenu = false;
    public int itemToUseUnsortedIndex;
	
	// Update is called once per frame
	void Update ()
    {
        if (canControl)
        {
            CheckInput<RectTransform>(listTexts, 1, listTexts.Length, null, true, 0, false/*, -Screen.width / 30*/);

            if (waitFrame && !cursorMoved && Input.GetButtonDown("BButton"))
            {
                CloseMenu();
            }

            if (!waitFrame)
            {
                waitFrame = true;
            }
        }
    }

    bool waitFrame = false;

    protected override void MakeMenuSelection(int menuIndex)
    {
        if (openedFromItemMenu)
        {
            ItemData itemToUse = Inventory.unsortedList[itemToUseUnsortedIndex];

            if (itemToUse.itemType == ItemTypes.ITEM && itemToUse.usableOutsideBattle)
            {
                BaseCharacter target = BattleManager.hpm.activePartyMembers[menuIndex];
                if (!target.isDead) // TODO Check revive
                {
                    itemToUse.Effect(target);
                    bool hasItemStock = GameManager.gm.inventory.DecrementSupply(itemToUseUnsortedIndex);

                    if (!hasItemStock)
                    {
                        CloseMenu();
                        PauseMenu.itemMenu.OpenMenu();
                    }
                    PauseMenu.itemMenu.UpdateItemCounts(false, itemToUseUnsortedIndex);
                    PauseMenu.pauseMenu.UpdateHeroDisplay();
                }
            } else
            {
                Debug.Log("Can't use this item!");
            }
        } else if (openedFromEquipMenu)
        {

            ItemData itemToUse = Inventory.equipList[itemToUseUnsortedIndex];
            if (itemToUse.itemType == ItemTypes.EQUIPMENT)
            {
                EquipData newEquip = (EquipData)itemToUse;
                EquipData oldEquip;
                
                switch (newEquip.equipType)
                {
                    case (EquipType.ARMOR):
                        // Remove old armor
                        oldEquip = BattleManager.hpm.activePartyMembers[menuIndex].armor;
                        
                        // add old armor back into inventory
                        if (oldEquip != null)
                        {
                            GameManager.gm.inventory.AddToInventory(oldEquip);
                        }

                        // Equip new armor
                        BattleManager.hpm.activePartyMembers[menuIndex].armor
                            = (EquipData)itemToUse;

                        break;
                    case (EquipType.WEAPON):
                        // Remove old weapon
                        oldEquip = BattleManager.hpm.activePartyMembers[menuIndex].weapon;
                        
                        // add old weapon back into inventory
                        if (oldEquip != null)
                        {
                            GameManager.gm.inventory.AddToInventory(oldEquip);
                        }

                        // Equip new weapon
                        BattleManager.hpm.activePartyMembers[menuIndex].weapon
                            = (EquipData)itemToUse;

                        break;
                    case (EquipType.HEADGEAR):
                        // Remove old headgear
                        oldEquip = BattleManager.hpm.activePartyMembers[menuIndex].headgear;
                        
                        // add old headgear back into inventory
                        if (oldEquip != null)
                        {
                            GameManager.gm.inventory.AddToInventory(oldEquip);
                        }

                        // Equip new headgear
                        BattleManager.hpm.activePartyMembers[menuIndex].headgear
                            = (EquipData)itemToUse;

                        break;
                    case (EquipType.ACCESSORY):
                        // Remove old accessory
                        oldEquip = BattleManager.hpm.activePartyMembers[menuIndex].accessory;
                        
                        // add old accessory back into inventory
                        if (oldEquip != null)
                        {
                            GameManager.gm.inventory.AddToInventory(oldEquip);
                        }

                        // Equip new accessory
                        BattleManager.hpm.activePartyMembers[menuIndex].accessory
                            = (EquipData)itemToUse;

                        break;
                }

                // Subtract from inventory
                int hasItemStock = GameManager.gm.inventory.DecrementSupply(itemToUse);

                if (hasItemStock == 0)
                {
                    // no more of this item
                }
                PauseMenu.pauseMenu.equipMenu
                    .UpdateItemCounts(true);

                CloseMenu();
                    PauseMenu.pauseMenu.equipMenu.OpenMenu();
            }
            else
            {
                Debug.Log("Can't use this item!");
            }
        }
    }

    protected override void OnMoveCursor()
    {
        if (openedFromEquipMenu)
        {
            PauseMenu.pauseMenu.equipMenu.UpdateHeroEquipsWindow(tempCursor);
        }
    }

    public override void OpenMenu()
    {
        HeroDisplayPanel[] hdps = transform.GetComponentsInChildren<HeroDisplayPanel>();

        listTexts = new RectTransform[hdps.Length];
        for (int i = 0; i < hdps.Length; i++)
        {
            listTexts[i] = hdps[i].GetComponent<RectTransform>();
        }

        waitFrame = false;
        canControl = true;

        rows = hdps.Length;
        visibleSize = rows;

        UpdateCursor(listTexts, 0);
     }

    public override void CloseMenu()
    {
        canControl = false;
        if (openedFromItemMenu)
        {
            PauseMenu.pauseMenu.itemWindow.GetComponent<ItemMenu>().OpenMenu();
            //PauseMenu.itemMenu.OpenMenu();
        }
        openedFromItemMenu = false;
    }
}
