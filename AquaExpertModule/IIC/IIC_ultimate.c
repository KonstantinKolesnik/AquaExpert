#include "IIC_ultimate.h"

void DoNothing(void);

uint8_t i2c_Do;									// ���������� ��������� ����������� IIC
uint8_t i2c_InBuff[i2c_MasterBytesRX];			// ����� ������ ��� ������ ��� Slave
uint8_t i2c_OutBuff[i2c_MasterBytesTX];			// ����� �������� ��� ������ ��� Slave
uint8_t i2c_SlaveIndex;							// ������ ������ Slave

uint8_t i2c_Buffer[i2c_MaxBuffer];				// ����� ��� ������ ������ � ������ Master
uint8_t i2c_index;								// ������ ����� ������

uint8_t i2c_ByteCount;							// ����� ���� ������������

uint8_t i2c_SlaveAddress;						// ����� ������������

uint8_t i2c_PageAddress[i2c_MaxPageAddrLgth];	// ����� ������ ������� (��� ������ � sawsarp)
uint8_t i2c_PageAddrIndex;						// ������ ������ ������ �������
uint8_t i2c_PageAddrCount;						// ����� ���� � ������ �������� ��� �������� Slave

// ��������� ������ �� ��������:
IIC_F MasterOutFunc = &DoNothing;			//  � Master ������
IIC_F SlaveOutFunc 	= &DoNothing;			//  � ������ Slave
IIC_F ErrorOutFunc 	= &DoNothing;			//  � ���������� ������ � ������ Master

uint8_t 	WorkLog[100];						// ��� ����� ����
uint8_t		WorkIndex = 0;						// ������ ����


//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

// This is true when the TWI is in the middle of a transfer
// and set to false when all bytes have been transmitted/received
// Also used to determine how deep we can sleep.
static unsigned char TWI_busy = 0;
union TWI_statusReg_t TWI_statusReg = {0};           // TWI_statusReg is defined in TWI_Slave.h
static unsigned char TWI_buf[TWI_BUFFER_SIZE];     // Transceiver buffer. Set the size in the header file
static uint8_t TWI_msgSize  = 0;             // Number of bytes to be transmitted.
static unsigned char TWI_state    = TWI_NO_STATE;  // State byte. Default set to TWI_NO_STATE.

