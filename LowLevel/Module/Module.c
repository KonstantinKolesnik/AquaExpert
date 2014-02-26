#include "Hardware.h"
#include "Digitals.h"
#include "Analogs.h"
#include "OW\onewire.h"
#include "I2C\I2C.h"
#include "WaterSensors.h"
#include "TemperatureSensors.h"
//****************************************************************************************
typedef struct
{
	uint8_t Type;
	uint8_t Address;
	uint8_t State[2];
} ControlLine_t;
//****************************************************************************************
static uint8_t msg[TWI_BUFFER_LENGTH];
static uint8_t msg_size = 0;

static ControlLine_t* controlLines = NULL;
static volatile uint16_t controlLinesCount = 0;

void OnMasterWrite(int howMany);
void OnMasterRead();
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
	
	Digitals_Init();
	Analogs_Init(true); // use internal Vcc reference
	OW_Init();

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
	
	I2C_beginWithAddress(5);                // join i2c bus with address #5
	I2C_onReceive(OnMasterWrite);
	I2C_onRequest(OnMasterRead);

	sei();
}

uint8_t GetControlLinesCount(uint8_t type)
{
	switch (MODULE_TYPE)
	{
		case 0: // test full module
			if		(type == CONTROL_LINE_TYPE_DIGITAL) return 8;
			else if (type == CONTROL_LINE_TYPE_ANALOG) return 8;
			else if (type == CONTROL_LINE_TYPE_PWM) return 2;
			else if (type == CONTROL_LINE_TYPE_ONE_WIRE) return OW_GetDeviceCount();
			break;
		case 1: // AE-R8
			if (type == CONTROL_LINE_TYPE_DIGITAL) return 8;
			break;
	}
	
	return 0;
}
void GetControlLineState(uint8_t type, uint8_t address, uint8_t* result)
{
	ControlLine_t* pLines = controlLines;
	
	for (uint8_t i = 0; i < controlLinesCount; i++)
	{
		if (pLines->Type == type && pLines->Address == address)
		{
			result[0] = pLines->State[0];
			result[1] = pLines->State[1];
			return;
		}
		
		pLines++;
	}
}
void SetControlLineState(uint8_t type, uint8_t address, uint8_t* state)
{
	ControlLine_t* pLines = controlLines;
	
	for (uint8_t i = 0; i < controlLinesCount; i++)
	{
		if (pLines->Type == type && pLines->Address == address)
		{
			switch (type)
			{
				case CONTROL_LINE_TYPE_DIGITAL:
					SetDigital(address, state[0]);
					controlLines[i].State[0] = state[0];
					break;
				case CONTROL_LINE_TYPE_PWM:

					break;
			}

			return;
		}
		
		pLines++;
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
			controlLines = (ControlLine_t*)realloc((void*)controlLines, controlLinesCount * sizeof(ControlLine_t));
			
			controlLines[idx].Type = type;
			controlLines[idx].Address = number;
			memset(controlLines[idx].State, 0, sizeof(controlLines[idx].State));
		}
	}
}
void PopulateModuleState()
{
	ControlLine_t* pLines = controlLines;
	
	for (uint8_t i = 0; i < controlLinesCount; i++)
	{
		pLines->State[0] = 0;
		pLines->State[1] = 0;
		
		switch (pLines->Type)
		{
			case CONTROL_LINE_TYPE_DIGITAL:
				pLines->State[0] = GetDigital(pLines->Address);
				pLines->State[1] = 0;
				break;
			case CONTROL_LINE_TYPE_ANALOG:
				//pLines->State[0] = 7;
				//pLines->State[1] = 3;
				break;
			case CONTROL_LINE_TYPE_PWM:
				//pLines->State[0] = 7;
				//pLines->State[1] = 3;
				break;
			//case CONTROL_LINE_TYPE_WATER_SENSOR:
				//pLines->State[0] = IsWaterSensorWet(pLines->Address);
				//pLines->State[1] = 0;
				//break;
			//case CONTROL_LINE_TYPE_TEMPERATURE_SENSOR:
				//ReadTemperature(pLines->Address, (void*)pLines->State);
				//_delay_ms(800); // needed to wait for temperature sensors conversion!!!!!!!!!!!!
				//break;
		}
		
		pLines++;
	}
}
//****************************************************************************************
void OnMasterWrite(int howMany)
{
	BlinkMsgLed();
	
	uint8_t i = 0; 
	while (I2C_available())
	{
		msg[i] = I2C_read();
		i++;
	}
}
void OnMasterRead()
{
	msg_size = 0;
	
	switch (msg[0]) // command type
	{
		case CMD_GET_TYPE:
			msg[0] = MODULE_TYPE;
			msg_size = 1;
			break;
		case CMD_GET_CONTROL_LINE_COUNT:
			msg[0] = GetControlLinesCount(msg[1]);
			msg_size = 1;
			break;
		case CMD_GET_CONTROL_LINE_STATE:
			GetControlLineState(msg[1], msg[2], msg);
			msg_size = 2;
			break;
		case CMD_SET_CONTROL_LINE_STATE:
			SetControlLineState(msg[1], msg[2], &msg[3]);
			GetControlLineState(msg[1], msg[2], msg);
			msg_size = 2;
			break;
		default:
			break;
	}
	
	I2C_write(msg, msg_size);
}
//****************************************************************************************
int main()
{
	InitHardware();
	InitModuleState();
	
	PopulateModuleState();
	
    while (true)
    {
		//wdt_reset();
		PopulateModuleState();
    }
	
	return 0;
}
