#include "DTCNode.h"
#include "utility/LowPower.h"
#include "ESP8266.h"

// Inline function and macros
inline DTCMessage& build(DTCMessage &msg, uint8_t sender, uint8_t destination, uint8_t sensor, uint8_t command, uint8_t type)
{
	msg.sender = sender;
	msg.destination = destination;
	msg.sensor = sensor;
	msg.type = type;
	mSetCommand(msg, command);
	return msg;
}

DTCNode::DTCNode(HardwareSerial &uart)
	: ESP8266(uart)
{
}

void DTCNode::begin(void(*_msgCallback)(const DTCMessage &), uint8_t _nodeId, uint8_t channel)
{
	Serial.begin(BAUD_RATE);

	isGateway = false;
	msgCallback = _msgCallback;

	setupRadio(channel);

	// Read settings from eeprom
	eeprom_read_block((void*)&nc, (void*)EEPROM_NODE_ID_ADDRESS, sizeof(NodeConfig));

	// Read latest received controller configuration from EEPROM
	eeprom_read_block((void*)&cc, (void*)EEPROM_CONTROLLER_CONFIG_ADDRESS, sizeof(ControllerConfig));
	if (cc.isMetric == 0xff) // Eeprom empty, set default to metric
		cc.isMetric = 1;

	// Set static id
	if ((_nodeId != AUTO) && (nc.nodeId != _nodeId))
	{
		nc.nodeId = _nodeId;
		eeprom_write_byte((uint8_t*)EEPROM_NODE_ID_ADDRESS, _nodeId);
	}

	// Try to fetch node-id from gateway
	if (nc.nodeId == AUTO)
		requestNodeId();

	debug(PSTR("started, id %d\n"), nc.nodeId);

	// If we got an id, set this node to use it
	if (nc.nodeId != AUTO)
	{
		setupNode();
		wait(2000); // wait configuration reply.
	}
}

void DTCNode::setupRadio(uint8_t channel)
{
	//// Start up the radio library
	//RF24::begin();

	//if (!RF24::isPVariant()) {
	//	debug(PSTR("check wires\n"));
	//	while (1);
	//}

	//RF24::setAutoAck(1);
	//RF24::setAutoAck(BROADCAST_PIPE, false); // Turn off auto ack for broadcast
	//RF24::enableAckPayload();
	//RF24::setChannel(channel);
	//RF24::setPALevel(paLevel);
	//RF24::setDataRate(dataRate);
	//RF24::setRetries(5, 15);
	//RF24::setCRCLength(RF24_CRC_16);
	//RF24::enableDynamicPayloads();

	//// All nodes listen to broadcast pipe (for FIND_PARENT_RESPONSE messages)
	//RF24::openReadingPipe(BROADCAST_PIPE, TO_ADDR(BROADCAST_ADDRESS));
}

void DTCNode::setupNode()
{
	// Open reading pipe for messages directed to this node (set write pipe to same)
	//RF24::openReadingPipe(WRITE_PIPE, TO_ADDR(nc.nodeId));
	//RF24::openReadingPipe(CURRENT_NODE_PIPE, TO_ADDR(nc.nodeId));

	// Send presentation for this radio node
	present(NODE_SENSOR_ID, S_ARDUINO_NODE);

	// Send a configuration exchange request to controller
	// Node sends parent node. Controller answers with latest node configuration which is picked up in process()
	sendRoute(build(msg, nc.nodeId, GATEWAY_ADDRESS, NODE_SENSOR_ID, C_INTERNAL, I_CONFIG).set(""/*nc.parentNodeId*/));
}

uint8_t DTCNode::getNodeId()
{
	return nc.nodeId;
}
ControllerConfig DTCNode::getConfig()
{
	return cc;
}
DTCMessage& DTCNode::getLastMessage()
{
	return msg;
}

