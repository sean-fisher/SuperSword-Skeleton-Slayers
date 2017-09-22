using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Since some sprites are larger than 16*16 and overlap with others, this makes sure sprites lower on the screen are in the foreground
public class SetSortLayerByY : MonoBehaviour {

    public bool updateEveryFrame;

    public int spriteOffsetX = 0;
    public int spriteOffsetY = 0;

    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = -(int)transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (updateEveryFrame)
        {
            sr.sortingOrder = -(int)transform.position.y;
        }
	}
}
