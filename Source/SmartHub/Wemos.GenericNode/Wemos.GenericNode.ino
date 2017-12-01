#include <Wemos.h>
//#include <SFE_MicroOLED.h>

//ADC_MODE(ADC_VCC);
//WemosNode node(2);

WemosNode node(8, "8 Relay Node", 1.0);

#pragma region MyRegion

#pragma endregion


//unsigned long prevMsTime = 0;
//const unsigned long intervalTime = 1000;

//#define PIN_RESET 255
//#define DC_JUMPER 0  // I2C Address: 0 - 0x3C, 1 - 0x3D
//MicroOLED oled(PIN_RESET, DC_JUMPER); // Example I2C declaration

void setup()
{
	//oled.begin();
	//oled.clear(ALL);
	//oled.setFontType(0);  // Set the text to small (10 columns, 6 rows worth of characters).
	////oled.setFontType(1);  // Set the text to medium (6 columns, 3 rows worth of characters).
	////oled.setFontType(2);  // Set the text to medium/7-segment (5 columns, 3 rows worth of characters).
	////oled.setFontType(3);  // Set the text to large (5 columns, 1 row worth of characters).
	//oled.display();

	//node.addModule(new WemosModuleRelay(0, D1));
	//node.addModule(new WemosModuleDHT(1, 2, 10000, WemosModuleDHTType::WMDHT22));

	node.addModule(new WemosModuleRelay(0, D1));
	node.addModule(new WemosModuleRelay(1, D2));
	node.addModule(new WemosModuleRelay(2, D3));
	node.addModule(new WemosModuleRelay(3, D4));
	node.addModule(new WemosModuleRelay(4, D5));
	node.addModule(new WemosModuleRelay(5, D6));
	node.addModule(new WemosModuleRelay(6, D7));
	node.addModule(new WemosModuleRelay(7, D8));

	//node.setOutMsgCallback(onOutMessage);

	node.begin();
	//node.sendFirmwareInfo();
}

//String tt = "", dt = "";
//float t = 0, h = 0;
void loop()
{
	node.process();

	//if (node.hasIntervalElapsed(&prevMsTime, intervalTime))
	//	if (node.getDateTimeString(dt, tt))
	//	{
	//		//Serial.println(dt + " " + tt);
	//	}

	//oled.clear(PAGE);
	//oled.setCursor(0, 0);
	//oled.println(dt);
	//oled.println(tt);
	//oled.println();
	//oled.println("T: " + String(t, 1) + " C");
	//oled.println("H: " + String(h, 1) + " %");
	//oled.display();
}

//void onOutMessage(const WemosMessage& msg)
//{
//	if (msg.subType == WemosLineType::WLT_Temperature)
//		t = msg.getFloat();
//	if (msg.subType == WemosLineType::WLT_Humidity)
//		h = msg.getFloat();
//}
