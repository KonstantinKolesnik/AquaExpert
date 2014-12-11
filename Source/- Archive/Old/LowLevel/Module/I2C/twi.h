#ifndef TWI_H_
#define TWI_H_
//****************************************************************************************
#include "..\Hardware.h"
#include <inttypes.h>
//****************************************************************************************
#define TWI_PORT					PORTC	// Порт где сидит нога TWI
#define TWI_DDR						DDRC
#define TWI_SCL						0	 // Биты соответствующих выводов
#define TWI_SDA						1

#define TWI_SLAVE_ADDRESS 			5	 // Slave address, 1...127
#define TWI_ACCEPT_BROADCAST		1	 // Accept broadcast messages

#ifndef TWI_FREQ
#define TWI_FREQ 100000L // 400 kHz
#endif

#ifndef TWI_BUFFER_LENGTH
#define TWI_BUFFER_LENGTH 32
#endif

#define TWI_READY 0
#define TWI_MRX   1
#define TWI_MTX   2
#define TWI_SRX   3
#define TWI_STX   4
//****************************************************************************************
void twi_init(void);
void twi_setAddress(uint8_t);
uint8_t twi_readFrom(uint8_t, uint8_t*, uint8_t, uint8_t);
uint8_t twi_writeTo(uint8_t, uint8_t*, uint8_t, uint8_t, uint8_t);
uint8_t twi_transmit(const uint8_t*, uint8_t);
void twi_attachSlaveRxEvent( void (*)(uint8_t*, int) );
void twi_attachSlaveTxEvent( void (*)(void) );
void twi_reply(uint8_t);
void twi_stop(void);
void twi_releaseBus(void);
//****************************************************************************************
#endif