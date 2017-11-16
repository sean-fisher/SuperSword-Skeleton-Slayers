using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GridOptions : MonoBehaviour {

    public int rows;
    public int cols;
    public bool isScrollable;

    public bool canControl = true;

    public string emptyText = "";

    public GameObject cursor;

    protected int visibleSize = 0;

    protected int cursorMarker = 0;
    protected int tempCursor = 0;

    protected bool cursorMoved = false;

    protected bool enableScrollingDown = false;
    protected bool enableScrollingUp = false;

    protected RectTransform[] listTexts;

    protected static GameObject cursor2;
    protected static GameObject cursor3;

    protected GameObject currCursor;

    public List<GridOptions> menuOptions;

    public int displaceCursorByOneOverThisTimesScreenWidth = 17;
    public float displaceCursorByOneOverThisTimesScreenWidthY = 0;

    // Open a submenu
    protected virtual void MakeMenuSelection(int menuIndex)
    {
        menuOptions[menuIndex].OpenMenu();
        DisableMenuControl();

        Sounds.audioSource.clip = Sounds.menuselect;
        Sounds.audioSource.Play();
    }

    // What is called to open and set up this menu.
    public abstract void OpenMenu();

    // Disables control of the current menu; used when a sub-menu is opened
    public virtual void DisableMenuControl()
    {
        canControl = false;
    }

    public void InitializeListText<T>(int firstVisibleIndex, List<T> scrollableList)
    {
        int i = 0;

        for (; i < visibleSize; i++)
        {
            object currElement = null;
            if (scrollableList != null && firstVisibleIndex + i < scrollableList.Count)
            {
                currElement = scrollableList[firstVisibleIndex + i];
            }
            if (currElement != null)
            {
                if (currElement is ItemData)
                {
                    // Displays the item name in the list
                    listTexts[i].GetComponent<Text>().text = ((ItemData)currElement).itemName;
                }
                else if (currElement is Attack)
                {
                    // TODO: Maybe change Attack class name to spell??
                    // Displays the spell name in the list
                    listTexts[i].GetComponent<Text>().text = ((Attack)currElement).attackName;
                }
                else
                {
                    throw new System.Exception("Error while comparing to ItemData or attack");
                }
            }
        }
    }


    public void InitializeListText<T>(int firstVisibleIndex, T[] scrollableList)
    {
        int i = 0;

        for (; i < visibleSize; i++)
        {
            object currElement = null;
            if (scrollableList != null && firstVisibleIndex + i < scrollableList.Length)
            {
                currElement = scrollableList[firstVisibleIndex + i];
            }
            if (currElement != null)
            {
                if (currElement is ItemData)
                {
                    // Displays the item name in the list
                    listTexts[i].GetComponent<InventoryEntry>().itemName.text = ((ItemData)currElement).itemName;
                }
                else if (currElement is Attack)
                {
                    // TODO: Maybe change Attack class name to spell??
                    // Displays the spell name in the list
                    listTexts[i].GetComponent<Text>().text = ((Attack)currElement).attackName;
                }
                else
                {
                    throw new System.Exception("Error while comparing to ItemData or attack");
                }
            }
            else if (i < listTexts.Length)
            {
                listTexts[i].GetComponentInChildren<Text>(true).text = "";

            }
        }
    }

    // Use this for initialization
    void Start () {
        visibleSize = rows * cols;
	}

    public abstract void CloseMenu();

    // Move the cursor to a RectTransform (i.e. an enemy's image)
    protected void UpdateCursor(RectTransform[] visibleRectArr, 
        int newIndex, int cursorNum = 0, float offsetNum = 0)
    {
        Vector3 optionPosition = Vector3.zero;

        // Places the cursor at the position of the chosen option, but offset to the left
        if (newIndex - cursorMarker < visibleRectArr.Length 
            && newIndex - cursorMarker > -1)
        {
            optionPosition = visibleRectArr[
                newIndex - cursorMarker].transform.position;
        }
        else
        {
            newIndex = newIndex < 0 ? 0 : newIndex;
            tempCursor = newIndex;
            optionPosition = visibleRectArr[newIndex % visibleSize].transform.position;
        }

        cursor.transform.position = new Vector3(optionPosition.x - (float)
            (Screen.width / displaceCursorByOneOverThisTimesScreenWidth) + offsetNum, 
            optionPosition.y + 4 + (Screen.height * displaceCursorByOneOverThisTimesScreenWidthY / 17));

        tempCursor = newIndex;
        cursorMoved = true;

        if (isScrollable && tempCursor == visibleSize - 1)
        {
            enableScrollingDown = true;
        }
        cursor.SetActive(true);

        OnMoveCursor();
    }


    void ScrollTo<T>(List<T> scrollableList, int firstVisibleIndex, bool listMustBeFilled = true)
    {
        Debug.Log("Scroll To " + firstVisibleIndex + " " + scrollableList);
        int j = 0;
        

        //tempCursor = firstVisibleIndex + cursorMarker;
        if (firstVisibleIndex + visibleSize >= scrollableList.Count)
        {
            enableScrollingDown = false;
        }
        for (; j < 4; j++)
        {
            listTexts[j].GetComponent<Text>().text = "";
        }
        if (firstVisibleIndex > 0)
        {
            enableScrollingUp = true;
        }
        else
        {
            enableScrollingUp = false;
        }
        cursorMoved = true;
    }


    // Update is called once per frame
    void Update () {
		if (canControl)
        {
            CheckInput<RectTransform>(listTexts, cols, rows);
        }
	}

    private void UpdateCursor(Text[] array, int current, int newIndex)
    {
        array[current].transform.GetChild(0).gameObject.SetActive(false);
        array[newIndex].transform.GetChild(0).gameObject.SetActive(true);
        tempCursor = newIndex;
        OnMoveCursor();
    }

    protected virtual void OnMoveCursor() {

        Sounds.audioSource.clip = Sounds.cursorMove;
        Sounds.audioSource.Play();
    }

    void CheckInput(Text[] array, int width, int height = 0)
    {
        // If only part of the array should be able to be scrolled through, pass a height smaller than the array's length
        int length; // length is the number of slots to scroll through
        if (height > 0)
        {
            length = width * height;
        }
        else
        {
            length = array.Length;
        }

        if (Input.GetAxis("Vertical") < -.5f)
        {
            tempCursor += width;
            if (tempCursor > length - 1)
            {
                UpdateCursor(array, tempCursor - width, tempCursor % width);
            }
            else
            {
                UpdateCursor(array, tempCursor - width, tempCursor);
            }
        }
        else if (Input.GetAxis("Vertical") > .5f)
        {
            tempCursor -= width;
            if (tempCursor < 0)
            {
                UpdateCursor(array, width + tempCursor, length - 1);

                // TODO: Scroll items
            }
            else
            {
                UpdateCursor(array, tempCursor + width, tempCursor);
            }
        }
        else if (Input.GetAxis("Horizontal") > .5f)
        {
            tempCursor++;
            if (tempCursor > length - 1)
            {
                UpdateCursor(array, --tempCursor, 0);

            }
            else
            {
                UpdateCursor(array, tempCursor - 1, tempCursor);
            }
        }
        else if (Input.GetAxis("Horizontal") < -.5f)
        {
            if (--tempCursor < 0)
            {
                UpdateCursor(array, ++tempCursor, length - 1);

            }
            else
            {
                UpdateCursor(array, tempCursor + 1, tempCursor);
            }
        }
    }


    protected void CheckInput<T>(RectTransform[] visibleTextArr, int width, int height = 0,
        List<T> scrollableList = null, bool listMustBeFilled = true, int cursorInt = 0, bool cantScrollHoriz = false, float offsetVal = 0)
    {
        if (!cursorMoved)
        {
            // If only part of the visibleTextArr should be able to be scrolled through, pass a height smaller than the visibleTextArr's length
            int length; // length is the number of slots to scroll through
            if (height > 0)
            {
                length = width * height;
            }
            else
            {
                length = visibleTextArr.Length;
            }


            if (Input.GetAxis("Vertical") < 0)
            {
                tempCursor += width;
                if (tempCursor > length - 1)
                {
                    if (isScrollable)
                    {
                        if (enableScrollingDown)
                        {
                            cursorMarker += cols;
                            ScrollTo(scrollableList, cursorMarker, listMustBeFilled); // Scroll down 1
                            UpdateCursor(visibleTextArr, tempCursor, 0, offsetVal);
                        }
                        else
                        {
                            // scroll to beginning
                            ScrollTo(scrollableList, 0, listMustBeFilled);
                            UpdateCursor(visibleTextArr, 0, cursorInt, offsetVal);
                            if (scrollableList.Count > visibleSize)
                            {
                                enableScrollingDown = true;
                            }
                            cursorMarker = 0;
                            tempCursor = 0;
                        }
                    }
                    else
                    {
                        UpdateCursor(visibleTextArr, tempCursor % width, cursorInt, offsetVal);
                    }


                }
                else
                {
                    UpdateCursor(visibleTextArr, tempCursor, cursorInt, offsetVal);
                }
            }
            else
            if (Input.GetAxis("Vertical") > 0)
            {
                tempCursor -= cols;

                //Debug.Log(string.Format("fvi = {0},  tempCursor = {1}", cursorMarker, tempCursor));
                if (tempCursor < cursorMarker)
                {
                    if (isScrollable)
                    {
                        if (enableScrollingUp)
                        {
                            // Scroll up one row
                            cursorMarker -= cols;
                            ScrollTo(scrollableList, cursorMarker, listMustBeFilled); // Scroll up 1
                        }
                        else
                        {
                            // scroll to end
                            int scrollVal = scrollableList.Count - visibleSize;
                            if (scrollVal > 0)
                            {
                                ScrollTo(scrollableList, scrollVal, listMustBeFilled);
                                tempCursor += cols;
                                Debug.Log(scrollableList.Count - (cols - (tempCursor)));
                                UpdateCursor(visibleTextArr, scrollableList.Count - (cols - (tempCursor)), 0, offsetVal);
                            }
                            else
                            {
                                UpdateCursor(visibleTextArr, visibleSize - 1, 0, offsetVal);
                            }
                            enableScrollingDown = false;

                            cursorMarker = scrollVal;
                        }
                    }
                    else
                    {
                        UpdateCursor(visibleTextArr, length - 1, 0, offsetVal);
                    }
                    // TODO: Scroll items
                }
                else
                {
                    // Moce cursor up one row
                    UpdateCursor(visibleTextArr, tempCursor, 0, offsetVal);
                }
            }
            else
            if (Input.GetAxis("Horizontal") > 0)
            {

                tempCursor++;
                //Debug.Log("TempCursor is " + tempCursor);
                /*if (tempCursor % visibleSize == 0)*/
                if (tempCursor - cursorMarker > visibleSize - 1)
                {
                    if (isScrollable)
                    {
                        //Debug.Log(string.Format("fvi = {0},  tempCursor = {1}", cursorMarker, tempCursor));

                        if (enableScrollingDown && tempCursor % (visibleSize - cursorMarker) == 0)
                        {
                            // Scroll down one row
                            cursorMarker += cols;

                            ScrollTo(scrollableList, cursorMarker, listMustBeFilled); // Scroll down 1
                            UpdateCursor(visibleTextArr, tempCursor, 0, offsetVal);
                        }
                        else if (!enableScrollingDown)
                        {
                            // scroll to beginning
                            ScrollTo(scrollableList, 0);
                            if (scrollableList.Count > 4)
                            {
                                enableScrollingDown = true;
                            }
                            cursorMarker = 0;
                            tempCursor = 0;
                            UpdateCursor(visibleTextArr, 0, 0, offsetVal);
                        }
                        else
                        {
                            // EnsableScrollingDown
                            UpdateCursor(visibleTextArr, tempCursor, 0, offsetVal);
                        }
                    }
                    else
                    {
                        //Debug.Log("Move to 0");
                        UpdateCursor(visibleTextArr, 0, 0, offsetVal);
                    }
                }
                else
                {
                    UpdateCursor(visibleTextArr, tempCursor, 0, offsetVal);
                }
            }
            else
            if (Input.GetAxis("Horizontal") < 0)
            {
                if (--tempCursor < 0)
                {
                    if (isScrollable)
                    {
                        if (enableScrollingUp)
                        {
                            ScrollTo(scrollableList, --cursorMarker, listMustBeFilled); // Scroll up 1
                        }
                        else
                        {
                            // scroll to end
                            int scrollVal = 0;
                            if (scrollableList != null)
                            {
                                scrollVal = scrollableList.Count - visibleSize;
                            }

                            if (scrollVal > 0)
                            {
                                ScrollTo(scrollableList, scrollVal, listMustBeFilled);
                                UpdateCursor(visibleTextArr, 3, 0, offsetVal);
                            }
                            else
                            {
                                UpdateCursor(visibleTextArr, visibleSize - 1, 0, offsetVal);
                            }
                            enableScrollingDown = false;

                            cursorMarker = scrollVal;
                        }
                    }
                    else
                    {
                        UpdateCursor(visibleTextArr, length - 1, 0, offsetVal);
                    }
                }
                else
                {
                    UpdateCursor(visibleTextArr, tempCursor, 0, offsetVal);
                }
            }

            aPressed = false;
            bPressed = false;

            if (!cursorMoved)
            {
                if (Input.GetButtonDown("AButton"))
                {
                    aPressed = true;
                    MakeMenuSelection(tempCursor);
                }
                else
                if (Input.GetButtonDown("BButton"))
                {
                    bPressed = true;
                    Sounds.audioSource.clip = Sounds.back;
                    Sounds.audioSource.Play();
                }
            } else
            {
            }
        }
        else
        {
            aPressed = false;
            bPressed = false;
            cursorMoved = !(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0);
        }
    }

    protected bool aPressed = false;
    protected bool bPressed = false;

}
