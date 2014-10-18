#ifndef _HARDWARE_h
#define _HARDWARE_h
//****************************************************************************************
#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif

#define EEPROM_OFFSET			10 // 6 bytes reserved for radio address
//****************************************************************************************
typedef enum
{
    GetControlLinesCount, // no params
    GetControlLineInfo, // [idx]
	GetControlLineMode, // [idx]
	SetControlLineMode, // [idx, mode]
	GetControlLineState, // [idx]
	SetControlLineState // [idx, state]
} CommandType_t;

typedef enum
{
    // states: 0, 1;
    DigitalInput = 1,
        
    // states: 0, 1
    DigitalOutput = 2,
        
    // ADC; states: 0...1024
    AnalogInput = 4,
        
    // states: 0...255
    PWM = 8,
        
    // state: float?
    OneWireBus = 16, // ?
        
    SPIBus = 32, // ?
        
    I2CBus = 64 // ?
} ControlLineMode_t;

typedef struct
{
	uint8_t modes;
	ControlLineMode_t mode;
} ControlLineInfo_t;

typedef enum
{
    Unknown,	// test module
    D5,			// SNM-D5
    D6,			// SNM-D6
    D8,			// SNM-D8





} ModuleType_t;
//****************************************************************************************
#endif