#include <DTCNode.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define NUMBER_OF_RELAYS		8  // Total number of attached relays
uint8_t pins[NUMBER_OF_RELAYS] = { A0, A1, A2, A3, A4, A5, 4, 2 };

#define RELAY_ON				0  // GPIO value to write to turn on attached relay
#define RELAY_OFF				1  // GPIO value to write to turn off attached relay
DTCMessage msgRelay(0, V_LIGHT);

//#if NUMBER_OF_RELAYS < 8
DTCNode node(DEFAULT_RX_PIN, DEFAULT_TX_PIN);
//#else
//DTCNode node(A0, DEFAULT_TX_PIN);
//#endif
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	node.begin(onMessageReceived);
	node.sendSketchInfo("Power switch 8", "1.0");

	//(sensorID = 0...7)
	for (int sensorID = 0; sensorID < NUMBER_OF_RELAYS; sensorID++)
	{
		pinMode(pins[sensorID], OUTPUT);
		uint8_t lastState = node.loadState(sensorID);
		digitalWrite(pins[sensorID], lastState ? RELAY_ON : RELAY_OFF);

		node.present(sensorID, S_LIGHT);
		node.send(msgRelay.setSensor(sensorID).set(lastState ? 1 : 0));
	}
}
void loop()
{
	node.process();
}
void onMessageReceived(const DTCMessage &message)
{
	if (message.type == V_LIGHT)
	{
		uint8_t newState = message.getByte();
		uint8_t lastState = node.loadState(message.sensor);

		if (newState != lastState)
		{
			digitalWrite(pins[message.sensor], newState ? RELAY_ON : RELAY_OFF);
			node.saveState(message.sensor, newState);

			node.send(msgRelay.setSensor(message.sensor).set(newState));
		}
	}
}
