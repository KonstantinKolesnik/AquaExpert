

#include	"LIS302DL.h"

void LIS302DL_init (void)
{
	i2c_start(I2C_LIS302DL + I2C_WRITE);
	i2c_write(0x21); // CTRL_REG2 (21h)
	i2c_write(0b00000000);
	i2c_stop();
	
	i2c_start(I2C_LIS302DL + I2C_WRITE);
	i2c_write(0x20); // CTRL_REG1 (20h)
	i2c_write(0b01000111);
	i2c_stop();

}

int LIS302DL_read (unsigned char axe )
{
	int axe_val;
	unsigned char tmp;

	i2c_start(I2C_LIS302DL + I2C_WRITE);
	i2c_write(statusReg);
	i2c_stop();
   
	i2c_start(I2C_LIS302DL + I2C_WRITE);
	i2c_write(axe);
	i2c_stop();
   
	i2c_start(I2C_LIS302DL + I2C_READ);
	tmp = i2c_readNak();
	i2c_stop();

	if (tmp > 127) //If axis value is negative,
	{
		axe_val = 256 - tmp;
		axe_val ^= 0xFFFF; //One's complement of axe_val.
	}   
   else
   	 axe_val = tmp;

	return (axe_val);

}