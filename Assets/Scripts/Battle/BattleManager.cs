using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/**
 * Stores information and has logic for the battle system. 
 * BattleMenu contains code involving player input, visual stuff, etc., 
 * but it would make sense to merge the classes. There is no good reason they are separate, 
 * especially since the UI objects are referenced here.
     */
public class BattleManager : MonoBehaviour {

    public static BattleManager bManager;
    public AreaEncounters areaEncounters;

    public static HeroPartyManager hpm;
    public static PartyManager epm;

    public List<Turn> turnList;

    public HeroDisplayPanel defaultHeroDisplay;

    public Image backgroundWindow;
    public Image enviroImg;
    public Image messageBoxImg;
    public Transform allHeroStats;
    public Texture2D whiteTexture;

    public BattleMenu battleMenu;

    public int heroesAlive = 0;
    public int enemiesAlive = 0;

    public static bool hasWon = false;
    public static bool hasLost = false;

    public StandardAttack defaultAttackPrefab;
    public Wait defaultWaitPrefab;
    public Defend defaultDefendPrefab;

    void Start()
    {
        if (bManager == null)
        {
            bManager = this;
        }
        if (hpm == null)
        {
            hpm = gameObject.GetComponent<HeroPartyManager>();
        }
        if (Attack.battleMessageWindow == null)
        {
            Attack.battleMessageWindow = messageBoxImg.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartBattle();
        }
    }

    public virtual void KillCharacter(BaseCharacter targetCharacter)
    {
        targetCharacter.currentHP = 0;
        targetCharacter.isDead = true;

        if (epm.activePartyMembers.Contains(targetCharacter))
        {
            // An enemy was defeated!
            epm.activePartyMembers.Remove(targetCharacter);
            GameObject explosion = GameObject.Instantiate(GameManager.gm.allEffects.smokeExplosion.gameObject);
            explosion.transform.position = targetCharacter.gameObject.transform.position;
            Destroy(targetCharacter.gameObject);
            enemiesAlive--;
            Debug.Log("Enemy dies: " + targetCharacter.characterName);

            CheckWin();
        }
        else if (hpm.activePartyMembers.Contains(targetCharacter))
        {
            // A hero was defeated!
            Debug.Log("Hero dies: " + targetCharacter.characterName);

            CheckLose();
        }

    }

    public void StartBattle()
    {
        epm = GameObject.Instantiate(areaEncounters.GetRandomEncounter(GameManager.currAreaName).gameObject)
            .GetComponent<EnemyPartyManager>();

        int heroDisplayCount = allHeroStats.transform.childCount;
        for (int k = 0; k < heroDisplayCount; k++)
        {
            Destroy(allHeroStats.transform.GetChild(0).gameObject);
        }
        
        // Activate the panels that display HP, MP, etc.
        for (int i = 0; i < hpm.activePartyMembers.Count; i++)
        {
            HeroDisplayPanel heroDispPanel = GameObject.Instantiate(
                defaultHeroDisplay.gameObject, allHeroStats).GetComponent<HeroDisplayPanel>();
            heroDispPanel.InitiatePanel(hpm.activePartyMembers[i]);

            hpm.activePartyMembers[i].GetComponent<GridController>().canMove = false;

            battleMenu.heroDisplayMidHeight = heroDispPanel.transform.position.y;
        }

        heroesAlive = hpm.activePartyMembers.Count;
        

        // Instantiate the Enemy sprites
        for (int j = 0; j < epm.activePartyMembers.Count; j++)
        {
            GameObject enemyObj = GameObject.Instantiate(epm.activePartyMembers[j].gameObject);
            epm.activePartyMembers[j] = enemyObj.GetComponent<BaseCharacter>();
            Vector3 tempScale = enemyObj.transform.lossyScale;
            enemyObj.transform.SetParent(enviroImg.transform, false);
            tempScale = new Vector3(tempScale.x / enviroImg.transform.localScale.x, 
                tempScale.y / enviroImg.transform.localScale.y, tempScale.z / enviroImg.transform.localScale.z);
            enemyObj.transform.localScale = tempScale;
            enemyObj.SetActive(false);
        }

        heroesAlive = epm.activePartyMembers.Count;

        OpenWindow();
    }

