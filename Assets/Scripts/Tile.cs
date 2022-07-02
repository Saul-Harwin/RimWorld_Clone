using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    private Vector2 position;
    public Color color, offsetColor;
    public GameObject go;

    public void Initialize(Vector2 pPos, bool isOffset, GameObject pTilePrefab){
        position = pPos;
        go = Instantiate(pTilePrefab, pPos, Quaternion.identity);
        go.AddComponent<TileGO>();
        if(isOffset) { go.GetComponent<SpriteRenderer>().color = color; }
        else { go.GetComponent<SpriteRenderer>().color = offsetColor; }
    }
}