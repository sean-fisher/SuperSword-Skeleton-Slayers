using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {

    public LayerMask wall;
    public LayerMask interactableTile;

    // Leader is 0, then 1, 2, etc. 
    // Used for making the party follow the leader.
    public int placeInParty;
    SpriteRenderer sr;

    /** ANIMATION STATES:
     * 0: idle
     * 1. Walk up
     * 2. Walk down
     * 3. Walk left
     * 4. Walk right
     * */


    public List<Direction> inputList = new List<Direction>();

    // Each grid space is 16 units wide; 1 unit represents one pixel. This always moves in increments of one pixel so as to reduce blur and increase sharpness.

    // Use this for initialization
    void Start () {
		if (GameManager.gm.leader == null)
        {
            GameManager.gm.leader = this;
            TextBoxManager.player = this;
        }
        sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (canMove)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                MoveOneSpace(Direction.LEFT);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                MoveOneSpace(Direction.RIGHT);
            }
            else if (Input.GetAxis("Vertical") < 0f)
            {
                MoveOneSpace(Direction.DOWN);
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                MoveOneSpace(Direction.UP);
            }
        }
    }

    public bool canMove = true;

    void MoveOneSpace(Direction destination, bool canMoveAfter = true)
    {
        inputList.Add(destination);

        if (inputList.Count > placeInParty)
        {
            if (placeInParty == 0)
            {
                //inputList.RemoveAt(0);
            }
            else
            {
            }
            destination = inputList[0];
            inputList.RemoveAt(0);

            Vector2 destinationVector;

            switch (destination)
            {
                case (Direction.LEFT):
                    destinationVector = new Vector2(gameObject.transform.position.x - 16, gameObject.transform.position.y);
                    walkState = 3;
                    break;
                case (Direction.RIGHT):
                    destinationVector = new Vector2(gameObject.transform.position.x + 16, gameObject.transform.position.y);
                    walkState = 4;
                    break;
                case (Direction.DOWN):
                    destinationVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 16);
                    walkState = 2;
                    break;
                default:
                    destinationVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 16);
                    walkState = 1;
                    break;
            }
            destinationVector = new Vector2(Mathf.Round(destinationVector.x), Mathf.Round(destinationVector.y));
            canMove = false;
            //Debug.Log("Set state to " + walkState);
            GetComponent<Animator>().SetInteger("WalkState", walkState);
            StartCoroutine(MovingOneSpace(destinationVector, canMoveAfter));

        } else
        {
            StartCoroutine(WaitWhileLeaderMoves());
        }

    }

    public int speed = 50;

    Vector2 tempPosition;

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

    IEnumerator MovingOneSpace(Vector2 tryDestination, bool canMoveAfter = true)
    {

        // Checks if objects are already at the destination and if they can be moved onto
        Collider2D objectsAtDestination = Physics2D.OverlapCircle(tryDestination, 4, wall);//Physics2D.OverlapSphere(tryDestination, 5f);
                
        if (objectsAtDestination)
        {
        } else
        {
            int distanceX = Mathf.Abs((int)(tryDestination.x - gameObject.transform.position.x));
            int distanceY = Mathf.Abs((int)(tryDestination.y - gameObject.transform.position.y));

            int directionX = 1;
            if (tryDestination.x < gameObject.transform.position.x)
            {
                directionX = -1;
            }
            int directionY = 1;
            if (tryDestination.y < gameObject.transform.position.y)
            {
                directionY = -1;
            }

            float progress = 0;
            while (progress < distanceX)
            {
                progress += Time.deltaTime * speed;
                tempPosition = new Vector2(tempPosition.x + Time.deltaTime * speed * directionX, tempPosition.y);
                transform.position = ClampToPixel(tempPosition);
                yield return null;
            }
            progress = 0;
            while (progress < distanceY)
            {
                progress += Time.deltaTime * speed;
                tempPosition = new Vector2(tempPosition.x, tempPosition.y + Time.deltaTime * speed * directionY);
                transform.position = ClampToPixel(tempPosition);
                yield return null;
            }
            tempPosition = tryDestination;
            gameObject.transform.position = tempPosition;

            GetComponent<Animator>().SetInteger("WalkState", 0);
        }

        //gameObject.transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

        if (canMoveAfter)
        {
            canMove = true;
            CheckTile(transform.position);
        }
        
       // sr.sortingOrder = -(int)transform.position.y;
    }

    static Vector2 ClampToPixel(Vector2 unclamped)
    {
        return new Vector2(Mathf.Round(unclamped.x), Mathf.Round(unclamped.y));
    }

    void CheckTile(Vector2 positionToCheck)
    {
        Collider2D checkedTileCol = Physics2D.OverlapCircle(positionToCheck, 4, interactableTile);

        if (checkedTileCol)
        {
            InteractableTile it = checkedTileCol.gameObject.GetComponent<InteractableTile>();
            it.ActivateInteraction();
        }
    }

    public void DisableMovement()
    {
        canMove = false;
    }
    public void DisablePartyMovement()
    {
        throw new System.NotImplementedException("Leftover Method from P1620");
    }
    public void EnableMovement()
    {
        BattleManager.bManager.CheckDisableMenu();
    }
}
