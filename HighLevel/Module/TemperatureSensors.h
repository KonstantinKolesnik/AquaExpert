#ifndef _TEMPERATURESENSORS_h
#define _TEMPERATURESENSORS_h
//****************************************************************************************
#include <OneWire.h>
#include "Hardware.h"
//****************************************************************************************
#define DS18S20_ID 0x10
#define DS18B20_ID 0x28
#define DS1822_ID 0x22
//****************************************************************************************
class TemperatureSensors
{
private:
	OneWire* ds1;
 public:
	bool GetTemperature();
};
//****************************************************************************************
#endif

