/*
*  ---------- - Connection guide-------------------------------------------------------------------------- -
* 13  Radio & Baro SPI SCK
* 12  Radio & Baro SPI MISO(SO)
* 11  Radio & Baro SPI MOSI(SI)
* 10  Radio CS pin
* 9   Radio CE pin
* 8   Baro CS pin
* 7   Baro CE pin
*/
//--------------------------------------------------------------------------------------------------------------------------------------------
#include <MySensor.h>
#include <SPI.h>
#include <DHT.h>
#include <eeprom.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_OUTER_SENSOR_ID	0
MyMessage msgTemperatureOuter(TEMPERATURE_OUTER_SENSOR_ID, V_TEMP);
float lastTemperatureOuter;
unsigned long prevMsTemperatureOuter = 0;
const long intervalTemperatureOuter = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_OUTER_SENSOR_ID	1
MyMessage msgHumidityOuter(HUMIDITY_OUTER_SENSOR_ID, V_HUM);
float lastHumidityOuter;
unsigned long prevMsHumidityOuter = 0;
const long intervalHumidityOuter = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_INNER_SENSOR_ID	2
MyMessage msgTemperatureInner(TEMPERATURE_INNER_SENSOR_ID, V_TEMP);
float lastTemperatureInner;
unsigned long prevMsTemperatureInner = 0;
const long intervalTemperatureInner = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_INNER_SENSOR_ID	3
MyMessage msgHumidityInner(HUMIDITY_INNER_SENSOR_ID, V_HUM);
float lastHumidityInner;
unsigned long prevMsHumidityInner = 0;
const long intervalHumidityInner = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define PRESSURE_SENSOR_ID			4
MyMessage msgPressureInner(PRESSURE_SENSOR_ID, V_PRESSURE);
float lastPressure;
unsigned long prevMsPressure = 0;
const long intervalPressure = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
bool isMetric = true;
MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
DHT dhtOuter, dhtInner;
#define DHT_OUTER_PIN	2
#define DHT_INNER_PIN	3

