#include <OneWire.h>
#include <EEPROM.h>
#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include "Hardware.h"
#include "ControlLine.h"
#include "BusModule.h"
//****************************************************************************************
#define MODULE_TYPE	Unknown
//****************************************************************************************
#define PRINT_DEBUG_INFO_RADIO
//#define PRINT_DEBUG_INFO_STATE
//****************************************************************************************
BusModule* pModule = NULL;

//radio address 3...5 bytes; the same for all modules
const uint64_t PROGMEM radioAddress = 0xABCDEFABCDLL;
uint32_t id; // 2^32 = 4294967296 different addresses
uint8_t radioCSPin = 10;
uint8_t requestBuffer[40];
uint8_t responseBuffer[40];
uint8_t payloadSize = 8; // default for static payload
RF24 radio(9, radioCSPin);

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
	Serial.begin(9600);
	//while (!Serial) { }
	//digitalWrite(ledPin, ledState);

	pModule = new BusModule(MODULE_TYPE);
	StartRadio();
}
void loop()
{
	pModule->QueryState();

#ifdef PRINT_DEBUG_INFO_STATE
	pModule->PrintState();
#endif

	PollRadio2();

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
void StartRadio()
{
	radio.begin();
	radio.setRetries(15, 15);
	
	//radio.setPayloadSize(payloadSize); // for static payload size
	radio.enableDynamicPayloads(); // for dynamic payload size

	//radio.setDataRate(RF24_250KBPS);
	radio.setDataRate(RF24_2MBPS);

	radio.openWritingPipe(radioAddress);
	radio.openReadingPipe(1, radioAddress);

	radio.startListening();

#ifdef PRINT_DEBUG_INFO_RADIO
	//radio.printDetails();
#endif
}
void PollRadio()
{
	if (radio.available())
	{
		unsigned long got_time;
		bool done = false;
		while (!done)
		{
			// Fetch the payload, and see if this was the last one.
			done = radio.read(&got_time, sizeof(unsigned long));

#ifdef PRINT_DEBUG_INFO_RADIO
			Serial.print("Got payload ");
			Serial.print(got_time);
#endif

			// Delay just a little bit to let the other unit make the transition to receiver
			//delay(10);
		}

		radio.stopListening();
		bool res = radio.write(&got_time, sizeof(unsigned long));
#ifdef PRINT_DEBUG_INFO_RADIO
		Serial.print(". Send response ... ");
		Serial.println(res ? "OK" : "Failed.");
#endif
		radio.startListening();
	}

	delay(1);
}
void PollRadio2()
{
	if (radio.available())
	{
		bool done = false;

		while (!done)
		{
			payloadSize = radio.getDynamicPayloadSize(); // comment out for static payload
			done = radio.read(requestBuffer, payloadSize);

#ifdef PRINT_DEBUG_INFO_RADIO
			Serial.print("Got payload ");
			Serial.print(requestBuffer[0]);
			Serial.print(requestBuffer[1]);
			Serial.print(requestBuffer[2]);
			Serial.print(requestBuffer[3]);
			Serial.print(requestBuffer[4]);
#endif
			// Delay just a little bit to let the other unit make the transition to receiver
			//delay(10);
		}


		payloadSize = 1; // comment out for static payload
		if (requestBuffer[0] == GetControlLinesCount)
			responseBuffer[0] = pModule->GetControlLinesCount();
		else
			responseBuffer[0] = 128;



		radio.stopListening();
		bool res = radio.write(responseBuffer, payloadSize);
#ifdef PRINT_DEBUG_INFO_RADIO
		Serial.print(". Send response... ");
		Serial.println(res ? "OK" : "Failed.");
#endif
		radio.startListening();
	}

	delay(1);
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
//uint8_t* OneWireSlave::GetROM()
//{
//	return m_rom;
//}
//void OneWireSlave::PrintROM()
//{
//	Serial.print("ROM: ");
//	Serial.print(m_rom[0]);
//	Serial.print(" ");
//	Serial.print(m_rom[1]);
//	Serial.print(" ");
//	Serial.print(m_rom[2]);
//	Serial.print(" ");
//	Serial.print(m_rom[3]);
//	Serial.print(" ");
//	Serial.print(m_rom[4]);
//	Serial.print(" ");
//	Serial.print(m_rom[5]);
//	Serial.print(" ");
//	Serial.print(m_rom[6]);
//	Serial.print(" ");
//	Serial.print(m_rom[7]);
//	Serial.print(" / CRC8 = ");
//	Serial.print(crc8(m_rom, 7));
//	Serial.println("");
//}
