/*
*  ---------- - Connection guide-------------------------------------------------------------------------- -
* 13  Radio SPI SCK
* 12  Radio SPI MISO(SO)
* 11  Radio SPI MOSI(SI)
* 10  Radio CS pin
* A5  Radio CE pin

// LCD Data Bit :    7    6    5    4    3    2    1    0
// Digital pin #:    7    6    5    4    3    2    9    8
// Uno port/pin :  PD7  PD6  PD5  PD4  PD3  PD2  PB1  PB0
// Mega port/pin   PH4	PH3	 PE3  PG5  PE5  PE4  PH6  PH5		
//#define LCD_CS A3 // Chip Select 
//#define LCD_CD A2// Command/Data 
//#define LCD_WR A1// LCD Write 
//#define LCD_RD A0// LCD Read 
//#define LCD_RESET A4 // Can alternately just connect to Arduino's reset pin
*/
//--------------------------------------------------------------------------------------------------------------------------------------------
#include <MySensor.h>
#include <SPI.h>
#include <Adafruit_GFX.h>
#include "SWTFT.h"
//#include <eeprom.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define DISPLAY_SENSOR_ID	0
//MyMessage msgDisplay(DISPLAY_SENSOR_ID, 0);
//--------------------------------------------------------------------------------------------------------------------------------------------
// Assign human-readable names to some common 16-bit color values:
#define	BLACK   0x0000
#define	BLUE    0x001F
#define	RED     0xF800
#define	GREEN   0x07E0
#define CYAN    0x07FF
#define MAGENTA 0xF81F
#define YELLOW  0xFFE0
#define WHITE   0xFFFF

SWTFT tft;
bool isMetric = true;
MySensor gw(A5, DEFAULT_CS_PIN);

#define	LINE_COUNT	10
String lines[LINE_COUNT];
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	//for (int i = 0; i < 512; i++)
	//	EEPROM.write(i, 255);
	//return;

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Display device", "1.0");
	isMetric = gw.getConfig().isMetric;
	gw.present(DISPLAY_SENSOR_ID, S_DISPLAY);

	Serial.begin(115200);

	tft.reset();

	uint16_t identifier = tft.readID();

	//Serial.print(F("LCD driver chip: "));
	//Serial.println(identifier, HEX);

	tft.begin(identifier);
	
	tft.setRotation(3);
	tft.fillScreen(BLACK);
	tft.setTextColor(WHITE);
	tft.setTextSize(2);
	tft.setTextWrap(true);
	tft.setCursor(0, 0);

	tft.println("Waiting for data...");
}
void loop()
{
	gw.process();
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	uint8_t cmd = mGetCommand(message);

	if (cmd == C_SET && message.sensor == DISPLAY_SENSOR_ID)
	{
		uint8_t lineNo = message.type;
		String txt = String((char*)message.getString());
		
		if (lines[lineNo] != txt)
		{
			lines[lineNo] = txt;
			displayText();
		}
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void displayText()
{
	tft.fillScreen(BLACK);
	tft.setCursor(0, 0);

	for (uint8_t lineNo = 0; lineNo < LINE_COUNT; lineNo++)
	{
		String txt = lines[lineNo];
		const char* pTxt = txt.length() > 0 ? txt.c_str() : "";
		
		Serial.println(pTxt);
		tft.println(pTxt);
	}
}
void testText()
{
	tft.fillScreen(BLACK);

	tft.setCursor(0, 0);
	tft.setTextColor(WHITE);
	tft.setTextSize(1);
	tft.println("Hallo, world!");
	
	tft.setTextColor(YELLOW);
	tft.setTextSize(2);
	tft.println(1234.56);
	
	tft.setTextColor(RED);
	tft.setTextSize(3);
	tft.println(0xDEADBEEF, HEX);

	tft.println();

	tft.setTextColor(GREEN);

	tft.setTextSize(6);
	tft.println("Groop");

	tft.setTextSize(2);
	tft.println("I implore thee,");

	tft.setTextSize(1);
	tft.println("my foonting turlingdromes.");
	tft.println("And hooptiously drangle me");
	tft.println("with crinkly bindlewurdles,");
	tft.println("Or I will rend thee");
	tft.println("in the gobberwarts");
	tft.println("with my blurglecruncheon,");
	tft.println("see if I don't!");
}