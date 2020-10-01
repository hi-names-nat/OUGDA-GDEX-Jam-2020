using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerController;
    public GameObject dogController;
    public GameObject playerCamera;
    public GameObject dogCamera;

    public bool dogActive = false;

    private void Awake()
    {
        DogSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("DogSwitch"))
        {
            dogActive = !dogActive;
            DogSwitch();
        }
    }

    void DogSwitch()
    {
        switch (dogActive)
        {
            case true:
                // Disable player components
                playerController.GetComponent<Movement>().enabled = false;
                playerCamera.SetActive(false);
                
                // Enable dog components
                dogController.GetComponent<Movement>().enabled = true;
                dogCamera.SetActive(true);
                break;
            case false:
                // Enable player components
                playerController.GetComponent<Movement>().enabled = true;
                playerCamera.SetActive(true);

                // Disable dog components
                dogController.GetComponent<Movement>().enabled = false;
                dogCamera.SetActive(false);
                break;
        }
    }
}
