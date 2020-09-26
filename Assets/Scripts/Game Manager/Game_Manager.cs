 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game_Manager : MonoBehaviour
{
    public MasterInput inputController;
    public bool dog_active;
    public Movement_FPS dog_controller;
    public Movement_FPS player_controller;

    private void Awake()
    {
        inputController = new MasterInput();

        inputController.Player.SwitchPlayer.performed += _ => switchPlayer();

    }

    void switchPlayer()
    {
        switch (dog_active)
        {
            case true:
                dog_controller.enabled = true;
                player_controller.enabled = false;
                break;
            case false:
                dog_controller.enabled = false;
                player_controller.enabled = true;
                break;
        }
        dog_active = !dog_active;
    }
}
