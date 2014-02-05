#include "TemperatureSensors.h"
//****************************************************************************************
float ReadTemperature(uint8_t channel)
{
	uint8_t idx = 0;
	
	for (unsigned char i = 0; i < owDevicesCount; i++)
	{
		// ������ ���������� ����� �� ��� ��������� ����, ������� ���������� � ������ ����� ������
		switch (owDevicesIDs[i][0])
		{
			case OW_DS18B20_FAMILY_CODE: // ���� ������ ����������� DS18B20
				if (idx == channel)
				{
					//print_address(owDevicesIDs[i]); // �������� �����
					DS18x20_StartMeasure(owDevicesIDs[i]); // ��������� ���������
					timerDelayMs(800); // ���� ������� 750 ��, ���� �������������� �����������
					unsigned char data[2]; // ���������� ��� �������� �������� � �������� ����� ������
					DS18x20_ReadData(owDevicesIDs[i], data); // ��������� ������
					
					//uint8_t ttt[2];
					//DS18x20_ConvertToThemperature(data, ttt);
					
					return DS18x20_ConvertToThemperatureFl(data); // ��������������� ����������� � ���������������� ���
				}
				else
					idx++;
				
				break;
		}
	}
	
	return 0.0;
}