bool DTCNode::sendWrite(uint8_t next, DTCMessage &message, bool broadcast)
{
	//uint8_t length = mGetLength(message);
	message.last = nc.nodeId;
	mSetVersion(message, PROTOCOL_VERSION);
	
	// Make sure radio has powered up
	//RF24::powerUp();
	//RF24::stopListening();
	//RF24::openWritingPipe(TO_ADDR(next));
	bool ok = true;// RF24::write(&message, min(MAX_MESSAGE_LENGTH, HEADER_SIZE + length), broadcast);
	//RF24::startListening();

	debug(PSTR("send: %d-%d-%d-%d s=%d,c=%d,t=%d,pt=%d,l=%d,st=%s:%s\n"),
		message.sender, message.last, next, message.destination, message.sensor, mGetCommand(message), message.type, mGetPayloadType(message), mGetLength(message), ok ? "ok" : "fail", message.getString(convBuf));

	return ok;
}
bool DTCNode::sendRoute(DTCMessage &message)
{
	// Make sure to process any incoming messages before sending (could this end up in recursive loop?)
	// process();

	// If we still don't have any node id, re-request and skip this message.
	if (nc.nodeId == AUTO && !(mGetCommand(message) == C_INTERNAL && message.type == I_ID_REQUEST))
		requestNodeId();
	else if (!isGateway)
		return sendWrite(GATEWAY_ADDRESS, message);

	return false;
}
bool DTCNode::send(DTCMessage &message)
{
	message.sender = nc.nodeId;
	mSetCommand(message, C_SET);
	return sendRoute(message);
}

void DTCNode::sendBatteryLevel(uint8_t value)
{
	sendRoute(build(msg, nc.nodeId, GATEWAY_ADDRESS, NODE_SENSOR_ID, C_INTERNAL, I_BATTERY_LEVEL).set(value));
}
void DTCNode::present(uint8_t childSensorId, uint8_t sensorType)
{
	sendRoute(build(msg, nc.nodeId, GATEWAY_ADDRESS, childSensorId, C_PRESENTATION, sensorType).set(LIBRARY_VERSION));
}
void DTCNode::sendSketchInfo(const char *name, const char *version)
{
	if (name != NULL)
		sendRoute(build(msg, nc.nodeId, GATEWAY_ADDRESS, NODE_SENSOR_ID, C_INTERNAL, I_SKETCH_NAME).set(name));
	if (version != NULL)
		sendRoute(build(msg, nc.nodeId, GATEWAY_ADDRESS, NODE_SENSOR_ID, C_INTERNAL, I_SKETCH_VERSION).set(version));
}

void DTCNode::request(uint8_t childSensorId, uint8_t variableType, uint8_t destination)
{
	sendRoute(build(msg, nc.nodeId, destination, childSensorId, C_REQ, variableType).set(""));
}
void DTCNode::requestTime(void(*_timeCallback)(unsigned long))
{
	timeCallback = _timeCallback;
	sendRoute(build(msg, nc.nodeId, GATEWAY_ADDRESS, NODE_SENSOR_ID, C_INTERNAL, I_TIME).set(""));
}
void DTCNode::requestNodeId()
{
	debug(PSTR("req node id\n"));
	//RF24::openReadingPipe(CURRENT_NODE_PIPE, TO_ADDR(nc.nodeId));
	sendRoute(build(msg, nc.nodeId, GATEWAY_ADDRESS, NODE_SENSOR_ID, C_INTERNAL, I_ID_REQUEST).set(""));
	wait(2000);
}

