#include "Hardware.h"
#include <avr/io.h>
#include <avr/interrupt.h>
#include "ShortCircuitDetector.h"
//****************************************************************************************
#define PERIOD_CHECK	5							// period to check for short circuit, ms; max=21.76ms
#define PERIOD_LED		2000L						// time for LED_SHORT to be on, ms
//****************************************************************************************
int pause = 0;
//****************************************************************************************
ISR(TIMER0_COMP_vect)
{
	//sei(); // needed for usb
	
	// main track
	if (MAIN_SHORT_DETECTED)
	{
		MAIN_TRACK_OFF;
		LED_SHORT_MAIN_ON;
		pause = 0;
	}
	else if (MAIN_SHORT_BLOCKED)
	{
		pause += PERIOD_CHECK;
		if (pause >= PERIOD_LED)
		{
			LED_SHORT_MAIN_OFF;
		}
	}
	// program track
	else if (PROG_SHORT_DETECTED)
	{
		PROG_TRACK_OFF;
		LED_SHORT_PROG_ON;
		pause = 0;
	}
	else if (PROG_SHORT_BLOCKED)
	{
		pause += PERIOD_CHECK;
		if (pause >= PERIOD_LED)
		{
			LED_SHORT_PROG_OFF;
		}
	}
}
//****************************************************************************************
void InitShortCircuitDetector()
{
	TCNT0 = 0;
	TIMSK |= (1<<OCIE0);							// T0 compare interrupt enabled
	
	TCCR0 =  (0<<COM01) | (0<<COM00)				// Normal port operation, OC0 pin disconnected
		   | (0<<FOC0)								// Force Output Compare disabled
           | (1<<WGM01) | (0<<WGM00)				// CTC mode, TOP = OCR0
           | (1<<CS02)  | (0<<CS01)  | (1<<CS00);	// clock source = sys_clk / 1024
		   
	OCR0 = (F_CPU / 1024) * PERIOD_CHECK / 1000L;		// output compare match value; max=255 (21.76ms); current = 59 (5 ms, PERIOD_SC)
}