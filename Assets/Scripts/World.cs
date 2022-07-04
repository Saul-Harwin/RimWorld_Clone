using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public TileData[] tileData;
    public int width, height;
    public Tile[,] tiles;
    public Object[] objects;

    // [SerializeField] Tile grassTile, waterTile;
    [SerializeField] GameObject tilePrefab;

    public MapData heightMapData; 
    public MapData moistureMapData; 
    public MapData temperatureMapData; 

    float[,] heightMap;
    float[,] moistureMap;
    float[,] temperatureMap;

    public void GenerateWorld() {
        heightMap = new Noise().GenerateNoiseMap(heightMapData, width, height);
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x,y] = ScriptableObject.Instantiate(PickTile(heightMap[x,y]).tile);
                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tiles[x,y].Initialize(new Vector2(x,y), PickTile(heightMap[x,y]), isOffset, tilePrefab);
            }
        }
        SpawnObjects();
    }

    TileData PickTile(float height) {
        for (var i = 0; i < tileData.Length; i++) {
            if (height <= tileData[i].height) {
                return tileData[i];
            }
        }
        return tileData[tileData.Length - 1];
    }

    void SpawnObjects() {
        foreach (Object worldObject in objects) {
            this.GetComponent<ObjectPlacer>().PlaceObject(worldObject);
        }
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
    public int type;
}