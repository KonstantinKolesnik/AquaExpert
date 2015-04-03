#ifndef DTCOtaBootloader_H
#define DTCOtaBootloader_H

#include <avr/boot.h>
#include <avr/eeprom.h>
#include <avr/interrupt.h>
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <avr/power.h>
#include <avr/wdt.h>
#include <stdint.h>
#include <string.h>
#include <util/crc16.h>

#include "DTCMessage.h"
#include "DTCSensor.h"
#include "DTCOtaBootloaderRF24.h"

#define FIRMWARE_BLOCK_SIZE	16

struct FirmwareConfig
{
	uint16_t type;
	uint16_t version;
	uint16_t blocks;
	uint16_t crc;
};

typedef struct
{
	uint16_t type;
	uint16_t version;
} FirmwareConfigRequest;

typedef struct
{
	uint16_t type;
	uint16_t version;
	uint16_t blocks;
	uint16_t crc;
} FirmwareConfigResponse;

typedef struct
{
	uint16_t type;
	uint16_t version;
	uint16_t block;
} FirmwareRequest;

typedef struct
{
	uint16_t type;
	uint16_t version;
	uint16_t block;
	uint8_t data[FIRMWARE_BLOCK_SIZE];
} FirmwareResponse;

static struct NodeConfig nc;
static struct FirmwareConfig fc;
static DTCMessage msg;
static DTCMessage rmsg;

static clock_div_t orgClockDiv = 0;

static uint8_t progBuf[SPM_PAGESIZE];

#endif // DTCOtaBootloader_H
