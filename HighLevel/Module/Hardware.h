#ifndef _HARDWARE_h
#define _HARDWARE_h
//****************************************************************************************
#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif
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
	Test,		// test full module
	D8,			// AE-D8


} ModuleType_t;
//****************************************************************************************
// commands (from BusMaster)
//****************************************************************************************
#define CMD_GET_TYPE							0
#define CMD_GET_CONTROL_LINE_COUNT				1
#define CMD_GET_CONTROL_LINE_STATE				2
#define CMD_SET_CONTROL_LINE_STATE				3
//****************************************************************************************
#endif