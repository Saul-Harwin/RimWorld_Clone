using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HaulableObject", menuName = "ScriptableObjects/Objects/HaulableObject", order = 1)]
public class HaulableObject : Object {
    public bool markedForHauling;
    public bool currentlyBeingHauled;
    public Stockpile stockpile;
    public override void Initialize(Tile pOccupyingTile){
        position = pOccupyingTile.position;
        occupyingTile = pOccupyingTile;
        occupyingTile.occupyingObject = this;
        go = Instantiate(objectPrefab, new Vector3(position.x, position.y), Quaternion.identity);
        go.AddComponent<HaulableObjectGO>().parent = this;
        go.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        go.transform.SetParent(GameObject.Find("Objects").transform);
        GameManager.Instance.world.objects.Add(this);
    }

    public static HaulableObject CreateNewObject(HaulableObject objectToInstantiate, Tile pOccupyingTile){
        HaulableObject newObj = ScriptableObject.Instantiate(objectToInstantiate);
        newObj.Initialize(pOccupyingTile);
        return newObj;
    }
}