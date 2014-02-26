#include "Digitals.h"
//****************************************************************************************
void Digitals_Init()
{
	DIGITAL_DDR = 0xFF;	// all outputs
	SetDigitals(DIGITAL_DEFAULT_STATE);
}
void SetDigital(uint8_t address, bool active)
{
	bool output = DIGITAL_ACTIVE_STATE ? active : !active;
	if (output)
		DIGITAL_PORT |= (1 << address);
	else
		DIGITAL_PORT &= ~(1 << address);
}
void SetDigitals(bool active)
{
	bool output = DIGITAL_ACTIVE_STATE ? active : !active;
	DIGITAL_PORT = output ? 0xFF : 0x00;
}
bool GetDigital(uint8_t address)
{
	bool output = (DIGITAL_PIN & (1 << address));
	return DIGITAL_ACTIVE_STATE ? output : !output;
}
uint8_t GetDigitals()
{
	uint8_t output = (DIGITAL_PORT);
	return DIGITAL_ACTIVE_STATE ? output : ~output;
}