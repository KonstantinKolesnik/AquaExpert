#include "TemperatureSensors.h"
//****************************************************************************************
void ReadTemperature(uint8_t channel, uint8_t* result)
{
	uint16_t idx = 0;

	for (uint16_t i = 0; i < owDevicesCount; i++)
	{
		uint8_t* rom = owDevicesIDs[i];
		
		switch (rom[0]) // ������ ���������� ����� �� ��� ��������� ����, ������� ���������� � ������ ����� ������; rom - �����
		{
			case OW_DS18B20_FAMILY_CODE: // DS18B20
				if (idx == channel) // idx = index of temp. sensor
				{
					result[0] = 44;
					result[1] = 44;
					
					//DS18x20_StartMeasure(rom);
					//timerDelayMs(800);
					//_delay_ms(800); // ���� ������� 750 ��, ���� �������������� �����������
					
					uint8_t data[2];
					//DS18x20_ReadData(rom, data);
					//if (DS18x20_ReadData(rom, data)) // ��������� ������
					{
						//result[0] = 55;
						//result[1] = 55;
						
						DS18x20_ConvertToTemperature(data, result);
						//return DS18x20_ConvertToTemperatureFloat(data); // ��������������� ����������� � ���������������� ���
					}
					
					return;
				}
				idx++;
				break;
			//default:
				//break;
		}
	}
}
