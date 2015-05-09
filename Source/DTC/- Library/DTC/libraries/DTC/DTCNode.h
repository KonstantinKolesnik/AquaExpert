#ifndef DTCSensor_h
#define DTCSensor_h

#include "Version.h"
#include "DTCConfig.h"
#include "DTCMessage.h"
#include <stddef.h>
#include <avr/eeprom.h>
#include <avr/pgmspace.h>
#include <avr/wdt.h>
#include <stdarg.h>

#ifdef __cplusplus
#include <Arduino.h>
#include "./WeeESP8266/ESP8266.h"
#include "utility/LowPower.h"
#endif

#ifdef DEBUG
#define debug(x,...) debugPrint(x, ##__VA_ARGS__)
#else
#define debug(x,...)
#endif

#define BAUD_RATE 115200

#define AUTO								0xFF // 0-254. Id 255 is reserved for auto initialization of nodeId.
#define NODE_SENSOR_ID						0xFF // Node child id is always created for when a node

// EEPROM start address for DTC library data
#define EEPROM_START						0

#define EEPROM_NODE_ID_ADDRESS				EEPROM_START // 32 bytes of GUID: 64722B95-C208-49EF-9B4F-9DC25117E86E
#define EEPROM_GATEWAY_MAC_ADDRESS			(EEPROM_NODE_ID_ADDRESS + 32) // 16 bytes of MAC address: DE-AD-BE-EF-FE-ED

#define EEPROM_CONTROLLER_CONFIG_ADDRESS	(EEPROM_GATEWAY_MAC_ADDRESS + 16) // Location of controller sent configuration (24 bytes reserved)

#define EEPROM_FIRMWARE_TYPE_ADDRESS		(EEPROM_CONTROLLER_CONFIG_ADDRESS + 24)
#define EEPROM_FIRMWARE_VERSION_ADDRESS		(EEPROM_FIRMWARE_TYPE_ADDRESS + 2)
#define EEPROM_FIRMWARE_BLOCKS_ADDRESS		(EEPROM_FIRMWARE_VERSION_ADDRESS + 2)
#define EEPROM_FIRMWARE_CRC_ADDRESS			(EEPROM_FIRMWARE_BLOCKS_ADDRESS + 2)

#define EEPROM_LOCAL_CONFIG_ADDRESS			(EEPROM_FIRMWARE_CRC_ADDRESS + 2) // First free address for sketch static configuration

// nodeId of net gateway receiver (where all nodes should send their data).
#define GATEWAY_ADDRESS						("00000000-0000-0000-0000-000000000000")
#define BROADCAST_ADDRESS					("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")

struct NodeConfig
{
	char* nodeId; // current node id
	char* gwMac; // current gateway mac-address
};
struct ControllerConfig
{
	uint8_t isMetric;
};

#ifdef __cplusplus
class DTCNode : public ESP8266
{
public:
	/*
	* Constuctor.
	*
	* @param uart - a reference of UART object for link to ESP8266
	*/
	DTCNode(HardwareSerial &uart);

	/**
	* Begin operation of the DTC library
	*
	* Call this in setup(), before calling any other sensor net library methods.
	* @param incomingMessageCallback Callback function for incoming messages from other nodes or controller and request responses. Default is NULL.
	* @param nodeId The unique id (1-254) for this sensor. Default is AUTO(255) which means sensor tries to fetch an id from controller.
	* @param channel Radio channel. Default is channel 6
	*/
	void begin(void(*msgCallback)(const DTCMessage &) = NULL, uint8_t channel = WIFI_CHANNEL);

	/**
	 * Return the nodes nodeId.
	 */
	uint8_t getNodeId();

	/**
	* Each node must present itself before any values can be handled correctly by the controller.
	* It is usually good to present after power-up in setup().
	*
	* @param nodeType The node type. See node typedef in DTCMessage.h.
	*/
	void present(uint8_t nodeType);

	/**
	 * Sends sketch meta information to the gateway. Not mandatory but a nice thing to do.
	 * @param name String containing a short Sketch name or NULL  if not applicable
	 * @param version String containing a short Sketch version or NULL if not applicable
	 * @param ack Set this to true if you want destination node to send ack back to this node. Default is not to request any ack.
	 *
	 */
	void sendSketchInfo(const char *name, const char *version);

	/**
	* Sends a message to gateway or one of the other nodes in the radio network
	*
	* @param msg Message to send
	* @param ack Set this to true if you want destination node to send ack back to this node. Default is not to request any ack.
	* @return true Returns true if message reached the first stop on its way to destination.
	*/
	bool send(DTCMessage &msg);

	/**
	 * Send this nodes battery level to gateway.
	 * @param level Level between 0-100(%)
	 * @param ack Set this to true if you want destination node to send ack back to this node. Default is not to request any ack.
	 *
	 */
	void sendBatteryLevel(uint8_t level);

