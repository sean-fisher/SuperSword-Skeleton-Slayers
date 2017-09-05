using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDisplayPanel : MonoBehaviour {

    public Text nameText;
    public Text hpText;
    public Text mpText;

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
    }
}
