#include "USB.h"
#include "USBHIDKeyboard.h"

//Install Board Libraries for "Arduino ESP32 Boards" by Arduino.
//Select Board: Arduino Nano ESP32

USBHIDKeyboard Keyboard;

const int ledPin = 2;  // the pin that the LED is attached to

int charsSent = 0;
int sendTimer = 0;
int sendTime = 100;

void setup() {
  // initialize the USB HID keyboard:
  Keyboard.begin();
  USB.begin();
  // initialize the serial communication:
  Serial.begin(9600);
  // initialize the ledPin as an output:
  pinMode(ledPin, OUTPUT);
}

void loop() {
  //Reading data from USB HID keyboard (Unity cannot send back to HID device):
  // Note: In HID keyboard mode, communication is one-way (Arduino -> Unity)
  // LED control would require Serial or other communication method

  //Sending keyboard keys on a repeating timer:
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;

    //Switch between sending keys 'a' or 'b':
    charsSent += 1;
    if(charsSent % 2 == 0) {
      Keyboard.write('a');
      digitalWrite(ledPin, HIGH);
    } else {
      Keyboard.write('b');
      digitalWrite(ledPin, LOW);
    }
  }

  //Quick delay:
  delay(10);
}
