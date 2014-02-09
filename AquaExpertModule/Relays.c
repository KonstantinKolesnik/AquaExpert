#include "Relays.h"
//****************************************************************************************
void InitRelays()
{
	// Port B
	DDRB = 0xFF;	// all outputs
	SetRelays(RELAY_DEFAULT_STATE);
}
void SetRelay(uint8_t idx, bool active)
{
	bool output = RELAY_ACTIVE_LEVEL ? active : !active;
	if (output)
		PORTB |= (1<<idx);
	else
		PORTB &= ~(1<<idx);
}
void SetRelays(bool active)
{
	bool output = RELAY_ACTIVE_LEVEL ? active : !active;
	PORTB = output ? 0xFF : 0x00;
}
bool GetRelay(uint8_t idx)
{
	bool output = (PINB & (1<<idx));
	return RELAY_ACTIVE_LEVEL ? output : !output;
}
uint8_t GetRelays()
{
	uint8_t output = (PORTB);
	return RELAY_ACTIVE_LEVEL ? output : ~output;
}