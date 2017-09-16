using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public bool debugEnabled = false;

    public FadeTransition fadeTransition;
    public FourSideTransition fsTransition;
    public SpiralTransition spiralTransition;

    public GridController leader;

    public static GameManager gm;
    public static AreaNames currAreaName;

    public GameObject cursor;

    public AllEffects allEffects;

    private void Start()
    {
        fadeTransition = GetComponent<FadeTransition>();
        fsTransition = GetComponent<FourSideTransition>();
        gm = this;
        currAreaName = AreaNames.GRASSLAND;

        allEffects = GetComponent<AllEffects>();
    }

    public void SwitchScene(TransitionType transitionType, string sceneName)
    {
        StartCoroutine(SwitchingScene(transitionType, sceneName));
    }

    IEnumerator SwitchingScene(TransitionType transitionType, string sceneName)
    {
        ScreenTransition st = null;
        for (int i = 0; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].gameObject.GetComponent<GridController>().canMove = false;
        }

        switch (transitionType)
        {
            case (TransitionType.FADE):
                st = fadeTransition;
                break;
            case (TransitionType.FOURSIDE):
                st = fsTransition;
                break;
            default:
                Debug.Log("Invalid Scene Name!");
                break;
        }


        st.enabled = true;
        st.transitioning = true;
        while (st.TransitionIn())
        {
            yield return null;
        }
        Debug.Log("Scene trans in");
        if (sceneName != null)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        for (int i = 1; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].transform.position = BattleManager.hpm.activePartyMembers[0].transform.position;
        }
        yield return new WaitForSeconds(1);
        while (st.TransitionOut())
        {
            yield return null;
        }
        for (int i = 0; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].gameObject.GetComponent<GridController>().inputList.Clear();
            BattleManager.hpm.activePartyMembers[i].gameObject.GetComponent<GridController>().canMove = true;
        }
        enabled = false;

        Debug.Log("Scene switch successful");
    }
}
