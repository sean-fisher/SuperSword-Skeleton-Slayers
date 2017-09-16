using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipTile : InteractableTile {

    AirshipController ac;

    float raiseSpeed = 32;

    private void Start()
    {
        ac = GetComponent<AirshipController>();
    }

    public override void ActivateInteraction()
    {
        StartCoroutine(MoveHeroesToShip());
    }

    IEnumerator MoveHeroesToShip()
    {
        for (int i = 0; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        yield return null;
        ac.canMove = true;
        GridController.encountersEnabled = false;
        GridController.clampToPixel = false;
        GridController.partyCanMove = false;
        Camera.main.GetComponent<CamFollow>().AirGroundViewSwitch(true);
        Camera.main.GetComponent<CamFollow>().targetToFollow = gameObject.transform;
        ac.speed = 100;

        /*while (transform.position.z < 16)
        {
            transform.position += new Vector3(0, 0, raiseSpeed * Time.deltaTime);
            yield return null;
        }*/
    }
}
