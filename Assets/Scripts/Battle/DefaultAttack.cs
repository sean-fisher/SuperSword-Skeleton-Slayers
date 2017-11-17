using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack : Attack {


    public override IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        if (attackName == "Pray")
        {

            Debug.Log("Target neam: " + target.characterName);
        }
        if (attacker)
        {
            if (attacker.currentMP >= mpUsed)
            {
                bool targetIsHero = false;
                if (BattleManager.hpm.PartyContainsCharacter(attacker))//.activePartyMembers.Contains(attacker))
                {
                    if (BattleManager.hpm.PartyContainsCharacter(target))
                    {
                        // Hero Attacking/healing a hero
                    } else
                    // A hero is attacking
                    if (!BattleManager.epm.PartyContainsCharacter(target))//.activePartyMembers.Contains(target))
                    {
                        // The enemy target is dead, find a new target
                        target = BattleManager.epm.activePartyMembers[0];
                    }
                }
                else if (BattleManager.epm.activePartyMembers.Contains(attacker))
                {
                    // An enemy is attacking
                    targetIsHero = true;

                }

                // visual effects, message
                int damageDealt = CalcAttackDamage(this, target, attacker);
                string battleMessage;
                if (attackType != AttackType.HEALING) {
                    battleMessage = String.Format("{0} uses {1} on {2}, scoring {3} damage!", attacker.characterName, this.attackName, target.characterName, damageDealt);
                } else
                {
                    battleMessage = String.Format("{0} uses {1} on {2}, healing {3} HP!", attacker.characterName, this.attackName, target.characterName, Mathf.Abs(damageDealt));
                    damageDealt = Mathf.Abs(damageDealt);
                }
                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);


                DealDamage(this, attacker, target, false, damageDealt);

                attacker.currentMP -= mpUsed;

                // update health display if hero
                yield return null;


                if (targetIsHero)
                {
                    BattleManager.bManager.battleMenu.UpdatePanel(target);
                }
            } else
            {
                // Mp isn't high enough
                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, attacker.characterName + " tried to use " + this.attackName + ", but didn't have enough MP!", false);

                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);
            }

            EndTurnCheck(turnList);
        } else if (!BattleManager.hasLost && !BattleManager.hasWon)
        {
            if (turnList.Count > 0)
            {
                Turn nextTurn = turnList[0];
                turnList.RemoveAt(0);
                BattleManager.bManager.StartInactiveTurn(nextTurn, turnList);
            } else
            {
                EndTurnCheck(turnList);
            }
            //StartCoroutine(nextTurn.attack.UseAttack(nextTurn.target, nextTurn.attacker, turnList));
        } else
        {
            BattleManager.bManager.GameOver();
        }
    }
}
