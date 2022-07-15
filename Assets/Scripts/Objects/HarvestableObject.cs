using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HarvestableObject", menuName = "ScriptableObjects/Objects/HarvestableObject", order = 1)]
public class HarvestableObject : Object {
    public bool markedForHarvest;
    public bool currentlyBeingHarvested;
    public HaulableObject droppedObject;
    public override void Initialize(Tile pOccupyingTile){
        position = pOccupyingTile.position;
        occupyingTile = pOccupyingTile;
        occupyingTile.occupyingObject = this;
        if(objectType == 0){ // If tree, place at an offset of .6f
            go = Instantiate(objectPrefab, new Vector3(position.x, position.y + 0.6f), Quaternion.identity);
        }
        else{
            go = Instantiate(objectPrefab, new Vector3(position.x, position.y), Quaternion.identity);
        }
        go.AddComponent<HarvestableObjectGO>().parent = this;
        go.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        go.transform.SetParent(GameObject.Find("Objects").transform);
        GameManager.Instance.world.objects.Add(this);
    }

    public static void CreateNewObject(HarvestableObject objectToInstantiate, Vector2Int pos, Tile pOccupyingTile){
        HarvestableObject newObj = ScriptableObject.Instantiate(objectToInstantiate);
        newObj.Initialize(pOccupyingTile);
    }
}