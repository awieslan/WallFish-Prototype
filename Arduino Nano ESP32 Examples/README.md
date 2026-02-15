# Arduino Nano ESP32 Examples

Comprehensive collection of Arduino Nano ESP32 examples - from basic GPIO to wireless communication.

## 📋 Overview

This folder contains **10 complete examples** covering everything from beginner-friendly LED blink to advanced wireless communication between multiple Arduino boards. Perfect for learning Arduino Nano ESP32 capabilities step-by-step.

## 📊 Quick Reference

| # | Example | Category | Difficulty | Hardware Needed |
|---|---------|----------|------------|-----------------|
| 1 | Blink LED | Basic | ⭐ Beginner | LED |
| 2 | Button Controls LED | Basic | ⭐ Beginner | Button, LED |
| 3 | Button Toggles LED | Basic | ⭐⭐ Beginner+ | Button, LED |
| 4 | HID Keyboard + Mouse | HID | ⭐⭐⭐ Intermediate | Button, LED |
| 5 | Servo Sweep | Servo | ⭐⭐ Beginner+ | Servo motor |
| 6 | Servo Potentiometer | Servo | ⭐⭐ Beginner+ | Servo, Potentiometer |
| 7 | UDP Send/Receive | WiFi | ⭐⭐⭐ Intermediate | 2 Arduinos, WiFi |
| 8 | HTTP Client/Server | WiFi | ⭐⭐⭐ Intermediate | 2 Arduinos, WiFi |

---

## 📚 Example Categories

### 🔰 **Basic Examples** - Getting Started
Simple examples to learn GPIO basics

### ⌨️ **HID Examples** - USB Input Devices  
Arduino as keyboard/mouse using USB HID

### 🎛️ **Servo Examples** - Motor Control
Control servo motors with different inputs

### 🌐 **Communication Examples** - Wireless Networking
Arduino-to-Arduino communication via WiFi

---

## 🔰 Basic Examples

Perfect for beginners learning Arduino basics.

### 1. **Blink LED**
**Folder**: `BlinkLED`

**What it does**: Classic Arduino example - blinks an LED on and off every second.

**Hardware**:
- LED at pin 4

**Learn**: Basic `digitalWrite()`, `delay()`, and Serial Monitor

---

### 2. **Button Controls LED**
**Folder**: `DigitalInputPullupButtonLED`

**What it does**: Press button to turn LED on, release to turn it off (direct control).

**Hardware**:
- Button at pin 2 (internal pullup)
- LED at pin 4

**Learn**: `digitalRead()`, `INPUT_PULLUP`, button debouncing basics

---

### 3. **Button Toggles LED**
**Folder**: `DigitalInputPullupButtonToggleLED`

**What it does**: Each button press toggles LED state (on ↔ off), like a light switch.

**Hardware**:
- Button at pin 2 (internal pullup)
- LED at pin 4

**Learn**: State management, toggle logic, proper debouncing with `delay(150)`

---

## ⌨️ HID Examples

Turn your Arduino into a USB input device (keyboard/mouse).

### 4. **HID Keyboard + Mouse with Button Toggle**
**Folder**: `HIDKeyboardMouseDigitalInputPullupButtonToggleLED`

**What it does**: 
- Press button to activate/deactivate HID mode
- **Mouse**: Moves in a smooth figure-8 pattern (Lissajous curve)
- **Keyboard**: Types "A hello" and "B world" alternating every 3 seconds
- LED shows active state

**Hardware**:
- Button at pin 2
- LED at pin 4

**Learn**: 
- `USBHIDKeyboard` and `USBHIDMouse` libraries
- Smooth mouse movement with error accumulation
- Trigonometric motion (sin waves)
- Multi-function toggle system

**Note**: Arduino will appear as both keyboard and mouse to your computer!

---

## 🎛️ Servo Examples

Control servo motors with sweep patterns or analog input.

### 5. **Servo Sweep**
**Folder**: `ServoSweep`

**What it does**: Continuously sweeps servo from 0° to 180° and back.

**Hardware**:
- Servo motor at pin 9
- Servo power: 5V (external power recommended)

**Learn**: `Servo.h` library, `attach()`, `write()`, basic servo control

---

### 6. **Servo Potentiometer Knob**
**Folder**: `ServoPotentiometerKnob`

