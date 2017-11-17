using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class EnemyOrItemChest : InteractableTile
{
    public ContinentType whereDoesThisAppear;
    bool heroOnTile = false;
    bool alreadyOpened = false;
    public BaseItem treasure;

    private void Start()
    {
        treasure = ItemGenerator.instance.GetTreasureBasedOnLocation(whereDoesThisAppear);
    }

    private void Update()
    {
        if (heroOnTile && Input.GetButtonDown("AButton") && !alreadyOpened)
        {
            if (currentlyStandingOnInteractableTile)
            {
                alreadyOpened = true;
                if (Random.Range(0, 2) == 0)
                {
                    StartCoroutine(ShowMessageThenStartBattle());
                    StartCoroutine(WaitThenDestroy());
                } else
                {

                    GameManager.gm.leader.DisableMovement();
                    TextBoxManager textManager = TextBoxManager.tbm;
                    textManager.currentLine = 0;
                    textManager.endLine = 0; // Controls how many windows

                    string boxMessage = "Inside the chest, you found a " + treasure.GetItemData().itemName + ".";
                    TextBoxManager.tbm.EnableTextBox(null, boxMessage, true, false, true);
                    // TODO: Open box animation
                    GameManager.gm.gameObject.GetComponent<Inventory>().AddToInventory(treasure.GetItemData());
                    StartCoroutine(WaitThenDestroy());
                }
                StartCoroutine(WaitUntilMessageDoneThenDestroy());
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
    protected IEnumerator WaitUntilMessageDoneThenDestroy()
    {
        yield return new WaitForSeconds(1);
        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        //Destroy(this.gameObject);
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
            (AreaEncounters.aeinstance.GetRandomEncounter(whereDoesThisAppear)
            .GetComponent<EnemyPartyManager>()));

    }

    public override void ActivateInteraction()
    {
        heroOnTile = true;
        Debug.Log("HeroOnTile");
    }
}
