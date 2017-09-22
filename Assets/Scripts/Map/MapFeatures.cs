using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapFeatures {

    /*
     * All MapFeatures are string arrays filled with characters that represent the tiles to spawn.
     * The tiles should be separated by two spaces so they more closely resemble a square grid in this file.
     * 
     *  airship;              // a
     *  mountainTile;         // m
     *  doorTile;             // d
     *  forestTile;           // f
     *  treasureChest;        // c
     */

    public static string[] forestCircle = new string[]{   
                                                       "   f  f  f   ",
                                                       "f  f     f  f",
                                                       "f     c     f",
                                                       "f  f     f  f",
                                                       "   f  f  f   ",
                                                       };/*{
                                                       "      f  f  f      ",
                                                       "   f  f  f  f  f   ",
                                                       "f  f  f     f  f  f",
                                                       "f  f     c     f  f",
                                                       "f  f  f     f  f  f",
                                                       "   f  f  f  f  f   ",
                                                       "      f  f  f      ",
                                                       };*/
}
public enum FeatureTypes
{
    FOREST_CIRCLE
}
