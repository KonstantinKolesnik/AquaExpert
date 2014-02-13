#include "TemperatureSensors.h"
//****************************************************************************************
void ReadTemperature(uint8_t channel, uint8_t* result)
{
	uint16_t idx = 0;

	for (uint16_t i = 0; i < owDevicesCount; i++)
	{
		uint8_t* rom = owDevicesIDs[i];
		
		switch (rom[0]) // узнать устройство можно по его груповому коду, который расположен в первом байте адреса; rom - адрес
		{
			case OW_DS18B20_FAMILY_CODE: // DS18B20
				if (idx == channel) // idx = index of temp. sensor
				{
					//if (DS18x20_StartMeasure(rom))
					if (DS18x20_StartMeasure(NULL))
					{
						//timerDelayMs(800);
						//_delay_ms(1000); // ждем минимум 750 мс, пока конвентируется температура
						//while (!digitalRead(pin_test_ds)) {} // задержка конвертации 0-конвертация, 1-окончание 
						//while(!(LINE_IS_ON));
					
						uint8_t data[2];
						if (DS18x20_ReadData(rom, data))
							DS18x20_ConvertToTemperature(data, result);
					}
					
					return;
				}
				idx++;
				break;
		}
	}
}
