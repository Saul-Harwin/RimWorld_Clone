using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stockpile 
{
    public List<Tile> tiles;
    
    public void Initialize(){
        tiles = new List<Tile>();
    }

    public void CreateOverlayImages(){
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if(tiles[i].tileData.type != 1 && tiles[i].tileData.type != 2 || TileExistsInStockPile(tiles[i])){
                tiles.RemoveAt(i);
            }
        }
        for (int i = 0; i < tiles.Count; i++)
        {
           GameObject highlight = GameObject.Instantiate(GameManager.Instance.world.stockpileHighlightGameObject, (Vector3Int)tiles[i].position, Quaternion.identity);
           highlight.transform.SetParent(GameObject.Find("World Canvas").transform);
        }

        // Add to stockpile list in world
        GameManager.Instance.world.stockpiles.Add(this);
    }

    public static Tile findFreeSpace(){
        for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.world.stockpiles[i].tiles.Count; j++)
            {
                if(GameManager.Instance.world.stockpiles[i].tiles[j].occupyingObject == null){
                    return GameManager.Instance.world.stockpiles[i].tiles[j];
                } 
            }
        }
        return null;
    }

    bool TileExistsInStockPile(Tile tile){
        for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.world.stockpiles[i].tiles.Count; j++)
            {
                if(GameManager.Instance.world.stockpiles[i].tiles[j] == tile){
                    return true;
                } 
            }
        }
        return false;
    }

    public static int freeSpaces(){
        int freeSpaceCount = 0;
        for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
        {
            for (int j = 0; j < GameManager.Instance.world.stockpiles[i].tiles.Count; j++)
            {
                if(GameManager.Instance.world.stockpiles[i].tiles[j].occupyingObject == null) freeSpaceCount++;
            }
        }
        return freeSpaceCount;
    }
}
