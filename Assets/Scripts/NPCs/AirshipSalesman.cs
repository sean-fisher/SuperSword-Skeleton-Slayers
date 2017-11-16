using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipSalesman : NPCTile {

    bool saleMade = false;

    protected override void NPCInteraction()
    {
        GridController.partyCanMove = false;
        if (Inventory.partyGold < 1000 && !saleMade)
        {
            SayLine("You want this airship, huh? I guess I'll sell it, but only if you got the money. Bring me 1000G.");
            GridController.partyCanMove = true;
        } else if (!saleMade)
        {
            StartCoroutine(Conversation());
        } else
        {
            SayLine("Thanks for taking that airship off of my hands. Now I have the money to vacation in Costa Rica!");
        }
    }

    IEnumerator Conversation()
    {
        SayLine("Do you want to buy the airship for 1000G?", true);

        while (TextBoxManager.tbm.isActive)
        {
            yield return null;
        }


        if (TextBoxManager.tbm.GetPlayerChoice() == 0)
        {
            // Yes has been picked
            saleMade = true;
            AirshipTile.canBoard = true;
            Inventory.SpendGold(1000);
        } else
        {
            // no has been picked
            Debug.Log("Pick No: " + TextBoxManager.tbm.GetPlayerChoice());
        }
        GridController.partyCanMove = true;
    }
}