bool DTCNode::process()
{
	return false;



	uint8_t pipe;
	boolean available = true;// RF24::available(&pipe);

	if (!available || pipe > 6)
		return false;

	//uint8_t len = RF24::getDynamicPayloadSize();
	//RF24::read(&msg, len);

	// Add string termination, good if we later would want to print it.
	msg.data[mGetLength(msg)] = '\0';
	debug(PSTR("read: %d-%d-%d s=%d,c=%d,t=%d,pt=%d,l=%d:%s\n"),
		msg.sender, msg.last, msg.destination, msg.sensor, mGetCommand(msg), msg.type, mGetPayloadType(msg), mGetLength(msg), msg.getString(convBuf));

	if (!(mGetVersion(msg) == PROTOCOL_VERSION))
	{
		debug(PSTR("version mismatch\n"));
		return false;
	}

	uint8_t command = mGetCommand(msg);
	uint8_t type = msg.type;
	uint8_t sender = msg.sender;
	//uint8_t last = msg.last;
	uint8_t destination = msg.destination;

	if (destination == nc.nodeId)
	{
		// This message is addressed to this node

		if (command == C_INTERNAL)
		{
			if (sender == GATEWAY_ADDRESS)
			{
				bool isMetric;

				if (type == I_REBOOT)
				{
					// Requires DTC or other bootloader with watchdogs enabled
					wdt_enable(WDTO_15MS);
					for (;;);
				}
				else if (type == I_ID_RESPONSE)
				{
					if (nc.nodeId == AUTO)
					{
						nc.nodeId = msg.getByte();
						if (nc.nodeId == AUTO)
						{
							// sensor net gateway will return max id if all sensor id are taken
							debug(PSTR("full\n"));
							while (1); // Wait here. Nothing else we can do...
						}

						setupNode();

						// Write id to EEPROM
						eeprom_write_byte((uint8_t*)EEPROM_NODE_ID_ADDRESS, nc.nodeId);
						debug(PSTR("id=%d\n"), nc.nodeId);
					}
				}
				else if (type == I_CONFIG)
				{
					// Pick up configuration from controller (currently only metric/imperial) and store it in eeprom if changed
					isMetric = msg.getString()[0] == 'M';
					if (cc.isMetric != isMetric)
					{
						cc.isMetric = isMetric;
						eeprom_write_byte((uint8_t*)EEPROM_CONTROLLER_CONFIG_ADDRESS, isMetric);
					}
				}
				else if (type == I_TIME)
				{
					if (timeCallback != NULL)
					{
						// Deliver time to callback
						timeCallback(msg.getULong());
					}
				}

				return false;
			}
		}

		// Call incoming message callback if available
		if (msgCallback != NULL)
			msgCallback(msg);

		// Return true if message was addressed for this node...
		return true;
	}

	return false;
}

void DTCNode::saveState(uint8_t pos, uint8_t value)
{
	if (loadState(pos) != value)
		eeprom_write_byte((uint8_t*)(EEPROM_LOCAL_CONFIG_ADDRESS + pos), value);
}
uint8_t DTCNode::loadState(uint8_t pos)
{
	return eeprom_read_byte((uint8_t*)(EEPROM_LOCAL_CONFIG_ADDRESS + pos));
}

int8_t pinIntTrigger = 0;
void wakeUp()	 //place to send the interrupts
{
	pinIntTrigger = 1;
}
void wakeUp2()	 //place to send the second interrupts
{
	pinIntTrigger = 2;
}

void DTCNode::internalSleep(unsigned long ms)
{
	while (!pinIntTrigger && ms >= 8000) { LowPower.powerDown(SLEEP_8S, ADC_OFF, BOD_OFF); ms -= 8000; }
	if (!pinIntTrigger && ms >= 4000)    { LowPower.powerDown(SLEEP_4S, ADC_OFF, BOD_OFF); ms -= 4000; }
	if (!pinIntTrigger && ms >= 2000)    { LowPower.powerDown(SLEEP_2S, ADC_OFF, BOD_OFF); ms -= 2000; }
	if (!pinIntTrigger && ms >= 1000)    { LowPower.powerDown(SLEEP_1S, ADC_OFF, BOD_OFF); ms -= 1000; }
	if (!pinIntTrigger && ms >= 500)     { LowPower.powerDown(SLEEP_500MS, ADC_OFF, BOD_OFF); ms -= 500; }
	if (!pinIntTrigger && ms >= 250)     { LowPower.powerDown(SLEEP_250MS, ADC_OFF, BOD_OFF); ms -= 250; }
	if (!pinIntTrigger && ms >= 125)     { LowPower.powerDown(SLEEP_120MS, ADC_OFF, BOD_OFF); ms -= 120; }
	if (!pinIntTrigger && ms >= 64)      { LowPower.powerDown(SLEEP_60MS, ADC_OFF, BOD_OFF); ms -= 60; }
	if (!pinIntTrigger && ms >= 32)      { LowPower.powerDown(SLEEP_30MS, ADC_OFF, BOD_OFF); ms -= 30; }
	if (!pinIntTrigger && ms >= 16)      { LowPower.powerDown(SLEEP_15Ms, ADC_OFF, BOD_OFF); ms -= 15; }
}

