#include "Hardware.h"
#include <string.h>
#include <stdbool.h>
#include <avr/io.h>
#include "DCCOut.h"
#include "OperationFifo.h"
//---------------------------------------------------------------------------------------------
#define OPERATION_FIFO_SIZE 20
static volatile Message_t OperationFifo[OPERATION_FIFO_SIZE];
static volatile uint8_t operWriteIdx = 0;
static volatile uint8_t operReadIdx = 0;
//---------------------------------------------------------------------------------------------
static bool IsFull();
static bool IsEmpty();
//---------------------------------------------------------------------------------------------
void AddToOperationQueue(Message_t *msg)
{
	if (!IsFull())
	{
		memcpy((void*)&OperationFifo[operWriteIdx], msg, sizeof(Message_t));
		operWriteIdx++;
		operWriteIdx %= OPERATION_FIFO_SIZE;
	}
}
Message_t* GetFromOperationQueue()
{
    if (!IsEmpty())
    {
		uint8_t idx = operReadIdx;
        operReadIdx++;
        operReadIdx %= OPERATION_FIFO_SIZE;
		
		return (Message_t*)&OperationFifo[idx];
    }
	else
		return NULL;
}

static bool IsFull()
{
    return (((operWriteIdx + 1) % OPERATION_FIFO_SIZE) == operReadIdx);
}
static bool IsEmpty()
{
    return (operReadIdx == operWriteIdx);
}
//---------------------------------------------------------------------------------------------
