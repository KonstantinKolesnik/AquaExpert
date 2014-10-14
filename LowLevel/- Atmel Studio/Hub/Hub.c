#include "Hardware.h"
//****************************************************************************************
void BlinkMsgLed()
{
	LED_MSG_ON;
	_delay_ms(5);
	LED_MSG_OFF;
}
void InitHardware()
{
	MCUCR &= 0b01111111;			// PUD bit = Off: pull up enabled
	ACSR |= (1<<ACD);				// ACD bit = On: Analog comparator disabled
	
	DDRD |= (1<<LED_MSG);
	LED_MSG_OFF;
	
	//Digitals_Init();
	//Analogs_Init(true); // use internal Vcc reference
	//OW_Init();
//
	//PWM_Init();
	//TIMSK = (1<<OCIE1A)     // Timer1/Counter1 Compare A Interrupt; reassigned in InitDCCOut
	//| (0<<OCIE1B)     // Timer1/Counter1 Compare B Interrupt
	//| (0<<TOIE1)      // Timer1/Counter1 Overflow Interrupt
	//| (0<<TICIE1)     // Timer1/Counter1 Capture Interrupt
	//
	//| (0<<OCIE2)      // Timer2/Counter2 Compare Interrupt
	//| (0<<TOIE2)	  // Timer2/Counter2 Overflow Interrupt
	//
	//| (1<<OCIE0)      // Timer0/Counter0 Compare Interrupt
	//| (0<<TOIE0);     // Timer0/Counter0 Overflow Interrupt
	
	//I2C_beginWithAddress(5);                // join i2c bus with address #5
	//I2C_onReceive(OnMasterWrite);
	//I2C_onRequest(OnMasterRead);
//
	sei();
}










//****************************************************************************************
int main(void)
{
	InitHardware();
	
    while (true)
    {
		BlinkMsgLed();
		
		_delay_ms(500);

    }
}