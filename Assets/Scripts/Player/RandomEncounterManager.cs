using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounterManager : MonoBehaviour {

    bool encountersEnabled;
    int stepsSinceBattle = 0;

    public static RandomEncounterManager rem;

	// Use this for initialization
	void Start () {
        if (rem == null)
        {
            rem = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
