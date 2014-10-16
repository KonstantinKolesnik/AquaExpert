#ifndef BUSMODULECOMMANDS_H_
#define BUSMODULECOMMANDS_H_
//****************************************************************************************
#define MAX_CONTROL_LINE_TYPES					4	// Digital, Analog, PWM, OneWire
#define MAX_ONE_WIRE_DEVICES					10
//****************************************************************************************
// commands (from BusMaster)
//****************************************************************************************
#define CMD_GET_TYPE							0
#define CMD_GET_CONTROL_LINE_COUNT				1
#define CMD_GET_CONTROL_LINE_STATE				2
#define CMD_SET_CONTROL_LINE_STATE				3
//****************************************************************************************
// control line types
//****************************************************************************************
#define CONTROL_LINE_TYPE_DIGITAL				0
#define CONTROL_LINE_TYPE_ANALOG				1
#define CONTROL_LINE_TYPE_PWM					2
#define CONTROL_LINE_TYPE_ONE_WIRE				3
//****************************************************************************************
#endif