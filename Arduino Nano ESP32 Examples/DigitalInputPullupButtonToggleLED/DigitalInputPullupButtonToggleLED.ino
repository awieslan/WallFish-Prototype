const int LED_PIN = 4;    // Physical hole A3 (Arduino Pin 4)
const int BUTTON_PIN = 2; // Physical hole D2 (Arduino Pin 2)

// Variables for toggle logic
bool ledState = LOW;         // Tracks if the LED should be ON or OFF
int lastButtonState = HIGH;  // Tracks the previous reading of the button

void setup() {
  Serial.begin(9600);
  pinMode(BUTTON_PIN, INPUT_PULLUP); // Internal pullup: 1 = Open, 0 = Pressed
  pinMode(LED_PIN, OUTPUT);
  
  Serial.println("--- Nano ESP32: Toggle Mode Active ---");
}

void loop() {
  // Read the current state of the button
  int currentButtonState = digitalRead(BUTTON_PIN);

  // Check if the button was JUST pressed
  // (Meaning: it was HIGH last time, but it is LOW now)
  if (lastButtonState == HIGH && currentButtonState == LOW) {
    
    // Reverse the LED state
    ledState = !ledState;
    digitalWrite(LED_PIN, ledState);
    
    // Print the result to the Serial Monitor
    Serial.print("Button Clicked! LED is now: ");
    Serial.println(ledState ? "ON" : "OFF");
    
    // DEBOUNCE: This 150ms delay is CRITICAL. It 'blinds' the ESP32
    // so it doesn't see the metal button vibrate (bounce)
    delay(150); 
  }

  // Save the current state for the next loop
  lastButtonState = currentButtonState;
}