

#define	I2C_LIS302DL	0x3A	// LIS302DL accelerometer default I2C address.
#define outX			0x29
#define outY			0x2B
#define outZ			0x2D
#define statusReg		0x27

#include	"i2cmaster.h"
  
void LIS302DL_init (void);
int LIS302DL_read (unsigned char axe);