void DTCNode::wait(unsigned long ms)
{
	// Let serial prints finish (debug, log etc)
	Serial.flush();
	unsigned long enter = millis();
	while (millis() - enter < ms)
	{
		// reset watchdog
		wdt_reset();
		process();
	}
}

void DTCNode::sleep(unsigned long ms)
{
	// Let serial prints finish (debug, log etc)
	Serial.flush();
	//RF24::powerDown();
	pinIntTrigger = 0;
	internalSleep(ms);
}
bool DTCNode::sleep(uint8_t interrupt, uint8_t mode, unsigned long ms)
{
	// Let serial prints finish (debug, log etc)
	bool pinTriggeredWakeup = true;
	Serial.flush();
	//RF24::powerDown();
	attachInterrupt(interrupt, wakeUp, mode);
	if (ms > 0) {
		pinIntTrigger = 0;
		sleep(ms);
		if (0 == pinIntTrigger) {
			pinTriggeredWakeup = false;
		}
	}
	else {
		Serial.flush();
		LowPower.powerDown(SLEEP_FOREVER, ADC_OFF, BOD_OFF);
	}
	detachInterrupt(interrupt);
	return pinTriggeredWakeup;
}
int8_t DTCNode::sleep(uint8_t interrupt1, uint8_t mode1, uint8_t interrupt2, uint8_t mode2, unsigned long ms)
{
	int8_t retVal = 1;
	Serial.flush(); // Let serial prints finish (debug, log etc)
	//RF24::powerDown();
	attachInterrupt(interrupt1, wakeUp, mode1);
	attachInterrupt(interrupt2, wakeUp2, mode2);
	if (ms > 0) {
		pinIntTrigger = 0;
		sleep(ms);
		if (0 == pinIntTrigger) {
			retVal = -1;
		}
	}
	else {
		Serial.flush();
		LowPower.powerDown(SLEEP_FOREVER, ADC_OFF, BOD_OFF);
	}
	detachInterrupt(interrupt1);
	detachInterrupt(interrupt2);

	if (1 == pinIntTrigger) {
		retVal = (int8_t)interrupt1;
	}
	else if (2 == pinIntTrigger) {
		retVal = (int8_t)interrupt2;
	}
	return retVal;
}

#ifdef DEBUG
void DTCNode::debugPrint(const char *fmt, ...)
{
	char fmtBuffer[300];

	if (isGateway)
	{
		// prepend debug message to be handled correctly by gw (C_INTERNAL, I_LOG_MESSAGE)
		snprintf_P(fmtBuffer, 299, PSTR("0;0;%d;%d;"), C_INTERNAL, I_LOG_MESSAGE);
		Serial.print(fmtBuffer);
	}
	va_list args;
	va_start(args, fmt);
	va_end(args);
	if (isGateway)
	{
		// Truncate message if this is gateway node
		vsnprintf_P(fmtBuffer, 60, fmt, args);
		fmtBuffer[59] = '\n';
		fmtBuffer[60] = '\0';
	}
	else {
		vsnprintf_P(fmtBuffer, 299, fmt, args);
	}
	va_end(args);
	Serial.print(fmtBuffer);
	Serial.flush();

	//Serial.write(freeRam());
}
int DTCNode::freeRam()
{
	extern int __heap_start, *__brkval;
	int v;
	return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}
#endif
