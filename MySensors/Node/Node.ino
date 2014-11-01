#include <MySensor.h>
#include <SPI.h>
 
#define ID 0
#define OPEN 1
#define CLOSE 0
 
//MySensor gw;
//MyMessage msg(ID, V_TRIPPED);
 
void setup() 
{ 
	//begin(msgCallback, AUTO, false, AUTO, RF24_PA_MAX, 76, RF24_250KBPS);

	//gw.sendSketchInfo("Node", "1.0");
	//gw.present(ID, S_DOOR);

	Serial.begin(115200);
}
 
void msgCallback (const MyMessage & msg)
{

}

void loop()
{
	//process(); // call if this is repeater or actuator

	//sendBatteryLevel(67); // in %
	//gw.send(msg.set(OPEN));

	//delay(10000); // Wait 10 seconds

	Serial.print("12;6;1;0;0;36.5");
	Serial.print('\n');
	delay(1000);
}