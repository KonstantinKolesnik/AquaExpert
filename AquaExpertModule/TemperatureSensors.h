#ifndef TEMPERATURESENSORS_H_
#define TEMPERATURESENSORS_H_
//****************************************************************************************
#include "Hardware.h"
#include "OW\onewire.h"
#include "OW\ds18x20.h"
//****************************************************************************************
uint16_t ReadTemperature(uint8_t channel);
//****************************************************************************************
#endif /* TEMPERATURESENSORS_H_ */