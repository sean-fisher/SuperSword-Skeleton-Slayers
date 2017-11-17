using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : Attack {
    

    public override IEnumerator UseAttack(BaseCharacter attacker, 
        BaseCharacter target, List<Turn> turnList)
    {
        // The attack is used within this bracket
        string battleMessage = String.Format("{0} tries to run away!", 
            attacker.characterName);
        TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        yield return new WaitForSeconds(.3f);

        int escapeChance = UnityEngine.Random.Range(0, 1);
        
        if (escapeChance == 0)
        {
            battleMessage = String.Format("The heroes escaped!", 
                attacker.characterName);
            TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

            Sounds.audioSource.clip = Sounds.runaway;
            Sounds.audioSource.Play();

            BattleManager.isFightingFinalBoss = false;
        }
        else
        {
            battleMessage = String.Format("{0} wasn't fast enough...", attacker.characterName);
            TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);
        }
        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        yield return new WaitForSeconds(.3f);

        if (escapeChance == 0)
        {
            BattleManager.bManager.RunAway();
        }
        else
        {

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
}
