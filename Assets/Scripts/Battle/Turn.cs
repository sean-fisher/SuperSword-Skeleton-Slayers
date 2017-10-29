using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Turn : IComparer<Turn> {

    public BaseCharacter attacker;
    public BaseCharacter target;

    public Attack attack;

    ItemData itemToUse;

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

    public Turn(BaseCharacter attacker, BaseCharacter target, ItemTurn itemTurn, ItemData itemToUse)
    {
        this.attacker = attacker;
        this.target = target;
        this.itemToUse = itemToUse;
    }

    public int Compare(Turn turnOne, Turn turnTwo)
    {
        return turnOne.attacker.currentEVA - turnTwo.attacker.currentEVA;
    }
}
