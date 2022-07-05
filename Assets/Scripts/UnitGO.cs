using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGO : MonoBehaviour
{
    public Unit parent;
    [SerializeField] Vector2 target;
    GameObject go;
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
        List<Tile> path = new Pathfinding().FindPath(parent.occupypingTile, dest);
        foreach(Tile t in path){
            target = t.go.transform.position;
            while ((Vector2)go.transform.position != target) {
                if(dest.occupyingUnit != null){
                    return; // Maybe search for nearby tiles in future?
                }
                await Task.Yield();
            }
            parent.occupypingTile.occupyingUnit = null;
            parent.occupypingTile = t;
            t.occupyingUnit = parent;
        }
    }
}
