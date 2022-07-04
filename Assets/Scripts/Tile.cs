using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    private Vector2 position;
    public Color color, offsetColor;
    public GameObject go;
    public Unit occupyingUnit = null;
    public GameObject content = null;
    public TileData  tileData;

    public void Initialize(Vector2 pPos, TileData pTileData, bool isOffset, GameObject pTilePrefab){
        position = pPos;
        tileData = pTileData;
        go = Instantiate(pTilePrefab, pPos, Quaternion.identity);
        go.GetComponent<TileGO>().parent = this;
        if(isOffset) { go.GetComponent<SpriteRenderer>().color = color; }
        else { go.GetComponent<SpriteRenderer>().color = offsetColor; }
    }

    public void SetOccupyingUnit(Unit unit){
        occupyingUnit = unit;
        unit.occupypingTile = this;
    }

}