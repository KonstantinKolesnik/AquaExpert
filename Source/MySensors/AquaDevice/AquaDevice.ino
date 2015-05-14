/*
We use Arduino Nano IO shield.
It uses DEFAULT_CE_PIN and DEFAULT_CS_PIN for connection
*/
//--------------------------------------------------------------------------------------------------------------------------------------------
#include <MySensor.h>
#include <SPI.h>
#include <DallasTemperature.h>
#include <OneWire.h>
#include <NewPing.h>
//#include <DS3232RTC.h>  // DS3231/DS3232 library
//#include <Time.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_SENSOR_ID	0
#define ONE_WIRE_PIN			A5
OneWire oneWire(ONE_WIRE_PIN);
DallasTemperature dallas(&oneWire);
MyMessage msgTemperature(TEMPERATURE_SENSOR_ID, V_TEMP);
float lastTemperature;
unsigned long prevMsTemperature = 0;
const long intervalTemperature = 30000;	// interval at which to measure (milliseconds)

//--------------------------------------------------------------------------------------------------------------------------------------------
#define PH_SENSOR_ID			1
#define PH_PIN					A4
#define PH_OFFSET				0.2f//-0.12
MyMessage msgPh(PH_SENSOR_ID, V_PH);
float lastPh;
unsigned long prevMsPh = 0;
const long intervalPh = 30000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define WATER_SENSOR_ID			2
#define WATER_PIN				A3
MyMessage msgWater(WATER_SENSOR_ID, V_TRIPPED);
bool lastWater;
unsigned long prevMsWater = 0;
const long intervalWater = 5000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define DISTANCE_SENSOR_ID		3
#define TRIGGER_PIN				A2  // Arduino pin tied to Trigger pin on the ultrasonic sensor.
#define ECHO_PIN				A1  // Arduino pin tied to Echo pin on the ultrasonic sensor.
#define MAX_DISTANCE			200 // Maximum distance to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.
NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
MyMessage msgDistance(DISTANCE_SENSOR_ID, V_DISTANCE);
uint16_t lastDistance;
unsigned long prevMsDistance = 0;
const long intervalDistance = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
bool isMetric = true;
unsigned long SLEEP_TIME = 0; //3000;	// sleep time between reads (in milliseconds)

