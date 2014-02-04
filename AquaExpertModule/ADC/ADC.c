#include "ADC.h"

float Vref = 5.00;

void InitADC(bool useVccRef)
{
	// ADC clock must be in range of 50KHz to 200KHz
	ADCSRA |= ((1<<ADPS2)|(1<<ADPS1)|(0<<ADPS0));    // ADC reference clock, 8 MHz / 64 = 125 KHz
	
	if (useVccRef)
	{
		ADMUX  |= (0<<REFS1) | (1<<REFS0);			// Voltage reference from Avcc (5v)
		Vref = 5.00;
	}
	else
	{
		ADMUX  |= (1<<REFS1) | (1<<REFS0);			// Internal 2.56V Voltage Reference with external capacitor at AREF pin
		Vref = 2.56;	
	}
	
	ADCSRA |= (1<<ADEN);							// Turn on ADC
	ADCSRA |= (1<<ADSC);							// Do an initial conversion because this one is the slowest and to ensure that everything is up and running
}

uint16_t ReadADC(char channel)
{
	channel &= 0b00000111;
	ADMUX |= channel;

	ADCSRA |= (1<<ADSC);                //Starts a new conversion
	while (ADCSRA & (1<<ADIF));         //Wait until the conversion is done
	
	ADCSRA |= (1<<ADIF);				//Clear ADIF by writing one to it

	return ADC;
}

float GetADCVref()
{
	return Vref;
}