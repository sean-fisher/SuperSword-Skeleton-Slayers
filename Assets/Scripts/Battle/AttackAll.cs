using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAll : Attack {
    
    public override IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        if (attacker)
        {
            // hero attacks all enemies

            if (BattleManager.hpm.PartyContainsCharacter(attacker))//.activePartyMembers.Contains(attacker))
            {
                // A hero is attacking
                List<BaseCharacter> enemies = BattleManager.epm.activePartyMembers;
                int totalDamage = 0;
                List<int> damagesToDeal = new List<int>();
                for (int i = 0; i < enemies.Count; i++)
                {
                    target = enemies[i];
                    int damageDealt = CalcAttackDamage(this, target, attacker);
                    totalDamage += damageDealt;
                    damagesToDeal.Add(damageDealt);
                }

                // visual effects, message
                string battleMessage = String.Format("{0} attacks every enemy with {2}, scoring an average of {1} damage!", attacker.characterName,
                    totalDamage / enemies.Count, attackName);

                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);

                for (int i = 0; i < enemies.Count; i++)
                {
                    int health = enemies[i].currentHP;
                    health -= DealDamage(this, attacker, enemies[i], false, damagesToDeal[i]);
                    if (health < 0)
                    {
                        i--;
                    }
                }

                // update health display if hero
                yield return null;
                

            } else if (BattleManager.epm.activePartyMembers.Contains(attacker))
            {
                // An enemy is attacking

                List<BaseCharacter> targets;

                if (attackType == AttackType.HEALING)
                {
                    targets = BattleManager.epm.activePartyMembers;
                } else
                {
                    targets = BattleManager.hpm.activePartyMembers;

                }

                int totalDamage = 0;
                List<int> damagesToDeal = new List<int>();
                for (int i = 0; i < targets.Count; i++)
                {
                    target = targets[i];
                    int damageDealt = CalcAttackDamage(this, target, attacker);
                    totalDamage += damageDealt;
                    damagesToDeal.Add(damageDealt);
                }

                // visual effects, message
                string battleMessage = String.Format("{0} attacks every hero, scoring an average of {1} damage!", attacker.characterName,
                    totalDamage / targets.Count);

                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);

                for (int i = 0; i < targets.Count; i++)
                {
                    DealDamage(this, attacker, targets[i], false, damagesToDeal[i]);
                }

                // update health display if hero
                yield return null;

            }
            else
            {
                // This is encountered when an enemy attacks after another enemy has died. 
                // It shouldn't happen, but I haven't found any issues with it.
            }

            if (mpUsed > 0)
            {
                attacker.currentMP -= mpUsed;
                BattleManager.bManager.battleMenu.UpdatePanel(attacker);
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
