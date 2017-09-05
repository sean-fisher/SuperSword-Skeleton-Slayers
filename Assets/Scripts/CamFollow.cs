using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

    public Transform targetToFollow;

    private void LateUpdate()
    {
        if (targetToFollow)
        {
            transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y, transform.position.z);
        }
    }
}
