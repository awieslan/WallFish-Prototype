#include <WiFi.h>
#include <WebServer.h>

/*
 * Arduino ESP32 Nano - HTTP Simple Server
 * 
 * USAGE:
 * - Receives brightness and button data from client via HTTP
 * - Controls LED based on received data
 * - Upload this first, note the IP, set it in client sketch
 */

// WiFi credentials
const char *ssid = "TP-Link_5E30"; //"YOUR_WIFI_SSID";
const char *password = "18506839"; //"YOUR_WIFI_PASSWORD";

// HTTP Server
WebServer server(80);

// Hardware pins
const int ledPin = 4;

// Variables
int brightness = 0;
bool buttonState = false;

void setup() {
  Serial.begin(9600);
  
  pinMode(ledPin, OUTPUT);
  
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi Connected!");
  Serial.print("Server IP: ");
  Serial.println(WiFi.localIP());
  Serial.println("Set this IP in the client sketch!");
  
  // Setup routes
  server.on("/", handleRoot);
  server.on("/control", handleControl);
  
  server.begin();
  Serial.println("HTTP Server started on port 80");
}

void loop() {
  server.handleClient();
  delay(10);
}

void handleRoot() {
  String html = "<h1>Arduino HTTP Server</h1>";
  html += "<p>GET /control?brightness=X&button=X - Control LED</p>";
  server.send(200, "text/html", html);
}

void handleControl() {
  if (server.hasArg("brightness") && server.hasArg("button")) {
    brightness = server.arg("brightness").toInt();
    buttonState = server.arg("button").toInt() == 1;
    
    // Update LED
    if (buttonState) {
      analogWrite(ledPin, brightness);
    } else {
      digitalWrite(ledPin, LOW);
    }
    
    Serial.println("← Received: Brightness=" + String(brightness) + 
                  ", Button=" + String(buttonState ? "ON" : "OFF"));
    
    server.send(200, "text/plain", "OK");
  } else {
    server.send(400, "text/plain", "Missing parameters");
  }
}
