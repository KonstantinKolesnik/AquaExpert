/*
We use Arduino Nano IO shield.
It uses DEFAULT_CE_PIN and DEFAULT_CS_PIN for connection
*/
//--------------------------------------------------------------------------------------------------------------------------------------------
#include <OneWire.h>
#include <SPI.h>
#include <MySensor.h>
#include <DallasTemperature.h>
#include <NewPing.h>
//#include <Ultrasonic.h>
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
float lastTemperature = 0;
unsigned long prevMsTemperature = 0;
const unsigned long intervalTemperature = 60000;	// interval at which to measure (milliseconds)

//--------------------------------------------------------------------------------------------------------------------------------------------
#define PH_SENSOR_ID			9
#define PH_PIN					A2
#define PH_OFFSET				0.2f//-0.12
MyMessage msgPh(PH_SENSOR_ID, V_PH);
float lastPh = 0;
unsigned long prevMsPh = 0;
const unsigned long intervalPh = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define WATERLEAK_SENSOR_ID		10
#define WATER_PIN				A3
MyMessage msgWater(WATERLEAK_SENSOR_ID, V_TRIPPED);
bool lastWater = false;
unsigned long prevMsWater = 0;
const unsigned long intervalWater = 10000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define DISTANCE_SENSOR_ID		11
#define TRIGGER_PIN				A5  // Arduino pin tied to Trigger pin on the ultrasonic sensor.
#define ECHO_PIN				A4  // Arduino pin tied to Echo pin on the ultrasonic sensor.
#define MAX_DISTANCE			200 // Maximum distance to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.
NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
//Ultrasonic ultrasonic(TRIGGER_PIN, ECHO_PIN);
MyMessage msgDistance(DISTANCE_SENSOR_ID, V_DISTANCE);
//float lastDistance = -1;
uint16_t lastDistance = 0;
unsigned long prevMsDistance = 0;
const unsigned long intervalDistance = 10000;

//--------------------------------------------------------------------------------------------------------------------------------------------
MySensor gw;

//MyTransportNRF24 transport(RF24_CE_PIN, RF24_CS_PIN);
//MySensor gw(transport);

