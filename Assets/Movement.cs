using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float movementSpeed = 0.0f;
    public float rotation = 0.0f;

    void Update()
    {
        if(Input.GetKey("w") == true)
        {
            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed * 2.5f;
        }
        if (Input.GetKey("s") == true)
        {
            transform.position += transform.TransformDirection(Vector3.back) * Time.deltaTime * movementSpeed * 2.5f;
        }
        /*if (Input.GetKeyDown("a") == true)
        {
            rotation += 1.0f;
        }
        if (Input.GetKeyDown("d") == true)
        {
            rotation -= 1.0f;
        } */

    }
}
