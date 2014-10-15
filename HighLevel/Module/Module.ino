#include <OneWire.h>
#include <EEPROM.h>
#include "Hardware.h"
#include "ControlLine.h"
#include "BusModule.h"
//****************************************************************************************
#define MODULE_TYPE	Unknown
BusModule* pModule = NULL;

//radio address 3...5 bytes; the same for all modules
const uint64_t PROGMEM radioPhysicalAddress = 0xABCDEFABCDLL;
uint32_t id; // 2^32 = 4294967296 different addresses



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
//OneWireSlave::OneWireSlave(uint8_t family)
//{
//	// read ROM from eeprom:
//	for (int i = 0; i < 8; i++)
//		m_rom[i] = EEPROM.read(i);
//
//	// check ROM:
//	if (m_rom[0] != family || m_rom[7] != crc8(m_rom, 7)) // type mismatch or crc mismatch
//	{
//		int seed = analogRead(A5);
//		randomSeed(seed);
//
//		// generate new ROM:
//		m_rom[0] = family;
//		m_rom[1] = random(0, 255);
//		m_rom[2] = random(0, 255);
//		m_rom[3] = random(0, 255);
//		m_rom[4] = random(0, 255);
//		m_rom[5] = random(0, 255);
//		m_rom[6] = random(0, 255);
//		m_rom[7] = crc8(m_rom, 7);
//
//		// save ROM to eeprom:
//		for (int i = 0; i < 8; i++)
//			EEPROM.write(i, m_rom[i]);
//	}
//}
