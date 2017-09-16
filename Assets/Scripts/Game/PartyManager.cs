using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour {

    // Used for enemy or hero party

    public List<BaseCharacter> activePartyMembers;
    public List<BaseCharacter> inactivePartyMembers;

    public Inventory inventory;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void KillCharacter(BaseCharacter targetCharacter)
    {

    }

    public bool PartyContainsCharacter(BaseCharacter character)
    {
        for (int i = 0; i < activePartyMembers.Count; i++)
        {
            if (activePartyMembers[i].characterName == character.characterName)
            {
                return true;
            }
        }
        return false;
    }
}
