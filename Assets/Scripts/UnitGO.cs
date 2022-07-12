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
    public GameObject highlight;
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
    }

    public async void PathToTile(Tile dest){
        path = GameManager.Instance.pathfinder.FindPath(parent.occupypingTile, dest);
        if(path == null) { parent.FreeUnitFromJob(); return; }
        target = path[0].go.transform.position;
        for (int i = 0; i < path.Count; i++)
        {
            target = path[i].go.transform.position;
            while ((Vector2)go.transform.position != target) {
                /*
                if(dest.occupyingUnit != null){
                    parent.occupypingTile.occupyingUnit = null;
                    parent.occupypingTile = path[i];
                    path[i].occupyingUnit = parent;
                    parent.position = parent.occupypingTile.position;
                    parent.currentJob = null;
                    parent.state = UnitState.IDLE;
                    Debug.Log("PATH INTERFERED");
                    return; // Maybe search for nearby tiles in future?
                } 
                */
                
                await Task.Yield();
            }
            parent.occupypingTile.occupyingUnit = null;
            parent.occupypingTile = path[i];
            path[i].occupyingUnit = parent;
        }
    }
}
