const int LED_PIN = 4;    // Physical D4 (GPIO 7)
const int BUTTON_PIN = 2; // Physical D2 (GPIO 5)

void setup() {
  Serial.begin(9600);
  pinMode(BUTTON_PIN, INPUT_PULLUP); // HIGH when open, LOW when pressed
  pinMode(LED_PIN, OUTPUT);
}

void loop() {
  int sensorVal = digitalRead(BUTTON_PIN);
  Serial.println(sensorVal);

  if (sensorVal == LOW) {      // Button is pressed
    digitalWrite(LED_PIN, HIGH); 
  } else {                     // Button is released
    digitalWrite(LED_PIN, LOW);
  }
}