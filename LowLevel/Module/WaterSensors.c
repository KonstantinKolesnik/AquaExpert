#include "Analogs.h"
#include "WaterSensors.h"

bool IsWaterSensorWet(uint8_t address) // channel = 0...7
{
	uint16_t v = GetAnalog(address);
	
	 // transistor: 524 for water;  838 for short circuit; (100/100/KT3102)
	 // Yusupov:    660 for water; 1005 for short circuit; (2k / 100k)
	return v >= 300;
}