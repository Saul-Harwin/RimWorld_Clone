using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {
    public float radius = 1.41421356237f;
    int width = 100;
    int height = 100;
    public int numSampleBeforeRejection = 30;

    public List<Vector2> GeneratePoints() {
        width = GameManager.Instance.world.width;
        height = GameManager.Instance.world.height;
        return PoissonDiscSampling.Place(width, height, radius, numSampleBeforeRejection);
    }

    public List<Vector2> Placer(List<Vector2> points, Object worldObject) {
        Vector2 point = points[0];
        if (GameManager.Instance.world.tiles[(int)(point.x), (int)(point.y)].tileData.type == 2 &&
            GameManager.Instance.world.tiles[(int)(point.x), (int)(point.y)].occupyingObject == null)
        {
            worldObject.Initialize(new Vector2Int((int)point.x, (int)point.y), GameManager.Instance.world.tiles[(int)point.x, (int)point.y], ObjectClass.HARVESTABLE);
            GameManager.Instance.world.tiles[(int)(point.x), (int)(point.y)].occupyingObject = worldObject;
        }
        points.RemoveAt(0);
        return points;
    }

}
