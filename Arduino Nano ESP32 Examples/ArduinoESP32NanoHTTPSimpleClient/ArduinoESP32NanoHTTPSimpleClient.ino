#include <WiFi.h>
#include <HTTPClient.h>

/*
 * Arduino ESP32 Nano - HTTP Simple Client
 * 
 * USAGE:
 * - Polls server Arduino for brightness and button data
 * - Controls LED brightness based on received potentiometer value
 * - Turns LED on/off based on received button state
 */

// WiFi credentials
const char *ssid = "TP-Link_5E30"; //"YOUR_WIFI_SSID";
const char *password = "18506839"; //"YOUR_WIFI_PASSWORD";

// Server settings
const char *serverIP = "192.168.0.103";  // Server Arduino's IP
const int serverPort = 80;
String serverURL;

// Hardware pins
const int potPin = A0;
const int buttonPin = 2;
const int ledPin = 4;

// Variables
int brightness = 0;
bool buttonState = false;
int sendTimer = 0;
int sendTime = 10;  // Send every 100ms

void setup() {
  Serial.begin(9600);
  
  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(ledPin, OUTPUT);
  
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi Connected!");
  Serial.print("Client IP: ");
  Serial.println(WiFi.localIP());
  
  serverURL = "http://" + String(serverIP) + ":" + String(serverPort);
  Serial.println("Server: " + serverURL);
}

void loop() {
  // Read potentiometer and map to PWM range
  int potValue = analogRead(potPin);
  brightness = map(potValue, 0, 4095, 0, 255);
  
  // Read button state
  buttonState = !digitalRead(buttonPin);
  
  // Local LED shows button state
  digitalWrite(ledPin, buttonState ? HIGH : LOW);
  
  // Send data to server on timer
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;
    sendDataToServer();
  }
  
  delay(10);
}

void sendDataToServer() {
  HTTPClient http;
  
  // Create URL with parameters
  String url = serverURL + "/control?brightness=" + String(brightness) + "&button=" + String(buttonState ? 1 : 0);
  http.begin(url);
  
  int httpCode = http.GET();
  
  if (httpCode == HTTP_CODE_OK) {
    Serial.println("→ Sent: Brightness=" + String(brightness) + 
                  ", Button=" + String(buttonState ? "ON" : "OFF"));
  } else {
    Serial.println("Error: " + String(httpCode));
  }
  
  http.end();
}
