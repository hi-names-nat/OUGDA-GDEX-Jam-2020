using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject playerController;
    public GameObject dogController;
    public GameObject playerCamera;
    public GameObject dogCamera;
    public GameObject pauseMenu;

    bool dogActive = false;
    bool dogFollow = true;
    bool pause = false;

    private void Awake()
    {
        DogSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("DogSwitch") && !pause)
        {
            dogActive = !dogActive;
            DogSwitch();
        }

        if (Input.GetButtonDown("DogCommand") && !pause)
        {
            dogFollow = !dogFollow;
            DogFollowSwitch();
        }

        if (Input.GetButtonDown("Pause"))
        {
            pause = !pause;
            PauseMenu();
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

                // Disable Dog's NavMesh
                dogFollow = false;
                DogFollowSwitch();
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

    void DogFollowSwitch()
    {
        dogController.GetComponent<NavMeshAgent>().enabled = dogFollow;
        dogController.GetComponent<DogMovementAI>().enabled = dogFollow;
    }

    void PauseMenu()
    {
        switch (pause)
        {
            case true:
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                break;
            case false:
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
}
