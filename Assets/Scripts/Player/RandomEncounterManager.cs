using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounterManager : MonoBehaviour {

    public bool enableEncounters;
    static bool encountersEnabled;
    static int encounterStep = 16;

    public static RandomEncounterManager rem;

	// Use this for initialization
	void Start () {
        encountersEnabled = enableEncounters;
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
        if (encountersEnabled) {
            AreaNames currArea = AreaNames.GRASSLAND;
            switch (groundType)
            {
                case ('g'):
                    encounterStep -= 1;
                    break;
                case ('s'):
                    encounterStep -= 1;
                    currArea = AreaNames.DESERT;
                    break;
                case ('r'):
                    encounterStep -= 2;
                    currArea = AreaNames.GRASSLAND;
                    break;
                case ('\0'):
                    encounterStep -= 1;
                    currArea = AreaNames.OCEAN;
                    break;
                default:
                    encounterStep--;
                    break;
            }
            GameManager.currAreaName = currArea;
            if (encounterStep < 0)
            {
                encounterStep = UnityEngine.Random.Range(8, 24);
                BattleManager.bManager.StartBattle();
                return true;
            }
            return false;
        }
        return false;
    }
}
