using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/Object", order = 1)]
public class Object : ScriptableObject {
    private Vector2 position;
    public GameObject gameObject;

    public void Initialize(Vector2 pos, GameObject objectPrefab){
        position = pos;
        ScriptableObject.Instantiate(objectPrefab, pos, Quaternion.identity);
    }
}