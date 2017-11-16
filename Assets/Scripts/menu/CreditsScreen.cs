using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//NOTE PART OF THIS SCRIPT WAS COPYPASTED FROM TitleScreen.cs SO SOME CODE MAY BE OBSOLETE
public class CreditsScreen : GridOptions
{

    public TitleScreen mainWindow;

    public override void OpenMenu()
    {
        gameObject.SetActive(true);
        canControl = true;
    }

    public override void CloseMenu()
    {
        gameObject.SetActive(false);
        canControl = false;
    }

    private void OnEnable()
    {
        listTexts = transform.GetComponentsInChildren<RectTransform>();
        RectTransform[] temp = new RectTransform[1];
        for (int i = listTexts.Length - 1; i < listTexts.Length; i++)
        {
            temp[i - (listTexts.Length - 1)] = listTexts[i];
        }
        listTexts = temp;
        UpdateCursor(listTexts, 0);
    }

    protected override void MakeMenuSelection(int menuIndex)
    {
        switch (menuIndex)
        {
            case (0):
                // Return to main menu
                mainWindow.OpenMenu();
                this.CloseMenu();
                mainWindow.GetComponent<GridOptions>().OpenMenu();
                break;
        }
    }

}
