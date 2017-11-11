using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public bool isDummy;
    public ScreenTransition st;
    public static ScreenTransition staticst;

    public static SceneSwitcher ss;

    public bool isSwitching = false;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MapGenTest") { 
            StartCoroutine(DummyRoutine());
        }
        if (st)
        {
            staticst = st;
        }
        if (ss == null)
        {

            ss = this;
        }
    }

    public void SwitchToOtherScene(string sceneName)
    {
        StartCoroutine(SwitchingScenes(sceneName));
    }

    IEnumerator SwitchingScenes(string sceneName)
    {
        isSwitching = true;
        st.enabled = true;

        while (st.TransitionIn())
        {
            yield return null;
        }

        if (sceneName != "")
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        while (st.TransitionOut())
        {
            yield return null;
        }

        isSwitching = false;
    }


    public void SwitchToMapScene(string sceneName)
    {
        StartCoroutine(SwitchingScenesWithMapGen(sceneName));
    }

    IEnumerator SwitchingScenesWithMapGen(string sceneName)
    {
        Debug.Log("Switch to map gen");
        isSwitching = true;
        st.enabled = true;
        
        // fade to black from title screen
        while (st.TransitionIn())
        {
            yield return null;
        }

        if (sceneName != "")
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        Debug.Log("Finished loading into new scene");

        StartCoroutine(DummyRoutine());
    }

    IEnumerator DummyRoutine()
    {
        Debug.Log("Start dummy routine");

        //((FadeTransition)st).SetBlacked();
        yield return new WaitForSeconds(.5f);
        // set cam position at animating airship sprite
        Camera.main.transform.position =
            new Vector3(3000, 3000, Camera.main.transform.position.z);

        Debug.Log("Move cam 3000");
        // fade in to show airship
        Debug.Log("Start Transition out");
        int timeout = 50;
        while (st.TransitionOut() && timeout > 0)
        {
            timeout--;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        // fade to black from airship
        Debug.Log("Start Transition in");
        while (st.TransitionIn())
        {
            yield return null;
        }
        Debug.Log("Cam can follow");
        Camera.main.GetComponent<CamFollow>().canFollow = true;
        GameManager.gm.musicPlayer.Play();
        // fade in to game
        while (st.TransitionOut())
        {
            yield return null;
        }

        isSwitching = false;
        Debug.Log("finish scene switch");
    }
}
