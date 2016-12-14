#include <Wemos.h>
#include <SFE_MicroOLED.h>

//ADC_MODE(ADC_VCC);
WemosNode node(2);

unsigned long prevMsTime = 0;
const unsigned long intervalTime = 1000;

#define PIN_RESET 255
#define DC_JUMPER 0  // I2C Address: 0 - 0x3C, 1 - 0x3D
MicroOLED oled(PIN_RESET, DC_JUMPER); // Example I2C declaration

void setup()
{
	oled.begin();
	oled.clear(ALL);
	oled.setFontType(0);  // Set the text to small (10 columns, 6 rows worth of characters).
	//oled.setFontType(1);  // Set the text to medium (6 columns, 3 rows worth of characters).
	//oled.setFontType(2);  // Set the text to medium/7-segment (5 columns, 3 rows worth of characters).
	//oled.setFontType(3);  // Set the text to large (5 columns, 1 row worth of characters).
	oled.display();

	node.addModule(new WemosModuleRelay(0, D1));
	node.addModule(new WemosModuleDHT(1, 2, 10000, WemosModuleDHTType::WMDHT22));

	node.setOutMsgCallback(onOutMessage);

	node.begin();
	node.sendFirmwareInfo("Test Node", 1.0);
}

String tt = "", dt = "";
float t = 0, h = 0;
void loop()
{
	node.process();

	if (node.hasIntervalElapsed(&prevMsTime, intervalTime))
		if (node.getDateTimeString(dt, tt))
		{
			//Serial.println(dt + " " + tt);
		}

	oled.clear(PAGE);
	oled.setCursor(0, 0);
	oled.println(dt);
	oled.println(tt);
	oled.println();
	oled.println("T: " + String(t, 1) + " C");
	oled.println("H: " + String(h, 1) + " %");
	oled.display();
}

void onOutMessage(const WemosMessage& msg)
{
	if (msg.subType == WemosLineType::WLT_Temperature)
		t = msg.getFloat();
	if (msg.subType == WemosLineType::WLT_Humidity)
		h = msg.getFloat();
}