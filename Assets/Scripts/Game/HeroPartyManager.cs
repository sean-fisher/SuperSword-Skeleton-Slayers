using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPartyManager : PartyManager {

    public BaseCharacter leader;

    public GameObject knightPrefab;
    public GameObject magePrefab;
    public GameObject monkPrefab;
    public GameObject ninjaPrefab;
    public GameObject chefPrefab;

    public GameObject actPartyObj;
    public GameObject inactPartyObj;

    public CamFollow camFollower;

    public static List<BaseCharacter> activeHeroes;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddCharacterToParty((GameObject.Instantiate(knightPrefab)).GetComponent<BaseCharacter>());
        } else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            AddCharacterToParty(GameObject.Instantiate(knightPrefab).GetComponent<BaseCharacter>());
        }
	}

    public new void KillCharacter(BaseCharacter targetCharacter)
    {

    }

    public void AddKnight(Vector3 knightPosition)
    {
        GameObject knightObj = GameObject.Instantiate(knightPrefab);
        AddCharacterToParty(knightObj.GetComponent<BaseCharacter>());
        knightObj.transform.position = new Vector2(knightPosition.x * 16, knightPosition.y * -16 + 4);
    }

    public void AddCharacterToActive(BaseCharacter newHero)
    {
        // This check is redundant if adding from AddCharacterToParty(BaseCharacter newhero)
        if (activePartyMembers.Count < 4 && !activePartyMembers.Contains(newHero))
        {
            activePartyMembers.Add(newHero);

            if (activePartyMembers.Count == 1)
            {
                GameManager.gm.leader = newHero.GetComponent<GridController>();
                leader = newHero;
                newHero.gameObject.transform.position = Vector2.zero;
                camFollower.targetToFollow = leader.gameObject.transform;
            } else
            {
                newHero.gameObject.transform.position = leader.transform.position;
                newHero.GetComponent<GridController>().placeInParty = activePartyMembers.Count - 1;
            }
            newHero.gameObject.transform.parent = actPartyObj.transform;

            newHero.GetComponent<GridController>().debugEnabled = GameManager.gm.debugEnabled;
            Debug.Log("Added Hero: " + newHero.name);
            newHero.transform.position = leader.transform.position;
            BattleManager.bManager.heroesAlive++;
        } else
        {
            Debug.Log("Active Party Full");
        }
    }

    public void AddCharacterToParty(BaseCharacter newHero)
    {
        if (!inactivePartyMembers.Contains(newHero) && !activePartyMembers.Contains(newHero))
        {
            if (activePartyMembers.Count < 4)
            {
                AddCharacterToActive(newHero);
            }
            else
            {
                inactivePartyMembers.Add(newHero);
                newHero.gameObject.transform.parent = inactPartyObj.transform;
            }
        }
    }
}
