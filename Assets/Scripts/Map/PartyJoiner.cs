using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyJoiner : NPCTile
{
    public BaseCharacter heroPrefab;
    public string message;

    bool saleMade = false;

    protected override void NPCInteraction()
    {
        GridController.partyCanMove = false;
        StartCoroutine(Conversation());
    }

    IEnumerator Conversation()
    {
        SayLine(message);

        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        BattleManager.hpm.InstantiateAndAdd(heroPrefab);
        Destroy(this.gameObject);
    }
}
