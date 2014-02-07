#include "IIC_ultimate.h"

void DoNothing(void);

uint8_t i2c_Do;									// Переменная состояния передатчика IIC
uint8_t i2c_InBuff[i2c_MasterBytesRX];			// Буфер приема при работе как Slave
uint8_t i2c_OutBuff[i2c_MasterBytesTX];			// Буфер передачи при работе как Slave
uint8_t i2c_SlaveIndex;							// Индекс буфера Slave

uint8_t i2c_Buffer[i2c_MaxBuffer];				// Буфер для данных работы в режиме Master
uint8_t i2c_index;								// Индекс этого буфера

uint8_t i2c_ByteCount;							// Число байт передаваемых

uint8_t i2c_SlaveAddress;						// Адрес подчиненного

uint8_t i2c_PageAddress[i2c_MaxPageAddrLgth];	// Буфер адреса страниц (для режима с sawsarp)
uint8_t i2c_PageAddrIndex;						// Индекс буфера адреса страниц
uint8_t i2c_PageAddrCount;						// Число байт в адресе страницы для текущего Slave

// Указатели выхода из автомата:
IIC_F MasterOutFunc = &DoNothing;			//  в Master режиме
IIC_F SlaveOutFunc 	= &DoNothing;			//  в режиме Slave
IIC_F ErrorOutFunc 	= &DoNothing;			//  в результате ошибки в режиме Master

