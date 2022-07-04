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
    public Sprite sprite;

    public void Initialize(Vector2Int pPos, GameObject pUnitPrefab){
        position = pPos;
        go = Instantiate(pUnitPrefab, new Vector3(pPos.x, pPos.y + 0.2f), Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        go.GetComponent<UnitGO>().parent = this;
    }
}

public class Stats {
    int health;
}