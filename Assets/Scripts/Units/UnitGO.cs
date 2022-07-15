using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGO : MonoBehaviour
{
    public Unit parent;
    public Vector2 target;
    GameObject go;
    List<Tile> path;
    public bool currentlyPathing;
    public GameObject highlight;
    public GameObject haulingSprite;
    public bool cancelPathing = false;
    void OnMouseDown(){
        if(!GameManager.Instance.unitManager.selectedUnits.Contains(parent)){
            GameManager.Instance.unitManager.selectedUnits.Add(parent);
        }
        else{
            GameManager.Instance.unitManager.selectedUnits.Remove(parent);
        }
    }

    void Start(){
        target = parent.position;
        go = parent.go;
    }

    void Update(){
        float step = parent.stats.moveSpeed * Time.deltaTime;
        go.transform.position = Vector2.MoveTowards(go.transform.position, target, step);
        UpdateHaulingObject();
    }

    void UpdateHaulingObject(){
        if(parent.haulingObject != null){
            haulingSprite.GetComponent<SpriteRenderer>().sprite = parent.haulingObject.sprites[0];
            haulingSprite.GetComponent<SpriteRenderer>().enabled = true;
        }
        else{
            haulingSprite.GetComponent<SpriteRenderer>().enabled = false;
        }

    }

    public async void PathToTile(Tile dest){
        while(currentlyPathing) await Task.Yield();
        path = GameManager.Instance.pathfinder.FindPath(parent.occupyingTile, dest);
        if(path == null) { parent.FreeUnitFromJob(); return; }
        for (int i = 0; i < path.Count; i++)
        {
            target = path[i].go.transform.position;
            while ((Vector2)go.transform.position != target){
                if(cancelPathing){
                    cancelPathing = false;
                    parent.occupyingTile.occupyingUnit = null;
                    parent.occupyingTile = path[i];
                    path[i].occupyingUnit = parent;
                    parent.position = parent.occupyingTile.position;
                    parent.currentJob = null;
                    parent.state = UnitState.IDLE;
                    target = parent.occupyingTile.position;
                    currentlyPathing = false;
                    return;
                } 
                parent.occupyingTile.occupyingUnit = null;
                parent.occupyingTile = path[i];
                path[i].occupyingUnit = parent;
                parent.position = parent.occupyingTile.position;
                currentlyPathing = true;
                await Task.Yield();
            }
        }
        parent.occupyingTile.occupyingUnit = null;
        parent.occupyingTile = dest;
        dest.occupyingUnit = parent;
        currentlyPathing = false;
    }
}