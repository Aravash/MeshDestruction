using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float speed = 25f;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 change = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            change += Vector3.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            change += Vector3.up;
        }

        transform.rotation = transform.rotation *= Quaternion.Euler(change * speed * Time.deltaTime);
    }
}
