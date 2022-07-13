using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Rivers : MonoBehaviour {
    public Vector2 riverStartPosition;
    public int riverLength = 50;
    public float weight = 0.6f;

    public void GenerateRivers() {
        float[,] noiseMap = GameManager.Instance.world.heightMap;
        var result = FindLocalMaxima(noiseMap);
        var toCreate = result.Where(pos => noiseMap[pos.x, pos.y] > GameManager.Instance.world.tileData[2].height).OrderBy(a => Guid.NewGuid()).Take(5).ToList();
        var waterMinimas = FindLocalMinima(noiseMap);
        waterMinimas = waterMinimas.Where(pos => noiseMap[pos.x, pos.y] < GameManager.Instance.world.tileData[0].height).OrderBy(pos => noiseMap[pos.x, pos.y]).Take(20).ToList();
        foreach (var item in toCreate) {
            //SetTileTo(item.x, item.y, maxPosTile);
            CreateRiver(item, waterMinimas);
            // CreateRiver(item, waterMinimas);
            // CreateRiver(item, waterMinimas);
            // CreateRiver(item, waterMinimas);
            // CreateRiver(item, waterMinimas);
            // CreateRiver(item, waterMinimas);
            //return;
        }
    }

    private void CreateRiver(Vector2Int startPosition, List<Vector2Int> waterMinimas) {
        PerlinWorm worm;
        var closestWaterPos = waterMinimas.OrderBy(pos => Vector2.Distance(pos, startPosition)).First();
        worm = new PerlinWorm(GameManager.Instance.world.heightMap, startPosition, closestWaterPos, weight);

        var position = worm.MoveLength(riverLength);
        PlaceRiverTile(position);
    }

    void PlaceRiverTile(List<Vector2> positons) {
        foreach (var pos in positons) {
            var tilePos = GameManager.Instance.world.tiles[(int)pos.x, (int)pos.y].position;
            if (tilePos.x - 1 < 0 || tilePos.x + 1 >= GameManager.Instance.world.width || tilePos.y - 1 < 0 || tilePos.y + 1 >= GameManager.Instance.world.height) {
                break;
            }
            UpdateTile(GameManager.Instance.world.tiles[(int)pos.x, (int)pos.y]);
            UpdateTile(GameManager.Instance.world.tiles[(int)(pos.x + Vector3Int.right.x), (int)(pos.y + Vector3Int.right.y)]);
            UpdateTile(GameManager.Instance.world.tiles[(int)(pos.x + Vector3Int.left.x),  (int)(pos.y + Vector3Int.left.y )]);
            UpdateTile(GameManager.Instance.world.tiles[(int)(pos.x + Vector3Int.up.x),    (int)(pos.y + Vector3Int.up.y   )]);
            UpdateTile(GameManager.Instance.world.tiles[(int)(pos.x + Vector3Int.down.x),  (int)(pos.y + Vector3Int.down.y )]);
        }
    }

    void UpdateTile(Tile tile) {
        if ((tile.position.x % 2 == 0 && tile.position.y % 2 != 0) || (tile.position.x % 2 != 0 && tile.position.y % 2 == 0)) {
            tile.go.GetComponent<SpriteRenderer>().color = new Color(0.07f, 0.31f, 0.54f);
        } else {
            tile.go.GetComponent<SpriteRenderer>().color = new Color(0, 0.6f, 0.9f);
        }
        tile.color = new Color(0, 0.6f, 0.9f);
        tile.offsetColor = new Color(0.07f, 0.31f, 0.54f);
        tile.tileData.type = 0;
        tile.tileData.walkable = false;
    }

    public static List<Vector2Int> FindLocalMaxima(float[,] noiseMap) {
        List<Vector2Int> maximas = new List<Vector2Int>();
        for (int x = 0; x < noiseMap.GetLength(0); x++) {
            for (int y = 0; y < noiseMap.GetLength(1); y++) {
                var noiseVal = noiseMap[x, y];
                if (CheckNeighbours(x, y, noiseMap, (neighbourNoise) => neighbourNoise > noiseVal)) {
                    maximas.Add(new Vector2Int(x, y));
                }
            }
        }
        return maximas;
    }

    public static List<Vector2Int> FindLocalMinima(float[,] noiseMap) {
        List<Vector2Int> minima = new List<Vector2Int>();
        for (int x = 0; x < noiseMap.GetLength(0); x++) {
            for (int y = 0; y < noiseMap.GetLength(1); y++) {
                var noiseVal = noiseMap[x, y];
                if (CheckNeighbours(x, y, noiseMap, (neighbourNoise) => neighbourNoise < noiseVal)) {
                    minima.Add(new Vector2Int(x, y));
                }
            }
        }
        return minima;
    }

    static List<Vector2Int> directions = new List<Vector2Int> {
        new Vector2Int( 0, 1),
        new Vector2Int( 1, 1),
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, 0), 
        new Vector2Int(-1,-1),
        new Vector2Int( 0,-1),
        new Vector2Int( 1,-1) 
    };

    private static bool CheckNeighbours(int x, int y, float[,] noiseMap, Func<float, bool> failCondition) {
        foreach (var dir in directions) {
            var newPost = new Vector2Int(x + dir.x, y + dir.y);

            if (newPost.x < 0 || newPost.x >= noiseMap.GetLength(0) || newPost.y < 0 || newPost.y >= noiseMap.GetLength(1)) {
                continue;
            }

            if (failCondition(noiseMap[x + dir.x, y + dir.y])) {
                return false;
            }
        }
        return true;
    }

}
