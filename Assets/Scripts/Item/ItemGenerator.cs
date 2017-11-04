using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {

    public List<BaseItem> itemsToSpawn = new List<BaseItem>();
    public List<BaseItem> mazeItemSpawns = new List<BaseItem>();
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
                return mazeItemSpawns[Random.Range(0, mazeItemSpawns.Count)];
            case (ContinentType.ICECAVE):
                return mazeItemSpawns[Random.Range(0, mazeItemSpawns.Count)];
            case (ContinentType.LAVACAVE):
                return mazeItemSpawns[Random.Range(0, mazeItemSpawns.Count)];
            case (ContinentType.MOUNTAINCAVE):
                return mazeItemSpawns[Random.Range(0, mazeItemSpawns.Count)];
            case (ContinentType.PYRAMID):
                return mazeItemSpawns[Random.Range(0, mazeItemSpawns.Count)];
            default:
                return itemsToSpawn[Random.Range(0, itemsToSpawn.Count)];

        }
    }
}
