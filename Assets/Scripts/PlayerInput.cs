using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public string verticalAxisName = "Vertical";        // name of the thruster axis
    public string horizontalAxisName = "Horizontal";    // name of the rudder axis

    [HideInInspector] public float thruster;    // current thruster value
    [HideInInspector] public float rudder;      // current rudder value

    public Slider thrustSlider, streeringSlider;

    void Start()
    {
        // Add a listener to the thrust slider and invoke a method when the value changes
        thrustSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        streeringSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {
        // Get thrust value from slider
        Debug.Log(thrustSlider.value);
        thruster = thrustSlider.value;

        // Get rudder value from slider
        Debug.Log(thrustSlider.value);
        rudder = streeringSlider.value;
    }
    // Update is called once per frame
    void Update()
    {
        // if GameManager exists and game is not active...
        if (GameManager.instance != null && !GameManager.instance.IsActiveGame())
        {
            // ...set all inputs to zero (neutral) and return
            thruster = 0f;
            rudder = 0f;
            return;
        }

        // Get the thruster input, rudder, and braking input
        // thruster = Input.GetAxis(verticalAxisName); // Use to test with manual input (w for thrust)
        // rudder = Input.GetAxis(horizontalAxisName); // Use to test with manual input (a + d for steering)
    }
}
