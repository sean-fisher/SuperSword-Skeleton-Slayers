using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour {

    public bool menuActive = false;

    // TempCursor is the position of the cursor in the whole list, greater than or equal to the visibleSize
    protected int tempCursor = 0;
    protected bool cursorMoved = false;
     
    protected bool enableScrollingUp = false; // This is set to true if the start point is greater than 0
    protected bool enableScrollingDown = true; // Set to true if the start point is greater than 0
    protected bool isScrollable = false;
     
    protected int cursorMarker = 0;
     
    protected RectTransform[] listTexts;
     
    protected int visibleSize = 1;

    protected int rows;
    protected int cols;

    protected static GameObject cursor;
    protected static GameObject cursor2;
    protected static GameObject cursor3;

    protected GameObject currCursor;

    GameObject attackHolderObj;

    public void SwitchTo(Menu otherMenu, bool closeCurrentMenu = false)
    {
        if (closeCurrentMenu)
        {
            CloseMenu();
        }
        else
        {
            menuActive = false;
        }

        otherMenu.OpenMenu();
    }

    public virtual void OpenMenu()
    {
        gameObject.SetActive(true);
        menuActive = true;
        currCursor = cursor;
    }

    public virtual void CloseMenu()
    {
        gameObject.SetActive(false);
        menuActive = false;
    }

    public void InitializeListText<T>(int firstVisibleIndex, List<T> scrollableList)
    {
        //Debug.Log("Initialize at " + firstVisibleIndex + " with size of " + visibleSize + " and length " + listTexts.Length);
        if (scrollableList != null)
        {
            //scrollableListO = scrollableList as List<object>;
        }
        int i = 0;
        
        for (; i < visibleSize; i++)
        {
            listTexts[i].GetComponent<Text>().text = tempSpellnames[i + firstVisibleIndex];

            
			object currElement = null;
			if (scrollableList != null && firstVisibleIndex + i < scrollableList.Count) {
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
            else
            {
                listTexts[i].GetComponent<Text>().text = "";// tempSpellnames[i + firstVisibleIndex];
            
            }
        }
    }

    // Move the cursor to a RectTransform (i.e. an enemy's image)
    protected void UpdateCursor(RectTransform[] visibleRectArr, int newIndex, int cursorNum = 0, float offsetNum = 0, bool isSelectingEnemies = false)
    {
        Sounds.audioSource.clip = Sounds.cursorMove;
        Sounds.audioSource.Play();
        if (cursorNum == 2)
        {
            currCursor = cursor2;
        }
        else
        if (cursorNum == 3)
        {
            currCursor = cursor3;
        }
        Vector3 optionPosition = Vector3.zero;
        
        // Places the cursor at the position of the chosen option, but offset to the left
        if (newIndex - cursorMarker < visibleRectArr.Length && newIndex - cursorMarker > -1)
        {
            optionPosition = visibleRectArr[newIndex - cursorMarker].transform.position;
        } else
        {
            optionPosition = visibleRectArr[newIndex % visibleSize].transform.position;
        }
        
        currCursor.transform.position = new Vector3(optionPosition.x - (float) (Screen.width / 17) + offsetNum, optionPosition.y + 4);

        tempCursor = newIndex;
        cursorMoved = true;

        if (isScrollable && tempCursor == visibleSize - 1)
        {
            enableScrollingDown = true;
        }
        currCursor.SetActive(true);
        
        //Debug.Log(tempCursor);
        //cursorMarker = tempCursor % visibleSize - 1;
    }

    List<string> tempSpellnames = new List<string> { "0", "1", "2",
                                                     "3", "4", "5",
                                                     "6", "7", "8",
                                                     "9", "10", "11",
                                                     "12", "13", "14",
                                                     "15", "16", "17",};

    void ScrollTo<T>(List<T> scrollableList, int firstVisibleIndex, bool listMustBeFilled = true)
    {
        Debug.Log("Scroll To " + firstVisibleIndex + " " + scrollableList);
        int j = 0;

        /*
        for (; j < visibleSize && j + firstVisibleIndex < scrollableList.Count; j++)
        {
            T currElement = scrollableList[firstVisibleIndex + j];
            if (currElement != null)
            {
                Debug.Log("Has name");
                if (currElement is ItemData)
                {
                    // Displays the item name in the list
                    listTexts[j].GetComponent<Text>().text = ((ItemData)(object)currElement).itemName;
                }
                else if (currElement is Attack)
                {
                    // TODO: Maybe change Attack class name to spell??
                    // Displays the spell name in the list
                    listTexts[j].GetComponent<Text>().text = ((Attack)(object)currElement).attackName;
                }
                else
                {
                    throw new System.Exception("Error while comparing to ItemData or attack");
                }
            }
            else
            {
                listTexts[j].GetComponent<Text>().text = tempSpellnames[j];
                
                //listTexts[j].GetComponent<Text>().text = "--------";
                if (!listMustBeFilled)
                {
                    enableScrollingDown = true;
                }
            }

        }*/

        //tempCursor = firstVisibleIndex + cursorMarker;
        if (firstVisibleIndex + visibleSize >= scrollableList.Count)
        {
            enableScrollingDown = false;
        }
        for (; j < 4; j++)
        {
            listTexts[j].GetComponent<Text>().text = "";
            //visibleSize--;
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
        InitializeListText<Attack>(firstVisibleIndex, null);
        //cursorMarker = firstVisibleIndex;
    }

    // The <T> is only necessary when a scrollableList is passed. Otherwise, substitute T with an arbitrary class.

    protected void CheckInput<T>(RectTransform[] visibleTextArr, int width, int height = 0, 
        List<T> scrollableList = null, bool listMustBeFilled = true, int cursorInt = 0, bool cantScrollHoriz = false, float offsetVal = 0)
    {
        if (!cursorMoved)
        {
            rows = height;
            cols = width;
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
                                UpdateCursor(visibleTextArr, scrollableList.Count - (cols -(tempCursor)), 0, offsetVal);
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

                        if (enableScrollingDown && tempCursor % (visibleSize -cursorMarker) == 0)
                        {
                            //Debug.Log("Scroll to next row");
                            // Scroll down one row
                            cursorMarker += cols;
                            
                            ScrollTo(scrollableList, cursorMarker, listMustBeFilled); // Scroll down 1
                            UpdateCursor(visibleTextArr, tempCursor, 0, offsetVal);
                            //tempCursor -= cols - 1;
                        }
                        else if (!enableScrollingDown)
                        {
                            //Debug.Log("Scroll to beginning");
                            // scroll to beginning
                            ScrollTo(scrollableList, 0);
                            if (scrollableList.Count > 4)
                            {
                                enableScrollingDown = true;
                            }
                            cursorMarker = 0;
                            tempCursor = 0;
                            UpdateCursor(visibleTextArr, 0, 0, offsetVal);
                        } else
                        {
                            //Debug.Log("Move right one");
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
                } else
                if (Input.GetButtonDown("BButton"))
                {
                    bPressed = true;
                }
            }
        } else
        {
            aPressed = false;
            bPressed = false;
            cursorMoved = !(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0);
        }
    }

    protected bool aPressed = false;
    protected bool bPressed = false;
}
