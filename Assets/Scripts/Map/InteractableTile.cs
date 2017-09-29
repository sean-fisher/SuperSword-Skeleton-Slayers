using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableTile : MonoBehaviour {

    public static bool currentlyStandingOnInteractableTile = false;

    public abstract void ActivateInteraction();
}
