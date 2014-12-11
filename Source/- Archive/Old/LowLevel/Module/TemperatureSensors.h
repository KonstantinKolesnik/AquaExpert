#ifndef TEMPERATURESENSORS_H_
#define TEMPERATURESENSORS_H_
//****************************************************************************************
#include "Hardware.h"
#include "OneWire.h"
#include "OW\ds18x20.h"
//****************************************************************************************
void ReadTemperature(uint8_t address, uint8_t* result);
//****************************************************************************************
#endif /* TEMPERATURESENSORS_H_ */