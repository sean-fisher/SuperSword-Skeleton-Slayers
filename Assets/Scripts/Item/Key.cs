using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : BaseItem
{

    [SerializeField]
    private SimpleItemData itemData;

    public override ItemData GetItemData()
    {
        return itemData;
    }

}

