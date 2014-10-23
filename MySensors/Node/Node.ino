#include <MySensor.h>
#include <SPI.h>
 
#define ID 1
#define OPEN 1
#define CLOSE 0
 
//MySensor gw;
//MyMessage msg(ID, V_TRIPPED);
 
void setup() 
{ 
	//gw.begin();
	//gw.present(ID, S_DOOR);

	Serial.begin(115200);
}
 
void loop()
{
	//gw.send(msg.set(OPEN));
	//delay(10000); // Wait 10 seconds

	Serial.print("12;6;1;0;0;36.5");
	Serial.print('\n');
	delay(1000);
}