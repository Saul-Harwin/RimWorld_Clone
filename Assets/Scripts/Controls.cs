using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraShiftSpeed;
    [SerializeField] float minZoom, maxZoom, zoomSpeed, zoomMoveSpeedEffect;
    World world;
    Camera cam;

    void Start(){
        world = GameManager.Instance.world;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        CameraControls();
        UnitControls();

        // Misc Keybinds 

        // Temporary Keybinds
        if(Input.GetKeyDown(KeyCode.P)){
                List<Tile> _tiles = new Pathfinding().FindPath(world.tiles[0, 0], world.tiles[53, 55]);
                for (int i = 0; i < _tiles.Count; i++)
                {
                   _tiles[i].go.GetComponent<SpriteRenderer>().color = Color.red;
                }
        }
    }

    void CameraControls(){
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
    }

    void UnitControls(){
        if(Input.GetMouseButtonDown(1)) {
            if(GameManager.Instance.unitManager.selectedUnits.Count == 1){
                Unit cUnit = GameManager.Instance.unitManager.selectedUnits[0];
                Vector2Int mp = (Vector2Int)Vector3Int.RoundToInt(cam.ScreenToWorldPoint(Input.mousePosition));
                cUnit.go.GetComponent<UnitGO>().PathToTile(world.tiles[mp.x, mp.y]);
            }
            GameManager.Instance.unitManager.selectedUnits = new List<Unit>();
        }
    }

}