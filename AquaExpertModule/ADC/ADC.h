#ifndef ADC_H_
#define ADC_H_
//****************************************************************************************
#include "../Hardware.h"
//****************************************************************************************
void InitADC(bool useVccRef); // useVccRef => Vcc or internal 2.56V
uint16_t ReadADC(uint8_t channel); // channel = 0...7
float GetADCVref();
//****************************************************************************************
#endif /* ADC_H_ */