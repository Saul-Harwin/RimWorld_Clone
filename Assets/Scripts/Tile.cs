using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    private Vector2 position;
    public Color color, offsetColor;
    public GameObject go;
    public bool occupied = false;
    public void Initialize(Vector2 pPos, bool isOffset, GameObject pTilePrefab){
        position = pPos;
        occupied = false;
        go = Instantiate(pTilePrefab, pPos, Quaternion.identity);
        if(isOffset) { go.GetComponent<SpriteRenderer>().color = color; }
        else { go.GetComponent<SpriteRenderer>().color = offsetColor; }
    }
}