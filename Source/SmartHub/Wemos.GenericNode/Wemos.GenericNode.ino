#include <WemosNode.h>
#include <WemosLine.h>
#include <TimeLib.h>

//ADC_MODE(ADC_VCC);
WemosNode node(0);

#define LAST_MS	0//-100000
unsigned long prevMsTime = LAST_MS;
const unsigned long intervalTime = 1000;

void setup()
{
	//node.addLine();
	//node.addLine();
	//node.addLine();

	node.begin();
}

void loop()
{
	node.process();

	if (node.hasIntervalElapsed(&prevMsTime, intervalTime))
	{
		String s = node.getTimeString();
		//if (!s.equals(""))
		if (s != "")
			Serial.println();

		//Serial.print(hour());
		//printDigits(minute());
		//printDigits(second());

		//Serial.print(" ");
		//Serial.print(day());

		//Serial.print("-");
		//Serial.print(month());

		//Serial.print("-");
		//Serial.print(year());

		//Serial.println();
	}
}

void printDigits(int digits)
{
	// utility function for digital clock display: prints preceding colon and leading 0
	Serial.print(":");
	if (digits < 10)
		Serial.print('0');
	Serial.print(digits);
}
