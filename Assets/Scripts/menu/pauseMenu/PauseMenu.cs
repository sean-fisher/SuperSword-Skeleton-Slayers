using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : GridOptions {

    public Image heroDisplayHolder;
    public Image mainOptionsWindow;
    public Image additionalStatsWindow;

    
    public static ItemMenu itemMenu;
    public EquipMenu equipMenu;
    public static PartySelectMenu partySelectMenu;
    public static PauseMenu pauseMenu;


    public GameObject itemWindow; // TODO this should be a gridOptions

    Text[] mainOptionsTexts;
    Text[] additionalStatsTexts;
    RectTransform[] mainOptions;
    RectTransform[] additionalStats;

    // Use this for initialization
    void Start ()
    {
        pauseMenu = this;
        if (cursor == null)
        {
            cursor = GameManager.gm.cursor;
            currCursor = cursor;
        }

        if (cursor2 == null)
        {
            cursor2 = GameObject.Instantiate(cursor, cursor.transform.parent);
        }
        if (cursor3 == null)
        {
            cursor3 = GameObject.Instantiate(cursor, cursor.transform.parent);
        }


    }

    bool waitFrame = false;

    protected override void MakeMenuSelection(int menuIndex)
    {
        this.enabled = false;
        switch(menuIndex)
        {
            case (0):
                // Open Item Window
                menuOptions[0].OpenMenu();
                break;
            case (1):
                // Open Equip Window
                equipMenu.OpenMenu();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update () {
        if (canControl)
        {
            CheckInput<RectTransform>(mainOptions, 1, mainOptions.Length, null, true, 0, false, -Screen.width / 30);

            if (waitFrame && !cursorMoved && Input.GetButtonDown("StartButton"))
            {
                CloseMenu();
            }

            if (!waitFrame)
            {
                waitFrame = true;
            }
        }
    }

    public override void OpenMenu()
    {
        gameObject.SetActive(true);
        canControl = true;
        waitFrame = false;
        InitMenu();
    }

    public override void CloseMenu()
    {
        gameObject.SetActive(false);
        GameManager.gm.leader.canMove = true;
        cursor.SetActive(false);
        cursor3.SetActive(false);
        cursor2.SetActive(false);

        canControl = false;
        waitFrame = false;
    }

    public override void DisableMenuControl()
    {
        base.DisableMenuControl();
    }

    public void InitMenu()
    {
        if (cursor == null)
        {
            cursor = GameManager.gm.cursor;
            currCursor = cursor;
        }

        if (cursor2 == null)
        {
            cursor2 = GameObject.Instantiate(cursor, cursor.transform.parent);
        }
        if (cursor3 == null)
        {
            cursor3 = GameObject.Instantiate(cursor, cursor.transform.parent);
        }
        this.enabled = true;

        mainOptionsTexts = mainOptionsWindow.transform.GetComponentsInChildren<Text>();
        additionalStatsTexts = additionalStatsWindow.transform.GetComponentsInChildren<Text>();

        List<RectTransform> tempRectList = new List<RectTransform>();
        for (int i = 0; i < mainOptionsTexts.Length; i++)
        {
            tempRectList.Add(mainOptionsTexts[i].GetComponent<RectTransform>());
        }
        mainOptions = tempRectList.ToArray();

        tempRectList.Clear();
        for (int i = 0; i < additionalStatsTexts.Length; i++)
        {
            tempRectList.Add(additionalStatsTexts[i].GetComponent<RectTransform>());
        }
        additionalStats = tempRectList.ToArray();
        currCursor = cursor2;
        cursor.SetActive(true);
        visibleSize = mainOptions.Length;
        rows = visibleSize;
        cols = 1;
        UpdateCursor(mainOptions, 0, 0, -Screen.width / 30);
    }
}
