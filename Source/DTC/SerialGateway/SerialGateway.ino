/*
* DESCRIPTION
* The Gateway prints data received from radio network to the serial link.
* The Gateway accepts input from serial which will be sent out to radio network.
*
* The GW code is designed for Arduino Micro/Leonardo with 2 hardware serial ports.
* Serial class refers to USB (CDC) communication; for TTL serial on pins 0 and 1, use the Serial1 class.
*
* Wire connections (OPTIONAL):
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
#include <aJSON.h>

DTCGateway gw(Serial1, 7, 6, 5);
aJsonStream serial_stream(&Serial);

//char inputCommand[MAX_RECEIVE_LENGTH] = ""; // a string to hold incoming commands from serial
//int inputPos = 0;
//bool isCommandComplete = false;

void setup()
{
	gw.begin();
}
void loop()
{
	gw.processRadioMessage();
	receiveFromController();
}

/*
SerialEvent occurs whenever a new data comes in the
hardware serial RX. This routine is run between each
time loop() runs, so using delay inside loop can delay
response. Multiple bytes of data may be available.
*/
void receiveFromController()
{
	if (serial_stream.available())
		serial_stream.skip(); // First, skip any accidental whitespace like newlines.

	if (serial_stream.available())
	{
		aJsonObject *msg = aJson.parse(&serial_stream);

		if (!msg)
			serial_stream.flush(); // We were not able to decode this, let's simply flush the buffer
		else
		{
			gw.processControllerMessage(msg);
			aJson.deleteItem(msg);
		}
	}


	//----------------------------

	//while (Serial.available())
	//{
	//	// get the new byte:
	//	char inChar = (char)Serial.read();

	//	// if the incoming character is a newline, set a flag so the main loop can do something about it:
	//	if (inputPos < MAX_RECEIVE_LENGTH - 1 && !isCommandComplete)
	//	{
	//		if (inChar == '\n')
	//		{
	//			inputCommand[inputPos] = 0;
	//			inputPos = 0;
	//			isCommandComplete = true;
	//		}
	//		else
	//		{
	//			// add it to the inputString:
	//			inputCommand[inputPos++] = inChar;
	//		}
	//	}
	//	else
	//	{
	//		// Incoming message too long. Throw away 
	//		inputPos = 0;
	//	}
	//}

	//if (isCommandComplete)
	//{
	//	gw.processControllerMessage(inputCommand);
	//	isCommandComplete = false;
	//}
}
