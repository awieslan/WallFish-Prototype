#include "USB.h"
#include "USBHIDKeyboard.h"
#include "USBHIDMouse.h"

const int LED_PIN = 4;    
const int BUTTON_PIN = 2; 

USBHIDKeyboard Keyboard;
USBHIDMouse Mouse;

bool active = false;          
int lastButtonState = HIGH;   
unsigned long lastActionTime = 0; 
bool alternateAB = true;      

// --- ADJUSTED FOR STABILITY ---
float angle = 0;              
const float speed = 0.05;     // Slower "creeping" speed for a smoother look
const float scaleX = 120.0;   // Keeping the wide arc you liked
const float scaleY = 60.0;    

// Error Accumulators (These stop the "creeping down" effect)
float remainderX = 0;
float remainderY = 0;

// Trackers
float currentX = 0;
float currentY = 0;

void setup() {
  Serial.begin(9600); 
  pinMode(LED_PIN, OUTPUT);
  pinMode(BUTTON_PIN, INPUT_PULLUP);
  
  Keyboard.begin();
  Mouse.begin();
  USB.begin(); 
}

void loop() {
  int currentButtonState = digitalRead(BUTTON_PIN);

  if (lastButtonState == HIGH && currentButtonState == LOW) {
    active = !active; 
    digitalWrite(LED_PIN, active ? HIGH : LOW);
    
    // Reset everything on toggle to start fresh in the center
    angle = 0;
    currentX = 0;
    currentY = 0;
    remainderX = 0;
    remainderY = 0;
    
    delay(250); 
  }
  lastButtonState = currentButtonState;

  if (active) {
    // 1. Calculate the math target
    float targetX = sin(angle) * scaleX;
    float targetY = sin(2.0 * angle) * scaleY;
    
    // 2. Calculate raw move needed (including fractions)
    float moveX = (targetX - currentX) + remainderX;
    float moveY = (targetY - currentY) + remainderY;
    
    // 3. Convert to whole pixels (integers)
    int dx = (int)moveX;
    int dy = (int)moveY;
    
    // 4. Store the leftover decimals for the next loop
    remainderX = moveX - dx;
    remainderY = moveY - dy;
    
    // 5. Execute move
    if (dx != 0 || dy != 0) {
      Mouse.move(dx, dy); 
      currentX += dx; 
      currentY += dy;
    }
    
    angle += speed; 
    if (angle > 6.28318) { // 2 * PI
      angle = 0;
      // Force sync at the end of every loop to kill drift
      currentX = 0;
      currentY = 0;
    }

    // Keyboard Logic (Every 3 seconds)
    if (millis() - lastActionTime > 3000) {
      if (alternateAB) Keyboard.print("A");
      else Keyboard.print("B");
      alternateAB = !alternateAB;

      static int wordCounter = 0;
      if (wordCounter % 2 == 0) Keyboard.println(" hello");
      else Keyboard.println(" world");
      wordCounter++;

      lastActionTime = millis(); 
    }
    
    delay(10); 
  }
}