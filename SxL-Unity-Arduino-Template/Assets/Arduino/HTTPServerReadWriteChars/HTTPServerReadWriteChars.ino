#include <WiFi.h>
#include <WebServer.h>

// WiFi credentials
const char *ssid = "YOUR_WIFI_SSID"; //"TP-Link_5E30";
const char *password = "YOUR_WIFI_PASSWORD"; //"18506839";

// HTTP Server
WebServer server(80);

// Hardware
const int ledPin = 4;

// Variables
int charsSent = 0;
int sendTimer = 0;
int sendTime = 100;  // Send every 1 second
char currentChar = 'a';

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
  Serial.print("Arduino IP: ");
  Serial.println(WiFi.localIP());
  Serial.println("Use this IP in Unity HTTPReadWriteChars script");
  
  // Setup HTTP server routes
  server.on("/", HTTP_GET, handleRoot);
  server.on("/data", HTTP_GET, handleData);
  server.on("/command", HTTP_GET, handleCommand);  // Changed to GET for Unity compatibility
  server.onNotFound(handleNotFound);
  
  server.begin();
  Serial.println("HTTP server started");
}

void loop() {
  server.handleClient();
  
  // Alternate between 'a' and 'b' on timer
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;
    charsSent += 1;
    currentChar = (charsSent % 2 == 0) ? 'a' : 'b';
    Serial.println("Current char: " + String(currentChar));
  }
  
  delay(10);
}

// Root endpoint
void handleRoot() {
  String html = "<h1>Arduino HTTP Server</h1>";
  html += "<p>GET /data - Get current character (a or b)</p>";
  html += "<p>GET /command?cmd=c - Turn LED ON</p>";
  html += "<p>GET /command?cmd=d - Turn LED OFF</p>";
  server.send(200, "text/html", html);
}

// Data endpoint - Unity polls this to get 'a' or 'b'
void handleData() {
  String response = String(currentChar);
  String clientIP = server.client().remoteIP().toString();
  
  Serial.println("========== HTTP REQUEST ==========");
  Serial.println("Method: GET");
  Serial.println("Endpoint: /data");
  Serial.println("Client IP: " + clientIP);
  Serial.println("Response: " + response);
  Serial.println("Status: 200 OK");
  Serial.println("==================================");
  
  server.send(200, "text/plain", response);
}

// Command endpoint - Unity sends 'c' or 'd'
void handleCommand() {
  String clientIP = server.client().remoteIP().toString();
  
  Serial.println("========== HTTP REQUEST ==========");
  Serial.println("Method: GET");
  Serial.println("Endpoint: /command");
  Serial.println("Client IP: " + clientIP);
  
  if (server.hasArg("cmd")) {
    String cmd = server.arg("cmd");
    Serial.println("Parameter: cmd=" + cmd);
    
    if (cmd == "c") {
      digitalWrite(ledPin, HIGH);
      server.send(200, "text/plain", "LED ON");
      Serial.println("Action: LED turned ON");
      Serial.println("Response: LED ON");
      Serial.println("Status: 200 OK");
    } 
    else if (cmd == "d") {
      digitalWrite(ledPin, LOW);
      server.send(200, "text/plain", "LED OFF");
      Serial.println("Action: LED turned OFF");
      Serial.println("Response: LED OFF");
      Serial.println("Status: 200 OK");
    } 
    else {
      server.send(400, "text/plain", "Invalid command");
      Serial.println("Error: Invalid command");
      Serial.println("Status: 400 Bad Request");
    }
  } else {
    server.send(400, "text/plain", "Missing cmd parameter");
    Serial.println("Error: Missing cmd parameter");
    Serial.println("Status: 400 Bad Request");
  }
  Serial.println("==================================");
}

// 404 handler
void handleNotFound() {
  server.send(404, "text/plain", "Not Found");
}
