using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeTransition : ScreenTransition {
    
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void OnEnable()
    {
    }

    int inOrOut = 1;
    public Texture2D blackTexture;

    public float speed;
    Color currGuiColor = Color.black;

    public void SetBlacked()
    {
        inOrOut = 1;
        transitioning = true;
        waitOnBlack = false;
        currGuiColor.a = .5f;
        /*
        GameManager.gm = GameObject.Find("Gamemanager").GetComponent<GameManager>();
        SceneSwitcher.staticst = GameManager.gm.fadeTransition;
        SceneSwitcher.staticst.enabled = true;*/
    }

    private void OnGUI()
    {
        if (transitioning && !waitOnBlack)
        {
            GUI.color = currGuiColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
            if (inOrOut > 0)
            {
                if (currGuiColor.a < 1)
                {
                    blackTexture = new Texture2D(1, 1);
                    //Debug.Log("Draw in");
                    currGuiColor.a += Time.deltaTime * speed * inOrOut;
                    blackTexture.SetPixel(0, 0, currGuiColor);
                    blackTexture.Apply();
                } else
                {
                }

            }
            else if (currGuiColor.a > 0)
            { 
                blackTexture = new Texture2D(1, 1);
                //Debug.Log("Draw out" + currGuiColor.a);
                currGuiColor.a += Time.deltaTime * speed * inOrOut;
                blackTexture.SetPixel(0, 0, currGuiColor);
                blackTexture.Apply();

            }
            
        }
    }
    
    public override bool TransitionIn(float widthRatio = 1, float speed = 1f)
    {
        if (currGuiColor.a == 1)
        {
            currGuiColor.a = 0;
        }
        transitioning = true;
        waitOnBlack = false;
        inOrOut = 1;
        return currGuiColor.a < 1;
    }

    public override bool TransitionOut(float widthRatio = 1, float speed = 1f)
    {
        transitioning = true;
        waitOnBlack = false;
        inOrOut = -1;
        transitioning = currGuiColor.a > 0;
        return transitioning;
    }
    
    public void Transition(string sceneName = null)
    {
        Debug.Log("Transition");
        StartCoroutine(TransitioningInOut(sceneName));
    }

    // Transitions out and then in. Waits one second between to allow changing onscreen objects if necessary

    IEnumerator TransitioningInOut(string sceneName)
    {
        enabled = true;
        transitioning = true;
        while (TransitionIn())
        {
            yield return null;
        }
        if (sceneName != null)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        yield return new WaitForSeconds(1);
        while (TransitionOut())
        {
            yield return null;
        }
        enabled = false;
    }
}
