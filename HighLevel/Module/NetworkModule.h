#ifndef _MODULE_h
#define _MODULE_h
//****************************************************************************************
#include "Hardware.h"
#include "ControlLine.h"
//****************************************************************************************
class NetworkModule
{
private:
	ModuleType_t m_type;
	ControlLine* m_pControlLines;
	uint16_t m_controlLinesCount;

	unsigned char m_rom[8];
	OneWireSlave* m_pds;

public:
	NetworkModule();
	
	void Init(ModuleType_t type);

	ModuleType_t GetType();
	uint16_t GetControlLinesCount();
	ControlLine* GetControlLines();

	void UpdateState();

	void PrintState();
	void PrintControlLineState(uint16_t idx);
};

extern NetworkModule Module;
//****************************************************************************************
#endif

