#include <OneWire.h>

#define DS18S20_ID				0x10
#define DS18B20_ID				0x28
#define DS1822_ID				0x22


OneWire ds(12);

void setup()
{

  /* add setup code here */

}

void loop()
{
	GetTemperature();
}

bool GetTemperature()
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

	//while (ds.search(addr))
	//{
	//	Serial.print("ROM: ");
	//	Serial.print(addr[0]);
	//	Serial.print(" ");
	//	Serial.print(addr[1]);
	//	Serial.print(" ");
	//	Serial.print(addr[2]);
	//	Serial.print(" ");
	//	Serial.print(addr[3]);
	//	Serial.print(" ");
	//	Serial.print(addr[4]);
	//	Serial.print(" ");
	//	Serial.print(addr[5]);
	//	Serial.print(" ");
	//	Serial.print(addr[6]);
	//	Serial.print(" ");
	//	Serial.print(addr[7]);
	//	Serial.println("");
	//}
	//Serial.println("---------------------");
	//return true;



	if (ds.search(addr)) // first device found
		ds.reset_search(); // reset search
	else // no devices
	{
		//Serial.println("No devices");
		return false;
	}
	if (OneWire::crc8(addr, 7) != addr[7])
	{
		Serial.println("Bad CRC");
		return false;
	}

	Serial.print("ROM: ");
	Serial.print(addr[0]);
	Serial.print(" ");
	Serial.print(addr[1]);
	Serial.print(" ");
	Serial.print(addr[2]);
	Serial.print(" ");
	Serial.print(addr[3]);
	Serial.print(" ");
	Serial.print(addr[4]);
	Serial.print(" ");
	Serial.print(addr[5]);
	Serial.print(" ");
	Serial.print(addr[6]);
	Serial.print(" ");
	Serial.print(addr[7]);
	Serial.println("");

	ds.reset();
	ds.select(addr);
	//ds.write(0x05, 0);
	uint8_t buf[2] = {0x06, 0x07};
	ds.write_bytes(buf, 2, 0);
	
	ds.reset();

	//for (i = 0; i < 2; i++)
	//	buf2[i] = ds.read();
	uint8_t buf2[2] = {50,50};
	ds.read_bytes(buf2, 2);

	Serial.print(buf2[0]);
	Serial.print(";");
	Serial.println(buf2[1]);


	delay(1000);
	return true;

	 
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

		//m_state[0] = -1000;
		//m_state[1] = 0;

		return false;
	}
	 
	ds.reset();

	ds.select(addr);
	ds.write(0x44, 1); // start conversion, with parasite power on at the end

	delay(1000); // wait for conversion; maybe 750ms is enough, maybe not

	present = ds.reset(); // we might do a ds.depower() here, but the reset will take care of it

	ds.select(addr);
	ds.write(0xBE); // Read scratchpad command

	// Receive 9 bytes
	for (i = 0; i < 9; i++)
		data[i] = ds.read();

	lowByte = data[0];
	highByte = data[1];



	//------------------------------------------------------------
	// Calculate temperature value
	temp = ((highByte << 8) + lowByte) * 0.0625;
	Serial.println(temp);
	//---------------------------------------------------------------
	//TReading = (highByte << 8) + lowByte;
	//SignBit = TReading & 0x8000;  // test most sig bit
	//if (SignBit) // negative
	//	TReading = (TReading ^ 0xffff) + 1; // 2's comp
	//Tc_100 = (6 * TReading) + TReading / 4;    // multiply by (100 * 0.0625) or 6.25

	//Whole = Tc_100 / 100;  // separate off the whole and fractional portions
	//Fract = Tc_100 % 100;

	//m_state[0] = SignBit ? -Whole : Whole;
	//m_state[1] = Fract < 10 ? 0 : Fract;
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

	delay(100);

	return true;
}
