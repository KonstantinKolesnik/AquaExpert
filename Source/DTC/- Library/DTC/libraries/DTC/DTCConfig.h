#ifndef DTCConfig_h
#define DTCConfig_h

/***
 * Configure node network
 */

#define ESP8266_USE_SOFTWARE_SERIAL

#define WIFI_CHANNEL	   6             // WiFi channel for the nodes network, 1-14.

// software UART pins defaults for ESP8266 connection
#define DEFAULT_RX_PIN		2
#define DEFAULT_TX_PIN		3

/***
 * Enable/Disable debug logging
 */
#define DEBUG

#endif
