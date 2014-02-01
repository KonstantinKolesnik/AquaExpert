
/*==================================================================================
	24cXX eeprom library.
	(c) 13 September 2013 by Vassilis Serasidis
	Home: http://www.serasidis.gr
	e-mail: avrsite@yahoo.gr , info@serasidis.gr
	
	This library is distributed under GPL v3 licence.
====================================================================================*/



#include	"24cXX.h"
#include <util/delay.h>

/*=================================================================================

=================================================================================*/
unsigned char eep24cXX_read_byte (unsigned int address)
{
		unsigned char data;
		unsigned char HighNibble, LowNibble;
			
		HighNibble = (address >> 8);
		LowNibble = (address & 0x00FF);	
		
		i2c_start(I2C_24cXX + I2C_WRITE);    // Set device address at write mode.	
		i2c_write(HighNibble);				// Write --- and --- to I2C data bus.
		i2c_write(LowNibble);		
				
		i2c_start(I2C_24cXX + I2C_READ);	// Set device address and read mode		
		data = i2c_readNak();			    // Read the data Byte from 24cXX eeprom. 
		
		i2c_stop();
		
		return (data); //Return the degrees value (16-bit integer from 0-3650).
}

/*=================================================================================

=================================================================================*/
void eep24cXX_write_byte (unsigned int address, unsigned char data)
{
	unsigned char HighNibble, LowNibble;
	
	HighNibble = ((address >> 8) & 0xff);
	LowNibble = (address & 0x00ff);
		
	i2c_start(I2C_24cXX + I2C_WRITE);    // Set device address at write mode.	
	i2c_write(HighNibble);				// Write --- and --- to I2C data bus.
	i2c_write(LowNibble);
	i2c_write(data);
	i2c_stop(); //Release I2C bus.
	_delay_ms(5); //Wait 5 ms.
}