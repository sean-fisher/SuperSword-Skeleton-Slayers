using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScreen : MonoBehaviour {

    public float secondsToScroll = 78;
    
    public float speed;

    public AudioClip song;
    public AudioSource source;

    private void Start()
    {
        source.clip = song;
        source.Play();
    }

    void Update () {
        if ((secondsToScroll -= Time.deltaTime) > 0)
        {
            if (Input.GetButtonDown("StartButton"))
            {
                WaitThenReturnToTitle();
            }
            else
            {
                transform.position += new Vector3(0, speed * Time.deltaTime);
            }
        }
        else
            StartCoroutine(WaitThenReturnToTitle());
    }


    IEnumerator WaitThenReturnToTitle()
    {
        yield return new WaitForSeconds(0);
        SceneSwitcher.ss.SwitchToOtherScene("Title");
    }

}
