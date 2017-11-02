using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {

    public List<BaseItem> itemsToSpawn = new List<BaseItem>();
    public List<BaseItem> mazeSpawns = new List<BaseItem>();
    public static ItemGenerator instance;

    private void Start()
    {
        instance = this;
    }

    public BaseItem GetTreasureBasedOnLocation(ContinentType area = ContinentType.None)
    {
        switch (area)
        {
            case (ContinentType.FOREST):
                return mazeSpawns[Random.Range(0, mazeSpawns.Count)];
            case (ContinentType.ICECAVE):
                return mazeSpawns[Random.Range(0, mazeSpawns.Count)];
            case (ContinentType.LAVACAVE):
                return mazeSpawns[Random.Range(0, mazeSpawns.Count)];
            case (ContinentType.MOUNTAINCAVE):
                return mazeSpawns[Random.Range(0, mazeSpawns.Count)];
            case (ContinentType.PYRAMID):
                return mazeSpawns[Random.Range(0, mazeSpawns.Count)];
            default:
                return itemsToSpawn[Random.Range(0, itemsToSpawn.Count)];

        }
    }
}
