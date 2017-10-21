using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipSalesman : NPCTile {

    protected override void NPCInteraction()
    {
        GridController.partyCanMove = false;
        if (Inventory.partyGold < 0)
        {
            SayLine("You want this airship, huh? I guess I'll sell it, but only if you got the money. Bring me 1000G.");
            GridController.partyCanMove = true;
        } else
        {
            StartCoroutine(Conversation());
        }
    }

    IEnumerator Conversation()
    {
        SayLine("Do you want to buy the airship for 1000G?", true);

        while (TextBoxManager.tbm.isActive)
        {
            yield return null;
        }

        Debug.Log(TextBoxManager.tbm.GetPlayerChoice());
        if (TextBoxManager.tbm.GetPlayerChoice() == 0)
        {
            // Yes has been picked
            AirshipTile.canBoard = true;
            Inventory.SpendGold(1000);
        } else
        {

        }
        GridController.partyCanMove = true;
    }
}
