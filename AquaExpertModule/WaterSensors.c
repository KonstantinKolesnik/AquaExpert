#include "WaterSensors.h"
#include "ADC/ADC.h"

bool IsWaterSensorWet(uint8_t channel) // channel = 0...2
{
	return ReadADC(channel) >= GetADCVref() / 2.0;
}