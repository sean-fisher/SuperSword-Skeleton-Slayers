using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This is essentially an enum, but with strings not ints.
 * 
 * Used for determining which heroes can equip particular equipment; 
 * it does not contain the custom names input by the player.
 * 
 * ALL NAMES TEMP
*/
public static class HeroNames
{

    public const string hero6Name = "Leonardo";
    public const string hero2Name = "Donatello";
    public const string hero3Name = "Raphael";
    public const string hero4Name = "Michelangelo";
    public const string hero5Name = "Kermit the Fron";
}
public enum HeroClasses
{
    KNIGHT,
    MAGE,
    MONK,
    NINJA,
    CHEF,
    ARCHER
}
