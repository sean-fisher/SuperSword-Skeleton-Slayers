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
                                                       "f     i     f",
                                                       "f  f     f  f",
                                                       "   f  f  f   ",
                                                       };
    public static string[] darkForest = new string[]{
                                                       "   f     ",
                                                       "f  f  f  ",
                                                       "   !     ",
                                                       "-  -  -  ",
                                                       };
    /*public static string[] darkForest = new string[]{
                                                       "      m     m     ",
                                                       "m  f  f  f     m  ",
                                                       "m  f  f  f     m  ",
                                                       "m  f  !  f     m  ",
                                                       "   m        m     ",
                                                       };*/

    public static string[] castle = new string[]{
                                                       "-  -  -  ",
                                                       "-  .  -  ",
                                                       "-  -  -  ",
                                                       };
    public static string[] airshipSalesman = new string[]{
                                                        "a  0"
                                                       };
    public static string[] itemShops = new string[]{
                                                        "(  )"
                                                       };

    public static string[] GetFeature(FeatureTypes featureType)
    {
        switch (featureType)
        {
            case (FeatureTypes.FOREST_CIRCLE):
                return MapFeatures.forestCircle;
            case (FeatureTypes.DARK_FOREST):
                return MapFeatures.darkForest;
            case (FeatureTypes.AIRSHIP_SALESMAN):
                return MapFeatures.airshipSalesman;
            case (FeatureTypes.CASTLE):
                return MapFeatures.castle;
            case (FeatureTypes.SHOPS):
                return MapFeatures.itemShops;
            default:
                return MapFeatures.forestCircle;
        }
    }
}
public enum FeatureTypes
{
    FOREST_CIRCLE,
    DARK_FOREST,
    AIRSHIP_SALESMAN,
    CASTLE,
    SHOPS
}
