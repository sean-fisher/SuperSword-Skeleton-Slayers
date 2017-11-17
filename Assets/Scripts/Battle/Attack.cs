using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class Attack : MonoBehaviour {

    public string attackName;
    //public int attackID;

    public AttackType attackType;
    public bool onlyUsableOnSelf = false;
    public int damage;
    public bool isMelee;

    public static GameObject battleMessageWindow;

    // enemy attacks are picked randomly; higher values are more frequent. 1 to 5.
    public int occurrenceChance = 5;

    //This can only be true if the attack is of AttackType HEALING.
    public bool canRevive;

    public string description;

    //This state int refers to the animation that is played when this attack is executed.
    public int state;

    public GameObject attackAnimation;

    public int mpUsed;

    // What an attack does after the player has selected all his/her characters' moves and the move is used.
    // If turnList is empty, all turns are over and the player gets to select moves again (assuming the player is alive)
    public virtual IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        // THIS IS THE BASIC OUTLINE FOR A USEATTACK(...) IMPLEMENTATION.
        if (attacker)
        {
            target = CheckTargetAlive(attacker, target);

            if (target)
            {
                // The attack is used within this bracket

                // Calculates damage dealt. The final bool parameter is only true for attacks with a chance of critical hits.
                int damageDealt = CalcAttackDamage(this, target, attacker, false);

                // Displays appropriate battle message
                string battleMessage = string.Format("{0} attacks {1}!", attacker.characterName, target.characterName);
                TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);

                // Gives time for the player to read the message
                while (TextBoxManager.tbm.isTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(.3f);

                // CalcAttackDamage will return -1 if a critical strike misses. ONLY ATTACKS WITH CRIT CHANCE CAN MISS. 
                // THIS CHECK IS THRERFORE NOT FUNCTIONAL FOR THIS EXAMPLE
                if (damageDealt < 0)
                {
                    battleMessage = string.Format("But {0} missed!", attacker.characterName, target.characterName);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);
                }
                else
                {
                    battleMessage = string.Format("{0}'s blow scores {2} points of damage to {1}!", attacker.characterName, target.characterName, damageDealt);
                    TextBoxManager.tbm.EnableTextBox(battleMessageWindow, battleMessage, false);
                    DealDamage(this, attacker, target, false, damageDealt);
                }

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

    public void EndTurnCheck(List<Turn> turnList)
    {
        Debug.Log("end turn check: " + turnList.Count);
        StartCoroutine(WaitThenReturnToMenu(turnList));
    }

    IEnumerator WaitThenReturnToMenu(List<Turn> turnList)
    {
        while (Explosion.isExploding)
        {
            yield return null;
        }
        BattleManager.bManager.battleMenu.UpdateAllPanels();
        // If all of this phase's turns are over, a win/lose double check occurs.
        if (turnList.Count == 0)
        {
            BattleManager.bManager.CheckWin();
            BattleManager.bManager.CheckLose();
            if (!BattleManager.hasLost && !BattleManager.hasWon)
            {
                ReturnToMenu();
            }
            else if (BattleManager.hasLost)
            {
                //BattleManager.bManager.GameOver();
            }
        }
        else if (!BattleManager.hasWon && !BattleManager.hasLost)
        {

            // The phase continues, and another hero or enemy takes his/her turn
            Turn nextTurn = turnList[0];
            turnList.RemoveAt(0);
            BattleManager.bManager.StartInactiveTurn(nextTurn, turnList);
        }
        else
        {
            Debug.Log("Error!");
            //BattleManager.bManager.GameOver();
        }
    }

    // END EXAMPLE IMPLEMENTATION

    protected BaseCharacter CheckTargetAlive(BaseCharacter attacker, BaseCharacter target)
    {
        
        if (BattleManager.hpm.activePartyMembers.Contains(attacker))
        {
            // A hero is attacking
            if (!BattleManager.hpm.activePartyMembers.Contains(attacker) 
                && !BattleManager.epm.activePartyMembers.Contains(target))
            {
                // The enemy target is dead, find a new target
                target = BattleManager.epm.activePartyMembers[0];
            }
        }
        else if (BattleManager.epm.activePartyMembers.Contains(attacker))
        {
            // An enemy is attacking

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

            }
        }
        return target;
    }

            //public abstract void UseDualTech(CharacterStateMachine target, BattleStateMachine BSM);


    public int DealDamage(Attack attack, BaseCharacter attacker, BaseCharacter targetCharacter, bool canCrit = false, int damageDealt = 0, bool checkWinLose = true)
    {
        //If the target was dead but was resurrected, it is set to alive
        bool targetIsDead = targetCharacter.currentHP < 1;

        if (damageDealt == 0)
        {
            damageDealt = CalcAttackDamage(attack, targetCharacter, attacker, canCrit);
        }

        if (attack.attackType != AttackType.HEALING)
        {
            //damage is dealt to the enemy's target hero
            targetCharacter.currentHP -= damageDealt;
        } else
        {
            targetCharacter.currentHP += damageDealt;

        }


        //damageDealt is displayed over hero's head.
        //hero.displayDamage(damageDealt);

        //if the target's health drops below zero, set to dead. If target is healed and HP is greater than max, currentHP is set to max.
        if (targetCharacter.currentHP <= 0)
        {
            BattleManager.bManager.KillCharacter(targetCharacter, checkWinLose);
            //targetCharacter.SetCurrentState(CharacterStateMachine.TurnState.DEAD);
            //targetCharacter.GetBaseCharacter().currentHP = 0;

            //BSM.PerformState = BattleStateMachine.PerformAction.CHECKWIN;
        }
        else
        {
            if (targetCharacter.currentHP > targetCharacter.baseHP)
            {
                targetCharacter.currentHP = targetCharacter.baseHP;
                if (attack.attackType == AttackType.HEALING)
                {
                    // update health
                    targetCharacter.currentHP += Mathf.Abs(damageDealt);
                    if (targetCharacter.currentHP > targetCharacter.baseHP)
                    {
                        targetCharacter.currentHP = targetCharacter.baseHP;
                    }

                    // Update health display
                    if (BattleManager.hpm.activePartyMembers.Contains(attacker))
                    {
                        BattleManager.bManager.battleMenu.UpdatePanel(attacker);
                    }
                    // Update health display
                    if (BattleManager.hpm.activePartyMembers.Contains(targetCharacter))
                    {
                        BattleManager.bManager.battleMenu.UpdatePanel(targetCharacter);
                    }
                }
            }
            else if (targetIsDead) //if the target was revived, heroesAlive is incremented.
            {
                //BSM.HeroesAlive++; //Be sure to make resurrection only possible when the proper spell is used.
                throw new System.Exception("This shouldn't be necessary");
            }
        }

        switch (attack.attackType)
        {
            case (AttackType.HEALING):
                Sounds.audioSource.clip = Sounds.heal;
                Sounds.audioSource.Play();
                break;
            case (AttackType.FIRE):
                Sounds.audioSource.clip = Sounds.fire;
                Sounds.audioSource.Play();
                break;
            case (AttackType.WATER):
                Sounds.audioSource.clip = Sounds.ice;
                Sounds.audioSource.Play();
                break;
            case (AttackType.LIGHTNING):
                Sounds.audioSource.clip = Sounds.lightning;
                Sounds.audioSource.Play();
                break;
            case (AttackType.NONELEMENTAL):
                Sounds.audioSource.clip = Sounds.slash;
                Sounds.audioSource.Play();
                break;
        }

        return damageDealt;
    }

    public int CalcAttackDamage(Attack attack, BaseCharacter targetCharacter, BaseCharacter user, bool canCrit = false)
    {
        int damageDealt = 0;

        //Determines damage to deal by summing the Attack's damage and the user's Atk is attack is melee
        if (attack.isMelee)
        {
            damageDealt = attack.damage + user.currentATK;
        }
        else //Sums Attack's damage and the skill value if not melee. CHANGE THIS IF I ADD MAGIC
        {
            damageDealt = attack.damage + targetCharacter.currentSKL;
        }
        damageDealt = attack.damage + user.currentATK;

        if (user.weapon != null)
        {
            damageDealt += user.weapon.boostATK;
        }

        //If the enemy is weak to the Attack's type, damage is doubled.
        if (targetCharacter.weakness == attack.attackType)
        {
            damageDealt *= 2;
        }
        else if (targetCharacter.strength == attack.attackType)
        {
            damageDealt /= 2;
        }

        //damageDealt has enemy's Def subtracted
        if (attack.attackType != AttackType.HEALING)
        {
            damageDealt -= targetCharacter.currentDEF;

            // if the target is a hero, check his equipment
            if (BattleManager.hpm.activePartyMembers.Contains(targetCharacter))
            {
                EquipData armor = targetCharacter.armor;
                // Check for armor
                if (armor != null)
                {
                    Debug.Log("Armor resisted damage");
                    damageDealt -= armor.boostDEF;
                    if (armor.strongAgainst.Contains(attack.attackType))
                    {
                        damageDealt /= 2;
                    }
                }
            }

            if (targetCharacter.isDefending)
            {
                damageDealt /= 2;
            }
        }

        //The damageDealt is then given some variance in the range of +-damageDealt / 7.
        damageDealt += Random.Range(-damageDealt / 7, damageDealt / 7);

        //if damage is 0 or less, damageDealt defaults to 1.
        if (damageDealt < 1)
        {
            damageDealt = 1;
        }

        if (canCrit)
        {

            int rand = UnityEngine.Random.Range(0, 2);

            switch (rand)
            {
                case (0):
                    // Miss
                    return -1;
                default:
                    // Critical hit
                    damageDealt *= 2;
                    break;
            }
        }

        // If it's a healing move, damage becomes negative
        if (attack.attackType == AttackType.HEALING)
        {
            damageDealt *= -1;

            //If the target is dead and the healing move being used cannot revive, no healing is done.
            if (targetCharacter.CurrTurnState == TurnState.DEAD && !(attack.canRevive))
            {
                damageDealt = 0;
            }
            else if (targetCharacter.CurrTurnState != TurnState.DEAD && attack.canRevive)
            {
                // If a reviving action is used on a character who is alive, no healing is done.
                //damageDealt = 0;
            }
            else if (targetCharacter.CurrTurnState == TurnState.DEAD && attack.canRevive)
            {
                //When the target is revived, its state is changed from DEAD to WAITING which sets alive to true.
                targetCharacter.CurrTurnState = TurnState.WAITING;

                // TODO: Update health display
            }
        }
        return damageDealt;
    }

    protected void ReturnToMenu()
    {
        BattleManager.bManager.battleMenu.ReturnToMenu();
    }
}