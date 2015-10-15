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
#include <math.h>
//#include <eeprom.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_INNER_SENSOR_ID	0
MyMessage msgTemperatureInner(TEMPERATURE_INNER_SENSOR_ID, V_TEMP);
float lastTemperatureInner;
unsigned long prevMsTemperatureInner = 0;
const long intervalTemperatureInner = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_INNER_SENSOR_ID	1
MyMessage msgHumidityInner(HUMIDITY_INNER_SENSOR_ID, V_HUM);
float lastHumidityInner;
unsigned long prevMsHumidityInner = 0;
const unsigned long intervalHumidityInner = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_OUTER_SENSOR_ID	2
MyMessage msgTemperatureOuter(TEMPERATURE_OUTER_SENSOR_ID, V_TEMP);
float lastTemperatureOuter;
unsigned long prevMsTemperatureOuter = 0;
const unsigned long intervalTemperatureOuter = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_OUTER_SENSOR_ID	3
MyMessage msgHumidityOuter(HUMIDITY_OUTER_SENSOR_ID, V_HUM);
float lastHumidityOuter;
unsigned long prevMsHumidityOuter = 0;
const unsigned long intervalHumidityOuter = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define PRESSURE_SENSOR_ID			4
const float ALTITUDE = 200; // sea level, meters
MyMessage msgPressure(PRESSURE_SENSOR_ID, V_PRESSURE);
int32_t lastPressure;
unsigned long prevMsPressure = 0;
const unsigned long intervalPressure = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define FORECAST_SENSOR_ID			5
MyMessage msgForecast(FORECAST_SENSOR_ID, V_FORECAST);
int lastForecast = 5;
const char *weather[] = { "stable", "sunny", "cloudy", "unstable", "thunderstorm", "unknown" };
enum FORECAST
{
	STABLE = 0,			// "Stable Weather Pattern"
	SUNNY = 1,			// "Slowly rising Good Weather", "Clear/Sunny "
	CLOUDY = 2,			// "Slowly falling L-Pressure ", "Cloudy/Rain "
	UNSTABLE = 3,		// "Quickly rising H-Press",     "Not Stable"
	THUNDERSTORM = 4,	// "Quickly falling L-Press",    "Thunderstorm"
	UNKNOWN = 5			// "Unknown (More Time needed)
};
const int LAST_SAMPLES_COUNT = 5;
float lastPressureSamples[LAST_SAMPLES_COUNT];
float dP_dt;
int minuteCount = 0;
bool firstRound = true;
// this CONVERSION_FACTOR is used to convert from Pa to kPa in forecast algorithm
// get kPa/h by dividing hPa by 10 
#define CONVERSION_FACTOR (1.0 / 10.0)
float pressureAvg; // average value is used in forecast algorithm
float pressureAvg2; // average after 2 hours is used as reference value for the next iteration

//--------------------------------------------------------------------------------------------------------------------------------------------
// MQ2 sensor: LPG, i-butane, CO, Hydrogen (H2), methane (CH4), alcohol, smoke, propane

#define GAS_SENSOR_ID				6
MyMessage msgGas(GAS_SENSOR_ID, V_VAR1);
uint16_t lastGas = 0;
unsigned long prevMsGas = 0;
const unsigned long intervalGas = 60000;

#define GAS_SENSOR_ANALOG_PIN		A0		// define which analog input channel you are going to use
#define RO_CLEAN_AIR_FACTOR         9.83	// RO_CLEAR_AIR_FACTOR=(Sensor resistance in clean air)/RO, which is derived from the chart in datasheet

#define CALIBARAION_SAMPLE_COUNT    50		// define how many samples you are going to take in the calibration phase
#define CALIBRATION_SAMPLE_INTERVAL 500		// define the time interal (in milisecond) between each samples in the cablibration phase
#define READ_SAMPLE_COUNT           5		// define how many samples you are going to take in normal operation
#define READ_SAMPLE_INTERVAL        50		// define the time interal (in milisecond) between each samples in normal operation

#define GAS_LPG                     0
#define GAS_CO                      1
#define GAS_SMOKE                   2

float Ro;

