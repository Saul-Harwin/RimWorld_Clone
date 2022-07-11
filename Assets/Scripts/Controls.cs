using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraShiftSpeed;
    [SerializeField] float minZoom, maxZoom, zoomSpeed, zoomMoveSpeedEffect;
    World world;
    Camera cam;
    Vector3 mouseScreenPosition;
    Vector3 mouseWorldPosition;
    Vector3 mousePos1;
    bool dragSelect;
    [Header("UI")]
    [SerializeField] GameObject selectionBox;

    [Header("Misc.")]
    public PlayerState playerState;

    void Start(){
        world = GameManager.Instance.world;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        mouseScreenPosition = Input.mousePosition;
        mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
        TransformSelectionBox();

        CameraControls();
        WorldInteraction();

        // Misc Keybinds 
        if(Input.GetKeyDown(KeyCode.Escape)) ChangeToViewingState();

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

    void WorldInteraction(){
        if(Input.GetMouseButtonDown(0)){
            mousePos1 = mouseScreenPosition;
        }

        if(Input.GetMouseButton(0)){
            if((mousePos1 - mouseScreenPosition).magnitude > 40){
                dragSelect = true;
            }
        }
        
        if(Input.GetMouseButtonUp(0)){
            // SINGLE SELECT
            if(dragSelect == false){
                switch(playerState){
                    case PlayerState.VIEWING:
                        break;
                    case PlayerState.FORESTING:
                        if(GameManager.Instance.world.screenToTilePosition().occupyingObject != null){
                            if(GameManager.Instance.world.screenToTilePosition().occupyingObject.objectType == 0){
                                GameManager.Instance.world.screenToTilePosition().occupyingObject.markedForHarvest = true;
                            }
                        }
                        break;
                    case PlayerState.MINING:
                        if(GameManager.Instance.world.screenToTilePosition().occupyingObject != null){
                            if(GameManager.Instance.world.screenToTilePosition().occupyingObject.objectType == 1){
                                GameManager.Instance.world.screenToTilePosition().occupyingObject.markedForHarvest = true;
                            }
                        }
                        break;
                }
            }
            // DRAG SELECT
            else{
                Vector2 lowerBound = cam.ScreenToWorldPoint(new Vector3(Mathf.Min(mousePos1.x, mouseScreenPosition.x), Mathf.Min(mousePos1.y, mouseScreenPosition.y)));
                Vector2 upperBound = cam.ScreenToWorldPoint(new Vector3(Mathf.Max(mousePos1.x, mouseScreenPosition.x), Mathf.Max(mousePos1.y, mouseScreenPosition.y)));

                if(lowerBound.x < 0) lowerBound.x = 0;
                if(lowerBound.y < 0) lowerBound.y = 0;
                if(upperBound.x > GameManager.Instance.world.width) upperBound.x = GameManager.Instance.world.width;
                if(upperBound.y > GameManager.Instance.world.height) upperBound.y = GameManager.Instance.world.height;

                Debug.Log($"lby {Mathf.RoundToInt(lowerBound.y)}, uby{Mathf.RoundToInt(upperBound.y)}");

                for (int x = Mathf.RoundToInt(lowerBound.x); x < Mathf.RoundToInt(upperBound.x); x++)
                {
                    for (int y = Mathf.RoundToInt(lowerBound.y); y < Mathf.RoundToInt(upperBound.y); y++){
                        switch(playerState){
                            case PlayerState.VIEWING:
                                break;
                            case PlayerState.FORESTING:
                                if(GameManager.Instance.world.tiles[x, y].occupyingObject != null){
                                    if(GameManager.Instance.world.tiles[x, y].occupyingObject.objectType == 0){
                                        GameManager.Instance.world.tiles[x, y].occupyingObject.markedForHarvest = true;
                                    }
                                }
                                break;
                            case PlayerState.MINING:
                                if(GameManager.Instance.world.tiles[x, y].occupyingObject != null){
                                    if(GameManager.Instance.world.tiles[x, y].occupyingObject.objectType == 1){
                                        GameManager.Instance.world.tiles[x, y].occupyingObject.markedForHarvest = true;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            dragSelect = false;
        }
    }

    void TransformSelectionBox(){
        selectionBox.SetActive(dragSelect);
        RectTransform rect = selectionBox.GetComponent<RectTransform>();
        float width = mouseScreenPosition.x - mousePos1.x;
        float height = mouseScreenPosition.y - mousePos1.y;
        rect.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        rect.anchoredPosition = new Vector2(mousePos1.x, mousePos1.y) + new Vector2(width / 2, height / 2);
    }

    public void ChangeToViewingState(){ playerState = PlayerState.VIEWING; }
    public void ChangeToForestingState(){ playerState = PlayerState.FORESTING; }
    public void ChangeToMiningState(){ playerState = PlayerState.MINING; }

}

public enum PlayerState {
    VIEWING,
    FORESTING,
    MINING,
}