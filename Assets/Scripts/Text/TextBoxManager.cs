using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;

/**
 * TODO: Credit due
 * */

public class TextBoxManager : MonoBehaviour {

    public static TextBoxManager tbm;

    public GameObject textBox;

    public Text activeText;
    public Text fadeText;

    public TextAsset textFile;
    public string[] textLines;

    public int currentLine;
    public int endLine;

    public static GridController player;

    public bool isActive;

    public bool stopPlayerMovement;
    private bool alreadyPressed;

    public GameObject yesNoOptions;
    public GameObject cursor;
    bool yesNoPicked = false;
    Text[] yesNoTexts;

    public bool isTyping = false;
    private bool cancelTyping = false;

    public float typingSpeed;

    public bool canAdvance = true;

    public static GameObject defaultMessageWindow;

    // Use this for initialization
    void Start()
    {

        if (tbm == null)
        {
            tbm = this;
        }

        if (yesNoOptions)
        {
            yesNoTexts = yesNoOptions.transform.GetComponentsInChildren<Text>();
        }

        if (textFile != null)
        {
            textLines = textFile.text.Split('\n');
        }

        if (endLine == 0)
        {
            endLine = textLines.Length - 1;
            if (endLine < 0)
            {
                endLine = 0;
            }
        }

        if (isActive)
        {

            //Debug.Log("Active enable");
            //EnableTextBox();
        }
        else if (textBox)
        {
            DisableTextBox();
        }
    }

    // Update is called once per frame
    void Update () {
        if (!isActive)
        {
            return;
        }
        alreadyPressed = true;

        //activeText.text = textLines[currentLine]; 

        if (Input.GetButtonDown("AButton") && canAdvance)
        {
            if(!isTyping)
            {
                currentLine++;

                if (currentLine > endLine)
                {
                    DisableTextBox();

                    alreadyPressed = false;
                    if (yesNoOptions && yesNoOptions.activeSelf)
                    {
                        yesNoPicked = true;
                    }

                    // If the battle was just won, 
                    // enable the player movement and disable the UI
                    if (BattleManager.hasWon)
                    {
                        BattleManager.hasWon = false;
                        BattleManager.bManager.CheckDisableMenu();
                    }
                }
                else
                {
                    // Debug.Log("Advance line " + currentLine + " " + textLines.Length);
                    if (currentLine < textLines.Length)
                    {
                        // Debug.Log("Advance array");
                        StartCoroutine(TextScroll(textLines[currentLine]));
                    } else
                    {
                        DisableTextBox();

                        alreadyPressed = false;
                        if (yesNoOptions && yesNoOptions.activeSelf)
                        {
                            yesNoPicked = true;
                        }

                        // If the battle was just won, 
                        // enable the player movement and disable the UI
                        if (BattleManager.hasWon)
                        {
                            BattleManager.hasWon = false;
                            BattleManager.bManager.CheckDisableMenu();
                        }
                    }
                }
            }
            else if (isTyping && !cancelTyping && !typingStartedThisFrame)
            {
                cancelTyping = true;
            }
        }
        
        if (yesNoOptions.activeSelf)
        {
            CheckInputSimple();
        }
	}
    bool typingStartedThisFrame = false;
    
    private IEnumerator TextScroll (string lineOfText, bool yesNo = false)
    {
        isTyping = true;
        //yield return new WaitForEndOfFrame();
        this.enabled = true;
        int letterIndex = 0;
        activeText.text = "";
        cancelTyping = false;

        while (isTyping && !cancelTyping && (letterIndex < lineOfText.Length - 1))
        {
            activeText.text += lineOfText[letterIndex];
            letterIndex++;
            yield return new WaitForSeconds(.01f / (typingSpeed + .1f));
            typingStartedThisFrame = false;
        }
        activeText.text = lineOfText;
        yield return new WaitForSeconds(.1f);
        isTyping = false;
        cancelTyping = false;

        if (yesNo)
        {
            yesNoOptions.SetActive(true);
            cursor.SetActive(true);
            UpdateCursor(0);
        }
    }

    public bool GetAlreadyPressed()
    {
        return alreadyPressed;
    }

    public void EnableTextBox(GameObject textBox = null, string text = null, bool canAdvance = true, bool yesNo = false, bool enableAfter = false)
    {
        ynCursor = 0;
        this.canAdvance = canAdvance;
        enableOnDisable = enableAfter;
        typingStartedThisFrame = true;

        if (textBox)
        {
            this.textBox = textBox;
            this.activeText = textBox.GetComponentInChildren<Text>();
        } else
        {
            this.textBox = defaultMessageWindow;
            textBox = defaultMessageWindow;
            this.activeText = textBox.GetComponentInChildren<Text>();
        }
        this.textBox.SetActive(true);
        isActive = true;
        if (stopPlayerMovement)
        {
            player.DisableMovement();
        }
        if (text == null)
        {
            StartCoroutine(TextScroll(textLines[currentLine], yesNo));
        } else
        {
            StartCoroutine(TextScroll(text, yesNo));
        }
    }

