using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/Object", order = 1)]
public class Object : ScriptableObject {
    public Vector2Int position;
    [SerializeField] private GameObject objectPrefab;
    public GameObject go;
    public int objectType;
    public Tile occupyingTile;
    public Sprite[] sprites;
    public bool markedForHarvest;
    public bool currentlyBeingHarvested;
    public Job associatedJob;
    public ObjectClass objectClass;
    public Object droppedObject;

    public void Initialize(Vector2Int pos, Tile pOccupyingTile, ObjectClass pObjectClass){
        position = pos;
        occupyingTile = pOccupyingTile;
        occupyingTile.occupyingObject = this;
        if(objectType == 0){ // If tree, place at an offset of .6f
            go = Instantiate(objectPrefab, new Vector3(position.x, position.y + 0.6f), Quaternion.identity);
        }
        else{
            go = Instantiate(objectPrefab, new Vector3(position.x, position.y), Quaternion.identity);
        }
        objectClass = pObjectClass;
        if(objectClass == ObjectClass.HARVESTABLE) go.AddComponent<HarvestableObjectGO>().parent = this;
        if(objectClass == ObjectClass.HAULABLE) go.AddComponent<HaulableObjectGO>().parent = this;
        
        go.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        go.transform.SetParent(GameObject.Find("Objects").transform);
        GameManager.Instance.world.objects.Add(this);
    }

    public static void CreateNewObject(Object objectToInstantiate, Vector2Int pos, Tile pOccupyingTile, ObjectClass pObjectClass){
        Object newObj = ScriptableObject.Instantiate(objectToInstantiate);
        newObj.Initialize(pos, pOccupyingTile, pObjectClass);
    }
}

public enum ObjectClass {
    HARVESTABLE,
    HAULABLE,
}