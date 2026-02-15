#include <WiFi.h>
#include <WiFiUdp.h>

/*
 * Arduino ESP32 Nano - UDP Simple Receiver
 * 
 * USAGE:
 * - Receives brightness and button state from sender
 * - LED brightness controlled by sender's potentiometer
 * - LED turns on/off based on sender's button state
 * - Upload this first, note the IP, and set it in sender sketch
 */

// WiFi credentials
const char *ssid = "YOUR_WIFI_SSID"; //"TP-Link_5E30";
const char *password = "YOUR_WIFI_PASSWORD"; //"18506839";

// UDP settings
WiFiUDP udp;
const int udpPort = 5000;

// Hardware pins
const int ledPin = 4;        // LED at pin 4

// Variables
int brightness = 0;
bool buttonState = false;

void setup() {
  Serial.begin(9600);
  
  // Setup pins
  pinMode(ledPin, OUTPUT);
  
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi Connected!");
  Serial.print("Receiver IP: ");
  Serial.println(WiFi.localIP());
  Serial.println("Set this IP in the Sender sketch!");
  
  // Start UDP
  udp.begin(udpPort);
  Serial.println("Listening on UDP port " + String(udpPort));
}

void loop() {
  // Check for incoming UDP packets
  int packetSize = udp.parsePacket();
  if (packetSize > 0) {
    // Read the packet
    char incomingPacket[255];
    int len = udp.read(incomingPacket, 255);
    if (len > 0) {
      incomingPacket[len] = 0;  // Null-terminate
      
      // Parse message: "brightness,button"
      String message = String(incomingPacket);
      int commaIndex = message.indexOf(',');
      
      if (commaIndex > 0) {
        brightness = message.substring(0, commaIndex).toInt();
        buttonState = message.substring(commaIndex + 1).toInt() == 1;
        
        // Update LED
        if (buttonState) {
          analogWrite(ledPin, brightness);
        } else {
          digitalWrite(ledPin, LOW);  // Turn off when button not pressed
        }
        
        Serial.println("← Received: Brightness=" + String(brightness) + 
                      ", Button=" + String(buttonState ? "ON" : "OFF"));
      }
    }
  }
  
  delay(10);
}
