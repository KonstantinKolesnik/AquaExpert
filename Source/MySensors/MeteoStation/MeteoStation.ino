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
#include <Wire.h>
#include <Adafruit_BMP085.h>
#include <eeprom.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_INNER_SENSOR_ID	0
MyMessage msgTemperatureInner(TEMPERATURE_INNER_SENSOR_ID, V_TEMP);
float lastTemperatureInner;
unsigned long prevMsTemperatureInner = -1000000;
const long intervalTemperatureInner = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_INNER_SENSOR_ID	1
MyMessage msgHumidityInner(HUMIDITY_INNER_SENSOR_ID, V_HUM);
float lastHumidityInner;
unsigned long prevMsHumidityInner = -1000000;
const long intervalHumidityInner = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_OUTER_SENSOR_ID	2
MyMessage msgTemperatureOuter(TEMPERATURE_OUTER_SENSOR_ID, V_TEMP);
float lastTemperatureOuter;
unsigned long prevMsTemperatureOuter = -1000000;
const long intervalTemperatureOuter = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_OUTER_SENSOR_ID	3
MyMessage msgHumidityOuter(HUMIDITY_OUTER_SENSOR_ID, V_HUM);
float lastHumidityOuter;
unsigned long prevMsHumidityOuter = -1000000;
const long intervalHumidityOuter = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define PRESSURE_SENSOR_ID			4
MyMessage msgPressure(PRESSURE_SENSOR_ID, V_PRESSURE);
int32_t lastPressure;
unsigned long prevMsPressure = -1000000;
const long intervalPressure = 60000;

#define FORECAST_SENSOR_ID			5
MyMessage msgForecast(FORECAST_SENSOR_ID, V_FORECAST);
int lastForecast = 5;
const char *weather[] = { "stable", "sunny", "cloudy", "unstable", "thunderstorm", "unknown" };
float pressureSamples[7][6];
float pressureAvg[7];
float dP_dt;
int minuteCount = 0;
bool firstRound = true;

