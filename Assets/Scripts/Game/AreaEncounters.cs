using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEncounters : MonoBehaviour {

    public static AreaEncounters aeinstance;

    public Transform grasslandParties;
    public Transform mountainParties;
    public Transform glacierParties;
    public Transform volcanoParties;
    public Transform desertParties;
    public Transform forestParties;
    public Transform iceCaveParties;
    public Transform pyramidParties;
    public Transform mountainCaveParties;
    public Transform lavaCaveParties;

    public EnemyPartyManager bossParty;
    public EnemyPartyManager glacierEnemyChestEncounter;

    public Sprite grasslandBackground;
    public Sprite forestBackground;
    public Sprite glacierBackground;
    public Sprite icecaveBackground;
    public Sprite desertBackground;
    public Sprite pyramidBackground;
    public Sprite mountainBackground;
    public Sprite caveBackground;
    public Sprite volcanoBackground;
    public Sprite lavacaveBackground;

    public static Sprite currBackground;

    EnemyPartyManager[] grasslandPartiesList;
    EnemyPartyManager[] mountainPartiesList;
    EnemyPartyManager[] glacierPartiesList;
    EnemyPartyManager[] volcanoPartiesList;
    EnemyPartyManager[] desertPartiesList;
    EnemyPartyManager[] forestPartiesList;
    EnemyPartyManager[] iceCavePartiesList;
    EnemyPartyManager[] pyramidPartiesList;
    EnemyPartyManager[] mountainCavePartiesList;
    EnemyPartyManager[] lavaCavePartiesList;


    // Use this for initialization
    void Start () {
        if (aeinstance == null)
        {
            aeinstance = this;
        }
        grasslandPartiesList = grasslandParties.GetComponentsInChildren<EnemyPartyManager>();
        mountainPartiesList = mountainParties.GetComponentsInChildren<EnemyPartyManager>();
        glacierPartiesList = glacierParties.GetComponentsInChildren<EnemyPartyManager>();
        volcanoPartiesList = volcanoParties.GetComponentsInChildren<EnemyPartyManager>();
        desertPartiesList = desertParties.GetComponentsInChildren<EnemyPartyManager>();
        forestPartiesList = forestParties.GetComponentsInChildren<EnemyPartyManager>();
        iceCavePartiesList = iceCaveParties.GetComponentsInChildren<EnemyPartyManager>();
        pyramidPartiesList = pyramidParties.GetComponentsInChildren<EnemyPartyManager>();
        mountainCavePartiesList = mountainCaveParties.GetComponentsInChildren<EnemyPartyManager>();
        lavaCavePartiesList= lavaCaveParties.GetComponentsInChildren<EnemyPartyManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    int tries = 100;
    static EnemyPartyManager[] currAreaList = null;

    public EnemyPartyManager GetRandomEncounter(ContinentType currArea)
    {
        Debug.Log("Encounter" + currArea);
        switch (currArea)
        {
            case (ContinentType.GRASSLAND):
                currAreaList = grasslandPartiesList;
                currBackground = grasslandBackground;
                break;
            case (ContinentType.MOUNTAIN):
                currAreaList = mountainPartiesList;
                currBackground = caveBackground;
                break;
            case (ContinentType.GLACIER):
                currAreaList = glacierPartiesList;
                currBackground = glacierBackground;
                break;
            case (ContinentType.DESERT):
                currAreaList = desertPartiesList;
                currBackground = desertBackground;
                break;
            case (ContinentType.VOLCANO):
                currAreaList = volcanoPartiesList;
                currBackground = volcanoBackground;
                break;
            case (ContinentType.FOREST):
                currAreaList = forestPartiesList;
                currBackground = forestBackground;
                break;
            case (ContinentType.MOUNTAINCAVE):
                currAreaList = mountainCavePartiesList;
                currBackground = caveBackground;
                break;
            case (ContinentType.LAVACAVE):
                currAreaList = lavaCavePartiesList;
                currBackground = lavacaveBackground;
                break;
            case (ContinentType.PYRAMID):
                currAreaList = pyramidPartiesList;
                currBackground = pyramidBackground;
                break;
            case (ContinentType.ICECAVE):
                currAreaList = iceCavePartiesList;
                currBackground = icecaveBackground;
                break;
            default:
                currAreaList = grasslandPartiesList;
                currBackground = grasslandBackground;
                break;
        }

        int randPartyIndex = UnityEngine.Random.Range(0, currAreaList.Length);
        int occurrenceChance = UnityEngine.Random.Range(1, 6);

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

    public void SetCurrArea(ContinentType currArea)
    {
        switch (currArea)
        {
            case (ContinentType.GRASSLAND):
                currAreaList = grasslandPartiesList;
                currBackground = grasslandBackground;
                break;
            case (ContinentType.MOUNTAIN):
                currAreaList = mountainPartiesList;
                currBackground = caveBackground;
                break;
            case (ContinentType.GLACIER):
                currAreaList = glacierPartiesList;
                currBackground = glacierBackground;
                break;
            case (ContinentType.DESERT):
                currAreaList = desertPartiesList;
                currBackground = desertBackground;
                break;
            case (ContinentType.VOLCANO):
                currAreaList = volcanoPartiesList;
                currBackground = volcanoBackground;
                break;
            case (ContinentType.FOREST):
                currAreaList = forestPartiesList;
                currBackground = forestBackground;
                break;
            case (ContinentType.MOUNTAINCAVE):
                currAreaList = mountainCavePartiesList;
                currBackground = caveBackground;
                break;
            case (ContinentType.LAVACAVE):
                currAreaList = lavaCavePartiesList;
                currBackground = lavacaveBackground;
                break;
            case (ContinentType.PYRAMID):
                currAreaList = pyramidPartiesList;
                currBackground = pyramidBackground;
                break;
            case (ContinentType.ICECAVE):
                currAreaList = iceCavePartiesList;
                currBackground = icecaveBackground;
                break;
            default:
                currAreaList = grasslandPartiesList;
                currBackground = grasslandBackground;
                break;
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