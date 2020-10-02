using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadToScene1 : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene(1);
    }
}
