using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UDPReceiver : MonoBehaviour {
 
    public event Action<string> onDataReceived;
 
    private string receiverMessage = null;
    
    [Header("Address")]
    public int receivePort;
    
    [Header("Init settings")]
    public bool autoStart = false;
    public int bufferSize = 32768;

    [Header("Rate settings")]
    public int sleep = 50;
    UdpClient receiver;
    Thread receiveThread;
    bool receivingData = true;

    private float timer = 0;
    public float resetDelay = 10;

    [Header("UI (Optional)")]
    public string prefName = "ReceivePort";
    public InputField PortField;
    public Text ConnectButton;

    public void startReceiveThread() {
        stopReceiverThread();
        if(receivePort > 0) {
            receivingData = true;
            if (receiveThread != null && receiveThread.IsAlive) { receiveThread.Abort(); }
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log("Receiver Started!");
            if(ConnectButton) { ConnectButton.text = "Restart UDP"; }
        }
    }
    public void stopReceiverThread() {
        if (receiver != null) { receiver.Close(); }
        receivingData = false;
    }
    public string getMessage() {
        if (receiverMessage != null) {
            string message = receiverMessage;
            receiverMessage = null;
            return message;
        } else {
            return null;
        }
    }
    public void setPort(string port) { 
        if(!int.TryParse(port, out receivePort)) { receivePort = 0; } 
        PlayerPrefs.SetInt(prefName, receivePort);
    }

    protected virtual void OnDataReceived(string message) { 
        onDataReceived?.Invoke(message);
    }
    private void ReceiveData() {
        receiver = new UdpClient(receivePort);
        receiver.Client.ReceiveBufferSize = bufferSize;
        receiver.Client.ReceiveTimeout = 1000; // 1 second timeout
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        
        Debug.Log("UDP Receiver thread started on port " + receivePort);
        
        while (receivingData) {
            try {
                byte[] data = receiver.Receive(ref anyIP);
                receiverMessage = Encoding.UTF8.GetString(data);
                Debug.Log("✓ Received message: '" + receiverMessage + "' from " + anyIP.Address + ":" + anyIP.Port);
                OnDataReceived(receiverMessage);
            } catch (SocketException e) {
                if (e.SocketErrorCode != SocketError.TimedOut) {
                    Debug.LogError("UDP Receive Error: " + e.Message);
                }
                // Timeout is normal, just continue
            } catch (Exception e) {
                Debug.LogError("UDP Receive Error: " + e.Message);
            }
        }
        
        Debug.Log("UDP Receiver thread stopped");
        receiver.Close();
    }
    
    void Start() {
    	//receivePort = PlayerPrefs.GetInt(prefName, 9000);
        if(PortField) { PortField.text = "" + receivePort; }
        if(autoStart && receivePort > 0) { startReceiveThread(); }
    }
    void Update() {
        if (timer > 0) { timer -= Time.deltaTime; }
        else { timer = resetDelay; }
    }
    void OnApplicationQuit() {
        Debug.Log("Closed application.");
        stopReceiverThread();
    }
}
 