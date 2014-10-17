#include "BusModule.h"
//****************************************************************************************
BusModule::BusModule(ModuleType_t type)
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
		case Unknown:
			// Arduino UNO
			m_controlLinesCount = 14;
			m_pControlLines = (ControlLine*)malloc(m_controlLinesCount * sizeof(ControlLine));

			m_pControlLines[0] = ControlLine(2, 0, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[1] = ControlLine(3, 1, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[2] = ControlLine(4, 2, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[3] = ControlLine(5, 3, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[4] = ControlLine(6, 4, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[5] = ControlLine(7, 5, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[6] = ControlLine(8, 6, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[7] = ControlLine(A4, 7, DigitalInput | DigitalOutput | PWM, DigitalOutput);
			m_pControlLines[8] = ControlLine(A5, 8, OneWireBus, OneWireBus); // Temperature
			m_pControlLines[9] = ControlLine(A0, 9, DigitalInput | DigitalOutput | PWM | AnalogInput, AnalogInput); // Liquid
			m_pControlLines[10] = ControlLine(A1, 10, DigitalInput | DigitalOutput | PWM | AnalogInput, AnalogInput); // Liquid
			m_pControlLines[11] = ControlLine(A2, 11, DigitalInput | DigitalOutput | PWM | AnalogInput, AnalogInput); // Liquid
			m_pControlLines[12] = ControlLine(A3, 12, DigitalInput | DigitalOutput | PWM | AnalogInput, AnalogInput); // PH
			m_pControlLines[13] = ControlLine(A4, 13, DigitalInput | DigitalOutput | PWM | AnalogInput, AnalogInput); // ORP
			break;

		case D5:
			m_controlLinesCount = 5;
			m_pControlLines = (ControlLine*)malloc(m_controlLinesCount * sizeof(ControlLine));

			m_pControlLines[0] = ControlLine(2, 0, DigitalOutput, DigitalOutput);
			m_pControlLines[1] = ControlLine(3, 1, DigitalOutput, DigitalOutput);
			m_pControlLines[2] = ControlLine(4, 2, DigitalOutput, DigitalOutput);
			m_pControlLines[3] = ControlLine(5, 3, DigitalOutput, DigitalOutput);
			m_pControlLines[4] = ControlLine(6, 4, DigitalOutput, DigitalOutput);
			break;

		case D6:
			m_controlLinesCount = 6;
			m_pControlLines = (ControlLine*)malloc(m_controlLinesCount * sizeof(ControlLine));

			m_pControlLines[0] = ControlLine(2, 0, DigitalOutput, DigitalOutput);
			m_pControlLines[1] = ControlLine(3, 1, DigitalOutput, DigitalOutput);
			m_pControlLines[2] = ControlLine(4, 2, DigitalOutput, DigitalOutput);
			m_pControlLines[3] = ControlLine(5, 3, DigitalOutput, DigitalOutput);
			m_pControlLines[4] = ControlLine(6, 4, DigitalOutput, DigitalOutput);
			m_pControlLines[5] = ControlLine(7, 5, DigitalOutput, DigitalOutput);
			break;

		case D8:
			m_controlLinesCount = 8;
			m_pControlLines = (ControlLine*)malloc(m_controlLinesCount * sizeof(ControlLine));

			m_pControlLines[0] = ControlLine(2, 0, DigitalOutput, DigitalOutput);
			m_pControlLines[1] = ControlLine(3, 1, DigitalOutput, DigitalOutput);
			m_pControlLines[2] = ControlLine(4, 2, DigitalOutput, DigitalOutput);
			m_pControlLines[3] = ControlLine(5, 3, DigitalOutput, DigitalOutput);
			m_pControlLines[4] = ControlLine(6, 4, DigitalOutput, DigitalOutput);
			m_pControlLines[5] = ControlLine(7, 5, DigitalOutput, DigitalOutput);
			m_pControlLines[6] = ControlLine(8, 6, DigitalOutput, DigitalOutput);
			m_pControlLines[7] = ControlLine(A0, 7, DigitalOutput, DigitalOutput);
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

		Serial.print("; Mode=");
		Serial.print(line.GetMode());




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
//****************************************************************************************