**What it does**: Potentiometer directly controls servo position (0-180°).

**Hardware**:
- Potentiometer at A0
- Servo motor at pin 9

**Learn**: `analogRead()`, `map()` function, real-time servo control

---

## 🌐 Communication Examples

Wireless Arduino-to-Arduino communication via WiFi.

### 7. **UDP Simple Send/Receive**

**Folders**: `ArduinoESP32NanoUDPSimpleSend` + `ArduinoESP32NanoUDPSimpleReceive`

**How it works**: One Arduino sends sensor data via UDP, the other receives and controls an LED.

#### **Sender** (ArduinoESP32NanoUDPSimpleSend)
- **Hardware**:
  - Potentiometer at A0 (controls brightness 0-255)
  - Button at pin 2 (on/off command)
  - LED at pin 4 (local feedback)
- **Does**: Reads sensors → Sends UDP packets every 100ms
- **LED behavior**: Shows button state (ON when pressed)

#### **Receiver** (ArduinoESP32NanoUDPSimpleReceive)
- **Hardware**:
  - LED at pin 4 (controlled remotely)
- **Does**: Receives UDP packets → Controls LED brightness and on/off
- **LED behavior**: 
  - Button ON → LED brightness = potentiometer value
  - Button OFF → LED off

#### **Setup**:
1. Update WiFi credentials in both sketches
2. Upload **Receiver** first → note its IP from Serial Monitor
3. Update **Sender** sketch with Receiver's IP
4. Upload **Sender**
5. Turn potentiometer, press button → Receiver LED responds!

#### **Message Format**: `"brightness,button"` (e.g., `"128,1"`)

---

### 8. **HTTP Simple Client/Server**

**Folders**: `ArduinoESP32NanoHTTPSimpleClient` + `ArduinoESP32NanoHTTPSimpleServer`

**How it works**: One Arduino runs HTTP server, the other polls it as a client. Same hardware setup as UDP example.

#### **Client** (ArduinoESP32NanoHTTPSimpleClient)
- **Hardware**:
  - Potentiometer at A0
  - Button at pin 2
  - LED at pin 4 (local feedback)
- **Does**: Reads sensors → Sends HTTP GET requests every 100ms
- **Endpoint**: `GET /control?brightness=X&button=X`

#### **Server** (ArduinoESP32NanoHTTPSimpleServer)
- **Hardware**:
  - LED at pin 4 (controlled remotely)
- **Does**: Receives HTTP requests → Controls LED
- **Port**: 80 (standard HTTP)
- **Endpoint**: `GET /control?brightness=X&button=X`

#### **Setup**:
1. Update WiFi credentials in both sketches
2. Upload **Server** first → note its IP
3. Update **Client** sketch with Server's IP
4. Upload **Client**
5. Turn potentiometer, press button → Server LED responds!

#### **Debugging**: You can test the server using a web browser:
- Navigate to `http://[SERVER_IP]/control?brightness=128&button=1`

---

## 🔧 Hardware Requirements

### What You'll Need (Depending on Example):

**All Examples**:
- **Arduino Nano ESP32** board
- **USB-C cable** for programming

**Basic Examples (1-3)**:
- LED + 220Ω resistor (or use built-in LED at pin 4)
- Push button (examples 2-3)

**HID Example (4)**:
- Push button
- LED + 220Ω resistor

**Servo Examples (5-6)**:
- Servo motor (e.g., SG90 or similar)
- External 5V power supply (recommended for servo)
- Potentiometer 10kΩ (example 6 only)

**Communication Examples (7-8)**:
- **2x Arduino Nano ESP32** boards
- 10kΩ potentiometer
- Push button
- LED + 220Ω resistor
- WiFi network (2.4GHz)

**Always Useful**:
- Breadboard
- Jumper wires
- Multimeter (for debugging)

### Wiring Examples:

**Basic LED + Button** (Examples 1-3, 4):
```
LED:
  - Anode (+) → Pin 4
  - Cathode (-) → 220Ω resistor → GND

Button (if needed):
  - One side → Pin 2
  - Other side → GND
  (Internal pullup enabled in code)
```

**Servo Motor** (Examples 5-6):
```
Servo:
  - Red wire → 5V (external power recommended)
  - Brown/Black wire → GND
  - Orange/Yellow wire → Pin 9

Potentiometer (example 6):
  - VCC → 3.3V
  - GND → GND
  - Signal → A0
```

