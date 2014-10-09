#include <OneWireSlave.h>
#include <OneWire.h>
#include <EEPROM.h>
#include "Hardware.h"
#include "ControlLine.h"
#include "NetworkModule.h"
//****************************************************************************************
void setup()
{
	Serial.begin(9600);
	
	// current module type!!!:
	Module.Init(Test);
}
//****************************************************************************************
void loop()
{
	Module.UpdateState();

	Module.PrintState();
	Serial.println("");



	//ControlLine* pControlLines = Module.GetControlLines();
	//int16_t st[2] = {1, 0};
	//pControlLines[0].SetState(st);

	//delay(1000);

	//st[0] = 0;
	//pControlLines[0].SetState(st);

	//delay(1000);
}
//****************************************************************************************
