using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {

    public List<BaseItem> itemsToSpawn = new List<BaseItem>();
    public List<BaseItem> mazeItemSpawns = new List<BaseItem>();
    public List<BaseItem> equipSpawns = new List<BaseItem>();
    public List<BaseItem> betterEquipSpawns = new List<BaseItem>();
    public static ItemGenerator instance;
    public Key key;
    int keySpawnTimer = 5;

    private void Start()
    {
        instance = this;
    }

    public BaseItem GetTreasureBasedOnLocation(ContinentType area = ContinentType.None)
    {
        if (--keySpawnTimer == 0)
        {
            keySpawnTimer = 5;
            return key;
        }
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

    public static BaseItem GetGoodEquip()
    {
        return instance.betterEquipSpawns[
            Random.Range(0, instance.betterEquipSpawns.Count)];
    }

    public static BaseItem GetEquip()
    {
        return instance.equipSpawns[
            Random.Range(0, instance.equipSpawns.Count)];
    }
}