uint8_t 	WorkLog[100];						// Лог пишем сюда
uint8_t		WorkIndex = 0;						// Индекс лога


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

	switch(TWSR & 0xF8)	// Отсекаем биты прескалера
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
		//case TWI_STX_DATA_ACK_LAST_BYTE: // Last data byte in TWDR has been transmitted (TWEA = “0”); ACK has been received
		////    case TWI_NO_STATE              // No relevant state information available; TWINT = “0”
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
			if ((i2c_Do & i2c_type_msk) == i2c_sarp)		// В зависимости от режима
				i2c_SlaveAddress |= 0x01;					// Шлем Addr+R
			else											// Или
				i2c_SlaveAddress &= 0xFE;					// Шлем Addr+W
			
			TWDR = i2c_SlaveAddress;						// Адрес слейва
			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;
			break;

		case TWI_REP_START:	// Повторный старт был
			if ((i2c_Do & i2c_type_msk) == i2c_sawsarp)		// В зависимости от режима
				i2c_SlaveAddress |= 0x01;					// Шлем Addr+R
			else
				i2c_SlaveAddress &= 0xFE;					// Шлем Addr+W
						
			// To Do: Добавить сюда обработку ошибок
			TWDR = i2c_SlaveAddress;				// Адрес слейва
			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;  	// Go!
			break;

		case TWI_MTX_ADR_ACK:	// Был послан SLA+W, получили ACK
			if ((i2c_Do & i2c_type_msk) == i2c_sawp)		// В зависимости от режима
			{
				TWDR = i2c_Buffer[i2c_index];				// Шлем байт данных
				i2c_index++;							// Увеличиваем указатель буфера
			}

			if ((i2c_Do & i2c_type_msk) == i2c_sawsarp)
			{
				TWDR = i2c_PageAddress[i2c_PageAddrIndex];	// Или шлем адрес странцы (по сути тоже байт данных)
				i2c_PageAddrIndex++;						// Увеличиваем указатель буфера страницы
			}
			
			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;	// Go!
			
			break;
		case TWI_MTX_ADR_NACK:	// Был послан SLA+W получили NACK - слейв либо занят, либо его нет
			i2c_Do |= i2c_ERR_NA;	// Код ошибки

			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;	// Шлем шине Stop

			MACRO_i2c_WhatDo_ErrorOut 	// Обрабатываем событие ошибки;
			break;
			
		case TWI_MTX_DATA_ACK: 	// Байт данных послали, получили ACK! (если sawp - это был байт данных, если sawsarp - байт адреса страницы)
			if ((i2c_Do & i2c_type_msk) == i2c_sawp)		// В зависимости от режима
			{
				if (i2c_index == i2c_ByteCount)				// Если был байт данных последний
				{
					TWCR = 	0<<TWSTA|
							1<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;						// Шлем Stop
					
					MACRO_i2c_WhatDo_MasterOut				// И выходим в обработку стопа
				}
				else
				{
					TWDR = i2c_Buffer[i2c_index];			// Либо шлем еще один байт
					i2c_index++;
					
					TWCR = 	0<<TWSTA|
							0<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;  						// Go!
				}
			}
			
			if ((i2c_Do & i2c_type_msk) == i2c_sawsarp)		// В другом режиме мы
			{
				if (i2c_PageAddrIndex == i2c_PageAddrCount)	// Если последний байт адреса страницы
					TWCR = 	1<<TWSTA|
							0<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;						// Запускаем Повторный старт!
				else
				{												// Иначе
					TWDR = i2c_PageAddress[i2c_PageAddrIndex];		// шлем еще один адрес страницы
					i2c_PageAddrIndex++;							// Увеличиваем индекс счетчика адреса страниц
					
					TWCR = 	0<<TWSTA|
							0<<TWSTO|
							1<<TWINT|
							i2c_i_am_slave<<TWEA|
							1<<TWEN|
							1<<TWIE;						// Go!
				}
			}
			
			break;
		case TWI_MTX_DATA_NACK:	//Байт ушел, но получили NACK; причин две: 1я - передача оборвана слейвом и так надо, 2я - слейв сглючил.
			i2c_Do |= i2c_ERR_NK;						// Запишем статус ошибки. Хотя это не факт, что ошибка.
			
			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// Шлем Stop
			
			MACRO_i2c_WhatDo_MasterOut					// Отрабатываем событие выхода
			break;
		
		case TWI_ARB_LOST:	// Коллизия на шине. Нашелся кто-то поглавней
			i2c_Do |= i2c_ERR_LP;						// Ставим ошибку потери приоритета
			
			// Настраиваем индексы заново.
			i2c_index = 0;
			i2c_PageAddrIndex = 0;
			
			TWCR = 	1<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// Как только шина будет свободна
			break;										// попробуем передать снова.
		
		case TWI_MRX_ADR_ACK: // Послали SLA+R, получили АСК; теперь будем получать байты
			if (i2c_index + 1 == i2c_ByteCount)			// Если буфер кончится на этом байте, то
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Требуем байт, а в ответ потом пошлем NACK(Disconnect) Что даст понять слейву, что мол хватит гнать. И он отпустит шину
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Или просто примем байт и скажем потом ACK
			
			break;
		case TWI_MRX_ADR_NACK: // Послали SLA+R, но получили NACK. Видать, slave занят или его нет.
			i2c_Do |= i2c_ERR_NA; // Код ошибки No Answer
			
			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// Шлем Stop
			
			MACRO_i2c_WhatDo_ErrorOut					// Отрабатываем выходную ситуацию ошибки
			break;
		
		case TWI_MRX_DATA_ACK: // Приняли байт.
			i2c_Buffer[i2c_index] = TWDR;				// Забрали его из буфера
			i2c_index++;
			
			// To Do: Добавить проверку переполнения буфера. А то мало ли что юзер затребует
			
			if (i2c_index + 1 == i2c_ByteCount)			// Если остался еще один байт из тех, что мы хотели считать
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Затребываем его и потом пошлем NACK (Disconnect)
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Если нет, то затребываем следующий байт, а в ответ скажем АСК
			
			break;
		case TWI_MRX_DATA_NACK:	// Вот мы взяли последний байт, сказали NACK слейв обиделся и отпал.
			i2c_Buffer[i2c_index] = TWDR;				// Взяли байт в буфер
			
			TWCR = 	0<<TWSTA|
					1<<TWSTO|
					1<<TWINT|
					i2c_i_am_slave<<TWEA|
					1<<TWEN|
					1<<TWIE;							// Передали Stop
			
			MACRO_i2c_WhatDo_MasterOut					// Отработали точку выхода
			break;
		
		// IIC Slave ===================================================================================================================================================================================
		
		case 0x68:	// RCV SLA+W Low Priority				// Словили свой адрес во время передачи мастером
		case 0x78:	// RCV SLA+W Low Priority (Broadcast)				// Или это был широковещательный пакет. Не важно
			i2c_Do |= i2c_ERR_LP | i2c_Interrupted;		// Ставим флаг ошибки Low Priority, а также флаг того, что мастера прервали
			
			// Restore Trans after.
			i2c_index = 0;								// Подготовили прерваную передачу заново
			i2c_PageAddrIndex = 0;
			// И пошли дальше. Внимание!!! break тут нет, а значит идем в "case 60"
		
		case 0x60: // RCV SLA+W  Incoming?					// Или просто получили свой адрес
		case 0x70: // RCV SLA+W  Incoming? (Broascast)		// Или широковещательный пакет
			i2c_Do |= i2c_Busy;							// Занимаем шину. Чтобы другие не совались
			i2c_SlaveIndex = 0;							// Указатель на начало буфера слейва, Неважно какой буфер. Не ошибемся
			
			if (i2c_MasterBytesRX == 1)					// Если нам суждено принять всего один байт, то готовимся принять его
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Принять и сказать пошли все н... NACK!
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// А если душа шире чем один байт, то сожрем и потребуем еще ACK!
			
			break;
		
		case 0x80:	// RCV Data Byte						// И вот мы приняли этот байт. Наш или широковещательный. Не важно
		case 0x90:	// RCV Data Byte (Broadcast)
			i2c_InBuff[i2c_SlaveIndex] = TWDR;			// Сжираем его в буфер.
			i2c_SlaveIndex++;							// Сдвигаем указатель
			
			if (i2c_SlaveIndex == i2c_MasterBytesRX - 1) 	// Свободно место всего под один байт?
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Приянть его и сказать NACK!
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Места еще дофига? Принять и ACK!
			
			break;

		case 0x88: // RCV Last Byte							// Приянли последний байт
		case 0x98: // RCV Last Byte (Broadcast)
			i2c_InBuff[i2c_SlaveIndex] = TWDR;			// Сожрали его в буфер
			
			if (i2c_Do & i2c_Interrupted)				// Если у нас был прерываный сеанс от имени мастера
				TWCR = 	1<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Влепим в шину свой Start поскорей и сделаем еще одну попытку
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						// Если не было такого факта, то просто отвалимся и будем ждать
			
			MACRO_i2c_WhatDo_SlaveOut					// И лениво отработаем наш выходной экшн для слейва
			break;
		
		case 0xA0: // Ой, мы получили Повторный старт. Но чо нам с ним делать?
			// Можно, конечно, сделать вспомогательный автомат, чтобы обрабатывать еще и адреса внутренних страниц, подобно еепромке.
			// Но я не стал заморачиваться. В этом случае делается это тут.

			TWCR = 	0<<TWSTA|
					0<<TWSTO|
					1<<TWINT|
					1<<TWEA|
					1<<TWEN|
					1<<TWIE;							// просто разадресуемся, проигнорировав этот посыл
			
			break;
		
		case TWI_STX_ADR_ACK_M_ARB_LOST:  // Поймали свой адрес на чтение во время передачи Мастером
			i2c_Do |= i2c_ERR_LP | i2c_Interrupted;		// Ну чо, коды ошибки и флаг прерваной передачи.
			
			// Восстанавливаем индексы
			i2c_index = 0;
			i2c_PageAddrIndex = 0;
			// Break нет! Идем дальше
			
		case TWI_STX_ADR_ACK:	// RCV SLA+R, ACK sent
			i2c_SlaveIndex = 0;							// Индексы слейвовых массивов на 0
			TWDR = i2c_OutBuff[i2c_SlaveIndex];			// Что ж, отдадим байт из тех, что есть.
			
			if (i2c_MasterBytesTX == 1) // Если байт последний, мы еще на NACK в ответ надеемся
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;						
			else // А если нет, то ждем ACK
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;						
			
			break;
		
		case TWI_STX_DATA_ACK: // Послали байт, получили ACK
			i2c_SlaveIndex++;								// Значит продолжаем дискотеку. Берем следующий байт
			TWDR = i2c_OutBuff[i2c_SlaveIndex];				// Даем его мастеру
			
			if (i2c_SlaveIndex == i2c_MasterBytesTX - 1)		// Если он последний был, то
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						0<<TWEA|
						1<<TWEN|
						1<<TWIE;							// Шлем его и ждем NACK
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						0<<TWEN|
						1<<TWIE;							// Если нет, то шлем и ждем ACK
			
			break;

		case TWI_STX_DATA_NACK: // Мы выслали последний байт, больше у нас нет, получили NACK
		case TWI_STX_DATA_ACK_LAST_BYTE: // или ACK. В данном случае нам пох. Т.к. больше байтов у нас нет.
			if (i2c_Do & i2c_Interrupted)		// Если там была прерваная передача мастера
			{									// То мы ему ее вернем
				i2c_Do &= i2c_NoInterrupted;	// Снимем флаг прерваности
				TWCR = 	1<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;					// Сгенерим старт сразу же как получим шину.
			}
			else
				TWCR = 	0<<TWSTA|
						0<<TWSTO|
						1<<TWINT|
						1<<TWEA|
						1<<TWEN|
						1<<TWIE;					// Если мы там одни, то просто отдадим шину
			
			MACRO_i2c_WhatDo_SlaveOut				// И отработаем выход слейва. Впрочем, он тут
													// Не особо то нужен. Разве что как сигнал, что мастер
			break;									// Нас почтил своим визитом.
		
		default:
			break;
		*/
	}
	
	LED_MSG_OFF;
}

void Init_i2c(void)							// Настройка режима мастера
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
void Init_i2c_Slave(IIC_F Addr)				// Настройка режима слейва (если нужно)
{
	TWAR = (i2c_MasterAddress << 1) | (ACCEPT_BROADCAST << 0);
												
	SlaveOutFunc = Addr;	// Присвоим указателю выхода по слейву функцию выхода
	
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
	// Функция пустышка, затыкать несуществующие ссылки
}
