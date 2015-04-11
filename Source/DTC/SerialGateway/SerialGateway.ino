/*
* DESCRIPTION
* The Gateway prints data received from nodes on the serial link.
* The gateway accepts input on seral which will be sent out on radio network.
*
* The GW code is designed for Arduino Nano 328p / 16MHz
*
* Wire connections (OPTIONAL):
* - Inclusion button should be connected between digital pin 3 and GND
* - RX/TX/ERR leds need to be connected between +5V (anode) and digital ping 6/5/4 with resistor 270-330R in a series
*
* LEDs (OPTIONAL):
* - RX (green) - blink fast on radio message recieved. In inclusion mode will blink fast only on presentation recieved
* - TX (yellow) - blink fast on radio message transmitted. In inclusion mode will blink slowly
* - ERR (red) - fast blink on error during transmission error or recieve crc error
*/


#include <DTCGateway.h>
#include <DTCNode.h>
#include <ESP8266.h>
#ifdef ESP8266_USE_SOFTWARE_SERIAL
#include <SoftwareSerial.h>
#endif

#define INCLUSION_MODE_TIME 1	// Number of minutes inclusion mode is enabled
#define INCLUSION_MODE_PIN	4	// Digital pin used for inclusion mode button

#ifdef ESP8266_USE_SOFTWARE_SERIAL
SoftwareSerial mySerial(DEFAULT_TX_PIN, DEFAULT_RX_PIN); // works up to 38400 kbs
DTCGateway gw(mySerial, INCLUSION_MODE_TIME, INCLUSION_MODE_PIN, 7, 6, 5);
#else
DTCGateway gw(Serial1, INCLUSION_MODE_TIME, INCLUSION_MODE_PIN, 7, 6, 5);
#endif

char inputCommand[MAX_RECEIVE_LENGTH] = ""; // a string to hold incoming commands from serial/ethernet interface
int inputPos = 0;
bool isCommandComplete = false;

void setup()
{
	gw.begin();
}
void loop()
{
	gw.processRadioMessage();

	receiveFromController();
	if (isCommandComplete)
	{
		// A command was issued from serial interface; send it to the node
		gw.processSerialMessage(inputCommand);
		
		isCommandComplete = false;
		inputPos = 0;
	}
}

/*
SerialEvent occurs whenever a new data comes in the
hardware serial RX. This routine is run between each
time loop() runs, so using delay inside loop can delay
response. Multiple bytes of data may be available.
*/
void receiveFromController()
{
	while (Serial.available())
	{
		// get the new byte:
		char inChar = (char)Serial.read();

		// if the incoming character is a newline, set a flag
		// so the main loop can do something about it:
		if (inputPos < MAX_RECEIVE_LENGTH - 1 && !isCommandComplete)
		{
			if (inChar == '\n')
			{
				inputCommand[inputPos] = 0;
				inputPos = 0;
				isCommandComplete = true;
			}
			else
			{
				// add it to the inputString:
				inputCommand[inputPos] = inChar;
				inputPos++;
			}
		}
		else
		{
			// Incoming message too long. Throw away 
			inputPos = 0;
		}
	}
}
