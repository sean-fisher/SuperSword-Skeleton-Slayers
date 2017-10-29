using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE PART OF THIS SCRIPT WAS COPYPASTED FROM TitleScreen.cs SO SOME CODE MAY BE OBSOLETE
public class CreditsScreen : GridOptions
{

    public GameObject mainWindow;


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
        RectTransform[] temp = new RectTransform[1];
        for (int i = listTexts.Length - 1; i < listTexts.Length; i++)
        {
            temp[i - (listTexts.Length - 1)] = listTexts[i];
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
        Debug.Log("Make credits selection");
        switch (menuIndex)
        {
            case (0):
                // Return to main menu
                Debug.Log("Return");
                mainWindow.SetActive(true);
                this.gameObject.SetActive(false);
                break;
        }
    }

}
