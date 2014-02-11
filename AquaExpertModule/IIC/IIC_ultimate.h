#ifndef IICULTIMATE_H
#define IICULTIMATE_H
//****************************************************************************************
#include "..\Hardware.h"
#include <avr/io.h>
#include <avr/interrupt.h>
//****************************************************************************************
#define TWI_PORT					PORTC	// ���� ��� ����� ���� TWI
#define TWI_DDR						DDRC
#define TWI_SCL						0	 // ���� ��������������� �������
#define TWI_SDA						1

#define TWI_SlaveAddress 			5	 // ����� �� ������� ����� ����������, 1...127

#define TWI_ACCEPT_BROADCAST		1	 // Accept broadcast messages
#define TWI_BUFFER_SIZE				20	 // Reserves memory for the drivers transceiver buffer. Set this to the largest message size that will be sent including address byte.

// TWI Miscellaneous status codes
#define TWI_NO_STATE               0xF8  // No relevant state information available; TWINT = �0�
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
#define TWI_STX_DATA_ACK_LAST_BYTE 0xC8  // Last data byte in TWDR has been transmitted (TWEA = �0�); ACK has been received

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
//****************************************************************************************
union TWI_statusReg_t                       // Status byte holding flags.
{
	uint8_t all;
	struct
	{
		bool lastTransOK:1;
		bool RxDataInBuf:1;
		bool genAddressCall:1;		// TRUE = General call, FALSE = TWI Address;
		unsigned char unusedBits:5;
	};
};

extern union TWI_statusReg_t TWI_statusReg;

void InitI2C();
bool TWI_TransceiverBusy();
uint8_t TWI_GetStateInfo();
void TWI_StartTransceiver();
void TWI_StartTransceiverWithData(unsigned char *msg, uint8_t msgSize);
bool TWI_GetDataFromTransceiver(unsigned char *msg, uint8_t msgSize);
//****************************************************************************************
#endif
