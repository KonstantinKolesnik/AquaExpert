#include "TemperatureSensors.h"
//****************************************************************************************
void ReadTemperature(uint8_t address, uint8_t* result)
{
	uint16_t idx = 0;

	for (uint16_t i = 0; i < OW_GetDeviceCount(); i++)
	{
		uint8_t* rom = owDevicesIDs[i]; // address
		
		switch (rom[0]) // group code
		{
			case OW_DS18B20_FAMILY_CODE: // DS18B20
				if (idx == address) // idx is index of temperature sensor
				{
					//if (DS18x20_StartMeasure(rom))
					if (DS18x20_StartMeasure(NULL))
					{
						//timerDelayMs(800);
						//_delay_ms(1000); // ���� ������� 750 ��, ���� �������������� �����������
						//while (!digitalRead(pin_test_ds)) {} // �������� ����������� 0-�����������, 1-��������� 
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
