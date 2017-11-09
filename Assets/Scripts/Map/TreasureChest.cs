using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class TreasureChest : InteractableTile
{
    public BaseItem treasure;
    public bool hasBeenOpened = false;
    public bool randomizeTreasure = true;

    bool heroStandingOnThisTile = false;

    private void Start()
    {
        if (treasure == null || randomizeTreasure)
        {
            treasure = ItemGenerator.instance.GetTreasureBasedOnLocation();
        }
    }

    private void Update()
    {
        if (heroStandingOnThisTile && Input.GetButtonDown("AButton"))
        {
            if (currentlyStandingOnInteractableTile)
            {
                if (!hasBeenOpened)
                {
                    GameManager.gm.leader.DisableMovement();
                    TextBoxManager textManager = TextBoxManager.tbm;
                    textManager.currentLine = 0;
                    textManager.endLine = 0; // Controls how many windows

                    string boxMessage = "Inside the chest, you found a " + treasure.GetItemData().itemName + ".";
                    TextBoxManager.tbm.EnableTextBox(null, boxMessage, true, false, true);
                    // TODO: Open box animation
                    GameManager.gm.gameObject.GetComponent<Inventory>().AddToInventory(treasure.GetItemData());


                    hasBeenOpened = true;
                    heroStandingOnThisTile = false;

                    StartCoroutine(WaitUntilMessageDoneThenDestroy());
                }
                else
                {

                }
            } else
            {
                heroStandingOnThisTile = false;
            }
        }
    }

    protected IEnumerator WaitUntilMessageDoneThenDestroy()
    {
        yield return new WaitForSeconds(1);
        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }

    public override void ActivateInteraction()
    {
        if (!hasBeenOpened)
        {
            heroStandingOnThisTile = true;
        }
    }
}
