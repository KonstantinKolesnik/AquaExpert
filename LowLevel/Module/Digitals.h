#ifndef DIGITALS_H_
#define DIGITALS_H_
//****************************************************************************************
#include "Hardware.h"
//****************************************************************************************
#define DIGITAL_ACTIVE_STATE		false	// 8-relay module active level is "0"
#define DIGITAL_DEFAULT_STATE		false	// off
//****************************************************************************************
void Digitals_Init();
void SetDigital(uint8_t address, bool active);
void SetDigitals(bool active);
bool GetDigital(uint8_t address);
uint8_t GetDigitals();
//****************************************************************************************
#endif