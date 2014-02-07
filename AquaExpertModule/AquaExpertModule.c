#include "Hardware.h"
#include "ADC\ADC.h"
#include "OW\onewire.h"
#include "IIC\IIC_ultimate.h"
#include "Relays.h"
#include "WaterSensors.h"
#include "TemperatureSensors.h"
//****************************************************************************************
// commands (from Master)
#define TWI_CMD_GET_PROPERTIES			0x00




//****************************************************************************************
typedef struct
{
	bool Relays[RELAY_COUNT];
	bool WaterSensors[WATER_SENSOR_COUNT];
	uint8_t PhSensors[PH_SENSOR_COUNT];
	uint8_t OrpSensors[ORP_SENSOR_COUNT];
	float TempSensors[TEMP_SENSOR_COUNT];
} State_t;
//****************************************************************************************
static volatile State_t moduleState;
uint8_t messageBuf[TWI_BUFFER_SIZE];
//****************************************************************************************
void InitHardware()
{
	MCUCR &= 0b01111111;			// PUD bit = Off: pull up enabled
	ACSR |= (1<<ACD);				// ACD bit = On: Analog comparator disabled
	
	DDRD |= (1<<LED_MSG);
	LED_MSG_OFF;
	
	InitRelays();
	//SetRelays(RELAY_DEFAULT_STATE);
	
	InitADC(true); // use internal Vcc reference
	
	InitOW();

	//InitPWM();
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
	
	InitI2C();
	TWI_StartTransceiver(); // Start the TWI transceiver to enable reception of the first command from the TWI Master.

	sei();
}
void PopulateModuleState()
{
	// for test:
	//SetRelay(0, GetRelay(0) ? false : true);
	//_delay_ms(200);
	
	for (uint8_t i = 0; i < WATER_SENSOR_COUNT; i++)
		moduleState.WaterSensors[i] = IsWaterSensorWet(i);
		
	for (uint8_t i = 0; i < RELAY_COUNT; i++)
		moduleState.Relays[i] = GetRelay(i);
		
	for (uint8_t i = 0; i < TEMP_SENSOR_COUNT; i++)
		moduleState.TempSensors[i] = ReadTemperature(i);
}
uint8_t OnFailureInLastTransmission(uint8_t TWIerrorMsg)
{
	// A failure has occurred, use TWIerrorMsg to determine the nature of the failure and take appropriate actions.
	// See header file for a list of possible failures messages.
	
	// This very simple example puts the error code on PORTB and restarts the transceiver with all the same data in the transmission buffers.
	//PORTB = TWIerrorMsg;
	TWI_StartTransceiver();
	
	return TWIerrorMsg;
}
void ProcessMasterMessages()
{
	if (!TWI_TransceiverBusy()) // Check if the TWI Transceiver has completed an operation
	{
		if (TWI_statusReg.lastTransOK) // Check if the last operation was successful
		{
			if (TWI_statusReg.RxDataInBuf) // Check if the last operation was a reception
			{
				LED_MSG_ON;
				_delay_ms(5);
				LED_MSG_OFF;
				
				TWI_GetDataFromTransceiver(messageBuf, TWI_BUFFER_SIZE);
					
				if (TWI_statusReg.genAddressCall) // last operation was a reception as General Call
				{
					// Put data received out to PORTB as an example.
					//PORTB = messageBuf[0];
				}
				else // last operation was a reception as Slave Address Match
				{
					switch (messageBuf[0]) // command type
					{
						case TWI_CMD_GET_PROPERTIES:
							messageBuf[0] = RELAY_COUNT;
							messageBuf[1] = WATER_SENSOR_COUNT;
							messageBuf[2] = PH_SENSOR_COUNT;
							messageBuf[3] = ORP_SENSOR_COUNT;
							messageBuf[4] = TEMP_SENSOR_COUNT;
							TWI_StartTransceiverWithData(messageBuf, 5);
							break;
						//case TWI_CMD_MASTER_WRITE:
							////PORTB = messageBuf[1];
							//break;
						//
						default:
							break;	
					}
				}
			}
			else // last operation was a transmission
			{
				//__no_operation(); // Put own code here.
			}
				
			// Check if the TWI Transceiver has already been started. If not then restart it to prepare it for new receptions.
			if (!TWI_TransceiverBusy())
				TWI_StartTransceiver();
		}
		else // Ends up here if the last operation completed unsuccessfully
			OnFailureInLastTransmission(TWI_GetStateInfo());
	}
}

int main()
{
	InitHardware();
	
    while (true)
    {
		wdt_reset();
		PopulateModuleState();
		ProcessMasterMessages();
    }
}
