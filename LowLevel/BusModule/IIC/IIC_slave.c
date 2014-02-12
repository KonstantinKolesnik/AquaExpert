#include "IIC_slave.h"
//****************************************************************************************
extern void ProcessMasterMessages();
union TWI_statusReg_t TWI_statusReg = {0};  // TWI_statusReg is defined in IIC_ultimate.h
static uint8_t TWI_buf[TWI_BUFFER_SIZE];	// Transceiver buffer. Set the size in the header file
static uint8_t TWI_msgSize = 0;             // Number of bytes to be transmitted.
static uint8_t TWI_state = TWI_NO_STATE;	// State byte. Default set to TWI_NO_STATE.
static bool TWI_busy = false;				// This is true when the TWI is in the middle of a transfer and set to false when all bytes have been transmitted/received. Also used to determine how deep we can sleep.
//****************************************************************************************
static void ClearInterruptFlag()
{
	TWCR =	(1<<TWEN)|                                 // TWI Interface enabled
			(1<<TWIE)|(1<<TWINT)|                      // Enable TWI Interrupt and clear the flag
			(1<<TWEA)|(0<<TWSTA)|(0<<TWSTO)|           // Acknowledge on any new requests.
			(0<<TWWC);
}
ISR(TWI_vect)	// TWI interrupt
{
	static uint8_t TWI_bufPtr;

	switch (TWSR & 0xF8)	// ќтсекаем биты прескэйлера
	{
		case TWI_STX_ADR_ACK:            // Own SLA+R has been received; ACK has been returned
		//case TWI_STX_ADR_ACK_M_ARB_LOST: // Arbitration lost in SLA+R/W as Master; own SLA+R has been received; ACK has been returned
			TWI_bufPtr = 0;                                 // Set buffer pointer to first data location
		case TWI_STX_DATA_ACK:           // Data byte in TWDR has been transmitted; ACK has been received
			// send next byte
			TWDR = TWI_buf[TWI_bufPtr++];
			ClearInterruptFlag();
			TWI_busy = true;
			break;
		case TWI_STX_DATA_NACK:          // Data byte from TWDR has been transmitted; NACK has been received. I.e. this could be the end of the transmission (master doesn't wait more data).
			if (TWI_bufPtr == TWI_msgSize) // Have we transceived all expected data?
				TWI_statusReg.lastTransOK = true;               // Set status bits to completed successfully.
			else                          // Master has sent a NACK before all data where sent.
				TWI_state = TWSR;                               // Store TWI State as error message.
			// wait for next address match;							
			ClearInterruptFlag();
			TWI_busy = false;   // Transmit is finished, we are not busy anymore
			break;
		case TWI_SRX_GEN_ACK:            // General call address has been received; ACK has been returned
		//case TWI_SRX_GEN_ACK_M_ARB_LOST: // Arbitration lost in SLA+R/W as Master; General call address has been received; ACK has been returned
			TWI_statusReg.genAddressCall = true;
		case TWI_SRX_ADR_ACK:            // Own SLA+W has been received ACK has been returned
		//case TWI_SRX_ADR_ACK_M_ARB_LOST: // Arbitration lost in SLA+R/W as Master; own SLA+W has been received; ACK has been returned
			// Don't need to clear TWI_S_statusRegister.generalAddressCall due to that it is the default state.
			TWI_statusReg.RxDataInBuf = true;
			TWI_bufPtr = 0;                                 // Set buffer pointer to first data location
			// wait for a new event.
			// Expect ACK on this transmission.
			ClearInterruptFlag();
			TWI_busy = true;
			break;
		case TWI_SRX_GEN_DATA_ACK:       // Previously addressed with general call; data has been received; ACK has been returned
		case TWI_SRX_ADR_DATA_ACK:       // Previously addressed with own SLA+W; data has been received; ACK has been returned
			TWI_buf[TWI_bufPtr++] = TWDR;
			TWI_statusReg.lastTransOK = true;                 // Set flag transmission successful.
			// wait for a new event.
			// send next byte
			// Send ACK after next reception
			ClearInterruptFlag();
			TWI_busy = true;
			break;
		case TWI_SRX_STOP_RESTART:       // A STOP condition or repeated START condition has been received while still addressed as Slave
			// Enter not addressed mode and listen to address match
			ClearInterruptFlag();
			TWI_busy = false;  // We are waiting for a new address match, so we are not busy
			ProcessMasterMessages();
			break;
		case TWI_SRX_GEN_DATA_NACK:      // Previously addressed with general call; data has been received; NOT ACK has been returned
		case TWI_SRX_ADR_DATA_NACK:      // Previously addressed with own SLA+W; data has been received; NOT ACK has been returned
		case TWI_STX_DATA_ACK_LAST_BYTE: // Last data byte in TWDR has been transmitted (TWEA = У0Ф); ACK has been received
		//case TWI_NO_STATE              // No relevant state information available; TWINT = У0Ф
		case TWI_BUS_ERROR:         // Bus error due to an illegal START or STOP condition
			TWI_state = TWSR;                 //Store TWI State as error message, operation also clears noErrors bit
			TWCR =   (1<<TWSTO)|(1<<TWINT);   //Recover from TWI_BUS_ERROR, this will release the SDA and SCL pins thus enabling other devices to use the bus
			break;
		default:
			TWI_state = TWSR;                                 // Store TWI State as error message, operation also clears the Success bit.
			ClearInterruptFlag();
			TWI_busy = false; // Unknown status, so we wait for a new address match that might be something we can handle
			break;
	}
}

void InitI2C()
{
	// pins pull up:
	TWI_PORT |=	(1<<TWI_SCL) |
				(1<<TWI_SDA);
				
	TWI_DDR &= ~(1<<TWI_SCL | 1<<TWI_SDA);

	TWSR = 0x00;							// prescaler = 0
	
	// Bit rate (for 8MHz):
	TWBR = 0x02;							// 400kHz
	TWBR = 0x20;							// 100kHz
	
	// slave mode:
	TWAR = (TWI_SlaveAddress << 1) | (TWI_ACCEPT_BROADCAST << 0);
	
	TWCR =	(1<<TWEN) |								// Enable TWI-interface and release TWI pins.
			(0<<TWIE) | (0<<TWINT) |				// Disable TWI Interrupt.
			(0<<TWEA) | (0<<TWSTA) | (0<<TWSTO) |	// Do not ACK on any requests, yet.
			(0<<TWWC);
			
	TWI_busy = 0;
}

/****************************************************************************
Call this function to test if the TWI_ISR is busy transmitting.
****************************************************************************/
bool TWI_TransceiverBusy()
{
	return TWI_busy;
}

/****************************************************************************
Call this function to fetch the state information of the previous operation. The function will hold execution (loop)
until the TWI_ISR has completed with the previous operation. If there was an error, then the function
will return the TWI State code.
****************************************************************************/
uint8_t TWI_GetStateInfo()
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
	while (TWI_TransceiverBusy());					// Wait until TWI is ready for next transmission.
	
	TWI_statusReg.all = 0;
	TWI_state = TWI_NO_STATE;
	
	ClearInterruptFlag();
			
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
	uint8_t i;

	while (TWI_TransceiverBusy());					// Wait until TWI is ready for next transmission.

	TWI_msgSize = msgSize;							// Number of data to transmit.
	for (i = 0; i < msgSize; i++)					// Copy data that may be transmitted if the TWI Master requests data.
		TWI_buf[i] = msg[i];
	
	TWI_statusReg.all = 0;
	TWI_state = TWI_NO_STATE;
	
	ClearInterruptFlag();
			
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