    public void EnableTextBox(GameObject textBox = null, string[] texts = null, bool canAdvance = true, bool yesNo = false)
    {
        ynCursor = 0;
        this.canAdvance = canAdvance;
        textLines = texts;
        currentLine = 0;
        endLine = texts.Length;
        StartCoroutine(TextScroll(textLines[currentLine], yesNo));
    }

    public void EnableTextBox(string text, bool enableOnDisable = true, 
        bool canAdvance = true)
    {
        ynCursor = 0;
        textBox.SetActive(true);
        isActive = true;
        if (stopPlayerMovement)
        { 
            player.DisableMovement();
        }
        this.enableOnDisable = enableOnDisable;
        StartCoroutine(TextScroll(text));
    }

    // Enable player movement afterwards
    bool enableOnDisable = true;

    public void PlayConversation(XElement convo, bool yesNoChoice = false)
    {
        StartCoroutine(PlayingConvo(convo, yesNoChoice));
    }

    public bool convoRunning = false;

    IEnumerator PlayingConvo(XElement convo, bool yesNoChoice = false)
    {
        yesNoPicked = false;
        convoRunning = true;
        endLine = 0;
        var lines = XMLLoader.GetLines(convo);
        foreach (XElement line in lines)
        {
            EnableTextBox(XMLLoader.GetSpeakerDialog(line), false);
            yield return null;
            while (isTyping)
            {
                yield return null;
            }
            if (yesNoChoice)
            {
                yesNoOptions.SetActive(true);
                ynCursor = 0;
                yesNoPicked = false;
                cursor.SetActive(true);
                UpdateCursor(0);
                //After the text is displayed, give the player two options: yes and no. Each triggers a different cutscene.
                while (!yesNoPicked)
                {
                    CheckInputSimple();
                    yield return null;
                }
            }
            else
            {
                while (!Input.GetButtonDown("AButton"))
                {
                    yield return null;
                }
            }
        }
        convoRunning = false;
    }

    public int ynCursor = 0;
    bool directionPressed = false;
    
    void CheckInputSimple()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && !directionPressed)
        {
            directionPressed = true;
            if (++ynCursor >= yesNoTexts.Length)
            {
                ynCursor = 0;
            }
            UpdateCursor(ynCursor);
        } else if (Input.GetAxisRaw("Horizontal") < 0 && !directionPressed)
        {
            directionPressed = true;
            if (--ynCursor < 0)
            {
                ynCursor = yesNoTexts.Length - 1;
            }
            UpdateCursor(ynCursor);
        }
        else if (Input.GetAxisRaw("Horizontal") == 0)
        {
            directionPressed = false;
        }
    }

    void UpdateCursor(int newPosition)
    {
        Vector3 optionPosition = yesNoTexts[newPosition].transform.position;
        cursor.transform.position = new Vector3(optionPosition.x - 24, optionPosition.y, optionPosition.z);
    }

    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;
        activeText.text = "";
        if (yesNoOptions.activeSelf)
        {
            yesNoOptions.SetActive(false);
            cursor.SetActive(false);

            yesNoPicked = true;
        }

        if (enableOnDisable)
        {
            player = GameManager.gm.leader;
            if (player)
            {
                player.EnableMovement();
            } else
            {
                // GameOver
            }
        }
        enabled = false;
        yesNoPicked = false;
        
    }

    public void ReloadScript(TextAsset textToLoad)
    {
        if (textToLoad != null)
        {
            textLines = new string[1];
            textLines = textToLoad.text.Split('\n');
        }
    }

    public void AppearThenFadeText(XElement convo)
    {
        StartCoroutine(FadingText(convo));
    }

    IEnumerator FadingText(XElement convo)
    {
        TextBoxManager.player.DisablePartyMovement();
        fadeText.gameObject.SetActive(true);
        convoRunning = true;
        yield return new WaitForSeconds(1);
        var lines = XMLLoader.GetLines(convo);
        foreach (XElement line in lines)
        {
            yield return new WaitForSeconds(1);
            fadeText.text = XMLLoader.GetSpeakerDialog(line);
            Color textColor = fadeText.color;
            for (int i = 0; i < 20; i++)
            {
                textColor.a += .05f;
                fadeText.color = textColor;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            for (int i = 0; i < 20; i++)
            {
                textColor.a -= .05f;
                fadeText.color = textColor;
                yield return null;
            }
        }
        yield return new WaitForSeconds(1);
        convoRunning = false;
        fadeText.enabled = false;
    }

    public int GetPlayerChoice()
    {
        return ynCursor;
    }
}