// two points are taken from the curve; with these two points, a line is formed which is "approximately equivalent" to the original curve.
// data format: {x, y, slope}; 
float LPGCurve[3] = { 2.3, 0.21, -0.47 };	// point1: (lg200, 0.21), point2: (lg10000, -0.59)
float COCurve[3] = { 2.3, 0.72, -0.34 };	// point1: (lg200, 0.72), point2: (lg10000,  0.15)
float SmokeCurve[3] = { 2.3, 0.53, -0.44 };	// point1: (lg200, 0.53), point2: (lg10000, -0.22)
//--------------------------------------------------------------------------------------------------------------------------------------------
#define RAIN_SENSOR_ID				7
#define RAIN_SENSOR_ANALOG_PIN		A1
MyMessage msgRain(RAIN_SENSOR_ID, V_RAINRATE); //V_RAIN
uint8_t lastRain = 0;
unsigned long prevMsRain = 0;
const unsigned long intervalRain = 60000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define LIGHT_SENSOR_ID				8
#define LIGHT_SENSOR_ANALOG_PIN		A2
MyMessage msgLight(LIGHT_SENSOR_ID, V_LIGHT_LEVEL);
uint16_t lastLight = 0;
unsigned long prevMsLight = 0;
const long intervalLight = 60000;

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
	//for (int i = 0; i < 512; i++)
	//	EEPROM.write(i, 255);
	//return;

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Meteo Station", "1.0");

	isMetric = gw.getConfig().isMetric;

	dhtOuter.setup(DHT_OUTER_PIN);
	dhtInner.setup(DHT_INNER_PIN);

	if (!bmp.begin())
	{
#ifdef DEBUG
		Serial.println("Could not find a valid BMP085/BMP180 sensor, check wiring!");
#endif
		while (1) {}
	}

	Ro = getGasSensorRo(); // calibrating the sensor; please make sure the sensor is in clean air when you perform the calibration

	pinMode(RAIN_SENSOR_ANALOG_PIN, INPUT);
	pinMode(LIGHT_SENSOR_ANALOG_PIN, INPUT);

	gw.present(TEMPERATURE_INNER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_INNER_SENSOR_ID, S_HUM);
	gw.present(TEMPERATURE_OUTER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_OUTER_SENSOR_ID, S_HUM);
	gw.present(PRESSURE_SENSOR_ID, S_BARO);
	gw.present(FORECAST_SENSOR_ID, S_BARO);
	gw.present(GAS_SENSOR_ID, S_AIR_QUALITY);
	gw.present(RAIN_SENSOR_ID, S_RAIN);
	gw.present(LIGHT_SENSOR_ID, S_LIGHT_LEVEL);
}
void loop()
{
	processTemperature(false);
	gw.process();

	processHumidity(false);
	gw.process();

	processTemperature(true);
	gw.process();

	processHumidity(true);
	gw.process();

	processPressure();
	gw.process();

	processGas();
	gw.process();

	processRain();
	gw.process();

	processLight();
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
		else if (message.sensor == FORECAST_SENSOR_ID && message.type == V_FORECAST)
			gw.send(msgForecast.set(lastForecast));
		else if (message.sensor == GAS_SENSOR_ID && message.type == V_VAR1)
			gw.send(msgGas.set(lastGas));
		else if (message.sensor == RAIN_SENSOR_ID && message.type == V_RAIN)
			gw.send(msgRain.set(lastRain));
		else if (message.sensor == LIGHT_SENSOR_ID && message.type == V_LIGHT_LEVEL)
			gw.send(msgLight.set(lastLight));
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

	if (hasIntervalElapsed(prevMsTemperature, intervalTemperature))
	{
		delay(pDht->getMinimumSamplingPeriod());

		float temperature = roundFloat(pDht->getTemperature(), 1);

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

	if (hasIntervalElapsed(prevMsHumidity, intervalHumidity))
	{
		delay(pDht->getMinimumSamplingPeriod());

		float humidity = roundFloat(pDht->getHumidity(), 1);

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
	if (hasIntervalElapsed(&prevMsPressure, intervalPressure))
	{
		//int32_t pressure = bmp.readSealevelPressure(190) / 1000; // 205 meters above sealevel
		//int forecast = samplePressure(pressure);

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
		//int32_t pressure = bmp.readSealevelPressure(ALTITUDE);

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

			//int forecast = samplePressure((float)pressure / 1000.0f); // in kPa
			int forecast = samplePressure((float)pressure / 100.0f); // in hPa

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
void processGas()
{
	if (hasIntervalElapsed(&prevMsGas, intervalGas))
	{
		float Rs_Ro = getGasSensorRatio();
		//Serial.print("Rs_Ro: ");
		//Serial.println(Rs_Ro);

		uint16_t val = getGasPercentage(Rs_Ro, GAS_CO);

		//Serial.print("LPG: ");
		//Serial.print(getGasPercentage(Rs_Ro, GAS_LPG));
		//Serial.println(" ppm");

		//Serial.print("CO: ");
		//Serial.print(getGasPercentage(Rs_Ro, GAS_CO));
		//Serial.println(" ppm");

		//Serial.print("SMOKE: ");
		//Serial.print(getGasPercentage(Rs_Ro, GAS_SMOKE));
		//Serial.println(" ppm");

		//Serial.println();

		if (val != lastGas)
		{
			lastGas = val;
			gw.send(msgGas.set((int)val));
		}
	}
}
void processRain()
{
	if (hasIntervalElapsed(&prevMsRain, intervalRain))
	{
		int val = analogRead(RAIN_SENSOR_ANALOG_PIN);

		//Serial.print("Rain: ");
		//Serial.println(val);

		//1010 - dry
		//250 - wet


		////greater than 1000, probably not touching anything
		//if (val >= 1000)
		//{
		//	Serial.println("I think your prong's come loose.");
		//}
		//if (val < 1000 && val >= 650)
		//	//less than 1000, greater than 650, dry soil
		//{
		//	Serial.println("Soil's rather dry, really.");
		//}
		//if (val < 650 && val >= 400)
		//	//less than 650, greater than 400, somewhat moist
		//{
		//	Serial.println("Soil's a bit damp.");
		//}
		//if (val < 400)
		//	//less than 400, quite moist
		//	Serial.println("Soil is quite most, thank you very much.");

		uint8_t range = map(val, 1023, 0, 0, 3);

		//Serial.print("Rain: ");
		//Serial.println(range);

		if (range != lastRain)
		{
			lastRain = range;
			gw.send(msgRain.set(range));
		}
	}
}
void processLight()
{
	if (hasIntervalElapsed(&prevMsLight, intervalLight))
	{
		int val = analogRead(LIGHT_SENSOR_ANALOG_PIN);

		//Serial.print("Light: ");
		//Serial.println(val);

		if (val != lastLight)
		{
			lastLight = val;
			//gw.send(msgLight.set(val));
		}
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
float getLastPressureSamplesAverage()
{
	float average = 0;

	for (int i = 0; i < LAST_SAMPLES_COUNT; i++)
		average += lastPressureSamples[i];

	average /= LAST_SAMPLES_COUNT;

	return average;
}
int samplePressure(float pressure /* in hPa */)
{
	// Algorithm found here
	// http://www.freescale.com/files/sensors/doc/app_note/AN3914.pdf
	// Pressure in hPa -->  forecast done by calculating kPa/h

	// Calculate the average of the last n minutes.
	int index = minuteCount % LAST_SAMPLES_COUNT;
	lastPressureSamples[index] = pressure;

	minuteCount++;
	if (minuteCount > 185)
		minuteCount = 6;

	if (minuteCount == 5)
		pressureAvg = getLastPressureSamplesAverage();
	else if (minuteCount == 35)
	{
		float lastPressureAvg = getLastPressureSamplesAverage();
		float change = (lastPressureAvg - pressureAvg) * CONVERSION_FACTOR;
		if (firstRound) // first time initial 3 hour
			dP_dt = change * 2; // note this is for t = 0.5hour
		else
			dP_dt = change / 1.5; // divide by 1.5 as this is the difference in time from 0 value.
	}
	else if (minuteCount == 65)
	{
		float lastPressureAvg = getLastPressureSamplesAverage();
		float change = (lastPressureAvg - pressureAvg) * CONVERSION_FACTOR;
		if (firstRound) //first time initial 3 hour
			dP_dt = change; //note this is for t = 1 hour
		else
			dP_dt = change / 2; //divide by 2 as this is the difference in time from 0 value
	}
	else if (minuteCount == 95)
	{
		float lastPressureAvg = getLastPressureSamplesAverage();
		float change = (lastPressureAvg - pressureAvg) * CONVERSION_FACTOR;
		if (firstRound) // first time initial 3 hour
			dP_dt = change / 1.5; // note this is for t = 1.5 hour
		else
			dP_dt = change / 2.5; // divide by 2.5 as this is the difference in time from 0 value
	}
	else if (minuteCount == 125)
	{
		float lastPressureAvg = getLastPressureSamplesAverage();
		pressureAvg2 = lastPressureAvg; // store for later use.
		float change = (lastPressureAvg - pressureAvg) * CONVERSION_FACTOR;
		if (firstRound) // first time initial 3 hour
			dP_dt = change / 2; // note this is for t = 2 hour
		else
			dP_dt = change / 3; // divide by 3 as this is the difference in time from 0 value
	}
	else if (minuteCount == 155)
	{
		float lastPressureAvg = getLastPressureSamplesAverage();
		float change = (lastPressureAvg - pressureAvg) * CONVERSION_FACTOR;
		if (firstRound) // first time initial 3 hour
			dP_dt = change / 2.5; // note this is for t = 2.5 hour
		else
			dP_dt = change / 3.5; // divide by 3.5 as this is the difference in time from 0 value
	}
	else if (minuteCount == 185)
	{
		float lastPressureAvg = getLastPressureSamplesAverage();
		float change = (lastPressureAvg - pressureAvg) * CONVERSION_FACTOR;
		if (firstRound) // first time initial 3 hour
			dP_dt = change / 3; // note this is for t = 3 hour
		else
			dP_dt = change / 4; // divide by 4 as this is the difference in time from 0 value

		pressureAvg = pressureAvg2; // Equating the pressure at 0 to the pressure at 2 hour after 3 hours have past.
		firstRound = false; // flag to let you know that this is on the past 3 hour mark. Initialized to 0 outside main loop.
	}

	int forecast = UNKNOWN;

	if (minuteCount < 35 && firstRound) //if time is less than 35 min on the first 3 hour interval.
		forecast = UNKNOWN;
	else if (dP_dt < -0.25)
		forecast = THUNDERSTORM;
	else if (dP_dt > 0.25)
		forecast = UNSTABLE;
	else if (dP_dt > -0.25 && dP_dt < -0.05)
		forecast = CLOUDY;
	else if (dP_dt > 0.05 && dP_dt < 0.25)
		forecast = SUNNY;
	else if (dP_dt > -0.05 && dP_dt < 0.05)
		forecast = STABLE;
	else
		forecast = UNKNOWN;

	// uncomment when debugging
	//Serial.print(F("Forecast at minute "));
	//Serial.print(minuteCount);
	//Serial.print(F(" dP/dt = "));
	//Serial.print(dP_dt);
	//Serial.print(F("kPa/h --> "));
	//Serial.println(weather[forecast]);

	return forecast;
}
//--------------------------------------------------------------------------------------------------------------------------------------------
float getGasSensorRo()
{
	float adc = 0;
	for (int i = 0; i < CALIBARAION_SAMPLE_COUNT; i++)
	{
		adc += analogRead(GAS_SENSOR_ANALOG_PIN);
		delay(CALIBRATION_SAMPLE_INTERVAL);
	}
	adc /= CALIBARAION_SAMPLE_COUNT;

	float Rs = (1024 - adc) / adc; // omit *Rl
	float Ro = Rs / RO_CLEAN_AIR_FACTOR; // The ratio of Rs/Ro is ~10 in a clear air

	return Ro;
}
float getGasSensorRatio()
{
	float adc = 0;
	for (int i = 0; i < READ_SAMPLE_COUNT; i++)
	{
		adc += analogRead(GAS_SENSOR_ANALOG_PIN);
		delay(READ_SAMPLE_INTERVAL);
	}
	adc /= READ_SAMPLE_COUNT;

	float Rs = (1024 - adc) / adc; // omit *Rl
	float RsRoRatio = Rs / Ro;

	return RsRoRatio;
}
/*****************************  getGasPercentage **********************************
Output:  ppm of the target gas
Remarks: This function calculates the ppm (parts per million) of the target gas.
		By using the slope and a point of the line, the X (logarithmic value of ppm)
		of the line could be derived if Y (rs_ro_ratio) is provided. As it is a
		logarithmic coordinate, power of 10 is used to convert the result to non-logarithmic value.
************************************************************************************/
float getGasPercentage(float rs_ro_ratio, int gasType)
{
	float *pCurve = NULL;
	switch (gasType)
	{
		case GAS_LPG: pCurve = LPGCurve; break;
		case GAS_CO: pCurve = COCurve; break;
		case GAS_SMOKE: pCurve = SmokeCurve; break;



		default: return 0;
	}

	//100ppm ... 10000ppm, i.e. 0.01% ... 1%

	float x0 = pCurve[0];
	float y0 = pCurve[1];
	float k = pCurve[2];

	float y = log(rs_ro_ratio);
	float x = (y - y0) / k + x0;

	return pow(10, x);
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