**Communication Setup** (Examples 7-8):

**Sender/Client Arduino**:
```
Potentiometer:
  - VCC → 3.3V
  - GND → GND
  - Signal → A0

Button:
  - One side → Pin 2
  - Other side → GND
  (Internal pullup enabled in code)

LED:
  - Anode (+) → Pin 4
  - Cathode (-) → 220Ω resistor → GND
```

**Receiver/Server Arduino**:
```
LED:
  - Anode (+) → Pin 4
  - Cathode (-) → 220Ω resistor → GND
```

### Wiring Diagram:

![Arduino ESP32 Nano Examples Wiring](Arduino%20ESP32%20Nano%20Examples%20Wiring.png)

*Complete wiring diagram showing Sender/Client setup with potentiometer, button, and LED.*

---

## 🚀 Getting Started

### Prerequisites
1. **Arduino IDE** with ESP32 board support:
   - Install "Arduino ESP32 Boards" by Arduino
   - Select Board: **Arduino Nano ESP32**

2. **WiFi Network**:
   - 2.4GHz WiFi (ESP32 doesn't support 5GHz)
   - Know your SSID and password

### Quick Start Guide

1. **Update WiFi Credentials**:
   ```cpp
   const char *ssid = "YOUR_WIFI_SSID";
   const char *password = "YOUR_WIFI_PASSWORD";
   ```

2. **Upload Receiver/Server First**:
   - Upload the sketch
   - Open Serial Monitor (9600 or 115200 baud)
   - Note the IP address printed (e.g., 192.168.0.101)

3. **Configure Sender/Client**:
   - Update the receiver/server IP in the code
   - Upload the sketch

4. **Test**:
   - Turn potentiometer → LED brightness changes
   - Press button → LED turns on/off

---

## 🆚 UDP vs HTTP - Which to Use?

| Feature | UDP | HTTP |
|---------|-----|------|
| **Speed** | Very fast | Slower (request/response overhead) |
| **Latency** | Low (~10-50ms) | Medium (~50-200ms) |
| **Reliability** | Packets can be lost | Guaranteed delivery |
| **Debugging** | Wireshark, packet capture | Web browser, cURL |
| **Simplicity** | More code | Cleaner REST API |
| **Use Case** | Real-time control, games | Status updates, commands |

**Recommendation**: 
- Use **UDP** for real-time control (games, robotics)
- Use **HTTP** for monitoring, status updates, or when debugging matters

---

## 🐛 Troubleshooting

### Arduino Won't Connect to WiFi
- ✅ Check SSID and password spelling
- ✅ Ensure WiFi is 2.4GHz (not 5GHz)
- ✅ Check Serial Monitor for error messages
- ✅ Press reset button to see initialization messages

### Communication Not Working
- ✅ Both Arduinos connected to WiFi (check Serial Monitor)
- ✅ Correct IP addresses configured
- ✅ Same WiFi network
- ✅ Router doesn't have "AP Isolation" enabled
- ✅ Firewall not blocking ports

### LED Not Responding
- ✅ Check wiring (LED polarity, resistor)
- ✅ Check Serial Monitor for received messages
- ✅ Verify sender is actually sending (check logs)
- ✅ Try different LED pin if built-in LED doesn't work

---

## 📊 Serial Monitor Output

You should see clear logs showing communication:

**Sender/Client**:
```
WiFi Connected!
Client IP: 192.168.0.102
→ Sent: Brightness=128, Button=ON
→ Sent: Brightness=200, Button=OFF
```

**Receiver/Server**:
```
WiFi Connected!
Server IP: 192.168.0.101
← Received: Brightness=128, Button=ON
← Received: Brightness=200, Button=OFF
```

---

## 💡 Tips

- **Always upload receiver/server first** to get its IP address
- **Use Serial Monitor** to verify WiFi connection and debug
- **Start simple**: Test with one example before moving to complex projects
- **IP addresses change**: If Arduino gets a new IP, update the sender/client code
- **Power cycle**: Reset both Arduinos if connection is lost

---

## 🔗 Related Examples

See the **SxL-Unity-Arduino-Template** folder for Unity integration examples using the same communication methods!

---

## 📝 License

Educational templates - free to use and modify!
