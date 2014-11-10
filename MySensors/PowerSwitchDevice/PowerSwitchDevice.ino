#include <MySensor.h>
#include <SPI.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define NUMBER_OF_RELAYS		8  // Total number of attached relays
uint8_t pins[NUMBER_OF_RELAYS] = { 2, 3, 4, 5, 6, 7, 8, 9 };

#define RELAY_ON				0  // GPIO value to write to turn on attached relay
#define RELAY_OFF				1  // GPIO value to write to turn off attached relay
MyMessage msgRelay(0, V_LIGHT);

#if NUMBER_OF_RELAYS < 8
MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
#else
MySensor gw(A0, DEFAULT_CS_PIN);
#endif
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	//Serial.begin(115200);

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Power switch 8", "1.0");

	//(sensorID = 0...7)
	for (int sensorID = 0; sensorID < NUMBER_OF_RELAYS; sensorID++)
	{
		pinMode(pins[sensorID], OUTPUT);
		uint8_t lastState = gw.loadState(sensorID);
		digitalWrite(pins[sensorID], lastState ? RELAY_ON : RELAY_OFF);

		gw.present(sensorID, S_LIGHT);
		gw.send(msgRelay.setSensor(sensorID).set(lastState ? 1 : 0));
	}
}

void loop()
{
	gw.process();
}

void onMessageReceived(const MyMessage &message)
{
	//Serial.println("onMessageReceived");

	if (message.type == V_LIGHT)
	{
		bool value = message.getBool();

		digitalWrite(pins[message.sensor], value ? RELAY_ON : RELAY_OFF);
		gw.saveState(message.sensor, value);

		gw.send(msgRelay.setSensor(message.sensor).set(value));

		//Serial.print("Incoming change for relay: ");
		//Serial.print(message.sensor);
		//Serial.print(", new status: ");
		//Serial.println(value);
	}
}
