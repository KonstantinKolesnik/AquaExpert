
/*==================================================================================
	GY-26 digital compass library.
	(c) 13 September 2013 by Vassilis Serasidis
	Home: http://www.serasidis.gr
	e-mail: avrsite@yahoo.gr , info@serasidis.gr
	
	This library is distributed under GPL v3 licence.
====================================================================================*/



#include	"GY26.h"

/*=================================================================================
Reads the GY-26 digital compass and returns the degrees value in integer format.
Example:

unsigned int degrees;
degrees = gy26Read();

The value in 'degrees' variable could be from 0 to 3650.
If the 'degrees' variable has the value 2568 that means 256.8 degrees.
=================================================================================*/
int gy26Read (void)
{
		unsigned int data;
		
		i2c_start(I2C_GY26 + I2C_WRITE);// Set device address at write mode.	
		i2c_write(0x00);				// Write 0x00 and 0x31 to I2C data bus.
		i2c_write(0x31);				// These two bytes measure the angle (return the value of the angle).
		i2c_stop();

		i2c_start(I2C_GY26 + I2C_WRITE);// Set device address and write mode	
		i2c_write(0x01);	  
		i2c_stop();

		i2c_start(I2C_GY26 + I2C_READ);	// Set device address and read mode		
		data = (i2c_readAck() << 8);	// Read the Most Significant Byte (MSB) from the GY-26 compass. 
		data += i2c_readNak();			// Read the Low Significant Byte (LSB) from the GY-26 compass. 
		i2c_stop();
		
		return (data); //Return the degrees value (16-bit integer from 0-3650).
}

void gy26_init (void)
{
	i2c_init();
}	