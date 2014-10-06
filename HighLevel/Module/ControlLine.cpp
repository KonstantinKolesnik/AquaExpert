#include "ControlLine.h"
//****************************************************************************************
#define RELAY_ACTIVE_LEVEL		LOW	// 8-relay module active level is "0"
#define DS18S20_ID				0x10
#define DS18B20_ID				0x28
#define DS1822_ID				0x22
//****************************************************************************************
ControlLine::ControlLine(ControlLineType_t type, uint8_t address, uint8_t pin)
{
	m_type = type;
	m_address = address;
	m_pin = pin;
	m_state[0] = 0;
	m_state[1] = 0;

	switch (m_type)
	{
	case Relay:
		pinMode(m_pin, OUTPUT);
		break;
	case Temperature:
		m_pds = new OneWire(m_pin); // (a 4.7K resistor is necessary)
		break;





	default:
		break;
	}
}

volatile uint8_t* ControlLine::GetState()
{
	return m_state;
}
void ControlLine::SetState(uint8_t* state)
{
	switch (m_type)
	{
	case Relay:
		digitalWrite(m_pin, state[0] ? RELAY_ACTIVE_LEVEL : !RELAY_ACTIVE_LEVEL);
		break;
	case PWM:
		break;
	default:
		break;
	}
}
void ControlLine::UpdateState()
{
	int level;

	switch (m_type)
	{
	case Relay:
		m_state[0] = RELAY_ACTIVE_LEVEL ? digitalRead(m_pin) : !digitalRead(m_pin);
		break;
	case Temperature:
		GetTemperature();
		break;




	default:
		break;
	}
}

bool ControlLine::GetTemperature()
{
	byte i;
	byte present = 0;
	byte type_s;
	byte data[12];
	byte addr[8];
	int highByte, lowByte, TReading, SignBit, Tc_100, Whole, Fract;
	float temp;
	float celsius, fahrenheit;

	//find a device
	if (!m_pds->search(addr))
	{
		m_pds->reset_search();
		return false;
	}
	if (OneWire::crc8(addr, 7) != addr[7])
		return false;
	 
	if (addr[0] == DS18S20_ID)
	{
		//Serial.println("DS18S20 family device.\n");
		type_s = 1;
	}
	else if (addr[0] == DS18B20_ID)
	{
		//Serial.println("DS18B20 family device.\n");
		type_s = 0;
	}
	else if (addr[0] == DS1822_ID)
	{
		//Serial.println("DS1822 family device.\n");
		type_s = 0;
	}
	else
	{
		//Serial.print("Device family is not recognized: 0x");
		//Serial.println(addr[0], HEX);
		return false;
	}
	 
	m_pds->reset();

	m_pds->select(addr);
	m_pds->write(0x44, 1); // start conversion, with parasite power on at the end

	delay(1000); // wait for conversion; maybe 750ms is enough, maybe not

	present = m_pds->reset(); // we might do a ds.depower() here, but the reset will take care of it

	m_pds->select(addr);
	m_pds->write(0xBE); // Read scratchpad command

	// Receive 9 bytes
	for (i = 0; i < 9; i++)
		data[i] = m_pds->read();

	lowByte = data[0];
	highByte = data[1];



	//------------------------------------------------------------
	// Calculate temperature value
	//temp = ((highByte << 8) + lowByte) * 0.0625;
	//Serial.println(temp);
	//---------------------------------------------------------------
	TReading = (highByte << 8) + lowByte;
	SignBit = TReading & 0x8000;  // test most sig bit
	if (SignBit) // negative
		TReading = (TReading ^ 0xffff) + 1; // 2's comp
	Tc_100 = (6 * TReading) + TReading / 4;    // multiply by (100 * 0.0625) or 6.25

	Whole = Tc_100 / 100;  // separate off the whole and fractional portions
	Fract = Tc_100 % 100;

	m_state[0] = Whole;
	m_state[1] = Fract;

	//if (SignBit) // If its negative
	//	Serial.print("-");
	//Serial.print(Whole);
	//Serial.print(".");
	//if (Fract < 10)
	//	Serial.print("0");
	//Serial.print(Fract);
	//Serial.println("\n");
	//--------------------------------------------------------------------------
	// Convert the data to actual temperature
	// because the result is a 16 bit signed integer, it should
	// be stored to an “int16_t” type, which is always 16 bits
	// even when compiled on a 32 bit processor.

	//int16_t raw = (highByte << 8) | lowByte;
	//if (type_s)
	//{
	//	raw = raw << 3; // 9 bit resolution default
	//	if (data[7] == 0x10)
	//	{
	//		// “count remain” gives full 12 bit resolution
	//		raw = (raw & 0xFFF0) + 12 - data[6];
	//	}
	//}
	//else
	//{
	//	byte cfg = (data[4] & 0x60);
	//	// at lower res, the low bits are undefined, so let’s zero them
	//	if (cfg == 0x00)
	//		raw = raw & ~7; // 9 bit resolution, 93.75 ms
	//	else
	//		if (cfg == 0x20)
	//			raw = raw & ~3; // 10 bit res, 187.5 ms
	//		else if (cfg == 0x40)
	//			raw = raw & ~1; // 11 bit res, 375 ms
	//	// default is 12 bit resolution, 750 ms conversion time
	//}
	//celsius = (float)raw / 16.0;
	//fahrenheit = celsius * 1.8 + 32.0;
	//Serial.print("Temperature = ");
	//Serial.print(celsius);
	//Serial.print(" Celsius, ");
	//Serial.print(fahrenheit);
	//Serial.println(" Fahrenheit");

	return true;
}