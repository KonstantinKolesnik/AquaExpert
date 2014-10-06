#include <OneWire.h>
#include <EEPROM.h>
#include "Hardware.h"
#include "ControlLine.h"
#include "NetworkModule.h"
//****************************************************************************************
//int RXLED = 17;  // The RX LED has a defined Arduino pin
// The TX LED was not so lucky, we'll need to use pre-defined macros (TXLED1, TXLED0) to control that.
// (We could use the same macros for the RX LED too -- RXLED1, and RXLED0.)
//****************************************************************************************
void setup()
{
	//pinMode(RXLED, OUTPUT);  // Set RX LED as an output; TX LED is set as an output behind the scenes

	Serial.begin(9600);
	
	// current module type!!!:
	Module.Init(Test);
}
//****************************************************************************************
void loop()
{
	Serial.println("UpdateState");
	
	Module.UpdateState();

	ControlLine* pControlLines = Module.GetControlLines();
	for (uint16_t i = 0; i < Module.GetControlLinesCount(); i++)
	{
		volatile uint8_t* state = pControlLines[i].GetState();
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
	pControlLines[0].SetState(st);
	//digitalWrite(RXLED, LOW);   // set the LED on
	//TXLED0; //TX LED is not tied to a normally controlled pin

	delay(1000);


	st[0] = 0;
	pControlLines[0].SetState(st);

	//Serial.println(EEPROM.read(0));

	//digitalWrite(RXLED, HIGH);    // set the LED off
	//TXLED1;

	delay(1000);
}
//****************************************************************************************
