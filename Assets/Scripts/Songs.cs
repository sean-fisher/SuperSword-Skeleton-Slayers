using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Songs : MonoBehaviour {

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
    }
}
