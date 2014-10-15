#include <EEPROM.h>
#include "ControlLine.h"
//****************************************************************************************
#define RELAY_ACTIVE_LEVEL		LOW	// 8-relay module active level is "0"

#define DS18S20_ID				0x10
#define DS18B20_ID				0x28
#define DS1822_ID				0x22
#define DS1820_ID				0x10//???

#define PH_OFFSET				0.0//-0.12
//****************************************************************************************
ControlLine::ControlLine(uint8_t pin, uint8_t address, uint8_t modes, ControlLineMode_t mode)
{
	m_pin = pin;
	m_address = address;
	m_modes = modes;
	m_mode = mode;

	m_state[0] = 0;
	m_state[1] = 0;

	int16_t initialState[2] = {EEPROM.read(EEPROM_OFFSET + m_address), 0};

	switch (m_mode)
	{
		case DigitalInput:
			pinMode(m_pin, INPUT_PULLUP);//INPUT
			break;
		case DigitalOutput:
			pinMode(m_pin, OUTPUT);
			SetState(initialState);
			break;
		case AnalogInput:
			analogRead(m_pin); // preread
			break;
		case PWM:
			SetState(initialState);
			break;
		case OneWireBus:
			m_pds = new OneWire(m_pin); // (a 4.7K resistor is necessary)
			m_state[0] = -1000;
			m_state[1] = 0;
			GetTemperature();
			break;
		//case Ph:
		//	analogRead(m_pin); // preread
		//	m_state[0] = -1000;
		//	m_state[1] = 0;
		//	break;



		default:
			break;
	}
}

uint8_t ControlLine::GetAddress()
{
	return m_address;
}
uint8_t ControlLine::GetModes()
{
	return m_modes;
}
ControlLineMode_t ControlLine::GetMode()
{
	return m_mode;
}
ControlLineInfo_t ControlLine::GetInfo()
{
	ControlLineInfo_t info;
	info.address = m_address;
	info.modes = m_modes;
	info.mode = m_mode;

	return info;
}

void ControlLine::QueryState()
{
	switch (m_mode)
	{
		case DigitalInput:
			m_state[0] = digitalRead(m_pin);
			break;
		//case DigitalOutput:
		//	m_state[0] = RELAY_ACTIVE_LEVEL ? digitalRead(m_pin) : !digitalRead(m_pin);
		//	break;
		case AnalogInput:
			// transistor: 524 for water;  838 for short circuit; (100/100/KT3102)
			// Yusupov:    ~650 for water; ~1000 for short circuit; ~1 for air; (2k / 100k)
			m_state[0] = analogRead(m_pin);
			break;
		//case PWM:
		//	m_state[0] = EEPROM.read(EEPROM_OFFSET + m_address);
		//	break;
		case OneWireBus:
			GetTemperature();
			break;
		//case Ph:
		//	GetPh();
		//	break;
		




		default:
			break;
	}
}
volatile int16_t* ControlLine::GetState()
{
	return m_state;
}
void ControlLine::SetState(int16_t* state)
{
	switch (m_mode)
	{
		case DigitalOutput:
			if (state[0] != HIGH && state[0] != LOW)
				state[0] = !RELAY_ACTIVE_LEVEL;
			digitalWrite(m_pin, state[0] ? RELAY_ACTIVE_LEVEL : !RELAY_ACTIVE_LEVEL);
			EEPROM.write(EEPROM_OFFSET + m_address, state[0]);
			break;
		case PWM:
			state[0] = constrain(state[0], 0, 100);
			analogWrite(m_pin, map(state[0], 0, 100, 0, 255));
			EEPROM.write(EEPROM_OFFSET + m_address, state[0]);
			break;
		default:
			break;
	}
}


