#ifndef DTCGateway_h
#define DTCGateway_h

#include "utility/MsTimer2.h"
#include "DTCNode.h"
#include <aJSON.h>

#define MAX_RECEIVE_LENGTH	200 // Max buffer size needed for messages coming from controller
#define MAX_SEND_LENGTH		200 // Max buffer size needed for messages destined for controller

class DTCGateway : public DTCNode
{
public:
	/**
	* Constructor
	*
	* Creates a new instance of DTCGateway class.
	* If you don't use status leds on your gateway
	* you can disable this functionality by calling the 1 argument constructor.
	*
	* @param _uartESP A reference of UART object for link to ESP8266
	* @param _rx Digital pin for receive led
	* @param _tx Digital pin for transfer led
	* @param _er Digital pin for error led
	*
	*/
	DTCGateway(HardwareSerial &_uartESP, uint8_t _rx = 7, uint8_t _tx = 6, uint8_t _er = 5);

	void begin(uint8_t channel = WIFI_CHANNEL, void(*dataCallback)(char*) = NULL);
	void processRadioMessage();
	//void processSerialMessage(char*);
	void processSerialMessage(aJsonObject *msg);

private:
	char convBuf[MAX_PAYLOAD * 2 + 1];
	char serialBuffer[MAX_SEND_LENGTH]; // buffer for building string when sending data to controller
	boolean useWriteCallback;
	void(*dataCallback)(char*);

	uint8_t h2i(char c);
	void serial(const char *fmt, ...);
	void serial(DTCMessage &msg);
	void ledTimers();
	void rxBlink(uint8_t cnt);
	void txBlink(uint8_t cnt);
	void errBlink(uint8_t cnt);
};

void ledTimersInterrupt();

#endif
