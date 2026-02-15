using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class ReadWriteCharsHTTP : MonoBehaviour
{
    [Header("Arduino HTTP Server")]
    public string arduinoIP = "192.168.0.101";  // Arduino's IP address
    public int arduinoPort = 80;
    
    [Header("Unity Objects")]
    public GameObject cube;
    
    [Header("Settings")]
    public float pollInterval = 0.5f;  // Poll Arduino every 0.5 seconds
    
    private string arduinoURL;
    private char receivedChar = ' ';
    private HttpClient httpClient;
    
    void Start() {
        arduinoURL = "http://" + arduinoIP + ":" + arduinoPort;
        httpClient = new HttpClient();
        httpClient.Timeout = System.TimeSpan.FromSeconds(5);
        StartCoroutine(PollArduinoData());
    }
    
    void OnDestroy() {
        httpClient?.Dispose();
    }
    
    void Update() {
        // Change cube color based on received character from Arduino
        if (receivedChar == 'a') {
            cube.GetComponent<Renderer>().material.color = Color.red;
        } else if (receivedChar == 'b') {
            cube.GetComponent<Renderer>().material.color = Color.blue;
        }
        
        // Send commands to Arduino when keys pressed
        if (Input.GetKeyDown(KeyCode.C)) {
            StartCoroutine(SendCommand('c'));
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            StartCoroutine(SendCommand('d'));
        }
    }
    
    // Poll Arduino for data (GET /data)
    IEnumerator PollArduinoData() {
        while (true) {
            Task<string> task = GetArduinoDataAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            
            if (!task.IsFaulted && task.Result != null) {
                string response = task.Result.Trim();
                if (response.Length > 0) {
                    receivedChar = response[0];
                    Debug.Log("Received from Arduino: " + receivedChar);
                }
            } else if (task.IsFaulted) {
                Debug.LogWarning("Failed to get data: " + task.Exception?.Message);
            }
            
            yield return new WaitForSeconds(pollInterval);
        }
    }
    
    async Task<string> GetArduinoDataAsync() {
        string url = arduinoURL + "/data";
        try {
            Debug.Log("========== HTTP REQUEST ==========");
            Debug.Log("Method: GET");
            Debug.Log("URL: " + url);
            Debug.Log("Target: Arduino Server");
            
            var startTime = System.DateTime.Now;
            string response = await httpClient.GetStringAsync(url);
            var elapsed = (System.DateTime.Now - startTime).TotalMilliseconds;
            
            Debug.Log("Response: " + response);
            Debug.Log("Status: 200 OK");
            Debug.Log("Time: " + elapsed.ToString("F0") + "ms");
            Debug.Log("==================================");
            
            return response;
        } catch (System.Exception e) {
            Debug.LogError("HTTP GET error: " + e.Message);
            Debug.Log("Status: Error");
            Debug.Log("==================================");
            return null;
        }
    }
    
    // Send command to Arduino (GET /command)
    IEnumerator SendCommand(char cmd) {
        string url = arduinoURL + "/command?cmd=" + cmd;
        
        Task<string> task = SendCommandAsync(url);
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (!task.IsFaulted && task.Result != null) {
            Debug.Log("Sent command: " + cmd + " - Response: " + task.Result);
        } else if (task.IsFaulted) {
            Debug.LogError("Failed to send command: " + task.Exception?.Message);
        }
    }
    
    async Task<string> SendCommandAsync(string url) {
        try {
            Debug.Log("========== HTTP REQUEST ==========");
            Debug.Log("Method: GET");
            Debug.Log("URL: " + url);
            Debug.Log("Target: Arduino Server");
            Debug.Log("Action: Send command");
            
            var startTime = System.DateTime.Now;
            string response = await httpClient.GetStringAsync(url);
            var elapsed = (System.DateTime.Now - startTime).TotalMilliseconds;
            
            Debug.Log("Response: " + response);
            Debug.Log("Status: 200 OK");
            Debug.Log("Time: " + elapsed.ToString("F0") + "ms");
            Debug.Log("==================================");
            
            return response;
        } catch (System.Exception e) {
            Debug.LogError("HTTP command error: " + e.Message);
            Debug.Log("Status: Error");
            Debug.Log("==================================");
            return null;
        }
    }
}
