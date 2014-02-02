#ifndef RELAYS_H_
#define RELAYS_H_

#include "Hardware.h"

#define RELAY_DEFAULT_STATE		false

void InitRelays();
void SetRelay(uint8_t idx, bool state);
void SetRelays(bool state);
bool GetRelay(uint8_t idx);

#endif /* RELAYS_H_ */