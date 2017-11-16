using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {

    public static int stepsTaken = 0;

    public static bool encountersEnabled = true;
    public static bool clampToPixel = true;
    public bool debugEnabled = false;

    public bool isLeader = false;

    public LayerMask wall;
    public LayerMask interactableTile;

    protected MapCoor currCoor = new MapCoor();

    // Leader is 0, then 1, 2, etc. 
    // Used for making the party follow the leader.
    public int placeInParty;

    /** ANIMATION STATES:
     * 0: idle
     * 1. Walk up
     * 2. Walk down
     * 3. Walk left
     * 4. Walk right
     * */


    public List<MoveDir> inputList = new List<MoveDir>();
    protected Animator anim;

    // Each grid space is 16 units wide; 1 unit represents one pixel. 
    // This always moves in increments of one pixel so as to 
    // reduce blur and increase sharpness.

    // Use this for initialization
    void Start () {
		if (!debugEnabled && GameManager.gm && GameManager.gm.leader == null)
        {
            GameManager.gm.leader = this;
            TextBoxManager.player = this;
        }
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //if (Input.GetKeyDown(KeyCode.Return))
        if (canMove)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                MoveOneSpace(MoveDir.LEFT);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                MoveOneSpace(MoveDir.RIGHT);
            }
            else if (Input.GetAxis("Vertical") < 0f)
            {
                MoveOneSpace(MoveDir.DOWN);
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                MoveOneSpace(MoveDir.UP);
            } else if (Input.GetButtonDown("StartButton") 
                && !AirshipController.isBoarded) {
                Debug.Log("open menu");
                GameManager.gm.pauseMenu.OpenMenu();
                canMove = false;
            } else if (Input.GetButtonDown("AButton"))
            {
                Debug.Log("inspect!");
                InspectTile();
            }
        }
    }

    public MapCoor GetCoor()
    {
        return currCoor;
    }

    public void SetCoor(MapCoor curr)
    {
        if (curr.x < 0)
        {
            curr.x += MapGenerator.mapWidth;
        } else
        {
            curr.x = curr.x % MapGenerator.mapWidth;
        }

        if (curr.y < 0)
        {
            curr.y += MapGenerator.mapWidth;
        }
        else
        {
            curr.y = curr.y % MapGenerator.mapWidth;
        }
        currCoor = curr;
    }

    public bool canMove = true;
    MoveDir lastMove = MoveDir.DOWN;

    public static bool partyCanMove = true;

    // Checks the tile in front of the hero and interacts with it 
    // if something is found there.
    // If nothing is in front, it checks the current tile.
    void InspectTile()
    {
        Vector3 tileToInspect = Vector3.zero;
        switch (lastMove)
        {
            case (MoveDir.LEFT):
                tileToInspect = new Vector3(gameObject.transform.position.x - 16, gameObject.transform.position.y, transform.position.z);
                break;
            case (MoveDir.RIGHT):
                tileToInspect = new Vector3(gameObject.transform.position.x + 16, gameObject.transform.position.y, transform.position.z);
                break;
            case (MoveDir.DOWN):
                tileToInspect = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 16, transform.position.z);
                break;
            case (MoveDir.UP):
                tileToInspect = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 16, transform.position.z);
                break;
            default:
                break;
        }
        bool adjacentTileFound = CheckTile(tileToInspect);

        if (!adjacentTileFound)
        {
            if (CheckTile(gameObject.transform.position)) {

                Debug.Log("Standing on something");
                InteractableTile.currentlyStandingOnInteractableTile = true;
            }
        } else
        {
            InteractableTile.currentlyStandingOnInteractableTile = true;
        }
    }

    void MoveOneSpace(MoveDir destination, bool canMoveAfter = true)
    {
        int tempWalkState = 0;

        if (GameManager.gm.leader == this)
        {

            Vector3 destinationVector = Vector3.zero;
            switch (destination)
            {
                case (MoveDir.LEFT):
                    destinationVector = new Vector3(gameObject.transform.position.x - 16, gameObject.transform.position.y, transform.position.z);
                    tempWalkState = 3;
                    break;
                case (MoveDir.RIGHT):
                    destinationVector = new Vector3(gameObject.transform.position.x + 16, gameObject.transform.position.y, transform.position.z);
                    tempWalkState = 4;
                    break;
                case (MoveDir.DOWN):
                    destinationVector = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 16, transform.position.z);
                    tempWalkState = 2;
                    break;
                case (MoveDir.UP):
                    destinationVector = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 16, transform.position.z);
                    tempWalkState = 1;
                    break;
                default:
                    break;
            }

            // Checks if objects are already at the destination and 
            // if they can be moved onto
            Collider2D objectsAtDestination = Physics2D.OverlapCircle
                (destinationVector, 4, wall);
            if (objectsAtDestination == null)
            {
                objectsAtDestination = Physics2D.OverlapCircle(
                    destinationVector, 4, interactableTile);

                // Prevent party from moving
                partyCanMove = objectsAtDestination == null 
                    || objectsAtDestination.isTrigger;
                if (!partyCanMove)
                {
                    if (!touchedDamageTile && objectsAtDestination.GetComponent<Cactus>())
                    {
                        touchedDamageTile = true;
                        objectsAtDestination.GetComponent<Cactus>().ActivateInteraction();
                    }
                } else
                {
                    stepsTaken++;
                }
            } else
            {
                partyCanMove = false;
            }

        }

        if (partyCanMove)
        {
            inputList.Add(destination);

            if (inputList.Count > placeInParty)
            {
                // Start to move
                destination = inputList[0];
                lastMove = destination;
                inputList.RemoveAt(0);

                Vector3 destinationVector = Vector3.zero;
                switch (destination)
                {
                    case (MoveDir.LEFT):
                        destinationVector = new Vector3(gameObject.transform.position.x - 16, gameObject.transform.position.y, transform.position.z);
                        tempWalkState = 3;

                        currCoor.x--;
                        break;
                    case (MoveDir.RIGHT):
                        destinationVector = new Vector3(gameObject.transform.position.x + 16, gameObject.transform.position.y, transform.position.z);
                        tempWalkState = 4;
                        
                        currCoor.x++;
                        break;
                    case (MoveDir.DOWN):
                        destinationVector = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 16, transform.position.z);
                        tempWalkState = 2;

                        currCoor.y++;
                        break;
                    case (MoveDir.UP):
                        destinationVector = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 16, transform.position.z);
                        tempWalkState = 1;

                        currCoor.y--;
                        break;
                    default:
                        break;
                }

                walkState = tempWalkState;

                destinationVector = new Vector3(Mathf.Round(destinationVector.x), Mathf.Round(destinationVector.y), destinationVector.z);
                canMove = false;

                if (anim)
                {
                    anim.SetInteger("WalkState", walkState);
                }

                if (canWrapMap)
                {
                    MapGenerator.mg.WrapMapOneColumn(destination);
                }
                StartCoroutine(MovingOneSpace(destinationVector, canMoveAfter));

            }
            else
            {
                StartCoroutine(WaitWhileLeaderMoves());
            }
        } else
        {
            if (placeInParty == 0)
            {
                // The leader turns to face the inputted direction
                lastMove = destination;
                switch (destination)
                {
                    case (MoveDir.LEFT):
                        tempWalkState = 3;
                        break;
                    case (MoveDir.RIGHT):
                        tempWalkState = 4;
                        break;
                    case (MoveDir.DOWN):
                        tempWalkState = 2;
                        break;
                    case (MoveDir.UP):
                        tempWalkState = 1;
                        break;
                    default:
                        break;
                }
                walkState = tempWalkState;
                if (anim)
                {
                    anim.SetInteger("WalkState", walkState);
                }
                StartCoroutine(WaitThenStopWalk());
            }
        }
    }

    bool touchedDamageTile = false;

    IEnumerator WaitThenStopWalk()
    {
        yield return null;
        if (anim)
        {
            anim.SetInteger("WalkState", 0);
        }
    }

    public static bool canWrapMap = true;

    //void CheckMap

    public int speed = 50;


    IEnumerator WaitWhileLeaderMoves()
    {
        canMove = false;
        while (!BattleManager.hpm.leader.gameObject.GetComponent<GridController>().canMove)
        {
            yield return null;
        }
        canMove = true;
    }

    int walkState;

    IEnumerator MovingOneSpace(Vector3 tryDestination, bool canMoveAfter = true)
    {
        //Physics2D.OverlapSphere(tryDestination, 5f);
        InteractableTile.currentlyStandingOnInteractableTile = false;

        Vector3 tempPosition = transform.position;
        bool battleActivated = false;

        if (false)
        {
        } else
        {
            int distanceX = Mathf.Abs(
                (int)(tryDestination.x - gameObject.transform.position.x));
            int distanceY = Mathf.Abs(
                (int)(tryDestination.y - gameObject.transform.position.y));

            int MoveDirX = 1;
            if (tryDestination.x < gameObject.transform.position.x)
            {
                MoveDirX = -1;
            }
            int MoveDirY = 1;
            if (tryDestination.y < gameObject.transform.position.y)
            {
                MoveDirY = -1;
            }

            float progress = 0;
            while (progress < distanceX)
            {
                progress += Time.deltaTime * speed;
                tempPosition = new Vector3(tempPosition.x + Time.deltaTime * speed
                    * MoveDirX, tempPosition.y, tempPosition.z);
                transform.position = ClampToPixel(tempPosition);
                yield return null;
            }
            progress = 0;
            while (progress < distanceY)
            {
                progress += Time.deltaTime * speed;
                tempPosition = new Vector3(tempPosition.x, tempPosition.y + Time.deltaTime * speed * MoveDirY, tempPosition.z);
                transform.position = ClampToPixel(tempPosition);
                yield return null;
            }
            tempPosition = tryDestination;
            gameObject.transform.position = ClampToPixel(tempPosition);

            Animator anim = GetComponent<Animator>();
            if (anim)
            {
                anim.SetInteger("WalkState", 0);
            }
            

        }

        if ( canMoveAfter && !battleActivated)
        {
            canMove = true;
            bool interactTileSteppedOn = CheckTile(transform.position);

            if (!interactTileSteppedOn && GameManager.gm.leader == this && !debugEnabled)
            {
                if (encountersEnabled)
                {
                    battleActivated = RandomEncounterManager.AdvanceStepCount(transform.position);
                }
            }
        }
        touchedDamageTile = false;
    }

    Vector3 ClampToPixel(Vector3 unclamped)
    {
        if (clampToPixel)
        {
            return new Vector3(Mathf.Round(unclamped.x), Mathf.Round(unclamped.y), unclamped.z);
        } else
        {
            return unclamped;
        }
    }

    Vector3 ClampToPixel(MapCoor nextCoor)
    {
        Vector3 newPos = new Vector3(
            currCoor.x * 16, currCoor.y * -16 + 4, transform.position.z);
        if (clampToPixel)
        {
            return new Vector3(Mathf.Round(newPos.x), Mathf.Round(newPos.y), newPos.z);
        }
        else
        {
            return newPos;
        }
    }

    bool CheckTile(Vector3 positionToCheck)
    {
        if (isLeader)
        {
            Collider2D checkedTileCol = Physics2D.OverlapCircle(positionToCheck, 4, interactableTile);

            if (checkedTileCol)
            {
                InteractableTile it = checkedTileCol.gameObject.GetComponent<InteractableTile>();
                it.ActivateInteraction();
                Debug.Log("found something to interact with");
                return true;
            }
        }
        return false;
    }

    public void DisableMovement()
    {
        canMove = false;
        partyCanMove = false;
    }
    public void DisablePartyMovement()
    {
        throw new System.NotImplementedException("Leftover Method from P1620");
    }
    public void EnableMovement()
    {
        if (!BattleManager.bManager.CheckDisableMenu())
        {
            canMove = true;
            partyCanMove = true;
        }
    }
}

public enum MoveDir
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}
