using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private int width, height;
    public Tile[,] tiles;
    [SerializeField] Tile grassTile, waterTile;
    [SerializeField] GameObject tilePrefab;
    
    void Start()
    {
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x,y] = grassTile;
                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tiles[x,y].Initialize(new Vector2(x,y), isOffset, tilePrefab);
            }
        }
    }
}