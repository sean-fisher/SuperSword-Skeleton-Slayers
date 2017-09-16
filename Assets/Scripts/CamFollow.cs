using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using code from https://forum.unity.com/threads/smooth-transition-between-perspective-and-orthographic-modes.32765/

public class CamFollow : MonoBehaviour {

    public Transform targetToFollow;
    Camera cam;

    private Matrix4x4 ortho,
                      perspective;
    public float fov = 60f,
                 near = .3f,
                 far = 1000f,
                 orthographicSize = 100;
    private float aspect;
    private bool orthoOn = false;

    private void Start()
    {
        cam = GetComponent<Camera>();

        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        cam.projectionMatrix = ortho;
        //blender = (MatrixBlender)GetComponent(typeof(MatrixBlender));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            orthoOn = !orthoOn;
            AirGroundViewSwitch(orthoOn);
        }
    }

    private void LateUpdate()
    {
        if (targetToFollow)
        {
            transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y, transform.position.z);
        }

    }

    float zSpeed = 7950;
    float rotSpeed = 15;
    float fovSpeed = 135;

    // Distance for z: -7950
    // Distance for rot: -15deg
    // distance for fov: 135

    public void AirGroundViewSwitch(bool groundToAir)
    {
        if (!groundToAir)
        {
            // Switches perspective to overhead
            Quaternion notAngled = Quaternion.Euler(new Vector3(0, 0, 0));
            BlendToMatrix(ortho, notAngled, .3f);
        }
        else
        {
            // Switches perspective to angled, used for airship)
            Quaternion angled = Quaternion.Euler(new Vector3(-15, 0, 0));
            BlendToMatrix(perspective, angled, .8f);
        }
    }

    IEnumerator SwitchToGround()
    {
        while (transform.rotation.x < 0 || transform.position.z > -8000)
        {
            if (transform.rotation.x < 0)
            {
                transform.Rotate(new Vector3(rotSpeed * Time.deltaTime, 0, 0));
            }
            if (transform.position.z > -8000)
            {
                transform.position -= new Vector3(0, 0, zSpeed * Time.deltaTime);
            }
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, -8000);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, transform.rotation.z));
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
            cam.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (Time.time - startTime) / duration);
            yield return 1;
        }
        cam.projectionMatrix = dest;
    }

    Coroutine BlendToMatrix(Matrix4x4 targetMatrix, Quaternion targetRotation, float duration)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(cam.projectionMatrix, targetMatrix, targetRotation, duration));
    }
}
