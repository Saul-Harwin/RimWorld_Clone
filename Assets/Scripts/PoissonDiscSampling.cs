using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSampling : MonoBehaviour {
    public static List<Vector2> Place(int width, int height, float radius, int numSampleBeforeRejection = 30) {
        float cellSize = radius / Mathf.Sqrt(2);
        int[,] grid = new int[width, height];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(new Vector2(width, height) / 2);
        while(spawnPoints.Count > 0) {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (var i = 0; i < numSampleBeforeRejection; i++) {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2*radius);
                
                if(IsValid(candidate, width, height, radius, points, grid)) {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)candidate.x, (int)candidate.y] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }

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
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX   = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY   = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

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
