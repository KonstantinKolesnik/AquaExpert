#include <OneWire.h>
#include <EEPROM.h>
#include "Hardware.h"
#include "qqq.h"
#include "ControlLine.h"
//****************************************************************************************
// current module type !!!:
ModuleType_t moduleType = Test;
//****************************************************************************************
static ControlLine* controlLines = NULL;
static uint16_t controlLinesCount = 0;
//****************************************************************************************
//int RXLED = 17;  // The RX LED has a defined Arduino pin
// The TX LED was not so lucky, we'll need to use pre-defined
// macros (TXLED1, TXLED0) to control that.
// (We could use the same macros for the RX LED too -- RXLED1,
//  and RXLED0.)


void setup()
{
	//pinMode(RXLED, OUTPUT);  // Set RX LED as an output; TX LED is set as an output behind the scenes

	Serial.begin(9600);
	InitModule();
}

void InitModule()
{
	switch (moduleType)
	{
	case Test:
		controlLinesCount = 14;
		//controlLines = (ControlLine*)realloc((void*)controlLines, controlLinesCount * sizeof(ControlLine));
		controlLines = (ControlLine*)malloc(controlLinesCount * sizeof(ControlLine));

		controlLines[0] = ControlLine(Relay, 0, 2);
		controlLines[1] = ControlLine(Relay, 1, 3);
		controlLines[2] = ControlLine(Relay, 2, 4);
		controlLines[3] = ControlLine(Relay, 3, 5);
		controlLines[4] = ControlLine(Relay, 4, 6);
		controlLines[5] = ControlLine(Relay, 5, 7);
		controlLines[6] = ControlLine(Relay, 6, 8);
		controlLines[7] = ControlLine(Relay, 7, 9);
		controlLines[8] = ControlLine(Temperature, 0, 16);
		controlLines[9] = ControlLine(Liquid, 0, 19);
		controlLines[10] = ControlLine(Liquid, 1, 20);
		controlLines[11] = ControlLine(Liquid, 2, 21);
		controlLines[12] = ControlLine(Ph, 0, 18);
		controlLines[13] = ControlLine(ORP, 0, 10);
		break;
	default:
		break;
	}
}

void loop()
{
	Serial.println("UpdateState");
	for (uint16_t i = 0; i < controlLinesCount; i++)
	{
		controlLines[i].UpdateState();
		
		volatile uint8_t* state = controlLines[i].GetState();
		Serial.print("Line #");
		Serial.print(i);
		Serial.print("; state[0] = ");
		Serial.print(state[0]);
		Serial.print("; state[1] = ");
		Serial.print(state[1]);
		Serial.println(";");
	}
	Serial.println("");




	uint8_t st[2] = {1, 0};
	controlLines[0].SetState(st);
	//digitalWrite(RXLED, LOW);   // set the LED on
	//TXLED0; //TX LED is not tied to a normally controlled pin

	delay(1000);


	st[0] = 0;
	controlLines[0].SetState(st);

	//Serial.println(EEPROM.read(0));

	//digitalWrite(RXLED, HIGH);    // set the LED off
	//TXLED1;

	delay(1000);
}
