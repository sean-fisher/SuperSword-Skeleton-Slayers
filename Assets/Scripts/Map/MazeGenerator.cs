using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    public static MazeGenerator mazeGenerator;

    int mazeSize = 50;
    int mazeWidth = 64;
    int mazeHeight = 64;
    char[,] mazeArray;
    char[,] walkLevelArray;

    public GameObject moneyChest;
    public GameObject swordChest;
    public GameObject wallTile;

    public GameObject entrance;

    Vector2 enteredFrom;

    List<CoorPathLengthPair> branchPoints;

    Direction startPathDir;

    ContinentType continentType;

    List<GameObject> objectsToAdd;

	// Use this for initialization
	void Start () {
        mazeGenerator = this;
        //GenerateMaze(ContinentType.STARTING_AREA);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GenerateMaze(ContinentType continentType, Vector2 enteredFrom)
    {
        Debug.Log("Generate Maze");
        objectsToAdd = new List<GameObject>();
        this.continentType = continentType;
        this.enteredFrom = enteredFrom;

        switch (continentType)
        {
            case (ContinentType.STARTING_AREA):
                objectsToAdd.Add(moneyChest.gameObject);
                objectsToAdd.Add(swordChest.gameObject);
                break;
            default:
                Debug.Log("No continent typ provided");
                objectsToAdd.Add(moneyChest.gameObject);
                objectsToAdd.Add(swordChest.gameObject);
                break;
        }


        mazeArray = new char[mazeWidth, mazeHeight];
        walkLevelArray = new char[mazeWidth, mazeHeight];
        startPathDir = (Direction) Random.Range(0, 4);

        branchPoints = new List<CoorPathLengthPair>();

        BattleManager.hpm.MovePartyTo(new Vector2(
            transform.position.x + mazeWidth / 2 * 16, 
            transform.position.y + mazeWidth / 2 * -16));
        mazeArray[mazeWidth / 2, mazeHeight / 2] = 'p';
        walkLevelArray[mazeWidth / 2, mazeHeight / 2] = 'e';
        GeneratePathFrom(mazeWidth / 2, mazeHeight / 2, mazeSize, 'c');

        for (int i = 0; i < branchPoints.Count; i++)
        {
            GeneratePathFrom(branchPoints[i].coor.x, branchPoints[i].coor.y,
                branchPoints[i].pathLength);
        }

        StartCoroutine(InstantiateTiles());
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
                    'g'), transform);
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
                                objectsToAdd[0], transform);
                            objectsToAdd.RemoveAt(0);
                            tile.transform.position = new Vector2
                                (transform.position.x + 16 * x,
                                transform.position.y + -16 * y);
                        }
                    }
                if (walkLevelArray[x, y] == 'e')
                {
                    tile = GameObject.Instantiate(
                        entrance, transform);
                    tile.GetComponent<MapEntrance>().exitPosition = enteredFrom;
                    tile.transform.position = new Vector2
                        (transform.position.x + 16 * x,
                        transform.position.y + -16 * y);
                }
                else if (mazeArray[x, y] != 'p')
                    {
                    tile = GameObject.Instantiate(wallTile, transform);
                        tile.transform.position = new Vector2
                            (transform.position.x + 16 * x,
                            transform.position.y + -16 * y);
                    }
               // }
            }
            yield return null;
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