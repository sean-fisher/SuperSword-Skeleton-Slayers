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
    public Image classsprite;

    public void UpdateDisplay(BaseCharacter hero)
    {
        Debug.Log("update");
        hpText.text = "HP: " + hero.currentHP + "/" + hero.baseHP;
        mpText.text = "AP: " + hero.currentMP + "/" + hero.baseMP;

        if (!isBattleDisplay)
        {
            nameText.text = hero.characterName;
            switch (hero.heroClass)
            {
                case (HeroClasses.KNIGHT):
                    classNameText.text = "Knight";
                    classsprite.sprite = PauseMenu.pauseMenu.knight;
                    break;
                case (HeroClasses.MAGE):
                    classNameText.text = "Mage";
                    classsprite.sprite = PauseMenu.pauseMenu.mage;
                    break;
                case (HeroClasses.ARCHER):
                    classNameText.text = "Archer";
                    classsprite.sprite = PauseMenu.pauseMenu.archer;
                    break;
            }
            RectTransform rt = GetComponent<RectTransform>();
            rt.position = new Vector2(76.7f, 136);
            rt.sizeDelta = new Vector2(46.22f, 63.26f);
        }
    }

    public void InitiatePanel(BaseCharacter hero)
    {
        nameText.text = hero.characterName;
        hpText.text = "HP: " +  hero.currentHP + "/" + hero.baseHP;
        mpText.text = "AP: " + hero.currentMP + "/" + hero.baseMP;
        if (classNameText)
        {
            mpText.text = hero.heroClass.ToString();
        }
        if (!isBattleDisplay)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.position = new Vector2(76.7f, 136);
            rt.sizeDelta = new Vector2(46.22f, 63.26f);
        }
    }
}
