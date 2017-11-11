using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using some code from https://forum.unity.com/threads/smooth-transition-between-perspective-and-orthographic-modes.32765/

public class CamFollow : MonoBehaviour {

    public Transform targetToFollow;
    Camera cam;
    public bool canFollow = false;

    private Matrix4x4 ortho,
                      perspective;
    public float fov = 60f,
                 near = .3f,
                 far = 1000f,
                 orthographicSize = 100;
    private float aspect;

    private void Start()
    {
        cam = GetComponent<Camera>();

        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect,
            -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        cam.projectionMatrix = ortho;
        //blender = (MatrixBlender)GetComponent(typeof(MatrixBlender));
    }
    
    private void LateUpdate()
    {
        if (targetToFollow && canFollow)
        {
            if (isFlying)
            {
                transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y - currDisplacement, transform.position.z);
            } else
            {
                transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y - currDisplacement, transform.position.z);
            }
        }

    }

    bool isFlying = false;
    

    public int displace = 0;
    public float currDisplacement = 0;
    public int displaceDest = 0;
    

    AirshipTile airship;

    // Distance for z: -7950
    // Distance for rot: -15deg
    // distance for fov: 135

    public void AirGroundViewSwitch(bool groundToAir)
    {
        if (airship == null)
        {
            airship = GameObject.FindObjectOfType<AirshipTile>();
        }
        if (!groundToAir)
        {
            displaceDest = 0;
            // Switches perspective to ortho overhead
            Quaternion notAngled = Quaternion.Euler(new Vector3(0, 0, 0));
            BlendToMatrix(ortho, notAngled, .5f);
            isFlying = false;
        }
        else
        {
            // Switches perspective to angled, used for airship)
            displaceDest = displace;
            isFlying = true;
            Quaternion angled = Quaternion.Euler(new Vector3(-15, 0, 0));
            BlendToMatrix(perspective, angled, .8f);
        }
    }

    static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }

    IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, Quaternion targetRotation, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            currDisplacement = Mathf.Lerp(currDisplacement, displaceDest, (Time.time - startTime) / duration);

            cam.projectionMatrix = MatrixLerp(src, dest, 
                (Time.time - startTime) / duration);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 
                (Time.time - startTime) / duration);
            airship.transform.rotation = Quaternion.Lerp(transform.rotation, 
                targetRotation, (Time.time - startTime) / duration);
            yield return 1;
        }
        cam.projectionMatrix = dest;
        currDisplacement = displaceDest;
    }

    Coroutine BlendToMatrix(Matrix4x4 targetMatrix, Quaternion targetRotation, float duration)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(cam.projectionMatrix, targetMatrix, targetRotation, duration));
    }
}
