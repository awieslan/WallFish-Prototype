import serial
import socket
import time
import select

# Serial port where Arduino is connected
SERIAL_PORT = "COM8"  # Update for your system (e.g., COM3 for Windows)
BAUD_RATE = 9600

# UDP settings
UDP_IP = "127.0.0.1"
PORT_FROM_UNITY = 5005   # Port to listen to Unity's messages
PORT_TO_UNITY = 5006     # Port to send messages to Unity

# Setup serial communication (Arduino to Python)
ser = serial.Serial(SERIAL_PORT, BAUD_RATE)
time.sleep(2)  # Wait for Arduino to initialize

# Create UDP socket to send data to Unity
sock_out = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Create UDP socket to listen for data from Unity
sock_in = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock_in.bind((UDP_IP, PORT_FROM_UNITY))

print(f"Listening for messages from Unity on port {PORT_FROM_UNITY}...")
print(f"Reading data from Arduino on {SERIAL_PORT}...")

while True:
    # Use select to check for data in the UDP socket (non-blocking)
    readable, _, _ = select.select([sock_in], [], [], 0.1)  # 0.1 seconds timeout
    
    for s in readable:
        if s is sock_in:
            data, addr = s.recvfrom(1024)  # Buffer size is 1024 bytes
            message_from_unity = data.decode()
            print(f"Received from Unity: {message_from_unity} from {addr}")

            # Forward Unity message to Arduino via serial
            ser.write(f"{message_from_unity}\n".encode())  # Send message to Arduino

            # Optionally, you can send the same message back to Unity to confirm reception
            sock_out.sendto(f"Received '{message_from_unity}' from Unity".encode(), (UDP_IP, PORT_TO_UNITY))

    # Check if there's data from Arduino on the serial port
    if ser.in_waiting > 0:
        try:
            arduino_data = ser.readline().decode('utf-8', errors='ignore').strip()  # Handle invalid characters
            print(f"Received from Arduino: {arduino_data}")

            # Forward data from Arduino to Unity via UDP
            sock_out.sendto(arduino_data.encode('utf-8'), (UDP_IP, PORT_TO_UNITY))

        except UnicodeDecodeError as e:
            print(f"Error decoding serial data: {e}")
            continue