// Example sketch showing how to create a node thay repeates messages from nodes far from gateway back to gateway. 
// It is important that nodes that has enabled repeater mode calls  gw.process() frequently.
// This node should never sleep.

#include <MySensor.h>
#include <SPI.h>

MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);

void setup()  
{  
	gw.begin(NULL, AUTO, true); // The third argument enables repeater mode.
	gw.sendSketchInfo("Repeater", "1.0");
}
void loop() 
{
	gw.process(); // by calling process() you route messages in the background
}
