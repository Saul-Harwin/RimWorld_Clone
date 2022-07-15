using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int seed;
    Rivers rivers;
    public TileData[] tileData;
    public int width, height;
    public Tile[,] tiles;
    [SerializeField] private Object[] possibleObjectTypes;
    public List<Object> objects;
    // public List<List<Vector2>> objectPoints;

    // [SerializeField] Tile grassTile, waterTile;
    [SerializeField] GameObject tilePrefab;

    public MapData heightMapData; 
    public MapData moistureMapData; 
    public MapData temperatureMapData; 

    public float[,] heightMap;
    public float[,] moistureMap;
    public float[,] temperatureMap;
    public List<Stockpile> stockpiles;
    public GameObject stockpileHighlightGameObject;

    public void GenerateWorld() {
        rivers = GameManager.Instance.world.GetComponent<Rivers>();
        heightMap = new Noise().GenerateNoiseMap(heightMapData);
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x,y] = ScriptableObject.Instantiate(PickTile(heightMap[x,y]).tile);
                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tiles[x,y].Initialize(new Vector2Int(x,y), PickTile(heightMap[x,y]), isOffset, tilePrefab);
            }
        }
        rivers.GenerateRivers();
        objects = new List<Object>();
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
        int i = 0;
        foreach (Object worldObject in possibleObjectTypes) {
            objectPoints.Add(this.GetComponent<ObjectPlacer>().GeneratePoints());
        }

        while (repeat == true) {
            repeat = false;
            for (int objectIndex = 0; objectIndex < possibleObjectTypes.Length; objectIndex++) {
                if (objectPoints[objectIndex].Count != 0) {
                    objectPoints[objectIndex] = this.GetComponent<ObjectPlacer>().Placer(objectPoints[objectIndex], ScriptableObject.Instantiate(possibleObjectTypes[objectIndex]));
                    repeat = true;
                }
            }
            i++;
        }
    }

    public Tile screenToTilePosition(){
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(mouseWorldPosition.x > 0 && mouseWorldPosition.x < width + 0.5f && mouseWorldPosition.y > 0 && mouseWorldPosition.x < height + 0.5f){
            return GameManager.Instance.world.tiles[Mathf.RoundToInt(mouseWorldPosition.x), Mathf.RoundToInt(mouseWorldPosition.y)];
        }
        return null;
    }

    public static Tile findNearbyWalkableTile(Tile tile){
        List<Tile> neighbours = GameManager.Instance.pathfinder.GetNeighbourList(tile);
        while(true){
            for (int i = neighbours.Count - 1; i >= 0; i--)
            {
                if(neighbours[i].tileData.walkable){
                    return neighbours[i];
                }
                neighbours.Add(neighbours[i]);
            }
        }
    }
    public static Tile findNearbyEmptyTile(Tile tile){
        List<Tile> neighbours = GameManager.Instance.pathfinder.GetNeighbourList(tile);
        while(true){
            for (int i = neighbours.Count - 1; i >= 0; i--)
            {
                if(neighbours[i].occupyingObject == null && neighbours[i].tileData.walkable){
                    return neighbours[i];
                }
                neighbours.Add(neighbours[i]);
            }
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
    public bool walkable;
}