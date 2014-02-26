#include "OneWire.h"
#ifdef UART_AS_OneWire
	#include <avr/interrupt.h>
#endif
//****************************************************************************************
#define sbi(reg,bit) reg |= (1<<bit)
#define cbi(reg,bit) reg &= ~(1<<bit)
#define ibi(reg,bit) reg ^= (1<<bit)
#define CheckBit(reg,bit) (reg&(1<<bit))
//****************************************************************************************
static uint16_t devicesCount;
//extern void RunTasks(unsigned char tasks);
//****************************************************************************************
void OthersTasks()
{
	//RunTasks(0xFF);
}

#ifndef UART_AS_OneWire
void OW_Set(unsigned char mode)
{
#ifndef OW_TWO_PINS
	if (mode) {
		cbi(OW_PORT, OW_BIT); 
		sbi(OW_DDR, OW_BIT);
	}
	else {
		cbi(OW_PORT, OW_BIT); 
		cbi(OW_DDR, OW_BIT);
	}
#else
	if (mode) cbi(OW_PORT, OW_BIT_OUT);
	else sbi(OW_PORT, OW_BIT_OUT);
#endif
}
unsigned char OW_CheckIn(void)
{
#ifndef OW_TWO_PINS
	return CheckBit(OW_PIN, OW_BIT);
#else
	return CheckBit(OW_PIN, OW_BIT_IN);
#endif
}
#endif

uint8_t OW_Reset()
{
#ifdef UART_AS_OneWire
	UCSRB=(1<<RXEN)|(1<<TXEN);
	//9600
	UBRRL = USART_BAUDRATE_9600;
	UBRRH = (USART_BAUDRATE_9600 >> 8);
	UCSRA &= ~(1<<U2X);
	
	while(CheckBit(UCSRA, RXC)) UDR; //Зачистка буферов
	cli();
	UDR = 0xF0;
	UCSRA = (1<<TXC);
	sei();
	//while(!CheckBit(UCSRA, TXC)) OthersTasks();
	while(!CheckBit(UCSRA, RXC)) OthersTasks();
	if (UDR != 0xF0) return 1;
 return 0;
#else
	uint8_t	status;
	OW_Set(1);
	_delay_us(480);
	OW_Set(0);
	_delay_us(60);
	//Store line value and wait until the completion of 480uS period
	status = OW_CheckIn();
	_delay_us(420);
	//Return the value read from the presence pulse (0=OK, 1=WRONG)
 return !status;
#endif
//	return 1 if found
//	return 0 if not found
}
void OW_WriteBit(uint8_t bit)
{
#ifdef UART_AS_OneWire	
	//115200
	UBRRL = USART_BAUDRATE_115200;
	UBRRH = (USART_BAUDRATE_115200 >> 8);
	UCSRA |= (1<<U2X);
	
	uint8_t	d = 0x00;	
	while(CheckBit(UCSRA, RXC)) UDR; //Зачистка буферов
	if (bit) d = 0xFF;
	cli();
	UDR = d;
	UCSRA=(1<<TXC);
	sei();
	while(!CheckBit(UCSRA,TXC));
	while(CheckBit(UCSRA, RXC)) UDR; //Зачистка буферов
#else	
	//Pull line low for 1uS
	OW_Set(1);
	_delay_us(1);
	//If we want to write 1, release the line (if not will keep low)
	if (bit)
		OW_Set(0); 
	//Wait for 60uS and release the line
	_delay_us(60);
	OW_Set(0);
#endif	
}
uint8_t OW_ReadBit()
{
#ifdef UART_AS_OneWire	
	//115200
	UBRRL = USART_BAUDRATE_115200;
	UBRRH = (USART_BAUDRATE_115200 >> 8);
	UCSRA |= (1<<U2X);
	
	uint8_t	c;
	while(CheckBit(UCSRA, RXC)) UDR; //Зачистка буферов
	cli();		
	UDR = 0xFF;
	UCSRA=(1<<TXC);
	sei();
	while(!CheckBit(UCSRA, TXC));
	while(!CheckBit(UCSRA, RXC));
	c = UDR;
	if (c>0xFE) return 1;
	return 0;
#else	
	uint8_t	bit=0;
	//Pull line low for 1uS
	OW_Set(1);
	_delay_us(1);
	//Release line and wait for 14uS
	OW_Set(0);
	_delay_us(14);
	//Read line value
	if(OW_CheckIn()) bit=1;
	//Wait for 45uS to end and return read value
	_delay_us(45);
	return bit;
#endif	
}

