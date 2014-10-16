#ifndef _CONTROLLINE_h
#define _CONTROLLINE_h
//****************************************************************************************
#include <OneWire.h>
#include "Hardware.h"
//****************************************************************************************
class ControlLine
{
	private:
		uint8_t m_pin;
		uint8_t m_address;
		uint8_t m_modes;
		ControlLineMode_t m_mode;
		volatile int16_t m_state[2];

		OneWire* m_pds;
		bool GetTemperature();
		bool GetPh();

	public:
		ControlLine(uint8_t pin, uint8_t address, uint8_t modes, ControlLineMode_t mode);

		uint8_t GetAddress();
		uint8_t GetModes();
		ControlLineMode_t GetMode();
		ControlLineInfo_t GetInfo();

		void QueryState();

		volatile int16_t* GetState();
		void SetState(int16_t* state);
};
//****************************************************************************************
#endif

