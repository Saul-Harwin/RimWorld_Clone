using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSampling : MonoBehaviour {
    public static List<Vector2> Place(int width, int height, float radius, int seed, int numSampleBeforeRejection = 30) {
        float cellSize = radius / Mathf.Sqrt(2);
        int[,] grid = new int[width, height];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();


        Vector2 randomStartPoint = new Vector2(Mathf.PerlinNoise(seed * 0.001f, seed * 0.001f) * width, Mathf.PerlinNoise(seed * 0.001f, seed * 0.001f) * height);
        spawnPoints.Add(randomStartPoint);

        float x = seed;
        while(spawnPoints.Count > 0) {
            int spawnIndex = Mathf.RoundToInt(Mathf.PerlinNoise((float)((x) * 0.01f), (float)((x) * 0.01f)) * (spawnPoints.Count - 1));
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (var i = 0; i < numSampleBeforeRejection; i++) {
                float angle = Mathf.PerlinNoise((spawnPoints.Count + i + x) * 0.1f, (spawnPoints.Count + i + x) * 0.1f) * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * ((Mathf.PerlinNoise(spawnPoints.Count * i * 0.01f, spawnPoints.Count * i * 0.01f) + 1) * radius);
                if(IsValid(candidate, width, height, radius, points, grid)) {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)candidate.x, (int)candidate.y] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            x += 2;

            if(!candidateAccepted) {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        return points;
    }

    static bool IsValid(Vector2 candidate, int width, int height, float radius, List<Vector2> points, int[,] grid) {
        if (candidate.x >= 0 && candidate.x < width && candidate.y >= 0 && candidate.y < height) {
            int cellX = (int)(candidate.x);
            int cellY = (int)(candidate.y);
            int searchStartX = Mathf.Max(0, cellX - (int)Mathf.Ceil(radius));
            int searchEndX   = Mathf.Min(cellX +    (int)Mathf.Ceil(radius), grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - (int)Mathf.Ceil(radius));
            int searchEndY   = Mathf.Min(cellY +    (int)Mathf.Ceil(radius), grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++) {
                for (int y = searchStartY; y <= searchEndY; y++) {
                    int pointIndex = grid[x,y] - 1;
                    if (pointIndex != -1) {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if(sqrDst < radius * radius) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
