#include <WemosNode.h>
#include <WemosShield.h>

WemosNode node;

#define LAST_MS	-100000
unsigned long prevMs = LAST_MS;
const unsigned long interval = 2000;

void setup()
{
	node.addShield();
	node.addShield();
	node.addShield();

	node.begin();
}

void loop()
{
	node.proceed();

	//if (node.hasIntervalElapsed(&prevMs, interval))
	//{

	//}
}
