using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : GridOptions {

    public SceneSwitcher sceneSwitcher;


    public override void OpenMenu()
    {
        throw new System.NotImplementedException();
    }

    public override void CloseMenu()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        listTexts = transform.GetComponentsInChildren<RectTransform>();
        RectTransform[] temp = new RectTransform[3];
        for (int i = 1; i < listTexts.Length; i++)
        {
            temp[i - 1] = listTexts[i];
        }
        listTexts = temp;

        //UpdateCursor(listTexts, 0, 0, 0);
        /*cursor.transform.position = new Vector3(
            listTexts[0].transform.position.x - (float)
            (Screen.width / displaceCursorByOneOverThisTimesScreenWidth) + 0,
            listTexts[0].transform.position.y + 4);*/
        Debug.Log(listTexts[0].gameObject.name);
    }

    protected override void MakeMenuSelection(int menuIndex)
    {
        Debug.Log("Make menu selection");
        switch (menuIndex)
        {
            case (0):
                // Start new game
                Debug.Log("Start game");
                sceneSwitcher.SwitchToOtherScene("MapGenTest");
                StartCoroutine(StartingGame());
                break;
            case (2):
                // View Controls
                break;
            case (3):
                // Go to credits
                break;
        }
    }

    IEnumerator StartingGame()
    {
        while (sceneSwitcher.isSwitching)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2);
    }
}
