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
                    if (GameManager.gm.gameObject.GetComponent<Inventory>().ContainsItem(key.GetItemData())){
                        //Debug.Log((GameManager.gm.gameObject.GetComponent<Inventory>().)
                        Debug.Log(key.GetItemData().itemName);
                        TextBoxManager textManager = TextBoxManager.tbm;
                        textManager.currentLine = 0;
                        textManager.endLine = 0; // Controls how many windows

                        GameManager.gm.leader.DisableMovement();

                        string boxMessage = "You unlock the chest and find a " 
                            + treasure.GetItemData().itemName + ".";
                        TextBoxManager.tbm.EnableTextBox(null, boxMessage, 
                            true, false, true);
                        // TODO: Open box animation
                        GameManager.gm.gameObject.GetComponent<Inventory>()
                            .AddToInventory(treasure.GetItemData());

                        GameManager.gm.gameObject.GetComponent<Inventory>()
                            .DecrementSupply(key.GetItemData());

                        opened = true;
                        heroOnTile = false;

                        StartCoroutine(WaitThenDestroy());
                    }
                    else if (!GameManager.gm.gameObject.GetComponent<Inventory>().ContainsItem(key.GetItemData()))
                    {
                        TextBoxManager textManager = TextBoxManager.tbm;
                        textManager.currentLine = 0;
                        textManager.endLine = 0;

                        GameManager.gm.leader.DisableMovement();

                        string lockedMessage = "The chest is locked. You need a " + key.GetItemData().itemName + ".";
                        TextBoxManager.tbm.EnableTextBox(null, lockedMessage, true, false, true);


                        heroOnTile = false;
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

    IEnumerator WaitThenDestroy()
    {
        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }
    public override void ActivateInteraction()
    {
        if (!opened)
        {
            heroOnTile = true;
        }
    }
}

