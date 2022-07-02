using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/Unit", order = 1)]
public class Unit : ScriptableObject
{
    public Vector2Int position;
    public Stats stats;
    public GameObject go;
    public Tile occupypingTile;

    public void Initialize(Vector2Int pPos, GameObject pUnitPrefab){
        position = pPos;
        go = Instantiate(pUnitPrefab, ((Vector3Int)pPos), Quaternion.identity);
    }
}

public class Stats {
    int health;
}