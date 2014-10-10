#include <OneWireSlave.h>
#include <OneWire.h>
#include <EEPROM.h>
#include "Hardware.h"
#include "ControlLine.h"
#include "BusModule.h"
//****************************************************************************************
void setup()
{
	//while (!Serial) {
	//	; // wait for serial port to connect. Needed for Leonardo only
	//}

	Serial.begin(9600);
	module.Init(MODULE_TYPE);
}
//****************************************************************************************
void loop()
{
	module.LoopProc();
	
	// for test:

	//ControlLine* pControlLines = module.GetControlLines();

	//int16_t st[2] = {1, 0};
	//pControlLines[0].SetState(st);
	//delay(1000);

	//st[0] = 0;
	//pControlLines[0].SetState(st);
	//delay(1000);
}
//****************************************************************************************
