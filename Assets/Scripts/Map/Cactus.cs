using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cactus : InteractableTile {
    

    public AudioClip hurtSound;
    public static Image redScreen;
    

    public override void ActivateInteraction()
    {
        List<BaseCharacter> heroes = BattleManager.hpm.activePartyMembers;

        for (int i = 0; i < heroes.Count; i++)
        {
            heroes[i].currentHP -= 20;
            if (heroes[i].currentHP < 0)
            {
                heroes[i].currentHP = 1;
            }
        }

        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        redScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(.08f);
        redScreen.gameObject.SetActive(false);
        yield return new WaitForSeconds(.08f);
        redScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(.08f);
        redScreen.gameObject.SetActive(false);
    }
}
