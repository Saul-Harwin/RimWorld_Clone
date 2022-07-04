using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/Object", order = 1)]
public class Object : ScriptableObject {
    private Vector2 position;
    public GameObject objectPrefab;
    public GameObject go;
    public int objectType;
    public Sprite[] sprites;

    public void Initialize(Vector2 pos){
        position = pos;
        if(objectType == 0) position.y += 0.6f;
        go = Instantiate(objectPrefab, position, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}