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
#define NUMBER_OF_LINES		5

typedef struct
{
	uint8_t id;
	uint8_t sensorType;
	uint8_t valueType;
	float lastValue = 0;
	unsigned long prevMs = 0;
	long interval = 3000;
	MyMessage msg;
} line;

line lines[NUMBER_OF_LINES];
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

	lines[0].sensorType = S_TEMP;
	lines[1].sensorType = S_HUM;
	lines[2].sensorType = S_TEMP;
	lines[3].sensorType = S_HUM;
	lines[4].sensorType = S_BARO;





	for (int sensorID = 0; sensorID < NUMBER_OF_LINES; sensorID++)
	{
		lines[sensorID].id = sensorID;
		lines[sensorID].msg.setSensor(sensorID).setType(lines[sensorID].valueType);
		gw.present(sensorID, lines[sensorID].sensorType);
	}






	gw.present(TEMPERATURE_OUTER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_OUTER_SENSOR_ID, S_HUM);
	gw.present(TEMPERATURE_INNER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_INNER_SENSOR_ID, S_HUM);
	gw.present(PRESSURE_SENSOR_ID, S_BARO);
}

void loop()
{
	gw.process();

	unsigned long ms = millis();

	processTemperatureOuter(ms);
	processHumidityOuter(ms);
	processTemperatureInner(ms);
	processHumidityInner(ms);
	processPressure(ms);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	uint8_t cmd = mGetCommand(message);
	if (cmd == C_REQ)
	{
		if (message.sensor == TEMPERATURE_OUTER_SENSOR_ID && message.type == V_TEMP)
		{
			delay(dhtOuter.getMinimumSamplingPeriod());

			float temperature = dhtOuter.getTemperature();
			if (!isnan(temperature))
			{
				if (!isMetric)
					temperature = dhtOuter.toFahrenheit(temperature);

#ifdef DEBUG
				Serial.print("Temperature Outer: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");
#endif

				gw.send(msgTemperatureOuter.set(temperature, 1));
			}
		}
		else if (message.sensor == HUMIDITY_OUTER_SENSOR_ID && message.type == V_PRESSURE)
		{


		}
		else if (message.sensor == TEMPERATURE_INNER_SENSOR_ID && message.type == V_TEMP)
		{
			delay(dhtInner.getMinimumSamplingPeriod());

			float temperature = dhtInner.getTemperature();
			if (!isnan(temperature))
			{
				if (!isMetric)
					temperature = dhtInner.toFahrenheit(temperature);

#ifdef DEBUG
				Serial.print("Temperature Inner: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");
#endif

				gw.send(msgTemperatureInner.set(temperature, 1));
			}
		}
		else if (message.sensor == HUMIDITY_INNER_SENSOR_ID && message.type == V_PRESSURE)
		{


		}

	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void processTemperatureOuter(unsigned long ms)
{
	if (ms - prevMsTemperatureOuter >= intervalTemperatureOuter)
	{
		prevMsTemperatureOuter = ms;

		delay(dhtOuter.getMinimumSamplingPeriod());

		float temperature = dhtOuter.getTemperature();
		if (!isnan(temperature))
		{
			if (lastTemperatureOuter != temperature)
			{
				lastTemperatureOuter = temperature;

				if (!isMetric)
					temperature = dhtOuter.toFahrenheit(temperature);

#ifdef DEBUG
				Serial.print("Temperature Outer: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");
#endif

				gw.send(msgTemperatureOuter.set(temperature, 1));
			}
		}
#ifdef DEBUG
		else
			Serial.println("Failed reading Temperature Outer");
#endif
	}
}
void processHumidityOuter(unsigned long ms)
{
	if (ms - prevMsHumidityOuter >= intervalHumidityOuter)
	{
		prevMsHumidityOuter = ms;

		delay(dhtOuter.getMinimumSamplingPeriod());

		float humidity = dhtOuter.getHumidity();
		if (!isnan(humidity))
		{
			if (lastHumidityOuter != humidity)
			{
				lastHumidityOuter = humidity;

#ifdef DEBUG
				Serial.print("Humidity Outer: ");
				Serial.print(humidity, 1);
				Serial.println("%");
#endif

				gw.send(msgHumidityOuter.set(humidity, 1));
			}
		}
#ifdef DEBUG
		else
			Serial.println("Failed reading Humidity Outer");
#endif
	}
}
void processTemperatureInner(unsigned long ms)
{
	if (ms - prevMsTemperatureInner >= intervalTemperatureInner)
	{
		prevMsTemperatureInner = ms;

		delay(dhtInner.getMinimumSamplingPeriod());

		float temperature = dhtInner.getTemperature();
		if (!isnan(temperature))
		{
			if (lastTemperatureInner != temperature)
			{
				lastTemperatureInner = temperature;

				if (!isMetric)
					temperature = dhtInner.toFahrenheit(temperature);

#ifdef DEBUG
				Serial.print("Temperature Inner: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");
#endif

				gw.send(msgTemperatureInner.set(temperature, 1));
			}
		}
#ifdef DEBUG
		else
			Serial.println("Failed reading Temperature Inner");
#endif

	}
}
void processHumidityInner(unsigned long ms)
{
	if (ms - prevMsHumidityInner >= intervalHumidityInner)
	{
		prevMsHumidityInner = ms;

		delay(dhtInner.getMinimumSamplingPeriod());

		float humidity = dhtInner.getHumidity();
		if (!isnan(humidity))
		{
			if (lastHumidityInner != humidity)
			{
				lastHumidityInner = humidity;

#ifdef DEBUG
				Serial.print("Humidity Inner: ");
				Serial.print(humidity, 1);
				Serial.println("%");
#endif

				gw.send(msgHumidityInner.set(humidity, 1));
			}
		}
#ifdef DEBUG
		else
			Serial.println("Failed reading Humidity Inner");
#endif
	}
}
void processPressure(unsigned long ms)
{
	if (ms - prevMsPressure >= intervalPressure)
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
//
//#ifdef DEBUG
//				Serial.print("Temperature Outer: ");
//				Serial.print(temperature, 1);
//				Serial.println(isMetric ? " C" : " F");
//#endif
//
//				gw.send(msgTemperatureOuter.set(temperature, 1));
//			}
//		}
//#ifdef DEBUG
//		else
//			Serial.println("Failed reading Temperature Outer");
//#endif
	}
}
