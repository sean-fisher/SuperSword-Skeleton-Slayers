using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : Attack {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Defend() { }

    public override IEnumerator UseAttack(BaseCharacter attacker, BaseCharacter target, List<Turn> turnList)
    {
        yield return null;

        EndTurnCheck(turnList);
    }
}
