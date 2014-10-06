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

public:
	NetworkModule();

	void Init(ModuleType_t type);
	void UpdateState();

	ControlLine* GetControlLines();
	uint16_t GetControlLinesCount();
	
	ModuleType_t GetType();
};

extern NetworkModule Module;
//****************************************************************************************
#endif

