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
#define NUMBER_OF_RELAYS		8  // Total number of attached relays
#define RELAY_ON				0  // GPIO value to write to turn on attached relay
#define RELAY_OFF				1  // GPIO value to write to turn off attached relay
uint8_t RELAY_SENSOR_ID = 0;	// 0...7
uint8_t pins[NUMBER_OF_RELAYS] = { 2, 3, 4, 5, 6, 7, 8, A0 };
MyMessage msgRelay(RELAY_SENSOR_ID, V_LIGHT);

//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_SENSOR_ID	8
#define ONE_WIRE_PIN			A1
OneWire oneWire(ONE_WIRE_PIN);
DallasTemperature dallas(&oneWire);
MyMessage msgTemperature(TEMPERATURE_SENSOR_ID, V_TEMP);
float lastTemperature = -1000;
unsigned long prevMsTemperature = -1000000;
const long intervalTemperature = 30000;	// interval at which to measure (milliseconds)

//--------------------------------------------------------------------------------------------------------------------------------------------
#define PH_SENSOR_ID			9
#define PH_PIN					A2
#define PH_OFFSET				0.2f//-0.12
MyMessage msgPh(PH_SENSOR_ID, V_PH);
float lastPh = -1000;
unsigned long prevMsPh = -1000000;
const long intervalPh = 30000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define WATER_SENSOR_ID			10
#define WATER_PIN				A3
MyMessage msgWater(WATER_SENSOR_ID, V_TRIPPED);
bool lastWater;
unsigned long prevMsWater = -1000000;
const long intervalWater = 5000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define DISTANCE_SENSOR_ID		11
#define TRIGGER_PIN				A5  // Arduino pin tied to Trigger pin on the ultrasonic sensor.
#define ECHO_PIN				A4  // Arduino pin tied to Echo pin on the ultrasonic sensor.
#define MAX_DISTANCE			200 // Maximum distance to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.
NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
MyMessage msgDistance(DISTANCE_SENSOR_ID, V_DISTANCE);
uint16_t lastDistance = -1;
unsigned long prevMsDistance = -1000000;
const long intervalDistance = 5000;

//--------------------------------------------------------------------------------------------------------------------------------------------
MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
bool isMetric = true;
unsigned long SLEEP_TIME = 0; //3000;	// sleep time between reads (in milliseconds)

//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Aquarium Controller", "1.0");

	isMetric = gw.getConfig().isMetric;

	for (RELAY_SENSOR_ID = 0; RELAY_SENSOR_ID < NUMBER_OF_RELAYS; RELAY_SENSOR_ID++)
	{
		uint8_t pin = pins[RELAY_SENSOR_ID];
		pinMode(pin, OUTPUT);
		uint8_t lastState = gw.loadState(RELAY_SENSOR_ID);
		digitalWrite(pin, lastState ? RELAY_ON : RELAY_OFF);

		gw.present(RELAY_SENSOR_ID, S_LIGHT);
		gw.send(msgRelay.setSensor(RELAY_SENSOR_ID).set(lastState ? 1 : 0));
	}

	dallas.begin();
	pinMode(WATER_PIN, INPUT);

	gw.present(TEMPERATURE_SENSOR_ID, S_TEMP);
	gw.present(PH_SENSOR_ID, S_PH);
	gw.present(WATER_SENSOR_ID, S_WATER);
	gw.present(DISTANCE_SENSOR_ID, S_DISTANCE);
}
void loop()
{
	processTemperature();
	processPH();
	processWater();
	processDistance();

	gw.process();

	//gw.requestTime(onTimeReceived);

	if (SLEEP_TIME > 0)
		gw.sleep(SLEEP_TIME);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void processTemperature()
{
	unsigned long ms = millis();

	if (ms - prevMsTemperature >= intervalTemperature)
	{
		prevMsTemperature = ms;

		float temperature = readTemperature();

		if (temperature != -127.00)
		{
			if (abs(lastTemperature - temperature) >= 0.1f)
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
void processPH()
{
	unsigned long ms = millis();

	if (ms - prevMsPh >= intervalPh)
	{
		prevMsPh = ms;

		float ph = readPh();

		if (abs(ph - lastPh) >= 0.1f)
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
void processWater()
{
	unsigned long ms = millis();

	if (ms - prevMsWater >= intervalWater)
	{
		prevMsWater = ms;

		bool water = readWater();

		if (water != lastWater)
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
void processDistance()
{
	unsigned long ms = millis();

	if (ms - prevMsDistance >= intervalDistance)
	{
		prevMsDistance = ms;

		uint16_t distance = readDistance();

		if (distance > 0)
		{
			if (distance != lastDistance)
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
		if (message.type == V_LIGHT)
			gw.send(msgRelay.setSensor(message.sensor).set(gw.loadState(message.sensor)));
		else if (message.sensor == TEMPERATURE_SENSOR_ID && message.type == V_TEMP)
			gw.send(msgTemperature.set(lastTemperature, 1));
		else if (message.sensor == PH_SENSOR_ID && message.type == V_PH)
			gw.send(msgPh.set(lastPh, 1));
		else if (message.sensor == WATER_SENSOR_ID && message.type == V_TRIPPED)
			gw.send(msgWater.set(lastWater ? 1 : 0));
		else if (message.sensor == DISTANCE_SENSOR_ID && message.type == V_DISTANCE)
			gw.send(msgDistance.set(lastDistance));
	}
	else if (cmd == C_SET)
	{
		if (message.type == V_LIGHT)
		{
			uint8_t lastState = gw.loadState(message.sensor);
			uint8_t newState = message.getByte();

			if (newState != lastState)
			{
				digitalWrite(pins[message.sensor], newState ? RELAY_ON : RELAY_OFF);
				gw.saveState(message.sensor, newState);

				gw.send(msgRelay.setSensor(message.sensor).set(newState));
			}
		}
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
	float t = isMetric ? dallas.getTempCByIndex(0) : dallas.getTempFByIndex(0);
	float temperature = static_cast<float>(static_cast<int>(t * 10.)) / 10.;
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
	// 1) Arduino pin tied to both trigger and echo pins on the ultrasonic sensor.
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
	unsigned int buf[10];
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
				unsigned int temp = buf[i];
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