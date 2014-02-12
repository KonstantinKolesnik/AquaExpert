#include "TemperatureSensors.h"
//****************************************************************************************
void ReadTemperature(uint8_t channel, uint8_t* result)
{
	uint8_t idx = 0;
	
	for (unsigned char i = 0; i < owDevicesCount; i++)
	{
		switch (owDevicesIDs[i][0]) // ������ ���������� ����� �� ��� ��������� ����, ������� ���������� � ������ ����� ������; owDevicesIDs[i] - �����
		{
			case OW_DS18B20_FAMILY_CODE: // ���� ������ ����������� DS18B20
				if (idx == channel)
				{
					DS18x20_StartMeasure(owDevicesIDs[i]); // ��������� ���������
					//timerDelayMs(800); // ���� ������� 750 ��, ���� �������������� �����������
					_delay_ms(800);
					
					unsigned char data[2]; // ���������� ��� �������� �������� � �������� ����� ������
					DS18x20_ReadData(owDevicesIDs[i], data); // ��������� ������
					
					DS18x20_ConvertToThemperature(data, result);
					return;// (result[0] << 8) | result[1];
					
					//return DS18x20_ConvertToThemperatureFl(data); // ��������������� ����������� � ���������������� ���
				}
				else
					idx++;
				
				break;
		}
	}
}
