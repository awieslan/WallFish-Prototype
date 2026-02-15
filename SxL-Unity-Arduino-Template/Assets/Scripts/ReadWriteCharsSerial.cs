using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class ReadWriteCharsSerial : MonoBehaviour
{
    [Header("Serial Port Info")]
    public string portName = "/dev/tty.usbserial-1420"; // Change to "COM3" for Windows
    public int baudRate = 9600;
    private SerialPort serialPort; //Use this for System.IO.Ports (free, but Windows only!)
    private char receivedChar = ' ';

    private Thread serialThread;
    private bool isRunning = true;

    [Header("Unity Objects")]
    public GameObject cube; 

    void Start() {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.DtrEnable = true;  // Enable Data Terminal Ready
        serialPort.RtsEnable = true;  // Enable Request to Send
        serialPort.ReadTimeout = 50;  // Set read timeout
        serialPort.Open();

        // Start a background thread to read serial data
        serialThread = new Thread(ReadSerial);
        serialThread.Start();
    }

    void Update() {
        // Change cube color based on received serial character 'a' or 'b':
        if (receivedChar == 'a') {
            cube.GetComponent<Renderer>().material.color = Color.red;
        } else if (receivedChar == 'b') {
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
        // If the serial port is open, send the character:
        if (serialPort.IsOpen) {
            serialPort.Write(""+sentChar);
            Debug.Log("Sent character: " + sentChar);
        }
    }

    private void ReadSerial() { //Runs on an infinite loop in another thread. Do not call manually elsewhere!
        while (isRunning) {
            if (serialPort.IsOpen && serialPort.BytesToRead > 0) {
                receivedChar = (char)serialPort.ReadByte();
                Debug.Log("Received character: " + receivedChar);
            }
            Thread.Sleep(10); // Prevent CPU overload with small 10ms delay.
        }
    }

    void OnApplicationQuit() { //Make sure to kill thread to prevent runaway zombie processes.
        isRunning = false;
        serialThread.Join();
        serialPort.Close();
    }
}
