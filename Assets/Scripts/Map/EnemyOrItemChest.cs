using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class EnemyOrItemChest : InteractableTile
{
    public ContinentType whereDoesThisAppear;
    bool heroOnTile = false;
    bool alreadyOpened = false;

    private void Update()
    {
        if (heroOnTile && Input.GetButtonDown("AButton") && !alreadyOpened)
        {
            if (currentlyStandingOnInteractableTile)
            {
                alreadyOpened = true;
                StartCoroutine(ShowMessageThenStartBattle());
            }
            else
            {
                heroOnTile = false;
            }
        }
    }

    IEnumerator ShowMessageThenStartBattle()
    {
        TextBoxManager.tbm.EnableTextBox(null, "Boo! A monster jumped out!", 
            true, false, true);

        while (TextBoxManager.tbm.isActive)
        {
            yield return null;
        }

        BattleManager.bManager.StartBattle(null, GameObject.Instantiate
            (BattleManager.bManager.areaEncounters
            .GetChestEnemy(whereDoesThisAppear).gameObject)
            .GetComponent<EnemyPartyManager>());

    }

    public override void ActivateInteraction()
    {
        heroOnTile = true;
        Debug.Log("HeroOnTile");
    }
}
