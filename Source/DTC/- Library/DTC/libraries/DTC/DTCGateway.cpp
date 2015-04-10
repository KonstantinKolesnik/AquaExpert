#include "DTCGateway.h"
#include "utility/MsTimer2.h"
#include "utility/PinChangeInt.h"

uint8_t pinRx;
uint8_t pinTx;
uint8_t pinEr;
boolean buttonTriggeredInclusion;
volatile uint8_t countRx;
volatile uint8_t countTx;
volatile uint8_t countErr;
boolean inclusionMode; // keeps track on inclusion mode

DTCGateway::DTCGateway(uint8_t _rxpin, uint8_t _txpin, uint8_t _inclusion_time, uint8_t _inclusion_pin, uint8_t _rx, uint8_t _tx, uint8_t _er) : DTCSensor(_rxpin, _txpin)
{
	pinInclusion = _inclusion_pin;
	inclusionTime = _inclusion_time;
	pinRx = _rx;
	pinTx = _tx;
	pinEr = _er;
}

void DTCGateway::begin(uint8_t channel, void(*inDataCallback)(char*))
{
	Serial.begin(BAUD_RATE);

	isGateway = true;

	if (inDataCallback != NULL)
	{
		useWriteCallback = true;
		dataCallback = inDataCallback;
	}
	else
		useWriteCallback = false;

	nc.nodeId = 0;
	inclusionMode = 0;
	buttonTriggeredInclusion = false;
	countRx = 0;
	countTx = 0;
	countErr = 0;

	// Setup led pins
	pinMode(pinRx, OUTPUT);
	pinMode(pinTx, OUTPUT);
	pinMode(pinEr, OUTPUT);
	digitalWrite(pinRx, LOW);
	digitalWrite(pinTx, LOW);
	digitalWrite(pinEr, LOW);

	// Setup digital in that triggers inclusion mode
	pinMode(pinInclusion, INPUT);
	digitalWrite(pinInclusion, HIGH);

	// Set initial state of leds
	digitalWrite(pinRx, HIGH);
	digitalWrite(pinTx, HIGH);
	digitalWrite(pinEr, HIGH);

	// Start up the radio library
	setupRadio(channel);
	//RF24::openReadingPipe(WRITE_PIPE, BASE_RADIO_ID);
	//RF24::openReadingPipe(CURRENT_NODE_PIPE, BASE_RADIO_ID);
	//RF24::startListening();

	// Add led timer interrupt
	MsTimer2::set(300, ledTimersInterrupt);
	MsTimer2::start();

	// Add interrupt for inclusion button to pin
	PCintPort::attachInterrupt(pinInclusion, startInclusionInterrupt, RISING);

	// Send startup log message on serial
	serial(PSTR("0;0;%d;%d;Gateway startup complete.\n"), C_INTERNAL, I_GATEWAY_READY);
}

void DTCGateway::checkButtonTriggeredInclusion()
{
	if (buttonTriggeredInclusion)
	{
		// Ok, someone pressed the inclusion button on the gateway, start inclusion mode for 1 munute.
		serial(PSTR("0;0;%d;%d;Inclusion started by button.\n"), C_INTERNAL, I_LOG_MESSAGE);
		buttonTriggeredInclusion = false;
		setInclusionMode(true);
	}
}
void DTCGateway::checkInclusionFinished()
{
	if (inclusionMode && millis() - inclusionStartTime > 60000UL * inclusionTime) // inclusionTimeInMinutes minute(s) has passed, stop inclusion mode
		setInclusionMode(false);
}
void DTCGateway::setInclusionMode(boolean newMode)
{
	if (inclusionMode != newMode)
		inclusionMode = newMode;

	// Send back mode change on serial line to ack command
	serial(PSTR("0;0;%d;%d;%d\n"), C_INTERNAL, I_INCLUSION_MODE, inclusionMode ? 1 : 0);

	if (inclusionMode)
		inclusionStartTime = millis();
}

void DTCGateway::processRadioMessage()
{
	if (process())
	{
		// A new message was received from one of the sensors
		DTCMessage message = getLastMessage();
		rxBlink((mGetCommand(message) == C_PRESENTATION && inclusionMode) ? 3 : 1);

		// pass the message to controller
		serial(message);
	}

	checkButtonTriggeredInclusion();
	checkInclusionFinished();
}

