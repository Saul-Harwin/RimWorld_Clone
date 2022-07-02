using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraShiftSpeed;

    void Update()
    {
        float speed = cameraSpeed;
        if(Input.GetKey(KeyCode.LeftShift)) speed = cameraShiftSpeed;
        if(Input.GetKey(KeyCode.W)) transform.Translate(new Vector2(0, speed));
        if(Input.GetKey(KeyCode.A)) transform.Translate(new Vector2(-speed, 0));
        if(Input.GetKey(KeyCode.S)) transform.Translate(new Vector2(0, -speed));
        if(Input.GetKey(KeyCode.D)) transform.Translate(new Vector2(speed, 0));
    }
}
