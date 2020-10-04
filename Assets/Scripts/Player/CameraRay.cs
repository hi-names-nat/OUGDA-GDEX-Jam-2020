using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRay : MonoBehaviour
{
    public Camera PlayerCamera;
    RaycastHit hit;
    public LayerMask ThisLayer;
    
    void Update()
    {
        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, ThisLayer) && Input.GetButtonDown("Interact") && hit.transform.gameObject.tag == "Interactable")
        {
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
            print(GlobalVariables.bottles);
            Destroy(objectHit);
        }
    }
}
