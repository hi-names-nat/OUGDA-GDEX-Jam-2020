using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    //public GameObject _Ghost;
    public Canvas dialogue;
    // Start is called before the first frame update
    void Start()
    {
        dialogue.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            dialogue.enabled = true;
            Debug.Log("TextEnabled");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            dialogue.enabled = false;
        }
    }
}
