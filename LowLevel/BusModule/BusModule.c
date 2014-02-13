#include "Hardware.h"
#include "ADC\ADC.h"
#include "OW\onewire.h"
#include "IIC\IIC_slave.h"
#include "Relays.h"
#include "WaterSensors.h"
#include "TemperatureSensors.h"
//****************************************************************************************
typedef struct
{
	uint8_t ControlLineType;
	uint8_t ControlLineNumber;
	uint8_t ControlLineState[2];
} ControlLineState_t;
//****************************************************************************************
static uint8_t msg[TWI_BUFFER_SIZE];

static ControlLineState_t* controlLinesStates = NULL;
static uint16_t controlLinesCount = 0;

void ProcessMasterMessages();
//****************************************************************************************
void BlinkMsgLed()
{
	LED_MSG_ON;
	_delay_ms(10);
	LED_MSG_OFF;
}
void InitHardware()
{
	MCUCR &= 0b01111111;			// PUD bit = Off: pull up enabled
	ACSR |= (1<<ACD);				// ACD bit = On: Analog comparator disabled
	
	DDRD |= (1<<LED_MSG);
	LED_MSG_OFF;
	
	InitRelays();
	SetRelay(1, true);
	SetRelay(3, true);
	
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

uint8_t GetControlLinesCount(uint8_t type)
{
	switch (MODULE_TYPE)
	{
		case 0:// test full module
			if (type == CONTROL_LINE_TYPE_RELAY) return 8;
			else if (type == CONTROL_LINE_TYPE_WATER_SENSOR) return 3;
			else if (type == CONTROL_LINE_TYPE_PH_SENSOR) return 2;
			else if (type == CONTROL_LINE_TYPE_ORP_SENSOR) return 2;
			else if (type == CONTROL_LINE_TYPE_TEMPERATURE_SENSOR) return 1;
			else if (type == CONTROL_LINE_TYPE_CONDUCTIVITY_SENSOR) return 1;
			else if (type == CONTROL_LINE_TYPE_DIMMER) return 2;
			break;
		case 1: // AE-R8
			if (type == CONTROL_LINE_TYPE_RELAY) return 8;
			break;
	}
	
	return 0;
}
void GetControlLineState(uint8_t type, uint8_t number, uint8_t* result)
{
	ControlLineState_t* pStates = controlLinesStates;
	
	for (uint8_t i = 0; i < controlLinesCount; i++)
	{
		if (pStates->ControlLineType == type && pStates->ControlLineNumber == number)
		{
			result[0] = pStates->ControlLineState[0];
			result[1] = pStates->ControlLineState[1];
			return;
		}
		
		pStates++;
	}
}
void SetControlLineState(uint8_t type, uint8_t number, uint8_t* state)
{
	ControlLineState_t* pStates = controlLinesStates;
	
	for (uint8_t i = 0; i < controlLinesCount; i++)
	{
		if (pStates->ControlLineType == type && pStates->ControlLineNumber == number)
		{
			switch (type)
			{
				case CONTROL_LINE_TYPE_RELAY:
					SetRelay(number, state[0]);
					break;
				case CONTROL_LINE_TYPE_DIMMER:

					break;
			}

			return;
		}
		
		pStates++;
	}
}

void InitModuleState()
{
	for (uint8_t type = 0; type < MAX_CONTROL_LINE_TYPES; type++)
	{
		uint8_t count = GetControlLinesCount(type);
	
		for (uint8_t number = 0; number < count; number++)
		{
			uint16_t idx = controlLinesCount++;
			controlLinesStates = (ControlLineState_t*)realloc((void*)controlLinesStates, controlLinesCount * sizeof(ControlLineState_t));
			
			controlLinesStates[idx].ControlLineType = type;
			controlLinesStates[idx].ControlLineNumber = number;
			controlLinesStates[idx].ControlLineState[0] = type;
			controlLinesStates[idx].ControlLineState[1] = number;
		}
	}
}
void PopulateModuleState()
{
	ControlLineState_t* pStates = controlLinesStates;
	
	for (uint8_t i = 0; i < controlLinesCount; i++)
	{
		pStates->ControlLineState[0] = 0;
		pStates->ControlLineState[1] = 0;
		
		switch (pStates->ControlLineType)
		{
			case CONTROL_LINE_TYPE_RELAY:
				pStates->ControlLineState[0] = GetRelay(pStates->ControlLineNumber);
				pStates->ControlLineState[1] = 0;
				break;
			case CONTROL_LINE_TYPE_WATER_SENSOR:
				pStates->ControlLineState[0] = IsWaterSensorWet(pStates->ControlLineNumber);
				pStates->ControlLineState[1] = 0;
				break;
			case CONTROL_LINE_TYPE_PH_SENSOR:
				//pStates->ControlLineState[0] = 7;
				//pStates->ControlLineState[1] = 3;
				break;
			case CONTROL_LINE_TYPE_ORP_SENSOR:
				//pStates->ControlLineState[0] = 7;
				//pStates->ControlLineState[1] = 3;
				break;
			case CONTROL_LINE_TYPE_TEMPERATURE_SENSOR:
				ReadTemperature(pStates->ControlLineNumber, pStates->ControlLineState);
				break;
			case CONTROL_LINE_TYPE_CONDUCTIVITY_SENSOR:
				//pStates->ControlLineState[0] = 7;
				//pStates->ControlLineState[1] = 3;
				break;
			case CONTROL_LINE_TYPE_DIMMER:
				//pStates->ControlLineState[0] = 7;
				//pStates->ControlLineState[1] = 3;
				break;
			
			
		}
		
		pStates++;
	}
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
					uint8_t* response = msg;
						
					switch (msg[0]) // command type
					{
						case CMD_GET_TYPE:
							response[0] = MODULE_TYPE;
							TWI_StartTransceiverWithData(response, 1);
							break;
						case CMD_GET_CONTROL_LINE_COUNT:
							response[0] = GetControlLinesCount(msg[1]);
							TWI_StartTransceiverWithData(response, 1);
							break;
						case CMD_GET_CONTROL_LINE_STATE:
							GetControlLineState(msg[1], msg[2], response);
							TWI_StartTransceiverWithData(response, 2);
							break;
						case CMD_SET_CONTROL_LINE_STATE:
							SetControlLineState(msg[1], msg[2], &msg[3]);
							break;
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
	InitModuleState();
	InitHardware();
	
	PopulateModuleState();
	
    while (true)
    {
		wdt_reset();
		PopulateModuleState();
    }
	
	return 0;
}
