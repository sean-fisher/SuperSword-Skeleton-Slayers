using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FourSideTransition : ScreenTransition {

    [Header("fourSideTransition")]
    public Texture2D blackTexture;
    public float speed;
    float distanceTraveled = 0.1f;

    // Set this to false when transitioning out
    int inOrOut = 1;

    private void OnEnable()
    {
        StartCoroutine(In());
    }

    private void OnGUI()
    {
        if (transitioning && !waitOnBlack)
        {
            //Left part
            GUI.DrawTexture(new Rect(0, 0, distanceTraveled * Screen.width / Screen.height, Screen.height), blackTexture);
            //Right part
            GUI.DrawTexture(new Rect(Screen.width - distanceTraveled * Screen.width / Screen.height, 0, distanceTraveled * Screen.width / Screen.height, Screen.height), blackTexture);
            //Top part
            GUI.DrawTexture(new Rect(0, Screen.height - distanceTraveled, Screen.width, distanceTraveled), blackTexture);
            //Bottom part
            GUI.DrawTexture(new Rect(0, 0, Screen.width, distanceTraveled), blackTexture);

            if (inOrOut > 0)
            {
                if (distanceTraveled < Screen.height / 2)
                {
                    distanceTraveled += Time.deltaTime * speed * inOrOut;
                }
            }
            else if (distanceTraveled > 0)
            {
                distanceTraveled += Time.deltaTime * speed * inOrOut;
            }
        }
        else
        {
            if (waitOnBlack)
            {
                // Draw black over entire screen
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
            }
        }
    }

    public override bool TransitionIn(float widthRatio = 0, float speed = 1)
    {
        waitOnBlack = false;
        frameCount++;
            enabled = true;
        transitioning = true;
        inOrOut = 1;

        if (distanceTraveled < Screen.height / 2)
        {
            distanceTraveled += Time.deltaTime * speed * inOrOut * Screen.height;
            return transitioning;
        }
        else
        {
            waitOnBlack = true;
            return false;
        }
    }
    int frameCount = 0;

    public override bool TransitionOut(float widthRatio = 0, float speed = 1)
    {
        waitOnBlack = false;
        transitioning = true;
        inOrOut = -1;
        if (distanceTraveled > 0)
        {
            distanceTraveled += Time.deltaTime * speed * inOrOut * Screen.height;
            return transitioning;
        }
        else
        {
            transitioning = false;
            return transitioning;
        }
    }

    IEnumerator In()
    {
        while (TransitionIn())
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        while (TransitionOut())
        {
            yield return null;
        }
    }
}
