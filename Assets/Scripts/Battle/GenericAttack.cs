using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAttack : Attack {

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
            } else if (BattleManager.epm.activePartyMembers.Contains(attacker))
            {
                // An enemy is attacking
                targetIsHero = true;

            }

            // visual effects, message
            int damageDealt = CalcAttackDamage(this, target, attacker);
            string battleMessage = String.Format("{0} uses {1} on {2}, scoring {3} damage!", attacker.characterName, this.attackName, target.characterName, damageDealt);
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
