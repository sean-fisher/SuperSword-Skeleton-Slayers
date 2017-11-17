using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour {
    

    public static MapGenerator mg;

    [Header("Ground Tiles")]                // CHARACTER GUIDE: each tile is represented by a character
    public GameObject grassTile;            // g
    public GameObject rockTile;             // r
                                            // n
    public GameObject sandTile;             // s
    public GameObject sandTileDark;         // d
    public GameObject waterTile;            // w or '\0' (Null character)
    public GameObject testTile;             // t
    public GameObject iceTile;              // h
    public GameObject icebergTile;          // b
    public GameObject obsidianTile;         // o
    public GameObject volcanoTile;         // v
                                            // 
    [Header("Interactable Tiles")]          // 
    public GameObject airship;              // a
    public GameObject invisibleChest;       // i
    public GameObject mountainTile;         // m
    public GameObject smallMountainTile;    // /
    public GameObject forestTile;           // f
    public GameObject treasureChest;        // c
    public GameObject potionChest;          // p
    public GameObject igloo;                // e
    public GameObject lockedTreasureChest;  // l
    public GameObject castle;               // .
    public GameObject pyramid;              // ^
    public GameObject armorChest;              // *
    public GameObject cactus;              // *
    public GameObject potionShop;              // (
    public GameObject etherShop;              // )

    [Header("Entrances")]
    public GameObject darkForestEntrance;   // !
    public GameObject iceMazeEntrance;      // @
    public GameObject pyramidEntrance;      // #
    public GameObject mountaincaveEntrance; // $
    public GameObject lavaCaveEntrance;     // %

    [Header("NPCs")]
    public GameObject airshipSalesman;      // 0
    public GameObject penguinSalesman;      // 1

    public GameObject emptyGameObject;      // -

                                            // EACH TILE TYPE MUST HAVE:
                                            //  Appropriate GameObject GetTileObj(...) code
                                            // CreateContinent(...) and CharIsGround(...) code if it is a ground type

        
    [Header("Which items appear on each continent?")]
    public List<BaseItem> grasslandItems;
    public List<BaseItem> glacierItems;

    // 2D character array represents the grid of tiles that comprises the world map (Grass, sand, etc.) 
    char[,] groundGrid;
    // 2D character array represents the grid of tiles that exists above the ground (Trees, mountains, doors, etc.)
    char[,] walkLevelGrid;
    // 2D character array represents the grid of tiles that will not ever be checked in the algo
    char[,] dontCheckGrid;

    List<GameObject>[,] instantiatedTiles; // used for wrapping the map

    bool mapGenerated = false;

    // Used to make sure continents don't overlap
    //List<CenterRadiusPair> continents = new List<CenterRadiusPair>();

    public static int mapWidth = 0;
    public static int mapHeight = 0;

    // Lower number is higher frquency
    const int forestCircleFrequency = 5;
    int forestCircleCounter = 0;


    const int darkForestFrequency = 3;
    int darkForestCounter = 0;

    const int numAirshipSalesmen = 1;
    int numAirshipSalesmenPlaced = 0;

    void ResetValues()
    {
        numAirshipSalesmenPlaced = 0;
        forestCircleCounter = 3;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameManager.gm.gameObject.GetComponent<Inventory>().AddToInventory(ItemGenerator.instance.GetTreasureBasedOnLocation().GetItemData());
        }
    }

    struct FeatureCenterPair
    {
        public MapCoor center;
        public FeatureTypes featureType;
        public bool mirrored;

        public FeatureCenterPair(MapCoor center, FeatureTypes featureType, bool mirrored = false)
        {
            this.center = center;
            this.featureType = featureType;
            this.mirrored = mirrored;
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
        GenerateMap(80, 80, 5, 2.5f);
    }

    void GenerateMap(int width, int height, int numContinents, float continentSize)
    {
        //Debug.Log(string.Format("Width {0}, height {1}, numContinents {2}, continentSize {3}", width, height, numContinents, continentSize));

        groundGrid = new char[width, height];
        walkLevelGrid = new char[width, height];
        dontCheckGrid = new char[width, height];
        instantiatedTiles = new List<GameObject>[width, height];
        mapHeight = height;
        mapWidth = width;

        // A map must always have a starting zone, but the other continents are mostly random.
        // 
        ContinentType[] cTypes = new ContinentType[numContinents];
        cTypes[0] = ContinentType.GRASSLAND;

        // Make sure the continents are varied;

        cTypes[1] = ContinentType.DESERT;
        cTypes[2] = ContinentType.GLACIER;
        cTypes[3] = ContinentType.MOUNTAIN;
        cTypes[4] = ContinentType.VOLCANO;

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
            CreateContinent(continentStartPos.ToVector(), circleRadius, 12, 
                cTypes[continent]);

            // Calculations for next circle
            theta += 6.28f / (float)(numContinents);
            fromMapCenterRadius = defaultRadius - Mathf.Abs(Mathf.Sin(theta) * defaultRadius * (1 - 1 / ratio));
        }

        BuildCoast();

        PlaceFeatures();

        StartCoroutine(InstantiateTiles());
    }

    bool castlePlaced = false;
    MapCoor knightStartPoint;

    void CreateContinent(Vector2 center, int largestRadius, int complexity, 
        ContinentType continentType, bool hasCoast = true)
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
            case (ContinentType.GRASSLAND):
                if (GameManager.gm.leader == null)
                {
                    GameManager.gm.GetComponent<HeroPartyManager>()
                        .AddKnight(center);
                    knightStartPoint = new MapCoor((int)center.x, (int)center.y);

                    walkLevelGrid[knightStartPoint.x, knightStartPoint.y] = '-';
                    dontCheckGrid[knightStartPoint.x, knightStartPoint.y] = '-';
                    //leaderCoorX = (int) center.x / 16;
                    //leaderCoorY = (int)(Mathf.Abs(center.y / 16));

                    // @@@@ Place test tiles @@@@

                    dontCheckGrid[knightStartPoint.x, knightStartPoint.y - 3] = '(';
                    dontCheckGrid[knightStartPoint.x + 1, knightStartPoint.y - 3] = ')';
                    //featuresToPlaceOnMap.Add(new FeatureCenterPair(new MapCoor((int)center.x + 4, (int)center.y - 4), FeatureTypes.CASTLE));
                } else
                {
                }
                break;
            case (ContinentType.MOUNTAIN):
                dontCheckGrid[(int)center.x, (int)center.y + 2] = '$';
                groundType = 'r'; // TODO
                break;
            case (ContinentType.DESERT):
                dontCheckGrid[(int)center.x, (int)center.y + 2] = '#';
                groundType = 's'; // TODO
                break;
            case (ContinentType.GLACIER):
                dontCheckGrid[(int)center.x, (int)center.y + 2] = '@';
                groundType = 'h'; // TODO
                break;
            case (ContinentType.VOLCANO):
                dontCheckGrid[(int)center.x, (int)center.y + 2] = '%';
                groundType = 'o'; // TODO
                break;
        }

        List<Vector2> circleCenters = new List<Vector2>();

        for (int circleNum = 0; circleNum < complexity; circleNum++)
        {
            CreateCircle(new MapCoor((int)tempCenter.x, (int)tempCenter.y), largestRadius, groundType, hasCoast);
            circleCenters.Add(tempCenter);

            int radius = largestRadius;

            int randGeoFeature = UnityEngine.Random.Range(0, 14);

            // Create geographical features particular to the continentType; 
            // this is where the majority of the environmental features 
            // will be implemented
            switch (continentType)
            {
                case (ContinentType.GRASSLAND):

                    if (circleNum == 3 && darkForestCounter-- == 0)
                    {
                        featuresToPlaceOnMap.Add(new FeatureCenterPair(
                            new MapCoor(tempCenter),
                            FeatureTypes.DARK_FOREST));
                    } else
                    if (randGeoFeature < 6)
                    {
                        // create a forest
                        CreateForest(tempCenter, largestRadius, 10);
                    }
                    else if (randGeoFeature < 12)
                    {
                        // create a forest circle (TODO)
                        if (--forestCircleCounter < 0)
                        {
                            if (radius > 5)
                            {
                                featuresToPlaceOnMap.Add(new FeatureCenterPair(
                                    new MapCoor(tempCenter),
                                    FeatureTypes.FOREST_CIRCLE));

                                forestCircleCounter = forestCircleFrequency;

                            }
                            else
                            {
                                forestCircleCounter = 0;
                            }
                        }
                    } else
                    {
                        // Place a chest

                        walkLevelGrid[(Mathf.Abs((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'c';
                    }
                    break;
                case (ContinentType.GLACIER):

                    if (randGeoFeature < 5)
                    {
                        // Place a penguin salesman
                        walkLevelGrid[(Mathf.Abs((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = '1';
                    } else if (randGeoFeature < 10)
                    {
                        // Place an igloo
                        walkLevelGrid[(Mathf.Abs((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'e';
                    }
                    else if (randGeoFeature > 11)
                    {
                        // Place a chest

                        walkLevelGrid[(Mathf.Abs((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'c';
                    }
                    break;
                case (ContinentType.MOUNTAIN):
                    if (randGeoFeature < 5)
                    {
                        // Place a mouuntain
                        walkLevelGrid[Mathf.Abs(((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'm';
                    }
                    else if (randGeoFeature > 11)
                    {
                        // Place a chest

                        walkLevelGrid[(Mathf.Abs((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'c';
                    }
                    break;
                case (ContinentType.DESERT):
                    if (randGeoFeature < 4)
                    {
                        // Place a pyramid
                        walkLevelGrid[((int)tempCenter.x) % mapWidth,
                            ((int)tempCenter.y) % mapHeight] = '^';
                    } else if (randGeoFeature < 11)
                    {
                        // Place a cactus
                        walkLevelGrid[((int)tempCenter.x) % mapWidth,
                            ((int)tempCenter.y) % mapHeight] = '*';
                    }
                    else if (randGeoFeature > 11)
                    {
                        // Place a chest

                        walkLevelGrid[(Mathf.Abs((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'c';
                    }
                    break;
                case (ContinentType.VOLCANO):
                    if (!castlePlaced && circleNum > 3)
                    {
                        castlePlaced = true;

                        featuresToPlaceOnMap.Add(new FeatureCenterPair(new MapCoor((int)center.x + 4, (int)center.y - 4), FeatureTypes.CASTLE));
                    } else if (randGeoFeature < 5)
                    {
                        // Place a mouuntain
                        walkLevelGrid[Mathf.Abs(((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'm';
                    }
                    else if (randGeoFeature > 11)
                    {
                        // Place a chest

                        walkLevelGrid[(Mathf.Abs((int)tempCenter.x) % mapWidth),
                            Mathf.Abs(((int)tempCenter.y) % mapHeight)] = 'c';
                    }
                    break;
            }
            // Features that are only added with the last land circle
            if (circleNum  == complexity - 1)
            {
                switch (continentType) {
                    case (ContinentType.GRASSLAND):
                        if (numAirshipSalesmenPlaced++ < numAirshipSalesmen)
                        {
                            MapCoor placeToPutAirship = 
                                new MapCoor((int)tempCenter.x, (int)tempCenter.y);
                            int leftOrRight = UnityEngine.Random.Range(-1, 2);
                            if (leftOrRight == 0)
                            {
                                leftOrRight++;
                            }
                            while (GetTile(placeToPutAirship, true) != '\0')
                            {
                                placeToPutAirship.x += leftOrRight;
                            }
                            placeToPutAirship.x -= leftOrRight;
                            if (leftOrRight < 0)
                            {
                                featuresToPlaceOnMap.Add(new FeatureCenterPair(placeToPutAirship, FeatureTypes.AIRSHIP_SALESMAN));
                            }
                            else
                            {
                                featuresToPlaceOnMap.Add(new FeatureCenterPair(placeToPutAirship, FeatureTypes.AIRSHIP_SALESMAN, true));
                            }
                        }
                        break;
                    default:
                        break;
                }

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
                                walkLevelGrid[Mathf.Abs((x) % mapWidth), y] = '/';
                                groundGrid[Mathf.Abs((x) % mapWidth), y] = 'd';
                                break;
                            case ('h'):
                                coastType = 'b';
                                break;
                            case ('b'):
                                coastType = 'b';
                                break;
                            case ('o'):
                                coastType = 'n';
                                walkLevelGrid[Mathf.Abs((x) % mapWidth), y] = '/';
                                groundGrid[Mathf.Abs((x) % mapWidth), y] = 'd';
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
                            walkLevelGrid[(x) % mapWidth, y] = '/';
                            break;
                        case ('d'):
                            break;
                        case ('h'):
                            coastType = 'b';
                            break;
                        case ('b'):
                            coastType = 'b';
                            break;
                        case ('o'):
                            coastType = 'n';
                            walkLevelGrid[(x) % mapWidth, y] = '/';
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
                                walkLevelGrid[Mathf.Abs((x) % mapWidth), y] = '/';
                                break;
                            case ('d'):
                                coastType = 'd';
                                walkLevelGrid[Mathf.Abs((x) % mapWidth), y] = '/';
                                break;
                            case ('h'):
                                coastType = 'b';
                                break;
                            case ('b'):
                                coastType = 'b';
                                break;
                            case ('n'):
                                coastType = 'n';
                                break;
                            case ('o'):
                                coastType = 'n';
                                walkLevelGrid[Mathf.Abs((x) % mapWidth), y] = '/';
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
                            rCount++;
                            coastType = 'd';
                            walkLevelGrid[Mathf.Abs((x) % mapWidth), y] = '/';
                            break;
                        case ('o'):
                            oCount ++;
                            coastType = 'n';
                            walkLevelGrid[Mathf.Abs((x) % mapWidth), y] = '/';
                            break;
                        case ('n'):
                            coastType = 'n';
                            break;
                        case ('d'):
                            coastType = 'd';
                            break;
                        case ('h'):
                            coastType = 'b';
                            break;
                        case ('b'):
                            coastType = 'b';
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

    int oCount = 0;
    int nCOunt = 0;
    int dCOunt = 0;
    int rCount = 0;

    void PlaceFeatures()
    {
        for (int i = 0; i < featuresToPlaceOnMap.Count; i++)
        {
            CreateMapFeature(featuresToPlaceOnMap[i]);
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
    void CreateCircle(MapCoor center, int radius, char groundType, 
        bool hasCoast = true, int patchy = 0)
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
        return checkGround == 'g' || checkGround == 's' ||
            checkGround == 'r' || 
            checkGround == 'h' || checkGround == 'o'; // TODO: other ground types
    }

    int sandCOunt = 0;

    // tileLayer: 0 is ground, 1 is interactable, 2 is dontCheck
    public GameObject GetTileObj (char tileType, int tileLayer = 0)
    {
        switch (tileType)
        {
            case ('a'):
                return airship;
            case ('c'):
                return treasureChest;
            case ('p'):
                return potionChest;
            case ('d'):
                return sandTileDark;
            case ('g'):
                return grassTile;
            case ('e'):
                return igloo;
            case ('f'):
                return forestTile;
            case ('i'):
                return invisibleChest;
            case ('m'):
                return mountainTile;
            case ('/'):
                return smallMountainTile;
            case ('r'):
                return rockTile;
            case ('s'):
                sandCOunt++;
                return sandTile;
            case ('h'):
                return iceTile;
            case ('n'):
                return rockTile;
            case ('o'):
                return obsidianTile;
            case ('v'):
                return volcanoTile;
            case ('b'):
                return icebergTile;
            case ('l'):
                return lockedTreasureChest;
            case ('.'):
                return castle;
            case ('!'):
                return darkForestEntrance;
            case ('@'):
                return iceMazeEntrance;
            case ('#'):
                return pyramidEntrance;
            case ('$'):
                return mountaincaveEntrance;
            case ('%'):
                return lavaCaveEntrance;
            case ('-'):
                return emptyGameObject;
            case ('0'):
                return airshipSalesman;
            case ('1'):
                return penguinSalesman;
            case ('&'):
                return armorChest;
            case ('*'):
                return cactus;
            case ('^'):
                return pyramid;
            case ('('):
                return potionShop;
            case (')'):
                return etherShop;
            default:
                if (tileType != '\0')
                {
                }
                if (tileLayer == 0)
                {
                    return waterTile;
                } else if (tileLayer == 2)
                {
                    return null;
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
                groundGrid[(coor.x) % mapWidth, (coor.y) % mapHeight] = c;
                break;
            case (1):
                walkLevelGrid[(coor.x) % mapWidth, (coor.y) % mapHeight] = c;
                break;
            case (2):
                dontCheckGrid[(coor.x) % mapWidth, (coor.y) % mapHeight] = c;
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


    public char GetTile(Vector2 location, char[,] grid)
    {
        return (grid[(int)(location.x / 16), -(int)(location.y / 16)]);
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

    bool instantiating = true;

    IEnumerator InstantiateTiles()
    {
        instantiating = true;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameObject tile = null;
                tile = GetTileObj(groundGrid[x, y], 0);
                if (tile != null) {
                    tile = GameObject.Instantiate(tile, transform);

                    tile.transform.position = new Vector2(16 * x, -16 * y);
                    if (instantiatedTiles[x,y] == null)
                    {
                        instantiatedTiles[x, y] = new List<GameObject>();
                    }
                    if (walkLevelGrid[x, y] != 'a')
                    {
                        instantiatedTiles[x, y].Add(tile);
                    }
                }

                tile = GetTileObj(walkLevelGrid[x, y], 1);
                if (tile != null && dontCheckGrid[x, y] == '\0' && walkLevelGrid[x, y] != '-')
                {

                    SetSortLayerByY sslby = tile.GetComponent<SetSortLayerByY>();
                    tile = GameObject.Instantiate(tile, transform);
                    if (sslby)
                    {
                        tile.transform.position = new Vector2(
                            16 * x + sslby.spriteOffsetX, 
                            -16 * y + sslby.spriteOffsetY);
                    } else
                    {
                        tile.transform.position = new Vector2(16 * x, -16 * y);
                    }
                    if (walkLevelGrid[x, y] != 'a')
                    {
                        instantiatedTiles[x, y].Add(tile);
                    }
                }

                tile = null;
                tile = GetTileObj(dontCheckGrid[x, y], 2);
                if (tile != null)
                {
                    {
                        SetSortLayerByY sslby = tile.GetComponent<SetSortLayerByY>();
                        tile = GameObject.Instantiate(tile, transform);
                        if (sslby)
                        {
                            tile.transform.position = new Vector2(
                                16 * x + sslby.spriteOffsetX,
                                -16 * y + sslby.spriteOffsetY);
                        }
                        else
                        {
                            tile.transform.position = new Vector2(16 * x, -16 * y);
                        }
                        if (walkLevelGrid[x, y] != 'a')
                        {
                            instantiatedTiles[x, y].Add(tile);
                        }
                    }
                }
            }
            yield return null;
        }

        // Wrap the map so the knight is in the center
        int distanceFromCenter = mapWidth - (int)knightStartPoint.x;
        while (distanceFromCenter > 0)
        {
            WrapMapOneColumn(MoveDir.RIGHT);
            distanceFromCenter--;
        }
        instantiating = false;
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

    void CreateMapFeature(FeatureCenterPair fcp)
    {
        FeatureTypes featureType = fcp.featureType;
        MapCoor center = fcp.center;

        string[] mapFeature = MapFeatures.GetFeature(featureType);

        int layer = 2;
        if (fcp.featureType == FeatureTypes.CASTLE)
        {
            layer = 2;
        }
        if (fcp.mirrored)
        {
            string[] copy = mapFeature;

            for (int i = 0; i < copy.Length; i++)
            {
                mapFeature[i] = new string(mapFeature[i].ToCharArray().Reverse().ToArray());
                for (int j = 0; j < copy[0].Length; j += 3)
                {
                   // mapFeature[i].ToCharArray()[j] = copy[i].ToCharArray()[copy[0].Length - j - 1];
                }
            }
        }
        
        for (int row = 0; row < mapFeature.Length; row++)
        {
            char[] rowOfChars = mapFeature[row].ToCharArray();
            for (int col = 0; col < rowOfChars.Length; col += 3)
            {
                if (rowOfChars[col] == ' ')
                {
                    SetTile(layer, new MapCoor(center.x + col / 3, center.y + row), '\0');
                } else
                {
                    SetTile(layer, new MapCoor(center.x + col / 3, center.y + row), 
                        rowOfChars[col]);
                }
            }
        }
    }

    int leaderCoorX;
    int leaderCoorY;

    public void WrapMapOneColumn(MoveDir directionMoved)
    {
        switch(directionMoved)
        {
            case (MoveDir.UP):


                List<GameObject>[] bottomMostRow =
                    new List<GameObject>[mapHeight];

                for (int i = 0; i < mapWidth; i++)
                {
                    for (int j = 0; j < instantiatedTiles[i, mapHeight - 1 - leaderCoorY].Count; j++)
                    {
                        if (bottomMostRow[i] == null)
                        {
                            bottomMostRow[i] = new List<GameObject>();
                        }
                        bottomMostRow[i].Add(instantiatedTiles[i, 
                            mapHeight - 1 - leaderCoorY]
                            [j]);
                    }
                }

                for (int i = 0; i < bottomMostRow.Length; i++)
                {
                    for (int tileOnCoor = 0;
                        tileOnCoor < bottomMostRow[i].Count; tileOnCoor++)
                    {
                        if (bottomMostRow[i][tileOnCoor])
                        {
                            bottomMostRow[i][tileOnCoor].transform.position
                                += new Vector3(0, mapHeight * 16);
                        }
                    }
                }
                leaderCoorY++;
                if (leaderCoorY > mapHeight - 1)
                {
                    leaderCoorY = 0;
                }
                break;
            case (MoveDir.DOWN):

                leaderCoorY--;
                if (leaderCoorY < 0)
                {
                    leaderCoorY = mapHeight - 1;
                }

                List<GameObject>[] topMostRow =
                    new List<GameObject>[mapHeight];

                for (int i = 0; i < mapWidth; i ++)
                {
                    for (int j = 0; j < instantiatedTiles[i, mapHeight - 1 - leaderCoorY].Count; j++)
                    {
                        if (topMostRow[i] == null)
                        {
                            topMostRow[i] = new List<GameObject>();
                        }
                        topMostRow[i].Add(instantiatedTiles[i, mapHeight - 1 - leaderCoorY][j]);
                    }
                }
                
                for (int i = 0; i < topMostRow.Length; i++)
                {
                    for (int tileOnCoor = 0;
                        tileOnCoor < topMostRow[i].Count; tileOnCoor++)
                    {
                        if (topMostRow[i][tileOnCoor])
                        {
                            topMostRow[i][tileOnCoor].transform.position
                                -= new Vector3(0, mapHeight * 16);
                        }
                    }
                }


                break;
            case (MoveDir.LEFT):

                leaderCoorX--;

                if (leaderCoorX < 0)
                {
                    leaderCoorX = mapWidth;
                }

                List<GameObject>[] rightmostCol = instantiatedTiles.Cast<List<GameObject>>()
                    .Skip(mapHeight * leaderCoorX).Take(mapHeight).ToArray();
                for (int i = 0; i < rightmostCol.Length; i++) {
                    for (int tileOnCoor = 0; 
                        tileOnCoor < rightmostCol[i].Count; tileOnCoor++)
                    {
                        GameObject tile = rightmostCol[i][tileOnCoor];
                        if (tile)
                        {
                            tile.transform.position -= new Vector3(mapWidth * 16, 0);
                        }
                    }
                }
                break;
            case (MoveDir.RIGHT):
               
                List<GameObject>[] leftMostCol = instantiatedTiles.Cast<List<GameObject>>()
                    .Skip(mapHeight * leaderCoorX).Take(mapHeight).ToArray();
                for (int i = 0; i < leftMostCol.Length; i++)
                {
                    for (int tileOnCoor = 0;
                        tileOnCoor < leftMostCol[i].Count; tileOnCoor++)
                    {
                        if (leftMostCol[i][tileOnCoor])
                        {
                            leftMostCol[i][tileOnCoor].transform.position
                                += new Vector3(mapWidth * 16, 0);
                        }
                    }
                }
                leaderCoorX++;
                if (leaderCoorX > mapWidth - 1)
                {
                    leaderCoorX = 0;
                }
                break;
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

public enum ContinentType
{
    None,
    // Different continent types will have different enemy types, geographical features, art, etc.

    GRASSLAND,      // Grass, coast, forests, some mountains, airship or tunnel (to be implemented) can be found
    MOUNTAIN,         // Surrounded by mountans, only accessible by air'
    GLACIER,
    DESERT,
    VOLCANO,
    FOREST,
    MOUNTAINCAVE,
    ICECAVE,
    LAVACAVE,
    PYRAMID,
    OCEAN
}
