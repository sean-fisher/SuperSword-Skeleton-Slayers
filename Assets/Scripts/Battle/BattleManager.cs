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
    public static EnemyPartyManager epm;

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
    public ItemTurn itemTurnPrefab;
    public Wait defaultWaitPrefab;
    public Defend defaultDefendPrefab;
    public Run defaultRunPrefab;

    public LayerMask groundLayer;
    public LayerMask oceanLayer;

    public static int battlesFought = 0;

    int totalGoldDrop = 0;

    void Start()
    {
            bManager = this;
        
            hpm = gameObject.GetComponent<HeroPartyManager>();
        
            Attack.battleMessageWindow = messageBoxImg.gameObject;
            TextBoxManager.defaultMessageWindow = messageBoxImg.gameObject;

        battlesFought = 0;
        hasLost = false;
        hasWon = false;

        isFightingFinalBoss = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartBattle();
        }
    }

    public virtual void KillCharacter(BaseCharacter targetCharacter, bool checkWinLose = true)
    {
        targetCharacter.currentHP = 0;
        targetCharacter.isDead = true;

        if (epm.activePartyMembers.Contains(targetCharacter))
        {
            // An enemy was defeated!
            epm.activePartyMembers.Remove(targetCharacter);
            StartCoroutine(EnemyDeathAnimation(targetCharacter.gameObject));
        }
        else if (hpm.activePartyMembers.Contains(targetCharacter))
        {
            // A hero was defeated!
            Debug.Log("Hero dies: " + targetCharacter.characterName + " " + heroesAlive);
            //hpm.activePartyMembers.Remove(targetCharacter);
            heroesAlive--;

            if (checkWinLose)
            {
                CheckLose();
            }
        }
    }

    IEnumerator EnemyDeathAnimation(GameObject enemyObj)
    {
        GameObject explosion = GameObject.Instantiate(GameManager.gm.allEffects.smokeExplosion.gameObject, enemyObj.gameObject.transform.parent);
        explosion.transform.position = enemyObj.gameObject.transform.position;
        //enemyObj.transform.parent = enemyObj.transform.parent.transform.parent;
        float frameCount = 0;
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            frameCount += Time.deltaTime;
            if (frameCount > .2f)
            {
                frameCount = 0;
                enemyObj.GetComponent<Image>().enabled = !enemyObj.GetComponent<Image>().enabled;
            }
            yield return null;
        }
        Destroy(enemyObj);
        enemiesAlive--;

        CheckWin();
        if (!BattleManager.hasLost && !BattleManager.hasWon)
        {
            //yield return new WaitForSeconds(1);
            //battleMenu.ReturnToMenu();
        }
    }

    public static bool isFightingFinalBoss = false;

    public void StartBattle(string[] introMessage = null, 
        EnemyPartyManager enemyEncounter = null)
    {
        hasLost = false;
        hasWon = false;
        Sounds.audioSource.clip = Sounds.encounter;
        Sounds.audioSource.Play();

        Songs.bgmmusicPlayer.Pause();
        Songs.songPlayer.disableOverworldLoop = true;
        Songs.songPlayer.disableBattleLoop = false;

        // Disable player movement
        GameManager.gm.leader.DisableMovement();

        if (enemyEncounter == null)
        {
            Collider2D currentTile = Physics2D.OverlapCircle(GameManager.gm.leader.transform.position, 4, groundLayer);

            GroundTile ground = currentTile.GetComponent<GroundTile>();
            if (ground)
            {
                if (!MazeGenerator.inMaze)
                {
                    RandomEncounterManager.SetCurrArea(ground.tileChar);
                }
            } else
            {
                Debug.Log("Invalid Ground");
            }

            epm = GameObject.Instantiate(areaEncounters
                .GetRandomEncounter(RandomEncounterManager.currArea).gameObject)
                .GetComponent<EnemyPartyManager>();
            //Debug.Log("encounter at " + RandomEncounterManager.currArea);
        } else
        {
            epm = enemyEncounter;
        }

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

        totalGoldDrop = 0;
        
        // Set the background image
        if (epm.background != null)
        {
            enviroImg.sprite = epm.background;
        }

        // The Skeleton King's body parts shouldn't be lined up, 
        // so disable the horizontal layout group.
        if (isFightingFinalBoss)
        {
            Songs.battlemusicPlayer.clip = Songs.bossMusic;
            Songs.battlemusicPlayer.Play();
            enviroImg.GetComponent<HorizontalLayoutGroup>().enabled = false;
        } else
        {
            Songs.songPlayer.PlayIntroThenLoop(Songs.battlemusicPlayer,
                Songs.battleIntro, Songs.battleMusic);
            enviroImg.GetComponent<HorizontalLayoutGroup>().enabled = true;
            enviroImg.sprite = AreaEncounters.currBackground;
        }

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

            if (isFightingFinalBoss)
            {
                enemyObj.GetComponent<RectTransform>().localScale *= 2;
                enemyObj.GetComponent<RectTransform>().position += new Vector3((enviroImg.rectTransform.rect.width / 2) * 1.5f, 
                    (-enviroImg.rectTransform.rect.height / 2) * 1.3f);
                enemyObj.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height / 2);
            }

            totalGoldDrop += enemyObj.GetComponent<BaseCharacter>().goldDrop;
        }

        enemiesAlive = epm.activePartyMembers.Count;

        OpenWindow(introMessage);
    }

    public void OpenWindow(string[] introMessage = null)
    {
        StartCoroutine(OpeningWindow(introMessage));
    }

    IEnumerator OpeningWindow(string[] introMessage = null)
    {
        GameManager.gm.leader.DisableMovement();
        float enviroWidth = enviroImg.rectTransform.rect.width;
        float enviroHeight = enviroImg.rectTransform.rect.height;

        RectTransform rt = backgroundWindow.rectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 24);
        backgroundWindow.gameObject.SetActive(true);

        while (rt.sizeDelta.y < 323)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + (1000 * Time.deltaTime));
            yield return null;
        }
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 323);
        yield return new WaitForSeconds(.1f);
        enviroImg.gameObject.SetActive(true);

        // A [Enemyname] has appeared! message scrolls
        if (introMessage == null)
        {
            TextBoxManager.tbm.EnableTextBox(messageBoxImg.transform.GetChild(0).gameObject, "Battle start", false);
        } else
        {
            TextBoxManager.tbm.EnableTextBox(messageBoxImg.transform.GetChild(0).gameObject, introMessage, false);
        }

        GameManager.gm.spiralTransition.SpiralOut((float)((float)Screen.width * .73f), 
            (float)(Screen.height * .605f), (float)((float)Screen.width / 7.3f),
            (float)(Screen.height / 5.05f));
        yield return new WaitForSeconds(.5f);


        // Enable the enemy sprites
        for (int i = 0; i < enviroImg.transform.childCount; i++)
        {
            enviroImg.transform.GetChild(i).gameObject.SetActive(true);
        }

        messageBoxImg.gameObject.SetActive(true);
        allHeroStats.gameObject.SetActive(true);

        while (TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        // Player now chooses actions

        messageBoxImg.gameObject.SetActive(false);
        battleMenu.InitMenu();
    }


    public void AddAttackTurn(BaseCharacter attacker, BaseCharacter target, int attackIndex)
    {
        turnList.Add(new Turn(attacker, target, attacker.usableAttacks[attackIndex]));
    }

    public List<ItemData> itemsToUse = new List<ItemData>();

    public void AddItemUseTurn(BaseCharacter attacker, BaseCharacter target, ItemData item)
    {
        itemTurnPrefab.itemBeingUsed = item;
        itemsToUse.Add(item);
        turnList.Add(new Turn(attacker, target, itemTurnPrefab));
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

    public void AddRunTurn(BaseCharacter runner)
    {
        turnList.Add(new Turn(runner, null, defaultRunPrefab));
    }

    public void RunAway()
    {
        Songs.songPlayer.FadeOut(Songs.battlemusicPlayer, 1);
        turnList.Clear();

        Destroy(epm.gameObject);

        CheckDisableMenu();
    }

    public void CheckWin()
    {
        if (!hasWon && enemiesAlive == 0)
        {
            StartCoroutine(WaitThenWin());
        }
    }

    IEnumerator WaitThenWin()
    {
        while (Explosion.isExploding || TextBoxManager.tbm.isTyping)
        {
            yield return null;
        }
        WinBattle();
    }

    public void WinBattle()
    {
        Songs.battlemusicPlayer.Stop();
        Songs.songPlayer.disableBattleLoop = true;

        hasWon = true;
        turnList.Clear();

        if (epm)
        {
            Destroy(epm.gameObject);
        }

        if (isFightingFinalBoss)
        {
            List<string> endBattleMessages = new List<string>();
            endBattleMessages.Add("NoooOOOOooooOO!!!!! How could you defeat meeeeee!!!!???!?!?!?");
            TextBoxManager.tbm.EnableTextBox(messageBoxImg.transform.GetChild(0).gameObject, endBattleMessages.ToArray(), false);
            StartCoroutine(WaitThenSwitch(5));
        } else
        {

            messageBoxImg.gameObject.SetActive(true);
            List<string> endBattleMessages = new List<string>();
            endBattleMessages.Add("You've Won!");
            endBattleMessages.Add("The defeated enemies dropped " + totalGoldDrop + " Gold!");
            Inventory.partyGold += totalGoldDrop;
            TextBoxManager.tbm.EnableTextBox(messageBoxImg.transform.GetChild(0).gameObject, endBattleMessages.ToArray(), true);
            battlesFought++;


            Songs.battlemusicPlayer.clip = Songs.victoryMusic;
            Songs.battlemusicPlayer.Play();
            Songs.battlemusicPlayer.loop = false;
        }
    }

    IEnumerator WaitThenSwitch(int time)
    {
        yield return new WaitForSeconds(time);
        SceneSwitcher.ss.SwitchToOtherScene("EndGame");
    }

    IEnumerator WaitForExplode()
    {
        yield return new WaitForSeconds(.7f);
    }

    public void CheckLose()
    {
        if (heroesAlive <= 0)
        {
            hasLost = true;
            GameOver();
        }
    }

    public void GameOver()
    {
        messageBoxImg.gameObject.SetActive(true);
        TextBoxManager.tbm.EnableTextBox(messageBoxImg.transform.GetChild(0).gameObject, "Game Over...");
        StartCoroutine(WaitThenReturnToTitle());
    }

    IEnumerator WaitThenReturnToTitle()
    {
        yield return new WaitForSeconds(2);
        GameObject.FindObjectOfType<SceneSwitcher>().SwitchToOtherScene("Title");
    }


    // If the battle menu is active, it will be disabled and allow the player to resume movement.
    // Called from GridController's EnableMovement()
    public bool CheckDisableMenu()
    {
        if (battleMenu.menuActive)
        {
            battleMenu.menuActive = false;
            StartCoroutine(ClosingWindow());
            return true;
        }
        return false;
    }

    IEnumerator ClosingWindow()
    {
        int enemyCount = enviroImg.transform.childCount;

        for (int i = 0; i < enemyCount; i++)
        {
            Destroy(enviroImg.transform.GetChild(i).gameObject);
        }
        yield return new WaitForSeconds(.4f);
        backgroundWindow.gameObject.SetActive(false);
        enviroImg.gameObject.SetActive(false);
        backgroundWindow.gameObject.SetActive(false);
        enviroImg.gameObject.SetActive(false);
        messageBoxImg.gameObject.SetActive(false);
        allHeroStats.gameObject.SetActive(false);


        int heroDisplayCount = allHeroStats.transform.childCount;

        for (int i = 0; i < heroDisplayCount; i++)
        {
            Destroy(allHeroStats.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < hpm.activePartyMembers.Count; i++)
        {
            hpm.activePartyMembers[i].GetComponent<GridController>().canMove = true;
        }
        int attackCount = transform.childCount;
        for (int i = 0; i < attackCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        Songs.bgmmusicPlayer.Play();
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
        if (!turn.attacker.isDead) { 
            StartCoroutine(attackObj.GetComponent<Attack>().UseAttack(turn.attacker, turn.target, turnList));
        } else
        {
            // attacker is dead
            if (turnList.Count > 0)
            {
                Turn currTurn = turnList[0];
                turnList.RemoveAt(0);
                StartInactiveTurn(currTurn, turnList);
            } else
            {
                turn.attack.EndTurnCheck(turnList);
            }
        }
    }
}
