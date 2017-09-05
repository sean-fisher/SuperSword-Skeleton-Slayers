using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Attack {

    public Wait() { }


    public override IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        string battleMessage = string.Format("{0} waits patiently for a better opportunity to strike.", attacker.characterName);
        //TextBoxManager.tbm.PlayConversation(XMLLoader.LoadConvo(XMLLoader.battleMessages, attackID));
        TextBoxManager.tbm.EnableTextBox(BattleManager.bManager.messageBoxImg.gameObject, battleMessage, false);

        // Gives time for the player to read the message
        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        yield return new WaitForSeconds(.3f);

        // If all of this phase's turns are over, a win/lose double check occurs.
        if (turnList.Count == 0)
        {
            if (!BattleManager.hasLost && !BattleManager.hasWon)
            {
                ReturnToMenu();
            }
            else if (BattleManager.hasLost)
            {
                Debug.Log("Game Over...");
            }
        }
        else
        {
            // The phase continues, and another hero or enemy takes his/her turn
            Turn nextTurn = turnList[0];
            turnList.RemoveAt(0);
            BattleManager.bManager.StartInactiveTurn(nextTurn, turnList);
        }
    }
}
