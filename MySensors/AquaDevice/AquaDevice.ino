#include <MySensor.h>
#include <SPI.h>
#include <DallasTemperature.h>
#include <OneWire.h>
#include <NewPing.h>
//#include <DS3232RTC.h>  // DS3231/DS3232 library
//#include <Time.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
// Relays section (id=0...7)
#define RELAY_ON				0  // GPIO value to write to turn on attached relay
#define RELAY_OFF				1  // GPIO value to write to turn off attached relay
#define NUMBER_OF_RELAYS		8  // Total number of attached relays
uint8_t relayPins[NUMBER_OF_RELAYS] = { 2, 3, 4, 5, 6, 7, 8, 9 };
MyMessage msgRelay(0, V_LIGHT);
//--------------------------------------------------------------------------------------------------------------------------------------------
// Temperature section (id=8)
#define ONE_WIRE_BUS			A5
OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);
float lastTemperature;
MyMessage msgTemperature(8, V_TEMP);
unsigned long prevMsTemperature = 0;
const long intervalTemperature = 3000;	// interval at which to send measurements (milliseconds)
//--------------------------------------------------------------------------------------------------------------------------------------------
// Ph section (id=9)
#define PH_PIN  A4
#define PH_OFFSET 0.3f//-0.12
MyMessage msgPh(9, V_VAR1);
float lastPh;
unsigned long prevMsPh = 0;
const long intervalPh = 5000;
//--------------------------------------------------------------------------------------------------------------------------------------------
// Moisture section (id=10)
#define WATER_PIN  A3
MyMessage msgWater(10, V_TRIPPED);
bool lastWater;
unsigned long prevMsWater = 0;
const long intervalWater = 5000;
//--------------------------------------------------------------------------------------------------------------------------------------------
// Sonar section (id=11)
#define TRIGGER_PIN  A2  // Arduino pin tied to trigger pin on the ultrasonic sensor.
#define ECHO_PIN     A1  // Arduino pin tied to echo pin on the ultrasonic sensor.
#define MAX_DISTANCE 100 // Maximum distance to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.
NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
MyMessage msgSonar(11, V_DISTANCE);
int lastDist;
unsigned long prevMsSonar = 0;
const long intervalSonar = 1000;
//--------------------------------------------------------------------------------------------------------------------------------------------
// common section
MySensor gw(A0/*DEFAULT_CE_PIN*/, DEFAULT_CS_PIN);
bool isMetric = true;
//unsigned long SLEEP_TIME = 3000;	// Sleep time between reads (in milliseconds)
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	Serial.begin(115200);

	sensors.begin();

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Aquarium Controller", "1.0");

	isMetric = gw.getConfig().isMetric;

	//(sensorID = 0...7)
	for (int sensorID = 0; sensorID < NUMBER_OF_RELAYS; sensorID++)
	{
		pinMode(relayPins[sensorID], OUTPUT);
		uint8_t lastState = gw.loadState(sensorID);
		digitalWrite(relayPins[sensorID], lastState ? RELAY_ON : RELAY_OFF);

		gw.present(sensorID, S_LIGHT);
		gw.send(msgRelay.setSensor(sensorID).set(lastState ? 1 : 0));
	}

	//for (int sensorID = 0; sensorID < NUMBER_OF_RELAYS; sensorID++)
	//	digitalWrite(relayPins[sensorID], RELAY_OFF);
	//for (int sensorID = 0; sensorID < 3; sensorID++)
	//	digitalWrite(relayPins[sensorID], RELAY_ON);



	//(sensorID = 8)
	gw.present(8, S_TEMP);
	gw.send(msgTemperature.set(readTemperature(), 1));

	//(sensorID = 9)
	gw.present(9, S_PH);
	gw.send(msgPh.set(readPh(), 1));

	//(sensorID = 10)
	pinMode(WATER_PIN, INPUT);
	gw.present(10, S_WATER);
	gw.send(msgWater.set(readWater() ? 1 : 0));

	//(sensorID = 11)
	gw.present(11, S_DISTANCE);
	gw.send(msgSonar.set(readDistance()));
}
void loop()
{
	gw.process();

	unsigned long ms = millis();

	// temperature
	if (ms - prevMsTemperature >= intervalTemperature)
	{
		prevMsTemperature = ms;

		float temperature = readTemperature();
		if (lastTemperature != temperature && temperature != -127.00)
		{
			Serial.print("Temperature: ");
			Serial.print(temperature);
			Serial.println(isMetric ? " C" : " F");

			lastTemperature = temperature;
			gw.send(msgTemperature.set(temperature, 1));
		}
	}

	// Ph:
	if (ms - prevMsPh >= intervalPh)
	{
		prevMsPh = ms;

		float phValue = readPh();
		if (phValue != lastPh)
		{
			Serial.print("pH: ");
			Serial.println(phValue, 2);

			lastPh = phValue;
			gw.send(msgPh.set(phValue, 2));
		}
	}

	// Water:
	if (ms - prevMsWater >= intervalWater)
	{
		prevMsWater = ms;

		bool water = readWater();
		if (water != lastWater)
		{
			Serial.print("Water: ");
			Serial.println(water);

			lastWater = water;
			gw.send(msgWater.set(water ? 1 : 0));
		}
	}

	// Distance
	if (ms - prevMsSonar >= intervalSonar)
	{
		prevMsSonar = ms;

		int distance = readDistance();
		if (distance != lastDist)
		{
			Serial.print("Distance: ");
			Serial.print(distance); // Convert ping time to distance in cm and print result (0 = outside set distance range)
			Serial.println(isMetric ? " cm" : " in");

			lastDist = distance;
			gw.send(msgSonar.set(distance));
		}
	}

	//gw.requestTime(onTimeReceived);

	//gw.sleep(SLEEP_TIME);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	//Serial.println("onMessageReceived");

	if (message.type == V_LIGHT)
	{
		bool value = message.getBool();

		digitalWrite(relayPins[message.sensor], value ? RELAY_ON : RELAY_OFF);
		gw.saveState(message.sensor, value);

		gw.send(msgRelay.setSensor(message.sensor).set(value));

		Serial.print("Incoming change for sensor:");
		Serial.print(message.sensor);
		Serial.print(", new status: ");
		Serial.println(value);
	}
}
void onTimeReceived(unsigned long time) //Incoming argument is seconds since 1970.
{
	//setTime(time);
	Serial.print("Time: ");
	Serial.println(time);
}
void printTime() {
	//sprintf(timeBuf, "%02d:%02d:%02d", hour(), minute(), second());
	//myGLCD.print(timeBuf, 60, 7);
}

