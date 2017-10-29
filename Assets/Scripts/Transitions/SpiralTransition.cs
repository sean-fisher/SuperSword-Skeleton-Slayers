using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiralTransition : ScreenTransition
{

    public Texture2D blackTexture;
    public Texture2D whiteTexture;
    public Texture2D blueTexture;
    public Texture2D redTexture;
    public Texture2D yellowTexture;
    public float speed;
    public float distanceTraveled = 0.1f;

    float spiralWidth;
    float spiralHeight;
    
    Direction currDir = Direction.RIGHT;


    int right = 0;
    int down = 0;
    int left = 0;
    int up = 0;

    float x = 0;
    float y = 0;

    private void Update()
    {
    }

    public void InOutMethod(float spiralWidth, float spiralheight, int x, int y)
    {
        StartCoroutine(InOut(spiralWidth, spiralheight, x, y));
    }

    private void OnGUI()
    {
        if (transitioning && !transitionOut)
        {

            //Draw already drawn portions
            // North
            GUI.DrawTexture(new Rect(x, y, spiralWidth, (float)spiralHeight / 8f * right), blackTexture);
            // east
            GUI.DrawTexture(new Rect(x + spiralWidth - (float)eigth * down * ratio, y, (float)eigth * down * ratio, spiralHeight), blackTexture);
            // South
            GUI.DrawTexture(new Rect(x, y +spiralHeight - (float)eigth * left, spiralWidth, (float)eigth * (left )), blackTexture);
            // West
            GUI.DrawTexture(new Rect(x, y, (float)eigth * up * ratio, spiralHeight), blackTexture);

            switch (currDir)
            {
                case (Direction.RIGHT):
                    //Draw already drawn portions
                    GUI.DrawTexture(new Rect(x + ratio * eigth * right, y + eigth * right,
                        distanceTraveled * ratio, eigth), blackTexture);

                    //distanceTraveled += Time.deltaTime * speed / ratio;
                    break;
                case (Direction.DOWN):
                    GUI.DrawTexture(new Rect(x + spiralWidth - (eigth * (down + 1) * ratio), y + eigth * right,
                        eigth * ratio, distanceTraveled), blackTexture);

                    //distanceTraveled += Time.deltaTime * speed;
                    break;
                case (Direction.LEFT):
                    GUI.DrawTexture(new Rect(x + spiralWidth - distanceTraveled * ratio - (down * ratio * eigth), y + spiralHeight - eigth * (left + 1),
                        distanceTraveled * ratio, eigth), blackTexture);

                    //distanceTraveled += Time.deltaTime * speed / ratio;
                    break;
                case (Direction.UP):
                    GUI.DrawTexture(new Rect(x + eigth * up * ratio, y + spiralHeight - eigth * (up + 1) - distanceTraveled,
                        eigth * ratio, distanceTraveled), blackTexture);

                    //distanceTraveled += Time.deltaTime * speed;
                    break;
                default:
                    break;
            }


            /*if (distanceTraveled > spiralWidth - (rotations * eigth))
            {
                distanceTraveled = eigth * rotations;
                Debug.Log(distanceTraveled);
            }*/
        }
        else if (transitionOut)
        {
            // Goes CounterClockwise

            //Draw already drawn portions
            // North
            
            GUI.DrawTexture(new Rect(x, y, 
                            spiralWidth, (float)spiralHeight / 8f * (up - 1)), blueTexture);
            // east
            GUI.DrawTexture(new Rect(x + spiralWidth, y, 
                            -(float)eigth * (right - 1) * ratio, spiralHeight), yellowTexture);
            // South
            GUI.DrawTexture(new Rect(x, spiralHeight + y, 
                            spiralWidth, -(float)eigth * (down - 2)), redTexture);
            // West
            GUI.DrawTexture(new Rect(x, y, 
                            (float)eigth * (left - 1) * ratio, spiralHeight), blackTexture);
        }
        else if (waitOnBlack)
        {
            //GUI.DrawTexture(new Rect(0, 0, spiralWidth, spiralHeight), blackTexture);
        }
    }

    int rotations = 0;

    float distanceNeededToTravel;

    float targetWidth;

    bool transitionOut = false;

    public override bool TransitionIn(float widthRatio = 0, float speed = 10)
    {
        waitOnBlack = false;
        frameCount++;
        enabled = true;
        transitioning = true;


        if (distanceTraveled < distanceNeededToTravel)
        {
            distanceTraveled += Time.deltaTime * speed * spiralHeight;
            return transitioning;
        }
        else
        {
            // Switch direction of spiral

            //Debug.Log("Traveled: " + distanceNeededToTravel);

            switch (currDir)
            {
                case (Direction.RIGHT):
                    right++;
                    currDir = Direction.DOWN;
                    distanceNeededToTravel = targetWidth - eigth * (right + left);
                    break;
                case (Direction.DOWN):
                    down++;
                    currDir = Direction.LEFT;
                    distanceNeededToTravel = targetWidth - eigth * (up + down);
                    break;
                case (Direction.LEFT):
                    left++;
                    currDir = Direction.UP;
                    distanceNeededToTravel = targetWidth - eigth * (right + left);
                    break;
                case (Direction.UP):
                    up++;
                    currDir = Direction.RIGHT;
                    distanceNeededToTravel = targetWidth - eigth * (up + down);
                    rotations++;
                    break;
                default:
                    break;
            }
            distanceTraveled = 0;

            if (rotations > 5)
            {
                waitOnBlack = true;
                return false;
            }
            return true;
        }
    }
    int frameCount = 0;

    public override bool TransitionOut(float widthRatio = 0, float speed = 10)
    {
        waitOnBlack = false;
        enabled = true;
        transitioning = true;
        

        this.speed = speed;

        if (distanceTraveled < distanceNeededToTravel)
        {
            distanceTraveled += Time.deltaTime * speed * spiralHeight;
            return transitioning;
        }
        else
        {
            // Switch direction of spiral

            //Debug.Log(currDir + " Rotations: " + rotations + " Traveled: " + distanceNeededToTravel);

            switch (currDir)
            {
                case (Direction.LEFT):
                    left--;
                    currDir = Direction.DOWN;
                    distanceNeededToTravel = spiralHeight -  (eigth * (right + left));
                    rotations--;
                    break;
                case (Direction.UP):
                    up--;
                    currDir = Direction.LEFT;
                    distanceNeededToTravel = spiralWidth - (eigth * (up + down) * ratio);
                    break;
                case (Direction.RIGHT):
                    right--;
                    currDir = Direction.UP;
                    distanceNeededToTravel = spiralHeight - (eigth * (right + left));
                    break;
                case (Direction.DOWN):
                    down--;
                    currDir = Direction.RIGHT;
                    distanceNeededToTravel = spiralWidth - (eigth * (up + down) * ratio);
                    break;
                default:
                    break;
            }
            distanceNeededToTravel = Mathf.Abs(distanceNeededToTravel);

            if (distanceNeededToTravel == 0)
            {
                //Debug.Log(currDir);
            }
            distanceTraveled = 0;
            //Debug.Log(String.Format("Right: {0} Down: {1} Left: {2} Up: {3}", right, down, left, up));

            if (left < 2)
            {
                return false;
            }
            return true;
        }
    }

    float ratio;
    float eigth;

    IEnumerator InOut(float spiralWidth, float spiralHeight, int x, int y)
    {
        this.x = x;
        this.y = y;

        this.spiralHeight = spiralHeight;
        this.spiralWidth = spiralWidth;
        distanceNeededToTravel = spiralHeight;
        targetWidth = distanceNeededToTravel;
        ratio = (float)spiralWidth / spiralHeight;
        Debug.Log(ratio);

        eigth = (float)spiralHeight / 8;
        currDir = Direction.RIGHT;

        while (TransitionIn())
        {
            yield return null;
        }
        transitionOut = true;
        waitOnBlack = false;
        //Debug.Log(String.Format("Rotations: {0} Right: {1} Down: {2} Left: {3} Up: {4} CurrDir: {5}", rotations, right, down, left, up, currDir));

        distanceNeededToTravel = 0;
        rotations = 5;
        right = 5;
        left = 5;
        down = 5;
        up = 5;

        targetWidth = 0;
        currDir = Direction.RIGHT;

        while (TransitionOut())
        {
            yield return null;
        }
        transitioning = false;
        Debug.Log("end");
    }

    public void BattleSpiral(float spiralWidth, float spiralHeight, float x, float y)
    {
        StartCoroutine(BattleSpiraling(spiralWidth, spiralHeight, x, y));
    }

    IEnumerator BattleSpiraling(float spiralWidth, float spiralHeight, float x, float y)
    {
        this.x = x;
        this.y = y;

        this.spiralHeight = spiralHeight;
        this.spiralWidth = spiralWidth;
        distanceNeededToTravel = spiralHeight;
        targetWidth = distanceNeededToTravel;
        ratio = (float)spiralWidth / spiralHeight;

        eigth = (float)spiralHeight / 8;
        currDir = Direction.RIGHT;

        while (TransitionIn())
        {
            yield return null;
        }
        transitionOut = true;
        waitOnBlack = false;
        distanceNeededToTravel = 0;
        rotations = 5;
        right = 5;
        left = 5;
        down = 5;
        up = 5;

        targetWidth = 0;
        currDir = Direction.RIGHT;
        while (TransitionOut())
        {
            yield return null;
        }
    }

    public void SpiralOut(float spiralWidth, float spiralHeight, float x, float y)
    {
        StartCoroutine(SpiralingOut(spiralWidth, spiralHeight, x, y));
    }

    IEnumerator SpiralingOut(float spiralWidth, float spiralHeight, float x, float y)
    {
        this.x = x;
        this.y = y;

        this.spiralHeight = spiralHeight;
        this.spiralWidth = spiralWidth;
        transitionOut = true;
        waitOnBlack = false;
        distanceNeededToTravel = 0;
        rotations = 5;
        right = 5;
        left = 5;
        down = 5;
        up = 5;
        ratio = (float)spiralWidth / spiralHeight;

        eigth = (float)spiralHeight / 8;
        currDir = Direction.RIGHT;

        targetWidth = 0;
        currDir = Direction.RIGHT;
        while (TransitionOut(0, 800))
        {
            yield return null;
        }
    }
    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}
