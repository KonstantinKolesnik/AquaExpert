#include "Hardware.h"
#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>
#include "AckDetector.h"
#include "DCCOut.h"
//---------------------------------------------------------------------------------------------
volatile uint8_t isProgPhase = 0;
volatile uint8_t isAckDetected = 0;
//---------------------------------------------------------------------------------------------
void ResetAckFlag()
{
	isAckDetected = 0;
}
uint8_t QueryAckFlag()
{
	return isAckDetected;
}
void CheckAcknowledgement()
{
	if (isProgPhase && ACK_DETECTED)
	{
		// prog command moved to dccout and is being repeated; now check for ACK
		unsigned char i, j, ackPulses;
		for (j = 0; j < 5; j++)				// ACK must be present for 6ms, we check only for approx 1ms
		{                                   // and we use a filter: small dropouts are ignored
			// count pulses for 0.2 ms
			ackPulses = 0;
			for (i = 0; i < 20; i++)
			{
				_delay_us(10);
				if (ACK_DETECTED)
					ackPulses++;
			}
			if (ackPulses < 20) // 17
				return;               // exits here, seems not to be a real ACK
		}
		// ACK received! skip further prog command repetitions
		cli();
		if (Repeats > 0)
			Repeats = 0;     
		sei();
            
		isAckDetected = 1;
	}
}
//---------------------------------------------------------------------------------------------
