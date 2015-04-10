#ifndef DTCConfig_h
#define DTCConfig_h

/***
 * Configure node network
 */
#define WIFI_CHANNEL	   6             //WiFi channel for the nodes network, 1-14.
#define BASE_RADIO_ID 	   ((uint64_t)0xA8A8E1FC00LL) // This is also act as base value for sensor nodeId addresses. Change this (or channel) if you have more than one sensor network.

// software/hardware UART pins defaults for ESP8266 connection
#define DEFAULT_RX_PIN		9
#define DEFAULT_TX_PIN		10

/***
 * Enable/Disable debug logging
 */
#define DEBUG

#endif
