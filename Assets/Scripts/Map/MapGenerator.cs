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
    public GameObject treasureChest;        // c
    public GameObject lockedChest;          // l

                                            // EACH TILE TYPE MUST HAVE:
                                            //  Appropriate GameObject GetTileObj(...) code
                                            // CreateContinent(...) and CharIsGround(...) code if it is a ground type



    // 2D character array represents the grid of tiles that comprises the world map (Grass, sand, etc.) 
    char[,] groundGrid;
    // 2D character array represents the grid of tiles that exists above the ground (Trees, mountains, doors, etc.)
    char[,] walkLevelGrid;
    // 2D character array represents the grid of tiles that will not ever be checked in the algo
    char[,] dontCheckGrid;

    static bool mapGenerated = false;

    // Used to make sure continents don't overlap
    //List<CenterRadiusPair> continents = new List<CenterRadiusPair>();

    int mapWidth = 0;
    int mapHeight = 0;

    // Lower number is higher frquency
    const int forestCircleFrequency = 5;
    int forestCircleCounter = 0;

    enum ContinentType
    {
        None,
        // Different continent types will have different enemy types, geographical features, art, etc.

        STARTING_AREA,      // Grass, coast, forests, some mountains, airship or tunnel (to be implemented) can be found
        MOUNTAINOUS         // Surrounded by mountans, only accessible 
    }

    struct FeatureCenterPair
    {
        public MapCoor center;
        public FeatureTypes featureType;

        public FeatureCenterPair(MapCoor center, FeatureTypes featureType)
        {
            this.center = center;
            this.featureType = featureType;
        }
    }

    List<FeatureCenterPair> featuresToPlaceOnMap = new List<FeatureCenterPair>();

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
        //TestGenerateMap(64, 50, 5, 1.8f);
        GenerateMap(128, 100, 6, 3f);
    }

    void TestGenerateMap(int width, int height, int numContinents, float continentSize)
    {
        groundGrid = new char[width, height];
        walkLevelGrid = new char[width, height];
        dontCheckGrid = new char[width, height];
        mapHeight = height;
        mapWidth = width;

        // A map must always have a starting zone, but the other continents are mostly random.
        // 
        ContinentType[] cTypes = new ContinentType[numContinents];
        int rand = UnityEngine.Random.Range(0, numContinents);
        cTypes[rand] = ContinentType.STARTING_AREA;

        // Make sure the continents are varied; 
        // if there is already a mountain, for example, a different type will be picked unless all types have been picked
        /*
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
            }
            else
            {
                cTypes[randContinent] = (ContinentType)Random.Range(1, 3);
            }
        }
        */
        MapCoor mapCenter = new MapCoor(mapWidth / 2, mapHeight / 2);

        

        CreateMapFeature(FeatureTypes.FOREST_CIRCLE, new MapCoor(mapWidth / 2, mapHeight / 2));
        /*
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
             /*
            int circleRadius = (int)(ratio * continentSize * 2.5f);


            // A continent is formed by several circles of varying size built off of each other.
            CreateContinent(continentStartPos.ToVector(), circleRadius, 8, cTypes[continent]);

            // Calculations for next circle
            theta += 6.28f / (float)(numContinents);
            fromMapCenterRadius = defaultRadius - Mathf.Abs(Mathf.Sin(theta) * defaultRadius * (1 - 1 / ratio));
        }*/

        //BuildCoast();

        StartCoroutine(InstantiateTiles());
    }


    void GenerateMap(int width, int height, int numContinents, float continentSize)
    {
        groundGrid = new char[width, height];
        walkLevelGrid = new char[width, height];
        dontCheckGrid = new char[width, height];
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
             *                   WIDTH              numContinents = 5
             *        _____________________
             *  H    |                     |
             *  E    |     C3      C2      |
             *  I    |                     |
             *  G    |                C1   |
             *  H    |    C4               |
             *  T    |           C5        |
             *       |_____________________|
             *       
             * */
             
            int circleRadius = (int)(ratio * continentSize * 2.5f);


            // A continent is formed by several circles of varying size built off of each other.
            CreateContinent(continentStartPos.ToVector(), circleRadius, 12, cTypes[continent]);

            // Calculations for next circle
            theta += 6.28f / (float)(numContinents);
            fromMapCenterRadius = defaultRadius - Mathf.Abs(Mathf.Sin(theta) * defaultRadius * (1 - 1 / ratio));
        }

        BuildCoast();

        PlaceFeatures();

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
                    walkLevelGrid[(int)center.x - 5, (int)center.y] = 'c';
                    walkLevelGrid[(int)center.x, (int)center.y + 5] = 'l';
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
                    int randGeoFeature = UnityEngine.Random.Range(0, 12);

                    if (randGeoFeature < 6)
                    {
                        // create a forest
                        CreateForest(tempCenter, largestRadius, 10);
                    }
                    else if (randGeoFeature < 9)
                    {
                        // create a forest circle (TODO)
                        if (--forestCircleCounter < 0)
                        {
                            if (radius > 5)
                            {
                                featuresToPlaceOnMap.Add(new FeatureCenterPair(new MapCoor(tempCenter), FeatureTypes.FOREST_CIRCLE));
                                //CreateMapFeature(FeatureTypes.FOREST_CIRCLE, new MapCoor(tempCenter));
                                forestCircleCounter = forestCircleFrequency;
                            }
                            else
                            {
                                forestCircleCounter = 0;
                            }
                        }
                    }
                    else if (randGeoFeature < 11)
                    {
                        // create a mountain circle (TODO)
                    } else
                    {
                        // create nothing
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
            largestRadius = Mathf.RoundToInt(radius * .8f);
        }
        
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
                                walkLevelGrid[Mathf.Abs((x - 1) % mapWidth), y] = 'm';
                                //SetTile(0, new MapCoor(x - 1, y), 'm');
                                dontCheckGrid[Mathf.Abs((x - 1) % mapWidth), y] = 'd';
                                //SetTile(2, new MapCoor(x - 1, y), 'd');
                                //SetTile(2, new MapCoor(x-2, y), 'd');
                                dontCheckGrid[Mathf.Abs((x - 2) % mapWidth), y] = 'd';
                                break;
                            default:
                                coastType = 's';
                                break;
                        }
                        lastWasWater = false;
                         groundGrid[x, y] = coastType;

                    }
                } else
                {
                    switch (GetTile(new MapCoor(x - 1, y), true))
                    {
                        case ('r'):
                            coastType = 'd';
                            if (GetTile(new MapCoor(x + 1, y), false) != 'm')
                            {
                                walkLevelGrid[(x + 2) % mapWidth, y] = 'm';
                                dontCheckGrid[(x + 2) % mapWidth, y] = 'd';
                                dontCheckGrid[(x + 1) % mapWidth, y] = 'd';
                            }
                            //groundGrid[(x + 1) % mapWidth, y] = 'd';
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
                                if (GetTile(new MapCoor(x - 1, y), false) != 'm')
                                {
                                    walkLevelGrid[x, y] = 'm';
                                    dontCheckGrid[x, y] = 'd';
                                    dontCheckGrid[Mathf.Abs((x - 1) % mapWidth), y] = 'd';
                                }
                                //groundGrid[x, y + 1] = 'r';
                                break;
                            case ('d'):
                                coastType = 'd';
                                if (GetTile(new MapCoor(x - 1, y), false) != 'm')
                                {
                                    walkLevelGrid[x, y] = 'm';
                                    dontCheckGrid[x, y] = 'd';
                                    dontCheckGrid[Mathf.Abs((x - 1) % mapWidth), y] = 'd';
                                }
                                //groundGrid[x, y + 1] = 'r';
                                break;
                            default:
                                coastType = 's';
                                break;
                        }
                        lastWasWater = false;
                        groundGrid[x, y] = coastType;

                    }
                }
                else
                {
                    switch (GetTile(new MapCoor(x, y - 1), true))
                    {
                        case ('r'):
                            coastType = 'd';
                            if (GetTile(new MapCoor(x - 1, y), false) != 'm')
                            {
                                walkLevelGrid[x, y] = 'm';
                                dontCheckGrid[x, y] = 'd';
                                //dontCheckGrid[x - 1, y] = 'd';
                                SetTile(2, new MapCoor(x - 1, y), 'd');
                            }
                            //groundGrid[x, y - 1] = 'r';
                            break;
                        case ('d'):
                            coastType = 'd';
                            if (GetTile(new MapCoor(x - 1, y), false) != 'm')
                            {
                                walkLevelGrid[x, y] = 'm';
                                dontCheckGrid[x, y] = 'd';
                                SetTile(2, new MapCoor(x - 1, y), 'd');
                                //dontCheckGrid[x - 1, y] = 'd';
                            }
                            //groundGrid[x, y - 1] = 'r';
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

    void PlaceFeatures()
    {
        for (int i = 0; i < featuresToPlaceOnMap.Count; i++)
        {
            CreateMapFeature(featuresToPlaceOnMap[i].featureType, featuresToPlaceOnMap[i].center);
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
                

                bool isSparse = 0 != UnityEngine.Random.Range(0, patchy + 1);

                if (!isSparse)
                {
                    if (CharIsGround(groundType))
                    {
                        groundGrid[x, y] = groundType;
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

    enum Direction
    {
        RIGHT,
        UP,
        LEFT,
        DOWN
    }

    bool CharIsGround(char checkGround)
    {
        return checkGround == 'g' || checkGround == 's' || checkGround == 'r'; // TODO: other ground types
    }

    // tileLayer: 0 is ground, 1 is interactable, 2 is dontCheck
    GameObject GetTileObj (char tileType, int tileLayer = 0)
    {
        switch (tileType)
        {
            case ('a'):
                return airship;
            case ('c'):
                return treasureChest;
            case ('d'):
                return sandTileDark;
            case ('g'):
                return grassTile;
            case ('f'):
                return forestTile;
            case ('l'):
                return lockedChest;
            case ('m'):
                return mountainTile;
            case ('r'):
                return rockTile;
            case ('s'):
                return sandTile;
            default:
                if (tileLayer == 0)
                {
                    return waterTile;
                }
                else return null;
        }
    }

    void SetTile(int tileLayer, MapCoor coor, char c)
    {
        if (coor.x > mapWidth)
        {
            coor.x -= mapWidth;
        } else if (coor.x < 0)
        {
            coor.x += mapWidth;
        }
        if (coor.y > mapHeight)
        {
            coor.y -= mapHeight;
        }
        else if (coor.y < 0)
        {
            coor.y += mapHeight;
        }
        switch (tileLayer)
        {
            case (0):
                groundGrid[coor.x, coor.y] = c;
                break;
            case (1):
                walkLevelGrid[coor.x, coor.y] = c;
                break;
            case (2):
                dontCheckGrid[coor.x, coor.y] = c;
                break;
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
                GameObject tile = null;

                    tile = GameObject.Instantiate(GetTileObj(groundGrid[x, y]));

                    tile.transform.position = new Vector2(16 * x, -16 * y);



                tile = GetTileObj(walkLevelGrid[x, y], 1);
                if (tile != null)
                {
                    SetSortLayerByY sslby = tile.GetComponent<SetSortLayerByY>();
                    tile = GameObject.Instantiate(tile);
                    if (sslby)
                    {
                        tile.transform.position = new Vector2(16 * x + sslby.spriteOffsetX, -16 * y + sslby.spriteOffsetY);
                    } else
                    {
                        tile.transform.position = new Vector2(16 * x, -16 * y);
                    }
                }

                tile = GetTileObj(dontCheckGrid[x, y], 1);
                if (tile)
                {

                    tile = GameObject.Instantiate(tile);

                    tile.transform.position = new Vector2(16 * x, -16 * y);
                }
            }
            yield return null;
        }
    }

    /*
     * MAP FEATURES
     */

    // GRASSLAND
    //
    void CreateForest(Vector2 center, int largestRadius, int complexity)
    {
        Vector2 newCenter = center;
        Vector2 tempCenter = center;
        float curveAngle = 0;

        int sparse = 0; // if this number, which is set to a random int between 0 and 7, is greater than 6, the forest will have a sparse spot (nothing).

        for (int circleNum = 0; circleNum < complexity; circleNum++)
        {
            if (sparse < 4)
            {
                CreateCircle(new MapCoor(tempCenter), largestRadius * 2 / 3, 'f', false, 5);


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
    //

    void CreateMapFeature(FeatureTypes featureType, MapCoor center)
    {

        string[] mapFeature;

        switch (featureType)
        {
            case (FeatureTypes.FOREST_CIRCLE):
                mapFeature = MapFeatures.forestCircle;
                break;
            default:
                mapFeature = MapFeatures.forestCircle;
                break;
        }

        MapCoor featureCenter = new MapCoor(mapFeature[0].Length / 2, mapFeature.Length / 2);

        for (int row = 0; row < mapFeature.Length; row++)
        {
            char[] rowOfChars = mapFeature[row].ToCharArray();
            for (int col = 0; col < rowOfChars.Length; col += 3)
            {
                if (rowOfChars[col] == ' ')
                {
                    SetTile(1, new MapCoor(center.x + col / 3, center.y + row), '\0');
                } else
                {
                    SetTile(1, new MapCoor(center.x + col / 3, center.y + row), rowOfChars[col]);
                }
            }
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

    public MapCoor(Vector2 coor)
    {
        this.x = (int)coor.x;
        this.y = (int)coor.y;
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
