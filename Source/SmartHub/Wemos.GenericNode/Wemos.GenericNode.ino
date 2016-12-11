#include <WemosNode.h>
#include <WemosModuleRelay.h>
#include <SFE_MicroOLED.h>

//ADC_MODE(ADC_VCC);
WemosNode node(2);

#define LAST_MS	0//-100000
unsigned long prevMsTime = LAST_MS;
const unsigned long intervalTime = 1000;

//#define PIN_RESET 255  //
//#define DC_JUMPER 0  // I2C Addres: 0 - 0x3C, 1 - 0x3D
//MicroOLED oled(PIN_RESET, DC_JUMPER); // Example I2C declaration

void setup()
{
	node.addModule(new WemosModuleRelay(0, "Relay 0", D1));
	//node.addModule();
	//node.addModule();

	node.begin();
	node.sendFirmwareInfo("Test Node", 1.0);

	//oled.begin();
	//oled.clear(ALL);
	//oled.setFontType(0);  // Set the text to small (10 columns, 6 rows worth of characters).
	////oled.setFontType(1);  // Set the text to medium (6 columns, 3 rows worth of characters).
	////oled.setFontType(2);  // Set the text to medium/7-segment (5 columns, 3 rows worth of characters).
	////oled.setFontType(3);  // Set the text to large (5 columns, 1 row worth of characters).
	//oled.display();
}

void loop()
{
	node.process();

	if (node.hasIntervalElapsed(&prevMsTime, intervalTime))
	{
		//String t = "", d = "";
		//if (node.getDateTimeString(d, t))
		//{
		//	//Serial.println(d +" " + t);

		//	oled.clear(PAGE);
		//	oled.setCursor(0, 0);
		//	oled.println(d);
		//	oled.println(t);
		//	oled.display();
		//}
	}
}
