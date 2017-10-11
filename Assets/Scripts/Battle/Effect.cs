using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour {
    public string display;
    public virtual bool AppliesTo(BaseCharacter attacker, BaseCharacter target, int damageDealt, Attack attack)
    {
        return true;
    }
    public abstract void Apply(BaseCharacter attacker, BaseCharacter target, int damageDealt, Attack attack);

    // Use this for initialization
    void Start () {}
	
	// Update is called once per frame
	void Update () {}
}
