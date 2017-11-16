using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Songs : MonoBehaviour {
    public static Songs songPlayer;
    public static AudioClip mazeMusic;
    public static AudioClip overworldMusic;
    public static AudioClip battleMusic;
    public static AudioClip bossMusic;
    public static AudioClip victoryMusic;
    public static AudioClip creditsMusic;
    public static AudioClip titleMusic;
    public static AudioClip oceanMusic;

    public AudioClip mazeMusicClip;
    public AudioClip overworldMusicClip;
    public AudioClip battleMusicClip;
    public AudioClip bossMusicClip;
    public AudioClip victoryMusicClip;
    public AudioClip creditsMusicClip;
    public AudioClip titleMusicClip;
    public AudioClip oceanMusicClip;

    public AudioSource battleaudioSource;
    public static AudioSource battlemusicPlayer;
    public AudioSource bgmaudioSource;
    public static AudioSource bgmmusicPlayer;

    private void Start()
    {
        mazeMusic = mazeMusicClip;
        overworldMusic = overworldMusicClip;
        battleMusic = battleMusicClip;
        bossMusic = bossMusicClip;
        victoryMusic = victoryMusicClip;
        creditsMusic = creditsMusicClip;
        titleMusic = titleMusicClip;
        oceanMusic = oceanMusicClip;

        if (battlemusicPlayer== null)
        {
            battlemusicPlayer = battleaudioSource;
        }
        if (bgmmusicPlayer== null)
        {
            bgmmusicPlayer = bgmaudioSource;
        }
        songPlayer = this;
    }

    public void FadeOut(AudioSource source, float timeToFade)
    {
        Debug.Log(source.name);
        StartCoroutine(FadingOut(source, timeToFade));

    }
    IEnumerator FadingOut(AudioSource source, float timeToFade)
    {
        float fadePerSec = 1 / timeToFade;
        while((timeToFade-=Time.deltaTime) > 0)
        {
            source.volume -= fadePerSec * Time.deltaTime;
            yield return null;
        }

        source.Stop();
        source.volume = 1;
    }
}
