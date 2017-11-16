using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public bool debugEnabled = false;

    public PauseMenu pauseMenu;

    public FadeTransition fadeTransition;
    public FourSideTransition fsTransition;
    public SpiralTransition spiralTransition;

    public GridController leader;

    public static GameManager gm;
    public static ContinentType currAreaName;
    public Inventory inventory;

    public GameObject cursor;

    public AllEffects allEffects;

    public GameObject circleOfLight;
    public Image redScreen;

    public AudioSource musicPlayer;


    private void Start()
    {
        Cactus.redScreen = redScreen;
        fadeTransition = GetComponent<FadeTransition>();
        fsTransition = GetComponent<FourSideTransition>();
        gm = this;
        currAreaName = ContinentType.GRASSLAND;

        allEffects = GetComponent<AllEffects>();
        inventory = GetComponent<Inventory>();

        PauseMenu.pauseMenu = pauseMenu;
    }

    public void SwitchScene(TransitionType transitionType, string sceneName, 
        bool enableDarkness, ContinentType mazeType = ContinentType.None)
    {
        StartCoroutine(SwitchingScene(transitionType, sceneName, 
            enableDarkness, mazeType));
    }

    public void SwitchScene(MapEntrance mapEntrance)
    {
        StartCoroutine(SwitchingScene(mapEntrance.transitionType,
            mapEntrance.sceneToLoad,
            mapEntrance.enableDarkness, mapEntrance.mazeToGenerate));
    }

    public void GoThruDoor(MapEntrance mapEntrance, bool createMaze = false)
    {
        StartCoroutine(GoingThruDoor(mapEntrance));
    }

    IEnumerator GoingThruDoor(MapEntrance mapEntrance)
    {
        BattleManager.hpm.DisablePartyMovement();
        GridController.partyCanMove = false;

        ScreenTransition st = null;
        for (int i = 0; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].gameObject.GetComponent<GridController>().canMove = false;
        }

        switch (mapEntrance.transitionType)
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
        if (mapEntrance.sceneToLoad != "")
        {
            SceneManager.LoadScene(mapEntrance.sceneToLoad, LoadSceneMode.Single);
        }
        else if (!mapEntrance.generateMap)
        {
            if (!mapEntrance.entersIntoMaze)
            {
                Debug.Log("enter maze: " + mapEntrance.mazeToGenerate);
                // Move party to exit, usually on world map
                BattleManager.hpm.MovePartyTo(new Vector2(mapEntrance.exitPosition.x, mapEntrance.exitPosition.y - 4));
                MazeGenerator.inMaze = false;
                MazeGenerator.SetGroundType(mapEntrance.mazeToGenerate, true);
                GridController.canWrapMap = true;
            } else
            {
                // enter maze that has already been generated
                MazeGenerator.inMaze = true;
                MazeGenerator.mazeGenerator.PlacePartyAtMazeEntrance
                    (ContinentType.GRASSLAND);
                MazeGenerator.SetGroundType(mapEntrance.mazeToGenerate, false);
                GridController.canWrapMap = false;
                AreaEncounters.aeinstance.SetCurrArea(mapEntrance.mazeToGenerate);
            }
        } else
        {
            // generate new maze and enter it
            MazeGenerator.inMaze = true;
            MazeGenerator.mazeGenerator.GenerateMaze(mapEntrance.mazeToGenerate,
                GameManager.gm.leader.gameObject.transform.position);
            mapEntrance.generateMap = false;
            MazeGenerator.mazeGenerator.PlacePartyAtMazeEntrance(ContinentType.FOREST);
            MazeGenerator.SetGroundType(mapEntrance.mazeToGenerate, false);
            GridController.canWrapMap = false;
        }
        for (int i = 1; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].transform.position = BattleManager.hpm.activePartyMembers[0].transform.position;
        }
        if (mapEntrance.enableDarkness)
        {
            circleOfLight.SetActive(true);
        }
        else
        {
            circleOfLight.SetActive(false);
        }
        bool changeSong = false;
        if (mapEntrance.playOnExit != null && mapEntrance.playOnExit != Songs.bgmmusicPlayer.clip)
        {
            changeSong = true;
            Songs.bgmmusicPlayer.Stop();
        }

        yield return new WaitForSeconds(1);

        if (changeSong)
        {
            Songs.bgmmusicPlayer.clip = mapEntrance.playOnExit;
            Songs.bgmmusicPlayer.Play();
        }

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
        BattleManager.hpm.ClearMoveQueues();

        BattleManager.hpm.EnablePartyMovement();
    }

    IEnumerator DestroyingChildrenOf(Transform parent)
    {
        int childCount = parent.childCount;
        Debug.Log("Destroy " + parent.name + " " + childCount);
        int i = 0;
        for (i = 0; i < childCount; i++)
        {
            Destroy(parent.GetChild(0).gameObject);
            if (i % 64 == 0)
            {
                yield return null;
            }
        }
        Debug.Log("End: " + i);
    }

    public void SwitchScene(TransitionType transitionType, Vector3 positionToMoveTo)
    {
        StartCoroutine(SwitchingScene(transitionType, positionToMoveTo));
    }

    IEnumerator SwitchingScene(TransitionType transitionType, string sceneName, 
        bool enableDarkness, ContinentType mazeType = ContinentType.None)
    {
        BattleManager.hpm.DisablePartyMovement();
        GridController.partyCanMove = false;

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
        if (sceneName != "")
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        } else
        {
            Debug.Log("this shouldn't be called");
            MazeGenerator.mazeGenerator.GenerateMaze(mazeType, 
                GameManager.gm.leader.gameObject.transform.position);
        }
        for (int i = 1; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].transform.position = BattleManager.hpm.activePartyMembers[0].transform.position;
        }
        if (enableDarkness)
        {
            circleOfLight.SetActive(true);
        } else
        {
            circleOfLight.SetActive(false);
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
        BattleManager.hpm.ClearMoveQueues();

        Debug.Log("Scene switch successful");
        BattleManager.hpm.EnablePartyMovement();
    }

    IEnumerator SwitchingScene(TransitionType transitionType, 
        Vector3 positionToMoveTo)
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

        for (int i = 1; i < BattleManager.hpm.activePartyMembers.Count; i++)
        {
            BattleManager.hpm.activePartyMembers[i].transform.position = positionToMoveTo;//BattleManager.hpm.activePartyMembers[0].transform.position;
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
