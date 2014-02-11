#include "Hardware.h"
#include "ADC\ADC.h"
#include "OW\onewire.h"
#include "IIC\IIC_ultimate.h"
#include "Relays.h"
#include "WaterSensors.h"
#include "TemperatureSensors.h"
//****************************************************************************************
typedef struct
{
	//bool Relays[RELAY_COUNT];
	//bool WaterSensors[WATER_SENSOR_COUNT];
	//uint8_t PhSensors[PH_SENSOR_COUNT];
	//uint16_t OrpSensors[ORP_SENSOR_COUNT];
	//uint16_t ConductivitySensors[CONDUCT_SENSOR_COUNT];
	//uint16_t TemperatureSensors[TEMP_SENSOR_COUNT];
	//uint8_t Dimmers[DIMMER_COUNT];
} State_t;
//****************************************************************************************
static uint8_t controlLinesCount[MAX_CONTROL_LINE_TYPES]; // index = ControlLineType
static volatile State_t moduleState;
static uint8_t msg[TWI_BUFFER_SIZE];

void ProcessMasterMessages();
//****************************************************************************************
void BlinkMsgLed()
{
	LED_MSG_ON;
	_delay_ms(10);
	LED_MSG_OFF;
}
void InitControlLinesCount()
{
	switch (MODULE_TYPE)
	{
		case 0:// test full module
			controlLinesCount[CONTROL_LINE_TYPE_RELAY] = 8;
			controlLinesCount[CONTROL_LINE_TYPE_WATER_SENSOR] = 3;
			controlLinesCount[CONTROL_LINE_TYPE_PH_SENSOR] = 2;
			controlLinesCount[CONTROL_LINE_TYPE_ORP_SENSOR] = 2;
			controlLinesCount[CONTROL_LINE_TYPE_TEMPERATURE_SENSOR] = 1;
			controlLinesCount[CONTROL_LINE_TYPE_CONDUCTIVITY_SENSOR] = 1;
			controlLinesCount[CONTROL_LINE_TYPE_DIMMER] = 2;
			break;
		case 1: // AE-R8
			controlLinesCount[CONTROL_LINE_TYPE_RELAY] = 8;
			break;
	}
}
void InitHardware()
{
	MCUCR &= 0b01111111;			// PUD bit = Off: pull up enabled
	ACSR |= (1<<ACD);				// ACD bit = On: Analog comparator disabled
	
	DDRD |= (1<<LED_MSG);
	LED_MSG_OFF;
	
	InitRelays();
	
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
	//for (uint8_t i = 0; i < RELAY_COUNT; i++)
		//moduleState.Relays[i] = GetRelay(i);
		//
	//for (uint8_t i = 0; i < WATER_SENSOR_COUNT; i++)
		//moduleState.WaterSensors[i] = IsWaterSensorWet(i);
		//
	//for (uint8_t i = 0; i < TEMP_SENSOR_COUNT; i++)
		//moduleState.TemperatureSensors[i] = ReadTemperature(i);
		//
		
}
uint8_t OnLastTransmissionError(uint8_t TWIerrorMsg)
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
				TWI_GetDataFromTransceiver(msg, TWI_BUFFER_SIZE);
					
				if (TWI_statusReg.genAddressCall) // last operation was a reception as General Call
				{
					//uint8_t cmd = msg[0];
				}
				else // last operation was a reception as Slave Address Match
				{
					switch (msg[0]) // command type
					{
						case CMD_GET_TYPE:
							msg[0] = MODULE_TYPE;
							TWI_StartTransceiverWithData(msg, 1);
						case CMD_GET_LINE_COUNT:
							msg[0] = controlLinesCount[msg[1]];
							TWI_StartTransceiverWithData(msg, 1);
							break;
						//case CMD_GET_RELAY_STATE:
							//msg[0] = moduleState.Relays[msg[1]];
							////TWI_StartTransceiverWithData(msg, 1);
							//break;
						//case CMD_SET_RELAY_STATE:
							////SetRelay(msg[1], msg[2]);
							//break;
						//case CMD_GET_TEMPERATURE:
							//msg[0] = moduleState.TemperatureSensors[msg[1]] >> 8;
							//msg[1] = moduleState.TemperatureSensors[msg[1]] & 0xFF;
							////TWI_StartTransceiverWithData(msg, 2);
							//break;	
						default:
							break;	
					}
				}
				BlinkMsgLed();
			}
			else // last operation was a transmission
			{
			}
				
			// Check if the TWI Transceiver has already been started. If not then restart it to prepare it for new receptions.
			if (!TWI_TransceiverBusy())
				TWI_StartTransceiver();
		}
		else // Ends up here if the last operation completed unsuccessfully
			OnLastTransmissionError(TWI_GetStateInfo());
	}
}

int main()
{
	InitControlLinesCount();
	InitHardware();
	
    while (true)
    {
		wdt_reset();
		//PopulateModuleState();
    }
}