//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	Serial.begin(115200);

	//for (int i = 0; i < 512; i++)
	//	EEPROM.write(i, 255);

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Meteo Station", "1.0");

	isMetric = gw.getConfig().isMetric;

	dhtOuter.setup(DHT_OUTER_PIN);
	dhtInner.setup(DHT_INNER_PIN);

	gw.present(TEMPERATURE_OUTER_SENSOR_ID, S_TEMP);
	processTemperature(true, true);

	gw.present(HUMIDITY_OUTER_SENSOR_ID, S_HUM);
	processHumidity(true, true);

	gw.present(TEMPERATURE_INNER_SENSOR_ID, S_TEMP);
	processTemperature(false, true);

	gw.present(HUMIDITY_INNER_SENSOR_ID, S_HUM);
	processHumidity(false, true);

	gw.present(PRESSURE_SENSOR_ID, S_BARO);
	processPressure(true);
}
void loop()
{
	gw.process();

	processTemperature(true, false);
	processHumidity(true, false);
	processTemperature(false, false);
	processHumidity(false, false);
	processPressure(false);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	uint8_t cmd = mGetCommand(message);

	if (cmd == C_REQ)
	{
		if (message.sensor == TEMPERATURE_OUTER_SENSOR_ID && message.type == V_TEMP)
		{
			float temperature = isMetric ? lastTemperatureOuter : dhtOuter.toFahrenheit(lastTemperatureOuter);
			gw.send(msgTemperatureOuter.set(temperature, 1));
		}
		else if (message.sensor == HUMIDITY_OUTER_SENSOR_ID && message.type == V_HUM)
			gw.send(msgHumidityOuter.set(lastHumidityOuter, 1));
		else if (message.sensor == TEMPERATURE_INNER_SENSOR_ID && message.type == V_TEMP)
		{
			float temperature = isMetric ? lastTemperatureInner : dhtInner.toFahrenheit(lastTemperatureInner);
			gw.send(msgTemperatureInner.set(temperature, 1));
		}
		else if (message.sensor == HUMIDITY_INNER_SENSOR_ID && message.type == V_HUM)
			gw.send(msgHumidityInner.set(lastHumidityInner, 1));
		else if (message.sensor == PRESSURE_SENSOR_ID && message.type == V_PRESSURE)
		{


		}
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void processTemperature(bool isOuter, bool force)
{
	MyMessage* msg = isOuter ? &msgTemperatureOuter : &msgTemperatureInner;
	DHT* pDht = isOuter ? &dhtOuter : &dhtInner;
	unsigned long* prevMsTemperature = isOuter ? &prevMsTemperatureOuter : &prevMsTemperatureInner;
	long intervalTemperature = isOuter ? intervalTemperatureOuter : intervalTemperatureInner;
	float* lastTemperature = isOuter ? &lastTemperatureOuter : &lastTemperatureInner;

	unsigned long ms = millis();

	if (force || (ms - *prevMsTemperature >= intervalTemperature))
	{
		*prevMsTemperature = ms;

		delay(pDht->getMinimumSamplingPeriod());
		float temperature = pDht->getTemperature();

		if (!isnan(temperature))
		{
			if (force || (*lastTemperature != temperature))
			{
				*lastTemperature = temperature;

				if (!isMetric)
					temperature = pDht->toFahrenheit(temperature);
				gw.send(msg->set(temperature, 1));

#ifdef DEBUG
				Serial.print(isOuter ? "Temperature Outer: " : "Temperature Inner: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");
#endif
			}
		}
#ifdef DEBUG
		else
			Serial.println(isOuter ? "Failed reading Temperature Outer" : "Failed reading Temperature Inner");
#endif
	}
}
void processHumidity(bool isOuter, bool force)
{
	MyMessage* msg = isOuter ? &msgHumidityOuter : &msgHumidityInner;
	DHT* pDht = isOuter ? &dhtOuter : &dhtInner;
	unsigned long* prevMsHumidity = isOuter ? &prevMsHumidityOuter : &prevMsHumidityInner;
	long intervalHumidity = isOuter ? intervalHumidityOuter : intervalHumidityInner;
	float* lastHumidity = isOuter ? &lastHumidityOuter : &lastHumidityInner;

	unsigned long ms = millis();

	if (force || (ms - *prevMsHumidity >= intervalHumidity))
	{
		*prevMsHumidity = ms;

		delay(pDht->getMinimumSamplingPeriod());

		float humidity = pDht->getHumidity();
		if (!isnan(humidity))
		{
			if (force || (*lastHumidity != humidity))
			{
				*lastHumidity = humidity;
				gw.send(msg->set(humidity, 1));

#ifdef DEBUG
				Serial.print(isOuter ? "Humidity Outer: " : "Humidity Inner: ");
				Serial.print(humidity, 1);
				Serial.println("%");
#endif
			}
		}
#ifdef DEBUG
		else
			Serial.println(isOuter ? "Failed reading Humidity Outer" : "Failed reading Humidity Inner");
#endif
	}
}
void processPressure(bool force)
{
	unsigned long ms = millis();

	if (force || (ms - prevMsPressure >= intervalPressure))
	{
		prevMsPressure = ms;

//		delay(dhtOuter.getMinimumSamplingPeriod());
//
//		float temperature = dhtOuter.getTemperature();
//		if (!isnan(temperature))
//		{
//			if (lastTemperatureOuter != temperature)
//			{
//				lastTemperatureOuter = temperature;
//
//				if (!isMetric)
//					temperature = dhtOuter.toFahrenheit(temperature);
//				gw.send(msgTemperatureOuter.set(temperature, 1));
//
//#ifdef DEBUG
//				Serial.print("Temperature Outer: ");
//				Serial.print(temperature, 1);
//				Serial.println(isMetric ? " C" : " F");
//#endif
//			}
//		}
//#ifdef DEBUG
//		else
//			Serial.println("Failed reading Temperature Outer");
//#endif
	}
}
