#include "Relays.h"
//****************************************************************************************
void InitRelays()
{
	// Port B
	DDRB = 0xFF;	// all outputs
	
	bool active = RELAY_ACTIVE_LEVEL ? RELAY_DEFAULT_STATE : !RELAY_DEFAULT_STATE;
	PORTB = active ? 0xFF : 0x00;	
}
void SetRelay(uint8_t idx, bool active)
{
	active = RELAY_ACTIVE_LEVEL ? active : !active;
	if (!active)
		PORTB |= (1<<idx);
	else
		PORTB &= ~(1<<idx);
}
void SetRelays(bool active)
{
	active = RELAY_ACTIVE_LEVEL ? active : !active;
	PORTB = active ? 0xFF : 0x00;
}
bool GetRelay(uint8_t idx)
{
	return RELAY_ACTIVE_LEVEL ? (PINB & (1<<idx)) : !(PINB & (1<<idx));
}
