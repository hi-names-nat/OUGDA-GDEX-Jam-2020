using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRay : MonoBehaviour
{
    public Camera PlayerCamera;
    RaycastHit hit;
    public int ThisLayer;
    
    void Update()
    {
        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && Input.GetButtonDown("Interact") && hit.transform.gameObject.tag == "Interactable" && hit.transform.gameObject.layer == ThisLayer)
        {
            print(ThisLayer);
            print(hit.transform.gameObject.layer);
            GameObject objectHit = hit.transform.gameObject;
            switch (hit.transform.gameObject.name)
            {
                case "Bottle":
                    GlobalVariables.bottles++;
                    break;
                case "Book":
                    GlobalVariables.books++;
                    break;
                case "Bucket":
                    GlobalVariables.buckets++;
                    break;
            }
            Destroy(objectHit);
        }
    }
}
