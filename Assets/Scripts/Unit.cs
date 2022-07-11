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
    public UnitState state;
    public Job currentJob;
    public Sprite sprite;

    public void Initialize(Vector2Int pPos, GameObject pUnitPrefab){
        position = pPos;
        state = UnitState.IDLE;
        go = Instantiate(pUnitPrefab, new Vector3(pPos.x, pPos.y), Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        go.GetComponent<UnitGO>().parent = this;
        go.transform.SetParent(GameObject.Find("Units").transform);
    }
}

public enum UnitState {
    IDLE,
    HARVESTING,
}

[System.Serializable]
public class Stats {
    public int health;
    public float moveSpeed;
}