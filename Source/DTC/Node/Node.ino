#include <DTCNode.h>
 
#define ID 0
#define OPEN 1
#define CLOSE 0
 
DTCNode node;
DTCMessage msg(ID, V_TRIPPED);
 
void setup() 
{ 
	node.begin(msgCallback, AUTO, 6);

	node.sendSketchInfo("Sample Node", "1.0");
	node.present(ID, S_DOOR);

	Serial.begin(115200);
}
 
void msgCallback(const DTCMessage & msg)
{

}

void loop()
{
	//process(); // call if this is repeater or actuator

	//sendBatteryLevel(67); // in %
	//node.send(msg.set(OPEN));

	//delay(10000); // Wait 10 seconds

	Serial.print("12;6;1;0;0;36.5");
	Serial.print('\n');
	delay(1000);
}