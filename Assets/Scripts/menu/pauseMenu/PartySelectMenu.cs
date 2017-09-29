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
            itemToUse.Effect(BattleManager.hpm.activePartyMembers[menuIndex]);
            bool hasItemStock = GameManager.gm.inventory.DecrementSupply(itemToUseUnsortedIndex);

            if (!hasItemStock)
            {
                CloseMenu();
                PauseMenu.itemMenu.OpenMenu();
            }
            PauseMenu.itemMenu.UpdateItemCounts(false, menuIndex);
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
            PauseMenu.itemMenu.OpenMenu();
        }
        openedFromItemMenu = false;
    }
}
