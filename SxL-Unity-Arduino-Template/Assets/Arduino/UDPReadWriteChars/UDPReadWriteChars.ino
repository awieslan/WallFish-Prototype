#include <WiFi.h>
#include <WiFiUdp.h>

//Install Board Libraries for "esp32 by expressif".
//Make board type NodeMCU-32S
//Sample board supplier: https://www.amazon.com/HiLetgo-ESP-WROOM-32-Development-Microcontroller-Integrated/dp/B0718T232Z

// Replace with your network credentials
const char *ssid = "WIFI NETWORK NAME";//"TP-Link_5E30";
const char *password = "WIFI NETWORK PASSWORD";//"18506839";

// Set up UDP
WiFiUDP udp;
const char *udpAddress = "192.168.0.102";  // Mac IP address 
const int udpSenderPort = 5006;  // Port for sending data to Unity
const int udpReceiverPort = 5005;  // Port for receiving data from Unity
String sentMessage;
String receivedMessage;

//Char LED Read/Write Demo Variables
const int ledPin = 4;  // the pin that the LED is attached to

int charsSent = 0;
int sendTimer = 0;
int sendTime = 100;

void setup() {
  // initialize the serial communication:
  Serial.begin(9600);

  // Init UDP
  initUDP();

  // initialize the ledPin as an output:
  pinMode(ledPin, OUTPUT);
}

void loop() {
  // Check WiFi connection status
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("WiFi disconnected! Attempting to reconnect...");
    digitalWrite(ledPin, LOW);
    WiFi.begin(ssid, password);
    delay(5000);
    return;
  }

  // Listen for a message from UDP (UDP -> Arduino ESP32)
  receivedMessage = receiveUDP();

  // Control the LED based on the received message
  if (receivedMessage == "c") {
    digitalWrite(ledPin, HIGH);  // Turn LED on
  } else if (receivedMessage == "d") {
    digitalWrite(ledPin, LOW);  // Turn LED off
  }

  //Sending data via UDP on a repeating timer (only if WiFi connected):
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;

    //Switch between sending chars 'a' or 'b':
    charsSent += 1;
    if(charsSent % 2 == 0) {
      sentMessage = "a";
    } else {
      sentMessage = "b";
    }

    // Send message to UDP
    sendUDP(sentMessage);
  }

  //Quick delay:
  delay(10);
}

void initUDP() {
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.println("Connecting to WiFi...");
  Serial.print("SSID: ");
  Serial.println(ssid);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  
  Serial.println("");
  Serial.println("Connected to WiFi!");
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());
  Serial.print("Arduino IP address: ");
  Serial.println(WiFi.localIP());
  Serial.print("Subnet Mask: ");
  Serial.println(WiFi.subnetMask());
  Serial.print("Gateway IP: ");
  Serial.println(WiFi.gatewayIP());
  Serial.print("DNS IP: ");
  Serial.println(WiFi.dnsIP());
  Serial.print("MAC Address: ");
  Serial.println(WiFi.macAddress());

  // Start listening for UDP on the receive port
  udp.begin(udpReceiverPort);
  Serial.println("UDP Initialized");
}

String receiveUDP() {
  int packetSize = udp.parsePacket();
  if (packetSize > 0) {
    char incomingPacket[255];
    int len = udp.read(incomingPacket, 255);
    if (len > 0) {
      incomingPacket[len] = 0; //Null-terminate string for formatting.
      String message = String(incomingPacket);
      Serial.println("← Received: '" + message + "' from " + udp.remoteIP().toString() + ":" + String(udp.remotePort()) + " (" + String(len) + " bytes)");
      return message;
    } else {
      Serial.println("ERROR: Receive failed - read 0 bytes");
      return "";
    }
  }
  return "";
}

void sendUDP(String message) {
  int beginResult = udp.beginPacket(udpAddress, udpSenderPort);
  if (beginResult == 0) {
    Serial.println("ERROR: Send failed - beginPacket");
    return;
  }
  
  size_t written = udp.write((const uint8_t *)message.c_str(), message.length());
  int endResult = udp.endPacket();
  
  if (endResult == 1) {
    Serial.println("→ Sent: '" + message + "' to " + String(udpAddress) + ":" + String(udpSenderPort) + " (" + String(written) + " bytes)");
  } else {
    Serial.println("ERROR: Send failed - endPacket (result: " + String(endResult) + ")");
  }
}
