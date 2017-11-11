using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardAttack : Attack {

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
            bool targetIsHero = false;
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
                    int damageDealt = CalcAttackDamage(this, target, attacker);
                    string battleMessage = String.Format("{0} attacks {1}, scoring {2} points of damage!", attacker.characterName, target.characterName, damageDealt);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                    while (TextBoxManager.tbm.isTyping)
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(.3f);


                    DealDamage(this, attacker, target, false, damageDealt);


                    // update health display if hero
                    yield return null;


                    if (targetIsHero)
                    {
                        BattleManager.bManager.battleMenu.UpdatePanel(target);
                    }
                    //Debug.Log(attacker.characterName + " deals " + damageDealt + " damage to " + target.characterName);

                } else
                {
                    //Debug.Log("Checkwin");
                }
            } else if (BattleManager.epm.activePartyMembers.Contains(attacker))
            {
                // An enemy is attacking
                targetIsHero = true;

                // 
                if (target == null)
                {
                    // All the party's heroes must be dead.
                    //BattleManager.bManager.
                }
                else
                {

                    // If the hero target is dead, find an alive hero
                    if (target.isDead)
                    {
                        for (int i = 0; i < BattleManager.hpm.activePartyMembers.Count && target.isDead; i++)
                        {
                            if (!BattleManager.hpm.activePartyMembers[i].isDead)
                            {
                                target = BattleManager.hpm.activePartyMembers[i];
                            }
                        }
                    }

                    // visual effects, message
                    int damageDealt = CalcAttackDamage(this, target, attacker);
                    string battleMessage = String.Format("{0} attacks {1}, scoring {2} points of damage!", attacker.characterName, target.characterName, damageDealt);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                    while (TextBoxManager.tbm.isTyping)
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(.3f);


                    DealDamage(this, attacker, target, false, damageDealt);


                    // update health display if hero
                    yield return null;


                    if (targetIsHero)
                    {
                        BattleManager.bManager.battleMenu.UpdatePanel(target);
                    }

                }

            }
            else
            {
                // This is encountered when an enemy attacks after another enemy has died. 
                // It shouldn't happen, but I haven't found any issues with it.
            }

            
            EndTurnCheck(turnList);
        } else if (!BattleManager.hasLost && !BattleManager.hasWon)
        {
            Turn nextTurn = turnList[0];
            turnList.RemoveAt(0);
            BattleManager.bManager.StartInactiveTurn(nextTurn, turnList);
            //StartCoroutine(nextTurn.attack.UseAttack(nextTurn.target, nextTurn.attacker, turnList));
        } else
        {
            BattleManager.bManager.GameOver();
        }
    }
}
