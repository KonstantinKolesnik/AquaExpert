#ifndef RELAYS_H_
#define RELAYS_H_
//****************************************************************************************
#include "Hardware.h"
//****************************************************************************************
#define RELAY_ACTIVE_LEVEL		0		// 8-relay module active level is "0"
#define RELAY_DEFAULT_STATE		false	// off
//****************************************************************************************
void InitRelays();
void SetRelay(uint8_t idx, bool active);
void SetRelays(bool active);
bool GetRelay(uint8_t idx);
uint8_t GetRelays();
//****************************************************************************************
#endif /* RELAYS_H_ */