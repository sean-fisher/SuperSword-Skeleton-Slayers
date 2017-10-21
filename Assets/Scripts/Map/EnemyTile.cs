using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class EnemyTile : InteractableTile
{
    public BaseItem treasure;

    public EnemyPartyManager epm;

    public string[] dialogueMessage;

    public override void ActivateInteraction()
    {
        if (Input.GetButtonDown("AButton"))
        {
            StartBattleInteraction();
        }
    }

    protected virtual void StartBattleInteraction()
    {
        SayLine(dialogueMessage);
    }

    protected void SayLine(string[] lines, bool yesNo = false)
    {
        GameManager.gm.leader.DisableMovement();
        TextBoxManager textManager = TextBoxManager.tbm;
        textManager.currentLine = 0;
        textManager.endLine = 0; // Controls how many windows

        BattleManager.bManager.StartBattle(dialogueMessage);
    }
}
