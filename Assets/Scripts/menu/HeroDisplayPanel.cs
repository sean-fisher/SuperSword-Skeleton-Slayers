using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDisplayPanel : MonoBehaviour {

    // If this is false, it is a display used in the pause menu. Else it is used in battle.
    public bool isBattleDisplay = true;
    public Text nameText;
    public Text hpText;
    public Text mpText;
    // Only needed if isBattleDisplay is false
    public Text classNameText;

    public void UpdateDisplay(BaseCharacter hero)
    {
        hpText.text = "HP: " + hero.currentHP + "/" + hero.baseHP;
        mpText.text = "MP: " + hero.currentMP + "/" + hero.baseMP;
    }

    public void InitiatePanel(BaseCharacter hero)
    {
        nameText.text = hero.characterName;
        hpText.text = "HP: " +  hero.currentHP + "/" + hero.baseHP;
        mpText.text = "MP: " + hero.currentMP + "/" + hero.baseMP;
        if (classNameText)
        {
            mpText.text = hero.heroClass.ToString();
        }
    }
}
