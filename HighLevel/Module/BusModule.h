#ifndef _MODULE_h
#define _MODULE_h
//****************************************************************************************
#include "Hardware.h"
#include "ControlLine.h"
//****************************************************************************************
class BusModule
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
	BusModule();
	
	void Init(ModuleType_t type);
	void LoopProc();

	ModuleType_t GetType();
	uint16_t GetControlLinesCount();
	ControlLine* GetControlLines();

	void QueryState();

	void PrintState();
	void PrintControlLineState(uint16_t idx);
};
//****************************************************************************************
extern BusModule module;
//****************************************************************************************
#endif

