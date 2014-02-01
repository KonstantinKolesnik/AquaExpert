#include "Hardware.h"
#include <string.h>
#include <stdbool.h>
#include <avr/io.h>
#include "DCCOut.h"
#include "ProgramFifo.h"
//---------------------------------------------------------------------------------------------
#define SERVICE_FIFO_SIZE 5
static volatile Message_t ServiceFifo[SERVICE_FIFO_SIZE];
static volatile uint8_t progWriteIdx = 0;
static volatile uint8_t progReadIdx = 0;
//---------------------------------------------------------------------------------------------
static bool IsFull();
static bool IsEmpty();
//---------------------------------------------------------------------------------------------
void AddToProgramQueue(Message_t* msg)
{
	if (!IsFull())
	{
		memcpy((void*)&ServiceFifo[progWriteIdx], msg, sizeof(Message_t));
		progWriteIdx++;
		progWriteIdx %= SERVICE_FIFO_SIZE;
	}
}
Message_t* GetFromProgramQueue()
{
    if (!IsEmpty())
    {
		uint8_t idx = progReadIdx;
        progReadIdx++;
        progReadIdx %= SERVICE_FIFO_SIZE;
		
		return (Message_t*)&ServiceFifo[idx];
    }
	else
		return NULL;
}

static bool IsFull()
{
    return (((progWriteIdx + 1) % SERVICE_FIFO_SIZE) == progReadIdx);
}
static bool IsEmpty()
{
    return (progReadIdx == progWriteIdx);
}
//---------------------------------------------------------------------------------------------