//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	Serial.begin(115200);

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Aquarium Controller", "1.0");

	isMetric = gw.getConfig().isMetric;

	dallas.begin();
	gw.present(TEMPERATURE_SENSOR_ID, S_TEMP);
	processTemperature(true);

	gw.present(PH_SENSOR_ID, S_PH);
	processPH(true);

	pinMode(WATER_PIN, INPUT);
	gw.present(WATER_SENSOR_ID, S_WATER);
	processWater(true);

	gw.present(DISTANCE_SENSOR_ID, S_DISTANCE);
	processDistance(true);
}
void loop()
{
	gw.process();

	processTemperature(false);
	processPH(false);
	processWater(false);
	processDistance(false);

	//gw.requestTime(onTimeReceived);

	if (SLEEP_TIME > 0)
		gw.sleep(SLEEP_TIME);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void processTemperature(bool force)
{
	unsigned long ms = millis();

	if (force || (ms - prevMsTemperature >= intervalTemperature))
	{
		prevMsTemperature = ms;

		float temperature = readTemperature();

		if (temperature != -127.00)
		{
			if (force || (abs(lastTemperature - temperature) >= 0.1f))
			{
				lastTemperature = temperature;
				gw.send(msgTemperature.set(temperature, 1));

#ifdef DEBUG
				Serial.print("Temperature: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");
#endif
			}
		}
	}
}
void processPH(bool force)
{
	unsigned long ms = millis();

	if (force || (ms - prevMsPh >= intervalPh))
	{
		prevMsPh = ms;

		float ph = readPh();

		if (force || (abs(ph - lastPh) >= 0.1f))
		{
			lastPh = ph;
			gw.send(msgPh.set(ph, 1));

#ifdef DEBUG
			Serial.print("pH: ");
			Serial.println(ph, 1);
#endif
		}
	}
}
void processWater(bool force)
{
	unsigned long ms = millis();

	if (force || (ms - prevMsWater >= intervalWater))
	{
		prevMsWater = ms;

		bool water = readWater();

		if (force || (water != lastWater))
		{
			lastWater = water;
			gw.send(msgWater.set(water ? 1 : 0));

#ifdef DEBUG
			Serial.print("Water: ");
			Serial.println(water ? "Yes" : "No");
#endif
		}
	}
}
void processDistance(bool force)
{
	unsigned long ms = millis();

	if (force || (ms - prevMsDistance >= intervalDistance))
	{
		prevMsDistance = ms;

		uint16_t distance = readDistance();

		if (distance != 0)
		{
			if (force || (distance != lastDistance))
			{
				lastDistance = distance;
				gw.send(msgDistance.set(distance));

#ifdef DEBUG
				Serial.print("Distance: ");
				Serial.print(distance); // Convert ping time to distance in cm and print result (0 = outside set distance range)
				Serial.println(isMetric ? " cm" : " in");
#endif
			}
		}
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	uint8_t cmd = mGetCommand(message);

	if (cmd == C_REQ)
	{
		if (message.sensor == TEMPERATURE_SENSOR_ID && message.type == V_TEMP)
			gw.send(msgTemperature.set(lastTemperature, 1));
		else if (message.sensor == PH_SENSOR_ID && message.type == V_PH)
			gw.send(msgPh.set(lastPh, 1));
		else if (message.sensor == WATER_SENSOR_ID && message.type == V_TRIPPED)
			gw.send(msgWater.set(lastWater ? 1 : 0));
		else if (message.sensor == DISTANCE_SENSOR_ID && message.type == V_DISTANCE)
			gw.send(msgDistance.set(lastDistance));
	}
}
void onTimeReceived(unsigned long time) // incoming argument is seconds since 1970.
{
	//setTime(time);
	//Serial.print("Time: ");
	//Serial.println(time);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void printTime()
{
	//sprintf(timeBuf, "%02d:%02d:%02d", hour(), minute(), second());
	//myGLCD.print(timeBuf, 60, 7);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
float readTemperature()
{
	dallas.requestTemperatures();
	float temperature = static_cast<float>(static_cast<int>((isMetric ? dallas.getTempCByIndex(0) : dallas.getTempFByIndex(0)) * 10.)) / 10.;
	return temperature;
}
float readPh()
{
	/*
	(1) Connect equipments according to the graphic, that is, the pH electrode is connected to the BNC connector on the pH meter board,
	and then use the connection lines, the pH meter board is connected to the ananlong port 0 of the Arduino controller.
	When the Arduino controller gets power, you will see the blue LED on board is on.
	(2) Upload the sample code to the Arduino controller.
	(3) Put the pH electrode into the standard solution whose pH value is 7.00, or directly shorten the input of the BNC connector.
	Open the serial monitor of the Arduino IDE, you can see the pH value printed on it, and the error does not exceed 0.3.
	Record the pH value printed, then compared with 7.00, and the difference should be assifned to the "Offset".
	For example, the pH value printed is 6.88, so the difference is 0.12. You should change the "# define Offset 0.00" into "# define Offset 0.12" in your program.
	(4) Put the pH electrode into the pH standard solution whose value is 4.00. Then wait about one minute, adjust the gain potential device,
	let the value stabilise at around 4.00. At this time the acidic calibration has been completed and you can measure the pH value of an acidic solution.
	Note: If you want to measure the pH value of other solution, you must wash the pH electrode first!
	(5) According to the linear characteristics of pH electrode itself, after the above calibration, you can directly measure the pH value of the alkaline solution,
	but if you want to get better accuracy, you can recalibrate it. Alkaline calibration use the standard solution whose pH value is 9.18.
	Also adjust the gain potential device, let the value stabilise at around 9.18. After this calibration, you can measure the pH value of the alkaline solution.
	*/

	// get 10 sample value from the sensor for smooth the value:
	int buf[10];
	for (int i = 0; i < 10; i++)
	{
		buf[i] = analogRead(PH_PIN);
		delay(10);
	}

	// sort the analog from small to large:
	for (int i = 0; i < 9; i++)
	{
		for (int j = i + 1; j < 10; j++)
		{
			if (buf[i] > buf[j])
			{
				int temp = buf[i];
				buf[i] = buf[j];
				buf[j] = temp;
			}
		}
	}

	// store the average value of the sensor feedback:
	unsigned long int avgValue = 0;
	for (int i = 2; i < 8; i++) // take the average value of 6 center samples
		avgValue += buf[i];

	float phValue = (float)(avgValue * 5.0 / 1024 / 6); // convert the analog into millivolt
	phValue = 3.5 * phValue + PH_OFFSET; // convert the millivolt into pH value

	return phValue;
}
uint16_t readDistance()
{
	// 1)
	//delay(50);                      // Wait 50ms between pings (about 20 pings/sec). 29ms should be the shortest delay between pings.
	//unsigned int uS = sonar.ping(); // Send ping, get ping time in microseconds (uS).
	//Serial.print("Distance: ");
	//Serial.print(uS / US_ROUNDTRIP_CM); // Convert ping time to distance in cm and print result (0 = outside set distance range)
	//Serial.println("cm");

	// 2)
	//uint16_t distance = isMetric ? sonar.ping_cm() : sonar.ping_in();
	//return distance;

	// 3)
	// get 10 sample value from the sensor for smooth the value:
	uint16_t buf[10];
	for (int i = 0; i < 10; i++)
	{
		buf[i] = isMetric ? sonar.ping_cm() : sonar.ping_in();
		delay(50);
	}

	// sort the analog from small to large:
	for (int i = 0; i < 9; i++)
	{
		for (int j = i + 1; j < 10; j++)
		{
			if (buf[i] > buf[j])
			{
				uint16_t temp = buf[i];
				buf[i] = buf[j];
				buf[j] = temp;
			}
		}
	}

	// store the average value:
	unsigned long int avgValue = 0;
	for (int i = 2; i < 8; i++) // take the average value of 6 center samples
		avgValue += buf[i];

	uint16_t distance = avgValue / 6;
	return distance;
}
bool readWater()
{
	// analogRead: moisture sensor:
	// transistor:			524 for water;  838 for short circuit; (100/100/KT3102)
	// Yusupov (resistors): ~650 for water; ~1000 for short circuit; ~1 for air; (2k / 100k)

	int v = analogRead(WATER_PIN);
	return v > 500;
}