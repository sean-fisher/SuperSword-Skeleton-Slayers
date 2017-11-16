using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemTurn : Attack {

    public ItemData itemBeingUsed;
    
    // What an attack does after the player has selected all his/her characters' moves and the move is used.
    // If turnList is empty, all turns are over and the player gets to select moves again (assuming the player is alive)
    public override IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        Debug.Log("Target name: " + target.characterName);
        // THIS IS THE BASIC OUTLINE FOR A USEATTACK(...) IMPLEMENTATION.
        if (attacker)
        {
            target = CheckTargetAlive(attacker, target);

            if (target)
            {
                // The attack is used within this bracket

                // Calculates damage dealt. The final bool parameter is only true for attacks with a chance of critical hits.
                int damageDealt = CalcAttackDamage(this, target, attacker, false);
                Debug.Log("Attack activated: " + attackName);

                if (itemBeingUsed == null)
                {
                    itemBeingUsed = BattleManager.bManager.itemsToUse[0];
                    BattleManager.bManager.itemsToUse.RemoveAt(0);
                }
                Debug.Log(itemBeingUsed.itemName);

                // Displays appropriate battle message
                string battleMessage = string.Format("{0} uses {2} on {1}!", 
                    attacker.characterName, 
                    target.characterName, 
                    itemBeingUsed.itemName);
                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                // Gives time for the player to read the message
                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);

                string message = itemBeingUsed.Effect(new BaseCharacter[] { target });
                
                battleMessage = string.Format(message, target.characterName);
                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                // Gives time for the player to read the message
                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);

                // If a hero was attacked, his/her HeroDisplayPanel is updated so the player can keep track of his/her HP.
                yield return null;
                if (BattleManager.hpm.activePartyMembers.Contains(target))
                {
                    BattleManager.bManager.battleMenu.UpdatePanel(target);
                }

            }
            else
            {
                Debug.Log("Error!!!");
            }
        }

        // NO NEED TO EDIT CODE FROM HERE TO END OF METHOD
        EndTurnCheck(turnList);
    }

    // END EXAMPLE IMPLEMENTATION
    
}