# Unity ↔ Arduino Communication Templates

This folder contains multiple examples demonstrating different communication methods between Unity and Arduino Nano ESP32.

## 📋 Overview

All examples follow a consistent pattern:
- **Arduino → Unity**: Sends 'a' or 'b' characters to change cube color (Red/Blue)
- **Unity → Arduino**: Sends 'c' or 'd' commands to control LED (ON/OFF)

![Unity Project Screenshot](Unity.png)

*Unity editor showing the ReadWriteCharsKeyboard scene with the communication scripts and cube GameObject.*

---

## 🔌 Communication Methods

### 1. **Serial Communication** (USB Cable)
**Folder**: `Arduino/SerialReadWriteChars` + `Scripts/ReadWriteCharsSerial.cs`

**How it works**: Direct serial communication via USB cable using `System.IO.Ports`

**Pros**:
- Simple, reliable, no WiFi needed
- Fast and low latency
- Works on Windows natively

**Cons**:
- Requires physical USB connection
- Arduino must be tethered to PC

**Setup**:
1. Upload `SerialReadWriteChars.ino` to Arduino
2. Note the COM port (e.g., COM4)
3. In Unity, set the COM port in Inspector
4. Run Unity scene

**Troubleshooting**:
- Close Arduino Serial Monitor before running Unity
- Unity needs DTR/RTS enabled (already configured in script)
- Check correct COM port in Device Manager

---

### 2. **UDP Communication** (WiFi)
**Folder**: `Arduino/UDPReadWriteChars` + `Scripts/ReadWriteCharsUDP.cs`

**How it works**: Wireless UDP packets over local network

**Pros**:
- Wireless, no cable needed
- Bidirectional, fast
- Good for real-time applications

**Cons**:
- Requires WiFi network
- More complex setup
- Possible firewall issues on Windows

**Setup**:
1. Update WiFi credentials in Arduino sketch
2. Upload to Arduino, note its IP address
3. In Unity Inspector:
   - UDPReceiver: Port 5006, Auto Start ON
   - UDPSender: Arduino's IP, Port 5005, Auto Start ON
4. Run Unity scene

**Troubleshooting**:
- **Arduino not connecting**: Check WiFi credentials, ensure 2.4GHz network
- **Unity not receiving**: 
  - Windows Firewall may block UDP port 5006
  - Check if router has "Client Isolation" disabled
  - Works better on Mac/Linux
- **Both on same network**: Arduino and Unity PC must be on same WiFi

**Known Issues**:
- Windows may have issues receiving UDP from Arduino (works fine on Mac)
- If Unity → Arduino works but not Arduino → Unity, it's likely a Windows firewall issue

---

### 3. **HID Keyboard** (USB HID)
**Folder**: `Arduino/KeyboardReadWriteChars` + `Scripts/ReadWriteCharsKeyboard.cs`

**How it works**: Arduino emulates a USB keyboard device

**Pros**:
- No drivers needed
- Works on any OS
- Arduino sends keypresses directly

**Cons**:
- One-way communication only (Arduino → Unity)
- Arduino appears as keyboard to OS
- Limited data types (only key presses)

**Setup**:
1. Upload `KeyboardReadWriteChars.ino` to Arduino
2. Arduino will appear as a USB keyboard
3. Run Unity scene
4. Arduino automatically sends 'a' and 'b' key presses

**Note**: Unity cannot send data back to Arduino in HID mode

---

### 4. **HTTP Communication** (WiFi)

#### 4a. **Unity as HTTP Client, Arduino as Server**
**Folder**: `Arduino/HTTPServerReadWriteChars` + `Scripts/HTTP/ReadWriteCharsHTTPClient.cs`

**How it works**: Arduino runs a web server, Unity polls it

**Pros**:
- RESTful API pattern
- Easy to debug (can use web browser)
- Stateless, simple

**Cons**:
- Higher latency than UDP
- Requires WiFi
- Polling overhead

**Setup**:
1. Update WiFi credentials in Arduino sketch
2. Upload to Arduino, note its IP
3. In Unity, set Arduino IP in Inspector
4. Run Unity scene

**Arduino Endpoints**:
- `GET /` - Info page
- `GET /data` - Returns 'a' or 'b'
- `GET /command?cmd=c` - LED ON
- `GET /command?cmd=d` - LED OFF

