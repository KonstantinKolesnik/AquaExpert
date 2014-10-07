#include "NetworkModule.h"
//****************************************************************************************
NetworkModule::NetworkModule()
{
	m_pControlLines = NULL;
	m_controlLinesCount = 0;
	analogReference(DEFAULT);
}

void NetworkModule::Init(ModuleType_t type)
{
	m_type = type;

	switch (m_type)
	{
	case Test:
		m_controlLinesCount = 14;
		m_pControlLines = (ControlLine*)malloc(m_controlLinesCount * sizeof(ControlLine));

		m_pControlLines[0] = ControlLine(Relay, 0, 2);
		m_pControlLines[1] = ControlLine(Relay, 1, 3);
		m_pControlLines[2] = ControlLine(Relay, 2, 4);
		m_pControlLines[3] = ControlLine(Relay, 3, 5);
		m_pControlLines[4] = ControlLine(Relay, 4, 6);
		m_pControlLines[5] = ControlLine(Relay, 5, 7);
		m_pControlLines[6] = ControlLine(Relay, 6, 8);
		m_pControlLines[7] = ControlLine(Relay, 7, 9);

		m_pControlLines[8] = ControlLine(Temperature, 0, 16);
		
		m_pControlLines[9] = ControlLine(Liquid, 0, 19);
		m_pControlLines[10] = ControlLine(Liquid, 1, 20);
		m_pControlLines[11] = ControlLine(Liquid, 2, 21);
		
		m_pControlLines[12] = ControlLine(Ph, 0, 18);
		
		m_pControlLines[13] = ControlLine(ORP, 0, 10);

		
		break;
	default:
		break;
	}

	// 15/14/16 - 1-Wire slave:
}

ModuleType_t NetworkModule::GetType()
{
	return m_type;
}
uint16_t NetworkModule::GetControlLinesCount()
{
	return m_controlLinesCount;
}
ControlLine* NetworkModule::GetControlLines()
{
	return m_pControlLines;
}

void NetworkModule::UpdateState()
{
	for (uint16_t i = 0; i < m_controlLinesCount; i++)
		m_pControlLines[i].UpdateState();
}

void NetworkModule::PrintState()
{
	for (uint16_t i = 0; i < m_controlLinesCount; i++)
		PrintControlLineState(i);
}
void NetworkModule::PrintControlLineState(uint16_t idx)
{
	if (m_controlLinesCount == 0)
		Serial.println("No control lines");
	else if (idx >= 0 && idx < m_controlLinesCount)
	{
		ControlLine line = m_pControlLines[idx];
		volatile int16_t* state = line.GetState();

		Serial.print("Line #");
		Serial.print(idx);

		Serial.print("; Address=");
		Serial.print(line.GetAddress());

		Serial.print("; Type=");
		Serial.print(line.GetType());

		Serial.print("; State=[");
		Serial.print(state[0]);
		Serial.print(", ");
		Serial.print(state[1]);
		Serial.println("];");
	}
}

NetworkModule Module;

