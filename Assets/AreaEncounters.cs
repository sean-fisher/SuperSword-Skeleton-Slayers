using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEncounters : MonoBehaviour {

    public List<EnemyPartyManager> grasslandParties;
    public List<EnemyPartyManager> mountainParties;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    int tries = 100;

    public EnemyPartyManager GetRandomEncounter(AreaNames currArea)
    {
        List<EnemyPartyManager> currAreaList = null;
        switch (currArea)
        {
            case (AreaNames.GRASSLAND):
                currAreaList = grasslandParties;
                break;
            case (AreaNames.MOUNTAIN):
                currAreaList = mountainParties;
                break;
        }

        int randPartyIndex = UnityEngine.Random.Range(0, currAreaList.Count);
        int occurrenceChance = UnityEngine.Random.Range(1, 5);


        if (occurrenceChance <= currAreaList[randPartyIndex].occurrenceRate)
        {
            return currAreaList[randPartyIndex];
        }
        else
        {
            if (tries-- < 0)
            {
                throw new System.Exception("Invalid party Occurrence rate!");
            }
            return GetRandomEncounter(currArea);
        }
    }
}

public enum AreaNames
{
    GRASSLAND,
    MOUNTAIN
}
