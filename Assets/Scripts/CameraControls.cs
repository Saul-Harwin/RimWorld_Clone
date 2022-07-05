using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraShiftSpeed;
    [SerializeField] float minZoom, maxZoom, zoomSpeed, zoomMoveSpeedEffect;
    World world;

    void Start(){
        world = GameManager.Instance.world;
    }

    void Update()
    {
        Camera cam = GetComponent<Camera>();
        float speed = cameraSpeed;
        if(Input.GetKey(KeyCode.LeftShift)) speed = cameraShiftSpeed;
        speed *= cam.orthographicSize * zoomMoveSpeedEffect;
        if(Input.GetKey(KeyCode.W)) transform.Translate(new Vector2(0, speed));
        if(Input.GetKey(KeyCode.A)) transform.Translate(new Vector2(-speed, 0));
        if(Input.GetKey(KeyCode.S)) transform.Translate(new Vector2(0, -speed));
        if(Input.GetKey(KeyCode.D)) transform.Translate(new Vector2(speed, 0));
        cam.orthographicSize -= Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
        if(cam.orthographicSize < minZoom) cam.orthographicSize = minZoom;
        if(cam.orthographicSize > maxZoom) cam.orthographicSize = maxZoom;

        if(Input.GetKeyDown(KeyCode.P)){
                List<Tile> _tiles = new Pathfinding().FindPath(world.tiles[0, 0], world.tiles[99, 99]);
                for (int i = 0; i < _tiles.Count; i++)
                {
                   _tiles[i].go.GetComponent<SpriteRenderer>().color = Color.red;
                }
        }
    }
}