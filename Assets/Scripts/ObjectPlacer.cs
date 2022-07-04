using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {
    public float radius = 1.41421356237f;
    int width = 100;
    int height = 100;
    public int numSampleBeforeRejection = 30;

    List<Vector2> points;

    public void PlaceObject() {
        width = GameManager.Instance.world.width;
        height = GameManager.Instance.world.height;
        points = PoissonDiscSampling.Place(width, height, radius, numSampleBeforeRejection);
        Tile[,] tiles = this.GetComponent<World>().tiles;
        foreach (Vector2 point in points) {
            //tiles[(int)(point.x), (int)(point.y)].go.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
}
