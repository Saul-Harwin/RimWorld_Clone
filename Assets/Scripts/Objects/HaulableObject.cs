using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HaulableObject", menuName = "ScriptableObjects/Objects/HaulableObject", order = 1)]
public class HaulableObject : Object {
    public bool markedForHauling;
    public bool currentlyBeingHauled;
    public override void Initialize(Vector2Int pos, Tile pOccupyingTile){
        position = pos;
        occupyingTile = pOccupyingTile;
        occupyingTile.occupyingObject = this;
        go = Instantiate(objectPrefab, new Vector3(position.x, position.y), Quaternion.identity);
        go.AddComponent<HaulableObjectGO>().parent = this;
        go.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        go.transform.SetParent(GameObject.Find("Objects").transform);
        markedForHauling = true;
        GameManager.Instance.world.objects.Add(this);
    }

    public static void CreateNewObject(HaulableObject objectToInstantiate, Vector2Int pos, Tile pOccupyingTile){
        HaulableObject newObj = ScriptableObject.Instantiate(objectToInstantiate);
        newObj.Initialize(pos, pOccupyingTile);
    }
}