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
	char m_rom[8];
	ControlLine* m_pControlLines;
	uint16_t m_controlLinesCount;

	OneWireSlave* m_pds;

	void InitROM();
	void InitControlLines();

public:
	NetworkModule();
	
	void Init(ModuleType_t type);
	void LoopProc();

	ModuleType_t GetType();
	uint16_t GetControlLinesCount();
	ControlLine* GetControlLines();

	void UpdateState();

	void PrintState();
	void PrintControlLineState(uint16_t idx);
};

extern NetworkModule module;
//****************************************************************************************
#endif

