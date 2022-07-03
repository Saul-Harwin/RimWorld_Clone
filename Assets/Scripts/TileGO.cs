using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGO : MonoBehaviour
{
    GameObject highlight;
    public Tile parent;
    void Start(){
        highlight = transform.GetChild(0).gameObject;
    }

    void OnMouseEnter(){
        highlight.gameObject.SetActive(true);
    }

    void OnMouseExit(){
        highlight.gameObject.SetActive(false);
    }
}
