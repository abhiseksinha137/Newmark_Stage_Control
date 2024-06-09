const String ARDUINO_ID = "trg01"; // The ID of the Arduino
const long baudRate = 9600; // Set the baud rate for serial communication
bool isAuthorized = false; // Variable to check if Arduino is authorized
const int ledPin = LED_BUILTIN; // Pin number for the built-in LED

void setup() {
  Serial.begin(baudRate); // Initialize serial communication
  //Serial.println("Arduino ready for ID check."); // Notify that Arduino is ready
  pinMode(ledPin, OUTPUT); // Set the LED pin as an output
  digitalWrite(ledPin, LOW); // Make sure the LED is off initially
}

void loop() {
  if (Serial.available() > 0) { // Check if data is available on serial port
    String receivedID = Serial.readStringUntil('\n'); // Read the incoming data until newline character

    // Check if the received ID matches the Arduino ID
    if (receivedID == ARDUINO_ID) {
      isAuthorized = true;
      //Serial.println("ID verified. Connection established."); // Send confirmation to the computer
    } else {
      //Serial.println("Invalid ID. Connection denied."); // Send error message to the computer
      isAuthorized=false;
    }
  }

  // Perform other tasks only if Arduino is authorized
  if (isAuthorized) {
    // Your main code here
    // For demonstration, we'll just send a message every second
    //Serial.println("Arduino is authorized and running...");

    blink(5);
    delay(2000); // Delay for 1 second

  }
}

void blink(int blinkNum){
  for (int i=1; i<=blinkNum; i++){
    digitalWrite(ledPin, HIGH); // Turn the LED on
    delay(500); // Wait for 500 milliseconds
    digitalWrite(ledPin, LOW); // Turn the LED off
    delay(500); // Wait for 500 milliseconds
  }
}
