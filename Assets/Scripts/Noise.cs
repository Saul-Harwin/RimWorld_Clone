using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour {
    float[,] noiseMap;

    public float[,] GenerateNoiseMap(MapData mapData, int width, int height) {
        noiseMap = new float[width, height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float perlinValue = 0; 
                float frequency = mapData.frequency;
                float amplitude = mapData.amplitude;

                for (var o = 0; o < mapData.numOctaves; o++) {
                    perlinValue += Mathf.PerlinNoise((x + mapData.seed) * frequency, (y + mapData.seed) * frequency) * amplitude;
                    frequency *= mapData.lacunarity;
                    amplitude *= mapData.persistance;
                }
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }
}
