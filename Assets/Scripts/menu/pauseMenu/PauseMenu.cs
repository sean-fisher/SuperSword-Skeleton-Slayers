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

    public Text goldText;
    public Text stepsText;
    public Text battlesText;
    public Text timeText;

    public Sprite knight;
    public Sprite archer;
    public Sprite mage;

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
            case (2):
                // Quit to title
                Debug.Log("Quit to title");
                GameObject.FindObjectOfType<SceneSwitcher>().SwitchToOtherScene("Title");
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

            if (waitFrame && !cursorMoved && (Input.GetButtonDown("StartButton") || Input.GetButtonDown("BButton")))
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
        Debug.Log("open menu");
        gameObject.SetActive(true);
        canControl = true;
        waitFrame = false;
        InitMenu();
    }

    public override void CloseMenu()
    {
        gameObject.SetActive(false);
        GameManager.gm.leader.canMove = true;
        BattleManager.hpm.EnablePartyMovement();
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
    bool intiialize = false;
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

        if (!intiialize)
        {
            UpdateHeroDisplay();
            intiialize = true;
        }
        tempRectList.Clear();
        for (int i = 0; i < additionalStatsTexts.Length; i++)
        {
            tempRectList.Add(additionalStatsTexts[i].GetComponent<RectTransform>());
        }

        goldText.text = "Gold: " + Inventory.partyGold + "G";
        stepsText.text = "Steps Taken: " + GridController.stepsTaken;
        battlesText.text = "Battles Won: " + BattleManager.battlesFought;
        timeText.text = "Time Played: " + Time.timeSinceLevelLoad;

        additionalStats = tempRectList.ToArray();
        currCursor = cursor2;
        cursor.SetActive(true);
        visibleSize = mainOptions.Length;
        rows = visibleSize;
        cols = 1;
        UpdateCursor(mainOptions, 0, 0, -Screen.width / 30);
    }

    public void UpdateHeroDisplay()
    {
        // Display the current hereoes and stats

        HeroDisplayPanel[] fourMenuDisplays = heroDisplayHolder.GetComponentsInChildren<HeroDisplayPanel>(true);

        for (int i = 0; i < fourMenuDisplays.Length; i++)
        {
            if (i < BattleManager.hpm.activePartyMembers.Count)
            {
                fourMenuDisplays[i].gameObject.SetActive(true);
                fourMenuDisplays[i].GetComponent<HeroDisplayPanel>().UpdateDisplay(BattleManager.hpm.activePartyMembers[i]);
            }
            else
            {
                fourMenuDisplays[i].gameObject.SetActive(false);
            }
        }
    }
}
