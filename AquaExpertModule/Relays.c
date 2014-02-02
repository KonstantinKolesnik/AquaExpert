#include "Relays.h"

void InitRelays()
{
	// Port B
	PORTB = 0xFF;	// all "1" as 8-relay module active value is "0"
	DDRB = 0xFF;	// all outs
}

void SetRelay(uint8_t idx, bool state)
{
	// we use inverted state!
	if (!state)
		PORTB |= (1<<idx);
	else
		PORTB &= ~(1<<idx);
}

void SetRelays(bool state)
{
	// we use inverted state!
	PORTB = !state ? 0xFF : 0x00;
}

bool GetRelay(uint8_t idx)
{
	// we use inverted state!
	return !(PINB & (1<<idx));
}
