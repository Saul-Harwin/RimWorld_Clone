using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PerlinWorm {
    private float[,] noiseMap;
    private Vector2 currentDirection;
    private Vector2 currentPosition;
    private Vector2 convergancePoint;
    [Range(0.5f, 0.9f)]
    public float weight = 0.6f;

    public PerlinWorm(float[,] noiseMap, Vector2 startPosition, Vector2 convergancePoint, float weight) {
        currentDirection = Random.insideUnitCircle.normalized;
        this.noiseMap = noiseMap;
        this.currentPosition = startPosition;
        this.convergancePoint = convergancePoint;
        this.weight = weight;
    }

    public Vector2 MoveTowardsConvergancePoint() {
        Vector3 direction = GetPerlinNoiseDirection();
        var directionToConvergancePoint = (this.convergancePoint - currentPosition).normalized;
        var endDirection = ((Vector2)direction * (1 - weight) + directionToConvergancePoint * weight).normalized;
        currentPosition += endDirection;
        return currentPosition;
    }

    private Vector3 GetPerlinNoiseDirection() {
        if ((int)currentPosition.x < GameManager.Instance.world.width && (int)currentPosition.x >= 0 &&  (int)currentPosition.y < GameManager.Instance.world.height && (int)currentPosition.y >= 0) {
            float noise = noiseMap[(int)currentPosition.x, (int)currentPosition.y];
            float degrees = -90 + (noise - 0) * (90 + 90) / (1 - 0);
            currentDirection = (Quaternion.AngleAxis(degrees, Vector3.forward) * currentDirection).normalized;
            return currentDirection;
        }
        return currentDirection;
    }

    public List<Vector2> MoveLength(int length) {
        var list = new List<Vector2>();
        foreach (var item in Enumerable.Range(0, length)) {
            var result = MoveTowardsConvergancePoint();
            list.Add(result);
            if (Vector2.Distance(this.convergancePoint, result) < 1) {
                break;
            }
        }

        while (Vector2.Distance(this.convergancePoint, currentPosition) > 1) {
            weight = 0.9f;
            var result = MoveTowardsConvergancePoint();
            list.Add(result);
            if (Vector2.Distance(this.convergancePoint, result) < 1) {
                break;
            }
        }
    

        return list;
    }
}
