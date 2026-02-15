const int ledPin = 4;  // the pin that the LED is attached to
char letter;

int charsSent = 0;
int sendTimer = 0;
int sendTime = 100;

void setup() {
  // initialize the serial communication:
  Serial.begin(9600);
  // initialize the ledPin as an output:
  pinMode(ledPin, OUTPUT);
}

void loop() {
  //Reading data from USB serial:
  while(Serial.available()) {
    // read the most recent char or byte (which will be from 0 to 255):
    letter = Serial.read();
    
    // turn LED on or off based on message received, 'A' for on, 'B' for off:
    if(letter == 'c') {
      digitalWrite(ledPin, HIGH);
    }
    else if(letter == 'd') {
      digitalWrite(ledPin, LOW);
    }
  }

  //Sending data from USB serial:
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;

    //Switch between sending chars 'a' or 'b':
    charsSent += 1;
    if(charsSent % 2 == 0) {
      Serial.println('a');
    } else {
      Serial.println('b');
    }
  }

  //Quick delay:
  delay(10);
}
