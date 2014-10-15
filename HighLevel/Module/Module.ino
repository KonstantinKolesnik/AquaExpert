#include <OneWire.h>
//#include "OneWireSlaveManager.h"
#include <EEPROM.h>
#include "Hardware.h"
#include "ControlLine.h"
#include "BusModule.h"
//****************************************************************************************
//uint8_t owPin = 2;
//OneWireSlaveManager* pOwsm = NULL;

BusModule* pModule = NULL;
//DS18B20* pTempSensor = NULL;

//const int ledPin =  13;         // the number of the LED pin
//int ledState = LOW;             // ledState used to set the LED
//long prevMS = 0;        // will store last time LED was updated
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
	// wait for serial port to connect. Needed for Leonardo only
	//while (!Serial) { }

	Serial.begin(9600);
	//digitalWrite(ledPin, ledState);

	pModule = new BusModule(MODULE_TYPE);

	//pTempSensor = new DS18B20(0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
	//pTempSensor->settemp(34.7);
	
	//pOwsm = new OneWireSlaveManager(owPin);
	//pOwsm->elms[0] = pModule;
	//pOwsm->elms[0] = pTempSensor;
	//pOwsm->calck_mask();
}
//****************************************************************************************
void loop()
{
	//pModule->PrintROM();

	pModule->QueryState();
	pModule->PrintState();

	//pOwsm->waitForRequest(false);

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
