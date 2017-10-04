using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritAttack : Attack {
    
    public CritAttack() { }


    public override IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        if (attacker)
        {
            target = CheckTargetAlive(attacker, target);

            if (target) { 
                // The attack is used within this bracket
                int damageDealt = CalcAttackDamage(this, target, attacker, true);
                string battleMessage = String.Format("{0} takes a vicious strike at {1}!", attacker.characterName, target.characterName);
                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);

                // CalcAttackDamage will return -1 if a critical strike misses.
                if (damageDealt < 0)
                {
                    battleMessage = String.Format("But {0} missed!", attacker.characterName, target.characterName);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);
                } else
                {
                    battleMessage = String.Format("{0}'s critical blow scores {2} points of damage to {1}!", attacker.characterName, target.characterName, damageDealt);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);
                    DealDamage(this, attacker, target, false, damageDealt, false);
                }
                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);

                // update health display if hero
                yield return null;

                if (BattleManager.hpm.activePartyMembers.Contains(target))
                {
                    BattleManager.bManager.battleMenu.UpdatePanel(target);
                }

                BattleManager.bManager.CheckLose();
                BattleManager.bManager.CheckWin();

            }
            else
            {
                Debug.Log("Error!!!");
            }


            if (turnList.Count == 0)
            {
                if (!BattleManager.hasLost && !BattleManager.hasWon)
                {
                    ReturnToMenu();
                } else if (BattleManager.hasLost)
                {
                    Debug.Log("Game Over...");
                }
            } else
            {
                Turn nextTurn = turnList[0];
                turnList.RemoveAt(0);
                BattleManager.bManager.StartInactiveTurn(nextTurn, turnList);
                //StartCoroutine(attack.UseAttack(nextTurn.attacker, nextTurn.target, turnList));
            }
        } else
        {
            Turn nextTurn = turnList[0];
            turnList.RemoveAt(0);
            BattleManager.bManager.StartInactiveTurn(nextTurn, turnList);
            //StartCoroutine(nextTurn.attack.UseAttack(nextTurn.target, nextTurn.attacker, turnList));
        }
    }
}