//MyHwATMega328 hw;
//MySensor gw(transport,
//	hw,
//	//signer, /* if MY_SIGNING_FEATURE is enabled */
//	DEFAULT_RX_LED_PIN, DEFAULT_TX_LED_PIN, DEFAULT_ERR_LED_PIN /* if WITH_LEDS_BLINKING is enabled */
//	);
bool isMetric = true;
//unsigned long SLEEP_TIME = 0; //3000;	// sleep time between reads (in milliseconds)

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
		if (lastState == 255)
		{
			lastState = 0;
			gw.saveState(RELAY_SENSOR_ID, lastState);
		}
		digitalWrite(pin, lastState ? RELAY_ON : RELAY_OFF);

		gw.present(RELAY_SENSOR_ID, S_LIGHT);
		gw.send(msgRelay.setSensor(RELAY_SENSOR_ID).set(lastState ? 1 : 0));
	}

	dallas.begin();
	pinMode(WATER_PIN, INPUT);

	gw.present(TEMPERATURE_SENSOR_ID, S_TEMP, "Water temperature");
	gw.present(PH_SENSOR_ID, S_PH, "Water PH level");
	gw.present(WATERLEAK_SENSOR_ID, S_WATER_LEAK, "Water leak");
	gw.present(DISTANCE_SENSOR_ID, S_DISTANCE, "Water distance");
}
void loop()
{
	processTemperature();
	gw.process();

	processPH();
	gw.process();

	processWaterLeak();
	gw.process();

	processDistance();
	gw.process();

	//gw.requestTime(onTimeReceived);

	//if (SLEEP_TIME > 0)
	//	gw.sleep(SLEEP_TIME);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void processTemperature()
{
	if (hasIntervalElapsed(&prevMsTemperature, intervalTemperature))
	{
		float temperature = roundFloat(readTemperature(), 1);

		if (temperature != -127.00)
		{
			if (lastTemperature != temperature)
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
	if (hasIntervalElapsed(&prevMsPh, intervalPh))
	{
		float ph = roundFloat(readPh(), 1);

		if (ph != lastPh)
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
void processWaterLeak()
{
	if (hasIntervalElapsed(&prevMsWater, intervalWater))
	{
		bool leak = readWater();

		if (leak != lastWater)
		{
			lastWater = leak;
			gw.send(msgWater.set(leak ? 1 : 0));

#ifdef DEBUG
			Serial.print("Water leak: ");
			Serial.println(water ? "Yes" : "No");
#endif
		}
	}
}
void processDistance()
{
	if (hasIntervalElapsed(&prevMsDistance, intervalDistance))
	{
		uint16_t distance = ceil(readDistance());
		
		//float distance = roundFloat(readDistanceUltrasonic(), 1);
		//uint16_t distance = ceil(readDistanceUltrasonic());

		if (distance > 0) // (0 = outside distance range)
		{
			if (distance != lastDistance)
			{
				lastDistance = distance;
				//gw.send(msgDistance.set(distance, 1));
				gw.send(msgDistance.set(distance));

#ifdef DEBUG
				Serial.print("Distance: ");
				Serial.print(distance, 1);
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
		else if (message.sensor == WATERLEAK_SENSOR_ID && message.type == V_TRIPPED)
			gw.send(msgWater.set(lastWater ? 1 : 0));
		else if (message.sensor == DISTANCE_SENSOR_ID && message.type == V_DISTANCE)
			//gw.send(msgDistance.set(lastDistance, 1));
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
				delay(10);
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
	float temperature = isMetric ? dallas.getTempCByIndex(0) : dallas.getTempFByIndex(0);
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
float readDistance()
{
	// 1) Arduino pin tied to both trigger and echo pins on the ultrasonic sensor.
	//delay(50);                      // Wait 50ms between pings (about 20 pings/sec). 29ms should be the shortest delay between pings.
	//unsigned int us = sonar.ping(); // Send ping, get ping time in microseconds (uS).
	//float distance = (us / (isMetric ? US_ROUNDTRIP_CM : US_ROUNDTRIP_IN)); // convert ping time to distance

	// 2)
	//uint16_t distance = isMetric ? sonar.ping_cm() : sonar.ping_in();

	// 3)
	unsigned long us = sonar.ping_median(10); // get 10 sample value from the sensor for smooth the value
	float distance = (us / (isMetric ? US_ROUNDTRIP_CM : US_ROUNDTRIP_IN)); // convert ping time to distance


	return distance;
}
//float readDistanceUltrasonic()
//{
//	// get 10 sample value from the sensor for smooth the value:
//	float buf[10];
//	for (int i = 0; i < 10; i++)
//	{
//		buf[i] = isMetric ? ultrasonic.Ranging(CM) : ultrasonic.Ranging(INC);
//		delay(50);
//	}
//
//	// sort the analog from small to large:
//	for (int i = 0; i < 9; i++)
//	{
//		for (int j = i + 1; j < 10; j++)
//		{
//			if (buf[i] > buf[j])
//			{
//				float temp = buf[i];
//				buf[i] = buf[j];
//				buf[j] = temp;
//			}
//		}
//	}
//
//	// store the average value:
//	float avgValue = 0;
//	for (int i = 2; i < 8; i++) // take the average value of 6 center samples
//		avgValue += buf[i];
//
//	float distance = avgValue / 6;
//
//	return distance;
//}
bool readWater()
{
	// analogRead: moisture sensor:
	// with transistor:		524 for water;  838 for short circuit; (100/100/KT3102)
	// Yusupov (resistors): ~650 for water; ~1000 for short circuit; ~1 for air; (2k / 100k)

	int v = analogRead(WATER_PIN);
	return v > 500;
}
//--------------------------------------------------------------------------------------------------------------------------------------------
float roundFloat(float val, uint8_t dec)
{
	double k = pow(10, dec);
	return (float)(round(val * k) / k);
}
bool hasIntervalElapsed(unsigned long* prevMs, const unsigned long interval)
{
	unsigned long ms = millis();

	if ((unsigned long)(ms - *prevMs) >= interval)
	{
		*prevMs = ms;
		return true;
	}

	return false;
}