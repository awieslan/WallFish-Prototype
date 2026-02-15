#include <Servo.h>

Servo myservo;  // create Servo object to control a servo

int pos = 0;    // variable to store the servo position

void setup() {
  Serial.begin(9600); // Initialize serial communication at 9600 baud
  myservo.attach(9);  // attaches the servo on pin 9 to the Servo object
  Serial.println("Servo Sweep Started"); // Initial status message
}

void loop() {
  // goes from 0 degrees to 180 degrees
  for (pos = 0; pos <= 180; pos += 1) { 
    myservo.write(pos);              // tell servo to go to position in variable 'pos'
    
    Serial.print("Position: ");      // Label for the value
    Serial.println(pos);             // Print current angle
    
    delay(15);                       // waits 15 ms for the servo to reach the position
  }
  
  // goes from 180 degrees to 0 degrees
  for (pos = 180; pos >= 0; pos -= 1) { 
    myservo.write(pos);              // tell servo to go to position in variable 'pos'
    
    Serial.print("Position: ");      // Label for the value
    Serial.println(pos);             // Print current angle
    
    delay(15);                       // waits 15 ms for the servo to reach the position
  }
}