---

#### 4b. **Unity as HTTP Server, Arduino as Client**
**Folder**: `Arduino/HTTPClientReadWriteChars` + `Scripts/HTTP/ReadWriteCharsHTTPServer.cs`

**How it works**: Unity runs HTTP server, Arduino polls it

**Pros**:
- Unity can easily serve data to multiple Arduinos
- Good for one-to-many scenarios
- Arduino stays simple

**Cons**:
- Requires Administrator on Windows (port binding)
- Higher latency
- More complex Unity setup

**Setup**:
1. Run Unity with HTTP server script (default port 8080)
2. Note Unity PC's IP address
3. Update Arduino sketch with Unity's IP
4. Upload to Arduino

**Unity Endpoints**:
- `GET /` - Info page
- `GET /data` - Returns 'a' or 'b'
- `GET /command?cmd=c` - Cube RED
- `GET /command?cmd=d` - Cube BLUE

**Important**: Unity must run as Administrator for HTTP server to work on Windows

---

## 🎯 Which Method Should I Use?

| Use Case | Recommended Method |
|----------|-------------------|
| Simple prototyping | **Serial** |
| Wireless, real-time control | **UDP** |
| Arduino sends input only | **HID Keyboard** |
| RESTful API, debuggable | **HTTP** |
| Multiple Arduinos → Unity | **HTTP Server (Unity)** |
| Production game | **UDP** or **Serial** |

---

## 🔧 General Troubleshooting

### Serial
- Close Arduino Serial Monitor before running Unity
- Check COM port number in Device Manager
- Ensure baud rate matches (9600)

### WiFi (UDP/HTTP)
- Arduino and PC must be on same WiFi network
- Check firewall settings (especially Windows)
- Ensure WiFi is 2.4GHz (ESP32 doesn't support 5GHz)
- Verify IP addresses are on same subnet

### Unity
- Assign Cube GameObject in Inspector
- Check Console for error messages
- Ensure correct ports are configured

---

## 📁 File Structure

```
Assets/
├── Arduino/                          # Arduino sketches
│   ├── SerialReadWriteChars/        # Serial USB example
│   ├── UDPReadWriteChars/           # UDP WiFi example
│   ├── KeyboardReadWriteChars/      # HID Keyboard example
│   ├── HTTPServerReadWriteChars/    # Arduino as HTTP server
│   └── HTTPClientReadWriteChars/    # Arduino as HTTP client
├── Scripts/                          # Unity C# scripts
│   ├── ReadWriteCharsSerial.cs      # Serial communication
│   ├── ReadWriteCharsUDP.cs         # UDP communication
│   ├── ReadWriteCharsKeyboard.cs    # Keyboard input
│   ├── HTTP/                         # HTTP scripts
│   │   ├── HTTPClient.cs            # HTTP client helper
│   │   ├── HTTPServer.cs            # HTTP server helper
│   │   ├── ReadWriteCharsHTTPClient.cs
│   │   └── ReadWriteCharsHTTPServer.cs
│   └── UDP/                          # UDP helper scripts
│       ├── UDPSender.cs
│       └── UDPReceiver.cs
└── Scenes/                           # Unity scenes
    ├── ReadWriteCharsSerial.unity
    ├── ReadWriteCharsUDP.unity
    ├── ReadWriteCharsKeyboard.unity
    ├── ReadWriteCharsHTTPClient.unity
    └── ReadWriteCharsHTTPServer.unity
```

---

## 🚀 Quick Start

1. **Choose a communication method** from above
2. **Upload Arduino sketch** from `Assets/Arduino/[method]/`
3. **Configure Unity script** in Inspector (set IP/port/COM)
4. **Open corresponding scene** in `Assets/Scenes/`
5. **Press Play** in Unity
6. **Test**: Cube should change color, press C/D keys to control LED

---

## 💡 Tips

- Start with **Serial** - it's the simplest
- Use **UDP** for wireless real-time games
- Use **HTTP** when you need debuggable REST API
- Always check Unity Console for connection status
- Use Serial Monitor to debug Arduino (but close it before running Unity for Serial mode)

---

## 📝 License

These are educational templates - feel free to use and modify for your projects!
