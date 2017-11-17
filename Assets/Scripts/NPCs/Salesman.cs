using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salesman : NPCTile {

    public ContinentType whereDoesThisAppear;

    // These must have a "{0}" which is replaced with the item name string, 
    // and {1} which is replaced by the item cost.
    public string notEnoughMoneyFormattedString;
    public string enoughMoneyFormattedString;
    public BaseItem itemToSell;
    public bool sellRandItem;

    private void Start()
    {
        // Chooses a random item to sell
        switch (whereDoesThisAppear)
        {
            case (ContinentType.GRASSLAND):
                break;
            case (ContinentType.GLACIER):
                itemToSell = ItemGenerator.instance.equipSpawns[
                    Random.Range(0, ItemGenerator.instance.equipSpawns.Count)];
                break;
            default:
                Debug.Log("Invalid continent Type");
                break;
        }
    }

    protected override void NPCInteraction()
    {
        GridController.partyCanMove = false;
        if (Inventory.partyGold < itemToSell.GetItemData().itemCost)
        {
            SayLine(string.Format(notEnoughMoneyFormattedString, 
                itemToSell.GetItemData().itemName, itemToSell.GetItemData().itemCost));
            GridController.partyCanMove = true;
        } else
        {
            StartCoroutine(Conversation());
        }
    }

    IEnumerator Conversation()
    {
        SayLine(string.Format(enoughMoneyFormattedString, itemToSell
            .GetItemData().itemName, itemToSell.GetItemData().itemCost), true);

        while (TextBoxManager.tbm.isActive)
        {
            yield return null;
        }

        Debug.Log(TextBoxManager.tbm.GetPlayerChoice());
        if (TextBoxManager.tbm.GetPlayerChoice() == 0)
        {
            // Yes has been picked
            Inventory.SpendGold(itemToSell.GetItemData().itemCost);

            GameManager.gm.gameObject.GetComponent<Inventory>().AddToInventory(itemToSell.GetItemData());
        } else
        {

        }
        GridController.partyCanMove = true;
    }
}
