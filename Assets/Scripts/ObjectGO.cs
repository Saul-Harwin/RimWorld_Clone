using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGO : MonoBehaviour
{
    public Object parent;
}

public class HarvestableObjectGO : ObjectGO {
    [SerializeField] SpriteRenderer harvestMark;
    void Start(){
        harvestMark = GetComponentsInChildren<SpriteRenderer>()[1];
    }
    void Update(){
        if(parent.markedForHarvest || parent.currentlyBeingHarvested){
            //harvestMark.gameObject.SetActive(true);
            harvestMark.enabled = true;
        }
        else{
            //harvestMark.gameObject.SetActive(false);
            harvestMark.enabled = false;
        }
    }
}

public class HaulableObjectGO : ObjectGO {

}