uint8_t DTCGateway::h2i(char c)
{
	uint8_t i = 0;
	if (c <= '9')
		i += c - '0';
	else if (c >= 'a')
		i += c - 'a' + 10;
	else
		i += c - 'A' + 10;
	return i;
}

void DTCGateway::parseAndSend(char *commandBuffer)
{
	boolean ok = false;
	char *str, *p, *value = NULL;
	uint8_t bvalue[MAX_PAYLOAD];
	uint8_t blen = 0;
	int i = 0;

	uint16_t destination = 0;
	uint8_t sensor = 0;
	uint8_t command = 0;
	uint8_t type = 0;

	// Extract command data coming on serial line
	for (str = strtok_r(commandBuffer, ";", &p);		// split using semicolon
		str && i < 6;									// loop while str is not null an max 5 times
		str = strtok_r(NULL, ";", &p)					// get subsequent tokens
		)
	{
		switch (i)
		{
			case 0: // Radioid (destination)
				destination = atoi(str);
				break;
			case 1: // Childid
				sensor = atoi(str);
				break;
			case 2: // Message type
				command = atoi(str);
				break;
			case 3: // Data type
				type = atoi(str);
				break;
			case 4: // Variable value
				if (command == C_STREAM)
				{
					blen = 0;
					uint8_t val;
					while (*str)
					{
						val = h2i(*str++) << 4;
						val += h2i(*str++);
						bvalue[blen] = val;
						blen++;
					}
				}
				else
				{
					value = str;
					// Remove ending carriage return character (if it exists)
					uint8_t lastCharacter = strlen(value) - 1;
					if (value[lastCharacter] == '\r')
						value[lastCharacter] = 0;
				}
				break;
		}
		i++;
	}

	if (destination == GATEWAY_ADDRESS && command == C_INTERNAL) // Handle messages directed to gateway
	{
		if (type == I_VERSION) // Request for version
			serial(PSTR("0;0;%d;%d;%s\n"), C_INTERNAL, I_VERSION, LIBRARY_VERSION);
		else if (type == I_INCLUSION_MODE) // Request to change inclusion mode
			setInclusionMode(atoi(value) == 1);
	}
	else
	{
		txBlink(1);

		msg.sender = GATEWAY_ADDRESS;
		msg.destination = destination;
		msg.sensor = sensor;
		msg.type = type;
		mSetCommand(msg, command);
		if (command == C_STREAM)
			msg.set(bvalue, blen);
		else
			msg.set(value);
		ok = sendRoute(msg);
		if (!ok)
			errBlink(1);
	}
}

void DTCGateway::rxBlink(uint8_t cnt)
{
	if (countRx == 255)
		countRx = cnt;
}
void DTCGateway::txBlink(uint8_t cnt)
{
	if (countTx == 255 && !inclusionMode)
		countTx = cnt;
}
void DTCGateway::errBlink(uint8_t cnt)
{
	if (countErr == 255)
		countErr = cnt;
}

void DTCGateway::serial(const char *fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	vsnprintf_P(serialBuffer, MAX_SEND_LENGTH, fmt, args);
	va_end(args);

	Serial.print(serialBuffer);

	if (useWriteCallback) // we have a registered write callback (probably Ethernet)
		dataCallback(serialBuffer);
}
void DTCGateway::serial(DTCMessage &msg)
{
	serial(PSTR("%d;%d;%d;%d;%s\n"), msg.sender, msg.sensor, mGetCommand(msg), msg.type, msg.getString(convBuf));
}


void ledTimersInterrupt()
{
	if (countRx && countRx != 255)
		digitalWrite(pinRx, LOW); // switch led on
	else if (!countRx)
		digitalWrite(pinRx, HIGH); // switching off
	if (countRx != 255)
		countRx--;

	if (countTx && countTx != 255)
		digitalWrite(pinTx, LOW); // switch led on
	else if (!countTx)
		digitalWrite(pinTx, HIGH); // switching off
	if (countTx != 255)
		countTx--;
	else if (inclusionMode)
		countTx = 8;

	if (countErr && countErr != 255)
		digitalWrite(pinEr, LOW); // switch led on
	else if (!countErr)
		digitalWrite(pinEr, HIGH); // switching off
	if (countErr != 255)
		countErr--;
}
void startInclusionInterrupt()
{
	buttonTriggeredInclusion = true;
}
