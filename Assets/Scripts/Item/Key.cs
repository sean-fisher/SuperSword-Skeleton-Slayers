using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : BaseItem
{

    [SerializeField]
    private SimpleItemData sitemData;

    public override ItemData GetItemData()
    {
        return sitemData;
    }

}

