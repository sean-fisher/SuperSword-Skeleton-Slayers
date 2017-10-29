using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public ScreenTransition st;

    public bool isSwitching = false;
    
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
}
