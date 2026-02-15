
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class ReadWriteCharsUDP : MonoBehaviour
{
    [Header("UDP Info")]    
    public UDPSender sender;
    public UDPReceiver receiver;
    
    string receivedString = "";

    [Header("Unity Objects")]
    public GameObject cube; 

    void Awake() {
        receiver.onDataReceived += ReadUDP; 
    }

    void Update() {
        // Change cube color based on received serial character 'a' or 'b':
        if (receivedString == "a") {
            cube.GetComponent<Renderer>().material.color = Color.red;
        } else if (receivedString == "b") {
            cube.GetComponent<Renderer>().material.color = Color.blue;
        }

        // Send characters 'c' and 'd' if respective keys are pressed:
        if (Input.GetKeyDown(KeyCode.C)) {
            SendCharacter('c');
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            SendCharacter('d');
        }
    }

    void SendCharacter(char sentChar) {
        sender.sendMessage(""+sentChar);
        Debug.Log("Sent character: " + sentChar);
    }

    private void ReadUDP(string s) { //Runs on an infinite loop in another thread. Do not call manually elsewhere!
        receivedString = s;
        Debug.Log("Received character: " + receivedString);
    }
}
