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
aJsonStream serialStream(&Serial);

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
	if (serialStream.available())
		serialStream.skip(); // First, skip any accidental whitespace like newlines.

	if (serialStream.available())
	{
		aJsonObject *msg = aJson.parse(&serialStream);

		if (!msg)
			serialStream.flush(); // we were not able to decode this, let's simply flush the buffer
		else
		{
			gw.processControllerMessage(msg);
			aJson.deleteItem(msg);
		}
	}
}