ISR(TWI_vect)	// TWI interrupt
{
	LED_MSG_ON;
	_delay_ms(5);
	
	static uint8_t TWI_bufPtr;

	switch(TWSR & 0xF8)	// �������� ���� ����������
	{
		//case TWI_STX_ADR_ACK:            // Own SLA+R has been received; ACK has been returned
		////    case TWI_STX_ADR_ACK_M_ARB_LOST: // Arbitration lost in SLA+R/W as Master; own SLA+R has been received; ACK has been returned
		//TWI_bufPtr   = 0;                                 // Set buffer pointer to first data location
		//case TWI_STX_DATA_ACK:           // Data byte in TWDR has been transmitted; ACK has been received
		//TWDR = TWI_buf[TWI_bufPtr++];
		//TWCR = (1<<TWEN)|                                 // TWI Interface enabled
		//(1<<TWIE)|(1<<TWINT)|                      // Enable TWI Interupt and clear the flag to send byte
		//(1<<TWEA)|(0<<TWSTA)|(0<<TWSTO)|           //
		//(0<<TWWC);                                 //
		//TWI_busy = 1;
		//break;
		//case TWI_STX_DATA_NACK:          // Data byte in TWDR has been transmitted; NACK has been received.
		//// I.e. this could be the end of the transmission.
		//if (TWI_bufPtr == TWI_msgSize) // Have we transceived all expected data?
		//{
			//TWI_statusReg.lastTransOK = TRUE;               // Set status bits to completed successfully.
		//}
		//else                          // Master has sent a NACK before all data where sent.
		//{
			//TWI_state = TWSR;                               // Store TWI State as errormessage.
		//}
    //
		//TWCR = (1<<TWEN)|                                 // Enable TWI-interface and release TWI pins
		//(1<<TWIE)|(1<<TWINT)|                      // Keep interrupt enabled and clear the flag
		//(1<<TWEA)|(0<<TWSTA)|(0<<TWSTO)|           // Answer on next address match
		//(0<<TWWC);                                 //
    //
		//TWI_busy = 0;   // Transmit is finished, we are not busy anymore
		//break;
		//case TWI_SRX_GEN_ACK:            // General call address has been received; ACK has been returned
		////    case TWI_SRX_GEN_ACK_M_ARB_LOST: // Arbitration lost in SLA+R/W as Master; General call address has been received; ACK has been returned
		//TWI_statusReg.genAddressCall = TRUE;
		//case TWI_SRX_ADR_ACK:            // Own SLA+W has been received ACK has been returned
		////    case TWI_SRX_ADR_ACK_M_ARB_LOST: // Arbitration lost in SLA+R/W as Master; own SLA+W has been received; ACK has been returned
		//// Dont need to clear TWI_S_statusRegister.generalAddressCall due to that it is the default state.
		//TWI_statusReg.RxDataInBuf = TRUE;
		//TWI_bufPtr   = 0;                                 // Set buffer pointer to first data location
    //
		//// Reset the TWI Interupt to wait for a new event.
		//TWCR = (1<<TWEN)|                                 // TWI Interface enabled
		//(1<<TWIE)|(1<<TWINT)|                      // Enable TWI Interupt and clear the flag to send byte
		//(1<<TWEA)|(0<<TWSTA)|(0<<TWSTO)|           // Expect ACK on this transmission
		//(0<<TWWC);
		//TWI_busy = 1;
    //
		//break;
		//case TWI_SRX_ADR_DATA_ACK:       // Previously addressed with own SLA+W; data has been received; ACK has been returned
		//case TWI_SRX_GEN_DATA_ACK:       // Previously addressed with general call; data has been received; ACK has been returned
		//TWI_buf[TWI_bufPtr++]     = TWDR;
		//TWI_statusReg.lastTransOK = TRUE;                 // Set flag transmission successfull.
		//// Reset the TWI Interupt to wait for a new event.
		//TWCR = (1<<TWEN)|                                 // TWI Interface enabled
		//(1<<TWIE)|(1<<TWINT)|                      // Enable TWI Interupt and clear the flag to send byte
		//(1<<TWEA)|(0<<TWSTA)|(0<<TWSTO)|           // Send ACK after next reception
		//(0<<TWWC);                                 //
		//TWI_busy = 1;
		//break;
		//case TWI_SRX_STOP_RESTART:       // A STOP condition or repeated START condition has been received while still addressed as Slave
		//// Enter not addressed mode and listen to address match
		//TWCR = (1<<TWEN)|                                 // Enable TWI-interface and release TWI pins
		//(1<<TWIE)|(1<<TWINT)|                      // Enable interrupt and clear the flag
		//(1<<TWEA)|(0<<TWSTA)|(0<<TWSTO)|           // Wait for new address match
		//(0<<TWWC);                                 //
    //
		//TWI_busy = 0;  // We are waiting for a new address match, so we are not busy
    //
		//break;
		//case TWI_SRX_ADR_DATA_NACK:      // Previously addressed with own SLA+W; data has been received; NOT ACK has been returned
		//case TWI_SRX_GEN_DATA_NACK:      // Previously addressed with general call; data has been received; NOT ACK has been returned
		//case TWI_STX_DATA_ACK_LAST_BYTE: // Last data byte in TWDR has been transmitted (TWEA = �0�); ACK has been received
		////    case TWI_NO_STATE              // No relevant state information available; TWINT = �0�
		//case TWI_BUS_ERROR:         // Bus error due to an illegal START or STOP condition
		//TWI_state = TWSR;                 //Store TWI State as errormessage, operation also clears noErrors bit
		//TWCR =   (1<<TWSTO)|(1<<TWINT);   //Recover from TWI_BUS_ERROR, this will release the SDA and SCL pins thus enabling other devices to use the bus
		//break;
		//default:
		//TWI_state = TWSR;                                 // Store TWI State as errormessage, operation also clears the Success bit.
		//TWCR = (1<<TWEN)|                                 // Enable TWI-interface and release TWI pins
		//(1<<TWIE)|(1<<TWINT)|                      // Keep interrupt enabled and clear the flag
		//(1<<TWEA)|(0<<TWSTA)|(0<<TWSTO)|           // Acknowledge on any new requests.
		//(0<<TWWC);                                 //
    //
		//TWI_busy = 0; // Unknown status, so we wait for a new address match that might be something we can handle
		//
		
		
		
		
		
		
		
		
		
		
		/*
		case TWI_BUS_ERROR:	// Bus Fail
			i2c_Do |= i2c_ERR_BF;
			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;
			
			MACRO_i2c_WhatDo_ErrorOut
			break;

		case TWI_START:	// Start sent
			if ((i2c_Do & i2c_type_msk) == i2c_sarp)		// � ����������� �� ������
				i2c_SlaveAddress |= 0x01;					// ���� Addr+R
			else											// ���
				i2c_SlaveAddress &= 0xFE;					// ���� Addr+W
			
			TWDR = i2c_SlaveAddress;						// ����� ������
			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;
			break;

		case TWI_REP_START:	// ��������� ����� ���
			if ((i2c_Do & i2c_type_msk) == i2c_sawsarp)		// � ����������� �� ������
				i2c_SlaveAddress |= 0x01;					// ���� Addr+R
			else
				i2c_SlaveAddress &= 0xFE;					// ���� Addr+W
						
			// To Do: �������� ���� ��������� ������
			TWDR = i2c_SlaveAddress;				// ����� ������
			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;  	// Go!
			break;

		case TWI_MTX_ADR_ACK:	// ��� ������ SLA+W, �������� ACK
			if ((i2c_Do & i2c_type_msk) == i2c_sawp)		// � ����������� �� ������
			{
				TWDR = i2c_Buffer[i2c_index];				// ���� ���� ������
				i2c_index++;							// ����������� ��������� ������
			}

			if ((i2c_Do & i2c_type_msk) == i2c_sawsarp)
			{
				TWDR = i2c_PageAddress[i2c_PageAddrIndex];	// ��� ���� ����� ������� (�� ���� ���� ���� ������)
				i2c_PageAddrIndex++;						// ����������� ��������� ������ ��������
			}
			
			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;	// Go!
			
			break;
		case TWI_MTX_ADR_NACK:	// ��� ������ SLA+W �������� NACK - ����� ���� �����, ���� ��� ���
			i2c_Do |= i2c_ERR_NA;	// ��� ������

			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;	// ���� ���� Stop

			MACRO_i2c_WhatDo_ErrorOut 	// ������������ ������� ������;
			break;
			
		case TWI_MTX_DATA_ACK: 	// ���� ������ �������, �������� ACK! (���� sawp - ��� ��� ���� ������, ���� sawsarp - ���� ������ ��������)
			if ((i2c_Do & i2c_type_msk) == i2c_sawp)		// � ����������� �� ������
			{
				if (i2c_index == i2c_ByteCount)				// ���� ��� ���� ������ ���������
				{
					TWCR = 	0<<TWSTA|
							1<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;						// ���� Stop
					
					MACRO_i2c_WhatDo_MasterOut				// � ������� � ��������� �����
				}
				else
				{
					TWDR = i2c_Buffer[i2c_index];			// ���� ���� ��� ���� ����
					i2c_index++;
					
					TWCR = 	0<<TWSTA|
							0<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;  						// Go!
				}
			}
			
			if ((i2c_Do & i2c_type_msk) == i2c_sawsarp)		// � ������ ������ ��
			{
				if (i2c_PageAddrIndex == i2c_PageAddrCount)	// ���� ��������� ���� ������ ��������
					TWCR = 	1<<TWSTA|
							0<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;						// ��������� ��������� �����!
				else
				{												// �����
					TWDR = i2c_PageAddress[i2c_PageAddrIndex];		// ���� ��� ���� ����� ��������
					i2c_PageAddrIndex++;							// ����������� ������ �������� ������ �������
					
					TWCR = 	0<<TWSTA|
							0<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;						// Go!
				}
			}
			
			break;
		case TWI_MTX_DATA_NACK:	//���� ����, �� �������� NACK; ������ ���: 1� - �������� �������� ������� � ��� ����, 2� - ����� �������.
			i2c_Do |= i2c_ERR_NK;						// ������� ������ ������. ���� ��� �� ����, ��� ������.
			
			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// ���� Stop
			
			MACRO_i2c_WhatDo_MasterOut					// ������������ ������� ������
			break;
		
		case TWI_ARB_LOST:	// �������� �� ����. ������� ���-�� ���������
			i2c_Do |= i2c_ERR_LP;						// ������ ������ ������ ����������
			
			// ����������� ������� ������.
			i2c_index = 0;
			i2c_PageAddrIndex = 0;
			
			TWCR = 	1<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// ��� ������ ���� ����� ��������
			break;										// ��������� �������� �����.
		
		case TWI_MRX_ADR_ACK: // ������� SLA+R, �������� ���; ������ ����� �������� �����
			if (i2c_index + 1 == i2c_ByteCount)			// ���� ����� �������� �� ���� �����, ��
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ������� ����, � � ����� ����� ������ NACK(Disconnect) ��� ���� ������ ������, ��� ��� ������ �����. � �� �������� ����
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ��� ������ ������ ���� � ������ ����� ACK
			
			break;
		case TWI_MRX_ADR_NACK: // ������� SLA+R, �� �������� NACK. ������, slave ����� ��� ��� ���.
			i2c_Do |= i2c_ERR_NA; // ��� ������ No Answer
			
			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// ���� Stop
			
			MACRO_i2c_WhatDo_ErrorOut					// ������������ �������� �������� ������
			break;
		
		case TWI_MRX_DATA_ACK: // ������� ����.
			i2c_Buffer[i2c_index] = TWDR;				// ������� ��� �� ������
			i2c_index++;
			
			// To Do: �������� �������� ������������ ������. � �� ���� �� ��� ���� ���������
			
			if (i2c_index + 1 == i2c_ByteCount)			// ���� ������� ��� ���� ���� �� ���, ��� �� ������ �������
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ����������� ��� � ����� ������ NACK (Disconnect)
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ���� ���, �� ����������� ��������� ����, � � ����� ������ ���
			
			break;
		case TWI_MRX_DATA_NACK:	// ��� �� ����� ��������� ����, ������� NACK ����� �������� � �����.
			i2c_Buffer[i2c_index] = TWDR;				// ����� ���� � �����
			
			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// �������� Stop
			
			MACRO_i2c_WhatDo_MasterOut					// ���������� ����� ������
			break;
		
		// IIC Slave ===================================================================================================================================================================================
		
		case 0x68:	// RCV SLA+W Low Priority				// ������� ���� ����� �� ����� �������� ��������
		case 0x78:	// RCV SLA+W Low Priority (Broadcast)				// ��� ��� ��� ����������������� �����. �� �����
			i2c_Do |= i2c_ERR_LP | i2c_Interrupted;		// ������ ���� ������ Low Priority, � ����� ���� ����, ��� ������� ��������
			
			// Restore Trans after.
			i2c_index = 0;								// ����������� ��������� �������� ������
			i2c_PageAddrIndex = 0;
			// � ����� ������. ��������!!! break ��� ���, � ������ ���� � "case 60"
		
		case 0x60: // RCV SLA+W  Incoming?					// ��� ������ �������� ���� �����
		case 0x70: // RCV SLA+W  Incoming? (Broascast)		// ��� ����������������� �����
			i2c_Do |= i2c_Busy;							// �������� ����. ����� ������ �� ��������
			i2c_SlaveIndex = 0;							// ��������� �� ������ ������ ������, ������� ����� �����. �� ��������
			
			if (i2c_MasterBytesRX == 1)					// ���� ��� ������� ������� ����� ���� ����, �� ��������� ������� ���
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ������� � ������� ����� ��� �... NACK!
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// � ���� ���� ���� ��� ���� ����, �� ������ � ��������� ��� ACK!
			
			break;
		
		case 0x80:	// RCV Data Byte						// � ��� �� ������� ���� ����. ��� ��� �����������������. �� �����
		case 0x90:	// RCV Data Byte (Broadcast)
			i2c_InBuff[i2c_SlaveIndex] = TWDR;			// ������� ��� � �����.
			i2c_SlaveIndex++;							// �������� ���������
			
			if (i2c_SlaveIndex == i2c_MasterBytesRX - 1) 	// �������� ����� ����� ��� ���� ����?
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ������� ��� � ������� NACK!
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ����� ��� ������? ������� � ACK!
			
			break;

		case 0x88: // RCV Last Byte							// ������� ��������� ����
		case 0x98: // RCV Last Byte (Broadcast)
			i2c_InBuff[i2c_SlaveIndex] = TWDR;			// ������� ��� � �����
			
			if (i2c_Do & i2c_Interrupted)				// ���� � ��� ��� ���������� ����� �� ����� �������
				TWCR = 	1<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ������ � ���� ���� Start �������� � ������� ��� ���� �������
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// ���� �� ���� ������ �����, �� ������ ��������� � ����� �����
			
			MACRO_i2c_WhatDo_SlaveOut					// � ������ ���������� ��� �������� ���� ��� ������
			break;
		
		case 0xA0: // ��, �� �������� ��������� �����. �� �� ��� � ��� ������?
			// �����, �������, ������� ��������������� �������, ����� ������������ ��� � ������ ���������� �������, ������� ��������.
			// �� � �� ���� ��������������. � ���� ������ �������� ��� ���.

			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					1<<TWEA|
					1<<TWEN|
					1<<TWIE;							// ������ �������������, �������������� ���� �����
			
			break;
		
		case TWI_STX_ADR_ACK_M_ARB_LOST:  // ������� ���� ����� �� ������ �� ����� �������� ��������
			i2c_Do |= i2c_ERR_LP | i2c_Interrupted;		// �� ��, ���� ������ � ���� ��������� ��������.
			
			// ��������������� �������
			i2c_index = 0;
			i2c_PageAddrIndex = 0;
			// Break ���! ���� ������
			
		case TWI_STX_ADR_ACK:	// RCV SLA+R, ACK sent
			i2c_SlaveIndex = 0;							// ������� ��������� �������� �� 0
			TWDR = i2c_OutBuff[i2c_SlaveIndex];			// ��� �, ������� ���� �� ���, ��� ����.
			
			if (i2c_MasterBytesTX == 1) // ���� ���� ���������, �� ��� �� NACK � ����� ��������
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						
			else // � ���� ���, �� ���� ACK
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						
			
			break;
		
		case TWI_STX_DATA_ACK: // ������� ����, �������� ACK
			i2c_SlaveIndex++;								// ������ ���������� ���������. ����� ��������� ����
			TWDR = i2c_OutBuff[i2c_SlaveIndex];				// ���� ��� �������
			
			if (i2c_SlaveIndex == i2c_MasterBytesTX - 1)		// ���� �� ��������� ���, ��
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;							// ���� ��� � ���� NACK
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						0<<TWEN|
						1<<TWIE;							// ���� ���, �� ���� � ���� ACK
			
			break;

		case TWI_STX_DATA_NACK: // �� ������� ��������� ����, ������ � ��� ���, �������� NACK
		case TWI_STX_DATA_ACK_LAST_BYTE: // ��� ACK. � ������ ������ ��� ���. �.�. ������ ������ � ��� ���.
			if (i2c_Do & i2c_Interrupted)		// ���� ��� ���� ��������� �������� �������
			{									// �� �� ��� �� ������
				i2c_Do &= i2c_NoInterrupted;	// ������ ���� �����������
				TWCR = 	1<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;					// �������� ����� ����� �� ��� ������� ����.
			}
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;					// ���� �� ��� ����, �� ������ ������� ����
			
			MACRO_i2c_WhatDo_SlaveOut				// � ���������� ����� ������. �������, �� ���
													// �� ����� �� �����. ����� ��� ��� ������, ��� ������
			break;									// ��� ������ ����� �������.
		
		default:
			break;
		*/
	}
	
	LED_MSG_OFF;
}

