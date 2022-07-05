using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {
    public float radius = 1.41421356237f;
    int width = 100;
    int height = 100;
    public int numSampleBeforeRejection = 30;
    List<Vector2> points;

    public void PlaceObject(Object worldObject, int seed) {
        width = GameManager.Instance.world.width;
        height = GameManager.Instance.world.height;
        points = PoissonDiscSampling.Place(width, height, radius, seed, numSampleBeforeRejection);
        foreach (Vector2 point in points) {
            if (GameManager.Instance.world.tiles[(int)(point.x), (int)(point.y)].tileData.type == 2 &&
                GameManager.Instance.world.tiles[(int)(point.x), (int)(point.y)].occupyingObject == null)
            {
                worldObject.Initialize(new Vector2((int)point.x, (int)point.y));
                GameManager.Instance.world.tiles[(int)(point.x), (int)(point.y)].occupyingObject = worldObject;
            }
        }
    }
}
