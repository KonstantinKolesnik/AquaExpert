#ifndef _HARDWARE_h
#define _HARDWARE_h
//****************************************************************************************
#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif
//****************************************************************************************
// module types:
#define MODULE_TYPE_TEST				0 // test full module
#define MODULE_TYPE_AE_R8				1 // AE-R8


// current module type:
#define MODULE_TYPE						MODULE_TYPE_TEST
//****************************************************************************************
#if MODULE_TYPE == MODULE_TYPE_TEST
	#define DIGITAL_0					2
	#define DIGITAL_1					3
	#define DIGITAL_2					4
	#define DIGITAL_3					5
	#define DIGITAL_4					6
	#define DIGITAL_5					7
	#define DIGITAL_6					8
	#define DIGITAL_7					9
	#define TEMPERATURE_BUS				16
#elif MODULE_TYPE == MODULE_TYPE_AE_R8


#endif










//****************************************************************************************
#endif