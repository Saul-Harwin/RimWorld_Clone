using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGO : MonoBehaviour
{
    public Object parent;
    [SerializeField] GameObject harvestMark;

    void OnMouseDown(){
        // parent.markedForHarvest = true;
    }

    void Update(){
        if(parent.markedForHarvest || parent.currentlyBeingHarvested){
            harvestMark.gameObject.SetActive(true);
        }
        else{
            harvestMark.gameObject.SetActive(false);
        }
    }

}
