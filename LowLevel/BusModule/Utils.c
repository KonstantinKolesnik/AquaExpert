#include "Utils.h"
//****************************************************************************************
void digitalWrite(uint8_t port, uint8_t pin, uint8_t value)
{
	if (value)
		port |= (1<<pin);
	else
		port &= ~(1<<pin);
}