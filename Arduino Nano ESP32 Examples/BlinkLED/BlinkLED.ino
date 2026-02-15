const int LED_PIN = 4; // GPIO 4 (Labeled as '4' or 'G4' on your clone)

void setup() {
  // Initialize serial at 115200 baud (standard for ESP32)
  Serial.begin(9600);
  
  pinMode(LED_PIN, OUTPUT);
  Serial.println("ESP32 Initialized. Starting Blink...");
}

void loop() {
  Serial.println("LED: HIGH (ON)");    // Status message
  digitalWrite(LED_PIN, HIGH); 
  delay(1000); 
  
  Serial.println("LED: LOW (OFF)");    // Status message
  digitalWrite(LED_PIN, LOW); 
  delay(1000); 
}