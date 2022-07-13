using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGO : MonoBehaviour
{
}

public class HarvestableObjectGO : ObjectGO {
    [SerializeField] SpriteRenderer harvestMark;
    public HarvestableObject parent;
    void Start(){
        harvestMark = GetComponentsInChildren<SpriteRenderer>()[1];
    }
    void Update(){
        if(parent.markedForHarvest || parent.currentlyBeingHarvested){
            harvestMark.enabled = true;
        }
        else{
            harvestMark.enabled = false;
        }
    }
}

public class HaulableObjectGO : ObjectGO {
    public HaulableObject parent;
}