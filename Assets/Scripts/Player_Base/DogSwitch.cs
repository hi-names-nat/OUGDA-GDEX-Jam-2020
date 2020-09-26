using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.CurrentDogState)
        {
            case (GameManager.DogState.CARRYING):
                Carrying();
                break;
            case (GameManager.DogState.WALKING):
                Walking();
                break;
        }
    }

    private void Carrying()
    {
        //GameObject.Find("Dog").transform.position = GameObject.Find("Player_Person").transform.position;
    }

    private void Walking()
    {

    }
}
