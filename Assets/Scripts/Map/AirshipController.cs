using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipController : GridController {
    
    SpriteRenderer sr2;
    Animator anim2;

    // Each grid space is 16 units wide; 1 unit represents one pixel. 
    // This always moves in increments of one pixel so as to 
    // reduce blur and increase sharpness.

    // Use this for initialization
    void Start () {
		if (!debugEnabled && GameManager.gm.leader == null)
        {
            GameManager.gm.leader = this;
            TextBoxManager.player = this;
        }
        anim2 = GetComponent<Animator>();
        sr2 = GetComponent<SpriteRenderer>();
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
            } else if (Input.GetButtonDown("StartButton")) {
                GameManager.gm.pauseMenu.OpenMenu();
                canMove = false;
            } else if (Input.GetButtonDown("AButton"))
            {
                InspectTile();
            }
        }
    }
    
    MoveDir lastMove2 = MoveDir.DOWN;

    // Checks the tile in front of the hero and interacts with it if something is found there.
    // If nothing is in front, it checks the current tile.
    void InspectTile()
    {
        Vector3 tileToInspect = Vector3.zero;
        switch (lastMove2)
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

                InteractableTile.currentlyStandingOnInteractableTile = true;
            }
        } else
        {
            InteractableTile.currentlyStandingOnInteractableTile = true;
        }
    }

    void MoveOneSpace(MoveDir destination, bool canMoveAfter = true)
    {
        canMove = false;
        int tempWalkState = 0;

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

            // Checks if objects are already at the destination and if they can be moved onto
            Collider2D objectsAtDestination = Physics2D.OverlapCircle(destinationVector, 4, wall);
            // Prevent party from moving
            partyCanMove = objectsAtDestination == null;
        }

        if (partyCanMove)
        {
            inputList.Add(destination);

            if (inputList.Count > placeInParty)
            {

                destination = inputList[0];
                lastMove2 = destination;
                inputList.RemoveAt(0);

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

                walkState2 = tempWalkState;

                destinationVector = new Vector3(Mathf.Round(destinationVector.x), Mathf.Round(destinationVector.y), destinationVector.z);
                canMove = false;

                if (anim2)
                {
                    anim2.SetInteger("WalkState", walkState2);
                }

                StartCoroutine(MovingOneSpace(destinationVector, canMoveAfter));

            }
            else
            {
                StartCoroutine(WaitWhileLeaderMoves());
            }
        }

    }

    IEnumerator WaitWhileLeaderMoves()
    {
        canMove = false;
        while (!BattleManager.hpm.leader.gameObject.GetComponent<GridController>().canMove)
        {
            yield return null;
        }
        canMove = true;
    }

    int walkState2;

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
            int distanceX = Mathf.Abs((int)(tryDestination.x - gameObject.transform.position.x));
            int distanceY = Mathf.Abs((int)(tryDestination.y - gameObject.transform.position.y));

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
                tempPosition = new Vector3(tempPosition.x + Time.deltaTime * speed * MoveDirX, tempPosition.y, tempPosition.z);
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
            gameObject.transform.position = tempPosition;

            Animator anim = GetComponent<Animator>();
            if (anim)
            {
                anim.SetInteger("WalkState", 0);
            }

            if (GameManager.gm.leader == this && !debugEnabled)
            {
                char currGround = MapGenerator.mg.GetTile(transform.position, true);
                if (encountersEnabled)
                {
                    battleActivated = RandomEncounterManager.AdvanceStepCount(currGround);
                }
            }
        }

        //gameObject.transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

        if (canMoveAfter && !battleActivated)
        {
            canMove = true;
            CheckTile(transform.position);
        }

        canMove = true;
        // sr.sortingOrder = -(int)transform.position.y;
    }

    static Vector3 ClampToPixel(Vector3 unclamped)
    {
        if (clampToPixel)
        {
            return new Vector3(Mathf.Round(unclamped.x), Mathf.Round(unclamped.y), unclamped.z);
        } else
        {
            return unclamped;
        }
    }

    bool CheckTile(Vector3 positionToCheck)
    {
        Collider2D checkedTileCol = Physics2D.OverlapCircle(positionToCheck, 4, interactableTile);

        if (checkedTileCol)
        {
            InteractableTile it = checkedTileCol.gameObject.GetComponent<InteractableTile>();
            it.ActivateInteraction();
            return true;
        }
        return false;
    }
}
