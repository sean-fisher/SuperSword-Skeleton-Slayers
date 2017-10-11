using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChangeStat : Effect {
    public enum Stat
    {
        ATK, DEF, EVA, HP, MP, SKL
    }
    public Stat stat;
    public int amount;
    public bool appliesToAttackUser;
    public override void Apply(BaseCharacter attacker, BaseCharacter target, int damageDealt, Attack attack)
    {
        BaseCharacter statChange = (appliesToAttackUser ? attacker : target);
        if(stat == Stat.ATK)
                statChange.currentATK += amount;
        else if(stat == Stat.DEF)
            statChange.currentDEF += amount;
        else if (stat == Stat.EVA)
            statChange.currentEVA += amount;
        else if (stat == Stat.HP)
            statChange.currentHP += amount;
        else if (stat == Stat.MP)
            statChange.currentMP += amount;
        else if (stat == Stat.SKL)
            statChange.currentSKL += amount;
    }
}
