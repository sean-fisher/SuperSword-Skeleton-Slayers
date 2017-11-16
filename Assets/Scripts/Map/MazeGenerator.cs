using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    public static MazeGenerator mazeGenerator;
    public static bool inMaze;
    public static char groundType;

    public GameObject currMazeParent;

    public GameObject forestMazeParent;
    public GameObject mountainCaveParent;
    public GameObject iceCaveParent;
    public GameObject pyramidParent;
    public GameObject lavaCaveParent;

    int mazeSize = 40;
    int mazeWidth = 64;
    int mazeHeight = 64;
    char[,] mazeArray;
    char[,] walkLevelArray;

    public GameObject moneyChest;
    public GameObject enemyOrTreasureChest;
    public GameObject treasureChest;
    public GameObject lockedChest;
    public GameObject swordChest;
    public GameObject forestWallTile;
    public GameObject iceWallTile;
    public GameObject pyramidWallTile;
    public GameObject mountainWallTile;
    public GameObject lavaWallTile;

    public GameObject betterEquipChest;
    public GameObject equipChest;

    public PartyJoiner archer;
    public PartyJoiner mage;

    GameObject wallTile;

    public GameObject entrance;

    public TreasureChest keyChest;

    Vector2 enteredFrom;

    List<CoorPathLengthPair> branchPoints;

    Direction startPathDir;

    List<GameObject> objectsToAdd;

    ContinentType currMaze;

	// Use this for initialization
	void Start () {
        mazeGenerator = this;
        //GenerateMaze(ContinentType.STARTING_AREA);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnableMazeParent(ContinentType mazeToEnter, bool disableIfFalse)
    {
        switch (mazeToEnter)
        {
            case (ContinentType.FOREST):
                currMazeParent = forestMazeParent;
                forestMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.ICECAVE):
                currMazeParent = iceCaveParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.PYRAMID):
                currMazeParent = pyramidParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.MOUNTAINCAVE):
                currMazeParent = mountainCaveParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.LAVACAVE):
                currMazeParent = lavaCaveParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.GRASSLAND):
                currMazeParent = forestMazeParent;
                forestMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.DESERT):
                currMazeParent = pyramidParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.GLACIER):
                currMazeParent = iceCaveParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.MOUNTAIN):
                currMazeParent = mountainCaveParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
            case (ContinentType.VOLCANO):
                currMazeParent = lavaCaveParent;
                currMazeParent.SetActive(disableIfFalse);
                break;
        }

    }

    public static void SetGroundType(ContinentType currArea, bool isExiting)
    {
        switch (currArea)
        {
            case (ContinentType.MOUNTAINCAVE):
                groundType = 'r';
                break;
            case (ContinentType.FOREST):
                groundType = 'g';
                break;
            case (ContinentType.ICECAVE):
                groundType = 'h';
                break;
            case (ContinentType.LAVACAVE):
                groundType = 'o';
                break;
            case (ContinentType.PYRAMID):
                groundType = 's';
                break;
            default:
                groundType = 'g';
                break;
        }
        RandomEncounterManager.currArea = currArea;
        mazeGenerator.currMaze = currArea;
        mazeGenerator.EnableMazeParent(currArea, !isExiting);
    }

    public void GenerateMaze(ContinentType continentType, Vector2 enteredFrom)
    {
        objectsToAdd = new List<GameObject>();

        this.enteredFrom = enteredFrom;

        switch (continentType)
        {
            case (ContinentType.FOREST):
                Debug.Log("Added forest objects");
                objectsToAdd.Add(moneyChest);
                objectsToAdd.Add(treasureChest);
                objectsToAdd.Add(archer.gameObject);
                objectsToAdd.Add(keyChest.gameObject);
                objectsToAdd.Add(swordChest);
                groundType = 'g';
                wallTile = forestWallTile;
                currMazeParent = forestMazeParent;
                break;
            case (ContinentType.ICECAVE):
                objectsToAdd.Add(treasureChest);
                objectsToAdd.Add(mage.gameObject);
                objectsToAdd.Add(lockedChest);
                objectsToAdd.Add(keyChest.gameObject);
                objectsToAdd.Add(swordChest);
                groundType = 'h';
                wallTile = iceWallTile;
                currMazeParent = iceCaveParent;
                break;
            case (ContinentType.PYRAMID):
                objectsToAdd.Add(treasureChest);
                objectsToAdd.Add(enemyOrTreasureChest);
                objectsToAdd.Add(treasureChest);
                objectsToAdd.Add(keyChest.gameObject);
                groundType = 's';
                wallTile = pyramidWallTile;
                currMazeParent = pyramidParent;
                break;
            case (ContinentType.MOUNTAINCAVE):
                objectsToAdd.Add(treasureChest);
                objectsToAdd.Add(lockedChest);
                objectsToAdd.Add(swordChest);
                objectsToAdd.Add(betterEquipChest);
                objectsToAdd.Add(keyChest.gameObject);
                groundType = 's';
                wallTile = mountainWallTile;
                currMazeParent = mountainCaveParent;
                break;
            case (ContinentType.LAVACAVE):
                objectsToAdd.Add(treasureChest);
                objectsToAdd.Add(equipChest);
                objectsToAdd.Add(betterEquipChest);
                objectsToAdd.Add(keyChest.gameObject);
                objectsToAdd.Add(moneyChest);
                groundType = 'r';
                wallTile = lavaWallTile;
                currMazeParent = lavaCaveParent;
                break;
            default:
                Debug.Log("Invalid continent type provided");
                objectsToAdd.Add(moneyChest);
                objectsToAdd.Add(swordChest);
                break;
        }
        RandomEncounterManager.currArea = continentType;


        mazeArray = new char[mazeWidth, mazeHeight];
        walkLevelArray = new char[mazeWidth, mazeHeight];
        startPathDir = (Direction) Random.Range(0, 4);

        branchPoints = new List<CoorPathLengthPair>();

        mazeArray[mazeWidth / 2, mazeHeight / 2] = 'p';
        walkLevelArray[mazeWidth / 2, mazeHeight / 2] = 'e';
        GeneratePathFrom(mazeWidth / 2, mazeHeight / 2, mazeSize, 'c');

        for (int i = 0; i < branchPoints.Count; i++)
        {
            GeneratePathFrom(branchPoints[i].coor.x, branchPoints[i].coor.y,
                branchPoints[i].pathLength, 'c');
        }

        StartCoroutine(InstantiateTiles());
    }

    public void PlacePartyAtMazeEntrance(ContinentType typeOfMaze)
    {
        BattleManager.hpm.MovePartyTo(new Vector2(
            transform.position.x + mazeWidth / 2 * 16,
            transform.position.y + mazeWidth / 2 * -16));
    }

    void GeneratePathFrom(int x, int y, int pathLength, char endItem = '\0')
    {
        int branchCounter = 3;
        int sameDirLength = Random.Range(0, 5);
        while (pathLength > 0)
        {
            if (TestAdjacent(new MapCoor(x, y), startPathDir))
            {
                mazeArray[Mathf.Abs(testAdjacent.x % mazeWidth), 
                    Mathf.Abs(testAdjacent.y % mazeHeight)] = 'p';

                if (--branchCounter < 0)
                {
                    branchPoints.Add(new CoorPathLengthPair(new MapCoor(
                        testAdjacent.x, testAdjacent.y), pathLength / 2));
                    branchCounter = Random.Range(2, 4);
                }
            }

            if (--sameDirLength < 0)
            {
                startPathDir = (Direction)Random.Range(0, 4);
                sameDirLength = Random.Range(0, 5);
            }


            x = testAdjacent.x;
            y = testAdjacent.y;
            pathLength--;
        }

        if (walkLevelArray[Mathf.Abs(testAdjacent.x % mazeWidth),
                    Mathf.Abs(testAdjacent.y % mazeHeight)] == '\0')
        {
            walkLevelArray[Mathf.Abs(testAdjacent.x % mazeWidth),
                        Mathf.Abs(testAdjacent.y % mazeHeight)] = endItem;
            if (endItem == 'c')
            {
                // Debug.Log("good " + testAdjacent.x + " " + testAdjacent.y);
            }
        }
        if (endItem == 'c')
        {
            // Debug.Log("bad " + testAdjacent.x + " " + testAdjacent.y);
        }
    }

    MapCoor testAdjacent;

    int errorCheck = 8;
    bool TestAdjacent(MapCoor current, Direction direction)
    {
        MapCoor possiblePathPoint = GetAdjacent(current, startPathDir);
        if (mazeArray[Mathf.Abs(possiblePathPoint.x % mazeWidth), Mathf.Abs(possiblePathPoint.y % mazeHeight)] == '\0')
        {
            testAdjacent = possiblePathPoint;
            return true;
        } else
        {
            // the point already has a path on it
            if (--errorCheck > 0)
            {
                errorCheck = 8;
                errors++;
                return false;
            }
            return TestAdjacent(current, direction);
        }
    }

    int errors = 0;

    MapCoor GetAdjacent(MapCoor current, Direction direction)
    {

        switch (direction) {
            case (Direction.LEFT):
                current.x--;
                break;
            case (Direction.RIGHT):
                current.x++;
                break;
            case (Direction.DOWN):
                current.y--;
                break;
            case (Direction.UP):
                current.y++;
                break;
        }

        return current;
    }

    IEnumerator InstantiateTiles()
    {
        if (MapGenerator.mg == null)
        {
            MapGenerator.mg = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        }

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                GameObject tile = null;
                
                tile = GameObject.Instantiate(MapGenerator.mg.GetTileObj(
                    groundType), currMazeParent.transform);
                tile.transform.position = new Vector2
                    (transform.position.x + 16 * x,
                    transform.position.y + -16 * y);

                /*
                if (mazeArray[x, y] != 'p')
                {*/
                if (walkLevelArray[x, y] == 'c')
                {
                    if (objectsToAdd.Count > 0 && objectsToAdd[0] != null)
                    {
                        tile = GameObject.Instantiate(
                            objectsToAdd[0], currMazeParent.transform);
                        objectsToAdd.RemoveAt(0);
                        tile.transform.position = new Vector2
                            (transform.position.x + 16 * x,
                            transform.position.y + -16 * y);
                 
                    } else
                    {
                    }
                }
                if (walkLevelArray[x, y] == 'e') // PLACE THE EXIT
                {
                    tile = GameObject.Instantiate(
                        entrance, currMazeParent.transform);
                    tile.GetComponent<MapEntrance>().exitPosition = enteredFrom;
                    tile.transform.position = new Vector2
                        (transform.position.x + 16 * x,
                        transform.position.y + -16 * y);
                    tile.GetComponent<MapEntrance>().mazeToGenerate 
                        = GetContinentCorrespondingToMazeType(currMaze); 
                }
                else if (mazeArray[x, y] != 'p')
                    {
                    tile = GameObject.Instantiate(wallTile, currMazeParent.transform);
                        tile.transform.position = new Vector2
                            (transform.position.x + 16 * x,
                            transform.position.y + -16 * y);
                    }
               // }
            }
            yield return null;
        }
    }

    ContinentType GetContinentCorrespondingToMazeType(ContinentType mazeType)
    {
        switch (mazeType)
        {
            case (ContinentType.FOREST):
                return ContinentType.GRASSLAND;
            case (ContinentType.ICECAVE):
                return ContinentType.GLACIER;
            case (ContinentType.LAVACAVE):
                return ContinentType.VOLCANO;
            case (ContinentType.MOUNTAINCAVE):
                return ContinentType.MOUNTAIN;
            case (ContinentType.PYRAMID):
                return ContinentType.DESERT;
            default:
                throw new System.Exception("Invalid maze exit type?");
        }
    }

    struct CoorPathLengthPair
    {
        public MapCoor coor;
        public int pathLength;

        public CoorPathLengthPair(MapCoor coor, int pathLength)
        {
            this.coor = coor;
            this.pathLength = pathLength;
        }
    }
}
public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}