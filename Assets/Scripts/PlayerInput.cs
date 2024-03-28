using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public string verticalAxisName = "Vertical";       // name of the thruster axis
    public string horizontalAxisName = "Horizontal";    // name of the rudder axis

    [HideInInspector] public float thruster;    // current thruster value
    [HideInInspector] public float rudder;      // current rudder value


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
        thruster = Input.GetAxis(verticalAxisName);
        rudder = Input.GetAxis(horizontalAxisName);
    }
}
