#include "ADC\ADC.h"
#include "WaterSensors.h"

bool IsWaterSensorWet(uint8_t channel) // channel = 0...2
{
	uint16_t v = ReadADC(channel);
	return v >= 300; // 524 for water; 838 for short circuit
}