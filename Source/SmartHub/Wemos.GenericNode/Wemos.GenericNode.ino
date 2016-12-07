//#include <ESP8266WiFi.h>
//#include <WiFiUdp.h>
#include <WemosNode.h>

WemosNode node;

#define LAST_MS						-100000
unsigned long prevMs = LAST_MS;
const unsigned long interval = 2000;


void setup()
{
	node.begin();
}

void loop()
{
	node.proceed();

	if (node.hasIntervalElapsed(&prevMs, interval))
	{

	}
}
