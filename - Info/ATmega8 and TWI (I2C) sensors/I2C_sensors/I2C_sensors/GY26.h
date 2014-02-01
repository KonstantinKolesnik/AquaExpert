
/*
	GY-26 digital compass library.
	(c) 13 September 2013 by Vassilis Serasidis
	Home: http://www.serasidis.gr
	e-mail: avrsite@yahoo.gr , info@serasidis.gr
	
	This library is distributed under GPL v3 licence.
*/


#define		I2C_GY26	0xE0	// GY26 compass default I2C address.
#include	"i2cmaster.h"


/*=================================================================================
Reads the GY-26 digital compass and returns the degrees value in integer format.
Example:

unsigned int degrees;
degrees = gy26Read();

The value in 'degrees' variable could be from 0 to 3600.
If the 'degrees' variable has the value 2568 that means 256.8 degrees.
=================================================================================*/
int gy26Read (void);
void gy26_init (void);