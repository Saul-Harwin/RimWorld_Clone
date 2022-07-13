using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/Objects/Object", order = 1)]
public abstract class Object : ScriptableObject {
    public Vector2Int position;
    public GameObject objectPrefab;
    public GameObject go;
    public int objectType;
    public Tile occupyingTile;
    public Sprite[] sprites;
    public Job associatedJob;
    public abstract void Initialize(Vector2Int pos, Tile pOccupyingTile);
}
