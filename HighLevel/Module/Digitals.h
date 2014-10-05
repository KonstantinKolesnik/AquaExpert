#ifndef _DIGITALS_h
#define _DIGITALS_h
//****************************************************************************************
#include <EEPROM.h>
#include "Hardware.h"
//****************************************************************************************
#define DIGITAL_ACTIVE_LEVEL		LOW	// 8-relay module active level is "0"
//****************************************************************************************
class Digitals
{
public:
    Digitals(bool isActive);

	void SetActive(uint8_t address, bool isActive);
	void SetActive(bool isActive);

	bool IsActive(uint8_t address);
};
//****************************************************************************************
#endif