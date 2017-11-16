using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingAttack : Attack {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        if (attacker)
        {
            if (BattleManager.hpm.PartyContainsCharacter(attacker))//.activePartyMembers.Contains(attacker))
            {
                // A hero is attacking
                if (!BattleManager.epm.PartyContainsCharacter(target))//.activePartyMembers.Contains(target))
                {
                    // The enemy target is dead, find a new target
                    target = BattleManager.epm.activePartyMembers[0];
                }
                if (target)
                {

                    // visual effects, message
                    string battleMessage = string.Format("{0} rolls aimlessly...", attacker.characterName);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                    while (TextBoxManager.tbm.isTyping)
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(.3f);
                }
            }
            else if (BattleManager.epm.activePartyMembers.Contains(attacker))
            {

                // 
                if (target == null)
                {
                    // All the party's heroes must be dead.
                    //BattleManager.bManager.
                }
                else
                {

                    // visual effects, message
                    string battleMessage = string.Format("{0} rolls aimlessly...", attacker.characterName);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                    while (TextBoxManager.tbm.isTyping)
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(.3f);
                }

            }
            else
            {
                // This is encountered when an enemy attacks after another enemy has died. 
                // It shouldn't happen, but I haven't found any issues with it.
            }


            EndTurnCheck(turnList);
        }
        else if (!BattleManager.hasLost && !BattleManager.hasWon)
        {
            Turn nextTurn = turnList[0];
            turnList.RemoveAt(0);
            BattleManager.bManager.StartInactiveTurn(nextTurn, turnList);
            //StartCoroutine(nextTurn.attack.UseAttack(nextTurn.target, nextTurn.attacker, turnList));
        }
        else
        {
            BattleManager.bManager.GameOver();
        }
    }
}