float readTemperature()
{
	sensors.requestTemperatures();
	float temperature = static_cast<float>(static_cast<int>((isMetric ? sensors.getTempCByIndex(0) : sensors.getTempFByIndex(0)) * 10.)) / 10.;
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
	for (int i = 2; i < 8; i++) // take the average value of 6 center sample
		avgValue += buf[i];

	float phValue = (float)(avgValue * 5.0 / 1024 / 6); // convert the analog into millivolt
	phValue = 3.5 * phValue + PH_OFFSET; // convert the millivolt into pH value

	return phValue;
}
int readDistance()
{
	//delay(50);                      // Wait 50ms between pings (about 20 pings/sec). 29ms should be the shortest delay between pings.

	//unsigned int uS = sonar.ping(); // Send ping, get ping time in microseconds (uS).
	//Serial.print("Distance: ");
	//Serial.print(uS / US_ROUNDTRIP_CM); // Convert ping time to distance in cm and print result (0 = outside set distance range)
	//Serial.println("cm");

	int distance = isMetric ? sonar.ping_cm() : sonar.ping_in();

	return distance;
}
bool readWater()
{
	// analogRead: moisture sensor:
	// transistor: 524 for water;  838 for short circuit; (100/100/KT3102)
	// Yusupov:    ~650 for water; ~1000 for short circuit; ~1 for air; (2k / 100k)

	int v = digitalRead(WATER_PIN);
	return v > 500;
}