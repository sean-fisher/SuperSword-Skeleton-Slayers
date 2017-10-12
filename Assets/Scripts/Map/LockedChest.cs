using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class LockedChest : InteractableTile
{
    public BaseItem treasure;
    public bool opened = false;
    public BaseItem key;

    bool heroOnTile = false;

    private void Update()
    {
        if (heroOnTile && Input.GetButtonDown("AButton"))
        {
            if (currentlyStandingOnInteractableTile)
            {
                if (!opened)
                {
                    if (GameManager.gm.gameObject.GetComponent<Inventory>().containsItem(key.GetItemData())){
                        TextBoxManager textManager = TextBoxManager.tbm;
                        textManager.currentLine = 0;
                        textManager.endLine = 0; // Controls how many windows

                        string boxMessage = "You unlock the chest and find a " + treasure.GetItemData().itemName + ".";
                        TextBoxManager.tbm.EnableTextBox(null, boxMessage, true);
                        // TODO: Open box animation
                        GameManager.gm.gameObject.GetComponent<Inventory>().AddToInventory(treasure.GetItemData());


                        opened = true;
                        heroOnTile = false;
                        Debug.Log("Open Locked Chest");
                    }
                    else if (!GameManager.gm.gameObject.GetComponent<Inventory>().containsItem(key.GetItemData()))
                    {
                        TextBoxManager textManager = TextBoxManager.tbm;
                        textManager.currentLine = 0;
                        textManager.endLine = 0;

                        string lockedMessage = "The chest is locked. You need a " + key.GetItemData().itemName + ".";
                        TextBoxManager.tbm.EnableTextBox(null, lockedMessage, true);

                        Debug.Log("Attempted opening");
                    }
                    else {
                        Debug.Log("Something went wrong opening the lock!");
                    }
                }
                else
                {

                }
            }
            else
            {
                heroOnTile = false;
            }
        }
    }

    public override void ActivateInteraction()
    {
        if (!opened)
        {
            heroOnTile = true;
            Debug.Log("HeroOnTile");
        }
    }
}

