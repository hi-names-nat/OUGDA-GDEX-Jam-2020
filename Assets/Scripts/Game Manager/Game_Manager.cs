using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public bool dog_active;
    public Movement_FPS dog_controller;
    public Movement_FPS player_controller;

    void Update()
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
    }
}
