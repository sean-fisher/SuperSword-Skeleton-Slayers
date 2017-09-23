using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOptions : MonoBehaviour {

    public int rows;
    public int cols;
    public bool isScrollable;

    public string emptyText = "";

    public GameObject cursor;

    int visibleSize = 0;

    int cursorMarker = 0;
    int tempCursor = 0;

    bool cursorMoved = false;

    bool enableScrollingDown = false;

	// Use this for initialization
	void Start () {
        visibleSize = rows * cols;
	}

    private void OnEnable()
    {
        
    }

    // Move the cursor to a RectTransform (i.e. an enemy's image)
    protected void UpdateCursor(RectTransform[] visibleRectArr, int newIndex, int cursorNum = 0, float offsetNum = 0)
    {
        Vector3 optionPosition = Vector3.zero;

        // Places the cursor at the position of the chosen option, but offset to the left
        if (newIndex - cursorMarker < visibleRectArr.Length && newIndex - cursorMarker > -1)
        {
            optionPosition = visibleRectArr[newIndex - cursorMarker].transform.position;
        }
        else
        {
            optionPosition = visibleRectArr[newIndex % visibleSize].transform.position;
        }

        cursor.transform.position = new Vector3(optionPosition.x - (float)(Screen.width / 17) + offsetNum, optionPosition.y + 4);

        tempCursor = newIndex;
        cursorMoved = true;

        if (isScrollable && tempCursor == visibleSize - 1)
        {
            enableScrollingDown = true;
        }
        cursor.SetActive(true);
        //Debug.Log(tempCursor);
        //cursorMarker = tempCursor % visibleSize - 1;
    }


    // Update is called once per frame
    void Update () {
		
	}
}
