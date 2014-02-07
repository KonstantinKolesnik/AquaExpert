#ifndef IICULTIMATE_H
#define IICULTIMATE_H

#include "..\Hardware.h"
#include <avr/io.h>
#include <avr/interrupt.h>

#define i2c_PORT	PORTC			// Порт где сидит нога TWI
#define i2c_DDR		DDRC
#define i2c_SCL		0				// Биты соответствующих выводов
#define i2c_SDA		1

#define i2c_MasterAddress 	5		// Адрес на который будем отзываться, 1...127
//#define i2c_i_am_slave		1		// Если мы еще и слейвом работаем то 1. А то не услышит!
//
//#define i2c_MasterBytesRX	1		// Величина принимающего буфера режима Slave, т.е. сколько байт жрем.
//#define i2c_MasterBytesTX	1		// Величина Передающего буфера режима Slave, т.е. сколько байт отдаем за сессию.
//
//#define i2c_MaxBuffer		3		// Величина буфера Master режима RX-TX. Зависит от того какой длины строки мы будем гонять
//#define i2c_MaxPageAddrLgth	2		// Максимальная величина адреса страницы. Обычно адрес страницы это один или два байта. Зависит от типа ЕЕПРОМ или другой микросхемы.
//
//#define i2c_type_msk	0b00001100	// Маска режима
//
//#define i2c_sarp	0b00000000		// Start-Addr_R-Read-Stop  							Это режим простого чтения. Например из слейва или из епрома с текущего адреса
//#define i2c_sawp	0b00000100		// Start-Addr_W-Write-Stop 							Это режим простой записи. В том числе и запись с адресом страницы.
//#define i2c_sawsarp	0b00001000		// Start-Addr_W-WrPageAdr-rStart-Addr_R-Read-Stop 	Это режим с предварительной записью текущего адреса страницы
//
//#define i2c_Err_msk	0b00110011		// Маска кода ошибок
//#define i2c_Err_NO	0b00000000		// All Right! 						-- Все окей, передача успешна.
//#define i2c_ERR_NA	0b00010000		// Device No Answer 				-- Слейв не отвечает. Т.к. либо занят, либо его нет на линии.
//#define i2c_ERR_LP	0b00100000		// Low Priority 					-- нас перехватили собственным адресом, либо мы проиграли арбитраж
//#define i2c_ERR_NK	0b00000010		// Received NACK. End Transmittion. -- Был получен NACK. Бывает и так.
//#define i2c_ERR_BF	0b00000001		// BUS FAIL 						-- Автобус сломался. И этим все сказано. Можно попробовать сделать переинициализацию шины
//
//#define i2c_Interrupted		0b10000000		// Transmiting Interrupted		Битмаска установки флага занятости
//#define i2c_NoInterrupted 	0b01111111  	// Transmiting No Interrupted	Битмаска снятия флага занятости
//
//#define i2c_Busy		0b01000000  		// Trans is Busy				Битмаска флага "Передатчик занят, руками не трогать".
//#define i2c_Free		0b10111111  		// Trans is Free				Битмаска снятия флага занятости.
//
//#define MACRO_i2c_WhatDo_MasterOut 	(MasterOutFunc)();		// Макросы для режима выхода. Пока тут функция, но может быть что угодно
//#define MACRO_i2c_WhatDo_SlaveOut   (SlaveOutFunc)();
//#define MACRO_i2c_WhatDo_ErrorOut   (ErrorOutFunc)();
//
//typedef void (*IIC_F)(void);		// Тип указателя на функцию
//
//extern IIC_F MasterOutFunc;			// Подрбрости в сишнике.
//extern IIC_F SlaveOutFunc;
//extern IIC_F ErrorOutFunc;
//
//extern uint8_t i2c_Do;
//extern uint8_t i2c_InBuff[i2c_MasterBytesRX];
//extern uint8_t i2c_OutBuff[i2c_MasterBytesTX];
//
//extern uint8_t i2c_SlaveIndex;
//
//extern uint8_t i2c_Buffer[i2c_MaxBuffer];
//extern uint8_t i2c_index;
//extern uint8_t i2c_ByteCount;
//
//extern uint8_t i2c_SlaveAddress;
//extern uint8_t i2c_PageAddress[i2c_MaxPageAddrLgth];
//
//extern uint8_t i2c_PageAddrIndex;
//extern uint8_t i2c_PageAddrCount;

