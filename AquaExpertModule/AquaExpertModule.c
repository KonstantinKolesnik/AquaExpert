#include "Hardware.h"
#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>
//#include "ShortCircuitDetector.h"
//#include "DCCOut.h"
//#include "AckDetector.h"

void SetSwitch(uint8_t idx, uint8_t state);
void SetSwitches(uint8_t state);
uint8_t GetSwitch(uint8_t idx);

//****************************************************************************************
void InitHardware()
{
	MCUCR &= 0b01111111;			// PUD bit = Off: pull up enabled
	//ACSR = 0b10000000;				// ACD bit = On: Analog comparator disabled
	ACSR |= (1 << ADC);				// ACD bit = On: Analog comparator disabled
	ADCSRA = 0b00000000;			// ADEN bit (#7) = Off: AD converter disabled
	//ADCSRA |= (0 << ADEN);			// ADEN bit (#7) = Off: AD convertor disabled
	
	// Port A
	//PORTA |= (1<<ACK_DETECT);			// pull up
	//DDRA  |= (0<<ACK_DETECT);    		// in

	// Port B (switches)
	PORTB	|= (SWITCH_DEFAULT_STATE<<SW_0)
			|  (SWITCH_DEFAULT_STATE<<SW_1)
			|  (SWITCH_DEFAULT_STATE<<SW_2)
			|  (SWITCH_DEFAULT_STATE<<SW_3)
			|  (SWITCH_DEFAULT_STATE<<SW_4)
			|  (SWITCH_DEFAULT_STATE<<SW_5)
			|  (SWITCH_DEFAULT_STATE<<SW_6)
			|  (SWITCH_DEFAULT_STATE<<SW_7);
	DDRB	|= (1<<SW_0)				// out
			|  (1<<SW_1)				// out
			|  (1<<SW_2)				// out
			|  (1<<SW_3)				// out
			|  (1<<SW_4)				// out
			|  (1<<SW_5)				// out
			|  (1<<SW_6)				// out
			|  (1<<SW_7);				// out
		  
	//// Port C
	//PORTC	|= (1<<SHORT_MAIN)			// pull up
			//|  (1<<SHORT_PROG);			// pull up
	//DDRC	|= (0<<SHORT_MAIN)			// in
			//|  (0<<SHORT_PROG);			// in
	//
	//// Port D
	//PORTD	|= (1<<MY_RXD)				// pull up
			//|  (1<<MY_TXD)				// pull up
	        //|  (0<<USB_DPLUS)			// pull down
			//|  (0<<USB_DMINUS)			// pull down
			//|  (0<<DCC)					// off
			//|  (0<<NDCC);				// off
	//DDRD	|= (0<<MY_RXD)    			// in
			//|  (1<<MY_TXD)   			// out
	        //|  (0<<USB_DPLUS)     		// in
			//|  (0<<USB_DMINUS)     		// in
			//|  (1<<DCC)       			// out
			//|  (1<<NDCC);     			// out
}
void InitInterrupt()
{
    //TIMSK = (1<<OCIE1A)     // Timer1/Counter1 Compare A Interrupt; reassigned in InitDCCOut
          //| (0<<OCIE1B)     // Timer1/Counter1 Compare B Interrupt
          //| (0<<TOIE1)      // Timer1/Counter1 Overflow Interrupt
		  //| (0<<TICIE1)     // Timer1/Counter1 Capture Interrupt
		  //
	      //| (0<<OCIE2)      // Timer2/Counter2 Compare Interrupt
          //| (0<<TOIE2)		// Timer2/Counter2 Overflow Interrupt
		  //
		  //| (1<<OCIE0)      // Timer0/Counter0 Compare Interrupt
		  //| (0<<TOIE0);     // Timer0/Counter0 Overflow Interrupt

	sei();
}
//****************************************************************************************
int main()
{
	InitHardware();
	//InitShortCircuitDetector();
	//InitDCCOut();
	InitInterrupt();
	
	//MAIN_TRACK_OFF;
	//PROG_TRACK_OFF;
	
	SetSwitches(SWITCH_DEFAULT_STATE);
	
    while (1)
    {
		//CheckAcknowledgement();
		
		//SetSwitch(0, GetSwitch(0) == 1 ? 0 : 1);


		//wdt_reset();
		_delay_ms(200);
    }
}
//****************************************************************************************
void SetSwitch(uint8_t idx, uint8_t state)
{
	if (state == 1)
		PORTB |= (1<<idx);
	else
		PORTB &= ~(1<<idx);
}
void SetSwitches(uint8_t state)
{
	PORTB = state;
}
uint8_t GetSwitch(uint8_t idx)
{
	return (PINB & (1<<idx));
}