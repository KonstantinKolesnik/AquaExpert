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
		uint8_t m_idx;
		uint8_t m_modes;
		ControlLineMode_t m_mode;
		double m_state; // 4 bytes

		OneWire* m_pds;
		bool GetTemperature();
		bool GetPh();

	public:
		ControlLine(uint8_t pin, uint8_t idx, uint8_t modes, ControlLineMode_t mode);

		ControlLineInfo_t GetInfo();

		ControlLineMode_t GetMode();
		void SetMode(ControlLineMode_t mode);

		void QueryState();

		double GetState();
		void SetState(double state);
};
//****************************************************************************************
#endif

