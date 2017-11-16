using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static AudioSource audioSource;
    public static AudioClip cursorMove;
    public static AudioClip menuselect;
    public static AudioClip fire;
    public static AudioClip ice;
    public static AudioClip lightning;
    public static AudioClip back;
    public static AudioClip defeatEnemy;
    public static AudioClip gameover;
    public static AudioClip slash;
    public static AudioClip miss;
    public static AudioClip runaway;
    public static AudioClip step;
    public static AudioClip encounter;
    public static AudioClip heal;
    public static AudioClip bump;

    public AudioClip cursorMoveClip;
    public AudioClip menuselectClip;
    public AudioClip fireClip;
    public AudioClip iceClip;
    public AudioClip lightningClip;
    public AudioClip backClip;
    public AudioClip defeatEnemyClip;
    public AudioClip gameoverClip;
    public AudioClip slashClip;
    public AudioClip missClip;
    public AudioClip runawayClip;
    public AudioClip stepClip;
    public AudioClip encounterClip;
    public AudioClip healClip;
    public AudioClip bumpClip;
    private void Start()
    {
        cursorMove = cursorMoveClip;
        menuselect = menuselectClip;
        fire       = fireClip;
        ice        = iceClip;
        lightning  = lightningClip;
        back       = backClip;
        defeatEnemy = defeatEnemyClip;
        gameover   = gameoverClip;
        miss = missClip;
        slash = slashClip;
        runaway = runawayClip;
        encounter = encounterClip;

        audioSource = this.GetComponent<AudioSource>();
    }
}