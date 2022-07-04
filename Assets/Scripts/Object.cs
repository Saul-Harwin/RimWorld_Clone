using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/Object", order = 1)]
public class Object : ScriptableObject {
    private Vector2 position;
    public GameObject gameObject;
    public int objectType;
    public Sprite[] sprites;

    public void Initialize(Vector2 pos, GameObject objectPrefab){
        position = pos;
        if(objectType == 0) position.y += 0.6f;
        gameObject = ScriptableObject.Instantiate(objectPrefab, position, Quaternion.identity);
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}