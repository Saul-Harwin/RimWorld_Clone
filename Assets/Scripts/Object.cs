using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/Object", order = 1)]
public class Object : ScriptableObject {
    public Vector2Int position;
    [SerializeField] private GameObject objectPrefab;
    public GameObject go;
    public int objectType;
    public Tile occupyingTile;
    public Sprite[] sprites;

    public void Initialize(Vector2Int pos, Tile pOccupyingTile){
        position = pos;
        occupyingTile = pOccupyingTile;
        if(objectType == 0){ // If tree, place at an offset of .6f
            go = Instantiate(objectPrefab, new Vector3(position.x, position.y + 0.6f), Quaternion.identity);
        }
        else{
            go = Instantiate(objectPrefab, new Vector3(position.x, position.y), Quaternion.identity);
        }
        go.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        go.transform.SetParent(GameObject.Find("Objects").transform);
    }
}