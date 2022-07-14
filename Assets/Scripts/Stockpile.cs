using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stockpile 
{
    public List<TileHighlight> tiles;
    
    public void Initialize(){
        tiles = new List<TileHighlight>();
    }

    public void CreateOverlayImages(){
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if(tiles[i].tile.tileData.type != 1 && tiles[i].tile.tileData.type != 2 || TileExistsInStockPile(tiles[i].tile)){
                tiles.RemoveAt(i);
            }
        }
        for (int i = 0; i < tiles.Count; i++)
        {
           tiles[i].highlight = GameObject.Instantiate(GameManager.Instance.world.stockpileHighlightGameObject, (Vector3Int)tiles[i].tile.position, Quaternion.identity);
           tiles[i].highlight.transform.SetParent(GameObject.Find("World Canvas").transform);
        }

        // Add to stockpile list in world
        GameManager.Instance.world.stockpiles.Add(this);
    }

    public static Tile findFreeSpace(){
        for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.world.stockpiles[i].tiles.Count; j++)
            {
                if(GameManager.Instance.world.stockpiles[i].tiles[j].tile.occupyingObject == null){
                    return GameManager.Instance.world.stockpiles[i].tiles[j].tile;
                } 
            }
        }
        return null;
    }

    public static bool TileExistsInStockPile(Tile tile){
        for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.world.stockpiles[i].tiles.Count; j++)
            {
                if(GameManager.Instance.world.stockpiles[i].tiles[j].tile == tile){
                    return true;
                } 
            }
        }
        return false;
    }

    public static TileHighlight returnHighlightFromTile(Tile tile){
        for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.world.stockpiles[i].tiles.Count; j++)
            {
                if(GameManager.Instance.world.stockpiles[i].tiles[j].tile == tile){
                    return GameManager.Instance.world.stockpiles[i].tiles[j];
                }
            }
        }
        return null;

    }

    public static int freeSpaces(){
        int freeSpaceCount = 0;
        for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.world.stockpiles[i].tiles.Count; j++)
            {
                if(GameManager.Instance.world.stockpiles[i].tiles[j].tile.occupyingObject == null) freeSpaceCount++;
            }
        }
        return freeSpaceCount;
    }
}

public class TileHighlight {
    public Tile tile;
    public GameObject highlight;
    public TileHighlight(Tile pTile){
        tile = pTile;
    }
}