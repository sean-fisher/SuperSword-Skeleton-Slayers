using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class NPCTile : InteractableTile
{
    

    public override void ActivateInteraction()
    {
        if (Input.GetButtonDown("AButton"))
        {
            NPCInteraction();
        }
    }

    protected virtual void NPCInteraction()
    {
        SayLine("No message provided");
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
