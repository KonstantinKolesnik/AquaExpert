#include "TemperatureSensors.h"
//****************************************************************************************
float ReadTemperature(uint8_t channel)
{
	uint8_t idx = 0;
	
	for (unsigned char i = 0; i < owDevicesCount; i++)
	{
		// узнать устройство можно по его груповому коду, который расположен в первом байте адреса
		switch (owDevicesIDs[i][0])
		{
			case OW_DS18B20_FAMILY_CODE: // если найден термодатчик DS18B20
				if (idx == channel)
				{
					//print_address(owDevicesIDs[i]); // печатаем адрес
					DS18x20_StartMeasure(owDevicesIDs[i]); // запускаем измерение
					timerDelayMs(800); // ждем минимум 750 мс, пока конвентируется температура
					unsigned char data[2]; // переменная для хранения старшего и младшего байта данных
					DS18x20_ReadData(owDevicesIDs[i], data); // считываем данные
					
					//uint8_t ttt[2];
					//DS18x20_ConvertToThemperature(data, ttt);
					
					return DS18x20_ConvertToThemperatureFl(data); // преобразовываем температуру в человекопонятный вид
				}
				else
					idx++;
				
				break;
		}
	}
	
	return 0.0;
}
