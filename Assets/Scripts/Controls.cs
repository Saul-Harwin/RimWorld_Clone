using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    [SerializeField] Color addToSelectionColor, removeFromSelectionColor;

    [Header("Misc.")]
    public PlayerState playerState;
    List<Tile> selectedTiles;

    void Start(){
        world = GameManager.Instance.world;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        selectedTiles = new List<Tile>();
        mouseScreenPosition = Input.mousePosition;
        mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
        TransformSelectionBox();

        CameraControls();
        WorldInteraction();

        // Misc Keybinds 
        if(Input.GetKeyDown(KeyCode.Escape)) ChangeToViewingState();

        // Temporary Keybinds
        if(Input.GetKeyDown(KeyCode.P)){
                List<Tile> _tiles = GameManager.Instance.pathfinder.FindPath(world.tiles[0, 0], world.tiles[53, 55]);
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
        if(Input.GetMouseButtonDown(0)) mousePos1 = mouseScreenPosition;

        if(Input.GetMouseButton(0)){
            if((mousePos1 - mouseScreenPosition).magnitude > 40) dragSelect = true;
        }
        
        if(Input.GetMouseButtonUp(0)){
            if(dragSelect == false){
                if(GameManager.Instance.world.screenToTilePosition() != null){
                    selectedTiles.Add(GameManager.Instance.world.screenToTilePosition());
                }
            }
            else{
                Vector2 lowerBound = cam.ScreenToWorldPoint(
                    new Vector3(
                        Mathf.Min(mousePos1.x, mouseScreenPosition.x),
                        Mathf.Min(mousePos1.y, mouseScreenPosition.y)
                    )
                );
                Vector2 upperBound = cam.ScreenToWorldPoint(
                    new Vector3(
                        Mathf.Max(mousePos1.x, mouseScreenPosition.x), 
                        Mathf.Max(mousePos1.y, mouseScreenPosition.y)
                    )
                );

                if(lowerBound.x < 0) lowerBound.x = 0;
                if(lowerBound.y < 0) lowerBound.y = 0;
                if(upperBound.x > GameManager.Instance.world.width) upperBound.x = GameManager.Instance.world.width;
                if(upperBound.y > GameManager.Instance.world.height) upperBound.y = GameManager.Instance.world.height;

                for (int x = Mathf.RoundToInt(lowerBound.x); x <= Mathf.RoundToInt(upperBound.x); x++)
                {
                    for (int y = Mathf.RoundToInt(lowerBound.y); y <= Mathf.RoundToInt(upperBound.y); y++){
                        selectedTiles.Add(GameManager.Instance.world.tiles[x, y]);
                    }
                }
            }
            dragSelect = false;
            SelectedTilesAction();
        }
    }

    void SelectedTilesAction(){
        List<Tile> cachedSelectedTiles = selectedTiles;

        // Harvesting
        //-------------------
        if(playerState == PlayerState.FORESTING || playerState == PlayerState.MINING){
            for (int i = 0; i < cachedSelectedTiles.Count; i++)
            {
                if(cachedSelectedTiles[i].occupyingObject != null){
                    switch(playerState){
                        case PlayerState.FORESTING: 
                            var fobj = cachedSelectedTiles[i].occupyingObject as HarvestableObject;
                            if(fobj != null) ForestingAction(cachedSelectedTiles[i].occupyingObject as HarvestableObject);
                            break;
                        case PlayerState.MINING:
                            var mobj = cachedSelectedTiles[i].occupyingObject as HarvestableObject;
                            if(mobj != null) MiningAction(cachedSelectedTiles[i].occupyingObject as HarvestableObject);
                            break;
                    }
                }
            }
        }

        // Stockpile
        //--------------- 
        StockPileAction();
    }

    void ForestingAction(HarvestableObject h){
        if(h.objectType == 0){
            if(h.currentlyBeingHarvested || h.markedForHarvest){
                if(Input.GetKey(KeyCode.LeftShift)){
                    h.associatedJob.CancelJob();
                }
            }
            if(!h.currentlyBeingHarvested){
                if(!Input.GetKey(KeyCode.LeftShift)){
                    h.markedForHarvest = true;
                }
            }
        }
    }

    void MiningAction(HarvestableObject h){
        if(h.objectType == 1){
            if(h.currentlyBeingHarvested || h.markedForHarvest){
                if(Input.GetKey(KeyCode.LeftShift)){
                    h.associatedJob.CancelJob();
                }
            }
            if(!h.currentlyBeingHarvested){
                if(!Input.GetKey(KeyCode.LeftShift)){
                    h.markedForHarvest = true;
                }
            }
        }
    }
    
    void StockPileAction(){
        if(playerState == PlayerState.CREATINGSTOCKPILE){
            if(!Input.GetKey(KeyCode.LeftShift)){
                Stockpile newStockpile = new Stockpile();
                newStockpile.Initialize();
                for (int i = 0; i < selectedTiles.Count; i++)
                {
                    newStockpile.tiles.Add(new StockpileTile(selectedTiles[i], newStockpile));
                }
                newStockpile.CreateOverlayImages();
            }
            else if(Input.GetKey(KeyCode.LeftShift)){
                for (int i = 0; i < GameManager.Instance.world.stockpiles.Count; i++)
                {
                    for (int j = 0; j < selectedTiles.Count; j++)
                    {
                        if(Stockpile.TileExistsInStockPile(selectedTiles[j])){
                            GameObject.Destroy(Stockpile.returnStockpileTile(selectedTiles[j]).highlight);
                            Stockpile.returnStockpileTile(selectedTiles[j]).highlight = null;
                            GameManager.Instance.world.stockpiles[i].tiles.Remove(Stockpile.returnStockpileTile(selectedTiles[j]));
                            if(selectedTiles[j].occupyingObject is HaulableObject){
                                (selectedTiles[j].occupyingObject as HaulableObject).markedForHauling = true;
                            }
                        }
                    }
                }
            }
        }
    }

void TransformSelectionBox(){
    selectionBox.SetActive(dragSelect);
        RectTransform rect = selectionBox.GetComponent<RectTransform>();
        float width = mouseScreenPosition.x - mousePos1.x;
        float height = mouseScreenPosition.y - mousePos1.y;
        rect.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        rect.anchoredPosition = new Vector2(mousePos1.x, mousePos1.y) + new Vector2(width / 2, height / 2);
        if(Input.GetKey(KeyCode.LeftShift)){
            rect.GetComponent<Image>().color = removeFromSelectionColor;
        }
        else {
            rect.GetComponent<Image>().color = addToSelectionColor;
        }
    }

    public void ChangeToViewingState(){ playerState = PlayerState.VIEWING; }
    public void ChangeToForestingState(){ playerState = PlayerState.FORESTING; }
    public void ChangeToMiningState(){ playerState = PlayerState.MINING; }
    public void ChangeToCreatingStockpileState(){ playerState = PlayerState.CREATINGSTOCKPILE; }

}

public enum PlayerState {
    VIEWING,
    FORESTING,
    MINING,
    CREATINGSTOCKPILE,
}