bool ControlLine::GetTemperature()
{
	/*
	DS18B20 water-proof:
	red:	+5V;
	blue:	signal;
	black:	GND;
	*/

	byte i;
	byte present = 0;
	byte type_s;
	byte data[12];
	byte addr[8];
	int highByte, lowByte, TReading, SignBit, Tc_100, Whole, Fract;
	float temp;
	float celsius, fahrenheit;

	if (m_pds->search(addr)) // first device found
		m_pds->reset_search(); // reset search
	else // no devices
	{
		m_state[0] = -1000;
		m_state[1] = 0;

		return false;
	}

	if (OneWire::crc8(addr, 7) != addr[7])
	{
		m_state[0] = -1000;
		m_state[1] = 0;

		return false;
	}
	 
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
		//Serial.println("Device family is not recognized: 0x");
		//Serial.println(addr[0], HEX);

		m_state[0] = -1000;
		m_state[1] = 0;

		return false;
	}
	 
	m_pds->reset();
	m_pds->select(addr);
	m_pds->write(0x44, 1); // start conversion, with parasite power on at the end

	//delay(1000); // wait for conversion; maybe 750ms is enough, maybe not
	while (!m_pds->read()) { }

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

	m_state[0] = SignBit ? -Whole : Whole;
	m_state[1] = Fract < 10 ? 0 : Fract;
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

	//delay(100);

	return true;
}
bool ControlLine::GetPh()
{
	/*
	(1) Connect equipments according to the graphic, that is, the pH electrode is connected to the BNC connector on the pH meter board,
		and then use the connection lines, the pH meter board is connected to the ananlong port 0 of the Arduino controller.
		When the Arduino controller gets power, you will see the blue LED on board is on.
	(2) Upload the sample code to the Arduino controller.
	(3) Put the pH electrode into the standard solution whose pH value is 7.00, or directly shorten the input of the BNC connector.
		Open the serial monitor of the Arduino IDE, you can see the pH value printed on it, and the error does not exceed 0.3.
		Record the pH value printed, then compared with 7.00, and the difference should be assifned to the "Offset".
		For example, the pH value printed is 6.88, so the difference is 0.12. You should change the "# define Offset 0.00" into "# define Offset 0.12" in your program.
	(4) Put the pH electrode into the pH standard solution whose value is 4.00. Then wait about one minute, adjust the gain potential device,
		let the value stabilise at around 4.00. At this time the acidic calibration has been completed and you can measure the pH value of an acidic solution.
		Note: If you want to measure the pH value of other solution, you must wash the pH electrode first!
	(5) According to the linear characteristics of pH electrode itself, after the above calibration, you can directly measure the pH value of the alkaline solution,
		but if you want to get better accuracy, you can recalibrate it. Alkaline calibration use the standard solution whose pH value is 9.18.
		Also adjust the gain potential device, let the value stabilise at around 9.18. After this calibration, you can measure the pH value of the alkaline solution.
	*/

	// get 10 sample value from the sensor for smooth the value:
	int buf[10];
	for (int i = 0; i < 10; i++)
	{
		buf[i] = analogRead(m_pin);
		delay(10);
	}

	// sort the analog from small to large:
	for (int i = 0; i < 9; i++)
	{
		for (int j = i + 1; j < 10; j++)
		{
			if (buf[i] > buf[j])
			{
				int temp = buf[i];
				buf[i] = buf[j];
				buf[j] = temp;
			}
		}
	}
	
	// store the average value of the sensor feedback:
	unsigned long int avgValue = 0;
	for (int i = 2; i < 8; i++) // take the average value of 6 center sample
		avgValue += buf[i];
	float phValue = (float)(avgValue * 5.0 / 1024 / 6); // convert the analog into millivolt
	phValue = 3.5 * phValue + PH_OFFSET; // convert the millivolt into pH value

	//Serial.print("    pH:");
	//Serial.print(phValue,2);
	//Serial.println("");

	m_state[0] = (int)phValue;
	m_state[1] = (phValue - m_state[0]) * 100;
}
