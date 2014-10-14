#ifndef ANALOGS_H_
#define ANALOGS_H_
//****************************************************************************************
#include "Hardware.h"
//****************************************************************************************
void Analogs_Init(bool useVccRef); // useVccRef => Vcc or internal 2.56V
uint16_t GetAnalog(uint8_t address); // address = 0...7
float GetAnalogsVref();
////****************************************************************************************
#endif /* ADC_H_ */