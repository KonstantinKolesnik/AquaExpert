#ifndef _BUSMODULE_h
#define _BUSMODULE_h
//****************************************************************************************
#include "Hardware.h"
#include "ControlLine.h"
//****************************************************************************************
class BusModule
{
private:
	ModuleType_t m_type;
	ControlLine* m_pControlLines;
	uint16_t m_controlLinesCount;

	void InitControlLines();

public:
	BusModule(ModuleType_t type);
	
	ModuleType_t GetType();
	uint16_t GetControlLinesCount();
	ControlLine* GetControlLines();

	void QueryState();

	void PrintState();
	void PrintControlLineState(uint16_t idx);
};
//****************************************************************************************
#endif

