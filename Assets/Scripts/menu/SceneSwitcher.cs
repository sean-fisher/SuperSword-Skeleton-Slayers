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


    public void SwitchToMapScene(string sceneName)
    {
        StartCoroutine(SwitchingScenesWithMapGen(sceneName));
    }

    IEnumerator SwitchingScenesWithMapGen(string sceneName)
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
        yield return new WaitForSeconds(.5f);
        Camera.main.transform.position = 
            new Vector3(3000, 3000, Camera.main.transform.position.z);
        
        while (st.TransitionOut())
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        while (st.TransitionIn())
        {
            yield return null;
        }
        Camera.main.GetComponent<CamFollow>().canFollow = true;
        while (st.TransitionOut())
        {
            yield return null;
        }

        isSwitching = false;
    }
}
