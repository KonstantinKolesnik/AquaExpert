#ifndef TEMPERATURESENSORS_H_
#define TEMPERATURESENSORS_H_

#include "OWI/OWI.h"

#define DS1820_FAMILY_ID                0x10
#define DS1820_START_CONVERSION         0x44
#define DS1820_READ_SCRATCHPAD          0xbe
#define DS1820_ERROR                    -1000   // Return code. Outside temperature range.

//#define DS2890_FAMILY_ID                0x2c
//#define DS2890_WRITE_CONTROL_REGISTER   0X55
//#define DS2890_RELEASE_CODE             0x96
//#define DS2890_WRITE_POSITION           0x0f
//
signed int DS1820_ReadTemperature(unsigned char bus, unsigned char* id);
//void DS2890_SetWiperPosition(unsigned char position, unsigned char bus, unsigned char* id);

#endif /* TEMPERATURESENSORS_H_ */