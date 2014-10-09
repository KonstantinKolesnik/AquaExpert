#ifndef _HARDWARE_h
#define _HARDWARE_h
//****************************************************************************************
#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif

#define EEPROM_OFFSET			10 // 6 bytes reserved for 1-Wire slave address
//****************************************************************************************
typedef enum
{
	Relay,
	PWM,
	Temperature,
	Liquid,
	Ph,
    ORP,
    Conductivity, // + Salinity


} ControlLineType_t;
typedef enum
{
	Unknown,
	Test,		// test full module
	D5,			// AE-D5
	
	
	D6,			// AE-D6



} ModuleType_t;
//****************************************************************************************
// commands (from BusHub)
//****************************************************************************************
#define CMD_GET_TYPE							0
#define CMD_GET_CONTROL_LINE_COUNT				1
#define CMD_GET_CONTROL_LINE_STATE				2
#define CMD_SET_CONTROL_LINE_STATE				3
//****************************************************************************************
#endif