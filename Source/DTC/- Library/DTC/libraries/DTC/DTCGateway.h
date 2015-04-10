#ifndef DTCGateway_h
#define DTCGateway_h

#include "DTCSensor.h"

#define MAX_RECEIVE_LENGTH 100 // Max buffer size needed for messages coming from controller
#define MAX_SEND_LENGTH 120 // Max buffer size needed for messages destined for controller

class DTCGateway : public DTCSensor
{
public:
	/**
	* Constructor
	*
	* Creates a new instance of DTCGateway class.
	* If you don't use status leds and/or inclusion mode button on your gateway
	* you can disable this functionality by calling the 2 argument constructor.
	*
	* @param _rxpin The pin attached to ESP8266 TX (default 9)
	* @param _txpin The pin attached to ESP8266 RX (defualt 10)
	* @param _inclusion_time Time of inclusion mode (in minutes, default 1)
	* @param _inclusion_pin Digital pin that triggers inclusion mode
	* @param _rx Digital pin for receive led
	* @param _tx Digital pin for transfer led
	* @param _er Digital pin for error led
	*
	*/
	DTCGateway(uint8_t _rxpin = DEFAULT_RX_PIN, uint8_t _txpin = DEFAULT_TX_PIN, uint8_t _inclusion_time = 1, uint8_t _inclusion_pin = 3, uint8_t _rx = 6, uint8_t _tx = 5, uint8_t _er = 4);
	void begin(uint8_t channel = WIFI_CHANNEL, void(*dataCallback)(char*) = NULL);
	void processRadioMessage();
	void parseAndSend(char *inputString);

private:
	char convBuf[MAX_PAYLOAD * 2 + 1];
	char serialBuffer[MAX_SEND_LENGTH]; // buffer for building string when sending data to controller
	unsigned long inclusionStartTime;
	boolean useWriteCallback;
	void(*dataCallback)(char*);
	uint8_t pinInclusion;
	uint8_t inclusionTime;

	uint8_t h2i(char c);
	void serial(const char *fmt, ...);
	void serial(DTCMessage &msg);
	void checkButtonTriggeredInclusion();
	void setInclusionMode(boolean newMode);
	void checkInclusionFinished();
	void ledTimers();
	void rxBlink(uint8_t cnt);
	void txBlink(uint8_t cnt);
	void errBlink(uint8_t cnt);
};

void ledTimersInterrupt();
void startInclusionInterrupt();

#endif
