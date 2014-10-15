#include <OneWire.h>
#include <EEPROM.h>
#include "Hardware.h"
#include "ControlLine.h"
#include "BusModule.h"
//****************************************************************************************
#define MODULE_TYPE	Unknown
BusModule* pModule = NULL;

//const int ledPin =  13;         // the number of the LED pin
//int ledState = LOW;             // ledState used to set the LED
//long prevMS = 0;					// will store last time LED was updated
//long intervalMS = 250;           // interval at which to blink (milliseconds)
//****************************************************************************************
//void blinkLed()
//{
//	unsigned long currentMS = millis();
// 
//	if (currentMS - prevMS > intervalMS)
//	{
//		// save the last time you blinked the LED 
//		prevMS = currentMS;
//
//		if (ledState == LOW)
//			ledState = HIGH;
//		else
//			ledState = LOW;
//
//		digitalWrite(ledPin, ledState);
//	}
//}
void setup()
{
	//while (!Serial) { }
	Serial.begin(9600);
	//digitalWrite(ledPin, ledState);

	pModule = new BusModule(MODULE_TYPE);
}
//****************************************************************************************
void loop()
{
	pModule->QueryState();
	//pModule->PrintState();


	//blinkLed();


	// for test:

	//ControlLine* pControlLines = pModule->GetControlLines();

	//int16_t st[2] = {1, 0};
	//for (int i = 0; i < 8; i++)
	//{
	//	pControlLines[i].SetState(st);
	//	delay(50);
	//}
	//delay(200);

	//st[0] = 0;
	//for (int i = 0; i < 8; i++)
	//{
	//	pControlLines[i].SetState(st);
	//	delay(50);
	//}
	//delay(200);
}
//****************************************************************************************
