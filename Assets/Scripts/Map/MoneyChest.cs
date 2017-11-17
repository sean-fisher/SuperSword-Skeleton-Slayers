using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MoneyChest : InteractableTile
{
    public int goldInside;
    public bool hasBeenOpened = false;

    bool heroStandingOnThisTile = false;

    private void Start()
    {
        goldInside = Random.Range(900, 1200);
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

                    string boxMessage = "Inside the chest, you found " 
                        + goldInside + " Gold!";
                    TextBoxManager.tbm.EnableTextBox(
                        null, boxMessage, true, false, true);

                    Inventory.GainGold(goldInside);

                    hasBeenOpened = true;
                    heroStandingOnThisTile = false;

                    StartCoroutine(WaitThenDestroy());
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

    public override void ActivateInteraction()
    {
        if (!hasBeenOpened)
        {
            heroStandingOnThisTile = true;
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
}
