/*=============================================================
Libray fuctions to access the popular DS1307 RTC IC with AVR
Microcontroller.

=============================================================*/

#include <avr/io.h>
#include <util/delay.h>
#include "i2cmaster.h"
#include "DS1307.h"

//======================================================================
//
//
//======================================================================
void DS1307Read(unsigned char address,unsigned char *data)
{
	
	i2c_start(DS1307_ADDR + I2C_WRITE);
	i2c_write(address);
	i2c_start(DS1307_ADDR + I2C_READ);
	*data=i2c_readNak();
	i2c_stop();
	_delay_ms(5);
}

//======================================================================
//
//
//======================================================================
void DS1307ReadASCII(char dateTime[21])
{
	unsigned char axe_value, clockData;
	DS1307Read(0x00,&clockData); //seconds.
	dateTime[20]='\0';
	dateTime[19]=48+(clockData & 0b00001111);
	dateTime[18]=48+((clockData & 0b01110000)>>4);
	dateTime[17]=':';
	
	DS1307Read(0x01,&clockData);	//minutes
	dateTime[16]=48+(clockData & 0b00001111);
	dateTime[15]=48+((clockData & 0b01110000)>>4);
	dateTime[14]=':';
	
	DS1307Read(0x02,&clockData);	//hours
	dateTime[13]=48+(clockData & 0b00001111);
	dateTime[12]=48+((clockData & 0b01110000)>>4);
	dateTime[11]=' ';
		
	DS1307Read(0x06,&clockData);	//year
	dateTime[10]=48+(clockData & 0b00001111);
	dateTime[9]=48+((clockData & 0b11110000)>>4);
	dateTime[8]='/';
		
	DS1307Read(0x05,&clockData);	//month
	dateTime[7]=48+(clockData & 0b00001111);
	dateTime[6]=48+((clockData & 0b00010000)>>4);
	dateTime[5]='/';
		
	DS1307Read(0x04,&clockData);	//day
	dateTime[4]=48+(clockData & 0b00000111);
	dateTime[3]=48+((clockData & 0b00110000)>>4);
	dateTime[2]=' ';
		
	DS1307Read(0x03,&clockData);	
	dateTime[1]=48+(clockData & 0b00001111); //day of the week (0-6 | 0 = Sunday)
	dateTime[0]=48;
}		
//======================================================================
//
//
//======================================================================
void DS1307Write(unsigned char address,unsigned char data)
{
	i2c_start(DS1307_ADDR + I2C_WRITE);
	i2c_write(address);
	i2c_write(data);
	i2c_stop();
	_delay_ms(5);
}
	
	

