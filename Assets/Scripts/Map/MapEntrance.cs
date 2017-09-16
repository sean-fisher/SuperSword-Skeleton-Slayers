using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEntrance : InteractableTile {

    public string sceneToLoad;
    public TransitionType transitionType;
    public int entranceID;

    public override void ActivateInteraction()
    {
        Debug.Log("Interact with door");
        GameManager.gm.SwitchScene(transitionType, sceneToLoad);
    }

}
