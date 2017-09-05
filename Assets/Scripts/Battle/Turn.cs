using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Turn {

    public BaseCharacter attacker;
    public BaseCharacter target;

    public Attack attack;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Turn(BaseCharacter attacker, BaseCharacter target, Attack attack)
    {
        this.attacker = attacker;
        this.target = target;
        this.attack = attack;
    }
}
