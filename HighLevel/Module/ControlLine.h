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
	volatile uint8_t m_state[2];

	OneWire* m_pds;
	bool GetTemperature();

 protected:


 public:
	ControlLine(ControlLineType_t type, uint8_t address, uint8_t pin);

	void UpdateState();
	volatile uint8_t* GetState();
	void SetState(uint8_t* state);
};
//****************************************************************************************
#endif