void Init_i2c(void)							// ��������� ������ �������
{
	// pins pull up:
	i2c_PORT |=	(1<<i2c_SCL) |
				(1<<i2c_SDA);
				
	i2c_DDR &= ~(1<<i2c_SCL | 1<<i2c_SDA);

	// Bit rate (for 8MHz):
	TWBR = 0x02;
	TWSR = 0x00;							// prescaler = 1; 400kHz
	//TWSR = 0x01;							// prescaler = 4; 100kHz
}
void Init_i2c_Slave(IIC_F Addr)				// ��������� ������ ������ (���� �����)
{
	TWAR = (i2c_MasterAddress << 1) | (ACCEPT_BROADCAST << 0);
												
	SlaveOutFunc = Addr;	// �������� ��������� ������ �� ������ ������� ������
	
	//TWCR =	(0<<TWSTA) |	// TWI START Condition Bit
			//(0<<TWSTO) |	// TWI STOP Condition Bit
			//(0<<TWINT) |	// TWI Interrupt Flag
			//(1<<TWEA)  |	// TWI Enable Acknowledge Bit; 0
			//(1<<TWEN)  |	// TWI Enable Bit
			//(1<<TWIE)  |	// TWI Interrupt Enable; 0
			//(0<<TWWC);

  TWCR =	(1<<TWEN) |								// Enable TWI-interface and release TWI pins.
			(0<<TWIE) | (0<<TWINT) |				// Disable TWI Interrupt.
			(0<<TWEA) | (0<<TWSTA) | (0<<TWSTO) |	// Do not ACK on any requests, yet.
			(0<<TWWC);
			
	TWI_busy = 0;
}

