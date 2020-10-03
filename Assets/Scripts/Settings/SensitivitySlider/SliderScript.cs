using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public void onSliderMoved(float sensitivity)
    {
        GlobalVariables.sensitivity = sensitivity;
        MouseLook.mouseSensitivity = sensitivity;
    }
}
