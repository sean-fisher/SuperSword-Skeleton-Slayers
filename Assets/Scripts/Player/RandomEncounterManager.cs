using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounterManager : MonoBehaviour {

    public bool enableEncounters;
    static bool encountersEnabled;
    static int encounterStep = 16;

    public static RandomEncounterManager rem;
    public static ContinentType currArea;

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
            ContinentType currArea = ContinentType.GRASSLAND;
            switch (groundType)
            {
                case ('g'):
                    encounterStep -= 1;
                    break;
                case ('s'):
                    encounterStep -= 1;
                    currArea = ContinentType.DESERT;
                    break;
                case ('r'):
                    encounterStep -= 2;
                    currArea = ContinentType.GRASSLAND;
                    break;
                case ('\0'):
                    encounterStep -= 1;
                    currArea = ContinentType.OCEAN;
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

    public static bool AdvanceStepCount(Vector2 currPos)
    {

        MapCoor currCoor = new MapCoor((int)currPos.x / 16, (int)currPos.y / 16);

        if (encountersEnabled)
        {
            char groundType = 'g';
            ContinentType currArea = ContinentType.GRASSLAND;

            if (MazeGenerator.inMaze)
            {
                groundType = MazeGenerator.groundType;

                switch (groundType)
                {
                    case ('g'):
                        encounterStep -= 1;
                        currArea = ContinentType.FOREST;
                        break;
                    case ('\0'):
                        encounterStep -= 1;
                        currArea = ContinentType.OCEAN;
                        break;
                    default:
                        encounterStep--;
                        currArea = ContinentType.DESERT;
                        break;
                }
            } else
            {
                currCoor.x %= MapGenerator.mapWidth;
                currCoor.y %= MapGenerator.mapHeight;
                currCoor.y = Mathf.Abs(currCoor.y) + 1;

                groundType = MapGenerator.mg.GetTile(currCoor, true);

                switch (groundType)
                {
                    case ('g'):
                        encounterStep -= 1;
                        break;
                    case ('s'):
                        encounterStep -= 1;
                        currArea = ContinentType.DESERT;
                        break;
                    case ('r'):
                        encounterStep -= 2;
                        currArea = ContinentType.GRASSLAND;
                        break;
                    case ('\0'):
                        encounterStep -= 1;
                        currArea = ContinentType.OCEAN;
                        break;
                    case ('o'):
                        encounterStep -= 1;
                        currArea = ContinentType.VOLCANO;
                        break;
                    default:
                        encounterStep--;
                        break;
                }
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

    public static void SetCurrArea(char groundChar)
    {

        switch (groundChar)
        {
            case ('g'):
                currArea = ContinentType.GRASSLAND;
                break;
            case ('h'):
                currArea = ContinentType.GLACIER;
                break;
            case ('b'):
                currArea = ContinentType.GLACIER;
                break;
            case ('o'):
                currArea = ContinentType.VOLCANO;
                break;
            case ('s'):
                currArea = ContinentType.DESERT;
                break;
            case ('d'):
                currArea = ContinentType.DESERT;
                break;
            case ('r'):
                currArea = ContinentType.MOUNTAIN;
                break;
            case ('\0'):
                currArea = ContinentType.OCEAN;
                break;
            default:
                break;
        }
    }
}
