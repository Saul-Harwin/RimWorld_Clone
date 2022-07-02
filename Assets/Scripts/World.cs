using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public TileData[] tileHeights;

    [SerializeField] private int width, height;
    public Tile[,] tiles;
    // [SerializeField] Tile grassTile, waterTile;
    [SerializeField] GameObject tilePrefab;

    public MapData heightMapData; 
    public MapData moistureMapData; 
    public MapData temperatureMapData; 

    float[,] heightMap;
    float[,] moistureMap;
    float[,] temperatureMap;

    void Start() {
        heightMap = new Noise().GenerateNoiseMap(heightMapData, width, height);
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x,y] = PickTile(heightMap[x,y]);
                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tiles[x,y].Initialize(new Vector2(x,y), isOffset, tilePrefab);
            }
        }
    }

    Tile PickTile(float height) {
        for (var i = 0; i < tileHeights.Length; i++) {
            if (height <= tileHeights[i].height) {
                return tileHeights[i].tile;
            }
        }
        return tileHeights[tileHeights.Length - 1].tile;
    }
}

[System.Serializable]
public struct MapData {
    public int seed; 
    public int numOctaves; 
    public float lacunarity; 
    public float frequency; 
    public float persistance; 
    public float amplitude;
}

[System.Serializable]
public struct TileData {
    public Tile tile; 
    public float height;
}