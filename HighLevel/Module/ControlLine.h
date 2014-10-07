#ifndef _CONTROLLINE_h
#define _CONTROLLINE_h
//****************************************************************************************
#include <OneWire.h>
#include "Hardware.h"
//****************************************************************************************
class ControlLine
{
 private:
	ControlLineType_t m_type;
	uint8_t m_address;
	uint8_t m_pin;
	volatile int16_t m_state[2];

	OneWire* m_pds;
	bool GetTemperature();
	bool GetPh();

 public:
	ControlLine(ControlLineType_t type, uint8_t address, uint8_t pin);

	uint8_t GetAddress();
	ControlLineType_t GetType();

	void UpdateState();
	volatile int16_t* GetState();
	void SetState(int16_t* state);
};
//****************************************************************************************
#endif

