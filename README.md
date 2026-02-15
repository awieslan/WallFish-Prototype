# SxL Arduino Templates

Educational templates for Arduino Nano ESP32 communication - Unity integration and Arduino-to-Arduino examples.

---

## 📦 What's Inside

This repository contains two main sets of examples:

### 🎮 **Unity ↔ Arduino Communication**
**Folder**: `SxL-Unity-Arduino-Template/`

Complete Unity project with multiple communication methods between Unity and Arduino Nano ESP32:
- **Serial** - USB cable communication
- **UDP** - WiFi wireless networking
- **HID Keyboard** - Arduino as USB keyboard device
- **HTTP** - RESTful web API (both Unity as client/server)

**Pattern**: Arduino sends 'a'/'b' to change Unity cube color, Unity sends 'c'/'d' to control Arduino LED.

![Unity Project](SxL-Unity-Arduino-Template/Unity.png)

[📖 Full Unity Documentation →](SxL-Unity-Arduino-Template/README.md)

---

### 🤖 **Arduino Nano ESP32 Standalone Examples**
**Folder**: `Arduino Nano ESP32 Examples/`

Complete collection of **10 Arduino examples** from basics to advanced:

**🔰 Basic (3 examples)**:
- Blink LED - Classic first program
- Button Controls LED - Direct input/output
- Button Toggles LED - State management with debouncing

**⌨️ HID (1 example)**:
- HID Keyboard + Mouse - Arduino as USB input device (figure-8 mouse, typing)

**🎛️ Servo (2 examples)**:
- Servo Sweep - Automatic 0-180° motion
- Servo Potentiometer - Manual servo control

**🌐 Communication (4 examples)**:
- UDP Send/Receive - Fast wireless Arduino-to-Arduino
- HTTP Client/Server - RESTful API between Arduinos

[📖 Full Arduino Documentation →](Arduino%20Nano%20ESP32%20Examples/README.md)

---

## 🚀 Quick Start

### Option 1: Unity Integration
1. Open `SxL-Unity-Arduino-Template/` in Unity
2. Choose a communication method (Serial, UDP, HTTP, Keyboard)
3. Upload corresponding Arduino sketch from `Assets/Arduino/`
4. Configure and play!

### Option 2: Arduino Standalone Examples
1. **Beginners**: Start with `BlinkLED` or `DigitalInputPullupButtonLED`
2. **Intermediate**: Try `ServoSweep` or `HIDKeyboardMouse` 
3. **Advanced**: Wire up 2 Arduinos for `UDP` or `HTTP` communication

---

## 🔌 Communication Methods Comparison

| Method | Use Case | Pros | Cons | Wireless |
|--------|----------|------|------|----------|
| **Serial** | Unity prototyping | Simple, reliable, fast | Requires USB cable | ❌ |
| **UDP** | Real-time gaming | Fast, low latency | Setup complexity, firewall | ✅ |
| **HID Keyboard** | Input-only | No drivers needed | One-way only | ❌ |
| **HTTP** | REST API, monitoring | Debuggable, standard | Higher latency | ✅ |

---

## 🛠️ Hardware Requirements

- **Arduino Nano ESP32** (1 for Unity, 2 for Arduino-to-Arduino)
- **USB-C cable** (for Serial/HID/programming)
- **WiFi network** (2.4GHz for UDP/HTTP)
- **For Arduino standalone examples**:
  - LED + 220Ω resistor (Basic examples)
  - Push button (Basic, HID examples)
  - Potentiometer 10kΩ (Servo, WiFi examples)
  - Servo motor (Servo examples)
  - Breadboard and wires

### Wiring Reference:

![Arduino Wiring Diagram](Arduino%20Nano%20ESP32%20Examples/Arduino%20ESP32%20Nano%20Examples%20Wiring.png)

*See [Arduino Examples folder](Arduino%20Nano%20ESP32%20Examples/) for complete wiring details.*

---

## 📚 Documentation Structure

