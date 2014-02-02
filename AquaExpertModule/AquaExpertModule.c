#include "Hardware.h"
#include "ADC.h"
#include "Relays.h"
#include "IIC_ultimate.h"

uint16_t vvv = 0;

//****************************************************************************************
void InitHardware()
{
	MCUCR &= 0b01111111;			// PUD bit = Off: pull up enabled
	ACSR |= (1 << ACD);				// ACD bit = On: Analog comparator disabled
	
	// Port A
	//PORTA |= (1<<ACK_DETECT);			// pull up
	//DDRA  |= (0<<ACK_DETECT);    		// in

	
			
			
		  
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
	InitRelays();
	InitADC(false); // use internal 2.56V reference
	
	SetRelays(RELAY_DEFAULT_STATE);
	
	Init_i2c();               		// Запускаем и конфигурируем i2c
	Init_Slave_i2c(SlaveOutFunc);   // Настраиваем событие выхода при сработке как Slave
	

	
    while (1)
    {
		//CheckAcknowledgement();
		
		//SetRelay(0, GetRelay(0) ? false : true);
		vvv = ReadADC(0);

		wdt_reset();
		_delay_ms(200);
    }
}
//****************************************************************************************
