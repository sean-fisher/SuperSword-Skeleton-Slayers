using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScreen : MonoBehaviour {

    public float secondsToScroll = 78;
    
    public float speed;

	void Update () {
        if ((secondsToScroll -= Time.deltaTime) > 0)
            transform.position += new Vector3(0, speed * Time.deltaTime);
        else
            StartCoroutine(WaitThenReturnToTitle());
    }


    IEnumerator WaitThenReturnToTitle()
    {
        yield return new WaitForSeconds(0);
        SceneSwitcher.ss.SwitchToOtherScene("Title");
    }

}
