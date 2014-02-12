#include "TemperatureSensors.h"
//****************************************************************************************
void ReadTemperature(uint8_t channel, uint8_t* result)
{
	uint8_t idx = 0;
	
	for (unsigned char i = 0; i < owDevicesCount; i++)
	{
		switch (owDevicesIDs[i][0]) // узнать устройство можно по его груповому коду, который расположен в первом байте адреса; owDevicesIDs[i] - адрес
		{
			case OW_DS18B20_FAMILY_CODE: // если найден термодатчик DS18B20
				if (idx == channel)
				{
					DS18x20_StartMeasure(owDevicesIDs[i]); // запускаем измерение
					//timerDelayMs(800); // ждем минимум 750 мс, пока конвентируется температура
					_delay_ms(800);
					
					unsigned char data[2]; // переменная для хранения старшего и младшего байта данных
					DS18x20_ReadData(owDevicesIDs[i], data); // считываем данные
					
					DS18x20_ConvertToThemperature(data, result);
					return;// (result[0] << 8) | result[1];
					
					//return DS18x20_ConvertToThemperatureFl(data); // преобразовываем температуру в человекопонятный вид
				}
				else
					idx++;
				
				break;
		}
	}
}
