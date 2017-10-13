using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseCharacter : MonoBehaviour {

    public string characterName;

    public HeroClasses heroClass;

    public AttackType strength;
    public AttackType weakness;

    public int baseHP;
    public int currentHP;

    public int baseMP;
    public int currentMP;

    public int baseATK;
    public int currentATK;

    public int baseDEF;
    public int currentDEF;

    public int baseEVA;
    public int currentEVA;

    public int baseSKL;
    public int currentSKL;

    public int goldDrop = 0;

    public bool isDead = false;

    // The list of (up to) four attacks that the hero can use in battle.
    [Header("Attacks usable in battle (<=4)")]
    public List<Attack> usableAttacks = new List<Attack>();

    // The list of all attacks that the hero can learn
    [Header("Attacks usable in battle (<=4)")]
    public List<Attack> allAttacks = new List<Attack>();

    public EquipData headgear;
    public EquipData armor;
    public EquipData accessory;
    public EquipData weapon;

    public bool isDefending = false;

    // Used for battle status;
    public TurnState CurrTurnState { get; set; }
}