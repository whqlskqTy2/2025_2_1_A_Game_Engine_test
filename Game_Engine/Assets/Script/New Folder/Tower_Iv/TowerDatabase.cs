using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class TowerEntry
{
    public TowerType type;
    public GameObject prefab;
}

public class TowerDatabase : MonoBehaviour
{
    public TowerEntry[] towers;

    public GameObject GetPrefab(TowerType type)
    {
        foreach (var t in towers)
        {
            if (t != null && t.type == type)
                return t.prefab;
        }
        return null;
    }
}