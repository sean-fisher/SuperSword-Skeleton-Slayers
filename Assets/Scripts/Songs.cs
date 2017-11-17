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
    public static AudioClip battleIntro;
    public static AudioClip overworldIntro;

    public AudioClip mazeMusicClip;
    public AudioClip overworldMusicClip;
    public AudioClip battleIntroClip;
    public AudioClip battleMusicClip;
    public AudioClip bossMusicClip;
    public AudioClip victoryMusicClip;
    public AudioClip creditsMusicClip;
    public AudioClip titleMusicClip;
    public AudioClip oceanMusicClip;
    public AudioClip overworldIntroClip;

    public AudioSource battleaudioSource;
    public static AudioSource battlemusicPlayer;
    public AudioSource bgmaudioSource;
    public static AudioSource bgmmusicPlayer;

    public bool disableOverworldLoop;
    public bool disableBattleLoop;

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
        battleIntro = battleIntroClip;
        overworldIntro = overworldIntroClip;
        disableOverworldLoop = false;

        if (battlemusicPlayer == null)
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

    public void PlayIntroThenLoop(AudioSource source, AudioClip intro, AudioClip loop)
    {
        Debug.Log("play intro: " + intro.name);
        source.clip = intro;
        source.Play();
        source.loop = false;

        if (source == battlemusicPlayer)
        {
            disableBattleLoop = false;
        } else
        {
            disableOverworldLoop = false;
        }


        StartCoroutine(WaitForTrackEndThenPlay(source, loop));
    }

    IEnumerator WaitForTrackEndThenPlay(AudioSource source, AudioClip loop)
    {
        yield return new WaitForSeconds(1);

        bool loopCondition = disableOverworldLoop;
        if (source == battlemusicPlayer)
        {
            loopCondition = disableBattleLoop;
        }

        while (source.isPlaying && !loopCondition)
        {
            yield return null;
        }
        if (source == battlemusicPlayer)
        {
            loopCondition = disableBattleLoop;
        }
        if (!loopCondition)
        {
            if (!battlemusicPlayer.isPlaying && !bgmmusicPlayer.isPlaying)
            {
                Debug.Log("play loop: " + loop.name);
                source.clip = loop;
                source.Play();
                source.loop = true;
            }
        }
    }

    public void PlayOverride(AudioSource source, AudioClip clip)
    {
        battlemusicPlayer.Stop();
        bgmmusicPlayer.Stop();
        source.clip = clip;
        source.Play();
    }
}
