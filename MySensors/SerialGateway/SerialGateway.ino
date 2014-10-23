/*
* Copyright (C) 2013 Henrik Ekblad <henrik.ekblad@gmail.com>
*
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* version 2 as published by the Free Software Foundation.
*
* DESCRIPTION
* The ArduinoGateway prints data received from sensors on the serial link.
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

#include <SPI.h>  
#include <MySensor.h>  
#include <MyGateway.h>  
#include <stdarg.h>

//#define INCLUSION_MODE_TIME 1 // Number of minutes inclusion mode is enabled
//#define INCLUSION_MODE_PIN 3 // Digital pin used for inclusion mode button

//MyGateway gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN, INCLUSION_MODE_TIME, INCLUSION_MODE_PIN, 6, 5, 4);
MyGateway gw(DEFAULT_CE_PIN, 8);// , INCLUSION_MODE_TIME, INCLUSION_MODE_PIN, 6, 5, 4);

char inputCommand[MAX_RECEIVE_LENGTH] = ""; // a string to hold incoming commands from serial/ethernet interface
int inputPos = 0;
boolean commandComplete = false;

void setup()
{
	gw.begin();
}

void loop()
{
	gw.processRadioMessage();

	if (commandComplete)
	{
		if (inputCommand[0] == 'G' && inputCommand[1] == 'W')
			Serial.print("GWOK\n");
		else
		{
			// A command was issued from serial interface; send it to the actuator
			gw.parseAndSend(inputCommand);
		}

		commandComplete = false;
	}

	// for test only:
	//Serial.print("12;6;1;0;0;36.5");
	//Serial.print('\n');
	//delay(500);
}

/*
SerialEvent occurs whenever a new data comes in the
hardware serial RX. This routine is run between each
time loop() runs, so using delay inside loop can delay
response. Multiple bytes of data may be available.
*/
void onSerialEvent()
{
	while (Serial.available())
	{
		// get the new byte:
		char inChar = (char)Serial.read();

		// if the incoming character is a newline, set a flag
		// so the main loop can do something about it:
		if (inputPos < MAX_RECEIVE_LENGTH - 1 && !commandComplete)
		{
			if (inChar == '\n')
			{
				inputCommand[inputPos] = 0;
				inputPos = 0;
				commandComplete = true;
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
