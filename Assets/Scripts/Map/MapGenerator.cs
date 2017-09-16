using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    

    public static MapGenerator mg;

    [Header("Ground Tiles")]                // CHARACTER GUIDE: each tile is represented by a character
    public GameObject grassTile;            // g
    public GameObject hillTile;             // h
    public GameObject rockTile;             // r
    public GameObject sandTile;             // s
    public GameObject sandTileDark;         // d
    public GameObject waterTile;            // w or '\0' (Null character)
    public GameObject testTile;             // t
                                            // 
    [Header("Interactable Tiles")]          // 
    public GameObject airship;              // a
    public GameObject mountainTile;         // m
    public GameObject doorTile;             // d
    public GameObject forestTile;           // f

                                            // EACH TILE TYPE MUST HAVE:
                                            //  Appropriate GameObject GetTileObj(...) code
                                            // CreateContinent(...) and CharIsGround(...) code if it is a ground type



    // 2D character array represents the grid of tiles that comprises the world map (Grass, sand, etc.) 
    char[,] groundGrid;
    // 2D character array represents the grid of tiles that exists above the ground (Trees, mountains, doors, etc.)
    char[,] walkLevelGrid;

    static bool mapGenerated = false;

    // Used to make sure continents don't overlap
    List<CenterRadiusPair> continents = new List<CenterRadiusPair>();

    int mapWidth = 0;
    int mapHeight = 0;

    enum ContinentType
    {
        None,
        // Different continent types will have different enemy types, geographical features, art, etc.

        STARTING_AREA,      // Grass, coast, forests, some mountains, airship or tunnel (to be implemented) can be found
        MOUNTAINOUS         // Surrounded by mountans, only accessible 
    }

    void Start()
    {
        if (mg == null)
        {
            mg = this;
        }
        if (!mapGenerated)
        {
            StartCoroutine(WaitThenGenerate());
        }
    }

    // If GenerateMap is called during start(), the GameManager.gm may not be set, as that is also set during start
    IEnumerator WaitThenGenerate()
    {
        yield return null;
        GenerateMap(128, 100, 5, 1.8f);
    }

    void GenerateMap(int width, int height, int numContinents, float continentSize)
    {
        groundGrid = new char[width, height];
        walkLevelGrid = new char[width, height];
        mapHeight = height;
        mapWidth = width;

        // A map must always have a starting zone, but the other continents are mostly random.
        // 
        ContinentType[] cTypes = new ContinentType[numContinents];
        int rand = UnityEngine.Random.Range(0, numContinents);
        cTypes[rand] = ContinentType.STARTING_AREA;

        // Make sure the continents are varied; 
        // if there is already a mountain, for example, a different type will be picked unless all types have been picked
        bool isMountain = false;

        for (int i = 1; i < numContinents; i++)
        {
            int randContinent = UnityEngine.Random.Range(0, numContinents);
            while (cTypes[randContinent] != ContinentType.None)
            {
                randContinent = UnityEngine.Random.Range(0, numContinents);
            }

            if (!isMountain)
            {
                cTypes[randContinent] = ContinentType.MOUNTAINOUS;
                isMountain = true;
            } else
            {
                cTypes[randContinent] = (ContinentType)Random.Range(1, 3);
            }
        }

        MapCoor mapCenter = new MapCoor(mapWidth / 2, mapHeight / 2);
        float fromMapCenterRadius = mapWidth / 3;
        float defaultRadius = fromMapCenterRadius;
        float theta = 0;
        float ratio = (float) mapWidth / mapHeight;

        for (int continent = 0; continent < numContinents - 0; continent++)
        {
            MapCoor continentStartPos = new MapCoor((int)(mapCenter.x + fromMapCenterRadius * Mathf.Cos(theta)), (int)(mapCenter.y + fromMapCenterRadius * Mathf.Sin(theta)));
            //Debug.Log(string.Format("Type: {3}, Radius = {0}, theta = {1}, Center = {2}", fromMapCenterRadius, theta, continentStartPos, cTypes[continent]));
            
            // Continents are created in a circle around the center of the map
            /**EXAMPLE:
             *                   WIDTH              numContinents = 3
             *        _____________________
             *  H    |                     |
             *  E    |     C3      C2      |
             *  I    |                     |
             *  G    |                C1   |
             *  H    |    C4               |
             *  T    |           C5        |
             *       |_____________________|
             *       
             *      
             *      
             * */
             
            int circleRadius = (int)(ratio * continentSize * 2.5f);


            // A continent is formed by several circles of varying size built off of each other.
            CreateContinent(continentStartPos.ToVector(), circleRadius, 8, cTypes[continent]);

            // Calculations for next circle
            theta += 6.28f / (float)(numContinents);
            fromMapCenterRadius = defaultRadius - Mathf.Abs(Mathf.Sin(theta) * defaultRadius * (1 - 1 / ratio));
        }

        BuildCoast();

        StartCoroutine(InstantiateTiles());
    }

    List<Vector2> continentCenters = new List<Vector2>();

    void CreateContinent(Vector2 center, int largestRadius, int complexity, ContinentType continentType, bool hasCoast = true)
    {
        Vector2 newCenter = center;
        Vector2 tempCenter = center;
        float curveAngle = 0;

        // The continents generally twist inwards
        // if the center is high on the map
        if (center.y > groundGrid.GetLength(1) / 2)
        {
            curveAngle = 270 + UnityEngine.Random.Range(-3.14f / 2, 3.14f / 2);
        } else
        {
            //if the center is low on the map
            curveAngle = 90 + UnityEngine.Random.Range(-3.14f / 2, 3.14f / 2);
        }
        curveAngle = UnityEngine.Random.Range(0, 360);

        char groundType = 'g';
        switch (continentType)
        {
            case (ContinentType.STARTING_AREA):
                if (GameManager.gm.leader == null)
                {
                    GameManager.gm.GetComponent<HeroPartyManager>().AddKnight(center);

                    walkLevelGrid[(int) center.x + 5, (int) center.y] = 'a';
                }
                break;
            case (ContinentType.MOUNTAINOUS):
                groundType = 'r'; // TODO
                break;
        }

        List<Vector2> circleCenters = new List<Vector2>();

        for (int circleNum = 0; circleNum < complexity; circleNum++)
        {
            CreateCircle(new MapCoor((int)tempCenter.x, (int)tempCenter.y), largestRadius, groundType, hasCoast);
            circleCenters.Add(tempCenter);

            int radius = largestRadius;

            // Create geographical features particular to the continentType; 
            // this is where the majority of the environmental features will be implemented
            switch (continentType)
            {
                case (ContinentType.STARTING_AREA):
                    int randGeoFeature = UnityEngine.Random.Range(0, 3);

                    switch (randGeoFeature)
                    {
                        case (0):
                            // create a forest circle
                            CreateForest(tempCenter, largestRadius * 2 / 3, 10);
                            break;
                        case (1):
                            // create a mountain circle (TODO)
                            break;
                        default:
                            // create nothing
                            break;
                    }
                    break;
                case (ContinentType.MOUNTAINOUS):
                    // TODO: Make continent surrounded by mountains. This likely can't be done in this switch, 
                    // and will be implemented after the whole continent land mass has been created.
                    break;
            }

            // Decide where the next circle is
            // tempCenter is the next circle's center.

            curveAngle += UnityEngine.Random.Range(-3.14f / 6, 3.14f / 6);

            int whichCircleToBuildOffOf = UnityEngine.Random.Range(0, 3);
            if (whichCircleToBuildOffOf != 0)
            {
                // Create a circle on the perimeter of a random circle that has already been built
                // I haven't tested how well this actually works.
                Vector2 randCenter = circleCenters[UnityEngine.Random.Range(0, circleCenters.Count)];
                int tempCurvAngle = UnityEngine.Random.Range(0, 360);
                tempCenter = new Vector2(randCenter.x + radius * Mathf.Cos(tempCurvAngle) * 1.2f, randCenter.y + radius * Mathf.Sin(tempCurvAngle) * 1.2f);
            }
            else
            {
                // Create a circle in the direction the ccontinent is generally going towards
                newCenter = new Vector2(newCenter.x + radius * Mathf.Cos(curveAngle), newCenter.y + radius * Mathf.Sin(curveAngle));
                tempCenter = newCenter;
            }

            // The next circle is smaller.
            radius = (int) (radius * .8f);
        }

        // A previous attempt at making the coast. Almost worked, but not to my satisfaction. 
        // Code will remain in case someone's interested in fixing it, but it shouldn't be a priority.
        /*if (hasCoast && false)
        {
            MapCoor potentialCoast = new MapCoor((int) center.x, (int)center.y);
            char coastType = 'c';
            if (groundType == 'r')
            {
                coastType = 'd';
            }

            char c = GetTile(potentialCoast, true);
            do
            {
                potentialCoast.y--;
                if (potentialCoast.y < 0)
                {
                    potentialCoast.y = mapHeight;
                }
                c = GetTile(potentialCoast, true);
            } while (c != '\0');

            MapCoor startCoast = potentialCoast;

            int safetyInt = 200;
            char probe = GetTile(potentialCoast, true);
            do
            {
                if (potentialCoast.x > mapWidth - 1)
                {
                    potentialCoast.x = 0;
                }

                if (potentialCoast.y > mapHeight - 1)
                {
                    potentialCoast.y = 0;
                }
                if (potentialCoast.x < 0)
                {
                    potentialCoast.x = mapWidth - 1;
                }

                if (potentialCoast.y < 0)
                {
                    potentialCoast.y = mapHeight - 1;
                }
                probe = groundGrid[potentialCoast.x, potentialCoast.y];

                Direction probeDir = 0;
                if (probe == '\0')
                {
                    probe = coastType;
                }
                else
                {
                    probeDir = (Direction)(((int)probeDir + 1) % 4);
                }

                // move the probe
                switch (probeDir)
                {
                    case (Direction.RIGHT):
                        potentialCoast.x++;
                        break;
                    case (Direction.UP):
                        potentialCoast.y++;
                        break;
                    case (Direction.LEFT):
                        potentialCoast.x--;
                        break;
                    case (Direction.DOWN):
                        potentialCoast.y--;
                        break;
                }

            } while (!potentialCoast.Equals(startCoast) && --safetyInt > 0);
            if (safetyInt <= 0)
            {
                Debug.Log("Error");
            } else
            {
                Debug.Log("Good!");
            }
        }*/
    }

    // Very inefficient, but very simple and effective. Inefficiency is not a concern for the map generation algorithm, 
    // as the player will find it reasonable to have some "loading" time as it generates. Currently the map only takes a few seconds to be fully instantiated.
    void BuildCoast()
    {
        bool lastWasWater = true;
        char coastType = 's';

        // Horizontal pass
        for (int y = 0; y < mapHeight; y++)
        {

            for (int x = 0; x < mapWidth; x++)
            {
                char currTile = groundGrid[x, y];
                if (currTile != '\0')
                {
                    if (lastWasWater)
                    {
                        switch (currTile)
                        {
                            case ('r'):
                                coastType = 'd';
                                break;
                            default:
                                coastType = 's';
                                break;
                        }
                        lastWasWater = false;
                        char leftTile = GetTile(new MapCoor(x - 1, y), true);
                         groundGrid[x, y] = coastType;
                        
                    }
                } else
                {
                    switch (GetTile(new MapCoor(x - 1, y), true))
                    {
                        case ('r'):
                            coastType = 'd';
                            break;
                        default:
                            coastType = 's';
                            break;
                    }
                    if (!lastWasWater)
                    {
                        groundGrid[x, y] = coastType;
                    }
                    lastWasWater = true;
                }
            }
        }

        // Vertical pass
        for (int x = 0; x < mapWidth; x++)
        {

            for (int y = 0; y < mapHeight; y++)
            {
                char currTile = groundGrid[x, y];
                if (currTile != '\0')
                {
                    if (lastWasWater)
                    {
                        switch (currTile)
                        {
                            case ('r'):
                                coastType = 'd';
                                break;
                            case ('d'):
                                coastType = 'd';
                                break;
                            default:
                                coastType = 's';
                                break;
                        }
                        lastWasWater = false;
                        char leftTile = GetTile(new MapCoor(x, y - 1), true);
                        groundGrid[x, y] = coastType;

                    }
                }
                else
                {
                    switch (GetTile(new MapCoor(x, y - 1), true))
                    {
                        case ('r'):
                            coastType = 'd';
                            break;
                        case ('d'):
                            coastType = 'd';
                            break;
                        default:
                            coastType = 's';
                            break;
                    }
                    if (!lastWasWater)
                    {
                        groundGrid[x, y] = coastType;
                    }
                    lastWasWater = true;
                }
            }
        }
    }

    void CreateForest(Vector2 center, int largestRadius, int complexity)
    {
        Vector2 newCenter = center;
        Vector2 tempCenter = center;
        float curveAngle = 0;

        int sparse = 0; // if this number, which is set to a random int between 0 and 7, is greater than 6, the forest will have a sparse spot (nothing).

        for (int circleNum = 0; circleNum < complexity; circleNum++)
        {
            if (sparse < 6)
            {
                CreateCircle(new MapCoor((int)tempCenter.x, (int)tempCenter.y), largestRadius, 'f', false, 5);


            }
            // Decide where the next circle is

            curveAngle += UnityEngine.Random.Range(-3.14f / 6, 3.14f / 6);

            int whichCircleToBuildOffOf = UnityEngine.Random.Range(0, 3);
            if (whichCircleToBuildOffOf == 0)
            {
                curveAngle = 0;
                //Debug.Log("Create circle at center");
                tempCenter = new Vector2(center.x + largestRadius * Mathf.Cos(curveAngle) * 1.2f, center.y + largestRadius * Mathf.Sin(curveAngle) * 1.2f);
            }
            else
            {
                newCenter = new Vector2(newCenter.x + largestRadius * Mathf.Cos(curveAngle), newCenter.y + largestRadius * Mathf.Sin(curveAngle));
                tempCenter = newCenter;
            }

            largestRadius = (int)(largestRadius * .9f);

            sparse = UnityEngine.Random.Range(0, 7);
        }

    }

    char GetGroundTileAtCoor(MapCoor coor)
    {
        int x = coor.x;
        int y = coor.y;
        if (x < 0)
        {
            x = mapWidth + x;
        }
        else if (x >= mapWidth)
        {
            x = x - mapWidth;
        }
        if (y < 0)
        {
            y = mapHeight + y;
        }
        else if (y >= mapHeight)
        {
            y = y - mapHeight;
        }
        return groundGrid[x, y];
    }

    // A land circle is mostly comprised of groundType, but has sand and coast around the outside if hasCoast is true.
    // The greater patchy is, the more empty spots the circle will have.
    void CreateCircle(MapCoor center, int radius, char groundType, bool hasCoast = true, int patchy = 0)
    {

        //groundGrid[(int)center.x, (int)center.y] = groundType;
        float area = 3.14f * radius * radius;
        float theta = 0;

        float deltaTheta = 3.14f / (radius * 5);

        for (; theta < 6.28f; theta += deltaTheta)
        {
            for (int r = 0; r < radius; r++)
            {
                int x = (int) Mathf.Round(center.x + Mathf.Cos(theta) * r);
                int y = (int) Mathf.Round(center.y + Mathf.Sin(theta) * r);

                // Make sure the new tile is within the map. If not, it will wrap around to the other side.
                if (x < 0) {
                    x = mapWidth + x;
                } else if (x >= mapWidth)
                {
                    x = x - mapWidth;
                }
                if (y < 0)
                {
                    y = mapHeight + y;
                }
                else if (y >= mapHeight)
                {
                    y = y - mapHeight;
                }

                // Set the ground tile
                char currTile = GetGroundTileAtCoor(new MapCoor(x, y));

                bool isSparse = 0 != UnityEngine.Random.Range(0, patchy + 1);

                if (!isSparse)
                {
                    if (CharIsGround(groundType))
                    {
                        try
                        {
                            groundGrid[x, y] = groundType;
                        } catch (System.Exception e)
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        // The circle is on the interactable level, such as trees or something
                        if (groundGrid[x, y] != 's' && groundGrid[x, y] != 'd' && CharIsGround(groundGrid[x, y]))
                        {
                            // interactable things can only be placedd on ground that isn't coast

                            walkLevelGrid[x, y] = groundType;
                        }
                    }
                }
            }
        }

    }

    int errors = 0;

    enum Direction
    {
        RIGHT,
        UP,
        LEFT,
        DOWN
    }

    int numTrees = 0;

    bool CharIsGround(char checkGround)
    {
        return checkGround == 'g' || checkGround == 's' || checkGround == 'r'; // TODO: other ground types
    }

    GameObject GetTileObj (char tileType, bool isGround = true)
    {
        switch (tileType)
        {
            case ('a'):
                return airship;
            case ('d'):
                return sandTileDark;
            case ('g'):
                return grassTile;
            case ('f'):
                return forestTile;
            case ('m'):
                return mountainTile;
            case ('r'):
                return rockTile;
            case ('s'):
                return sandTile;
            default:
                if (isGround)
                {
                    return waterTile;
                }
                else return null;
        }
    }

    // used when the hero steps on a new tile.
    public char GetTile(Vector2 location, bool isGround)
    {
        if (isGround)
        {
            return (groundGrid[(int)(location.x / 16), -(int)(location.y / 16)]);
        } else
        {
            return (walkLevelGrid[(int)(location.x / 16), -(int)(location.y / 16)]);
        }
    }

    public char GetTile(MapCoor coor, bool isGround)
    {

        if (coor.x < 0)
        {
            coor.x = mapWidth + coor.x;
        }
        else if (coor.x >= mapWidth)
        {
            coor.x = coor.x - mapWidth;
        }
        if (coor.y < 0)
        {
            coor.y = mapHeight + coor.y;
        }
        else if (coor.y >= mapHeight)
        {
            coor.y = coor.y - mapHeight;
        }

        if (isGround)
        {
            return groundGrid[coor.x, coor.y];
        } else
        {
            return walkLevelGrid[coor.x, coor.y];
        }
    }

    IEnumerator InstantiateTiles()
    {

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                char groundTile = groundGrid[x, y];
                GameObject tile = null;
                if (groundTile == 'd')
                {
                    tile = GameObject.Instantiate(GetTileObj(groundTile));
                    tile.transform.position = new Vector2(16 * x, -16 * y);
                }
                else
                {
                    tile = GameObject.Instantiate(GetTileObj(groundGrid[x, y]));
                    tile.transform.position = new Vector2(16 * x, -16 * y);
                }


                tile = GetTileObj(walkLevelGrid[x, y], false);
                if (tile != null)
                {
                    tile = GameObject.Instantiate(tile);
                    tile.transform.position = new Vector2(16 * x, -16 * y);
                }
            }
            yield return null;
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap(128, 100, 5, 2.2f);
        }
	}
}

// This struct is currently not used, but may be used later to prevent continents from overlapping.
public struct CenterRadiusPair
{
    public MapCoor center;
    public int radius;

    public CenterRadiusPair(MapCoor center, int radius)
    {
        this.center = center;
        this.radius = radius;
    }
}
// Used for easy of use when looking for particular coordinates in the ground character grid.
// Stores coordinates as ints, so Float -> Integer conversion is bypassed. Vector2's aren't needed
public struct MapCoor
{
    public int x;
    public int y;

    public MapCoor(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public override bool Equals(object obj)
    {
        return ((MapCoor)obj).x == this.x && ((MapCoor)obj).y == this.y;
    }

    public override int GetHashCode()
    {
        return (int) (Mathf.Pow(x, y) + Mathf.Pow(y, x));
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }
}
