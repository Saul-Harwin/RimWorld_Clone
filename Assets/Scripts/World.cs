using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int seed;
    public TileData[] tileData;
    public int width, height;
    public Tile[,] tiles;
    public Object[] objects;
    // public List<List<Vector2>> objectPoints;

    // [SerializeField] Tile grassTile, waterTile;
    [SerializeField] GameObject tilePrefab;

    public MapData heightMapData; 
    public MapData moistureMapData; 
    public MapData temperatureMapData; 

    float[,] heightMap;
    float[,] moistureMap;
    float[,] temperatureMap;

    public void GenerateWorld() {
        heightMap = new Noise().GenerateNoiseMap(heightMapData);
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
        List<List<Vector2>> objectPoints = new List<List<Vector2>>();
        bool repeat = true;
        int _seed = seed;
        int i = 0;
        foreach (Object worldObject in objects) {
            _seed = (int)(_seed / 3.8f);
            objectPoints.Add(this.GetComponent<ObjectPlacer>().GeneratePoints(_seed));
        }

        while (repeat == true) {
            repeat = false;
            for (int objectIndex = 0; objectIndex < objects.Length; objectIndex++) {
                if (objectPoints[objectIndex].Count != 0) {
                    objectPoints[objectIndex] = this.GetComponent<ObjectPlacer>().Placer(objectPoints[objectIndex], objects[objectIndex]);
                    repeat = true;
                }
            }
            i ++;
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