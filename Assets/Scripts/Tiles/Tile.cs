using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    public Vector2Int position;
    public Color color, offsetColor;
    public GameObject go;
    public Unit occupyingUnit = null;
    public Object occupyingObject = null;
    public TileData tileData;
    public PathFindingData pathFindingData;

    public void Initialize(Vector2Int pPos, TileData pTileData, bool isOffset, GameObject pTilePrefab){
        position = pPos;
        tileData = pTileData;
        pathFindingData = new PathFindingData();
        go = Instantiate(pTilePrefab, new Vector3(pPos.x, pPos.y), Quaternion.identity);
        go.GetComponent<TileGO>().parent = this;
        if(isOffset) { go.GetComponent<SpriteRenderer>().color = color; }
        else { go.GetComponent<SpriteRenderer>().color = offsetColor; }
        go.transform.SetParent(GameObject.Find("Tiles").transform);
    }

    public void SetOccupyingUnit(Unit unit){
        occupyingUnit = unit;
        unit.occupyingTile = this;
    }

}

public class PathFindingData
{
    public int gCost;
    public int hCost;
    public int fCost;
    public Tile cameFromTile;

    public void CalculateFCost(){
        fCost = gCost + hCost;
    }
}