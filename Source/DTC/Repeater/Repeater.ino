// Example sketch showing how to create a node thay repeates messages from nodes far from gateway back to gateway.
// It is important that nodes that has enabled repeater mode calls node.process() frequently.
// This node should never sleep.

#include <DTCSensor.h>
#include <SPI.h>

DTCSensor node;

void setup()
{
	node.begin(NULL, AUTO, true); // the third argument enables repeater mode
	node.sendSketchInfo("Repeater", "1.0");
}
void loop()
{
	node.process(); // by calling process() you route messages in the background
}
