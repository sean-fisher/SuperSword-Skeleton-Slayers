using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : Menu {
    public HeroPartyManager hpm;
    public BattleManager bm;

    public GameObject defaultActionMenu;

    RectTransform[] actionSelect;
    RectTransform[] enemyRects;
    RectTransform[] heroRects;
    RectTransform[] itemRects;

    RectTransform[] currRects;

    public Transform listSelectWindow;
    public Transform itemSelectWindow;

    public Transform targetNameWindow;

    public ItemMenuBattle itemMenu;
    

    /**
     * Layer 0: Select from Attack, Defend, etc.
     *       1: Targeting an enemy/hero to attack
     *       2: Searching through items
     *       3: Searching through spells
     * */
    int menuLayer = 0;

    bool canSelect = false;

    int selectingHeroIndex = 0;

    public float heroDisplayMidHeight;

    private void Start()
    {
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
        currCursor = cursor;

        defaultActionMenu.SetActive(true);

        List<RectTransform> textList = new List<RectTransform>();

        // Finds the texts within "row" children of the menu
        foreach (Transform child in defaultActionMenu.transform)
        {
            foreach (Transform grandChild in child)
            {
                if (grandChild.GetComponent<Text>())
                {
                    textList.Add(grandChild.GetComponent<RectTransform>());
                }
            }
        }
        actionSelect = textList.ToArray();

        textList.Clear();
        // Finds the texts within row children of the menu
        foreach (Transform child in listSelectWindow)
        {
            foreach (Transform grandChild in child)
            {
                if (grandChild.GetComponent<Text>())
                {
                    textList.Add(grandChild.GetComponent<RectTransform>());
                }
            }
        }
        listTexts = textList.ToArray();


        textList.Clear();
        // Finds the texts within row children of the menu
        foreach (Transform child in itemSelectWindow)
        {
            foreach (Transform grandChild in child)
            {
                if (grandChild.GetComponent<Text>())
                {
                    textList.Add(grandChild.GetComponent<RectTransform>());
                }
            }
        }
        itemRects = textList.ToArray();

        //actionSelect = defaultActionMenu.transform.GetComponentsInChildren<RectTransform>();

        isScrollable = false;

        cursor.SetActive(true);
        UpdateCursor(actionSelect, 0);

        menuLayer = 0;
        menuActive = true;
        canSelect = true;
        visibleSize = 6;

        List<RectTransform> heroRectsList = new List<RectTransform>();
        for (int j = 0; j < bm.allHeroStats.transform.childCount; j++)
        {
            heroRectsList.Add(bm.allHeroStats.transform.GetChild(j).GetComponent<RectTransform>());
        }
        heroRects = heroRectsList.ToArray();

        selectingHeroIndex = -1;
        NextHeroSelectsAttack(false);
    }

    private void Update()
    {
        if (canSelect)
        {
            switch (menuLayer)
            {
                case (0):
                    if (!waitFrame)
                    {
                        waitFrame = true;
                    }
                    else
                    {
                        CheckInput<Text>(actionSelect, 3, 2);
                        if (aPressed)
                        {

                            // Looks at the different action menu options (Attack, Magic, item, etc.
                            switch (tempCursor)
                            {
                                case (0):
                                    // Attack selected; now target enemy
                                    attackIndex = 0;
                                    OpenMenu(1, 0);
                                    break;
                                case (1):
                                    // Player chooses to use magic
                                    OpenMenu(3, 0);
                                    break;
                                case (2):
                                    // Player chooses to use an item
                                    OpenMenu(2, 0);
                                    break;
                                case (3):
                                    // Player chooses to defend

                                    bm.AddDefendTurn(GetAttacker());
                                    NextHeroSelectsAttack(true);
                                    aPressed = false;
                                    break;
                                case (4):
                                    // Player chooses to wait
                                    bm.AddWaitTurn(GetAttacker());
                                    NextHeroSelectsAttack(true);
                                    aPressed = false;
                                    break;
                                case (5):
                                    // Player chooses to run
                                    bm.AddRunTurn(GetAttacker());
                                    NextHeroSelectsAttack(true);
                                    aPressed = false;
                                    break;
                            }
                        }
                        else if (bPressed)
                        {
                            waitFrame = false;
                            PreviousHeroSelectsAttack(true);
                        }
                    }
                    break;
                case (1): // Selecting a hero or enemy
                    CheckHeroesAndEnemies();
                    if (aPressed)
                    {
                        if (lastMenuLayer == 2)
                        {
                            if (!GetTarget().isDead) //TODO check if can revive
                            {
                                // Item used on target
                                itemSelectWindow.gameObject.SetActive(false);

                                Debug.Log(itemMenu.selectedItem.itemName);
                                BattleManager.bManager.AddItemUseTurn(
                                    GetAttacker(), GetTarget(), itemMenu.selectedItem);
                                aPressed = false;
                                NextHeroSelectsAttack(true);
                            }
                        }
                        else
                        {
                            if (attackIndex == 0)
                            {
                                BattleManager.bManager.AddStandardAttackTurn(GetAttacker(), GetTarget());
                            }
                            else
                            {
                                BattleManager.bManager.AddAttackTurn(GetAttacker(), GetTarget(), attackIndex - 1);
                            }

                            NextHeroSelectsAttack(true);
                            aPressed = false;
                        }
                        targetNameWindow.gameObject.SetActive(false);
                        //CloseMenu(lastMenuLayer, 1);
                    }
                    else if (bPressed)
                    {
                        // Go back to action selection
                        menuLayer = lastMenuLayer;
                        OpenMenu(lastMenuLayer);
                        waitFrame = false;

                        if (lastMenuLayer != 2)
                        {
                            UpdateCursor(currRects, 0);
                        }
                        targetNameWindow.gameObject.SetActive(false);
                    }
                    break;
                case (2):
                    // Selecting an item

                    break;
                case (3):
                    CheckInput<Attack>(currRects, 3, 3, emptyAttackList, false, 0, false, -Screen.width / 40);
                    if (!waitFrame)
                    {
                        waitFrame = true;
                    }
                    else
                    {
                        // Selecting a spell

                        if (aPressed)
                        {
                            List<Attack> heroAttacks = GetAttacker().usableAttacks;

                            Attack chosenSpell = null;
                            if (tempCursor < heroAttacks.Count)
                            {
                                chosenSpell = heroAttacks[tempCursor];
                            }
                            if (chosenSpell)
                            {
                                menuLayer = 1;
                                attackIndex = tempCursor + 1;
                                OpenMenu(1, 3);
                            }
                            else
                            {
                                // play "no" sound
                            }
                        }
                        else if (bPressed)
                        {
                            // close spell menu
                            CloseMenu(3, 0);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    int attackIndex = 0;

    public void OpenMenu(int newMenuLayer, int lastMenuLayer = 0)
    {
        this.lastMenuLayer = lastMenuLayer;
        switch (newMenuLayer)
        {
            case (0): // Open action select menu
                currRects = actionSelect;
                menuLayer = 0;
                cols = 3;
                rows = 2;
                visibleSize = 6;
                currCursor = cursor;
                isScrollable = false;
                currRects = actionSelect;
                UpdateCursor(actionSelect, 0);
                break;
            case (1): // open target menu
                menuLayer = 1;
                this.lastMenuLayer = lastMenuLayer;
                List<RectTransform> enemyRectList = new List<RectTransform>();
                RectTransform[] enemyRectsTemp = bm.enviroImg.transform
                    .GetComponentsInChildren<RectTransform>();

                targetNameWindow.gameObject.SetActive(true);

                for (int i = 1; i < enemyRectsTemp.Length; i++)
                {
                    enemyRectList.Add(enemyRectsTemp[i]);
                }
                bm.enemiesAlive = enemyRectList.Count;

                enemyRects = enemyRectList.ToArray();
                currRects = enemyRects;
                UpdateCursor(enemyRects, 0);
                break;
            case (2):
                // Open item menu
                menuLayer = 2;

                itemMenu.OpenMenu();/*
                itemSelectWindow.gameObject.SetActive(true);
                currRects = listTexts;
                //isScrollable = true;
                visibleSize = 9;
                rows = 3;
                cols = 3;
                InitializeListText(0, GetAttacker().usableAttacks);

                cursor2.SetActive(true);
                UpdateCursor(currRects, 0, 2, -Screen.width / 40);*/
                break;
            case (3): // open spell menu
                menuLayer = 3;

                listSelectWindow.gameObject.SetActive(true);
                currRects = listTexts;
                //isScrollable = true;
                visibleSize = 9;
                rows = 3;
                cols = 3;
                InitializeListText(0,  GetAttacker().usableAttacks);

                cursor2.SetActive(true);
                UpdateCursor(currRects, 0, 2, -Screen.width / 40);
                break;
            case (4):
                break;
        }
    }

    void CloseMenu(int currMenu, int nextMenu)
    {
        switch (currMenu)
        {
            case (0): // Close main menu
                break;
            case (1): // Close target menu
                targetNameWindow.gameObject.SetActive(false);
                break;
            case (2):
                break;
            case (3): // Close spell menu
                currRects = actionSelect;
                currCursor = cursor;
                UpdateCursor(currRects, 0);
                listSelectWindow.gameObject.SetActive(false);
                menuLayer = 0;
                cursor2.SetActive(false);
                waitFrame = false;
                break;
            case (4):
                break;
        }
        OpenMenu(nextMenu);
    }

    int lastMenuLayer = 0;
    bool waitFrame = false;

    // The list of spells should be able to be scrolled through even if the spell hasn't been learned yet
    List<Attack> emptyAttackList = new List<Attack>(new Attack[9]);

    int heroEnemyCursor = 0;
    bool heroCursorMoved = false;

    public void NextHeroSelectsAttack(bool calledFromPrevHero)
    {
        targetingEnemies = true;
        attackIndex = 0;
        listSelectWindow.gameObject.SetActive(false);
        cursor2.SetActive(false);
        if (calledFromPrevHero)
        {
            MovePanelVert(selectingHeroIndex, 1);
        }

        if (++selectingHeroIndex < hpm.activePartyMembers.Count && selectingHeroIndex > -1 && hpm.activePartyMembers[selectingHeroIndex])
        {
            // More heroes must input their moves
            if (hpm.activePartyMembers[selectingHeroIndex].isDead)
            {
                NextHeroSelectsAttack(false);
            }
            else
            {
                MovePanelVert(selectingHeroIndex, 2);

                menuLayer = 0;
                cols = 3;
                rows = 2;
                visibleSize = 6;
                currCursor = cursor;
                isScrollable = false;
                currRects = actionSelect;
                UpdateCursor(actionSelect, 0);
            }
        }
        else
        {
            MovePanelVert(selectingHeroIndex - 1, 1);
            selectingHeroIndex = 0;

            PickEnemyAttacks();
            canSelect = false;
            cursor.SetActive(false);
            defaultActionMenu.SetActive(false);
        }
    }

    public void PreviousHeroSelectsAttack(bool fromNextHero = false)
    {
        targetingEnemies = true;
        Debug.Log(string.Format("hero {0} selects attack", selectingHeroIndex - 1));
        if (selectingHeroIndex-- > 0)
        {
            if (hpm.activePartyMembers[selectingHeroIndex].isDead)
            {
                PreviousHeroSelectsAttack();
            } else
            {
                MovePanelVert(selectingHeroIndex, 2);
                menuLayer = 0;
                currRects = actionSelect;
                visibleSize = 6;

                if (fromNextHero)
                {
                    MovePanelVert(selectingHeroIndex + 1, 1);
                }

                UpdateCursor(actionSelect, 0);

                bm.turnList.RemoveAt(bm.turnList.Count - 1);
            }
        } else
        {
            selectingHeroIndex++;

            // Play "You can't do that" noise
        }
    }

    BaseCharacter GetTarget()
    {
        if (!targetingEnemies)
        {
            return BattleManager.hpm.activePartyMembers[heroEnemyCursor % BattleManager.hpm.activePartyMembers.Count];
        } else
        {
            return BattleManager.epm.activePartyMembers[heroEnemyCursor % BattleManager.epm.activePartyMembers.Count];
        }
    }

    BaseCharacter GetAttacker()
    {
        return BattleManager.hpm.activePartyMembers[selectingHeroIndex];
    }

    bool targetingEnemies = true;

    void CheckHeroesAndEnemies()
    {
        if (!heroCursorMoved)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                heroEnemyCursor++;
                heroEnemyCursor %= currRects.Length;

                UpdateCursor(currRects, heroEnemyCursor);

                heroCursorMoved = true;
            } else if (Input.GetAxis("Horizontal") < 0)
            {
                heroEnemyCursor--;
                heroEnemyCursor %= currRects.Length;
                heroEnemyCursor = Mathf.Abs(heroEnemyCursor);
                UpdateCursor(currRects, heroEnemyCursor);

                heroCursorMoved = true;
            } else if (Input.GetAxis("Vertical") > 0 && currRects == heroRects)
            {
                currRects = enemyRects;
                Debug.Log("Move cursor to enemies");
                targetingEnemies = true;
                // Move cursor from enemies to heroes
                UpdateCursor(currRects, heroEnemyCursor % bm.enemiesAlive);

                heroCursorMoved = true;
            }
            else if (Input.GetAxis("Vertical") < 0 && currRects == enemyRects)
            {
                currRects = heroRects;
                // Move cursor from heroes to enemies

                heroEnemyCursor = Mathf.Abs(heroEnemyCursor % bm.heroesAlive);
                if (heroEnemyCursor >= currRects.Length)
                {
                    heroEnemyCursor = 0;
                }
                UpdateCursor(currRects, heroEnemyCursor);

                targetingEnemies = false;
                Debug.Log("Move cursor to heroes");

                heroCursorMoved = true;
            }

            aPressed = false;
            bPressed = false;
            if (!heroCursorMoved)
            {
                if (Input.GetButtonDown("AButton"))
                {
                    aPressed = true;
                }
                else
                if (Input.GetButtonDown("BButton"))
                {
                    bPressed = true;
                }
            }
        } else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            heroCursorMoved = false;
        }

        targetNameWindow.GetComponentInChildren<Text>().text = GetTarget().characterName;
    }

    void PickEnemyAttacks()
    {
        bool usesAttack = true;
        selectingHeroIndex = -1;

        for (int i = 0; i < BattleManager.epm.activePartyMembers.Count; i++)
        {
            BaseCharacter enemy = BattleManager.epm.activePartyMembers[i];
            int occurrenceVal = UnityEngine.Random.Range(1, 6);

            Attack enemyAttack = null;
            int tries = 100;
            while (enemyAttack == null && --tries > 0)
            {
                if (!BattleManager.isFightingFinalBoss)
                {
                    int randMoveIndex = UnityEngine.Random.Range(0, enemy.usableAttacks.Count);

                    if (occurrenceVal <= enemy.usableAttacks[randMoveIndex].occurrenceChance)
                    {
                        enemyAttack = enemy.usableAttacks[randMoveIndex];
                    }
                } else
                {
                    // Lower chance of attacking since there are 5 body parts

                    int randMoveIndex = UnityEngine.Random.Range(0, enemy.usableAttacks.Count);

                    int usesMoveChance = UnityEngine.Random.Range(0, 5);

                    if (usesMoveChance > 2)
                    {
                        if (occurrenceVal <= enemy.usableAttacks[randMoveIndex].occurrenceChance)
                        {
                            enemyAttack = enemy.usableAttacks[randMoveIndex];
                        }
                    } else
                    {
                        usesAttack = false;
                    }
                }
            }
            BaseCharacter target = null;
            tries = 100;
            // Select a living target
            if (enemyAttack.attackType != AttackType.HEALING)
            {
                while (target == null && --tries > 0)
                {
                    int randHeroIndex = UnityEngine.Random.Range(0, BattleManager.hpm.activePartyMembers.Count);
                    if (!BattleManager.hpm.activePartyMembers[randHeroIndex].isDead)
                    {
                        target = BattleManager.hpm.activePartyMembers[randHeroIndex];
                    }
                }
            } else
            {
                while (target == null && --tries > 0)
                {
                    int randHeroIndex = UnityEngine.Random.Range(0, BattleManager.epm.activePartyMembers.Count);
                    if (!BattleManager.epm.activePartyMembers[randHeroIndex].isDead)
                    {
                        target = BattleManager.epm.activePartyMembers[randHeroIndex];
                    }
                }

            }
            if (usesAttack)
            {
                bm.turnList.Add(new Turn(enemy, target, enemyAttack));
            }
        }
        ExecuteAttacks();
        
    }

    void MovePanelVert(int heroIndex, int lowMidHigh)
    {
        if (lowMidHigh == 0)
        {
            // Move Panel to lowest position
            heroRects[heroIndex].position = new Vector3(heroRects[heroIndex].position.x, heroDisplayMidHeight - Screen.height / 16);
        } else if (lowMidHigh == 2)
        {
            // Move Panel to highest position
            heroRects[heroIndex].position = new Vector3(heroRects[heroIndex].position.x, heroDisplayMidHeight + Screen.height / 16);
        } else
        {
            // Move to mid positon
            heroRects[heroIndex].position = new Vector3(heroRects[heroIndex].position.x, heroDisplayMidHeight);
        }
    }

    void ExecuteAttacks()
    {
        List<Turn> turnList = bm.turnList;
        
        turnList.Sort((t1, t2) => ( t1.attacker.currentEVA - t2.attacker.currentEVA));

        for (int i = 0; i < turnList.Count; i++) 
        {
            Turn turn = turnList[i];
            if (turn.attack is Defend)
            {
                //Debug.Log(turn.attacker + " is defending");
                turn.attacker.isDefending = true;
            }
        }


        Turn currTurn = turnList[0];
            turnList.RemoveAt(0);

        bm.StartInactiveTurn(currTurn, turnList);
            //StartCoroutine(currTurn.attack.UseAttack(currTurn.attacker, currTurn.target, turnList));
        
    }

    public void ReturnToMenu()
    {
        if (!BattleManager.hasLost && !BattleManager.hasWon)
        {

            canSelect = true;
            cursor.SetActive(true);
            defaultActionMenu.SetActive(true);
            bm.messageBoxImg.gameObject.SetActive(false);
            menuLayer = 1;
            NextHeroSelectsAttack(false);
        }
    }
    

    void UpdatePanel(int heroIndex)
    {
        bm.allHeroStats.GetChild(heroIndex).gameObject.GetComponent<HeroDisplayPanel>()
            .UpdateDisplay(hpm.activePartyMembers[heroIndex]);
    }

    public void UpdatePanel(BaseCharacter hero)
    {
        int heroIndex = 0;
        bool heroFound = false;
        for (; heroIndex < hpm.activePartyMembers.Count && !heroFound; heroIndex++)
        {
            if (hpm.activePartyMembers[heroIndex] == hero)
            {
                heroFound = true;
            }
        }
        UpdatePanel(--heroIndex);
    }
}