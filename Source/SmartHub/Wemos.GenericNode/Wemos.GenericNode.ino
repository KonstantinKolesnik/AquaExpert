#include <WemosNode.h>
#include <WemosShield.h>

ADC_MODE(ADC_VCC);
WemosNode node;

#define LAST_MS	-100000
unsigned long prevMs = LAST_MS;
const unsigned long interval = 2000;

void setup()
{
	node.addLine();
	node.addLine();
	node.addLine();

	node.begin();
}

void loop()
{
	node.process();

	//if (node.hasIntervalElapsed(&prevMs, interval))
	//{

	//}
}
