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
    GetControlLinesCount,
    GetControlLineInfo,
    SetControlLineMode,
    GetControlLineState,
    SetControlLineState
} CommandType_t;

typedef enum
{
    // states: 0, 1;
    DigitalInput = 0,
        
    // states: 0, 1
    DigitalOutput = 1,
        
    // ADC; states: 0...1024
    AnalogInput = 2,
        
    // states: 0...255
    PWM = 4,
        
    // state: float?
    OneWire = 8, // ?
        
    SPI = 16, // ?
        
    I2C = 32 // ?
} ControlLineMode_t;

typedef enum
{
    Unknown,	// test module
    D5,			// SNM-D5
    D6,			// SNM-D6
    D8,			// SNM-D8





} ModuleType_t;
//****************************************************************************************
#endif