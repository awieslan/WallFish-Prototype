
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadCharsKeyboard : MonoBehaviour
{
    [Header("Unity Objects")]
    public GameObject cube;

    void Update() {
        // Change cube color based on keyboard input from Arduino HID
        // Arduino acts as a keyboard and sends 'a' or 'b' key presses
        if (Input.GetKeyDown(KeyCode.A)) {
            cube.GetComponent<Renderer>().material.color = Color.red;
            Debug.Log("Received key 'a' from Arduino - Color changed to RED");
        } 
        else if (Input.GetKeyDown(KeyCode.B)) {
            cube.GetComponent<Renderer>().material.color = Color.blue;
            Debug.Log("Received key 'b' from Arduino - Color changed to BLUE");
        }
    }
}