```
SxL-Arduino-Templates/
├── README.md                           # This file
├── SxL-Unity-Arduino-Template/         # Unity integration examples
│   ├── README.md                       # Unity documentation
│   ├── Assets/
│   │   ├── Arduino/                    # Arduino sketches
│   │   │   ├── SerialReadWriteChars/
│   │   │   ├── UDPReadWriteChars/
│   │   │   ├── KeyboardReadWriteChars/
│   │   │   ├── HTTPServerReadWriteChars/
│   │   │   └── HTTPClientReadWriteChars/
│   │   ├── Scripts/                    # Unity C# scripts
│   │   └── Scenes/                     # Unity scenes
└── Arduino Nano ESP32 Examples/        # Arduino standalone examples (10 total)
    ├── README.md                       # Arduino documentation
    ├── BlinkLED/                       # Basic: LED blink
    ├── DigitalInputPullupButtonLED/    # Basic: Button → LED
    ├── DigitalInputPullupButtonToggleLED/  # Basic: Toggle LED
    ├── HIDKeyboardMouseDigitalInputPullupButtonToggleLED/  # HID device
    ├── ServoSweep/                     # Servo: Auto sweep
    ├── ServoPotentiometerKnob/         # Servo: Manual control
    ├── ArduinoESP32NanoUDPSimpleSend/  # WiFi: UDP sender
    ├── ArduinoESP32NanoUDPSimpleReceive/  # WiFi: UDP receiver
    ├── ArduinoESP32NanoHTTPSimpleClient/  # WiFi: HTTP client
    └── ArduinoESP32NanoHTTPSimpleServer/  # WiFi: HTTP server
```

---

## 🎯 Learning Path

### Beginner (Start Here!)
1. **Arduino Blink LED** - Your first Arduino program
2. **Arduino Button LED** - Learn digital input/output
3. **Unity Serial** - Simplest Unity-Arduino connection

### Intermediate
4. **Arduino Servo Examples** - Control motors
5. **Arduino UDP Send/Receive** - Learn wireless basics
6. **Unity UDP** - Add wireless to Unity
7. **Unity HTTP Client** - REST APIs

### Advanced
8. **Arduino HID Keyboard/Mouse** - Custom USB input devices
9. **Unity HTTP Server** - Unity serves multiple Arduinos
10. **Multi-Arduino Networks** - Build distributed systems

---

## 🐛 Common Issues & Solutions

### Serial
- **Unity not receiving**: Close Arduino Serial Monitor, check COM port
- **Wrong port**: Use Device Manager (Windows) to find correct COM port

### WiFi (UDP/HTTP)
- **Arduino won't connect**: Check WiFi is 2.4GHz, verify credentials
- **Unity not receiving UDP**: Windows firewall may block - works better on Mac
- **Can't find Arduino IP**: Check router's DHCP client list

### Unity
- **Cube not changing color**: Assign cube in Inspector
- **HTTP "Insecure connection"**: Use `System.Net.Http.HttpClient` (already done)

---

## 💡 Tips

- **Start simple**: Begin with Serial or Arduino-to-Arduino UDP
- **Use Serial Monitor**: Essential for debugging WiFi connection
- **Check IP addresses**: WiFi examples require correct IPs
- **Firewall matters**: Windows may block UDP/HTTP ports
- **Read the logs**: Both Unity Console and Arduino Serial Monitor show detailed status

---

## 🔗 Resources

- [Arduino Nano ESP32 Official Docs](https://docs.arduino.cc/hardware/nano-esp32)
- [ESP32 WiFi Library](https://docs.espressif.com/projects/arduino-esp32/en/latest/api/wifi.html)
- [Unity System.IO.Ports](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports)

---

## 📝 License

These are educational templates - free to use and modify for your projects!

---

## 🤝 Contributing

Found a bug or have an improvement? Feel free to submit issues or pull requests!

---

## 📧 Support

- Check README files in each folder for detailed documentation
- Use Serial Monitor and Unity Console for debugging
- Ensure hardware is wired correctly (see wiring diagrams in docs)

---

**Happy Making!** 🚀
