using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounterManager : MonoBehaviour {

    static bool encountersEnabled;
    static int encounterStep = 16;

    public static RandomEncounterManager rem;

	// Use this for initialization
	void Start () {
        if (rem == null)
        {
            rem = this;
            encounterStep = UnityEngine.Random.Range(8, 24);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // returns true if a battle started
    public static bool AdvanceStepCount(char groundType)
    {
        switch (groundType)
        {
            case ('g'):
                encounterStep -= 1;
                break;
            case ('s'):
                encounterStep -= 1;
                break;
            case ('r'):
                encounterStep -= 2;
                break;
            default:
                encounterStep--;
                break;
        }
        if (encounterStep < 0)
        {
            encounterStep = UnityEngine.Random.Range(8, 24);
            BattleManager.bManager.StartBattle();
            return true;
        }
        return false;
    }
}
