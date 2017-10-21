using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEncounters : MonoBehaviour {

    public List<EnemyPartyManager> grasslandParties;
    public List<EnemyPartyManager> mountainParties;
    public List<EnemyPartyManager> glacierParties;
    public List<EnemyPartyManager> volcanoParties;
    public List<EnemyPartyManager> desertParties;
    public List<EnemyPartyManager> forestParties;
    public List<EnemyPartyManager> iceCaveParties;
    public List<EnemyPartyManager> pyramidParties;
    public List<EnemyPartyManager> mountainCaveParties;
    public List<EnemyPartyManager> lavaCaveParties;

    public EnemyPartyManager bossParty;
    public EnemyPartyManager glacierEnemyChestEncounter;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    int tries = 100;

    public EnemyPartyManager GetRandomEncounter(ContinentType currArea)
    {

        List<EnemyPartyManager> currAreaList = null;
        switch (currArea)
        {
            case (ContinentType.GRASSLAND):
                currAreaList = forestParties;
                break;
            case (ContinentType.MOUNTAIN):
                currAreaList = mountainParties;
                break;
            case (ContinentType.GLACIER):
                currAreaList = glacierParties;
                break;
            case (ContinentType.DESERT):
                currAreaList = desertParties;
                break;
            case (ContinentType.VOLCANO):
                currAreaList = volcanoParties;
                break;
            case (ContinentType.FOREST):
                currAreaList = forestParties;
                break;
            case (ContinentType.MOUNTAINCAVE):
                currAreaList = mountainCaveParties;
                break;
            case (ContinentType.LAVACAVE):
                currAreaList = lavaCaveParties;
                break;
            case (ContinentType.PYRAMID):
                currAreaList = pyramidParties;
                break;
            case (ContinentType.ICECAVE):
                currAreaList = iceCaveParties;
                break;
            default:
                currAreaList = grasslandParties;
                break;
        }

        int randPartyIndex = UnityEngine.Random.Range(0, currAreaList.Count);
        int occurrenceChance = UnityEngine.Random.Range(1, 6);

        if (occurrenceChance <= currAreaList[randPartyIndex].occurrenceRate)
        {
            Debug.Log(currArea);
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

    public EnemyPartyManager GetChestEnemy(ContinentType whatRegion)
    {
        switch (whatRegion)
        {
            case (ContinentType.GLACIER):
                return glacierEnemyChestEncounter;
            default:
                return null;
        }
    }

    public EnemyPartyManager GetFinalBoss()
    {
        return bossParty;
    }
}