#include "Analogs.h"
//****************************************************************************************
static float vRef = 5.00;
//****************************************************************************************
void Analogs_Init(bool useVccRef)
{
	// ADC clock must be in range of 50KHz to 200KHz
	ADCSRA |= ((1 << ADPS2) | (1 << ADPS1) | (0 << ADPS0));    // ADC reference clock, 8 MHz / 64 = 125 KHz
	
	if (useVccRef)
	{
		ADMUX  |= (0 << REFS1) | (1 << REFS0);			// Voltage reference from Avcc (5v)
		vRef = 5.00;
	}
	else
	{
		ADMUX  |= (1 << REFS1) | (1 << REFS0);			// Internal 2.56V Voltage Reference with external capacitor at AREF pin
		vRef = 2.56;	
	}
	
	ADCSRA |= (1<<ADEN);							// Turn on ADC
	ADCSRA |= (1<<ADSC);							// Do an initial conversion because this one is the slowest and to ensure that everything is up and running
}
uint16_t GetAnalog(uint8_t address)
{
	address &= 0b00000111;
	ADMUX = (ADMUX & 0xF8) | address;	// clears the bottom 3 bits before ORing

	ADCSRA |= (1<<ADSC);                // starts a new conversion
	
	//while (ADCSRA & (1<<ADIF));         // wait until the conversion is done
	while (ADCSRA & (1<<ADSC));
	
	//ADCSRA |= (1<<ADIF);				// clear ADIF by writing one to it

	return (ADC);
}
float GetAnalogsVref()
{
	return vRef;
}