/****************************************************************************
Call this function to test if the TWI_ISR is busy transmitting.
****************************************************************************/
unsigned char TWI_TransceiverBusy()
{
	return TWI_busy;
}

/****************************************************************************
Call this function to fetch the state information of the previous operation. The function will hold execution (loop)
until the TWI_ISR has completed with the previous operation. If there was an error, then the function
will return the TWI State code.
****************************************************************************/
unsigned char TWI_GetStateInfo()
{
	while (TWI_TransceiverBusy());			// Wait until TWI has completed the transmission.
	return (TWI_state);						// Return error state.
}

/****************************************************************************
Call this function to start the Transceiver without specifying new transmission data. Useful for restarting
a transmission, or just starting the transceiver for reception. The driver will reuse the data previously put
in the transceiver buffers. The function will hold execution (loop) until the TWI_ISR has completed with the
previous operation, then initialize the next operation and return.
****************************************************************************/
void TWI_StartTransceiver()
{
	while (TWI_TransceiverBusy());             // Wait until TWI is ready for next transmission.
	
	TWI_statusReg.all = 0;
	TWI_state = TWI_NO_STATE ;
	
	TWCR =	(1<<TWEN) |								// TWI Interface enabled.
			(1<<TWIE) | (1<<TWINT) |				// Enable TWI Interupt and clear the flag.
			(1<<TWEA) | (0<<TWSTA) | (0<<TWSTO) |	// Prepare to ACK next time the Slave is addressed.
			(0<<TWWC);
			
	TWI_busy = 0;
}

