using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Unity as HTTP Client - Arduino as HTTP Server
/// Polls Arduino for 'a'/'b' data, sends 'c'/'d' commands
/// </summary>
public class ReadWriteCharsHTTPClient : MonoBehaviour
{
    [Header("Arduino HTTP Server")]
    public string arduinoIP = "192.168.0.101";
    public int arduinoPort = 80;
    
    [Header("Unity Objects")]
    public GameObject cube;
    
    [Header("Settings")]
    public float pollInterval = 0.5f;
    
    private HTTPClient client;
    private char receivedChar = ' ';
    
    void Start() {
        client = new HTTPClient(arduinoIP, arduinoPort);
        StartCoroutine(PollArduinoData());
    }
    
    void OnDestroy() {
        client?.Dispose();
    }
    
    void Update() {
        // Change cube color based on received character
        if (receivedChar == 'a') {
            cube.GetComponent<Renderer>().material.color = Color.red;
        } else if (receivedChar == 'b') {
            cube.GetComponent<Renderer>().material.color = Color.blue;
        }
        
        // Send commands when keys pressed
        if (Input.GetKeyDown(KeyCode.C)) {
            StartCoroutine(SendCommand('c'));
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            StartCoroutine(SendCommand('d'));
        }
    }
    
    // Poll Arduino for data
    IEnumerator PollArduinoData() {
        while (true) {
            Task<string> task = client.GetAsync("/data");
            yield return new WaitUntil(() => task.IsCompleted);
            
            if (!task.IsFaulted && task.Result != null) {
                string response = task.Result.Trim();
                if (response.Length > 0) {
                    receivedChar = response[0];
                }
            }
            
            yield return new WaitForSeconds(pollInterval);
        }
    }
    
    // Send command to Arduino
    IEnumerator SendCommand(char cmd) {
        Task<string> task = client.GetAsync($"/command?cmd={cmd}");
        yield return new WaitUntil(() => task.IsCompleted);
    }
}