extern void Init_i2c();
extern void Init_i2c_Slave();//IIC_F Addr);

//------------------------------------------------------------------------------------------------------------------------------------
#define ACCEPT_BROADCAST			1	 // Accept broadcast messages
#define TWI_BUFFER_SIZE				4	 // Reserves memory for the drivers transceiver buffer. Set this to the largest message size that will be sent including address byte.

// TWI Miscellaneous status codes
#define TWI_NO_STATE               0xF8  // No relevant state information available; TWINT = “0”
#define TWI_BUS_ERROR              0x00  // Bus error due to an illegal START or STOP condition

// General TWI Master status codes
#define TWI_START                  0x08  // START has been transmitted
#define TWI_REP_START              0x10  // Repeated START has been transmitted
#define TWI_ARB_LOST               0x38  // Arbitration lost

// TWI Master Transmitter status codes
#define TWI_MTX_ADR_ACK            0x18  // SLA+W has been transmitted and ACK received
#define TWI_MTX_ADR_NACK           0x20  // SLA+W has been transmitted and NACK received
#define TWI_MTX_DATA_ACK           0x28  // Data byte has been transmitted and ACK received
#define TWI_MTX_DATA_NACK          0x30  // Data byte has been transmitted and NACK received

// TWI Master Receiver status codes
#define TWI_MRX_ADR_ACK            0x40  // SLA+R has been transmitted and ACK received
#define TWI_MRX_ADR_NACK           0x48  // SLA+R has been transmitted and NACK received
#define TWI_MRX_DATA_ACK           0x50  // Data byte has been received and ACK transmitted
#define TWI_MRX_DATA_NACK          0x58  // Data byte has been received and NACK transmitted

// TWI Slave Transmitter status codes
#define TWI_STX_ADR_ACK            0xA8  // Own SLA+R has been received; ACK has been returned
#define TWI_STX_ADR_ACK_M_ARB_LOST 0xB0  // Arbitration lost in SLA+R/W as Master; own SLA+R has been received; ACK has been returned
#define TWI_STX_DATA_ACK           0xB8  // Data byte in TWDR has been transmitted; ACK has been received
#define TWI_STX_DATA_NACK          0xC0  // Data byte in TWDR has been transmitted; NOT ACK has been received
#define TWI_STX_DATA_ACK_LAST_BYTE 0xC8  // Last data byte in TWDR has been transmitted (TWEA = “0”); ACK has been received

// TWI Slave Receiver status codes
#define TWI_SRX_ADR_ACK            0x60  // Own SLA+W has been received ACK has been returned
#define TWI_SRX_ADR_ACK_M_ARB_LOST 0x68  // Arbitration lost in SLA+R/W as Master; own SLA+W has been received; ACK has been returned
#define TWI_SRX_GEN_ACK            0x70  // General call address has been received; ACK has been returned
#define TWI_SRX_GEN_ACK_M_ARB_LOST 0x78  // Arbitration lost in SLA+R/W as Master; General call address has been received; ACK has been returned
#define TWI_SRX_ADR_DATA_ACK       0x80  // Previously addressed with own SLA+W; data has been received; ACK has been returned
#define TWI_SRX_ADR_DATA_NACK      0x88  // Previously addressed with own SLA+W; data has been received; NOT ACK has been returned
#define TWI_SRX_GEN_DATA_ACK       0x90  // Previously addressed with general call; data has been received; ACK has been returned
#define TWI_SRX_GEN_DATA_NACK      0x98  // Previously addressed with general call; data has been received; NOT ACK has been returned
#define TWI_SRX_STOP_RESTART       0xA0  // A STOP condition or repeated START condition has been received while still addressed as Slave

union TWI_statusReg_t                       // Status byte holding flags.
{
	unsigned char all;
	struct
	{
		unsigned char lastTransOK:1;
		unsigned char RxDataInBuf:1;
		unsigned char genAddressCall:1;		// TRUE = General call, FALSE = TWI Address;
		unsigned char unusedBits:5;
	};
};

extern union TWI_statusReg_t TWI_statusReg;

unsigned char TWI_TransceiverBusy();
unsigned char TWI_GetStateInfo();
void TWI_StartTransceiver();
void TWI_StartTransceiverWith_Data(unsigned char *msg, uint8_t msgSize);
bool TWI_GetDataFromTransceiver(unsigned char *msg, uint8_t msgSize);

#endif