#ifdef UART_AS_OneWire
uint8_t OW_WriteByte(uint8_t byte)
{
	uint8_t	i = 8;
	//115200
	UBRRL = USART_BAUDRATE_115200;
	UBRRH = (USART_BAUDRATE_115200 >> 8);
	UCSRA |= (1<<U2X);
  
	do
	{
		uint8_t	d = 0x00;
		if (byte&1) d = 0xFF;
		cli();
		UDR = d;
		UCSRA=(1<<TXC);
		sei();
		OthersTasks();
		while(!CheckBit(UCSRA,RXC)) OthersTasks();
		byte>>=1;
		if (UDR>0xFE) byte|=128;
	}
	while(--i);
	
	return byte&255;
}
#else
void OW_WriteByte(uint8_t byte)
{
	for (uint8_t i = 0; i < 8; i++)
		OW_WriteBit(CheckBit(byte, i));
}
uint8_t OW_ReadByte()
{
	uint8_t n = 0;

	for (uint8_t i = 0; i < 8; i++)
		if (OW_ReadBit())
			sbi(n, i);
	
	return n;
}
#endif

uint8_t OW_SearchROM(uint8_t diff, uint8_t *id)
{ 	
	unsigned char i, j, next_diff;
	unsigned char b;

	if (!OW_Reset()) 
		return OW_PRESENCE_ERR;       // error, no device found

	OW_WriteByte(OW_CMD_SEARCHROM);     // ROM search command
	next_diff = OW_LAST_DEVICE;      // unchanged on last device
	
	i = OW_ROMCODE_SIZE * 8;         // 8 bytes
	do 
	{	
		j = 8;                        // 8 bits
		do 
		{ 
			b = OW_ReadBit();			// read bit
			if (OW_ReadBit()) 
			{	// read complement bit
				if (b)                 // 11
					return OW_DATA_ERR;  // data error
			}
			else 
			{ 
				if (!b) // 00 = 2 devices
				{
					if (diff > i || ((*id & 1) && diff != i))
					{
						b = 1;               // now 1
						next_diff = i;       // next pass 0
					}
				}
			}
			
			OW_WriteBit(b);               // write bit
			*id >>= 1;
			if (b)
				*id |= 0x80;			// store bit
				
			i--;
		}
		
		while (--j);
		
		id++;                            // next byte
    }
	
	while (i);
	
	return next_diff;                  // to continue search
}
bool OW_ReadROM(uint8_t *buffer)
{
	if (!OW_Reset())
		return false;
		
	OW_WriteByte(OW_CMD_READROM);
	for (uint8_t i = 0; i < 8; i++)
		buffer[i] = OW_ReadByte();
		
	return true;
}
bool OW_MatchROM(uint8_t* rom)
{
 	if (!OW_Reset())
		return false;
		
	OW_WriteByte(OW_CMD_MATCHROM);	
	
	for (uint8_t i = 0; i < 8; i++)
		OW_WriteByte(rom[i]);
		
	return true;
}

void OW_Init()
{
	//OW_PORT |= (1<<OW_BIT);			// pull up
	//OW_DDR  |= (0<<OW_BIT);    		// in
	
	//timerDelayInit();

	devicesCount = OW_Scan();
}
uint16_t OW_Scan()
{
	uint8_t	i;
	uint8_t	id[OW_ROMCODE_SIZE];
	uint8_t	diff;
	uint8_t count = 0;

	for (diff = OW_SEARCH_FIRST; diff != OW_LAST_DEVICE && count < MAXDEVICES; )
	{
		diff = OW_SearchROM(diff, &id[0]);

		if (diff == OW_PRESENCE_ERR || diff == OW_DATA_ERR)
			break;

		for (i = 0; i < OW_ROMCODE_SIZE; i++)
			owDevicesIDs[count][i] = id[i];
		
		count++;
	}
	
	return count;
}
uint16_t OW_GetDeviceCount()
{
	return devicesCount;
}