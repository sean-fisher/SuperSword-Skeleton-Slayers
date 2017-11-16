using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject cursor;
    

    // Specifications for the current options window
    int tempCursor;
    GameObject currOptions;
    int horizontalSize;
    public int vertSize;

    private enum MenuType
    {
        MAIN,
        ITEM,
        EQUIP,
        SKILLS,
        STATUS,
        FORMATION,
        BESTIARY,
        CONFIG,
        SAVE,
        EXIT,
        CLOSED
    }

    private void UpdateCursor(GameObject options, int current, int newIndex)
    {
        Sounds.audioSource.clip = Sounds.cursorMove;
        Sounds.audioSource.Play();
        int index = 1;
        if (options.name == "BarracksWindow")
        {
            index = 5;
        } else if (options.name == "HeroDisplay")
        {
            index = 4;
        }
        options.transform.GetChild(current).GetChild(index).gameObject.SetActive(false);
        options.transform.GetChild(newIndex).GetChild(index).gameObject.SetActive(true);
        tempCursor = newIndex;
    }

    private void UpdateCursor(Text[] array, int current, int newIndex)
    {
        Sounds.audioSource.clip = Sounds.cursorMove;
        Sounds.audioSource.Play();
        array[current].transform.GetChild(0).gameObject.SetActive(false);
        array[newIndex].transform.GetChild(0).gameObject.SetActive(true);
        tempCursor = newIndex;

        
    }
    
    
    void CheckInput()
    {

        if (Input.GetAxis("Vertical") < -.5f)
        {
            int origTemp = tempCursor;
            tempCursor += vertSize % horizontalSize + 1;
            if (vertSize == 1)
            {
                tempCursor = origTemp + 1;
            }
            if (tempCursor > vertSize * horizontalSize - 1) // tempCursor exceeds end of options
            {
                UpdateCursor(currOptions, origTemp, 0);
            }
            else
            {
                UpdateCursor(currOptions, origTemp, tempCursor);
            }
        }
        else if (Input.GetAxis("Vertical") < -.5f)
        {
            if (--tempCursor < 0)
            {
                UpdateCursor(currOptions, ++tempCursor, vertSize * horizontalSize - 1);
            }
            else
            {
                UpdateCursor(currOptions, tempCursor + 1, tempCursor);
            }
        }
        else if (Input.GetAxis("Horizontal") > .5f)
        {
            tempCursor++;
            if (tempCursor > vertSize * horizontalSize - 1)
            {
                UpdateCursor(currOptions, --tempCursor, 0);
            }
            else
            {
                UpdateCursor(currOptions, tempCursor - 1, tempCursor);
            }
        }
        else if (Input.GetAxis("Horizontal") < -.5f)
        {
            if (--tempCursor < 0)
            {
                UpdateCursor(currOptions, ++tempCursor, vertSize * horizontalSize - 1);
            }
            else
            {
                UpdateCursor(currOptions, tempCursor + 1, tempCursor);
            }
        }
    }
    
    void CheckInput(Text[] array, int width, int height = 0)
    {
        // If only part of the array should be able to be scrolled through, pass a height smaller than the array's length
        int length; // length is the number of slots to scroll through
        if (height > 0)
        {
            length = width * height;
        } else
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
}