/****************************************************************************
Call this function to send a prepared message, or start the Transceiver for reception. Include
a pointer to the data to be sent if a SLA+W is received. The data will be copied to the TWI buffer.
Also include how many bytes that should be sent. Note that unlike the similar Master function, the
Address byte is not included in the message buffers.
The function will hold execution (loop) until the TWI_ISR has completed with the previous operation,
then initialize the next operation and return.
****************************************************************************/
void TWI_StartTransceiverWithData(unsigned char *msg, uint8_t msgSize)
{
	uint8_t temp;

	while (TWI_TransceiverBusy());             // Wait until TWI is ready for next transmission.

	TWI_msgSize = msgSize;                        // Number of data to transmit.
	for (temp = 0; temp < msgSize; temp++)      // Copy data that may be transmitted if the TWI Master requests data.
		TWI_buf[temp] = msg[temp];
	
	TWI_statusReg.all = 0;
	TWI_state         = TWI_NO_STATE ;
	
	TWCR =	(1<<TWEN) |                             // TWI Interface enabled.
			(1<<TWIE) | (1<<TWINT) |                  // Enable TWI Interupt and clear the flag.
			(1<<TWEA) | (0<<TWSTA) | (0<<TWSTO) |       // Prepare to ACK next time the Slave is addressed.
			(0<<TWWC);
			
	TWI_busy = 1;
}

/****************************************************************************
Call this function to read out the received data from the TWI transceiver buffer. I.e. first call
TWI_Start_Transceiver to get the TWI Transceiver to fetch data. Then Run this function to collect the
data when they have arrived. Include a pointer to where to place the data and the number of bytes
to fetch in the function call. The function will hold execution (loop) until the TWI_ISR has completed
with the previous operation, before reading out the data and returning.
If there was an error in the previous transmission the function will return the TWI State code.
****************************************************************************/
bool TWI_GetDataFromTransceiver(unsigned char *msg, uint8_t msgSize)
{
	uint8_t i;

	while (TWI_TransceiverBusy());             // Wait until TWI is ready for next transmission.

	if (TWI_statusReg.lastTransOK)               // Last transmission completed successfully.
	{
		for (i = 0; i < msgSize; i++)                 // Copy data from Transceiver buffer.
			msg[i] = TWI_buf[i];
		
		TWI_statusReg.RxDataInBuf = false;          // Slave Receive data has been read from buffer.
	}
	
	return (TWI_statusReg.lastTransOK);
}

void DoNothing(void)
{
	// ������� ��������, �������� �������������� ������
}
