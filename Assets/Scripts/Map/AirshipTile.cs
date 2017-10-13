using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipTile : InteractableTile {

    GridController gc;
    public static bool canBoard = false;

    bool isFlying = false;

    private void Update()
    {
        if (Input.GetButtonDown("AButton") && gc.canMove)
        {
            SwitchGroundAir();
        }
    }

    private void Start()
    {
        gc = GetComponent<GridController>();
    }

    public override void ActivateInteraction()
    {
        if (canBoard)
        {
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
        gc.speed = 100;
    }

    void SwitchGroundAir()
    {
        if (isFlying)
        {
            GridController.encountersEnabled = false;
            GridController.clampToPixel = true;
            GridController.partyCanMove = false;
            Camera.main.GetComponent<CamFollow>().AirGroundViewSwitch(false);
        } else
        {
            Camera.main.GetComponent<CamFollow>().AirGroundViewSwitch(true);
            GridController.encountersEnabled = false;
            GridController.clampToPixel = false;
            GridController.partyCanMove = false;
        }
    }
}
