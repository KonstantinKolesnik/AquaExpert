#include "BusModule.h"
//****************************************************************************************
BusModule::BusModule(ModuleType_t type)
	: OneWireSlave((uint8_t)type)
{
	m_type = type;
	m_controlLinesCount = 0;
	m_pControlLines = NULL;
	analogReference(DEFAULT);

	InitControlLines();
}

void BusModule::InitControlLines()
{
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

			m_pControlLines[8] = ControlLine(Temperature, 8, 16);
		
			m_pControlLines[9] = ControlLine(Liquid, 9, A3);
			m_pControlLines[10] = ControlLine(Liquid, 10, A2);
			m_pControlLines[11] = ControlLine(Liquid, 11, A1);
		
			m_pControlLines[12] = ControlLine(Ph, 12, A0);
		
			m_pControlLines[13] = ControlLine(ORP, 13, 10);

			break;



		default:
			break;
	}
}

ModuleType_t BusModule::GetType()
{
	return m_type;
}
uint16_t BusModule::GetControlLinesCount()
{
	return m_controlLinesCount;
}
ControlLine* BusModule::GetControlLines()
{
	return m_pControlLines;
}

void BusModule::QueryState()
{
	for (uint16_t i = 0; i < m_controlLinesCount; i++)
		m_pControlLines[i].QueryState();
}

void BusModule::PrintState()
{
	//Serial.print("Type: ");
	//Serial.println(m_type);

	//Serial.print("CRC8 to be: ");
	//Serial.println(m_pds->crc8(m_rom, 7));

	//Serial.print("ROM: ");
	//Serial.print((uint8_t)m_rom[0]);
	//Serial.print(" ");
	//Serial.print((uint8_t)m_rom[1]);
	//Serial.print(" ");
	//Serial.print((uint8_t)m_rom[2]);
	//Serial.print(" ");
	//Serial.print((uint8_t)m_rom[3]);
	//Serial.print(" ");
	//Serial.print((uint8_t)m_rom[4]);
	//Serial.print(" ");
	//Serial.print((uint8_t)m_rom[5]);
	//Serial.print(" ");
	//Serial.print((uint8_t)m_rom[6]);
	//Serial.print(" ");
	//Serial.print((uint8_t)m_rom[7]);
	//Serial.println("");
	//Serial.println("");


	for (uint16_t i = 0; i < m_controlLinesCount; i++)
		PrintControlLineState(i);

	Serial.println("");
}
void BusModule::PrintControlLineState(uint16_t idx)
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




		//Serial.print("; State=[");
		//Serial.print(state[0]);
		//Serial.print(", ");
		//Serial.print(state[1]);
		//Serial.println("];");

		Serial.print("; State=");
		Serial.print(state[0]);
		Serial.print(".");
		Serial.print(state[1]);
		Serial.println(";");
	}
}

bool BusModule::OnDuty(OneWireSlaveManager* hub)
{
	//QueryState();

	Serial.print("OnDuty: ");

	uint8_t b = hub->recv();
	Serial.println(b);

	//uint8_t b[2];
	//hub->recvData(b, 2);
	//Serial.print(b[0]);
	//Serial.print(";");
	//Serial.println(b[1]);

	//uint8_t buf[2] = {0x08, 0x09};
	//hub->sendData(buf, 2);
	//if (hub->errno != ONEWIRE_NO_ERROR)
	//	return FALSE;  

	return TRUE;
}
//****************************************************************************************
