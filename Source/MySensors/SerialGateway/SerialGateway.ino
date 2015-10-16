/*
* Copyright (C) 2013 Henrik Ekblad <henrik.ekblad@gmail.com>
*
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* version 2 as published by the Free Software Foundation.
*
* DESCRIPTION
* The ArduinoGateway prints data received from sensors on the serial link.
* The gateway accepts input on serial which will be sent out on radio network.
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
//--------------------------------------------------------------------------------------------------------------------------------------------

// !!!!!!!!!!!!!! enable WITH_LEDS_BLINKING in MyConfig.h !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

//#include <MySigningNone.h>
//#include <MyTransportRFM69.h>
//#include <MyTransportNRF24.h>
//#include <MyHwATMega328.h>
//#include <MySigningAtsha204Soft.h>
//#include <MySigningAtsha204.h>
//#include <MyTransport.h>

#include <SPI.h>
#include <MyParserSerial.h>
#include <MySensor.h>
#include <stdarg.h>
//#include <PinChangeInt.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define INCLUSION_MODE_TIME 1	// Number of minutes inclusion mode is enabled
#define INCLUSION_MODE_PIN	3	// Digital pin used for inclusion mode button

#define RADIO_RX_LED_PIN    6  // Receive led pin
#define RADIO_TX_LED_PIN    5  // the PCB, on board LED
#define RADIO_ERROR_LED_PIN 4  // Error led pin

#define MAX_RECEIVE_LENGTH	100 // Max buffersize needed for messages coming from controller
#define MAX_SEND_LENGTH		120 // Max buffersize needed for messages destined for controller
//--------------------------------------------------------------------------------------------------------------------------------------------
MyTransportNRF24 transport(RF24_CE_PIN, RF24_CS_PIN, RF24_PA_LEVEL_GW); // NRFRF24L01 radio driver (set low transmit power by default)
MyHwATMega328 hw; // Hardware profile

// Message signing driver (signer needed if MY_SIGNING_FEATURE is turned on in MyConfig.h)
//MySigningNone signer;
//MySigningAtsha204Soft signer;
//MySigningAtsha204 signer;

// Construct MySensors library (signer needed if MY_SIGNING_FEATURE is turned on in MyConfig.h)
// To use LEDs blinking, uncomment WITH_LEDS_BLINKING in MyConfig.h
#ifdef WITH_LEDS_BLINKING
MySensor gw(transport, hw /*, signer*/, RADIO_RX_LED_PIN, RADIO_TX_LED_PIN, RADIO_ERROR_LED_PIN);
//uint8_t ledInitCycleCount = 3;
#else
MySensor gw(transport, hw /*, signer*/);
#endif
//--------------------------------------------------------------------------------------------------------------------------------------------
MyParserSerial parser;
//--------------------------------------------------------------------------------------------------------------------------------------------
volatile bool buttonTriggeredInclusion = false;
bool inclusionMode = 0; // Keeps track on inclusion mode
unsigned long inclusionStartTime;
//--------------------------------------------------------------------------------------------------------------------------------------------
char convBuf[MAX_PAYLOAD * 2 + 1];
char serialBuffer[MAX_SEND_LENGTH]; // Buffer for building string when sending data to controller
void serial(const char *fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	vsnprintf_P(serialBuffer, MAX_SEND_LENGTH, fmt, args);
	va_end(args);
	Serial.print(serialBuffer);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	gw.begin(onMessageReceived, 0, true, 0);

	//pinMode(INCLUSION_MODE_PIN, INPUT);
	//digitalWrite(INCLUSION_MODE_PIN, HIGH);
	//PCintPort::attachInterrupt(INCLUSION_MODE_PIN, startInclusionInterrupt, RISING); // Add interrupt for inclusion button to pin

	// Send startup log message on serial
	serial(PSTR("0;0;%d;0;%d;Gateway startup complete.\n"), C_INTERNAL, I_GATEWAY_READY);

//#ifdef WITH_LEDS_BLINKING
//		gw.rxBlink(ledInitCycleCount);
//		gw.txBlink(ledInitCycleCount);
//		gw.errBlink(ledInitCycleCount);
//
//		gw.wait(1000);
//#endif
}
void loop()
{
	gw.process();
	readSerialEvent();
	//checkButtonTriggeredInclusion();
	//checkInclusionFinished();
}
//--------------------------------------------------------------------------------------------------------------------------------------------
/*
SerialEvent occurs whenever a new data comes in the
hardware serial RX. This routine is run between each
time loop() runs, so using delay inside loop can delay
response. Multiple bytes of data may be available.
*/
void readSerialEvent()
{
	static int inputPos = 0;
	static bool commandComplete = false;
	static char inputCommand[MAX_RECEIVE_LENGTH] = ""; // a string to hold incoming commands from serial/ethernet interface

	while (Serial.available())
	{
		// get the new byte:
		char inChar = (char)Serial.read();

		// if the incoming character is a newline, set a flag so the main loop can do something about it:
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

	if (commandComplete)
	{
		// A command was issued from serial interface; send it to the actuator
		commandComplete = false;
		parseAndSend(inputCommand);
	}
}
void parseAndSend(char *commandBuffer)
{
	boolean ok;
	MyMessage &msg = gw.getLastMessage();

	if (parser.parse(msg, commandBuffer))
	{
		uint8_t command = mGetCommand(msg);

		if (msg.destination == GATEWAY_ADDRESS && command == C_INTERNAL) // message to gateway
		{
			// Handle messages directed to gateway
			if (msg.type == I_VERSION)
			{
				// Request for version
				serial(PSTR("0;0;%d;0;%d;%s\n"), C_INTERNAL, I_VERSION, LIBRARY_VERSION);
			}
			else if (msg.type == I_INCLUSION_MODE)
			{
				// Request to change inclusion mode
				setInclusionMode(atoi(msg.data) == 1);
			}
		}
		else // message to node
		{
#ifdef WITH_LEDS_BLINKING
			gw.txBlink(1);
#endif
			ok = gw.sendRoute(msg);
			if (!ok)
			{
#ifdef WITH_LEDS_BLINKING
				gw.errBlink(1);
#endif
			}
		}
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	//if (mGetCommand(message) == C_PRESENTATION && inclusionMode)
	//	gw.rxBlink(3);
	//else
	//	gw.rxBlink(1);

	// Pass along the message from sensors to serial line
	serial(PSTR("%d;%d;%d;%d;%d;%s\n"), message.sender, message.sensor, mGetCommand(message), mGetAck(message), message.type, message.getString(convBuf));
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void startInclusionInterrupt()
{
	buttonTriggeredInclusion = true;
}
void checkButtonTriggeredInclusion()
{
	if (buttonTriggeredInclusion)
	{
		// Ok, someone pressed the inclusion button on the gateway, start inclusion mode for 1 munute.
#ifdef DEBUG
		serial(PSTR("0;0;%d;0;%d;Inclusion started by button.\n"), C_INTERNAL, I_LOG_MESSAGE);
#endif
		buttonTriggeredInclusion = false;
		setInclusionMode(true);
	}
}
void checkInclusionFinished()
{
	if (inclusionMode && millis() - inclusionStartTime > 60000UL * INCLUSION_MODE_TIME)
	{
		// inclusionTimeInMinutes minute(s) has passed.. stop inclusion mode
		setInclusionMode(false);
	}
}
void setInclusionMode(bool newMode)
{
	if (newMode != inclusionMode)
	{
		inclusionMode = newMode;

		// Send back mode change on serial line to ack command
		serial(PSTR("0;0;%d;0;%d;%d\n"), C_INTERNAL, I_INCLUSION_MODE, inclusionMode ? 1 : 0);

		if (inclusionMode)
			inclusionStartTime = millis();
	}
}
