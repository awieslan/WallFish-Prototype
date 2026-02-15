#include <WiFi.h>
#include <HTTPClient.h>

// WiFi credentials
const char *ssid = "YOUR_WIFI_SSID"; //"TP-Link_5E30";
const char *password = "YOUR_WIFI_PASSWORD"; //"18506839";

// Unity HTTP Server settings
const char *unityIP = "192.168.0.102";  // Unity PC's IP address
const int unityPort = 8080;
String unityURL;

// Hardware
const int ledPin = 4;

// Variables
int charsSent = 0;
int sendTimer = 0;
int sendTime = 100;  // Poll every 1 second

void setup() {
  Serial.begin(115200);
  pinMode(ledPin, OUTPUT);
  
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi Connected!");
  Serial.print("Arduino IP: ");
  Serial.println(WiFi.localIP());
  
  unityURL = "http://" + String(unityIP) + ":" + String(unityPort);
  Serial.println("Unity Server: " + unityURL);
  Serial.println("HTTP Client initialized");
}

void loop() {
  // Poll Unity server for data on timer
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;
    pollUnityData();
  }
  
  // Check for Serial input to send commands (for testing)
  if (Serial.available()) {
    char cmd = Serial.read();
    if (cmd == 'c' || cmd == 'd') {
      sendCommandToUnity(cmd);
    }
  }
  
  delay(10);
}

// Poll Unity for 'a' or 'b' data
void pollUnityData() {
  HTTPClient http;
  
  String url = unityURL + "/data";
  http.begin(url);
  
  Serial.println("========== HTTP REQUEST ==========");
  Serial.println("Method: GET");
  Serial.println("URL: " + url);
  Serial.println("Target: Unity Server");
  
  int httpCode = http.GET();
  
  if (httpCode == HTTP_CODE_OK) {
    String response = http.getString();
    Serial.println("Response: " + response);
    Serial.println("Status: " + String(httpCode) + " OK");
    
    // Control LED and send command back based on response
    if (response == "a") {
      digitalWrite(ledPin, HIGH);
      Serial.println("Action: LED ON");
      sendCommandToUnity('c');  // Unity sent 'a' → send 'c' → Cube RED
    } else if (response == "b") {
      digitalWrite(ledPin, LOW);
      Serial.println("Action: LED OFF");
      sendCommandToUnity('d');  // Unity sent 'b' → send 'd' → Cube BLUE
    }
  } else {
    Serial.println("Error: " + String(httpCode));
    Serial.println("Response: " + http.getString());
  }
  
  Serial.println("==================================");
  http.end();
}

// Send command to Unity
void sendCommandToUnity(char cmd) {
  HTTPClient http;
  
  String url = unityURL + "/command?cmd=" + String(cmd);
  http.begin(url);
  
  Serial.println("========== HTTP REQUEST ==========");
  Serial.println("Method: GET");
  Serial.println("URL: " + url);
  Serial.println("Target: Unity Server");
  Serial.println("Command: " + String(cmd));
  
  int httpCode = http.GET();
  
  if (httpCode == HTTP_CODE_OK) {
    String response = http.getString();
    Serial.println("Response: " + response);
    Serial.println("Status: " + String(httpCode) + " OK");
  } else {
    Serial.println("Error: " + String(httpCode));
  }
  
  Serial.println("==================================");
  http.end();
}