    public void OpenWindow()
    {
        StartCoroutine(OpeningWindow());
    }

    IEnumerator OpeningWindow()
    {
        GameManager.gm.leader.DisableMovement();
        float enviroWidth = enviroImg.rectTransform.rect.width;
        float enviroHeight = enviroImg.rectTransform.rect.height;

        RectTransform rt = backgroundWindow.rectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 24);
        backgroundWindow.gameObject.SetActive(true);

        while (rt.sizeDelta.y < 400)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + (1000 * Time.deltaTime));
            yield return null;
        }
        yield return new WaitForSeconds(.1f);
        enviroImg.gameObject.SetActive(true);

        // A [Enemyname] has appeared! message scrolls
        TextBoxManager.tbm.EnableTextBox(messageBoxImg.transform.GetChild(0).gameObject, "Battle start", false);

        GameManager.gm.spiralTransition.SpiralOut((float)((float)Screen.width * .75f), (float)(Screen.height * .75f), (float)((float)Screen.width / 8.1f), (float)(Screen.height / 8.1f));
        yield return new WaitForSeconds(.5f);

        // Enable the enemy sprites
        for (int i = 0; i < enviroImg.transform.childCount; i++)
        {
            enviroImg.transform.GetChild(i).gameObject.SetActive(true);
        }

        messageBoxImg.gameObject.SetActive(true);
        allHeroStats.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        // Player now chooses actions

        messageBoxImg.gameObject.SetActive(false);
        battleMenu.InitMenu();
    }
    


    public void AddStandardAttackTurn(BaseCharacter attacker, BaseCharacter target)
    {
        turnList.Add(new Turn(attacker, target, defaultAttackPrefab));
    }

    public void AddWaitTurn(BaseCharacter waiter)
    {
        turnList.Add(new Turn(waiter, null, defaultWaitPrefab));
    }

    public void AddDefendTurn(BaseCharacter defender)
    {
        turnList.Add(new Turn(defender, null, defaultDefendPrefab));
    }

    void CheckWin()
    {
        if (enemiesAlive == 0)
        {
            Debug.Log("Game Won!");
            hasWon = true;
            turnList.Clear();

            Destroy(epm.gameObject);

            messageBoxImg.gameObject.SetActive(true);
            TextBoxManager.tbm.EnableTextBox(messageBoxImg.transform.GetChild(0).gameObject, "you've Won!");
        }
    }

    void CheckLose()
    {
        if (heroesAlive == 0)
        {
            Debug.Log("Game Lost...");
            hasLost = true;
        }
    }


    // If the battle menu is active, it will be disabled and allow the player to resume movement.
    // Called from GridController's EnableMovement()
    public void CheckDisableMenu()
    {
        if (battleMenu.menuActive)
        {
            battleMenu.menuActive = false;
            StartCoroutine(ClosingWindow());
        }
    }

    IEnumerator ClosingWindow()
    {
        yield return null;
        backgroundWindow.gameObject.SetActive(false);
        enviroImg.gameObject.SetActive(false);
        backgroundWindow.gameObject.SetActive(false);
        enviroImg.gameObject.SetActive(false);
        messageBoxImg.gameObject.SetActive(false);
        allHeroStats.gameObject.SetActive(false);

        int heroDisplayCount = allHeroStats.transform.childCount;

        for (int i = 0; i < heroDisplayCount; i++)
        {
            Destroy(allHeroStats.transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < hpm.activePartyMembers.Count; i++)
        {
            hpm.activePartyMembers[i].GetComponent<GridController>().canMove = true;
        }
        int attackCount = transform.childCount;
        for (int i = 0; i < attackCount; i++)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void StartInactiveTurn(Turn turn, List<Turn> turnList)
    {
        StartCoroutine(StartingInactiveTurn(turn, turnList));
        //StartCoroutine(turn.attack.UseAttack(turn.attacker, turn.target, turnList));
    }

    IEnumerator StartingInactiveTurn(Turn turn, List<Turn> turnList)
    {
        GameObject attackObj = GameObject.Instantiate(turn.attack.gameObject, transform);
        yield return null;
        StartCoroutine(attackObj.GetComponent<Attack>().UseAttack(turn.attacker, turn.target, turnList));
    }
}
