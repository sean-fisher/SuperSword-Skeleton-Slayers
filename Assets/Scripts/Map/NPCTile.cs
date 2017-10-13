using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class NPCTile : InteractableTile
{
    public BaseItem treasure;

    public string dialogueMessage;

    public override void ActivateInteraction()
    {
        if (Input.GetButtonDown("AButton"))
        {
            NPCInteraction();
        }
    }

    protected virtual void NPCInteraction()
    {
        SayLine(dialogueMessage);
    }

    protected void SayLine(string line, bool yesNo = false)
    {
        GameManager.gm.leader.DisableMovement();
        TextBoxManager textManager = TextBoxManager.tbm;
        textManager.currentLine = 0;
        textManager.endLine = 0; // Controls how many windows

        TextBoxManager.tbm.EnableTextBox(null, line, true, yesNo, true);
    }
}
