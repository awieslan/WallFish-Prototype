#include <WiFi.h>
#include <WiFiUdp.h>

/*
 * Arduino ESP32 Nano - UDP Simple Sender
 * 
 * USAGE:
 * - Turn potentiometer to set brightness value (0-255)
 * - Press button to send brightness to receiver LED
 * - Release button to turn off receiver LED
 * - Sender's local LED shows button state
 */

// WiFi credentials
const char *ssid = "YOUR_WIFI_SSID"; //"TP-Link_5E30";
const char *password = "YOUR_WIFI_PASSWORD"; //"18506839";

// UDP settings
WiFiUDP udp;
const char *receiverIP = "192.168.0.103";  // Receiver Arduino's IP
const int udpPort = 5000;

// Hardware pins
const int potPin = A0;       // Potentiometer at A0
const int buttonPin = 2;     // Button at pin 2
const int ledPin = 4;        // Local feedback LED at pin 4

// Variables
int brightness = 0;
bool buttonState = false;
bool lastButtonState = false;

void setup() {
  Serial.begin(9600);
  
  // Setup pins
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
  Serial.print("Sender IP: ");
  Serial.println(WiFi.localIP());
  
  // Start UDP
  udp.begin(udpPort);
}

void loop() {
  // Read potentiometer (0-4095) and map to PWM range (0-255)
  int potValue = analogRead(potPin);
  brightness = map(potValue, 0, 4095, 0, 255);
  
  // Read button state (active LOW with pullup)
  buttonState = !digitalRead(buttonPin);
  
  // Update local LED for feedback
  analogWrite(ledPin, brightness);
  
  // Send UDP packet if button state changed or every 100ms
  static unsigned long lastSend = 0;
  if (millis() - lastSend >= 100 || buttonState != lastButtonState) {
    lastSend = millis();
    lastButtonState = buttonState;
    
    // Create message: "brightness,button"
    String message = String(brightness) + "," + String(buttonState ? 1 : 0);
    
    // Send UDP
    udp.beginPacket(receiverIP, udpPort);
    udp.print(message);
    udp.endPacket();
    
    Serial.println("→ Sent: " + message);
  }
  
  delay(10);
}
