using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipTile : InteractableTile {

    GridController gc;
    public static bool canBoard = false;

    public bool isFlying = false;
    public int liftSpeed;

    private void Update()
    {
        if (Input.GetButtonDown("AButton") && gc.canMove && gc.enabled)
        {
            SwitchGroundAir();
        }
    }

    private void Start()
    {
        AirshipController.at = this;
        gc = GetComponent<GridController>();
    }

    public override void ActivateInteraction()
    {
        if (canBoard)
        {
            Debug.Log("board");
            AirshipController.isBoarded = true;
            StartCoroutine(MoveHeroesToShip());
        }
    }

    IEnumerator MoveHeroesToShip()
    {
        for (int i = 0; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].GetComponent<SpriteRenderer>
                ().enabled = false;
            BattleManager.hpm.activePartyMembers[i].GetComponent<GridController>
                ().enabled = false;
        }
        GameManager.gm.leader.enabled = false;
        yield return null;
        gc.canMove = true;
        Camera.main.GetComponent<CamFollow>().targetToFollow = 
            gameObject.transform;
        gc.enabled = true;
        MapEntrance.canEnter = false;
    }

    public void SwitchGroundAir()
    {
        if (isFlying)
        {
            Collider[] thingsAtDestination = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y), 5f);
            for (int i = 0; i < thingsAtDestination.Length; i++)
            {
                Debug.Log(thingsAtDestination[i].name);
            }

            //Debug.Log("Air -> Ground");
            GridController.encountersEnabled = true;
            GridController.clampToPixel = true;
            GridController.partyCanMove = false;
            gc.canMove = false;
            Camera.main.GetComponent<CamFollow>().AirGroundViewSwitch(false);
            StartCoroutine(MoveVert(false));
        } else
        {
            //Debug.Log("Ground -> Air");
            Camera.main.GetComponent<CamFollow>().AirGroundViewSwitch(true);
            gc.canMove = false;
            StartCoroutine(MoveVert(true));
        }
    }

    IEnumerator MoveVert(bool moveUp)
    {
        if (moveUp)
        {
            while (transform.position.z > -16)
            {
                transform.position -= new Vector3(0, -Time.deltaTime * liftSpeed * 2, Time.deltaTime * liftSpeed);
                yield return null;
            }
            transform.position = new Vector3(transform.position.x, 
                transform.position.y, -16);
            RoundPositionToSixteens();
            GridController.encountersEnabled = false;
            GridController.clampToPixel = false;
            gc.canMove = true;
            GridController.partyCanMove = false;

        } else
        {
            while (transform.position.z < 0)
            {
                transform.position += new Vector3(0, -Time.deltaTime * liftSpeed * 2, Time.deltaTime * liftSpeed);
                yield return null;
            }
            transform.position = new Vector3(transform.position.x, (int) transform.position.y, 0);
            gc.canMove = true;
            RoundPositionToSixteens();
        }
        isFlying = moveUp;
    }

    void RoundPositionToSixteens()
    {
        float xCoor = transform.position.x / 16;
        xCoor = Mathf.Round(xCoor);
        float yCoor = (transform.position.y) / 16;
        yCoor = Mathf.Round(yCoor);

        transform.position = new Vector3(xCoor * 16, yCoor * 16, transform.position.z);
        
    }
}
