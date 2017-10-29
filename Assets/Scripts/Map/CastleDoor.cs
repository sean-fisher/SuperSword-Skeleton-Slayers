using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class CastleDoor : InteractableTile
{
    public BaseItem key;
    bool heroOnTile = false;

    private void Update()
    {
        if (heroOnTile && Input.GetButtonDown("AButton"))
        {
            if (currentlyStandingOnInteractableTile)
            {
                if (GameManager.gm.gameObject.GetComponent<Inventory>().ContainsItem(key.GetItemData()))
                {
                    BattleManager.isFightingFinalBoss = true;
                    BattleManager.bManager.StartBattle(new string[] { "Boo! Welcome to the castle of spook!" }, GameObject.Instantiate(BattleManager.bManager.areaEncounters.GetFinalBoss().gameObject).GetComponent<EnemyPartyManager>());

                    heroOnTile = false;
                }
                else
                {
                    TextBoxManager textManager = TextBoxManager.tbm;
                    textManager.currentLine = 0;
                    textManager.endLine = 0;

                    GameManager.gm.leader.DisableMovement();

                    string lockedMessage = "A foreboding structure looms before you, but you still need a " + key.GetItemData().itemName + ".";
                    TextBoxManager.tbm.EnableTextBox(null, lockedMessage, true, false, true);


                    heroOnTile = false;
                    Debug.Log("Attempted opening");
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
        heroOnTile = true;
        Debug.Log("HeroOnTile");
    }
}
