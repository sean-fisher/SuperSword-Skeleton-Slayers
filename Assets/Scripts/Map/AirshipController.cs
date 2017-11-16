using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipController : GridController {
    
    public static AirshipTile at;
    public static bool isBoarded;

    // Each grid space is 16 units wide; 1 unit represents one pixel. 
    // This always moves in increments of one pixel so as to 
    // reduce blur and increase sharpness.

    // Use this for initialization
    void Start () {
        isBoarded = false;
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
            } else if (Input.GetButtonDown("AButton"))
            {
                //InspectTile();
            }
            else if (Input.GetButtonDown("BButton"))
            {
                if (at.isFlying)
                {
                    at.SwitchGroundAir();
                } else
                {
                    // Exit ship
                    ExitShip(transform.position);
                }
            }
        }
    }

    void ExitShip(Vector2 placeToExitAt)
    {
        Debug.Log("get off");
        isBoarded = false;
        for (int i = 0; i < BattleManager.hpm.activePartyMembers.Count;
            i++)
        {
            BaseCharacter characterBeingActivated = BattleManager.hpm
                .activePartyMembers[i];

            characterBeingActivated.GetComponent<SpriteRenderer>()
                .enabled = true;
            characterBeingActivated
                .GetComponent<GridController>().enabled = true;
            characterBeingActivated.transform.position
                = new Vector3(transform.position.x,
                placeToExitAt.y + 4);
            characterBeingActivated.GetComponent<GridController>()
                .inputList.Clear();
        }
        GameManager.gm.leader.enabled = true;
        MapEntrance.canEnter = true;
        canMove = false;
        Camera.main.GetComponent<CamFollow>().targetToFollow =
            GameManager.gm.leader.gameObject.transform;
        enabled = false;
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
        int tempWalkState = 0;
        
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
        if (anim)
        {
            anim.SetInteger("WalkState", tempWalkState);
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
            canMove = objectsAtDestination == null
                || objectsAtDestination.isTrigger;
        }
        else if (!at.isFlying && objectsAtDestination.tag != "Mountain")
        {
            canMove = false;
            //ExitShip(destinationVector);
        }

        

        if (canMove)
        {
            Sounds.audioSource.clip = Sounds.step;
            Sounds.audioSource.Play();
            canMove = false;
            destinationVector = Vector3.zero;
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
            destinationVector = new Vector3(Mathf.Round(destinationVector.x), Mathf.Round(destinationVector.y), destinationVector.z);
            StartCoroutine(MovingOneSpace(destinationVector, canMoveAfter));
            MapGenerator.mg.WrapMapOneColumn(destination);
        } else
        {
            canMove = true;
            Sounds.audioSource.clip = Sounds.bump;
            Sounds.audioSource.Play();
        }
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
        }
        else
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
            Animator anim = GetComponent<Animator>();
            if (anim)
            {
                anim.SetInteger("WalkState", 0);
            }

            SetCoor(new MapCoor((int)tryDestination.x, (int)tryDestination.y));

            if (GameManager.gm.leader == this && !debugEnabled)
            {
                //char currGround = MapGenerator.mg.GetTile(transform.position, true);
                if (encountersEnabled)
                {
                    battleActivated = RandomEncounterManager.AdvanceStepCount(transform.position);
                }
            }
            Vector3 newPos = new Vector3(
                currCoor.x * 16, currCoor.y * -16, transform.position.z);
            //transform.position = newPos;
        }
        RoundPositionToSixteens();
        //gameObject.transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

        if (!at.isFlying && !battleActivated)
        {
            canMove = true;
            CheckTile(transform.position);
        } else if (at.isFlying)
        {
            canMove = true;
        }
    }

    void RoundPositionToSixteens()
    {

        if (at.isFlying)
        {
            float xCoor = transform.position.x / 16;
            xCoor = Mathf.Round(xCoor);
            float yCoor = (transform.position.y - 32) / 16;
            yCoor = Mathf.Round(yCoor);

            transform.position = new Vector3(xCoor * 16,32 + yCoor * 16, transform.position.z);
        } else
        {
            float xCoor = transform.position.x / 16;
            xCoor = Mathf.Round(xCoor);
            float yCoor = transform.position.y / 16;
            yCoor = Mathf.Round(yCoor);

            transform.position = new Vector3(xCoor * 16, yCoor * 16, transform.position.z);
        }
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

        if (checkedTileCol && checkedTileCol.GetComponent<AirshipTile>() == null)
        {
            InteractableTile it = checkedTileCol.gameObject.GetComponent<InteractableTile>();
            it.ActivateInteraction();
            Debug.Log("interact" + checkedTileCol.gameObject.name);
            return true;
        }
        return false;
    }
}
