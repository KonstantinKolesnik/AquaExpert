#include "ADC\ADC.h"
#include "WaterSensors.h"

bool IsWaterSensorWet(uint8_t channel) // channel = 0...2
{
	uint16_t v = ReadADC(channel);
	
	 // transistor: 524 for water;  838 for short circuit; (100/100/KT3102)
	 // Yusupov:    660 for water; 1005 for short circuit; (2k / 100k)
	return v >= 300;
}