	/**
	* Requests a value from gateway or some other sensor in the radio network.
	* Make sure to add callback-method in begin-method to handle request responses.
	*
	* @param childSensorId The unique child id for the different sensors connected to this Arduino. 0-254.
	* @param variableType The variableType to fetch
	* @param destination The nodeId of other node in radio network. Default is gateway
	*/
	void request(uint8_t variableType, char* destination = GATEWAY_ADDRESS);

	/**
	 * Requests time from controller. Answer will be delivered to callback.
	 *
	 * @param callback for time request. Incoming argument is seconds since 1970.
	 */
	void requestTime(void(*timeCallback)(unsigned long));


	/**
	 * Processes incoming messages to this node. If this is a relaying node it will
	 * Returns true if there is a message addressed for this node just was received.
	 * Use callback to handle incoming messages.
	 */
	boolean process();

	/**
	 * Returns the most recent node configuration received from controller
	 */
	ControllerConfig getConfig();

	/**
	 * Save a state (in local EEPROM). Good for actuators to "remember" state between
	 * power cycles.
	 *
	 * You have 256 bytes to play with. Note that there is a limitation on the number
	 * of writes the EEPROM can handle (~100 000 cycles).
	 *
	 * @param pos The position to store value in (0-255)
	 * @param Value to store in position
	 */
	void saveState(uint8_t pos, uint8_t value);

	/**
	 * Load a state (from local EEPROM).
	 *
	 * @param pos The position to fetch value from  (0-255)
	 * @return Value to store in position
	 */
	uint8_t loadState(uint8_t pos);

	/**
	* Returns the last received message
	*/
	DTCMessage& getLastMessage();

	/**
	 * Sleep (PowerDownMode) the Arduino and radio. Wake up on timer.
	 * @param ms Number of milliseconds to sleep.
	 */
	void sleep(unsigned long ms);

	/**
	 * Wait for a specified amount of time to pass.  Keeps process()ing.
	 * This does not power-down the radio nor the Arduino.
	 * Because this calls process() in a loop, it is a good way to wait
	 * in your loop() on a repeater node or sensor that listens to messages.
	 * @param ms Number of milliseconds to sleep.
	 */
	void wait(unsigned long ms);

	/**
	 * Sleep (PowerDownMode) the Arduino and radio. Wake up on timer or pin change.
	 * See: http://arduino.cc/en/Reference/attachInterrupt for details on modes and which pin
	 * is assigned to what interrupt. On Nano/Pro Mini: 0=Pin2, 1=Pin3
	 * @param interrupt Interrupt that should trigger the wakeup
	 * @param mode RISING, FALLING, CHANGE
	 * @param ms Number of milliseconds to sleep or 0 to sleep forever
	 * @return true if wake up was triggered by pin change and false means timer woke it up.
	 */
	bool sleep(uint8_t interrupt, uint8_t mode, unsigned long ms = 0);

	/**
	 * Sleep (PowerDownMode) the Arduino and radio. Wake up on timer or pin change for two separate interrupts.
	 * See: http://arduino.cc/en/Reference/attachInterrupt for details on modes and which pin
	 * is assigned to what interrupt. On Nano/Pro Mini: 0=Pin2, 1=Pin3
	 * @param interrupt1 First interrupt that should trigger the wakeup
	 * @param mode1 Mode for first interrupt (RISING, FALLING, CHANGE)
	 * @param interrupt2 Second interrupt that should trigger the wakeup
	 * @param mode2 Mode for second interrupt (RISING, FALLING, CHANGE)
	 * @param ms Number of milliseconds to sleep or 0 to sleep forever
	 * @return Interrupt number wake up was triggered by pin change and negative if timer woke it up.
	 */
	int8_t sleep(uint8_t interrupt1, uint8_t mode1, uint8_t interrupt2, uint8_t mode2, unsigned long ms = 0);


#ifdef DEBUG
	void debugPrint(const char *fmt, ...);
	int freeRam();
#endif

protected:
	NodeConfig nc; // Essential settings for node to work
	ControllerConfig cc; // Configuration coming from controller
	bool isGateway;
	DTCMessage msg;  // Buffer for incoming messages.

	void setupRadio(uint8_t channel);
	boolean sendRoute(DTCMessage &message);
	boolean sendWrite(char* dest, DTCMessage &message, bool broadcast = false);

private:
//#ifdef DEBUG
//	char convBuf[MAX_PAYLOAD * 2 + 1];
//#endif
	void(*timeCallback)(unsigned long); // Callback for requested time messages
	void(*msgCallback)(const DTCMessage &); // Callback for incoming messages from other nodes and gateway.

	void requestNodeId();
	void setupNode();
	uint8_t crc8Message(DTCMessage &message);
	void internalSleep(unsigned long ms);
};
#endif

#endif