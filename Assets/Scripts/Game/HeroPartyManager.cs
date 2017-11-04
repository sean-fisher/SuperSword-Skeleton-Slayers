using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPartyManager : PartyManager {

    public BaseCharacter leader;

    public GameObject knightPrefab;
    public GameObject magePrefab;
    public GameObject archerPrefab;
    public GameObject monkPrefab;
    public GameObject ninjaPrefab;
    public GameObject chefPrefab;

    public GameObject actPartyObj;
    public GameObject inactPartyObj;

    public CamFollow camFollower;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddCharacterToParty((GameObject.Instantiate
                (knightPrefab)).GetComponent<BaseCharacter>());
        } else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            AddCharacterToParty(GameObject.Instantiate
                (magePrefab).GetComponent<BaseCharacter>());
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            AddCharacterToParty(GameObject.Instantiate
                (archerPrefab).GetComponent<BaseCharacter>());
        }
    }

    public void MovePartyTo(Vector2 destination)
    {
        for (int i = 0; i < activePartyMembers.Count; i++)
        {
            activePartyMembers[i].transform.position = new Vector2(
                destination.x, destination.y + 4);
        }
    }

    public void DisablePartyMovement()
    {
        for (int i = 0; i < activePartyMembers.Count; i++) {
            activePartyMembers[i].GetComponent<GridController>().DisableMovement();
        }
    }

    public void EnablePartyMovement()
    {
        for (int i = 0; i < activePartyMembers.Count; i++)
        {
            activePartyMembers[i].GetComponent<GridController>().EnableMovement();
        }
    }

    public void AddKnight(Vector2 knightCoor)
    {
        GameObject knightObj = GameObject.Instantiate(knightPrefab);
        AddCharacterToParty(knightObj.GetComponent<BaseCharacter>());
        knightObj.GetComponent<GridController>().isLeader = true;
        knightObj.transform.position = new Vector2(
            knightCoor.x * 16, knightCoor.y * -16 + 4);
        knightObj.GetComponent<GridController>().SetCoor(new MapCoor(
            (int)knightCoor.x, (int)knightCoor.y));
    }

    public void InstantiateAndAdd(BaseCharacter characterPrefab)
    {

        GameObject heroObj = GameObject.Instantiate(characterPrefab.gameObject);
        AddCharacterToParty(heroObj.GetComponent<BaseCharacter>());
        Vector3 partyPos = GameManager.gm.leader.transform.position;
        heroObj.GetComponent<GridController>().isLeader = true;
        heroObj.GetComponent<GridController>().SetCoor(new MapCoor(
            (int)partyPos.x, (int)partyPos.y));

    }

    public void AddCharacterToActive(BaseCharacter newHero)
    {
        // This check is redundant if adding from 
        // AddCharacterToParty(BaseCharacter newhero)
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
                newHero.GetComponent<GridController>().placeInParty = 
                    activePartyMembers.Count - 1;
            }
            newHero.gameObject.transform.parent = actPartyObj.transform;

            newHero.GetComponent<GridController>().debugEnabled = 
                GameManager.gm.debugEnabled;
            Debug.Log("Added Hero: " + newHero.name);
            newHero.transform.position = leader.transform.position;
            newHero.GetComponent<GridController>().SetCoor(
                leader.GetComponent<GridController>().GetCoor());
                /*new MapCoor(
                (int)newHero.transform.position.x / 16, 
                (int)newHero.transform.position.y / 16));*/
            BattleManager.bManager.heroesAlive++;
        } else
        {
            Debug.Log("Active Party Full");
        }
    }

    public void ClearMoveQueues()
    {
        for (int i = 0; i < activePartyMembers.Count; i++)
        {
            activePartyMembers[i].GetComponent<GridController>().inputList.Clear();
        }
    }

    public void AddCharacterToParty(BaseCharacter newHero)
    {
        if (!inactivePartyMembers.Contains(newHero) && 
            !activePartyMembers.Contains(newHero))
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