//--------------------------------------------------------------------------------------------------------------------------------------------
bool isMetric = true;
MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
DHT dhtOuter, dhtInner;
#define DHT_INNER_PIN	2
#define DHT_OUTER_PIN	3
Adafruit_BMP085 bmp = Adafruit_BMP085();
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

	if (!bmp.begin())
	{
		Serial.println("Could not find a valid BMP085/BMP180 sensor, check wiring!");
		while (1) {}
	}

	gw.present(TEMPERATURE_INNER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_INNER_SENSOR_ID, S_HUM);
	gw.present(TEMPERATURE_OUTER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_OUTER_SENSOR_ID, S_HUM);
	gw.present(PRESSURE_SENSOR_ID, S_BARO);
	gw.present(FORECAST_SENSOR_ID, S_BARO);
}
void loop()
{
	processTemperature(false);
	processHumidity(false);
	processTemperature(true);
	processHumidity(true);
	processPressure();

	gw.process();
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
			gw.send(msgPressure.set(lastPressure));
		else if (message.sensor == PRESSURE_SENSOR_ID && message.type == V_FORECAST)
			gw.send(msgForecast.set(lastForecast));
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void processTemperature(bool isOuter)
{
	MyMessage* msg = isOuter ? &msgTemperatureOuter : &msgTemperatureInner;
	DHT* pDht = isOuter ? &dhtOuter : &dhtInner;
	unsigned long* prevMsTemperature = isOuter ? &prevMsTemperatureOuter : &prevMsTemperatureInner;
	long intervalTemperature = isOuter ? intervalTemperatureOuter : intervalTemperatureInner;
	float* lastTemperature = isOuter ? &lastTemperatureOuter : &lastTemperatureInner;

	unsigned long ms = millis();

	if (ms - *prevMsTemperature >= intervalTemperature)
	{
		*prevMsTemperature = ms;

		delay(pDht->getMinimumSamplingPeriod());
		float temperature = pDht->getTemperature();

		if (!isnan(temperature))
		{
			if (*lastTemperature != temperature)
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
void processHumidity(bool isOuter)
{
	MyMessage* msg = isOuter ? &msgHumidityOuter : &msgHumidityInner;
	DHT* pDht = isOuter ? &dhtOuter : &dhtInner;
	unsigned long* prevMsHumidity = isOuter ? &prevMsHumidityOuter : &prevMsHumidityInner;
	long intervalHumidity = isOuter ? intervalHumidityOuter : intervalHumidityInner;
	float* lastHumidity = isOuter ? &lastHumidityOuter : &lastHumidityInner;

	unsigned long ms = millis();

	if (ms - *prevMsHumidity >= intervalHumidity)
	{
		*prevMsHumidity = ms;

		delay(pDht->getMinimumSamplingPeriod());

		float humidity = pDht->getHumidity();
		if (!isnan(humidity))
		{
			if (*lastHumidity != humidity)
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
void processPressure()
{
	unsigned long ms = millis();

	if (ms - prevMsPressure >= intervalPressure)
	{
		prevMsPressure = ms;

		//int32_t pressure = bmp.readSealevelPressure(190) / 1000; // 205 meters above sealevel
		//int forecast = sample(pressure);

		//float altitude = bmp.readAltitude();

		//Serial.println(bmp.readSealevelPressure(altitude));
		//Serial.println(bmp.readPressure());
		//Serial.println(bmp.readSealevelPressure(altitude) / 133.3);
		//Serial.println(bmp.readPressure() / 133.3);
		//Serial.println(weather[forecast]);
		//Serial.println(bmp.readTemperature());
		//Serial.println((int16_t)altitude);
		//Serial.println("-------------------------");

		int32_t pressure = bmp.readPressure(); // in Pa

		if (!isnan(pressure))
		{
			if (lastPressure != pressure)
			{
				lastPressure = pressure;
				gw.send(msgPressure.set(pressure));

#ifdef DEBUG
				Serial.print("Pressure: ");
				Serial.print(pressure);
				Serial.println(" Pa");
#endif
			}

			int forecast = sample(pressure / 1000); // in kPa
			if (lastForecast != forecast)
			{
				lastForecast = forecast;
				//gw.send(msgForecast.set(weather[forecast]));
				gw.send(msgForecast.set(forecast));

#ifdef DEBUG
				Serial.print("Forecast: ");
				Serial.println(weather[forecast]);
#endif
			}
		}
#ifdef DEBUG
		else
			Serial.println("Failed reading Pressure");
#endif
	}
}

int sample(float pressure)
{
	// Algorithm found here
	// http://www.freescale.com/files/sensors/doc/app_note/AN3914.pdf
	if (minuteCount == 180)
		minuteCount = 5;

	//From 0 to 5 min.
	if (minuteCount <= 5)
		pressureSamples[0][minuteCount] = pressure;
	//From 30 to 35 min.
	if ((minuteCount <= 30) && (minuteCount >= 35))
		pressureSamples[1][minuteCount - 30] = pressure;
	//From 55 to 60 min.
	if ((minuteCount <= 55) && (minuteCount >= 60))
		pressureSamples[2][minuteCount - 55] = pressure;
	//From 90 to 95 min.
	if ((minuteCount <= 90) && (minuteCount >= 95))
		pressureSamples[3][minuteCount - 90] = pressure;
	//From 115 to 119 min.
	if ((minuteCount <= 115) && (minuteCount >= 120))
		pressureSamples[4][minuteCount - 115] = pressure;
	//From 150 to 155 min.
	if ((minuteCount <= 150) && (minuteCount >= 155))
		pressureSamples[5][minuteCount - 150] = pressure;
	//From 175 to 180 min.
	if ((minuteCount <= 175) && (minuteCount >= 180))
		pressureSamples[6][minuteCount - 175] = pressure;

	minuteCount++;

	if (minuteCount == 5)
	{
		// Avg pressure in first 5 min, value averaged from 0 to 5 min.
		pressureAvg[0] = ((pressureSamples[0][0] + pressureSamples[0][1]
			+ pressureSamples[0][2] + pressureSamples[0][3]
			+ pressureSamples[0][4] + pressureSamples[0][5]) / 6);
	}
	else if (minuteCount == 35)
	{
		// Avg pressure in 30 min, value averaged from 0 to 5 min.
		pressureAvg[1] = ((pressureSamples[1][0] + pressureSamples[1][1]
			+ pressureSamples[1][2] + pressureSamples[1][3]
			+ pressureSamples[1][4] + pressureSamples[1][5]) / 6);
		float change = (pressureAvg[1] - pressureAvg[0]);
		if (firstRound) // first time initial 3 hour
			dP_dt = ((65.0 / 1023.0) * 2 * change); // note this is for t = 0.5hour
		else
			dP_dt = (((65.0 / 1023.0) * change) / 1.5); // divide by 1.5 as this is the difference in time from 0 value.
	}
	else if (minuteCount == 60)
	{
		// Avg pressure at end of the hour, value averaged from 0 to 5 min.
		pressureAvg[2] = ((pressureSamples[2][0] + pressureSamples[2][1]
			+ pressureSamples[2][2] + pressureSamples[2][3]
			+ pressureSamples[2][4] + pressureSamples[2][5]) / 6);
		float change = (pressureAvg[2] - pressureAvg[0]);
		if (firstRound) //first time initial 3 hour
			dP_dt = ((65.0 / 1023.0) * change); //note this is for t = 1 hour
		else
			dP_dt = (((65.0 / 1023.0) * change) / 2); //divide by 2 as this is the difference in time from 0 value
	}
	else if (minuteCount == 95)
	{
		// Avg pressure at end of the hour, value averaged from 0 to 5 min.
		pressureAvg[3] = ((pressureSamples[3][0] + pressureSamples[3][1]
			+ pressureSamples[3][2] + pressureSamples[3][3]
			+ pressureSamples[3][4] + pressureSamples[3][5]) / 6);
		float change = (pressureAvg[3] - pressureAvg[0]);
		if (firstRound) // first time initial 3 hour
			dP_dt = (((65.0 / 1023.0) * change) / 1.5); // note this is for t = 1.5 hour
		else
			dP_dt = (((65.0 / 1023.0) * change) / 2.5); // divide by 2.5 as this is the difference in time from 0 value
	}
	else if (minuteCount == 120)
	{
		// Avg pressure at end of the hour, value averaged from 0 to 5 min.
		pressureAvg[4] = ((pressureSamples[4][0] + pressureSamples[4][1]
			+ pressureSamples[4][2] + pressureSamples[4][3]
			+ pressureSamples[4][4] + pressureSamples[4][5]) / 6);
		float change = (pressureAvg[4] - pressureAvg[0]);
		if (firstRound) // first time initial 3 hour
			dP_dt = (((65.0 / 1023.0) * change) / 2); // note this is for t = 2 hour
		else
			dP_dt = (((65.0 / 1023.0) * change) / 3); // divide by 3 as this is the difference in time from 0 value
	}
	else if (minuteCount == 155)
	{
		// Avg pressure at end of the hour, value averaged from 0 to 5 min.
		pressureAvg[5] = ((pressureSamples[5][0] + pressureSamples[5][1]
			+ pressureSamples[5][2] + pressureSamples[5][3]
			+ pressureSamples[5][4] + pressureSamples[5][5]) / 6);
		float change = (pressureAvg[5] - pressureAvg[0]);
		if (firstRound) // first time initial 3 hour
			dP_dt = (((65.0 / 1023.0) * change) / 2.5); // note this is for t = 2.5 hour
		else
			dP_dt = (((65.0 / 1023.0) * change) / 3.5); // divide by 3.5 as this is the difference in time from 0 value
	}
	else if (minuteCount == 180)
	{
		// Avg pressure at end of the hour, value averaged from 0 to 5 min.
		pressureAvg[6] = ((pressureSamples[6][0] + pressureSamples[6][1]
			+ pressureSamples[6][2] + pressureSamples[6][3]
			+ pressureSamples[6][4] + pressureSamples[6][5]) / 6);
		float change = (pressureAvg[6] - pressureAvg[0]);
		if (firstRound) // first time initial 3 hour
			dP_dt = (((65.0 / 1023.0) * change) / 3); // note this is for t = 3 hour
		else
			dP_dt = (((65.0 / 1023.0) * change) / 4); // divide by 4 as this is the difference in time from 0 value
		pressureAvg[0] = pressureAvg[5]; // Equating the pressure at 0 to the pressure at 2 hour after 3 hours have past.
		firstRound = false; // flag to let you know that this is on the past 3 hour mark. Initialized to 0 outside main loop.
	}

	if (minuteCount < 35 && firstRound) //if time is less than 35 min on the first 3 hour interval.
		return 5; // Unknown, more time needed
	else if (dP_dt < (-0.25))
		return 4; // Quickly falling LP, Thunderstorm, not stable
	else if (dP_dt > 0.25)
		return 3; // Quickly rising HP, not stable weather
	else if ((dP_dt >(-0.25)) && (dP_dt < (-0.05)))
		return 2; // Slowly falling Low Pressure System, stable rainy weather
	else if ((dP_dt > 0.05) && (dP_dt < 0.25))
		return 1; // Slowly rising HP stable good weather
	else if ((dP_dt > (-0.05)) && (dP_dt < 0.05))
		return 0; // Stable weather
	else
		return 5; // Unknown
}
