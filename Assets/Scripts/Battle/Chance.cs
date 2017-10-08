using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chance : Effect {
    public float chance;
    public Effect effect;
    public override bool AppliesTo(BaseCharacter attacker, BaseCharacter target, int damageDealt, Attack attack)
    {
        return effect.AppliesTo(attacker, target, damageDealt, attack) && Random.value < chance;
    }
    public override void Apply(BaseCharacter attacker, BaseCharacter target, int damageDealt, Attack attack)
    {
        effect.Apply(attacker, target, damageDealt, attack);
    }
}
